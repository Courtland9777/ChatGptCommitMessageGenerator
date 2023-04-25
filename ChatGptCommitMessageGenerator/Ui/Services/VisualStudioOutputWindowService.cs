using System;
using System.Threading.Tasks;
using ChatGptCommitMessageGenerator.Abstractions;
using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;

namespace ChatGptCommitMessageGenerator.Ui.Services
{
    public class VisualStudioOutputWindowService : IOutputWindowService
    {
        private readonly Guid _errorPaneGuid = new Guid("{87C24C9A-0A0B-461D-807C-0C70F2868496}");

        public async Task WriteAsync(string message, string paneName = "Commit Message Generator")
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            // Get the Output Window service
            var outputWindow = await VS.Services.GetOutputWindowAsync();

            // Create or get an existing Output Window pane
            var paneGuid = new Guid("{7EAA32EB-D52E-4455-9BA6-079F73FA768A}");
            if (ErrorHandler.Failed(outputWindow.GetPane(ref paneGuid, out var pane)))
            {
                ErrorHandler.ThrowOnFailure(outputWindow.CreatePane(ref paneGuid, paneName, 1, 1));
                ErrorHandler.ThrowOnFailure(outputWindow.GetPane(ref paneGuid, out pane));
            }

            // Activate and show the Output Window pane
            ErrorHandler.ThrowOnFailure(pane.Activate());

            // Write the message
            ErrorHandler.ThrowOnFailure(pane.OutputStringThreadSafe(message));
        }

        public async Task WriteErrorAsync(Exception ex)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            // Get the Output Window service
            var outputWindow = await VS.Services.GetOutputWindowAsync();

            // Create or get an existing Output Window pane
            var errorPaneGuid = _errorPaneGuid;
            if (ErrorHandler.Failed(outputWindow.GetPane(ref errorPaneGuid, out var errorPane)))
            {
                ErrorHandler.ThrowOnFailure(outputWindow.CreatePane(ref errorPaneGuid, "Commit Message Generator Error",
                    1, 1));
                ErrorHandler.ThrowOnFailure(outputWindow.GetPane(ref errorPaneGuid, out errorPane));
            }

            // Activate and show the Output Window pane
            ErrorHandler.ThrowOnFailure(errorPane.Activate());

            // Write the error message
            var errorMessage = $"Error: {ex.Message}\r\nStack Trace: {ex.StackTrace}\r\n";
            ErrorHandler.ThrowOnFailure(errorPane.OutputStringThreadSafe(errorMessage));
        }
    }
}