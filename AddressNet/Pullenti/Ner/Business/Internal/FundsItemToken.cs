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

namespace Pullenti.Ner.Business.Internal
{
    class FundsItemToken : Pullenti.Ner.MetaToken
    {
        public FundsItemToken(Pullenti.Ner.Token b, Pullenti.Ner.Token e) : base(b, e, null)
        {
        }
        public FundsItemTyp Typ = FundsItemTyp.Undefined;
        public Pullenti.Ner.Business.FundsKind Kind = Pullenti.Ner.Business.FundsKind.Undefined;
        public Pullenti.Ner.Referent Ref = null;
        public Pullenti.Ner.NumberToken NumVal;
        public string StringVal;
        public override string ToString()
        {
            StringBuilder res = new StringBuilder();
            res.Append(Typ);
            if (Kind != Pullenti.Ner.Business.FundsKind.Undefined) 
                res.AppendFormat(" K={0}", Kind);
            if (NumVal != null) 
                res.AppendFormat(" N={0}", NumVal.Value);
            if (Ref != null) 
                res.AppendFormat(" R={0}", Ref.ToString());
            if (StringVal != null) 
                res.AppendFormat(" S={0}", StringVal);
            return res.ToString();
        }
        static List<string> m_ActTypes = new List<string>(new string[] {"ОБЫКНОВЕННЫЙ", "ПРИВИЛЕГИРОВАННЫЙ", "ГОЛОСУЮЩИЙ", "ЗВИЧАЙНИЙ", "ПРИВІЛЕЙОВАНОГО", "ГОЛОСУЄ"});
        public static FundsItemToken TryParse(Pullenti.Ner.Token t, FundsItemToken prev = null)
        {
            if (t == null) 
                return null;
            FundsItemTyp typ0 = FundsItemTyp.Undefined;
            for (Pullenti.Ner.Token tt = t; tt != null; tt = tt.Next) 
            {
                if (tt.Morph.Class.IsPreposition || tt.Morph.Class.IsAdverb) 
                    continue;
                if ((tt.IsValue("СУММА", null) || tt.IsValue("ОКОЛО", null) || tt.IsValue("БОЛЕЕ", null)) || tt.IsValue("МЕНЕЕ", null) || tt.IsValue("СВЫШЕ", null)) 
                    continue;
                if ((tt.IsValue("НОМИНАЛ", null) || tt.IsValue("ЦЕНА", null) || tt.IsValue("СТОИМОСТЬ", null)) || tt.IsValue("СТОИТЬ", null)) 
                {
                    typ0 = FundsItemTyp.Price;
                    continue;
                }
                if (tt.IsValue("НОМИНАЛЬНАЯ", null) || tt.IsValue("ОБЩАЯ", null)) 
                    continue;
                if (tt.IsValue("СОСТАВЛЯТЬ", null)) 
                    continue;
                Pullenti.Ner.Referent re = tt.GetReferent();
                if (re is Pullenti.Ner.Org.OrganizationReferent) 
                    return new FundsItemToken(t, tt) { Typ = FundsItemTyp.Org, Ref = re };
                if (re is Pullenti.Ner.Money.MoneyReferent) 
                {
                    if (typ0 == FundsItemTyp.Undefined) 
                        typ0 = FundsItemTyp.Sum;
                    if ((tt.Next != null && tt.Next.IsValue("ЗА", null) && tt.Next.Next != null) && ((tt.Next.Next.IsValue("АКЦИЯ", null) || tt.Next.Next.IsValue("АКЦІЯ", null)))) 
                        typ0 = FundsItemTyp.Price;
                    FundsItemToken res = new FundsItemToken(t, tt) { Typ = typ0, Ref = re };
                    return res;
                }
                if (re != null) 
                    break;
                Pullenti.Ner.Core.NounPhraseToken npt = Pullenti.Ner.Core.NounPhraseHelper.TryParse(tt, Pullenti.Ner.Core.NounPhraseParseAttr.No, 0, null);
                if (npt != null && npt.Noun.IsValue("ПАКЕТ", null)) 
                    npt = Pullenti.Ner.Core.NounPhraseHelper.TryParse(npt.EndToken.Next, Pullenti.Ner.Core.NounPhraseParseAttr.No, 0, null);
                if (npt != null) 
                {
                    FundsItemToken res = null;
                    if (npt.Noun.IsValue("АКЦІЯ", null) || npt.Noun.IsValue("АКЦИЯ", null)) 
                    {
                        res = new FundsItemToken(t, npt.EndToken) { Typ = FundsItemTyp.Noun, Kind = Pullenti.Ner.Business.FundsKind.Stock };
                        if (npt.Adjectives.Count > 0) 
                        {
                            foreach (string v in m_ActTypes) 
                            {
                                if (npt.Adjectives[0].IsValue(v, null)) 
                                {
                                    res.StringVal = npt.GetNormalCaseText(null, Pullenti.Morph.MorphNumber.Singular, Pullenti.Morph.MorphGender.Undefined, false).ToLower();
                                    if (res.StringVal == "голосовавшая акция") 
                                        res.StringVal = "голосующая акция";
                                    break;
                                }
                            }
                        }
                    }
                    else if (((npt.Noun.IsValue("БУМАГА", null) || npt.Noun.IsValue("ПАПІР", null))) && npt.EndToken.Previous != null && ((npt.EndToken.Previous.IsValue("ЦЕННЫЙ", null) || npt.EndToken.Previous.IsValue("ЦІННИЙ", null)))) 
                        res = new FundsItemToken(t, npt.EndToken) { Typ = FundsItemTyp.Noun, Kind = Pullenti.Ner.Business.FundsKind.Stock, StringVal = "ценные бумаги" };
                    else if (((npt.Noun.IsValue("КАПИТАЛ", null) || npt.Noun.IsValue("КАПІТАЛ", null))) && npt.Adjectives.Count > 0 && ((npt.Adjectives[0].IsValue("УСТАВНОЙ", null) || npt.Adjectives[0].IsValue("УСТАВНЫЙ", null) || npt.Adjectives[0].IsValue("СТАТУТНИЙ", null)))) 
                        res = new FundsItemToken(t, npt.EndToken) { Typ = FundsItemTyp.Noun, Kind = Pullenti.Ner.Business.FundsKind.Capital };
                    if (res != null) 
                    {
                        Pullenti.Ner.ReferentToken rt = res.Kit.ProcessReferent(Pullenti.Ner.Org.OrganizationAnalyzer.ANALYZER_NAME, res.EndToken.Next, null);
                        if (rt != null) 
                        {
                            res.Ref = rt.Referent;
                            res.EndToken = rt.EndToken;
                        }
                        return res;
                    }
                }
                if (prev != null && prev.Typ == FundsItemTyp.Count) 
                {
                    string val = null;
                    foreach (string v in m_ActTypes) 
                    {
                        if (tt.IsValue(v, null)) 
                        {
                            val = v;
                            break;
                        }
                    }
                    if (val != null) 
                    {
                        int cou = 0;
                        bool ok = false;
                        for (Pullenti.Ner.Token ttt = tt.Previous; ttt != null; ttt = ttt.Previous) 
                        {
                            if ((++cou) > 100) 
                                break;
                            List<Pullenti.Ner.Referent> refs = ttt.GetReferents();
                            if (refs == null) 
                                continue;
                            foreach (Pullenti.Ner.Referent r in refs) 
                            {
                                if (r is Pullenti.Ner.Business.FundsReferent) 
                                {
                                    ok = true;
                                    break;
                                }
                            }
                            if (ok) 
                                break;
                        }
                        cou = 0;
                        if (!ok) 
                        {
                            for (Pullenti.Ner.Token ttt = tt.Next; ttt != null; ttt = ttt.Next) 
                            {
                                if ((++cou) > 100) 
                                    break;
                                FundsItemToken fi = FundsItemToken.TryParse(ttt, null);
                                if (fi != null && fi.Kind == Pullenti.Ner.Business.FundsKind.Stock) 
                                {
                                    ok = true;
                                    break;
                                }
                            }
                        }
                        if (ok) 
                        {
                            FundsItemToken res = new FundsItemToken(t, tt) { Kind = Pullenti.Ner.Business.FundsKind.Stock, Typ = FundsItemTyp.Noun };
                            res.StringVal = string.Format("{0}ая акция", val.Substring(0, val.Length - 2).ToLower());
                            return res;
                        }
                    }
                }
                if (tt is Pullenti.Ner.NumberToken) 
                {
                    Pullenti.Ner.Core.NumberExToken num = Pullenti.Ner.Core.NumberHelper.TryParseNumberWithPostfix(tt);
                    if (num != null) 
                    {
                        if (tt.Previous != null && tt.Previous.IsValue("НА", null)) 
                            break;
                        if (num.ExTyp == Pullenti.Ner.Core.NumberExType.Percent) 
                        {
                            FundsItemToken res = new FundsItemToken(t, num.EndToken) { Typ = FundsItemTyp.Percent, NumVal = num };
                            t = num.EndToken.Next;
                            if (t != null && ((t.IsChar('+') || t.IsValue("ПЛЮС", null))) && (t.Next is Pullenti.Ner.NumberToken)) 
                            {
                                res.EndToken = t.Next;
                                t = res.EndToken.Next;
                            }
                            if ((t != null && t.IsHiphen && t.Next != null) && t.Next.Chars.IsAllLower && !t.IsWhitespaceAfter) 
                                t = t.Next.Next;
                            if (t != null && ((t.IsValue("ДОЛЯ", null) || t.IsValue("ЧАСТКА", null)))) 
                                res.EndToken = t;
                            return res;
                        }
                        break;
                    }
                    Pullenti.Ner.Token t1 = tt;
                    if (t1.Next != null && t1.Next.IsValue("ШТУКА", null)) 
                        t1 = t1.Next;
                    return new FundsItemToken(t, t1) { Typ = FundsItemTyp.Count, NumVal = tt as Pullenti.Ner.NumberToken };
                }
                break;
            }
            return null;
        }
        public static Pullenti.Ner.ReferentToken TryAttach(Pullenti.Ner.Token t)
        {
            if (t == null) 
                return null;
            FundsItemToken f = TryParse(t, null);
            if (f == null) 
                return null;
            if (f.Typ == FundsItemTyp.Org) 
                return null;
            if (f.Typ == FundsItemTyp.Price || f.Typ == FundsItemTyp.Percent || f.Typ == FundsItemTyp.Count) 
            {
                if (t.Previous != null && t.Previous.IsCharOf(",.") && (t.Previous.Previous is Pullenti.Ner.NumberToken)) 
                    return null;
            }
            List<FundsItemToken> li = new List<FundsItemToken>();
            li.Add(f);
            bool isInBr = false;
            for (Pullenti.Ner.Token tt = f.EndToken.Next; tt != null; tt = tt.Next) 
            {
                if ((tt.IsWhitespaceBefore && tt.Previous != null && tt.Previous.IsChar('.')) && tt.Chars.IsCapitalUpper) 
                    break;
                FundsItemToken f0 = TryParse(tt, f);
                if (f0 != null) 
                {
                    if (f0.Kind == Pullenti.Ner.Business.FundsKind.Capital && isInBr) 
                    {
                        foreach (FundsItemToken l in li) 
                        {
                            if (l.Typ == FundsItemTyp.Noun) 
                            {
                                f0.Kind = l.Kind;
                                break;
                            }
                        }
                    }
                    li.Add((f = f0));
                    tt = f.EndToken;
                    continue;
                }
                if (tt.IsChar('(')) 
                {
                    isInBr = true;
                    continue;
                }
                if (tt.IsChar(')')) 
                {
                    if (isInBr || ((t.Previous != null && t.Previous.IsChar('(')))) 
                    {
                        isInBr = false;
                        li[li.Count - 1].EndToken = tt;
                        continue;
                    }
                }
                if (tt.Morph.Class.IsVerb || tt.Morph.Class.IsAdverb) 
                    continue;
                break;
            }
            Pullenti.Ner.Business.FundsReferent funds = new Pullenti.Ner.Business.FundsReferent();
            Pullenti.Ner.ReferentToken res = new Pullenti.Ner.ReferentToken(funds, t, t);
            Pullenti.Ner.Org.OrganizationReferent orgProb = null;
            for (int i = 0; i < li.Count; i++) 
            {
                if (li[i].Typ == FundsItemTyp.Noun) 
                {
                    funds.Kind = li[i].Kind;
                    if (li[i].StringVal != null) 
                        funds.Typ = li[i].StringVal;
                    if (li[i].Ref is Pullenti.Ner.Org.OrganizationReferent) 
                        orgProb = li[i].Ref as Pullenti.Ner.Org.OrganizationReferent;
                    res.EndToken = li[i].EndToken;
                }
                else if (li[i].Typ == FundsItemTyp.Count) 
                {
                    if (funds.Count > 0 || li[i].NumVal == null || li[i].NumVal.IntValue == null) 
                        break;
                    funds.Count = li[i].NumVal.IntValue.Value;
                    res.EndToken = li[i].EndToken;
                }
                else if (li[i].Typ == FundsItemTyp.Org) 
                {
                    if (funds.Source != null && funds.Source != li[i].Ref) 
                        break;
                    funds.Source = li[i].Ref as Pullenti.Ner.Org.OrganizationReferent;
                    res.EndToken = li[i].EndToken;
                }
                else if (li[i].Typ == FundsItemTyp.Percent) 
                {
                    if (funds.Percent > 0 || li[i].NumVal == null || li[i].NumVal.RealValue == 0) 
                        break;
                    funds.Percent = li[i].NumVal.RealValue;
                    res.EndToken = li[i].EndToken;
                }
                else if (li[i].Typ == FundsItemTyp.Sum) 
                {
                    if (funds.Sum != null) 
                        break;
                    funds.Sum = li[i].Ref as Pullenti.Ner.Money.MoneyReferent;
                    res.EndToken = li[i].EndToken;
                }
                else if (li[i].Typ == FundsItemTyp.Price) 
                {
                    if (funds.Price != null) 
                        break;
                    funds.Price = li[i].Ref as Pullenti.Ner.Money.MoneyReferent;
                    res.EndToken = li[i].EndToken;
                }
                else 
                    break;
            }
            if (funds.Percent > 0 && funds.Source != null && funds.Kind == Pullenti.Ner.Business.FundsKind.Undefined) 
                funds.Kind = Pullenti.Ner.Business.FundsKind.Stock;
            if (!funds.CheckCorrect()) 
                return null;
            if (funds.Source == null) 
            {
                int cou = 0;
                for (Pullenti.Ner.Token tt = res.BeginToken.Previous; tt != null; tt = tt.Previous) 
                {
                    if ((++cou) > 500) 
                        break;
                    if (tt.IsNewlineAfter) 
                        cou += 10;
                    Pullenti.Ner.Business.FundsReferent fr = tt.GetReferent() as Pullenti.Ner.Business.FundsReferent;
                    if (fr != null && fr.Source != null) 
                    {
                        funds.Source = fr.Source;
                        break;
                    }
                }
            }
            if (funds.Source == null && orgProb != null) 
                funds.Source = orgProb;
            if (funds.Source == null) 
            {
                int cou = 0;
                for (Pullenti.Ner.Token tt = res.BeginToken.Previous; tt != null; tt = tt.Previous) 
                {
                    if ((++cou) > 300) 
                        break;
                    if (tt.IsNewlineAfter) 
                        cou += 10;
                    List<Pullenti.Ner.Referent> refs = tt.GetReferents();
                    if (refs != null) 
                    {
                        foreach (Pullenti.Ner.Referent r in refs) 
                        {
                            if (r is Pullenti.Ner.Org.OrganizationReferent) 
                            {
                                Pullenti.Ner.Org.OrganizationKind ki = (r as Pullenti.Ner.Org.OrganizationReferent).Kind;
                                if (ki == Pullenti.Ner.Org.OrganizationKind.Justice || ki == Pullenti.Ner.Org.OrganizationKind.Govenment) 
                                    continue;
                                funds.Source = r as Pullenti.Ner.Org.OrganizationReferent;
                                cou = 10000;
                                break;
                            }
                        }
                    }
                }
            }
            return res;
        }
    }
}