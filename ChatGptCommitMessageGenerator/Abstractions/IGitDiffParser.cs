using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatGptCommitMessageGenerator.Abstractions
{
    public interface IGitDiffParser
    {
        Task<List<string>> ExtractGitDiffChangesAsync(string gitDiff);
    }
}