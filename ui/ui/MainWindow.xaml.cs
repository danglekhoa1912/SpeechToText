﻿using System;
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
        private KeyboardHookManager keyboardHookManager;

        static MediaConnector connector;

        static Microphone microphone;

        static GoogleSTT googleSTT;


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
            RunFromMic();
            //StartEvent();

        }

        void StartEvent()
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

            var notificationManager = new NotificationManager();

            notificationManager.Show(new NotificationContent
            {
                Title = "Thông Báo 🎉🎉",
                Message = "Chương trình bắt đầu thực thi",
                Type = NotificationType.Success
            }, expirationTime: TimeSpan.FromSeconds(3));

        }

        private void StopButton_OnClickopButton_Click(object sender, RoutedEventArgs e)
        {
            StopMic();
            //StopEvent();
        }

        void StopEvent()
        {
            _stopTaskCompletionSource.TrySetResult(0);
            StartButton.IsEnabled = true;
            StopButton.IsEnabled = false;
            ClearButton.IsEnabled = true;
            var notificationManager = new NotificationManager();

            notificationManager.Show(new NotificationContent
            {
                
                Title = "Thông Báo 🎉🎉",
                Message = "Chương trình dã tạm dừng",
                Type = NotificationType.Error,
                
                
            },expirationTime:TimeSpan.FromSeconds(3));
        }

        private void RecognitionCallback(string result)
        {
            this.Dispatcher.Invoke(() =>
            {
                SendMess send = new SendMess();
                send.send(result);
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
            keyboardHookManager.UnregisterAll();
            keyboardHookManager.Stop();
        }

        void RunFromMic()
        {

            connector = new MediaConnector();
            microphone = Microphone.GetDefaultDevice();

            StartButton.IsEnabled = false;
            StopButton.IsEnabled = true;
            ClearButton.IsEnabled = false;

            var notificationManager = new NotificationManager();

           

            var format = new WaveFormat(16000, 16, 1);

            microphone.ChangeFormat(format);

            googleSTT =
            new GoogleSTT(GoogleLanguage.Vietnamese,
                                        format.AsVoIPMediaFormat(),RecognitionCallback);

            connector.Connect(microphone, googleSTT);

            microphone.Start();

            googleSTT.Start();

            Console.WriteLine("Speak !!");
            notificationManager.Show(new NotificationContent
            {
                Title = "Thông Báo 🎉🎉",
                Message = "Chương trình bắt đầu thực thi",
                Type = NotificationType.Success
            }, expirationTime: TimeSpan.FromSeconds(3));

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
    }
}
