using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ChatGptCommitMessageGenerator.Abstractions;
using ChatGptCommitMessageGenerator.Helpers;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace ChatGptCommitMessageGeneratorTests
{
    public class GenerateMessageHelpersTests
    {
        [Fact]
        public async Task GetChatGptCommitMessageAsync_Returns_Valid_Commit_MessageAsync()
        {
            // Arrange
            const string diff = "+ This is a test diff";
            var fakeResponse = new
            {
                choices = new[]
                {
                    new
                    {
                        message = new { content = "Add test diff" }
                    }
                }
            };
            var fakeJson = JsonConvert.SerializeObject(fakeResponse);
            var httpClientWrapperMock = new Mock<IHttpClientWrapper>();
            httpClientWrapperMock.Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(fakeJson)
                });

            // Act
            var result = await GenerateMessageHelpers.GetChatGptCommitMessageAsync(httpClientWrapperMock.Object, diff);

            // Assert
            Assert.Equal("Add test diff", result);
        }
    }
}