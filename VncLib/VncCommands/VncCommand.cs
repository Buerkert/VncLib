// Copyright 2017 The VncLib Authors. All rights reserved.
// Use of this source code is governed by a BSD-style
// license that can be found in the LICENSE file.

using System;
using System.Threading.Tasks;

namespace VncLib.VncCommands
{
    public class VncCommand : IVncCommand
    {
        public VncCommand()
        {
        }

        public Command Action { get; set; }
        public virtual async Task Execute(IRfbClient vncClient)
        {
            if (Action == Command.Pause)
            {
                await Task.Delay(100);
            }
            else if (Action == Command.Home)
            {
                vncClient.SendMouseClick(0, 0, 4);
            }
        }

        public ushort X { get; set; }
        public ushort Y { get; set; }

        public virtual string LuaTable => $"{{Action=\"{ActionString}\"}}";

        public virtual string ActionString
        {
            get
            {
                switch (Action)
                {
                    case Command.Pause:
                        return "Pause";
                    case Command.Move:
                        return "Move";
                    case Command.Click:
                        return "Click";
                    case Command.Drag:
                        return "Drag";
                    case Command.Home:
                        return "Home";
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}