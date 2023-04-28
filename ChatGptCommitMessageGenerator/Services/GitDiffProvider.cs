using System.Diagnostics;
using System.Threading.Tasks;
using ChatGptCommitMessageGenerator.Abstractions;

namespace ChatGptCommitMessageGenerator.Services
{
    public class GitDiffProvider : IGitDiffProvider
    {
        private readonly IProcessExecutor _processExecutor;

        public GitDiffProvider(IProcessExecutor processExecutor) => _processExecutor = processExecutor;

        public async Task<string> GetGitDiffAsync(string repositoryPath)
        {
            if (repositoryPath.Equals(string.Empty)) return string.Empty;
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

            var output = await _processExecutor.ExecuteAsync(startInfo);

            return output;
        }
    }
}