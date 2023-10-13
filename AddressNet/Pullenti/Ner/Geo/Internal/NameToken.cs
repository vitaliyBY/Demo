/*
 * SDK Pullenti Address, version 4.20, august 2023. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Text;

namespace Pullenti.Ner.Geo.Internal
{
    class NameToken : Pullenti.Ner.MetaToken
    {
        public NameToken(Pullenti.Ner.Token b, Pullenti.Ner.Token e) : base(b, e, null)
        {
        }
        public string Name;
        public string Number;
        public string Pref;
        public string MiscTyp;
        public bool IsDoubt;
        public bool IsEponym;
        int m_lev;
        GeoTokenType m_typ;
        public override string ToString()
        {
            StringBuilder res = new StringBuilder();
            if (IsDoubt) 
                res.Append("? ");
            if (Pref != null) 
                res.AppendFormat("{0} ", Pref);
            if (Name != null) 
                res.AppendFormat("\"{0}\"", Name);
            if (Number != null) 
                res.AppendFormat(" N{0}", Number);
            return res.ToString();
        }
        public static NameToken TryParse(Pullenti.Ner.Token t, GeoTokenType ty, int lev, bool afterTyp = false)
        {
            if (t == null || lev > 3) 
                return null;
            Pullenti.Ner.Core.BracketSequenceToken br = Pullenti.Ner.Core.BracketHelper.TryParse(t, Pullenti.Ner.Core.BracketParseAttr.No, 100);
            if (br == null && Pullenti.Ner.Core.BracketHelper.IsBracket(t, true) && MiscLocationHelper.IsUserParamAddress(t)) 
            {
                for (Pullenti.Ner.Token tt = t.Next; tt != null; tt = tt.Next) 
                {
                    if (!Pullenti.Ner.Core.BracketHelper.IsBracket(tt, true)) 
                        continue;
                    if ((tt.EndChar - t.BeginChar) > 30) 
                        break;
                    if (Pullenti.Ner.Core.BracketHelper.TryParse(tt, Pullenti.Ner.Core.BracketParseAttr.No, 100) != null) 
                        break;
                    br = new Pullenti.Ner.Core.BracketSequenceToken(t, tt);
                    break;
                }
            }
            NameToken res = null;
            Pullenti.Ner.Token ttt;
            Pullenti.Ner.NumberToken num;
            Pullenti.Ner.Core.TerminToken ttok;
            if (br != null) 
            {
                if (!Pullenti.Ner.Core.BracketHelper.IsBracket(t, true)) 
                    return null;
                Pullenti.Ner.Address.Internal.AddressItemToken ait = Pullenti.Ner.Address.Internal.AddressItemToken.TryParsePureItem(t.Next, null, null);
                if (ait != null && ait.Typ != Pullenti.Ner.Address.Internal.AddressItemType.Number && ait.EndToken.Next == br.EndToken) 
                    return null;
                NameToken nam = TryParse(t.Next, ty, lev + 1, false);
                if (nam != null && nam.EndToken.Next == br.EndToken) 
                {
                    res = nam;
                    nam.BeginToken = t;
                    nam.EndToken = br.EndToken;
                    res.IsDoubt = false;
                }
                else 
                {
                    res = new NameToken(t, br.EndToken);
                    Pullenti.Ner.Token tt = br.EndToken.Previous;
                    if (tt is Pullenti.Ner.NumberToken) 
                    {
                        res.Number = (tt as Pullenti.Ner.NumberToken).Value;
                        tt = tt.Previous;
                        if (tt != null && tt.IsHiphen) 
                            tt = tt.Previous;
                    }
                    if (tt != null && tt.BeginChar > br.BeginChar) 
                        res.Name = Pullenti.Ner.Core.MiscHelper.GetTextValue(t.Next, tt, Pullenti.Ner.Core.GetTextAttr.No);
                }
            }
            else if ((t is Pullenti.Ner.ReferentToken) && (t as Pullenti.Ner.ReferentToken).BeginToken == (t as Pullenti.Ner.ReferentToken).EndToken && !(t as Pullenti.Ner.ReferentToken).BeginToken.Chars.IsAllLower) 
            {
                res = new NameToken(t, t) { IsDoubt = true };
                res.Name = Pullenti.Ner.Core.MiscHelper.GetTextValueOfMetaToken(t as Pullenti.Ner.ReferentToken, Pullenti.Ner.Core.GetTextAttr.No);
            }
            else if ((((ttt = Pullenti.Ner.Core.MiscHelper.CheckNumberPrefix(t)))) is Pullenti.Ner.NumberToken) 
            {
                res = new NameToken(t, ttt) { Number = (ttt as Pullenti.Ner.NumberToken).Value };
                if (ttt.WhitespacesAfterCount < 2) 
                {
                    Pullenti.Ner.Token ttt3 = ttt.Next;
                    if (ttt3 != null && ttt3.IsHiphen) 
                        ttt3 = ttt3.Next;
                    NameToken nam = TryParse(ttt3, ty, lev + 1, false);
                    if (nam != null && nam.Name != null && nam.Number == null) 
                    {
                        res.Name = nam.Name;
                        res.EndToken = nam.EndToken;
                    }
                }
            }
            else if ((((num = Pullenti.Ner.Core.NumberHelper.TryParseAge(t)))) != null) 
                res = new NameToken(t, num.EndToken) { Pref = (num as Pullenti.Ner.NumberToken).Value + " ЛЕТ" };
            else if ((((num = Pullenti.Ner.Core.NumberHelper.TryParseAnniversary(t)))) != null) 
                res = new NameToken(t, num.EndToken) { Pref = (num as Pullenti.Ner.NumberToken).Value + " ЛЕТ" };
            else if (t is Pullenti.Ner.NumberToken) 
            {
                Pullenti.Ner.Core.NumberExToken nn = Pullenti.Ner.Core.NumberHelper.TryParseNumberWithPostfix(t);
                if (nn != null && !MiscLocationHelper.IsUserParamAddress(t)) 
                {
                    if (nn.ExTyp != Pullenti.Ner.Core.NumberExType.Undefined) 
                        return null;
                }
                res = new NameToken(t, t) { Number = (t as Pullenti.Ner.NumberToken).Value };
                if ((t.WhitespacesAfterCount < 3) && afterTyp) 
                {
                    NameToken next = TryParse(t.Next, ty, lev + 1, afterTyp);
                    if (next != null && next.Number == null && next.Name != null) 
                    {
                        next.Number = res.Number;
                        next.BeginToken = res.BeginToken;
                        res = next;
                    }
                }
                if (t.Next != null && t.Next.IsHiphen && !t.Next.IsWhitespaceAfter) 
                {
                    NameToken next = TryParse(t.Next.Next, ty, lev + 1, afterTyp);
                    if (next != null && next.Number == null && next.Name != null) 
                    {
                        next.Number = res.Number;
                        next.BeginToken = res.BeginToken;
                        res = next;
                    }
                }
            }
            else if (t.IsHiphen && (t.Next is Pullenti.Ner.NumberToken)) 
            {
                num = Pullenti.Ner.Core.NumberHelper.TryParseAge(t.Next);
                if (num == null) 
                    num = Pullenti.Ner.Core.NumberHelper.TryParseAnniversary(t.Next);
                if (num != null) 
                    res = new NameToken(t, num.EndToken) { Pref = (num as Pullenti.Ner.NumberToken).Value + " ЛЕТ" };
                else 
                    res = new NameToken(t, t.Next) { Number = (t.Next as Pullenti.Ner.NumberToken).Value, IsDoubt = true };
            }
            else if ((t is Pullenti.Ner.ReferentToken) && t.GetReferent().TypeName == "DATE") 
            {
                string year = t.GetReferent().GetStringValue("YEAR");
                if (year != null) 
                    res = new NameToken(t, t) { Pref = year + " ГОДА" };
                else 
                {
                    string mon = t.GetReferent().GetStringValue("MONTH");
                    string day = t.GetReferent().GetStringValue("DAY");
                    if (day != null && mon == null && t.GetReferent().ParentReferent != null) 
                        mon = t.GetReferent().ParentReferent.GetStringValue("MONTH");
                    if (mon != null) 
                        res = new NameToken(t, t) { Name = t.GetReferent().ToString().ToUpper() };
                }
            }
            else if (!(t is Pullenti.Ner.TextToken)) 
                return null;
            else if (t.LengthChar == 1) 
            {
                if ((t.GetMorphClassInDictionary().IsPreposition && t.Chars.IsAllUpper && t.WhitespacesAfterCount > 0) && (t.WhitespacesAfterCount < 3) && (t.Next is Pullenti.Ner.TextToken)) 
                {
                    Pullenti.Ner.Core.NounPhraseToken npt = Pullenti.Ner.Core.NounPhraseHelper.TryParse(t, Pullenti.Ner.Core.NounPhraseParseAttr.ParsePreposition, 0, null);
                    if (npt != null && npt.EndToken != t) 
                        return new NameToken(t, npt.EndToken) { IsDoubt = true, Name = Pullenti.Ner.Core.MiscHelper.GetTextValue(t, npt.EndToken, Pullenti.Ner.Core.GetTextAttr.No) };
                }
                if ((t.Chars.IsAllUpper && t.Next != null && t.Next.IsHiphen) && (t.Next.Next is Pullenti.Ner.TextToken)) 
                    return new NameToken(t, t.Next.Next) { IsDoubt = true, Name = Pullenti.Ner.Core.MiscHelper.GetTextValue(t, t.Next.Next, Pullenti.Ner.Core.GetTextAttr.No) };
                bool ok = false;
                if (t.IsNewlineAfter || t.Next == null) 
                    ok = true;
                else if (t.Next.IsComma) 
                    ok = true;
                else if (t.Previous != null && t.Previous.IsValue("СЕКТОР", null)) 
                    ok = true;
                if (ty == GeoTokenType.Org && ok && t.Chars.IsLetter) 
                    return new NameToken(t, t) { Name = (t as Pullenti.Ner.TextToken).Term };
                if ((((ty != GeoTokenType.Org && ty != GeoTokenType.Strong)) || !t.Chars.IsAllUpper || !t.Chars.IsLetter) || t.IsWhitespaceAfter) 
                    return null;
                NameToken next = TryParse(t.Next, ty, lev + 1, false);
                if (next != null && next.Number != null && next.Name == null) 
                {
                    res = next;
                    res.BeginToken = t;
                    res.Name = (t as Pullenti.Ner.TextToken).Term;
                }
                else if (t.Next != null && t.Next.IsChar('.')) 
                {
                    StringBuilder nam = new StringBuilder();
                    nam.Append((t as Pullenti.Ner.TextToken).Term);
                    Pullenti.Ner.Token t1 = t.Next;
                    for (Pullenti.Ner.Token tt = t1.Next; tt != null; tt = tt.Next) 
                    {
                        if (!(tt is Pullenti.Ner.TextToken) || tt.LengthChar != 1 || !tt.Chars.IsLetter) 
                            break;
                        if (tt.Next == null || !tt.Next.IsChar('.')) 
                            break;
                        nam.Append((tt as Pullenti.Ner.TextToken).Term);
                        tt = tt.Next;
                        t1 = tt;
                    }
                    if (nam.Length >= 3) 
                        res = new NameToken(t, t1) { Name = nam.ToString() };
                    else 
                    {
                        Pullenti.Ner.ReferentToken rt = t.Kit.ProcessReferent("PERSON", t, null);
                        if (rt != null) 
                        {
                            res = new NameToken(t, rt.EndToken) { Name = rt.Referent.GetStringValue("LASTNAME") };
                            if (res.Name == null) 
                                res.Name = rt.Referent.ToStringEx(false, null, 0).ToUpper();
                            else 
                                for (Pullenti.Ner.Token tt = t; tt != null && tt.EndChar <= rt.EndChar; tt = tt.Next) 
                                {
                                    if ((tt is Pullenti.Ner.TextToken) && tt.IsValue(res.Name, null)) 
                                    {
                                        res.Name = (tt as Pullenti.Ner.TextToken).Term;
                                        break;
                                    }
                                }
                        }
                    }
                }
            }
            else if ((t as Pullenti.Ner.TextToken).Term == "ИМЕНИ" || (t as Pullenti.Ner.TextToken).Term == "ИМ") 
            {
                Pullenti.Ner.Token tt = t.Next;
                if (t.IsValue("ИМ", null) && tt != null && tt.IsChar('.')) 
                    tt = tt.Next;
                NameToken nam = TryParse(tt, GeoTokenType.Strong, lev + 1, false);
                if (nam != null) 
                {
                    nam.BeginToken = t;
                    nam.IsDoubt = false;
                    nam.IsEponym = true;
                    res = nam;
                }
            }
            else if ((((ttok = m_Onto.TryParse(t, Pullenti.Ner.Core.TerminParseAttr.No)))) != null) 
            {
                res = new NameToken(t, ttok.EndToken) { Name = ttok.Termin.CanonicText };
                Pullenti.Ner.Token tt = ttok.EndToken.Next;
                if (tt != null && tt.IsValue("СССР", null)) 
                    res.EndToken = tt;
            }
            else if (t.IsValue("ОТДЕЛЕНИЕ", null)) 
            {
                res = TryParse(t.Next, ty, lev + 1, afterTyp);
                if (res != null) 
                    res.BeginToken = t;
            }
            else 
            {
                if (afterTyp && ((t.Morph.Class.IsProperSurname || t.GetMorphClassInDictionary().IsProperName))) 
                {
                    Pullenti.Ner.ReferentToken rt = t.Kit.ProcessReferent("PERSON", t, null);
                    if (rt != null) 
                    {
                        res = new NameToken(t, rt.EndToken);
                        string sur = rt.Referent.GetStringValue("LASTNAME");
                        if (sur != null) 
                        {
                            for (Pullenti.Ner.Token tt = t; tt != null && tt.EndChar <= rt.EndChar; tt = tt.Next) 
                            {
                                if ((tt is Pullenti.Ner.TextToken) && tt.IsValue(sur, null)) 
                                {
                                    res.Name = (tt as Pullenti.Ner.TextToken).Term;
                                    break;
                                }
                            }
                        }
                        if (res.Name == null) 
                            res.Name = Pullenti.Ner.Core.MiscHelper.GetTextValueOfMetaToken(rt, Pullenti.Ner.Core.GetTextAttr.No);
                    }
                }
                if (res == null) 
                {
                    Pullenti.Ner.Core.NounPhraseToken npt = Pullenti.Ner.Core.NounPhraseHelper.TryParse(t, Pullenti.Ner.Core.NounPhraseParseAttr.No, 0, null);
                    if (npt != null && npt.EndToken.IsValue("КВАРТАЛ", null)) 
                    {
                        if (npt.BeginToken == npt.EndToken || npt.BeginToken.IsValue("КАДАСТРОВЫЙ", null)) 
                        {
                            NumToken num2 = NumToken.TryParse(npt.EndToken.Next, ty);
                            if (num2 != null && num2.IsCadasterNumber) 
                            {
                                res = new NameToken(t, num2.EndToken) { Number = num2.Value };
                                res.MiscTyp = "кадастровый квартал";
                                if ((res.WhitespacesAfterCount < 2) && !Pullenti.Ner.Core.BracketHelper.IsBracket(res.EndToken.Next, false)) 
                                {
                                    if (res.EndToken.Next.IsValue("ЛЕСНОЙ", null)) 
                                    {
                                        res.EndToken = res.EndToken.Next;
                                        res.Name = "ЛЕСНОЙ";
                                    }
                                }
                                return res;
                            }
                        }
                    }
                    if (npt != null && npt.BeginToken == npt.EndToken) 
                        npt = null;
                    if (npt != null) 
                    {
                        OrgTypToken ttt2 = OrgTypToken.TryParse(npt.EndToken, false, null);
                        if (ttt2 != null && ttt2.EndChar > npt.EndChar) 
                            npt = null;
                        else if (ttt2 != null && !afterTyp && !npt.Morph.Case.IsGenitive) 
                            npt = null;
                        else if (npt.Adjectives.Count > 1 && OrgTypToken.TryParse(npt.EndToken.Previous, false, null) != null) 
                            npt = null;
                    }
                    if (npt != null && npt.EndToken.Chars.IsAllLower) 
                    {
                        if (t.Chars.IsAllLower) 
                            npt = null;
                        else if (Pullenti.Ner.Address.Internal.StreetItemToken.CheckKeyword(npt.EndToken)) 
                        {
                            if (npt.Morph.Number == Pullenti.Morph.MorphNumber.Plural) 
                            {
                            }
                            else if (npt.EndToken.IsValue("САД", null) || npt.EndToken.IsValue("ПАРК", null)) 
                            {
                            }
                            else 
                                npt = null;
                        }
                    }
                    if (npt != null) 
                        res = new NameToken(t, npt.EndToken) { Morph = npt.Morph, Name = Pullenti.Ner.Core.MiscHelper.GetTextValueOfMetaToken(npt, Pullenti.Ner.Core.GetTextAttr.No).Replace("-", " ") };
                    else if (!t.Chars.IsAllLower || t.IsValue("МЕСТНОСТЬ", null) || ((afterTyp && MiscLocationHelper.IsUserParamAddress(t)))) 
                    {
                        if (TerrItemToken.CheckKeyword(t) != null) 
                        {
                            if (t.Chars.IsCapitalUpper && afterTyp) 
                            {
                            }
                            else 
                                return null;
                        }
                        Pullenti.Ner.Address.Internal.AddressItemToken ait = Pullenti.Ner.Address.Internal.AddressItemToken.TryParsePureItem(t, null, null);
                        if ((ait != null && ait.Typ != Pullenti.Ner.Address.Internal.AddressItemType.Number && ait.Value != null) && ait.Value != "0") 
                            return null;
                        res = new NameToken(t, t) { Name = (t as Pullenti.Ner.TextToken).Term, Morph = t.Morph };
                        if (t.Chars.IsAllLower) 
                            res.IsDoubt = true;
                        if ((((Pullenti.Morph.LanguageHelper.EndsWith(res.Name, "ОВ") || Pullenti.Morph.LanguageHelper.EndsWith(res.Name, "ВО"))) && (t.Next is Pullenti.Ner.TextToken) && !t.Next.Chars.IsAllLower) && t.Next.LengthChar > 1 && !t.Next.GetMorphClassInDictionary().IsUndefined) 
                        {
                            if (Pullenti.Ner.Address.Internal.StreetItemToken.CheckKeyword(t.Next)) 
                            {
                            }
                            else if (OrgTypToken.TryParse(t.Next, false, null) != null) 
                            {
                            }
                            else 
                            {
                                res.EndToken = t.Next;
                                res.Name = string.Format("{0} {1}", res.Name, (t.Next as Pullenti.Ner.TextToken).Term);
                                res.Morph = t.Next.Morph;
                            }
                        }
                        if ((t.WhitespacesAfterCount < 2) && (t.Next is Pullenti.Ner.TextToken) && t.Next.Chars.IsLetter) 
                        {
                            bool ok = false;
                            if (MiscLocationHelper.CheckTerritory(t.Next) != null) 
                            {
                            }
                            else if (t.Next.LengthChar >= 3 && t.Next.GetMorphClassInDictionary().IsUndefined) 
                                ok = true;
                            else if (MiscLocationHelper.CheckNameLong(res) != null) 
                                ok = true;
                            else if (MiscLocationHelper.CheckTerritory(t.Next) != null) 
                            {
                            }
                            else if (Pullenti.Ner.Address.Internal.StreetItemToken.CheckKeyword(t.Next)) 
                            {
                            }
                            else 
                            {
                                bool ok1 = false;
                                if ((((t.Next.LengthChar < 4) || t.GetMorphClassInDictionary().IsUndefined)) && t.Next.Chars.Equals(t.Chars)) 
                                    ok1 = true;
                                else if (t.IsValue("МЕСТНОСТЬ", null) && !t.Next.Chars.IsAllLower) 
                                    ok = true;
                                else if (!t.Next.Chars.IsAllLower || !Pullenti.Ner.Address.Internal.AddressItemToken.CheckHouseAfter(t.Next, false, false)) 
                                {
                                    if (MiscLocationHelper.CheckTerritory(t.Next) == null) 
                                    {
                                        if (t.Next.IsNewlineAfter || t.Next.Next.IsComma || Pullenti.Ner.Address.Internal.AddressItemToken.CheckHouseAfter(t.Next.Next, false, false)) 
                                            ok = true;
                                    }
                                    if (!ok && t.Next.Next != null) 
                                    {
                                        OrgTypToken typ = OrgTypToken.TryParse(t.Next.Next, false, null);
                                        if (typ != null && typ.NotOrg) 
                                            ok = true;
                                        else if (t.Next.Next.IsValue("МАССИВ", null)) 
                                            ok = true;
                                    }
                                }
                                if (ok1) 
                                {
                                    NameToken next = TryParse(t.Next, ty, lev + 1, false);
                                    if (next == null || next.BeginToken == next.EndToken) 
                                        ok = true;
                                }
                            }
                            if (!ok && t.Next.GetMorphClassInDictionary().IsAdjective) 
                            {
                                Pullenti.Morph.MorphClass mc = t.GetMorphClassInDictionary();
                                if (mc.IsNoun || mc.IsProperGeo) 
                                {
                                    if (((t.Morph.Gender & t.Next.Morph.Gender)) != Pullenti.Morph.MorphGender.Undefined) 
                                    {
                                        Pullenti.Ner.Token tt = t.Next.Next;
                                        if (tt == null) 
                                            ok = true;
                                        else if (tt.IsComma || tt.IsNewlineAfter) 
                                            ok = true;
                                        else if (Pullenti.Ner.Address.Internal.AddressItemToken.CheckHouseAfter(tt, false, false)) 
                                            ok = true;
                                        else if (Pullenti.Ner.Address.Internal.AddressItemToken.CheckStreetAfter(tt, false)) 
                                            ok = true;
                                    }
                                }
                            }
                            if (ok) 
                            {
                                if (OrgTypToken.TryParse(t.Next, false, null) != null) 
                                    ok = false;
                            }
                            if (ok) 
                            {
                                Pullenti.Ner.Token tt = MiscLocationHelper.CheckNameLong(res);
                                if (tt == null) 
                                    tt = t.Next;
                                res.Name = string.Format("{0} {1}", res.Name, Pullenti.Ner.Core.MiscHelper.GetTextValue(res.EndToken.Next, tt, Pullenti.Ner.Core.GetTextAttr.No));
                                res.EndToken = tt;
                            }
                            else 
                            {
                                Pullenti.Ner.NumberToken lat = Pullenti.Ner.Core.NumberHelper.TryParseRoman(t.Next);
                                if (lat != null && lat.Typ == Pullenti.Ner.NumberSpellingType.Roman) 
                                {
                                    res.Number = lat.Value;
                                    res.EndToken = lat.EndToken;
                                }
                            }
                        }
                    }
                    if (res != null && res.EndToken.IsValue("УСАДЬБА", null) && (res.WhitespacesAfterCount < 2)) 
                    {
                        NameToken res1 = TryParse(res.EndToken.Next, ty, lev + 1, false);
                        if (res1 != null && res1.Name != null) 
                        {
                            res.EndToken = res1.EndToken;
                            res.Name = string.Format("{0} {1}", res.Name, res1.Name);
                        }
                    }
                }
            }
            if (res == null || res.WhitespacesAfterCount > 2) 
                return res;
            ttt = res.EndToken.Next;
            if (ttt != null && ttt.IsHiphen) 
            {
                num = Pullenti.Ner.Core.NumberHelper.TryParseAge(ttt.Next);
                if (num == null) 
                    num = Pullenti.Ner.Core.NumberHelper.TryParseAnniversary(ttt.Next);
                if (num != null) 
                {
                    res.Pref = num.Value + " ЛЕТ";
                    res.EndToken = num.EndToken;
                }
                else if ((ttt.Next is Pullenti.Ner.NumberToken) && res.Number == null) 
                {
                    res.Number = (ttt.Next as Pullenti.Ner.NumberToken).Value;
                    res.EndToken = ttt.Next;
                }
                else if (res.Number == null) 
                {
                    Pullenti.Ner.NumberToken nt = Pullenti.Ner.Core.NumberHelper.TryParseRoman(ttt.Next);
                    if (nt != null) 
                    {
                        res.Number = nt.Value;
                        res.EndToken = nt.EndToken;
                    }
                }
                if ((ttt == res.EndToken.Next && (ttt.Next is Pullenti.Ner.TextToken) && !ttt.IsWhitespaceAfter) && res.Name != null) 
                {
                    res.Name = string.Format("{0} {1}", res.Name, (ttt.Next as Pullenti.Ner.TextToken).Term);
                    res.EndToken = ttt.Next;
                }
            }
            else if ((((num = Pullenti.Ner.Core.NumberHelper.TryParseAge(ttt)))) != null) 
            {
                res.Pref = num.Value + " ЛЕТ";
                res.EndToken = num.EndToken;
            }
            else if ((((num = Pullenti.Ner.Core.NumberHelper.TryParseAnniversary(ttt)))) != null) 
            {
                res.Pref = num.Value + " ЛЕТ";
                res.EndToken = num.EndToken;
            }
            else if (ttt is Pullenti.Ner.NumberToken) 
            {
                bool ok = false;
                if (ty == GeoTokenType.Org && (ttt.WhitespacesBeforeCount < 2)) 
                    ok = true;
                if (ok) 
                {
                    if (Pullenti.Ner.Address.Internal.StreetItemToken.CheckKeyword(ttt.Next)) 
                        ok = false;
                    else if (ttt.Next != null) 
                    {
                        if (ttt.Next.IsValue("КМ", null) || ttt.Next.IsValue("КИЛОМЕТР", null)) 
                            ok = false;
                    }
                }
                if (ok) 
                {
                    res.Number = (ttt as Pullenti.Ner.NumberToken).Value;
                    res.EndToken = ttt;
                }
            }
            if (res.Number == null && res.EndToken.Next != null) 
            {
                NumToken nnn = NumToken.TryParse(res.EndToken.Next, ty);
                if (nnn != null && nnn.HasPrefix) 
                {
                    res.Number = nnn.Value;
                    res.EndToken = nnn.EndToken;
                }
                else if (nnn == null && res.EndToken.Next.IsComma && (res.EndToken.Next.WhitespacesAfterCount < 3)) 
                {
                    nnn = NumToken.TryParse(res.EndToken.Next.Next, ty);
                    if (nnn != null && nnn.HasSpecWord) 
                    {
                        res.Number = nnn.Value;
                        res.EndToken = nnn.EndToken;
                    }
                }
            }
            if ((res.WhitespacesAfterCount < 3) && res.Name == null && Pullenti.Ner.Core.BracketHelper.CanBeStartOfSequence(res.EndToken.Next, false, false)) 
            {
                NameToken nam = TryParse(res.EndToken.Next, ty, lev + 1, false);
                if (nam != null) 
                {
                    res.Name = nam.Name;
                    res.EndToken = nam.EndToken;
                    res.IsDoubt = false;
                }
            }
            if (res.Pref != null && res.Name == null && res.Number == null) 
            {
                NameToken nam = TryParse(res.EndToken.Next, ty, lev + 1, false);
                if (nam != null && nam.Name != null && nam.Pref == null) 
                {
                    res.Name = nam.Name;
                    res.Number = nam.Number;
                    res.EndToken = nam.EndToken;
                }
            }
            res.m_lev = lev;
            res.m_typ = ty;
            if (res.WhitespacesAfterCount < 3) 
            {
                Pullenti.Ner.Core.TerminToken nn = m_Onto.TryParse(res.EndToken.Next, Pullenti.Ner.Core.TerminParseAttr.No);
                if (nn != null) 
                {
                    res.EndToken = nn.EndToken;
                    res.Name = string.Format("{0} {1}", res.Name, Pullenti.Ner.Core.MiscHelper.GetTextValueOfMetaToken(nn, Pullenti.Ner.Core.GetTextAttr.No));
                }
            }
            if (res.Name != null && res.BeginToken == res.EndToken) 
            {
                Pullenti.Ner.Token end = MiscLocationHelper.CheckNameLong(res);
                if (end != null) 
                {
                    if (OrgTypToken.TryParse(res.EndToken.Next, false, null) == null) 
                    {
                        res.EndToken = end;
                        res.Name = string.Format("{0} {1}", res.Name, Pullenti.Ner.Core.MiscHelper.GetTextValue(res.BeginToken.Next, end, Pullenti.Ner.Core.GetTextAttr.No));
                    }
                }
            }
            res.TryAttachNumber();
            return res;
        }
        public static Pullenti.Ner.Token CheckInitial(Pullenti.Ner.Token t)
        {
            if (!(t is Pullenti.Ner.TextToken) || t.LengthChar > 2 || !t.Chars.IsLetter) 
                return null;
            string term = (t as Pullenti.Ner.TextToken).Term;
            Pullenti.Ner.Token t1 = t.Next;
            if (t1 != null && ((t1.IsCharOf(".,") || t1.IsHiphen))) 
                t1 = t1.Next;
            else if (t.Chars.IsAllLower) 
                return null;
            if (t1 == null) 
                return null;
            if (CheckInitialAndSurname(term, t1)) 
                return t1;
            return null;
        }
        public static bool CheckInitialBack(Pullenti.Ner.Token t)
        {
            if (!(t is Pullenti.Ner.TextToken) || t.WhitespacesBeforeCount > 2) 
                return false;
            if (t.LengthChar > 2 || !t.Chars.IsLetter) 
                return false;
            if (t.Next != null && t.Next.IsChar('.')) 
            {
            }
            else if (!t.Chars.IsAllUpper) 
                return false;
            return CheckInitialAndSurname((t as Pullenti.Ner.TextToken).Term, t.Previous);
        }
        public static bool CheckInitialAndSurname(string ini, Pullenti.Ner.Token sur)
        {
            if (sur == null || sur == null) 
                return false;
            if (ini == "А") 
            {
                if ((((((((sur.IsValue("МАТРОСОВ", null) || sur.IsValue("ПУШКИН", null) || sur.IsValue("УЛЬЯНОВ", null)) || sur.IsValue("СУВОРОВ", null) || sur.IsValue("АХМАТОВА", null)) || sur.IsValue("КАДЫРОВ", null) || sur.IsValue("АБУБАКАРОВ", null)) || sur.IsValue("АЛИША", null) || sur.IsValue("БЛОК", null)) || sur.IsValue("ГАЙДАР", null) || sur.IsValue("НЕВСКИЙ", null)) || sur.IsValue("НЕВСКИЙ", null) || sur.IsValue("СУЛТАН", null)) || sur.IsValue("ТОЛСТОЙ", null) || sur.IsValue("ШЕРИПОВ", null)) || sur.IsValue("ГРИН", null)) 
                    return true;
            }
            if (ini == "Б") 
            {
                if (sur.IsValue("ХМЕЛЬНИЦКИЙ", null)) 
                    return true;
            }
            if (ini == "В" || ini == "B") 
            {
                if (((((sur.IsValue("ЛЕНИН", null) || sur.IsValue("ТЕРЕШКОВА", null) || sur.IsValue("УЛЬЯНОВ", null)) || sur.IsValue("ВЫСОЦКИЙ", null) || sur.IsValue("ПАСТЕРНАК", null)) || sur.IsValue("ЧАПАЕВ", null) || sur.IsValue("ЧКАЛОВ", null)) || sur.IsValue("ЭМИРОВ", null) || sur.IsValue("ШУКШИН", null)) || sur.IsValue("МАЯКОВСКИЙ", null)) 
                    return true;
            }
            if (ini == "Г") 
            {
                if (((sur.IsValue("ЖУКОВ", null) || sur.IsValue("ИБРАГИМОВ", null) || sur.IsValue("ТУКАЙ", null)) || sur.IsValue("ЦАДАС", null) || sur.IsValue("ТИТОВ", null)) || sur.IsValue("УСПЕНСКИЙ", null) || sur.IsValue("ГАМИДОВ", null)) 
                    return true;
            }
            if (ini == "Д") 
            {
                if (sur.IsValue("УЛЬЯНОВ", null) || sur.IsValue("ДОНСКОЙ", null)) 
                    return true;
            }
            if (ini == "Е") 
            {
                if (sur.IsValue("ПУГАЧЕВ", null) || sur.IsValue("ЭМИН", null) || sur.IsValue("КОТИН", null)) 
                    return true;
            }
            if (ini == "З") 
            {
                if (sur.IsValue("КОСМОДЕМЬЯНСКАЯ", null)) 
                    return true;
            }
            if (ini == "И") 
            {
                if (((sur.IsValue("ФРАНКО", null) || sur.IsValue("ШАМИЛЬ", null) || sur.IsValue("АЙВАЗОВСКИЙ", null)) || sur.IsValue("ТУРГЕНЕВ", null) || sur.IsValue("АРМАНД", null)) || sur.IsValue("КАЗАК", null)) 
                    return true;
            }
            if (ini == "К") 
            {
                if (sur.IsValue("МАРКС", null) || sur.IsValue("ЛИБКНЕХТ", null) || sur.IsValue("ЦЕТКИН", null)) 
                    return true;
            }
            if (ini == "Л") 
            {
                if ((sur.IsValue("ТОЛСТОЙ", null) || sur.IsValue("ЧАЙКИНА", null) || sur.IsValue("ШЕВЦОВА", null)) || sur.IsValue("УКРАИНКА", null)) 
                    return true;
            }
            if (ini == "М" || ini == "M") 
            {
                if ((((((((((sur.IsValue("ГОРЬКИЙ", null) || sur.IsValue("АЛИЕВ", null) || sur.IsValue("БУЛГАКОВ", null)) || sur.IsValue("ДЖАЛИЛЬ", null) || sur.IsValue("КАРИМ", null)) || sur.IsValue("КУТУЗОВ", null) || sur.IsValue("ЛЕРМОНТОВ", null)) || sur.IsValue("ЦВЕТАЕВА", null) || sur.IsValue("ГАДЖИЕВ", null)) || sur.IsValue("ЯРАГСКИЙ", null) || sur.IsValue("ГАФУРИ", null)) || sur.IsValue("РАСКОВА", null) || sur.IsValue("УЛЬЯНОВА", null)) || sur.IsValue("ЛОМОНОСОВА", null) || sur.IsValue("ФРУНЗЕ", null)) || sur.IsValue("ШОЛОХОВА", null) || sur.IsValue("ТОРЕЗ", null)) || sur.IsValue("ЖУКОВ", null) || sur.IsValue("РОКОССОВСКИЙ", null)) || sur.IsValue("ВАСИЛЕВСКИЙ", null) || sur.IsValue("ТИМОШЕНКО", null)) 
                    return true;
            }
            if (ini == "Н") 
            {
                if ((sur.IsValue("ГОГОЛЬ", null) || sur.IsValue("КРУПСКАЯ", null) || sur.IsValue("ОСТРОВСКИЙ", null)) || sur.IsValue("САМУРСКИЙ", null)) 
                    return true;
            }
            if (ini == "О") 
            {
                if (sur.IsValue("КОШЕВОЙ", null) || sur.IsValue("ДУНДИЧ", null) || sur.IsValue("ШМИДТ", null)) 
                    return true;
            }
            if (ini == "П") 
            {
                if (sur is Pullenti.Ner.TextToken) 
                {
                    if ((sur as Pullenti.Ner.TextToken).Term == "САВЕЛЬЕВОЙ") 
                        return true;
                }
                if ((sur.IsValue("МОРОЗОВ", null) || sur.IsValue("КОРЧАГИН", null) || sur.IsValue("ОСИПЕНКО", null)) || sur.IsValue("ЛУМУМБА", null) || sur.IsValue("ГАМЗАТОВ", null)) 
                    return true;
            }
            if (ini == "Р") 
            {
                if (sur.IsValue("ЛЮКСЕМБУРГ", null) || sur.IsValue("КАДЫРОВ", null) || sur.IsValue("ЗОРГЕ", null)) 
                    return true;
            }
            if ((ini == "СТ" || ini == "CT" || ini == "С") || ini == "C") 
            {
                if (((((sur.IsValue("РАЗИН", null) || sur.IsValue("ХАЛТУРИН", null) || sur.IsValue("ЕСЕНИН", null)) || sur.IsValue("ЛАЗО", null) || sur.IsValue("КИРОВ", null)) || sur.IsValue("ОРДЖОНИКИДЗЕ", null) || sur.IsValue("ПЕТРОВСКАЯ", null)) || sur.IsValue("ЮЛАЕВ", null) || sur.IsValue("РАДОНЕЖСКИЙ", null)) || sur.IsValue("ПЕТРОВСКАЯ", null) || sur.IsValue("КОВАЛЕВСКАЯ", null)) 
                    return true;
            }
            if (ini == "Т") 
            {
                if (sur.IsValue("ШЕВЧЕНКО", null) || sur.IsValue("ХАХЛЫНОВА", null) || sur.IsValue("ФРУНЗЕ", null)) 
                    return true;
            }
            if (ini == "У") 
            {
                if (sur.IsValue("ГРОМОВА", null) || sur.IsValue("АЛИЕВ", null) || sur.IsValue("БУЙНАКСКИЙ", null)) 
                    return true;
            }
            if (ini == "Х" || ini == "X") 
            {
                if (sur.IsValue("АХМЕТОВ", null) || sur.IsValue("ТАКТАШ", null) || sur.IsValue("ДАВЛЕТШИНА", null)) 
                    return true;
            }
            if (ini == "Ч") 
            {
                if (sur.IsValue("АЙТМАТОВ", null)) 
                    return true;
            }
            if (ini == "Ш") 
            {
                if (sur.IsValue("УСМАНОВ", null) || sur.IsValue("БАБИЧ", null) || sur.IsValue("РУСТАВЕЛИ", null)) 
                    return true;
            }
            if (ini == "Ю") 
            {
                if (sur.IsValue("ГАГАРИН", null) || sur.IsValue("АКАЕВ", null) || sur.IsValue("ФУЧИК", null)) 
                    return true;
            }
            return false;
        }
        public void TryAttachNumber()
        {
            if (WhitespacesAfterCount > 1) 
                return;
            if (Number == null && EndToken.Next != null && m_lev == 0) 
            {
                Pullenti.Ner.Token tt = EndToken.Next;
                bool pref = false;
                if (tt.IsValue("БРИГАДА", null) || tt.IsValue("ОТДЕЛЕНИЕ", null) || tt.IsValue("ОЧЕРЕДЬ", null)) 
                    tt = tt.Next;
                else if (tt.IsValue("ОТД", null)) 
                {
                    tt = tt.Next;
                    if (tt != null && tt.IsChar('.')) 
                        tt = tt.Next;
                }
                NameToken nam2 = TryParse(tt, m_typ, m_lev + 1, false);
                if ((nam2 != null && nam2.Number != null && nam2.Name == null) && nam2.Pref == null) 
                {
                    if (tt == EndToken.Next && Pullenti.Ner.Address.Internal.StreetItemToken.CheckKeyword(nam2.EndToken.Next)) 
                    {
                    }
                    else 
                    {
                        Number = nam2.Number;
                        EndToken = nam2.EndToken;
                    }
                }
                else if (nam2 != null && nam2.IsEponym) 
                {
                    EndToken = nam2.EndToken;
                    if (Name == null) 
                        Name = nam2.Name;
                    else 
                        Name = string.Format("{0} {1}", Name, nam2.Name);
                    if (nam2.Number != null) 
                        Number = nam2.Number;
                }
            }
            if ((m_typ == GeoTokenType.Org && (EndToken is Pullenti.Ner.NumberToken) && Number == (EndToken as Pullenti.Ner.NumberToken).Value) && !IsWhitespaceAfter) 
            {
                StringBuilder tmp = new StringBuilder(Number);
                string delim = null;
                for (Pullenti.Ner.Token tt = EndToken.Next; tt != null; tt = tt.Next) 
                {
                    if (tt.IsWhitespaceBefore) 
                        break;
                    if (tt.IsCharOf(",.") || tt.IsTableControlChar) 
                        break;
                    if (tt.IsCharOf("\\/")) 
                    {
                        delim = "/";
                        continue;
                    }
                    else if (tt.IsHiphen) 
                    {
                        delim = "-";
                        continue;
                    }
                    if ((tt is Pullenti.Ner.NumberToken) && (tt as Pullenti.Ner.NumberToken).Typ == Pullenti.Ner.NumberSpellingType.Digit) 
                    {
                        if (delim != null) 
                            tmp.Append(delim);
                        delim = null;
                        tmp.Append((tt as Pullenti.Ner.NumberToken).Value);
                        EndToken = tt;
                        continue;
                    }
                    if ((tt is Pullenti.Ner.TextToken) && tt.LengthChar == 1 && tt.Chars.IsLetter) 
                    {
                        if (delim != null && char.IsLetter(tmp[tmp.Length - 1])) 
                            tmp.Append(delim);
                        delim = null;
                        tmp.Append((tt as Pullenti.Ner.TextToken).Term);
                        EndToken = tt;
                        continue;
                    }
                    break;
                }
                Number = tmp.ToString();
            }
            if ((m_typ == GeoTokenType.Org && (EndToken is Pullenti.Ner.NumberToken) && EndToken.Next != null) && Number == (EndToken as Pullenti.Ner.NumberToken).Value && (WhitespacesAfterCount < 3)) 
            {
                Pullenti.Ner.Token t1 = EndToken.Next;
                if (t1.IsValue("БРИГАДА", null) || t1.IsValue("ОЧЕРЕДЬ", null) || t1.IsValue("ОТДЕЛЕНИЕ", null)) 
                {
                    if (t1.Next is Pullenti.Ner.NumberToken) 
                        return;
                    if (Pullenti.Ner.Core.MiscHelper.CheckNumberPrefix(t1.Next) != null) 
                        return;
                    EndToken = t1;
                }
            }
            if (Number != null && (EndToken is Pullenti.Ner.NumberToken)) 
            {
                Pullenti.Ner.Token tt = EndToken.Next;
                if (((tt is Pullenti.Ner.TextToken) && tt.Chars.IsLetter && tt.LengthChar == 1) && (tt.WhitespacesBeforeCount < 3)) 
                {
                    bool ok = false;
                    if (!tt.IsWhitespaceBefore) 
                        ok = true;
                    else if (tt.IsNewlineAfter) 
                        ok = true;
                    else if (tt.Next.IsComma) 
                        ok = true;
                    if (ok) 
                    {
                        char ch = (tt as Pullenti.Ner.TextToken).Term[0];
                        char ch1 = Pullenti.Morph.LanguageHelper.GetCyrForLat(ch);
                        if (ch1 != 0) 
                            ch = ch1;
                        Number = string.Format("{0}{1}", Number, ch);
                        EndToken = tt;
                    }
                }
            }
        }
        public static Pullenti.Ner.Core.TerminCollection m_Onto;
        internal static string[] StandardNames = new string[] {"ЭНГЕЛЬСА;ФРИДРИХА ЭНГЕЛЬСА;ФРИД.ЭНГЕЛЬСА;ФР.ЭНГЕЛЬСА;Ф.ЭНГЕЛЬСА", "МАРКСА;КАРЛА МАРКСА;К.МАРКСА", "ЛИБКНЕХТА;КАРЛА ЛИБКНЕХТА;К.ЛИБКНЕХТА", "ЛЮКСЕМБУРГ;РОЗЫ ЛЮКСЕМБУРГ;Р.ЛЮКСЕМБУРГ", "УЧАСТНИКОВ ВОВ;УЧАСТНИКОВ ВЕЛИКОЙ ОТЕЧЕСТВЕННОЙ ВОЙНЫ", "ТРУД И ОТДЫХ", "СЪЕЗДА КПСС;ПАРТСЪЕЗДА КПСС", "ПОБЕДЫ;ВЕЛИКОЙ ПОБЕДЫ;ВЕЛ.ПОБЕДЫ;В.ПОБЕДЫ", "КРАСНОЙ АРМИИ;КР.АРМИИ", "СОВЕТСКОЙ АРМИИ;СОВ.АРМИИ;СА", "СОВЕТСКОЙ ВЛАСТИ;СОВ.ВЛАСТИ", "СА И ВМФ;СОВЕТСКОЙ АРМИИ И ВОЕННО МОРСКОГО ФЛОТА", "ВОЕННО МОРСКОЙ ФЛОТ;ВМФ", "МОЛОДАЯ ГВАРДИЯ", "ЗАЩИТНИКИ БЕЛОГО ДОМА", "ЗАРЯ ВОСТОКА", "ЗАРЯ КОММУНИЗМА", "ДРУЖБЫ НАРОДОВ", "ВЕТЕРАН ВС;ВЕТЕРАН ВООРУЖЕННЫХ СИЛ", "ВЕТЕРАН МО;ВЕТЕРАН МИНИСТЕРСТВА ОБОРОНЫ", "ГОРКИ ЛЕНИНСКИЕ", "ГОРОДОК ПИСАТЕЛЕЙ ПЕРЕДЕЛКИНО", "СВЕТЛЫЙ ПУТЬ ЛЕНИНА", "ЗАВЕТЫ ИЛЬИЧА", "СЕРП И МОЛОТ", "СОЦТРУДА;СОЦ.ТРУДА;СОЦИАЛИСТИЧЕСКОГО ТРУДА", "ПАРИЖСКОЙ КОММУНЫ;П.КОММУНЫ;ПАР.КОММУНЫ;ПАРИЖ.КОММУНЫ", "АЛМА-АТИНСКАЯ;А.АТИНСКАЯ;АЛМАТИНСКАЯ", "КИМ ИР СЕНА;КИМ ИРСЕНА", "ХО ШИ МИНА;ХОШИМИНА;ХО ШИМИНА", "ДРУЖБЫ НАРОДОВ;ДР.НАРОДОВ;ДРУЖ.НАРОДОВ", "КИРИЛЛА И МЕФОДИЯ;КИРИЛА И МЕФОДИЯ", "ПАМЯТИ И СЛАВЫ"};
        public static void Initialize()
        {
            m_Onto = new Pullenti.Ner.Core.TerminCollection();
            Pullenti.Ner.Core.Termin t = new Pullenti.Ner.Core.Termin("МИНИСТЕРСТВО ОБОРОНЫ") { Acronym = "МО" };
            m_Onto.Add(t);
            foreach (string s in StandardNames) 
            {
                string[] pp = s.Split(';');
                t = new Pullenti.Ner.Core.Termin(pp[0]) { IgnoreTermsOrder = true };
                for (int kk = 1; kk < pp.Length; kk++) 
                {
                    if (pp[kk].IndexOf('.') > 0) 
                        t.AddAbridge(pp[kk].Replace('.', ' '));
                    else if (t.Acronym == null && (pp[kk].Length < 4)) 
                        t.Acronym = pp[kk];
                    else 
                        t.AddVariant(pp[kk], false);
                }
                m_Onto.Add(t);
            }
            t = new Pullenti.Ner.Core.Termin("ПАРТСЪЕЗДА");
            t.AddAbridge("П/СЪЕЗДА");
            t.AddVariant("ПАРТИЙНОГО СЪЕЗДА", false);
            t.AddAbridge("ПАРТ.СЪЕЗДА");
            m_Onto.Add(t);
        }
    }
}