using System;
using System.Threading.Tasks;

namespace ChatGptCommitMessageGenerator.Abstractions
{
    internal interface IOutputWindowService
    {
        Task WriteAsync(string message, string paneName = "Commit Message Generator");
        Task WriteErrorAsync(Exception ex);
    }
}