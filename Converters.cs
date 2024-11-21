using System;
using System.Windows;
using System.Windows.Data;
using System.Text.RegularExpressions;

namespace Meine_Ki
{
    public class MessageBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (bool)value ? "#3E3E3E" : "#7B68EE";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MessageAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (bool)value ? HorizontalAlignment.Right : HorizontalAlignment.Left;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MarkdownConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string text)
            {
                // Konvertiere Markdown in normalen Text
                text = Regex.Replace(text, @"\*\*(.*?)\*\*", "$1"); // Fett
                text = Regex.Replace(text, @"\*(.*?)\*", "$1");     // Kursiv
                text = Regex.Replace(text, @"__(.*?)__", "$1");     // Unterstrichen
                text = Regex.Replace(text, @"```(.*?)```", "$1");   // Code-Blöcke
                return text;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}