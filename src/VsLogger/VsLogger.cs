using DMS.GLSL.Contracts;

using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading.Tasks;

namespace DMS.GLSL.VsLogger
{
    [Export(typeof(ILogger))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class VsLogger : ILogger
    {
        private readonly string logFileName;
        private static readonly object _lock = new object();

        public string LoggerInfo => $"Writing logger output to {logFileName}.";

        public VsLogger()
        {
            logFileName = Path.Combine(Path.GetTempPath(), "GLSL VSX language extension.log");
            Log($"Logging to {logFileName}.");
        }

        public async Task LogAsync(string message, bool highPriority = false)
        {
            lock (_lock)
            {
                File.AppendAllText(logFileName, $"[{DateTime.Now:MM.d HH:mm:ss.fff}] {message} \n");
            }
            await VsOutput.WindowPaneAsync(message + '\n').ConfigureAwait(false);
            if (highPriority)
            {
                await VsOutput.StatusBarAsync($"{DateTime.Now:HH:mm:ss.fff} {message}").ConfigureAwait(false);
            }
        }

        public void Log(string message, bool highPriority = false)
        {
            lock (_lock)
            {
                File.AppendAllText(logFileName, $"[{DateTime.Now:MM.d HH:mm:ss.fff}] {message} \n");
            }
            VsOutput.WindowPane(message + '\n');
            if (highPriority)
            {
                VsOutput.StatusBar($"{DateTime.Now:HH:mm:ss.fff} {message}");
            }
        }
    }
}
