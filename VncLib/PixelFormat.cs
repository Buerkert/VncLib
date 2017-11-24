// Copyright 2017 The VncLib Authors. All rights reserved.
// Use of this source code is governed by a BSD-style
// license that can be found in the LICENSE file.

using System;

namespace VncLib
{
    public class PixelFormat
    {
        private Byte _BitsPerPixel;
        private Byte _Depth;
        private Boolean _BigEndianFlag;
        private Boolean _TrueColourFlag;
        private UInt16 _RedMax;
        private UInt16 _GreenMax;
        private UInt16 _BlueMax;
        private Byte _RedShift;
        private Byte _GreenShift;
        private Byte _BlueShift;
        private Byte[] _Padding = new Byte[3];

        public PixelFormat()
        {
        }

        public PixelFormat(Byte bpp, Byte dp, Boolean bef, Boolean tcf, UInt16 rm, UInt16 gm,
            UInt16 bm, Byte rs, Byte gs, Byte bs)
        {
            BitsPerPixel = bpp;
            Depth = dp;
            BigEndianFlag = bef;
            TrueColourFlag = tcf;
            RedMax = rm;
            GreenMax = gm;
            BlueMax = bm;
            RedShift = rs;
            GreenShift = gs;
            BlueShift = bs;
        }

        public byte[] getPixelFormat(bool bigEndianFlag)
        {
            var ret = new Byte[16];
            ret[0] = BitsPerPixel;
            ret[1] = Depth;
            ret[2] = Helper.ConvertToByteArray(BigEndianFlag)[0];
            ret[3] = Helper.ConvertToByteArray(TrueColourFlag)[0];
            ret[4] = Helper.ConvertToByteArray(RedMax, bigEndianFlag)[0];
            ret[5] = Helper.ConvertToByteArray(RedMax, bigEndianFlag)[1];
            ret[6] = Helper.ConvertToByteArray(GreenMax, bigEndianFlag)[0];
            ret[7] = Helper.ConvertToByteArray(GreenMax, bigEndianFlag)[1];
            ret[8] = Helper.ConvertToByteArray(BlueMax, bigEndianFlag)[0];
            ret[9] = Helper.ConvertToByteArray(BlueMax, bigEndianFlag)[1];
            ret[10] = RedShift;
            ret[11] = GreenShift;
            ret[12] = BlueShift;
            ret[13] = Padding[0];
            ret[14] = Padding[1];
            ret[15] = Padding[2];

            return ret;
        }

        public Byte BitsPerPixel
        {
            get { return _BitsPerPixel; }
            set { _BitsPerPixel = value; }
        }

        public Byte Depth
        {
            get { return _Depth; }
            set { _Depth = value; }
        }

        public Boolean BigEndianFlag
        {
            get { return _BigEndianFlag; }
            set { _BigEndianFlag = value; }
        }

        public Boolean TrueColourFlag
        {
            get { return _TrueColourFlag; }
            set { _TrueColourFlag = value; }
        }

        public UInt16 RedMax
        {
            get { return _RedMax; }
            set { _RedMax = value; }
        }

        public UInt16 GreenMax
        {
            get { return _GreenMax; }
            set { _GreenMax = value; }
        }

        public UInt16 BlueMax
        {
            get { return _BlueMax; }
            set { _BlueMax = value; }
        }

        public Byte RedShift
        {
            get { return _RedShift; }
            set { _RedShift = value; }
        }

        public Byte GreenShift
        {
            get { return _GreenShift; }
            set { _GreenShift = value; }
        }

        public Byte BlueShift
        {
            get { return _BlueShift; }
            set { _BlueShift = value; }
        }

        public Byte[] Padding
        {
            get { return _Padding; }
            set
            {
                if (value.Length == 3)
                    _Padding = value;
            }
        }
    }
}