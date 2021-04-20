using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Routindo.Contract;
using Routindo.Contract.Arguments;
using Routindo.Contract.Services;
using Routindo.Plugins.Process.Components.Actions;

namespace Routindo.Plugins.Process.Tests
{
    [TestClass]
    public class StartProcessActionTests
    {
        [TestMethod]
        [TestCategory("Integration Test")]
        public void StartProcessTest()
        {
            string processPath = @"%userprofile%\Desktop\processtest\sample.bat";
            StartProcessAction startProcessAction = new StartProcessAction()
            {
                Id = PluginUtilities.GetUniqueId(),
                LoggingService = ServicesContainer.ServicesProvider.GetLoggingService(nameof(StartProcessAction)),
                ProcessPath = Environment.ExpandEnvironmentVariables(processPath),
                WaitForExit = true,
                WaitForExitTimeout = 10000
            };

            var actionResult = startProcessAction.Execute(ArgumentCollection.New());
            Assert.IsNotNull(actionResult);
            Assert.IsTrue(actionResult.Result);
            Assert.IsNull(actionResult.AttachedException);
            Assert.IsNotNull(actionResult.AdditionalInformation);
            Assert.IsTrue(actionResult.AdditionalInformation.HasArgument(StartProcessActionResultArgs.ProcessPath));
            Assert.IsTrue(
                actionResult.AdditionalInformation.HasArgument(StartProcessActionResultArgs.ProcessArguments));
            Assert.IsTrue(
                actionResult.AdditionalInformation.HasArgument(StartProcessActionResultArgs.ProcessWorkingDirectory));
            Assert.IsTrue(actionResult.AdditionalInformation.HasArgument(StartProcessActionResultArgs.ProcessExited));
            Assert.IsTrue(actionResult.AdditionalInformation.HasArgument(StartProcessActionResultArgs.ProcessStarted));
            Assert.AreEqual(Environment.ExpandEnvironmentVariables(processPath),
                actionResult.AdditionalInformation.GetValue<string>(StartProcessActionResultArgs.ProcessPath));
            Assert.AreEqual(Path.GetDirectoryName(Environment.ExpandEnvironmentVariables(processPath)),
                actionResult.AdditionalInformation.GetValue<string>(
                    StartProcessActionResultArgs.ProcessWorkingDirectory));
            Assert.IsTrue(string.IsNullOrWhiteSpace(
                actionResult.AdditionalInformation.GetValue<string>(StartProcessActionResultArgs.ProcessArguments)));
            Assert.AreEqual(true,
                actionResult.AdditionalInformation.GetValue<bool>(StartProcessActionResultArgs.ProcessStarted));
            Assert.AreEqual(true,
                actionResult.AdditionalInformation.GetValue<bool>(StartProcessActionResultArgs.ProcessExited));
        }

        [TestMethod]
        [TestCategory("Integration Test")]
        public void StartProcessNotepadTest() 
        {
            string processPath = @"notepad";
            StartProcessAction startProcessAction = new StartProcessAction()
            {
                Id = PluginUtilities.GetUniqueId(),
                LoggingService = ServicesContainer.ServicesProvider.GetLoggingService(nameof(StartProcessAction)),
                ProcessPath = processPath,
            };

            var actionResult = startProcessAction.Execute(ArgumentCollection.New());
            Assert.IsNotNull(actionResult);
            Assert.IsTrue(actionResult.Result);

            System.Diagnostics.Process.GetProcessesByName(processPath).FirstOrDefault()?.Kill();
        }
    }
}
