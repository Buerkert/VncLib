// Copyright 2017 The VncLib Authors. All rights reserved.
// Use of this source code is governed by a BSD-style
// license that can be found in the LICENSE file.

using System;

namespace VncLib
{
    public class RfbRectangle
    {
        public RfbRectangle()
        {
        }

        public RfbRectangle(UInt16 posX, UInt16 posY, UInt16 width, UInt16 height, Byte[] encodingType)
        {
            PosX = posX;
            PosY = posY;
            Width = width;
            Height = height;
            EncodingType = encodingType;
        }

        public UInt16 PosX { get; set; }

        public UInt16 PosY { get; set; }

        public UInt16 Width { get; set; }

        public UInt16 Height { get; set; }

        public Byte[] EncodingType { get; set; }

        public Byte[] PixelData { get; set; }
    }
}
