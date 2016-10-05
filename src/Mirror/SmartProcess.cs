using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using nucs.Automation.Internals;

namespace nucs.Automation.Mirror {
    [DebuggerDisplay("{ProcessName} - {MainWindow.Title}")]
    public class SmartProcess {

        /// <summary>
        ///     Unique instance identifier.
        /// </summary>
        public readonly Guid GUID = Guid.NewGuid();

        /// <summary>
        ///     Original Process
        /// </summary>
        public Process OGProcess { get; set; }

        private SmartProcess(Process ogProcess) {
            OGProcess = ogProcess;
            bool hasexited = false;
            try {
                hasexited = ogProcess.HasExited;
            } catch (Win32Exception) {
                hasexited = Process.GetProcesses().FirstOrDefault(p=>p.Id== ogProcess.Id) == null;
            }
            if (hasexited)
                _exitWaiter.Set();
            else
                OGProcess.Exited += (sender, args) => _exitWaiter.Set();
        }

        public IEnumerable<Window> Windows {
            get { return this.GetWindows(); }
        }

        public Window MainWindow => Window.Create(this, MainWindowHandle);

        /// <summary>
        ///     Sets this window as ForegroundWindow, basiclly set focus on it and bring to the front
        /// </summary>
        /// <returns>If returns false meaning it can be set to foreground</returns>
        public bool BringToFront() => Native.SetForegroundWindow(MainWindowHandle);

        #region Delegating Process 

        public event EventHandler Disposed {
            add { OGProcess.Disposed += value; }
            remove { OGProcess.Disposed -= value; }
        }

        /// <summary>Closes a process that has a user interface by sending a close message to its main window.</summary>
        /// <returns>true if the close message was successfully sent; false if the associated process does not have a main window or if the main window is disabled (for example if a modal dialog is being shown).</returns>
        /// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Windows Me); set the <see cref="P:System.Diagnostics.ProcessStartInfo.UseShellExecute" /> property to false to access this property on Windows 98 and Windows Me.</exception>
        /// <exception cref="T:System.InvalidOperationException">The process has already exited. -or-No process is associated with this <see cref="T:System.Diagnostics.Process" /> object.</exception>
        public bool CloseMainWindow() {
            return OGProcess.CloseMainWindow();
        }

        /// <summary>Frees all the resources that are associated with this component.</summary>
        public void Close() {
            OGProcess.Close();
        }

        /// <summary>Discards any information about the associated process that has been cached inside the process component.</summary>
        public void Refresh() {
            OGProcess.Refresh();
        }

        /// <summary>Starts (or reuses) the process resource that is specified by the <see cref="P:System.Diagnostics.Process.StartInfo" /> property of this <see cref="T:System.Diagnostics.Process" /> component and associates it with the component.</summary>
        /// <returns>true if a process resource is started; false if no new process resource is started (for example, if an existing process is reused).</returns>
        /// <exception cref="T:System.InvalidOperationException">No file name was specified in the <see cref="T:System.Diagnostics.Process" /> component's <see cref="P:System.Diagnostics.Process.StartInfo" />.-or- The <see cref="P:System.Diagnostics.ProcessStartInfo.UseShellExecute" /> member of the <see cref="P:System.Diagnostics.Process.StartInfo" /> property is true while <see cref="P:System.Diagnostics.ProcessStartInfo.RedirectStandardInput" />, <see cref="P:System.Diagnostics.ProcessStartInfo.RedirectStandardOutput" />, or <see cref="P:System.Diagnostics.ProcessStartInfo.RedirectStandardError" /> is true. </exception>
        /// <exception cref="T:System.ComponentModel.Win32Exception">There was an error in opening the associated file. </exception>
        /// <exception cref="T:System.ObjectDisposedException">The process object has already been disposed. </exception>
        public bool Start() {
            return OGProcess.Start();
        }

        /// <summary>Immediately stops the associated process.</summary>
        /// <exception cref="T:System.ComponentModel.Win32Exception">The associated process could not be terminated. -or-The process is terminating.-or- The associated process is a Win16 executable.</exception>
        /// <exception cref="T:System.NotSupportedException">You are attempting to call <see cref="M:System.Diagnostics.Process.Kill" /> for a process that is running on a remote computer. The method is available only for processes running on the local computer.</exception>
        /// <exception cref="T:System.InvalidOperationException">The process has already exited. -or-There is no process associated with this <see cref="T:System.Diagnostics.Process" /> object.</exception>
        public void Kill() {
            OGProcess.Kill();
        }

        /// <summary>Instructs the <see cref="T:System.Diagnostics.Process" /> component to wait the specified number of milliseconds for the associated process to exit.</summary>
        /// <returns>true if the associated process has exited; otherwise, false.</returns>
        /// <param name="milliseconds">The amount of time, in milliseconds, to wait for the associated process to exit. The maximum is the largest possible value of a 32-bit integer, which represents infinity to the operating system. </param>
        /// <exception cref="T:System.ComponentModel.Win32Exception">The wait setting could not be accessed. </exception>
        /// <exception cref="T:System.SystemException">No process <see cref="P:System.Diagnostics.Process.Id" /> has been set, and a <see cref="P:System.Diagnostics.Process.Handle" /> from which the <see cref="P:System.Diagnostics.Process.Id" /> property can be determined does not exist.-or- There is no process associated with this <see cref="T:System.Diagnostics.Process" /> object.-or- You are attempting to call <see cref="M:System.Diagnostics.Process.WaitForExit(System.Int32)" /> for a process that is running on a remote computer. This method is available only for processes that are running on the local computer. </exception>
        public bool WaitForExit(int milliseconds) {
            return OGProcess.WaitForExit(milliseconds);
        }

        /// <summary>Instructs the <see cref="T:System.Diagnostics.Process" /> component to wait indefinitely for the associated process to exit.</summary>
        /// <exception cref="T:System.ComponentModel.Win32Exception">The wait setting could not be accessed. </exception>
        /// <exception cref="T:System.SystemException">No process <see cref="P:System.Diagnostics.Process.Id" /> has been set, and a <see cref="P:System.Diagnostics.Process.Handle" /> from which the <see cref="P:System.Diagnostics.Process.Id" /> property can be determined does not exist.-or- There is no process associated with this <see cref="T:System.Diagnostics.Process" /> object.-or- You are attempting to call <see cref="M:System.Diagnostics.Process.WaitForExit" /> for a process that is running on a remote computer. This method is available only for processes that are running on the local computer. </exception>
        public void WaitForExit() {
            OGProcess.WaitForExit();
        }

        /// <summary>Causes the <see cref="T:System.Diagnostics.Process" /> component to wait the specified number of milliseconds for the associated process to enter an idle state. This overload applies only to processes with a user interface and, therefore, a message loop.</summary>
        /// <returns>true if the associated process has reached an idle state; otherwise, false.</returns>
        /// <param name="milliseconds">A value of 1 to <see cref="F:System.Int32.MaxValue" /> that specifies the amount of time, in milliseconds, to wait for the associated process to become idle. A value of 0 specifies an immediate return, and a value of -1 specifies an infinite wait. </param>
        /// <exception cref="T:System.InvalidOperationException">The process does not have a graphical interface.-or-An unknown error occurred. The process failed to enter an idle state.-or-The process has already exited. -or-No process is associated with this <see cref="T:System.Diagnostics.Process" /> object.</exception>
        public bool WaitForInputIdle(int milliseconds) {
            try {
                return OGProcess.WaitForInputIdle(milliseconds);
            } catch (InvalidOperationException) {
                return false;
            }
        }

        /// <summary>Causes the <see cref="T:System.Diagnostics.Process" /> component to wait indefinitely for the associated process to enter an idle state. This overload applies only to processes with a user interface and, therefore, a message loop.</summary>
        /// <returns>true if the associated process has reached an idle state.</returns>
        /// <exception cref="T:System.InvalidOperationException">The process does not have a graphical interface.-or-An unknown error occurred. The process failed to enter an idle state.-or-The process has already exited. -or-No process is associated with this <see cref="T:System.Diagnostics.Process" /> object.</exception>
        public bool WaitForInputIdle() {
            try {
                return OGProcess.WaitForInputIdle();
            } catch (InvalidOperationException) {
                return false;
            }
        }

        /// <summary>Gets a value indicating whether the associated process has been terminated.</summary>
        /// <returns>true if the operating system process referenced by the <see cref="T:System.Diagnostics.Process" /> component has terminated; otherwise, false.</returns>
        /// <exception cref="T:System.InvalidOperationException">There is no process associated with the object. </exception>
        /// <exception cref="T:System.ComponentModel.Win32Exception">The exit code for the process could not be retrieved. </exception>
        /// <exception cref="T:System.NotSupportedException">You are trying to access the <see cref="P:System.Diagnostics.Process.HasExited" /> property for a process that is running on a remote computer. This property is available only for processes that are running on the local computer.</exception>
        public bool HasExited {
            get { return OGProcess.HasExited; }
        }

        /// <summary>Gets the time that the associated process exited.</summary>
        /// <returns>A <see cref="T:System.DateTime" /> that indicates when the associated process was terminated.</returns>
        /// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Windows Me), which does not support this property. </exception>
        /// <exception cref="T:System.NotSupportedException">You are trying to access the <see cref="P:System.Diagnostics.Process.ExitTime" /> property for a process that is running on a remote computer. This property is available only for processes that are running on the local computer.</exception>
        public DateTime ExitTime {
            get { return OGProcess.ExitTime; }
        }

        /// <summary>Gets the native handle of the associated process.</summary>
        /// <returns>The handle that the operating system assigned to the associated process when the process was started. The system uses this handle to keep track of process attributes.</returns>
        /// <exception cref="T:System.InvalidOperationException">The process has not been started or has exited. The <see cref="P:System.Diagnostics.Process.Handle" /> property cannot be read because there is no process associated with this <see cref="T:System.Diagnostics.Process" /> instance.-or- The <see cref="T:System.Diagnostics.Process" /> instance has been attached to a running process but you do not have the necessary permissions to get a handle with full access rights. </exception>
        /// <exception cref="T:System.NotSupportedException">You are trying to access the <see cref="P:System.Diagnostics.Process.Handle" /> property for a process that is running on a remote computer. This property is available only for processes that are running on the local computer.</exception>
        public IntPtr Handle {
            get { return OGProcess.Handle; }
        }

        /// <summary>Gets the unique identifier for the associated process.</summary>
        /// <returns>The system-generated unique identifier of the process that is referenced by this <see cref="T:System.Diagnostics.Process" /> instance.</returns>
        /// <exception cref="T:System.InvalidOperationException">The process's <see cref="P:System.Diagnostics.Process.Id" /> property has not been set.-or- There is no process associated with this <see cref="T:System.Diagnostics.Process" /> object. </exception>
        /// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Windows Me); set the <see cref="P:System.Diagnostics.ProcessStartInfo.UseShellExecute" /> property to false to access this property on Windows 98 and Windows Me.</exception>
        public int Id {
            get { return OGProcess.Id; }
        }

        /// <summary>Gets the name of the computer the associated process is running on.</summary>
        /// <returns>The name of the computer that the associated process is running on.</returns>
        /// <exception cref="T:System.InvalidOperationException">There is no process associated with this <see cref="T:System.Diagnostics.Process" /> object. </exception>
        public string MachineName {
            get { return OGProcess.MachineName; }
        }

        /// <summary>Gets the window handle of the main window of the associated process.</summary>
        /// <returns>The system-generated window handle of the main window of the associated process.</returns>
        /// <exception cref="T:System.InvalidOperationException">The <see cref="P:System.Diagnostics.Process.MainWindowHandle" /> is not defined because the process has exited. </exception>
        /// <exception cref="T:System.NotSupportedException">You are trying to access the <see cref="P:System.Diagnostics.Process.MainWindowHandle" /> property for a process that is running on a remote computer. This property is available only for processes that are running on the local computer.</exception>
        /// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Windows Me); set <see cref="P:System.Diagnostics.ProcessStartInfo.UseShellExecute" /> to false to access this property on Windows 98 and Windows Me. </exception>
        public IntPtr MainWindowHandle {
            get { return OGProcess.MainWindowHandle; }
        }

        /// <summary>Gets the caption of the main window of the process.</summary>
        /// <returns>The main window title of the process.</returns>
        /// <exception cref="T:System.InvalidOperationException">The <see cref="P:System.Diagnostics.Process.MainWindowTitle" /> property is not defined because the process has exited. </exception>
        /// <exception cref="T:System.NotSupportedException">You are trying to access the <see cref="P:System.Diagnostics.Process.MainWindowTitle" /> property for a process that is running on a remote computer. This property is available only for processes that are running on the local computer.</exception>
        /// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Windows Me); set <see cref="P:System.Diagnostics.ProcessStartInfo.UseShellExecute" /> to false to access this property on Windows 98 and Windows Me.</exception>
        public string MainWindowTitle {
            get { return OGProcess.MainWindowTitle; }
        }

        /// <summary>Gets the main module for the associated process.</summary>
        /// <returns>The <see cref="T:System.Diagnostics.ProcessModule" /> that was used to start the process.</returns>
        /// <exception cref="T:System.NotSupportedException">You are trying to access the <see cref="P:System.Diagnostics.Process.MainModule" /> property for a process that is running on a remote computer. This property is available only for processes that are running on the local computer.</exception>
        /// <exception cref="T:System.ComponentModel.Win32Exception">A 32-bit process is trying to access the modules of a 64-bit process.</exception>
        /// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Windows Me); set <see cref="P:System.Diagnostics.ProcessStartInfo.UseShellExecute" /> to false to access this property on Windows 98 and Windows Me.</exception>
        /// <exception cref="T:System.InvalidOperationException">The process <see cref="P:System.Diagnostics.Process.Id" /> is not available.-or- The process has exited. </exception>
        public ProcessModule MainModule {
            get {
                try {
                    return OGProcess.MainModule;
                } catch {
                    return null;
                }
            }
        }

        /// <summary>Gets the name of the process.</summary>
        /// <returns>The name that the system uses to identify the process to the user.</returns>
        /// <exception cref="T:System.InvalidOperationException">The process does not have an identifier, or no process is associated with the <see cref="T:System.Diagnostics.Process" />.-or- The associated process has exited. </exception>
        /// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Windows Me); set <see cref="P:System.Diagnostics.ProcessStartInfo.UseShellExecute" /> to false to access this property on Windows 98 and Windows Me.</exception>
        /// <exception cref="T:System.NotSupportedException">The process is not on this computer.</exception>
        public string ProcessName {
            get { return OGProcess.ProcessName; }
        }

        /// <summary>Gets a value indicating whether the user interface of the process is responding.</summary>
        /// <returns>true if the user interface of the associated process is responding to the system; otherwise, false.</returns>
        /// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Windows Me); set <see cref="P:System.Diagnostics.ProcessStartInfo.UseShellExecute" /> to false to access this property on Windows 98 and Windows Me.</exception>
        /// <exception cref="T:System.InvalidOperationException">There is no process associated with this <see cref="T:System.Diagnostics.Process" /> object. </exception>
        /// <exception cref="T:System.NotSupportedException">You are attempting to access the <see cref="P:System.Diagnostics.Process.Responding" /> property for a process that is running on a remote computer. This property is available only for processes that are running on the local computer. </exception>
        public bool Responding {
            get { return OGProcess.Responding; }
        }

        /// <summary>Gets or sets the properties to pass to the <see cref="M:System.Diagnostics.Process.Start" /> method of the <see cref="T:System.Diagnostics.Process" />.</summary>
        /// <returns>The <see cref="T:System.Diagnostics.ProcessStartInfo" /> that represents the data with which to start the process. These arguments include the name of the executable file or document used to start the process.</returns>
        /// <exception cref="T:System.ArgumentNullException">The value that specifies the <see cref="P:System.Diagnostics.Process.StartInfo" /> is null. </exception>
        public ProcessStartInfo StartInfo {
            get { return OGProcess.StartInfo; }
            set { OGProcess.StartInfo = value; }
        }

        /// <summary>Gets the time that the associated process was started.</summary>
        /// <returns>An object  that indicates when the process started. An exception is thrown if the process is not running.</returns>
        /// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Windows Me), which does not support this property. </exception>
        /// <exception cref="T:System.NotSupportedException">You are attempting to access the <see cref="P:System.Diagnostics.Process.StartTime" /> property for a process that is running on a remote computer. This property is available only for processes that are running on the local computer. </exception>
        /// <exception cref="T:System.InvalidOperationException">The process has exited.-or-The process has not been started.</exception>
        /// <exception cref="T:System.ComponentModel.Win32Exception">An error occurred in the call to the Windows function.</exception>
        public DateTime StartTime {
            get {
                try {
                    return OGProcess.StartTime;
                } catch (Exception) {
                    
                    return DateTime.MinValue;
                }
            }
        } 

        public event EventHandler Exited {
            add { OGProcess.Exited += value; }
            remove { OGProcess.Exited -= value; }
        }
        #endregion

        #region Async

        private readonly AsyncManualResetEvent _exitWaiter = new AsyncManualResetEvent();
        public async Task WaitForExitAsync() {
            await _exitWaiter.WaitAsync();
        }

        public async Task WaitForExitAsync(int milliseconds) {
            await _exitWaiter.WaitAsync(milliseconds);
        }

        /// <summary>
        ///     Waits for the process to be responding
        /// </summary>
        public void WaitForResponding() {
            Refresh();
            this.WaitForInputIdle();
            SpinWait.SpinUntil(() => Responding);
        }

        /// <summary>
        ///     Waits for the process to be responding
        /// </summary>
        public async Task WaitForRespondingAsync() {
            await Task.Yield();
            WaitForResponding();
        }
        #endregion

        #region Static
        private static readonly List<SmartProcess> sporcs = new List<SmartProcess>();

        /// <summary>
        ///     Gets or create the instance out of the process name.
        /// </summary>
        /// <param name="processname">The process name</param>
        /// <returns></returns>
        public static SmartProcess Get(string processname) {
            if (string.IsNullOrEmpty(processname))
                throw new ArgumentNullException(nameof(processname));

            var local = sporcs.ToArray();
            var @out = local.FirstOrDefault(sp => sp.ProcessName.Equals(processname));
            if (@out == null) {
                var ogproc = Process.GetProcessesByName(processname);
                if (ogproc.Length > 1) {
                    throw new IndexOutOfRangeException($"Too many processes behold the name '{processname}'. Failed to get SmartProcess.");
                } if (ogproc.Length == 0) {
                    throw new IndexOutOfRangeException($"No process beholds the name '{processname}'. Failed to get SmartProcess.");
                }
                @out = new SmartProcess(ogproc[0]);
                sporcs.Add(@out);
            }
            return @out;
        }

        /// <summary>
        ///     Gets or create the instance out of the process object.
        /// </summary>
        /// <param name="process">The process to smart.</param>
        /// <returns></returns>
        public static SmartProcess Get(Process process) {
            if (process==null)
                throw new ArgumentNullException(nameof(process));

            var local = sporcs.ToArray();
            var @out = local.FirstOrDefault(sp => sp.OGProcess.Id== process.Id && sp.OGProcess.ProcessName==process.ProcessName);
            if (@out == null) {
                @out = new SmartProcess(process);
                sporcs.Add(@out);
            }
            return @out;
        }
                /// <summary>
        ///     Gets or create the instance out of the process object.
        /// </summary>
        /// <param name="process">The process to smart.</param>
        /// <returns></returns>
        public static SmartProcess GetCached(Guid guid) {
            return sporcs.SingleOrDefault(sproc => sproc.GUID.Equals(guid));
        }
        /// <summary>
        ///     Gets or create the instance out of the foreground window.
        /// </summary>
        /// <param name="process">The process to smart.</param>
        /// <returns></returns>
        public static SmartProcess GetForeground() {
            IntPtr hwnd = Native.GetForegroundWindow();
            uint pid;
            Native.GetWindowThreadProcessId(hwnd, out pid);
            Process p = Process.GetProcessById((int)pid);
            return Get(p);
        }


        #endregion
    }
}