using System;
using System.Net;

namespace ChatGptCommitMessageGenerator.Exceptions
{
    public sealed class GptApiException : Exception
    {
        public GptApiException(HttpStatusCode statusCode)
            : base($"API call resulted in an error: {statusCode}") =>
            StatusCode = statusCode;

        public GptApiException(HttpStatusCode statusCode, string message)
            : base(message) =>
            StatusCode = statusCode;

        public GptApiException(HttpStatusCode statusCode, string message, Exception innerException)
            : base(message, innerException) =>
            StatusCode = statusCode;

        public HttpStatusCode StatusCode { get; set; }
    }
}