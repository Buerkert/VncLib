// Copyright 2017 The VncLib Authors. All rights reserved.
// Use of this source code is governed by a BSD-style
// license that can be found in the LICENSE file.

using System;

namespace VncLib
{
    internal class ConnectionProperties
    {
        private string _RfbServerVersion = "";
        private Version _RfbServerVersion2 = new Version();
        private string _RfbClientVersion = "";
        private Version _RfbClientVersion2 = new Version();
        private SecurityType _RfbSecurityType = SecurityType.Invalid;

        private UInt16 _FramebufferWidth;
        private UInt16 _FramebufferHeight;
        private PixelFormat _PxFormat = new PixelFormat();
        private string _ConnectionName = "";
        private RfbEncoding _EncodingType;

        private string _Server = "";
        private int _Port = 5900;
        private string _Password = "";
        private bool _SharedFlag = true;

        public ConnectionProperties()
        {
        }

        public ConnectionProperties(string server, string password, int port)
        {
            RfbClientVersion = "RFB 003.008\n";
            Server = server;
            Password = password;
            Port = port;
        }

        public string RfbServerVersion
        {
            get { return _RfbServerVersion; }
            set
            {
                _RfbServerVersion = value;

                var strVer = value.Substring(4).Replace("\n", "");
                var strVer2 = strVer.Split('.');

                _RfbServerVersion2 = new Version(Convert.ToInt16(strVer2[0]), Convert.ToInt16(strVer2[1]));
            }
        }

        public Version RfbServerVersion2
        {
            get { return _RfbServerVersion2; }
        }

        public string RfbClientVersion
        {
            get { return _RfbClientVersion; }
            set
            {
                _RfbClientVersion = value;

                var strVer = value.Substring(4).Replace("\n", "");
                var strVer2 = strVer.Split('.');

                _RfbClientVersion2 = new Version(Convert.ToInt16(strVer2[0]), Convert.ToInt16(strVer2[1]));
            }
        }

        public Version RfbClientVersion2
        {
            get { return _RfbClientVersion2; }
        }

        public SecurityType RfbSecurityType
        {
            get { return _RfbSecurityType; }
            set { _RfbSecurityType = value; }
        }

        public UInt16 FramebufferWidth
        {
            get { return _FramebufferWidth; }
            set { _FramebufferWidth = value; }
        }

        public UInt16 FramebufferHeight
        {
            get { return _FramebufferHeight; }
            set { _FramebufferHeight = value; }
        }

        public PixelFormat PxFormat
        {
            get { return _PxFormat; }
            set { _PxFormat = value; }
        }

        public string ConnectionName
        {
            get { return _ConnectionName; }
            set { _ConnectionName = value; }
        }

        public RfbEncoding EncodingType
        {
            get { return _EncodingType; }
            set { _EncodingType = value; }
        }

        public string Server
        {
            get { return _Server; }
            set { _Server = value; }
        }

        public int Port
        {
            get { return _Port; }
            set { _Port = value; }
        }

        public string Password
        {
            get { return _Password; }
            set { _Password = value; }
        }

        public bool SharedFlag
        {
            get { return _SharedFlag; }
            set { _SharedFlag = value; }
        }
    }
}