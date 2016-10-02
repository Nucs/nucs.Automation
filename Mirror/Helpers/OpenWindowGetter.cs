using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using HWND = System.IntPtr;

namespace nucs.Automation.Mirror.Helpers {
    /// <summary>Contains functionality to get all the open windows.</summary>
    public static class OpenWindowGetter {
        /// <summary>Returns a dictionary that contains the handle and title of all the open windows.</summary>
        /// <returns>A dictionary that contains the handle and title of all the open windows.</returns>
        public static List<HWND> GetOpenWindows() {
            var shellWindow = GetShellWindow();
            var windows = new List<HWND>();

            EnumWindows(delegate(HWND hWnd, int lParam) {
                if (hWnd == shellWindow)
                    return true;
                if (!IsWindowVisible(hWnd))
                    return true;
                windows.Add(hWnd);
                return true;
            }, 0);

            return windows;
        }

        /// <summary>Returns a dictionary that contains the handle and title of all the open windows.</summary>
        /// <returns>A dictionary that contains the handle and title of all the open windows.</returns>
        public static List<HWND> GetOpenWindows(int process_pid) {
            return EnumerateProcessWindowHandles(process_pid).ToList();
        }

        private delegate bool EnumWindowsProc(HWND hWnd, int lParam);

        [DllImport("USER32.DLL")]
        private static extern bool EnumWindows(EnumWindowsProc enumFunc, int lParam);

        [DllImport("USER32.DLL")]
        private static extern bool IsWindowVisible(HWND hWnd);

        [DllImport("USER32.DLL")]
        private static extern IntPtr GetShellWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowThreadProcessId(HandleRef handle, out int processId);


        private delegate bool EnumThreadDelegate(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool EnumThreadWindows(int dwThreadId, EnumThreadDelegate lpfn,
            IntPtr lParam);

        private static IEnumerable<IntPtr> EnumerateProcessWindowHandles(int processId) {
            var handles = new List<IntPtr>();

            foreach (ProcessThread thread in Process.GetProcessById(processId).Threads)
                EnumThreadWindows(thread.Id,
                    (hWnd, lParam) => {
                        handles.Add(hWnd);
                        return true;
                    }, IntPtr.Zero);

            return handles;
        }
    }
}