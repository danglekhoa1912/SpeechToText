using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
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
using Microsoft.Win32;

namespace ui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SpeechService _speechService;
        private TaskCompletionSource<int> _stopTaskCompletionSource;
        private string wavFileName;

        public MainWindow()
        {
            InitializeComponent();
            BtnFile.IsEnabled = false;
            FromMic.IsChecked = true;
            fileNameTextBox.IsReadOnly = true;
            //TestProcessingContent();
            _speechService = new SpeechService();
            try
            {
                string key = System.IO.File.ReadAllText("./configkey.txt");
                _speechService.SetConfig(key, "eastus", "vi-VN");

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)FromFile.IsChecked)
            {
                wavFileName = GetFile();
                if (wavFileName.Length <= 0) return;
                Task.Run(() => this.PlayAudioFile());
                _speechService.setWavFileName(wavFileName);
                _stopTaskCompletionSource = new TaskCompletionSource<int>();
                Task.Run(async () => { await _speechService.Start(_stopTaskCompletionSource, RecognitionCallback, false).ConfigureAwait(false); });
            }

            else
            {
                _stopTaskCompletionSource = new TaskCompletionSource<int>();
                Task.Run(async () => { await _speechService.Start(_stopTaskCompletionSource, RecognitionCallback, true).ConfigureAwait(false); });
            }


            StartButton.IsEnabled = false;
            StopButton.IsEnabled = true;
            ClearButton.IsEnabled = false;
        }

        private void StopButton_OnClickopButton_Click(object sender, RoutedEventArgs e)
        {
            _stopTaskCompletionSource.TrySetResult(0);
            StartButton.IsEnabled = true;
            StopButton.IsEnabled = false;
            ClearButton.IsEnabled = true;
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

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            DisplayText.Text = "";

        }

        //private void TestProcessingContent()
        //{
        //    DisplayText.Text = "ádkajshdajk nhập dấu.".ProcessingContent();
        //}
        public string GetFile()
        {
            string filePath = "";
            this.Dispatcher.Invoke(() =>
            {
                filePath = this.fileNameTextBox.Text;
            });
            if (!File.Exists(filePath))
            {
                MessageBox.Show("File does not exist!");
                return "";
            }
            return filePath;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            OpenFileDialog fileDialog = new OpenFileDialog();
            if (fileDialog.ShowDialog() == true)
                this.fileNameTextBox.Text = fileDialog.FileName;

        }
        private void PlayAudioFile()
        {
            SoundPlayer player = new SoundPlayer(wavFileName);
            player.Load();
            player.Play();
        }

        private void FromFile_Checked(object sender, RoutedEventArgs e)
        {
            BtnFile.IsEnabled = true;
            fileNameTextBox.IsEnabled = true;

        }

        private void fileNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void FromMic_Checked_1(object sender, RoutedEventArgs e)
        {
            BtnFile.IsEnabled = false;
            fileNameTextBox.IsEnabled = false;
        }
    }
}
