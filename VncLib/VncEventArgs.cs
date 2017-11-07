// Copyright 2017 The VncLib Authors. All rights reserved.
// Use of this source code is governed by a BSD-style
// license that can be found in the LICENSE file.

using System;

namespace VncLib
{
	public class VncEventArgs : EventArgs
	{
	    public VncEventArgs(IDesktopUpdater updater)
		{
			DesktopUpdater = updater;
		}

		public IDesktopUpdater DesktopUpdater { get; }
	}
}