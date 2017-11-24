// Copyright 2017 The VncLib Authors. All rights reserved.
// Use of this source code is governed by a BSD-style
// license that can be found in the LICENSE file.

namespace VncLib
{
    internal enum ClientMessageType
    {
        Unknown,
        SetPixelFormat, //0
        SetEncodings, //2
        FramebufferUpdateRequest, //3
        KeyEvent, //4
        PointerEvent, //5
        ClientCutText, //6
        OLIVE_Call_Control, //249
        Colin_dean_xvp, //250
        Pierre_Ossman_SetDesktopSize, //251
        tight, //252
        gii, //253
        VMWare, //254/127
        Anthony_Liguori //255
    }
}