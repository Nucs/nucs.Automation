using System;
using System.Drawing;
using System.Threading.Tasks;
using nucs.Automation.Controllers;

namespace nucs.Automation {
    /// <summary>
    ///     Performs various actions with a mouse by instance of <see cref="MouseController"/>
    /// </summary>
    public static class Mouse {
        private static readonly IMouseController controller = new MouseController();

        public static void LeftDown() {
            controller.LeftDown();
        }

        public static void LeftUp() {
            controller.LeftUp();
        }

        public static void RightDown() {
            controller.RightDown();
        }

        public static void RightUp() {
            controller.RightUp();
        }

        public static void MiddleDown() {
            controller.MiddleDown();
        }

        public static void MiddleUp() {
            controller.MiddleUp();
        }

        public static void WheelDown() {
            controller.WheelDown();
        }

        public static void WheelUp() {
            controller.WheelUp();
        }

        /// <summary>
        ///     Moves the cursor instantly to a given x,y.
        /// </summary>
        public static void AbsoluteMove(int x, int y) {
            controller.AbsoluteMove(x, y);
        }

        /// <summary>
        ///     Moves the cursor instantly to a given x,y.
        /// </summary>
        public static void AbsoluteMove(Point aDestination) {
            controller.AbsoluteMove(aDestination);
        }

        /// <summary>
        ///     Moves cursor in a given velocity, higher is faster (log-n).
        /// </summary>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="aMovementVelocityLogFactor"> higher is faster (log-n).</param>
        public static Task Move(int dx, int dy, double aMovementVelocityLogFactor = 1) {
            return controller.Move(dx, dy, aMovementVelocityLogFactor);
        }

        /// <summary>
        ///     Moves cursor in a given velocity, higher is faster (log-n).
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="aMovementVelocityLogFactor"> higher is faster (log-n).</param>
        public static Task Move(Point destination, int aMovementVelocityLogFactor = 1) {
            return controller.Move(destination, aMovementVelocityLogFactor);
        }

        /// <summary>
        ///     Moves cursor in a given velocity, relativly to current position, higher is faster (log-n).
        /// </summary>
        /// <param name="yDisplacement"></param>
        /// <param name="xDisplacement"></param>
        /// <param name="aMovementVelocityLogFactor"> higher is faster (log-n).</param>
        public static Task MoveRelative(int xDisplacement, int yDisplacement, int aMovementVelocityLogFactor = 2) {
            return controller.MoveRelative(xDisplacement, yDisplacement, aMovementVelocityLogFactor);
        }

        /// <summary>
        ///     Moves cursor in a given velocity, relativly to current position, higher is faster (log-n).
        /// </summary>
        /// <param name="aDisplacement"></param>
        /// <param name="aMovementVelocityLogFactor"> higher is faster (log-n).</param>
        public static Task MoveRelative(Point aDisplacement, int aMovementVelocityLogFactor = 2) {
            return controller.MoveRelative(aDisplacement, aMovementVelocityLogFactor);
        }

        /// <summary>
        ///     Moves and clicks.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="btn">The specific button to click</param>
        /// <returns></returns>
        public static Task MoveClick(int x, int y, MouseButton btn = MouseButton.Left) {
            return controller.MoveClick(x, y, btn);
        }

        public static Task MoveClick(Point aPoint) {
            return controller.MoveClick(aPoint);
        }

        /// <summary>
        ///     Moves, presses key down, waits for given time and releases the key up.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="aWaitPeriod">The time to wait between the press down and up</param>
        /// <param name="btn">Which button to click</param>
        /// <returns></returns>
        public static Task MoveClickHold(int x, int y, TimeSpan aWaitPeriod, MouseButton btn = MouseButton.Left) {
            return controller.MoveClickHold(x, y, aWaitPeriod, btn);
        }

        /// <summary>
        ///     Moves, presses key down, waits for given time and releases the key up.
        /// </summary>
        /// <param name="aPoint"></param>
        /// <param name="aWaitPeriod">The time to wait between the press down and up</param>
        /// <param name="btn">Which button to click</param>
        /// <returns></returns>
        public static Task MoveClickHold(Point aPoint, TimeSpan aWaitPeriod, MouseButton btn = MouseButton.Left) {
            return controller.MoveClickHold(aPoint, aWaitPeriod, btn);
        }

        /// <summary>
        ///     Clicks, by default - left button.
        /// </summary>
        public static Task Click(MouseButton btn = MouseButton.Left) {
            return controller.Click(btn);
        }

        /// <summary>
        ///     Double clicks, by default - left button
        /// </summary>
        public static Task DoubleClick(MouseButton btn = MouseButton.Left) {
            return controller.DoubleClick(btn);
        }

        public static Task MiddleClick() {
            return controller.MiddleClick();
        }

        public static Task RightClick() {
            return controller.RightClick();
        }

        /// <summary>
        ///     moves to (o), presses left, moves to (d), releases left
        /// </summary>
        /// <param name="ox">From x</param>
        /// <param name="oy">From y</param>
        /// <param name="dx">To x</param>
        /// <param name="dy">To y</param>
        public static Task DragDrop(int ox, int oy, int dx, int dy) {
            return controller.DragDrop(ox, oy, dx, dy);
        }

        /// <summary>
        ///     moves to (o), presses left, moves to (d), releases left
        /// </summary>
        /// <param name="o">From</param>
        /// <param name="d">To</param>
        public static Task DragDrop(Point o, Point d) {
            return controller.DragDrop(o, d);
        }
    }
}