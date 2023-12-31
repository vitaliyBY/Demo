﻿/*
 * SDK Pullenti Address, version 4.20, august 2023. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Ner.Measure.Internal
{
    class MeasureHelper
    {
        static MeasureHelper()
        {
        }
        public static bool TryParseDouble(string val, out double f)
        {
            f = 0;
            if (string.IsNullOrEmpty(val)) 
                return false;
            if (val.IndexOf(',') >= 0 && double.TryParse(val.Replace(',', '.'), out f)) 
                return true;
            if (double.TryParse(val, out f)) 
                return true;
            return false;
        }
        public static bool IsMultChar(Pullenti.Ner.Token t)
        {
            Pullenti.Ner.TextToken tt = t as Pullenti.Ner.TextToken;
            if (tt == null) 
                return false;
            if (tt.LengthChar == 1) 
            {
                if (tt.IsCharOf("*xXхХ·×◦∙•") || tt.IsChar((char)0x387) || tt.IsChar((char)0x22C5)) 
                    return true;
            }
            return false;
        }
        public static bool IsMultCharEnd(Pullenti.Ner.Token t)
        {
            Pullenti.Ner.TextToken tt = t as Pullenti.Ner.TextToken;
            if (tt == null) 
                return false;
            string term = tt.Term;
            if (term.EndsWith("X") || term.EndsWith("Х")) 
                return true;
            return false;
        }
    }
}