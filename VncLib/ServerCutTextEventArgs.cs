// Copyright 2017 The VncLib Authors. All rights reserved.
// Use of this source code is governed by a BSD-style
// license that can be found in the LICENSE file.

using System;

namespace VncLib
{
    public class ServerCutTextEventArgs : EventArgs
    {
        private string _Text = "";

        public ServerCutTextEventArgs(string text)
        {
            _Text = text;
        }

        public string Text
        {
            get => _Text;
            set => _Text = value;
        }
    }
}