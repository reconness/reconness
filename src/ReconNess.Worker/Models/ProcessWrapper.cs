using System.Diagnostics;

namespace ReconNess.Worker.Models
{
    public class ProcessWrapper
    {
        private Process process;

        public void Start(string command)
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

                return this.process.StandardOutput.EndOfStream;
            }
        }

        public string TerminalLineOutput()
        {
            if (!this.EndOfStream)
            {
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
