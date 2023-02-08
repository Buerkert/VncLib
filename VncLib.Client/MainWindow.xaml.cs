﻿// Copyright 2017 The VncLib Authors. All rights reserved.
// Use of this source code is governed by a BSD-style
// license that can be found in the LICENSE file.

using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace VncLib.Client
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            VncLibControl.VncLibUserCallback = VncLibUserCallback;
            VncLibControl.Connect();
            VncLibControl.UpdateScreen();
            VncLibControl.EnableMouseCapturing();
        }

        private void VncLibUserCallback(MouseEventArgs mouseEventArgs, double x, double y)
        {
            Console.WriteLine($"X: {x} Y: {y}");
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            VncLibControl.Disconnect();
        }
    }
}
