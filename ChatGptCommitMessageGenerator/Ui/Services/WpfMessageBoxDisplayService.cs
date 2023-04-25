using System.Windows;
using ChatGptCommitMessageGenerator.Abstractions;

namespace ChatGptCommitMessageGenerator.Ui.Services
{
    internal class WpfMessageBoxDisplayService : IMessageBoxDisplayService
    {
        public void Show(string message, string title, MessageBoxButton button, MessageBoxImage image)
        {
            MessageBox.Show(message, title, button, image);
        }
    }
}