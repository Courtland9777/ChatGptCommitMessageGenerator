using System.Threading.Tasks;

namespace ChatGptCommitMessageGenerator.Abstractions
{
    public interface IGitDiffProvider
    {
        Task<string> GetGitDiffAsync(string repositoryPath);
    }
}