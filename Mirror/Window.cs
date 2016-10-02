using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using nucs.Automation.Controllers;
using nucs.Automation.Internals;
using nucs.Automation.Mirror.Helpers;
using nucs.Filesystem.Monitoring.Windows;
using nucs.SystemCore;
using HWND = System.IntPtr;

namespace nucs.Automation.Mirror {
    /// <summary>
    ///     Tools to find a window specific
    /// </summary>
    [DebuggerDisplay("{ProcessName} - {Title}")]
    public class Window {
        /// <summary>
        ///     The process this window belongs to.
        /// </summary>
        public SmartProcess Process { get; set; }

        /// <summary>
        ///     The window handle (hWnd).
        /// </summary>
        public HWND Handle { get; set; }

        /// <summary>
        ///     Returns window title, if failed returns null.
        /// </summary>
        public string Title {
            get {
                try {
                    int length = Native.GetWindowTextLength(Handle);
                    if (length == 0)
                        return string.Empty;

                    StringBuilder builder = new StringBuilder(length);
                    Native.GetWindowText(Handle, builder, length + 1);
                    return builder.ToString();
                } catch {
                    return null;
                }
            }
        }

        /// <summary>
        ///     The process name of this window
        /// </summary>
        public string ProcessName => Process.ProcessName;

        /// <summary>
        ///     Is the handle still valid and window still exist.
        /// </summary>
        public bool IsValid => Native.IsWindow(Handle);

        /// <summary>
        ///     Is the window actually visible or just a part of a bigger window
        /// </summary>
        public bool IsVisible => Native.IsWindowVisible(Handle);

        /// <summary>
        ///     Sets this window as ForegroundWindow, basiclly set focus on it and bring to the front
        /// </summary>
        /// <returns>If returns false meaning it can be set to foreground</returns>
        public bool BringToFront() => Native.SetForegroundWindow(Handle);

        /// <summary>
        ///     Position of the window
        /// </summary>
        public Rectangle Position => Native.GetWindowRect(Handle);

        /// <summary>
        ///     The classification of this window
        /// </summary>
        public WindowType Type { get; set; } = WindowType.Generic;

        /// <summary>
        ///     A dictionary to contain specific classification objects.
        /// </summary>
        public Dictionary<string, object> Data { get; } = new Dictionary<string, object>(0);

        /// <summary>
        ///     Is the current window is at the ForegroundWindow
        /// </summary>
        public bool IsFocused => Native.GetForegroundWindow().ToInt32() == Handle.ToInt32();

        /// <summary>
        ///     Moves the current window to the given position while maintaining the window size.
        /// </summary>
        public void SetWindowPosition(Point point) {
            SetWindowPosition(point.X, point.Y);
        }

        /// <summary>
        ///     Moves the current window to the given position while maintaining the window size.
        /// </summary>
        public void SetWindowPosition(int x, int y) {
            var rect = Position;
            Native.MoveWindow(Handle, 0, 0, rect.Width, rect.Height, true);
        }

        /// <summary>
        ///     Sets the window size to the given new size
        /// </summary>
        public void SetWindowSize(Size size) {
            SetWindowSize(size.Width, size.Height);
        }

        /// <summary>
        ///     Sets the window size to the given new size
        /// </summary>
        public void SetWindowSize(uint width, uint height) {
            SetWindowSize(Convert.ToInt32(width), Convert.ToInt32(height));
        }

        /// <summary>
        ///     Sets the window size to the given new size
        /// </summary>
        public void SetWindowSize(int width, int height) {
            var rect = Position;
            Native.MoveWindow(Handle, rect.X, rect.Y, width, height, true);
        }

        public void ChangeWindowState(WindowState state) {
            Native.ShowWindowAsync(Handle, (int) state);
        }
        /// <summary>
        ///     Waits for the process to be responding
        /// </summary>
        public void WaitForResponding() {
            Process.Refresh();
            this.Process.WaitForInputIdle();
            SpinWait.SpinUntil(() => Process.Responding);
        }

        /// <summary>
        ///     Waits for the process to be responding
        /// </summary>
        public async Task WaitForRespondingAsync() {
            await Task.Yield();
            WaitForResponding();
        }
        #region Constructor

        private static readonly ExplorerMonitor _monitor;
        private static readonly Cache<ExplorerWindowRepresentor[]> _explorer_monitor;

        static Window() {
            _monitor = new ExplorerMonitor();
            _explorer_monitor = Cache<ExplorerWindowRepresentor[]>.FiveSeconds(() => _monitor.FetchCurrent().ToArray(), false);
        }

        public static Window Create(SmartProcess process, HWND handle) {
            var wnd = new Window(process, handle);
            var title = wnd.Title;
            if (wnd.ProcessName == "explorer") {
                if (title == "Start") { //start window
                    wnd.Type = WindowType.Windows;
                } else if (title == "Run") {
                    wnd.Type = WindowType.Run;
                } else {
                    var ie = _explorer_monitor.Object.FirstOrDefault(_ie => _ie.hWnd == handle);
                    if (ie != null) {
                        wnd.Type = WindowType.Explorer;
                        wnd.Data.Add("Path", ie.Location);
                    }
                }
            } else if (wnd.ProcessName == "chrome" || wnd.ProcessName == "firefox" || wnd.ProcessName == "opera" || wnd.ProcessName == "iexplorer") {
                wnd.Type = WindowType.Explorer;
            }

            return wnd;
        }

        private Window(SmartProcess process, HWND handle) {
            Process = process;
            Handle = handle;
            Mouse = new RelativeMouseEmulator(this);
            Keyboard = new KeyboardEmulator();
        }

        #endregion

        #region Mouse

        /// <summary>
        ///     Perform clicks relative to the window position only!
        ///     For otherwise, access static instance Mouse
        /// </summary>
        public RelativeMouseEmulator Mouse { get; }

        public class RelativeMouseEmulator {
            private Window window { get; }
            private readonly MouseController controller = new MouseController();

            private Point tr(int x, int y) {
                return tr(new Point(x, y));
            }

            private Point tr(Point p) {
                var rect = window.Position;
                return new Point(p.X + rect.X, p.Y + rect.Y);
            }

            public RelativeMouseEmulator(Window window) {
                this.window = window;
            }

            public void LeftDown() {
                controller.LeftDown();
            }

            public void LeftUp() {
                controller.LeftUp();
            }

            public void RightDown() {
                controller.RightDown();
            }

            public void RightUp() {
                controller.RightUp();
            }

            public void MiddleDown() {
                controller.MiddleDown();
            }

            public void MiddleUp() {
                controller.MiddleUp();
            }

            public void WheelDown() {
                controller.WheelDown();
            }

            public void WheelUp() {
                controller.WheelUp();
            }

            public void AbsoluteMove(int x, int y) {
                controller.AbsoluteMove(tr(x, y));
            }

            public void AbsoluteMove(Point aDestination) {
                controller.AbsoluteMove(tr(aDestination));
            }

            public void Move(int dx, int dy, double aMovementVelocityLogFactor = 1) {
                var rel = tr(dx, dy);

                controller.Move(rel.X, rel.Y, aMovementVelocityLogFactor);
            }

            public void Move(Point destination, int aMovementVelocityLogFactor = 1) {
                controller.Move(tr(destination), aMovementVelocityLogFactor);
            }

            public void MoveRelative(int xDisplacement, int yDisplacement, int aMovementVelocityLogFactor = 2) {
                controller.MoveRelative(tr(xDisplacement, yDisplacement), aMovementVelocityLogFactor);
            }

            public void MoveRelative(Point aDisplacement, int aMovementVelocityLogFactor = 2) {
                controller.MoveRelative(tr(aDisplacement), aMovementVelocityLogFactor);
            }

            public void MoveClick(int x, int y) {
                controller.MoveClick(tr(x, y));
            }

            public void MoveClick(Point aPoint) {
                controller.MoveClick(tr(aPoint));
            }

            public void MoveClickHold(int x, int y, TimeSpan aWaitPeriod) {
                controller.MoveClickHold(tr(x, y), aWaitPeriod);
            }

            public void MoveClickHold(Point aPoint, TimeSpan aWaitPeriod) {
                controller.MoveClickHold(tr(aPoint), aWaitPeriod);
            }

            public void MoveClickDelay(int x, int y, TimeSpan aWaitPeriod) {
                controller.MoveClickDelay(tr(x, y), aWaitPeriod);
            }

            public void MoveClickDelay(Point aPoint, TimeSpan aWaitPeriod) {
                controller.MoveClickDelay(tr(aPoint), aWaitPeriod);
            }

            public void MoveDelayClick(int x, int y, TimeSpan aWaitPeriod) {
                controller.MoveDelayClick(tr(x, y), aWaitPeriod);
            }

            public void MoveDelayClick(Point aPoint, TimeSpan aWaitPeriod) {
                controller.MoveDelayClick(tr(aPoint), aWaitPeriod);
            }

            public void Click() {
                controller.Click();
            }

            public void DoubleClick() {
                controller.DoubleClick();
            }

            public void MiddleClick() {
                controller.MiddleClick();
            }

            public void RightClick() {
                controller.RightClick();
            }

            public void DragDrop(int ox, int oy, int dx, int dy) {
                controller.DragDrop(tr(ox, oy), tr(dx, dy));
            }

            public void DragDrop(Point aFirstPoint, Point aSecondPoint) {
                controller.DragDrop(tr(aFirstPoint), tr(aSecondPoint));
            }
        }

        #endregion

        #region Keyboard

        /// <summary>
        ///     Perform clicks relative to the window position only!
        ///     For otherwise, access static instance Mouse
        /// </summary>
        public KeyboardEmulator Keyboard { get; }

        public class KeyboardEmulator : IModernKeyboard {
            /// <summary>
            ///     Writes down the char that this key represents as if it was through the keyboard. - won't work on Keys like 'End' or 'Backspace' or 'Control'
            /// </summary>
            public void Write(Keys key) {
                Automation.Keyboard.Write(key);
            }

            /// <summary>
            ///     Writes down this string as if it was through the keyboard.
            /// </summary>
            public void Write(string text) {
                Automation.Keyboard.Write(text);
            }

            /// <summary>
            ///     Writes down the char that this key represents as if it was through the keyboard. - won't work on Keys like 'End' or 'Backspace' or 'Control'
            /// </summary>
            public void Write(KeyCode keycode) {
                Automation.Keyboard.Write(keycode);
            }

            /// <summary>
            ///     Writes down the char that this key represents as if it was through the keyboard.
            /// </summary>
            public void Write(char @char) {
                Automation.Keyboard.Write(@char);
            }

            /// <summary>
            ///     Writes down the characters as if it was through the keyboard.
            /// </summary>
            public void Write(int utf32) {
                Automation.Keyboard.Write(utf32);
            }

            /// <summary>
            ///     Writes down the characters as if it was through the keyboard.
            /// </summary>
            public void Write(params char[] chars) {
                Automation.Keyboard.Write(chars);
            }

            /// <summary>Presses down this keycode.</summary>
            public void Down(KeyCode keycode) {
                Automation.Keyboard.Down(keycode);
            }

            /// <summary>Releases this keycode.</summary>
            public void Up(KeyCode keycode) {
                Automation.Keyboard.Up(keycode);
            }

            /// <summary>
            ///     Presses down and releases this keycode with the given delay between them
            /// </summary>
            /// <param name="keycode">The keycode to press</param>
            /// <param name="delay">The delay between the actions in milliseconds</param>
            public void Press(KeyCode keycode, uint delay = 20) {
                Automation.Keyboard.Press(keycode, delay);
            }

            public void PressAsync(KeyCode keycode, uint delay = 20) {
                Automation.Keyboard.PressAsync(keycode, delay);
            }

            public void Enter() {
                Automation.Keyboard.Enter();
            }

            public void Back() {
                Automation.Keyboard.Back();
            }

            public void Control(KeyCode keycode) {
                Automation.Keyboard.Control(keycode);
            }

            public void Win(KeyCode keycode) {
                Automation.Keyboard.Win(keycode);
            }

            public void Shift(KeyCode keycode) {
                Automation.Keyboard.Shift(keycode);
            }

            public void Alt(KeyCode keycode) {
                Automation.Keyboard.Alt(keycode);
            }

            public void Window(KeyCode keycode) {
                Automation.Keyboard.Window(keycode);
            }
        }

        #endregion
    }

    public static class WindowEx {
        /// <summary>
        ///     Gets all the windows in the smart proc
        /// </summary>
        public static List<Window> GetWindows(this SmartProcess sproc) {
            return OpenWindowGetter.GetOpenWindows(sproc.Id).Select(hwnd => Window.Create(sproc, hwnd)).ToList();
        }
    }
}