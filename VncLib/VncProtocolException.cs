// Copyright 2017 The VncLib Authors. All rights reserved.
// Use of this source code is governed by a BSD-style
// license that can be found in the LICENSE file.

using System;
using System.Runtime.Serialization;

namespace VncLib
{
	public class VncProtocolException : ApplicationException
	{
		public VncProtocolException()
		{
		}

		public VncProtocolException(string message) : base(message)
		{
		}
		
		public VncProtocolException(string message, Exception inner) : base(message, inner)
		{
		}
		
		public VncProtocolException(SerializationInfo info, StreamingContext cxt) : base(info, cxt)
		{
		}
	}
}