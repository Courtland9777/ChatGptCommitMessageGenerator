using System.Windows;

namespace ChatGptCommitMessageGenerator.Abstractions
{
    public interface IMessageBoxDisplayService
    {
        void Show(string message, string title, MessageBoxButton button, MessageBoxImage image);
    }
}