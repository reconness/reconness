using System;
using System.Diagnostics;
using System.Threading;

namespace ReconNess.Services
{
    public class RunnerProcess
    {
        private Process process;

        public RunnerProcess(string command)
        {
            this.process = new Process()
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
            
            this.process.Start();
        }

        public bool EndOfStream
        {
            get
            {
                if (this.process == null)
                {
                    return true;
                }
                
                return this.process.StandardOutput.EndOfStream && this.process.StandardError.EndOfStream;
            }
        }

        public string TerminalLineOutput()
        {
            if (!this.EndOfStream)
            {
                if (!this.process.StandardError.EndOfStream)
                {
                    return this.process.StandardError.ReadLine();
                }
                return this.process.StandardOutput.ReadLine();
            }

            return string.Empty;
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
    }
}
