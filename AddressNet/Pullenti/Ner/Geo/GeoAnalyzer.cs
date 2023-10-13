﻿/*
 * SDK Pullenti Address, version 4.20, august 2023. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Pullenti.Ner.Geo
{
    /// <summary>
    /// Анализатор географических объектов (стран, регионов, населённых пунктов)
    /// </summary>
    public class GeoAnalyzer : Pullenti.Ner.Analyzer
    {
        /// <summary>
        /// Имя анализатора ("GEO")
        /// </summary>
        public const string ANALYZER_NAME = "GEO";
        public override string Name
        {
            get
            {
                return ANALYZER_NAME;
            }
        }
        public override string Caption
        {
            get
            {
                return "Страны, регионы, города";
            }
        }
        public override Pullenti.Ner.Analyzer Clone()
        {
            return new GeoAnalyzer();
        }
        public override ICollection<Pullenti.Ner.Metadata.ReferentClass> TypeSystem
        {
            get
            {
                return new Pullenti.Ner.Metadata.ReferentClass[] {Pullenti.Ner.Geo.Internal.MetaGeo.GlobalMeta};
            }
        }
        public override IEnumerable<string> UsedExternObjectTypes
        {
            get
            {
                return new string[] {"PHONE"};
            }
        }
        internal static Pullenti.Ner.Geo.Internal.GeoAnalyzerData GetData(Pullenti.Ner.Token t)
        {
            if (t == null) 
                return null;
            return t.Kit.GetAnalyzerDataByAnalyzerName(ANALYZER_NAME) as Pullenti.Ner.Geo.Internal.GeoAnalyzerData;
        }
        public override Dictionary<string, byte[]> Images
        {
            get
            {
                Dictionary<string, byte[]> res = new Dictionary<string, byte[]>();
                res.Add(Pullenti.Ner.Geo.Internal.MetaGeo.CountryCityImageId, Pullenti.Ner.Core.Internal.ResourceHelper.GetBytes("countrycity.png"));
                res.Add(Pullenti.Ner.Geo.Internal.MetaGeo.CountryImageId, Pullenti.Ner.Core.Internal.ResourceHelper.GetBytes("country.png"));
                res.Add(Pullenti.Ner.Geo.Internal.MetaGeo.CityImageId, Pullenti.Ner.Core.Internal.ResourceHelper.GetBytes("city.png"));
                res.Add(Pullenti.Ner.Geo.Internal.MetaGeo.DistrictImageId, Pullenti.Ner.Core.Internal.ResourceHelper.GetBytes("district.png"));
                res.Add(Pullenti.Ner.Geo.Internal.MetaGeo.RegionImageId, Pullenti.Ner.Core.Internal.ResourceHelper.GetBytes("region.png"));
                res.Add(Pullenti.Ner.Geo.Internal.MetaGeo.UnionImageId, Pullenti.Ner.Core.Internal.ResourceHelper.GetBytes("union.png"));
                return res;
            }
        }
        public override Pullenti.Ner.Referent CreateReferent(string type)
        {
            if (type == GeoReferent.OBJ_TYPENAME) 
                return new GeoReferent();
            return null;
        }
        public override int ProgressWeight
        {
            get
            {
                return 15;
            }
        }
        public override Pullenti.Ner.Core.AnalyzerData CreateAnalyzerData()
        {
            return new Pullenti.Ner.Geo.Internal.GeoAnalyzerData();
        }
        public override void Process(Pullenti.Ner.Core.AnalysisKit kit)
        {
            Pullenti.Ner.Geo.Internal.GeoAnalyzerData ad = kit.GetAnalyzerData(this) as Pullenti.Ner.Geo.Internal.GeoAnalyzerData;
            for (Pullenti.Ner.Token t = kit.FirstToken; t != null; t = t.Next) 
            {
                t.InnerBool = false;
            }
            Stopwatch sw = new Stopwatch();
            sw.Reset();
            sw.Start();
            Pullenti.Ner.Geo.Internal.MiscLocationHelper.PrepareAllData(kit.FirstToken);
            sw.Stop();
            kit.Msgs.Add(string.Format("Npt: {0}ms", sw.ElapsedMilliseconds));
            if (!this.OnProgress(10, 100, kit)) 
                return;
            sw.Reset();
            sw.Start();
            Pullenti.Ner.Address.Internal.AddressItemToken.SpeedRegime = true;
            Pullenti.Ner.Address.Internal.AddressItemToken.PrepareAllData(kit.FirstToken);
            sw.Stop();
            kit.Msgs.Add(string.Format("AddressItemToken: {0}ms", sw.ElapsedMilliseconds));
            if (!this.OnProgress(20, 100, kit)) 
                return;
            sw.Reset();
            sw.Start();
            Pullenti.Ner.Geo.Internal.OrgTypToken.SpeedRegime = true;
            Pullenti.Ner.Geo.Internal.OrgTypToken.PrepareAllData(kit.FirstToken);
            sw.Stop();
            kit.Msgs.Add(string.Format("OrgTypToken: {0}ms", sw.ElapsedMilliseconds));
            if (!this.OnProgress(30, 100, kit)) 
                return;
            sw.Reset();
            sw.Start();
            Pullenti.Ner.Geo.Internal.OrgItemToken.SpeedRegime = true;
            Pullenti.Ner.Geo.Internal.OrgItemToken.PrepareAllData(kit.FirstToken);
            sw.Stop();
            kit.Msgs.Add(string.Format("OrgItemToken: {0}ms", sw.ElapsedMilliseconds));
            if (!this.OnProgress(40, 100, kit)) 
                return;
            sw.Reset();
            sw.Start();
            Pullenti.Ner.Address.Internal.StreetItemToken.SpeedRegime = true;
            Pullenti.Ner.Address.Internal.StreetItemToken.PrepareAllData(kit.FirstToken);
            sw.Stop();
            kit.Msgs.Add(string.Format("StreetItemToken: {0}ms", sw.ElapsedMilliseconds));
            if (!this.OnProgress(60, 100, kit)) 
                return;
            sw.Reset();
            sw.Start();
            Pullenti.Ner.Geo.Internal.TerrItemToken.SpeedRegime = true;
            Pullenti.Ner.Geo.Internal.TerrItemToken.PrepareAllData(kit.FirstToken);
            sw.Stop();
            kit.Msgs.Add(string.Format("TerrItemToken: {0}ms", sw.ElapsedMilliseconds));
            if (!this.OnProgress(65, 100, kit)) 
                return;
            sw.Reset();
            sw.Start();
            Pullenti.Ner.Geo.Internal.CityItemToken.SpeedRegime = true;
            Pullenti.Ner.Geo.Internal.CityItemToken.PrepareAllData(kit.FirstToken);
            sw.Stop();
            kit.Msgs.Add(string.Format("CityItemToken: {0}ms", sw.ElapsedMilliseconds));
            if (!this.OnProgress(85, 100, kit)) 
                return;
            sw.Reset();
            sw.Start();
            List<GeoReferent> nonRegistered = new List<GeoReferent>();
            for (int step = 0; step < 2; step++) 
            {
                ad.Step = step;
                for (Pullenti.Ner.Token t = kit.FirstToken; t != null; t = t.Next) 
                {
                    if (t.IsIgnored) 
                        continue;
                    if (ad.Referents.Count >= 2000) 
                        break;
                    if (step > 0 && (t is Pullenti.Ner.ReferentToken)) 
                    {
                        GeoReferent geo = t.GetReferent() as GeoReferent;
                        if (((geo != null && t.Next != null && t.Next.IsChar('(')) && t.Next.Next != null && geo.CanBeEquals(t.Next.Next.GetReferent(), Pullenti.Ner.Core.ReferentsEqualType.WithinOneText)) && t.Next.Next.Next != null && t.Next.Next.Next.IsChar(')')) 
                        {
                            Pullenti.Ner.ReferentToken rt0 = new Pullenti.Ner.ReferentToken(geo, t, t.Next.Next.Next) { Morph = t.Morph };
                            kit.EmbedToken(rt0);
                            t = rt0;
                            continue;
                        }
                        if ((geo != null && t.Next != null && t.Next.IsHiphen) && t.Next.Next != null && geo.CanBeEquals(t.Next.Next.GetReferent(), Pullenti.Ner.Core.ReferentsEqualType.WithinOneText)) 
                        {
                            Pullenti.Ner.ReferentToken rt0 = new Pullenti.Ner.ReferentToken(geo, t, t.Next.Next) { Morph = t.Morph };
                            kit.EmbedToken(rt0);
                            t = rt0;
                            continue;
                        }
                    }
                    bool ok = false;
                    if (step == 0 || t.InnerBool) 
                        ok = true;
                    else if ((t is Pullenti.Ner.TextToken) && t.Chars.IsLetter && !t.Chars.IsAllLower) 
                        ok = true;
                    List<Pullenti.Ner.Geo.Internal.TerrItemToken> cli = null;
                    if (ok) 
                        cli = Pullenti.Ner.Geo.Internal.TerrItemToken.TryParseList(t, 5, ad);
                    if (cli == null) 
                        continue;
                    t.InnerBool = true;
                    Pullenti.Ner.ReferentToken rt = Pullenti.Ner.Geo.Internal.TerrDefineHelper.TryDefine(cli, ad, false, null, nonRegistered);
                    if ((rt == null && cli.Count == 1 && cli[0].IsAdjective) && cli[0].OntoItem != null) 
                    {
                        Pullenti.Ner.Token tt = cli[0].EndToken.Next;
                        if (tt != null) 
                        {
                            if (tt.IsChar(',')) 
                                tt = tt.Next;
                            else if (tt.Morph.Class.IsConjunction) 
                            {
                                tt = tt.Next;
                                if (tt != null && tt.Morph.Class.IsConjunction) 
                                    tt = tt.Next;
                            }
                            List<Pullenti.Ner.Geo.Internal.TerrItemToken> cli1 = Pullenti.Ner.Geo.Internal.TerrItemToken.TryParseList(tt, 2, null);
                            if (cli1 != null && cli1[0].OntoItem != null) 
                            {
                                GeoReferent g0 = cli[0].OntoItem.Referent as GeoReferent;
                                GeoReferent g1 = cli1[0].OntoItem.Referent as GeoReferent;
                                if ((g0 != null && g1 != null && g0.IsRegion) && g1.IsRegion) 
                                {
                                    if (g0.IsCity == g1.IsCity || g0.IsRegion == g1.IsRegion || g0.IsState == g1.IsState) 
                                        rt = Pullenti.Ner.Geo.Internal.TerrDefineHelper.TryDefine(cli, ad, true, null, null);
                                }
                            }
                            if (rt == null && (cli[0].OntoItem.Referent as GeoReferent).IsState) 
                            {
                                if ((rt == null && tt != null && (tt.GetReferent() is GeoReferent)) && tt.WhitespacesBeforeCount == 1) 
                                {
                                    GeoReferent geo2 = tt.GetReferent() as GeoReferent;
                                    if (Pullenti.Ner.Geo.Internal.GeoOwnerHelper.CanBeHigher(cli[0].OntoItem.Referent as GeoReferent, geo2, null, null)) 
                                    {
                                        Pullenti.Ner.Referent cl = cli[0].OntoItem.Referent.Clone();
                                        cl.Occurrence.Clear();
                                        rt = new Pullenti.Ner.ReferentToken(cl, cli[0].BeginToken, cli[0].EndToken) { Morph = cli[0].Morph };
                                    }
                                }
                                if (rt == null && step == 0) 
                                {
                                    Pullenti.Ner.Core.NounPhraseToken npt = Pullenti.Ner.Geo.Internal.MiscLocationHelper.TryParseNpt(cli[0].BeginToken);
                                    if (npt != null && npt.EndChar >= tt.BeginChar) 
                                    {
                                        List<Pullenti.Ner.Geo.Internal.CityItemToken> cits = Pullenti.Ner.Geo.Internal.CityItemToken.TryParseList(tt, 5, ad);
                                        Pullenti.Ner.ReferentToken rt1 = (cits == null ? null : Pullenti.Ner.Geo.Internal.CityAttachHelper.TryDefine(cits, ad, false));
                                        if (rt1 != null) 
                                        {
                                            rt1.Referent = ad.RegisterReferent(rt1.Referent);
                                            kit.EmbedToken(rt1);
                                            Pullenti.Ner.Referent cl = cli[0].OntoItem.Referent.Clone();
                                            cl.Occurrence.Clear();
                                            rt = new Pullenti.Ner.ReferentToken(cl, cli[0].BeginToken, cli[0].EndToken) { Morph = cli[0].Morph };
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (rt == null) 
                    {
                        List<Pullenti.Ner.Geo.Internal.CityItemToken> cits = this.TryParseCityListBack(t.Previous);
                        if (cits != null) 
                            rt = Pullenti.Ner.Geo.Internal.TerrDefineHelper.TryDefine(cli, ad, false, cits, null);
                    }
                    if (rt == null && cli.Count == 2) 
                    {
                        Pullenti.Ner.Token te = cli[cli.Count - 1].EndToken.Next;
                        if (te != null) 
                        {
                            if (te.Morph.Class.IsPreposition || te.IsChar(',')) 
                                te = te.Next;
                        }
                        List<Pullenti.Ner.Address.Internal.AddressItemToken> li = Pullenti.Ner.Address.Internal.AddressItemToken.TryParseList(te, 2);
                        if (li != null && li.Count > 0) 
                        {
                            if (li[0].Typ == Pullenti.Ner.Address.Internal.AddressItemType.Street || li[0].Typ == Pullenti.Ner.Address.Internal.AddressItemType.Kilometer || li[0].Typ == Pullenti.Ner.Address.Internal.AddressItemType.House) 
                            {
                                Pullenti.Ner.Address.Internal.StreetItemToken ad0 = Pullenti.Ner.Address.Internal.StreetItemToken.TryParse(cli[0].BeginToken.Previous, null, false, null);
                                if (ad0 != null && ad0.Typ == Pullenti.Ner.Address.Internal.StreetItemType.Noun) 
                                {
                                }
                                else if (!cli[0].IsAdjective) 
                                    rt = Pullenti.Ner.Geo.Internal.TerrDefineHelper.TryDefine(cli, ad, true, null, null);
                                else 
                                {
                                    Pullenti.Ner.Address.Internal.AddressItemToken aaa = Pullenti.Ner.Address.Internal.AddressItemToken.TryParse(cli[0].BeginToken, false, null, null);
                                    if (aaa != null && aaa.Typ == Pullenti.Ner.Address.Internal.AddressItemType.Street) 
                                    {
                                    }
                                    else 
                                        rt = Pullenti.Ner.Geo.Internal.TerrDefineHelper.TryDefine(cli, ad, true, null, null);
                                }
                            }
                        }
                    }
                    if ((rt == null && cli.Count > 2 && cli[0].TerminItem == null) && cli[1].TerminItem == null && cli[2].TerminItem != null) 
                    {
                        Pullenti.Ner.Geo.Internal.CityItemToken cit = Pullenti.Ner.Geo.Internal.CityItemToken.TryParseBack(cli[0].BeginToken.Previous, false);
                        if (cit != null && cit.Typ == Pullenti.Ner.Geo.Internal.CityItemToken.ItemType.Noun) 
                        {
                            if (((cli.Count > 4 && cli[1].TerminItem == null && cli[2].TerminItem != null) && cli[3].TerminItem == null && cli[4].TerminItem != null) && cli[2].TerminItem.CanonicText.EndsWith(cli[4].TerminItem.CanonicText)) 
                            {
                            }
                            else 
                            {
                                cli.RemoveAt(0);
                                rt = Pullenti.Ner.Geo.Internal.TerrDefineHelper.TryDefine(cli, ad, true, null, null);
                            }
                        }
                    }
                    if (rt != null) 
                    {
                        if (Pullenti.Ner.Geo.Internal.MiscLocationHelper.CheckTerritory(rt.BeginToken.Previous) != null) 
                        {
                            if (!rt.BeginToken.Previous.IsValue("ГРАНИЦА", null)) 
                                rt.BeginToken = rt.BeginToken.Previous;
                        }
                        GeoReferent geo = rt.Referent as GeoReferent;
                        if ((!Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(rt) && !geo.IsCity && !geo.IsState) && geo.FindSlot(GeoReferent.ATTR_TYPE, "республика", true) == null) 
                            nonRegistered.Add(geo);
                        else 
                            rt.Referent = ad.RegisterReferent(geo);
                        Pullenti.Ner.Token tt2 = rt.BeginToken.Previous;
                        if (tt2 != null && tt2.IsComma) 
                            tt2 = tt2.Previous;
                        if (tt2 != null && tt2.GetReferent() == rt.Referent) 
                        {
                            rt.BeginToken = tt2;
                            tt2 = rt.EndToken.Next;
                            if (tt2 != null && tt2.IsChar(')')) 
                            {
                                if (rt.GetSourceText().IndexOf('(') >= 0) 
                                    rt.EndToken = tt2;
                            }
                        }
                        kit.EmbedToken(rt);
                        t = rt;
                        if (step == 0) 
                        {
                            Pullenti.Ner.Token tt = t;
                            while (true) 
                            {
                                Pullenti.Ner.ReferentToken rr = this.TryAttachTerritoryBeforeCity(tt, ad);
                                if (rr == null) 
                                    break;
                                geo = rr.Referent as GeoReferent;
                                if (!geo.IsCity && !geo.IsState) 
                                    nonRegistered.Add(geo);
                                else 
                                    rr.Referent = ad.RegisterReferent(geo);
                                kit.EmbedToken(rr);
                                tt = rr;
                            }
                            if (t.Next != null && ((t.Next.IsComma || t.Next.IsChar('(')))) 
                            {
                                Pullenti.Ner.ReferentToken rt1 = Pullenti.Ner.Geo.Internal.TerrDefineHelper.TryAttachStateUSATerritory(t.Next.Next);
                                if (rt1 != null) 
                                {
                                    rt1.Referent = ad.RegisterReferent(rt1.Referent);
                                    kit.EmbedToken(rt1);
                                    t = rt1;
                                }
                            }
                        }
                        continue;
                    }
                }
            }
            for (Pullenti.Ner.Token t = kit.FirstToken; t != null; t = (t == null ? null : t.Next)) 
            {
                if (t.IsIgnored) 
                    continue;
                GeoReferent g = t.GetReferent() as GeoReferent;
                if (g == null) 
                    continue;
                if (t.Next != null && t.Next.IsCharOf("(/\\") && (t.Next.Next is Pullenti.Ner.ReferentToken)) 
                {
                    GeoReferent g2 = t.Next.Next.GetReferent() as GeoReferent;
                    if (g2 != null && g2 == g) 
                    {
                        Pullenti.Ner.ReferentToken rt2 = new Pullenti.Ner.ReferentToken(g, t, t.Next.Next);
                        if (rt2.EndToken.Next != null && rt2.EndToken.Next.IsCharOf(")/\\")) 
                            rt2.EndToken = rt2.EndToken.Next;
                        t.Kit.EmbedToken(rt2);
                        t = rt2;
                    }
                }
                if (!(t.Previous is Pullenti.Ner.TextToken)) 
                    continue;
                Pullenti.Ner.Token t0 = null;
                if (t.Previous.IsValue("СОЮЗ", null)) 
                    t0 = t.Previous;
                else if (t.Previous.IsValue("ГОСУДАРСТВО", null) && t.Previous.Previous != null && t.Previous.Previous.IsValue("СОЮЗНЫЙ", null)) 
                    t0 = t.Previous.Previous;
                if (t0 == null) 
                    continue;
                Pullenti.Ner.Core.NounPhraseToken npt = Pullenti.Ner.Geo.Internal.MiscLocationHelper.TryParseNpt(t0.Previous);
                if (npt != null && npt.EndToken == t.Previous) 
                    t0 = t0.Previous;
                GeoReferent uni = new GeoReferent();
                string typ = Pullenti.Ner.Core.MiscHelper.GetTextValue(t0, t.Previous, Pullenti.Ner.Core.GetTextAttr.FirstNounGroupToNominative);
                if (typ == null) 
                    continue;
                uni.AddTypUnion(t0.Kit.BaseLanguage);
                uni.AddTyp(typ.ToLower());
                uni.AddSlot(GeoReferent.ATTR_REF, g, false, 0);
                Pullenti.Ner.Token t1 = t;
                int i = 1;
                for (t = t.Next; t != null; t = t.Next) 
                {
                    if (t.IsCommaAnd) 
                        continue;
                    if ((((g = t.GetReferent() as GeoReferent))) == null) 
                        break;
                    if (uni.FindSlot(GeoReferent.ATTR_REF, g, true) != null) 
                        break;
                    if (t.IsNewlineBefore) 
                        break;
                    t1 = t;
                    uni.AddSlot(GeoReferent.ATTR_REF, g, false, 0);
                    i++;
                }
                if (i < 2) 
                    continue;
                uni = ad.RegisterReferent(uni) as GeoReferent;
                Pullenti.Ner.ReferentToken rt = new Pullenti.Ner.ReferentToken(uni, t0, t1);
                kit.EmbedToken(rt);
                t = rt;
            }
            sw.Stop();
            kit.Msgs.Add(string.Format("Territories: {0}ms", sw.ElapsedMilliseconds));
            if (!this.OnProgress(90, 100, kit)) 
                return;
            sw.Reset();
            sw.Start();
            bool newCities = false;
            bool isCityBefore = false;
            for (Pullenti.Ner.Token t = kit.FirstToken; t != null; t = t.Next) 
            {
                if (t.IsIgnored) 
                    continue;
                if (t.IsCharOf(".,")) 
                    continue;
                if (t.IsValue("ГЛАВА", null)) 
                {
                }
                List<Pullenti.Ner.Geo.Internal.CityItemToken> li = null;
                li = Pullenti.Ner.Geo.Internal.CityItemToken.TryParseList(t, 5, ad);
                Pullenti.Ner.ReferentToken rt;
                if (li != null) 
                {
                    if ((((rt = Pullenti.Ner.Geo.Internal.CityAttachHelper.TryDefine(li, ad, false)))) != null) 
                    {
                        Pullenti.Ner.Token tt = t.Previous;
                        if (tt != null && tt.IsComma) 
                            tt = tt.Previous;
                        if (tt != null && (tt.GetReferent() is GeoReferent)) 
                        {
                            if (tt.GetReferent().CanBeEquals(rt.Referent, Pullenti.Ner.Core.ReferentsEqualType.WithinOneText)) 
                            {
                                rt.BeginToken = tt;
                                rt.Referent = ad.RegisterReferent(rt.Referent);
                                kit.EmbedToken(rt);
                                t = rt;
                                continue;
                            }
                        }
                        if (ad.Referents.Count > 2000) 
                            break;
                        if (li.Count == 2 && li[0].OrtoCity != null) 
                        {
                            li[0].OrtoCity.Referent = ad.RegisterReferent(li[0].OrtoCity.Referent);
                            Pullenti.Ner.ReferentToken rt1 = new Pullenti.Ner.ReferentToken(li[0].OrtoCity.Referent, li[0].BeginToken, li[1].BeginToken.Previous);
                            kit.EmbedToken(rt1);
                            rt.BeginToken = li[1].BeginToken;
                            rt.EndToken = li[1].EndToken;
                        }
                        rt.Referent = ad.RegisterReferent(rt.Referent) as GeoReferent;
                        kit.EmbedToken(rt);
                        t = rt;
                        isCityBefore = true;
                        newCities = true;
                        tt = t;
                        while (true) 
                        {
                            Pullenti.Ner.ReferentToken rr = this.TryAttachTerritoryBeforeCity(tt, ad);
                            if (rr == null) 
                                break;
                            GeoReferent geo = rr.Referent as GeoReferent;
                            if (!geo.IsCity && !geo.IsState) 
                                nonRegistered.Add(geo);
                            else 
                                rr.Referent = ad.RegisterReferent(geo);
                            kit.EmbedToken(rr);
                            tt = rr;
                        }
                        rt = this.TryAttachTerritoryAfterCity(t, ad);
                        if (rt != null) 
                        {
                            rt.Referent = ad.RegisterReferent(rt.Referent);
                            kit.EmbedToken(rt);
                            t = rt;
                        }
                        continue;
                    }
                }
                if (!t.InnerBool) 
                {
                    isCityBefore = false;
                    continue;
                }
                if (!isCityBefore) 
                    continue;
                List<Pullenti.Ner.Geo.Internal.TerrItemToken> tts = Pullenti.Ner.Geo.Internal.TerrItemToken.TryParseList(t, 5, null);
                if (tts != null && tts.Count > 1 && ((tts[0].TerminItem != null || tts[1].TerminItem != null))) 
                {
                    if ((((rt = Pullenti.Ner.Geo.Internal.TerrDefineHelper.TryDefine(tts, ad, true, null, null)))) != null) 
                    {
                        GeoReferent geo = rt.Referent as GeoReferent;
                        if (!geo.IsCity && !geo.IsState) 
                            nonRegistered.Add(geo);
                        else 
                            rt.Referent = ad.RegisterReferent(geo);
                        kit.EmbedToken(rt);
                        t = rt;
                        continue;
                    }
                }
                isCityBefore = false;
            }
            sw.Stop();
            kit.Msgs.Add(string.Format("Cities: {0}ms", sw.ElapsedMilliseconds));
            sw.Reset();
            sw.Start();
            if (newCities && ad.LocalOntology.Items.Count > 0) 
            {
                for (Pullenti.Ner.Token t = kit.FirstToken; t != null; t = t.Next) 
                {
                    if (t.IsIgnored) 
                        continue;
                    if (!(t is Pullenti.Ner.TextToken)) 
                        continue;
                    if (t.Chars.IsAllLower) 
                        continue;
                    List<Pullenti.Ner.Core.IntOntologyToken> li = ad.LocalOntology.TryAttach(t, null, false);
                    if (li == null) 
                        continue;
                    Pullenti.Morph.MorphClass mc = t.GetMorphClassInDictionary();
                    if (mc.IsProperSurname || mc.IsProperName || mc.IsProperSecname) 
                        continue;
                    if (t.Morph.Class.IsAdjective) 
                        continue;
                    GeoReferent geo = li[0].Item.Referent as GeoReferent;
                    if (geo != null) 
                    {
                        geo = geo.Clone() as GeoReferent;
                        geo.Occurrence.Clear();
                        Pullenti.Ner.ReferentToken rt = new Pullenti.Ner.ReferentToken(geo, li[0].BeginToken, li[0].EndToken) { Morph = t.Morph };
                        if (rt.BeginToken == rt.EndToken) 
                            geo.AddName((t as Pullenti.Ner.TextToken).Term);
                        if (rt.BeginToken.Previous != null && rt.BeginToken.Previous.IsValue("СЕЛО", null) && geo.IsCity) 
                        {
                            rt.BeginToken = rt.BeginToken.Previous;
                            rt.Morph = rt.BeginToken.Morph;
                            geo.AddSlot(GeoReferent.ATTR_TYPE, "село", true, 0);
                        }
                        kit.EmbedToken(rt);
                        t = li[0].EndToken;
                    }
                }
            }
            bool goBack = false;
            for (Pullenti.Ner.Token t = kit.FirstToken; t != null; t = t.Next) 
            {
                if (t.IsIgnored) 
                    continue;
                if (goBack) 
                {
                    goBack = false;
                    if (t.Previous != null) 
                        t = t.Previous;
                }
                GeoReferent geo = t.GetReferent() as GeoReferent;
                if (geo == null) 
                    continue;
                GeoReferent geo1 = null;
                Pullenti.Ner.Token tt = t.Next;
                bool bra = false;
                bool comma1 = false;
                bool comma2 = false;
                bool inp = false;
                bool adj = false;
                for (; tt != null; tt = tt.Next) 
                {
                    if (tt.IsCharOf(",")) 
                    {
                        comma1 = true;
                        continue;
                    }
                    if (tt.IsValue("IN", null) || tt.IsValue("В", null)) 
                    {
                        inp = true;
                        continue;
                    }
                    if (Pullenti.Ner.Core.MiscHelper.IsEngAdjSuffix(tt)) 
                    {
                        adj = true;
                        tt = tt.Next;
                        continue;
                    }
                    Pullenti.Ner.Address.Internal.AddressItemToken det = Pullenti.Ner.Address.Internal.AddressItemToken.TryParsePureItem(tt, null, ad);
                    if (det != null && det.Typ == Pullenti.Ner.Address.Internal.AddressItemType.Detail) 
                    {
                        tt = det.EndToken;
                        comma1 = true;
                        continue;
                    }
                    if (tt.Morph.Class.IsPreposition) 
                        continue;
                    if (tt.IsChar('(') && tt == t.Next) 
                    {
                        bra = true;
                        continue;
                    }
                    if ((tt is Pullenti.Ner.TextToken) && Pullenti.Ner.Core.BracketHelper.IsBracket(tt, true)) 
                        continue;
                    geo1 = tt.GetReferent() as GeoReferent;
                    break;
                }
                if (geo1 == null) 
                    continue;
                if (tt.WhitespacesBeforeCount > 15) 
                    continue;
                else if ((tt != null && tt.Next != null && tt.Next.IsHiphen) && Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(tt)) 
                {
                    List<Pullenti.Ner.Address.Internal.StreetItemToken> sit = Pullenti.Ner.Address.Internal.StreetItemToken.TryParseSpec(tt, null);
                    if (sit != null && sit[0].Typ == Pullenti.Ner.Address.Internal.StreetItemType.Name) 
                        continue;
                }
                Pullenti.Ner.Token ttt = tt.Next;
                GeoReferent geo2 = null;
                for (; ttt != null; ttt = ttt.Next) 
                {
                    if (ttt.IsCommaAnd) 
                    {
                        comma2 = true;
                        continue;
                    }
                    Pullenti.Ner.Address.Internal.AddressItemToken det = Pullenti.Ner.Address.Internal.AddressItemToken.TryParsePureItem(ttt, null, ad);
                    if (det != null && det.Typ == Pullenti.Ner.Address.Internal.AddressItemType.Detail) 
                    {
                        ttt = det.EndToken;
                        comma2 = true;
                        continue;
                    }
                    if (ttt.Morph.Class.IsPreposition) 
                        continue;
                    geo2 = ttt.GetReferent() as GeoReferent;
                    break;
                }
                if (ttt != null && ttt.WhitespacesBeforeCount > 15) 
                    geo2 = null;
                else if ((ttt != null && ttt.Next != null && ttt.Next.IsHiphen) && Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(ttt)) 
                {
                    List<Pullenti.Ner.Address.Internal.StreetItemToken> sit = Pullenti.Ner.Address.Internal.StreetItemToken.TryParseSpec(ttt, null);
                    if (sit != null && sit[0].Typ == Pullenti.Ner.Address.Internal.StreetItemType.Name) 
                        geo2 = null;
                }
                if (geo2 != null) 
                {
                    bool ok2 = comma1 && comma2;
                    if (comma1 != comma2) 
                    {
                        if (geo.IsRegion && geo1.IsRegion && geo2.IsCity) 
                            ok2 = true;
                    }
                    if (ok2 && Pullenti.Ner.Geo.Internal.GeoOwnerHelper.CanBeHigherToken(t, tt) && Pullenti.Ner.Geo.Internal.GeoOwnerHelper.CanBeHigherToken(tt, ttt)) 
                    {
                        geo2.Higher = geo1;
                        geo1.Higher = geo;
                        Pullenti.Ner.ReferentToken rt0 = new Pullenti.Ner.ReferentToken(geo1, t, tt) { Morph = tt.Morph };
                        kit.EmbedToken(rt0);
                        Pullenti.Ner.ReferentToken rt = new Pullenti.Ner.ReferentToken(geo2, rt0, ttt) { Morph = ttt.Morph };
                        kit.EmbedToken(rt);
                        t = rt;
                        goBack = true;
                        continue;
                    }
                    else if (Pullenti.Ner.Geo.Internal.GeoOwnerHelper.CanBeHigherToken(ttt, tt)) 
                    {
                        if (Pullenti.Ner.Geo.Internal.GeoOwnerHelper.CanBeHigherToken(t, ttt)) 
                        {
                            if (geo2.FindSlot(GeoReferent.ATTR_TYPE, "город", true) != null && geo1.FindSlot(GeoReferent.ATTR_TYPE, "район", true) != null && geo.IsRegion) 
                            {
                                geo2.Higher = geo1;
                                geo1.Higher = geo;
                                Pullenti.Ner.ReferentToken rt0 = new Pullenti.Ner.ReferentToken(geo1, t, tt) { Morph = tt.Morph };
                                kit.EmbedToken(rt0);
                                Pullenti.Ner.ReferentToken rt = new Pullenti.Ner.ReferentToken(geo2, rt0, ttt) { Morph = ttt.Morph };
                                kit.EmbedToken(rt);
                                t = rt;
                                goBack = true;
                                continue;
                            }
                            else 
                            {
                                geo2.Higher = geo;
                                geo1.Higher = geo2;
                                Pullenti.Ner.ReferentToken rt = new Pullenti.Ner.ReferentToken(geo1, t, ttt) { Morph = tt.Morph };
                                kit.EmbedToken(rt);
                                t = rt;
                                goBack = true;
                                continue;
                            }
                        }
                        if (Pullenti.Ner.Geo.Internal.GeoOwnerHelper.CanBeHigherToken(ttt, t) && Pullenti.Ner.Geo.Internal.GeoOwnerHelper.CanBeHigherToken(t, tt)) 
                        {
                            if (ttt.IsNewlineBefore) 
                                ttt = tt;
                            else 
                                geo.Higher = geo2;
                            geo1.Higher = geo;
                            Pullenti.Ner.ReferentToken rt = new Pullenti.Ner.ReferentToken(geo1, t, ttt) { Morph = tt.Morph };
                            kit.EmbedToken(rt);
                            t = rt;
                            goBack = true;
                            continue;
                        }
                        if (Pullenti.Ner.Geo.Internal.GeoOwnerHelper.CanBeHigherToken(tt, t)) 
                        {
                            geo.Higher = geo1;
                            geo1.Higher = geo2;
                            Pullenti.Ner.ReferentToken rt0 = new Pullenti.Ner.ReferentToken(geo1, tt, ttt) { Morph = tt.Morph };
                            kit.EmbedToken(rt0);
                            Pullenti.Ner.ReferentToken rt = new Pullenti.Ner.ReferentToken(geo, t, rt0) { Morph = t.Morph };
                            kit.EmbedToken(rt);
                            t = rt;
                            goBack = true;
                            continue;
                        }
                        if (Pullenti.Ner.Geo.Internal.GeoOwnerHelper.CanBeHigherToken(t, tt) && Pullenti.Ner.Geo.Internal.GeoOwnerHelper.CanBeHigherToken(ttt, tt)) 
                        {
                            if (geo1.FindSlot(GeoReferent.ATTR_TYPE, "муниципальный округ", true) != null && geo.FindSlot(GeoReferent.ATTR_TYPE, "город", true) != null && geo2.FindSlot(GeoReferent.ATTR_TYPE, "город", true) != null) 
                            {
                                if (geo2.FindSlot(GeoReferent.ATTR_NAME, "МОСКВА", true) != null) 
                                {
                                    geo.Higher = geo1;
                                    geo1.Higher = geo2;
                                    Pullenti.Ner.ReferentToken rt0 = new Pullenti.Ner.ReferentToken(geo1, tt, ttt) { Morph = tt.Morph };
                                    kit.EmbedToken(rt0);
                                    Pullenti.Ner.ReferentToken rt = new Pullenti.Ner.ReferentToken(geo, t, rt0) { Morph = t.Morph };
                                    kit.EmbedToken(rt);
                                    t = rt;
                                    goBack = true;
                                    continue;
                                }
                                else 
                                {
                                    geo2.Higher = geo1;
                                    geo1.Higher = geo;
                                    Pullenti.Ner.ReferentToken rt0 = new Pullenti.Ner.ReferentToken(geo1, t, tt) { Morph = tt.Morph };
                                    kit.EmbedToken(rt0);
                                    Pullenti.Ner.ReferentToken rt = new Pullenti.Ner.ReferentToken(geo2, rt0, ttt) { Morph = ttt.Morph };
                                    kit.EmbedToken(rt);
                                    t = rt;
                                    goBack = true;
                                    continue;
                                }
                            }
                        }
                    }
                    if (comma2) 
                        continue;
                }
                if (Pullenti.Ner.Geo.Internal.GeoOwnerHelper.CanBeHigherToken(t, tt) && ((!Pullenti.Ner.Geo.Internal.GeoOwnerHelper.CanBeHigherToken(tt, t) || adj))) 
                {
                    geo1.Higher = geo;
                    Pullenti.Ner.ReferentToken rt = new Pullenti.Ner.ReferentToken(geo1, t, tt) { Morph = tt.Morph };
                    if ((geo1.IsCity && !geo.IsCity && t.Previous != null) && t.Previous.IsValue("СТОЛИЦА", "СТОЛИЦЯ")) 
                    {
                        rt.BeginToken = t.Previous;
                        rt.Morph = t.Previous.Morph;
                    }
                    kit.EmbedToken(rt);
                    t = rt;
                    goBack = true;
                    continue;
                }
                if (Pullenti.Ner.Geo.Internal.GeoOwnerHelper.CanBeHigherToken(tt, t) && ((!Pullenti.Ner.Geo.Internal.GeoOwnerHelper.CanBeHigherToken(t, tt) || inp))) 
                {
                    if (geo.Higher == null) 
                        geo.Higher = geo1;
                    else if (geo1.Higher == null && Pullenti.Ner.Geo.Internal.GeoOwnerHelper.CanBeHigher(geo.Higher, geo1, null, null) && !Pullenti.Ner.Geo.Internal.GeoOwnerHelper.CanBeHigher(geo1, geo.Higher, null, null)) 
                    {
                        geo1.Higher = geo.Higher;
                        geo.Higher = geo1;
                    }
                    else 
                        geo.Higher = geo1;
                    if (bra && tt.Next != null && tt.Next.IsChar(')')) 
                        tt = tt.Next;
                    if ((bra && geo1.IsRegion && geo1.Higher == null) && geo.IsCity && Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(tt)) 
                    {
                        Pullenti.Ner.Token tt2 = tt.Next;
                        while (tt2 != null) 
                        {
                            if (tt2.IsComma) 
                                tt2 = tt2.Next;
                            else 
                                break;
                        }
                        if (tt2 is Pullenti.Ner.TextToken) 
                        {
                            Pullenti.Ner.Geo.Internal.TerrItemToken ter = Pullenti.Ner.Geo.Internal.TerrItemToken.TryParse(tt2, null, null);
                            if (ter != null && ter.OntoItem == null && ter.TerminItem == null) 
                            {
                                tt2 = ter.EndToken.Next;
                                while (tt2 != null) 
                                {
                                    if (tt2.IsComma) 
                                        tt2 = tt2.Next;
                                    else 
                                        break;
                                }
                                if (tt2 != null && (tt2.GetReferent() is GeoReferent)) 
                                {
                                    if (Pullenti.Ner.Geo.Internal.GeoOwnerHelper.CanBeHigher(tt2.GetReferent() as GeoReferent, geo1, null, null)) 
                                    {
                                        geo1.Higher = tt2.GetReferent() as GeoReferent;
                                        tt = tt2;
                                    }
                                }
                            }
                        }
                    }
                    Pullenti.Ner.ReferentToken rt = new Pullenti.Ner.ReferentToken(geo, t, tt) { Morph = t.Morph };
                    kit.EmbedToken(rt);
                    t = rt;
                    goBack = true;
                    continue;
                }
                if ((!tt.Morph.Class.IsAdjective && !t.Morph.Class.IsAdjective && tt.Chars.IsCyrillicLetter) && t.Chars.IsCyrillicLetter && !tt.Morph.Case.IsInstrumental) 
                {
                    for (GeoReferent geo0 = geo; geo0 != null; geo0 = geo0.Higher) 
                    {
                        if (Pullenti.Ner.Geo.Internal.GeoOwnerHelper.CanBeHigher(geo1, geo0, null, null)) 
                        {
                            geo0.Higher = geo1;
                            Pullenti.Ner.ReferentToken rt = new Pullenti.Ner.ReferentToken(geo, t, tt) { Morph = t.Morph };
                            kit.EmbedToken(rt);
                            t = rt;
                            goBack = true;
                            break;
                        }
                    }
                }
            }
            Dictionary<string, GeoReferent> citiesSettls = new Dictionary<string, GeoReferent>();
            Dictionary<string, GeoReferent> citiesSettls2 = new Dictionary<string, GeoReferent>();
            foreach (Pullenti.Ner.Core.IntOntologyItem v in ad.LocalOntology.Items) 
            {
                GeoReferent g = v.Referent as GeoReferent;
                if (g == null || !g.IsCity) 
                    continue;
                if (g.FindSlot(GeoReferent.ATTR_TYPE, "городское поселение", true) != null) 
                {
                    foreach (string n in g.GetStringValues(GeoReferent.ATTR_NAME)) 
                    {
                        if (!citiesSettls.ContainsKey(n)) 
                            citiesSettls.Add(n, g);
                    }
                }
            }
            foreach (GeoReferent g in nonRegistered) 
            {
                if (!g.IsRegion) 
                    continue;
                if (g.FindSlot(GeoReferent.ATTR_TYPE, "городской округ", true) == null) 
                    continue;
                foreach (string n in g.GetStringValues(GeoReferent.ATTR_NAME)) 
                {
                    if (!citiesSettls2.ContainsKey(n)) 
                        citiesSettls2.Add(n, g);
                }
            }
            foreach (Pullenti.Ner.Core.IntOntologyItem v in ad.LocalOntology.Items) 
            {
                GeoReferent g = v.Referent as GeoReferent;
                if (g == null || !g.IsCity) 
                    continue;
                if (g.Higher != null) 
                    continue;
                if (g.FindSlot(GeoReferent.ATTR_TYPE, "город", true) == null) 
                    continue;
                foreach (string n in g.GetStringValues(GeoReferent.ATTR_NAME)) 
                {
                    if (citiesSettls.ContainsKey(n)) 
                    {
                        g.Higher = citiesSettls[n];
                        break;
                    }
                    else if (citiesSettls2.ContainsKey(n)) 
                    {
                        g.Higher = citiesSettls2[n];
                        break;
                    }
                }
            }
            for (int k = 0; k < nonRegistered.Count; k++) 
            {
                bool ch = false;
                for (int i = 0; i < (nonRegistered.Count - 1); i++) 
                {
                    if (geoComp(nonRegistered[i], nonRegistered[i + 1]) > 0) 
                    {
                        ch = true;
                        GeoReferent v = nonRegistered[i];
                        nonRegistered[i] = nonRegistered[i + 1];
                        nonRegistered[i + 1] = v;
                    }
                }
                if (!ch) 
                    break;
            }
            foreach (GeoReferent g in nonRegistered) 
            {
                g.Tag = null;
            }
            foreach (GeoReferent ng in nonRegistered) 
            {
                foreach (Pullenti.Ner.Slot s in ng.Slots) 
                {
                    if (s.Value is GeoReferent) 
                    {
                        if ((s.Value as GeoReferent).Tag is GeoReferent) 
                            ng.UploadSlot(s, (s.Value as GeoReferent).Tag as GeoReferent);
                    }
                }
                GeoReferent rg = ad.RegisterReferent(ng) as GeoReferent;
                if (rg == ng) 
                    continue;
                ng.Tag = rg;
                foreach (Pullenti.Ner.TextAnnotation oc in ng.Occurrence) 
                {
                    oc.OccurenceOf = rg;
                    rg.AddOccurence(oc);
                }
            }
            if (nonRegistered.Count > 0) 
            {
                for (Pullenti.Ner.Token t = kit.FirstToken; t != null; t = t.Next) 
                {
                    if (t.IsIgnored) 
                        continue;
                    GeoReferent geo = t.GetReferent() as GeoReferent;
                    if (geo == null) 
                        continue;
                    _replaceTerrs(t as Pullenti.Ner.ReferentToken);
                }
            }
            ad.ORegime = false;
            ad.OTRegime = false;
            ad.TRegime = false;
            ad.CRegime = false;
            ad.SRegime = false;
            ad.ARegime = false;
            sw.Stop();
            kit.Msgs.Add(string.Format("GeoMisc: {0}ms", sw.ElapsedMilliseconds));
            if (!this.OnProgress(100, 100, kit)) 
                return;
        }
        static void _replaceTerrs(Pullenti.Ner.ReferentToken mt)
        {
            if (mt == null) 
                return;
            GeoReferent geo = mt.Referent as GeoReferent;
            if (geo != null && (geo.Tag is GeoReferent)) 
                mt.Referent = geo.Tag as GeoReferent;
            if (geo != null) 
            {
                foreach (Pullenti.Ner.Slot s in geo.Slots) 
                {
                    if (s.Value is GeoReferent) 
                    {
                        GeoReferent g = s.Value as GeoReferent;
                        if (g.Tag is GeoReferent) 
                            geo.UploadSlot(s, g.Tag);
                    }
                }
            }
            for (Pullenti.Ner.Token t = mt.BeginToken; t != null; t = t.Next) 
            {
                if (t.EndChar > mt.EndToken.EndChar) 
                    break;
                else 
                {
                    if (t is Pullenti.Ner.ReferentToken) 
                        _replaceTerrs(t as Pullenti.Ner.ReferentToken);
                    if (t == mt.EndToken) 
                        break;
                }
            }
        }
        static int geoComp(GeoReferent x, GeoReferent y)
        {
            int xcou = 0;
            for (GeoReferent g = x.Higher; g != null; g = g.Higher) 
            {
                xcou++;
            }
            int ycou = 0;
            for (GeoReferent g = y.Higher; g != null; g = g.Higher) 
            {
                ycou++;
            }
            if (xcou < ycou) 
                return -1;
            if (xcou > ycou) 
                return 1;
            return string.Compare(x.ToStringEx(true, Pullenti.Morph.MorphLang.Unknown, 0), y.ToStringEx(true, Pullenti.Morph.MorphLang.Unknown, 0));
        }
        List<Pullenti.Ner.Geo.Internal.CityItemToken> TryParseCityListBack(Pullenti.Ner.Token t)
        {
            if (t == null) 
                return null;
            while (t != null && ((t.Morph.Class.IsPreposition || t.IsCharOf(",.") || t.Morph.Class.IsConjunction))) 
            {
                t = t.Previous;
            }
            if (t == null) 
                return null;
            List<Pullenti.Ner.Geo.Internal.CityItemToken> res = null;
            for (Pullenti.Ner.Token tt = t; tt != null; tt = tt.Previous) 
            {
                if (!(tt is Pullenti.Ner.TextToken)) 
                    break;
                if (tt.Previous != null && tt.Previous.IsHiphen && (tt.Previous.Previous is Pullenti.Ner.TextToken)) 
                {
                    if (!tt.IsWhitespaceBefore && !tt.Previous.IsWhitespaceBefore) 
                        tt = tt.Previous.Previous;
                }
                List<Pullenti.Ner.Geo.Internal.CityItemToken> ci = Pullenti.Ner.Geo.Internal.CityItemToken.TryParseList(tt, 5, null);
                if (ci == null && tt.Previous != null) 
                    ci = Pullenti.Ner.Geo.Internal.CityItemToken.TryParseList(tt.Previous, 5, null);
                if (ci == null) 
                    break;
                if (ci[ci.Count - 1].EndToken == t) 
                    res = ci;
            }
            if (res != null) 
                res.Reverse();
            return res;
        }
        Pullenti.Ner.ReferentToken TryAttachTerritoryBeforeCity(Pullenti.Ner.Token t, Pullenti.Ner.Core.AnalyzerDataWithOntology ad)
        {
            if (t is Pullenti.Ner.ReferentToken) 
                t = t.Previous;
            for (; t != null; t = t.Previous) 
            {
                if (!t.IsCharOf(",.") && !t.Morph.Class.IsPreposition) 
                    break;
            }
            if (t == null) 
                return null;
            int i = 0;
            Pullenti.Ner.ReferentToken res = null;
            for (Pullenti.Ner.Token tt = t; tt != null; tt = tt.Previous) 
            {
                i++;
                if (tt.IsNewlineAfter && !tt.InnerBool) 
                    break;
                if (i > 10) 
                    break;
                List<Pullenti.Ner.Geo.Internal.TerrItemToken> tits0 = Pullenti.Ner.Geo.Internal.TerrItemToken.TryParseList(tt, 5, null);
                if (tits0 == null) 
                    continue;
                if (tits0[tits0.Count - 1].EndToken != t) 
                    break;
                List<Pullenti.Ner.Geo.Internal.TerrItemToken> tits1 = Pullenti.Ner.Geo.Internal.TerrItemToken.TryParseList(tt.Previous, 5, null);
                if (tits1 != null && tits1[tits1.Count - 1].EndToken == t && tits1.Count == tits0.Count) 
                    tits0 = tits1;
                Pullenti.Ner.ReferentToken rr = Pullenti.Ner.Geo.Internal.TerrDefineHelper.TryDefine(tits0, ad, false, null, null);
                if (rr != null) 
                    res = rr;
            }
            return res;
        }
        Pullenti.Ner.ReferentToken TryAttachTerritoryAfterCity(Pullenti.Ner.Token t, Pullenti.Ner.Core.AnalyzerDataWithOntology ad)
        {
            if (t == null) 
                return null;
            GeoReferent city = t.GetReferent() as GeoReferent;
            if (city == null) 
                return null;
            if (!city.IsCity) 
                return null;
            if (t.Next == null || !t.Next.IsComma || t.Next.WhitespacesAfterCount > 1) 
                return null;
            Pullenti.Ner.Token tt = t.Next.Next;
            if (tt == null || !tt.Chars.IsCapitalUpper || !(tt is Pullenti.Ner.TextToken)) 
                return null;
            if (tt.Chars.IsLatinLetter) 
            {
                Pullenti.Ner.ReferentToken re1 = Pullenti.Ner.Geo.Internal.TerrDefineHelper.TryAttachStateUSATerritory(tt);
                if (re1 != null) 
                    return re1;
            }
            Pullenti.Ner.Token t0 = tt;
            Pullenti.Ner.Token t1 = tt;
            for (int i = 0; i < 2; i++) 
            {
                Pullenti.Ner.Geo.Internal.TerrItemToken tit0 = Pullenti.Ner.Geo.Internal.TerrItemToken.TryParse(tt, null, null);
                if (tit0 == null || tit0.TerminItem != null) 
                {
                    if (i == 0) 
                        return null;
                }
                Pullenti.Ner.Geo.Internal.CityItemToken cit0 = Pullenti.Ner.Geo.Internal.CityItemToken.TryParse(tt, null, false, null);
                if (cit0 == null || cit0.Typ == Pullenti.Ner.Geo.Internal.CityItemToken.ItemType.Noun) 
                {
                    if (i == 0) 
                        return null;
                }
                Pullenti.Ner.Address.Internal.AddressItemToken ait0 = Pullenti.Ner.Address.Internal.AddressItemToken.TryParse(tt, false, null, null);
                if (ait0 != null) 
                    return null;
                if (tit0 == null) 
                {
                    if (!tt.Chars.IsCyrillicLetter) 
                        return null;
                    Pullenti.Morph.MorphClass cla = tt.GetMorphClassInDictionary();
                    if (!cla.IsNoun && !cla.IsAdjective) 
                        return null;
                    t1 = tt;
                }
                else 
                    t1 = (tt = tit0.EndToken);
                if (tt.Next == null) 
                    return null;
                if (tt.Next.IsComma) 
                {
                    tt = tt.Next.Next;
                    break;
                }
                if (i > 0) 
                    return null;
                tt = tt.Next;
            }
            while (tt != null) 
            {
                if (tt.IsComma) 
                    tt = tt.Next;
                else 
                    break;
            }
            Pullenti.Ner.Address.Internal.AddressItemToken ait = Pullenti.Ner.Address.Internal.AddressItemToken.TryParse(tt, false, null, null);
            if (ait == null) 
                return null;
            if (ait.Typ == Pullenti.Ner.Address.Internal.AddressItemType.Street && ait.RefToken == null) 
            {
                GeoReferent reg = new GeoReferent();
                reg.AddTyp("муниципальный район");
                reg.AddName(Pullenti.Ner.Core.MiscHelper.GetTextValue(t0, t1, Pullenti.Ner.Core.GetTextAttr.No));
                return new Pullenti.Ner.ReferentToken(reg, t0, t1);
            }
            if (ait.Typ == Pullenti.Ner.Address.Internal.AddressItemType.Region && (ait.Referent is GeoReferent)) 
            {
                GeoReferent reg = new GeoReferent();
                reg.AddTyp("район");
                reg.AddName(Pullenti.Ner.Core.MiscHelper.GetTextValue(t0, t1, Pullenti.Ner.Core.GetTextAttr.No));
                reg.Higher = ait.Referent as GeoReferent;
                return new Pullenti.Ner.ReferentToken(reg, t0, t1);
            }
            return null;
        }
        // Это привязка стран к прилагательным (например, "французский лидер")
        public override Pullenti.Ner.ReferentToken ProcessReferent(Pullenti.Ner.Token begin, string param)
        {
            if (!(begin is Pullenti.Ner.TextToken)) 
                return null;
            Pullenti.Ner.Geo.Internal.GeoAnalyzerData ad = GetData(begin);
            if (ad == null) 
                return null;
            if (ad.Level > 1) 
                return null;
            ad.Level++;
            List<Pullenti.Ner.Core.TerminToken> toks = Pullenti.Ner.Geo.Internal.CityItemToken.m_CityAdjectives.TryParseAll(begin, Pullenti.Ner.Core.TerminParseAttr.FullwordsOnly);
            ad.Level--;
            Pullenti.Ner.ReferentToken res1 = null;
            if (toks != null) 
            {
                foreach (Pullenti.Ner.Core.TerminToken tok in toks) 
                {
                    Pullenti.Ner.Core.IntOntologyItem cit = tok.Termin.Tag as Pullenti.Ner.Core.IntOntologyItem;
                    if (cit == null) 
                        continue;
                    GeoReferent city = new GeoReferent();
                    city.AddName(cit.CanonicText);
                    city.AddTypCity(begin.Kit.BaseLanguage, true);
                    res1 = new Pullenti.Ner.ReferentToken(city, tok.BeginToken, tok.EndToken) { Morph = tok.Morph, Data = begin.Kit.GetAnalyzerData(this) };
                    break;
                }
            }
            if (!begin.Morph.Class.IsAdjective) 
            {
                Pullenti.Ner.TextToken te = begin as Pullenti.Ner.TextToken;
                if ((te.Chars.IsAllUpper && te.Chars.IsCyrillicLetter && te.LengthChar == 2) && te.GetMorphClassInDictionary().IsUndefined) 
                {
                    string abbr = te.Term;
                    GeoReferent geo0 = null;
                    int cou = 0;
                    foreach (Pullenti.Ner.Core.IntOntologyItem t in ad.LocalOntology.Items) 
                    {
                        GeoReferent geo = t.Referent as GeoReferent;
                        if (geo == null) 
                            continue;
                        if (!geo.IsRegion && !geo.IsState) 
                            continue;
                        if (geo.CheckAbbr(abbr)) 
                        {
                            cou++;
                            geo0 = geo;
                        }
                    }
                    if (cou == 1 && res1 == null) 
                        res1 = new Pullenti.Ner.ReferentToken(geo0, begin, begin) { Data = ad };
                }
                ad.Level++;
                Pullenti.Ner.Geo.Internal.TerrItemToken tt0 = Pullenti.Ner.Geo.Internal.TerrItemToken.TryParse(begin, null, null);
                ad.Level--;
                if (tt0 != null && tt0.TerminItem != null && tt0.TerminItem.CanonicText == "РАЙОН") 
                {
                    ad.Level++;
                    Pullenti.Ner.Geo.Internal.TerrItemToken tt1 = Pullenti.Ner.Geo.Internal.TerrItemToken.TryParse(tt0.EndToken.Next, null, null);
                    ad.Level--;
                    if ((tt1 != null && tt1.Chars.IsCapitalUpper && tt1.TerminItem == null) && tt1.OntoItem == null) 
                    {
                        List<Pullenti.Ner.Geo.Internal.TerrItemToken> li = new List<Pullenti.Ner.Geo.Internal.TerrItemToken>();
                        li.Add(tt0);
                        li.Add(tt1);
                        Pullenti.Ner.ReferentToken res = Pullenti.Ner.Geo.Internal.TerrDefineHelper.TryDefine(li, ad, true, null, null);
                        if (res == null) 
                            return null;
                        res.Morph = begin.Morph;
                        res.Data = ad;
                        if (res1 == null || res.LengthChar > res1.LengthChar) 
                            res1 = res;
                    }
                }
                ad.Level++;
                List<Pullenti.Ner.Geo.Internal.CityItemToken> ctoks = Pullenti.Ner.Geo.Internal.CityItemToken.TryParseList(begin, 3, null);
                if (ctoks == null && begin.Morph.Class.IsPreposition) 
                    ctoks = Pullenti.Ner.Geo.Internal.CityItemToken.TryParseList(begin.Next, 3, null);
                ad.Level--;
                if (ctoks != null) 
                {
                    if (((ctoks.Count == 2 && ctoks[0].Typ == Pullenti.Ner.Geo.Internal.CityItemToken.ItemType.Noun && ctoks[1].Typ == Pullenti.Ner.Geo.Internal.CityItemToken.ItemType.ProperName)) || ((ctoks.Count == 1 && ctoks[0].Typ == Pullenti.Ner.Geo.Internal.CityItemToken.ItemType.City))) 
                    {
                        if (ctoks.Count == 1 && ctoks[0].BeginToken.GetMorphClassInDictionary().IsProperSurname) 
                        {
                            Pullenti.Ner.ReferentToken kk = begin.Kit.ProcessReferent("PERSON", ctoks[0].BeginToken, null);
                            if (kk != null) 
                                return null;
                        }
                        Pullenti.Ner.ReferentToken res = Pullenti.Ner.Geo.Internal.CityAttachHelper.TryDefine(ctoks, ad, true);
                        if (res != null) 
                        {
                            res.Data = ad;
                            if (res1 == null || res.LengthChar > res1.LengthChar) 
                                res1 = res;
                        }
                    }
                }
                if ((ctoks != null && ctoks.Count == 1 && ctoks[0].Typ == Pullenti.Ner.Geo.Internal.CityItemToken.ItemType.Noun) && ctoks[0].Value == "ГОРОД") 
                {
                    int cou = 0;
                    for (Pullenti.Ner.Token t = begin.Previous; t != null; t = t.Previous) 
                    {
                        if ((++cou) > 500) 
                            break;
                        if (!(t is Pullenti.Ner.ReferentToken)) 
                            continue;
                        List<Pullenti.Ner.Referent> geos = t.GetReferents();
                        if (geos == null) 
                            continue;
                        foreach (Pullenti.Ner.Referent g in geos) 
                        {
                            GeoReferent gg = g as GeoReferent;
                            if (gg != null) 
                            {
                                Pullenti.Ner.ReferentToken res = null;
                                if (gg.IsCity) 
                                    res = new Pullenti.Ner.ReferentToken(gg, begin, ctoks[0].EndToken) { Morph = ctoks[0].Morph, Data = ad };
                                if (gg.Higher != null && gg.Higher.IsCity) 
                                    res = new Pullenti.Ner.ReferentToken(gg.Higher, begin, ctoks[0].EndToken) { Morph = ctoks[0].Morph, Data = ad };
                                if (res != null && ((res1 == null || res.LengthChar > res1.LengthChar))) 
                                    res1 = res;
                            }
                        }
                    }
                }
                if (tt0 != null && tt0.OntoItem != null) 
                {
                }
                else 
                    return res1;
            }
            ad.Level++;
            Pullenti.Ner.Geo.Internal.TerrItemToken tt = Pullenti.Ner.Geo.Internal.TerrItemToken.TryParse(begin, null, null);
            ad.Level--;
            if (tt == null || tt.OntoItem == null) 
            {
                List<Pullenti.Ner.Core.IntOntologyToken> tok = Pullenti.Ner.Geo.Internal.TerrItemToken.m_TerrOntology.TryAttach(begin, null, false);
                if ((tok != null && tok[0].Item != null && (tok[0].Item.Referent is GeoReferent)) && (tok[0].Item.Referent as GeoReferent).IsState) 
                    tt = new Pullenti.Ner.Geo.Internal.TerrItemToken(tok[0].BeginToken, tok[0].EndToken) { OntoItem = tok[0].Item };
            }
            if (tt == null) 
                return res1;
            if (tt.OntoItem != null) 
            {
                ad.Level++;
                List<Pullenti.Ner.Geo.Internal.TerrItemToken> li = Pullenti.Ner.Geo.Internal.TerrItemToken.TryParseList(begin, 3, null);
                Pullenti.Ner.ReferentToken res = Pullenti.Ner.Geo.Internal.TerrDefineHelper.TryDefine(li, ad, true, null, null);
                ad.Level--;
                if (res == null) 
                    tt.OntoItem = null;
                else 
                {
                    if (res.BeginToken == res.EndToken) 
                    {
                        Pullenti.Morph.MorphClass mc = res.BeginToken.GetMorphClassInDictionary();
                        if (mc.IsAdjective) 
                        {
                            GeoReferent geo = tt.OntoItem.Referent as GeoReferent;
                            if (geo.IsCity || geo.IsState) 
                            {
                            }
                            else if (geo.FindSlot(GeoReferent.ATTR_TYPE, "федеральный округ", true) != null) 
                                return null;
                        }
                    }
                    res.Data = ad;
                    if (res1 == null || res.LengthChar > res1.LengthChar) 
                        res1 = res;
                }
            }
            if (!tt.IsAdjective) 
                return res1;
            if (tt.OntoItem == null) 
            {
                Pullenti.Ner.Token t1 = tt.EndToken.Next;
                if (t1 == null) 
                    return res1;
                ad.Level++;
                Pullenti.Ner.Geo.Internal.TerrItemToken ttyp = Pullenti.Ner.Geo.Internal.TerrItemToken.TryParse(t1, null, null);
                ad.Level--;
                if (ttyp == null || ttyp.TerminItem == null) 
                {
                    ad.Level++;
                    List<Pullenti.Ner.Geo.Internal.CityItemToken> cits = Pullenti.Ner.Geo.Internal.CityItemToken.TryParseList(begin, 2, null);
                    ad.Level--;
                    if (cits != null && cits[0].Typ == Pullenti.Ner.Geo.Internal.CityItemToken.ItemType.City) 
                    {
                        ad.Level++;
                        Pullenti.Ner.ReferentToken res2 = Pullenti.Ner.Geo.Internal.CityAttachHelper.TryDefine(cits, ad, true);
                        ad.Level--;
                        if (res2 != null) 
                        {
                            if (res1 == null || res2.LengthChar > res1.LengthChar) 
                                res1 = res2;
                        }
                    }
                    return res1;
                }
                if (t1.GetMorphClassInDictionary().IsAdjective) 
                    return res1;
                List<Pullenti.Ner.Geo.Internal.TerrItemToken> li = new List<Pullenti.Ner.Geo.Internal.TerrItemToken>();
                li.Add(tt);
                li.Add(ttyp);
                ad.Level++;
                Pullenti.Ner.ReferentToken res = Pullenti.Ner.Geo.Internal.TerrDefineHelper.TryDefine(li, ad, true, null, null);
                ad.Level--;
                if (res == null) 
                    return res1;
                res.Morph = ttyp.Morph;
                res.Data = ad;
                if (res1 == null || res.LengthChar > res1.LengthChar) 
                    res1 = res;
            }
            return res1;
        }
        public Pullenti.Ner.ReferentToken ProcessCitizen(Pullenti.Ner.Token begin)
        {
            if (!(begin is Pullenti.Ner.TextToken)) 
                return null;
            Pullenti.Ner.Core.TerminToken tok = Pullenti.Ner.Geo.Internal.TerrItemToken.m_MansByState.TryParse(begin, Pullenti.Ner.Core.TerminParseAttr.FullwordsOnly);
            if (tok != null) 
                tok.Morph.Gender = tok.Termin.Gender;
            if (tok == null) 
                return null;
            GeoReferent geo0 = tok.Termin.Tag as GeoReferent;
            if (geo0 == null) 
                return null;
            GeoReferent geo = new GeoReferent();
            geo.MergeSlots2(geo0, begin.Kit.BaseLanguage);
            Pullenti.Ner.ReferentToken res = new Pullenti.Ner.ReferentToken(geo, tok.BeginToken, tok.EndToken);
            res.Morph = tok.Morph;
            Pullenti.Ner.Core.AnalyzerDataWithOntology ad = begin.Kit.GetAnalyzerData(this) as Pullenti.Ner.Core.AnalyzerDataWithOntology;
            res.Data = ad;
            return res;
        }
        public override Pullenti.Ner.ReferentToken ProcessOntologyItem(Pullenti.Ner.Token begin)
        {
            List<Pullenti.Ner.Geo.Internal.CityItemToken> li = Pullenti.Ner.Geo.Internal.CityItemToken.TryParseList(begin, 4, null);
            if (li != null && li.Count > 1 && li[0].Typ == Pullenti.Ner.Geo.Internal.CityItemToken.ItemType.Noun) 
            {
                Pullenti.Ner.ReferentToken rt = Pullenti.Ner.Geo.Internal.CityAttachHelper.TryDefine(li, null, true);
                if (rt == null) 
                    return null;
                GeoReferent city = rt.Referent as GeoReferent;
                for (Pullenti.Ner.Token t = rt.EndToken.Next; t != null; t = t.Next) 
                {
                    if (!t.IsChar(';')) 
                        continue;
                    t = t.Next;
                    if (t == null) 
                        break;
                    li = Pullenti.Ner.Geo.Internal.CityItemToken.TryParseList(t, 4, null);
                    Pullenti.Ner.ReferentToken rt1 = Pullenti.Ner.Geo.Internal.CityAttachHelper.TryDefine(li, null, false);
                    if (rt1 != null) 
                    {
                        t = (rt.EndToken = rt1.EndToken);
                        city.MergeSlots2(rt1.Referent, begin.Kit.BaseLanguage);
                    }
                    else 
                    {
                        Pullenti.Ner.Token tt = null;
                        for (Pullenti.Ner.Token ttt = t; ttt != null; ttt = ttt.Next) 
                        {
                            if (ttt.IsChar(';')) 
                                break;
                            else 
                                tt = ttt;
                        }
                        if (tt != null) 
                        {
                            string str = Pullenti.Ner.Core.MiscHelper.GetTextValue(t, tt, Pullenti.Ner.Core.GetTextAttr.No);
                            if (str != null) 
                                city.AddName(str);
                            t = (rt.EndToken = tt);
                        }
                    }
                }
                return rt;
            }
            string typ = null;
            GeoReferent terr = null;
            Pullenti.Ner.Token te = null;
            for (Pullenti.Ner.Token t = begin; t != null; t = t.Next) 
            {
                Pullenti.Ner.Token t0 = t;
                Pullenti.Ner.Token t1 = null;
                Pullenti.Ner.Token tn0 = null;
                Pullenti.Ner.Token tn1 = null;
                for (Pullenti.Ner.Token tt = t0; tt != null; tt = tt.Next) 
                {
                    if (tt.IsCharOf(";")) 
                        break;
                    Pullenti.Ner.Geo.Internal.TerrItemToken tit = Pullenti.Ner.Geo.Internal.TerrItemToken.TryParse(tt, null, null);
                    if (tit != null && tit.TerminItem != null) 
                    {
                        if (!tit.IsAdjective) 
                        {
                            if (typ == null) 
                                typ = tit.TerminItem.CanonicText;
                            tt = tit.EndToken;
                            t1 = tt;
                            continue;
                        }
                    }
                    else if (tit != null && tit.OntoItem != null) 
                    {
                    }
                    if (tn0 == null) 
                        tn0 = tt;
                    if (tit != null) 
                        tt = tit.EndToken;
                    t1 = (tn1 = tt);
                }
                if (t1 == null) 
                    continue;
                if (terr == null) 
                    terr = new GeoReferent();
                if (tn0 != null) 
                    terr.AddName(Pullenti.Ner.Core.MiscHelper.GetTextValue(tn0, tn1, Pullenti.Ner.Core.GetTextAttr.No));
                t = (te = t1);
            }
            if (terr == null || te == null) 
                return null;
            if (typ != null) 
                terr.AddTyp(typ);
            if (!terr.IsCity && !terr.IsRegion && !terr.IsState) 
                terr.AddTypReg(begin.Kit.BaseLanguage);
            return new Pullenti.Ner.ReferentToken(terr, begin, te);
        }
        /// <summary>
        /// Получить список всех стран из внутреннего словаря
        /// </summary>
        public static List<Pullenti.Ner.Referent> GetAllCountries()
        {
            return Pullenti.Ner.Geo.Internal.TerrItemToken.m_AllStates;
        }
        static bool m_Initialized = false;
        public static void Initialize()
        {
            if (m_Initialized) 
                return;
            m_Initialized = true;
            Pullenti.Ner.Geo.Internal.MetaGeo.Initialize();
            Pullenti.Ner.Address.Internal.MetaAddress.Initialize();
            Pullenti.Ner.Address.Internal.MetaStreet.Initialize();
            try 
            {
                Pullenti.Ner.Core.Termin.AssignAllTextsAsNormal = false;
                Pullenti.Ner.Geo.Internal.MiscLocationHelper.Initialize();
                Pullenti.Ner.Geo.Internal.OrgTypToken.Initialize();
                Pullenti.Ner.Geo.Internal.NameToken.Initialize();
                Pullenti.Ner.Geo.Internal.OrgItemToken.Initialize();
                Pullenti.Ner.Geo.Internal.TerrItemToken.Initialize();
                Pullenti.Ner.Geo.Internal.CityItemToken.Initialize();
                Pullenti.Ner.Address.AddressAnalyzer.Initialize();
            }
            catch(Exception ex) 
            {
                throw new Exception(ex.Message, ex);
            }
            Pullenti.Ner.Core.Termin.AssignAllTextsAsNormal = false;
            Pullenti.Ner.ProcessorService.RegisterAnalyzer(new GeoAnalyzer());
        }
    }
}