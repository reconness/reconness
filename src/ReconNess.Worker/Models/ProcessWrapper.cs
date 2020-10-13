using NLog;
using System;
using System.Diagnostics;

namespace ReconNess.Worker.Models
{
    public class ProcessWrapper
    {
        protected static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        private Process process;

        public bool Stopped { get; set; } = false;

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

        public void StopProcess()
        {
            if (process != null)
            {
                try
                {
                    Stopped = true;

                    process.WaitForExit();
                }
                catch (Exception ex)
                {
                    _logger.Info("Exception stopping the process");
                    _logger.Error(ex, ex.Message);
                }
                finally
                {
                    process = null;
                }
            }
        }
    }
}
