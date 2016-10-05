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

        /// <summary>
        /// The GetAsyncKeyState function determines whether a key is up or down at the time the function is called, and whether the key was pressed after a previous call to GetAsyncKeyState. (See: http://msdn.microsoft.com/en-us/library/ms646293(VS.85).aspx)
        /// 
        /// </summary>
        /// <param name="virtualKeyCode">Specifies one of 256 possible virtual-key codes. For more information, see Virtual Key Codes. Windows NT/2000/XP: You can use left- and right-distinguishing constants to specify certain keys. See the Remarks section for further information.</param>
        /// <returns>
        /// If the function succeeds, the return value specifies whether the key was pressed since the last call to GetAsyncKeyState, and whether the key is currently up or down. If the most significant bit is set, the key is down, and if the least significant bit is set, the key was pressed after the previous call to GetAsyncKeyState. However, you should not rely on this last behavior; for more information, see the Remarks.
        /// 
        ///             Windows NT/2000/XP: The return value is zero for the following cases:
        ///             - The current desktop is not the active desktop
        ///             - The foreground thread belongs to another process and the desktop does not allow the hook or the journal record.
        /// 
        ///             Windows 95/98/Me: The return value is the global asynchronous key state for each virtual key. The system does not check which thread has the keyboard focus.
        /// 
        ///             Windows 95/98/Me: Windows 95 does not support the left- and right-distinguishing constants. If you call GetAsyncKeyState with these constants, the return value is zero.
        /// 
        /// </returns>
        /// 
        /// <remarks>
        /// The GetAsyncKeyState function works with mouse buttons. However, it checks on the state of the physical mouse buttons, not on the logical mouse buttons that the physical buttons are mapped to. For example, the call GetAsyncKeyState(VK_LBUTTON) always returns the state of the left physical mouse button, regardless of whether it is mapped to the left or right logical mouse button. You can determine the system's current mapping of physical mouse buttons to logical mouse buttons by calling
        ///             Copy CodeGetSystemMetrics(SM_SWAPBUTTON) which returns TRUE if the mouse buttons have been swapped.
        /// 
        ///             Although the least significant bit of the return value indicates whether the key has been pressed since the last query, due to the pre-emptive multitasking nature of Windows, another application can call GetAsyncKeyState and receive the "recently pressed" bit instead of your application. The behavior of the least significant bit of the return value is retained strictly for compatibility with 16-bit Windows applications (which are non-preemptive) and should not be relied upon.
        /// 
        ///             You can use the virtual-key code constants VK_SHIFT, VK_CONTROL, and VK_MENU as values for the vKey parameter. This gives the state of the SHIFT, CTRL, or ALT keys without distinguishing between left and right.
        /// 
        ///             Windows NT/2000/XP: You can use the following virtual-key code constants as values for vKey to distinguish between the left and right instances of those keys.
        /// 
        ///             Code Meaning
        ///             VK_LSHIFT Left-shift key.
        ///             VK_RSHIFT Right-shift key.
        ///             VK_LCONTROL Left-control key.
        ///             VK_RCONTROL Right-control key.
        ///             VK_LMENU Left-menu key.
        ///             VK_RMENU Right-menu key.
        /// 
        ///             These left- and right-distinguishing constants are only available when you call the GetKeyboardState, SetKeyboardState, GetAsyncKeyState, GetKeyState, and MapVirtualKey functions.
        /// 
        /// </remarks>
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, SetLastError = true)]
        public static extern short GetAsyncKeyState(ushort virtualKeyCode);

        /// <summary>
        /// The GetKeyState function retrieves the status of the specified virtual key. The status specifies whether the key is up, down, or toggled (on, off alternating each time the key is pressed). (See: http://msdn.microsoft.com/en-us/library/ms646301(VS.85).aspx)
        /// 
        /// </summary>
        /// <param name="virtualKeyCode">Specifies a virtual key. If the desired virtual key is a letter or digit (A through Z, a through z, or 0 through 9), nVirtKey must be set to the ASCII value of that character. For other keys, it must be a virtual-key code.
        ///             If a non-English keyboard layout is used, virtual keys with values in the range ASCII A through Z and 0 through 9 are used to specify most of the character keys. For example, for the German keyboard layout, the virtual key of value ASCII O (0x4F) refers to the "o" key, whereas VK_OEM_1 refers to the "o with umlaut" key.
        ///             </param>
        /// <returns>
        /// The return value specifies the status of the specified virtual key, as follows:
        ///             If the high-order bit is 1, the key is down; otherwise, it is up.
        ///             If the low-order bit is 1, the key is toggled. A key, such as the CAPS LOCK key, is toggled if it is turned on. The key is off and untoggled if the low-order bit is 0. A toggle key's indicator light (if any) on the keyboard will be on when the key is toggled, and off when the key is untoggled.
        /// 
        /// </returns>
        /// 
        /// <remarks>
        /// The key status returned from this function changes as a thread reads key messages from its message queue. The status does not reflect the interrupt-level state associated with the hardware. Use the GetAsyncKeyState function to retrieve that information.
        ///             An application calls GetKeyState in response to a keyboard-input message. This function retrieves the state of the key when the input message was generated.
        ///             To retrieve state information for all the virtual keys, use the GetKeyboardState function.
        ///             An application can use the virtual-key code constants VK_SHIFT, VK_CONTROL, and VK_MENU as values for the nVirtKey parameter. This gives the status of the SHIFT, CTRL, or ALT keys without distinguishing between left and right. An application can also use the following virtual-key code constants as values for nVirtKey to distinguish between the left and right instances of those keys.
        ///             VK_LSHIFT
        ///             VK_RSHIFT
        ///             VK_LCONTROL
        ///             VK_RCONTROL
        ///             VK_LMENU
        ///             VK_RMENU
        /// 
        ///             These left- and right-distinguishing constants are available to an application only through the GetKeyboardState, SetKeyboardState, GetAsyncKeyState, GetKeyState, and MapVirtualKey functions.
        /// 
        /// </remarks>
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, SetLastError = true)]
        public static extern short GetKeyState(int virtualKeyCode);

        /// <summary>Synthesizes keystrokes, mouse motions, and button clicks.</summary>
        /// <param name="nInputs">
        ///     The number of structures in the <paramref name="pInputs" /> array.
        /// </param>
        /// <param name="pInputs">
        ///     An array of <see cref="INPUT" /> structures. Each structure represents an event to be inserted into the keyboard or mouse input stream.
        /// </param>
        /// <param name="cbSize">
        ///     The size, in bytes, of an <see cref="INPUT" /> structure. If <paramref name="cbSize" /> is not the size of an <see cref="INPUT" /> structure, the
        ///     function fails.
        /// </param>
        /// <returns>
        ///     <para>
        ///         The function returns the number of events that it successfully inserted into the keyboard or mouse input stream. If the function returns
        ///         zero, the input was already blocked by another thread. To get extended error information, call GetLastError.
        ///     </para>
        ///     <para>
        ///         This function fails when it is blocked by UIPI. Note that neither GetLastError nor the return value will indicate the failure was caused by
        ///         UIPI blocking.
        ///     </para>
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         This function is subject to UIPI. Applications are permitted to inject input only into applications that are at an equal or lesser
        ///         integrity level.
        ///     </para>
        ///     <para>
        ///         The <see cref="SendInput" /> function inserts the events in the <see cref="INPUT" /> structures serially into the keyboard or mouse input
        ///         stream. These events are not interspersed with other keyboard or mouse input events inserted either by the user (with the keyboard or mouse)
        ///         or by calls to <see cref="keybd_event" />, <see cref="mouse_event" />, or other calls to <see cref="SendInput" />.
        ///     </para>
        ///     <para>
        ///         This function does not reset the keyboard's current state. Any keys that are already pressed when the function is called might interfere with
        ///         the events that this function generates. To avoid this problem, check the keyboard's state with the <see cref="GetAsyncKeyState" /> function
        ///         and correct as necessary.
        ///     </para>
        ///     <para>
        ///         Because the touch keyboard uses the surrogate macros defined in winnls.h to send input to the system, a listener on the keyboard event hook
        ///         must decode input originating from the touch keyboard. For more information, see Surrogates and Supplementary Characters.
        ///     </para>
        ///     <para>
        ///         An accessibility application can use <see cref="SendInput" /> to inject keystrokes corresponding to application launch shortcut keys that are
        ///         handled by the shell. This functionality is not guaranteed to work for other types of applications.
        ///     </para>
        /// </remarks>
        [DllImport("user32.dll", EntryPoint = "SendInput")]
        public static extern uint SendInput(uint nInputs, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct, SizeParamIndex = 0)] INPUT[] pInputs, int cbSize);

        /// <summary>
        ///     <para>Translates (maps) a virtual-key code into a scan code or character value, or translates a scan code into a virtual-key code.</para>
        ///     <para>
        ///         To specify a handle to the keyboard layout to use for translating the specified code, use the <see cref="MapVirtualKeyEx" /> function.
        ///     </para>
        /// </summary>
        /// <param name="uCode">
        ///     The virtual key code or scan code for a key. How this value is interpreted depends on the value of the <paramref name="uMapType" /> parameter.
        /// </param>
        /// <param name="uMapType">
        ///     <para>
        ///         The translation to be performed. The value of this parameter depends on the value of the <paramref name="uCode" /> parameter.
        ///     </para>
        ///     <list type="table">
        ///         <item>
        ///             <term>MAPVK_VK_TO_CHAR 2</term>
        ///             <description>
        ///                 <paramref name="uCode" /> is a virtual-key code and is translated into an unshifted character value in the low-order word of the
        ///                 return value. Dead keys (diacritics) are indicated by setting the top bit of the return value. If there is no translation, the
        ///                 function returns 0.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>MAPVK_VK_TO_VSC 0</term>
        ///             <description>
        ///                 <paramref name="uCode" /> is a virtual-key code and is translated into a scan code. If it is a virtual-key code that does not
        ///                 distinguish between left- and right-hand keys, the left-hand scan code is returned. If there is no translation, the function returns
        ///                 0.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>MAPVK_VSC_TO_VK 1</term>
        ///             <description>
        ///                 <paramref name="uCode" /> is a scan code and is translated into a virtual-key code that does not distinguish between left- and
        ///                 right-hand keys. If there is no translation, the function returns 0.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>MAPVK_VSC_TO_VK_EX 3</term>
        ///             <description>
        ///                 <paramref name="uCode" /> is a scan code and is translated into a virtual-key code that distinguishes between left- and right-hand
        ///                 keys. If there is no translation, the function returns 0.
        ///             </description>
        ///         </item>
        ///     </list>
        /// </param>
        /// <returns>
        ///     The return value is either a scan code, a virtual-key code, or a character value, depending on the value of <paramref name="uCode" /> and
        ///     <paramref name="uMapType" />. If there is no translation, the return value is zero.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         An application can use <see cref="MapVirtualKey" /> to translate scan codes to the virtual-key code constants <see cref="VK.VK_SHIFT" />,
        ///         <see cref="VK.VK_CONTROL" />, and <see cref="VK.VK_MENU" />, and vice versa. These translations do not distinguish between the left and right
        ///         instances of the SHIFT, CTRL, or ALT keys.
        ///     </para>
        ///     <para>
        ///         An application can get the scan code corresponding to the left or right instance of one of these keys by calling <see cref="MapVirtualKey" />
        ///         with <paramref name="uCode" /> set to one of the following virtual-key code constants.
        ///     </para>
        /// </remarks>
        [DllImport("user32.dll", EntryPoint = "MapVirtualKey")]
        public static extern uint MapVirtualKey(uint uCode, uint uMapType);
    }

    #region From Win32Interop 

    [StructLayout(LayoutKind.Sequential)]
    internal struct INPUT {
        /// DWORD->unsigned int
        public uint type;

        /// Anonymous_dccf47da_5155_438b_92bc_41adbefe840c
        public INPUT_DATA inputData;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct INPUT_DATA {
        /// MOUSEINPUT->tagMOUSEINPUT
        [FieldOffset(0)] public MOUSEINPUT mi;

        /// KEYBDINPUT->tagKEYBDINPUT
        [FieldOffset(0)] public KEYBDINPUT ki;

        /// HARDWAREINPUT->tagHARDWAREINPUT
        [FieldOffset(0)] public HARDWAREINPUT hi;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MOUSEINPUT {
        /// LONG->int
        public int dx;

        /// LONG->int
        public int dy;

        /// DWORD->unsigned int
        public uint mouseData;

        /// DWORD->unsigned int
        public uint dwFlags;

        /// DWORD->unsigned int
        public uint time;

        /// ULONG_PTR->unsigned int
        public uint dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct KEYBDINPUT {
        /// WORD->unsigned short
        public ushort wVk;

        /// WORD->unsigned short
        public ushort wScan;

        /// DWORD->unsigned int
        public uint dwFlags;

        /// DWORD->unsigned int
        public uint time;

        /// ULONG_PTR->unsigned int
        public uint dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct HARDWAREINPUT {
        /// DWORD->unsigned int
        public uint uMsg;

        /// WORD->unsigned short
        public ushort wParamL;

        /// WORD->unsigned short
        public ushort wParamH;
    }

    #endregion
}