using System;
using ChatGptCommitMessageGenerator.Abstractions;
using ChatGptCommitMessageGenerator.Services;
using Xunit;

namespace ChatGptCommitMessageGenerator.Test.ServiceTests
{
    public class DefaultOutputHandlerTests
    {
        private readonly IOutputHandler _outputHandler;

        public DefaultOutputHandlerTests() => _outputHandler = new DefaultOutputHandler();

        [Fact]
        public void HandleOutput_ReturnsExpectedValue()
        {
            const string output = "output";
            string errorOutput = null;
            const string expected = "output";

            var result = _outputHandler.HandleOutput(output, errorOutput);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void HandleOutput_ReturnsExpectedErrorMessage()
        {
            const string output = "output";
            const string errorOutput = "error";

            var ex = Record.Exception(() => _outputHandler.HandleOutput(output, errorOutput));

            Assert.IsType<InvalidOperationException>(ex);
            Assert.Equal($"Error creating Git diff: {errorOutput}", ex.Message);
        }
    }
}