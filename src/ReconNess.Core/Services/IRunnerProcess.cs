namespace ReconNess.Core.Services
{
    public interface IRunnerProcess
    {
        bool Stopped { get; set; }
        bool EndOfStream { get; }
        bool IsRunning();
        void KillProcess();
        void StartProcess(string command);
        string TerminalLineOutput();
    }
}