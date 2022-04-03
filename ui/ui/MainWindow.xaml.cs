using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using BaseService;
using Microsoft.Win32;
using Notifications.Wpf;
using NonInvasiveKeyboardHookLibrary;
using Ozeki.Media;
using System.Threading;
using Application = System.Windows.Forms.Application;

namespace ui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private KeyboardHookManager keyboardHookManager;

        static MediaConnector connector;

        static Microphone microphone;

        static GoogleSTT googleSTT;


        public MainWindow()
        {
            InitializeComponent();
            //BtnFile.IsEnabled = false;
            FromMic.IsChecked = true;
            fileNameTextBox.IsReadOnly = true;

            //TestProcessingContent();

        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            StartEvent();
        }

        void StartEvent()
        {
            StartButton.IsEnabled = false;
            StopButton.IsEnabled = true;
            ClearButton.IsEnabled = false;
           

            var notificationManager = new NotificationManager();

            notificationManager.Show(new NotificationContent
            {
                Title = "Thông Báo 🎉🎉",
                Message = "Chương trình bắt đầu thực thi",
                Type = NotificationType.Success
            }, expirationTime: TimeSpan.FromSeconds(3));
            if (FromFile.IsChecked != null && (bool)FromFile.IsChecked)
            {
                RunFromFile();
                StopEvent();
            }

            else
            {
                RunFromMic();
            }
        }

        private void StopButton_OnClickopButton_Click(object sender, RoutedEventArgs e)
        {
            StopEvent();
        }

        void StopEvent()
        {
            if ((bool)FromMic.IsChecked)    
            {
                StopMic();
            }
            StartButton.IsEnabled = true;
            StopButton.IsEnabled = false;
            ClearButton.IsEnabled = true;
            var notificationManager = new NotificationManager();

            notificationManager.Show(new NotificationContent
            {

                Title = "Thông Báo 🎉🎉",
                Message = "Chương trình dã tạm dừng",
                Type = NotificationType.Error,


            }, expirationTime: TimeSpan.FromSeconds(3));
        }

        private void RecognitionCallback(string result)
        {
            SendMess send = new SendMess();

            this.Dispatcher.Invoke(() =>
            {
                DisplayText.Text += BusinessLogic.ProcessingContent(result);
                send.Run(BusinessLogic.ProcessingContent(result));
            });

        }


        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            DisplayText.Text = "";
        }

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

            OpenFileDialog fileDialog = new OpenFileDialog()
            {
                FileName = "Select a text file",
                Filter = "Text files (*.flac)|*.flac",
                Title = "Open text file"
            };
            if (fileDialog.ShowDialog() == true)
                this.fileNameTextBox.Text = fileDialog.FileName;

        }

        private void FromFile_Checked(object sender, RoutedEventArgs e)
        {
            /*BtnFile.IsEnabled = true;
            fileNameTextBox.IsEnabled = true;*/
            BtnFile.Visibility = Visibility.Visible;
            fileNameTextBox.Visibility = Visibility.Visible;
        }

        private void FromMic_Checked_1(object sender, RoutedEventArgs e)
        {
            /*BtnFile.IsEnabled = false;
            fileNameTextBox.IsEnabled = false;*/
            BtnFile.Visibility = Visibility.Hidden;
            fileNameTextBox.Visibility = Visibility.Hidden;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            keyboardHookManager = new KeyboardHookManager();
            keyboardHookManager.Start();
            keyboardHookManager.RegisterHotkey(new[] { NonInvasiveKeyboardHookLibrary.ModifierKeys.Control }, 0x70, () =>
             {
                 this.Dispatcher.Invoke(() =>
                 {
                     StartEvent();
                 });
             }
            );
            keyboardHookManager.RegisterHotkey(new[] { NonInvasiveKeyboardHookLibrary.ModifierKeys.Control }, 0x71, () =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    StopEvent();
                });
            });

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
            keyboardHookManager.UnregisterAll();
            keyboardHookManager.Stop();
        }

        void RunFromFile()
        {
            String filePath = GetFile();

            if (filePath != "")
            {
                var format = new WaveFormat(24000, 16, 1);

                googleSTT =
                new GoogleSTT(GoogleLanguage.Vietnamese,
                                            format.AsVoIPMediaFormat(), RecognitionCallback, false, filePath);
            }
        }

        void RunFromMic()
        {
            connector = new MediaConnector();
            microphone = Microphone.GetDefaultDevice();

            var format = new WaveFormat(16000, 16, 1);

            microphone.ChangeFormat(format);

            googleSTT =
            new GoogleSTT(GoogleLanguage.Vietnamese,
                                        format.AsVoIPMediaFormat(), RecognitionCallback, true);

            connector.Connect(microphone, googleSTT);

            microphone.Start();

            googleSTT.Start();


            //Console.ReadLine();

            //Console.WriteLine("Disconnect");

            //connector.Disconnect(microphone, googleSTT);

            //Console.WriteLine("Google dispose");

            //googleSTT.Dispose();
            //googleSTT = null;

            //Console.WriteLine("microphone dispose");

            //microphone.Dispose();
            //microphone = null;

            //Console.WriteLine("connector dispose");

            //connector.Dispose();
            //connector = null;
        }
        void StopMic()
        {
            microphone.Stop();
            googleSTT.Stop();
        }
    }
}
