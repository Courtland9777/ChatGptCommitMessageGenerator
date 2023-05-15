using System;
using System.ComponentModel.Design;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using ChatGptCommitMessageGenerator.Abstractions;
using ChatGptCommitMessageGenerator.Commands;
using ChatGptCommitMessageGenerator.Services;
using ChatGptCommitMessageGenerator.TokenManagement;
using Community.VisualStudio.Toolkit;
using Community.VisualStudio.Toolkit.DependencyInjection.Microsoft;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Shell;

namespace ChatGptCommitMessageGenerator
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(PackageGuidString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    public sealed class
        ChatGptCommitMessageGeneratorPackage : MicrosoftDIToolkitPackage<ChatGptCommitMessageGeneratorPackage>
    {
        public const string PackageGuidString = "d8e4b240-c322-4352-a821-a9bd396da863";
        private const int GenerateMessageCommandId = 0x0100;
        private OleMenuCommand _generateMessageCommand;

        protected override void InitializeServices(IServiceCollection services)
        {
            services.AddSingleton<ITokenManager, DeepDevTokenManager>();
            services.AddSingleton<IGitCommitMessageGenerator>(serviceProvider =>
            {
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
                var gptApiClient = new GptApiClient(httpClient);
                return new GitCommitMessageGenerator(gptApiClient);
            });
            services.AddSingleton<IGitDiffParser, GitDiffParser>();
            services.AddSingleton<IGitDiffProvider>(serviceProvider =>
            {
                var processExecutor = new ProcessExecutor(new DefaultOutputHandler());
                return new GitDiffProvider(processExecutor);
            });

            services.AddSingleton<GenerateMessageCommand>();
        }

        protected override async Task InitializeAsync(CancellationToken cancellationToken,
            IProgress<ServiceProgressData> progress)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync();
            VS.Events.SolutionEvents.OnAfterOpenSolution += OnSolutionOpened;
            VS.Events.SolutionEvents.OnAfterCloseSolution += OnSolutionAfterClosing;

            using (var commandService = await GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService ??
                                        throw new ArgumentNullException())
            {
                _generateMessageCommand = new OleMenuCommand(null,
                    new CommandID(Guid.Parse("guidChatGptCommitMessageGeneratorPackageCmdSet"),
                        GenerateMessageCommandId));
                commandService.AddCommand(_generateMessageCommand);
            }
        }

        private void OnSolutionOpened(Solution solution)
        {
            _generateMessageCommand.Enabled = true;
        }

        private void OnSolutionAfterClosing()
        {
            _generateMessageCommand.Enabled = false;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing) return;
            // Unsubscribe from the events when the package is disposed
            VS.Events.SolutionEvents.OnAfterOpenSolution -= OnSolutionOpened;
            VS.Events.SolutionEvents.OnAfterCloseSolution -= OnSolutionAfterClosing;
        }
    }
}