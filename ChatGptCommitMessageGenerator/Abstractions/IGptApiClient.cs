using System.Threading.Tasks;
using ChatGptCommitMessageGenerator.Models;

namespace ChatGptCommitMessageGenerator.Abstractions
{
    public interface IGptApiClient
    {
        Task<Response> PostAsync(Request requestData, int maxRetries = 5,
            int initialWaitTimeMilliseconds = 1000);
    }
}