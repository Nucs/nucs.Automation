using System.Windows.Forms;

namespace nucs.Automation.Controllers {


    /// <summary>
    ///     The basic implementation of a keyboard where you can press a button and write text.
    /// </summary>
    public interface IKeyboard {
        /// <summary>
        ///     Writes down the char that this key represents as if it was through the keyboard. - won't work on Keys like 'End' or 'Backspace' or 'Control'
        /// </summary>
        void Write(Keys key);
        /// <summary>
        ///     Writes down this string as if it was through the keyboard.
        /// </summary>
        void Write(string text);
        /// <summary>
        ///     Writes down the char that this key represents as if it was through the keyboard. - won't work on Keys like 'End' or 'Backspace' or 'Control'
        /// </summary>
        void Write(KeyCode keycode);
        /// <summary>
        ///     Writes down the char that this key represents as if it was through the keyboard.
        /// </summary>
        void Write(char @char);
        /// <summary>
        ///     Writes down the characters as if it was through the keyboard.
        /// </summary>
        void Write(params char[] chars);
        /// <summary>
        ///     Writes down the characters as if it was through the keyboard.
        /// </summary>
        void Write(int utf32);

        /// <summary>
        ///     Presses down this keycode.
        /// </summary>
        void Down(KeyCode keycode);
        /// <summary>
        ///     Releases this keycode.
        /// </summary>
        void Up(KeyCode keycode);
        /// <summary>
        ///     Presses down and releases this keycode with the given delay between them
        /// </summary>
        /// <param name="keycode">The keycode to press</param>
        /// <param name="delay">The delay between the actions in milliseconds</param>
        void Press(KeyCode keycode, uint delay=20);
    }
}