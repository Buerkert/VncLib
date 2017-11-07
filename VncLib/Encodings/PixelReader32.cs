// Copyright 2017 The VncLib Authors. All rights reserved.
// Use of this source code is governed by a BSD-style
// license that can be found in the LICENSE file.

using System.IO;

namespace VncLib.Encodings
{
	/// <summary>
	/// A 32-bit PixelReader.
	/// </summary>
	public sealed class PixelReader32 : PixelReader
	{
		public PixelReader32(BinaryReader reader, Framebuffer framebuffer) : base(reader, framebuffer)
		{
		}
	
		public override int ReadPixel()
		{
			// Read the pixel value
			var b = Reader.ReadBytes(4);

            var pixel = (uint)b[0] & 0xFF | 
                         (uint)b[1] << 8   | 
                         (uint)b[2] << 16  | 
                         (uint)b[3] << 24;

			// Extract RGB intensities from pixel
			var red   = (byte) ((pixel >> Framebuffer.RedShift)   & Framebuffer.RedMax);
			var green = (byte) ((pixel >> Framebuffer.GreenShift) & Framebuffer.GreenMax);
			var blue  = (byte) ((pixel >> Framebuffer.BlueShift)  & Framebuffer.BlueMax);

			return ToGdiPlusOrder(red, green, blue);			
		}
	}
}
