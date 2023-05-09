using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using ChatGptCommitMessageGenerator.Abstractions;
using ChatGptCommitMessageGenerator.Exceptions;
using ChatGptCommitMessageGenerator.Models;
using ChatGptCommitMessageGenerator.Services;
using Moq;
using Xunit;

namespace ChatGptCommitMessageGenerator.Test.ServiceTests
{
    public class GitCommitMessageGeneratorTests
    {
        [Fact]
        public void Constructor_Initializes_GptApiClient()
        {
            // Arrange
            var gptApiClientMock = new Mock<IGptApiClient>();

            // Act
            var gitCommitMessageGenerator = new GitCommitMessageGenerator(gptApiClientMock.Object);

            // Assert
            Assert.NotNull(gitCommitMessageGenerator);
        }

        [Fact]
        public async Task GetResponseMessageAsync_AllGroupNumberZero_ReturnsSinglePassResponseAsync()
        {
            // Arrange
            var gitChanges = new List<GitChange>
            {
                new GitChange { GroupNumber = 0, Change = "Change 1", TokenCount = 5 },
                new GitChange { GroupNumber = 0, Change = "Change 2", TokenCount = 8 }
            };

            var gptApiClientMock = new Mock<IGptApiClient>();
            gptApiClientMock.Setup(client => client.PostAsync(
                    It.IsAny<Request>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()))
                .ReturnsAsync(new Response
                {
                    Choices = new[]
                    {
                        new ResponseChoice { Message = new ResponseMessage { Content = "Single Pass Response" } }
                    }
                });

            var gitCommitMessageGenerator = new GitCommitMessageGenerator(gptApiClientMock.Object);

            // Act
            var response = await gitCommitMessageGenerator.GetResponseMessageAsync(gitChanges);

            // Assert
            Assert.Equal("Single Pass Response", response);
        }

        [Fact]
        public async Task GetResponseMessageAsync_MultipleGroupNumbers_ReturnsExpectedResponseAsync()
        {
            // Arrange
            var gitChanges = new List<GitChange>
            {
                new GitChange { GroupNumber = 1, Change = "Change 1", TokenCount = 5 },
                new GitChange { GroupNumber = 2, Change = "Change 2", TokenCount = 8 }
            };

            var gptApiClientMock = new Mock<IGptApiClient>();

            gptApiClientMock.SetupSequence(client => client.PostAsync(
                    It.IsAny<Request>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()))
                .ReturnsAsync(new Response
                {
                    Choices = new[]
                    {
                        new ResponseChoice { Message = new ResponseMessage { Content = "Change 1 Summary" } }
                    }
                })
                .ReturnsAsync(new Response
                {
                    Choices = new[]
                    {
                        new ResponseChoice { Message = new ResponseMessage { Content = "Change 2 Summary" } }
                    }
                })
                .ReturnsAsync(new Response
                {
                    Choices = new[]
                    {
                        new ResponseChoice { Message = new ResponseMessage { Content = "Final Commit Message" } }
                    }
                });

            var gitCommitMessageGenerator = new GitCommitMessageGenerator(gptApiClientMock.Object);

            // Act
            var response = await gitCommitMessageGenerator.GetResponseMessageAsync(gitChanges);

            // Assert
            Assert.Equal("Final Commit Message", response);
        }

        [Fact]
        public async Task GetResponseMessageAsync_EmptyGitChanges_ReturnsEmptyStringAsync()
        {
            // Arrange
            var gitChanges = new List<GitChange>();
            var gptApiClientMock = new Mock<IGptApiClient>();

            var gitCommitMessageGenerator = new GitCommitMessageGenerator(gptApiClientMock.Object);

            // Act
            var response = await gitCommitMessageGenerator.GetResponseMessageAsync(gitChanges);

            // Assert
            Assert.Equal(string.Empty, response);
        }

        [Fact]
        public async Task GetResponseMessageAsync_NullGitChanges_ThrowsArgumentNullExceptionAsync()
        {
            // Arrange
            var gptApiClientMock = new Mock<IGptApiClient>();

            var gitCommitMessageGenerator = new GitCommitMessageGenerator(gptApiClientMock.Object);

            // Act and Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                gitCommitMessageGenerator.GetResponseMessageAsync(null));
        }

        [Theory]
        [InlineData("Added new feature", "New Feature Summary")]
        [InlineData("Fixed a bug", "Bug Fix Summary")]
        public async Task PostGitChangesToChatGptAsync_DifferentGitDiff_ReturnsExpectedResponseAsync(string gitDiff,
            string expectedResponse)
        {
            // Arrange
            var gptApiClientMock = new Mock<IGptApiClient>();
            gptApiClientMock.Setup(client => client.PostAsync(
                    It.IsAny<Request>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()))
                .ReturnsAsync(new Response
                {
                    Choices = new[]
                    {
                        new ResponseChoice { Message = new ResponseMessage { Content = expectedResponse } }
                    }
                });

            var gitCommitMessageGenerator = new GitCommitMessageGenerator(gptApiClientMock.Object);

            // Act
            var response =
                await gitCommitMessageGenerator.PostGitChangesToChatGptAsync(gitDiff, Prompts.Prompts.SinglePass);

            // Assert
            Assert.Equal(expectedResponse, response);
        }

        [Theory]
        [InlineData(Prompts.Prompts.SinglePass, "Single Pass Response")]
        [InlineData(Prompts.Prompts.MultiplePassesStart, "Multiple Passes Start Response")]
        [InlineData(Prompts.Prompts.MultiplePassesEnd, "Multiple Passes End Response")]
        public async Task PostGitChangesToChatGptAsync_DifferentPromptSelection_ReturnsExpectedResponseAsync(
            Prompts.Prompts promptSelection, string expectedResponse)
        {
            // Arrange
            const string gitDiff = "Sample Git Diff";
            var gptApiClientMock = new Mock<IGptApiClient>();
            gptApiClientMock.Setup(client => client.PostAsync(
                    It.IsAny<Request>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()))
                .ReturnsAsync(new Response
                {
                    Choices = new[]
                    {
                        new ResponseChoice { Message = new ResponseMessage { Content = expectedResponse } }
                    }
                });

            var gitCommitMessageGenerator = new GitCommitMessageGenerator(gptApiClientMock.Object);

            // Act
            var response = await gitCommitMessageGenerator.PostGitChangesToChatGptAsync(gitDiff, promptSelection);

            // Assert
            Assert.Equal(expectedResponse, response);
        }

        [Fact]
        public async Task PostGitChangesToChatGptAsync_FailedApiCall_ThrowsGptApiExceptionAsync()
        {
            // Arrange
            const string gitDiff = "Sample Git Diff";
            var gptApiClientMock = new Mock<IGptApiClient>();
            gptApiClientMock.Setup(client => client.PostAsync(
                    It.IsAny<Request>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()))
                .ThrowsAsync(new GptApiException(HttpStatusCode.BadRequest, "Bad Request"));

            var gitCommitMessageGenerator = new GitCommitMessageGenerator(gptApiClientMock.Object);

            // Act and Assert
            await Assert.ThrowsAsync<GptApiException>(() =>
                gitCommitMessageGenerator.PostGitChangesToChatGptAsync(gitDiff, Prompts.Prompts.SinglePass));
        }
    }
}