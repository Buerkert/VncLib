// Copyright 2017 The VncLib Authors. All rights reserved.
// Use of this source code is governed by a BSD-style
// license that can be found in the LICENSE file.

using System.Diagnostics;
using System.Drawing;
using VncLib.Encodings;

namespace VncLib
{
    /// <summary>
    /// Factory class used to create derived EncodedRectangle objects at runtime based on data sent by VNC Server.
    /// </summary>
    public class EncodedRectangleFactory
    {
        private Framebuffer _framebuffer;
        private Rfb _rfb;

        /// <summary>
        /// Creates an instance of the EncodedRectangleFactory using the connected RfbProtocol object and associated Framebuffer object.
        /// </summary>
        /// <param name="rfb">An RfbProtocol object that will be passed to any created EncodedRectangle objects.  Must be non-null, already initialized, and connected.</param>
        /// <param name="framebuffer">A Framebuffer object which will be used by any created EncodedRectangle objects in order to decode and draw rectangles locally.</param>
        public EncodedRectangleFactory(Rfb rfb, Framebuffer framebuffer)
        {
            Debug.Assert(rfb != null, "RfbProtocol object must be non-null");
            Debug.Assert(framebuffer != null, "Framebuffer object must be non-null");

            _rfb = rfb;
            _framebuffer = framebuffer;
        }

        /// <summary>
        /// Creates an object type derived from EncodedRectangle, based on the value of encoding.
        /// </summary>
        /// <param name="rectangle">A Rectangle object defining the bounds of the rectangle to be created</param>
        /// <param name="encoding">An Integer indicating the encoding type to be used for this rectangle.  Used to determine the type of EncodedRectangle to create.</param>
        /// <returns></returns>
        public EncodedRectangle Build(Rectangle rectangle, int encoding)
        {
            EncodedRectangle e;

            switch (encoding)
            {
                case Rfb.RAW_ENCODING:
                    e = new RawRectangle(_rfb, _framebuffer, rectangle);
                    break;
                case Rfb.COPYRECT_ENCODING:
                    e = new CopyRectRectangle(_rfb, _framebuffer, rectangle);
                    break;
                case Rfb.RRE_ENCODING:
                    e = new RreRectangle(_rfb, _framebuffer, rectangle);
                    break;
                case Rfb.CORRE_ENCODING:
                    e = new CoRreRectangle(_rfb, _framebuffer, rectangle);
                    break;
                case Rfb.HEXTILE_ENCODING:
                    e = new HextileRectangle(_rfb, _framebuffer, rectangle);
                    break;
                case Rfb.ZRLE_ENCODING:
                    e = new ZrleRectangle(_rfb, _framebuffer, rectangle);
                    break;
                default:
                    // Sanity check
                    throw new VncProtocolException("Unsupported Encoding Format received: " + encoding + ".");
            }
            return e;
        }
    }
}