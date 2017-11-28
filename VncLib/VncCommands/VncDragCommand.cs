// Copyright 2017 The VncLib Authors. All rights reserved.
// Use of this source code is governed by a BSD-style
// license that can be found in the LICENSE file.

using System;
using System.Threading.Tasks;

namespace VncLib.VncCommands
{
    public class VncDragCommand : VncCommand
    {
        public VncDragCommand()
        {
            Action = Command.Drag;
        }


        public ushort X2 { get; set; }
        public ushort Y2 { get; set; }

        public override Task Execute(IRfbClient vncClient)
        {
            if (Action == Command.Drag)
            {
                return Task.Run(async () =>
                {
                    vncClient.SendMouseClick(X, Y, 0);
                    vncClient.SendMouseClick(X, Y, 1);

                    int dx = (X2 - X) / 10;
                    int dy = (Y2 - Y) / 10;
                    Console.WriteLine("before x:" + X + " y:" + Y);
                    for (int i = 1; i < 10; i++)
                    {
                        await Task.Delay(100);
                        ushort dPx = (ushort)(X + (dx * i));
                        ushort dPy = (ushort)(Y + (dy * i));
                        Console.WriteLine("in x:" + dPx + " y:" + dPy);
                        vncClient.SendMouseClick(Convert.ToUInt16(dPx), dPy, 1);
                    }
                    await Task.Delay(100);
                    vncClient.SendMouseClick(X2, Y2, 1); // stop dragging
                    vncClient.SendMouseClick(X2, Y2, 1); // stop dragging

                    Console.WriteLine("after x:" + X2 + " y:" + Y2);
                });
            }
            else
            {
                throw new Exception("wrong execute called!");
            }
        }

        public override string LuaTable => $"{{Action=\"{ActionString}\",x1={X},y1={Y},x2={X2},y2={Y2}}}";
    }
}