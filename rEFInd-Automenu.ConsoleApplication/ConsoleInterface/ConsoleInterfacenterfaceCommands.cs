using Rikitav.Plasma.Tasks.Execution;

namespace rEFInd_Automenu.ConsoleApplication.ConsoleInterface
{
    public interface IConsoleInterfacenterfaceCommands : ITaskControllerCommands
    {
        void SetLockHandle(object SyncLockObject);
        void SetWorkLine(int WorkLine);
        void ChangeLabel(string WorkText);
    }
}
