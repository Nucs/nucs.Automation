using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using HWND = System.IntPtr;
namespace nucs.Automation {
    internal static class Native {
        
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool MoveWindow(HWND hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        public static Rectangle GetWindowRect(HWND hWnd) {
            RECT rect;
            if (GetWindowRect(hWnd, out rect) == false)
                return Rectangle.Empty;
            return new Rectangle {X = rect.Left, Y = rect.Top, Width = rect.Right - rect.Left + 1, Height = rect.Bottom - rect.Top + 1};
        }

        [DllImport("USER32.DLL")]
        public static extern int GetWindowTextLength(HWND hWnd);

        [DllImport("USER32.DLL")]
        public static extern int GetWindowText(HWND hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(HWND hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT {
            public int Left; // x position of upper-left corner
            public int Top; // y position of upper-left corner
            public int Right; // x position of lower-right corner
            public int Bottom; // y position of lower-right corner
        }

        public const int SW_SHOWNORMAL = 1;
        public const int SW_SHOWMINIMIZED = 2;
        public const int SW_SHOWMAXIMIZED = 3;

        [DllImport("user32.dll")]
        public static extern bool ShowWindowAsync(HWND hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern HWND GetForegroundWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindow(HWND hWnd);

        [DllImport("USER32.DLL")]
        public static extern bool IsWindowVisible(HWND hWnd);

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(HWND hWnd);

        [DllImport("user32.dll")]
        public static extern HWND GetWindowThreadProcessId(HWND hWnd, out uint ProcessId);

        /// Return Type: BOOL->int
        ///fBlockIt: BOOL->int
        [DllImportAttribute("user32.dll", EntryPoint = "BlockInput")]
        [return: MarshalAsAttribute(UnmanagedType.Bool)]
        public static extern bool BlockInput([MarshalAsAttribute(UnmanagedType.Bool)] bool fBlockIt);

    }
}