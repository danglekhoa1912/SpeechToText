﻿using System;
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



            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
