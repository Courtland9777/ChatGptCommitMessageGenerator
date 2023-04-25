using System;
using System.Threading.Tasks;
using ChatGptCommitMessageGenerator.Abstractions;
using ChatGptCommitMessageGenerator.Ui.Services;

namespace ChatGptCommitMessageGenerator.Services
{
    internal class OutputManager
    {
        public static async Task WriteErrorToOutputWindowAsync(Exception ex)
        {
            IOutputWindowService outputWindowService = new VisualStudioOutputWindowService();
            await outputWindowService.WriteErrorAsync(ex);
        }

        public static async Task WriteToOutputWindowAsync(string message, string paneName = "Commit Message Generator")
        {
            IOutputWindowService outputWindowService = new VisualStudioOutputWindowService();
            await outputWindowService.WriteAsync(message, paneName);
        }

        public static async Task WriteCommitMessageToOutputWindowAsync(string message,
            string paneName = "Commit Message Generator")
        {
            var outputMessage = $"Commit Message: {message}\r\n";
            await WriteToOutputWindowAsync(outputMessage, paneName);
        }

        public static async Task WriteNoChangesMessageToOutputWindowAsync(string paneName = "Commit Message Generator")
        {
            const string outputMessage = "No changes found.";
            await WriteToOutputWindowAsync(outputMessage, paneName);
        }


        public static async Task ShowCommitMessageAsync(string message)
        {
            ICommitMessageDisplayService commitMessageDisplayService = new WpfCommitMessageDisplayService();
            await commitMessageDisplayService.ShowAsync(message);
        }
    }
}