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

            process.Start();
        }

        public bool EndOfStream
        {
            get
            {
                if (process == null)
                {
                    return true;
                }

                return process.StandardOutput.EndOfStream;
            }
        }

        public string TerminalLineOutput()
        {
            if (!EndOfStream)
            {
                return process.StandardOutput.ReadLine();
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

                    process.Kill(true);
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
