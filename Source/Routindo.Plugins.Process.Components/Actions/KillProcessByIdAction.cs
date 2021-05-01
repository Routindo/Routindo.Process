using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Routindo.Contract.Arguments;
using Routindo.Contract.Attributes;
using Routindo.Contract.Exceptions;

namespace Routindo.Plugins.Process.Components.Actions
{
    [PluginItemInfo(ComponentUniqueId, nameof(KillProcessByIdAction),
         "forces a termination of the process using its ID", Category = "Process", FriendlyName = "Kill Process By ID"),
     ExecutionArgumentsClass(typeof(KillProcessByIdActionExecutionArgs)),
     ResultArgumentsClass(typeof(KillProcessActionResultArgs))]
    public class KillProcessByIdAction: KillProcessAction
    {
        public const string ComponentUniqueId = "AF38D568-BA81-4DA1-B033-2D6188B7C6A0";
        protected override IEnumerable<System.Diagnostics.Process> GetProcesses(ArgumentCollection arguments)
        {
            int processId = default;
            if (arguments.HasArgument(KillProcessByIdActionExecutionArgs.ProcessId))
            {
                processId = arguments.GetValue<int>(KillProcessByIdActionExecutionArgs.ProcessId);
            }

            if(processId == default)
                throw new MissingArgumentException(KillProcessByIdActionExecutionArgs.ProcessId);

            var process = System.Diagnostics.Process.GetProcessById(processId);
            yield return process;
        }
    }
}
