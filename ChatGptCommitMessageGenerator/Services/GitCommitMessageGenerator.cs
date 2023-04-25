using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatGptCommitMessageGenerator.Abstractions;
using ChatGptCommitMessageGenerator.Models;

namespace ChatGptCommitMessageGenerator.Services
{
    public class GitCommitMessageGenerator : IGitCommitMessageGenerator
    {
        private readonly IGptApiClient _gptApiClient;

        public GitCommitMessageGenerator(IGptApiClient gptApiClient) => _gptApiClient = gptApiClient;

        public async Task<string> GetResponseMessageAsync(List<GitChange> gitChanges)
        {
            if (gitChanges.All(c => c.GroupNumber == 0))
                return await PostGitChangesToChatGptAsync(
                    string.Join(string.Empty, gitChanges.Select(c => c.Change)),
                    Prompts.Prompts.SinglePass
                ).ConfigureAwait(false);

            var commitMessageBuilder = new StringBuilder();
            foreach (var groupedChanges in gitChanges.GroupBy(x => x.GroupNumber))
            {
                var changes = string.Join(string.Empty, groupedChanges.Select(c => c.Change));
                var commitChangesSummaries = await PostGitChangesToChatGptAsync(
                    changes,
                    Prompts.Prompts.MultiplePassesStart
                ).ConfigureAwait(false);
                commitMessageBuilder.AppendLine(commitChangesSummaries);
            }

            return await PostGitChangesToChatGptAsync(
                commitMessageBuilder.ToString().TrimEnd(),
                Prompts.Prompts.MultiplePassesEnd
            ).ConfigureAwait(false);
        }

        public async Task<string> PostGitChangesToChatGptAsync(string gitDiff, Prompts.Prompts promptSelection)
        {
            var requestData = GptRequestBuilder.Build(gitDiff, promptSelection);

            var response = await _gptApiClient.PostAsync(requestData).ConfigureAwait(false);
            return response.Choices[0].Message.Content;
        }
    }
}