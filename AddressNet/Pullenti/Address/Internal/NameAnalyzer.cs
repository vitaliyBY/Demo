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
    public class NameAnalyzer
    {
        public Pullenti.Address.GarStatus Status = Pullenti.Address.GarStatus.Error;
        public Pullenti.Ner.Referent Ref;
        public List<string> Types;
        public List<string> Strings;
        public List<string> StringsEx;
        public List<string> DoubtStrings;
        public List<string> Miscs;
        public Pullenti.Address.AddrLevel Level;
        public NameAnalyzer Sec;
        public override string ToString()
        {
            StringBuilder tmp = new StringBuilder();
            this.OutInfo(tmp);
            return tmp.ToString();
        }
        public void InitByReferent(Pullenti.Ner.Referent r, bool garRegime)
        {
            Ref = r;
            Strings = new List<string>();
            DoubtStrings = new List<string>();
            if (garRegime) 
                StringsEx = new List<string>();
            else 
                StringsEx = null;
            if ((Ref is Pullenti.Ner.Geo.GeoReferent) && string.Compare(Ref.ToString(), "ДНР", true) == 0) 
            {
                Ref = new Pullenti.Ner.Geo.GeoReferent();
                Ref.AddSlot(Pullenti.Ner.Geo.GeoReferent.ATTR_TYPE, "республика", false, 0);
                Ref.AddSlot(Pullenti.Ner.Geo.GeoReferent.ATTR_NAME, "ДОНЕЦКАЯ", false, 0);
                Level = Pullenti.Address.AddrLevel.RegionArea;
            }
            else if ((Ref is Pullenti.Ner.Geo.GeoReferent) && string.Compare(Ref.ToString(), "ЛНР", true) == 0) 
            {
                Ref = new Pullenti.Ner.Geo.GeoReferent();
                Ref.AddSlot(Pullenti.Ner.Geo.GeoReferent.ATTR_TYPE, "республика", false, 0);
                Ref.AddSlot(Pullenti.Ner.Geo.GeoReferent.ATTR_NAME, "ЛУГАНСКАЯ", false, 0);
                Level = Pullenti.Address.AddrLevel.RegionArea;
            }
            GetStrings(Ref, Strings, DoubtStrings, StringsEx);
            Types = Ref.GetStringValues((Ref is Pullenti.Ner.Address.StreetReferent ? Pullenti.Ner.Address.StreetReferent.ATTR_TYPE : Pullenti.Ner.Geo.GeoReferent.ATTR_TYPE));
            if (Ref is Pullenti.Ner.Address.StreetReferent) 
            {
                string num = (Ref as Pullenti.Ner.Address.StreetReferent).Numbers;
                if (num != null && num.EndsWith("км")) 
                {
                    if ((Ref as Pullenti.Ner.Address.StreetReferent).Names.Count > 0) 
                    {
                        Sec = new NameAnalyzer();
                        Pullenti.Ner.Address.StreetReferent s1 = new Pullenti.Ner.Address.StreetReferent();
                        s1.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NUMBER, num, false, 0);
                        Sec.InitByReferent(s1, garRegime);
                        Level = Pullenti.Address.AddrLevel.Territory;
                        r.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NUMBER, null, true, 0);
                        Strings.Clear();
                        DoubtStrings.Clear();
                        if (StringsEx != null) 
                            StringsEx.Clear();
                        GetStrings(r, Strings, DoubtStrings, StringsEx);
                        r.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NUMBER, num, true, 0);
                    }
                    else 
                        Types.Add("километр");
                }
            }
            if (Level == Pullenti.Address.AddrLevel.Undefined) 
                Level = CalcLevel(Ref);
            Miscs = Ref.GetStringValues("MISC");
            if (Miscs.Count > 0) 
            {
                List<string> addMisc = new List<string>();
                foreach (string m in Miscs) 
                {
                    string s = null;
                    if (m.Contains("гараж")) 
                        s = "гаражи";
                    else if (m.Contains("садов") || m.Contains("дачн")) 
                        s = "дачи";
                    else if (m.Contains("жилищ")) 
                        s = "жилье";
                    else if (m.Contains("месторожде")) 
                        s = "месторождение";
                    if (s != null && !addMisc.Contains(s)) 
                        addMisc.Add(s);
                }
                bool hasUp = false;
                foreach (string m in Miscs) 
                {
                    if (char.IsUpper(m[0])) 
                        hasUp = true;
                }
                if (hasUp) 
                {
                    for (int i = Miscs.Count - 1; i >= 0; i--) 
                    {
                        if (!char.IsUpper(Miscs[i][0]) && Miscs[i].IndexOf(' ') > 0) 
                            Miscs.RemoveAt(i);
                    }
                }
                if (addMisc.Count > 0) 
                {
                    foreach (string m in addMisc) 
                    {
                        if (!Miscs.Contains(m)) 
                            Miscs.Add(m);
                    }
                }
                if (Ref is Pullenti.Ner.Address.StreetReferent) 
                {
                    if ((Ref as Pullenti.Ner.Address.StreetReferent).Kind == Pullenti.Ner.Address.StreetKind.Road) 
                        Miscs.Add("дорога");
                }
            }
            Status = Pullenti.Address.GarStatus.Ok;
        }
        public static Pullenti.Ner.Referent MergeObjects(Pullenti.Ner.Referent hi, Pullenti.Ner.Referent lo)
        {
            return null;
        }
        public NameAnalyzer TryCreateAlternative(bool sec, Pullenti.Address.AddrObject prev, Pullenti.Address.AddrObject next)
        {
            Pullenti.Ner.Address.StreetReferent street = Ref as Pullenti.Ner.Address.StreetReferent;
            if (street != null) 
            {
                string name = street.GetStringValue(Pullenti.Ner.Address.StreetReferent.ATTR_NAME);
                List<string> typs = street.Typs;
                if (!sec && street.Numbers != null && name == "МИКРОРАЙОН") 
                {
                    Pullenti.Ner.Address.StreetReferent sr = new Pullenti.Ner.Address.StreetReferent();
                    sr.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_TYPE, name.ToLower(), false, 0);
                    sr.Numbers = street.Numbers;
                    sr.Kind = Pullenti.Ner.Address.StreetKind.Area;
                    NameAnalyzer alt = new NameAnalyzer();
                    alt.InitByReferent(sr, false);
                    return alt;
                }
                else if ((name == null && !sec && street.Numbers != null) && ((typs.Contains("микрорайон") || typs.Contains("набережная")))) 
                {
                    Pullenti.Ner.Address.StreetReferent sr = new Pullenti.Ner.Address.StreetReferent();
                    sr.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NAME, (typs.Contains("микрорайон") ? "МИКРОРАЙОН" : "НАБЕРЕЖНАЯ"), false, 0);
                    sr.Numbers = street.Numbers;
                    NameAnalyzer alt = new NameAnalyzer();
                    alt.InitByReferent(sr, false);
                    return alt;
                }
            }
            Pullenti.Ner.Geo.GeoReferent geo = Ref as Pullenti.Ner.Geo.GeoReferent;
            if (geo != null) 
            {
                List<string> typs = geo.Typs;
                if (typs.Count == 1 && ((typs[0] == "район" || typs[0] == "муниципальный район" || typs[0] == "населенный пункт"))) 
                {
                    string num = geo.GetStringValue("NUMBER");
                    if (!sec) 
                    {
                        Pullenti.Ner.Geo.GeoReferent geo2 = new Pullenti.Ner.Geo.GeoReferent();
                        geo2.AddSlot(Pullenti.Ner.Geo.GeoReferent.ATTR_TYPE, "населенный пункт", false, 0);
                        foreach (string s in geo.GetStringValues(Pullenti.Ner.Geo.GeoReferent.ATTR_NAME)) 
                        {
                            geo2.AddSlot(Pullenti.Ner.Geo.GeoReferent.ATTR_NAME, s, false, 0);
                        }
                        if (num != null) 
                            geo2.AddSlot("NUMBER", num, false, 0);
                        NameAnalyzer alt = new NameAnalyzer();
                        alt.InitByReferent(geo2, false);
                        return alt;
                    }
                    else 
                    {
                        if (prev != null && prev.Level == Pullenti.Address.AddrLevel.RegionArea) 
                            return null;
                        if (next != null && next.Level == Pullenti.Address.AddrLevel.Street) 
                            return null;
                        Pullenti.Ner.Address.StreetReferent sr = new Pullenti.Ner.Address.StreetReferent();
                        foreach (string s in geo.GetStringValues(Pullenti.Ner.Geo.GeoReferent.ATTR_NAME)) 
                        {
                            sr.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NAME, s, false, 0);
                        }
                        if (num != null) 
                            sr.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NUMBER, num, false, 0);
                        NameAnalyzer alt = new NameAnalyzer();
                        alt.InitByReferent(sr, false);
                        return alt;
                    }
                }
                if (geo.IsCity && !typs.Contains("город")) 
                {
                    if (prev != null && ((prev.Level == Pullenti.Address.AddrLevel.City || prev.Level == Pullenti.Address.AddrLevel.RegionCity || prev.Level == Pullenti.Address.AddrLevel.Locality))) 
                    {
                        if (next != null && ((next.Level == Pullenti.Address.AddrLevel.Street || next.Level == Pullenti.Address.AddrLevel.Territory))) 
                            return null;
                        Pullenti.Ner.Address.StreetReferent sr = new Pullenti.Ner.Address.StreetReferent();
                        foreach (string s in geo.GetStringValues(Pullenti.Ner.Geo.GeoReferent.ATTR_NAME)) 
                        {
                            sr.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NAME, s, false, 0);
                        }
                        foreach (string ty in typs) 
                        {
                            sr.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_MISC, ty, false, 0);
                        }
                        string num = geo.GetStringValue("NUMBER");
                        if (num != null) 
                            sr.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NUMBER, num, false, 0);
                        NameAnalyzer alt = new NameAnalyzer();
                        alt.InitByReferent(sr, false);
                        return alt;
                    }
                }
            }
            return null;
        }
        public void ProcessEx(Pullenti.Address.GarObject go)
        {
            Pullenti.Address.AreaAttributes aa = go.Attrs as Pullenti.Address.AreaAttributes;
            this.Process(aa.Names, aa.Types[0]);
        }
        public void Process(List<string> names, string typ)
        {
            Strings = null;
            Status = Pullenti.Address.GarStatus.Error;
            Ref = null;
            if (typ == "чувашия") 
            {
                typ = "республика";
                names[0] = "Чувашская Республика";
            }
            int bestCoef = 10000;
            Pullenti.Ner.Referent bestRef = null;
            Pullenti.Ner.Referent bestSecRef = null;
            Pullenti.Ner.Referent bestRef2 = null;
            for (int nn = 0; nn < names.Count; nn++) 
            {
                string name = CorrectFiasName(names[nn]);
                for (int jj = nn + 1; jj < names.Count; jj++) 
                {
                    if (names[jj] == name) 
                    {
                        names.RemoveAt(jj);
                        jj--;
                    }
                }
                if (name.Contains("Капотня")) 
                {
                }
                if (name == "ЖСТ Чаевод квартал Питомник-2") 
                {
                }
                if (name.IndexOf('/') > 0) 
                {
                    Pullenti.Ner.AnalysisResult ar;
                    try 
                    {
                        ar = Pullenti.Ner.ProcessorService.EmptyProcessor.Process(new Pullenti.Ner.SourceOfAnalysis(name) { UserParams = "GARADDRESS" }, null, null);
                    }
                    catch(Exception ex92) 
                    {
                        continue;
                    }
                    for (Pullenti.Ner.Token t = ar.FirstToken; t != null; t = t.Next) 
                    {
                        if (t.IsChar('/')) 
                        {
                            if (!(t.Previous is Pullenti.Ner.TextToken) || !(t.Next is Pullenti.Ner.TextToken)) 
                                continue;
                            if ((t.EndChar + 5) > name.Length) 
                                break;
                            if (t.BeginChar < 10) 
                                break;
                            if (!t.Chars.IsCapitalUpper) 
                                break;
                            string n1 = name.Substring(0, t.BeginChar).Trim();
                            string n2 = name.Substring(t.BeginChar + 1).Trim();
                            name = (names[nn] = n1);
                            names.Insert(nn + 1, n2);
                            break;
                        }
                    }
                }
                if (string.IsNullOrEmpty(name)) 
                    continue;
                name = _corrName(name);
                if (typ == "муниципальный округ") 
                {
                    if (name.StartsWith("поселение ")) 
                        name = name.Substring(10).Trim();
                }
                if (name.Contains("Олимп.дер")) 
                    name = "улица Олимпийская Деревня";
                else if (string.Compare("ЛЕНИНСКИЕ ГОРЫ", name, true) == 0) 
                    name = "улица " + name;
                for (int k = 0; k < 1; k++) 
                {
                    if (k > 0 && string.IsNullOrEmpty(typ)) 
                        continue;
                    string txt = (string.IsNullOrEmpty(typ) ? name : (k == 1 ? string.Format("{0} \"{1}\"", typ, name) : string.Format("{0} {1}", typ, name)));
                    if (string.Compare(typ ?? "", "километр", true) == 0 && (((name.Length < 6) || !char.IsDigit(name[0])))) 
                        txt = string.Format("{0} {1}", name, typ);
                    string txt0 = txt;
                    Pullenti.Ner.Geo.Internal.INameChecker ncheck = Pullenti.Ner.Geo.Internal.MiscLocationHelper.NameChecker;
                    Pullenti.Ner.Geo.Internal.MiscLocationHelper.NameChecker = null;
                    Pullenti.Ner.AnalysisResult ar = null;
                    try 
                    {
                        ar = Pullenti.Ner.ProcessorService.StandardProcessor.Process(new Pullenti.Ner.SourceOfAnalysis(txt) { UserParams = "GARADDRESS" }, null, null);
                    }
                    catch(Exception ex) 
                    {
                    }
                    Pullenti.Ner.Geo.Internal.MiscLocationHelper.NameChecker = ncheck;
                    Pullenti.Ner.Referent r = null;
                    Pullenti.Ner.Referent r2 = null;
                    if (ar == null) 
                        continue;
                    for (int ii = ar.Entities.Count - 1; ii >= 0; ii--) 
                    {
                        if (ar.Entities[ii] is Pullenti.Ner.Geo.GeoReferent) 
                        {
                            Pullenti.Ner.Geo.GeoReferent geo = ar.Entities[ii] as Pullenti.Ner.Geo.GeoReferent;
                            if (geo.FindSlot("NAME", "МОСКВА", true) != null) 
                            {
                                if (string.Compare("МОСКВА", name, true) == 0) 
                                {
                                }
                                else 
                                    continue;
                            }
                            if (geo.Occurrence.Count == 0 || geo.Occurrence[0].BeginChar > 8) 
                                continue;
                            if (r == null) 
                                r = geo;
                        }
                        else if (ar.Entities[ii] is Pullenti.Ner.Address.StreetReferent) 
                        {
                            if (r == null) 
                                r = ar.Entities[ii];
                            else if ((ar.Entities[ii] as Pullenti.Ner.Address.StreetReferent).Higher == r) 
                                r = ar.Entities[ii];
                        }
                        else if (ar.Entities[ii] is Pullenti.Ner.Address.AddressReferent) 
                        {
                            Pullenti.Ner.Address.AddressReferent aa = ar.Entities[ii] as Pullenti.Ner.Address.AddressReferent;
                            if (aa.Block != null) 
                            {
                                r2 = new Pullenti.Ner.Address.StreetReferent();
                                r2.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_TYPE, "блок", false, 0);
                                r2.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NUMBER, aa.Block, false, 0);
                                r2.Occurrence.AddRange(aa.Occurrence);
                            }
                        }
                        else 
                        {
                        }
                    }
                    int co = 0;
                    if (r == null) 
                    {
                        if ((name.IndexOf(' ') < 0) && (name.IndexOf('.') < 0) && string.IsNullOrEmpty(typ)) 
                        {
                            r = new Pullenti.Ner.Address.StreetReferent();
                            r.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NAME, name.ToUpper(), false, 0);
                            r.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_TYPE, "улица", false, 0);
                            co = 10;
                        }
                        else 
                        {
                            Pullenti.Ner.AnalysisResult ar1 = null;
                            try 
                            {
                                ar1 = Pullenti.Ner.ProcessorService.StandardProcessor.Process(new Pullenti.Ner.SourceOfAnalysis(txt0), null, null);
                            }
                            catch(Exception ex96) 
                            {
                            }
                            if (ar1 != null && ar1.FirstToken != null) 
                            {
                                if (txt0.Contains("линия")) 
                                {
                                }
                                List<Pullenti.Ner.Address.Internal.StreetItemToken> strs = Pullenti.Ner.Address.Internal.StreetItemToken.TryParseList(ar1.FirstToken, 10, null);
                                Pullenti.Ner.ReferentToken rt = Pullenti.Ner.Address.Internal.StreetDefineHelper.TryParseExtStreet(strs);
                                if (rt != null && rt.EndToken.Next == null) 
                                {
                                    txt = txt0;
                                    r = rt.Referent;
                                }
                            }
                            if (r == null) 
                                continue;
                        }
                    }
                    else if (r.Occurrence.Count > 0) 
                    {
                        if (r.Occurrence[0].EndChar < (txt.Length - 1)) 
                        {
                            if (r2 != null && r2.Occurrence.Count > 0 && r2.Occurrence[0].EndChar >= (txt.Length - 1)) 
                            {
                            }
                            else 
                                co += (txt.Length - 1 - r.Occurrence[0].EndChar);
                        }
                    }
                    if (co < bestCoef) 
                    {
                        bestCoef = co;
                        bestRef = r;
                        bestSecRef = r2;
                        bestRef2 = null;
                        if (bestCoef == 0) 
                            break;
                    }
                    else if (co == bestCoef) 
                    {
                        if (bestRef2 == null) 
                            bestRef2 = r;
                        else if (bestRef2.CanBeEquals(bestRef, Pullenti.Ner.Core.ReferentsEqualType.WithinOneText)) 
                            bestRef2 = r;
                    }
                }
                if (bestRef != null) 
                {
                    Ref = bestRef;
                    this.InitByReferent(Ref, true);
                    if (bestCoef > 0) 
                        Status = Pullenti.Address.GarStatus.Warning;
                    Pullenti.Ner.Referent secRef = null;
                    if (Ref is Pullenti.Ner.Address.StreetReferent) 
                    {
                        Pullenti.Ner.Address.StreetReferent str = Ref as Pullenti.Ner.Address.StreetReferent;
                        if (str.Higher != null) 
                            secRef = str.Higher;
                        else 
                        {
                            Pullenti.Ner.Geo.GeoReferent geo = str.GetSlotValue("GEO") as Pullenti.Ner.Geo.GeoReferent;
                            if (geo != null && geo.FindSlot("NAME", "Москва", true) == null) 
                                secRef = geo;
                        }
                    }
                    if (secRef != null) 
                    {
                        Sec = new NameAnalyzer();
                        Sec.InitByReferent(Ref, true);
                        this.InitByReferent(secRef, true);
                    }
                    else if (bestSecRef != null) 
                    {
                        Sec = new NameAnalyzer();
                        Sec.InitByReferent(bestSecRef, true);
                    }
                }
                if (Status == Pullenti.Address.GarStatus.Ok && Level == Pullenti.Address.AddrLevel.Undefined) 
                    Status = Pullenti.Address.GarStatus.Warning;
                if (Sec != null) 
                {
                    if (Sec.Sec != null) 
                        Status = Pullenti.Address.GarStatus.Error;
                    else if (Sec.Status != Pullenti.Address.GarStatus.Ok) 
                        Status = Sec.Status;
                    if (Status == Pullenti.Address.GarStatus.Ok) 
                        Status = Pullenti.Address.GarStatus.Ok2;
                }
            }
        }
        public void OutInfo(StringBuilder tmp)
        {
            if (Status == Pullenti.Address.GarStatus.Error && Sec == null) 
            {
                tmp.Append("ошибка");
                return;
            }
            tmp.AppendFormat("{0} ", Level);
            if (Types != null) 
            {
                for (int i = 0; i < Types.Count; i++) 
                {
                    tmp.AppendFormat("{0}{1}", (i > 0 ? "/" : ""), Types[i]);
                }
            }
            if (Strings != null && Strings.Count > 0) 
            {
                tmp.Append(" <");
                for (int i = 0; i < Strings.Count; i++) 
                {
                    tmp.AppendFormat("{0}{1}", (i > 0 ? ", " : ""), Strings[i]);
                }
                tmp.Append(">");
            }
            if (Miscs != null && Miscs.Count > 0) 
            {
                tmp.Append(" [");
                for (int i = 0; i < Miscs.Count; i++) 
                {
                    tmp.AppendFormat("{0}{1}", (i > 0 ? ", " : ""), Miscs[i]);
                }
                tmp.Append("]");
            }
            if (Sec != null) 
            {
                tmp.Append(" + ");
                Sec.OutInfo(tmp);
            }
            if (Status == Pullenti.Address.GarStatus.Warning) 
                tmp.Append(" (неточность при анализе)");
            else if (Status == Pullenti.Address.GarStatus.Error) 
                tmp.Append(" (ОШИБКА)");
        }
        public static Pullenti.Address.AddrLevel CalcLevel(Pullenti.Ner.Referent r)
        {
            Pullenti.Ner.Geo.GeoReferent geo = r as Pullenti.Ner.Geo.GeoReferent;
            Pullenti.Address.AddrLevel res = Pullenti.Address.AddrLevel.Undefined;
            if (geo != null) 
            {
                if (geo.IsState) 
                    return Pullenti.Address.AddrLevel.Country;
                if (geo.IsCity) 
                {
                    res = Pullenti.Address.AddrLevel.Locality;
                    foreach (string ty in geo.Typs) 
                    {
                        if (ty == "город" || ty == "місто") 
                        {
                            res = Pullenti.Address.AddrLevel.City;
                            string nam = geo.GetStringValue(Pullenti.Ner.Geo.GeoReferent.ATTR_NAME);
                            if (nam == "МОСКВА" || nam == "САНКТ-ПЕТЕРБУРГ" || nam == "СЕВАСТОПОЛЬ") 
                                res = Pullenti.Address.AddrLevel.RegionCity;
                            break;
                        }
                        else if (ty == "городское поселение" || ty == "сельское поселение") 
                        {
                            res = Pullenti.Address.AddrLevel.Settlement;
                            break;
                        }
                        else if (ty == "населенный пункт" && geo.Typs.Count == 1) 
                        {
                            string nam = geo.GetStringValue(Pullenti.Ner.Geo.GeoReferent.ATTR_NAME);
                            if (RegionHelper.IsBigCity(nam) != null) 
                                res = Pullenti.Address.AddrLevel.City;
                        }
                        else if (ty == "улус") 
                            res = Pullenti.Address.AddrLevel.District;
                    }
                }
                else if (geo.IsRegion) 
                {
                    res = Pullenti.Address.AddrLevel.District;
                    foreach (string ty in geo.Typs) 
                    {
                        if ((ty == "городской округ" || ty == "муниципальный район" || ty == "муниципальный округ") || ty == "федеральная территория") 
                        {
                            res = Pullenti.Address.AddrLevel.District;
                            break;
                        }
                        else if (ty == "район" || ty == "автономный округ") 
                        {
                            res = Pullenti.Address.AddrLevel.District;
                            break;
                        }
                        else if (ty == "область" || ty == "край") 
                        {
                            res = Pullenti.Address.AddrLevel.RegionArea;
                            break;
                        }
                        else if (ty == "сельский округ") 
                        {
                            res = Pullenti.Address.AddrLevel.Settlement;
                            break;
                        }
                        else if (ty == "республика") 
                        {
                            res = Pullenti.Address.AddrLevel.RegionArea;
                            break;
                        }
                    }
                }
                return res;
            }
            Pullenti.Ner.Address.StreetReferent street = r as Pullenti.Ner.Address.StreetReferent;
            if (street != null) 
            {
                res = Pullenti.Address.AddrLevel.Street;
                Pullenti.Ner.Address.StreetKind ki = (r as Pullenti.Ner.Address.StreetReferent).Kind;
                if (ki == Pullenti.Ner.Address.StreetKind.Area || ki == Pullenti.Ner.Address.StreetKind.Org) 
                    res = Pullenti.Address.AddrLevel.Territory;
            }
            if (r is Pullenti.Ner.Org.OrganizationReferent) 
                return Pullenti.Address.AddrLevel.Territory;
            return res;
        }
        public static string CorrectFiasName(string name)
        {
            if (name == null) 
                return null;
            int ii = name.IndexOf(", находящ");
            if (ii < 0) 
                ii = name.IndexOf(",находящ");
            if (ii > 0) 
                name = name.Substring(0, ii).Trim();
            if (name.Contains("Г СК ")) 
                name = name.Replace("Г СК ", "ГСК ");
            return name;
        }
        public static string CorrName(string str)
        {
            StringBuilder res = new StringBuilder(str.Length);
            _corrName2(res, str.ToUpper());
            return res.ToString();
        }
        static int _corrName2(StringBuilder res, string str)
        {
            int corr = 0;
            for (int i = 0; i < str.Length; i++) 
            {
                char ch = str[i];
                if (ch == 'Ь' || ch == 'Ъ') 
                {
                    corr++;
                    continue;
                }
                if (char.IsLetterOrDigit(ch) || ch == ' ' || ch == '-') 
                {
                    if (ch == '-') 
                    {
                        ch = ' ';
                        corr++;
                    }
                    if (i > 0 && res.Length > 0 && res[res.Length - 1] == ch) 
                    {
                        corr++;
                        continue;
                    }
                    res.Append(ch);
                }
            }
            if (str.Length > 4 && res.Length > 4) 
            {
                char ch1 = res[res.Length - 1];
                char ch2 = res[res.Length - 2];
                char ch3 = res[res.Length - 3];
                if (Pullenti.Morph.LanguageHelper.IsCyrillicVowel(ch1) || ch1 == 'Й') 
                {
                    if (!Pullenti.Morph.LanguageHelper.IsCyrillicVowel(ch2)) 
                    {
                        if (ch2 == 'Г' && ch3 == 'О') 
                            res.Length -= 2;
                        res[res.Length - 1] = '@';
                    }
                    else if (!Pullenti.Morph.LanguageHelper.IsCyrillicVowel(ch3)) 
                    {
                        res.Length -= 1;
                        res[res.Length - 1] = '@';
                    }
                }
            }
            return corr;
        }
        static string _corrName(string name)
        {
            int jj = name.IndexOf('(');
            if (jj > 0) 
                name = name.Substring(0, jj).Trim();
            string secVar;
            string det;
            name = CorrectionHelper.Correct(name, out secVar, out det);
            if (char.IsDigit(name[name.Length - 1])) 
            {
                for (int i = name.Length - 1; i > 0; i--) 
                {
                    if (!char.IsDigit(name[i])) 
                    {
                        if (name[i] != '-') 
                            name += "-й";
                        break;
                    }
                }
            }
            return name;
        }
        public static void CreateSearchVariants(List<string> res, List<string> res1, List<string> res2, string name, string num = null)
        {
            if (name == null) 
                return;
            List<string> items = new List<string>();
            int sps = 0;
            int hiphs = 0;
            for (int i = 0; i < name.Length; i++) 
            {
                char ch = name[i];
                int j;
                if (char.IsLetter(ch)) 
                {
                    for (j = i; j < name.Length; j++) 
                    {
                        if (!char.IsLetter(name[j])) 
                            break;
                    }
                    if (i == 0 && j == name.Length) 
                        items.Add(name);
                    else 
                        items.Add(name.Substring(i, j - i));
                    i = j - 1;
                }
                else if (ch == ' ' || ch == '.') 
                    sps++;
                else if (ch == '-') 
                    hiphs++;
                else if (char.IsDigit(ch) && num == null) 
                {
                    for (j = i; j < name.Length; j++) 
                    {
                        if (!char.IsDigit(name[j])) 
                            break;
                    }
                    num = name.Substring(i, j - i);
                    i = j - 1;
                }
            }
            string stdAdj = null;
            if (items.Count > 1) 
            {
                for (int i = 0; i < items.Count; i++) 
                {
                    string it = items[i];
                    if (it == "И") 
                    {
                        items.RemoveAt(i);
                        i--;
                        if (items.Count == 1) 
                            break;
                        continue;
                    }
                    for (int k = 0; k < 2; k++) 
                    {
                        string[] adjs = (k == 0 ? m_StdArjsO : m_StdArjsE);
                        string[] adjsAbbr = (k == 0 ? m_StdArjsOAbbr : m_StdArjsEAbbr);
                        for (int j = 0; j < adjs.Length; j++) 
                        {
                            string a = adjs[j];
                            if (it.StartsWith(a)) 
                            {
                                if (it.Length == (a.Length + 2)) 
                                {
                                    stdAdj = adjsAbbr[j];
                                    items.RemoveAt(i);
                                    break;
                                }
                                if (it.Length == (a.Length + 1)) 
                                {
                                    if (k == 0 && it[a.Length] == 'О') 
                                    {
                                    }
                                    else if (k == 1 && it[a.Length] == 'Е') 
                                    {
                                    }
                                    else 
                                        continue;
                                    stdAdj = adjsAbbr[j];
                                    items.RemoveAt(i);
                                    break;
                                }
                                if (it.Length > (a.Length + 3)) 
                                {
                                    if (k == 0 && it[a.Length] == 'О') 
                                    {
                                    }
                                    else if (k == 1 && it[a.Length] == 'Е') 
                                    {
                                    }
                                    else 
                                        continue;
                                    stdAdj = adjsAbbr[j];
                                    items[i] = it.Substring(a.Length + 1);
                                    break;
                                }
                            }
                        }
                        if (stdAdj != null) 
                            break;
                    }
                    if (stdAdj != null) 
                        break;
                }
            }
            if (items.Count > 1) 
                items.Sort();
            string pref = null;
            if (stdAdj != null) 
            {
                pref = stdAdj.ToLower();
                if (num != null) 
                    pref += num;
            }
            else if (num != null) 
                pref = num;
            StringBuilder tmp = new StringBuilder();
            if (pref != null) 
                tmp.Append(pref);
            for (int i = 0; i < items.Count; i++) 
            {
                _corrName2(tmp, items[i]);
            }
            string r = tmp.ToString();
            if (!res.Contains(r)) 
                res.Add(r);
            if (items.Count == 1 && items[0].EndsWith("ОГО")) 
            {
                string rr = r.Substring(0, r.Length - 1) + "ОГ@";
                if (!res.Contains(rr)) 
                    res.Add(rr);
            }
            if (res2 != null && pref != null) 
            {
                tmp.Remove(0, pref.Length);
                tmp.Append(pref);
                r = tmp.ToString();
                if (!res2.Contains(r)) 
                    res2.Add(r);
            }
        }
        public static string CorrectAdj(string val)
        {
            for (int i = 0; i < m_StdArjsE.Length; i++) 
            {
                if (val.StartsWith(m_StdArjsE[i])) 
                    return m_StdArjsEAbbr[i].ToLower();
            }
            for (int i = 0; i < m_StdArjsO.Length; i++) 
            {
                if (val.StartsWith(m_StdArjsO[i])) 
                    return m_StdArjsOAbbr[i].ToLower();
            }
            return null;
        }
        static string[] m_StdArjsO = new string[] {"СТАР", "НОВ", "МАЛ", "СЕВЕР", "ЮГ", "ЮЖН", "ЗАПАДН", "ВОСТОЧН", "КРАСН", "БЕЛ", "ГЛАВН", "ВЕЛИК"};
        static string[] m_StdArjsOAbbr = new string[] {"СТ", "НВ", "МЛ", "СВ", "ЮГ", "ЮГ", "ЗП", "ВС", "КР", "БЛ", "ГЛ", "ВЛ"};
        static string[] m_StdArjsE = new string[] {"ВЕРХН", "НИЖН", "СРЕДН", "БОЛЬШ"};
        static string[] m_StdArjsEAbbr = new string[] {"ВР", "НЖ", "СР", "БЛ"};
        static void GetStrings(Pullenti.Ner.Referent r, List<string> res, List<string> doubts, List<string> revs)
        {
            if (r == null) 
                return;
            if ((r is Pullenti.Ner.Geo.GeoReferent) || (r is Pullenti.Ner.Org.OrganizationReferent)) 
            {
                string num = r.GetStringValue("NUMBER");
                foreach (Pullenti.Ner.Slot s in r.Slots) 
                {
                    if (s.TypeName == Pullenti.Ner.Geo.GeoReferent.ATTR_NAME) 
                    {
                        string str = s.Value as string;
                        if (string.IsNullOrEmpty(str)) 
                            continue;
                        CreateSearchVariants(res, doubts, revs, str, num);
                        if ((str.Length > 3 && !Pullenti.Morph.LanguageHelper.IsCyrillicVowel(str[str.Length - 1]) && str[str.Length - 1] != 'Й') && str[str.Length - 1] != 'Ь') 
                            CreateSearchVariants(doubts, null, null, str + "А", num);
                    }
                }
            }
            else if (r is Pullenti.Ner.Address.StreetReferent) 
            {
                Pullenti.Ner.Address.StreetReferent str = r as Pullenti.Ner.Address.StreetReferent;
                string num = str.Numbers;
                foreach (Pullenti.Ner.Slot s in r.Slots) 
                {
                    if (s.TypeName == Pullenti.Ner.Address.StreetReferent.ATTR_NAME) 
                        CreateSearchVariants(res, doubts, revs, s.Value as string, num);
                }
                if (res.Count == 0 && num != null) 
                {
                    if (num.EndsWith("км")) 
                        num = num.Substring(0, num.Length - 2);
                    res.Add(num);
                }
                if (res.Count == 0) 
                {
                    string ty = r.GetStringValue(Pullenti.Ner.Address.StreetReferent.ATTR_TYPE);
                    if (ty != null) 
                        res.Add(CorrName(ty.ToUpper()));
                }
            }
            for (int i = 0; i < res.Count; i++) 
            {
                for (int j = 0; j < (res.Count - 1); j++) 
                {
                    if (res[j].Length < res[j + 1].Length) 
                    {
                        string s = res[j];
                        res[j] = res[j + 1];
                        res[j + 1] = s;
                    }
                }
            }
        }
        public int CalcEqualCoef(NameAnalyzer na)
        {
            if (na == null) 
                return -1;
            if (na.Level == Pullenti.Address.AddrLevel.Territory && Level == Pullenti.Address.AddrLevel.Territory) 
            {
                if (((na.Ref.FindSlot("NAME", null, true) == null || Ref.FindSlot("NAME", null, true) == null)) && Miscs.Count > 0) 
                {
                    foreach (string m in Miscs) 
                    {
                        if (na.Miscs.Contains(m)) 
                            return 0;
                    }
                    return -1;
                }
            }
            return 0;
        }
        public bool CanBeEquals(NameAnalyzer na)
        {
            if (na == null) 
                return false;
            bool ok = false;
            foreach (string s in Strings) 
            {
                if (na.Strings.Contains(s)) 
                {
                    ok = true;
                    break;
                }
            }
            if (!ok) 
                return false;
            return true;
        }
    }
}