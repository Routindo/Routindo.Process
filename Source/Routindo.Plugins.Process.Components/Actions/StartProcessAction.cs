using System;
using System.IO;
using Routindo.Contract.Actions;
using Routindo.Contract.Arguments;
using Routindo.Contract.Attributes;
using Routindo.Contract.Exceptions;
using Routindo.Contract.Services;
using Exception = System.Exception;

namespace Routindo.Plugins.Process.Components.Actions
{
    [PluginItemInfo(ComponentUniqueId, "Start Process",
         "Start a specific process in background with arguments and on a specific working directory"),
     ExecutionArgumentsClass(typeof(StartProcessActionExecutionArgs)), 
     ResultArgumentsClass(typeof(StartProcessActionResultArgs))]
    public class StartProcessAction: IAction
    {
        public const string ComponentUniqueId = "24B4DD70-395D-4414-8D1A-36CC1D494B13";

        public string Id { get; set; }
        public ILoggingService LoggingService { get; set; }

        [Argument(StartProcessActionArgs.ProcessPath)] public string ProcessPath { get; set; }

        [Argument(StartProcessActionArgs.ProcessArguments)] public string ProcessArguments { get; set; }

        [Argument(StartProcessActionArgs.ProcessWorkingDirectory)] public string ProcessWorkingDirectory { get; set; }

        [Argument(StartProcessActionArgs.WaitForExit)] public bool WaitForExit { get; set; }

        [Argument(StartProcessActionArgs.WaitForExitTimeout)] public int WaitForExitTimeout { get; set; }

        public ActionResult Execute(ArgumentCollection arguments)
        {
            ArgumentCollection resultArgumentCollection = ArgumentCollection.New();
            try
            {
                #region process path

                var processPath = arguments.GetValue<string>(StartProcessActionExecutionArgs.ProcessPath);
                if (string.IsNullOrWhiteSpace(processPath))
                    processPath = ProcessPath;

                if(string.IsNullOrWhiteSpace(processPath))
                    throw new MissingArgumentException(StartProcessActionArgs.ProcessPath);

                processPath = Environment.ExpandEnvironmentVariables(processPath);

                if(Path.IsPathRooted(processPath) && !File.Exists(processPath))
                    throw new Exception($"The file set as process path doesn't exist, {processPath}");
                #endregion 

                #region process working directory

                var processWorkingDirectory = arguments.GetValue<string>(StartProcessActionExecutionArgs.ProcessWorkingDirectory);
                if (string.IsNullOrWhiteSpace(processWorkingDirectory))
                    processWorkingDirectory = ProcessWorkingDirectory;

                if (Path.IsPathRooted(processPath) && string.IsNullOrWhiteSpace(processWorkingDirectory))
                    processWorkingDirectory = Path.GetDirectoryName(processPath);

                //if (string.IsNullOrWhiteSpace(processWorkingDirectory))
                //    throw new MissingArgumentException(StartProcessActionArgs.ProcessWorkingDirectory);

                #endregion 

                #region process arguments

                var processArguments = arguments.GetValue<string>(StartProcessActionExecutionArgs.ProcessArguments);
                if (string.IsNullOrWhiteSpace(processArguments))
                    processArguments = ProcessArguments;

                #endregion

                System.Diagnostics.Process process = new System.Diagnostics.Process
                {
                    StartInfo =
                    {
                        FileName = processPath,
                        Arguments = processArguments,
                        CreateNoWindow = true,
                    }
                };

                if (!string.IsNullOrWhiteSpace(processWorkingDirectory))
                    process.StartInfo.WorkingDirectory = processWorkingDirectory;

                resultArgumentCollection = ArgumentCollection.New()
                    .WithArgument(StartProcessActionResultArgs.ProcessPath, processPath)
                    .WithArgument(StartProcessActionResultArgs.ProcessArguments, processArguments)
                    .WithArgument(StartProcessActionResultArgs.ProcessWorkingDirectory, processWorkingDirectory);

                if (!process.Start())
                {
                    resultArgumentCollection.Add(StartProcessActionResultArgs.ProcessStarted, false);
                    return ActionResult.Failed().WithAdditionInformation(resultArgumentCollection);
                }

                if (WaitForExit)
                {
                    var waitResult = process.WaitForExit(WaitForExitTimeout);
                    resultArgumentCollection.Add(StartProcessActionResultArgs.ProcessStarted, true);
                    resultArgumentCollection.Add(StartProcessActionResultArgs.ProcessExited, waitResult);
                    return ActionResult.Succeeded().WithAdditionInformation(resultArgumentCollection);
                }

                resultArgumentCollection.Add(StartProcessActionResultArgs.ProcessStarted, true);
                return ActionResult.Succeeded().WithAdditionInformation(resultArgumentCollection);
            }
            catch (Exception exception)
            {
                LoggingService.Error(exception);

                return ActionResult.Failed().WithAdditionInformation(resultArgumentCollection);
            }
        }
    }
}
