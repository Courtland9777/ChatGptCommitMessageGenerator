using System;
using System.Reflection;
using ChatGptCommitMessageGenerator.Models;
using ChatGptCommitMessageGenerator.Services;
using Xunit;

namespace ChatGptCommitMessageGenerator.Test.ServiceTests
{
    public class GptRequestBuilderTests
    {
        private static string GenerateMockGitDiff() =>
            "@@ -1,3 +1,9 @@\n" +
            "+using System;\n" +
            "+using System.Collections.Generic;\n" +
            "+using System.Text;\n" +
            " using ChatGptCommitMessageGenerator.Models;\n" +
            " using Newtonsoft.Json;\n" +
            " \n";

        [Fact]
        public void Build_SinglePass_ReturnsValidRequest()
        {
            // Arrange
            var gitDiff = GenerateMockGitDiff();
            const Prompts.Prompts promptSelection = Prompts.Prompts.SinglePass;

            // Act
            var result = GptRequestBuilder.Build(gitDiff, promptSelection);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Messages.Length);
            Assert.Equal("system", result.Messages[0].Role);
            Assert.Equal("user", result.Messages[1].Role);
            Assert.Contains("You are a helpful assistant that can summarize git diffs and suggest commit messages.",
                result.Messages[0].Content);
            Assert.Contains("In up to two sentences, create a concise and informative commit message",
                result.Messages[1].Content);
        }

        [Fact]
        public void Build_MultiplePassesStart_ReturnsValidRequest()
        {
            // Arrange
            var gitDiff = GenerateMockGitDiff();
            const Prompts.Prompts promptSelection = Prompts.Prompts.MultiplePassesStart;

            // Act
            var result = GptRequestBuilder.Build(gitDiff, promptSelection);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Messages.Length);
            Assert.Equal("system", result.Messages[0].Role);
            Assert.Equal("user", result.Messages[1].Role);
            Assert.Contains("You are a helpful assistant that can summarize git diffs.", result.Messages[0].Content);
            Assert.Contains("A git diff has been parsed", result.Messages[1].Content);
        }

        [Fact]
        public void Build_InvalidPrompt_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var gitDiff = GenerateMockGitDiff();
            const Prompts.Prompts invalidPromptSelection = (Prompts.Prompts)99;

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => GptRequestBuilder.Build(gitDiff, invalidPromptSelection));
        }

        [Fact]
        public void CreateRequest_ValidInputs_ReturnsCorrectRequest()
        {
            // Arrange
            var gitDiff = GenerateMockGitDiff();
            const string systemMessage = "Test system message.";
            const string userMessage = "Test user message with diff: {0}";

            // Use Reflection to get a reference to the CreateRequest method
            var createRequestMethod = typeof(GptRequestBuilder)
                .GetMethod("CreateRequest", BindingFlags.NonPublic | BindingFlags.Static);

            // Act
            var result =
                (Request)createRequestMethod?.Invoke(null, new object[] { gitDiff, systemMessage, userMessage });

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Messages.Length);
            Assert.Equal("system", result.Messages[0].Role);
            Assert.Equal("user", result.Messages[1].Role);
            Assert.Equal(systemMessage, result.Messages[0].Content);
            Assert.Equal(string.Format(userMessage, gitDiff), result.Messages[1].Content);
        }
    }
}