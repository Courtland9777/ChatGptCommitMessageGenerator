using System;
using ChatGptCommitMessageGenerator.Abstractions;

namespace ChatGptCommitMessageGenerator.Services
{
    public class DefaultOutputHandler : IOutputHandler
    {
        public string HandleOutput(string output, string errorOutput)
        {
            if (!string.IsNullOrEmpty(errorOutput))
                throw new InvalidOperationException($"Error creating Git diff: {errorOutput}");

            return output;
        }
    }
}