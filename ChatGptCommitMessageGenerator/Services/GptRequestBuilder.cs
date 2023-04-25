using System;
using System.Collections.Generic;
using ChatGptCommitMessageGenerator.Models;

namespace ChatGptCommitMessageGenerator.Services
{
    public class GptRequestBuilder
    {
        public static Request Build(string gitDiff, Prompts.Prompts promptSelection)
        {
            var promptBuilders = new Dictionary<Prompts.Prompts, Func<string, Request>>
            {
                { Prompts.Prompts.SinglePass, CreateSinglePassRequest },
                { Prompts.Prompts.MultiplePassesStart, CreateMultiplePassesStartRequest },
                { Prompts.Prompts.MultiplePassesEnd, CreateMultiplePassesEndRequest }
            };

            if (!promptBuilders.TryGetValue(promptSelection, out var promptBuilder))
                throw new ArgumentOutOfRangeException(nameof(promptSelection), promptSelection, null);

            return promptBuilder(gitDiff);
        }

        private static Request CreateRequest(string gitDiff, string systemMessage, string userMessage)
        {
            return new Request
            {
                Messages = new[]
                {
                    new RequestMessage { Role = "system", Content = systemMessage },
                    new RequestMessage { Role = "user", Content = string.Format(userMessage, gitDiff) }
                }
            };
        }

        private static Request CreateSinglePassRequest(string gitDiff)
        {
            const string systemMessage =
                "You are a helpful assistant that can summarize git diffs and suggest commit messages.";
            const string userMessage =
                "In up to two sentences, create a concise and informative commit message that summarizes the following diff:\n{0}\nCommit message:";

            return CreateRequest(gitDiff, systemMessage, userMessage);
        }

        private static Request CreateMultiplePassesStartRequest(string gitDiff)
        {
            const string systemMessage = "You are a helpful assistant that can summarize git diffs.";
            const string userMessage =
                "A git diff has been parsed using ```csharp Regex(@\"^@@ -\\d+(,\\d+)? \\+(\\d+)(,\\d+)? @@.*$\", RegexOptions.Multiline)```. For each match, In one sentence, create a concise and informative message that summarizes the following matches:\n{0}\n match messages:";

            return CreateRequest(gitDiff, systemMessage, userMessage);
        }

        private static Request CreateMultiplePassesEndRequest(string gitDiff)
        {
            const string systemMessage = "You are a helpful assistant that can summarize git diffs.";
            const string userMessage =
                "In up to two sentences, create a concise, professional and informative commit message that summarizes the following change descriptions Changes:\n{0}\nCommit message:";

            return CreateRequest(gitDiff, systemMessage, userMessage);
        }
    }
}