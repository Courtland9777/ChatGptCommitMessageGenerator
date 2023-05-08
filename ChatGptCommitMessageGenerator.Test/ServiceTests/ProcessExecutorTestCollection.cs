using Xunit;

namespace ChatGptCommitMessageGenerator.Test.ServiceTests
{
    [CollectionDefinition("ProcessExecutorTests")]
    public class ProcessExecutorTestCollection : ICollectionFixture<ProcessExecutorTestFixture>
    {
    }
}