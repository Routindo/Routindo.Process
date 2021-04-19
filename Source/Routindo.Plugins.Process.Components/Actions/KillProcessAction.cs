using System;
using System.Collections.Generic;
using System.Linq;
using Routindo.Contract.Actions;
using Routindo.Contract.Arguments;
using Routindo.Contract.Attributes;
using Routindo.Contract.Services;

namespace Routindo.Plugins.Process.Components.Actions
{
    public abstract class KillProcessAction: IAction
    {
        public string Id { get; set; }

        public ILoggingService LoggingService { get; set; }

        [Argument(KillProcessActionArgs.EntireProcessTree)] public bool EntireProcessTree { get; set; }

        [Argument(KillProcessActionArgs.WaitForExit)] public bool WaitForExit { get; set; }

        [Argument(KillProcessActionArgs.WaitForExitTimeout)] public int WaitForExitTimeout { get; set; }

        public ActionResult Execute(ArgumentCollection arguments)
        {
            try
            {
                var processes = GetProcesses(arguments).ToList();
                if (!processes.Any())
                {
                    LoggingService.Error("No process found.");
                    return ActionResult.Failed();
                }

                List<int> succeeded = new List<int>();
                List<int> failed = new List<int>();
                foreach (var process in processes)
                {
                    int processId = process.Id;
                    if (!process.HasExited)
                    {
                        
                        string processName = process.ProcessName;
                        if (KillProcess(process))
                        {
                            LoggingService.Debug($"Killed Process ({processName}) [{processId}]");
                            succeeded.Add(processId);
                        }
                        else
                        {
                            LoggingService.Debug($"Failed to kill Process ({processName}) [{processId}]");
                            failed.Add(processId);
                        }
                    }
                    else
                    {
                        LoggingService.Debug($"Failed to kill Process [{processId}] - Process Already Exited");
                        failed.Add(processId);
                    }
                }

                var result = ActionResult.Succeeded()
                    .WithAdditionInformation(
                        ArgumentCollection.New()
                            .WithArgument(KillProcessActionResultArgs.ProcessesKilledSuccessfully, succeeded)
                            .WithArgument(KillProcessActionResultArgs.ProcessesKillingFailed, failed)
                    );
                return result;
            }
            catch (Exception exception)
            {
                LoggingService.Error(exception);
                return ActionResult.Failed().WithException(exception);
            }
        }

        protected abstract IEnumerable<System.Diagnostics.Process> GetProcesses(ArgumentCollection arguments);  

        private bool KillProcess(System.Diagnostics.Process process)
        {
            try
            {
                process.Kill(this.EntireProcessTree);
                return !WaitForExit || process.WaitForExit(WaitForExitTimeout);
            }
            catch (Exception exception)
            {
                LoggingService.Error(exception);
                return false;
            }
        }
    }
}
