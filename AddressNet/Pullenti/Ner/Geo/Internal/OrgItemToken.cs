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

namespace Pullenti.Ner.Geo.Internal
{
    public class OrgItemToken : Pullenti.Ner.ReferentToken
    {
        public OrgItemToken(Pullenti.Ner.Referent r, Pullenti.Ner.Token b, Pullenti.Ner.Token e) : base(r, b, e, null)
        {
        }
        public bool IsDoubt;
        public bool HasTerrKeyword;
        public bool KeywordAfter;
        public bool IsGsk;
        public bool NotOrg;
        public bool NotGeo;
        public void SetGsk()
        {
            IsGsk = false;
            if (NotOrg) 
            {
                IsGsk = true;
                return;
            }
            foreach (Pullenti.Ner.Slot s in Referent.Slots) 
            {
                if (s.TypeName == "TYPE" && (s.Value is string)) 
                {
                    string ty = s.Value as string;
                    if ((((((((ty.Contains("товарищество") || ty.Contains("кооператив") || ty.Contains("коллектив")) || Pullenti.Morph.LanguageHelper.EndsWithEx(ty, "поселок", " отдыха", " часть", null) || ty.Contains("партнерство")) || ty.Contains("объединение") || ty.Contains("бизнес")) || ty.Contains("офисн") || ((ty.Contains("станция") && !ty.Contains("заправоч")))) || ty.Contains("аэропорт") || ty.Contains("пансионат")) || ty.Contains("санаторий") || ty.Contains("база")) || ty.Contains("урочище") || ty.Contains("кадастровый")) || ty.Contains("лесничество")) 
                    {
                        IsGsk = true;
                        return;
                    }
                    if (ty == "АОЗТ" || ty == "пядь") 
                    {
                        IsGsk = true;
                        return;
                    }
                    if (ty.Contains("хозяйство")) 
                    {
                        if (ty.Contains("кресьян") || ty.Contains("фермер")) 
                        {
                            IsGsk = true;
                            return;
                        }
                    }
                }
                else if (s.TypeName == "NAME" && (s.Value is string)) 
                {
                    string nam = s.Value as string;
                    if (Pullenti.Morph.LanguageHelper.EndsWithEx(nam, "ГЭС", "АЭС", "ТЭС", null)) 
                    {
                        IsGsk = true;
                        return;
                    }
                }
            }
        }
        public override string ToString()
        {
            StringBuilder tmp = new StringBuilder();
            if (IsDoubt) 
                tmp.Append("? ");
            if (HasTerrKeyword) 
                tmp.Append("Terr ");
            if (IsGsk) 
                tmp.Append("Gsk ");
            if (NotOrg) 
                tmp.Append("NotOrg ");
            if (NotGeo) 
                tmp.Append("NotGeo ");
            tmp.Append(Referent.ToString());
            return tmp.ToString();
        }
        public static bool SpeedRegime = false;
        internal static void PrepareAllData(Pullenti.Ner.Token t0)
        {
            if (!SpeedRegime) 
                return;
            GeoAnalyzerData ad = Pullenti.Ner.Geo.GeoAnalyzer.GetData(t0);
            if (ad == null) 
                return;
            ad.ORegime = false;
            for (Pullenti.Ner.Token t = t0; t != null; t = t.Next) 
            {
                GeoTokenData d = t.Tag as GeoTokenData;
                OrgItemToken org = TryParse(t, ad);
                if (org != null) 
                {
                    if (d == null) 
                        d = new GeoTokenData(t);
                    d.Org = org;
                    if (org.HasTerrKeyword || org.NotGeo || ((org.IsGsk && !org.KeywordAfter && !org.NotOrg))) 
                    {
                        for (Pullenti.Ner.Token tt = org.BeginToken; tt != null && tt.EndChar <= org.EndChar; tt = tt.Next) 
                        {
                            GeoTokenData dd = tt.Tag as GeoTokenData;
                            if (dd == null) 
                                dd = new GeoTokenData(tt);
                            dd.NoGeo = true;
                        }
                        if (!org.HasTerrKeyword) 
                            t = org.EndToken;
                    }
                }
            }
            ad.ORegime = true;
        }
        public static OrgItemToken TryParse(Pullenti.Ner.Token t, GeoAnalyzerData ad = null)
        {
            if (!(t is Pullenti.Ner.TextToken)) 
                return null;
            if (ad == null) 
                ad = Pullenti.Ner.Geo.GeoAnalyzer.GetData(t);
            if (ad == null) 
                return null;
            if (SpeedRegime && ((ad.ORegime || ad.AllRegime))) 
            {
                if ((t is Pullenti.Ner.TextToken) && t.IsChar('м')) 
                {
                }
                else 
                {
                    GeoTokenData d = t.Tag as GeoTokenData;
                    if (d != null) 
                        return d.Org;
                    return null;
                }
            }
            if (ad.OLevel > 1) 
                return null;
            ad.OLevel++;
            OrgItemToken res = _TryParse(t, false, 0, ad);
            if (res != null) 
                res._tryParseDetails();
            ad.OLevel--;
            return res;
        }
        static OrgItemToken _TryParse(Pullenti.Ner.Token t, bool afterTerr, int lev, GeoAnalyzerData ad)
        {
            if (lev > 3 || t == null || t.IsComma) 
                return null;
            Pullenti.Ner.Token tt2 = MiscLocationHelper.CheckTerritory(t);
            if (tt2 != null && tt2.Next != null) 
            {
                tt2 = tt2.Next;
                bool br = false;
                if (Pullenti.Ner.Core.BracketHelper.IsBracket(tt2, true)) 
                {
                    br = true;
                    tt2 = tt2.Next;
                }
                if (tt2 == null || lev > 3) 
                    return null;
                OrgItemToken re2 = _TryParse(tt2, true, lev + 1, ad);
                if (re2 == null && tt2 != null && tt2.IsValue("ВЛАДЕНИЕ", null)) 
                    re2 = _TryParse(tt2.Next, true, lev + 1, ad);
                if (re2 != null) 
                {
                    Pullenti.Ner.Analyzer a = t.Kit.Processor.FindAnalyzer("GEO");
                    if (a != null && !MiscLocationHelper.IsUserParamAddress(t)) 
                    {
                        Pullenti.Ner.ReferentToken rt = a.ProcessReferent(tt2, null);
                        if (rt != null) 
                            return null;
                    }
                    for (Pullenti.Ner.Token tt = tt2; tt != null && tt.EndChar <= re2.EndChar; tt = tt.Next) 
                    {
                        Pullenti.Ner.Address.Internal.StreetItemToken sit = Pullenti.Ner.Address.Internal.StreetItemToken.TryParse(tt, null, false, null);
                        if (sit != null && sit.Typ == Pullenti.Ner.Address.Internal.StreetItemType.Noun && ((sit.IsRoad || sit.IsRailway))) 
                            return null;
                    }
                    if (tt2.IsValue("ВЛАДЕНИЕ", null)) 
                        re2.Referent.AddSlot("TYPE", "владение", false, 0);
                    re2.BeginToken = t;
                    if (br && Pullenti.Ner.Core.BracketHelper.CanBeEndOfSequence(re2.EndToken.Next, false, null, false)) 
                        re2.EndToken = re2.EndToken.Next;
                    re2.HasTerrKeyword = true;
                    return re2;
                }
                else if ((t is Pullenti.Ner.TextToken) && (((t as Pullenti.Ner.TextToken).Term.StartsWith("ТЕР") || (t as Pullenti.Ner.TextToken).Term.StartsWith("ПЛОЩ"))) && (tt2.WhitespacesBeforeCount < 3)) 
                {
                    NameToken nam1 = NameToken.TryParse(tt2, GeoTokenType.Org, 0, true);
                    if (nam1 != null && ((nam1.Name != null || ((nam1.Number != null && MiscLocationHelper.IsUserParamAddress(tt2)))))) 
                    {
                        if (Pullenti.Ner.Address.Internal.StreetItemToken.CheckKeyword(tt2)) 
                            return null;
                        if (t.Next != nam1.EndToken && Pullenti.Ner.Address.Internal.StreetItemToken.CheckKeyword(nam1.EndToken)) 
                            return null;
                        if (TerrItemToken.CheckKeyword(tt2) != null) 
                            return null;
                        if (t.Next != nam1.EndToken && TerrItemToken.CheckKeyword(nam1.EndToken) != null) 
                            return null;
                        Pullenti.Ner.Core.IntOntologyToken ter = TerrItemToken.CheckOntoItem(tt2);
                        if (ter != null) 
                        {
                            Pullenti.Ner.Geo.GeoReferent geo = ter.Item.Referent as Pullenti.Ner.Geo.GeoReferent;
                            if (geo.IsCity || geo.IsState) 
                                return null;
                        }
                        if (CityItemToken.CheckKeyword(tt2) != null) 
                            return null;
                        if (CityItemToken.CheckOntoItem(tt2) != null) 
                            return null;
                        Pullenti.Ner.Token tt = nam1.EndToken;
                        bool ok = false;
                        if (tt.IsNewlineAfter) 
                            ok = true;
                        else if (tt.Next != null && ((tt.Next.IsComma || tt.Next.IsChar(')')))) 
                            ok = true;
                        else if (Pullenti.Ner.Address.Internal.AddressItemToken.CheckHouseAfter(tt2, false, false)) 
                            ok = true;
                        else 
                        {
                            Pullenti.Ner.Address.Internal.AddressItemToken ait = Pullenti.Ner.Address.Internal.AddressItemToken.TryParsePureItem(nam1.EndToken.Next, null, null);
                            if (ait != null && ait.Typ != Pullenti.Ner.Address.Internal.AddressItemType.Number) 
                                ok = true;
                            else 
                            {
                                Pullenti.Ner.Address.Internal.AddressItemToken a2 = Pullenti.Ner.Address.Internal.AddressItemToken.TryParse(nam1.EndToken.Next, false, null, ad);
                                if (a2 != null) 
                                {
                                    Pullenti.Ner.Address.Internal.AddressItemToken a1 = Pullenti.Ner.Address.Internal.AddressItemToken.TryParse(tt2, false, null, ad);
                                    if (a1 == null || (a1.EndChar < a2.EndChar)) 
                                        ok = true;
                                }
                            }
                        }
                        if (ok) 
                        {
                            Pullenti.Ner.Referent org1 = t.Kit.CreateReferent("ORGANIZATION");
                            if (nam1.Name != null) 
                                org1.AddSlot("NAME", nam1.Name, false, 0);
                            if (nam1.Number != null) 
                                org1.AddSlot("NUMBER", nam1.Number, false, 0);
                            if (tt2.Previous != null && tt2.Previous.IsValue("ВЛАДЕНИЕ", null)) 
                                org1.AddSlot("TYPE", "владение", false, 0);
                            OrgItemToken res1 = new OrgItemToken(org1, t, nam1.EndToken);
                            res1.Data = t.Kit.GetAnalyzerDataByAnalyzerName("ORGANIZATION");
                            res1.HasTerrKeyword = true;
                            return res1;
                        }
                    }
                    Pullenti.Ner.ReferentToken rt = t.Kit.ProcessReferent("NAMEDENTITY", tt2, null);
                    if (rt != null) 
                    {
                        OrgItemToken res1 = new OrgItemToken(rt.Referent, t, rt.EndToken);
                        res1.Data = t.Kit.GetAnalyzerDataByAnalyzerName("NAMEDENTITY");
                        res1.HasTerrKeyword = true;
                        return res1;
                    }
                }
                if (!t.IsValue("САД", null)) 
                    return null;
            }
            bool typAfter = false;
            bool doubt0 = false;
            OrgTypToken tokTyp = OrgTypToken.TryParse(t, afterTerr, ad);
            NameToken nam = null;
            bool ignoreNum = false;
            if (tokTyp == null) 
            {
                NumToken num = NumToken.TryParse(t, GeoTokenType.Org);
                if (num != null && num.HasSpecWord) 
                {
                    OrgItemToken next = TryParse(num.EndToken.Next, ad);
                    if (next != null && next.Referent.FindSlot("NUMBER", null, true) == null) 
                    {
                        next.BeginToken = t;
                        next.Referent.AddSlot("NUMBER", num.Value, false, 0);
                        return next;
                    }
                }
                Pullenti.Ner.Address.Internal.AddressItemToken ait = Pullenti.Ner.Address.Internal.AddressItemToken.TryParsePureItem(t, null, null);
                if ((ait != null && ait.Typ == Pullenti.Ner.Address.Internal.AddressItemType.House && ait.HouseType == Pullenti.Ner.Address.AddressHouseType.Estate) && ait.Value != null) 
                {
                    bool ok3 = false;
                    if (afterTerr) 
                        ok3 = true;
                    else if (Pullenti.Ner.Address.Internal.AddressItemToken.CheckStreetAfter(ait.EndToken.Next, false)) 
                        ok3 = true;
                    if (ok3) 
                    {
                        Pullenti.Ner.Referent org3 = t.Kit.CreateReferent("ORGANIZATION");
                        org3.AddSlot("TYPE", "владение", false, 0);
                        StringBuilder num3 = new StringBuilder();
                        StringBuilder nam3 = new StringBuilder();
                        foreach (char ch in ait.Value) 
                        {
                            if (char.IsDigit(ch)) 
                                num3.Append(ch);
                            else if (char.IsLetter(ch)) 
                                nam3.Append(ch);
                        }
                        if (num3.Length > 0) 
                            org3.AddSlot("NUMBER", num3.ToString(), false, 0);
                        if (nam3.Length > 0) 
                            org3.AddSlot("NAME", nam3.ToString(), false, 0);
                        OrgItemToken res3 = new OrgItemToken(org3, t, ait.EndToken);
                        res3.Data = t.Kit.GetAnalyzerDataByAnalyzerName("ORGANIZATION");
                        res3.NotOrg = true;
                        res3.HasTerrKeyword = afterTerr;
                        res3.NotGeo = true;
                        return res3;
                    }
                }
                int ok = 0;
                if (Pullenti.Ner.Core.BracketHelper.CanBeStartOfSequence(t, true, false)) 
                    ok = 2;
                else if (t.IsValue("ИМ", null) || t.IsValue("ИМЕНИ", null)) 
                    ok = 2;
                else if ((t is Pullenti.Ner.TextToken) && !t.Chars.IsAllLower && t.LengthChar > 1) 
                    ok = 1;
                else if (afterTerr) 
                    ok = 1;
                if (ok == 0) 
                    return null;
                if (CityItemToken.CheckKeyword(t) != null) 
                    return null;
                if (CityItemToken.CheckOntoItem(t) != null) 
                    return null;
                if ((t.LengthChar > 5 && (t is Pullenti.Ner.TextToken) && !t.Chars.IsAllUpper) && !t.Chars.IsAllLower && !t.Chars.IsCapitalUpper) 
                {
                    string namm = (t as Pullenti.Ner.TextToken).GetSourceText();
                    if (char.IsUpper(namm[0]) && char.IsUpper(namm[1])) 
                    {
                        for (int i = 0; i < namm.Length; i++) 
                        {
                            if (char.IsLower(namm[i]) && i > 2) 
                            {
                                string abbr = namm.Substring(0, i - 1);
                                Pullenti.Ner.Core.Termin te = new Pullenti.Ner.Core.Termin(abbr) { Acronym = abbr };
                                List<Pullenti.Ner.Core.Termin> li = OrgTypToken.FindTerminByAcronym(abbr);
                                if (li != null && li.Count > 0) 
                                {
                                    nam = new NameToken(t, t);
                                    nam.Name = (t as Pullenti.Ner.TextToken).Term.Substring(i - 1);
                                    tokTyp = new OrgTypToken(t, t);
                                    tokTyp.Vals.Add(li[0].CanonicText.ToLower());
                                    tokTyp.Vals.Add(abbr);
                                    nam.TryAttachNumber();
                                    break;
                                }
                            }
                        }
                    }
                }
                if (nam == null) 
                {
                    if (afterTerr) 
                        ok = 2;
                    if (ok < 2) 
                    {
                        int kk = 0;
                        for (Pullenti.Ner.Token tt = t.Next; tt != null && (kk < 5); tt = tt.Next,kk++) 
                        {
                            if (tt.IsNewlineBefore) 
                                break;
                            OrgTypToken ty22 = OrgTypToken.TryParse(tt, false, ad);
                            if (ty22 == null || ty22.IsDoubt || ty22.CanBeSingle) 
                                continue;
                            ok = 2;
                            break;
                        }
                    }
                    if (ok < 2) 
                        return null;
                    typAfter = true;
                    nam = NameToken.TryParse(t, GeoTokenType.Org, 0, false);
                    if (nam == null) 
                        return null;
                    tokTyp = OrgTypToken.TryParse(nam.EndToken.Next, afterTerr, ad);
                    if (tokTyp == null && !afterTerr && MiscLocationHelper.IsUserParamAddress(t)) 
                    {
                        tt2 = MiscLocationHelper.CheckTerritory(nam.EndToken.Next);
                        if (tt2 != null && tt2.Next != null) 
                        {
                            tokTyp = OrgTypToken.TryParse(tt2.Next, true, ad);
                            if (tokTyp != null) 
                            {
                                NameToken nam2 = NameToken.TryParse(tokTyp.EndToken.Next, GeoTokenType.Org, 0, false);
                                if (nam2 != null && nam2.Name != null) 
                                    tokTyp = null;
                            }
                        }
                    }
                    if (nam.Name == null && nam.MiscTyp == null) 
                    {
                        if (nam.Number != null && tokTyp != null) 
                        {
                        }
                        else if (afterTerr) 
                        {
                        }
                        else 
                            return null;
                    }
                    if (tokTyp != null) 
                    {
                        if (nam.BeginToken == nam.EndToken) 
                        {
                            Pullenti.Morph.MorphClass mc = nam.GetMorphClassInDictionary();
                            if (mc.IsConjunction || mc.IsPreposition || mc.IsPronoun) 
                                return null;
                        }
                        OrgItemToken rt2 = TryParse(tokTyp.BeginToken, null);
                        if (rt2 != null && rt2.IsDoubt) 
                            rt2 = null;
                        if (rt2 != null) 
                        {
                            if (MiscLocationHelper.CheckTerritory(tokTyp.EndToken.Next) != null) 
                            {
                                OrgItemToken rt3 = TryParse(tokTyp.EndToken.Next, ad);
                                if (rt3 != null) 
                                    rt2 = null;
                            }
                        }
                        NameToken nam2 = NameToken.TryParse(tokTyp.EndToken.Next, GeoTokenType.Org, 0, false);
                        if (tokTyp.IsNewlineAfter) 
                            nam2 = null;
                        if (rt2 != null && rt2.EndChar > tokTyp.EndChar) 
                        {
                            if (nam2 == null || nam2.EndToken != rt2.EndToken) 
                                return null;
                            if (((nam.Number == null && nam2.Name == null && nam2.Number != null)) || (((nam.Name == null && nam.Number != null && nam2.Number == null) && nam2.Name != null))) 
                            {
                                if (nam2.Number != null) 
                                    nam.Number = nam2.Number;
                                if (nam2.Name != null) 
                                    nam.Name = nam2.Name;
                                tokTyp = tokTyp.Clone();
                                tokTyp.EndToken = nam2.EndToken;
                            }
                            else 
                                return null;
                        }
                        else if ((nam.Number == null && nam2 != null && nam2.Name == null) && nam2.Number != null) 
                        {
                            nam.Number = nam2.Number;
                            tokTyp = tokTyp.Clone();
                            tokTyp.EndToken = nam2.EndToken;
                        }
                        nam.EndToken = tokTyp.EndToken;
                        doubt0 = true;
                    }
                    else if (nam.Name != null || nam.MiscTyp != null) 
                    {
                        bool busines = false;
                        if (nam.MiscTyp != null) 
                        {
                        }
                        else if (nam.Name.EndsWith("ПЛАЗА") || nam.Name.StartsWith("БИЗНЕС")) 
                            busines = true;
                        else if (afterTerr && MiscLocationHelper.IsUserParamAddress(nam)) 
                        {
                            if (Pullenti.Ner.Address.Internal.StreetItemToken.CheckKeyword(nam.BeginToken)) 
                                return null;
                        }
                        else if (nam.BeginToken == nam.EndToken) 
                            return null;
                        else if (nam.Name != null && nam.Name.Length == 1 && nam.Number == null) 
                            return null;
                        else if (Pullenti.Ner.Core.BracketHelper.CanBeStartOfSequence(nam.BeginToken, false, false) && MiscLocationHelper.IsUserParamAddress(nam)) 
                        {
                        }
                        else if ((((tokTyp = OrgTypToken.TryParse(nam.EndToken, false, ad)))) == null) 
                            return null;
                        else if (nam.Morph.Case.IsGenitive && !nam.Morph.Case.IsNominative) 
                        {
                            nam.Name = Pullenti.Ner.Core.MiscHelper.GetTextValueOfMetaToken(nam, Pullenti.Ner.Core.GetTextAttr.FirstNounGroupToNominativeSingle).Replace("-", " ");
                            if (tokTyp != null && tokTyp.Vals.Count > 0) 
                            {
                                if (nam.Name.EndsWith(tokTyp.Vals[0], StringComparison.OrdinalIgnoreCase)) 
                                    nam.Name = nam.Name.Substring(0, nam.Name.Length - tokTyp.Vals[0].Length).Trim();
                            }
                        }
                        if (tokTyp == null) 
                        {
                            tokTyp = new OrgTypToken(t, t);
                            if (busines) 
                            {
                                tokTyp.Vals.Add("бизнес центр");
                                tokTyp.Vals.Add("БЦ");
                            }
                            else if (t.Previous != null && t.Previous.IsValue("САД", null)) 
                                tokTyp.Vals.Add("сад");
                            else if (nam.MiscTyp != null) 
                                tokTyp.Vals.Add(nam.MiscTyp);
                        }
                        nam.IsDoubt = tokTyp.Vals.Count == 0;
                        doubt0 = tokTyp.Vals.Count == 0;
                    }
                    else 
                        return null;
                }
            }
            else 
            {
                if (tokTyp.WhitespacesAfterCount > 3 && !tokTyp.IsNewlineAfter) 
                    return null;
                Pullenti.Ner.Token tt3 = MiscLocationHelper.CheckTerritory(tokTyp.EndToken.Next);
                if (tt3 != null) 
                {
                    tokTyp = tokTyp.Clone();
                    tokTyp.EndToken = tt3;
                    afterTerr = true;
                    OrgTypToken tokTyp2 = OrgTypToken.TryParse(tokTyp.EndToken.Next, true, ad);
                    if (tokTyp2 != null && !tokTyp2.IsDoubt) 
                        tokTyp.MergeWith(tokTyp2);
                }
                else 
                {
                    OrgTypToken tokTyp2 = OrgTypToken.TryParse(tokTyp.EndToken.Next, true, ad);
                    if (tokTyp2 != null && tokTyp2.BeginToken == tokTyp2.EndToken) 
                    {
                        Pullenti.Morph.MorphClass mc = tokTyp2.BeginToken.GetMorphClassInDictionary();
                        if (!mc.IsUndefined) 
                            tokTyp2 = null;
                    }
                    if (tokTyp2 != null && !tokTyp2.IsDoubt) 
                    {
                        tokTyp = tokTyp.Clone();
                        tokTyp.MergeWith(tokTyp2);
                    }
                }
                if (Pullenti.Ner.Core.BracketHelper.IsBracket(tokTyp.EndToken.Next, true)) 
                {
                    OrgTypToken tokTyp2 = OrgTypToken.TryParse(tokTyp.EndToken.Next.Next, afterTerr, ad);
                    if (tokTyp2 != null && !tokTyp2.IsDoubt) 
                    {
                        tokTyp = tokTyp.Clone();
                        tokTyp.IsDoubt = false;
                        nam = NameToken.TryParse(tokTyp2.EndToken.Next, GeoTokenType.Org, 0, false);
                        if (nam != null && Pullenti.Ner.Core.BracketHelper.CanBeEndOfSequence(nam.EndToken.Next, false, null, false)) 
                        {
                            tokTyp.MergeWith(tokTyp2);
                            nam.EndToken = nam.EndToken.Next;
                        }
                        else if (nam != null && Pullenti.Ner.Core.BracketHelper.CanBeEndOfSequence(nam.EndToken, false, null, false)) 
                            tokTyp.MergeWith(tokTyp2);
                        else 
                            nam = null;
                    }
                }
            }
            if (m_Onto.TryParse(tokTyp.EndToken.Next, Pullenti.Ner.Core.TerminParseAttr.No) != null) 
            {
            }
            else if (Pullenti.Ner.Address.Internal.StreetItemToken.CheckKeyword(tokTyp.EndToken.Next) && !tokTyp.EndToken.Next.Chars.IsCapitalUpper) 
            {
            }
            else 
            {
                if (nam == null && (tokTyp.WhitespacesAfterCount < 3)) 
                    nam = NameToken.TryParse(tokTyp.EndToken.Next, GeoTokenType.Org, 0, true);
                if ((nam == null && tokTyp.EndToken.Next != null && tokTyp.Chars.IsAllUpper) && tokTyp.EndToken.Next.IsHiphen && !tokTyp.IsWhitespaceAfter) 
                {
                    nam = NameToken.TryParse(tokTyp.EndToken.Next.Next, GeoTokenType.Org, 0, true);
                    if (nam != null) 
                    {
                        if (nam.Chars.IsAllLower || (nam.LengthChar < 4)) 
                            nam = null;
                    }
                }
                if (nam == null) 
                {
                    bool ok = false;
                    if (afterTerr && MiscLocationHelper.IsUserParamAddress(tokTyp)) 
                        ok = true;
                    else if (tokTyp.CanBeSingle) 
                        ok = true;
                    else if (tokTyp.BeginToken != tokTyp.EndToken) 
                    {
                        if (MiscLocationHelper.CheckGeoObjectBefore(tokTyp.BeginToken, false)) 
                            ok = true;
                        else if (MiscLocationHelper.IsUserParamAddress(tokTyp) && ((tokTyp.EndToken.IsNewlineAfter || tokTyp.EndToken.Next.IsComma))) 
                            ok = true;
                        else if (Pullenti.Ner.Address.Internal.AddressItemToken.CheckHouseAfter(tokTyp.EndToken.Next, false, false)) 
                            ok = true;
                    }
                    if (!ok) 
                        return null;
                    if (tokTyp.Vals[0].EndsWith("район")) 
                        return null;
                }
            }
            if (tokTyp.IsDoubt && ((nam == null || nam.IsDoubt || nam.Chars.IsAllUpper))) 
                return null;
            if (((tokTyp.LengthChar < 3) && nam != null && nam.Name == null) && nam.Pref == null) 
            {
                if (afterTerr || MiscLocationHelper.IsUserParamAddress(tokTyp)) 
                {
                }
                else 
                    return null;
            }
            if (((tokTyp.BeginToken.IsValue("СП", null) || tokTyp.BeginToken.IsValue("ГП", null))) && nam != null) 
            {
                Pullenti.Ner.Token tt = nam.EndToken.Next;
                if (tt != null && tt.IsComma) 
                    tt = tt.Next;
                if (Pullenti.Ner.Address.Internal.AddressItemToken.CheckHouseAfter(tt, false, false)) 
                {
                }
                else if (CityItemToken.CheckKeyword(tt) != null) 
                    return null;
            }
            Pullenti.Ner.Referent org = t.Kit.CreateReferent("ORGANIZATION");
            OrgItemToken res = new OrgItemToken(org, t, (nam != null ? nam.EndToken : tokTyp.EndToken));
            res.Data = t.Kit.GetAnalyzerDataByAnalyzerName("ORGANIZATION");
            res.HasTerrKeyword = afterTerr;
            res.IsDoubt = doubt0 || tokTyp.IsDoubt || nam == null;
            res.KeywordAfter = typAfter;
            res.NotOrg = tokTyp.NotOrg;
            res.NotGeo = tokTyp.NotGeo;
            if (tokTyp.CanBeSingle) 
            {
                org.AddSlot("NAME", tokTyp.Vals[0].ToUpper(), false, 0);
                res.IsDoubt = false;
                res.IsGsk = true;
                return res;
            }
            foreach (string ty in tokTyp.Vals) 
            {
                org.AddSlot("TYPE", ty, false, 0);
                if (ty == "поле") 
                    res.IsDoubt = true;
            }
            bool ignoreNext = false;
            if ((res.WhitespacesAfterCount < 3) && res.EndToken.Next != null) 
            {
                Pullenti.Ner.Token ttt = MiscLocationHelper.CheckTerritory(res.EndToken.Next);
                if (ttt != null && _TryParse(ttt.Next, true, lev + 1, ad) == null) 
                {
                    res.EndToken = ttt;
                    ignoreNext = true;
                }
                else if (nam != null) 
                {
                    OrgTypToken tokTyp2 = OrgTypToken.TryParse(res.EndToken.Next, false, null);
                    if (tokTyp2 != null) 
                    {
                        OrgItemToken rrr2 = _TryParse(res.EndToken.Next, false, lev + 1, ad);
                        if (rrr2 == null || rrr2.EndChar <= tokTyp2.EndChar) 
                        {
                            res.EndToken = tokTyp2.EndToken;
                            foreach (string ty in tokTyp2.Vals) 
                            {
                                org.AddSlot("TYPE", ty, false, 0);
                            }
                        }
                    }
                }
            }
            if (((res.WhitespacesAfterCount < 3) && nam != null && (res.EndToken.Next is Pullenti.Ner.TextToken)) && res.EndToken.Next.LengthChar == 1 && res.EndToken.Next.Chars.IsLetter) 
            {
                Pullenti.Ner.Token tt3 = res.EndToken.Next;
                if (((tt3.Next != null && tt3.Next.IsChar('.') && (tt3.Next.Next is Pullenti.Ner.TextToken)) && tt3.Next.Next.Chars.IsLetter && tt3.Next.Next.LengthChar == 1) && tt3.Next.Next.Next != null && tt3.Next.Next.Next.IsChar('.')) 
                    res.EndToken = tt3.Next.Next.Next;
            }
            if ((res.WhitespacesAfterCount < 3) && !tokTyp.NotOrg) 
            {
                Pullenti.Ner.Token tt = res.EndToken.Next;
                OrgItemToken next = _TryParse(tt, false, lev + 1, ad);
                if (next != null) 
                {
                    bool merge = true;
                    if (next.IsGsk) 
                    {
                        merge = false;
                        if ((nam != null && nam.Name != null && nam.Number == null) && next.Referent.FindSlot("NAME", null, true) == null && next.Referent.FindSlot("NUMBER", null, true) != null) 
                        {
                            foreach (string ty in org.GetStringValues("TYPE")) 
                            {
                                if (next.Referent.FindSlot("TYPE", ty, true) != null) 
                                    merge = true;
                            }
                        }
                    }
                    if (merge) 
                    {
                        res.EndToken = next.EndToken;
                        foreach (Pullenti.Ner.Slot s in next.Referent.Slots) 
                        {
                            res.Referent.AddSlot(s.TypeName, s.Value, false, 0);
                        }
                    }
                    ignoreNext = true;
                }
                else 
                {
                    if (tt != null && tt.IsValue("ПРИ", null)) 
                        tt = tt.Next;
                    Pullenti.Ner.ReferentToken rt = t.Kit.ProcessReferent("ORGANIZATION", tt, null);
                    if (rt != null) 
                    {
                    }
                    if (rt != null) 
                    {
                        res.EndToken = rt.EndToken;
                        Pullenti.Ner.Core.IntOntologyToken ter = TerrItemToken.CheckOntoItem(res.EndToken.Next);
                        if (ter != null) 
                            res.EndToken = ter.EndToken;
                        ignoreNext = true;
                    }
                }
            }
            string suffName = null;
            if (!ignoreNext && (res.WhitespacesAfterCount < 2) && !tokTyp.NotOrg) 
            {
                OrgTypToken tokTyp2 = OrgTypToken.TryParse(res.EndToken.Next, true, ad);
                if (tokTyp2 != null) 
                {
                    res.EndToken = tokTyp2.EndToken;
                    if (tokTyp2.IsDoubt && nam.Name != null) 
                        suffName = tokTyp2.Vals[0];
                    else 
                        foreach (string ty in tokTyp2.Vals) 
                        {
                            org.AddSlot("TYPE", ty, false, 0);
                        }
                    if (nam != null && nam.Number == null) 
                    {
                        NameToken nam2 = NameToken.TryParse(res.EndToken.Next, GeoTokenType.Org, 0, false);
                        if ((nam2 != null && nam2.Number != null && nam2.Name == null) && nam2.Pref == null) 
                        {
                            nam.Number = nam2.Number;
                            res.EndToken = nam2.EndToken;
                        }
                    }
                }
            }
            if (nam == null) 
            {
                res.SetGsk();
                return res;
            }
            if (nam != null && nam.Name != null) 
            {
                if (nam.Pref != null) 
                {
                    org.AddSlot("NAME", string.Format("{0} {1}", nam.Pref, nam.Name), false, 0);
                    if (suffName != null) 
                        org.AddSlot("NAME", string.Format("{0} {1} {2}", nam.Pref, nam.Name, suffName), false, 0);
                }
                else 
                {
                    org.AddSlot("NAME", nam.Name, false, 0);
                    if (suffName != null) 
                        org.AddSlot("NAME", string.Format("{0} {1}", nam.Name, suffName), false, 0);
                }
            }
            else if (nam.Pref != null) 
                org.AddSlot("NAME", nam.Pref, false, 0);
            else if (nam.Number != null && (res.WhitespacesAfterCount < 2)) 
            {
                NameToken nam2 = NameToken.TryParse(res.EndToken.Next, GeoTokenType.Org, 0, false);
                if (nam2 != null && nam2.Name != null && nam2.Number == null) 
                {
                    res.EndToken = nam2.EndToken;
                    org.AddSlot("NAME", nam2.Name, false, 0);
                }
            }
            if (nam.Number != null) 
                org.AddSlot("NUMBER", nam.Number, false, 0);
            else if (res.EndToken.Next != null && res.EndToken.Next.IsHiphen && (res.EndToken.Next.Next is Pullenti.Ner.NumberToken)) 
            {
                NameToken nam2 = NameToken.TryParse(res.EndToken.Next.Next, GeoTokenType.Org, 0, false);
                if (nam2 != null && nam2.Number != null && nam2.Name == null) 
                {
                    org.AddSlot("NUMBER", nam2.Number, false, 0);
                    res.EndToken = nam2.EndToken;
                }
            }
            bool ok1 = false;
            int cou = 0;
            for (Pullenti.Ner.Token tt = res.BeginToken; tt != null && tt.EndChar <= res.EndChar; tt = tt.Next) 
            {
                if ((tt is Pullenti.Ner.TextToken) && tt.LengthChar > 1) 
                {
                    if (nam != null && tt.BeginChar >= nam.BeginChar && tt.EndChar <= nam.EndChar) 
                    {
                        if (tokTyp != null && tt.BeginChar >= tokTyp.BeginChar && tt.EndChar <= tokTyp.EndChar) 
                        {
                        }
                        else 
                            cou++;
                    }
                    if (!tt.Chars.IsAllLower) 
                        ok1 = true;
                }
                else if (tt is Pullenti.Ner.ReferentToken) 
                    ok1 = true;
            }
            res.SetGsk();
            if (!ok1) 
            {
                if (!res.IsGsk && !res.HasTerrKeyword && !MiscLocationHelper.IsUserParamAddress(res)) 
                    return null;
            }
            if (cou > 4) 
                return null;
            if (tokTyp != null && tokTyp.BeginToken.IsValue("СП", null)) 
            {
                tt2 = res.EndToken.Next;
                if (tt2 != null && tt2.IsComma) 
                    tt2 = tt2.Next;
                List<CityItemToken> cits = CityItemToken.TryParseList(tt2, 3, null);
                if (cits != null && cits.Count == 2 && cits[0].Typ == CityItemToken.ItemType.Noun) 
                    return null;
            }
            if (tokTyp != null && tokTyp.BeginToken.IsValue("МАГАЗИН", null)) 
                return null;
            if (res.NotOrg && (res.WhitespacesAfterCount < 2)) 
            {
                Pullenti.Ner.Token tt = res.EndToken.Next;
                if ((tt is Pullenti.Ner.TextToken) && tt.LengthChar == 1 && ((tt.IsValue("П", null) || tt.IsValue("Д", null)))) 
                {
                    if (!Pullenti.Ner.Address.Internal.AddressItemToken.CheckHouseAfter(tt, false, false)) 
                    {
                        res.EndToken = res.EndToken.Next;
                        if (res.EndToken.Next != null && res.EndToken.Next.IsChar('.')) 
                            res.EndToken = res.EndToken.Next;
                    }
                }
            }
            if (tokTyp != null && tokTyp.Vals.Contains("лесничество") && MiscLocationHelper.IsUserParamAddress(tokTyp)) 
            {
                tt2 = tokTyp.EndToken.Next;
                if (tt2 != null && tt2.IsComma) 
                    tt2 = tt2.Next;
                Pullenti.Ner.Address.Internal.AddressItemToken ait = Pullenti.Ner.Address.Internal.AddressItemToken.TryParsePureItem(tt2, null, null);
                if (ait != null && ait.Typ == Pullenti.Ner.Address.Internal.AddressItemType.Flat && ait.Value != null) 
                {
                    org.AddSlot("NUMBER", ait.Value, false, 0);
                    res.EndToken = ait.EndToken;
                    for (tt2 = res.EndToken.Next; tt2 != null; tt2 = tt2.Next) 
                    {
                        if (!tt2.IsCommaAnd) 
                            break;
                        ait = Pullenti.Ner.Address.Internal.AddressItemToken.TryParsePureItem(tt2.Next, null, null);
                        if (ait == null || ait.Value == null) 
                            break;
                        if (ait.Typ != Pullenti.Ner.Address.Internal.AddressItemType.Number && ait.Typ != Pullenti.Ner.Address.Internal.AddressItemType.Flat) 
                            break;
                        org.AddSlot("NUMBER", ait.Value, false, 0);
                        res.EndToken = (tt2 = ait.EndToken);
                    }
                }
                else 
                {
                    NumToken nu = NumToken.TryParse(tt2, GeoTokenType.Org);
                    if (nu != null) 
                    {
                        org.AddSlot("NUMBER", nu.Value, false, 0);
                        res.EndToken = nu.EndToken;
                    }
                    else 
                    {
                        Pullenti.Ner.Address.Internal.StreetItemToken sit = Pullenti.Ner.Address.Internal.StreetItemToken.TryParse(tt2, null, false, null);
                        if (sit != null && sit.Typ == Pullenti.Ner.Address.Internal.StreetItemType.Noun && sit.Termin.CanonicText.Contains("КВАРТАЛ")) 
                        {
                            for (tt2 = sit.EndToken.Next; tt2 != null; tt2 = tt2.Next) 
                            {
                                NumToken num = NumToken.TryParse(tt2, GeoTokenType.Org);
                                if (num == null) 
                                    break;
                                org.AddSlot("NUMBER", num.Value, false, 0);
                                res.EndToken = num.EndToken;
                                tt2 = num.EndToken.Next;
                                if (tt2 == null) 
                                    break;
                                if (!tt2.IsCommaAnd) 
                                    break;
                            }
                        }
                    }
                }
            }
            return res;
        }
        public static Pullenti.Ner.Address.Internal.StreetItemToken TryParseRailway(Pullenti.Ner.Token t)
        {
            if (!(t is Pullenti.Ner.TextToken) || !t.Chars.IsLetter) 
                return null;
            if (t.IsValue("ДОРОГА", null) && (t.WhitespacesAfterCount < 3)) 
            {
                Pullenti.Ner.Address.Internal.StreetItemToken next = TryParseRailway(t.Next);
                if (next != null) 
                {
                    next.BeginToken = t;
                    return next;
                }
            }
            GeoAnalyzerData ad = Pullenti.Ner.Geo.GeoAnalyzer.GetData(t);
            if (ad == null) 
                return null;
            if (ad.OLevel > 0) 
                return null;
            ad.OLevel++;
            Pullenti.Ner.Address.Internal.StreetItemToken res = _tryParseRailway(t);
            ad.OLevel--;
            return res;
        }
        static Pullenti.Ner.ReferentToken _tryParseRailwayOrg(Pullenti.Ner.Token t)
        {
            if (t == null) 
                return null;
            int cou = 0;
            bool ok = false;
            for (Pullenti.Ner.Token tt = t; tt != null && (cou < 4); tt = tt.Next,cou++) 
            {
                if (tt is Pullenti.Ner.TextToken) 
                {
                    string val = (tt as Pullenti.Ner.TextToken).Term;
                    if (val == "Ж" || val.StartsWith("ЖЕЛЕЗ")) 
                    {
                        ok = true;
                        break;
                    }
                    if (Pullenti.Morph.LanguageHelper.EndsWith(val, "ЖД")) 
                    {
                        ok = true;
                        break;
                    }
                }
            }
            if (!ok) 
                return null;
            Pullenti.Ner.ReferentToken rt = t.Kit.ProcessReferent("ORGANIZATION", t, null);
            if (rt == null) 
                return null;
            foreach (string ty in rt.Referent.GetStringValues("TYPE")) 
            {
                if (ty.EndsWith("дорога")) 
                    return rt;
            }
            return null;
        }
        static Pullenti.Ner.Address.Internal.StreetItemToken _tryParseRailway(Pullenti.Ner.Token t)
        {
            Pullenti.Ner.ReferentToken rt0 = _tryParseRailwayOrg(t);
            if (rt0 != null) 
            {
                Pullenti.Ner.Address.Internal.StreetItemToken res = new Pullenti.Ner.Address.Internal.StreetItemToken(t, rt0.EndToken) { Typ = Pullenti.Ner.Address.Internal.StreetItemType.Fix, IsRailway = true };
                res.Value = rt0.Referent.GetStringValue("NAME");
                t = res.EndToken.Next;
                if (t != null && t.IsComma) 
                    t = t.Next;
                Pullenti.Ner.Address.Internal.StreetItemToken next = _tryParseRzdDir(t);
                if (next != null) 
                {
                    res.EndToken = next.EndToken;
                    res.Value = string.Format("{0} {1}", res.Value, next.Value);
                }
                else if ((t is Pullenti.Ner.TextToken) && t.Morph.Class.IsAdjective && !t.Chars.IsAllLower) 
                {
                    bool ok = false;
                    if (t.IsNewlineAfter || t.Next == null) 
                        ok = true;
                    else if (t.Next.IsCharOf(".,")) 
                        ok = true;
                    else if (Pullenti.Ner.Address.Internal.AddressItemToken.CheckHouseAfter(t.Next, false, false) || Pullenti.Ner.Address.Internal.AddressItemToken.CheckKmAfter(t.Next)) 
                        ok = true;
                    if (ok) 
                    {
                        res.Value = string.Format("{0} {1} НАПРАВЛЕНИЕ", res.Value, (t as Pullenti.Ner.TextToken).Term);
                        res.EndToken = t;
                    }
                }
                if (res.Value == "РОССИЙСКИЕ ЖЕЛЕЗНЫЕ ДОРОГИ") 
                    res.NounIsDoubtCoef = 2;
                return res;
            }
            Pullenti.Ner.Address.Internal.StreetItemToken dir = _tryParseRzdDir(t);
            if (dir != null && dir.NounIsDoubtCoef == 0) 
                return dir;
            return null;
        }
        static Pullenti.Ner.Address.Internal.StreetItemToken _tryParseRzdDir(Pullenti.Ner.Token t)
        {
            Pullenti.Ner.Token napr = null;
            Pullenti.Ner.Token tt0 = null;
            Pullenti.Ner.Token tt1 = null;
            string val = null;
            int cou = 0;
            for (Pullenti.Ner.Token tt = t; tt != null && (cou < 4); tt = tt.Next,cou++) 
            {
                if (tt.IsCharOf(",.")) 
                    continue;
                if (tt.IsNewlineBefore) 
                    break;
                if (tt.IsValue("НАПРАВЛЕНИЕ", null)) 
                {
                    napr = tt;
                    continue;
                }
                if (tt.IsValue("НАПР", null)) 
                {
                    if (tt.Next != null && tt.Next.IsChar('.')) 
                        tt = tt.Next;
                    napr = tt;
                    continue;
                }
                Pullenti.Ner.Core.NounPhraseToken npt = MiscLocationHelper.TryParseNpt(tt);
                if (npt != null && npt.Adjectives.Count > 0 && npt.Noun.IsValue("КОЛЬЦО", null)) 
                {
                    tt0 = tt;
                    tt1 = npt.EndToken;
                    val = npt.GetNormalCaseText(null, Pullenti.Morph.MorphNumber.Singular, Pullenti.Morph.MorphGender.Undefined, false);
                    break;
                }
                if ((tt is Pullenti.Ner.TextToken) && ((!tt.Chars.IsAllLower || napr != null)) && ((tt.Morph.Gender & Pullenti.Morph.MorphGender.Neuter)) != Pullenti.Morph.MorphGender.Undefined) 
                {
                    tt0 = (tt1 = tt);
                    continue;
                }
                if ((((tt is Pullenti.Ner.TextToken) && ((!tt.Chars.IsAllLower || napr != null)) && tt.Next != null) && tt.Next.IsHiphen && (tt.Next.Next is Pullenti.Ner.TextToken)) && ((tt.Next.Next.Morph.Gender & Pullenti.Morph.MorphGender.Neuter)) != Pullenti.Morph.MorphGender.Undefined) 
                {
                    tt0 = tt;
                    tt = tt.Next.Next;
                    tt1 = tt;
                    continue;
                }
                break;
            }
            if (tt0 == null) 
                return null;
            Pullenti.Ner.Address.Internal.StreetItemToken res = new Pullenti.Ner.Address.Internal.StreetItemToken(tt0, tt1) { Typ = Pullenti.Ner.Address.Internal.StreetItemType.Fix, IsRailway = true, NounIsDoubtCoef = 1 };
            if (val != null) 
                res.Value = val;
            else 
            {
                res.Value = tt1.GetNormalCaseText(Pullenti.Morph.MorphClass.Adjective, Pullenti.Morph.MorphNumber.Singular, Pullenti.Morph.MorphGender.Neuter, false);
                if (tt0 != tt1) 
                    res.Value = string.Format("{0} {1}", (tt0 as Pullenti.Ner.TextToken).Term, res.Value);
                res.Value += " НАПРАВЛЕНИЕ";
            }
            if (napr != null && napr.EndChar > res.EndChar) 
                res.EndToken = napr;
            t = res.EndToken.Next;
            if (t != null && t.IsComma) 
                t = t.Next;
            if (t != null) 
            {
                Pullenti.Ner.ReferentToken rt0 = _tryParseRailwayOrg(t);
                if (rt0 != null) 
                {
                    res.Value = string.Format("{0} {1}", rt0.Referent.GetStringValue("NAME"), res.Value);
                    res.EndToken = rt0.EndToken;
                    res.NounIsDoubtCoef = 0;
                }
            }
            return res;
        }
        void _tryParseDetails()
        {
            if (WhitespacesAfterCount > 2) 
                return;
            Pullenti.Ner.Token t = EndToken.Next;
            if (t == null) 
                return;
            Pullenti.Ner.Core.TerminToken tok = m_Onto.TryParse(t, Pullenti.Ner.Core.TerminParseAttr.No);
            if (tok != null) 
            {
                t = tok.EndToken.Next;
                List<CityItemToken> cits = CityItemToken.TryParseList(t, 5, null);
                if (cits != null && cits.Count > 1) 
                {
                    Pullenti.Ner.ReferentToken rt = CityAttachHelper.TryDefine(cits, null, false);
                    if (rt != null) 
                    {
                        EndToken = rt.EndToken;
                        t = EndToken.Next;
                        this._mergeWith(rt.Referent);
                    }
                }
                Pullenti.Ner.Address.Internal.AddressItemToken ait = Pullenti.Ner.Address.Internal.AddressItemToken.TryParse(t, false, null, null);
                if (ait != null && ait.Typ == Pullenti.Ner.Address.Internal.AddressItemType.Street && ait.Referent != null) 
                {
                    EndToken = ait.EndToken;
                    t = EndToken.Next;
                    this._mergeWith(ait.Referent);
                    ait = Pullenti.Ner.Address.Internal.AddressItemToken.TryParse(t, false, null, null);
                    if (ait != null && ait.Typ == Pullenti.Ner.Address.Internal.AddressItemType.House && ait.Value != null) 
                    {
                        Referent.AddSlot("NUMBER", ait.Value, false, 0);
                        EndToken = ait.EndToken;
                        t = EndToken.Next;
                    }
                }
                else if ((ait != null && ait.Typ == Pullenti.Ner.Address.Internal.AddressItemType.House && ait.HouseType != Pullenti.Ner.Address.AddressHouseType.Special) && ait.Value != null) 
                {
                    Referent.AddSlot("NUMBER", ait.Value, false, 0);
                    EndToken = ait.EndToken;
                    t = EndToken.Next;
                }
                if (t == tok.EndToken.Next) 
                {
                    if (t == null || tok.IsNewlineAfter) 
                    {
                        EndToken = tok.EndToken;
                        return;
                    }
                    NameToken name = NameToken.TryParse(t, GeoTokenType.Org, 0, false);
                    if (name != null && name.Name != null && Referent.FindSlot("NAME", null, true) == null) 
                    {
                        Referent.AddSlot("NAME", name.Name, false, 0);
                        if (name.Number != null) 
                            Referent.AddSlot("NUMBER", name.Number, false, 0);
                        EndToken = name.EndToken;
                        t = EndToken.Next;
                    }
                }
            }
            else 
            {
                bool isGaraz = false;
                foreach (Pullenti.Ner.Slot s in Referent.Slots) 
                {
                    if (s.TypeName == "TYPE" && (s.Value is string)) 
                    {
                        string ty = s.Value as string;
                        if (ty.Contains("гараж") || ty.Contains("автомоб")) 
                        {
                            isGaraz = true;
                            break;
                        }
                    }
                }
                if (isGaraz) 
                {
                    Pullenti.Ner.Address.Internal.AddressItemToken ait = Pullenti.Ner.Address.Internal.AddressItemToken.TryParse(t, false, null, null);
                    if (ait != null && ait.Typ == Pullenti.Ner.Address.Internal.AddressItemType.Street && ait.Referent != null) 
                    {
                    }
                    else if (ait != null && ait.Typ == Pullenti.Ner.Address.Internal.AddressItemType.House && ait.Value != null) 
                    {
                        Referent.AddSlot("NUMBER", ait.Value, false, 0);
                        EndToken = ait.EndToken;
                        t = EndToken.Next;
                    }
                    else if (ait != null && ait.DetailType == Pullenti.Ner.Address.AddressDetailType.Near) 
                    {
                        Pullenti.Ner.Address.Internal.AddressItemToken ait2 = Pullenti.Ner.Address.Internal.AddressItemToken.TryParse(ait.EndToken.Next, false, null, null);
                        if (ait2 != null && ait2.Typ == Pullenti.Ner.Address.Internal.AddressItemType.House && ait2.Value != null) 
                        {
                            Referent.AddSlot("NUMBER", ait2.Value, false, 0);
                            EndToken = ait2.EndToken;
                            t = EndToken.Next;
                        }
                    }
                }
            }
        }
        void _mergeWith(Pullenti.Ner.Referent r)
        {
            List<string> names = Referent.GetStringValues("NAME");
            foreach (string n in r.GetStringValues("NAME")) 
            {
                if (names.Count > 0) 
                {
                    foreach (string n0 in names) 
                    {
                        Referent.AddSlot("NAME", string.Format("{0} {1}", n0, n), false, 0);
                    }
                }
                else 
                    Referent.AddSlot("NAME", n, false, 0);
            }
            foreach (string n in r.GetStringValues("NUMBER")) 
            {
                Referent.AddSlot("NUMBER", n, false, 0);
            }
            foreach (string n in r.GetStringValues("MISC")) 
            {
                Referent.AddSlot("MISC", n, false, 0);
            }
        }
        internal static Pullenti.Ner.Core.TerminCollection m_Onto;
        public static void Initialize()
        {
            m_Onto = new Pullenti.Ner.Core.TerminCollection();
            Pullenti.Ner.Core.Termin t;
            t = new Pullenti.Ner.Core.Termin("В РАЙОНЕ");
            t.AddAbridge("В Р-НЕ");
            m_Onto.Add(t);
            t = new Pullenti.Ner.Core.Termin("РАЙОН");
            t.AddAbridge("Р-Н");
            t.AddAbridge("Р-ОН");
            m_Onto.Add(t);
            m_Onto.Add(new Pullenti.Ner.Core.Termin("ПО"));
            m_Onto.Add(new Pullenti.Ner.Core.Termin("ОКОЛО"));
            m_Onto.Add(new Pullenti.Ner.Core.Termin("ВО ДВОРЕ"));
        }
    }
}