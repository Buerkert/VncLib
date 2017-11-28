// Copyright 2017 The VncLib Authors. All rights reserved.
// Use of this source code is governed by a BSD-style
// license that can be found in the LICENSE file.

using System;

namespace VncLib.VncCommands
{
    public interface IVncDragCommand : IVncCommand
    {
        UInt16 X2 { get; set; }
        UInt16 Y2 { get; set; }
    }
}