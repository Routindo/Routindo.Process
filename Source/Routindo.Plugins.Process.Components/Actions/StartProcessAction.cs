using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Routindo.Contract.Actions;
using Routindo.Contract.Arguments;
using Routindo.Contract.Exceptions;
using Routindo.Contract.Services;
using Exception = System.Exception;

namespace Routindo.Plugins.Process.Components.Actions
{
    public class StartProcessAction: IAction
    {
        public string Id { get; set; }
        public ILoggingService LoggingService { get; set; }

        public string ProcessPath { get; set; }

        public string ProcessArguments { get; set; }

        public string ProcessWorkingDirectory { get; set; }

        public bool WaitForExit { get; set; }

        public int WaitForExitTimeout { get; set; }

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

                if(!File.Exists(processPath))
                    throw new Exception($"The file set as process path doesn't exist, {processPath}");
                #endregion 

                #region process working directory

                var processWorkingDirectory = arguments.GetValue<string>(StartProcessActionExecutionArgs.ProcessWorkingDirectory);
                if (string.IsNullOrWhiteSpace(processWorkingDirectory))
                    processWorkingDirectory = ProcessWorkingDirectory;

                if (string.IsNullOrWhiteSpace(processWorkingDirectory))
                    processWorkingDirectory = Path.GetDirectoryName(processPath);

                if (string.IsNullOrWhiteSpace(processWorkingDirectory))
                    throw new MissingArgumentException(StartProcessActionArgs.ProcessWorkingDirectory);

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
                        WorkingDirectory = processWorkingDirectory,
                        CreateNoWindow = true,
                    }
                };

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

    public static class StartProcessActionArgs
    {
        public const string ProcessPath = nameof(ProcessPath);
        public const string ProcessArguments = nameof(ProcessArguments);
        public const string ProcessWorkingDirectory = nameof(ProcessWorkingDirectory);
        public const string WaitForExit = nameof(WaitForExit);
        public const string WaitForExitTimeout = nameof(WaitForExitTimeout);
    } 

    public static class StartProcessActionExecutionArgs
    {
        public const string ProcessPath = nameof(ProcessPath);
        public const string ProcessArguments = nameof(ProcessArguments);
        public const string ProcessWorkingDirectory = nameof(ProcessWorkingDirectory);
    }

    public static class StartProcessActionResultArgs
    {
        public const string ProcessPath = nameof(ProcessPath);
        public const string ProcessArguments = nameof(ProcessArguments);
        public const string ProcessWorkingDirectory = nameof(ProcessWorkingDirectory);
        public const string ProcessExited = nameof(ProcessExited);
        public const string ProcessStarted = nameof(ProcessStarted);
    }
}
