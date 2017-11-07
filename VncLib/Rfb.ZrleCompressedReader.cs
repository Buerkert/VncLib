// Copyright 2017 The VncLib Authors. All rights reserved.
// Use of this source code is governed by a BSD-style
// license that can be found in the LICENSE file.

using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using VncLib.zlib.NET;

namespace VncLib
{
    public partial class Rfb
	{
        /// <summary>
        /// ZRLE compressed binary reader, used by ZrleRectangle.
        /// </summary>
        public sealed class ZrleCompressedReader : BinaryReader
		{
		    private readonly MemoryStream _zlibMemoryStream;
		    private readonly ZOutputStream _zlibDecompressedStream;
		    private readonly BinaryReader _uncompressedReader;

			public ZrleCompressedReader(Stream uncompressedStream) : base(uncompressedStream)
			{
				_zlibMemoryStream = new MemoryStream();
				_zlibDecompressedStream = new ZOutputStream(_zlibMemoryStream);
				_uncompressedReader = new BinaryReader(_zlibMemoryStream);
			}

			public override byte ReadByte()
			{
				return _uncompressedReader.ReadByte();
			}

			public override byte[] ReadBytes(int count)
			{
				return _uncompressedReader.ReadBytes(count);
			}

			public void DecodeStream()
			{
				// Reset position to use same buffer
				_zlibMemoryStream.Position = 0;

				// Get compressed stream length to read
				var buff = new byte[4];
				if (BaseStream.Read(buff, 0, 4) != 4)
					throw new Exception("ZRLE decoder: Invalid compressed stream size");

				// BigEndian to LittleEndian conversion
				var compressedBufferSize = buff[3] | buff[2] << 8 | buff[1] << 16 | buff[0] << 24;
				if (compressedBufferSize > 64 * 1024 * 1024)
					throw new Exception("ZRLE decoder: Invalid compressed data size");

				#region Decode stream
				// Decode stream
				// int pos = 0;
				// while (pos++ < compressedBufferSize)
				// 	zlibDecompressedStream.WriteByte(this.BaseStream.ReadByte());
				#endregion

				#region Decode stream in blocks
				// Decode stream in blocks
			    var bytesNeeded = compressedBufferSize;
				const int maxBufferSize = 64 * 1024; // 64k buffer
				var receiveBuffer = new byte[maxBufferSize];
				var netStream = (NetworkStream)BaseStream;
				do
				{
					if (netStream.DataAvailable)
					{
						var bytesToRead = bytesNeeded;

						// the byteToRead should never exceed the maxBufferSize
						if (bytesToRead > maxBufferSize)
							bytesToRead = maxBufferSize;

						// try reading bytes
						var bytesRead = netStream.Read(receiveBuffer, 0, bytesToRead);
						// lower the bytesNeeded with the bytesRead.
						bytesNeeded -= bytesRead;

						// write the readed bytes to the decompression stream.
						_zlibDecompressedStream.Write(receiveBuffer, 0, bytesRead);
					}
					else
						// there isn't any data atm. let's give the processor some time.
						Thread.Sleep(1);

				} while (bytesNeeded > 0);
				#endregion
				
				_zlibMemoryStream.Position = 0;
			}
		}
	}
}