using System.Diagnostics;
using System.Threading.Tasks;

namespace ChatGptCommitMessageGenerator.Abstractions
{
    public interface IProcessExecutor
    {
        Task<string> ExecuteAsync(ProcessStartInfo startInfo);
    }
}