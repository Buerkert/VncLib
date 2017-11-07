// Copyright 2017 The VncLib Authors. All rights reserved.
// Use of this source code is governed by a BSD-style
// license that can be found in the LICENSE file.

using System;
using System.IO;
using System.Text;

namespace VncLib
{
    public partial class Rfb
	{
        /// <summary>
        /// BigEndianBinaryWriter is a wrapper class used to write .NET integral types in Big-Endian order to a stream.  It inherits from BinaryWriter and adds Little- to Big-Endian conversion.
        /// </summary>
        protected sealed class BigEndianBinaryWriter : BinaryWriter
		{
			public BigEndianBinaryWriter(Stream input) : base(input)
			{
			}

			public BigEndianBinaryWriter(Stream input, Encoding encoding) : base(input, encoding)
			{
			}
			
			public override void Write(ushort value)
			{
				FlipAndWrite(BitConverter.GetBytes(value));
			}
			
			public override void Write(short value)
			{
				FlipAndWrite(BitConverter.GetBytes(value));
			}

			public override void Write(uint value)
			{
				FlipAndWrite(BitConverter.GetBytes(value));
			}
			
			public override void Write(int value)
			{
				FlipAndWrite(BitConverter.GetBytes(value));
			}

			public override void Write(ulong value)
			{
				FlipAndWrite(BitConverter.GetBytes(value));
			}
			
			public override void Write(long value)
			{
				FlipAndWrite(BitConverter.GetBytes(value));
			}

			private void FlipAndWrite(byte[] b)
			{
				// Given an array of bytes, flip and write to underlying stream
				Array.Reverse(b);
				base.Write(b);
			}
		}
	}
}