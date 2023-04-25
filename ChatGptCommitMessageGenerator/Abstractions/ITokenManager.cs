using System.Collections.Generic;
using System.Threading.Tasks;
using ChatGptCommitMessageGenerator.Models;

namespace ChatGptCommitMessageGenerator.Abstractions
{
    public interface ITokenManager
    {
        int Encode(string text, out IList<int> encoded);
        string Decode(IList<int> encoded);
        Task<List<GitChange>> GroupStringsAsync(IEnumerable<string> gitDiff);
    }
}