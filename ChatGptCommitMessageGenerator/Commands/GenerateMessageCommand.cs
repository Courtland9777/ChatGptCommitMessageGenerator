using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using ChatGptCommitMessageGenerator.Abstractions;
using ChatGptCommitMessageGenerator.Helpers;
using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.Shell;
using static ChatGptCommitMessageGenerator.Helpers.GenerateMessageHelpers;
using static ChatGptCommitMessageGenerator.Helpers.GitDiffHelper;
using static ChatGptCommitMessageGenerator.Helpers.OutputHelpers;


namespace ChatGptCommitMessageGenerator.Commands
{
    [Command(" d0b258db-0f38-436f-93ca-e462bf2bd67c", 0x0100)]
    internal sealed class GenerateMessageCommand : BaseCommand<GenerateMessageCommand>
    {
        private readonly IHttpClientWrapper _httpClient;

        public GenerateMessageCommand() => _httpClient = new HttpClientWrapper(new HttpClient());

        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            try
            {
                var solution = await VS.Solutions.GetCurrentSolutionAsync();
                if (solution == null) throw new ArgumentNullException(nameof(solution));
                var workingDirectory = Path.GetDirectoryName($"{solution?.FullPath}");
                var diffFileContent = await CreateGitDiffAsync(workingDirectory);
                var diffForRequest = await OptimizeDiffForCommitMessageAsync(diffFileContent);
                if (string.IsNullOrWhiteSpace(diffForRequest)) throw new ArgumentNullException(nameof(diffForRequest));
                var commitMessage = await GetChatGptCommitMessageWithRetryAsync(_httpClient, diffForRequest);

                await WriteToOutputWindowAsync(commitMessage);
            }
            catch (Exception ex)
            {
                await ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
                {
                    await WriteErrorToOutputWindowAsync(ex);
                });
            }
        }
    }
}