using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using ChatGptCommitMessageGenerator.Abstractions;
using ChatGptCommitMessageGenerator.Commands;
using ChatGptCommitMessageGenerator.Services;
using ChatGptCommitMessageGenerator.TokenManagement;
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

        protected override void InitializeServices(IServiceCollection services)
        {
            services.AddSingleton<ITokenManager, DeepDevTokenManager>();
            services.AddSingleton<IGptApiClient, GptApiClient>();
            services.AddSingleton<IGitCommitMessageGenerator>(serviceProvider =>
            {
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
                var gptApiClient = new GptApiClient(httpClient);
                return new GitCommitMessageGenerator(gptApiClient);
            });
            services.AddSingleton<IGitDiffParser, GitDiffParser>();
            services.AddSingleton<IGitDiffProvider, GitDiffProvider>();

            services.AddSingleton<GenerateMessageCommand>();
        }
    }
}