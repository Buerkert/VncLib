// Copyright 2017 The VncLib Authors. All rights reserved.
// Use of this source code is governed by a BSD-style
// license that can be found in the LICENSE file.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace VncLib.VncCommands
{
    public class LuaTableVncCommandConverter
    {
        public virtual IVncCommand GetCommandFromLuaTable(Dictionary<object, object> luaTable)
        {
            if (!luaTable.ContainsKey("Action"))
                throw new Exception("invalid table data! no Action found!");

            Command cmd;
            var value = (string) luaTable["Action"];
            switch (value)
            {
                case "Pause":
                    cmd = Command.Pause;
                    break;
                case "Move":
                    cmd = Command.Move;
                    break;
                case "Click":
                    cmd = Command.Click;
                    break;
                case "Drag":
                    cmd = Command.Drag;
                    break;
                case "Home":
                    cmd = Command.Home;
                    break;
                default:
                    throw new Exception("invalid Action!");
            }
                

            VncPointCommand dtc = null;
            VncDragCommand ddc = null;
            switch (cmd)
            {
                case Command.Home:
                    return new VncCommand(){Action = Command.Home};
                case Command.Pause:
                    return new VncCommand(){Action = Command.Pause};
                case Command.Move:
                    dtc = new VncPointCommand(){Action = Command.Move};
                    break;
                case Command.Click:
                    dtc = new VncPointCommand(){Action=Command.Click};
                    break;
                case Command.Drag:
                    ddc = new VncDragCommand(){Action = Command.Drag};
                    break;
                default:
                    throw new NotImplementedException();
            }

            if (dtc != null)
            {
                if (!luaTable.ContainsKey("x") || !luaTable.ContainsKey("y"))
                    throw new Exception("invalid table data!");
                dtc.X = Convert.ToUInt16(luaTable["x"]);
                dtc.Y = Convert.ToUInt16(luaTable["y"]);
                return dtc;
            }
            else
            {
                if (!luaTable.ContainsKey("x1") || !luaTable.ContainsKey("y1") || !luaTable.ContainsKey("x2") || !luaTable.ContainsKey("y2"))
                    throw new Exception("invalid table data!");
                ddc.X = Convert.ToUInt16(luaTable["x1"]);
                ddc.Y = Convert.ToUInt16(luaTable["y1"]);
                ddc.X2 = Convert.ToUInt16(luaTable["x2"]);
                ddc.Y2 = Convert.ToUInt16(luaTable["y2"]);
                return ddc;
            }
        }

        public virtual IVncCommand GetCommandFromLuaTable(string table)
        {
            Dictionary<object, object> ltable = new Dictionary<object, object>();

            table = table.TrimStart('{');
            table = table.TrimEnd('}');
            string[] entries = table.Split(',');
            foreach (var entry in entries)
            {
                string[] tokens = entry.Split('=');

                // check validity
                if (tokens.Length != 2) // there has to be exactly one = sign
                {
                    throw new Exception("invalid table data!");
                }
                ltable[tokens[0]] = tokens[1].TrimStart('"').TrimEnd('"');
            }

            return GetCommandFromLuaTable(ltable);
        }

        public virtual string ConvertToVncCommandTable(IEnumerable<IVncCommand> commands)
        {
            string table = "";
            int i = 1;
            foreach (IVncCommand command in commands)
            {
                table += "[" + (i++) + "]=" + command.LuaTable + ",";
            }
            table = table.TrimEnd(',');
            return "{" + table + "}";
        }

        public virtual List<IVncCommand> ConvertToVncCommands(string table)
        {
            table = table.Trim();
            if (table.StartsWith("{") && table.EndsWith("}"))
            {
                List<IVncCommand> list = new List<IVncCommand>();
                table = table.Substring(1, table.Length - 2);
                MatchCollection matches = Regex.Matches(table, @"\{.*?\}");
                foreach (Match match in matches)
                {
                    list.Add(GetCommandFromLuaTable(match.Groups[0].Value));
                }
                return list;
            }
            else
            {
                throw new Exception("'" + table + "' is not valid Command Table!");
            }
        }
    }
}