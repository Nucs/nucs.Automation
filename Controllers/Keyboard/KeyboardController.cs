// Decompiled with JetBrains decompiler
// Type: HenoohDeviceEmulator.KeyboardController
// Assembly: HenoohDeviceEmulator, Version=1.0.3.0, Culture=neutral, PublicKeyToken=null
// MVID: B0E65FD6-BFBF-4C93-B7DA-58548A4136BE
// Assembly location: D:\C#\nlib\packages\HenoohDeviceEmulator.1.00.03.000\lib\HenoohDeviceEmulator.dll

using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Win32Interop.Methods;
using Win32Interop.Structs;
using KEYBDINPUT = Win32Interop.Structs.KEYBDINPUT;

namespace nucs.Automation.Controllers {
    public class KeyboardController : IModernKeyboard {
        #region Write

        /// <summary>
        ///     Writes down the char that this key represents as if it was through the keyboard. - won't work on Keys like 'End' or 'Backspace' or 'Control'
        /// </summary>
        public void Write(Keys key) {
            this.Write((char) key);
        }

        /// <summary>
        ///     Writes down this string as if it was through the keyboard.
        /// </summary>
        public void Write(string text) {
            this.Write(text.ToCharArray());
        }

        /// <summary>
        ///     Writes down the char that this key represents as if it was through the keyboard. - won't work on Keys like 'End' or 'Backspace' or 'Control'
        /// </summary>
        public void Write(KeyCode keycode) {
            this.Write((char) User32.MapVirtualKey((uint) keycode, 2));
        }

        /// <summary>
        ///     Writes down the char that this key represents as if it was through the keyboard.
        /// </summary>
        public void Write(char @char) {
            this.Write(new[] {@char});
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
            var input = new INPUT[chars.Length];
            for (int i = 0; i < input.Length; i++) {
                input[i] = new INPUT {type = 1};
                input[i].inputData.ki.wVk = 0;
                input[i].inputData.ki.wScan = @chars[i];
                input[i].inputData.ki.time = 0;
                input[i].inputData.ki.dwFlags = (uint) Win32Interop.Enums.KEYEVENTF.KEYEVENTF_UNICODE;
                input[i].inputData.ki.dwExtraInfo = 0;
            }

            User32.SendInput((uint) input.Length, input, Marshal.SizeOf(typeof(INPUT)));
        }

        #endregion

        #region Actions

        public void Down(KeyCode keycode) {
            var inputBuffer = new INPUT {
                type = 1U,
                inputData = {
                    ki = new KEYBDINPUT {
                        wVk = (ushort) keycode,
                        wScan = this.Convert(keycode),
                        dwFlags = this.IsExtendedKey(keycode) ? 1U : 8U,
                        time = 0U,
                        dwExtraInfo = 0
                    }
                }
            };
            User32.SendInput(1U, new[] {inputBuffer}, Marshal.SizeOf(typeof(INPUT)));
        }

        public void Up(KeyCode keycode) {
            var inputBuffer = new INPUT {
                type = 1U,
                inputData = {
                    ki = new KEYBDINPUT {
                        wVk = (ushort) keycode,
                        wScan = this.Convert(keycode),
                        dwFlags = this.IsExtendedKey(keycode) ? 3U : 2U,
                        time = 0U,
                        dwExtraInfo = 0
                    }
                }
            };
            User32.SendInput(1U, new[] {inputBuffer}, Marshal.SizeOf(typeof(INPUT)));
        }

        /// <summary>
        ///     Presses down and releases this keycode with the given delay between them
        /// </summary>
        /// <param name="keycode">The keycode to press</param>
        /// <param name="delay">The delay between the actions in milliseconds</param>
        public void Press(KeyCode keycode, uint delay = 20) {
            Down(keycode);
            Thread.Sleep((int) delay);
            Up(keycode);
        }

#if !NET4 && !NET40
        public async void PressAsync(KeyCode keycode, uint delay = 20) {
            Down(keycode);
            await Task.Delay((int) delay);
            Up(keycode);
        }
#endif

        public void Enter() {
            Press(KeyCode.Enter);
        }

        public void Back() {
            Press(KeyCode.Back);
        }


        public void Control(KeyCode keycode) {
            this.Down(KeyCode.LControl);
            this.Press(keycode);
            this.Up(KeyCode.LControl);
        }

        public void Win(KeyCode keycode) {
            this.Down(KeyCode.LWIN);
            this.Press(keycode);
            this.Up(KeyCode.LWin);
        }


        public void Shift(KeyCode keycode) {
            this.Down(KeyCode.LShift);
            this.Press(keycode);
            this.Up(KeyCode.LShift);
        }

        public void Alt(KeyCode keycode) {
            this.Down(KeyCode.LAlt);
            this.Press(keycode);
            this.Up(KeyCode.LAlt);
        }

        public void Window(KeyCode keycode) {
            this.Down(KeyCode.LWin);
            this.Press(keycode);
            this.Up(KeyCode.LWin);
        }

        #endregion

        #region Helpers

        private bool IsExtendedKey(KeyCode keyCode) {
            return keyCode == KeyCode.Menu || keyCode == KeyCode.LMenu || (keyCode == KeyCode.RMenu || keyCode == KeyCode.Control) || (keyCode == KeyCode.RControl || keyCode == KeyCode.Insert || (keyCode == KeyCode.Delete || keyCode == KeyCode.Home)) || (keyCode == KeyCode.End || keyCode == KeyCode.Prior || (keyCode == KeyCode.Next || keyCode == KeyCode.Right) || (keyCode == KeyCode.Up || keyCode == KeyCode.Left || (keyCode == KeyCode.Down || keyCode == KeyCode.Numlock))) || (keyCode == KeyCode.Cancel || keyCode == KeyCode.Snapshot || (keyCode == KeyCode.Divide || keyCode == KeyCode.LWIN) || keyCode == KeyCode.RWIN);
        }

        private ushort Convert(KeyCode keycode) {
            switch (keycode) {
                case KeyCode.Back:
                    return 1038;
                case KeyCode.Tab:
                    return 1039;
                case KeyCode.Return:
                    return 1052;
                case KeyCode.Capital:
                    return 1082;
                case KeyCode.KANA:
                    return 1136;
                case KeyCode.HANJA:
                    return 1172;
                case KeyCode.Escape:
                    return 1025;
                case KeyCode.Convert:
                    return 1145;
                case KeyCode.NonConvert:
                    return 1147;
                case KeyCode.Space:
                    return 1081;
                case KeyCode.Prior:
                    return 1225;
                case KeyCode.Next:
                    return 1233;
                case KeyCode.End:
                    return 1231;
                case KeyCode.Home:
                    return 1223;
                case KeyCode.Left:
                    return 1227;
                case KeyCode.Up:
                    return 1224;
                case KeyCode.Right:
                    return 1229;
                case KeyCode.Down:
                    return 1232;
                case KeyCode.Insert:
                    return 1234;
                case KeyCode.Delete:
                    return 1235;
                case KeyCode.D0:
                    return 1035;
                case KeyCode.D1:
                    return 1026;
                case KeyCode.D2:
                    return 1027;
                case KeyCode.D3:
                    return 1028;
                case KeyCode.D4:
                    return 1029;
                case KeyCode.D5:
                    return 1030;
                case KeyCode.D6:
                    return 1031;
                case KeyCode.D7:
                    return 1032;
                case KeyCode.D8:
                    return 1033;
                case KeyCode.D9:
                    return 1034;
                case KeyCode.A:
                    return 1054;
                case KeyCode.B:
                    return 1072;
                case KeyCode.C:
                    return 1070;
                case KeyCode.D:
                    return 1056;
                case KeyCode.E:
                    return 1042;
                case KeyCode.F:
                    return 1057;
                case KeyCode.G:
                    return 1058;
                case KeyCode.H:
                    return 1059;
                case KeyCode.I:
                    return 1047;
                case KeyCode.J:
                    return 1060;
                case KeyCode.K:
                    return 1061;
                case KeyCode.L:
                    return 1062;
                case KeyCode.M:
                    return 1074;
                case KeyCode.N:
                    return 1073;
                case KeyCode.O:
                    return 1048;
                case KeyCode.P:
                    return 1049;
                case KeyCode.Q:
                    return 1040;
                case KeyCode.R:
                    return 1043;
                case KeyCode.S:
                    return 1055;
                case KeyCode.T:
                    return 1044;
                case KeyCode.U:
                    return 1046;
                case KeyCode.V:
                    return 1071;
                case KeyCode.W:
                    return 1041;
                case KeyCode.X:
                    return 1069;
                case KeyCode.Y:
                    return 1045;
                case KeyCode.Z:
                    return 1068;
                case KeyCode.LWIN:
                    return 1243;
                case KeyCode.RWIN:
                    return 1244;
                case KeyCode.Apps:
                    return 1245;
                case KeyCode.NUMPAD0:
                    return 1106;
                case KeyCode.NUMPAD1:
                    return 1103;
                case KeyCode.NUMPAD2:
                    return 1104;
                case KeyCode.NUMPAD3:
                    return 1105;
                case KeyCode.NUMPAD4:
                    return 1099;
                case KeyCode.NUMPAD5:
                    return 1100;
                case KeyCode.NUMPAD6:
                    return 1101;
                case KeyCode.NUMPAD7:
                    return 1095;
                case KeyCode.NUMPAD8:
                    return 1096;
                case KeyCode.NUMPAD9:
                    return 1097;
                case KeyCode.Multiply:
                    return 1079;
                case KeyCode.Add:
                    return 1102;
                case KeyCode.Substract:
                    return 1098;
                case KeyCode.Decimal:
                    return 1107;
                case KeyCode.Divide:
                    return 1205;
                case KeyCode.F1:
                    return 1083;
                case KeyCode.F2:
                    return 1084;
                case KeyCode.F3:
                    return 1085;
                case KeyCode.F4:
                    return 1086;
                case KeyCode.F5:
                    return 1087;
                case KeyCode.F6:
                    return 1088;
                case KeyCode.F7:
                    return 1089;
                case KeyCode.F8:
                    return 1090;
                case KeyCode.F9:
                    return 1091;
                case KeyCode.F10:
                    return 1092;
                case KeyCode.F11:
                    return 1111;
                case KeyCode.F12:
                    return 1112;
                case KeyCode.F13:
                    return 1124;
                case KeyCode.F14:
                    return 1125;
                case KeyCode.F15:
                    return 1126;
                case KeyCode.Numlock:
                    return 1093;
                case KeyCode.Scroll:
                    return 1094;
                case KeyCode.LShift:
                    return 1066;
                case KeyCode.RShift:
                    return 1078;
                case KeyCode.LControl:
                    return 1053;
                case KeyCode.RControl:
                    return 1181;
                case KeyCode.LMenu:
                    return 1080;
                case KeyCode.RMenu:
                    return 1208;
                case KeyCode.Browser_Back:
                    return 1258;
                case KeyCode.Browser_Forward:
                    return 1257;
                case KeyCode.Browser_Refresh:
                    return 1255;
                case KeyCode.Browser_Stop:
                    return 1256;
                case KeyCode.Browser_Search:
                    return 1253;
                case KeyCode.Browser_Favorites:
                    return 1254;
                case KeyCode.Browser_Home:
                    return 1202;
                case KeyCode.Volume_Mute:
                    return 1184;
                case KeyCode.Volume_Down:
                    return 1198;
                case KeyCode.Volume_Up:
                    return 1200;
                case KeyCode.Media_Next_Track:
                    return 1177;
                case KeyCode.Media_Prev_Track:
                    return 1168;
                case KeyCode.Media_Stop:
                    return 1188;
                case KeyCode.Media_Play_Pause:
                    return 1186;
                case KeyCode.Launch_Media_Select:
                    return 1261;
                case KeyCode.OEM_1:
                    return 1063;
                case KeyCode.OEM_Plus:
                    return 1037;
                case KeyCode.OEM_Comma:
                    return 1075;
                case KeyCode.OEM_Minus:
                    return 1036;
                case KeyCode.OEM_Period:
                    return 1076;
                case KeyCode.OEM_2:
                    return 1205;
                case KeyCode.OEM_3:
                    return 1065;
                case KeyCode.OEM_4:
                    return 1050;
                case KeyCode.OEM_5:
                    return 1077;
                case KeyCode.OEM_6:
                    return 1051;
                case KeyCode.OEM_7:
                    return 1064;
                default:
                    return 0;
            }
        }

        #endregion
    }
}