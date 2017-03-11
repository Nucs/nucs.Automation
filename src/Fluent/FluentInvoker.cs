using System;
using System.Drawing;
using System.Runtime.Serialization;
using System.Windows.Forms;
using nucs.Automation.Controllers;

namespace nucs.Automation.Fluent {
    public class FluentInvoker {

        /// <summary>
        ///     Creates a new instance with default controllers.
        /// </summary>
        /// <returns></returns>
        public static FluentInvoker CreateDefault() {
            return new FluentInvoker(new MouseController(), new KeyboardController());
        }

        public IMouseController MouseController { get; set; }
        public IExtendedKeyboardController KeyboardController { get; set; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public FluentInvoker(IMouseController mouseController, IExtendedKeyboardController keyboardController) {
            MouseController = mouseController;
            KeyboardController = keyboardController;
        }

        #region Mouse

        public FluentInvoker LeftDown() {
            MouseController.LeftDown();
            return this;
        }

        public FluentInvoker LeftUp() {
            MouseController.LeftUp();
            return this;
        }

        public FluentInvoker RightDown() {
            MouseController.RightDown();
            return this;
        }

        public FluentInvoker RightUp() {
            MouseController.RightUp();
            return this;
        }

        public FluentInvoker MiddleDown() {
            MouseController.MiddleDown();
            return this;
        }

        public FluentInvoker MiddleUp() {
            MouseController.MiddleUp();
            return this;
        }

        public FluentInvoker WheelDown() {
            MouseController.WheelDown();
            return this;
        }

        public FluentInvoker WheelUp() {
            MouseController.WheelUp();
            return this;
        }

        /// <summary>
        ///     Moves the cursor instantly to a given x,y.
        /// </summary>
        public FluentInvoker AbsoluteMove(int x, int y) {
            MouseController.AbsoluteMove(x, y);
            return this;
        }

        /// <summary>
        ///     Moves the cursor instantly to a given x,y.
        /// </summary>
        public FluentInvoker AbsoluteMove(Point aDestination) {
            MouseController.AbsoluteMove(aDestination);
            return this;
        }

        /// <summary>
        ///     Moves cursor in a given velocity, higher is faster (log-n).
        /// </summary>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="aMovementVelocityLogFactor"> higher is faster (log-n).</param>
        public FluentInvoker Move(int dx, int dy, double aMovementVelocityLogFactor = 1) {
            MouseController.Move(dx, dy, aMovementVelocityLogFactor).Wait();
            return this;
        }

        /// <summary>
        ///     Moves cursor in a given velocity, higher is faster (log-n).
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="aMovementVelocityLogFactor"> higher is faster (log-n).</param>
        public FluentInvoker Move(Point destination, double aMovementVelocityLogFactor = 1) {
            MouseController.Move(destination, aMovementVelocityLogFactor).Wait();
            return this;
        }

        /// <summary>
        ///     Moves cursor in a given velocity, relativly to current position, higher is faster (log-n).
        /// </summary>
        /// <param name="yDisplacement"></param>
        /// <param name="xDisplacement"></param>
        /// <param name="aMovementVelocityLogFactor"> higher is faster (log-n).</param>
        public FluentInvoker MoveRelative(int xDisplacement, int yDisplacement, double aMovementVelocityLogFactor = 2) {
            MouseController.MoveRelative(xDisplacement, yDisplacement, aMovementVelocityLogFactor).Wait();
            return this;
        }

        /// <summary>
        ///     Moves cursor in a given velocity, relativly to current position, higher is faster (log-n).
        /// </summary>
        /// <param name="aDisplacement"></param>
        /// <param name="aMovementVelocityLogFactor"> higher is faster (log-n).</param>
        public FluentInvoker MoveRelative(Point aDisplacement, double aMovementVelocityLogFactor = 2) {
            MouseController.MoveRelative(aDisplacement, aMovementVelocityLogFactor).Wait();
            return this;
        }

        /// <summary>
        ///     Moves and clicks.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="btn">The specific button to click</param>
        /// <returns></returns>
        public FluentInvoker MoveClick(int x, int y, MouseButton btn = MouseButton.Left) {
            MouseController.MoveClick(x, y, btn).Wait();
            return this;
        }

        public FluentInvoker MoveClick(Point aPoint, MouseButton btn = MouseButton.Left) {
            MouseController.MoveClick(aPoint, btn).Wait();
            return this;
        }

        /// <summary>
        ///     Moves, presses key down, waits for given time and releases the key up.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="aWaitPeriod">The time to wait between the press down and up</param>
        /// <param name="btn">Which button to click</param>
        /// <returns></returns>
        public FluentInvoker MoveClickHold(int x, int y, TimeSpan aWaitPeriod, MouseButton btn = MouseButton.Left) {
            MouseController.MoveClickHold(x, y, aWaitPeriod, btn).Wait();
            return this;
        }

        /// <summary>
        ///     Moves, presses key down, waits for given time and releases the key up.
        /// </summary>
        /// <param name="aPoint"></param>
        /// <param name="aWaitPeriod">The time to wait between the press down and up</param>
        /// <param name="btn">Which button to click</param>
        /// <returns></returns>
        public FluentInvoker MoveClickHold(Point aPoint, TimeSpan aWaitPeriod, MouseButton btn = MouseButton.Left) {
            MouseController.MoveClickHold(aPoint, aWaitPeriod, btn).Wait();
            return this;
        }

        /// <summary>
        ///     Clicks, by default - left button.
        /// </summary>
        public FluentInvoker Click(MouseButton btn = MouseButton.Left) {
            MouseController.Click(btn).Wait();
            return this;
        }

        /// <summary>
        ///     Double clicks, by default - left button
        /// </summary>
        public FluentInvoker DoubleClick(MouseButton btn = MouseButton.Left) {
            MouseController.DoubleClick(btn).Wait();
            return this;
        }

        public FluentInvoker MiddleClick() {
            MouseController.MiddleClick().Wait();
            return this;
        }

        public FluentInvoker RightClick() {
            MouseController.RightClick().Wait();
            return this;
        }

        /// <summary>
        ///     moves to (o), presses left, moves to (d), releases left
        /// </summary>
        /// <param name="ox">From x</param>
        /// <param name="oy">From y</param>
        /// <param name="dx">To x</param>
        /// <param name="dy">To y</param>
        public FluentInvoker DragDrop(int ox, int oy, int dx, int dy) {
            MouseController.DragDrop(ox, oy, dx, dy).Wait();
            return this;
        }

        /// <summary>
        ///     moves to (o), presses left, moves to (d), releases left
        /// </summary>
        /// <param name="o">From</param>
        /// <param name="d">To</param>
        public FluentInvoker DragDrop(Point o, Point d) {
            MouseController.DragDrop(o, d).Wait();
            return this;
        }

        #endregion

        #region Keyboard

        /// <summary>
        ///     Writes down the char that this key represents as if it was through the keyboard. - won't work on Keys like 'End' or 'Backspace' or 'Control'
        /// </summary>
        public FluentInvoker Write(Keys key) {
            KeyboardController.Write(key);
            return this;
        }

        /// <summary>
        ///     Writes down this string as if it was through the keyboard.
        /// </summary>
        public FluentInvoker Write(string text) {
            KeyboardController.Write(text);
            return this;
        }

        /// <summary>
        ///     Writes down the char that this key represents as if it was through the keyboard. - won't work on Keys like 'End' or 'Backspace' or 'Control'
        /// </summary>
        public FluentInvoker Write(KeyCode keycode) {
            KeyboardController.Write(keycode);
            return this;
        }

        /// <summary>
        ///     Writes down the char that this key represents as if it was through the keyboard.
        /// </summary>
        public FluentInvoker Write(char @char) {
            KeyboardController.Write(@char);
            return this;
        }

        /// <summary>
        ///     Writes down the characters as if it was through the keyboard.
        /// </summary>
        public FluentInvoker Write(params char[] chars) {
            KeyboardController.Write(chars);
            return this;
        }

        /// <summary>
        ///     Writes down the characters as if it was through the keyboard.
        /// </summary>
        public FluentInvoker Write(int utf32) {
            KeyboardController.Write(utf32);
            return this;
        }

        /// <summary>
        ///     Presses down this keycode.
        /// </summary>
        public FluentInvoker Down(KeyCode keycode) {
            KeyboardController.Down(keycode);
            return this;
        }

        /// <summary>
        ///     Releases this keycode.
        /// </summary>
        public FluentInvoker Up(KeyCode keycode) {
            KeyboardController.Up(keycode);
            return this;
        }

        /// <summary>
        ///     Presses down and releases this keycode with the given delay between them
        /// </summary>
        /// <param name="keycode">The keycode to press</param>
        /// <param name="delay">The delay between the actions in milliseconds</param>
        public FluentInvoker Press(KeyCode keycode, uint delay = 20) {
            KeyboardController.Press(keycode, delay);
            return this;
        }

        public FluentInvoker Enter() {
            KeyboardController.Enter();
            return this;
        }

        public FluentInvoker Back() {
            KeyboardController.Back();
            return this;
        }

        public FluentInvoker Alt(KeyCode keycode) {
            KeyboardController.Alt(keycode);
            return this;
        }

        public FluentInvoker Shift(KeyCode keycode) {
            KeyboardController.Shift(keycode);
            return this;
        }

        public FluentInvoker Control(KeyCode keycode) {
            KeyboardController.Control(keycode);
            return this;
        }

        public FluentInvoker Win(KeyCode keycode) {
            KeyboardController.Win(keycode);
            return this;
        }

        public FluentInvoker PressAsync(KeyCode keycode, uint delay = 20) {
            KeyboardController.PressAsync(keycode, delay);
            return this;
        }

        #endregion

        public static FluentTemplate<TInvoker> CreateTemplate<TInvoker>(TInvoker invoker) where TInvoker : FluentInvoker {
            return new FluentTemplate<TInvoker>(invoker);
        }

    }
}