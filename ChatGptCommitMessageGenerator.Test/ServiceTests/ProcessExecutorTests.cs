using System;
using System.Threading.Tasks;
using ChatGptCommitMessageGenerator.Abstractions;
using ChatGptCommitMessageGenerator.Services;
using Moq;
using Xunit;

namespace ChatGptCommitMessageGenerator.Test.ServiceTests
{
    public class ProcessExecutorTestFixture
    {
        public ProcessExecutorTestFixture() => OutputHandlerMock = new Mock<IOutputHandler>();

        public Mock<IOutputHandler> OutputHandlerMock { get; }
    }

    [Collection("ProcessExecutorTests")]
    public class ProcessExecutorTests
    {
        private readonly ProcessExecutorTestFixture _fixture;

        public ProcessExecutorTests(ProcessExecutorTestFixture fixture) => _fixture = fixture;

        [Fact]
        public async Task ExecuteAsync_WithNullStartInfo_ThrowsArgumentNullExceptionAsync()
        {
            // Arrange
            var processExecutor = new ProcessExecutor(_fixture.OutputHandlerMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => processExecutor.ExecuteAsync(null));
        }


        [Fact]
        public void Constructor_WithNullOutputHandler_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ProcessExecutor(null));
        }
    }
}