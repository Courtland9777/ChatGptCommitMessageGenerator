using System.Threading.Tasks;
using ChatGptCommitMessageGenerator.Abstractions;
using ChatGptCommitMessageGenerator.Services;
using Xunit;

namespace ChatGptCommitMessageGenerator.Test.ServiceTests
{
    public class GitDiffParserTests
    {
        private readonly IGitDiffParser _gitDiffParser;

        public GitDiffParserTests() => _gitDiffParser = new GitDiffParser();

        [Fact]
        public async Task ExtractGitDiffChangesAsync_WithValidGitDiff_ReturnsListOfChangesAsync()
        {
            // Arrange
            const string gitDiff = @"
diff --git a/README.md b/README.md
index 4f4d3e1..2d5f5b5 100644
--- a/README.md
+++ b/README.md
@@ -1,4 +1,4 @@
-# OldProjectName
+# NewProjectName
 This is a simple project.
 
@@ -10,4 +10,4 @@
-This line has been removed.
+This line has been added.
";

            const int expectedChangesCount = 2;

            // Act
            var result = await _gitDiffParser.ExtractGitDiffChangesAsync(gitDiff);

            // Assert
            Assert.Equal(expectedChangesCount, result.Count);
            Assert.Contains("@@ -1,4 +1,4 @@\r\n", result);
            Assert.Contains("@@ -10,4 +10,4 @@\r\n", result);
        }

        [Fact]
        public async Task ExtractGitDiffChangesAsync_WithEmptyGitDiff_ReturnsEmptyListAsync()
        {
            // Arrange
            var gitDiff = string.Empty;

            // Act
            var result = await _gitDiffParser.ExtractGitDiffChangesAsync(gitDiff);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task ExtractGitDiffChangesAsync_WithInvalidGitDiff_ReturnsEmptyListAsync()
        {
            // Arrange
            const string gitDiff = @"
diff --git a/README.md b/README.md
index 4f4d3e1..2d5f5b5 100644
--- a/README.md
+++ b/README.md
-# OldProjectName
+# NewProjectName
 This is a simple project.
 
-This line has been removed.
+This line has been added.
";
            // Act
            var result = await _gitDiffParser.ExtractGitDiffChangesAsync(gitDiff);

            // Assert
            Assert.Empty(result);
        }
    }
}