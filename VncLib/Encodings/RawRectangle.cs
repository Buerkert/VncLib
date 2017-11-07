// Copyright 2017 The VncLib Authors. All rights reserved.
// Use of this source code is governed by a BSD-style
// license that can be found in the LICENSE file.

using System.Drawing;

namespace VncLib.Encodings
{
	/// <summary>
	/// Implementation of Raw encoding, as well as drawing support. See RFB Protocol document v. 3.8 section 6.5.1.
	/// </summary>
	public sealed class RawRectangle : EncodedRectangle
	{
		public RawRectangle(Rfb rfb, Framebuffer framebuffer, Rectangle rectangle)
			: base(rfb, framebuffer, rectangle, Rfb.RAW_ENCODING)
		{
		}

		public override void Decode()
		{
			// Each pixel from the remote server represents a pixel to be drawn
			for (var i = 0; i < Rectangle.Width * Rectangle.Height; ++i) {
				Framebuffer[i] = Preader.ReadPixel();
			}
		}
	}
}
