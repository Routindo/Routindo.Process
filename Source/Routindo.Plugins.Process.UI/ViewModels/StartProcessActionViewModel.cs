
using System.Windows.Forms;
using System.Windows.Input;
using Routindo.Contract.Arguments;
using Routindo.Contract.UI;
using Routindo.Plugins.Process.Components.Actions;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace Routindo.Plugins.Process.UI.ViewModels
{
    public class StartProcessActionViewModel: PluginConfiguratorViewModelBase
    {
        private string _processPath;
        private string _processArguments;
        private string _processWorkingDirectory;
        private bool _waitForExit;
        private int _waitForExitTimeout;

        public StartProcessActionViewModel()
        {
            this.SelectDirectoryCommand = new RelayCommand(SelectDirectory); 
            this.SelectFilePathCommand = new RelayCommand(SelectFilePath);
        }

        public string ProcessPath
        {
            get => _processPath;
            set
            {
                _processPath = value;
                OnPropertyChanged();
            }
        }

        public string ProcessArguments
        {
            get => _processArguments;
            set
            {
                _processArguments = value;
                OnPropertyChanged();
            }
        }

        public string ProcessWorkingDirectory
        {
            get => _processWorkingDirectory;
            set
            {
                _processWorkingDirectory = value;
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
                ClearPropertyErrors();
                _waitForExitTimeout = value;
                ValidateNumber(_waitForExitTimeout, i => !_waitForExit || (_waitForExit && i > 0));
                OnPropertyChanged();
            }
        }

        public ICommand SelectDirectoryCommand { get; }

        private void SelectDirectory()
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                if (!string.IsNullOrWhiteSpace(ProcessWorkingDirectory))
                {
                    dialog.SelectedPath = ProcessWorkingDirectory;
                }

                dialog.Description = "Directory where to watch for new files";
                dialog.ShowNewFolderButton = true;
                dialog.UseDescriptionForTitle = true;
                var dialogResult = dialog.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    ProcessWorkingDirectory = dialog.SelectedPath;
                }
            }
        }

        private void SelectFilePath() 
        {
            var openFileDialog = new OpenFileDialog { CheckFileExists = false, Title = "Select File" };
            var result = openFileDialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                this.ProcessPath = openFileDialog.FileName;
            }
        }

        public ICommand SelectFilePathCommand { get; } 

        public override void Configure()
        {
            this.InstanceArguments = ArgumentCollection.New()
                .WithArgument(StartProcessActionArgs.ProcessPath, ProcessPath)
                .WithArgument(StartProcessActionArgs.ProcessWorkingDirectory, ProcessWorkingDirectory)
                .WithArgument(StartProcessActionArgs.ProcessArguments, ProcessArguments)
                .WithArgument(StartProcessActionArgs.WaitForExit, WaitForExit)
                .WithArgument(StartProcessActionArgs.WaitForExitTimeout, WaitForExitTimeout);
        }

        public override void SetArguments(ArgumentCollection arguments)
        {
            if (arguments.HasArgument(StartProcessActionArgs.ProcessPath))
                ProcessPath = arguments.GetValue<string>(StartProcessActionArgs.ProcessPath);

            if (arguments.HasArgument(StartProcessActionArgs.ProcessWorkingDirectory))
                ProcessWorkingDirectory = arguments.GetValue<string>(StartProcessActionArgs.ProcessWorkingDirectory);

            if (arguments.HasArgument(StartProcessActionArgs.ProcessArguments))
                ProcessArguments = arguments.GetValue<string>(StartProcessActionArgs.ProcessArguments);

            if (arguments.HasArgument(StartProcessActionArgs.WaitForExit))
                WaitForExit = arguments.GetValue<bool>(StartProcessActionArgs.WaitForExit);

            if (arguments.HasArgument(StartProcessActionArgs.WaitForExitTimeout))
                WaitForExitTimeout = arguments.GetValue<int>(StartProcessActionArgs.WaitForExitTimeout);
        }
    }
}
