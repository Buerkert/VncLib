// Copyright 2017 The VncLib Authors. All rights reserved.
// Use of this source code is governed by a BSD-style
// license that can be found in the LICENSE file.

using System.IO;
using System.Text;

namespace VncLib
{
    public partial class Rfb
	{
        /// <summary>
        /// BigEndianBinaryReader is a wrapper class used to read .NET integral types from a Big-Endian stream.  It inherits from BinaryReader and adds Big- to Little-Endian conversion.
        /// </summary>
        protected sealed class BigEndianBinaryReader : BinaryReader
		{
			private readonly byte[] _buff = new byte[4];

			public BigEndianBinaryReader(Stream input) : base(input)
			{
			}
			
			public BigEndianBinaryReader(Stream input, Encoding encoding) : base(input, encoding)
			{
			}

			// Since this is being used to communicate with an RFB host, only some of the overrides are provided below.
	
			public override ushort ReadUInt16()
			{
				FillBuff(2);
				return (ushort)(_buff[1] | (uint)_buff[0] << 8);
				
			}
			
			public override short ReadInt16()
			{
				FillBuff(2);
				return (short)(_buff[1] & 0xFF | _buff[0] << 8);
			}

			public override uint ReadUInt32()
			{
				FillBuff(4);
				return (uint)_buff[3] & 0xFF | (uint)_buff[2] << 8 | (uint)_buff[1] << 16 | (uint)_buff[0] << 24;
			}
			
			public override int ReadInt32()
			{
				FillBuff(4);
				return _buff[3] | _buff[2] << 8 | _buff[1] << 16 | _buff[0] << 24;
			}

			private void FillBuff(int totalBytes)
			{
				var bytesRead = 0;

			    do {
					var n = BaseStream.Read(_buff, bytesRead, totalBytes - bytesRead);
					
					if (n == 0)
						throw new IOException("Unable to read next byte(s).");

					bytesRead += n;
				} while (bytesRead < totalBytes);
			}
		}
	}
}