using System;
using System.Drawing;
using System.Threading.Tasks;
using nucs.Automation.Controllers;

namespace nucs.Automation {
        /// <summary>
        ///     Performs various actions with a mouse
        /// </summary>

        public static class Mouse {
            private static readonly MouseController controller = new MouseController();
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

            public static void AbsoluteMove(int x, int y) {
                controller.AbsoluteMove(x, y);
            }

            public static void AbsoluteMove(Point aDestination) {
                controller.AbsoluteMove(aDestination);
            }

            public static Task Move(int dx, int dy, double aMovementVelocityLogFactor = 1) {
                return controller.Move(dx, dy, aMovementVelocityLogFactor);
            }

            public static Task Move(Point destination, int aMovementVelocityLogFactor = 1) {
                return controller.Move(destination, aMovementVelocityLogFactor);
            }

            public static Task MoveRelative(int xDisplacement, int yDisplacement, int aMovementVelocityLogFactor = 2) {
                return controller.MoveRelative(xDisplacement, yDisplacement, aMovementVelocityLogFactor);
            }

            public static Task MoveRelative(Point aDisplacement, int aMovementVelocityLogFactor = 2) {
                return controller.MoveRelative(aDisplacement, aMovementVelocityLogFactor);
            }

            public static Task MoveClick(int x, int y) {
                return controller.MoveClick(x, y);
            }

            public static Task MoveClick(Point aPoint) {
                return controller.MoveClick(aPoint);
            }

            public static Task MoveClickHold(int x, int y, TimeSpan aWaitPeriod) {
                return controller.MoveClickHold(x, y, aWaitPeriod);
            }

            public static Task MoveClickHold(Point aPoint, TimeSpan aWaitPeriod) {
                return controller.MoveClickHold(aPoint, aWaitPeriod);
            }

            public static Task MoveClickDelay(int x, int y, TimeSpan aWaitPeriod) {
                return controller.MoveClickDelay(x, y, aWaitPeriod);
            }

            public static Task MoveClickDelay(Point aPoint, TimeSpan aWaitPeriod) {
                return controller.MoveClickDelay(aPoint, aWaitPeriod);
            }

            public static Task MoveDelayClick(int x, int y, TimeSpan aWaitPeriod) {
                return controller.MoveDelayClick(x, y, aWaitPeriod);
            }

            public static Task MoveDelayClick(Point aPoint, TimeSpan aWaitPeriod) {
                return controller.MoveDelayClick(aPoint, aWaitPeriod);
            }

            public static Task Click(MouseButton btn = MouseButton.Left) {
                return controller.Click(btn);
            }

            public static Task DoubleClick(MouseButton btn = MouseButton.Left) {
                return controller.DoubleClick(btn);
            }

            public static Task MiddleClick() {
                return controller.MiddleClick();
            }

            public static Task RightClick() {
                return controller.RightClick();
            }

            public static Task DragDrop(int ox, int oy, int dx, int dy) {
                return controller.DragDrop(ox, oy, dx, dy);
            }

            public static Task DragDrop(Point aFirstPoint, Point aSecondPoint) {
                return controller.DragDrop(aFirstPoint, aSecondPoint);
            }
        }


}