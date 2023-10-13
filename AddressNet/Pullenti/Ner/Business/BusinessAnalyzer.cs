/*
 * SDK Pullenti Address, version 4.20, august 2023. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Collections;
using System.Collections.Generic;

namespace Pullenti.Ner.Business
{
    /// <summary>
    /// Анализатор для бизнес-фактов (в реальных проектах не использовалось). 
    /// Специфический анализатор, то есть нужно явно создавать процессор через функцию CreateSpecificProcessor, 
    /// указав имя анализатора.
    /// </summary>
    public class BusinessAnalyzer : Pullenti.Ner.Analyzer
    {
        /// <summary>
        /// Имя анализатора ("BUSINESS")
        /// </summary>
        public const string ANALYZER_NAME = "BUSINESS";
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
                return "Бизнес-объекты";
            }
        }
        public override string Description
        {
            get
            {
                return "Бизнес факты";
            }
        }
        /// <summary>
        /// Этот анализатор является специфическим (IsSpecific = true)
        /// </summary>
        public override bool IsSpecific
        {
            get
            {
                return true;
            }
        }
        public override Pullenti.Ner.Analyzer Clone()
        {
            return new BusinessAnalyzer();
        }
        public override ICollection<Pullenti.Ner.Metadata.ReferentClass> TypeSystem
        {
            get
            {
                return new Pullenti.Ner.Metadata.ReferentClass[] {Pullenti.Ner.Business.Internal.MetaBusinessFact.GlobalMeta, Pullenti.Ner.Business.Internal.FundsMeta.GlobalMeta};
            }
        }
        public override Dictionary<string, byte[]> Images
        {
            get
            {
                Dictionary<string, byte[]> res = new Dictionary<string, byte[]>();
                res.Add(Pullenti.Ner.Business.Internal.MetaBusinessFact.ImageId, Pullenti.Ner.Core.Internal.ResourceHelper.GetBytes("businessfact.png"));
                res.Add(Pullenti.Ner.Business.Internal.FundsMeta.ImageId, Pullenti.Ner.Core.Internal.ResourceHelper.GetBytes("creditcards.png"));
                return res;
            }
        }
        public override Pullenti.Ner.Referent CreateReferent(string type)
        {
            if (type == BusinessFactReferent.OBJ_TYPENAME) 
                return new BusinessFactReferent();
            if (type == FundsReferent.OBJ_TYPENAME) 
                return new FundsReferent();
            return null;
        }
        public override int ProgressWeight
        {
            get
            {
                return 1;
            }
        }
        public override void Process(Pullenti.Ner.Core.AnalysisKit kit)
        {
            Pullenti.Ner.Core.AnalyzerData ad = kit.GetAnalyzerData(this);
            for (Pullenti.Ner.Token t = kit.FirstToken; t != null; t = t.Next) 
            {
                if (t.IsIgnored) 
                    continue;
                Pullenti.Ner.ReferentToken rt = Pullenti.Ner.Business.Internal.FundsItemToken.TryAttach(t);
                if (rt != null) 
                {
                    rt.Referent = ad.RegisterReferent(rt.Referent);
                    kit.EmbedToken(rt);
                    t = rt;
                }
            }
            for (Pullenti.Ner.Token t = kit.FirstToken; t != null; t = t.Next) 
            {
                if (t.IsIgnored) 
                    continue;
                Pullenti.Ner.ReferentToken rt = this.AnalizeFact(t);
                if (rt != null) 
                {
                    rt.Referent = ad.RegisterReferent(rt.Referent);
                    kit.EmbedToken(rt);
                    t = rt;
                    List<Pullenti.Ner.ReferentToken> rts = this._analizeLikelihoods(rt);
                    if (rts != null) 
                    {
                        foreach (Pullenti.Ner.ReferentToken rt0 in rts) 
                        {
                            foreach (Pullenti.Ner.Slot s in rt0.Referent.Slots) 
                            {
                                if (s.TypeName == BusinessFactReferent.ATTR_WHAT && (s.Value is FundsReferent)) 
                                    rt0.Referent.UploadSlot(s, ad.RegisterReferent(s.Value as Pullenti.Ner.Referent));
                            }
                            rt0.Referent = ad.RegisterReferent(rt0.Referent);
                            kit.EmbedToken(rt0);
                            t = rt0;
                        }
                    }
                    continue;
                }
            }
        }
        Pullenti.Ner.ReferentToken AnalizeFact(Pullenti.Ner.Token t)
        {
            if (t == null) 
                return null;
            Pullenti.Ner.Business.Internal.BusinessFactItem bfi = Pullenti.Ner.Business.Internal.BusinessFactItem.TryParse(t);
            if (bfi == null) 
                return null;
            if (bfi.Typ == Pullenti.Ner.Business.Internal.BusinessFactItemTyp.Base) 
            {
                if (bfi.BaseKind == BusinessFactKind.Get || bfi.BaseKind == BusinessFactKind.Sell) 
                    return this._analizeGet(bfi);
                if (bfi.BaseKind == BusinessFactKind.Have) 
                {
                    if (bfi.IsBasePassive || bfi.Morph.Class.IsNoun) 
                    {
                        Pullenti.Ner.ReferentToken re = this._analizeHave(bfi);
                        if (re != null) 
                            return re;
                    }
                    return this._analizeGet(bfi);
                }
                if (bfi.BaseKind == BusinessFactKind.Profit || bfi.BaseKind == BusinessFactKind.Damages) 
                    return this._analizeProfit(bfi);
                if (bfi.BaseKind == BusinessFactKind.Agreement || bfi.BaseKind == BusinessFactKind.Lawsuit) 
                    return this._analizeAgreement(bfi);
                if (bfi.BaseKind == BusinessFactKind.Subsidiary) 
                    return this._analizeSubsidiary(bfi);
                if (bfi.BaseKind == BusinessFactKind.Finance) 
                    return this._analizeFinance(bfi);
            }
            return null;
        }
        Pullenti.Ner.ReferentToken _FindRefBefore(Pullenti.Ner.Token t)
        {
            if (t == null) 
                return null;
            int points = 0;
            Pullenti.Ner.Token t0 = null;
            Pullenti.Ner.Token t1 = t;
            for (; t != null; t = t.Previous) 
            {
                if (t.IsNewlineAfter) 
                    break;
                if (t.Morph.Class.IsAdverb || t.Morph.Class.IsPreposition || t.IsComma) 
                    continue;
                if (t.Morph.Class.IsPersonalPronoun) 
                    break;
                if (t.IsValue("ИНФОРМАЦИЯ", null) || t.IsValue("ДАННЫЕ", null)) 
                    continue;
                if (t.IsValue("ІНФОРМАЦІЯ", null) || t.IsValue("ДАНІ", null)) 
                    continue;
                if (t is Pullenti.Ner.TextToken) 
                {
                    if (t.Morph.Class.IsVerb) 
                        break;
                    if (t.IsChar('.')) 
                        break;
                    continue;
                }
                Pullenti.Ner.Referent r = t.GetReferent();
                if ((r is Pullenti.Ner.Date.DateReferent) || (r is Pullenti.Ner.Date.DateRangeReferent)) 
                    continue;
                break;
            }
            if (t == null) 
                return null;
            if (t.Morph.Class.IsPersonalPronoun) 
            {
                t0 = t;
                points = 1;
                t = t.Previous;
            }
            else 
            {
                if (t.Morph.Class.IsPronoun) 
                {
                    t = t.Previous;
                    if (t != null && t.IsChar(',')) 
                        t = t.Previous;
                }
                if (t == null) 
                    return null;
                List<Pullenti.Ner.Referent> refs = t.GetReferents();
                if (refs != null) 
                {
                    foreach (Pullenti.Ner.Referent r in refs) 
                    {
                        if ((r is Pullenti.Ner.Person.PersonReferent) || (r is Pullenti.Ner.Org.OrganizationReferent) || (r is FundsReferent)) 
                            return new Pullenti.Ner.ReferentToken(r, t, t1);
                    }
                }
                return null;
            }
            for (; t != null; t = t.Previous) 
            {
                if (t.IsChar('.')) 
                {
                    if ((--points) < 0) 
                        break;
                    continue;
                }
                List<Pullenti.Ner.Referent> refs = t.GetReferents();
                if (refs != null) 
                {
                    foreach (Pullenti.Ner.Referent r in refs) 
                    {
                        if ((r is Pullenti.Ner.Person.PersonReferent) || (r is Pullenti.Ner.Org.OrganizationReferent)) 
                            return new Pullenti.Ner.ReferentToken(r, t0, t1);
                    }
                }
            }
            return null;
        }
        Pullenti.Ner.ReferentToken _FindSecRefBefore(Pullenti.Ner.ReferentToken rt)
        {
            Pullenti.Ner.Token t = (rt == null ? null : rt.BeginToken.Previous);
            if (t == null || t.WhitespacesAfterCount > 2) 
                return null;
            if ((rt.GetReferent() is Pullenti.Ner.Person.PersonReferent) && (t.GetReferent() is Pullenti.Ner.Org.OrganizationReferent)) 
                return t as Pullenti.Ner.ReferentToken;
            return null;
        }
        bool _findDate(BusinessFactReferent bfr, Pullenti.Ner.Token t)
        {
            for (Pullenti.Ner.Token tt = t; tt != null; tt = tt.Previous) 
            {
                Pullenti.Ner.Referent r = tt.GetReferent();
                if ((r is Pullenti.Ner.Date.DateReferent) || (r is Pullenti.Ner.Date.DateRangeReferent)) 
                {
                    bfr.When = r;
                    return true;
                }
                if (tt.IsChar('.')) 
                    break;
                if (tt.IsNewlineBefore) 
                    break;
            }
            for (Pullenti.Ner.Token tt = t; tt != null; tt = tt.Next) 
            {
                if (tt != t && tt.IsNewlineBefore) 
                    break;
                Pullenti.Ner.Referent r = tt.GetReferent();
                if ((r is Pullenti.Ner.Date.DateReferent) || (r is Pullenti.Ner.Date.DateRangeReferent)) 
                {
                    bfr.When = r;
                    return true;
                }
                if (tt.IsChar('.')) 
                    break;
            }
            return false;
        }
        bool _findSum(BusinessFactReferent bfr, Pullenti.Ner.Token t)
        {
            for (; t != null; t = t.Next) 
            {
                if (t.IsChar('.') || t.IsNewlineBefore) 
                    break;
                Pullenti.Ner.Referent r = t.GetReferent();
                if (r is Pullenti.Ner.Money.MoneyReferent) 
                {
                    FundsReferent fu = bfr.GetSlotValue(BusinessFactReferent.ATTR_WHAT) as FundsReferent;
                    if (fu != null) 
                    {
                        if (fu.Sum == null) 
                        {
                            fu.Sum = r as Pullenti.Ner.Money.MoneyReferent;
                            return true;
                        }
                    }
                    bfr.AddSlot(BusinessFactReferent.ATTR_MISC, r, false, 0);
                    return true;
                }
            }
            return false;
        }
        Pullenti.Ner.ReferentToken _analizeGet(Pullenti.Ner.Business.Internal.BusinessFactItem bfi)
        {
            Pullenti.Ner.ReferentToken bef = this._FindRefBefore(bfi.BeginToken.Previous);
            if (bef == null) 
                return null;
            Pullenti.Ner.Token t1 = bfi.EndToken.Next;
            if (t1 == null) 
                return null;
            for (; t1 != null; t1 = t1.Next) 
            {
                if (t1.Morph.Class.IsAdverb) 
                    continue;
                if (t1.IsValue("ПРАВО", null) || t1.IsValue("РАСПОРЯЖАТЬСЯ", null) || t1.IsValue("РОЗПОРЯДЖАТИСЯ", null)) 
                    continue;
                break;
            }
            if (t1 == null) 
                return null;
            if ((t1.GetReferent() is FundsReferent) && !(bef.Referent is FundsReferent)) 
            {
                FundsReferent fr = t1.GetReferent() as FundsReferent;
                BusinessFactReferent bfr = new BusinessFactReferent() { Kind = bfi.BaseKind };
                bfr.Who = bef.Referent;
                Pullenti.Ner.ReferentToken bef2 = this._FindSecRefBefore(bef);
                if (bef2 != null) 
                {
                    bfr.AddSlot(BusinessFactReferent.ATTR_WHO, bef2.Referent, false, 0);
                    bef = bef2;
                }
                if (fr.Source == bef.Referent && bef2 == null) 
                {
                    bef2 = this._FindRefBefore(bef.BeginToken.Previous);
                    if (bef2 != null) 
                    {
                        bef = bef2;
                        bfr.Who = bef.Referent;
                    }
                }
                if (fr.Source == bef.Referent) 
                {
                    int cou = 0;
                    for (Pullenti.Ner.Token tt = bef.BeginToken.Previous; tt != null; tt = tt.Previous) 
                    {
                        if ((++cou) > 100) 
                            break;
                        List<Pullenti.Ner.Referent> refs = tt.GetReferents();
                        if (refs == null) 
                            continue;
                        foreach (Pullenti.Ner.Referent r in refs) 
                        {
                            if ((r is Pullenti.Ner.Org.OrganizationReferent) && r != bef.Referent) 
                            {
                                cou = 1000;
                                fr.Source = r as Pullenti.Ner.Org.OrganizationReferent;
                                break;
                            }
                        }
                    }
                }
                bfr.AddWhat(fr);
                bfr.Typ = (bfi.BaseKind == BusinessFactKind.Get ? "покупка ценных бумаг" : (bfi.BaseKind == BusinessFactKind.Sell ? "продажа ценных бумаг" : "владение ценными бумагами"));
                this._findDate(bfr, bef.BeginToken);
                this._findSum(bfr, bef.EndToken);
                return new Pullenti.Ner.ReferentToken(bfr, bef.BeginToken, t1);
            }
            if ((bfi.Morph.Class.IsNoun && ((bfi.BaseKind == BusinessFactKind.Get || bfi.BaseKind == BusinessFactKind.Sell)) && (t1.GetReferent() is Pullenti.Ner.Org.OrganizationReferent)) || (t1.GetReferent() is Pullenti.Ner.Person.PersonReferent)) 
            {
                if ((bef.Referent is FundsReferent) || (bef.Referent is Pullenti.Ner.Org.OrganizationReferent)) 
                {
                    BusinessFactReferent bfr = new BusinessFactReferent() { Kind = bfi.BaseKind };
                    if (bfi.BaseKind == BusinessFactKind.Get) 
                        bfr.Typ = (bef.Referent is FundsReferent ? "покупка ценных бумаг" : "покупка компании");
                    else if (bfi.BaseKind == BusinessFactKind.Sell) 
                        bfr.Typ = (bef.Referent is FundsReferent ? "продажа ценных бумаг" : "продажа компании");
                    bfr.Who = t1.GetReferent();
                    bfr.AddWhat(bef.Referent);
                    this._findDate(bfr, bef.BeginToken);
                    this._findSum(bfr, bef.EndToken);
                    t1 = _addWhosList(t1, bfr);
                    return new Pullenti.Ner.ReferentToken(bfr, bef.BeginToken, t1);
                }
            }
            if ((bef.Referent is Pullenti.Ner.Org.OrganizationReferent) || (bef.Referent is Pullenti.Ner.Person.PersonReferent)) 
            {
                Pullenti.Ner.Token tt = t1;
                if (tt != null && tt.Morph.Class.IsPreposition) 
                    tt = tt.Next;
                Pullenti.Ner.Referent slav = (tt == null ? null : tt.GetReferent());
                if ((((slav is Pullenti.Ner.Person.PersonReferent) || (slav is Pullenti.Ner.Org.OrganizationReferent))) && tt.Next != null && (tt.Next.GetReferent() is FundsReferent)) 
                {
                    BusinessFactReferent bfr = new BusinessFactReferent() { Kind = bfi.BaseKind };
                    bfr.Typ = (bfi.BaseKind == BusinessFactKind.Get ? "покупка ценных бумаг" : "продажа ценных бумаг");
                    bfr.Who = bef.Referent;
                    Pullenti.Ner.ReferentToken bef2 = this._FindSecRefBefore(bef);
                    if (bef2 != null) 
                    {
                        bfr.AddSlot(BusinessFactReferent.ATTR_WHO, bef2.Referent, false, 0);
                        bef = bef2;
                    }
                    bfr.Whom = slav;
                    bfr.AddWhat(tt.Next.GetReferent());
                    this._findDate(bfr, bef.BeginToken);
                    this._findSum(bfr, bef.EndToken);
                    return new Pullenti.Ner.ReferentToken(bfr, bef.BeginToken, tt.Next);
                }
                else if (slav is Pullenti.Ner.Org.OrganizationReferent) 
                {
                    BusinessFactReferent bfr = new BusinessFactReferent() { Kind = bfi.BaseKind };
                    bfr.Typ = (bfi.BaseKind == BusinessFactKind.Get ? "покупка компании" : "продажа компании");
                    bfr.Who = bef.Referent;
                    Pullenti.Ner.ReferentToken bef2 = this._FindSecRefBefore(bef);
                    if (bef2 != null) 
                    {
                        bfr.AddSlot(BusinessFactReferent.ATTR_WHO, bef2.Referent, false, 0);
                        bef = bef2;
                    }
                    bfr.AddWhat(slav);
                    this._findDate(bfr, bef.BeginToken);
                    this._findSum(bfr, bef.EndToken);
                    return new Pullenti.Ner.ReferentToken(bfr, bef.BeginToken, tt.Next);
                }
            }
            if ((bef.Referent is FundsReferent) && (((t1.GetReferent() is Pullenti.Ner.Org.OrganizationReferent) || (t1.GetReferent() is Pullenti.Ner.Person.PersonReferent)))) 
            {
                BusinessFactReferent bfr = new BusinessFactReferent() { Kind = bfi.BaseKind };
                bfr.Typ = (bfi.BaseKind == BusinessFactKind.Get ? "покупка ценных бумаг" : (bfi.BaseKind == BusinessFactKind.Sell ? "продажа ценных бумаг" : "владение ценными бумагами"));
                bfr.Who = t1.GetReferent();
                bfr.AddWhat(bef.Referent);
                this._findDate(bfr, bef.BeginToken);
                this._findSum(bfr, bef.EndToken);
                return new Pullenti.Ner.ReferentToken(bfr, bef.BeginToken, t1);
            }
            return null;
        }
        static Pullenti.Ner.Token _addWhosList(Pullenti.Ner.Token t1, BusinessFactReferent bfr)
        {
            if (t1 == null) 
                return null;
            if ((t1.Next != null && t1.Next.IsCommaAnd && (t1.Next.Next is Pullenti.Ner.ReferentToken)) && t1.Next.Next.GetReferent().TypeName == t1.GetReferent().TypeName) 
            {
                List<Pullenti.Ner.Referent> li = new List<Pullenti.Ner.Referent>();
                li.Add(t1.Next.Next.GetReferent());
                if (t1.Next.IsAnd) 
                    t1 = t1.Next.Next;
                else 
                {
                    bool ok = false;
                    for (Pullenti.Ner.Token tt = t1.Next.Next.Next; tt != null; tt = tt.Next) 
                    {
                        if (!tt.IsCommaAnd) 
                            break;
                        if (!(tt.Next is Pullenti.Ner.ReferentToken)) 
                            break;
                        if (tt.Next.GetReferent().TypeName != t1.GetReferent().TypeName) 
                            break;
                        li.Add(tt.Next.GetReferent());
                        if (tt.IsAnd) 
                        {
                            ok = true;
                            t1 = tt.Next;
                            break;
                        }
                    }
                    if (!ok) 
                        li = null;
                }
                if (li != null) 
                {
                    foreach (Pullenti.Ner.Referent r in li) 
                    {
                        bfr.AddSlot(BusinessFactReferent.ATTR_WHO, r, false, 0);
                    }
                }
            }
            return t1;
        }
        Pullenti.Ner.ReferentToken _analizeGet2(Pullenti.Ner.Token t)
        {
            if (t == null) 
                return null;
            Pullenti.Ner.Token tt = t.Previous;
            Pullenti.Ner.Token ts = t;
            if (tt != null && tt.IsComma) 
                tt = tt.Previous;
            Pullenti.Ner.ReferentToken bef = this._FindRefBefore(tt);
            Pullenti.Ner.Referent master = null;
            Pullenti.Ner.Referent slave = null;
            if (bef != null && (bef.Referent is FundsReferent)) 
            {
                slave = bef.Referent;
                ts = bef.BeginToken;
            }
            tt = t.Next;
            if (tt == null) 
                return null;
            Pullenti.Ner.Token te = tt;
            Pullenti.Ner.Referent r = tt.GetReferent();
            if ((r is Pullenti.Ner.Person.PersonReferent) || (r is Pullenti.Ner.Org.OrganizationReferent)) 
            {
                master = r;
                if (slave == null && tt.Next != null) 
                {
                    if ((((r = tt.Next.GetReferent()))) != null) 
                    {
                        if ((r is FundsReferent) || (r is Pullenti.Ner.Org.OrganizationReferent)) 
                        {
                            slave = r as FundsReferent;
                            te = tt.Next;
                        }
                    }
                }
            }
            if (master != null && slave != null) 
            {
                BusinessFactReferent bfr = new BusinessFactReferent() { Kind = BusinessFactKind.Have };
                bfr.Who = master;
                if (slave is Pullenti.Ner.Org.OrganizationReferent) 
                {
                    bfr.AddWhat(slave);
                    bfr.Typ = "владение компанией";
                }
                else if (slave is FundsReferent) 
                {
                    bfr.AddWhat(slave);
                    bfr.Typ = "владение ценными бумагами";
                }
                else 
                    return null;
                return new Pullenti.Ner.ReferentToken(bfr, ts, te);
            }
            return null;
        }
        Pullenti.Ner.ReferentToken _analizeHave(Pullenti.Ner.Business.Internal.BusinessFactItem bfi)
        {
            Pullenti.Ner.Token t = bfi.EndToken.Next;
            Pullenti.Ner.Token t1 = null;
            if (t != null && ((t.IsValue("КОТОРЫЙ", null) || t.IsValue("ЯКИЙ", null)))) 
                t1 = t.Next;
            else 
            {
                for (Pullenti.Ner.Token tt = bfi.BeginToken; tt != bfi.EndToken; tt = tt.Next) 
                {
                    if (tt.Morph.Class.IsPronoun) 
                        t1 = t;
                }
                if (t1 == null) 
                {
                    if (bfi.IsBasePassive && t != null && (((t.GetReferent() is Pullenti.Ner.Person.PersonReferent) || (t.GetReferent() is Pullenti.Ner.Org.OrganizationReferent)))) 
                    {
                        t1 = t;
                        if (t.Next != null && (t.Next.GetReferent() is FundsReferent)) 
                        {
                            BusinessFactReferent bfr = new BusinessFactReferent() { Kind = BusinessFactKind.Have };
                            bfr.Who = t.GetReferent();
                            bfr.AddWhat(t.Next.GetReferent());
                            bfr.Typ = "владение ценными бумагами";
                            return new Pullenti.Ner.ReferentToken(bfr, bfi.BeginToken, t.Next);
                        }
                    }
                }
            }
            Pullenti.Ner.Token t0 = null;
            Pullenti.Ner.Referent slave = null;
            bool musBeVerb = false;
            if (t1 != null) 
            {
                Pullenti.Ner.Token tt0 = bfi.BeginToken.Previous;
                if (tt0 != null && tt0.IsChar(',')) 
                    tt0 = tt0.Previous;
                Pullenti.Ner.ReferentToken bef = this._FindRefBefore(tt0);
                if (bef == null) 
                    return null;
                if (!(bef.Referent is Pullenti.Ner.Org.OrganizationReferent)) 
                    return null;
                t0 = bef.BeginToken;
                slave = bef.Referent;
            }
            else if (bfi.EndToken.GetMorphClassInDictionary().IsNoun && (t.GetReferent() is Pullenti.Ner.Org.OrganizationReferent)) 
            {
                slave = t.GetReferent();
                t1 = t.Next;
                t0 = bfi.BeginToken;
                musBeVerb = true;
            }
            if (t0 == null || t1 == null || slave == null) 
                return null;
            if ((t1.IsHiphen || t1.IsValue("ЯВЛЯТЬСЯ", null) || t1.IsValue("БУТИ", null)) || t1.IsValue("Є", null)) 
                t1 = t1.Next;
            else if (musBeVerb) 
                return null;
            Pullenti.Ner.Referent r = (t1 == null ? null : t1.GetReferent());
            if ((r is Pullenti.Ner.Org.OrganizationReferent) || (r is Pullenti.Ner.Person.PersonReferent)) 
            {
                BusinessFactReferent bfr = new BusinessFactReferent() { Kind = BusinessFactKind.Have };
                bfr.Who = r;
                bfr.AddWhat(slave);
                if (bfi.EndToken.IsValue("АКЦИОНЕР", null) || bfi.EndToken.IsValue("АКЦІОНЕР", null)) 
                    bfr.Typ = "владение ценными бумагами";
                else 
                    bfr.Typ = "владение компанией";
                t1 = _addWhosList(t1, bfr);
                return new Pullenti.Ner.ReferentToken(bfr, t0, t1);
            }
            return null;
        }
        Pullenti.Ner.ReferentToken _analizeProfit(Pullenti.Ner.Business.Internal.BusinessFactItem bfi)
        {
            if (bfi.EndToken.Next == null) 
                return null;
            Pullenti.Ner.Token t0 = bfi.BeginToken;
            Pullenti.Ner.Token t1 = bfi.EndToken;
            string typ = t1.GetNormalCaseText(null, Pullenti.Morph.MorphNumber.Singular, Pullenti.Morph.MorphGender.Undefined, false).ToLower();
            Pullenti.Ner.Org.OrganizationReferent org = null;
            org = t1.Next.GetReferent() as Pullenti.Ner.Org.OrganizationReferent;
            Pullenti.Ner.Token t = t1;
            if (org != null) 
                t = t.Next;
            else 
            {
                Pullenti.Ner.ReferentToken rt = t.Kit.ProcessReferent(Pullenti.Ner.Org.OrganizationAnalyzer.ANALYZER_NAME, t.Next, null);
                if (rt != null) 
                {
                    org = rt.Referent as Pullenti.Ner.Org.OrganizationReferent;
                    t = rt.EndToken;
                }
            }
            Pullenti.Ner.Referent dt = null;
            Pullenti.Ner.Money.MoneyReferent sum = null;
            for (t = t.Next; t != null; t = t.Next) 
            {
                if (t.IsChar('.')) 
                    break;
                if (t.IsChar('(')) 
                {
                    Pullenti.Ner.Core.BracketSequenceToken br = Pullenti.Ner.Core.BracketHelper.TryParse(t, Pullenti.Ner.Core.BracketParseAttr.No, 100);
                    if (br != null) 
                    {
                        t = br.EndToken;
                        continue;
                    }
                }
                if ((((t.Morph.Class.IsVerb || t.IsValue("ДО", null) || t.IsHiphen) || t.IsValue("РАЗМЕР", null) || t.IsValue("РОЗМІР", null))) && t.Next != null && (t.Next.GetReferent() is Pullenti.Ner.Money.MoneyReferent)) 
                {
                    if (sum != null) 
                        break;
                    sum = t.Next.GetReferent() as Pullenti.Ner.Money.MoneyReferent;
                    t1 = (t = t.Next);
                    continue;
                }
                Pullenti.Ner.Referent r = t.GetReferent();
                if ((r is Pullenti.Ner.Date.DateRangeReferent) || (r is Pullenti.Ner.Date.DateReferent)) 
                {
                    if (dt == null) 
                    {
                        dt = r;
                        t1 = t;
                    }
                }
                else if ((r is Pullenti.Ner.Org.OrganizationReferent) && org == null) 
                {
                    org = r as Pullenti.Ner.Org.OrganizationReferent;
                    t1 = t;
                }
            }
            if (sum == null) 
                return null;
            if (org == null) 
            {
                for (Pullenti.Ner.Token tt = t0.Previous; tt != null; tt = tt.Previous) 
                {
                    if (tt.IsChar('.')) 
                        break;
                    BusinessFactReferent b0 = tt.GetReferent() as BusinessFactReferent;
                    if (b0 != null) 
                    {
                        org = b0.Who as Pullenti.Ner.Org.OrganizationReferent;
                        break;
                    }
                    if ((((org = tt.GetReferent() as Pullenti.Ner.Org.OrganizationReferent))) != null) 
                        break;
                }
            }
            if (org == null) 
                return null;
            BusinessFactReferent bfr = new BusinessFactReferent() { Kind = bfi.BaseKind };
            bfr.Who = org;
            bfr.Typ = typ;
            bfr.AddSlot(BusinessFactReferent.ATTR_MISC, sum, false, 0);
            if (dt != null) 
                bfr.When = dt;
            else 
                this._findDate(bfr, bfi.BeginToken);
            return new Pullenti.Ner.ReferentToken(bfr, t0, t1);
        }
        Pullenti.Ner.ReferentToken _analizeAgreement(Pullenti.Ner.Business.Internal.BusinessFactItem bfi)
        {
            Pullenti.Ner.Referent first = null;
            Pullenti.Ner.Referent second = null;
            Pullenti.Ner.Token t0 = bfi.BeginToken;
            Pullenti.Ner.Token t1 = bfi.EndToken;
            int maxLines = 1;
            for (Pullenti.Ner.Token t = bfi.BeginToken.Previous; t != null; t = t.Previous) 
            {
                if (t.IsChar('.') || t.IsNewlineAfter) 
                {
                    if ((--maxLines) == 0) 
                        break;
                    continue;
                }
                if (t.IsValue("СТОРОНА", null) && t.Previous != null && ((t.Previous.IsValue("МЕЖДУ", null) || t.Previous.IsValue("МІЖ", null)))) 
                {
                    maxLines = 2;
                    t0 = (t = t.Previous);
                    continue;
                }
                Pullenti.Ner.Referent r = t.GetReferent();
                if (r is BusinessFactReferent) 
                {
                    BusinessFactReferent b = r as BusinessFactReferent;
                    if (b.Who != null && ((b.Who2 != null || b.Whom != null))) 
                    {
                        first = b.Who;
                        second = b.Who2 ?? b.Whom;
                        break;
                    }
                }
                if (!(r is Pullenti.Ner.Org.OrganizationReferent)) 
                    continue;
                if ((t.Previous != null && ((t.Previous.IsAnd || t.Previous.IsValue("К", null))) && t.Previous.Previous != null) && (t.Previous.Previous.GetReferent() is Pullenti.Ner.Org.OrganizationReferent)) 
                {
                    t0 = t.Previous.Previous;
                    first = t0.GetReferent();
                    second = r;
                    break;
                }
                else 
                {
                    t0 = t;
                    first = r;
                    break;
                }
            }
            if (second == null) 
            {
                for (Pullenti.Ner.Token t = bfi.EndToken.Next; t != null; t = t.Next) 
                {
                    if (t.IsChar('.')) 
                        break;
                    if (t.IsNewlineBefore) 
                        break;
                    Pullenti.Ner.Referent r = t.GetReferent();
                    if (!(r is Pullenti.Ner.Org.OrganizationReferent)) 
                        continue;
                    if ((t.Next != null && ((t.Next.IsAnd || t.Next.IsValue("К", null))) && t.Next.Next != null) && (t.Next.Next.GetReferent() is Pullenti.Ner.Org.OrganizationReferent)) 
                    {
                        t1 = t.Next.Next;
                        first = r;
                        second = t1.GetReferent();
                        break;
                    }
                    else 
                    {
                        t1 = t;
                        second = r;
                        break;
                    }
                }
            }
            if (first == null || second == null) 
                return null;
            BusinessFactReferent bf = new BusinessFactReferent() { Kind = bfi.BaseKind };
            bf.Who = first;
            if (bfi.BaseKind == BusinessFactKind.Lawsuit) 
                bf.Whom = second;
            else 
                bf.Who2 = second;
            this._findDate(bf, bfi.BeginToken);
            this._findSum(bf, bfi.BeginToken);
            return new Pullenti.Ner.ReferentToken(bf, t0, t1);
        }
        Pullenti.Ner.ReferentToken _analizeSubsidiary(Pullenti.Ner.Business.Internal.BusinessFactItem bfi)
        {
            Pullenti.Ner.Token t1 = bfi.EndToken.Next;
            if (t1 == null || !(t1.GetReferent() is Pullenti.Ner.Org.OrganizationReferent)) 
                return null;
            Pullenti.Ner.Token t;
            Pullenti.Ner.Org.OrganizationReferent org0 = null;
            for (t = bfi.BeginToken.Previous; t != null; t = t.Previous) 
            {
                if (t.IsChar('(') || t.IsChar('%')) 
                    continue;
                if (t.Morph.Class.IsVerb) 
                    continue;
                if (t is Pullenti.Ner.NumberToken) 
                    continue;
                org0 = t.GetReferent() as Pullenti.Ner.Org.OrganizationReferent;
                if (org0 != null) 
                    break;
            }
            if (org0 == null) 
                return null;
            BusinessFactReferent bfr = new BusinessFactReferent() { Kind = bfi.BaseKind };
            bfr.Who = org0;
            bfr.Whom = t1.GetReferent();
            return new Pullenti.Ner.ReferentToken(bfr, t, t1);
        }
        Pullenti.Ner.ReferentToken _analizeFinance(Pullenti.Ner.Business.Internal.BusinessFactItem bfi)
        {
            Pullenti.Ner.ReferentToken bef = this._FindRefBefore(bfi.BeginToken.Previous);
            if (bef == null) 
                return null;
            if (!(bef.Referent is Pullenti.Ner.Org.OrganizationReferent) && !(bef.Referent is Pullenti.Ner.Person.PersonReferent)) 
                return null;
            Pullenti.Ner.ReferentToken whom = null;
            Pullenti.Ner.Money.MoneyReferent sum = null;
            FundsReferent funds = null;
            for (Pullenti.Ner.Token t = bfi.EndToken.Next; t != null; t = t.Next) 
            {
                if (t.IsNewlineBefore || t.IsChar('.')) 
                    break;
                Pullenti.Ner.Referent r = t.GetReferent();
                if (r is Pullenti.Ner.Org.OrganizationReferent) 
                {
                    if (whom == null) 
                        whom = t as Pullenti.Ner.ReferentToken;
                }
                else if (r is Pullenti.Ner.Money.MoneyReferent) 
                {
                    if (sum == null) 
                        sum = r as Pullenti.Ner.Money.MoneyReferent;
                }
                else if (r is FundsReferent) 
                {
                    if (funds == null) 
                        funds = r as FundsReferent;
                }
            }
            if (whom == null) 
                return null;
            BusinessFactReferent bfr = new BusinessFactReferent();
            if (funds == null) 
                bfr.Kind = BusinessFactKind.Finance;
            else 
            {
                bfr.Kind = BusinessFactKind.Get;
                bfr.Typ = "покупка ценных бумаг";
            }
            bfr.Who = bef.Referent;
            bfr.Whom = whom.Referent;
            if (funds != null) 
                bfr.AddWhat(funds);
            if (sum != null) 
                bfr.AddSlot(BusinessFactReferent.ATTR_MISC, sum, false, 0);
            this._findDate(bfr, bef.BeginToken);
            return new Pullenti.Ner.ReferentToken(bfr, bef.BeginToken, whom.EndToken);
        }
        List<Pullenti.Ner.ReferentToken> _analizeLikelihoods(Pullenti.Ner.ReferentToken rt)
        {
            BusinessFactReferent bfr0 = rt.Referent as BusinessFactReferent;
            if (bfr0 == null || bfr0.Whats.Count != 1 || !(bfr0.Whats[0] is FundsReferent)) 
                return null;
            FundsReferent funds0 = bfr0.Whats[0] as FundsReferent;
            Pullenti.Ner.Token t;
            List<Pullenti.Ner.ReferentToken> whos = new List<Pullenti.Ner.ReferentToken>();
            List<FundsReferent> funds = new List<FundsReferent>();
            for (t = rt.EndToken.Next; t != null; t = t.Next) 
            {
                if (t.IsNewlineBefore || t.IsChar('.')) 
                    break;
                if (t.Morph.Class.IsAdverb) 
                    continue;
                if (t.IsHiphen || t.IsCommaAnd) 
                    continue;
                if (t.Morph.Class.IsConjunction || t.Morph.Class.IsPreposition || t.Morph.Class.IsMisc) 
                    continue;
                Pullenti.Ner.Referent r = t.GetReferent();
                if ((r is Pullenti.Ner.Org.OrganizationReferent) || (r is Pullenti.Ner.Person.PersonReferent)) 
                {
                    whos.Add(t as Pullenti.Ner.ReferentToken);
                    continue;
                }
                if (r is FundsReferent) 
                {
                    funds0 = r as FundsReferent;
                    funds.Add(funds0);
                    continue;
                }
                Pullenti.Ner.Business.Internal.FundsItemToken it = Pullenti.Ner.Business.Internal.FundsItemToken.TryParse(t, null);
                if (it == null) 
                    break;
                FundsReferent fu = funds0.Clone() as FundsReferent;
                fu.Occurrence.Clear();
                fu.AddOccurenceOfRefTok(new Pullenti.Ner.ReferentToken(fu, it.BeginToken, it.EndToken));
                if (it.Typ == Pullenti.Ner.Business.Internal.FundsItemTyp.Percent && it.NumVal != null) 
                    fu.Percent = it.NumVal.RealValue;
                else if (it.Typ == Pullenti.Ner.Business.Internal.FundsItemTyp.Count && it.NumVal != null && it.NumVal.IntValue != null) 
                    fu.Count = it.NumVal.IntValue.Value;
                else if (it.Typ == Pullenti.Ner.Business.Internal.FundsItemTyp.Sum) 
                    fu.Sum = it.Ref as Pullenti.Ner.Money.MoneyReferent;
                else 
                    break;
                funds.Add(fu);
                t = it.EndToken;
            }
            if (whos.Count == 0 || whos.Count != funds.Count) 
                return null;
            List<Pullenti.Ner.ReferentToken> res = new List<Pullenti.Ner.ReferentToken>();
            for (int i = 0; i < whos.Count; i++) 
            {
                BusinessFactReferent bfr = new BusinessFactReferent() { Kind = bfr0.Kind, Typ = bfr0.Typ };
                bfr.Who = whos[i].Referent;
                bfr.AddWhat(funds[i]);
                foreach (Pullenti.Ner.Slot s in bfr0.Slots) 
                {
                    if (s.TypeName == BusinessFactReferent.ATTR_MISC || s.TypeName == BusinessFactReferent.ATTR_WHEN) 
                        bfr.AddSlot(s.TypeName, s.Value, false, 0);
                }
                res.Add(new Pullenti.Ner.ReferentToken(bfr, whos[i].BeginToken, whos[i].EndToken));
            }
            return res;
        }
        static bool m_Inited;
        public static void Initialize()
        {
            if (m_Inited) 
                return;
            m_Inited = true;
            Pullenti.Ner.Business.Internal.MetaBusinessFact.Initialize();
            Pullenti.Ner.Business.Internal.FundsMeta.Initialize();
            Pullenti.Ner.Core.Termin.AssignAllTextsAsNormal = true;
            Pullenti.Ner.Business.Internal.BusinessFactItem.Initialize();
            Pullenti.Ner.Core.Termin.AssignAllTextsAsNormal = false;
            Pullenti.Ner.ProcessorService.RegisterAnalyzer(new BusinessAnalyzer());
        }
    }
}