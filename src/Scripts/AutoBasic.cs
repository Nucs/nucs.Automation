using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using nucs.Automation.Controllers;
using nucs.Automation;
using nucs.Automation.Mirror;
using nucs.Filesystem;
using Shell32;

namespace nucs.Automation.Scripts {
    /// <summary>
    ///     Has the basic methods for automation
    /// </summary>
    public static class AutoBasic {
        /// <summary>
        ///     Will start an application using Run and will return approximately the process that was opened.
        /// </summary>
        /// <param name="application">The text to type into the textbox in the run window</param>
        /// <param name="returnProcess">Should the code go through returning the started process?</param>
        /// <param name="processIdentifier">A method to identify the new process, null if not to use this method.</param>
        public static async Task<SmartProcess> Run(string application,bool returnProcess ,Func<Process, bool> processIdentifier = null) {
            await Task.Yield();
            var sproc = SmartProcess.Get("explorer");
            _recapture:
            var win = sproc.Windows.FirstOrDefault(w => w.Type == WindowType.Run);
            if (win == null) {
                /*Keyboard.Window(KeyCode.R);*/
                new Shell32.Shell().FileRun(); //faster
                goto _recapture;
            }
            win.BringToFront();
            Thread.Sleep(300);
            await win.WaitForRespondingAsync();

            win.Keyboard.Write(application);
            win.Keyboard.Enter();
            if (!returnProcess)
                return null;
            Thread.Sleep(200);

            //Get the process
            var foreg = SmartProcess.GetForeground();
            if (processIdentifier != null) {
                var p = Process.GetProcesses().Where(processIdentifier).OrderByDescending(pp => {
                    try {
                        return pp.StartTime.Ticks;
                    } catch {
                        return 0;
                    }
                }).ToArray();

                var @out = p.FirstOrDefault(proc => proc.Id == foreg.Id && proc.ProcessName == foreg.ProcessName);
                if (@out != null) {
                    return SmartProcess.Get(@out);
                }
            }

            return foreg;
        }

        /// <summary>
        ///     Generates run as admin startinfo
        /// </summary>
        private static ProcessStartInfo __generate_info => new ProcessStartInfo() { Verb = "runas", WorkingDirectory = null, CreateNoWindow = false, UseShellExecute = true};
        /// <summary>
        ///     Will start an application using Process.Start() with default ProcessStartInfo including 'runas' to elevate priviledges.
        /// </summary>
        /// <param name="filename">The file to start.</param>
        public static SmartProcess RunUsingProcessStart(string filename) {
            var t = __generate_info;

            filename = Paths.NormalizePath(filename);
            if (File.Exists(filename) && File.GetAttributes(filename).HasFlag(FileAttributes.Directory) == false) {
                t.WorkingDirectory = Path.GetDirectoryName(filename) ?? "";
            }

            t.FileName = filename;
            var proc = Process.Start(t);
            return proc == null ? null : SmartProcess.Get(proc);
        }

        /// <summary>
        ///     Run commands in a new Cmd window.
        /// </summary>
        /// <param name="scripts">The commands to run, in order</param>
        /// <returns></returns>
        public static async Task Cmd(params string[] scripts) {
            var sproc = await Run("cmd.exe", true, process => process.ProcessName=="cmd");
            if (sproc == null || sproc.ProcessName != "cmd") //last stand chance
                sproc = SmartProcess.Get("cmd");
            
            sproc.BringToFront();
            Thread.Sleep(300);
            //await sproc.WaitForRespondingAsync();
            Keyboard.Write(string.Join(" & ", scripts)); 
            Keyboard.Enter();
        }
    }
}