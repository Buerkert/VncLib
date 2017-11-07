// Copyright 2017 The VncLib Authors. All rights reserved.
// Use of this source code is governed by a BSD-style
// license that can be found in the LICENSE file.

using System.Drawing;

namespace VncLib.Encodings
{
	/// <summary>
	/// Implementation of Hextile encoding, as well as drawing support. See RFB Protocol document v. 3.8 section 6.5.5.
	/// </summary>
	public sealed class HextileRectangle : EncodedRectangle 
	{
		private const int Raw					= 0x01;
		private const int BackgroundSpecified	= 0x02;
		private const int ForegroundSpecified	= 0x04;
		private const int AnySubrects			= 0x08;
		private const int SubrectsColoured		= 0x10;

		public HextileRectangle(Rfb rfb, Framebuffer framebuffer, Rectangle rectangle)
			: base(rfb, framebuffer, rectangle, Rfb.HEXTILE_ENCODING) 
		{
		}

		public override void Decode()
		{
			// Subrectangle co-ordinates and info
			int sx;
			int sy;
			int sw;
			int sh;
			var numSubrects = 0;
			int xAnDy;
			int widthAnDheight;

			// Colour values to be used--black by default.
			var backgroundPixelValue = 0;
			var foregroundPixelValue = 0;
			
			// NOTE: the way that this is set-up, a Rectangle can be anywhere within the bounds
			// of the framebuffer (i.e., its x and y may not be (0,0)).  However, I ignore this
			// since the pixels for the tiles and subrectangles are all relative to this rectangle.
			// When the rectangle is drawn to the desktop later, its (x,y) position will become
			// significant again.  All of this to say that in the two main loops below, ty=0 and
			// tx=0, and all calculations are based on a (0,0) origin.
			for (var ty = 0; ty < Rectangle.Height; ty += 16) {			
				// Tiles in the last row will often be less than 16 pixels high.
				// All others will be 16 high.
				var th = Rectangle.Height - ty < 16 ? Rectangle.Height - ty : 16;

				for (var tx = 0; tx < Rectangle.Width; tx += 16) {				
					// Tiles in the list column will often be less than 16 pixels wide.
					// All others will be 16 wide.
					var tw = Rectangle.Width - tx < 16 ? Rectangle.Width - tx : 16;

					var tlStart = ty * Rectangle.Width + tx;
					var tlOffset = Rectangle.Width - tw;

					var subencoding = Rfb.ReadByte();

					// See if Raw bit is set in subencoding, and if so, ignore all other bits
					if ((subencoding & Raw) != 0) {
						FillRectangle(new Rectangle(tx, ty, tw, th));
					} else {
						if ((subencoding & BackgroundSpecified) != 0) {
							backgroundPixelValue = Preader.ReadPixel();
						}

						// Fill-in background colour
						FillRectangle(new Rectangle(tx, ty, tw, th), backgroundPixelValue);
												
						if ((subencoding & ForegroundSpecified) != 0) {
							foregroundPixelValue = Preader.ReadPixel();
						}

						if ((subencoding & AnySubrects) != 0) {
							// Get the number of sub-rectangles in this tile
							numSubrects = Rfb.ReadByte();

							for (var i = 0; i < numSubrects; i++) {
								if ((subencoding & SubrectsColoured) != 0) {
									foregroundPixelValue = Preader.ReadPixel();	// colour of this sub rectangle
								}

								xAnDy = Rfb.ReadByte();					// X-position (4 bits) and Y-Postion (4 bits) of this sub rectangle in the tile
								widthAnDheight = Rfb.ReadByte();		// Width (4 bits) and Height (4 bits) of this sub rectangle
								
								// Get the proper x, y, w, and h values out of xANDy and widthANDheight
								sx = (xAnDy >> 4) & 0xf;
								sy = xAnDy & 0xf;
								sw = ((widthAnDheight >> 4) & 0xf) + 1;	// have to add 1 to get width
								sh = (widthAnDheight & 0xf) + 1;		// same for height.

								FillRectangle(new Rectangle(tx + sx, ty + sy, sw, sh), foregroundPixelValue);
							}
						}
					}
				}
			}
		}
	}
}