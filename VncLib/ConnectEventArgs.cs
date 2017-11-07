// Copyright 2017 The VncLib Authors. All rights reserved.
// Use of this source code is governed by a BSD-style
// license that can be found in the LICENSE file.

using System;

namespace VncLib
{
    /// <summary>
    /// Used in connection with the ConnectComplete event. Contains information about the remote desktop useful for setting-up the client's GUI.
    /// </summary>
    public class ConnectEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor for ConnectEventArgs
        /// </summary>
        /// <param name="width">An Integer indicating the Width of the remote framebuffer.</param>
        /// <param name="height">An Integer indicating the Height of the remote framebuffer.</param>
        /// <param name="name">A String containing the name of the remote Desktop.</param>
        public ConnectEventArgs(int width, int height, string name)
        {
            DesktopWidth = width;
            DesktopHeight = height;
            DesktopName = name;
        }

        /// <summary>
        /// Gets the Width of the remote Desktop.
        /// </summary>
        public int DesktopWidth { get; }

        /// <summary>
        /// Gets the Height of the remote Desktop.
        /// </summary>
        public int DesktopHeight { get; }

        /// <summary>
        /// Gets the name of the remote Desktop, if any.
        /// </summary>
        public string DesktopName { get; }
    }
}