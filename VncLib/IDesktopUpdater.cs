// Copyright 2017 The VncLib Authors. All rights reserved.
// Use of this source code is governed by a BSD-style
// license that can be found in the LICENSE file.

using System.Drawing;

namespace VncLib
{
	/// <summary>
	/// Classes that implement IDesktopUpdater are used to update and Draw on a local Bitmap representation of the remote desktop.
	/// </summary>
	public interface IDesktopUpdater
	{
		/// <summary>
		/// Given a desktop Bitmap that is a local representation of the remote desktop, updates sent by the server are drawn into the area specifed by UpdateRectangle.
		/// </summary>
		/// <param name="desktop">The desktop Bitmap on which updates should be drawn.</param>
		void Draw(Bitmap desktop);
		
		/// <summary>
		/// The region of the desktop Bitmap that needs to be re-drawn.
		/// </summary>
		Rectangle UpdateRectangle {
			get;
		}
	}
}