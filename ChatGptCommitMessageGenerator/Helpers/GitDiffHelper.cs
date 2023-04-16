using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Threading;

namespace ChatGptCommitMessageGenerator.Helpers
{
    public static class GitDiffHelper
    {
        public static async Task<string> CreateGitDiffAsync(string repositoryPath)
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

                var output = await process.StandardOutput.ReadToEndAsync();
                var errorOutput = await process.StandardError.ReadToEndAsync();

                if (!string.IsNullOrEmpty(errorOutput))
                    throw new InvalidOperationException($"Error creating Git diff: {errorOutput}");

                await process.WaitForExitAsync();

                return output;
            }
        }


        public static async Task<string> OptimizeDiffForCommitMessageAsync(string diff)
        {
            return await Task.Run(() =>
            {
                if (string.IsNullOrEmpty(diff))
                    throw new ArgumentException("Diff cannot be null or empty.", nameof(diff));

                var lines = diff.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                var optimizedLines = new List<string>();
                var changeStatusPattern = new Regex(@"^(Added|Deleted|Modified):\s+(.*)$");

                foreach (var line in lines)
                    if (changeStatusPattern.IsMatch(line))
                        optimizedLines.Add(line);
                    else if (line.StartsWith("+") && !line.StartsWith("++"))
                        optimizedLines.Add($"Added line: {line.Substring(1).Trim()}");
                    else if (line.StartsWith("-") && !line.StartsWith("--"))
                        optimizedLines.Add($"Removed line: {line.Substring(1).Trim()}");

                return string.Join(Environment.NewLine, optimizedLines);
            });
        }
    }
}