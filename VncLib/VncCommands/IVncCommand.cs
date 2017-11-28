// Copyright 2017 The VncLib Authors. All rights reserved.
// Use of this source code is governed by a BSD-style
// license that can be found in the LICENSE file.

using System;
using System.Threading.Tasks;

namespace VncLib.VncCommands
{
    public interface IVncCommand
    {
        Task Execute(IRfbClient vncClient);

        UInt16 X { get; set; }
        UInt16 Y { get; set; }

        string LuaTable { get; }
        string ActionString { get; }

        Command Action { get; set; }
    }
}