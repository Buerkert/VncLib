// Copyright 2017 The VncLib Authors. All rights reserved.
// Use of this source code is governed by a BSD-style
// license that can be found in the LICENSE file.

using System;

namespace VncLib
{
    public class ConnectionFailedEventArgs : EventArgs
    {
        private string _ShortMessage = "";
        private string _FullMessage = "";
        private int _ErrorNumber;

        public ConnectionFailedEventArgs(string sMsg, string fMsg, int eNo)
        {
            ShortMessage = sMsg;
            FullMessage = fMsg;
            ErrorNumber = eNo;
        }

        public string ShortMessage
        {
            get => _ShortMessage;
            set => _ShortMessage = value;
        }

        public string FullMessage
        {
            get => _FullMessage;
            set => _FullMessage = value;
        }

        public int ErrorNumber
        {
            get => _ErrorNumber;
            set => _ErrorNumber = value;
        }
    }
}