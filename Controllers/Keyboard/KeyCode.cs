using System;
using System.Windows.Forms;
using nucs.Windows;

namespace nucs.Automation.Controllers {

    public static class KeyCodeExtensions {
    #region Differentiating

        /// <summary>
        /// Is the given KeyCode is a modifier (regular or sided)
        /// </summary>
        public static bool IsModifier(this KeyCode kc) {
            var i = (ushort)kc;
            return (i >= 16 && i <= 18) || (i >= 160 && i <= 165);
        }

        /// <summary>
        /// Is the given KeyCode is a modifier, but contains a side indicator e.g. LControl or RShift
        /// </summary>
        public static bool IsSidedModifier(this KeyCode kc) {
            var i = (ushort)kc;
            return (i >= 160 && i <= 165);
        }

        /// <summary>
        /// Is the given KeyCode is a key
        /// </summary>
        public static bool IsKey(this KeyCode kc) {
            return !IsModifier(kc);
        }

        #endregion

        #region Conversion

        /// <summary>
        /// Converts a <see cref="KeyCode"/> Enum item to <see cref="Keys"/> of WinForm item. incase of missfit in conversion, <see cref="Keys.None"/> is returned
        /// </summary>
        /// <param name="kc"></param>
        /// <returns></returns>
        public static Keys ToKeys(this KeyCode kc) {
            switch (kc) {
                case KeyCode.LControl:
                    return Keys.LControlKey;
                case KeyCode.RControl:
                    return Keys.RControlKey;
                case KeyCode.LMenu:
                    return Keys.LMenu;
                case KeyCode.RMenu:
                    return Keys.RMenu;
                case KeyCode.LShift:
                    return Keys.LShiftKey;
                case KeyCode.RShift:
                    return Keys.RShiftKey;
                case KeyCode.Control:
                    return Keys.Control;
                case KeyCode.Menu:
                    return Keys.Alt;
                case KeyCode.Shift:
                    return Keys.Shift;
            }
            try {
                return (Keys) kc;
            } catch (InvalidCastException) {
                return Keys.None;
            }
        }
    /// <summary>
    /// Compares between two modifiers, where sided always equal to regular.
    /// </summary>
    public static bool CompareModifiers(this Keys key, Keys to) {
        if (key == Keys.Control && (to == Keys.LControlKey || to == Keys.RControlKey)) return true;
        if (key == Keys.Alt && (to == Keys.LMenu || to == Keys.RMenu)) return true;
        if (key == Keys.Shift && (to == Keys.LShiftKey || to == Keys.RShiftKey)) return true;
        return key.Equals(to);
    }

        /// <summary>
        /// Converts a <see cref="Keys"/> Enum item to <see cref="KeyCode"/> of WinForm item. incase of missfit in conversion, <see cref="KeyCode.None"/> is returned
        /// </summary>
        /// <param name="kc"></param>
        /// <returns></returns>
        public static KeyCode ToKeyCode(this Keys kc) {
            try {
                return (KeyCode)kc;
            } catch {
                return KeyCode.None;
            }
        }

        #endregion
        
        #region Status

        /// <summary>
        /// Determines whether a key is up or down at the time the function is called by calling the GetAsyncKeyState function. (See: http://msdn.microsoft.com/en-us/library/ms646293(VS.85).aspx)
        /// 
        /// </summary>
        /// <param name="keyCode">The key code.</param>
        /// <returns>
        /// <c>true</c> if the key is down; otherwise, <c>false</c>.
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
        public static bool IsKeyDownAsync(this KeyCode keyCode) {
            return NativeWin32.GetAsyncKeyState((ushort)keyCode) < 0;
        }

        /// <summary>
        /// Determines whether the specified key is up or down by calling the GetKeyState function. (See: http://msdn.microsoft.com/en-us/library/ms646301(VS.85).aspx)
        /// 
        /// </summary>
        /// <param name="keyCode">The <see cref="T:WindowsInput.KeyCode"/> for the key.</param>
        /// <returns>
        /// <c>true</c> if the key is down; otherwise, <c>false</c>.
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
        public static bool IsKeyDown(this KeyCode keyCode) {
            return NativeWin32.GetKeyState((ushort)keyCode) < 0;
        }

        /// <summary>
        /// Determines whether the toggling key is toggled on (in-effect) or not by calling the GetKeyState function.  (See: http://msdn.microsoft.com/en-us/library/ms646301(VS.85).aspx)
        /// 
        /// </summary>
        /// <param name="keyCode">The <see cref="T:WindowsInput.KeyCode"/> for the key.</param>
        /// <returns>
        /// <c>true</c> if the toggling key is toggled on (in-effect); otherwise, <c>false</c>.
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
        public static bool IsTogglingKeyInEffect(this KeyCode keyCode) {
            return (NativeWin32.GetKeyState((ushort)keyCode) & 1) == 1;
        }

        #endregion

}

    /// <summary>
    /// A list of the modifiers available in the keyboard
    /// </summary>
    [Serializable]
    public enum KeyCodeModifiers : ushort
    {
        Shift = (ushort)16,
        Control = (ushort)17,
        /// <summary>
        /// Equivilant to Alt
        /// </summary>
        Menu = (ushort)18,
        /// <summary>
        /// Equivilant to Menu
        /// </summary>
        Alt = (ushort)18,
        LShift = (ushort)160,
        RShift = (ushort)161,
        LControl = (ushort)162,
        RControl = (ushort)163,
        /// <summary>
        /// Equivalent to LMenu
        /// </summary>
        LAlt = (ushort)164,
        /// <summary>
        /// Equivalent to RMenu
        /// </summary>
        RAlt = (ushort)165,
        /// <summary>
        /// Equivalent to LAlt
        /// </summary>
        LMenu = (ushort)164,
        /// <summary>
        /// Equivalent to RAlt
        /// </summary>
        RMenu = (ushort)165
    }

    /// <summary>
    /// A list of only keys available on the keyboard
    /// </summary>
    [Serializable]
    public enum KeyCodeKeys : ushort
    {
        None = 0,
        LButton = (ushort)1,
        RButton = (ushort)2,
        Cancel = (ushort)3,
        MButton = (ushort)4,
        XButton1 = (ushort)5,
        XButton2 = (ushort)6,
        Back = (ushort)8,
        Tab = (ushort)9,
        Clear = (ushort)12,
        Return = (ushort)13,
        Pause = (ushort)19,
        Capital = (ushort)20,
        HANGEUL = (ushort)21,
        HANGUL = (ushort)21,
        KANA = (ushort)21,
        JUNJA = (ushort)23,
        Final = (ushort)24,
        HANJA = (ushort)25,
        KANJI = (ushort)25,
        Escape = (ushort)27,
        Convert = (ushort)28,
        NonConvert = (ushort)29,
        Accept = (ushort)30,
        ModeChange = (ushort)31,
        Space = (ushort)32,
        Prior = (ushort)33,
        Next = (ushort)34,
        End = (ushort)35,
        Home = (ushort)36,
        Left = (ushort)37,
        Up = (ushort)38,
        Right = (ushort)39,
        Down = (ushort)40,
        Select = (ushort)41,
        Print = (ushort)42,
        Execute = (ushort)43,
        Snapshot = (ushort)44,
        Insert = (ushort)45,
        Delete = (ushort)46,
        Help = (ushort)47,
        D0 = (ushort)48,
        D1 = (ushort)49,
        D2 = (ushort)50,
        D3 = (ushort)51,
        D4 = (ushort)52,
        D5 = (ushort)53,
        D6 = (ushort)54,
        D7 = (ushort)55,
        D8 = (ushort)56,
        D9 = (ushort)57,
        A = (ushort)65,
        B = (ushort)66,
        C = (ushort)67,
        D = (ushort)68,
        E = (ushort)69,
        F = (ushort)70,
        G = (ushort)71,
        H = (ushort)72,
        I = (ushort)73,
        J = (ushort)74,
        K = (ushort)75,
        L = (ushort)76,
        M = (ushort)77,
        N = (ushort)78,
        O = (ushort)79,
        P = (ushort)80,
        Q = (ushort)81,
        R = (ushort)82,
        S = (ushort)83,
        T = (ushort)84,
        U = (ushort)85,
        V = (ushort)86,
        W = (ushort)87,
        X = (ushort)88,
        Y = (ushort)89,
        Z = (ushort)90,
        /// <summary>
        /// Do not mistake, this button is not a modifier.
        /// </summary>
        LWIN = (ushort)91,
        /// <summary>
        /// Do not mistake, this button is not a modifier.
        /// </summary>
        RWIN = (ushort)92,
        Apps = (ushort)93,
        Sleep = (ushort)95,
        NUMPAD0 = (ushort)96,
        NUMPAD1 = (ushort)97,
        NUMPAD2 = (ushort)98,
        NUMPAD3 = (ushort)99,
        NUMPAD4 = (ushort)100,
        NUMPAD5 = (ushort)101,
        NUMPAD6 = (ushort)102,
        NUMPAD7 = (ushort)103,
        NUMPAD8 = (ushort)104,
        NUMPAD9 = (ushort)105,
        Multiply = (ushort)106,
        Add = (ushort)107,
        Seperator = (ushort)108,
        Substract = (ushort)109,
        Decimal = (ushort)110,
        Divide = (ushort)111,
        F1 = (ushort)112,
        F2 = (ushort)113,
        F3 = (ushort)114,
        F4 = (ushort)115,
        F5 = (ushort)116,
        F6 = (ushort)117,
        F7 = (ushort)118,
        F8 = (ushort)119,
        F9 = (ushort)120,
        F10 = (ushort)121,
        F11 = (ushort)122,
        F12 = (ushort)123,
        F13 = (ushort)124,
        F14 = (ushort)125,
        F15 = (ushort)126,
        F16 = (ushort)127,
        F17 = (ushort)128,
        F18 = (ushort)129,
        F19 = (ushort)130,
        F20 = (ushort)131,
        F21 = (ushort)132,
        F22 = (ushort)133,
        F23 = (ushort)134,
        F24 = (ushort)135,
        Numlock = (ushort)144,
        Scroll = (ushort)145,
        Browser_Back = (ushort)166,
        Browser_Forward = (ushort)167,
        Browser_Refresh = (ushort)168,
        Browser_Stop = (ushort)169,
        Browser_Search = (ushort)170,
        Browser_Favorites = (ushort)171,
        Browser_Home = (ushort)172,
        Volume_Mute = (ushort)173,
        Volume_Down = (ushort)174,
        Volume_Up = (ushort)175,
        Media_Next_Track = (ushort)176,
        Media_Prev_Track = (ushort)177,
        Media_Stop = (ushort)178,
        Media_Play_Pause = (ushort)179,
        Launch_Mail = (ushort)180,
        Launch_Media_Select = (ushort)181,
        Launch_App1 = (ushort)182,
        Launch_App2 = (ushort)183,
        OEM_1 = (ushort)186,
        OEM_Plus = (ushort)187,
        OEM_Comma = (ushort)188,
        OEM_Minus = (ushort)189,
        OEM_Period = (ushort)190,
        OEM_2 = (ushort)191,
        OEM_3 = (ushort)192,
        OEM_4 = (ushort)219,
        OEM_5 = (ushort)220,
        OEM_6 = (ushort)221,
        OEM_7 = (ushort)222,
        OEM_8 = (ushort)223,
        OEM_102 = (ushort)226,
        ProcessKey = (ushort)229,
        Packet = (ushort)231,
        ATTN = (ushort)246,
        CRSEL = (ushort)247,
        EXSEL = (ushort)248,
        EREOF = (ushort)249,
        Play = (ushort)250,
        Zoom = (ushort)251,
        NoName = (ushort)252,
        PA1 = (ushort)253,
        OEM_Clear = (ushort)254,
    }

    /// <summary>
    /// A full list of keys available on the keyboard.
    /// </summary>
    [Serializable]
    public enum KeyCode : ushort
    {
        None = 0,
        LButton = (ushort)1,
        RButton = (ushort)2,
        Cancel = (ushort)3,
        MButton = (ushort)4,
        XButton1 = (ushort)5,
        XButton2 = (ushort)6,
        Back = (ushort)8,
        Tab = (ushort)9,
        Clear = (ushort)12,
        Return = (ushort)13,
        /// <summary>
        /// Equivalent to Return
        /// </summary>
        Enter = Return,
        Shift = (ushort)16,
        Control = (ushort)17,
        /// <summary>
        /// Equivilant to Menu
        /// </summary>
        Alt = (ushort)18,
        /// <summary>
        /// Equivilant to Alt
        /// </summary>
        Menu = (ushort)18,
        Pause = (ushort)19,
        Capital = (ushort)20,
        HANGEUL = (ushort)21,
        HANGUL = (ushort)21,
        KANA = (ushort)21,
        JUNJA = (ushort)23,
        Final = (ushort)24,
        HANJA = (ushort)25,
        KANJI = (ushort)25,
        Escape = (ushort)27,
        Convert = (ushort)28,
        NonConvert = (ushort)29,
        Accept = (ushort)30,
        ModeChange = (ushort)31,
        Space = (ushort)32,
        Prior = (ushort)33,
        Next = (ushort)34,
        End = (ushort)35,
        Home = (ushort)36,
        Left = (ushort)37,
        Up = (ushort)38,
        Right = (ushort)39,
        Down = (ushort)40,
        Select = (ushort)41,
        Print = (ushort)42,
        Execute = (ushort)43,
        Snapshot = (ushort)44,
        Insert = (ushort)45,
        Delete = (ushort)46,
        Help = (ushort)47,
        D0 = (ushort)48,
        D1 = (ushort)49,
        D2 = (ushort)50,
        D3 = (ushort)51,
        D4 = (ushort)52,
        D5 = (ushort)53,
        D6 = (ushort)54,
        D7 = (ushort)55,
        D8 = (ushort)56,
        D9 = (ushort)57,
        A = (ushort)65,
        B = (ushort)66,
        C = (ushort)67,
        D = (ushort)68,
        E = (ushort)69,
        F = (ushort)70,
        G = (ushort)71,
        H = (ushort)72,
        I = (ushort)73,
        J = (ushort)74,
        K = (ushort)75,
        L = (ushort)76,
        M = (ushort)77,
        N = (ushort)78,
        O = (ushort)79,
        P = (ushort)80,
        Q = (ushort)81,
        R = (ushort)82,
        S = (ushort)83,
        T = (ushort)84,
        U = (ushort)85,
        V = (ushort)86,
        W = (ushort)87,
        X = (ushort)88,
        Y = (ushort)89,
        Z = (ushort)90,
        /// <summary>
        /// Do not mistake, this button is not a modifier.
        /// </summary>
        LWIN = (ushort)91,
        /// <summary>
        /// Do not mistake, this button is not a modifier.
        /// </summary>
        RWIN = (ushort)92,
        /// <summary>
        /// Do not mistake, this button is not a modifier.
        /// </summary>
        LWin = (ushort)91,
        /// <summary>
        /// Do not mistake, this button is not a modifier.
        /// </summary>
        RWin = (ushort)92,
        Apps = (ushort)93,
        Sleep = (ushort)95,
        NUMPAD0 = (ushort)96,
        NUMPAD1 = (ushort)97,
        NUMPAD2 = (ushort)98,
        NUMPAD3 = (ushort)99,
        NUMPAD4 = (ushort)100,
        NUMPAD5 = (ushort)101,
        NUMPAD6 = (ushort)102,
        NUMPAD7 = (ushort)103,
        NUMPAD8 = (ushort)104,
        NUMPAD9 = (ushort)105,
        Multiply = (ushort)106,
        Add = (ushort)107,
        Seperator = (ushort)108,
        Substract = (ushort)109,
        Decimal = (ushort)110,
        Divide = (ushort)111,
        F1 = (ushort)112,
        F2 = (ushort)113,
        F3 = (ushort)114,
        F4 = (ushort)115,
        F5 = (ushort)116,
        F6 = (ushort)117,
        F7 = (ushort)118,
        F8 = (ushort)119,
        F9 = (ushort)120,
        F10 = (ushort)121,
        F11 = (ushort)122,
        F12 = (ushort)123,
        F13 = (ushort)124,
        F14 = (ushort)125,
        F15 = (ushort)126,
        F16 = (ushort)127,
        F17 = (ushort)128,
        F18 = (ushort)129,
        F19 = (ushort)130,
        F20 = (ushort)131,
        F21 = (ushort)132,
        F22 = (ushort)133,
        F23 = (ushort)134,
        F24 = (ushort)135,
        Numlock = (ushort)144,
        Scroll = (ushort)145,
        LShift = (ushort)160,
        RShift = (ushort)161,
        LControl = (ushort)162,
        RControl = (ushort)163,
        LAlt = (ushort)164,
        RAlt = (ushort)165,
        LMenu = (ushort)164,
        RMenu = (ushort)165,
        Browser_Back = (ushort)166,
        Browser_Forward = (ushort)167,
        Browser_Refresh = (ushort)168,
        Browser_Stop = (ushort)169,
        Browser_Search = (ushort)170,
        Browser_Favorites = (ushort)171,
        Browser_Home = (ushort)172,
        Volume_Mute = (ushort)173,
        Volume_Down = (ushort)174,
        Volume_Up = (ushort)175,
        Media_Next_Track = (ushort)176,
        Media_Prev_Track = (ushort)177,
        Media_Stop = (ushort)178,
        Media_Play_Pause = (ushort)179,
        Launch_Mail = (ushort)180,
        Launch_Media_Select = (ushort)181,
        Launch_App1 = (ushort)182,
        Launch_App2 = (ushort)183,
        OEM_1 = (ushort)186,
        OEM_Plus = (ushort)187,
        OEM_Comma = (ushort)188,
        OEM_Minus = (ushort)189,
        OEM_Period = (ushort)190,
        OEM_2 = (ushort)191,
        OEM_3 = (ushort)192,
        OEM_4 = (ushort)219,
        OEM_5 = (ushort)220,
        OEM_6 = (ushort)221,
        OEM_7 = (ushort)222,
        OEM_8 = (ushort)223,
        OEM_102 = (ushort)226,
        ProcessKey = (ushort)229,
        Packet = (ushort)231,
        ATTN = (ushort)246,
        CRSEL = (ushort)247,
        EXSEL = (ushort)248,
        EREOF = (ushort)249,
        Play = (ushort)250,
        Zoom = (ushort)251,
        NoName = (ushort)252,
        PA1 = (ushort)253,
        OEM_Clear = (ushort)254
    }

}