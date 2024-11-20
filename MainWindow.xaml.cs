using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Input;
using WpfChatbot;

namespace Meine_Ki
{
    public partial class MainWindow : Window
    {
        private ChatbotBrain chatbot;
        private WebSearchService webSearch;
        private ObservableCollection<ChatMessage> chatMessages;

        public MainWindow()
        {
            InitializeComponent();
            chatbot = new ChatbotBrain();
            webSearch = new WebSearchService();
            chatMessages = new ObservableCollection<ChatMessage>();
            chatDisplay.ItemsSource = chatMessages;

            // Füge Event-Handler hinzu
            userInput.KeyDown += UserInput_KeyDown;
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(userInput.Text)) return;

            var message = userInput.Text;
            userInput.Clear();

            AddMessage(message, true);
            loadingIndicator.Visibility = Visibility.Visible;

            try
            {
                var results = await webSearch.SearchAndSummarize(message);
                if (results.Count > 0)
                {
                    var summary = new StringBuilder();
                    summary.AppendLine("📚 Zusammenfassung der Suchergebnisse:\n");

                    foreach (var result in results)
                    {
                        summary.AppendLine($"• {result.Summary}");
                    }

                    summary.AppendLine("\n🔍 Quellen:");
                    foreach (var result in results)
                    {
                        summary.AppendLine($"- {result.Title}: {result.Url}");
                    }

                    AddMessage(summary.ToString(), false);
                }
                else
                {
                    AddMessage("Leider konnte ich keine relevanten Informationen finden.", false);
                }
            }
            catch (Exception ex)
            {
                AddMessage($"Es tut mir leid, es ist ein Fehler aufgetreten: {ex.Message}", false);
            }
            finally
            {
                loadingIndicator.Visibility = Visibility.Collapsed;
            }
        }

        private void UserInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendButton_Click(sender, null);
            }
        }

        private void AddMessage(string message, bool isUser)
        {
            var chatMessage = new ChatMessage
            {
                Message = message,
                IsUser = isUser,
                Timestamp = DateTime.Now
            };
            chatMessages.Add(chatMessage);
            chatScrollViewer.ScrollToBottom();
        }
    }
}