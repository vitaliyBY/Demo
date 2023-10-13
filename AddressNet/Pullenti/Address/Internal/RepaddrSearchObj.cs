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
    class RepaddrSearchObj
    {
        public List<string> SearchStrs = new List<string>();
        public List<short> TypeIds = new List<short>();
        public Pullenti.Address.AddrLevel Lev;
        public Pullenti.Address.AddrObject Src;
        const int ERROR = 1000;
        public RepaddrSearchObj(Pullenti.Address.AddrObject a, RepTypTable typs)
        {
            Src = a;
            Pullenti.Address.AreaAttributes aa = a.Attrs as Pullenti.Address.AreaAttributes;
            if (aa == null) 
                return;
            Lev = a.Level;
            foreach (string ty in aa.Types) 
            {
                TypeIds.Add((short)typs.GetId(ty));
            }
            if (a.Gars.Count > 0) 
            {
                foreach (Pullenti.Address.GarObject g in a.Gars) 
                {
                    if (g != null) 
                        SearchStrs.Add(g.Guid.Replace("-", ""));
                }
            }
            int i0 = SearchStrs.Count;
            if (aa.Names != null) 
            {
                foreach (string v in aa.Names) 
                {
                    SearchStrs.Add(this._corrString(v.ToUpper()));
                }
            }
            if (aa.Number != null) 
            {
                if (i0 == SearchStrs.Count) 
                    SearchStrs.Add(aa.Number);
                else 
                    for (int i = i0; i < SearchStrs.Count; i++) 
                    {
                        SearchStrs[i] = string.Format("{0} {1}", SearchStrs[i], aa.Number);
                    }
            }
        }
        string _corrString(string str)
        {
            bool needCorr = false;
            foreach (char ch in str) 
            {
                if (ch == 'Ь' || ch == 'Ъ') 
                    needCorr = true;
            }
            for (int i = 0; i < (str.Length - 1); i++) 
            {
                if (str[i] == str[i + 1]) 
                    needCorr = true;
            }
            if (!needCorr) 
                return str;
            StringBuilder res = new StringBuilder();
            for (int i = 0; i < str.Length; i++) 
            {
                char ch = str[i];
                if (ch == 'Ь' || ch == 'Ъ') 
                    continue;
                if (res.Length > 0 && res[res.Length - 1] == ch) 
                    continue;
                res.Append(ch);
            }
            return res.ToString();
        }
        public int CalcCoef(RepAddrTreeNodeObj o, Pullenti.Address.RepaddrObject parent, Pullenti.Address.RepaddrObject parent2)
        {
            if (o.Lev != Lev) 
                return ERROR;
            int ret = 0;
            bool eqTyps = false;
            foreach (short id in TypeIds) 
            {
                if (o.TypIds.Contains(id)) 
                {
                    eqTyps = true;
                    break;
                }
            }
            if (!eqTyps) 
            {
                if (Lev == Pullenti.Address.AddrLevel.Territory || Lev == Pullenti.Address.AddrLevel.Locality) 
                {
                    if ((Src.Attrs as Pullenti.Address.AreaAttributes).Names.Count == 0) 
                        return ERROR;
                    ret += 10;
                }
                else 
                    return ERROR;
            }
            if (parent == null) 
            {
                if (o.Parents == null || o.Parents.Count == 0) 
                {
                }
                else 
                    return ERROR;
            }
            else 
            {
                if (o.Parents == null || o.Parents.Count == 0) 
                    return ERROR;
                int i = o.Parents.IndexOf(parent.Id);
                if (i < 0) 
                {
                    if (parent2 != null && Pullenti.Address.AddressHelper.CanBeParent(Lev, parent2.Level)) 
                    {
                        i = o.Parents.IndexOf(parent2.Id);
                        if (i >= 0) 
                            ret += 10;
                        else 
                            return ERROR;
                    }
                    else 
                        return ERROR;
                }
            }
            return ret;
        }
        public static List<string> GetSearchStrings(Pullenti.Address.AddrObject o)
        {
            List<string> res = new List<string>();
            StringBuilder tmp = new StringBuilder();
            Pullenti.Address.HouseAttributes h = o.Attrs as Pullenti.Address.HouseAttributes;
            Pullenti.Address.RoomAttributes r = o.Attrs as Pullenti.Address.RoomAttributes;
            if (h != null) 
            {
                if (h.Number != null || h.Typ != Pullenti.Address.HouseType.Undefined) 
                {
                    if (h.Typ == Pullenti.Address.HouseType.Plot) 
                        tmp.Append('p');
                    else if (h.Typ == Pullenti.Address.HouseType.Garage) 
                        tmp.Append('g');
                    tmp.Append((string.IsNullOrEmpty(h.Number) ? "0" : h.Number));
                }
                if (h.BuildNumber != null) 
                    tmp.AppendFormat("b{0}", h.BuildNumber);
                if (h.StroenNumber != null) 
                    tmp.AppendFormat("s{0}", h.StroenNumber);
            }
            else if (r != null) 
            {
                if (r.Number != null || r.Typ != Pullenti.Address.RoomType.Undefined) 
                {
                    if (r.Typ == Pullenti.Address.RoomType.Flat) 
                        tmp.Append('f');
                    else if (r.Typ == Pullenti.Address.RoomType.Carplace) 
                        tmp.Append('c');
                    tmp.Append((string.IsNullOrEmpty(r.Number) ? "0" : r.Number));
                }
            }
            if (tmp.Length == 0) 
                tmp.Append(o.ToString());
            res.Add(tmp.ToString());
            for (int i = 0; i < tmp.Length; i++) 
            {
                if (char.IsLetter(tmp[i]) && char.IsUpper(tmp[i])) 
                {
                    char ch = tmp[i];
                    char ch1 = Pullenti.Morph.LanguageHelper.GetCyrForLat(ch);
                    if (ch1 != 0) 
                    {
                        tmp[i] = ch1;
                        res.Add(tmp.ToString());
                        tmp[i] = ch;
                    }
                    if (h != null) 
                    {
                        if ((h.StroenNumber == null && h.BuildNumber == null && i > 0) && char.IsDigit(tmp[i - 1])) 
                        {
                            tmp.Insert(i, "s");
                            res.Add(tmp.ToString());
                            tmp.Remove(i, 1);
                        }
                    }
                }
            }
            foreach (Pullenti.Address.GarObject g in o.Gars) 
            {
                res.Add(g.Guid.Replace("-", ""));
            }
            return res;
        }
    }
}