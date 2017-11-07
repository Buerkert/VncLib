// Copyright 2017 The VncLib Authors. All rights reserved.
// Use of this source code is governed by a BSD-style
// license that can be found in the LICENSE file.

using System.IO;

namespace VncLib.Encodings
{
	/// <summary>
	/// A compressed PixelReader.
	/// </summary>
	public sealed class CPixelReader : PixelReader
	{
		public CPixelReader(BinaryReader reader, Framebuffer framebuffer) : base(reader, framebuffer)
		{
		}

		public override int ReadPixel()
		{
			var b = Reader.ReadBytes(3);
			return ToGdiPlusOrder(b[2], b[1], b[0]);
		}
	}
}
