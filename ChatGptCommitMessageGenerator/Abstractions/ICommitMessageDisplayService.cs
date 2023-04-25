using System.Threading.Tasks;

namespace ChatGptCommitMessageGenerator.Abstractions
{
    public interface ICommitMessageDisplayService
    {
        Task ShowAsync(string message);
    }
}