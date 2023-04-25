using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatGptCommitMessageGenerator.Abstractions;
using ChatGptCommitMessageGenerator.Models;
using Microsoft.DeepDev;

namespace ChatGptCommitMessageGenerator.TokenManagement
{
    public class DeepDevTokenManager : ITokenManager
    {
        private readonly int _baseRequestTokenCount;
        private readonly int _maxTokens;
        private readonly ITokenizer _tokenizer;

        public DeepDevTokenManager(int baseRequestTokenCount = 118, int maxTokens = 2000)
        {
            var specialTokens = new Dictionary<string, int>
            {
                { "start", 100264 },
                { "end", 100265 }
            };

            _tokenizer = TokenizerBuilder.CreateByModelName("gpt-3.5-turbo", specialTokens);
            _baseRequestTokenCount = baseRequestTokenCount;
            _maxTokens = maxTokens;
        }

        public int Encode(string text, out IList<int> encoded)
        {
            encoded = _tokenizer.Encode(text, new HashSet<string>(new[] { "", "" }));
            return encoded.Count;
        }

        public string Decode(IList<int> encoded) => _tokenizer.Decode(encoded.ToArray());

        public async Task<List<GitChange>> GroupStringsAsync(IEnumerable<string> gitDiff)
        {
            var inputList = new List<(string Text, int TokenCount)>();
            var totalTokenCount = _baseRequestTokenCount;

            foreach (var text in gitDiff)
            {
                var tokenCount = Encode(text, out var _);
                inputList.Add((text, tokenCount));
                totalTokenCount += tokenCount;
            }

            if (totalTokenCount <= _maxTokens)
            {
                var singleGroup = inputList.Select(i => new GitChange
                {
                    Change = i.Text,
                    TokenCount = i.TokenCount,
                    GroupNumber = 0
                }).ToList();

                return await Task.FromResult(singleGroup);
            }

            // Sort the inputList in descending order based on token count
            inputList.Sort((a, b) => b.TokenCount.CompareTo(a.TokenCount));

            var groupedStrings = new List<StringBuilder>();
            var groupTokens = new List<int>();
            var gitChanges = new List<GitChange>();

            foreach (var (text, tokenCount) in inputList)
            {
                var itemPlaced = false;

                for (var i = 0; i < groupedStrings.Count; i++)
                {
                    if (groupTokens[i] + tokenCount >= _maxTokens) continue;

                    if (groupedStrings[i].Length > 0) groupedStrings[i].AppendLine();

                    groupedStrings[i].Append(text);
                    groupTokens[i] += tokenCount;

                    gitChanges.Add(new GitChange
                    {
                        Change = text,
                        TokenCount = tokenCount,
                        GroupNumber = i
                    });

                    itemPlaced = true;
                    break;
                }

                if (itemPlaced) continue;
                var newGroup = new StringBuilder(text);
                groupedStrings.Add(newGroup);
                groupTokens.Add(_baseRequestTokenCount + tokenCount);

                gitChanges.Add(new GitChange
                {
                    Change = text,
                    TokenCount = tokenCount,
                    GroupNumber = groupedStrings.Count - 1
                });
            }

            return await Task.FromResult(gitChanges);
        }
    }
}