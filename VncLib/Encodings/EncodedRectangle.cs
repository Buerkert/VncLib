// Copyright 2017 The VncLib Authors. All rights reserved.
// Use of this source code is governed by a BSD-style
// license that can be found in the LICENSE file.

using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace VncLib.Encodings
{
	/// <summary>
	/// Abstract class representing an Encoded Rectangle to be read, decoded, and drawn.
	/// </summary>
	public abstract class EncodedRectangle : IDesktopUpdater
	{
		protected Rfb	Rfb;
		protected Rectangle		Rectangle;
		protected Framebuffer	Framebuffer;
		protected PixelReader	Preader;

		public EncodedRectangle(Rfb rfb, Framebuffer framebuffer, Rectangle rectangle, int encoding)
		{
			this.Rfb = rfb;
			this.Framebuffer = framebuffer;
			this.Rectangle = rectangle;

			//Select appropriate reader
			var reader = encoding == Rfb.ZRLE_ENCODING ? rfb.ZrleReader : rfb.Reader;

			// Create the appropriate PixelReader depending on screen size and encoding
			switch (framebuffer.BitsPerPixel)
			{
				case 32:
					if (encoding == Rfb.ZRLE_ENCODING)
					{
						Preader = new CPixelReader(reader, framebuffer);
					}
					else
					{
						Preader = new PixelReader32(reader, framebuffer);
					}
					break;
				case 16:
					Preader = new PixelReader16(reader, framebuffer);
					break;
				case 8:
					Preader = new PixelReader8(reader, framebuffer, rfb);
					break;
				default:
					throw new ArgumentOutOfRangeException("BitsPerPixel", framebuffer.BitsPerPixel, "Valid VNC Pixel Widths are 8, 16 or 32 bits.");
			}
		}

		/// <summary>
		/// Gets the rectangle that needs to be decoded and drawn.
		/// </summary>
		public Rectangle UpdateRectangle => Rectangle;

	    /// <summary>
		/// Obtain all necessary information from VNC Host (i.e., read) in order to Draw the rectangle, and store in colours[].
		/// </summary>
		public abstract void Decode();
		
		/// <summary>
		/// After calling Decode() an EncodedRectangle can be drawn to a Bitmap, which is the local representation of the remote desktop.
		/// </summary>
		/// <param name="desktop">The image the represents the remote desktop. NOTE: this image will be altered.</param>
		public unsafe virtual void Draw(Bitmap desktop)
		{
			// Lock the bitmap's scan-lines in RAM so we can iterate over them using pointers and update the area
			// defined in rectangle.
			var bmpd = desktop.LockBits(new Rectangle(new Point(0,0), desktop.Size), ImageLockMode.ReadWrite, desktop.PixelFormat);

			try {
				// For speed I'm using pointers to manipulate the desktop bitmap, which is unsafe.
				// Get a pointer to the start of the bitmap in memory (IntPtr) and cast to a 
				// Byte pointer (need void* first) so desktop can be traversed as GDI+ 
				// colour values form.
				var pInt = (int*)(void*)bmpd.Scan0; 

				// Move pointer to position in desktop bitmap where rectangle begins
				pInt += Rectangle.Y * desktop.Width + Rectangle.X;
				
				var offset = desktop.Width - Rectangle.Width;
				var row = 0;
				
				for (var y = 0; y < Rectangle.Height; ++y) {
					row = y * Rectangle.Width;

					for (var x = 0; x < Rectangle.Width; ++x) {
						*pInt++ = Framebuffer[row + x];
					}

					// Move pointer to beginning of next row in rectangle
					pInt += offset;
				}
			} finally {
				desktop.UnlockBits(bmpd);
				bmpd = null;
			}		
		}

		/// <summary>
		/// Fills the given Rectangle with a solid colour (i.e., all pixels will have the same value--colour).
		/// </summary>
		/// <param name="rect">The rectangle to be filled.</param>
		/// <param name="colour">The colour to use when filling the rectangle.</param>
		protected void FillRectangle(Rectangle rect, int colour)
		{
			var ptr = 0;
			var offset = 0;

			// If the two rectangles don't match, then rect is contained within rectangle, and
			// ptr and offset need to be adjusted to position things at the proper starting point.
			if (rect != Rectangle) {
				ptr = rect.Y * Rectangle.Width + rect.X;	// move to the start of the rectangle in pixels
				offset = Rectangle.Width - rect.Width;		// calculate the offset to get to the start of the next row
			}

			for (var y = 0; y < rect.Height; ++y) {
				for (var x = 0; x < rect.Width; ++x) {
					Framebuffer[ptr++] = colour;			// colour every pixel the same
				}
				ptr += offset;								// advance to next row within pixels
			}
		}

		protected void FillRectangle(Rectangle rect, int[] tile)
		{
			var ptr = 0;
			var offset = 0;

			// If the two rectangles don't match, then rect is contained within rectangle, and
			// ptr and offset need to be adjusted to position things at the proper starting point.
			if (rect != Rectangle) {
				ptr = rect.Y * Rectangle.Width + rect.X;	// move to the start of the rectangle in pixels
				offset = Rectangle.Width - rect.Width;		// calculate the offset to get to the start of the next row
			}

			var idx = 0;
			for (var y = 0; y < rect.Height; ++y) {
				for (var x = 0; x < rect.Width; ++x) {
					Framebuffer[ptr++] = tile[idx++];
				}
				ptr += offset;								// advance to next row within pixels
			}
		}
		
		/// <summary>
		/// Fills the given Rectangle with pixel values read from the server (i.e., each pixel may have its own value).
		/// </summary>
		/// <param name="rect">The rectangle to be filled.</param>
		protected void FillRectangle(Rectangle rect)
		{
			var ptr = 0;
			var offset = 0;

			// If the two rectangles don't match, then rect is contained within rectangle, and
			// ptr and offset need to be adjusted to position things at the proper starting point.
			if (rect != Rectangle) {
				ptr = rect.Y * Rectangle.Width + rect.X;	// move to the start of the rectangle in pixels
				offset = Rectangle.Width - rect.Width;		// calculate the offset to get to the start of the next row
			}

			for (var y = 0; y < rect.Height; ++y) {
				for (var x = 0; x < rect.Width; ++x) {
					Framebuffer[ptr++] = Preader.ReadPixel();	// every pixel needs to be read from server
				}
				ptr += offset;								    // advance to next row within pixels
			}
		}
	}
}
