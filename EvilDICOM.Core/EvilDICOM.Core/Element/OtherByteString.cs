﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EvilDICOM.Core.Interfaces;
using EvilDICOM.Core.IO.Data;

namespace EvilDICOM.Core.Element
{
    /// <summary>
    /// Encapsulates the OtherByteString VR type
    /// </summary>
    public class OtherByteString : AbstractElement<byte[]>
    {
        public OtherByteString() { }

        public OtherByteString(Tag tag, byte[] data)
        {
            Tag = tag;
            Data = data;
            VR = Enums.VR.OtherByteString;
        }
    }
}