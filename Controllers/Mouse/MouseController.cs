using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Win32Interop.Methods;
using Win32Interop.Structs;

namespace nucs.Automation.Controllers {
    public class MouseController {
        private readonly int desktopHeight = SystemInformation.PrimaryMonitorSize.Height;
        private readonly int desktopWidth = SystemInformation.PrimaryMonitorSize.Width;

        /// <summary>
        /// The speed at which the wheel will spin
        /// </summary>
        public int WheelSpeed = 120;

        public void LeftDown() {
            var inputBuffer = new INPUT {type = 0U};
            inputBuffer.inputData.mi.dwFlags = 2U;
            User32.SendInput(1U, new[] {inputBuffer}, Marshal.SizeOf(typeof(INPUT)));
        }

        public void LeftUp() {
            var inputBuffer = new INPUT {type = 0U};
            inputBuffer.inputData.mi.dwFlags = 4U;
            User32.SendInput(1U, new[] {inputBuffer}, Marshal.SizeOf(typeof(INPUT)));
        }

        public void RightDown() {
            var inputBuffer = new INPUT {type = 0U};
            inputBuffer.inputData.mi.dwFlags = 8U;
            User32.SendInput(1U, new[] {inputBuffer}, Marshal.SizeOf(typeof(INPUT)));
        }

        public void RightUp() {
            var inputBuffer = new INPUT {type = 0U};
            inputBuffer.inputData.mi.dwFlags = 16U;

            User32.SendInput(1U, new[] {inputBuffer}, Marshal.SizeOf(typeof(INPUT)));
        }

        public void MiddleDown() {
            var inputBuffer = new INPUT {type = 0U};
            inputBuffer.inputData.mi.dwFlags = 32U;

            User32.SendInput(1U, new[] {inputBuffer}, Marshal.SizeOf(typeof(INPUT)));
        }

        public void MiddleUp() {
            var inputBuffer = new INPUT {type = 0U};
            inputBuffer.inputData.mi.dwFlags = 64U;

            User32.SendInput(1U, new[] {inputBuffer}, Marshal.SizeOf(typeof(INPUT)));
        }

        public void WheelDown() {
            var inputBuffer = new INPUT {type = 0U};
            inputBuffer.inputData.mi.dwFlags = 2048U;
            inputBuffer.inputData.mi.mouseData = (uint)-WheelSpeed;

            User32.SendInput(1U, new[] {inputBuffer}, Marshal.SizeOf(typeof(INPUT)));
        }

        public void WheelUp() {
            var inputBuffer = new INPUT {type = 0U};
            inputBuffer.inputData.mi.dwFlags = 2048U;
            inputBuffer.inputData.mi.mouseData = (uint)WheelSpeed;

            User32.SendInput(1U, new[] {inputBuffer}, Marshal.SizeOf(typeof(INPUT)));
        }

        public void AbsoluteMove(int x, int y) {
            var inputBuffer = new INPUT {type = 0U};
            inputBuffer.inputData.mi.dwFlags = 32769U;
            inputBuffer.inputData.mi.dx = (int)(ushort.MaxValue * (double)x / desktopWidth);
            inputBuffer.inputData.mi.dy = (int)(ushort.MaxValue * (double)y / desktopHeight);
            User32.SendInput(1U, new[] {inputBuffer}, Marshal.SizeOf(typeof(INPUT)));
        }

        public void AbsoluteMove(Point aDestination) {
            AbsoluteMove(aDestination.X, aDestination.Y);
        }

        public void Move(int dx, int dy, double aMovementVelocityLogFactor = 1.0) {
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
                Thread.Sleep(5);
            }
            AbsoluteMove(dx, dy);
            Thread.Sleep(5);
        }

        public void Move(Point destination, int aMovementVelocityLogFactor = 1) {
            Move(destination.X, destination.Y, aMovementVelocityLogFactor);
        }

        public void MoveRelative(int xDisplacement, int yDisplacement, int aMovementVelocityLogFactor = 2) {
            var position = Cursor.Position;
            var dx = position.X + xDisplacement;
            position = Cursor.Position;
            var dy = position.Y + yDisplacement;
            double aMovementVelocityLogFactor1 = aMovementVelocityLogFactor;
            Move(dx, dy, aMovementVelocityLogFactor1);
        }

        public void MoveRelative(Point aDisplacement, int aMovementVelocityLogFactor = 2) {
            MoveRelative(aDisplacement.X, aDisplacement.Y, aMovementVelocityLogFactor);
        }

        public void MoveClick(int x, int y) {
            Move(x, y, 1.0);
            Thread.Sleep(50);
            Click();
        }

        public void MoveClick(Point aPoint) {
            MoveClick(aPoint.X, aPoint.Y);
        }

        public void MoveClickHold(int x, int y, TimeSpan aWaitPeriod) {
            var num = new Random().Next(5, 20);
            Move(x, y, 1.0);
            LeftDown();
            Thread.Sleep(aWaitPeriod + TimeSpan.FromMilliseconds(num));
            LeftUp();
        }

        public void MoveClickHold(Point aPoint, TimeSpan aWaitPeriod) {
            MoveClickHold(aPoint.X, aPoint.Y, aWaitPeriod);
        }

        public void MoveClickDelay(int x, int y, TimeSpan aWaitPeriod) {
            MoveClick(x, y);
            Thread.Sleep(aWaitPeriod);
        }

        public void MoveClickDelay(Point aPoint, TimeSpan aWaitPeriod) {
            MoveClickDelay(aPoint.X, aPoint.Y, aWaitPeriod);
        }

        public void MoveDelayClick(int x, int y, TimeSpan aWaitPeriod) {
            Move(x, y, 1.0);
            Thread.Sleep(aWaitPeriod);
            Click();
        }

        public void MoveDelayClick(Point aPoint, TimeSpan aWaitPeriod) {
            MoveDelayClick(aPoint.X, aPoint.Y, aWaitPeriod);
        }

        public void Click() {
            var num1 = new Random().Next(0, 20);
            LeftDown();
            var num2 = 35;
            Thread.Sleep(num1 + num2);
            LeftUp();
            var num3 = 35;
            Thread.Sleep(num1 + num3);
        }

        public void DoubleClick() {
            var num1 = new Random().Next(0, 20);
            Click();
            var num2 = 35;
            Thread.Sleep(num1 + num2);
            Click();
        }

        public void MiddleClick() {
            var num1 = new Random().Next(0, 20);
            MiddleDown();
            var num2 = 35;
            Thread.Sleep(num1 + num2);
            MiddleUp();
            var num3 = 35;
            Thread.Sleep(num1 + num3);
        }

        public void RightClick() {
            var num1 = new Random().Next(0, 20);
            RightDown();
            var num2 = 35;
            Thread.Sleep(num1 + num2);
            RightUp();
            var num3 = 35;
            Thread.Sleep(num1 + num3);
        }

        public void DragDrop(int ox, int oy, int dx, int dy) {
            Move(ox, oy, 1.0);
            LeftDown();
            Move(dx, dy, 1.0);
            LeftUp();
        }

        public void DragDrop(Point aFirstPoint, Point aSecondPoint) {
            DragDrop(aFirstPoint.X, aFirstPoint.Y, aSecondPoint.X, aSecondPoint.Y);
        }
    }
}