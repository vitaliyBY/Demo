/*
 * SDK Pullenti Address, version 4.20, august 2023. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Pullenti.Address.Internal
{
    public class NumberItem
    {
        public NumberItemClass Cla = NumberItemClass.Undefined;
        public string Value;
        public string Typ;
        public bool Slash;
        public bool CanBeFlat;
        public NumberItem Twix;
        public override string ToString()
        {
            return string.Format("{0} ({1}): {2}{3}{4}", Typ ?? "?", Cla, Value ?? "", (Slash ? " (after slash)" : ""), (CanBeFlat ? " (flat?)" : ""));
        }
        public double EqualCoef(NumberItem it)
        {
            if (Cla != it.Cla && Cla != NumberItemClass.Undefined) 
            {
                if (Cla == NumberItemClass.Flat && it.Cla == NumberItemClass.Space) 
                {
                }
                else if (Cla == NumberItemClass.Space && it.Cla == NumberItemClass.Flat) 
                {
                }
                else 
                    return 0;
            }
            double res = (double)0;
            if (Value != it.Value) 
            {
                if (Value == "1" && it.Value == "А") 
                    res += 0.1;
                else if (Value == "А" && it.Value == "1") 
                    res += 0.1;
                else 
                    return 0;
            }
            else 
                res += 1;
            if (Typ != it.Typ || Typ == null) 
            {
                res /= 2;
                if (Cla == NumberItemClass.Undefined) 
                {
                    if (it.Cla == NumberItemClass.Plot) 
                        res *= 0.3;
                    else if (it.Cla != NumberItemClass.House) 
                        return 0;
                }
            }
            return res;
        }
        public static List<NumberItem> Parse(string val, string typ, NumberItemClass cla)
        {
            if (string.IsNullOrEmpty(val)) 
                val = "0";
            if (string.Compare(val, "Б/Н", true) == 0) 
                val = "0";
            List<NumberItem> res = new List<NumberItem>();
            for (int i = 0; i < val.Length; i++) 
            {
                char ch = val[i];
                if (!char.IsLetterOrDigit(ch)) 
                    continue;
                bool dig = char.IsDigit(ch);
                int j;
                for (j = i + 1; j < val.Length; j++) 
                {
                    if (dig != char.IsDigit(val[j])) 
                        break;
                }
                NumberItem num = new NumberItem();
                if (i == 0 && j == val.Length) 
                    num.Value = val;
                else 
                {
                    num.Value = val.Substring(i, j - i);
                    while (num.Value.Length > 1 && num.Value[0] == '0') 
                    {
                        num.Value = num.Value.Substring(1);
                    }
                }
                if (!dig) 
                {
                    num.Value = num.Value.ToUpper();
                    if (((int)num.Value[0]) < 0x80) 
                    {
                        StringBuilder tmp = new StringBuilder();
                        tmp.Append(num.Value);
                        for (int k = 0; k < tmp.Length; k++) 
                        {
                            ch = Pullenti.Morph.LanguageHelper.GetCyrForLat(tmp[k]);
                            if (ch != ((char)0)) 
                                tmp[k] = ch;
                        }
                        num.Value = tmp.ToString();
                    }
                }
                if (i > 0 && ((val[i - 1] == '/' || val[i - 1] == '\\'))) 
                    num.Slash = true;
                res.Add(num);
                i = j - 1;
            }
            if (typ != null && res.Count > 0 && res[0].Typ == null) 
                res[0].Typ = typ;
            foreach (NumberItem r in res) 
            {
                r.Cla = cla;
            }
            return res;
        }
    }
}