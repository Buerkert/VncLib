// Copyright 2017 The VncLib Authors. All rights reserved.
// Use of this source code is governed by a BSD-style
// license that can be found in the LICENSE file.

using System.IO;

namespace VncLib.Encodings
{
	/// <summary>
	/// Used to read the appropriate number of bytes from the server based on the
	/// width of pixels and convert to a GDI+ colour value (i.e., BGRA).
	/// </summary>
	public abstract class PixelReader
	{
		protected BinaryReader Reader;
		protected Framebuffer Framebuffer;

		protected PixelReader(BinaryReader reader, Framebuffer framebuffer)
		{
			this.Reader = reader;
			this.Framebuffer = framebuffer;
		}

		public abstract int ReadPixel();

		protected int ToGdiPlusOrder(byte red, byte green, byte blue)
		{
			// Put colour values into proper order for GDI+ (i.e., BGRA, where Alpha is always 0xFF)
			return blue & 0xFF | green << 8 | red << 16 | 0xFF << 24;			
		}
	}
}
