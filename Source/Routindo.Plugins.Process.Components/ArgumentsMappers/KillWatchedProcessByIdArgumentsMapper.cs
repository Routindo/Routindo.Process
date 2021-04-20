using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Routindo.Contract.Arguments;
using Routindo.Contract.Attributes;
using Routindo.Contract.Services;
using Routindo.Plugins.Process.Components.Actions;
using Routindo.Plugins.Process.Components.Watchers;

namespace Routindo.Plugins.Process.Components.ArgumentsMappers
{

    [PluginItemInfo(ComponentId, nameof(KillWatchedProcessByIdArgumentsMapper),
        "Map arguments of " + nameof(ProcessWatcher) + "to arguments of " + nameof(KillProcessByIdAction))]
    public class KillWatchedProcessByIdArgumentsMapper: IArgumentsMapper
    {
        public const string ComponentId = "6E4227DA-A3AC-43B9-A5E5-9A232CD8BB51";
        public string Id { get; set; }
        public ILoggingService LoggingService { get; set; }
        public ArgumentCollection Map(ArgumentCollection arguments)
        {
            ArgumentCollection argumentCollection = ArgumentCollection.New();
            if (arguments.HasArgument(ProcessWatcherResultArgs.ProcessId))
            {
                argumentCollection.Add(KillProcessByIdActionExecutionArgs.ProcessId,
                    arguments[ProcessWatcherResultArgs.ProcessId]);
            }

            return argumentCollection;
        }
    }
}
