using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using Keyboard;
using nucs.Automation.Controllers;
using nucs.Automation.Internals;
using nucs.Automation.Mirror.Helpers;
using nucs.Filesystem.Monitoring.Windows;
using nucs.SystemCore;
using HWND = System.IntPtr;
using Key = System.Windows.Input.Key;
using KeyDesc = Keyboard.Key;
using MouseButton = nucs.Automation.Controllers.MouseButton;

namespace nucs.Automation.Mirror {
    /// <summary>
    ///     Tools to find a window specific
    /// </summary>
    [DebuggerDisplay("{ProcessName} - {Title} - {ClassName}")]
    public class Window {
        /// <summary>
        ///     The process this window belongs to.
        /// </summary>
        public SmartProcess Process { get; set; }

        /// <summary>
        ///     The window handle (hWnd).
        /// </summary>
        public IntPtr Handle { get; set; }

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
        ///     Gets this window / control associated text.
        /// </summary>
        /// <returns></returns>
        public string GetText() {
            StringBuilder sb = new StringBuilder(65535);
            // needs to be big enough for the whole text
            Native.SendMessage(Handle, 0xD, (IntPtr) sb.Length, sb);
            return sb.ToString();
        }
        /// <summary>
        ///     The parent window handle of this window.
        /// </summary>
        public HWND Parent => Native.GetParent(Handle);

        /// <summary>
        ///     Gets or Sets the window state.
        /// </summary>
        public WindowState WindowState {
            get { return Native.GetPlacement(this.Handle).showCmd; }
            set { Native.ShowWindowAsync(this.Handle, value); }
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
        ///     Gets the ClassName of the current window.
        /// </summary>
        public string ClassName {
            get {
                StringBuilder _cn = new StringBuilder(256);
                return Native.GetClassName(Handle, _cn, _cn.Capacity) != 0 ? _cn.ToString() : null;
            }
        }

        /// <summary>
        ///     Shows the window and set it as foreground.
        /// </summary>
        public void Show() {
            Process.Show();
        }

        /// <summary>
        ///     Hides the window.
        /// </summary>
        public void Hide() {
            Process.Hide();
        }

        /// <summary>
        ///     Sets this window as ForegroundWindow, basiclly set focus on it and bring to the front
        /// </summary>
        /// <returns>If returns false meaning it can be set to foreground</returns>
        public bool BringToFront() => Native.SetForegroundWindow(Handle);

        /// <summary>
        ///     Position of the window
        /// </summary>
        public Rectangle Position {
            get { return Native.GetWindowRect(Handle); }
            set {
                if (value.X != -1 && value.Y != -1)
                    SetWindowPosition(value.X, value.Y);
                if (value.Width != -1 && value.Height != -1)
                    SetWindowSize(value.Width, value.Height);
            }
        }

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
            KeyboardController = new KeyboardControllerEmulator(handle);
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

            /// <summary>
            ///     Moves the cursor instantly to a given x,y.
            /// </summary>
            public void AbsoluteMove(int x, int y) {
                controller.AbsoluteMove(tr(x, y));
            }

            /// <summary>
            ///     Moves the cursor instantly to a given x,y.
            /// </summary>
            public void AbsoluteMove(Point aDestination) {
                controller.AbsoluteMove(tr(aDestination));
            }

            /// <summary>
            ///     Moves cursor in a given velocity, higher is faster (log-n).
            /// </summary>
            /// <param name="dx"></param>
            /// <param name="dy"></param>
            /// <param name="aMovementVelocityLogFactor"> higher is faster (log-n).</param>
            public Task Move(int dx, int dy, double aMovementVelocityLogFactor = 1) {
                return controller.Move(tr(dx, dy), aMovementVelocityLogFactor);
            }

            /// <summary>
            ///     Moves cursor in a given velocity, higher is faster (log-n).
            /// </summary>
            /// <param name="destination"></param>
            /// <param name="aMovementVelocityLogFactor"> higher is faster (log-n).</param>
            public Task Move(Point destination, double aMovementVelocityLogFactor = 1) {
                return controller.Move(tr(destination), aMovementVelocityLogFactor);
            }

            /// <summary>
            ///     Moves cursor in a given velocity, relativly to current position, higher is faster (log-n).
            /// </summary>
            /// <param name="yDisplacement"></param>
            /// <param name="xDisplacement"></param>
            /// <param name="aMovementVelocityLogFactor"> higher is faster (log-n).</param>
            public Task MoveRelative(int xDisplacement, int yDisplacement, double aMovementVelocityLogFactor = 2) {
                return controller.MoveRelative(tr(xDisplacement, yDisplacement), aMovementVelocityLogFactor);
            }

            /// <summary>
            ///     Moves cursor in a given velocity, relativly to current position, higher is faster (log-n).
            /// </summary>
            /// <param name="aDisplacement"></param>
            /// <param name="aMovementVelocityLogFactor"> higher is faster (log-n).</param>
            public Task MoveRelative(Point aDisplacement, int aMovementVelocityLogFactor = 2) {
                return controller.MoveRelative(tr(aDisplacement), aMovementVelocityLogFactor);
            }

            /// <summary>
            ///     Moves and clicks.
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="btn">The specific button to click</param>
            /// <returns></returns>
            public Task MoveClick(int x, int y, MouseButton btn = MouseButton.Left) {
                return controller.MoveClick(tr(x, y), btn);
            }

            public Task MoveClick(Point aPoint, MouseButton btn = MouseButton.Left) {
                return controller.MoveClick(tr(aPoint), btn);
            }

            /// <summary>
            ///     Moves, presses key down, waits for given time and releases the key up.
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="aWaitPeriod">The time to wait between the press down and up</param>
            /// <param name="btn">Which button to click</param>
            /// <returns></returns>
            public Task MoveClickHold(int x, int y, TimeSpan aWaitPeriod, MouseButton btn = MouseButton.Left) {
                return controller.MoveClickHold(tr(x, y), aWaitPeriod, btn);
            }

            /// <summary>
            ///     Moves, presses key down, waits for given time and releases the key up.
            /// </summary>
            /// <param name="aPoint"></param>
            /// <param name="aWaitPeriod">The time to wait between the press down and up</param>
            /// <param name="btn">Which button to click</param>
            /// <returns></returns>
            public Task MoveClickHold(Point aPoint, TimeSpan aWaitPeriod, MouseButton btn = MouseButton.Left) {
                return controller.MoveClickHold(tr(aPoint), aWaitPeriod, btn);
            }

            /// <summary>
            ///     Clicks, by default - left button.
            /// </summary>
            public Task Click(MouseButton btn = MouseButton.Left) {
                return controller.Click(btn);
            }

            /// <summary>
            ///     Double clicks, by default - left button
            /// </summary>
            public Task DoubleClick(MouseButton btn = MouseButton.Left) {
                return controller.DoubleClick(btn);
            }

            public Task MiddleClick() {
                return controller.MiddleClick();
            }

            public Task RightClick() {
                return controller.RightClick();
            }

            /// <summary>
            ///     moves to (o), presses left, moves to (d), releases left
            /// </summary>
            /// <param name="ox">From x</param>
            /// <param name="oy">From y</param>
            /// <param name="dx">To x</param>
            /// <param name="dy">To y</param>
            public Task DragDrop(int ox, int oy, int dx, int dy) {
                return controller.DragDrop(tr(ox, oy), tr(dx, dy));
            }

            /// <summary>
            ///     moves to (o), presses left, moves to (d), releases left
            /// </summary>
            /// <param name="o">From</param>
            /// <param name="d">To</param>
            public Task DragDrop(Point o, Point d) {
                return controller.DragDrop(tr(o), tr(d));
            }
        }

        #endregion

        #region Keyboard

        /// <summary>
        ///     Perform clicks relative to the window position only!
        ///     For otherwise, access static instance Mouse
        /// </summary>
        public KeyboardControllerEmulator KeyboardController { get; }

        public class KeyboardControllerEmulator : IExtendedKeyboardController {
            public HWND hWnd { get; set; }

            public KeyboardControllerEmulator(HWND hWnd) {
                this.hWnd = hWnd;
            }

            /// <summary>
            ///     Writes down the char that this key represents as if it was through the keyboard. - won't work on Keys like 'End' or 'Backspace' or 'Control'
            /// </summary>
            public void Write(Keys key) {
                Write((char) key);
            }

            /// <summary>
            ///     Writes down this string as if it was through the keyboard.
            /// </summary>
            public void Write(string text) {
                Messaging.SendChatTextSend(hWnd, text);
            }

            /// <summary>
            ///     Writes down the char that this key represents as if it was through the keyboard. - won't work on Keys like 'End' or 'Backspace' or 'Control'
            /// </summary>
            public void Write(KeyCode keycode) {
                this.Write((char) Native.MapVirtualKey((uint) keycode, 2));
            }

            /// <summary>
            ///     Writes down the char that this key represents as if it was through the keyboard.
            /// </summary>
            public void Write(char @char) {
                    Messaging.SendChar(hWnd, @char, false);
            }

            /// <summary>
            ///     Writes down the characters as if it was through the keyboard.
            /// </summary>
            public void Write(int utf32) {
                string unicodeString = Char.ConvertFromUtf32(utf32);
                this.Write(unicodeString);
            }

            /// <summary>
            ///     Writes down the characters as if it was through the keyboard.
            /// </summary>
            public void Write(params char[] chars) {
                foreach (var c in chars) {
                    Messaging.SendChar(hWnd, c, false);
                }
            }

            /// <summary>Presses down this keycode.</summary>
            public void Down(KeyCode keycode) {
                Messaging.SendPostDown(hWnd, new KeyDesc(keycode));
            }

            /// <summary>Releases this keycode.</summary>
            public void Up(KeyCode keycode) {
                Messaging.SendPostUp(hWnd, new KeyDesc(keycode));
            }

            /// <summary>
            ///     Presses down and releases this keycode with the given delay between them
            /// </summary>
            /// <param name="keycode">The keycode to press</param>
            /// <param name="delay">The delay between the actions in milliseconds</param>
            public void Press(KeyCode keycode, uint delay = 20) {
                Messaging.PostMessage(hWnd, new KeyDesc(keycode));
            }

            public void PressAsync(KeyCode keycode, uint delay = 20) {
                Messaging.PostMessage(hWnd, new KeyDesc(keycode));
            }

            public void Enter() {
                Messaging.PostMessage(hWnd, new KeyDesc(KeyCode.Enter));
            }

            public void Back() {
                Messaging.PostMessage(hWnd, new KeyDesc(KeyCode.Back));
            }

            public void Control(KeyCode keycode) {
                Messaging.SendPostDown(hWnd, new KeyDesc(KeyCode.Control));
                Messaging.PostMessage(hWnd, new KeyDesc(keycode));
                Messaging.SendPostUp(hWnd, new KeyDesc(KeyCode.Control));
            }

            public void Win(KeyCode keycode) {
                Messaging.SendPostDown(hWnd, new KeyDesc(KeyCode.LWin));
                Messaging.PostMessage(hWnd, new KeyDesc(keycode));
                Messaging.SendPostUp(hWnd, new KeyDesc(KeyCode.LWin));
            }

            public void Shift(KeyCode keycode) {
                Messaging.SendPostDown(hWnd, new KeyDesc(KeyCode.Shift));
                Messaging.PostMessage(hWnd, new KeyDesc(keycode));
                Messaging.SendPostUp(hWnd, new KeyDesc(KeyCode.Shift));
            }

            public void Alt(KeyCode keycode) {
                Messaging.SendPostDown(hWnd, new KeyDesc(KeyCode.Alt));
                Messaging.PostMessage(hWnd, new KeyDesc(keycode));
                Messaging.SendPostUp(hWnd, new KeyDesc(KeyCode.Alt));
            }

            public void Window(KeyCode keycode) {
                Win(keycode);
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