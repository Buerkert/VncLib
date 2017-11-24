// Copyright 2017 The VncLib Authors. All rights reserved.
// Use of this source code is governed by a BSD-style
// license that can be found in the LICENSE file.

using System;
using System.Collections.Generic;

namespace VncLib
{
    public class ScreenUpdateEventArgs : EventArgs
    {
        public ScreenUpdateEventArgs() { }

        public List<RfbRectangle> Rects { get; set; }

        //public byte[] PixelData { get; set; }

        //public UInt16 X { get; set; }

        //public UInt16 Y { get; set; }

        //public UInt16 Width { get; set; }

        //public UInt16 Height { get; set; }
    }
}