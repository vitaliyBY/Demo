﻿/*
 * SDK Pullenti Address, version 4.20, august 2023. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Collections;
using System.Collections.Generic;

namespace Pullenti.Ner.Date
{
    /// <summary>
    /// Анализатор для дат и их диапазонов
    /// </summary>
    public class DateAnalyzer : Pullenti.Ner.Analyzer
    {
        /// <summary>
        /// Имя анализатора ("DATE")
        /// </summary>
        public const string ANALYZER_NAME = "DATE";
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
                return "Даты";
            }
        }
        public override string Description
        {
            get
            {
                return "Даты и диапазоны дат";
            }
        }
        public override Pullenti.Ner.Analyzer Clone()
        {
            return new DateAnalyzer();
        }
        public override ICollection<Pullenti.Ner.Metadata.ReferentClass> TypeSystem
        {
            get
            {
                return new Pullenti.Ner.Metadata.ReferentClass[] {Pullenti.Ner.Date.Internal.MetaDate.GlobalMeta, Pullenti.Ner.Date.Internal.MetaDateRange.GlobalMeta};
            }
        }
        public override IEnumerable<string> UsedExternObjectTypes
        {
            get
            {
                return new string[] {"PHONE"};
            }
        }
        public override Dictionary<string, byte[]> Images
        {
            get
            {
                Dictionary<string, byte[]> res = new Dictionary<string, byte[]>();
                res.Add(Pullenti.Ner.Date.Internal.MetaDate.DateFullImageId, Pullenti.Ner.Core.Internal.ResourceHelper.GetBytes("datefull.png"));
                res.Add(Pullenti.Ner.Date.Internal.MetaDate.DateImageId, Pullenti.Ner.Core.Internal.ResourceHelper.GetBytes("date.png"));
                res.Add(Pullenti.Ner.Date.Internal.MetaDate.DateRelImageId, Pullenti.Ner.Core.Internal.ResourceHelper.GetBytes("daterel.png"));
                res.Add(Pullenti.Ner.Date.Internal.MetaDateRange.DateRangeImageId, Pullenti.Ner.Core.Internal.ResourceHelper.GetBytes("daterange.png"));
                res.Add(Pullenti.Ner.Date.Internal.MetaDateRange.DateRangeRelImageId, Pullenti.Ner.Core.Internal.ResourceHelper.GetBytes("daterangerel.png"));
                return res;
            }
        }
        public override Pullenti.Ner.Referent CreateReferent(string type)
        {
            if (type == DateReferent.OBJ_TYPENAME) 
                return new DateReferent();
            if (type == DateRangeReferent.OBJ_TYPENAME) 
                return new DateRangeReferent();
            return null;
        }
        public override int ProgressWeight
        {
            get
            {
                return 10;
            }
        }
        public override Pullenti.Ner.Core.AnalyzerData CreateAnalyzerData()
        {
            return new Pullenti.Ner.Date.Internal.DateAnalyzerData();
        }
        internal static Pullenti.Ner.Date.Internal.DateAnalyzerData GetData(Pullenti.Ner.Token t)
        {
            if (t == null) 
                return null;
            return t.Kit.GetAnalyzerDataByAnalyzerName(ANALYZER_NAME) as Pullenti.Ner.Date.Internal.DateAnalyzerData;
        }
        public override void Process(Pullenti.Ner.Core.AnalysisKit kit)
        {
            Pullenti.Ner.Date.Internal.DateAnalyzerData ad = kit.GetAnalyzerData(this) as Pullenti.Ner.Date.Internal.DateAnalyzerData;
            Pullenti.Ner.Date.Internal.DateItemToken.SpeedRegime = false;
            Pullenti.Ner.Date.Internal.DateItemToken.PrepareAllData(kit.FirstToken);
            ad.DRegime = true;
            for (Pullenti.Ner.Token t = kit.FirstToken; t != null; t = t.Next) 
            {
                if (t.IsIgnored) 
                    continue;
                List<Pullenti.Ner.ReferentToken> rts = null;
                Pullenti.Ner.Date.Internal.DateItemToken about = null;
                Pullenti.Ner.Token t1 = null;
                List<Pullenti.Ner.Date.Internal.DateItemToken> pli = Pullenti.Ner.Date.Internal.DateItemToken.TryParseList(t, 20);
                if (pli == null || pli.Count == 0) 
                {
                }
                else 
                {
                    bool high = false;
                    for (Pullenti.Ner.Token tt = t.Previous; tt != null; tt = tt.Previous) 
                    {
                        if (tt.IsValue("ДАТА", null) || tt.IsValue("DATE", null) || tt.IsValue("ВЫДАТЬ", null)) 
                        {
                            high = true;
                            break;
                        }
                        if (tt.IsChar(':') || tt.IsHiphen) 
                            continue;
                        if (tt.IsNewlineAfter) 
                            break;
                        if (tt.GetReferent() is DateReferent) 
                        {
                            high = true;
                            break;
                        }
                        if (!(tt is Pullenti.Ner.TextToken)) 
                            break;
                        if (!(tt.Morph.Case.IsGenitive)) 
                            break;
                    }
                    if (!high && IsUserParamDate(t)) 
                        high = true;
                    if (pli.Count > 1 && pli[0].Ptr == DatePointerType.About) 
                    {
                        about = pli[0];
                        pli.RemoveAt(0);
                    }
                    rts = TryAttach(pli, high);
                    if ((rts == null && pli.Count > 2 && pli[2].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Delim) && pli[2].BeginToken.IsComma) 
                    {
                        pli.RemoveRange(2, pli.Count - 2);
                        rts = TryAttach(pli, high);
                    }
                    if (rts == null && IsUserParamDate(t)) 
                    {
                        for (int i = 0; i < (pli.Count - 2); i++) 
                        {
                            if (pli[i].Typ != Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Number) 
                                break;
                            else if (pli[i + 1].CanBeDay && pli[i + 2].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Month) 
                            {
                                List<Pullenti.Ner.Date.Internal.DateItemToken> pli1 = new List<Pullenti.Ner.Date.Internal.DateItemToken>();
                                pli1.AddRange(pli);
                                pli1.RemoveRange(0, i + 1);
                                rts = TryAttach(pli1, high);
                                break;
                            }
                        }
                    }
                    t1 = pli[pli.Count - 1].EndToken;
                }
                if (rts == null) 
                {
                    if (rts == null) 
                    {
                        if (t1 != null) 
                            t = t1;
                        continue;
                    }
                }
                DateReferent dat = null;
                DateReferent hi = null;
                for (int i = 0; i < rts.Count; i++) 
                {
                    Pullenti.Ner.ReferentToken rt = rts[i];
                    if (rt.Referent is DateRangeReferent) 
                    {
                        DateRangeReferent dr = rt.Referent as DateRangeReferent;
                        if (dr.DateFrom != null) 
                            dr.DateFrom = ad.RegisterReferent(dr.DateFrom) as DateReferent;
                        if (dr.DateTo != null) 
                            dr.DateTo = ad.RegisterReferent(dr.DateTo) as DateReferent;
                        rt.Referent = ad.RegisterReferent(rt.Referent);
                        if (rt.BeginToken.Previous != null && rt.BeginToken.Previous.IsValue("ПЕРИОД", null)) 
                            rt.BeginToken = rt.BeginToken.Previous;
                        kit.EmbedToken(rt);
                        t = rt;
                        break;
                    }
                    DateReferent dt = rt.Referent as DateReferent;
                    if (dt.Higher != null) 
                        dt.Higher = ad.RegisterReferent(dt.Higher) as DateReferent;
                    rt.Referent = ad.RegisterReferent(dt);
                    hi = rt.Referent as DateReferent;
                    if ((i < (rts.Count - 1)) && rt.Tag == null) 
                        rt.Referent.AddOccurence(new Pullenti.Ner.TextAnnotation() { Sofa = kit.Sofa, BeginChar = rt.BeginChar, EndChar = rt.EndChar, OccurenceOf = rt.Referent });
                    else 
                    {
                        dat = rt.Referent as DateReferent;
                        if (about != null) 
                        {
                            if (rt.BeginChar > about.BeginChar) 
                                rt.BeginToken = about.BeginToken;
                            dat.Pointer = DatePointerType.About;
                        }
                        kit.EmbedToken(rt);
                        t = rt;
                        for (int j = i + 1; j < rts.Count; j++) 
                        {
                            if (rts[j].BeginChar == t.BeginChar) 
                                rts[j].BeginToken = t;
                            if (rts[j].EndChar == t.EndChar) 
                                rts[j].EndToken = t;
                        }
                    }
                }
                if ((dat != null && t.Previous != null && t.Previous.IsHiphen) && t.Previous.Previous != null && (t.Previous.Previous.GetReferent() is DateReferent)) 
                {
                    DateReferent dat0 = t.Previous.Previous.GetReferent() as DateReferent;
                    DateRangeReferent dr = ad.RegisterReferent(new DateRangeReferent() { DateFrom = dat0, DateTo = dat }) as DateRangeReferent;
                    Pullenti.Ner.ReferentToken diap = new Pullenti.Ner.ReferentToken(dr, t.Previous.Previous, t);
                    kit.EmbedToken(diap);
                    t = diap;
                    continue;
                }
                if ((dat != null && t.Previous != null && ((t.Previous.IsHiphen || t.Previous.IsValue("ПО", null) || t.Previous.IsValue("И", null)))) && (t.Previous.Previous is Pullenti.Ner.NumberToken) && (t.Previous.Previous as Pullenti.Ner.NumberToken).IntValue != null) 
                {
                    Pullenti.Ner.Token t0 = t.Previous.Previous;
                    DateReferent dat0 = null;
                    int num = (t0 as Pullenti.Ner.NumberToken).IntValue.Value;
                    if (dat.Day > 0 && (num < dat.Day) && num > 0) 
                    {
                        if (dat.Higher != null) 
                            dat0 = new DateReferent() { Higher = dat.Higher, Day = num };
                        else if (dat.Month > 0) 
                            dat0 = new DateReferent() { Month = dat.Month, Day = num };
                    }
                    else if (dat.Year > 0 && (num < dat.Year) && ((num > 1000 || ((t.Previous.Previous.Previous != null && t.Previous.Previous.Previous.IsValue("С", null)))))) 
                        dat0 = new DateReferent() { Year = num };
                    else if ((dat.Year < 0) && num > (-dat.Year)) 
                        dat0 = new DateReferent() { Year = -num };
                    if (dat0 != null) 
                    {
                        Pullenti.Ner.ReferentToken rt0 = new Pullenti.Ner.ReferentToken(ad.RegisterReferent(dat0), t0, t0);
                        kit.EmbedToken(rt0);
                        if (!t.Previous.IsHiphen && !t.Previous.IsValue("ПО", null)) 
                            continue;
                        dat0 = rt0.Referent as DateReferent;
                        DateRangeReferent dr = ad.RegisterReferent(new DateRangeReferent() { DateFrom = dat0, DateTo = dat }) as DateRangeReferent;
                        Pullenti.Ner.ReferentToken diap = new Pullenti.Ner.ReferentToken(dr, rt0, t);
                        if (diap.BeginToken.Previous != null && diap.BeginToken.Previous.IsValue("С", null)) 
                            diap.BeginToken = diap.BeginToken.Previous;
                        kit.EmbedToken(diap);
                        t = diap;
                        continue;
                    }
                }
            }
            ApplyDateRange0(kit, ad);
            for (Pullenti.Ner.Token t = kit.FirstToken; t != null; t = t.Next) 
            {
                if (t.IsIgnored) 
                    continue;
                Pullenti.Ner.Date.Internal.DateExToken det = Pullenti.Ner.Date.Internal.DateExToken.TryParse(t);
                if (det == null) 
                    continue;
                bool rel = false;
                foreach (Pullenti.Ner.Date.Internal.DateExToken.DateExItemToken it in det.ItemsFrom) 
                {
                    if (it.IsValueRelate) 
                        rel = true;
                }
                foreach (Pullenti.Ner.Date.Internal.DateExToken.DateExItemToken it in det.ItemsTo) 
                {
                    if (it.IsValueRelate) 
                        rel = true;
                }
                if (!rel) 
                {
                    t = det.EndToken;
                    continue;
                }
                List<Pullenti.Ner.ReferentToken> rts = Pullenti.Ner.Date.Internal.DateRelHelper.CreateReferents(det);
                if (rts == null || rts.Count == 0) 
                    continue;
                Pullenti.Ner.Referent root = rts[0].Tag as Pullenti.Ner.Referent;
                for (int i = 0; i < rts.Count; i++) 
                {
                    Pullenti.Ner.ReferentToken rt = rts[i];
                    Pullenti.Ner.Referent old = rt.Referent;
                    rt.Referent = ad.RegisterReferent(rt.Referent);
                    if (old == root) 
                        root = rt.Referent;
                    if (old != rt.Referent) 
                    {
                        for (int j = i + 1; j < rts.Count; j++) 
                        {
                            foreach (Pullenti.Ner.Slot s in rts[j].Referent.Slots) 
                            {
                                if (s.Value == old) 
                                    s.Value = rt.Referent;
                            }
                        }
                    }
                    if (root != null) 
                    {
                        foreach (Pullenti.Ner.Slot s in root.Slots) 
                        {
                            if (s.Value == old) 
                                s.Value = rt.Referent;
                        }
                    }
                    if (rt.Referent == root) 
                    {
                        if (rt.BeginChar > t.BeginChar) 
                            rt.BeginToken = t;
                        if (rt.EndChar < det.EndChar) 
                            rt.EndToken = det.EndToken;
                        root = null;
                    }
                    kit.EmbedToken(rt);
                    t = rt;
                    for (int j = i + 1; j < rts.Count; j++) 
                    {
                        if (rts[j].BeginChar == t.BeginChar) 
                            rts[j].BeginToken = t;
                        if (rts[j].EndChar == t.EndChar) 
                            rts[j].EndToken = t;
                    }
                }
                if (root != null) 
                {
                    if (t.BeginChar > det.BeginChar || (t.EndChar < det.EndChar)) 
                    {
                        Pullenti.Ner.ReferentToken rt = new Pullenti.Ner.ReferentToken(root, (t.BeginChar > det.BeginChar ? det.BeginToken : t), (t.EndChar < det.EndChar ? det.EndToken : t));
                        kit.EmbedToken(rt);
                        t = rt;
                    }
                }
            }
            ad.DRegime = false;
        }
        public override Pullenti.Ner.ReferentToken ProcessReferent(Pullenti.Ner.Token begin, string param)
        {
            return ProcessReferentStat(begin, param);
        }
        public static Pullenti.Ner.ReferentToken ProcessReferentStat(Pullenti.Ner.Token begin, string param = null)
        {
            if (begin == null) 
                return null;
            Pullenti.Ner.Date.Internal.DateAnalyzerData ad = GetData(begin);
            if (ad == null) 
                return null;
            if (ad.Level > 2) 
                return null;
            if (begin.IsValue("ДО", null) && (begin.Next is Pullenti.Ner.ReferentToken) && (begin.Next.GetReferent() is DateReferent)) 
            {
                DateRangeReferent drr = new DateRangeReferent() { DateTo = begin.Next.GetReferent() as DateReferent };
                Pullenti.Ner.ReferentToken res1 = new Pullenti.Ner.ReferentToken(drr, begin, begin.Next);
                if (res1.EndToken.Next != null && res1.EndToken.Next.IsValue("ВКЛЮЧИТЕЛЬНО", null)) 
                    res1.EndToken = res1.EndToken.Next;
                else 
                    drr.AddSlot("@EXCLUDE", "true", true, 0);
                res1.Data = ad;
                return res1;
            }
            if (begin.IsValue("ПОСЛЕ", null) && (begin.Next is Pullenti.Ner.ReferentToken) && (begin.Next.GetReferent() is DateReferent)) 
            {
                DateRangeReferent drr = new DateRangeReferent() { DateFrom = begin.Next.GetReferent() as DateReferent };
                Pullenti.Ner.ReferentToken res1 = new Pullenti.Ner.ReferentToken(drr, begin, begin.Next);
                if (res1.EndToken.Next != null && res1.EndToken.Next.IsValue("ВКЛЮЧИТЕЛЬНО", null)) 
                    res1.EndToken = res1.EndToken.Next;
                else 
                    drr.AddSlot("@EXCLUDE", "true", true, 0);
                res1.Data = ad;
                return res1;
            }
            ad.Level++;
            List<Pullenti.Ner.Date.Internal.DateItemToken> pli = Pullenti.Ner.Date.Internal.DateItemToken.TryParseList(begin, 10);
            ad.Level--;
            if (pli == null || pli.Count == 0) 
                return null;
            ad.Level++;
            List<Pullenti.Ner.ReferentToken> rts = TryAttach(pli, true);
            ad.Level--;
            if (rts == null || rts.Count == 0) 
                return null;
            Pullenti.Ner.ReferentToken res = rts[rts.Count - 1];
            for (int i = 0; i < (rts.Count - 1); i++) 
            {
                if ((res.Referent is DateReferent) && (rts[i].Referent is DateReferent)) 
                    res.Referent.MergeSlots(rts[i].Referent, true);
                else 
                    rts[i].Data = ad;
            }
            res.Referent.AddSlot(DateReferent.ATTR_HIGHER, null, true, 0);
            res.Data = ad;
            return res;
        }
        static List<Pullenti.Ner.ReferentToken> TryAttach(List<Pullenti.Ner.Date.Internal.DateItemToken> dts, bool high)
        {
            if (dts == null || dts.Count == 0) 
                return null;
            if ((dts[0].CanBeHour && dts.Count > 2 && dts[1].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Delim) && dts[2].IntValue >= 0 && (dts[2].IntValue < 60)) 
            {
                if (dts[0].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Hour || ((dts[0].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Number && ((dts[2].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Hour || dts[2].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Number))))) 
                {
                    if (dts.Count > 3 && dts[3].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Delim && dts[3].StringValue == dts[1].StringValue) 
                    {
                    }
                    else if (!IsUserParamIgnoreTime(dts[0])) 
                    {
                        List<Pullenti.Ner.Date.Internal.DateItemToken> dts1 = new List<Pullenti.Ner.Date.Internal.DateItemToken>(dts);
                        dts1.RemoveRange(0, 3);
                        List<Pullenti.Ner.ReferentToken> res1 = TryAttach(dts1, false);
                        if (res1 != null && (res1[res1.Count - 1].Referent is DateReferent) && (res1[res1.Count - 1].Referent as DateReferent).Day > 0) 
                        {
                            DateReferent time = new DateReferent() { Hour = dts[0].IntValue, Minute = dts[2].IntValue };
                            time.Higher = res1[res1.Count - 1].Referent as DateReferent;
                            res1.Add(new Pullenti.Ner.ReferentToken(time, dts[0].BeginToken, res1[res1.Count - 1].EndToken));
                            return res1;
                        }
                    }
                }
            }
            if ((dts[0].CanBeDay && dts.Count > 6 && dts[1].StringValue == "-") && dts[2].CanBeDay && (dts[0].IntValue < dts[2].IntValue)) 
            {
                List<Pullenti.Ner.Date.Internal.DateItemToken> dts1 = new List<Pullenti.Ner.Date.Internal.DateItemToken>(dts);
                dts1.RemoveRange(0, 2);
                List<Pullenti.Ner.ReferentToken> res1 = TryAttach(dts1, false);
                if (res1 != null && res1[res1.Count - 1].BeginToken == dts[2].BeginToken && (res1[res1.Count - 1].Referent as DateReferent).Day > 0) 
                {
                    DateReferent day1 = res1[res1.Count - 1].Referent as DateReferent;
                    DateReferent day0 = new DateReferent();
                    day0.Higher = day1.Higher;
                    day0.Day = dts[0].IntValue;
                    Pullenti.Ner.Token tend = res1[res1.Count - 1].EndToken;
                    res1.Add(new Pullenti.Ner.ReferentToken(day0, dts[0].BeginToken, dts[0].EndToken));
                    DateRangeReferent ra = new DateRangeReferent();
                    ra.DateFrom = day0;
                    ra.DateTo = day1;
                    res1.Add(new Pullenti.Ner.ReferentToken(ra, dts[0].BeginToken, tend));
                    return res1;
                }
            }
            if (dts[0].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Hour && dts.Count > 2 && dts[1].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Minute) 
            {
                int ii = 2;
                if ((ii < dts.Count) && dts[ii].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Second) 
                    ii++;
                List<Pullenti.Ner.Date.Internal.DateItemToken> dts1 = new List<Pullenti.Ner.Date.Internal.DateItemToken>(dts);
                dts1.RemoveRange(0, ii);
                List<Pullenti.Ner.ReferentToken> res1 = TryAttach(dts1, false);
                if (res1 != null && (res1[res1.Count - 1].Referent is DateReferent) && (res1[res1.Count - 1].Referent as DateReferent).Day > 0) 
                {
                    DateReferent time = new DateReferent() { Hour = dts[0].IntValue, Minute = dts[1].IntValue };
                    if (ii > 2) 
                        time.Second = dts[2].IntValue;
                    time.Higher = res1[res1.Count - 1].Referent as DateReferent;
                    res1.Add(new Pullenti.Ner.ReferentToken(time, dts[0].BeginToken, res1[res1.Count - 1].EndToken));
                    return res1;
                }
            }
            if ((dts[0].CanBeDay && dts.Count > 4 && dts[1].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Delim) && dts[1].BeginToken.IsCommaAnd && dts[2].CanBeDay) 
            {
                bool hasMonth = false;
                bool hasYear = false;
                for (int kk = 0; kk < 2; kk++) 
                {
                    for (int ii = 3; ii < dts.Count; ii++) 
                    {
                        if (dts[ii].CanBeDay || ((dts[ii].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Delim && dts[ii].BeginToken.IsCommaAnd))) 
                        {
                        }
                        else if (dts[ii].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Month) 
                            hasMonth = true;
                        else if (dts[ii].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Year) 
                        {
                            hasYear = true;
                            break;
                        }
                        else 
                            break;
                    }
                    if (hasYear) 
                        break;
                    if (!hasMonth || kk > 0) 
                        break;
                    if (dts.Count < 17) 
                        break;
                    List<Pullenti.Ner.Date.Internal.DateItemToken> dts1 = Pullenti.Ner.Date.Internal.DateItemToken.TryParseList(dts[0].BeginToken, 100);
                    if (dts1 != null && dts1.Count > dts.Count) 
                        dts = dts1;
                    else 
                        break;
                }
                if (hasYear && hasMonth) 
                {
                    List<Pullenti.Ner.Date.Internal.DateItemToken> dts2 = new List<Pullenti.Ner.Date.Internal.DateItemToken>(dts);
                    dts2.RemoveRange(0, 2);
                    List<Pullenti.Ner.ReferentToken> res2 = TryAttach(dts2, high);
                    if (res2 != null && (res2[res2.Count - 1].Referent is DateReferent)) 
                    {
                        DateReferent dd = new DateReferent();
                        dd.Day = dts[0].IntValue;
                        dd.Higher = (res2[res2.Count - 1].Referent as DateReferent).Higher;
                        res2.Add(new Pullenti.Ner.ReferentToken(dd, dts[0].BeginToken, dts[0].EndToken));
                        return res2;
                    }
                }
            }
            if (((dts[0].CanBeDay && dts.Count > 5 && dts[1].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Month) && dts[2].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Delim && dts[2].BeginToken.IsCommaAnd) && dts[3].CanBeDay) 
            {
                bool hasMonth = false;
                bool hasYear = false;
                for (int kk = 0; kk < 2; kk++) 
                {
                    for (int ii = 3; ii < dts.Count; ii++) 
                    {
                        if (dts[ii].CanBeDay || ((dts[ii].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Delim && dts[ii].BeginToken.IsCommaAnd))) 
                        {
                        }
                        else if (dts[ii].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Month) 
                            hasMonth = true;
                        else if (dts[ii].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Year) 
                        {
                            hasYear = true;
                            break;
                        }
                        else 
                            break;
                    }
                    if (hasYear) 
                        break;
                    if (!hasMonth || kk > 0) 
                        break;
                    if (dts.Count < 17) 
                        break;
                    List<Pullenti.Ner.Date.Internal.DateItemToken> dts1 = Pullenti.Ner.Date.Internal.DateItemToken.TryParseList(dts[0].BeginToken, 100);
                    if (dts1 != null && dts1.Count > dts.Count) 
                        dts = dts1;
                    else 
                        break;
                }
                if (hasYear && hasMonth) 
                {
                    List<Pullenti.Ner.Date.Internal.DateItemToken> dts2 = new List<Pullenti.Ner.Date.Internal.DateItemToken>(dts);
                    dts2.RemoveRange(0, 3);
                    List<Pullenti.Ner.ReferentToken> res2 = TryAttach(dts2, high);
                    if (res2 != null && (res2[res2.Count - 1].Referent is DateReferent)) 
                    {
                        DateReferent yy = (res2[res2.Count - 1].Referent as DateReferent).Higher;
                        if (yy != null) 
                            yy = yy.Higher;
                        if (yy != null && yy.Year > 0) 
                        {
                            DateReferent mm = new DateReferent();
                            mm.Month = dts[1].IntValue;
                            mm.Higher = yy;
                            res2.Add(new Pullenti.Ner.ReferentToken(mm, dts[1].BeginToken, dts[1].EndToken));
                            DateReferent dd = new DateReferent();
                            dd.Day = dts[0].IntValue;
                            dd.Higher = mm;
                            res2.Add(new Pullenti.Ner.ReferentToken(dd, dts[0].BeginToken, dts[1].EndToken));
                            return res2;
                        }
                    }
                }
            }
            Pullenti.Ner.Date.Internal.DateItemToken year;
            Pullenti.Ner.Date.Internal.DateItemToken mon;
            Pullenti.Ner.Date.Internal.DateItemToken day;
            Pullenti.Ner.Date.Internal.DateItemToken cent = null;
            Pullenti.Ner.Date.Internal.DateItemToken tenyears = null;
            Pullenti.Ner.Date.Internal.DateItemToken point = null;
            bool yearIsDif = false;
            bool b = false;
            b = ApplyRuleFormal(dts, high, out year, out mon, out day);
            if (b) 
            {
                Pullenti.Ner.Token tt = dts[0].BeginToken.Previous;
                if (tt != null) 
                {
                    if (tt.IsValue("№", null) || tt.IsValue("N", null)) 
                        b = false;
                }
            }
            if (dts[0].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Century) 
            {
                if (dts.Count == 1) 
                {
                    if (dts[0].BeginToken is Pullenti.Ner.NumberToken) 
                        return null;
                    if (Pullenti.Ner.Core.NumberHelper.TryParseRoman(dts[0].BeginToken) == null) 
                        return null;
                }
                cent = dts[0];
                b = true;
            }
            else if (dts[0].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Tenyears) 
            {
                tenyears = dts[0];
                b = true;
            }
            if (dts.Count == 1 && dts[0].Ptr == DatePointerType.Today) 
            {
                List<Pullenti.Ner.ReferentToken> res0 = new List<Pullenti.Ner.ReferentToken>();
                res0.Add(new Pullenti.Ner.ReferentToken(new DateReferent() { Pointer = DatePointerType.Today }, dts[0].BeginToken, dts[0].EndToken));
                return res0;
            }
            if (dts.Count == 1 && dts[0].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Year && dts[0].Year <= 0) 
            {
                List<Pullenti.Ner.ReferentToken> res0 = new List<Pullenti.Ner.ReferentToken>();
                res0.Add(new Pullenti.Ner.ReferentToken(new DateReferent() { Pointer = DatePointerType.Undefined }, dts[0].BeginToken, dts[0].EndToken));
                return res0;
            }
            if (!b && dts[0].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Pointer && dts.Count > 1) 
            {
                if (dts[1].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Year) 
                {
                    year = dts[1];
                    point = dts[0];
                    b = true;
                }
                else if (dts[1].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Century) 
                {
                    cent = dts[1];
                    point = dts[0];
                    b = true;
                }
                else if (dts[1].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Tenyears) 
                {
                    tenyears = dts[1];
                    point = dts[0];
                    b = true;
                }
                else if (dts[1].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Month) 
                {
                    mon = dts[1];
                    point = dts[0];
                    if (dts.Count > 2 && ((dts[2].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Year || dts[2].CanBeYear))) 
                        year = dts[2];
                    b = true;
                }
            }
            if (!b) 
                b = ApplyRuleWithMonth(dts, high, out year, out mon, out day, out yearIsDif);
            if (!b) 
                b = ApplyRuleYearOnly(dts, out year, out mon, out day);
            if (!b) 
            {
                if (dts.Count == 2 && dts[0].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Hour && dts[1].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Minute) 
                {
                    Pullenti.Ner.Token t00 = dts[0].BeginToken.Previous;
                    if (t00 != null && (((t00.IsValue("ТЕЧЕНИЕ", null) || t00.IsValue("ПРОТЯГОМ", null) || t00.IsValue("ЧЕРЕЗ", null)) || t00.IsValue("ТЕЧІЮ", null)))) 
                    {
                    }
                    else 
                    {
                        List<Pullenti.Ner.ReferentToken> res0 = new List<Pullenti.Ner.ReferentToken>();
                        DateReferent time = new DateReferent() { Hour = dts[0].IntValue, Minute = dts[1].IntValue };
                        res0.Add(new Pullenti.Ner.ReferentToken(time, dts[0].BeginToken, dts[1].EndToken));
                        int cou = 0;
                        for (Pullenti.Ner.Token tt = dts[0].BeginToken.Previous; tt != null && (cou < 1000); tt = tt.Previous,cou++) 
                        {
                            if (tt.GetReferent() is DateReferent) 
                            {
                                DateReferent dr = tt.GetReferent() as DateReferent;
                                if (dr.FindSlot(DateReferent.ATTR_DAY, null, true) == null && dr.Higher != null) 
                                    dr = dr.Higher;
                                if (dr.FindSlot(DateReferent.ATTR_DAY, null, true) != null) 
                                {
                                    time.Higher = dr;
                                    break;
                                }
                            }
                        }
                        return res0;
                    }
                }
                if ((dts.Count == 4 && dts[0].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Month && dts[1].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Delim) && dts[2].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Month && dts[3].CanBeYear) 
                {
                    List<Pullenti.Ner.ReferentToken> res0 = new List<Pullenti.Ner.ReferentToken>();
                    DateReferent yea = new DateReferent() { Year = dts[3].IntValue };
                    res0.Add(new Pullenti.Ner.ReferentToken(yea, dts[3].BeginToken, dts[3].EndToken) { Morph = dts[3].Morph });
                    DateReferent mon1 = new DateReferent() { Month = dts[0].IntValue, Higher = yea };
                    res0.Add(new Pullenti.Ner.ReferentToken(mon1, dts[0].BeginToken, dts[0].EndToken) { Tag = mon1 });
                    DateReferent mon2 = new DateReferent() { Month = dts[2].IntValue, Higher = yea };
                    res0.Add(new Pullenti.Ner.ReferentToken(mon2, dts[2].BeginToken, dts[3].EndToken));
                    return res0;
                }
                if (((dts.Count >= 4 && dts[0].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Number && dts[0].CanBeDay) && dts[1].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Delim && dts[2].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Number) && dts[2].CanBeDay && dts[3].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Month) 
                {
                    if (dts.Count == 4 || ((dts.Count == 5 && dts[4].CanBeYear))) 
                    {
                        List<Pullenti.Ner.ReferentToken> res0 = new List<Pullenti.Ner.ReferentToken>();
                        DateReferent yea = null;
                        if (dts.Count == 5) 
                            res0.Add(new Pullenti.Ner.ReferentToken((yea = new DateReferent() { Year = dts[4].Year }), dts[4].BeginToken, dts[4].EndToken));
                        DateReferent mo = new DateReferent() { Month = dts[3].IntValue, Higher = yea };
                        res0.Add(new Pullenti.Ner.ReferentToken(mo, dts[3].BeginToken, dts[dts.Count - 1].EndToken));
                        DateReferent da1 = new DateReferent() { Day = dts[0].IntValue, Higher = mo };
                        res0.Add(new Pullenti.Ner.ReferentToken(da1, dts[0].BeginToken, dts[0].EndToken));
                        DateReferent da2 = new DateReferent() { Day = dts[2].IntValue, Higher = mo };
                        res0.Add(new Pullenti.Ner.ReferentToken(da2, dts[2].BeginToken, dts[dts.Count - 1].EndToken));
                        DateRangeReferent dr = new DateRangeReferent();
                        dr.DateFrom = da1;
                        dr.DateTo = da2;
                        res0.Add(new Pullenti.Ner.ReferentToken(dr, dts[0].BeginToken, dts[dts.Count - 1].EndToken));
                        return res0;
                    }
                }
                if ((dts.Count >= 3 && dts[0].CanByMonth && dts[1].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Delim) && dts[2].CanBeYear && dts[1].StringValue == ".") 
                {
                    if (((dts.Count >= 7 && dts[3].BeginToken.IsHiphen && dts[4].CanByMonth) && dts[5].StringValue == "." && dts[6].CanBeYear) && dts[2].IntValue <= dts[6].IntValue) 
                    {
                        List<Pullenti.Ner.ReferentToken> res0 = new List<Pullenti.Ner.ReferentToken>();
                        DateReferent yea1 = new DateReferent() { Year = dts[2].Year };
                        res0.Add(new Pullenti.Ner.ReferentToken(yea1, dts[2].BeginToken, dts[2].EndToken));
                        DateReferent mon1 = new DateReferent() { Month = dts[0].IntValue, Higher = yea1 };
                        res0.Add(new Pullenti.Ner.ReferentToken(mon1, dts[0].BeginToken, dts[2].EndToken));
                        DateReferent yea2 = new DateReferent() { Year = dts[6].Year };
                        res0.Add(new Pullenti.Ner.ReferentToken(yea2, dts[6].BeginToken, dts[6].EndToken));
                        DateReferent mon2 = new DateReferent() { Month = dts[4].IntValue, Higher = yea2 };
                        res0.Add(new Pullenti.Ner.ReferentToken(mon2, dts[4].BeginToken, dts[6].EndToken));
                        DateRangeReferent dr = new DateRangeReferent();
                        dr.DateFrom = mon1;
                        dr.DateTo = mon2;
                        res0.Add(new Pullenti.Ner.ReferentToken(dr, dts[0].BeginToken, dts[6].EndToken));
                        return res0;
                    }
                    bool ok2 = false;
                    if (dts.Count == 5 && dts[3].BeginToken.IsHiphen && dts[4].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Pointer) 
                        ok2 = true;
                    if ((dts.Count == 3 && dts[2].EndToken.Next != null && dts[2].EndToken.Next.IsHiphen) && dts[2].EndToken.Next.Next != null && dts[2].EndToken.Next.Next.IsValue("ПО", null)) 
                        ok2 = true;
                    if (ok2) 
                    {
                        List<Pullenti.Ner.ReferentToken> res0 = new List<Pullenti.Ner.ReferentToken>();
                        DateReferent yea1 = new DateReferent() { Year = dts[2].Year };
                        res0.Add(new Pullenti.Ner.ReferentToken(yea1, dts[2].BeginToken, dts[2].EndToken));
                        DateReferent mon1 = new DateReferent() { Month = dts[0].IntValue, Higher = yea1 };
                        res0.Add(new Pullenti.Ner.ReferentToken(mon1, dts[0].BeginToken, dts[2].EndToken));
                        Pullenti.Ner.Token tt2 = dts[2].EndToken.Next.Next;
                        if (tt2.IsValue("ПО", null)) 
                            tt2 = tt2.Next;
                        List<Pullenti.Ner.Date.Internal.DateItemToken> dts2 = Pullenti.Ner.Date.Internal.DateItemToken.TryParseList(tt2, 20);
                        if (dts2 != null && dts2.Count == 1 && dts2[0].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Pointer) 
                        {
                            List<Pullenti.Ner.ReferentToken> nows = TryAttach(dts2, false);
                            if (nows != null && nows.Count == 1 && (nows[0].Referent is DateReferent)) 
                            {
                                DateRangeReferent dr = new DateRangeReferent();
                                dr.DateFrom = mon1;
                                dr.DateTo = nows[0].Referent as DateReferent;
                                res0.Add(nows[0]);
                                res0.Add(new Pullenti.Ner.ReferentToken(dr, dts[0].BeginToken, nows[0].EndToken));
                            }
                        }
                        return res0;
                    }
                }
                if ((dts[0].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Month && dts.Count == 1 && dts[0].EndToken.Next != null) && ((dts[0].EndToken.Next.IsHiphen || dts[0].EndToken.Next.IsValue("ПО", null) || dts[0].EndToken.Next.IsValue("НА", null)))) 
                {
                    Pullenti.Ner.ReferentToken rt = DateAnalyzer.ProcessReferentStat(dts[0].EndToken.Next.Next, null);
                    if (rt != null) 
                    {
                        DateReferent dr0 = rt.Referent as DateReferent;
                        if ((dr0 != null && dr0.Year > 0 && dr0.Month > 0) && dr0.Day == 0 && dr0.Month > dts[0].IntValue) 
                        {
                            DateReferent drYear0 = new DateReferent() { Year = dr0.Year };
                            List<Pullenti.Ner.ReferentToken> res0 = new List<Pullenti.Ner.ReferentToken>();
                            res0.Add(new Pullenti.Ner.ReferentToken(drYear0, dts[0].EndToken, dts[0].EndToken));
                            DateReferent drMon0 = new DateReferent() { Month = dts[0].IntValue, Higher = drYear0 };
                            res0.Add(new Pullenti.Ner.ReferentToken(drMon0, dts[0].BeginToken, dts[0].EndToken));
                            return res0;
                        }
                    }
                }
                if (((dts.Count == 3 && dts[1].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Delim && dts[1].BeginToken.IsHiphen) && dts[0].CanBeYear && dts[2].CanBeYear) && (dts[0].IntValue < dts[2].IntValue)) 
                {
                    bool ok = false;
                    if (dts[2].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Year) 
                        ok = true;
                    else if (dts[0].LengthChar == 4 && dts[2].LengthChar == 4 && dts[0].BeginToken.Previous != null) 
                    {
                        Pullenti.Ner.Token tt0 = dts[0].BeginToken.Previous;
                        if (tt0.IsChar('(') && dts[2].EndToken.Next != null && dts[2].EndToken.Next.IsChar(')')) 
                            ok = true;
                        else if (tt0.IsValue("IN", null) || tt0.IsValue("SINCE", null) || tt0.IsValue("В", "У")) 
                            ok = true;
                    }
                    if (ok) 
                    {
                        List<Pullenti.Ner.ReferentToken> res0 = new List<Pullenti.Ner.ReferentToken>();
                        res0.Add(new Pullenti.Ner.ReferentToken(new DateReferent() { Year = dts[0].Year }, dts[0].BeginToken, dts[0].EndToken));
                        res0.Add(new Pullenti.Ner.ReferentToken(new DateReferent() { Year = dts[2].Year }, dts[2].BeginToken, dts[2].EndToken));
                        return res0;
                    }
                }
                if (dts.Count > 1 && dts[0].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Year) 
                {
                    List<Pullenti.Ner.ReferentToken> res0 = new List<Pullenti.Ner.ReferentToken>();
                    res0.Add(new Pullenti.Ner.ReferentToken(new DateReferent() { Year = dts[0].Year }, dts[0].BeginToken, dts[0].EndToken));
                    return res0;
                }
                if (dts[0].LTyp != Pullenti.Ner.Date.Internal.DateItemToken.FirstLastTyp.No && dts.Count > 1) 
                    high = true;
                if (((dts.Count == 5 && dts[0].CanBeDay && dts[1].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Month) && dts[2].CanBeHour && dts[3].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Delim) && dts[4].CanBeMinute) 
                {
                    List<Pullenti.Ner.ReferentToken> res0 = new List<Pullenti.Ner.ReferentToken>();
                    DateReferent mm = new DateReferent() { Month = dts[1].IntValue };
                    res0.Add(new Pullenti.Ner.ReferentToken(mm, dts[1].BeginToken, dts[1].EndToken));
                    DateReferent dd = new DateReferent() { Day = dts[0].IntValue, Higher = mm };
                    res0.Add(new Pullenti.Ner.ReferentToken(dd, dts[0].BeginToken, dts[2].EndToken));
                    DateReferent hh = new DateReferent() { Hour = dts[2].IntValue, Minute = dts[4].IntValue, Higher = dd };
                    res0.Add(new Pullenti.Ner.ReferentToken(hh, dts[0].BeginToken, dts[4].EndToken));
                    return res0;
                }
                if (high) 
                {
                    if (dts.Count == 1 && dts[0].CanBeYear && dts[0].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Number) 
                    {
                        List<Pullenti.Ner.ReferentToken> res0 = new List<Pullenti.Ner.ReferentToken>();
                        res0.Add(new Pullenti.Ner.ReferentToken(new DateReferent() { Year = dts[0].Year }, dts[0].BeginToken, dts[0].EndToken));
                        return res0;
                    }
                    if ((((dts.Count == 3 && dts[0].CanBeYear && dts[0].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Number) && dts[2].CanBeYear && dts[2].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Number) && (dts[0].Year < dts[2].Year) && dts[1].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Delim) && dts[1].BeginToken.IsHiphen) 
                    {
                        List<Pullenti.Ner.ReferentToken> res0 = new List<Pullenti.Ner.ReferentToken>();
                        DateReferent y1 = new DateReferent() { Year = dts[0].Year };
                        res0.Add(new Pullenti.Ner.ReferentToken(y1, dts[0].BeginToken, dts[0].EndToken));
                        DateReferent y2 = new DateReferent() { Year = dts[2].Year };
                        res0.Add(new Pullenti.Ner.ReferentToken(y2, dts[2].BeginToken, dts[2].EndToken));
                        DateRangeReferent ra = new DateRangeReferent() { DateFrom = y1, DateTo = y2 };
                        res0.Add(new Pullenti.Ner.ReferentToken(ra, dts[0].BeginToken, dts[2].EndToken));
                        return res0;
                    }
                    if (IsUserParamDate(dts[0])) 
                    {
                        if (((dts.Count == 3 && dts[0].CanBeDay && dts[0].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Number) && dts[2].CanByMonth && dts[2].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Number) && dts[1].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Delim && dts[1].BeginToken.IsCharOf("./\\")) 
                        {
                            if (dts[0].BeginToken.Previous != null && dts[0].BeginToken.Previous.IsValue("В", null)) 
                            {
                            }
                            else 
                            {
                                List<Pullenti.Ner.ReferentToken> res0 = new List<Pullenti.Ner.ReferentToken>();
                                DateReferent y1 = new DateReferent() { Month = dts[2].IntValue };
                                res0.Add(new Pullenti.Ner.ReferentToken(y1, dts[2].BeginToken, dts[2].EndToken));
                                DateReferent y2 = new DateReferent() { Day = dts[0].IntValue, Higher = y1 };
                                res0.Add(new Pullenti.Ner.ReferentToken(y2, dts[0].BeginToken, dts[2].EndToken));
                                return res0;
                            }
                        }
                        if (((((dts.Count == 7 && dts[0].CanBeDay && dts[0].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Number) && dts[2].CanByMonth && dts[2].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Number) && dts[1].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Delim && dts[1].BeginToken.IsCharOf("./\\")) && dts[3].BeginToken.IsHiphen && dts[4].CanBeDay) && dts[6].CanByMonth && dts[5].BeginToken.IsCharOf("./\\")) 
                        {
                            List<Pullenti.Ner.ReferentToken> res0 = new List<Pullenti.Ner.ReferentToken>();
                            DateReferent m1 = new DateReferent() { Month = dts[2].IntValue };
                            res0.Add(new Pullenti.Ner.ReferentToken(m1, dts[2].BeginToken, dts[2].EndToken));
                            DateReferent d1 = new DateReferent() { Day = dts[0].IntValue, Higher = m1 };
                            res0.Add(new Pullenti.Ner.ReferentToken(d1, dts[0].BeginToken, dts[2].EndToken));
                            DateReferent m2 = new DateReferent() { Month = dts[6].IntValue };
                            res0.Add(new Pullenti.Ner.ReferentToken(m2, dts[6].BeginToken, dts[6].EndToken));
                            DateReferent d2 = new DateReferent() { Day = dts[4].IntValue, Higher = m2 };
                            res0.Add(new Pullenti.Ner.ReferentToken(d1, dts[4].BeginToken, dts[6].EndToken));
                            DateRangeReferent ra = new DateRangeReferent() { DateFrom = d1, DateTo = d2 };
                            res0.Add(new Pullenti.Ner.ReferentToken(ra, dts[0].BeginToken, dts[6].EndToken));
                            return res0;
                        }
                    }
                }
                if (dts[0].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Quartal || dts[0].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Halfyear || ((dts[0].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Month && dts[0].LTyp != Pullenti.Ner.Date.Internal.DateItemToken.FirstLastTyp.No))) 
                {
                    if (dts.Count == 1 || dts[1].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Year) 
                    {
                        List<Pullenti.Ner.ReferentToken> res0 = new List<Pullenti.Ner.ReferentToken>();
                        int ii = 0;
                        DateReferent yea = null;
                        if (dts.Count > 1) 
                        {
                            ii = 1;
                            yea = new DateReferent() { Year = dts[1].IntValue };
                            res0.Add(new Pullenti.Ner.ReferentToken(yea, dts[1].BeginToken, dts[1].EndToken) { Morph = dts[1].Morph });
                        }
                        else 
                        {
                            int cou = 0;
                            for (Pullenti.Ner.Token tt = dts[0].BeginToken; tt != null; tt = tt.Previous) 
                            {
                                if ((++cou) > 200) 
                                    break;
                                if (tt is Pullenti.Ner.ReferentToken) 
                                {
                                    if ((((yea = _findYear_(tt.GetReferent())))) != null) 
                                        break;
                                }
                                if (tt.IsNewlineBefore) 
                                    break;
                            }
                        }
                        if (yea == null) 
                            return null;
                        int m1 = 0;
                        int m2 = 0;
                        if (dts[0].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Halfyear) 
                        {
                            if (dts[0].IntValue == 2 || dts[0].LTyp == Pullenti.Ner.Date.Internal.DateItemToken.FirstLastTyp.Last) 
                            {
                                m1 = 7;
                                m2 = 12;
                            }
                            else if (dts[0].IntValue == 1) 
                            {
                                m1 = 1;
                                m2 = 6;
                            }
                            else 
                                return null;
                        }
                        else if (dts[0].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Quartal) 
                        {
                            if (dts[0].LTyp == Pullenti.Ner.Date.Internal.DateItemToken.FirstLastTyp.First) 
                            {
                                m1 = 1;
                                m2 = dts[0].IntValue * 3;
                            }
                            else if (dts[0].LTyp == Pullenti.Ner.Date.Internal.DateItemToken.FirstLastTyp.Last) 
                            {
                                m1 = 13 - (dts[0].IntValue * 3);
                                m2 = 12;
                            }
                            else if (dts[0].IntValue == 1) 
                            {
                                m1 = 1;
                                m2 = 3;
                            }
                            else if (dts[0].IntValue == 2) 
                            {
                                m1 = 4;
                                m2 = 6;
                            }
                            else if (dts[0].IntValue == 3) 
                            {
                                m1 = 7;
                                m2 = 9;
                            }
                            else if (dts[0].IntValue == 4 || dts[0].LTyp == Pullenti.Ner.Date.Internal.DateItemToken.FirstLastTyp.Last) 
                            {
                                m1 = 10;
                                m2 = 12;
                            }
                            else 
                                return null;
                        }
                        else if (dts[0].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Month && dts[0].LTyp != Pullenti.Ner.Date.Internal.DateItemToken.FirstLastTyp.No) 
                        {
                            if (dts[0].LTyp == Pullenti.Ner.Date.Internal.DateItemToken.FirstLastTyp.First) 
                            {
                                m1 = 1;
                                m2 = dts[0].IntValue;
                            }
                            else 
                            {
                                m1 = 13 - dts[0].IntValue;
                                m2 = 12;
                            }
                        }
                        else 
                            return null;
                        DateReferent mon1 = new DateReferent() { Month = m1, Higher = yea };
                        res0.Add(new Pullenti.Ner.ReferentToken(mon1, dts[0].BeginToken, dts[0].BeginToken));
                        DateReferent mon2 = new DateReferent() { Month = m2, Higher = yea };
                        res0.Add(new Pullenti.Ner.ReferentToken(mon2, dts[0].EndToken, dts[0].EndToken));
                        DateRangeReferent dr = new DateRangeReferent();
                        dr.DateFrom = mon1;
                        dr.DateTo = mon2;
                        res0.Add(new Pullenti.Ner.ReferentToken(dr, dts[0].BeginToken, dts[ii].EndToken));
                        return res0;
                    }
                }
                if (((dts.Count == 3 && dts[1].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Delim && ((dts[1].StringValue == "." || dts[1].StringValue == ":"))) && dts[0].CanBeHour && dts[2].CanBeMinute) && !IsUserParamIgnoreTime(dts[0])) 
                {
                    bool ok = false;
                    if (dts[0].BeginToken.Previous != null && ((dts[0].BeginToken.Previous.IsValue("В", null) || dts[0].BeginToken.Previous.IsValue("ОКОЛО", null)))) 
                        ok = true;
                    if (ok) 
                    {
                        DateReferent time = new DateReferent() { Hour = dts[0].IntValue, Minute = dts[2].IntValue };
                        Pullenti.Ner.Token t00 = dts[0].BeginToken;
                        int cou = 0;
                        for (Pullenti.Ner.Token tt = dts[0].BeginToken.Previous; tt != null && (cou < 1000); tt = tt.Previous,cou++) 
                        {
                            if (tt.GetReferent() is DateReferent) 
                            {
                                DateReferent dr = tt.GetReferent() as DateReferent;
                                if (dr.FindSlot(DateReferent.ATTR_DAY, null, true) == null && dr.Higher != null) 
                                    dr = dr.Higher;
                                if (dr.FindSlot(DateReferent.ATTR_DAY, null, true) != null) 
                                {
                                    time.Higher = dr;
                                    if ((tt.EndChar + 10) >= t00.BeginChar) 
                                        t00 = tt;
                                    break;
                                }
                            }
                        }
                        Pullenti.Ner.Token tt1 = dts[2].EndToken;
                        if (tt1.Next != null && tt1.Next.IsValue("ЧАС", null)) 
                        {
                            tt1 = tt1.Next;
                            List<Pullenti.Ner.Date.Internal.DateItemToken> dtsli = Pullenti.Ner.Date.Internal.DateItemToken.TryParseList(tt1.Next, 20);
                            if (dtsli != null) 
                            {
                                List<Pullenti.Ner.ReferentToken> res1 = TryAttach(dtsli, true);
                                if (res1 != null && (res1[res1.Count - 1].Referent as DateReferent).Day > 0) 
                                {
                                    time.Higher = res1[res1.Count - 1].Referent as DateReferent;
                                    res1.Add(new Pullenti.Ner.ReferentToken(time, dts[0].BeginToken, tt1));
                                    return res1;
                                }
                            }
                        }
                        Pullenti.Ner.Token tt2 = _corrTime(tt1.Next, time);
                        if (tt2 != null) 
                            tt1 = tt2;
                        List<Pullenti.Ner.ReferentToken> res0 = new List<Pullenti.Ner.ReferentToken>();
                        res0.Add(new Pullenti.Ner.ReferentToken(time, t00, tt1));
                        return res0;
                    }
                }
                if ((dts.Count == 1 && dts[0].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Month && dts[0].BeginToken.Previous != null) && dts[0].BeginToken.Previous.Morph.Class.IsPreposition) 
                {
                    if (dts[0].Chars.IsLatinLetter && dts[0].Chars.IsAllLower) 
                    {
                    }
                    else 
                    {
                        List<Pullenti.Ner.ReferentToken> res0 = new List<Pullenti.Ner.ReferentToken>();
                        res0.Add(new Pullenti.Ner.ReferentToken(new DateReferent() { Month = dts[0].IntValue }, dts[0].BeginToken, dts[0].EndToken));
                        return res0;
                    }
                }
                return null;
            }
            List<Pullenti.Ner.ReferentToken> res = new List<Pullenti.Ner.ReferentToken>();
            DateReferent drYear = null;
            DateReferent drMon = null;
            DateReferent drDay = null;
            Pullenti.Ner.Token t0 = null;
            Pullenti.Ner.Token t1 = null;
            if (cent != null) 
            {
                DateReferent ce = new DateReferent() { Century = (cent.NewAge < 0 ? -cent.IntValue : cent.IntValue), IsRelative = cent.Relate };
                Pullenti.Ner.ReferentToken rt = new Pullenti.Ner.ReferentToken(ce, cent.BeginToken, (t1 = cent.EndToken));
                res.Add(rt);
            }
            if (tenyears != null) 
            {
                DateReferent ce = new DateReferent() { Decade = tenyears.IntValue, IsRelative = tenyears.Relate };
                if (cent != null) 
                    ce.Higher = res[res.Count - 1].Referent as DateReferent;
                Pullenti.Ner.ReferentToken rt = new Pullenti.Ner.ReferentToken(ce, tenyears.BeginToken, (t1 = tenyears.EndToken));
                res.Add(rt);
            }
            if (year != null && year.Year > 0) 
            {
                drYear = new DateReferent() { Year = (year.NewAge < 0 ? -year.Year : year.Year) };
                if (!yearIsDif) 
                {
                    t1 = year.EndToken;
                    if (t1.Next != null && t1.Next.IsValue("ГОРОД", null)) 
                    {
                        Pullenti.Ner.Token tt2 = t1.Next.Next;
                        if (tt2 == null) 
                            year.EndToken = (t1 = t1.Next);
                        else if ((tt2.WhitespacesBeforeCount < 3) && ((tt2.Morph.Class.IsPreposition || tt2.Chars.IsAllLower))) 
                            year.EndToken = (t1 = t1.Next);
                    }
                }
                res.Add(new Pullenti.Ner.ReferentToken(drYear, (t0 = year.BeginToken), year.EndToken) { Morph = year.Morph });
                if (((dts.Count == 3 && year == dts[2] && mon == null) && day == null && dts[0].Year > 0) && dts[1].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Delim && dts[1].EndToken.IsHiphen) 
                {
                    DateReferent drYear0 = new DateReferent() { Year = (year.NewAge < 0 ? -dts[0].Year : dts[0].Year) };
                    res.Add(new Pullenti.Ner.ReferentToken(drYear0, (t0 = dts[0].BeginToken), dts[0].EndToken));
                }
            }
            if (mon != null) 
            {
                drMon = new DateReferent() { Month = mon.IntValue };
                if (drYear != null) 
                    drMon.Higher = drYear;
                if (t0 == null || (mon.BeginChar < t0.BeginChar)) 
                    t0 = mon.BeginToken;
                if (t1 == null || mon.EndChar > t1.EndChar) 
                    t1 = mon.EndToken;
                if (drYear == null && t1.Next != null && ((t1.Next.IsValue("ПО", null) || t1.Next.IsValue("НА", null)))) 
                {
                    Pullenti.Ner.ReferentToken rt = DateAnalyzer.ProcessReferentStat(t1.Next.Next, null);
                    if (rt != null) 
                    {
                        DateReferent dr0 = rt.Referent as DateReferent;
                        if (dr0 != null && dr0.Year > 0 && dr0.Month > 0) 
                        {
                            drYear = new DateReferent() { Year = dr0.Year };
                            res.Add(new Pullenti.Ner.ReferentToken(drYear, (t0 = t1), t1));
                            drMon.Higher = drYear;
                        }
                    }
                }
                res.Add(new Pullenti.Ner.ReferentToken(drMon, t0, t1) { Morph = mon.Morph });
                if (day != null) 
                {
                    drDay = new DateReferent() { Day = day.IntValue };
                    drDay.Higher = drMon;
                    if (day.BeginChar < t0.BeginChar) 
                        t0 = day.BeginToken;
                    if (day.EndChar > t1.EndChar) 
                        t1 = day.EndToken;
                    Pullenti.Ner.Token tt;
                    for (tt = t0.Previous; tt != null; tt = tt.Previous) 
                    {
                        if (!tt.IsCharOf(",.")) 
                            break;
                    }
                    Pullenti.Ner.Core.TerminToken dow = Pullenti.Ner.Date.Internal.DateItemToken.DaysOfWeek.TryParse(tt, Pullenti.Ner.Core.TerminParseAttr.No);
                    if (dow != null) 
                    {
                        t0 = tt;
                        drDay.DayOfWeek = (int)dow.Termin.Tag;
                    }
                    res.Add(new Pullenti.Ner.ReferentToken(drDay, t0, t1) { Morph = day.Morph });
                    if (dts[0].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Hour) 
                    {
                        DateReferent hou = new DateReferent() { Higher = drDay };
                        hou.Hour = dts[0].IntValue;
                        hou.Minute = 0;
                        if (dts[1].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Minute) 
                        {
                            hou.Minute = dts[1].IntValue;
                            if (dts[2].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Second) 
                                hou.Second = dts[2].IntValue;
                        }
                        res.Add(new Pullenti.Ner.ReferentToken(hou, dts[0].BeginToken, t1));
                        return res;
                    }
                }
            }
            if (point != null && res.Count > 0) 
            {
                DateReferent poi = new DateReferent();
                poi.Pointer = point.Ptr;
                poi.Higher = res[res.Count - 1].Referent as DateReferent;
                res.Add(new Pullenti.Ner.ReferentToken(poi, point.BeginToken, t1));
                return res;
            }
            if (drDay != null && !yearIsDif) 
            {
                Pullenti.Ner.ReferentToken rt = TryAttachTime(t1.Next, true);
                if (rt != null) 
                {
                    (rt.Referent as DateReferent).Higher = drDay;
                    rt.BeginToken = t0;
                    res.Add(rt);
                    if (rt.EndToken.Next != null && rt.EndToken.Next.IsHiphen) 
                    {
                        Pullenti.Ner.ReferentToken rt1 = TryAttachTime(rt.EndToken.Next.Next, true);
                        if (rt1 != null) 
                        {
                            (rt1.Referent as DateReferent).Higher = drDay;
                            res.Add(rt1);
                            DateRangeReferent rr = new DateRangeReferent();
                            rr.DateFrom = rt.Referent as DateReferent;
                            rr.DateTo = rt1.Referent as DateReferent;
                            res.Add(new Pullenti.Ner.ReferentToken(rr, rt.BeginToken, rt1.EndToken));
                        }
                    }
                }
                else 
                    for (int i = 1; i < dts.Count; i++) 
                    {
                        if (t0.BeginChar == dts[i].BeginChar) 
                        {
                            if (i > 2) 
                            {
                                dts.RemoveRange(i, dts.Count - i);
                                rt = TryAttachTimeLi(dts, true);
                                if (rt != null) 
                                {
                                    (rt.Referent as DateReferent).Higher = drDay;
                                    rt.EndToken = t1;
                                    res.Add(rt);
                                }
                                break;
                            }
                        }
                    }
            }
            if (res.Count == 1) 
            {
                DateReferent dt0 = res[0].Referent as DateReferent;
                if (dt0.Month == 0) 
                {
                    Pullenti.Ner.Token tt = res[0].BeginToken.Previous;
                    if (tt != null && tt.IsChar('_') && !tt.IsNewlineAfter) 
                    {
                        for (; tt != null; tt = tt.Previous) 
                        {
                            if (!tt.IsChar('_')) 
                                break;
                            else 
                                res[0].BeginToken = tt;
                        }
                        if (Pullenti.Ner.Core.BracketHelper.CanBeEndOfSequence(tt, true, null, false)) 
                        {
                            for (tt = tt.Previous; tt != null; tt = tt.Previous) 
                            {
                                if (tt.IsNewlineAfter) 
                                    break;
                                else if (tt.IsChar('_')) 
                                {
                                }
                                else 
                                {
                                    if (Pullenti.Ner.Core.BracketHelper.CanBeStartOfSequence(tt, true, false)) 
                                        res[0].BeginToken = tt;
                                    break;
                                }
                            }
                        }
                    }
                    tt = res[0].EndToken.Next;
                    if (tt != null && tt.IsCharOf("(,")) 
                    {
                        Pullenti.Ner.Date.Internal.DateItemToken dit = Pullenti.Ner.Date.Internal.DateItemToken.TryParse(tt.Next, null, false);
                        if (dit != null && dit.Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Month) 
                        {
                            drMon = new DateReferent() { Higher = dt0, Month = dit.IntValue };
                            Pullenti.Ner.ReferentToken prMon = new Pullenti.Ner.ReferentToken(drMon, res[0].BeginToken, dit.EndToken);
                            if (tt.IsChar('(') && prMon.EndToken.Next != null && prMon.EndToken.Next.IsChar(')')) 
                                prMon.EndToken = prMon.EndToken.Next;
                            res.Add(prMon);
                        }
                    }
                }
            }
            if (res.Count > 0 && drDay != null) 
            {
                Pullenti.Ner.ReferentToken la = res[res.Count - 1];
                Pullenti.Ner.Token tt = la.EndToken.Next;
                if (tt != null && tt.IsChar(',')) 
                    tt = tt.Next;
                Pullenti.Ner.Core.TerminToken tok = Pullenti.Ner.Date.Internal.DateItemToken.DaysOfWeek.TryParse(tt, Pullenti.Ner.Core.TerminParseAttr.No);
                if (tok != null) 
                {
                    la.EndToken = tok.EndToken;
                    drDay.DayOfWeek = (int)tok.Termin.Tag;
                }
            }
            return res;
        }
        static DateReferent _findYear_(Pullenti.Ner.Referent r)
        {
            DateReferent dr = r as DateReferent;
            if (dr != null) 
            {
                for (; dr != null; dr = dr.Higher) 
                {
                    if (dr.Higher == null && dr.Year > 0) 
                        return dr;
                }
                return null;
            }
            DateRangeReferent drr = r as DateRangeReferent;
            if (drr != null) 
            {
                if ((((dr = _findYear_(drr.DateFrom)))) != null) 
                    return dr;
                if ((((dr = _findYear_(drr.DateTo)))) != null) 
                    return dr;
            }
            return null;
        }
        static Pullenti.Ner.ReferentToken TryAttachTime(Pullenti.Ner.Token t, bool afterDate)
        {
            if (t == null) 
                return null;
            if (IsUserParamIgnoreTime(t)) 
                return null;
            if (t.IsValue("ГОРОД", null) && t.Next != null) 
                t = t.Next;
            while (t != null && ((t.Morph.Class.IsPreposition || t.Morph.Class.IsAdverb || t.IsComma))) 
            {
                if (t.Morph.Language.IsRu) 
                {
                    if (!t.IsValue("ПО", null) && !t.IsValue("НА", null)) 
                        t = t.Next;
                    else 
                        break;
                }
                else 
                    t = t.Next;
            }
            if (t == null) 
                return null;
            List<Pullenti.Ner.Date.Internal.DateItemToken> dts = Pullenti.Ner.Date.Internal.DateItemToken.TryParseList(t, 10);
            if (dts == null || dts.Count == 0) 
                return null;
            if (dts[0].IsNewlineBefore && dts[0].Typ != Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Hour && !IsUserParamDate(dts[0])) 
                return null;
            return TryAttachTimeLi(dts, afterDate);
        }
        static Pullenti.Ner.Token _corrTime(Pullenti.Ner.Token t0, DateReferent time)
        {
            Pullenti.Ner.Token t1 = null;
            for (Pullenti.Ner.Token t = t0; t != null; t = t.Next) 
            {
                if (!(t is Pullenti.Ner.TextToken)) 
                    break;
                string term = (t as Pullenti.Ner.TextToken).Term;
                if (term == "МСК") 
                {
                    t1 = t;
                    continue;
                }
                Pullenti.Ner.Core.NounPhraseToken npt = Pullenti.Ner.Core.NounPhraseHelper.TryParse(t, Pullenti.Ner.Core.NounPhraseParseAttr.ParsePreposition, 0, null);
                if (npt != null && npt.EndToken.IsValue("ВРЕМЯ", null)) 
                {
                    t = (t1 = npt.EndToken);
                    continue;
                }
                if ((t.IsCharOf("(") && t.Next != null && t.Next.IsValue("МСК", null)) && t.Next.Next != null && t.Next.Next.IsChar(')')) 
                {
                    t1 = (t = t.Next.Next);
                    continue;
                }
                if ((term == "PM" || term == "РМ" || t.IsValue("ВЕЧЕР", "ВЕЧІР")) || t.IsValue("ДЕНЬ", null)) 
                {
                    if (time.Hour < 12) 
                        time.Hour = time.Hour + 12;
                    t1 = t;
                    continue;
                }
                if ((term == "AM" || term == "АМ" || term == "Ч") || t.IsValue("ЧАС", null)) 
                {
                    t1 = t;
                    continue;
                }
                if (t.IsChar('+')) 
                {
                    List<Pullenti.Ner.Date.Internal.DateItemToken> ddd = Pullenti.Ner.Date.Internal.DateItemToken.TryParseList(t.Next, 20);
                    if ((ddd != null && ddd.Count == 3 && ddd[0].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Number) && ddd[1].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Delim && ddd[2].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Number) 
                    {
                        t1 = ddd[2].EndToken;
                        continue;
                    }
                }
                if (t.IsCharOf(",.")) 
                    continue;
                break;
            }
            return t1;
        }
        static Pullenti.Ner.ReferentToken TryAttachTimeLi(List<Pullenti.Ner.Date.Internal.DateItemToken> dts, bool afterDate)
        {
            if (dts == null || (dts.Count < 1)) 
                return null;
            Pullenti.Ner.Token t0 = dts[0].BeginToken;
            Pullenti.Ner.Token t1 = null;
            DateReferent time = null;
            if (dts.Count == 1) 
            {
                if (dts[0].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Hour && afterDate) 
                {
                    time = new DateReferent() { Hour = dts[0].IntValue, Minute = 0 };
                    t1 = dts[0].EndToken;
                }
                else 
                    return null;
            }
            else if (dts[0].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Hour && dts[1].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Minute) 
            {
                time = new DateReferent() { Hour = dts[0].IntValue, Minute = dts[1].IntValue };
                t1 = dts[1].EndToken;
                if (dts.Count > 2 && dts[2].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Second) 
                {
                    t1 = dts[2].EndToken;
                    time.Second = dts[2].IntValue;
                }
            }
            else if ((((dts.Count > 2 && dts[0].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Number && dts[1].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Delim) && ((dts[1].StringValue == ":" || dts[1].StringValue == "." || dts[1].StringValue == "-")) && dts[2].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Number) && (dts[0].IntValue < 24) && (dts[2].IntValue < 60)) && dts[2].LengthChar == 2 && afterDate) 
            {
                if (dts.Count >= 5) 
                {
                    List<Pullenti.Ner.ReferentToken> dat = TryAttach(dts, false);
                    if (dat != null && dat.Count > 0) 
                        return null;
                }
                time = new DateReferent() { Hour = dts[0].IntValue, Minute = dts[2].IntValue };
                t1 = dts[2].EndToken;
                if ((dts.Count > 4 && dts[3].StringValue == dts[1].StringValue && dts[4].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Number) && (dts[4].IntValue < 60)) 
                {
                    time.Second = dts[4].IntValue;
                    t1 = dts[4].EndToken;
                }
            }
            if (time == null) 
                return null;
            Pullenti.Ner.Token tt = _corrTime(t1.Next, time);
            if (tt != null) 
                t1 = tt;
            int cou = 0;
            for (tt = t0.Previous; tt != null && (cou < 1000); tt = tt.Previous,cou++) 
            {
                if (tt.GetReferent() is DateReferent) 
                {
                    DateReferent dr = tt.GetReferent() as DateReferent;
                    if (dr.FindSlot(DateReferent.ATTR_DAY, null, true) == null && dr.Higher != null) 
                        dr = dr.Higher;
                    if (dr.FindSlot(DateReferent.ATTR_DAY, null, true) != null) 
                    {
                        time.Higher = dr;
                        break;
                    }
                }
            }
            if (t1.Next != null) 
            {
                if (t1.Next.IsValue("ЧАС", null)) 
                    t1 = t1.Next;
            }
            return new Pullenti.Ner.ReferentToken(time, t0, t1);
        }
        static bool ApplyRuleFormal(List<Pullenti.Ner.Date.Internal.DateItemToken> its, bool high, out Pullenti.Ner.Date.Internal.DateItemToken year, out Pullenti.Ner.Date.Internal.DateItemToken mon, out Pullenti.Ner.Date.Internal.DateItemToken day)
        {
            year = null;
            mon = null;
            day = null;
            if (!high && its.Count == 5) 
            {
                if (its[0].BeginToken.Previous != null && its[0].BeginToken.Previous.IsValue("ОТ", null)) 
                    high = true;
            }
            int i;
            int j;
            for (i = 0; i < (its.Count - 4); i++) 
            {
                if ((its[i].BeginToken.Previous != null && its[i].BeginToken.Previous.IsChar(')') && (its[i].WhitespacesBeforeCount < 2)) && i > 0) 
                    return false;
                if (!its[i].CanBeDay && !its[i].CanBeYear && !its[i].CanByMonth) 
                    continue;
                if (!its[i].IsWhitespaceBefore) 
                {
                    if (its[i].BeginToken.Previous != null && ((its[i].BeginToken.Previous.IsCharOf("(;,") || its[i].BeginToken.Previous.Morph.Class.IsPreposition || its[i].BeginToken.Previous.IsTableControlChar))) 
                    {
                    }
                    else if (i > 0) 
                        continue;
                }
                for (j = i; j < (i + 4); j++) 
                {
                    if (its[j].IsWhitespaceAfter) 
                    {
                        if (high && !its[j].IsNewlineAfter) 
                            continue;
                        if (i == 0 && its.Count == 5 && ((j == 1 || j == 3))) 
                        {
                            if (its[j].WhitespacesAfterCount < 2) 
                                continue;
                        }
                        break;
                    }
                }
                if (j < (i + 4)) 
                    continue;
                if (its[i + 1].Typ != Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Delim || its[i + 3].Typ != Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Delim || its[i + 1].StringValue != its[i + 3].StringValue) 
                    continue;
                j = i + 5;
                if ((j < its.Count) && !its[j].IsWhitespaceBefore) 
                {
                    if (its[j].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Delim && its[j].IsWhitespaceAfter) 
                    {
                    }
                    else 
                        continue;
                }
                mon = (its[i + 2].CanByMonth ? its[i + 2] : null);
                if (!its[i].CanBeDay) 
                {
                    if (!its[i].CanBeYear) 
                        continue;
                    year = its[i];
                    if (mon != null && its[i + 4].CanBeDay) 
                        day = its[i + 4];
                    else if (its[i + 2].CanBeDay && its[i + 4].CanByMonth) 
                    {
                        day = its[i + 2];
                        mon = its[i + 4];
                    }
                    else 
                        continue;
                }
                else if (!its[i].CanBeYear) 
                {
                    if (!its[i + 4].CanBeYear) 
                    {
                        if (!high) 
                            continue;
                    }
                    year = its[i + 4];
                    if (mon != null && its[i].CanBeDay) 
                        day = its[i];
                    else if (its[i].CanByMonth && its[i + 2].CanBeDay) 
                    {
                        mon = its[i];
                        day = its[i + 2];
                    }
                    else 
                        continue;
                }
                else 
                    continue;
                if ((mon.IntValue < 10) && !mon.IsZeroHeaded) 
                {
                    if (year.IntValue < 1980) 
                        continue;
                }
                char delim = its[i + 1].StringValue[0];
                if ((delim != '/' && delim != '\\' && delim != '.') && delim != '-') 
                    continue;
                if (delim == '.' || delim == '-') 
                {
                    if (year == its[i] && (year.IntValue < 1900)) 
                        continue;
                }
                if ((i + 5) < its.Count) 
                    its.RemoveRange(i + 5, its.Count - i - 5);
                if (i > 0) 
                    its.RemoveRange(0, i);
                return true;
            }
            if (its.Count >= 5 && its[0].IsWhitespaceBefore && its[4].IsWhitespaceAfter) 
            {
                if (its[1].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Delim && its[2].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Delim) 
                {
                    if (its[0].LengthChar == 2 && its[2].LengthChar == 2 && ((its[4].LengthChar == 2 || its[4].LengthChar == 4))) 
                    {
                        if (its[0].CanBeDay && its[2].CanByMonth && its[4].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Number) 
                        {
                            if ((!its[0].IsWhitespaceAfter && !its[1].IsWhitespaceAfter && !its[2].IsWhitespaceAfter) && !its[3].IsWhitespaceAfter) 
                            {
                                int iyear = 0;
                                int y = its[4].IntValue;
                                if (y > 80 && (y < 100)) 
                                    iyear = 1900 + y;
                                else if (y <= (DateTime.Today.Year - 2000)) 
                                    iyear = y + 2000;
                                else 
                                    return false;
                                its[4].Year = iyear;
                                year = its[4];
                                mon = its[2];
                                day = its[0];
                                return true;
                            }
                        }
                    }
                }
            }
            if (high && its[0].CanBeYear && its.Count == 1) 
            {
                year = its[0];
                if (its[0].NewStyle != null && its[0].NewStyle.Count > 0) 
                    year = new Pullenti.Ner.Date.Internal.DateItemToken(its[0].BeginToken, its[0].BeginToken) { IntValue = its[0].IntValue };
                return true;
            }
            if (its[0].BeginToken.Previous != null && its[0].BeginToken.Previous.IsValue("ОТ", null) && its.Count == 4) 
            {
                if (its[0].CanBeDay && its[3].CanBeYear) 
                {
                    if (its[1].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Delim && its[2].CanByMonth) 
                    {
                        year = its[3];
                        mon = its[2];
                        day = its[0];
                        return true;
                    }
                    if (its[2].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Delim && its[1].CanByMonth) 
                    {
                        year = its[3];
                        mon = its[1];
                        day = its[0];
                        return true;
                    }
                }
            }
            if ((its.Count == 3 && its[0].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Number && its[0].CanBeDay) && its[1].CanByMonth) 
            {
                if (its[2].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Year || ((its[2].CanBeYear && its[0].BeginToken.Previous != null && its[0].BeginToken.Previous.IsValue("ОТ", null)))) 
                {
                    if (((Pullenti.Ner.Core.BracketHelper.IsBracket(its[0].BeginToken, false) && Pullenti.Ner.Core.BracketHelper.IsBracket(its[0].EndToken, false))) || ((its[0].BeginToken.Previous != null && its[0].BeginToken.Previous.GetMorphClassInDictionary().IsPreposition))) 
                    {
                        year = its[2];
                        mon = its[1];
                        day = its[0];
                        return true;
                    }
                }
            }
            if (high) 
            {
                if (((its.Count >= 5 && its[0].CanBeDay && its[1].StringValue == ".") && its[2].CanByMonth && its[3].StringValue == ".") && its[4].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Number) 
                {
                    if (its.Count == 5 || its[4].EndToken.Next.IsHiphen) 
                    {
                        year = its[4];
                        mon = its[2];
                        day = its[0];
                        return true;
                    }
                }
            }
            return false;
        }
        static bool ApplyRuleWithMonth(List<Pullenti.Ner.Date.Internal.DateItemToken> its, bool high, out Pullenti.Ner.Date.Internal.DateItemToken year, out Pullenti.Ner.Date.Internal.DateItemToken mon, out Pullenti.Ner.Date.Internal.DateItemToken day, out bool yearIsDiff)
        {
            year = null;
            mon = null;
            day = null;
            yearIsDiff = false;
            int i;
            if (its.Count == 2) 
            {
                if (its[0].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Month && its[0].LTyp == Pullenti.Ner.Date.Internal.DateItemToken.FirstLastTyp.No && its[1].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Year) 
                {
                    year = its[1];
                    mon = its[0];
                    return true;
                }
                if (its[0].CanBeDay && its[1].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Month) 
                {
                    mon = its[1];
                    day = its[0];
                    return true;
                }
            }
            for (i = 0; i < its.Count; i++) 
            {
                if (its[i].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Month && its[i].LTyp == Pullenti.Ner.Date.Internal.DateItemToken.FirstLastTyp.No) 
                    break;
            }
            if (i >= its.Count) 
                return false;
            Pullenti.Morph.MorphLang lang = its[i].Lang;
            year = null;
            day = null;
            mon = its[i];
            int i0 = i;
            int i1 = i;
            int yearVal = 0;
            if ((lang.IsRu || lang.IsIt || lang.IsBy) || lang.IsUa) 
            {
                if (((i + 1) < its.Count) && its[i + 1].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Year) 
                {
                    year = its[i + 1];
                    i1 = i + 1;
                    if (i > 0 && its[i - 1].CanBeDay) 
                    {
                        day = its[i - 1];
                        i0 = i - 1;
                    }
                }
                else if (i > 0 && its[i - 1].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Year) 
                {
                    year = its[i - 1];
                    i0 = i - 1;
                    if (((i + 1) < its.Count) && its[i + 1].CanBeDay) 
                    {
                        day = its[i + 1];
                        i1 = i + 1;
                    }
                }
                else if (((i + 1) < its.Count) && its[i + 1].CanBeYear) 
                {
                    if (its[i + 1].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Number) 
                    {
                        Pullenti.Ner.Token t00 = its[0].BeginToken;
                        if (t00.Previous != null && t00.Previous.IsCharOf(".,")) 
                            t00 = t00.Previous.Previous;
                        if (t00 != null && (t00.WhitespacesAfterCount < 3)) 
                        {
                            if (((t00.IsValue("УЛИЦА", null) || t00.IsValue("УЛ", null) || t00.IsValue("ПРОСПЕКТ", null)) || t00.IsValue("ПРОСП", null) || t00.IsValue("ПР", null)) || t00.IsValue("ПЕРЕУЛОК", null) || t00.IsValue("ПЕР", null)) 
                                return false;
                        }
                    }
                    year = its[i + 1];
                    i1 = i + 1;
                    if (i > 0 && its[i - 1].CanBeDay) 
                    {
                        day = its[i - 1];
                        i0 = i - 1;
                    }
                }
                else if ((i == 0 && its[0].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Month && its.Count == 3) && its[i + 1].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Delim && its[i + 2].CanBeYear) 
                {
                    year = its[i + 2];
                    i1 = i + 2;
                }
                else if (i > 1 && its[i - 2].CanBeYear && its[i - 1].CanBeDay) 
                {
                    year = its[i - 2];
                    day = its[i - 1];
                    i0 = i - 2;
                }
                else if (i > 0 && its[i - 1].CanBeYear) 
                {
                    year = its[i - 1];
                    i0 = i - 1;
                    if (((i + 1) < its.Count) && its[i + 1].CanBeDay) 
                    {
                        day = its[i + 1];
                        i1 = i + 1;
                    }
                }
                if (year == null && i == 1 && its[i - 1].CanBeDay) 
                {
                    for (int j = i + 1; j < its.Count; j++) 
                    {
                        if (its[j].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Delim) 
                            continue;
                        if (its[j].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Year) 
                        {
                            year = its[j];
                            day = its[i - 1];
                            i0 = i - 1;
                            i1 = i;
                            yearIsDiff = true;
                            break;
                        }
                        if (!its[j].CanBeDay) 
                            break;
                        if ((++j) >= its.Count) 
                            break;
                        if (its[j].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Month) 
                            continue;
                        if (its[j].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Delim && ((j + 1) < its.Count) && its[j + 1].CanBeDay) 
                            continue;
                        break;
                    }
                }
            }
            else if (lang.IsEn) 
            {
                if (i == 1 && its[0].CanBeDay) 
                {
                    i1 = 2;
                    day = its[0];
                    i0 = 0;
                    if ((i1 < its.Count) && its[i1].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Delim) 
                        i1++;
                    if ((i1 < its.Count) && its[i1].CanBeYear) 
                        year = its[i1];
                    if (year == null) 
                    {
                        i1 = 1;
                        yearVal = FindYear(its[0].BeginToken);
                    }
                }
                else if (i == 0) 
                {
                    if (its.Count > 1 && its[1].CanBeYear && !its[1].CanBeDay) 
                    {
                        i1 = 2;
                        year = its[1];
                    }
                    else if (its.Count > 1 && its[1].CanBeDay) 
                    {
                        day = its[1];
                        i1 = 2;
                        if ((i1 < its.Count) && its[i1].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Delim) 
                            i1++;
                        if ((i1 < its.Count) && its[i1].CanBeYear) 
                            year = its[i1];
                        if (year == null) 
                        {
                            i1 = 1;
                            yearVal = FindYear(its[0].BeginToken);
                        }
                    }
                }
            }
            if (year == null && yearVal == 0 && its.Count == 3) 
            {
                if (its[0].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Year && its[1].CanBeDay && its[2].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Month) 
                {
                    i1 = 2;
                    year = its[0];
                    day = its[1];
                }
            }
            if (year != null || yearVal > 0) 
                return true;
            if (day != null && its.Count == 2) 
                return true;
            return false;
        }
        static int FindYear(Pullenti.Ner.Token t)
        {
            int year = 0;
            int prevdist = 0;
            for (Pullenti.Ner.Token tt = t; tt != null; tt = tt.Previous) 
            {
                if (tt.IsNewlineBefore) 
                    prevdist += 10;
                prevdist++;
                if (tt is Pullenti.Ner.ReferentToken) 
                {
                    if ((tt as Pullenti.Ner.ReferentToken).Referent is DateReferent) 
                    {
                        year = ((tt as Pullenti.Ner.ReferentToken).Referent as DateReferent).Year;
                        break;
                    }
                }
            }
            int dist = 0;
            for (Pullenti.Ner.Token tt = t; tt != null; tt = tt.Next) 
            {
                if (tt.IsNewlineAfter) 
                    dist += 10;
                dist++;
                if (tt is Pullenti.Ner.ReferentToken) 
                {
                    if ((tt as Pullenti.Ner.ReferentToken).Referent is DateReferent) 
                    {
                        if (year > 0 && (prevdist < dist)) 
                            return year;
                        else 
                            return ((tt as Pullenti.Ner.ReferentToken).Referent as DateReferent).Year;
                    }
                }
            }
            return year;
        }
        static bool ApplyRuleYearOnly(List<Pullenti.Ner.Date.Internal.DateItemToken> its, out Pullenti.Ner.Date.Internal.DateItemToken year, out Pullenti.Ner.Date.Internal.DateItemToken mon, out Pullenti.Ner.Date.Internal.DateItemToken day)
        {
            year = null;
            mon = null;
            day = null;
            int i;
            bool doubt = false;
            for (i = 0; i < its.Count; i++) 
            {
                if (its[i].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Year) 
                    break;
                else if (its[i].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Number) 
                    doubt = true;
                else if (its[i].Typ != Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Delim) 
                    return false;
            }
            if (i >= its.Count) 
            {
                if (((its.Count == 1 && its[0].CanBeYear && its[0].IntValue > 1900) && its[0].CanBeYear && (its[0].IntValue < 2100)) && its[0].BeginToken.Previous != null) 
                {
                    if (((its[0].BeginToken.Previous.IsValue("В", null) || its[0].BeginToken.Previous.IsValue("У", null) || its[0].BeginToken.Previous.IsValue("З", null)) || its[0].BeginToken.Previous.IsValue("IN", null) || its[0].BeginToken.Previous.IsValue("SINCE", null))) 
                    {
                        if (its[0].LengthChar == 4 || its[0].BeginToken.Morph.Class.IsAdjective) 
                        {
                            year = its[0];
                            return true;
                        }
                    }
                }
                return false;
            }
            if ((i + 1) == its.Count) 
            {
                if (its[i].IntValue > 1900 || its[i].NewAge != 0) 
                {
                    year = its[i];
                    return true;
                }
                if (doubt) 
                    return false;
                if (its[i].IntValue > 10 && (its[i].IntValue < 100)) 
                {
                    if (its[i].BeginToken.Previous != null) 
                    {
                        if (its[i].BeginToken.Previous.IsValue("В", null) || its[i].BeginToken.Previous.IsValue("IN", null) || its[i].BeginToken.Previous.IsValue("У", null)) 
                        {
                            year = its[i];
                            return true;
                        }
                    }
                    if (its[i].BeginToken.IsValue("В", null) || its[i].BeginToken.IsValue("У", null) || its[i].BeginToken.IsValue("IN", null)) 
                    {
                        year = its[i];
                        return true;
                    }
                }
                if (its[i].IntValue >= 100) 
                {
                    year = its[i];
                    return true;
                }
                return false;
            }
            if (its.Count == 1 && its[0].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Year && its[0].Year <= 0) 
            {
                year = its[0];
                return true;
            }
            if (((its.Count > 2 && its[0].CanBeYear && its[1].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Delim) && its[1].BeginToken.IsHiphen && its[2].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Year) && (its[0].Year0 < its[2].Year0)) 
            {
                year = its[0];
                return true;
            }
            if (its[0].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Year) 
            {
                if ((its[0].BeginToken.Previous != null && its[0].BeginToken.Previous.IsHiphen && (its[0].BeginToken.Previous.Previous is Pullenti.Ner.ReferentToken)) && (its[0].BeginToken.Previous.Previous.GetReferent() is DateReferent)) 
                {
                    year = its[0];
                    return true;
                }
            }
            return false;
        }
        static DateRangeReferent ApplyDateRange(Pullenti.Ner.Core.AnalyzerData ad, List<Pullenti.Ner.Date.Internal.DateItemToken> its, out Pullenti.Morph.MorphLang lang)
        {
            lang = new Pullenti.Morph.MorphLang();
            if (its == null || (its.Count < 3)) 
                return null;
            if ((its[0].CanBeYear && its[1].StringValue == "-" && its[2].Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Year) && (its[0].Year < its[2].Year)) 
            {
                DateRangeReferent res = new DateRangeReferent();
                res.DateFrom = ad.RegisterReferent(new DateReferent() { Year = its[0].Year }) as DateReferent;
                Pullenti.Ner.ReferentToken rt1 = new Pullenti.Ner.ReferentToken(res.DateFrom, its[0].BeginToken, its[0].EndToken);
                res.DateTo = ad.RegisterReferent(new DateReferent() { Year = its[2].Year }) as DateReferent;
                Pullenti.Ner.ReferentToken rt2 = new Pullenti.Ner.ReferentToken(res.DateTo, its[2].BeginToken, its[2].EndToken);
                lang = its[2].Lang;
                return res;
            }
            return null;
        }
        static void ApplyDateRange0(Pullenti.Ner.Core.AnalysisKit kit, Pullenti.Ner.Core.AnalyzerData ad)
        {
            for (Pullenti.Ner.Token t = kit.FirstToken; t != null; t = t.Next) 
            {
                if (t.IsIgnored) 
                    continue;
                if (!(t is Pullenti.Ner.TextToken)) 
                    continue;
                int yearVal1 = 0;
                int yearVal2 = 0;
                DateReferent date1 = null;
                DateReferent date2 = null;
                Pullenti.Morph.MorphLang lang = new Pullenti.Morph.MorphLang();
                Pullenti.Ner.Token t0 = t.Next;
                string str = (t as Pullenti.Ner.TextToken).Term;
                if (str == "ON" && (t0 is Pullenti.Ner.TextToken)) 
                {
                    Pullenti.Ner.Core.TerminToken tok = Pullenti.Ner.Date.Internal.DateItemToken.DaysOfWeek.TryParse(t0, Pullenti.Ner.Core.TerminParseAttr.No);
                    if (tok != null) 
                    {
                        DateReferent dow = new DateReferent() { DayOfWeek = (int)tok.Termin.Tag };
                        Pullenti.Ner.ReferentToken rtd = new Pullenti.Ner.ReferentToken(ad.RegisterReferent(dow), t, tok.EndToken);
                        kit.EmbedToken(rtd);
                        t = rtd;
                        continue;
                    }
                }
                bool isBetwen = false;
                if (str == "С" || str == "C") 
                    lang = Pullenti.Morph.MorphLang.RU;
                else if (str == "МЕЖДУ") 
                {
                    lang = Pullenti.Morph.MorphLang.RU;
                    isBetwen = true;
                }
                else if (str == "З") 
                    lang = Pullenti.Morph.MorphLang.UA;
                else if (str == "ПОМІЖ" || str == "МІЖ") 
                {
                    lang = Pullenti.Morph.MorphLang.UA;
                    isBetwen = true;
                }
                else if (str == "BETWEEN") 
                {
                    lang = Pullenti.Morph.MorphLang.EN;
                    isBetwen = true;
                }
                else if (str == "IN") 
                {
                    lang = Pullenti.Morph.MorphLang.EN;
                    if ((t0 != null && t0.IsValue("THE", null) && t0.Next != null) && t0.Next.IsValue("PERIOD", null)) 
                        t0 = t0.Next.Next;
                }
                else if (str == "ПО" || str == "ДО" || str == "BEFORE") 
                {
                    if ((t.Next is Pullenti.Ner.ReferentToken) && (t.Next.GetReferent() is DateReferent)) 
                    {
                        DateRangeReferent dr = new DateRangeReferent() { DateTo = t.Next.GetReferent() as DateReferent };
                        Pullenti.Ner.ReferentToken rt0 = new Pullenti.Ner.ReferentToken(ad.RegisterReferent(dr), t, t.Next);
                        if (rt0.EndToken.Next != null && rt0.EndToken.Next.IsValue("ВКЛЮЧИТЕЛЬНО", null)) 
                            rt0.EndToken = rt0.EndToken.Next;
                        else 
                            dr.AddSlot("@EXCLUDE", "true", true, 0);
                        kit.EmbedToken(rt0);
                        t = rt0;
                        continue;
                    }
                }
                else if (((str == "ПОСЛЕ" || str == "AFTER")) && (t.Next is Pullenti.Ner.ReferentToken) && (t.Next.GetReferent() is DateReferent)) 
                {
                    DateRangeReferent dr = new DateRangeReferent() { DateFrom = t.Next.GetReferent() as DateReferent };
                    Pullenti.Ner.ReferentToken rt0 = new Pullenti.Ner.ReferentToken(ad.RegisterReferent(dr), t, t.Next);
                    if (rt0.EndToken.Next != null && rt0.EndToken.Next.IsValue("ВКЛЮЧИТЕЛЬНО", null)) 
                        rt0.EndToken = rt0.EndToken.Next;
                    else 
                        dr.AddSlot("@EXCLUDE", "true", true, 0);
                    kit.EmbedToken(rt0);
                    t = rt0;
                    continue;
                }
                else 
                    continue;
                if (t0 == null) 
                    continue;
                if (t0 is Pullenti.Ner.ReferentToken) 
                    date1 = (t0 as Pullenti.Ner.ReferentToken).Referent as DateReferent;
                if (date1 == null) 
                {
                    if ((t0 is Pullenti.Ner.NumberToken) && (t0 as Pullenti.Ner.NumberToken).IntValue != null) 
                    {
                        int v = (t0 as Pullenti.Ner.NumberToken).IntValue.Value;
                        if ((v < 1000) || v >= 2100) 
                            continue;
                        yearVal1 = v;
                    }
                    else 
                        continue;
                }
                else 
                    yearVal1 = date1.Year;
                Pullenti.Ner.Token t1 = t0.Next;
                if (t1 == null) 
                    continue;
                if (t1.IsValue("ПО", "ДО") || t1.IsValue("ДО", null)) 
                    lang = t1.Morph.Language;
                else if (t1.IsValue("AND", null)) 
                    lang = Pullenti.Morph.MorphLang.EN;
                else if (t1.IsHiphen && lang.Equals(Pullenti.Morph.MorphLang.EN)) 
                {
                }
                else if (lang.IsUa && t1.IsValue("І", null)) 
                {
                }
                else if (t1.IsAnd && isBetwen) 
                {
                }
                else 
                    continue;
                t1 = t1.Next;
                if (t1 == null) 
                    continue;
                if (t1 is Pullenti.Ner.ReferentToken) 
                    date2 = (t1 as Pullenti.Ner.ReferentToken).Referent as DateReferent;
                if (date2 == null) 
                {
                    if ((t1 is Pullenti.Ner.NumberToken) && (t1 as Pullenti.Ner.NumberToken).IntValue != null) 
                    {
                        Pullenti.Ner.Core.NumberExToken nt1 = Pullenti.Ner.Core.NumberHelper.TryParseNumberWithPostfix(t1);
                        if (nt1 != null) 
                            continue;
                        int v = (t1 as Pullenti.Ner.NumberToken).IntValue.Value;
                        if (v > 0 && (v < yearVal1)) 
                        {
                            int yy = yearVal1 % 100;
                            if (yy < v) 
                                v += (((yearVal1 / 100)) * 100);
                        }
                        if ((v < 1000) || v >= 2100) 
                            continue;
                        yearVal2 = v;
                    }
                    else 
                        continue;
                }
                else 
                    yearVal2 = date2.Year;
                if (yearVal1 > yearVal2 && yearVal2 > 0) 
                    continue;
                if (yearVal1 == yearVal2) 
                {
                    if (date1 == null || date2 == null) 
                        continue;
                    if (DateReferent.Compare(date1, date2) >= 0) 
                        continue;
                }
                if (date1 == null) 
                {
                    date1 = ad.RegisterReferent(new DateReferent() { Year = yearVal1 }) as DateReferent;
                    Pullenti.Ner.ReferentToken rt0 = new Pullenti.Ner.ReferentToken(date1, t0, t0);
                    kit.EmbedToken(rt0);
                    if (t0 == t) 
                        t = rt0;
                }
                if (date2 == null) 
                {
                    date2 = ad.RegisterReferent(new DateReferent() { Year = yearVal2 }) as DateReferent;
                    Pullenti.Ner.ReferentToken rt1 = new Pullenti.Ner.ReferentToken(date2, t1, t1);
                    kit.EmbedToken(rt1);
                    t1 = rt1;
                }
                Pullenti.Ner.ReferentToken rt = new Pullenti.Ner.ReferentToken(ad.RegisterReferent(new DateRangeReferent() { DateFrom = date1, DateTo = date2 }), t, t1);
                if (t.Previous != null) 
                {
                    if (t.Previous.IsValue("ПРОМЕЖУТОК", "ПРОМІЖОК") || t.Previous.IsValue("ДИАПАЗОН", "ДІАПАЗОН") || t.Previous.IsValue("ПЕРИОД", "ПЕРІОД")) 
                        rt.BeginToken = t.Previous;
                }
                kit.EmbedToken(rt);
                t = rt;
            }
        }
        public static bool IsUserParamDate(Pullenti.Ner.Token t)
        {
            if (t == null) 
                return false;
            if (t.Kit.Sofa.UserParams != null) 
            {
                if (t.Kit.Sofa.UserParams.Contains("DATE")) 
                    return true;
            }
            return false;
        }
        public static bool IsUserParamIgnoreTime(Pullenti.Ner.Token t)
        {
            if (t == null) 
                return false;
            if (t.Kit.Sofa.UserParams != null) 
            {
                if (t.Kit.Sofa.UserParams.Contains("IGNORETIME")) 
                    return true;
            }
            return false;
        }
        static object m_Lock = new object();
        static bool m_Inited;
        public static void Initialize()
        {
            lock (m_Lock) 
            {
                if (m_Inited) 
                    return;
                m_Inited = true;
                Pullenti.Ner.Measure.MeasureAnalyzer.Initialize();
                Pullenti.Ner.Date.Internal.MetaDate.Initialize();
                Pullenti.Ner.Date.Internal.MetaDateRange.Initialize();
                try 
                {
                    Pullenti.Ner.Core.Termin.AssignAllTextsAsNormal = true;
                    Pullenti.Ner.Date.Internal.DateItemToken.Initialize();
                    Pullenti.Ner.Core.Termin.AssignAllTextsAsNormal = false;
                }
                catch(Exception ex) 
                {
                    throw new Exception(ex.Message, ex);
                }
                Pullenti.Ner.ProcessorService.RegisterAnalyzer(new DateAnalyzer());
            }
            Pullenti.Ner.Measure.MeasureAnalyzer.Initialize();
        }
    }
}