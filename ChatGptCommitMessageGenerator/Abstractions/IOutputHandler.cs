namespace ChatGptCommitMessageGenerator.Abstractions
{
    public interface IOutputHandler
    {
        string HandleOutput(string output, string errorOutput);
    }
}