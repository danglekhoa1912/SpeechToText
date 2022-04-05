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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ui
{
    /// <summary>
    /// Interaction logic for Help.xaml
    /// </summary>
    public partial class Help : Window
    {
        public Help()
        {
            InitializeComponent();
            lblHelp.Content = "Bắt đấu : Ctrl + F1\nDừng nhập : Ctrl + F2\nXóa dữ liệu : Ctrl+F3";
            lblCommand.Content = "Để nhập dấu bất kì vui lòng nói nhập dấu + tên dấu câu\nĐể xuống dòng vui lòng nói 'Nhập dấu xuống dòng'\n ";
        }
    }
}
