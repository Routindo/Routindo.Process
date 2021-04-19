using Routindo.Contract.Arguments;
using Routindo.Contract.UI;
using Routindo.Plugins.Process.Components.Actions;

namespace Routindo.Plugins.Process.UI.ViewModels
{
    public abstract class KillProcessActionViewModel: PluginConfiguratorViewModelBase
    {
        private bool _entireProcessTree;
        private bool _waitForExit;
        private int _waitForExitTimeout = 100;

        public bool EntireProcessTree
        {
            get => _entireProcessTree;
            set
            {
                _entireProcessTree = value;
                OnPropertyChanged();
            }
        }

        public bool WaitForExit
        {
            get => _waitForExit;
            set
            {
                _waitForExit = value;
                OnPropertyChanged();
            }
        }

        public int WaitForExitTimeout
        {
            get => _waitForExitTimeout;
            set
            {
                _waitForExitTimeout = value;
                OnPropertyChanged();
            }
        }

        public override void Configure()
        {
            this.InstanceArguments = ArgumentCollection.New()
                .WithArgument(KillProcessActionArgs.EntireProcessTree, EntireProcessTree)
                .WithArgument(KillProcessActionArgs.WaitForExitTimeout, WaitForExitTimeout)
                .WithArgument(KillProcessActionArgs.WaitForExitTimeout, WaitForExitTimeout);
        }

        public override void SetArguments(ArgumentCollection arguments)
        {
            if (arguments.HasArgument(KillProcessActionArgs.EntireProcessTree))
                EntireProcessTree = arguments.GetValue<bool>(KillProcessActionArgs.EntireProcessTree);

            if (arguments.HasArgument(KillProcessActionArgs.WaitForExit))
                WaitForExit = arguments.GetValue<bool>(KillProcessActionArgs.WaitForExit);

            if (arguments.HasArgument(KillProcessActionArgs.WaitForExitTimeout))
                WaitForExitTimeout = arguments.GetValue<int>(KillProcessActionArgs.WaitForExitTimeout);
        }
    }
}
