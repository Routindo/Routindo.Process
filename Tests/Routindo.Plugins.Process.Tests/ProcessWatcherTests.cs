using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Routindo.Contract;
using Routindo.Contract.Exceptions;
using Routindo.Contract.Services;
using Routindo.Contract.Watchers;
using Routindo.Plugins.Process.Components.Watchers;

namespace Routindo.Plugins.Process.Tests
{
    [TestClass]
    public class ProcessWatcherTests
    {
        [TestMethod]
        [TestCategory("Integration Test")]
        public void WatchProcessNotepadTest()
        {
            IWatcher watcher = new ProcessWatcher()
            {
                Id = PluginUtilities.GetUniqueId(),
                LoggingService = ServicesContainer.ServicesProvider.GetLoggingService(nameof(ProcessWatcher)),
                ProcessName = "notepad"
            };

            var watcherResult = watcher.Watch();
            Assert.IsNotNull(watcherResult);
            Assert.IsFalse(watcherResult.Result);

            var process = System.Diagnostics.Process.Start("notepad");
            Assert.IsNotNull(process);
            Thread.Sleep(1000);

            watcherResult = watcher.Watch();
            Assert.IsNotNull(watcherResult);
            Assert.IsTrue(watcherResult.Result);
            Assert.IsTrue(watcherResult.WatchingArguments.HasArgument(ProcessWatcherResultArgs.ProcessName));
            Assert.IsTrue(watcherResult.WatchingArguments.HasArgument(ProcessWatcherResultArgs.ProcessId));
            Assert.AreEqual(process.ProcessName, watcherResult.WatchingArguments.GetValue<string>(ProcessWatcherResultArgs.ProcessName));
            Assert.AreEqual(process.Id, watcherResult.WatchingArguments.GetValue<int>(ProcessWatcherResultArgs.ProcessId));

            process.Kill();
        }

        [TestMethod]
        [TestCategory("Integration Test")]
        public void WatchProcessFailsOnNoNameTest()
        {
            IWatcher watcher = new ProcessWatcher()
            {
                Id = PluginUtilities.GetUniqueId(),
                LoggingService = ServicesContainer.ServicesProvider.GetLoggingService(nameof(ProcessWatcher)),
            };

            var watcherResult = watcher.Watch();
            Assert.IsNotNull(watcherResult);
            Assert.IsFalse(watcherResult.Result);
            Assert.IsNotNull(watcherResult.AttachedException);
            Assert.AreEqual(typeof(MissingArgumentException), watcherResult.AttachedException.GetType());
        }
    }
}
