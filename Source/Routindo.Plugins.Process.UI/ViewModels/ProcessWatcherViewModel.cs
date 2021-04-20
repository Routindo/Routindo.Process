using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Routindo.Contract.Arguments;
using Routindo.Contract.UI;
using Routindo.Plugins.Process.Components.Watchers;

namespace Routindo.Plugins.Process.UI.ViewModels
{
    public class ProcessWatcherViewModel: PluginConfiguratorViewModelBase
    {
        private string _processName;
        private string _selectedProcessName;
        private bool _watchForStopped;

        public ProcessWatcherViewModel()
        {
            ProcessesNames = new ObservableCollection<string>();
            ProcessesNames.CollectionChanged += OnProcessesNamesCollectionChanged;
            this.UseSelectedProcessNameCommand = new RelayCommand(UseSelectedProcessName, CanUseSelectedProcessName);
            this.LoadProcessesNamesCommand = new RelayCommand(LoadProcessesNames);
        }

        private void OnProcessesNamesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(AllowSelectionFromLoadedProcesses));
        }

        public bool AllowSelectionFromLoadedProcesses => ProcessesNames.Any();

        private void LoadProcessesNames()
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                ProcessesNames.Clear();
                var processNames = System.Diagnostics.Process.GetProcesses()
                    .Where(p =>
                    {
                        bool hasException = false;
                        try
                        {
                            IntPtr x = p.Handle;
                        }
                        catch
                        {
                            hasException = true;
                        }

                        return !hasException;
                    })
                    .Where(p => !p.HasExited).Select(p => p.ProcessName).Distinct().OrderBy(n => n);

                foreach (var processName in processNames)
                {
                    ProcessesNames.Add(processName);
                }
            }));
        }

        public ICommand LoadProcessesNamesCommand { get; set; }

        public string SelectedProcessName
        {
            get => _selectedProcessName;
            set
            {
                _selectedProcessName = value;
                OnPropertyChanged();
            }
        }

        public bool WatchForStopped
        {
            get => _watchForStopped;
            set
            {
                _watchForStopped = value;
                OnPropertyChanged();
            }
        }

        private bool CanUseSelectedProcessName()
        {
            return !string.IsNullOrWhiteSpace(SelectedProcessName);
        }

        private void UseSelectedProcessName()
        {
            this.ProcessName = this.SelectedProcessName;
        }

        public ICommand UseSelectedProcessNameCommand { get; set; }

        public string ProcessName
        {
            get => _processName;
            set
            {
                ClearPropertyErrors();
                _processName = value;
                ValidateNonNullOrEmptyString(_processName);
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> ProcessesNames { get; set; }

        public override void Configure()
        {
            this.InstanceArguments = ArgumentCollection.New()
                .WithArgument(ProcessWatcherArgs.ProcessName, ProcessName)
                .WithArgument(ProcessWatcherArgs.WatchForStopped, WatchForStopped);
        }

        public override void SetArguments(ArgumentCollection arguments)
        {
            if (arguments.HasArgument(ProcessWatcherArgs.ProcessName))
                ProcessName = arguments.GetValue<string>(ProcessWatcherArgs.ProcessName);

            if (arguments.HasArgument(ProcessWatcherArgs.WatchForStopped))
                WatchForStopped = arguments.GetValue<bool>(ProcessWatcherArgs.WatchForStopped);
        }
    }
}
