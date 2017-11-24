// Copyright 2017 The VncLib Authors. All rights reserved.
// Use of this source code is governed by a BSD-style
// license that can be found in the LICENSE file.

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using VncLib.Client.Annotations;

namespace VncLib.Client
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private string _serverAddress;
        private string _serverPassword;
        private int _port;
        private bool _connect;
        private string _serverPort;

        public string ServerAddress
        {
            get { return _serverAddress; }
            set
            {
                _serverAddress = value;
                OnPropertyChanged();
            }
        }

        public string ServerPassword
        {
            get { return _serverPassword; }
            set
            {
                _serverPassword = value;
                OnPropertyChanged();
            }
        }

        public int Port
        {
            get { return _port; }
            set
            {
                _port = value;
                OnPropertyChanged();
            }
        }

        public bool Connect
        {
            get { return _connect; }
            set
            {
                _connect = value;
                OnPropertyChanged();
            }
        }

        public string ServerPort
        {
            get { return _serverPort; }
            set
            {
                _serverPort = value;
                Port = Convert.ToInt32(_serverPort);
                OnPropertyChanged();
            }
        }

        public ICommand ConnectCommand
        {
            get { return new CommandHandler(() => Connect = true, true); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class CommandHandler : ICommand
    {
        private Action _action;
        private bool _canExecute;
        public CommandHandler(Action action, bool canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _action();
        }
    }
}