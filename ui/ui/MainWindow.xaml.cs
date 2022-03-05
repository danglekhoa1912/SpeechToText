using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BaseService;

namespace ui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SpeechService _speechService;
        private TaskCompletionSource<int> _stopTaskCompletionSource;

        public MainWindow()
        {
            InitializeComponent();

            TestProcessingContent();
            //_speechService = new SpeechService();
            //_speechService.SetConfig("a11e89949b6543bb83fe92e30c04b67b", "eastus", "vi-VN");
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            _stopTaskCompletionSource = new TaskCompletionSource<int>();
            Task.Run(async () => { await _speechService.Start(_stopTaskCompletionSource, RecognitionCallback).ConfigureAwait(false); });
            StartButton.IsEnabled = false;
            StopButton.IsEnabled = true;
            ClearButton.IsEnabled = false;
        }

        private void StopButton_OnClickopButton_Click(object sender, RoutedEventArgs e)
        {
            _stopTaskCompletionSource.TrySetResult(0);
            StartButton.IsEnabled = true;
            StopButton.IsEnabled = false;
            ClearButton.IsEnabled=true;
        }

        private void RecognitionCallback(string result)
        {
            this.Dispatcher.Invoke(() =>
            {
                DisplayText.Text += result;
            });
        }

        private void FromMic_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void FromFile_Checked_1(object sender, RoutedEventArgs e)
        {

        }
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            DisplayText.Text = "";

        }

        private void TestProcessingContent()
        {
            DisplayText.Text = "ádkajshdajk nhập dấu.".ProcessingContent();
        }
    }
}
