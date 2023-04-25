using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ChatGptCommitMessageGenerator.Abstractions;

namespace ChatGptCommitMessageGenerator.Services
{
    public class GitDiffParser : IGitDiffParser
    {
        private static readonly Regex DiffRegexPattern =
            new Regex($"^@@ -\\d+(,\\d+)? \\+\\d+(,\\d+)? @@.*{Environment.NewLine}", RegexOptions.Multiline);

        public async Task<List<string>> ExtractGitDiffChangesAsync(string gitDiff)
        {
            return await Task.Run(() =>
            {
                var matches = DiffRegexPattern.Matches(gitDiff).Cast<Match>();

                var changes = matches.Select(match => new { match, startIndex = match.Index })
                    .Select(t => new { t, length = t.match.Length })
                    .Select(t => gitDiff.Substring(t.t.startIndex, t.length)).ToList();


                return changes.Count == 0 ? new List<string>() : changes;
            }).ConfigureAwait(false);
        }
    }
}