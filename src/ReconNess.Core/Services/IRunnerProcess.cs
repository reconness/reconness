namespace ReconNess.Core.Services
{
    public interface IRunnerProcess
    {
        bool IsRunning();
        void KillProcess();
        void StartProcess(string command);
        string TerminalLineOutput();
    }
}