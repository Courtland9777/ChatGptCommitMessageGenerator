using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ChatGptCommitMessageGenerator.Abstractions;
using ChatGptCommitMessageGenerator.Exceptions;
using ChatGptCommitMessageGenerator.Models;
using ChatGptCommitMessageGenerator.Services;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Xunit;

namespace ChatGptCommitMessageGenerator.Test.ServiceTests
{
    public class GptApiClientTests
    {
        private const string RequestUri = "https://api.openai.com/v1/chat/completions";

        [Fact]
        public async Task PostAsync_ReturnsExpectedResponse_WhenHttpRequestSucceedsAsync()
        {
            // Arrange
            const string expectedId = "response_id";
            const string expectedRole = "assistant";
            const string expectedContent = "sample text";
            var request = CreateRequest("test", "sample text");
            var responseContent = JsonConvert.SerializeObject(new Response
            {
                Id = expectedId,
                Choices = new[]
                {
                    new ResponseChoice
                    {
                        Index = 0,
                        FinishReason = "stop",
                        Message = new ResponseMessage { Role = expectedRole, Content = expectedContent }
                    }
                },
                Created = 1,
                Model = "gpt-3.5-turbo",
                Usage = new ResponseUsage { CompletionTokens = 1, PromptTokens = 2, TotalTokens = 3 }
            });
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseContent, Encoding.UTF8, "application/json")
            };

            var httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(m =>
                        m.Method == HttpMethod.Post &&
                        m.RequestUri.ToString().Equals(RequestUri) &&
                        m.Content.Headers.ContentType.MediaType == "application/json" &&
                        m.Content.Headers.ContentType.CharSet == "utf-8"), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponseMessage);

            var httpClient = new HttpClient(httpMessageHandlerMock.Object);
            var client = CreateGptApiClient(httpClient);

            // Act
            var result = await client.PostAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedId, result.Id);
            Assert.Single(result.Choices);
            Assert.Equal(expectedRole, result.Choices[0].Message.Role);
            Assert.Equal(expectedContent, result.Choices[0].Message.Content);
        }

        [Fact]
        public async Task PostAsync_SuccessfulResponse_DeserializesResponseAsync()
        {
            // Arrange
            var httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            var request = new Request { Messages = new RequestMessage[] { } };
            var responseContent = JsonConvert.SerializeObject(new Response
            {
                Id = "response_id",
                Choices = new[]
                {
                    new ResponseChoice
                    {
                        Index = 0,
                        FinishReason = "stop",
                        Message = new ResponseMessage { Role = "assistant", Content = "sample text" }
                    }
                },
                Created = 1,
                Model = "gpt-3.5-turbo",
                Usage = new ResponseUsage { CompletionTokens = 1, PromptTokens = 2, TotalTokens = 3 }
            });

            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseContent, Encoding.UTF8, "application/json")
            };

            httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(m =>
                        m.Method == HttpMethod.Post &&
                        m.RequestUri.ToString().Equals(RequestUri) &&
                        m.Content.Headers.ContentType.MediaType == "application/json" &&
                        m.Content.Headers.ContentType.CharSet == "utf-8"), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponseMessage);

            var httpClient = new HttpClient(httpMessageHandlerMock.Object);
            var client = CreateGptApiClient(httpClient);

            // Act
            var result = await client.PostAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("response_id", result.Id);
            Assert.Single(result.Choices);
            Assert.Equal("assistant", result.Choices[0].Message.Role);
            Assert.Equal("sample text", result.Choices[0].Message.Content);
            Assert.Equal(1, result.Created);
            Assert.Equal("gpt-3.5-turbo", result.Model);
            Assert.Equal(1, result.Usage.CompletionTokens);
            Assert.Equal(2, result.Usage.PromptTokens);
            Assert.Equal(3, result.Usage.TotalTokens);
        }

        [Fact]
        public async Task PostAsync_ThrowsGptApiException_WhenHttpStatusCodeIsBadRequestAsync()
        {
            // Arrange
            var request = new Request { Messages = Array.Empty<RequestMessage>() };
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);

            var httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(m =>
                        m.Method == HttpMethod.Post &&
                        m.RequestUri.ToString().Equals(RequestUri) &&
                        m.Content.Headers.ContentType.MediaType == "application/json" &&
                        m.Content.Headers.ContentType.CharSet == "utf-8"), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponseMessage);

            var httpClient = new HttpClient(httpMessageHandlerMock.Object);
            var client = CreateGptApiClient(httpClient);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<GptApiException>(() => client.PostAsync(request));
            Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
        }

        private static IGptApiClient CreateGptApiClient(HttpClient httpClient) => new GptApiClient(httpClient);

        private static Request CreateRequest(string systemMessage, string userMessage)
        {
            return new Request
            {
                Messages = new[]
                {
                    new RequestMessage { Role = "system", Content = systemMessage },
                    new RequestMessage { Role = "assistant", Content = userMessage }
                }
            };
        }
    }
}