using System.ComponentModel;
using System.Threading.Tasks;
using ChatGptCommitMessageGenerator.Helpers;
using Xunit;

namespace ChatGptCommitMessageGeneratorTests
{
    public class GitDiffHelperTests
    {
        //[Fact]
        //public async Task CreateGitDiffAsync_WithValidRepoPath_ReturnsModifiedContentAsync()
        //{
        //    // Arrange
        //    var testRepoPath = Path.Combine(Directory.GetCurrentDirectory(), "TestObjects");
        //    var expectedResult = "modified content"; // Replace with the expected Git diff result

        //    // Act
        //    var result =
        //        await GitDiffHelper.CreateGitDiffAsync(testRepoPath);

        //    // Assert
        //    Assert.Equal(expectedResult, result);
        //}

        [Fact]
        public async Task CreateGitDiffFileAsync_ThrowsException_WhenErrorOccursAsync()
        {
            // Arrange
            var repositoryPath = "invalid_path";

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Win32Exception>(
                () => GitDiffHelper.CreateGitDiffAsync(repositoryPath));
            Assert.Contains("The directory name is invalid", exception.Message);
        }
    }
}