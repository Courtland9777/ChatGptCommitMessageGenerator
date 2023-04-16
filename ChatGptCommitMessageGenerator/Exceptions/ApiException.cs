using System;
using System.Net;

namespace ChatGptCommitMessageGenerator.Exceptions
{
    internal class ApiException : Exception
    {
        public ApiException(HttpStatusCode statusCode, string message) : base(message) => StatusCode = statusCode;
        public HttpStatusCode StatusCode { get; }
    }
}