using System.Collections.Generic;
using System.Threading.Tasks;
using ChatGptCommitMessageGenerator.Models;

namespace ChatGptCommitMessageGenerator.Abstractions
{
    public interface IGitCommitMessageGenerator
    {
        Task<string> GetResponseMessageAsync(List<GitChange> gitChanges);
        Task<string> PostGitChangesToChatGptAsync(string gitDiff, Prompts.Prompts promptSelection);
    }
}