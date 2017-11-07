// Copyright 2017 The VncLib Authors. All rights reserved.
// Use of this source code is governed by a BSD-style
// license that can be found in the LICENSE file.

using System.Drawing;

// FIXME: I can't understand why yet, but under the Xvnc server in Unix (v. 3.3.7), this doesn't work.  
// Everything is fine using the Windows server!?

namespace VncLib.Encodings
{
	/// <summary>
	/// Implementation of CoRRE encoding, as well as drawing support. See RFB Protocol document v. 3.8 section 6.5.4.
	/// </summary>
	public sealed class CoRreRectangle : EncodedRectangle 
	{
		public CoRreRectangle(Rfb rfb, Framebuffer framebuffer, Rectangle rectangle) : base(rfb, framebuffer, rectangle, Rfb.CORRE_ENCODING)
		{
		}

		/// <summary>
		/// Decodes a CoRRE Encoded Rectangle.
		/// </summary>
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
				x			= Rfb.ReadByte();
				y			= Rfb.ReadByte();
				w			= Rfb.ReadByte();
				h			= Rfb.ReadByte();
				
				// Colour in this sub-rectangle with the colour provided.
				FillRectangle(new Rectangle(x, y, w, h), subRectVal);
			}
		}
	}
}