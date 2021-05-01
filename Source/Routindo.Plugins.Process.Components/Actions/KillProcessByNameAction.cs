using System.Collections.Generic;
using Routindo.Contract.Arguments;
using Routindo.Contract.Attributes;
using Routindo.Contract.Exceptions;

namespace Routindo.Plugins.Process.Components.Actions
{
    [PluginItemInfo(ComponentUniqueId, nameof(KillProcessByNameAction),
         "forces a termination of one or more processes using the process name", Category = "Process", FriendlyName = "Kill Process by Name"),
     ExecutionArgumentsClass(typeof(KillProcessByNameActionExecutionArgs)),
     ResultArgumentsClass(typeof(KillProcessActionResultArgs))]
    public class KillProcessByNameAction: KillProcessAction
    {
        public const string ComponentUniqueId = "09938242-199B-44C7-9CF2-837718524117";

        [Argument(KillProcessByNameActionArgs.ProcessName)] public string ProcessName { get; set; }

        protected override IEnumerable<System.Diagnostics.Process> GetProcesses(ArgumentCollection arguments)
        {
            
            string processName = ProcessName;
            if (string.IsNullOrWhiteSpace(processName))
            {
                processName = arguments.GetValue<string>(KillProcessByNameActionExecutionArgs.ProcessName);
            }

            if (string.IsNullOrWhiteSpace(processName))
            {
                throw new MissingArgumentException(KillProcessByNameActionExecutionArgs.ProcessName);
            }

            LoggingService.Trace($"Getting process by name {processName}");
            return System.Diagnostics.Process.GetProcessesByName(processName);
        }
    }
}
