// Copyright 2017 The VncLib Authors. All rights reserved.
// Use of this source code is governed by a BSD-style
// license that can be found in the LICENSE file.

using System;

namespace VncLib
{
    public class LogMessageEventArgs : EventArgs
    {
        private string _logMessage = "";
        private DateTime _logTime;
        private Logtype _logType;

        public LogMessageEventArgs()
        {
        }

        public LogMessageEventArgs(string logMessage, DateTime logTime, Logtype logType)
        {
            _logMessage = logMessage;
            _logTime = logTime;
            _logType = logType;
        }

        public string LogMessage
        {
            get => _logMessage;
            set => _logMessage = value;
        }

        public DateTime LogTime
        {
            get => _logTime;
            set => _logTime = value;
        }

        public Logtype LogType
        {
            get => _logType;
            set => _logType = value;
        }
    }
}