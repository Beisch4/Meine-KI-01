using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Meine_Ki
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly GeminiSearchService _searchService;
        private ObservableCollection<ChatMessage> chatMessages;

        public MainWindow()
        {
            InitializeComponent();
            _searchService = new GeminiSearchService();
            chatMessages = new ObservableCollection<ChatMessage>();
            chatDisplay.ItemsSource = chatMessages;
            AddMessage("Willkommen! Ich bin Ihr KI-Assistent mit Google Gemini. Wie kann ich Ihnen helfen?", false);
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            await ProcessUserInput();
        }

        private async void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await ProcessUserInput();
            }
        }

        private async Task ProcessUserInput()
        {
            if (string.IsNullOrWhiteSpace(inputTextBox.Text)) return;

            var query = inputTextBox.Text;
            inputTextBox.Clear();
            AddMessage(query, true);
            loadingIndicator.Visibility = Visibility.Visible;

            try
            {
                var result = await _searchService.GetResponseAsync(query);
                AddMessage(result.Content, false);
            }
            catch (Exception ex)
            {
                AddMessage($"Entschuldigung, es ist ein Fehler aufgetreten: {ex.Message}", false);
            }
            finally
            {
                loadingIndicator.Visibility = Visibility.Collapsed;
            }
        }

        private void NewChat_Click(object sender, RoutedEventArgs e)
        {
            chatMessages.Clear();
            AddMessage("Wie kann ich Ihnen helfen?", false);
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
            scrollViewer?.ScrollToBottom();
        }
    }
}