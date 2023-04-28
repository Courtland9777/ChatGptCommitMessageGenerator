using System;
using System.Diagnostics;
using System.Threading.Tasks;
using ChatGptCommitMessageGenerator.Abstractions;
using ChatGptCommitMessageGenerator.Services;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace ChatGptCommitMessageGenerator.Test.ServiceTests
{
    public class GitDiffProviderTests
    {
        private readonly GitDiffProvider _gitDiffProvider;
        private readonly Mock<IProcessExecutor> _mockProcessExecutor = new Mock<IProcessExecutor>();

        public GitDiffProviderTests()
        {
            var services = new ServiceCollection();
            services.AddSingleton(_mockProcessExecutor.Object);
            _gitDiffProvider = new GitDiffProvider(_mockProcessExecutor.Object);
        }

        [Fact]
        public async Task GetGitDiffAsync_ValidRepositoryPath_ReturnsExpectedOutputAsync()
        {
            // Arrange
            const string repositoryPath = @"C:\Test\MyRepository";
            const string expectedOutput = "diff output";
            _mockProcessExecutor.Setup(x => x.ExecuteAsync(It.IsAny<ProcessStartInfo>())).ReturnsAsync(expectedOutput);

            // Act
            var actualOutput = await _gitDiffProvider.GetGitDiffAsync(repositoryPath);

            // Assert
            Assert.Equal(expectedOutput, actualOutput);
        }

        [Fact]
        public async Task GetGitDiffAsync_EmptyRepositoryPath_ReturnsEmptyStringAsync()
        {
            // Arrange
            const string repositoryPath = "";
            const string expectedOutput = "";

            // Act
            var actualOutput = await _gitDiffProvider.GetGitDiffAsync(repositoryPath);

            // Assert
            Assert.Equal(expectedOutput, actualOutput);
        }


        [Fact]
        public async Task GetGitDiffAsync_InvalidCommand_ThrowsExceptionAsync()
        {
            // Arrange
            const string repositoryPath = @"C:\Test\MyRepository";
            _mockProcessExecutor.Setup(x => x.ExecuteAsync(It.IsAny<ProcessStartInfo>()))
                .ThrowsAsync(new Exception("Error executing command"));

            // Act and Assert
            await Assert.ThrowsAsync<Exception>(() => _gitDiffProvider.GetGitDiffAsync(repositoryPath));
        }

        [Theory]
        [InlineData(@"C:\Test\My Repository")]
        public async Task GetGitDiffAsync_RepositoryPathWithSpaces_ReturnsExpectedOutputAsync(string repositoryPath)
        {
            // Arrange
            const string expectedOutput = "diff output";
            _mockProcessExecutor.Setup(x => x.ExecuteAsync(It.IsAny<ProcessStartInfo>())).ReturnsAsync(expectedOutput);

            // Act
            var actualOutput = await _gitDiffProvider.GetGitDiffAsync(repositoryPath);

            // Assert
            Assert.Equal(expectedOutput, actualOutput);
        }

        [Fact]
        public async Task GetGitDiffAsync_NoPermissionsToExecuteCommand_ThrowsExceptionAsync()
        {
            // Arrange
            const string repositoryPath = @"C:\Test\MyRepository";
            _mockProcessExecutor.Setup(x => x.ExecuteAsync(It.IsAny<ProcessStartInfo>()))
                .ThrowsAsync(new Exception("Access denied"));

            // Act and Assert
            await Assert.ThrowsAsync<Exception>(() => _gitDiffProvider.GetGitDiffAsync(repositoryPath));
        }
    }
}