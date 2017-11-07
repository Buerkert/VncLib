// Copyright 2017 The VncLib Authors. All rights reserved.
// Use of this source code is governed by a BSD-style
// license that can be found in the LICENSE file.

using System.IO;

namespace VncLib.Encodings
{
	/// <summary>
	/// An 8-bit PixelReader
	/// </summary>
	public sealed class PixelReader8 : PixelReader
	{
		private Rfb _rfb;

		public PixelReader8(BinaryReader reader, Framebuffer framebuffer, Rfb rfb) : base(reader, framebuffer)
		{
			this._rfb = rfb;
		}
	
		/// <summary>
		/// Reads an 8-bit pixel.
		/// </summary>
		/// <returns>Returns an Integer value representing the pixel in GDI+ format.</returns>
		public override int ReadPixel()
		{
			var idx = Reader.ReadByte();
			return ToGdiPlusOrder((byte)_rfb.MapEntries[idx, 0], (byte)_rfb.MapEntries[idx, 1], (byte)_rfb.MapEntries[idx, 2]);
		}
	}
}
