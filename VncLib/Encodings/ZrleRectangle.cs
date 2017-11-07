// Copyright 2017 The VncLib Authors. All rights reserved.
// Use of this source code is governed by a BSD-style
// license that can be found in the LICENSE file.

using System;
using System.Drawing;

namespace VncLib.Encodings
{
	/// <summary>
	/// Implementation of ZRLE encoding, as well as drawing support. See RFB Protocol document v. 3.8 section 6.6.5.
	/// </summary>
	public sealed class ZrleRectangle : EncodedRectangle
	{
		private const int TileWidth = 64;
		private const int TileHeight = 64;

		private readonly int[] _palette = new int[128];
		private readonly int[] _tileBuffer = new int[TileWidth * TileHeight];

		public ZrleRectangle(Rfb rfb, Framebuffer framebuffer, Rectangle rectangle)
			: base(rfb, framebuffer, rectangle, Rfb.ZRLE_ENCODING)
		{
		}

		public override void Decode()
		{
			Rfb.ZrleReader.DecodeStream();

			for (var ty = 0; ty < Rectangle.Height; ty += TileHeight) {
				var th = Math.Min(Rectangle.Height - ty, TileHeight);

				for (var tx = 0; tx < Rectangle.Width; tx += TileWidth) {
					var tw = Math.Min(Rectangle.Width - tx, TileWidth);

					var subencoding = Rfb.ZrleReader.ReadByte();

					if (subencoding >= 17 && subencoding <= 127 || subencoding == 129)
						throw new Exception("Invalid subencoding value");

					var isRle = (subencoding & 128) != 0;
					var paletteSize = subencoding & 127;

					// Fill palette
					for (var i = 0; i < paletteSize; i++)
						_palette[i] = Preader.ReadPixel();

					if (paletteSize == 1) {
						// Solid tile
						FillRectangle(new Rectangle(tx, ty, tw, th), _palette[0]);
						continue;
					}

					if (!isRle) {
						if (paletteSize == 0) {
							// Raw pixel data
							FillRectangle(new Rectangle(tx, ty, tw, th));
						} else {
							// Packed palette
							ReadZrlePackedPixels(tw, th, _palette, paletteSize, _tileBuffer);
							FillRectangle(new Rectangle(tx, ty, tw, th), _tileBuffer);
						}
					} else {
						if (paletteSize == 0) {
							// Plain RLE
							ReadZrlePlainRlePixels(tw, th, _tileBuffer);
							FillRectangle(new Rectangle(tx, ty, tw, th), _tileBuffer);
						} else {
							// Packed RLE palette
							ReadZrlePackedRlePixels(tx, ty, tw, th, _palette, _tileBuffer);
							FillRectangle(new Rectangle(tx, ty, tw, th), _tileBuffer);
						}
					}
				}
			}
		}
		
		private void ReadZrlePackedPixels(int tw, int th, int[] palette, int palSize, int[] tile)
		{
			var bppp = palSize > 16 ? 8 :
			    (palSize > 4 ? 4 : (palSize > 2 ? 2 : 1));
			var ptr = 0;

			for (var i = 0; i < th; i++) {
				var eol = ptr + tw;
				var b = 0;
				var nbits = 0;

				while (ptr < eol) {
					if (nbits == 0)	{
						b = Rfb.ZrleReader.ReadByte();
						nbits = 8;
					}
					nbits -= bppp;
					var index = (b >> nbits) & ((1 << bppp) - 1) & 127;
					tile[ptr++] = palette[index];
				}
			}
		}

		private void ReadZrlePlainRlePixels(int tw, int th, int[] tileBuffer)
		{
			var ptr = 0;
			var end = ptr + tw * th;
			while (ptr < end) {
				var pix = Preader.ReadPixel();
				var len = 1;
				int b;
				do {
					b = Rfb.ZrleReader.ReadByte();
					len += b;
				} while (b == byte.MaxValue);

				while (len-- > 0) tileBuffer[ptr++] = pix;
			}
		}

		private void ReadZrlePackedRlePixels(int tx, int ty, int tw, int th, int[] palette, int[] tile)
		{
			var ptr = 0;
			var end = ptr + tw * th;
			while (ptr < end) {
				int index = Rfb.ZrleReader.ReadByte();
				var len = 1;
				if ((index & 128) != 0) {
					int b;
					do {
						b = Rfb.ZrleReader.ReadByte();
						len += b;
					} while (b == byte.MaxValue);
				}

				index &= 127;

				while (len-- > 0) tile[ptr++] = palette[index];
			}
		}
	}
}