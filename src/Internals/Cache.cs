using System;
using System.Threading;
using Timer = System.Timers.Timer;

namespace nucs.Automation.Internals {
    internal class Cache<T> : IDisposable {
        public static Cache<T> Minute(Func<T> update, bool constantly_update) {
            return new Cache<T>(update, 60 * 1000, constantly_update);
        }

        public static Cache<T> Hour(Func<T> update, bool constantly_update) {
            return new Cache<T>(update, 60 * 60 * 1000, constantly_update);
        }

        public static Cache<T> FiveSeconds(Func<T> update, bool constantly_update) {
            return new Cache<T>(update, 5 * 1000, constantly_update);
        }

        public static Cache<T> Seconds(Func<T> update, uint seconds, bool constantly_update) {
            return new Cache<T>(update, seconds * 1000, constantly_update);
        }

        public static Cache<T> Minutes(Func<T> update, uint minutes, bool constantly_update) {
            return new Cache<T>(update, minutes * 60 * 1000, constantly_update);
        }

        public static Cache<T> Hours(Func<T> update, uint hours, bool constantly_update) {
            return new Cache<T>(update, hours * 60 * 60 * 1000, constantly_update);
        }

        /// <summary>
        ///     Once the cachetimeout passes, should it always refresh without asking or wait for first access.
        /// </summary>
        public readonly bool ConstantlyUpdate;

        private readonly object sync = new object();

        /// <summary>
        ///     The update method that retrieves the item
        /// </summary>
        public readonly Func<T> Update;

        private T _obj;

        /// <summary>
        ///     For how long the cached object is relevant, in milliseconds
        /// </summary>
        public uint CacheTimeout {
            get { return _cacheTimeout; }
            set {
                if (_timer != null)
                    throw new InvalidOperationException("Cant change CacheTimeout on non-ConstantlyUpdated cache object");
                _cacheTimeout = value;
            }
        }

        private bool stopflag;
        private readonly CountdownTimer _timer;
        private uint _cacheTimeout;
        private Exception laste;

        /// <summary>
        /// </summary>
        /// <param name="update">The update method that retrieves the item</param>
        /// <param name="cachetimeout">For how long the cached object is relevant, in milliseconds</param>
        /// <param name="constantly_update">
        ///     Once the cachetimeout passes, should it always refresh without asking or wait for first
        ///     access.
        /// </param>
        public Cache(Func<T> update, uint cachetimeout, bool constantly_update) : this(default(T), update, cachetimeout, constantly_update) {}

        /// <summary>
        /// </summary>
        /// <param name="value">Default value to begin with untill next refresh</param>
        /// <param name="update">The update method that retrieves the item</param>
        /// <param name="cachetimeout">For how long the cached object is relevant, in milliseconds</param>
        /// <param name="constantly_update">
        ///     Once the cachetimeout passes, should it always refresh without asking or wait for first
        ///     access.
        /// </param>
        public Cache(T value, Func<T> update, uint cachetimeout, bool constantly_update) {
            ConstantlyUpdate = constantly_update;
            Update = () => {
                try {
                    return update();
                } catch (Exception e) {
                    laste = e;
                    return default(T);
                }
            };
            _cacheTimeout = cachetimeout;
            _obj = value;

            if (ConstantlyUpdate)
                new Thread(Updater).Start();
            else
                _timer = new CountdownTimer((int) cachetimeout);
        }

        public T Object {
            get {
                if (laste != null) {
                    var local = laste;
                    laste = null;
                    throw local;
                }
                if (ConstantlyUpdate) {
                    lock (sync)
                        return _obj;
                } else {
                    if (_timer.Working) {
                        return _obj;
                    } else {
                        _timer.Reset();
                        _timer.Start();
                        return _obj = Update();
                    }
                }
            }
            private set { _obj = value; }
        }

        public void Dispose() {
            stopflag = true;
        }

        public static implicit operator T(Cache<T> m) {
            if (m.laste != null) {
                var local = m.laste;
                m.laste = null;
                throw local;
            }
            return m.Object;
        }

        private void Updater() {
            while (true) {
                Thread.Sleep((int) CacheTimeout);
                if (stopflag)
                    break;
                lock (sync) {
                    try {
                        Object = Update();
                    } catch {
                        Object = default(T);
                    }
                }
            }
        }

        private class CountdownTimer {
            public event Action Elapsed;

            public bool Working {
                get { return t.Enabled; }
            }

            public bool Enabled {
                get { return enabled; }
            }

            private DateTime? starttime;

            public int Interval {
                get { return _interval; }
                set {
                    _interval = value;
                    enabled = value > 0;
                    if (value <= 0) {
                        t.Interval = 1;
                        return;
                    }

                    t.Interval = value;
                }
            }

            internal bool enabled = false;
            private int _interval;
            private readonly System.Timers.Timer t;

            public CountdownTimer(int interval, bool start = false) {
                _interval = interval;
                enabled = interval > 0;
                t = new Timer(interval <= 0 ? 1 : interval) {AutoReset = false, Enabled = false};

                t.Elapsed += (sender, args) => {
                    Stop();
                    _wait_sem.Release(100);
                    Elapsed?.Invoke();
                };

                if (start)
                    Start();
            }

            public void Start() {
                starttime = DateTime.Now;
                t.Start();
            }

            private readonly SemaphoreSlim _wait_sem = new SemaphoreSlim(0);

            public void Wait() {
                if (t.Enabled)
                    _wait_sem.Wait();
            }

            public void Stop() {
                t.Stop();
                starttime = null;
                enabled = false;
            }

            /// <summary>
            /// Resets/restarts
            /// </summary>
            public void Reset() {
                Stop();
                Start();
            }
        }
    }
}