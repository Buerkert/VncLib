// Copyright 2017 The VncLib Authors. All rights reserved.
// Use of this source code is governed by a BSD-style
// license that can be found in the LICENSE file.

using System;

namespace VncLib
{
    public class RfbEncodingDetails
    {
        private Int32 _Id;
        private string _Name = "";
        private UInt16 _Priority;

        public RfbEncodingDetails(Int32 id, string name, UInt16 prio)
        {
            Id = id;
            Name = name;
            Priority = prio;
        }

        public Int32 Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        public UInt16 Priority
        {
            get { return _Priority; }
            set { _Priority = value; }
        }
    }
}