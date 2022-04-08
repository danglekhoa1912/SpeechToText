using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using ThreadState = System.Threading.ThreadState;

namespace ui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private KeyboardHookManager keyboardHookManager = new KeyboardHookManager();

        static MediaConnector connector;

        static Microphone microphone;

        static GoogleSTT googleSTT;

        static bool start1, start2, start3;
        static bool stop1, stop2, stop3;
        static bool delete1, delete2, delete3;

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

        public static bool Start1
        {
            get => start1;
            set => start1 = value;
        }

        public static bool Start2
        {
            get => start2;
            set => start2 = value;
        }

        public static bool Start3
        {
            get => start3;
            set => start3 = value;
        }

        public static bool Stop1
        {
            get => stop1;
            set => stop1 = value;
        }

        public static bool Stop2
        {
            get => stop2;
            set => stop2 = value;
        }

        public static bool Stop3
        {
            get => stop3;
            set => stop3 = value;
        }

        public static bool Delete1
        {
            get => delete1;
            set => delete1 = value;
        }

        public static bool Delete2
        {
            get => delete2;
            set => delete2 = value;
        }

        public static bool Delete3
        {
            get => delete3;
            set => delete3 = value;
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
                Console.WriteLine("check");
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

                //if ((bool)FromMic.IsChecked)
                //{
                //    send.Send(BusinessLogic.ProcessingContent(result));
                //}


                send.Send(BusinessLogic.ProcessingContent(result));

            });

        }


        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ClearEvent();
        }

        private void ClearEvent()
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
            keyboardHookManager.Start();
            setKeyHook();
        }

        public void setKeyHook()
        {

            if (!start1 && !start2 && !start3)
            {
                start2 = true;
            }
            if (!stop1 && !stop2 && !stop3)
            {
                stop2 = true;
            }
            if (!delete1 && !delete2 && !delete3)
            {
                delete2 = true;
            }
            ObservableCollection<ModifierKeys> startArr = new ObservableCollection<ModifierKeys>();
            ObservableCollection<ModifierKeys> stopArr = new ObservableCollection<ModifierKeys>();
            ObservableCollection<ModifierKeys> clearArr = new ObservableCollection<ModifierKeys>();

            if (start1)
            {
                startArr.Add(NonInvasiveKeyboardHookLibrary.ModifierKeys.Shift);
            }
            if (start2)
            {
                startArr.Add(NonInvasiveKeyboardHookLibrary.ModifierKeys.Control);
            }
            if (start3)
            {
                startArr.Add(NonInvasiveKeyboardHookLibrary.ModifierKeys.Alt);
            }
            if (stop1)
            {
                stopArr.Add(NonInvasiveKeyboardHookLibrary.ModifierKeys.Shift);

            }
            if (stop2)
            {
                stopArr.Add(NonInvasiveKeyboardHookLibrary.ModifierKeys.Control);
            }
            if (stop3)
            {
                stopArr.Add(NonInvasiveKeyboardHookLibrary.ModifierKeys.Alt);
            }
            if (delete1)
            {
                clearArr.Add(NonInvasiveKeyboardHookLibrary.ModifierKeys.Shift);
            }
            if (delete2)
            {
                clearArr.Add(NonInvasiveKeyboardHookLibrary.ModifierKeys.Control);
            }
            if (delete3)
            {
                clearArr.Add(NonInvasiveKeyboardHookLibrary.ModifierKeys.Alt);
            }
            keyboardHookManager.Stop();
            keyboardHookManager.Start();
            keyboardHookManager.UnregisterAll();
            Console.WriteLine(this.Dispatcher.Thread.ThreadState.ToString());

            keyboardHookManager.RegisterHotkey(startArr.ToArray(), 0x70, () =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        StartEvent();
                    });
                }
            );
            keyboardHookManager.RegisterHotkey(stopArr.ToArray(), 0x71, () =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    StopEvent();
                });
            });
            keyboardHookManager.RegisterHotkey(clearArr.ToArray(), 0x72, () =>
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

        void check()
        {
            MessageBox.Show("aa");
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

        private void Setting_Click(object sender, RoutedEventArgs e)
        {
            ui.Setting setting = new ui.Setting();

            setting.Show();
        }
    }
}
