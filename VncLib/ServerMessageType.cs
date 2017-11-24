// Copyright 2017 The VncLib Authors. All rights reserved.
// Use of this source code is governed by a BSD-style
// license that can be found in the LICENSE file.

namespace VncLib
{
    internal enum ServerMessageType
    {
        Unknown,
        FramebufferUpdate, //0
        SetColourMapEntries, //1
        Bell, //2
        ServerCutText, //3
        OLIVE_Call_Control, //249
        Colin_dean_xvp, //250
        tight, //252
        gii, //253
        VMWare, //254/127
        Anthony_Liguori, //255
        Pseudo_DesktopSize, //FF FF FF 21 / -239
        Pseudo_Cursor //FF FF FF 11 / -223
    }
}