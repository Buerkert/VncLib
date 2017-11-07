// Copyright 2017 The VncLib Authors. All rights reserved.
// Use of this source code is governed by a BSD-style
// license that can be found in the LICENSE file.

using System.IO;

namespace VncLib.Encodings
{
	/// <summary>
	/// A 16-bit PixelReader.
	/// </summary>
	public sealed class PixelReader16 : PixelReader
	{
		public PixelReader16(BinaryReader reader, Framebuffer framebuffer) : base(reader, framebuffer)
		{
		}
	
		public override int ReadPixel()
		{
			var b = Reader.ReadBytes(2);

            var pixel = (ushort)((uint)b[0] & 0xFF | (uint)b[1] << 8);

			var red = (byte)(((pixel >> Framebuffer.RedShift) & Framebuffer.RedMax) * 255 / Framebuffer.RedMax);
			var green = (byte)(((pixel >> Framebuffer.GreenShift) & Framebuffer.GreenMax) * 255 / Framebuffer.GreenMax);
			var blue = (byte)(((pixel >> Framebuffer.BlueShift) & Framebuffer.BlueMax) * 255 / Framebuffer.BlueMax);

			return ToGdiPlusOrder(red, green, blue);			
		}
	}
}
