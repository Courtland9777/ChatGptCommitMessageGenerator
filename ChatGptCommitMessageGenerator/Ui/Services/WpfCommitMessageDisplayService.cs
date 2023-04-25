using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ChatGptCommitMessageGenerator.Abstractions;
using ChatGptCommitMessageGenerator.Ui.Events;
using Microsoft.VisualStudio.Shell;

namespace ChatGptCommitMessageGenerator.Ui.Services
{
    public class WpfCommitMessageDisplayService : ICommitMessageDisplayService
    {
        public async Task ShowAsync(string message)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            const string title = "Commit Message";

            var copyButton = new Button { Content = "Copy to Clipboard", Margin = new Thickness(0, 0, 5, 0) };
            var confirmButton = new Button { Content = "Confirm", IsDefault = true };

            copyButton.Click += UiEvents.CopyButton_Click;
            confirmButton.Click += UiEvents.ConfirmButton_Click;

            var buttonStackPanel = new StackPanel
                { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center };
            buttonStackPanel.Children.Add(copyButton);
            buttonStackPanel.Children.Add(confirmButton);

            var stackPanel = new StackPanel();
            stackPanel.Children.Add(new TextBlock
                { Text = message, TextWrapping = TextWrapping.Wrap, Margin = new Thickness(10) });
            stackPanel.Children.Add(buttonStackPanel);

            var commitMessageWindow = new Window
            {
                Title = title,
                Content = stackPanel,
                SizeToContent = SizeToContent.WidthAndHeight,
                ResizeMode = ResizeMode.NoResize,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            confirmButton.Tag = commitMessageWindow;

            // Apply the current theme to the custom message box.
            commitMessageWindow.Resources.MergedDictionaries.Add(Application.Current.Resources);

            commitMessageWindow.ShowDialog();
        }
    }
}