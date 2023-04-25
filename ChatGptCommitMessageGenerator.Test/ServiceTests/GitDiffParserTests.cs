using System.Collections.Generic;
using System.Threading.Tasks;
using ChatGptCommitMessageGenerator.Services;
using Xunit;

namespace ChatGptCommitMessageGenerator.Test.ServiceTests
{
    public class GitDiffParserTests
    {
        private readonly GitDiffParser _gitDiffParser;

        public GitDiffParserTests() => _gitDiffParser = new GitDiffParser();

        public static IEnumerable<object[]> GitDiffTestData =>
            new List<object[]>
            {
                new object[]
                {
                    @"@@ -1,3 +1,4 @@
some code
+added code
some more code",
                    1
                },
                new object[] { "", 0 },
                new object[] { "some text without git diff pattern", 0 },
                new object[]
                {
                    @"@@ -1,3 +1,4 @@
some code
+added code
some more code
@@ -5,6 +5,7 @@
more code
+another added code
more code again",
                    2
                },
                new object[]
                {
                    @"@@ -1,3 +1,5 @@
some code
+added code
+another added code
some more code",
                    1
                },
                new object[]
                {
                    @"@@ -1,4 +1,3 @@
some code
-removed code
some more code
+added code",
                    1
                }
            };

        [Theory]
        [MemberData(nameof(GitDiffTestData))]
        public async Task ExtractGitDiffChangesAsync_ValidCases_ReturnsExpectedChangesCountAsync(string gitDiff,
            int expectedChangesCount)
        {
            // Act
            var changes = await _gitDiffParser.ExtractGitDiffChangesAsync(gitDiff);

            // Assert
            Assert.Equal(expectedChangesCount, changes.Count);
        }
    }
}