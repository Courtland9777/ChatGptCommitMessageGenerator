using System;
using System.Diagnostics;
using System.Threading.Tasks;
using ChatGptCommitMessageGenerator.Abstractions;
using Microsoft.VisualStudio.Threading;

namespace ChatGptCommitMessageGenerator.Services
{
    public class ProcessExecutor : IProcessExecutor
    {
        private readonly IOutputHandler _outputHandler;

        public ProcessExecutor(IOutputHandler outputHandler) =>
            _outputHandler = outputHandler ?? throw new ArgumentNullException(nameof(outputHandler));

        public async Task<string> ExecuteAsync(ProcessStartInfo startInfo)
        {
            using (var process = CreateProcess(startInfo))
            {
                process.Start();
                await process.WaitForExitAsync();

                return await HandleProcessOutputAsync(process);
            }
        }

        private static Process CreateProcess(ProcessStartInfo startInfo) =>
            new Process { StartInfo = startInfo };

        private async Task<string> HandleProcessOutputAsync(Process process)
        {
            var output = await process.StandardOutput.ReadToEndAsync();
            var errorOutput = await process.StandardError.ReadToEndAsync();

            return _outputHandler.HandleOutput(output, errorOutput);
        }
    }
}