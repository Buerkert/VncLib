// Copyright 2017 The VncLib Authors. All rights reserved.
// Use of this source code is governed by a BSD-style
// license that can be found in the LICENSE file.

using System;

namespace VncLib
{
    public class ScreenResolutionChangeEventArgs : EventArgs
    {
        private int _ResX;
        private int _ResY;

        public ScreenResolutionChangeEventArgs(int resX, int resY)
        {
            ResX = resX;
            ResY = resY;
        }

        public int ResX
        {
            get => _ResX;
            set => _ResX = value;
        }

        public int ResY
        {
            get => _ResY;
            set => _ResY = value;
        }
    }
}