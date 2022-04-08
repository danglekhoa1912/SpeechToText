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
using System.Windows.Shapes;
using static ui.MainWindow;

namespace ui
{
    /// <summary>
    /// Interaction logic for Setting.xaml
    /// </summary>
    public partial class Setting : Window
    {
        bool start1, start2, start3;
        bool stop1, stop2, stop3;
        bool delete1, delete2, delete3;
        private MainWindow main = new MainWindow();
        public Setting()
        {
            InitializeComponent();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            start1 = (bool)Start1.IsChecked;
            start2 = (bool)Start2.IsChecked;
            start3 = (bool)Start3.IsChecked;

            stop1 = (bool)Stop1.IsChecked;
            stop2 = (bool)Stop2.IsChecked;
            stop3 = (bool)Stop3.IsChecked;

            delete1 = (bool)Delete1.IsChecked;
            delete2 = (bool)Delete2.IsChecked;
            delete3 = (bool)Delete3.IsChecked;

            MainWindow.Start1 = start1;
            MainWindow.Start2 = start2;
            MainWindow.Start3 = start3;
            
            MainWindow.Stop1 = stop1;
            MainWindow.Stop2 = stop2;
            MainWindow.Stop3 = stop3;

            MainWindow.Delete1 = delete1;
            MainWindow.Delete2 = delete2;
            MainWindow.Delete3 = delete3;
            //main.setKeyHook();
            this.Close();

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
