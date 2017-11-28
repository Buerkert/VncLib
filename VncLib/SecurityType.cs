// Copyright 2017 The VncLib Authors. All rights reserved.
// Use of this source code is governed by a BSD-style
// license that can be found in the LICENSE file.

namespace VncLib
{
    public enum SecurityType
    {
        Unknown,
        Invalid, //0
        None, //1
        VNCAuthentication, //2
        RA2, //5
        RA2ne, //6
        Tight, //16
        UltraVNC, //17
        TLS, //18
        VeNCrypt, //19
        GTK_VNC_SASL, //20
        MD5_hash_authentication, //21
        Colin_Dean_xvp //22
    }
}