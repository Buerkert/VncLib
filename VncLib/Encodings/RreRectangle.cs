// Copyright 2017 The VncLib Authors. All rights reserved.
// Use of this source code is governed by a BSD-style
// license that can be found in the LICENSE file.

using System.Drawing;

namespace VncLib.Encodings
{
	/// <summary>
	/// Implementation of RRE encoding, as well as drawing support. See RFB Protocol document v. 3.8 section 6.5.3.
	/// </summary>
	public sealed class RreRectangle : EncodedRectangle 
	{
		public RreRectangle(Rfb rfb, Framebuffer framebuffer, Rectangle rectangle)
			: base(rfb, framebuffer, rectangle, Rfb.RRE_ENCODING) 
		{
		}

		public override void Decode()
		{
			var numSubRect = (int) Rfb.ReadUint32();	// Number of sub-rectangles within this rectangle
			var bgPixelVal = Preader.ReadPixel();		// Background colour
			var subRectVal = 0;							// Colour to be used for each sub-rectangle
			
			// Dimensions of each sub-rectangle will be read into these
			int x, y, w, h;

			// Initialize the full pixel array to the background colour
			FillRectangle(Rectangle, bgPixelVal);

			// Colour in all the subrectangles, reading the properties of each one after another.
			for (var i = 0; i < numSubRect; i++) {
				subRectVal	= Preader.ReadPixel();
				x			= Rfb.ReadUInt16();
				y			= Rfb.ReadUInt16();
				w			= Rfb.ReadUInt16();
				h			= Rfb.ReadUInt16();
				
				// Colour in this sub-rectangle
				FillRectangle(new Rectangle(x, y, w, h), subRectVal);
			}
		}
	}
}