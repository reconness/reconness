using System;
using System.Diagnostics;
using System.Threading;
using ReconNess.Core.Services;

namespace ReconNess.Services
{
    public class RunnerProcess : IRunnerProcess
    {
        private Process process;

        public bool Stopped { get; set; }

        public void StartProcess(string command)
        {
            if (this.IsRunning())
            {
                this.KillProcess();
            }

            process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{command}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };

            Thread.Sleep(1000);

            process.Start();
        }

        public bool IsRunning()
        {
            try
            {
                return !(process == null || process.HasExited || process.StandardOutput.EndOfStream);
            }
            catch (Exception)
            {
                process = null;
                return false;
            }
        }

        public void KillProcess()
        {
            if (process != null)
            {
                process.Kill();
                process.WaitForExit();
                process = null;
            }
        }

        public string TerminalLineOutput()
        {
            if (this.IsRunning())
            {
                return process.StandardOutput.ReadLine();
            }

            return string.Empty;
        }

    }
}
