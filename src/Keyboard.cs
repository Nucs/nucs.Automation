using System.Windows.Forms;
using nucs.Automation.Controllers;

namespace nucs.Automation {

    /// <summary>
    ///     Performs various actions with a keyboard by instance of <see cref="KeyboardController"/>
    /// </summary>
    public class Keyboard {
        private static readonly KeyboardController _controller = new KeyboardController();

        /// <summary>
        ///     Writes down the char that this key represents as if it was through the keyboard. - won't work on Keys like 'End' or 'Backspace' or 'Control'
        /// </summary>
        public static void Write(Keys key) {
            _controller.Write(key);
        }

        /// <summary>
        ///     Writes down this string as if it was through the keyboard.
        /// </summary>
        public static void Write(string text) {
            _controller.Write(text);
        }

        /// <summary>
        ///     Writes down the char that this key represents as if it was through the keyboard. - won't work on Keys like 'End' or 'Backspace' or 'Control'
        /// </summary>
        public static void Write(KeyCode keycode) {
            _controller.Write(keycode);
        }

        /// <summary>
        ///     Writes down the char that this key represents as if it was through the keyboard.
        /// </summary>
        public static void Write(char @char) {
            _controller.Write(@char);
        }

        /// <summary>
        ///     Writes down the characters as if it was through the keyboard.
        /// </summary>
        public static void Write(int utf32) {
            _controller.Write(utf32);
        }

        /// <summary>
        ///     Writes down the characters as if it was through the keyboard.
        /// </summary>
        public static void Write(params char[] chars) {
            _controller.Write(chars);
        }

        /// <summary>Presses down this keycode.</summary>
        public static void Down(KeyCode keycode) {
            _controller.Down(keycode);
        }

        /// <summary>Releases this keycode.</summary>
        public static void Up(KeyCode keycode) {
            _controller.Up(keycode);
        }

        /// <summary>
        ///     Presses down and releases this keycode with the given delay between them
        /// </summary>
        /// <param name="keycode">The keycode to press</param>
        /// <param name="delay">The delay between the actions in milliseconds</param>
        public static void Press(KeyCode keycode, uint delay = 20) {
            _controller.Press(keycode, delay);
        }

        public static void PressAsync(KeyCode keycode, uint delay = 20) {
            _controller.PressAsync(keycode, delay);
        }

        public static void Enter() {
            _controller.Enter();
        }

        public static void Back() {
            _controller.Back();
        }

        public static void Control(KeyCode keycode) {
            _controller.Control(keycode);
        }

        public static void Win(KeyCode keycode) {
            _controller.Win(keycode);
        }

        public static void Shift(KeyCode keycode) {
            _controller.Shift(keycode);
        }

        public static void Alt(KeyCode keycode) {
            _controller.Alt(keycode);
        }

        public static void Window(KeyCode keycode) {
            _controller.Window(keycode);
        }
    }
}