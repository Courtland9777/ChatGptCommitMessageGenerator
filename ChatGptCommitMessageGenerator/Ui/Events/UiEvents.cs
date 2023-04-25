using System.Windows;
using System.Windows.Controls;
using ChatGptCommitMessageGenerator.Ui.Services;

namespace ChatGptCommitMessageGenerator.Ui.Events
{
    public class UiEvents
    {
        public static void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            var message = ((TextBlock)((StackPanel)((Button)sender).Parent).Children[0]).Text;
            Clipboard.SetText(message);
            var messageBoxDisplayService = new WpfMessageBoxDisplayService();
            messageBoxDisplayService.Show("Message copied to clipboard.", "Success", MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        public static void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            ((Window)((Button)sender).Tag).Close();
        }
    }
}