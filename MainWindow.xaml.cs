using SentimentAnalysisTool.Services;
using System.Windows;
using System.Windows.Media;

namespace SentimentAnalysisTool
{
    public partial class MainWindow : Window
    {
        private readonly SentimentAnalysisService _sentimentService;

        public MainWindow()
        {
            InitializeComponent();
            _sentimentService = new SentimentAnalysisService();
        }

        private void AnalyzeButton_Click(object sender, RoutedEventArgs e)
        {
            string text = InputTextBox.Text.Trim();

            if (string.IsNullOrEmpty(text))
            {
                MessageBox.Show("Please enter some text to analyze.", "Empty Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var prediction = _sentimentService.PredictSentiment(text);
            string sentimentCategory = _sentimentService.GetSentimentCategory(prediction);

            // Update UI with results
            SentimentResultTextBlock.Text = sentimentCategory;
            ScoreResultTextBlock.Text = prediction.Score.ToString("F4");
            ProbabilityResultTextBlock.Text = prediction.Probability.ToString("P2");

            // Set color based on sentiment
            switch (sentimentCategory)
            {
                case "Positive":
                    SentimentResultTextBlock.Foreground = new SolidColorBrush(Colors.Green);
                    break;
                case "Negative":
                    SentimentResultTextBlock.Foreground = new SolidColorBrush(Colors.Red);
                    break;
                default:
                    SentimentResultTextBlock.Foreground = new SolidColorBrush(Colors.Orange);
                    break;
            }
        }
    }
} 