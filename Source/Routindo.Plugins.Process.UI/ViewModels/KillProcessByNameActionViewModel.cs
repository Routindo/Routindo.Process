using System;
using Routindo.Contract.Arguments;
using Routindo.Contract.UI;
using Routindo.Plugins.Process.Components.Actions;

namespace Routindo.Plugins.Process.UI.ViewModels
{
    public class KillProcessByNameActionViewModel: KillProcessActionViewModel
    {
        private string _processName;

        public string ProcessName
        {
            get => _processName;
            set
            {
                _processName = value;
                OnPropertyChanged();
            }
        }

        public override void Configure()
        {
            base.Configure();
            this.InstanceArguments.Add(KillProcessByNameActionArgs.ProcessName, ProcessName);
        }

        public override void SetArguments(ArgumentCollection arguments)
        {
            base.SetArguments(arguments);       
            if (arguments.HasArgument(KillProcessByNameActionArgs.ProcessName))
                ProcessName = arguments.GetValue<string>(KillProcessByNameActionArgs.ProcessName);
        }
    }
}
