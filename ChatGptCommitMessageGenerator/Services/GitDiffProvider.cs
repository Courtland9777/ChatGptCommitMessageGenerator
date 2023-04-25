using System;
using System.Diagnostics;
using System.Threading.Tasks;
using ChatGptCommitMessageGenerator.Abstractions;
using Microsoft.VisualStudio.Threading;

namespace ChatGptCommitMessageGenerator.Services
{
    public class GitDiffProvider : IGitDiffProvider
    {
        public async Task<string> GetGitDiffAsync(string repositoryPath)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = "diff HEAD",
                WorkingDirectory = repositoryPath,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = new Process { StartInfo = startInfo })
            {
                process.Start();

                await process.WaitForExitAsync();

                var output = await process.StandardOutput.ReadToEndAsync();
                var errorOutput = await process.StandardError.ReadToEndAsync();

                if (!string.IsNullOrEmpty(errorOutput))
                    throw new InvalidOperationException($"Error creating Git diff: {errorOutput}");

                return output;
            }
        }
    }
}