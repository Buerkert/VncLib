// Copyright 2017 The VncLib Authors. All rights reserved.
// Use of this source code is governed by a BSD-style
// license that can be found in the LICENSE file.

namespace VncLib
{
    /// <summary>
    /// Raw Encoding (see 6.5.1)
    /// </summary>
    public static class RawRectangle
    {
        //public static byte[, ,] EncodeRawRectangle(int frameHeight, int frameWidth, byte[] frameData)
        //{
        //	 byte[,,] retValue = new byte[frameWidth,frameHeight,4];

        //	 for (int h = 0; h < frameHeight; h++)
        //	 {
        //		  for (int w = 0; w < frameWidth; w++)
        //		  {
        //				//Update Pixel for Backbuffer (Read every Frame)
        //				retValue[w, h, 2] = frameData[h * frameWidth * 4 + w * 4]; //blue
        //				retValue[w, h, 1] = frameData[h * frameWidth * 4 + w * 4 + 1]; //green
        //				retValue[w, h, 0] = frameData[h * frameWidth * 4 + w * 4 + 2]; //red
        //				retValue[w, h, 3] = frameData[h * frameWidth * 4 + w * 4 + 3]; //alpha

        //		  }
        //	 }

        //	 return (retValue);
        //}
    }
}