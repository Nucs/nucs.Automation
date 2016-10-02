using System;
using System.Drawing;
using nucs.Automation.Controllers;

namespace nucs.Automation {
        #region Keyboard

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

            public static void Move(int dx, int dy, double aMovementVelocityLogFactor = 1) {
                controller.Move(dx, dy, aMovementVelocityLogFactor);
            }

            public static void Move(Point destination, int aMovementVelocityLogFactor = 1) {
                controller.Move(destination, aMovementVelocityLogFactor);
            }

            public static void MoveRelative(int xDisplacement, int yDisplacement, int aMovementVelocityLogFactor = 2) {
                controller.MoveRelative(xDisplacement, yDisplacement, aMovementVelocityLogFactor);
            }

            public static void MoveRelative(Point aDisplacement, int aMovementVelocityLogFactor = 2) {
                controller.MoveRelative(aDisplacement, aMovementVelocityLogFactor);
            }

            public static void MoveClick(int x, int y) {
                controller.MoveClick(x, y);
            }

            public static void MoveClick(Point aPoint) {
                controller.MoveClick(aPoint);
            }

            public static void MoveClickHold(int x, int y, TimeSpan aWaitPeriod) {
                controller.MoveClickHold(x, y, aWaitPeriod);
            }

            public static void MoveClickHold(Point aPoint, TimeSpan aWaitPeriod) {
                controller.MoveClickHold(aPoint, aWaitPeriod);
            }

            public static void MoveClickDelay(int x, int y, TimeSpan aWaitPeriod) {
                controller.MoveClickDelay(x, y, aWaitPeriod);
            }

            public static void MoveClickDelay(Point aPoint, TimeSpan aWaitPeriod) {
                controller.MoveClickDelay(aPoint, aWaitPeriod);
            }

            public static void MoveDelayClick(int x, int y, TimeSpan aWaitPeriod) {
                controller.MoveDelayClick(x, y, aWaitPeriod);
            }

            public static void MoveDelayClick(Point aPoint, TimeSpan aWaitPeriod) {
                controller.MoveDelayClick(aPoint, aWaitPeriod);
            }

            public static void Click() {
                controller.Click();
            }

            public static void DoubleClick() {
                controller.DoubleClick();
            }

            public static void MiddleClick() {
                controller.MiddleClick();
            }

            public static void RightClick() {
                controller.RightClick();
            }

            public static void DragDrop(int ox, int oy, int dx, int dy) {
                controller.DragDrop(ox, oy, dx, dy);
            }

            public static void DragDrop(Point aFirstPoint, Point aSecondPoint) {
                controller.DragDrop(aFirstPoint, aSecondPoint);
            }
        }

        #endregion

}