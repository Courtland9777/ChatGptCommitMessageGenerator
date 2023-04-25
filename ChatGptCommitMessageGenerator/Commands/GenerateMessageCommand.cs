using System;
using System.IO;
using System.Threading.Tasks;
using ChatGptCommitMessageGenerator.Abstractions;
using Community.VisualStudio.Toolkit;
using Community.VisualStudio.Toolkit.DependencyInjection;
using Community.VisualStudio.Toolkit.DependencyInjection.Core;
using Microsoft.VisualStudio.Shell;
using static ChatGptCommitMessageGenerator.Services.OutputManager;

namespace ChatGptCommitMessageGenerator.Commands
{
    [Command(CommandGuid, CommandId)]
    internal sealed class GenerateMessageCommand : BaseDICommand
    {
        private const string CommandGuid = "d0b258db-0f38-436f-93ca-e462bf2bd67c";
        private const int CommandId = 0x0100;

        private readonly ITokenManager _deepDev;
        private readonly IGitCommitMessageGenerator _gitCommitMessageGenerator;
        private readonly IGitDiffParser _gitDiffParser;
        private readonly IGitDiffProvider _gitDiffProvider;

        public GenerateMessageCommand(
            DIToolkitPackage package,
            ITokenManager tokenManager,
            IGitCommitMessageGenerator gitCommitMessageGenerator,
            IGitDiffParser gitDiffParser,
            IGitDiffProvider gitDiffProvider)
            : base(package)
        {
            _deepDev = tokenManager;
            _gitCommitMessageGenerator = gitCommitMessageGenerator;
            _gitDiffParser = gitDiffParser;
            _gitDiffProvider = gitDiffProvider;
        }

        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            try
            {
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