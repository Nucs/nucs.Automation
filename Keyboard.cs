using System.Windows.Forms;
using nucs.Automation.Controllers;

namespace nucs.Automation {

    #region Keyboard

    /// <summary>
    ///     Performs various actions with a keyboard
    /// </summary>
    public class Keyboard {
        private static readonly KeyboardController controller = new KeyboardController();

        /// <summary>
        ///     Writes down the char that this key represents as if it was through the keyboard. - won't work on Keys like 'End' or 'Backspace' or 'Control'
        /// </summary>
        public static void Write(Keys key) {
            controller.Write(key);
        }

        /// <summary>
        ///     Writes down this string as if it was through the keyboard.
        /// </summary>
        public static void Write(string text) {
            controller.Write(text);
        }

        /// <summary>
        ///     Writes down the char that this key represents as if it was through the keyboard. - won't work on Keys like 'End' or 'Backspace' or 'Control'
        /// </summary>
        public static void Write(KeyCode keycode) {
            controller.Write(keycode);
        }

        /// <summary>
        ///     Writes down the char that this key represents as if it was through the keyboard.
        /// </summary>
        public static void Write(char @char) {
            controller.Write(@char);
        }

        /// <summary>
        ///     Writes down the characters as if it was through the keyboard.
        /// </summary>
        public static void Write(int utf32) {
            controller.Write(utf32);
        }

        /// <summary>
        ///     Writes down the characters as if it was through the keyboard.
        /// </summary>
        public static void Write(params char[] chars) {
            controller.Write(chars);
        }

        /// <summary>Presses down this keycode.</summary>
        public static void Down(KeyCode keycode) {
            controller.Down(keycode);
        }

        /// <summary>Releases this keycode.</summary>
        public static void Up(KeyCode keycode) {
            controller.Up(keycode);
        }

        /// <summary>
        ///     Presses down and releases this keycode with the given delay between them
        /// </summary>
        /// <param name="keycode">The keycode to press</param>
        /// <param name="delay">The delay between the actions in milliseconds</param>
        public static void Press(KeyCode keycode, uint delay = 20) {
            controller.Press(keycode, delay);
        }

        public static void PressAsync(KeyCode keycode, uint delay = 20) {
            controller.PressAsync(keycode, delay);
        }

        public static void Enter() {
            controller.Enter();
        }

        public static void Back() {
            controller.Back();
        }

        public static void Control(KeyCode keycode) {
            controller.Control(keycode);
        }

        public static void Win(KeyCode keycode) {
            controller.Win(keycode);
        }

        public static void Shift(KeyCode keycode) {
            controller.Shift(keycode);
        }

        public static void Alt(KeyCode keycode) {
            controller.Alt(keycode);
        }

        public static void Window(KeyCode keycode) {
            controller.Window(keycode);
        }
    }

    #endregion
}