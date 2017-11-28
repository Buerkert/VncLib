// Copyright 2017 The VncLib Authors. All rights reserved.
// Use of this source code is governed by a BSD-style
// license that can be found in the LICENSE file.

namespace VncLib
{
    public enum RfbEncoding
    {
        ZRLE_ENCODING, //0
        Hextile_ENCODING,  //1
        RRE_ENCODING, //2
        CopyRect_ENCODING, //5
        Raw_ENCODING, //16

        //Inofficials
        CoRRE_ENCODING, //4
        zlib_ENCODING, //6
        tight_ENCODING, //7
        zlibhex_ENCODING, //8
        TRLE_ENCODING, //15
        Hitachi_ZYWRLE_ENCODING, //17
        Adam_Walling_XZ_ENCODING, //18
        Adam_Walling_XZYW_ENCODING, //19
        tight_options_ENCODING, //-240 - -256
        Anthony_Liguori_ENCODING, //-257 - -272
        VMWare_ENCODING, //-273 - -304 + 0x574d5600 - 0x574d56ff
        gii_ENCODING, //-305
        popa_ENCODING, //-306
        Peter_Astrand_DesktopName_ENCODING, //-307
        Pierre_Ossman_ExtendedDesktopSize_ENCODING, //-308
        Colin_dean_xvp_ENCODING, //-309
        OLIVE_Call_Control_ENCODING, //-310
        CursorWithAlpha_ENCODING, //-311
        TurboVNC_fine_grained_quality_level_ENCODING, //-412 - -512
        TurboVNC_subsampling_level_ENCODING, //-763 - -768

        //Pseudo Encodings
        Pseudo_Cursor_ENCODING, //-239
        Pseudo_DesktopSize_ENCODING //-223
    }
}