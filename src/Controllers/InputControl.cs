using System;
using System.Threading;
using System.Threading.Tasks;

namespace nucs.Automation.Controllers {
    /// <summary>
    ///     Completely blocks all input, never forget to unblock!
    ///     Any emulator or simulator will be blocked too!
    ///     Use static for async or manual or use this object for disposable pattern.
    ///     Please note that this requires Administrator permissions.
    /// </summary>
    public class InputControl : IDisposable {
        /// <summary>
        ///     Counts the amount of blocks
        /// </summary>
        public static int BlockAdder = 0;
        public static void Block() {
            Interlocked.Increment(ref BlockAdder);
            Native.BlockInput(true);
        }

        public static void Unblock() {
            Interlocked.Decrement(ref BlockAdder);
            var n = Thread.VolatileRead(ref BlockAdder);
            if (n==0)
                Native.BlockInput(false);
        }

        public static Task BlockFor(TimeSpan timespan) {
            return BlockFor(Convert.ToInt32(timespan.TotalMilliseconds));
        }

        public static async Task BlockFor(int milliseconds) {
            Block();
            await Task.Delay(milliseconds);
            Unblock();
        }

        public InputControl() {
            Block();
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose() {
            Unblock();
        }
    }
}