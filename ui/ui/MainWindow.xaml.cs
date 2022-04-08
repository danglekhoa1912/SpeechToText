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
using System.Threading;
using Application = System.Windows.Forms.Application;
using System.Reflection;

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
            StopButton.IsEnabled = false;
            ClearButton.IsEnabled = false;
            slash_1.Visibility = Visibility.Hidden;
            FromMic.IsChecked = true;
            fileNameTextBox.IsReadOnly = true;

        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            StartEvent();
        }

        void StartEvent()
        {
            StartButton.IsEnabled = false;
            slash_1.Visibility = Visibility.Visible;
            StopButton.IsEnabled = true;
            slash2.Visibility = Visibility.Hidden;
            ClearButton.IsEnabled = false;
            slash3.Visibility = Visibility.Visible;
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
            slash_1.Visibility = Visibility.Hidden;
            StopButton.IsEnabled = false;
            slash2.Visibility = Visibility.Visible;
            ClearButton.IsEnabled = true;
            slash3.Visibility = Visibility.Hidden;
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
                DisplayText.Text += "-" + BusinessLogic.ProcessingContent(result) + "\n";
                send.Run(BusinessLogic.ProcessingContent(result));
            });

        }


        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ClearEvent();
        }

        private void ClearEvent()
        {
            /*WMPLib.WindowsMediaPlayer wplayer = new WMPLib.WindowsMediaPlayer();
            wplayer.URL = Path.GetFullPath(@"../../../../Sound/delete.mp3");
            wplayer.controls.play();*/
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
                WMPLib.WindowsMediaPlayer wplayer = new WMPLib.WindowsMediaPlayer();
                wplayer.URL = Path.GetFullPath(@"../../../../Sound/file-error.mp3");
                wplayer.controls.play();
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
            keyboardHookManager.RegisterHotkey(new[] { NonInvasiveKeyboardHookLibrary.ModifierKeys.Control }, 0x72, () =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    ClearEvent();
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

       
        }
        void StopMic()
        {
            microphone.Stop();
            googleSTT.Stop();
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            ui.Help help = new ui.Help();
            help.ShowDialog();
        }
    }
}
