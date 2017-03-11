using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace nucs.Automation.Controllers {
    public class MouseController : IMouseController {
        private readonly int desktopHeight = SystemInformation.PrimaryMonitorSize.Height;
        private readonly int desktopWidth = SystemInformation.PrimaryMonitorSize.Width;

        #region Clicking

        /// <summary>
        ///     The speed at which the wheel will spin
        /// </summary>
        public int WheelSpeed = 120;

        /// <summary>
        ///     Milliseconds delay between mouse down and up.
        /// </summary>
        public int BaseDelay = 35;

        private static readonly Random _rand = new Random();
        private int rand(int from, int to) {
            return _rand.Next(from, to);
        }

        public int CommonDelay => BaseDelay+rand(0, 20);

        /// <summary>
        /// Sends input based on the given enums
        /// </summary>
        protected void SendInput(MouseButton btn, MouseDirection dir) {
            var inputBuffer = new INPUT {type = 0U};
            switch (btn) {
                case MouseButton.Left:
                    inputBuffer.inputData.mi.dwFlags = dir == MouseDirection.Down ? 2U : 4U;
                    break;
                case MouseButton.Right:
                    inputBuffer.inputData.mi.dwFlags = dir == MouseDirection.Down ? 8U : 16U;
                    break;
                case MouseButton.Middle:
                    inputBuffer.inputData.mi.dwFlags = dir == MouseDirection.Down ? 32U : 64U;
                    break;
                case MouseButton.Scroll:
                    inputBuffer.inputData.mi.dwFlags = 2048U;
                    inputBuffer.inputData.mi.mouseData = (uint) (dir == MouseDirection.Up ? WheelSpeed : -WheelSpeed);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(btn), btn, null);
            }

            SendInput(inputBuffer);
        }

        private void SendInput(INPUT input) {
            Native.SendInput(1U, new[] { input }, Marshal.SizeOf(typeof(INPUT)));
        }

        public void LeftDown() {
            SendInput(MouseButton.Left, MouseDirection.Down);
        }

        public void LeftUp() {
            SendInput(MouseButton.Left, MouseDirection.Up);
        }

        public void RightDown() {
            SendInput(MouseButton.Right, MouseDirection.Down);
        }

        public void RightUp() {
            SendInput(MouseButton.Right, MouseDirection.Up);
        }

        public void MiddleDown() {
            SendInput(MouseButton.Middle, MouseDirection.Down);
        }

        public void MiddleUp() {
            SendInput(MouseButton.Middle, MouseDirection.Up);
        }

        public void WheelDown() {
            SendInput(MouseButton.Scroll, MouseDirection.Down);
        }

        public void WheelUp() {
            SendInput(MouseButton.Scroll, MouseDirection.Up);
        }

        #endregion

        #region Move

        /// <summary>
        ///     Moves the cursor instantly to a given x,y.
        /// </summary>
        public void AbsoluteMove(int x, int y) {
            var inputBuffer = new INPUT {type = 0U};
            inputBuffer.inputData.mi.dwFlags = 32769U;
            inputBuffer.inputData.mi.dx = (int)(ushort.MaxValue * (double)x / desktopWidth);
            inputBuffer.inputData.mi.dy = (int)(ushort.MaxValue * (double)y / desktopHeight);
            SendInput(inputBuffer);
        }

        /// <summary>
        ///     Moves the cursor instantly to a given x,y.
        /// </summary>
        public void AbsoluteMove(Point aDestination) {
            AbsoluteMove(aDestination.X, aDestination.Y);
        }

        /// <summary>
        ///     Moves cursor in a given velocity, higher is faster (log-n).
        /// </summary>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="aMovementVelocityLogFactor"> higher is faster (log-n).</param>
        public async Task Move(int dx, int dy, double aMovementVelocityLogFactor = 1.0) {
            var x = Cursor.Position.X;
            var y = Cursor.Position.Y;
            var num1 = (int) Math.Log(Math.Sqrt((dx - x) * (dx - x) + (dy - y) * (dy - y)), 1001.0 / 1000.0 + 1.0 * aMovementVelocityLogFactor) + 5;
            double num2 = x;
            double num3 = y;
            for (var index = 1; index < num1; ++index) {
                var num4 = Math.Sin(index / (double) num1 * Math.PI) * 1.57;
                num2 += num4 * (dx - x) / num1;
                num3 += num4 * (dy - y) / num1;
                if (index == num1) {
                    num2 = dx;
                    num3 = dy;
                }
                AbsoluteMove((int) num2, (int) num3);
                await Task.Delay(5);
            }
            AbsoluteMove(dx, dy);
            await Task.Delay(5);
        }

        /// <summary>
        ///     Moves cursor in a given velocity, higher is faster (log-n).
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="aMovementVelocityLogFactor"> higher is faster (log-n).</param>
        public Task Move(Point destination, double aMovementVelocityLogFactor = 1) {
            return Move(destination.X, destination.Y, aMovementVelocityLogFactor);
        }

        /// <summary>
        ///     Moves cursor in a given velocity, relativly to current position, higher is faster (log-n).
        /// </summary>
        /// <param name="yDisplacement"></param>
        /// <param name="xDisplacement"></param>
        /// <param name="aMovementVelocityLogFactor"> higher is faster (log-n).</param>
        public async Task MoveRelative(int xDisplacement, int yDisplacement, double aMovementVelocityLogFactor = 2) {
            var position = Cursor.Position;
            var dx = position.X + xDisplacement;
            var dy = position.Y + yDisplacement;
            double aMovementVelocityLogFactor1 = aMovementVelocityLogFactor;
            await Move(dx, dy, aMovementVelocityLogFactor1);
        }

        /// <summary>
        ///     Moves cursor in a given velocity, relativly to current position, higher is faster (log-n).
        /// </summary>
        /// <param name="aDisplacement"></param>
        /// <param name="aMovementVelocityLogFactor"> higher is faster (log-n).</param>
        public Task MoveRelative(Point aDisplacement, double aMovementVelocityLogFactor = 2) {
            return MoveRelative(aDisplacement.X, aDisplacement.Y, aMovementVelocityLogFactor);
        }
        #endregion

        #region Combo

        /// <summary>
        ///     Moves and clicks.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="btn">The specific button to click</param>
        /// <returns></returns>
        public async Task MoveClick(int x, int y, MouseButton btn = MouseButton.Left) {
            await Move(x, y, 1.0);
            await Task.Delay(CommonDelay);
            await Click(btn);
        }

        public Task MoveClick(Point aPoint, MouseButton btn = MouseButton.Left) {
            return MoveClick(aPoint.X, aPoint.Y, btn);
        }

        /// <summary>
        ///     Moves, presses key down, waits for given time and releases the key up.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="aWaitPeriod">The time to wait between the press down and up</param>
        /// <param name="btn">Which button to click</param>
        /// <returns></returns>
        public async Task MoveClickHold(int x, int y, TimeSpan aWaitPeriod, MouseButton btn = MouseButton.Left) {
            await Move(x, y, 1.0d);
            SendInput(btn, MouseDirection.Down);
            await Task.Delay(aWaitPeriod + TimeSpan.FromMilliseconds(CommonDelay));
            SendInput(btn, MouseDirection.Up);
        }

        /// <summary>
        ///     Moves, presses key down, waits for given time and releases the key up.
        /// </summary>
        /// <param name="aPoint"></param>
        /// <param name="aWaitPeriod">The time to wait between the press down and up</param>
        /// <param name="btn">Which button to click</param>
        /// <returns></returns>
        public Task MoveClickHold(Point aPoint, TimeSpan aWaitPeriod, MouseButton btn = MouseButton.Left) {
            return MoveClickHold(aPoint.X, aPoint.Y, aWaitPeriod, btn);
        }

        /// <summary>
        ///     Clicks, by default - left button.
        /// </summary>
        public async Task Click(MouseButton btn = MouseButton.Left) {
            SendInput(btn, MouseDirection.Down);
            await Task.Delay(CommonDelay);
            SendInput(btn, MouseDirection.Up);
            await Task.Delay(CommonDelay);
        }

        /// <summary>
        ///     Double clicks, by default - left button
        /// </summary>
        public async Task DoubleClick(MouseButton btn = MouseButton.Left) {
            await Click(btn);
            await Task.Delay(15); //extra delay
            await Click(btn);
        }

        public async Task MiddleClick() {
            await Click(MouseButton.Middle);
        }

        public async Task RightClick() {
            await Click(MouseButton.Right);
        }

        /// <summary>
        ///     moves to (o), presses left, moves to (d), releases left
        /// </summary>
        /// <param name="ox">From x</param>
        /// <param name="oy">From y</param>
        /// <param name="dx">To x</param>
        /// <param name="dy">To y</param>
        public async Task DragDrop(int ox, int oy, int dx, int dy) {
            await Move(ox, oy, 1.0);
            LeftDown();
            await Move(dx, dy, 1.0);
            LeftUp();
        }
        /// <summary>
        ///     moves to (o), presses left, moves to (d), releases left
        /// </summary>
        /// <param name="o">From</param>
        /// <param name="d">To</param>
        public Task DragDrop(Point o, Point d) {
            return DragDrop(o.X, o.Y, d.X, d.Y);
        }

        #endregion
    }

}