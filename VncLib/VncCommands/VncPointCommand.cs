using System;
using System.Threading.Tasks;
// Copyright 2017 The VncLib Authors. All rights reserved.
// Use of this source code is governed by a BSD-style
// license that can be found in the LICENSE file.

namespace VncLib.VncCommands
{
    public class VncPointCommand : VncCommand
    {
        public VncPointCommand()
        {
            Action = Command.Click;
        }

        public override Task Execute(IRfbClient vncClient)
        {
            if (Action == Command.Move)
            {

                return Task.Run(() => vncClient.SendMouseClick(X, Y, 0));
            }
            else if (Action == Command.Click)
            {
                return Task.Run(() =>
                {
                    vncClient.SendMouseClick(X, Y, 0);
                    vncClient.SendMouseClick(X, Y, 1);
                    vncClient.SendMouseClick(X, Y, 0);
                });
            }
            else
            {
                throw new Exception("wrong execute called!");
            }
        }

        public override string LuaTable => $"{{Action=\"{ActionString}\",x={X},y={Y}}}";
    }
}