// Copyright 2017 The VncLib Authors. All rights reserved.
// Use of this source code is governed by a BSD-style
// license that can be found in the LICENSE file.

using System;

namespace VncLib
{
    public class NotSupportedServerMessageEventArgs : EventArgs
    {
        private string _MessageTypeName = "";
        private byte _MessageId;

        public NotSupportedServerMessageEventArgs(string msgType)
        {
            MessageTypeName = msgType;
        }

        public NotSupportedServerMessageEventArgs(string msgType, byte msgId)
        {
            MessageTypeName = msgType;
            MessageId = msgId;
        }

        public string MessageTypeName
        {
            get => _MessageTypeName;
            set => _MessageTypeName = value;
        }

        public byte MessageId
        {
            get => _MessageId;
            set => _MessageId = value;
        }
    }
}