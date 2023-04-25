using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ChatGptCommitMessageGenerator.Abstractions;
using ChatGptCommitMessageGenerator.Services;
using ChatGptCommitMessageGenerator.TokenManagement;
using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.Shell;
using static ChatGptCommitMessageGenerator.Services.OutputManager;

namespace ChatGptCommitMessageGenerator.Commands
{
    [Command(" d0b258db-0f38-436f-93ca-e462bf2bd67c", 0x0100)]
    internal sealed class GenerateMessageCommand : BaseCommand<GenerateMessageCommand>
    {
        private readonly ITokenManager _deepDev;
        private readonly IGitCommitMessageGenerator _gitCommitMessageGenerator;
        private readonly IGitDiffParser _gitDiffParser;
        private readonly IGitDiffProvider _gitDiffProvider;
        private readonly HttpClient _httpClient;

        public GenerateMessageCommand()
        {
            _httpClient = new HttpClient();
            _deepDev = new DeepDevTokenManager();
            IGptApiClient gptApiClient = new GptApiClient(_httpClient);
            _gitCommitMessageGenerator = new GitCommitMessageGenerator(gptApiClient);
            _gitDiffProvider = new GitDiffProvider();
            _gitDiffParser = new GitDiffParser();
        }

        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
                var solution = await VS.Solutions.GetCurrentSolutionAsync() ??
                               throw new InvalidOperationException("No solution is open");
                var workingDirectory = Path.GetDirectoryName($"{solution?.FullPath}");

                var gitDiff = await _gitDiffProvider.GetGitDiffAsync(workingDirectory);
                var gitDiffChanges = await _gitDiffParser.ExtractGitDiffChangesAsync(gitDiff);
                if (gitDiffChanges.Count == 0)
                {
                    await WriteNoChangesMessageToOutputWindowAsync();
                    return;
                }

                var gitChanges = await _deepDev.GroupStringsAsync(gitDiffChanges);

                var responseMessage = await _gitCommitMessageGenerator.GetResponseMessageAsync(gitChanges);

                await WriteCommitMessageToOutputWindowAsync(responseMessage);
                await ShowCommitMessageAsync(responseMessage);
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