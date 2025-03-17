using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SentimentAnalysisTool.Controls
{
    public class PlaceholderTextBox : TextBox
    {
        public static readonly DependencyProperty PlaceholderTextProperty =
            DependencyProperty.Register("PlaceholderText", typeof(string), typeof(PlaceholderTextBox), new PropertyMetadata(string.Empty));

        public string PlaceholderText
        {
            get { return (string)GetValue(PlaceholderTextProperty); }
            set { SetValue(PlaceholderTextProperty, value); }
        }

        private Brush _originalForeground;
        private bool _isPlaceholderVisible;

        public PlaceholderTextBox()
        {
            Loaded += PlaceholderTextBox_Loaded;
            GotFocus += PlaceholderTextBox_GotFocus;
            LostFocus += PlaceholderTextBox_LostFocus;
        }

        private void PlaceholderTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            _originalForeground = Foreground;
            UpdatePlaceholder();
        }

        private void PlaceholderTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (_isPlaceholderVisible)
            {
                Text = string.Empty;
                Foreground = _originalForeground;
                _isPlaceholderVisible = false;
            }
        }

        private void PlaceholderTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdatePlaceholder();
        }

        private void UpdatePlaceholder()
        {
            if (string.IsNullOrEmpty(Text) && !string.IsNullOrEmpty(PlaceholderText))
            {
                Text = PlaceholderText;
                Foreground = new SolidColorBrush(Colors.Gray);
                _isPlaceholderVisible = true;
            }
            else
            {
                _isPlaceholderVisible = false;
            }
        }
    }
} 