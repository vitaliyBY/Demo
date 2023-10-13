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
    class OrgTypToken : Pullenti.Ner.MetaToken
    {
        public OrgTypToken(Pullenti.Ner.Token b, Pullenti.Ner.Token e, string val = null) : base(b, e, null)
        {
            if (val != null) 
                Vals.Add(val);
        }
        public bool IsDoubt;
        public bool NotOrg;
        public bool NotGeo;
        public bool CanBeSingle;
        public List<string> Vals = new List<string>();
        public OrgTypToken Clone()
        {
            OrgTypToken res = new OrgTypToken(BeginToken, EndToken);
            res.Vals.AddRange(Vals);
            res.IsDoubt = IsDoubt;
            res.NotOrg = NotOrg;
            res.NotGeo = NotGeo;
            res.CanBeSingle = CanBeSingle;
            return res;
        }
        public override string ToString()
        {
            StringBuilder tmp = new StringBuilder();
            if (IsDoubt) 
                tmp.Append("? ");
            for (int i = 0; i < Vals.Count; i++) 
            {
                if (i > 0) 
                    tmp.Append(" / ");
                tmp.Append(Vals[i]);
            }
            if (NotOrg) 
                tmp.Append(", not Org");
            if (NotGeo) 
                tmp.Append(", not Geo");
            if (CanBeSingle) 
                tmp.Append(", Single");
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
            ad.OTRegime = false;
            OrgTypToken lastTyp = null;
            for (Pullenti.Ner.Token t = t0; t != null; t = t.Next) 
            {
                bool afterTerr = false;
                Pullenti.Ner.Token tt = MiscLocationHelper.CheckTerritory(t);
                if (tt != null && tt.Next != null) 
                {
                    afterTerr = true;
                    t = tt.Next;
                }
                else if (lastTyp != null && lastTyp.EndToken.Next == t) 
                    afterTerr = true;
                GeoTokenData d = t.Tag as GeoTokenData;
                OrgTypToken ty = TryParse(t, afterTerr, ad);
                if (ty != null) 
                {
                    if (d == null) 
                        d = new GeoTokenData(t);
                    d.OrgTyp = ty;
                    t = ty.EndToken;
                    lastTyp = ty;
                }
            }
            ad.OTRegime = true;
        }
        public static OrgTypToken TryParse(Pullenti.Ner.Token t, bool afterTerr, GeoAnalyzerData ad = null)
        {
            if (!(t is Pullenti.Ner.TextToken)) 
                return null;
            if (t.LengthChar == 1 && !t.Chars.IsLetter) 
                return null;
            if ((t.LengthChar == 1 && t.Chars.IsAllLower && t.IsChar('м')) && t.Next != null && t.Next.IsChar('.')) 
            {
                if (MiscLocationHelper.IsUserParamAddress(t)) 
                {
                    Pullenti.Ner.Token tt = t.Previous;
                    if (tt != null && tt.IsComma) 
                        tt = tt.Previous;
                    if (tt is Pullenti.Ner.ReferentToken) 
                    {
                        Pullenti.Ner.Geo.GeoReferent geo = tt.GetReferent() as Pullenti.Ner.Geo.GeoReferent;
                        if (geo != null && geo.IsRegion) 
                        {
                            OrgTypToken mm = new OrgTypToken(t, t.Next);
                            mm.Vals.Add("местечко");
                            return mm;
                        }
                    }
                }
            }
            if (((t.LengthChar == 1 && t.Next != null && t.Next.IsHiphen) && t.IsValue("П", null) && (t.Next.Next is Pullenti.Ner.TextToken)) && t.Next.Next.IsValue("Т", null)) 
            {
                if (Pullenti.Ner.Core.BracketHelper.IsBracket(t.Next.Next.Next, true)) 
                    return new OrgTypToken(t, t.Next.Next, "пансионат");
                Pullenti.Ner.Token tt = t.Previous;
                if (tt != null && tt.IsComma) 
                    tt = tt.Previous;
                if (tt is Pullenti.Ner.ReferentToken) 
                {
                    Pullenti.Ner.Geo.GeoReferent geo = tt.GetReferent() as Pullenti.Ner.Geo.GeoReferent;
                    if (geo != null && geo.IsCity && !geo.IsBigCity) 
                        return new OrgTypToken(t, t.Next.Next, "пансионат");
                }
            }
            if (ad == null) 
                ad = Pullenti.Ner.Geo.GeoAnalyzer.GetData(t);
            if (ad == null) 
                return null;
            if (ad != null && SpeedRegime && ((ad.OTRegime || ad.AllRegime))) 
            {
                GeoTokenData d = t.Tag as GeoTokenData;
                if (d != null) 
                    return d.OrgTyp;
                return null;
            }
            if (ad.OLevel > 2) 
                return null;
            ad.OLevel++;
            OrgTypToken res = _TryParse(t, afterTerr, 0);
            ad.OLevel--;
            return res;
        }
        static OrgTypToken _TryParse(Pullenti.Ner.Token t, bool afterTerr, int lev = 0)
        {
            if (t == null) 
                return null;
            if (t is Pullenti.Ner.TextToken) 
            {
                string term = (t as Pullenti.Ner.TextToken).Term;
                if (term == "СП") 
                {
                    if (!afterTerr && t.Chars.IsAllLower) 
                        return null;
                }
                if (term == "НП") 
                {
                    if (!afterTerr && t.Chars.IsAllLower) 
                        return null;
                }
                if (term == "АК") 
                {
                    if (t.Next != null && t.Next.IsChar('.')) 
                        return null;
                    if (!afterTerr && t.Chars.IsCapitalUpper) 
                        return null;
                }
                if ((t.IsValue("ОФИС", null) || term == "ФАД" || term == "АД") || t.IsValue("КОРПУС", null)) 
                    return null;
                if ((t.IsValue("ФЕДЕРАЦИЯ", null) || t.IsValue("СОЮЗ", null) || t.IsValue("ПРЕФЕКТУРА", null)) || t.IsValue("ОТДЕЛЕНИЕ", null)) 
                    return null;
                if (t.IsValue("РАДИО", null) || t.IsValue("АППАРАТ", null)) 
                    return null;
                if (t.IsValue("ГОРОДОК", null) && !MiscLocationHelper.IsUserParamAddress(t)) 
                    return null;
                if (t.IsValue2("СО", "СТ")) 
                    return null;
                if (t.IsValue("ПОЛЕ", null) && (t.Previous is Pullenti.Ner.TextToken)) 
                {
                    Pullenti.Ner.Core.NounPhraseToken npt = Pullenti.Ner.Core.NounPhraseHelper.TryParse(t.Previous, Pullenti.Ner.Core.NounPhraseParseAttr.No, 0, null);
                    if (npt != null && npt.EndToken == t) 
                        return null;
                }
                if (term == "АО") 
                {
                    int cou = 5;
                    for (Pullenti.Ner.Token tt = t.Previous; tt != null && cou > 0; tt = tt.Previous,cou--) 
                    {
                        Pullenti.Ner.Core.IntOntologyToken ter = TerrItemToken.CheckOntoItem(tt);
                        if (ter != null) 
                        {
                            if (ter.Item != null && ter.Item.Referent.ToString().Contains("округ")) 
                                return null;
                        }
                    }
                }
                if (term.StartsWith("УЛ")) 
                {
                    Pullenti.Ner.Address.Internal.StreetItemToken sti = Pullenti.Ner.Address.Internal.StreetItemToken.TryParse(t, null, false, null);
                    if (sti != null && sti.Typ == Pullenti.Ner.Address.Internal.StreetItemType.Noun) 
                    {
                        OrgTypToken next = TryParse(sti.EndToken.Next, afterTerr, null);
                        if (next != null) 
                        {
                            if (next.Vals.Contains("ВЧ")) 
                            {
                                next = next.Clone();
                                next.BeginToken = t;
                                return next;
                            }
                        }
                    }
                }
            }
            Pullenti.Ner.Token t1 = null;
            List<string> typs = null;
            bool doubt = false;
            bool notorg = false;
            bool notgeo = false;
            bool canbesingle = false;
            Pullenti.Ner.MorphCollection morph = null;
            Pullenti.Ner.Core.TerminToken tok = m_Ontology.TryParse(t, Pullenti.Ner.Core.TerminParseAttr.No);
            if ((tok == null && afterTerr && (t is Pullenti.Ner.TextToken)) && (t as Pullenti.Ner.TextToken).Term == "СТ") 
                tok = new Pullenti.Ner.Core.TerminToken(t, t) { Termin = m_St };
            if (tok != null) 
            {
                string val = tok.Termin.CanonicText.ToLower();
                if (val == "гаражное товарищество" && (tok.LengthChar < 6)) 
                {
                    Pullenti.Ner.Token tt1 = t.Previous;
                    if (tt1 != null && tt1.IsChar('.')) 
                        tt1 = tt1.Previous;
                    if (tt1 != null && tt1.IsValue("П", null)) 
                        return null;
                }
                for (Pullenti.Ner.Token tt = tok.EndToken.Next; tt != null; tt = tt.Next) 
                {
                    if (!(tt is Pullenti.Ner.TextToken) || tt.WhitespacesBeforeCount > 2) 
                        break;
                    if ((tt.IsValue("ГРАЖДАНИН", null) || tt.IsValue("ЗАСТРОЙЩИК", null) || tt.IsValue("ГАРАЖ", null)) || tt.IsValue("СОБСТВЕННИК", null)) 
                    {
                        if (!tt.Chars.IsAllLower) 
                        {
                            if (tt.Next == null || tt.Next.IsComma) 
                                break;
                        }
                        tok.EndToken = tt;
                        val = string.Format("{0} {1}", val, (tt as Pullenti.Ner.TextToken).Term.ToLower());
                    }
                    else 
                        break;
                }
                t1 = tok.EndToken;
                typs = new List<string>();
                morph = tok.Morph;
                notorg = tok.Termin.Tag3 != null;
                notgeo = tok.Termin.Tag2 != null;
                if ((tok.Termin.Tag is Pullenti.Ner.Address.Internal.StreetItemType) && ((Pullenti.Ner.Address.Internal.StreetItemType)tok.Termin.Tag) == Pullenti.Ner.Address.Internal.StreetItemType.StdName) 
                    canbesingle = true;
                typs.Add(val);
                if (tok.Termin.Acronym != null) 
                    typs.Add(tok.Termin.Acronym);
                if (tok.EndToken == t) 
                {
                    if ((t.LengthChar < 4) && (t is Pullenti.Ner.TextToken) && Pullenti.Morph.LanguageHelper.EndsWith((t as Pullenti.Ner.TextToken).Term, "К")) 
                    {
                        Pullenti.Ner.Core.IntOntologyToken oi = TerrItemToken.CheckOntoItem(t.Next);
                        if (oi != null) 
                        {
                            if (t.Next.GetMorphClassInDictionary().IsAdjective && oi.BeginToken == oi.EndToken) 
                            {
                            }
                            else 
                                return null;
                        }
                        if ((!afterTerr && t.Chars.IsAllUpper && t.Next != null) && t.Next.Chars.IsAllUpper && t.Next.LengthChar > 1) 
                            return null;
                    }
                }
                if (tok.Termin.CanonicText == "МЕСТОРОЖДЕНИЕ" && (tok.EndToken.Next is Pullenti.Ner.TextToken) && tok.EndToken.Next.Chars.IsAllLower) 
                {
                    Pullenti.Ner.Core.NounPhraseToken npt = Pullenti.Ner.Core.NounPhraseHelper.TryParse(tok.EndToken.Next, Pullenti.Ner.Core.NounPhraseParseAttr.No, 0, null);
                    if (npt != null && npt.Chars.IsAllLower) 
                        tok.EndToken = npt.EndToken;
                }
                if (((t.LengthChar == 1 && t.Next != null && t.Next.IsChar('.')) && (t.Next.Next is Pullenti.Ner.TextToken) && t.Next.Next.LengthChar == 1) && t.Next.Next.Next == tok.EndToken && tok.EndToken.IsChar('.')) 
                {
                    bool ok2 = false;
                    if (canbesingle) 
                        ok2 = _checkPiter(t);
                    if (!ok2) 
                    {
                        if (t.Chars.IsAllUpper && t.Next.Next.Chars.IsAllUpper) 
                            return null;
                        if (tok.Termin.CanonicText == "ГАРАЖНОЕ ТОВАРИЩЕСТВО") 
                        {
                            if (!t.IsWhitespaceBefore && t.Previous != null && t.Previous.IsChar('.')) 
                                return null;
                        }
                    }
                }
            }
            else 
            {
                if (Pullenti.Ner.Address.Internal.StreetItemToken.CheckKeyword(t)) 
                    return null;
                Pullenti.Ner.ReferentToken rtok = t.Kit.ProcessReferent("ORGANIZATION", t, "MINTYPE");
                if (rtok != null) 
                {
                    if (t.IsValue("ДИВИЗИЯ", null) || t.IsValue("АРМИЯ", null) || t.IsValue("СЕКТОР", null)) 
                        return null;
                    if (rtok.EndToken == t && t.IsValue("ТК", null)) 
                    {
                        if (TerrItemToken.CheckOntoItem(t.Next) != null) 
                            return null;
                        if (t.Chars.IsAllUpper && t.Next != null && t.Next.Chars.IsAllUpper) 
                            return null;
                    }
                    if (rtok.BeginToken != rtok.EndToken) 
                    {
                        for (Pullenti.Ner.Token tt = rtok.BeginToken.Next; tt != null && tt.EndChar <= rtok.EndChar; tt = tt.Next) 
                        {
                            if (tt.LengthChar > 3) 
                                continue;
                            OrgTypToken next = TryParse(tt, afterTerr, null);
                            if (next != null && next.EndChar > rtok.EndChar) 
                                return null;
                        }
                    }
                    string prof = rtok.Referent.GetStringValue("PROFILE");
                    if (string.Compare(prof ?? "", "UNIT", true) == 0) 
                        doubt = true;
                    t1 = rtok.EndToken;
                    typs = rtok.Referent.GetStringValues("TYPE");
                    morph = rtok.Morph;
                    if (t.IsValue("БРИГАДА", null)) 
                        doubt = true;
                }
            }
            if (((t1 == null && (t is Pullenti.Ner.TextToken) && t.LengthChar >= 2) && t.LengthChar <= 4 && t.Chars.IsAllUpper) && t.Chars.IsCyrillicLetter) 
            {
                if (Pullenti.Ner.Address.Internal.AddressItemToken.TryParsePureItem(t, null, null) != null) 
                    return null;
                if (t.LengthChar == 2) 
                    return null;
                if (TerrItemToken.CheckOntoItem(t) != null) 
                    return null;
                typs = new List<string>();
                typs.Add((t as Pullenti.Ner.TextToken).Term);
                t1 = t;
                doubt = true;
            }
            if (t1 == null) 
                return null;
            if (morph == null) 
                morph = t1.Morph;
            OrgTypToken res = new OrgTypToken(t, t1) { IsDoubt = doubt, Vals = typs, Morph = morph, NotOrg = notorg, NotGeo = notgeo, CanBeSingle = canbesingle };
            if (t.IsValue("ОБЪЕДИНЕНИЕ", null)) 
                res.IsDoubt = true;
            else if ((t is Pullenti.Ner.TextToken) && (t as Pullenti.Ner.TextToken).Term == "СО") 
                res.IsDoubt = true;
            if (canbesingle) 
            {
                if (res.LengthChar < 6) 
                {
                    if (!_checkPiter(t)) 
                        return null;
                }
                return res;
            }
            if ((t == t1 && (t.LengthChar < 3) && t.Next != null) && t.Next.IsChar('.')) 
                res.EndToken = t1.Next;
            if ((lev < 2) && (res.WhitespacesAfterCount < 3)) 
            {
                OrgTypToken next = TryParse(res.EndToken.Next, afterTerr, null);
                if (next != null && next.Vals.Contains("участок")) 
                    next = null;
                if (next != null && !next.BeginToken.Chars.IsAllLower) 
                {
                    NameToken nam = NameToken.TryParse(next.EndToken.Next, GeoTokenType.Org, 0, false);
                    if (nam == null || next.WhitespacesAfterCount > 3) 
                        next = null;
                    else if ((nam.Number != null && nam.Name == null && next.LengthChar > 2) && next.IsDoubt) 
                        next = null;
                }
                if (next != null) 
                {
                    if (!next.IsDoubt) 
                        res.IsDoubt = false;
                    res.MergeWith(next);
                }
                else 
                {
                    t1 = res.EndToken.Next;
                    if (t1 != null && (t1.WhitespacesBeforeCount < 3)) 
                    {
                        if (t1.IsValue("СН", null)) 
                            res.EndToken = t1;
                    }
                }
            }
            t1 = res.EndToken;
            if ((t1.Next != null && t1.Next.IsAnd && t1.Next.Next != null) && ((t1.Next.Next.IsValue("ПОСТРОЙКА", null) || t1.Next.Next.IsValue("ХОЗПОСТРОЙКА", null)))) 
                res.EndToken = t1.Next.Next;
            return res;
        }
        static bool _checkPiter(Pullenti.Ner.Token t)
        {
            if (MiscLocationHelper.IsUserParamGarAddress(t)) 
                return true;
            int cou = 0;
            for (Pullenti.Ner.Token ttt = t.Previous; ttt != null && (cou < 20); ttt = ttt.Previous,cou++) 
            {
                if (ttt.IsValue("ПЕТЕРБУРГ", null) || ttt.IsValue("СПБ", null) || ttt.IsValue("ЛЕНИНГРАД", null)) 
                    return true;
            }
            for (Pullenti.Ner.Token ttt = t.Next; ttt != null && cou > 0; ttt = ttt.Next,cou--) 
            {
                if (ttt.IsValue("ПЕТЕРБУРГ", null) || ttt.IsValue("СПБ", null) || ttt.IsValue("ЛЕНИНГРАД", null)) 
                    return true;
            }
            return false;
        }
        public void MergeWith(OrgTypToken ty)
        {
            foreach (string v in ty.Vals) 
            {
                if (!Vals.Contains(v)) 
                    Vals.Add(v);
            }
            if (!ty.NotOrg) 
                NotOrg = false;
            EndToken = ty.EndToken;
        }
        public static List<Pullenti.Ner.Core.Termin> FindTerminByAcronym(string abbr)
        {
            Pullenti.Ner.Core.Termin te = new Pullenti.Ner.Core.Termin(abbr) { Acronym = abbr };
            return m_Ontology.FindTerminsByTermin(te);
        }
        static Pullenti.Ner.Core.Termin m_St;
        public static void Initialize()
        {
            m_Ontology = new Pullenti.Ner.Core.TerminCollection();
            Pullenti.Ner.Core.Termin t = new Pullenti.Ner.Core.Termin("САДОВОЕ ТОВАРИЩЕСТВО") { Acronym = "СТ" };
            t.AddVariant("САДОВОДЧЕСКОЕ ТОВАРИЩЕСТВО", false);
            t.Acronym = "СТ";
            t.AddAbridge("С/ТОВ");
            t.AddAbridge("ПК СТ");
            t.AddAbridge("САД.ТОВ.");
            t.AddAbridge("САДОВ.ТОВ.");
            t.AddAbridge("С/Т");
            t.AddVariant("ВЕДЕНИЕ ГРАЖДАНАМИ САДОВОДСТВА ИЛИ ОГОРОДНИЧЕСТВА ДЛЯ СОБСТВЕННЫХ НУЖД", false);
            m_St = t;
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ДАЧНОЕ ТОВАРИЩЕСТВО") { Acronym = "ДТ", AcronymCanBeLower = true };
            t.AddAbridge("Д/Т");
            t.AddAbridge("ДАЧ/Т");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ЖИЛИЩНОЕ ТОВАРИЩЕСТВО") { Acronym = "ЖТ", AcronymCanBeLower = true };
            t.AddAbridge("Ж/Т");
            t.AddAbridge("ЖИЛ/Т");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("САДОВЫЙ КООПЕРАТИВ") { Acronym = "СК", AcronymCanBeLower = true };
            t.AddVariant("САДОВОДЧЕСКИЙ КООПЕРАТИВ", false);
            t.AddAbridge("С/К");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПОТРЕБИТЕЛЬСКИЙ КООПЕРАТИВ") { Acronym = "ПК", AcronymCanBeLower = true };
            t.AddVariant("ПОТРЕБКООПЕРАТИВ", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("САДОВОЕ ОБЩЕСТВО") { Acronym = "СО", AcronymCanBeLower = true };
            t.AddVariant("САДОВОДЧЕСКОЕ ОБЩЕСТВО", false);
            t.AddVariant("САДОВОДСТВО", false);
            t.AddAbridge("С/О");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПОТРЕБИТЕЛЬСКОЕ САДОВОДЧЕСКОЕ ОБЩЕСТВО") { Acronym = "ПСО", AcronymCanBeLower = true };
            t.AddVariant("ПОТРЕБИТЕЛЬСКОЕ САДОВОЕ ОБЩЕСТВО", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("САДОВОЕ ТОВАРИЩЕСКОЕ ОБЩЕСТВО") { Acronym = "СТО", AcronymCanBeLower = true, AcronymCanBeSmart = true };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("САДОВОЕ ПОТРЕБИТЕЛЬСКОЕ ОБЩЕСТВО") { Acronym = "СПО", AcronymCanBeLower = true, AcronymCanBeSmart = true };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("САДОВОДЧЕСКОЕ ДАЧНОЕ ТОВАРИЩЕСТВО") { Acronym = "СДТ", AcronymCanBeSmart = true, AcronymCanBeLower = true };
            t.AddVariant("САДОВОЕ ДАЧНОЕ ТОВАРИЩЕСТВО", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ДАЧНОЕ НЕКОММЕРЧЕСКОЕ ОБЪЕДИНЕНИЕ") { Acronym = "ДНО", AcronymCanBeSmart = true, AcronymCanBeLower = true };
            t.AddVariant("ДАЧНОЕ НЕКОММЕРЧЕСКОЕ ОБЪЕДИНЕНИЕ ГРАЖДАН", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ДАЧНОЕ НЕКОММЕРЧЕСКОЕ ПАРТНЕРСТВО") { Acronym = "ДНП", AcronymCanBeSmart = true, AcronymCanBeLower = true };
            t.AddVariant("ДАЧНОЕ НЕКОММЕРЧЕСКОЕ ПАРТНЕРСТВО ГРАЖДАН", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ДАЧНОЕ НЕКОММЕРЧЕСКОЕ ТОВАРИЩЕСТВО") { Acronym = "ДНТ", AcronymCanBeSmart = true, AcronymCanBeLower = true };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ДАЧНЫЙ ПОТРЕБИТЕЛЬСКИЙ КООПЕРАТИВ");
            t.Acronym = "ДПК";
            t.AcronymCanBeLower = true;
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ДАЧНО СТРОИТЕЛЬНЫЙ КООПЕРАТИВ") { Acronym = "ДСК", AcronymCanBeSmart = true, AcronymCanBeLower = true };
            t.AddVariant("ДАЧНЫЙ СТРОИТЕЛЬНЫЙ КООПЕРАТИВ", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("СТРОИТЕЛЬНО ПРОИЗВОДСТВЕННЫЙ КООПЕРАТИВ") { Acronym = "СПК", AcronymCanBeSmart = true, AcronymCanBeLower = true };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ВОДНО МОТОРНЫЙ КООПЕРАТИВ") { Acronym = "ВМК", AcronymCanBeSmart = true, AcronymCanBeLower = true };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("САДОВОДЧЕСКОЕ НЕКОММЕРЧЕСКОЕ ТОВАРИЩЕСТВО") { Acronym = "СНТ", AcronymCanBeSmart = true, AcronymCanBeLower = true };
            t.AddVariant("САДОВОЕ НЕКОММЕРЧЕСКОЕ ТОВАРИЩЕСТВО", false);
            t.AddAbridge("САДОВ.НЕКОМ.ТОВ.");
            t.AddVariant("ТСНСТ", false);
            t.AddAbridge("САДОВОЕ НЕКОМ-Е ТОВАРИЩЕСТВО");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("САДОВОДЧЕСКОЕ НЕКОММЕРЧЕСКОЕ ОБЪЕДИНЕНИЕ") { Acronym = "СНО", AcronymCanBeSmart = true, AcronymCanBeLower = true };
            t.AddVariant("САДОВОЕ НЕКОММЕРЧЕСКОЕ ОБЪЕДИНЕНИЕ", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("САДОВОДЧЕСКОЕ НЕКОММЕРЧЕСКОЕ ПАРТНЕРСТВО") { Acronym = "СНП", AcronymCanBeSmart = true, AcronymCanBeLower = true };
            t.AddVariant("САДОВОЕ НЕКОММЕРЧЕСКОЕ ПАРТНЕРСТВО", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("САДОВОДЧЕСКИЙ НЕКОММЕРЧЕСКИЙ ПОТРЕБИТЕЛЬСКИЙ КООПЕРАТИВ") { Acronym = "СНПК", AcronymCanBeSmart = true, AcronymCanBeLower = true };
            t.AddVariant("САДОВЫЙ НЕКОММЕРЧЕСКИЙ ПОТРЕБИТЕЛЬСКИЙ КООПЕРАТИВ", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("САДОВОДЧЕСКОЕ НЕКОММЕРЧЕСКОЕ ТОВАРИЩЕСТВО") { Acronym = "СНТ", AcronymCanBeSmart = true, AcronymCanBeLower = true };
            t.AddVariant("САДОВОЕ НЕКОММЕРЧЕСКОЕ ТОВАРИЩЕСТВО", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("САДОВОДЧЕСКОЕ ОГОРОДНИЧЕСКОЕ ТОВАРИЩЕСТВО") { Acronym = "СОТ", AcronymCanBeSmart = true, AcronymCanBeLower = true };
            t.AddVariant("САДОВОЕ ОГОРОДНИЧЕСКОЕ ТОВАРИЩЕСТВО", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ДАЧНОЕ НЕКОММЕРЧЕСКОЕ ТОВАРИЩЕСТВО") { Acronym = "ДНТ", AcronymCanBeSmart = true, AcronymCanBeLower = true };
            t.AddVariant("ДАЧНО НЕКОММЕРЧЕСКОЕ ТОВАРИЩЕСТВО", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("НЕКОММЕРЧЕСКОЕ САДОВОДЧЕСКОЕ ТОВАРИЩЕСТВО") { Acronym = "НСТ", AcronymCanBeSmart = true, AcronymCanBeLower = true };
            t.AddVariant("НЕКОММЕРЧЕСКОЕ САДОВОЕ ТОВАРИЩЕСТВО", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ОБЪЕДИНЕННОЕ НЕКОММЕРЧЕСКОЕ САДОВОДЧЕСКОЕ ТОВАРИЩЕСТВО") { Acronym = "ОНСТ", AcronymCanBeSmart = true, AcronymCanBeLower = true };
            t.AddVariant("ОБЪЕДИНЕННОЕ НЕКОММЕРЧЕСКОЕ САДОВОЕ ТОВАРИЩЕСТВО", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("САДОВОДЧЕСКАЯ ПОТРЕБИТЕЛЬСКАЯ КООПЕРАЦИЯ") { Acronym = "СПК", AcronymCanBeSmart = true, AcronymCanBeLower = true };
            t.AddVariant("САДОВАЯ ПОТРЕБИТЕЛЬСКАЯ КООПЕРАЦИЯ", false);
            t.AddVariant("САДОВОДЧЕСКИЙ ПОТРЕБИТЕЛЬНЫЙ КООПЕРАТИВ", false);
            t.AddVariant("САДОВОДЧЕСКИЙ ПОТРЕБИТЕЛЬСКИЙ КООПЕРАТИВ", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ДАЧНО СТРОИТЕЛЬНЫЙ КООПЕРАТИВ") { Acronym = "ДСК", AcronymCanBeSmart = true, AcronymCanBeLower = true };
            t.AddVariant("ДАЧНЫЙ СТРОИТЕЛЬНЫЙ КООПЕРАТИВ", false);
            m_Ontology.Add(t);
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("ДАЧНО СТРОИТЕЛЬНО ПРОИЗВОДСТВЕННЫЙ КООПЕРАТИВ") { Acronym = "ДСПК", AcronymCanBeSmart = true, AcronymCanBeLower = true });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("ЖИЛИЩНЫЙ СТРОИТЕЛЬНО ПРОИЗВОДСТВЕННЫЙ КООПЕРАТИВ") { Acronym = "ЖСПК", AcronymCanBeSmart = true, AcronymCanBeLower = true });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("ЖИЛИЩНЫЙ СТРОИТЕЛЬНО ПРОИЗВОДСТВЕННЫЙ КООПЕРАТИВ ИНДИВИДУАЛЬНЫХ ЗАСТРОЙЩИКОВ") { Acronym = "ЖСПКИЗ", AcronymCanBeSmart = true, AcronymCanBeLower = true });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("ЖИЛИЩНЫЙ СТРОИТЕЛЬНЫЙ КООПЕРАТИВ") { Acronym = "ЖСК", AcronymCanBeSmart = true, AcronymCanBeLower = true });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("ЖИЛИЩНЫЙ СТРОИТЕЛЬНЫЙ КООПЕРАТИВ ИНДИВИДУАЛЬНЫХ ЗАСТРОЙЩИКОВ") { Acronym = "ЖСКИЗ", AcronymCanBeSmart = true, AcronymCanBeLower = true });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("ЖИЛИЩНОЕ СТРОИТЕЛЬНОЕ ТОВАРИЩЕСТВО") { Acronym = "ЖСТ", AcronymCanBeSmart = true, AcronymCanBeLower = true });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("ПОТРЕБИТЕЛЬСКОЕ ОБЩЕСТВО ИНДИВИДУАЛЬНЫХ ЗАСТРОЙЩИКОВ") { Acronym = "ПОТЗ", AcronymCanBeSmart = true, AcronymCanBeLower = true });
            t = new Pullenti.Ner.Core.Termin("ОГОРОДНИЧЕСКОЕ НЕКОММЕРЧЕСКОЕ ОБЪЕДИНЕНИЕ") { Acronym = "ОНО", AcronymCanBeSmart = true, AcronymCanBeLower = true };
            t.AddVariant("ОГОРОДНИЧЕСКОЕ ОБЪЕДИНЕНИЕ", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ОГОРОДНИЧЕСКОЕ НЕКОММЕРЧЕСКОЕ ПАРТНЕРСТВО") { Acronym = "ОНП", AcronymCanBeSmart = true, AcronymCanBeLower = true };
            t.AddVariant("ОГОРОДНИЧЕСКОЕ ПАРТНЕРСТВО", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ОГОРОДНИЧЕСКОЕ НЕКОММЕРЧЕСКОЕ ТОВАРИЩЕСТВО") { Acronym = "ОНТ", AcronymCanBeSmart = true, AcronymCanBeLower = true };
            t.AddVariant("ОГОРОДНИЧЕСКОЕ ТОВАРИЩЕСТВО", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ОГОРОДНИЧЕСКИЙ ПОТРЕБИТЕЛЬСКИЙ КООПЕРАТИВ") { Acronym = "ОПК", AcronymCanBeSmart = true, AcronymCanBeLower = true };
            t.AddVariant("ОГОРОДНИЧЕСКИЙ КООПЕРАТИВ", false);
            m_Ontology.Add(t);
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("ТОВАРИЩЕСТВО СОБСТВЕННИКОВ НЕДВИЖИМОСТИ") { Acronym = "СТСН", AcronymCanBeSmart = true, AcronymCanBeLower = true });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("САДОВОДЧЕСКОЕ ТОВАРИЩЕСТВО СОБСТВЕННИКОВ НЕДВИЖИМОСТИ") { Acronym = "ТСН", AcronymCanBeSmart = true, AcronymCanBeLower = true });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("САДОВОДЧЕСКОЕ ТОВАРИЩЕСТВО ЧАСТНЫХ ВЛАДЕЛЬЦЕВ") { Acronym = "СТЧВ", AcronymCanBeSmart = true, AcronymCanBeLower = true });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("ТОВАРИЩЕСТВО СОБСТВЕННИКОВ ЖИЛЬЯ") { Acronym = "ТСЖ", AcronymCanBeSmart = true, AcronymCanBeLower = true });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("ТОВАРИЩЕСТВО СОБСТВЕННИКОВ ЖИЛЬЯ КЛУБНОГО ПОСЕЛКА") { Acronym = "ТСЖКП", AcronymCanBeSmart = true, AcronymCanBeLower = true });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("САДОВЫЕ ЗЕМЕЛЬНЫЕ УЧАСТКИ") { Acronym = "СЗУ", AcronymCanBeSmart = true, AcronymCanBeLower = true });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("ТОВАРИЩЕСТВО ИНДИВИДУАЛЬНЫХ ЗАСТРОЙЩИКОВ") { Acronym = "ТИЗ", AcronymCanBeSmart = true, AcronymCanBeLower = true });
            t = new Pullenti.Ner.Core.Termin("КОЛЛЕКТИВ ИНДИВИДУАЛЬНЫХ ЗАСТРОЙЩИКОВ") { Acronym = "КИЗ", AcronymCanBeSmart = true, AcronymCanBeLower = true };
            t.AddVariant("КИЗК", false);
            m_Ontology.Add(t);
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("ОБЩЕСТВО ИНДИВИДУАЛЬНЫХ ЗАСТРОЙЩИКОВ ГАРАЖЕЙ") { Acronym = "ОИЗГ", AcronymCanBeSmart = true, AcronymCanBeLower = true });
            t = new Pullenti.Ner.Core.Termin("САДОВОЕ НЕКОММЕРЧЕСКОЕ ТОВАРИЩЕСТВО СОБСТВЕННИКОВ НЕДВИЖИМОСТИ") { Acronym = "СНТСН", AcronymCanBeSmart = true, AcronymCanBeLower = true };
            t.AddVariant("САДОВОДЧЕСКОЕ НЕКОММЕРЧЕСКОЕ ТОВАРИЩЕСТВО СОБСТВЕННИКОВ НЕДВИЖИМОСТИ", false);
            t.AddVariant("СНТ СН", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПОТРЕБИТЕЛЬСКОЕ ГАРАЖНО СТРОИТЕЛЬНОЕ ОБЩЕСТВО") { Acronym = "ПГСО", AcronymCanBeSmart = true, AcronymCanBeLower = true };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПОТРЕБИТЕЛЬСКОЕ КООПЕРАТИВНОЕ ОБЩЕСТВО ГАРАЖЕЙ") { Acronym = "ПКОГ", AcronymCanBeSmart = true, AcronymCanBeLower = true };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("НЕКОММЕРЧЕСКОЕ ПАРТНЕРСТВО ГАРАЖНЫЙ КООПЕРАТИВ") { Acronym = "НПГК", AcronymCanBeSmart = true, AcronymCanBeLower = true };
            t.AddAbridge("НП ГК");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("НЕКОММЕРЧЕСКОЕ ПАРТНЕРСТВО СОБСТВЕННИКОВ") { Acronym = "НПС", AcronymCanBeSmart = true, AcronymCanBeLower = true };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("СУБЪЕКТ МАЛОГО ПРЕДПРИНИМАТЕЛЬСТВА") { Acronym = "СМП", AcronymCanBeSmart = true, AcronymCanBeLower = true };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ЛИЧНОЕ ПОДСОБНОЕ ХОЗЯЙСТВО") { Acronym = "ЛПХ", AcronymCanBeSmart = true, AcronymCanBeLower = true };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ИНДИВИДУАЛЬНОЕ САДОВОДСТВО") { Tag3 = 1 };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("КОЛЛЕКТИВНЫЙ ГАРАЖ") { Tag3 = 1 };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("КОМПЛЕКС ГАРАЖЕЙ") { Tag3 = 1 };
            t.AddVariant("РАЙОН ГАРАЖЕЙ", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ГАРАЖНЫЙ МАССИВ") { Tag3 = 1 };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПЛОЩАДКА ГАРАЖЕЙ") { Tag3 = 1 };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("КОЛЛЕКТИВНЫЙ САД") { Tag3 = 1 };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("САД") { Tag3 = 1 };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("КОМПЛЕКС ЗДАНИЙ И СООРУЖЕНИЙ") { Tag3 = 1 };
            t.AddVariant("КОМПЛЕКС СТРОЕНИЙ И СООРУЖЕНИЙ", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ОБЪЕДИНЕНИЕ");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПРОМЫШЛЕННАЯ ПЛОЩАДКА") { Tag3 = 1 };
            t.AddVariant("ПРОМПЛОЩАДКА", false);
            t.AddAbridge("ПРОМ.ПЛОЩАДКА");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПРОИЗВОДСТВЕННАЯ ПЛОЩАДКА") { Tag3 = 1 };
            t.AddAbridge("ПРОИЗВ.ПЛОЩАДКА");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ИМУЩЕСТВЕННЫЙ КОМПЛЕКС") { Tag3 = 1 };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("СОВМЕСТНОЕ ПРЕДПРИЯТИЕ");
            t.Acronym = "СП";
            t.AcronymCanBeLower = true;
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("НЕКОММЕРЧЕСКОЕ ПАРТНЕРСТВО");
            t.Acronym = "НП";
            t.AcronymCanBeLower = true;
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("АВТОМОБИЛЬНЫЙ КООПЕРАТИВ");
            t.AddVariant("АВТОКООПЕРАТИВ", false);
            t.AddVariant("АВТО КООПЕРАТИВ", false);
            t.AddAbridge("А/К");
            t.Acronym = "АК";
            t.AcronymCanBeLower = true;
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ГАРАЖНЫЙ КООПЕРАТИВ");
            t.AddAbridge("Г/К");
            t.AddAbridge("ГР.КОП.");
            t.AddAbridge("ГАР.КОП.");
            t.AddAbridge("ГАР.КООП.");
            t.AddVariant("ГАРАЖНЫЙ КООП", false);
            t.AddVariant("ГАРАЖНЫЙ КВАРТАЛ", false);
            t.Acronym = "ГК";
            t.AcronymCanBeLower = true;
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("АВТОГАРАЖНЫЙ КООПЕРАТИВ");
            t.AddVariant("АВТО ГАРАЖНЫЙ КООПЕРАТИВ", false);
            t.Acronym = "АГК";
            t.AcronymCanBeLower = true;
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ГАРАЖНОЕ ТОВАРИЩЕСТВО");
            t.AddAbridge("Г/Т");
            t.AddAbridge("ГР.ТОВ.");
            t.AddAbridge("ГАР.ТОВ.");
            t.AddAbridge("ГАР.ТОВ-ВО");
            t.Acronym = "ГТ";
            t.AcronymCanBeLower = true;
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПЛОЩАДКА ГАРАЖЕЙ") { Tag3 = 1 };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ГАРАЖНЫЙ КОМПЛЕКС") { Tag3 = 1 };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПРОИЗВОДСТВЕННЫЙ СЕЛЬСКОХОЗЯЙСТВЕННЫЙ КООПЕРАТИВ");
            t.AddVariant("ПРОИЗВОДСТВЕННО СЕЛЬСКОХОЗЯЙСТВЕННЫЙ КООПЕРАТИВ", false);
            t.Acronym = "ПСК";
            t.AcronymCanBeLower = true;
            m_Ontology.Add(t);
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("ГАРАЖНО СТРОИТЕЛЬНЫЙ КООПЕРАТИВ") { Acronym = "ГСК", AcronymCanBeLower = true, AcronymCanBeSmart = true });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("ГАРАЖНО ЭКСПЛУАТАЦИОННЫЙ КООПЕРАТИВ") { Acronym = "ГЭК", AcronymCanBeLower = true, AcronymCanBeSmart = true });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("ГАРАЖНО ПОТРЕБИТЕЛЬСКИЙ КООПЕРАТИВ") { Acronym = "ГПК", AcronymCanBeLower = true, AcronymCanBeSmart = true });
            t = new Pullenti.Ner.Core.Termin("КООПЕРАТИВ ПО СТРОИТЕЛЬСТВУ И ЭКСПЛУАТАЦИИ ГАРАЖЕЙ") { Acronym = "КСЭГ", AcronymCanBeLower = true, AcronymCanBeSmart = true };
            t.AddVariant("КСИЭГ", false);
            t.AddVariant("КССГ", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("КООПЕРАТИВ ПО СТРОИТЕЛЬСТВУ ГАРАЖЕЙ") { Acronym = "КСГ", AcronymCanBeLower = true, AcronymCanBeSmart = true };
            t.AddVariant("КССГ", false);
            m_Ontology.Add(t);
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("ПОТРЕБИТЕЛЬСКИЙ ГАРАЖНО СТРОИТЕЛЬНЫЙ КООПЕРАТИВ") { Acronym = "ПГСК", AcronymCanBeLower = true, AcronymCanBeSmart = true });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("ПОТРЕБИТЕЛЬСКИЙ ГАРАЖНО ЭКСПЛУАТАЦИОННЫЙ КООПЕРАТИВ") { Acronym = "ПГЭК", AcronymCanBeLower = true, AcronymCanBeSmart = true });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("ОБЩЕСТВЕННО ПОТРЕБИТЕЛЬСКИЙ ГАРАЖНЫЙ КООПЕРАТИВ") { Acronym = "ОПГК", AcronymCanBeLower = true, AcronymCanBeSmart = true });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("ГАРАЖНЫЙ СТРОИТЕЛЬНО ПОТРЕБИТЕЛЬСКИЙ КООПЕРАТИВ") { Acronym = "ГСПК", AcronymCanBeLower = true, AcronymCanBeSmart = true });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("КООПЕРАТИВ ПО СТРОИТЕЛЬСТВУ И ЭКСПЛУАТАЦИИ ИНДИВИДУАЛЬНЫХ ГАРАЖЕЙ") { Acronym = "КСЭИГ", AcronymCanBeLower = true, AcronymCanBeSmart = true });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("ПОТРЕБИТЕЛЬСКИЙ ГАРАЖНЫЙ КООПЕРАТИВ") { Acronym = "ПГК", AcronymCanBeLower = true, AcronymCanBeSmart = true });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("ИНДИВИДУАЛЬНОЕ ЖИЛИЩНОЕ СТРОИТЕЛЬСТВО") { Acronym = "ИЖС", AcronymCanBeLower = true, AcronymCanBeSmart = true });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("ЖИВОТНОВОДЧЕСКАЯ ТОЧКА"));
            t = new Pullenti.Ner.Core.Termin("ДАЧНАЯ ЗАСТРОЙКА") { Acronym = "ДЗ", AcronymCanBeLower = true, Tag3 = 1 };
            t.AddVariant("КВАРТАЛ ДАЧНОЙ ЗАСТРОЙКИ", false);
            t.AddVariant("ЗОНА ДАЧНОЙ ЗАСТРОЙКИ", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("КОТТЕДЖНЫЙ ПОСЕЛОК") { Acronym = "КП", AcronymCanBeLower = true, Tag2 = 1, Tag3 = 1 };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ДАЧНЫЙ ПОСЕЛОК") { Acronym = "ДП", AcronymCanBeLower = true, Tag2 = 1, Tag3 = 1 };
            t.AddAbridge("Д/П");
            t.AddVariant("ДАЧНЫЙ ПОСЕЛОК МАССИВ", false);
            t.AddVariant("ДП МАССИВ", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("САДОВОДЧЕСКИЙ МАССИВ") { Tag2 = 1, Tag3 = 1 };
            t.AddVariant("САД. МАССИВ", false);
            t.AddVariant("САДОВЫЙ МАССИВ", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("САНАТОРИЙ");
            t.AddAbridge("САН.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПАНСИОНАТ");
            t.AddAbridge("ПАНС.");
            t.AddVariant("ТУРИСТИЧЕСКИЙ ПАНСИОНАТ", false);
            t.AddAbridge("ТУР.ПАНСИОНАТ");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ДЕТСКИЙ ГОРОДОК") { Tag3 = 1 };
            t.AddAbridge("ДЕТ.ГОРОДОК");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ДОМ ОТДЫХА") { Acronym = "ДО" };
            t.AddAbridge("Д/О");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("БАЗА ОТДЫХА") { Acronym = "БО", AcronymCanBeLower = true };
            t.AddAbridge("Б/О");
            t.AddVariant("БАЗА ОТДЫХА РЫБАКА И ОХОТНИКА", false);
            t.AddVariant("БАЗА ОТДЫХА СЕМЕЙНОГО ТИПА", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ТУРИСТИЧЕСКАЯ БАЗА") { Acronym = "ТБ", AcronymCanBeLower = true };
            t.AddAbridge("Т/Б");
            t.AddVariant("ТУРБАЗА", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ФЕРМЕРСКОЕ ХОЗЯЙСТВО") { Acronym = "ФХ", AcronymCanBeLower = true, AcronymCanBeSmart = true };
            t.AddAbridge("Ф/Х");
            t.AddAbridge("ФЕРМЕРСКОЕ Х-ВО");
            t.AddAbridge("ФЕРМЕРСКОЕ ХОЗ-ВО");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("КРЕСТЬЯНСКОЕ ХОЗЯЙСТВО") { Acronym = "КХ", AcronymCanBeLower = true, AcronymCanBeSmart = true };
            t.AddAbridge("К/Х");
            t.AddAbridge("КРЕСТЬЯНСКОЕ Х-ВО");
            t.AddAbridge("КРЕСТЬЯНСКОЕ ХОЗ-ВО");
            t.AddAbridge("КР.Х-ВО");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("КРЕСТЬЯНСКОЕ ФЕРМЕРСКОЕ ХОЗЯЙСТВО") { Acronym = "КФХ", AcronymCanBeLower = true, AcronymCanBeSmart = true };
            t.AddVariant("КРЕСТЬЯНСКОЕ (ФЕРМЕРСКОЕ) ХОЗЯЙСТВО", false);
            t.AddAbridge("К.Ф.Х.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("САД-ОГОРОД") { Tag3 = 1 };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ОВЦЕВОДЧЕСКАЯ ТОВАРНАЯ ФЕРМА") { Acronym = "ОТФ", AcronymCanBeLower = true, AcronymCanBeSmart = true };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("УЧЕБНОЕ ХОЗЯЙСТВО") { Acronym = "УХ", AcronymCanBeLower = true };
            t.AddAbridge("У/Х");
            t.AddVariant("УЧХОЗ", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ЗАВОД");
            t.AddVariant("ЗВД", false);
            t.AddAbridge("З-Д");
            t.AddAbridge("З-ДА");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("НЕФТЕПЕРЕРАБАТЫВАЮЩИЙ ЗАВОД") { Acronym = "НПЗ", AcronymCanBeLower = true, AcronymCanBeSmart = true };
            t.AddVariant("НЕФТЕ ПЕРЕРАБАТЫВАЮЩИЙ ЗАВОД", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ГАЗОПЕРЕРАБАТЫВАЮЩИЙ ЗАВОД") { Acronym = "ГПЗ", AcronymCanBeLower = true, AcronymCanBeSmart = true };
            t.AddVariant("ГАЗО ПЕРЕРАБАТЫВАЮЩИЙ ЗАВОД", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ФАБРИКА");
            t.AddVariant("Ф-КА", false);
            t.AddVariant("Ф-КИ", false);
            t.AddAbridge("ФАБР.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("СОВХОЗ");
            t.AddVariant("СВХ", false);
            t.AddAbridge("С-ЗА");
            t.AddAbridge("С/ЗА");
            t.AddAbridge("С/З");
            t.AddAbridge("СХ.");
            t.AddAbridge("С/Х");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("КОЛХОЗ");
            t.AddVariant("КЛХ", false);
            t.AddAbridge("К-ЗА");
            t.AddAbridge("К/ЗА");
            t.AddAbridge("К/З");
            t.AddAbridge("КХ.");
            t.AddAbridge("К/Х");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("РЫБНОЕ ХОЗЯЙСТВО");
            t.AddVariant("РЫБХОЗ", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ЖИВОТНОВОДЧЕСКИЙ КОМПЛЕКС");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ЖИВОТНОВОДЧЕСКАЯ СТОЯНКА");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ЖИВОТНОВОДЧЕСКОЕ ТОВАРИЩЕСТВО");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ХОЗЯЙСТВО");
            t.AddAbridge("ХОЗ-ВО");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("СЕЛЬСКОХОЗЯЙСТВЕННАЯ ЗЕМЛЯ") { Tag3 = 1 };
            t.AddVariant("СЕЛЬХОЗ ЗЕМЛЯ", false);
            t.AddAbridge("С/Х ЗЕМЛЯ");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПИОНЕРСКИЙ ЛАГЕРЬ");
            t.AddAbridge("П/Л");
            t.AddAbridge("П.Л.");
            t.AddAbridge("ПИОНЕР.ЛАГ.");
            t.AddVariant("ПИОНЕРЛАГЕРЬ", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("СПОРТИВНЫЙ ЛАГЕРЬ");
            t.AddVariant("СПОРТЛАГЕРЬ", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ОЗДОРОВИТЕЛЬНЫЙ ЛАГЕРЬ") { Acronym = "ОЛ", AcronymCanBeLower = true };
            t.AddAbridge("О/Л");
            t.AddAbridge("О.Л.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ОЗДОРОВИТЕЛЬНЫЙ КОМПЛЕКС") { Acronym = "ОК", AcronymCanBeLower = true };
            t.AddAbridge("О/К");
            t.AddAbridge("О.К.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("СПОРТИВНО ОЗДОРОВИТЕЛЬНЫЙ ЛАГЕРЬ");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("СПОРТИВНО ОЗДОРОВИТЕЛЬНАЯ БАЗА");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("КУРОРТ");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("КОЛЛЕКТИВ ИНДИВИДУАЛЬНЫХ ВЛАДЕЛЬЦЕВ") { Acronym = "КИВ", AcronymCanBeLower = true, AcronymCanBeSmart = true };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПОДСОБНОЕ ХОЗЯЙСТВО");
            t.AddAbridge("ПОДСОБНОЕ Х-ВО");
            t.AddAbridge("ПОДСОБНОЕ ХОЗ-ВО");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("БИЗНЕС ЦЕНТР") { Acronym = "БЦ", AcronymCanBeLower = true, AcronymCanBeSmart = true };
            t.AddVariant("БІЗНЕС ЦЕНТР", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ТОРГОВЫЙ ЦЕНТР") { Acronym = "ТЦ", AcronymCanBeLower = true, AcronymCanBeSmart = true };
            t.AddVariant("ТОРГОВИЙ ЦЕНТР", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ТОРГОВО ОФИСНЫЙ ЦЕНТР") { Acronym = "ТОЦ", AcronymCanBeLower = true, AcronymCanBeSmart = true };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ТОРГОВО ОФИСНЫЙ КОМПЛЕКС") { Acronym = "ТОК", AcronymCanBeLower = true, AcronymCanBeSmart = true };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ТОРГОВО РАЗВЛЕКАТЕЛЬНЫЙ ЦЕНТР") { Acronym = "ТРЦ", AcronymCanBeLower = true, AcronymCanBeSmart = true };
            t.AddVariant("ТОРГОВО РОЗВАЖАЛЬНИЙ ЦЕНТР", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ТОРГОВО РАЗВЛЕКАТЕЛЬНЫЙ КОМПЛЕКС") { Acronym = "ТРК", AcronymCanBeLower = true, AcronymCanBeSmart = true };
            t.AddVariant("ТОРГОВО РОЗВАЖАЛЬНИЙ КОМПЛЕКС", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("АЭРОПОРТ") { Tag3 = 1 };
            t.AddAbridge("А/П");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("АЭРОДРОМ") { Tag3 = 1 };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ГИДРОУЗЕЛ") { Tag3 = 1 };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ВОДОЗАБОР") { Tag3 = 1 };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ВОДОХРАНИЛИЩЕ") { Tag3 = 1 };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("МОРСКОЙ ПОРТ") { Tag3 = 1 };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("РЕЧНОЙ ПОРТ") { Tag3 = 1 };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("СКЛАД") { Tag3 = 1 };
            t.AddVariant("ЦЕНТРАЛЬНЫЙ СКЛАД", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПОЛЕ") { Tag3 = 1 };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПОЛЕВОЙ СТАН") { Tag3 = 1 };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ЧАБАНСКАЯ СТОЯНКА") { Tag3 = 1 };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ЛИЦЕНЗИОННЫЙ УЧАСТОК") { Tag3 = 1 };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("УРОЧИЩЕ") { Tag3 = 1 };
            t.AddAbridge("УР-ЩЕ");
            t.AddAbridge("УР.");
            t.AddAbridge("УРОЧ.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПАДЬ") { Tag3 = 1 };
            t.AddVariant("ПЯДЬ", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПАРК") { Tag3 = 1 };
            t.AddVariant("ПРИРОДНЫЙ ПАРК", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПАРК КУЛЬТУРЫ И ОТДЫХА") { Acronym = "ПКО", Tag3 = 1 };
            t.AddVariant("ПКИО", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ЗАИМКА") { Tag3 = 1 };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ОСТРОВ") { Tag3 = 1 };
            t.AddAbridge("О-В");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ИСТОРИЧЕСКИЙ РАЙОН") { Tag3 = 1 };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("КЛАДБИЩЕ") { Tag3 = 1 };
            t.AddAbridge("КЛ-ЩЕ");
            t.AddVariant("ГОРОДСКОЕ КЛАДБИЩЕ", false);
            t.AddVariant("ПРАВОСЛАВНОЕ КЛАДБИЩЕ", false);
            t.AddVariant("МУСУЛЬМАНСКОЕ КЛАДБИЩЕ", false);
            t.AddVariant("ВОИНСКОЕ КЛАДБИЩЕ", false);
            t.AddVariant("МЕМОРИАЛЬНОЕ КЛАДБИЩЕ", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("БАЗА");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПРОИЗВОДСТВЕННАЯ БАЗА");
            t.AddVariant("БАЗА ПРОИЗВОДСТВЕННОГО ОБЕСПЕЧЕНИЯ", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПРОМЫШЛЕННАЯ БАЗА");
            t.AddVariant("ПРОМБАЗА", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("СТРОИТЕЛЬНАЯ БАЗА");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("СТРОИТЕЛЬНО МОНТАЖНОЕ УПРАВЛЕНИЕ") { Acronym = "СМУ" };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ВОЙСКОВАЯ ЧАСТЬ") { Acronym = "ВЧ", AcronymCanBeLower = true };
            t.AddVariant("ВОИНСКАЯ ЧАСТЬ", false);
            t.AddAbridge("В/Ч");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПОЖАРНАЯ ЧАСТЬ") { Acronym = "ПЧ", AcronymCanBeLower = true };
            t.AddAbridge("ПОЖ. ЧАСТЬ");
            t.AddAbridge("П/Ч");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ВОЕННЫЙ ГОРОДОК") { Tag2 = 1, Tag3 = 1 };
            t.AddAbridge("В.ГОРОДОК");
            t.AddAbridge("В/Г");
            t.AddAbridge("В/ГОРОДОК");
            t.AddAbridge("В/ГОР");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("СТРОИТЕЛЬНОЕ УПРАВЛЕНИЕ") { Acronym = "СУ", AcronymCanBeLower = true };
            t.AddAbridge("С/У");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("МЕСТЕЧКО") { Tag3 = 1 };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ГОРОДОК") { Tag2 = 1, Tag3 = 1 };
            t.AddVariant("ВАГОН ГОРОДОК", false);
            m_Ontology.Add(t);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("МІСТЕЧКО") { Lang = Pullenti.Morph.MorphLang.UA, Tag2 = 1, Gender = Pullenti.Morph.MorphGender.Neuter, Tag3 = 1 };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("HILL") { Tag2 = 1, Tag3 = 1 };
            t.AddAbridge("HL.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("КВАРТИРНО ЭКСПЛУАТАЦИОННАЯ ЧАСТЬ") { Acronym = "КЭЧ", AcronymCanBeLower = true };
            m_Ontology.Add(t);
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("КАРЬЕР") { Tag3 = 1 });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("РУДНИК") { Tag3 = 1 });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("ПРИИСК") { Tag3 = 1 });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("РАЗРЕЗ") { Tag3 = 1 });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("ФАКТОРИЯ") { Tag3 = 1 });
            t = new Pullenti.Ner.Core.Termin("МЕСТОРОЖДЕНИЕ") { Tag3 = 1 };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ЛОКАЛЬНОЕ ПОДНЯТИЕ") { Tag3 = 1 };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("НЕФТЯНОЕ МЕСТОРОЖДЕНИЕ") { Tag3 = 1 };
            t.AddVariant("МЕСТОРОЖДЕНИЕ НЕФТИ", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ГАЗОВОЕ МЕСТОРОЖДЕНИЕ") { Acronym = "ГМ", AcronymCanBeLower = true, AcronymCanBeSmart = true, Tag3 = 1 };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("НЕФТЕГАЗОВОЕ МЕСТОРОЖДЕНИЕ") { Acronym = "НГМ", AcronymCanBeLower = true, AcronymCanBeSmart = true, Tag3 = 1 };
            t.AddVariant("НЕФТЯНОЕ ГАЗОВОЕ МЕСТОРОЖДЕНИЕ", false);
            t.AddVariant("ГАЗОНЕФТЯНОЕ МЕСТОРОЖДЕНИЕ", false);
            t.AddVariant("ГАЗОВО НЕФТЯНОЕ МЕСТОРОЖДЕНИЕ", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("НЕФТЕГАЗОКОНДЕНСАТНОЕ МЕСТОРОЖДЕНИЕ") { Acronym = "НГКМ", AcronymCanBeLower = true, AcronymCanBeSmart = true, Tag3 = 1 };
            t.AddVariant("НЕФТЕГАЗОВОЕ КОНДЕНСАТНОЕ МЕСТОРОЖДЕНИЕ", false);
            t.AddVariant("НЕФТЕГАЗОВОКОНДЕНСАТНОЕ МЕСТОРОЖДЕНИЕ", false);
            t.AddVariant("НЕФТЕГАЗКОНДЕНСАТНОЕ МЕСТОРОЖДЕНИЕ", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ГАЗОВОНЕФТЕКОНДЕНСАТНОЕ МЕСТОРОЖДЕНИЕ") { Acronym = "ГНКМ", AcronymCanBeLower = true, AcronymCanBeSmart = true, Tag3 = 1 };
            t.AddVariant("ГАЗОВО НЕФТЕ КОНДЕНСАТНОЕ МЕСТОРОЖДЕНИЕ", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ГАЗОКОНДЕНСАТНОЕ МЕСТОРОЖДЕНИЕ") { Acronym = "ГКМ", AcronymCanBeLower = true, AcronymCanBeSmart = true, Tag3 = 1 };
            t.AddVariant("ГАЗОВОЕ КОНДЕНСАТНОЕ МЕСТОРОЖДЕНИЕ", false);
            t.AddVariant("ГАЗОВОКОНДЕНСАТНОЕ МЕСТОРОЖДЕНИЕ", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("НЕФТЕПЕРЕКАЧИВАЮЩАЯ СТАНЦИЯ") { Tag3 = 1 };
            m_Ontology.Add(t);
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("ЛЕСНОЙ ТЕРМИНАЛ") { Tag3 = 1 });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("МОЛОЧНЫЙ КОМПЛЕКС") { Tag3 = 1 });
            t = new Pullenti.Ner.Core.Termin("МЕСТОРОЖДЕНИЕ") { Tag3 = 1 };
            t.AddAbridge("МЕСТОРОЖД.");
            t.AddVariant("МЕСТОРОЖДЕНИЕ ЗОЛОТА", false);
            t.AddVariant("МЕСТОРОЖДЕНИЕ РОССЫПНОГО ЗОЛОТА", false);
            t.AddVariant("ЗОЛОТОЕ МЕСТОРОЖДЕНИЕ", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("МЕСТНОСТЬ") { Tag = Pullenti.Ner.Address.Internal.StreetItemType.Noun, Gender = Pullenti.Morph.MorphGender.Feminie, Tag3 = 1 };
            t.AddAbridge("МЕСТН.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ЛЕСНИЧЕСТВО") { Tag3 = 1 };
            t.AddAbridge("ЛЕС-ВО");
            t.AddAbridge("ЛЕСН-ВО");
            t.AddVariant("УЧАСТКОВОЕ ЛЕСНИЧЕСТВО", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ЛЕСОПАРК") { Tag3 = 1 };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ЛЕСОУЧАСТОК") { Tag3 = 1 };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ЗАПОВЕДНИК") { Tag3 = 1 };
            t.AddAbridge("ЗАП-К");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ЦЕНТРАЛЬНАЯ УСАДЬБА") { Tag3 = 1 };
            t.AddVariant("УСАДЬБА", false);
            t.AddAbridge("ЦЕНТР.УС.");
            t.AddAbridge("ЦЕНТР.УСАДЬБА");
            t.AddAbridge("Ц/У");
            t.AddAbridge("УС-БА");
            t.AddAbridge("ЦЕНТР.УС-БА");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ЦЕНТРАЛЬНОЕ ОТДЕЛЕНИЕ");
            t.AddAbridge("Ц/О");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("СЕКТОР") { Tag3 = 1 };
            t.AddAbridge("СЕК.");
            t.AddAbridge("СЕКТ.");
            t.AddAbridge("С-Р");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("МАССИВ") { Tag = Pullenti.Ner.Address.Internal.StreetItemType.Noun, Gender = Pullenti.Morph.MorphGender.Masculine, Tag3 = 1 };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ЗОНА") { Tag = Pullenti.Ner.Address.Internal.StreetItemType.Noun, Gender = Pullenti.Morph.MorphGender.Feminie, Tag3 = 1 };
            t.AddVariant("ЗОНА (МАССИВ)", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ЗОНА ГАРАЖЕЙ") { Tag = Pullenti.Ner.Address.Internal.StreetItemType.Noun, Gender = Pullenti.Morph.MorphGender.Feminie, Tag3 = 1 };
            t.AddVariant("ЗОНА (МАССИВ) ГАРАЖЕЙ", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ГАРАЖНАЯ ПЛОЩАДКА") { Tag3 = 1 };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПОЛЕВОЙ МАССИВ") { Tag3 = 1 };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПОЛЕВОЙ УЧАСТОК") { Tag3 = 1 };
            t.AddAbridge("ПОЛЕВОЙ УЧ-К");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПРОМЗОНА") { Tag = Pullenti.Ner.Address.Internal.StreetItemType.Noun, Gender = Pullenti.Morph.MorphGender.Feminie, Tag3 = 1 };
            t.AddVariant("ПРОМЫШЛЕННАЯ ЗОНА", false);
            t.AddVariant("ПРОИЗВОДСТВЕННАЯ ЗОНА", false);
            t.AddVariant("ПРОМЫШЛЕННО КОММУНАЛЬНАЯ ЗОНА", false);
            t.AddVariant("ЗОНА ПРОИЗВОДСТВЕННОГО ИСПОЛЬЗОВАНИЯ", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПРОМУЗЕЛ") { Tag = Pullenti.Ner.Address.Internal.StreetItemType.Noun, Gender = Pullenti.Morph.MorphGender.Masculine, Tag3 = 1 };
            t.AddVariant("ПРОМЫШЛЕННЫЙ УЗЕЛ", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПРОМЫШЛЕННЫЙ РАЙОН") { Tag = Pullenti.Ner.Address.Internal.StreetItemType.Noun, Tag2 = 1, Gender = Pullenti.Morph.MorphGender.Feminie, Tag3 = 1 };
            t.AddVariant("ПРОМРАЙОН", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПЛАНИРОВОЧНЫЙ РАЙОН") { Tag = Pullenti.Ner.Address.Internal.StreetItemType.Noun, Tag2 = 1, Gender = Pullenti.Morph.MorphGender.Feminie, Tag3 = 1 };
            t.AddAbridge("П/Р");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПРОИЗВОДСТВЕННО АДМИНИСТРАТИВНАЯ ЗОНА") { Tag = Pullenti.Ner.Address.Internal.StreetItemType.Noun, Tag2 = 1, Gender = Pullenti.Morph.MorphGender.Feminie, Tag3 = 1 };
            t.AddAbridge("ПРОИЗВ. АДМ. ЗОНА");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ЖИЛАЯ ЗОНА") { Tag = Pullenti.Ner.Address.Internal.StreetItemType.Noun, Tag2 = 1, Gender = Pullenti.Morph.MorphGender.Feminie, Tag3 = 1 };
            t.AddVariant("ЖИЛЗОНА", false);
            t.AddVariant("ЖИЛ.ЗОНА", false);
            t.AddVariant("Ж.ЗОНА", false);
            t.AddAbridge("Ж/З");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ОСОБАЯ ЭКОНОМИЧЕСКАЯ ЗОНА") { Acronym = "ОЭЗ", AcronymCanBeSmart = true, AcronymCanBeLower = true, Tag2 = 1, Tag3 = 1 };
            t.AddVariant("ОСОБАЯ ЭКОНОМИЧЕСКАЯ ЗОНА ПРОМЫШЛЕННО ПРОИЗВОДСТВЕННОГО ТИПА", false);
            t.AddVariant("ОЭЗ ППТ", false);
            t.AddVariant("ОЭЖЗППТ", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ЗОНА ОТДЫХА") { Tag = Pullenti.Ner.Address.Internal.StreetItemType.Noun, Tag2 = 1, Gender = Pullenti.Morph.MorphGender.Feminie, Tag3 = 1 };
            t.AddAbridge("З/О");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("КОММУНАЛЬНАЯ ЗОНА") { Tag = Pullenti.Ner.Address.Internal.StreetItemType.Noun, Tag2 = 1, Gender = Pullenti.Morph.MorphGender.Feminie, Tag3 = 1 };
            t.AddVariant("КОМЗОНА", false);
            t.AddAbridge("КОММУН. ЗОНА");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ЖИЛОЙ МАССИВ") { Tag = Pullenti.Ner.Address.Internal.StreetItemType.Noun, Tag2 = 1, Gender = Pullenti.Morph.MorphGender.Masculine, Tag3 = 1 };
            t.AddAbridge("Ж.М.");
            t.AddAbridge("Ж/М");
            t.AddVariant("ЖИЛМАССИВ", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ЖИЛГОРОДОК") { Tag2 = 1, Tag3 = 1 };
            t.AddVariant("ЖИЛИЩНЫЙ ГОРОДОК", false);
            t.AddVariant("ЖИЛОЙ ГОРОДОК", false);
            t.AddAbridge("Ж/Г");
            t.AddAbridge("ЖИЛ.ГОР.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ЖИЛРАЙОН") { Tag3 = 1 };
            t.AddVariant("ЖИЛИЩНЫЙ РАЙОН", false);
            t.AddVariant("ЖИЛОЙ РАЙОН", false);
            t.AddAbridge("Ж/Р");
            t.AddAbridge("ЖИЛ.РАЙОН");
            t.AddAbridge("ЖИЛ.Р-Н");
            t.AddAbridge("Ж/Р-Н");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ЗАГОРОДНЫЙ КОМПЛЕКС") { Tag3 = 1 };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ИНДУСТРИАЛЬНЫЙ ПАРК") { Tag = Pullenti.Ner.Address.Internal.StreetItemType.Noun, Tag2 = 1, Gender = Pullenti.Morph.MorphGender.Masculine, Tag3 = 1 };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("КВАРТАЛ ДАЧНОЙ ЗАСТРОЙКИ") { CanonicText = "КВАРТАЛ", Tag = Pullenti.Ner.Address.Internal.StreetItemType.Noun, Tag2 = 0, Gender = Pullenti.Morph.MorphGender.Masculine, Tag3 = 1 };
            t.AddVariant("ПРОМЫШЛЕННЫЙ КВАРТАЛ", false);
            t.AddVariant("ИНДУСТРИАЛЬНЫЙ КВАРТАЛ", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ЖИЛОЙ КОМПЛЕКС") { Tag = Pullenti.Ner.Address.Internal.StreetItemType.Noun, Acronym = "ЖК", Tag2 = 0, Gender = Pullenti.Morph.MorphGender.Masculine, Tag3 = 1 };
            t.AddVariant("ЖИЛКОМПЛЕКС", false);
            t.AddAbridge("ЖИЛ.К.");
            t.AddAbridge("Ж/К");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ВАХТОВЫЙ ЖИЛОЙ КОМПЛЕКС") { Tag = Pullenti.Ner.Address.Internal.StreetItemType.Noun, Acronym = "ВЖК", Tag2 = 0, Gender = Pullenti.Morph.MorphGender.Masculine, Tag3 = 1 };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ВАСИЛЬЕВСКИЙ ОСТРОВ") { Tag = Pullenti.Ner.Address.Internal.StreetItemType.StdName, Acronym = "ВО", Tag3 = 1 };
            t.AddAbridge("В.О.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПЕТРОГРАДСКАЯ СТОРОНА") { Tag = Pullenti.Ner.Address.Internal.StreetItemType.StdName, Tag3 = 1 };
            t.AddAbridge("П.С.");
            m_Ontology.Add(t);
        }
        internal static Pullenti.Ner.Core.TerminCollection m_Ontology;
    }
}