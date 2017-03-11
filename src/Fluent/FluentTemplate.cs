using System;
using System.Drawing;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using nucs.Automation.Controllers;

namespace nucs.Automation.Fluent {
    /// <summary>
    ///     A chain of actions with the invoker that can be executed at request and cloned.
    /// </summary>
    /// <typeparam name="TInvoker"></typeparam>
    public class FluentTemplate<TInvoker> : ICloneable where TInvoker : FluentInvoker {
        public delegate void FluentAction(TInvoker invoker, FluentTemplate<TInvoker> t);

        /// <summary>
        ///     The <see cref="FluentInvoker"/> that will call those methods.
        /// </summary>
        private TInvoker Invoker { get; set; }

        #region CommonInvokerMethods

        public FluentTemplate<TInvoker> LeftDown() {
            Chain((inv, tmp)=> inv.LeftDown());
            return this;
        }

        public FluentTemplate<TInvoker> LeftUp() {
            Chain((inv, tmp)=> inv.LeftUp());
            return this;
        }

        public FluentTemplate<TInvoker> RightDown() {
            Chain((inv, tmp)=> inv.RightDown());
            return this;
        }

        public FluentTemplate<TInvoker> RightUp() {
            Chain((inv, tmp)=> inv.RightUp());
            return this;
        }

        public FluentTemplate<TInvoker> MiddleDown() {
            Chain((inv, tmp)=> inv.MiddleDown());
            return this;
        }

        public FluentTemplate<TInvoker> MiddleUp() {
            Chain((inv, tmp)=> inv.MiddleUp());
            return this;
        }

        public FluentTemplate<TInvoker> WheelDown() {
            Chain((inv, tmp)=> inv.WheelDown());
            return this;
        }

        public FluentTemplate<TInvoker> WheelUp() {
            Chain((inv, tmp)=> inv.WheelUp());
            return this;
        }

        /// <summary>
        ///     Moves the cursor instantly to a given x,y.
        /// </summary>
        public FluentTemplate<TInvoker> AbsoluteMove(int x, int y) {
            Chain((inv, tmp)=> inv.AbsoluteMove(x, y));
            return this;
        }

        /// <summary>
        ///     Moves the cursor instantly to a given x,y.
        /// </summary>
        public FluentTemplate<TInvoker> AbsoluteMove(Point aDestination) {
            Chain((inv, tmp)=> inv.AbsoluteMove(aDestination));
            return this;
        }

        /// <summary>
        ///     Moves cursor in a given velocity, higher is faster (log-n).
        /// </summary>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="aMovementVelocityLogFactor"> higher is faster (log-n).</param>
        public FluentTemplate<TInvoker> Move(int dx, int dy, double aMovementVelocityLogFactor = 1) {
            Chain((inv, tmp)=> inv.Move(dx, dy, aMovementVelocityLogFactor));
            return this;
        }

        /// <summary>
        ///     Moves cursor in a given velocity, higher is faster (log-n).
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="aMovementVelocityLogFactor"> higher is faster (log-n).</param>
        public FluentTemplate<TInvoker> Move(Point destination, double aMovementVelocityLogFactor = 1) {
            Chain((inv, tmp)=> inv.Move(destination, aMovementVelocityLogFactor));
            return this;
        }

        /// <summary>
        ///     Moves cursor in a given velocity, relativly to current position, higher is faster (log-n).
        /// </summary>
        /// <param name="yDisplacement"></param>
        /// <param name="xDisplacement"></param>
        /// <param name="aMovementVelocityLogFactor"> higher is faster (log-n).</param>
        public FluentTemplate<TInvoker> MoveRelative(int xDisplacement, int yDisplacement, double aMovementVelocityLogFactor = 2) {
            Chain((inv, tmp)=> inv.MoveRelative(xDisplacement, yDisplacement, aMovementVelocityLogFactor));
            return this;
        }

        /// <summary>
        ///     Moves cursor in a given velocity, relativly to current position, higher is faster (log-n).
        /// </summary>
        /// <param name="aDisplacement"></param>
        /// <param name="aMovementVelocityLogFactor"> higher is faster (log-n).</param>
        public FluentTemplate<TInvoker> MoveRelative(Point aDisplacement, double aMovementVelocityLogFactor = 2) {
            Chain((inv, tmp)=> inv.MoveRelative(aDisplacement, aMovementVelocityLogFactor));
            return this;
        }

        /// <summary>
        ///     Moves and clicks.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="btn">The specific button to click</param>
        /// <returns></returns>
        public FluentTemplate<TInvoker> MoveClick(int x, int y, MouseButton btn = MouseButton.Left) {
            Chain((inv, tmp)=> inv.MoveClick(x, y, btn));
            return this;
        }

        public FluentTemplate<TInvoker> MoveClick(Point aPoint, MouseButton btn = MouseButton.Left) {
            Chain((inv, tmp)=> inv.MoveClick(aPoint, btn));
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
        public FluentTemplate<TInvoker> MoveClickHold(int x, int y, TimeSpan aWaitPeriod, MouseButton btn = MouseButton.Left) {
            Chain((inv, tmp)=> inv.MoveClickHold(x, y, aWaitPeriod, btn));
            return this;
        }

        /// <summary>
        ///     Moves, presses key down, waits for given time and releases the key up.
        /// </summary>
        /// <param name="aPoint"></param>
        /// <param name="aWaitPeriod">The time to wait between the press down and up</param>
        /// <param name="btn">Which button to click</param>
        /// <returns></returns>
        public FluentTemplate<TInvoker> MoveClickHold(Point aPoint, TimeSpan aWaitPeriod, MouseButton btn = MouseButton.Left) {
            Chain((inv, tmp)=> inv.MoveClickHold(aPoint, aWaitPeriod, btn));
            return this;
        }

        /// <summary>
        ///     Clicks, by default - left button.
        /// </summary>
        public FluentTemplate<TInvoker> Click(MouseButton btn = MouseButton.Left) {
            Chain((inv, tmp)=> inv.Click(btn));
            return this;
        }

        /// <summary>
        ///     Double clicks, by default - left button
        /// </summary>
        public FluentTemplate<TInvoker> DoubleClick(MouseButton btn = MouseButton.Left) {
            Chain((inv, tmp)=> inv.DoubleClick(btn));
            return this;
        }

        public FluentTemplate<TInvoker> MiddleClick() {
            Chain((inv, tmp)=> inv.MiddleClick());
            return this;
        }

        public FluentTemplate<TInvoker> RightClick() {
            Chain((inv, tmp)=> inv.RightClick());
            return this;
        }

        /// <summary>
        ///     moves to (o), presses left, moves to (d), releases left
        /// </summary>
        /// <param name="ox">From x</param>
        /// <param name="oy">From y</param>
        /// <param name="dx">To x</param>
        /// <param name="dy">To y</param>
        public FluentTemplate<TInvoker> DragDrop(int ox, int oy, int dx, int dy) {
            Chain((inv, tmp)=> inv.DragDrop(ox, oy, dx, dy));
            return this;
        }

        /// <summary>
        ///     moves to (o), presses left, moves to (d), releases left
        /// </summary>
        /// <param name="o">From</param>
        /// <param name="d">To</param>
        public FluentTemplate<TInvoker> DragDrop(Point o, Point d) {
            Chain((inv, tmp)=> inv.DragDrop(o, d));
            return this;
        }

        /// <summary>
        ///     Writes down the char that this key represents as if it was through the keyboard. - won't work on Keys like 'End' or 'Backspace' or 'Control'
        /// </summary>
        public FluentTemplate<TInvoker> Write(Keys key) {
            Chain((inv, tmp)=> inv.Write(key));
            return this;
        }

        /// <summary>
        ///     Writes down this string as if it was through the keyboard.
        /// </summary>
        public FluentTemplate<TInvoker> Write(string text) {
            Chain((inv, tmp)=> inv.Write(text));
            return this;
        }

        /// <summary>
        ///     Writes down the char that this key represents as if it was through the keyboard. - won't work on Keys like 'End' or 'Backspace' or 'Control'
        /// </summary>
        public FluentTemplate<TInvoker> Write(KeyCode keycode) {
            Chain((inv, tmp)=> inv.Write(keycode));
            return this;
        }

        /// <summary>
        ///     Writes down the char that this key represents as if it was through the keyboard.
        /// </summary>
        public FluentTemplate<TInvoker> Write(char @char) {
            Chain((inv, tmp)=> inv.Write(@char));
            return this;
        }

        /// <summary>
        ///     Writes down the characters as if it was through the keyboard.
        /// </summary>
        public FluentTemplate<TInvoker> Write(params char[] chars) {
            Chain((inv, tmp)=> inv.Write(chars));
            return this;
        }

        /// <summary>
        ///     Writes down the characters as if it was through the keyboard.
        /// </summary>
        public FluentTemplate<TInvoker> Write(int utf32) {
            Chain((inv, tmp)=> inv.Write(utf32));
            return this;
        }

        /// <summary>
        ///     Presses down this keycode.
        /// </summary>
        public FluentTemplate<TInvoker> Down(KeyCode keycode) {
            Chain((inv, tmp)=> inv.Down(keycode));
            return this;
        }

        /// <summary>
        ///     Releases this keycode.
        /// </summary>
        public FluentTemplate<TInvoker> Up(KeyCode keycode) {
            Chain((inv, tmp)=> inv.Up(keycode));
            return this;
        }

        /// <summary>
        ///     Presses down and releases this keycode with the given delay between them
        /// </summary>
        /// <param name="keycode">The keycode to press</param>
        /// <param name="delay">The delay between the actions in milliseconds</param>
        public FluentTemplate<TInvoker> Press(KeyCode keycode, uint delay = 20) {
            Chain((inv, tmp)=> inv.Press(keycode, delay));
            return this;
        }

        public FluentTemplate<TInvoker> Enter() {
            Chain((inv, tmp)=> inv.Enter());
            return this;
        }

        public FluentTemplate<TInvoker> Back() {
            Chain((inv, tmp)=> inv.Back());
            return this;
        }

        public FluentTemplate<TInvoker> Alt(KeyCode keycode) {
            Chain((inv, tmp)=> inv.Alt(keycode));
            return this;
        }

        public FluentTemplate<TInvoker> Shift(KeyCode keycode) {
            Chain((inv, tmp)=> inv.Shift(keycode));
            return this;
        }

        public FluentTemplate<TInvoker> Control(KeyCode keycode) {
            Chain((inv, tmp)=> inv.Control(keycode));
            return this;
        }

        public FluentTemplate<TInvoker> Win(KeyCode keycode) {
            Chain((inv, tmp)=> inv.Win(keycode));
            return this;
        }

        public FluentTemplate<TInvoker> PressAsync(KeyCode keycode, uint delay = 20) {
            Chain((inv, tmp)=> inv.PressAsync(keycode, delay));
            return this;
        }

        #endregion

        #region Invoke

        /// <summary>Gets the method represented by the delegate.</summary>
        /// <returns>A <see cref="T:System.Reflection.MethodInfo" /> describing the method represented by the delegate.</returns>
        /// <exception cref="T:System.MemberAccessException">The caller does not have access to the method represented by the delegate (for example, if the method is private). </exception>
        public MethodInfo Method => chain.Method;

        public void Invoke() {
            chain.Invoke(Invoker, this);
        }

        public Task InvokeAsync() {
            return Task.Run(() => Invoke());
        }

        public IAsyncResult BeginInvoke(AsyncCallback callback, object @object) {
            return chain.BeginInvoke(Invoker,this, callback, @object);
        }

        public void EndInvoke(IAsyncResult result) {
            chain.EndInvoke(result);
        }

        #endregion

        public FluentTemplate(TInvoker invoker) {
            if (invoker == null) throw new ArgumentNullException(nameof(invoker));
            Invoker = invoker;
        }

        #region Building

        /// <summary>
        ///     Clears/Resets the chain of actions.
        /// </summary>
        public void Clear() {
            this.chain = null;
        }

        public FluentTemplate<TInvoker> ChangeInvoker(TInvoker invoker) {
            if (invoker == null) throw new ArgumentNullException(nameof(invoker));
            _chain((inv,tmp) => tmp.Invoker = invoker);
            return this;
        }

        /// <summary>
        ///     Will temporarly change the <see cref="FluentInvoker"/> for the passed given actions
        /// </summary>
        /// <param name="actions"></param>
        /// <param name="invoker"></param>
        /// <returns></returns>
        public FluentTemplate<TInvoker> ChangeTemporarlyInvoker(FluentAction actions, TInvoker invoker) {
            if (invoker == null) throw new ArgumentNullException(nameof(invoker));
            _chain((inv, tmp) => {
                var last = tmp.Invoker;
                tmp.Invoker = invoker;
                actions(invoker,tmp);
                tmp.Invoker = last;
            });
            return this;
        }

        /// <summary>
        ///     Add an action manually.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="append">True (default) will add this action to the end, false to the start.</param>
        /// <returns></returns>
        public FluentTemplate<TInvoker> Chain(FluentAction action, bool append = true) {
            if (action == null) throw new ArgumentNullException(nameof(action));
            _chain(action, append);
            return this;
        }

        private void _chain(FluentAction action, bool append = true) {
            if (chain == null) {
                chain = action;
                return;
            }
            var localchain = chain;
            if (append)
                chain = (inv, tmp) => {
                    localchain(inv, tmp);
                    action(inv, tmp);
                };
            else
                chain = (inv, tmp) => {
                    action(inv, tmp);
                    localchain(inv, tmp);
                };
        }

        #endregion

        #region Chaining

        private FluentAction chain = null;

        #region Implementation of ICloneable

        /// <summary>Creates a new object that is a copy of the current instance.</summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone() {
            return new FluentTemplate<TInvoker>(this.Invoker) {chain = (FluentAction) chain.Clone()};
        }

        public FluentTemplate<TInvoker> CloneTemplate() {
            return (FluentTemplate<TInvoker>) Clone();
        }

        #endregion

        #endregion
    }
}