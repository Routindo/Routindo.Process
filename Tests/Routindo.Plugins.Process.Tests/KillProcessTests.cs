using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Routindo.Contract;
using Routindo.Contract.Arguments;
using Routindo.Contract.Services;
using Routindo.Plugins.Process.Components.Actions;

namespace Routindo.Plugins.Process.Tests
{
    [TestClass]
    public class KillProcessTests
    {
        [TestMethod]
        [TestCategory("Integration Test")]
        public void KillProcessByNameFromInstanceArgsTest()
        {
            var processId = StartNotepad();
            Assert.AreNotEqual(0, processId);
            Console.WriteLine($"Created Process with ID {processId}");
            Thread.Sleep(1000);
            KillProcessByNameAction action = new KillProcessByNameAction()
            {
                Id = PluginUtilities.GetUniqueId(),
                LoggingService = ServicesContainer.ServicesProvider.GetLoggingService(nameof(KillProcessByNameAction)),
                ProcessName = "notepad",
            };

            var actionResult = action.Execute(ArgumentCollection.New());
            Assert.IsNotNull(actionResult);
            Assert.IsTrue(actionResult.Result);
            Assert.IsTrue(actionResult.AdditionalInformation.HasArgument(KillProcessActionResultArgs.ProcessesKilledSuccessfully));
            Assert.AreEqual(1, actionResult.AdditionalInformation.GetValue<List<int>>(KillProcessActionResultArgs.ProcessesKilledSuccessfully).Count);
            Assert.AreEqual(processId, actionResult.AdditionalInformation.GetValue<List<int>>(KillProcessActionResultArgs.ProcessesKilledSuccessfully).Single());
            Thread.Sleep(2000);
            actionResult = action.Execute(ArgumentCollection.New());
            Assert.IsNotNull(actionResult);
            Assert.IsFalse(actionResult.Result);
        }

        [TestMethod]
        [TestCategory("Integration Test")]
        public void KillProcessByNameFromExecutionArgsTest()
        {
            var processId = StartNotepad();
            Assert.AreNotEqual(0, processId);
            Console.WriteLine($"Created Process with ID {processId}");
            Thread.Sleep(1000);
            KillProcessByNameAction action = new KillProcessByNameAction()
            {
                Id = PluginUtilities.GetUniqueId(),
                LoggingService = ServicesContainer.ServicesProvider.GetLoggingService(nameof(KillProcessByNameAction)),
            };

            var actionResult = action.Execute(ArgumentCollection.New()
                .WithArgument(KillProcessByNameActionExecutionArgs.ProcessName, "notepad")
            );
            Assert.IsNotNull(actionResult);
            Assert.IsTrue(actionResult.Result);
            Assert.IsTrue(
                actionResult.AdditionalInformation.HasArgument(KillProcessActionResultArgs
                    .ProcessesKilledSuccessfully));
            Assert.AreEqual(1,
                actionResult.AdditionalInformation
                    .GetValue<List<int>>(KillProcessActionResultArgs.ProcessesKilledSuccessfully).Count);
            Assert.AreEqual(processId,
                actionResult.AdditionalInformation
                    .GetValue<List<int>>(KillProcessActionResultArgs.ProcessesKilledSuccessfully).Single());
            Thread.Sleep(2000);
            actionResult = action.Execute(ArgumentCollection.New()
                .WithArgument(KillProcessByNameActionExecutionArgs.ProcessName, "notepad")
            );
            Assert.IsNotNull(actionResult);
            Assert.IsFalse(actionResult.Result);
        }

        [TestMethod]
        [TestCategory("Integration Test")]
        public void KillProcessByIdTest() 
        {
            var processId = StartNotepad();
            Assert.AreNotEqual(0, processId);
            Console.WriteLine($"Created Process with ID {processId}");
            Thread.Sleep(1000);
            KillProcessByIdAction action = new KillProcessByIdAction()
            {
                Id = PluginUtilities.GetUniqueId(),
                LoggingService = ServicesContainer.ServicesProvider.GetLoggingService(nameof(KillProcessByNameAction))
            };

            var actionResult = action.Execute(ArgumentCollection.New()
                .WithArgument(KillProcessByIdActionExecutionArgs.ProcessId, processId)
            );
            Assert.IsNotNull(actionResult);
            Assert.IsTrue(actionResult.Result);
            Assert.IsTrue(
                actionResult.AdditionalInformation.HasArgument(KillProcessActionResultArgs
                    .ProcessesKilledSuccessfully));
            Assert.AreEqual(1,
                actionResult.AdditionalInformation
                    .GetValue<List<int>>(KillProcessActionResultArgs.ProcessesKilledSuccessfully).Count);
            Assert.AreEqual(processId,
                actionResult.AdditionalInformation
                    .GetValue<List<int>>(KillProcessActionResultArgs.ProcessesKilledSuccessfully).Single());
            Thread.Sleep(2000);

            actionResult = action.Execute(ArgumentCollection.New()
                .WithArgument(KillProcessByIdActionExecutionArgs.ProcessId, processId));
            Assert.IsNotNull(actionResult);

            if (actionResult.Result)
            {
                Assert.IsTrue(
                    actionResult.AdditionalInformation.HasArgument(KillProcessActionResultArgs.ProcessesKillingFailed));
                Assert.AreEqual(1,
                    actionResult.AdditionalInformation
                        .GetValue<List<int>>(KillProcessActionResultArgs.ProcessesKillingFailed).Count);
                Assert.AreEqual(processId,
                    actionResult.AdditionalInformation
                        .GetValue<List<int>>(KillProcessActionResultArgs.ProcessesKillingFailed).Single());
            }
        }

        [TestMethod]
        [TestCategory("Integration Test")]
        public void KillProcessByIdNotExistTest()
        {
            var processId = int.MaxValue;
            Assert.AreNotEqual(0, processId);
            Console.WriteLine($"Created Process with ID {processId}");
            Thread.Sleep(1000);
            KillProcessByIdAction action = new KillProcessByIdAction()
            {
                Id = PluginUtilities.GetUniqueId(),
                LoggingService = ServicesContainer.ServicesProvider.GetLoggingService(nameof(KillProcessByNameAction))
            };

            var actionResult = action.Execute(ArgumentCollection.New()
                .WithArgument(KillProcessByIdActionExecutionArgs.ProcessId, processId)
            );
            Assert.IsNotNull(actionResult);
            Assert.IsFalse(actionResult.Result);
            Assert.IsNotNull(actionResult.AttachedException);
            
        }

        private int StartNotepad()
        {
            var process = System.Diagnostics.Process.Start("notepad.exe");
            if (process != null) return process.Id;
            return 0;
        }
    }
}
