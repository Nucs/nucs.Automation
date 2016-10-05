using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Win32Interop.Methods;
using Win32Interop.Structs;

namespace nucs.Automation.Controllers {
    public class MouseController {
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

        protected int commondelay => BaseDelay+rand(0, 20);

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

        protected void SendInput(INPUT input) {
            User32.SendInput(1U, new[] { input }, Marshal.SizeOf(typeof(INPUT)));
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

        public void AbsoluteMove(int x, int y) {
            var inputBuffer = new INPUT {type = 0U};
            inputBuffer.inputData.mi.dwFlags = 32769U;
            inputBuffer.inputData.mi.dx = (int)(ushort.MaxValue * (double)x / desktopWidth);
            inputBuffer.inputData.mi.dy = (int)(ushort.MaxValue * (double)y / desktopHeight);
            SendInput(inputBuffer);
        }

        public void AbsoluteMove(Point aDestination) {
            AbsoluteMove(aDestination.X, aDestination.Y);
        }

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

        public async Task Move(Point destination, int aMovementVelocityLogFactor = 1) {
            await Move(destination.X, destination.Y, aMovementVelocityLogFactor);
        }

        public async Task MoveRelative(int xDisplacement, int yDisplacement, int aMovementVelocityLogFactor = 2) {
            var position = Cursor.Position;
            var dx = position.X + xDisplacement;
            position = Cursor.Position;
            var dy = position.Y + yDisplacement;
            double aMovementVelocityLogFactor1 = aMovementVelocityLogFactor;
            await Move(dx, dy, aMovementVelocityLogFactor1);
        }

        public Task MoveRelative(Point aDisplacement, int aMovementVelocityLogFactor = 2) {
            return MoveRelative(aDisplacement.X, aDisplacement.Y, aMovementVelocityLogFactor);
        }
        #endregion

        #region Combo

        public async Task MoveClick(int x, int y) {
            await Move(x, y, 1.0);
            await Task.Delay(commondelay);
            await Click();
        }

        public Task MoveClick(Point aPoint) {
            return MoveClick(aPoint.X, aPoint.Y);
        }

        public async Task MoveClickHold(int x, int y, TimeSpan aWaitPeriod) {
            await Move(x, y, 1.0d);
            LeftDown();
            await Task.Delay(aWaitPeriod + TimeSpan.FromMilliseconds(commondelay));
            LeftUp();
        }

        public Task MoveClickHold(Point aPoint, TimeSpan aWaitPeriod) {
            return MoveClickHold(aPoint.X, aPoint.Y, aWaitPeriod);
        }

        public async Task MoveClickDelay(int x, int y, TimeSpan aWaitPeriod) {
            await MoveClick(x, y);
            await Task.Delay(aWaitPeriod);
        }

        public Task MoveClickDelay(Point aPoint, TimeSpan aWaitPeriod) {
            return MoveClickDelay(aPoint.X, aPoint.Y, aWaitPeriod);
        }

        public async Task MoveDelayClick(int x, int y, TimeSpan aWaitPeriod) {
            await Move(x, y, 1.0d);
            await Task.Delay(aWaitPeriod);
            await Click();
        }

        public async Task MoveDelayClick(Point aPoint, TimeSpan aWaitPeriod) {
            await MoveDelayClick(aPoint.X, aPoint.Y, aWaitPeriod);
        }

        public async Task Click(MouseButton btn = MouseButton.Left) {
            SendInput(btn, MouseDirection.Down);
            await Task.Delay(commondelay);
            SendInput(btn, MouseDirection.Up);
            await Task.Delay(commondelay);
        }

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

        public async Task DragDrop(int ox, int oy, int dx, int dy) {
            await Move(ox, oy, 1.0);
            LeftDown();
            await Move(dx, dy, 1.0);
            LeftUp();
        }

        public Task DragDrop(Point aFirstPoint, Point aSecondPoint) {
            return DragDrop(aFirstPoint.X, aFirstPoint.Y, aSecondPoint.X, aSecondPoint.Y);
        }

        #endregion
    }
}