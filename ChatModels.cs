using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace WpfChatbot
{
    // Diese Klasse repräsentiert eine einzelne Chatnachricht
    public class ChatMessage
    {
        // Der eigentliche Text der Nachricht
        public string Message { get; set; }

        // Gibt an, ob die Nachricht vom Benutzer (true) oder vom Bot (false) ist
        public bool IsUser { get; set; }

        // Zeitpunkt, wann die Nachricht gesendet wurde
        public DateTime Timestamp { get; set; }
    }

    // Dieser Converter bestimmt die Hintergrundfarbe der Chat-Bubbles
    public class BubbleColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // Wenn es eine Benutzernachricht ist (IsUser = true), wird sie grün,
            // ansonsten weiß dargestellt
            return (bool)value ? "#DCF8C6" : "White";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Dieser Converter bestimmt die Ausrichtung der Chat-Bubbles
    public class BubbleAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // Benutzernachrichten werden rechts, Bot-Nachrichten links angezeigt
            return (bool)value ? HorizontalAlignment.Right : HorizontalAlignment.Left;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}