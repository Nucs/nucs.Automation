using System;
using System.Drawing;
using System.Threading.Tasks;

namespace nucs.Automation.Controllers {
    public interface IMouseController {
        /// <summary>
        ///     The delay between clicks, e.g. on <see cref="MoveClick(int,int,nucs.Automation.Controllers.MouseButton)"/> requires a small time between actions, it'll be it.
        /// </summary>
        int CommonDelay { get; }

        void LeftDown();
        void LeftUp();
        void RightDown();
        void RightUp();
        void MiddleDown();
        void MiddleUp();
        void WheelDown();
        void WheelUp();

        /// <summary>
        ///     Moves the cursor instantly to a given x,y.
        /// </summary>
        void AbsoluteMove(int x, int y);

        /// <summary>
        ///     Moves the cursor instantly to a given x,y.
        /// </summary>
        void AbsoluteMove(Point aDestination);

        /// <summary>
        ///     Moves cursor in a given velocity, higher is faster (log-n).
        /// </summary>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="aMovementVelocityLogFactor"> higher is faster (log-n).</param>
        Task Move(int dx, int dy, double aMovementVelocityLogFactor = 1.0);

        /// <summary>
        ///     Moves cursor in a given velocity, higher is faster (log-n).
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="aMovementVelocityLogFactor"> higher is faster (log-n).</param>
        Task Move(Point destination, double aMovementVelocityLogFactor = 1);

        /// <summary>
        ///     Moves cursor in a given velocity, relativly to current position, higher is faster (log-n).
        /// </summary>
        /// <param name="yDisplacement"></param>
        /// <param name="xDisplacement"></param>
        /// <param name="aMovementVelocityLogFactor"> higher is faster (log-n).</param>
        Task MoveRelative(int xDisplacement, int yDisplacement, double aMovementVelocityLogFactor = 2);

        /// <summary>
        ///     Moves cursor in a given velocity, relativly to current position, higher is faster (log-n).
        /// </summary>
        /// <param name="aDisplacement"></param>
        /// <param name="aMovementVelocityLogFactor"> higher is faster (log-n).</param>
        Task MoveRelative(Point aDisplacement, double aMovementVelocityLogFactor = 2);

        /// <summary>
        ///     Moves and clicks.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="btn">The specific button to click</param>
        /// <returns></returns>
        Task MoveClick(int x, int y, MouseButton btn = MouseButton.Left);

        Task MoveClick(Point aPoint, MouseButton btn = MouseButton.Left);

        /// <summary>
        ///     Moves, presses key down, waits for given time and releases the key up.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="aWaitPeriod">The time to wait between the press down and up</param>
        /// <param name="btn">Which button to click</param>
        /// <returns></returns>
        Task MoveClickHold(int x, int y, TimeSpan aWaitPeriod, MouseButton btn = MouseButton.Left);

        /// <summary>
        ///     Moves, presses key down, waits for given time and releases the key up.
        /// </summary>
        /// <param name="aPoint"></param>
        /// <param name="aWaitPeriod">The time to wait between the press down and up</param>
        /// <param name="btn">Which button to click</param>
        /// <returns></returns>
        Task MoveClickHold(Point aPoint, TimeSpan aWaitPeriod, MouseButton btn = MouseButton.Left);

        /// <summary>
        ///     Clicks, by default - left button.
        /// </summary>
        Task Click(MouseButton btn = MouseButton.Left);

        /// <summary>
        ///     Double clicks, by default - left button
        /// </summary>
        Task DoubleClick(MouseButton btn = MouseButton.Left);

        Task MiddleClick();

        Task RightClick();

        /// <summary>
        ///     moves to (o), presses left, moves to (d), releases left
        /// </summary>
        /// <param name="ox">From x</param>
        /// <param name="oy">From y</param>
        /// <param name="dx">To x</param>
        /// <param name="dy">To y</param>
        Task DragDrop(int ox, int oy, int dx, int dy);

        /// <summary>
        ///     moves to (o), presses left, moves to (d), releases left
        /// </summary>
        /// <param name="o">From</param>
        /// <param name="d">To</param>
        Task DragDrop(Point o, Point d);
    }
}