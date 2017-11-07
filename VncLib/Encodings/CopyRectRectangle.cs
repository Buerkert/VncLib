// Copyright 2017 The VncLib Authors. All rights reserved.
// Use of this source code is governed by a BSD-style
// license that can be found in the LICENSE file.

using System.Drawing;
using System.Drawing.Imaging;

namespace VncLib.Encodings
{
	/// <summary>
	/// Implementation of CopyRect encoding, as well as drawing support. See RFB Protocol document v. 3.8 section 6.5.2.
	/// </summary>
	public sealed class CopyRectRectangle : EncodedRectangle 
	{
		public CopyRectRectangle(Rfb rfb, Framebuffer framebuffer, Rectangle rectangle)
			: base(rfb, framebuffer, rectangle, Rfb.COPYRECT_ENCODING) 
		{
		}

		// CopyRect Source Point (x,y) from which to copy pixels in Draw
	    private Point _source;

		/// <summary>
		/// Decodes a CopyRect encoded rectangle.
		/// </summary>
		public override void Decode()
		{
			// Read the source point from which to begin copying pixels
			_source = new Point();
			_source.X = Rfb.ReadUInt16();
			_source.Y = Rfb.ReadUInt16();
		}

		public unsafe override void Draw(Bitmap desktop)
		{
			// Given a source area, copy this region to the point specified by destination
			var bmpd = desktop.LockBits(new Rectangle(new Point(0,0), desktop.Size),
											   ImageLockMode.ReadWrite, 
											   desktop.PixelFormat);

			
			// Avoid exception if window is dragged bottom of screen
			if (Rectangle.Top + Rectangle.Height >= Framebuffer.Height)
			{
				Rectangle.Height = Framebuffer.Height - Rectangle.Top - 1;
			}

			try {
				var pSrc  = (int*)(void*)bmpd.Scan0;
				var pDest = (int*)(void*)bmpd.Scan0;

                // Calculate the difference between the stride of the desktop, and the pixels we really copied. 
                var nonCopiedPixelStride = desktop.Width - Rectangle.Width;

                // Move source and destination pointers
                pSrc += _source.Y * desktop.Width + _source.X;
                pDest += Rectangle.Y * desktop.Width + Rectangle.X;

                // BUG FIX (Peter Wentworth) EPW:  we need to guard against overwriting old pixels before
                // they've been moved, so we need to work out whether this slides pixels upwards in memeory,
                // or downwards, and run the loop backwards if necessary. 
                if (pDest < pSrc) {   // we can copy with pointers that increment
                    for (var y = 0; y < Rectangle.Height; ++y) {
                        for (var x = 0; x < Rectangle.Width; ++x) {
                            *pDest++ = *pSrc++;
                        }

                        // Move pointers to beginning of next row in rectangle
                        pSrc  += nonCopiedPixelStride;
                        pDest += nonCopiedPixelStride;
                    }
                } else {
                    // Move source and destination pointers to just beyond the furthest-from-origin 
                    // pixel to be copied.
                    pSrc  += Rectangle.Height * desktop.Width + Rectangle.Width;
                    pDest += Rectangle.Height * desktop.Width + Rectangle.Width;

                    for (var y = 0; y < Rectangle.Height; ++y) {
                        for (var x = 0; x < Rectangle.Width; ++x) {
                            *--pDest = *--pSrc;
                        }

                        // Move pointers to end of previous row in rectangle
                        pSrc  -= nonCopiedPixelStride;
                        pDest -= nonCopiedPixelStride;
                    }
                }
			} finally {
				desktop.UnlockBits(bmpd);
				bmpd = null;
			}
		}
	}
}