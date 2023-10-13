﻿/*
 * SDK Pullenti Address, version 4.20, august 2023. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Pullenti.Ner.Address.Internal
{
    public class StreetItemToken : Pullenti.Ner.MetaToken
    {
        public static List<StreetItemToken> TryParseList(Pullenti.Ner.Token t, int maxCount = 10, Pullenti.Ner.Geo.Internal.GeoAnalyzerData ad = null)
        {
            if (t == null) 
                return null;
            if (ad == null) 
                ad = Pullenti.Ner.Geo.GeoAnalyzer.GetData(t);
            if (ad == null) 
                return null;
            if (ad.SLevel > 2) 
                return null;
            ad.SLevel++;
            List<StreetItemToken> res = _tryParseList(t, maxCount, ad);
            ad.SLevel--;
            return res;
        }
        static List<StreetItemToken> _tryParseList(Pullenti.Ner.Token t, int maxCount, Pullenti.Ner.Geo.Internal.GeoAnalyzerData ad)
        {
            List<StreetItemToken> res = null;
            StreetItemToken sit = TryParse(t, null, false, ad);
            if (sit != null) 
            {
                res = new List<StreetItemToken>();
                res.Add(sit);
                t = sit.EndToken.Next;
            }
            else 
            {
                res = TryParseSpec(t, null);
                if (res == null) 
                    return null;
                sit = res[res.Count - 1];
                t = sit.EndToken.Next;
                StreetItemToken sit2 = TryParse(t, null, false, null);
                if (sit2 != null && sit2.Typ == StreetItemType.Noun) 
                {
                }
                else if (AddressItemToken.CheckHouseAfter(t, false, true)) 
                {
                }
                else 
                    return null;
            }
            for (; t != null; t = (t == null ? null : t.Next)) 
            {
                if (maxCount > 0 && res.Count >= maxCount) 
                    break;
                if (t.IsNewlineBefore) 
                {
                    if (t.NewlinesBeforeCount > 1) 
                        break;
                    if (((t.WhitespacesAfterCount < 15) && sit != null && sit.Typ == StreetItemType.Noun) && t.Chars.IsCapitalUpper) 
                    {
                    }
                    else 
                    {
                        bool ok = false;
                        if (res.Count == 1 && res[0].Typ == StreetItemType.Name) 
                        {
                            StreetItemToken sit1 = TryParse(sit.EndToken.Next, sit, false, ad);
                            if (sit1 != null && sit1.Typ == StreetItemType.Noun) 
                            {
                                StreetItemToken sit2 = TryParse(sit1.EndToken.Next, sit1, false, ad);
                                if (sit2 == null) 
                                    ok = true;
                            }
                        }
                        if (!ok) 
                            break;
                    }
                }
                if (t.IsHiphen && sit != null && ((sit.Typ == StreetItemType.Name || sit.Typ == StreetItemType.StdName || (sit.Typ == StreetItemType.StdAdjective)))) 
                {
                    StreetItemToken sit1 = TryParse(t.Next, sit, false, ad);
                    if (sit1 == null) 
                    {
                        Pullenti.Ner.NumberToken num = Pullenti.Ner.Core.NumberHelper.TryParseRoman(t.Next);
                        if (num != null) 
                        {
                            sit = new StreetItemToken(t, num.EndToken) { Typ = StreetItemType.Number, Value = num.Value, NumberHasPrefix = true };
                            res.Add(sit);
                            t = sit.EndToken;
                            continue;
                        }
                        break;
                    }
                    if (sit1.Typ == StreetItemType.Number) 
                    {
                        Pullenti.Ner.Token tt = sit1.EndToken.Next;
                        if (tt != null && tt.IsComma) 
                            tt = tt.Next;
                        bool ok = false;
                        AddressItemToken ait = AddressItemToken.TryParsePureItem(tt, null, null);
                        if (ait != null) 
                        {
                            if (ait.Typ == AddressItemType.House) 
                                ok = true;
                        }
                        if (!ok) 
                        {
                            if (res.Count == 2 && res[0].Typ == StreetItemType.Noun) 
                            {
                                if (res[0].Termin.CanonicText == "МИКРОРАЙОН") 
                                    ok = true;
                            }
                        }
                        if (!ok && t.IsHiphen) 
                            ok = true;
                        if (ok) 
                        {
                            sit = sit1;
                            res.Add(sit);
                            t = sit.EndToken;
                            sit.NumberHasPrefix = true;
                            continue;
                        }
                    }
                    if (sit1.Typ != StreetItemType.Name && sit1.Typ != StreetItemType.Name) 
                    {
                        if (sit1.Typ == StreetItemType.Noun && sit1.NounCanBeName) 
                        {
                        }
                        else 
                            break;
                    }
                    if (t.IsWhitespaceBefore && t.IsWhitespaceAfter) 
                        break;
                    if (res[0].BeginToken.Previous != null) 
                    {
                        AddressItemToken aaa = AddressItemToken.TryParsePureItem(res[0].BeginToken.Previous, null, null);
                        if (aaa != null && aaa.Typ == AddressItemType.Detail && aaa.DetailType == Pullenti.Ner.Address.AddressDetailType.Cross) 
                            break;
                    }
                    sit = sit1;
                    res.Add(sit);
                    t = sit.EndToken;
                    continue;
                }
                else if (t.IsHiphen && sit != null && sit.Typ == StreetItemType.Number) 
                {
                    StreetItemToken sit1 = TryParse(t.Next, null, false, ad);
                    if (sit1 != null && (((sit1.Typ == StreetItemType.StdAdjective || sit1.Typ == StreetItemType.StdName || sit1.Typ == StreetItemType.Name) || sit1.Typ == StreetItemType.Noun))) 
                    {
                        sit.NumberHasPrefix = true;
                        sit = sit1;
                        res.Add(sit);
                        t = sit.EndToken;
                        continue;
                    }
                }
                if (t.IsChar('.') && sit != null && sit.Typ == StreetItemType.Noun) 
                {
                    if (t.WhitespacesAfterCount > 1) 
                        break;
                    sit = TryParse(t.Next, null, false, ad);
                    if (sit == null) 
                        break;
                    if (sit.Typ == StreetItemType.Number || sit.Typ == StreetItemType.StdAdjective) 
                    {
                        StreetItemToken sit1 = TryParse(sit.EndToken.Next, null, false, ad);
                        if (sit1 != null && ((sit1.Typ == StreetItemType.StdAdjective || sit1.Typ == StreetItemType.StdName || sit1.Typ == StreetItemType.Name))) 
                        {
                        }
                        else if (!Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(sit)) 
                            break;
                        else 
                        {
                            AddressItemToken ai = AddressItemToken.TryParsePureItem(t.Next, null, null);
                            if (ai != null && ai.Typ != AddressItemType.Number) 
                                break;
                        }
                    }
                    else if (sit.Typ != StreetItemType.Name && sit.Typ != StreetItemType.StdName && sit.Typ != StreetItemType.Age) 
                        break;
                    if (t.Previous.GetMorphClassInDictionary().IsNoun) 
                    {
                        if (!sit.IsInDictionary) 
                        {
                            Pullenti.Ner.Token tt = sit.EndToken.Next;
                            bool hasHouse = false;
                            for (; tt != null; tt = tt.Next) 
                            {
                                if (tt.IsNewlineBefore) 
                                    break;
                                if (tt.IsComma) 
                                    continue;
                                AddressItemToken ai = AddressItemToken.TryParsePureItem(tt, null, null);
                                if (ai != null && ((ai.Typ == AddressItemType.House || ai.Typ == AddressItemType.Building || ai.Typ == AddressItemType.Corpus))) 
                                {
                                    hasHouse = true;
                                    break;
                                }
                                if (tt is Pullenti.Ner.NumberToken) 
                                {
                                    hasHouse = true;
                                    break;
                                }
                                StreetItemToken vv = TryParse(tt, null, false, ad);
                                if (vv == null || vv.Typ == StreetItemType.Noun) 
                                    break;
                                tt = vv.EndToken;
                            }
                            if (!hasHouse) 
                                break;
                        }
                        if (t.Previous.Previous != null) 
                        {
                            Pullenti.Ner.Core.NounPhraseToken npt11 = Pullenti.Ner.Geo.Internal.MiscLocationHelper.TryParseNpt(t.Previous.Previous);
                            if (npt11 != null && npt11.EndToken == t.Previous) 
                                break;
                        }
                    }
                    res.Add(sit);
                }
                else 
                {
                    sit = TryParse(t, res[res.Count - 1], false, ad);
                    if (sit == null) 
                    {
                        List<StreetItemToken> spli = TryParseSpec(t, res[res.Count - 1]);
                        if (spli != null && spli.Count > 0) 
                        {
                            res.AddRange(spli);
                            t = spli[spli.Count - 1].EndToken;
                            continue;
                        }
                        if (((t is Pullenti.Ner.TextToken) && ((res.Count == 2 || res.Count == 3)) && res[0].Typ == StreetItemType.Noun) && res[1].Typ == StreetItemType.Number && ((((t as Pullenti.Ner.TextToken).Term == "ГОДА" || (t as Pullenti.Ner.TextToken).Term == "МАЯ" || (t as Pullenti.Ner.TextToken).Term == "МАРТА") || (t as Pullenti.Ner.TextToken).Term == "СЪЕЗДА"))) 
                        {
                            res.Add((sit = new StreetItemToken(t, t) { Typ = StreetItemType.StdName, Value = (t as Pullenti.Ner.TextToken).Term }));
                            continue;
                        }
                        sit = res[res.Count - 1];
                        if (t == null) 
                            break;
                        if (sit.Typ == StreetItemType.Noun && ((sit.Termin.CanonicText == "МИКРОРАЙОН" || sit.Termin.CanonicText == "МІКРОРАЙОН")) && (t.WhitespacesBeforeCount < 2)) 
                        {
                            Pullenti.Ner.Token tt1 = t;
                            if (tt1.IsHiphen && tt1.Next != null) 
                                tt1 = tt1.Next;
                            if (Pullenti.Ner.Core.BracketHelper.IsBracket(tt1, true) && tt1.Next != null) 
                                tt1 = tt1.Next;
                            Pullenti.Ner.Token tt2 = tt1.Next;
                            bool br = false;
                            if (Pullenti.Ner.Core.BracketHelper.IsBracket(tt2, true)) 
                            {
                                tt2 = tt2.Next;
                                br = true;
                            }
                            if (((tt1 is Pullenti.Ner.TextToken) && tt1.LengthChar == 1 && tt1.Chars.IsLetter) && ((AddressItemToken.CheckHouseAfter(tt2, false, true) || tt2 == null))) 
                            {
                                sit = new StreetItemToken(t, (br ? tt1.Next : tt1)) { Typ = StreetItemType.Name, Value = (tt1 as Pullenti.Ner.TextToken).Term };
                                char ch1 = AddressItemToken.CorrectChar(sit.Value[0]);
                                if (ch1 != 0 && ch1 != sit.Value[0]) 
                                    sit.AltValue = string.Format("{0}", ch1);
                                res.Add(sit);
                                break;
                            }
                        }
                        if (t.IsComma && (((sit.Typ == StreetItemType.Name || sit.Typ == StreetItemType.StdName || sit.Typ == StreetItemType.StdPartOfName) || sit.Typ == StreetItemType.StdAdjective || ((sit.Typ == StreetItemType.Number && res.Count > 1 && (((res[res.Count - 2].Typ == StreetItemType.Name || res[res.Count - 2].Typ == StreetItemType.StdName || res[res.Count - 2].Typ == StreetItemType.StdAdjective) || res[res.Count - 2].Typ == StreetItemType.StdPartOfName))))))) 
                        {
                            StreetItemToken sit2 = TryParse(t.Next, null, false, ad);
                            if (sit2 != null && sit2.Typ == StreetItemType.Noun) 
                            {
                                Pullenti.Ner.Token ttt = sit2.EndToken.Next;
                                if (ttt != null && ttt.IsComma) 
                                    ttt = ttt.Next;
                                AddressItemToken add = AddressItemToken.TryParsePureItem(ttt, null, null);
                                if (add != null && ((add.Typ == AddressItemType.House || add.Typ == AddressItemType.Corpus || add.Typ == AddressItemType.Building))) 
                                {
                                    res.Add(sit2);
                                    t = sit2.EndToken;
                                    continue;
                                }
                            }
                        }
                        else if (t.IsComma && sit.Typ == StreetItemType.Noun && sit.Termin.CanonicText.Contains("КВАРТАЛ")) 
                        {
                            Pullenti.Ner.Geo.Internal.NumToken num = Pullenti.Ner.Geo.Internal.NumToken.TryParse(t.Next, Pullenti.Ner.Geo.Internal.GeoTokenType.Street);
                            if (num != null && num.IsCadasterNumber) 
                                continue;
                        }
                        if (Pullenti.Ner.Core.BracketHelper.CanBeStartOfSequence(t, true, false)) 
                        {
                            StreetItemToken sit1 = res[res.Count - 1];
                            if (sit1.Typ == StreetItemType.Noun && ((sit1.NounIsDoubtCoef == 0 || (((t.Next is Pullenti.Ner.TextToken) && !t.Next.Chars.IsAllLower))))) 
                            {
                                Pullenti.Ner.Core.BracketSequenceToken br = Pullenti.Ner.Core.BracketHelper.TryParse(t, Pullenti.Ner.Core.BracketParseAttr.No, 100);
                                if (br != null && (br.LengthChar < 50)) 
                                {
                                    StreetItemToken sit2 = TryParse(t.Next, null, false, ad);
                                    if (sit2 != null && sit2.EndToken.Next == br.EndToken) 
                                    {
                                        if (sit2.Value == null && sit2.Typ == StreetItemType.Name) 
                                            sit2.Value = Pullenti.Ner.Core.MiscHelper.GetTextValue(sit2.BeginToken, sit2.EndToken, Pullenti.Ner.Core.GetTextAttr.No);
                                        sit2.BeginToken = t;
                                        sit2.IsInBrackets = true;
                                        t = (sit2.EndToken = br.EndToken);
                                        res.Add(sit2);
                                        continue;
                                    }
                                    res.Add(new StreetItemToken(t, br.EndToken) { Typ = StreetItemType.Name, Value = Pullenti.Ner.Core.MiscHelper.GetTextValue(t, br.EndToken, Pullenti.Ner.Core.GetTextAttr.No), IsInBrackets = true });
                                    t = br.EndToken;
                                    continue;
                                }
                            }
                        }
                        if (t.IsHiphen && (t.Next is Pullenti.Ner.NumberToken) && (t.Next as Pullenti.Ner.NumberToken).IntValue != null) 
                        {
                            sit = res[res.Count - 1];
                            if (sit.Typ == StreetItemType.Noun && (((sit.Termin.CanonicText == "КВАРТАЛ" || sit.Termin.CanonicText == "МИКРОРАЙОН" || sit.Termin.CanonicText == "ГОРОДОК") || sit.Termin.CanonicText == "МІКРОРАЙОН"))) 
                            {
                                sit = new StreetItemToken(t, t.Next) { Typ = StreetItemType.Number, Value = (t.Next as Pullenti.Ner.NumberToken).Value, NumberType = (t.Next as Pullenti.Ner.NumberToken).Typ, NumberHasPrefix = true };
                                res.Add(sit);
                                t = t.Next;
                                continue;
                            }
                        }
                        if ((((t.IsChar(':') || t.IsHiphen)) && res.Count == 1 && res[0].Typ == StreetItemType.Noun) && (t.WhitespacesAfterCount < 3)) 
                            continue;
                        if ((t.IsComma && res.Count == 1 && res[0].Typ == StreetItemType.Noun) && Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(t)) 
                        {
                            sit = TryParse(t.Next, null, false, null);
                            if (sit != null && ((sit.Typ == StreetItemType.Name || sit.Typ == StreetItemType.StdName || sit.Typ == StreetItemType.StdAdjective))) 
                            {
                                res.Add(sit);
                                t = sit.EndToken;
                                continue;
                            }
                        }
                        break;
                    }
                    res.Add(sit);
                    if (sit.Typ == StreetItemType.Name) 
                    {
                        int cou = 0;
                        int jj;
                        for (jj = res.Count - 1; jj >= 0; jj--) 
                        {
                            if (res[jj].Typ == StreetItemType.Name) 
                                cou++;
                            else 
                                break;
                        }
                        if (cou > 4) 
                        {
                            if (jj < 0) 
                                return null;
                            res.RemoveRange(jj, res.Count - jj);
                            break;
                        }
                        if (res.Count > 1 && res[0].Typ == StreetItemType.Noun && res[0].IsRoad) 
                        {
                            Pullenti.Ner.Token tt = sit.EndToken.Next;
                            if (tt != null) 
                            {
                                if (tt.IsValue("Ш", null) || tt.IsValue("ШОССЕ", null) || tt.IsValue("ШОС", null)) 
                                {
                                    sit = sit.Clone();
                                    res[res.Count - 1] = sit;
                                    sit.EndToken = tt;
                                    if (tt.Next != null && tt.Next.IsChar('.') && tt.LengthChar <= 3) 
                                        sit.EndToken = sit.EndToken.Next;
                                }
                            }
                        }
                    }
                }
                t = sit.EndToken;
            }
            for (int i = 0; i < (res.Count - 1); i++) 
            {
                if (res[i].Typ == StreetItemType.Name && ((res[i + 1].AltTyp == StreetItemType.StdPartOfName || res[i + 1].Typ == StreetItemType.StdPartOfName))) 
                {
                    StreetItemToken r = res[i].Clone();
                    if (r.Value == null) 
                        r.Value = Pullenti.Ner.Core.MiscHelper.GetTextValueOfMetaToken(r, Pullenti.Ner.Core.GetTextAttr.No);
                    r.Misc = res[i + 1].Value ?? Pullenti.Ner.Core.MiscHelper.GetTextValueOfMetaToken(res[i + 1], Pullenti.Ner.Core.GetTextAttr.No);
                    r.EndToken = res[i + 1].EndToken;
                    res[i] = r;
                    res.RemoveAt(i + 1);
                }
                else if (res[i + 1].Typ == StreetItemType.Name && ((res[i].AltTyp == StreetItemType.StdPartOfName || res[i].Typ == StreetItemType.StdPartOfName))) 
                {
                    StreetItemToken r = res[i + 1].Clone();
                    if (r.Value == null) 
                        r.Value = Pullenti.Ner.Core.MiscHelper.GetTextValueOfMetaToken(r, Pullenti.Ner.Core.GetTextAttr.No);
                    r.Misc = res[i].Value ?? Pullenti.Ner.Core.MiscHelper.GetTextValueOfMetaToken(res[i], Pullenti.Ner.Core.GetTextAttr.No);
                    r.BeginToken = res[i].BeginToken;
                    res[i] = r;
                    res.RemoveAt(i + 1);
                }
            }
            for (int i = 0; i < (res.Count - 1); i++) 
            {
                if (res[i].Typ == StreetItemType.Name && res[i + 1].Typ == StreetItemType.Name && (res[i].WhitespacesAfterCount < 3)) 
                {
                    bool isProp = false;
                    bool isPers = false;
                    if (res[i].BeginToken.Morph.Class.IsNoun) 
                    {
                        Pullenti.Ner.ReferentToken rt = res[i].Kit.ProcessReferent("PERSON", res[i].BeginToken, null);
                        if (rt != null) 
                        {
                            if (rt.Referent.TypeName == "PERSONPROPERTY") 
                                isProp = true;
                            else if (rt.EndToken == res[i + 1].EndToken) 
                                isPers = true;
                        }
                    }
                    if ((i == 0 && ((!isProp && !isPers)) && ((i + 2) < res.Count)) && res[i + 2].Typ == StreetItemType.Noun && !res[i].BeginToken.Morph.Class.IsAdjective) 
                    {
                        if (Pullenti.Ner.Geo.Internal.MiscLocationHelper.CheckGeoObjectBefore(res[0].BeginToken, false) && res[0].EndToken.Next == res[1].BeginToken && (res[0].WhitespacesAfterCount < 2)) 
                        {
                        }
                        else 
                        {
                            res.RemoveAt(i);
                            i--;
                            continue;
                        }
                    }
                    if (res[i].Morph.Class.IsAdjective && res[i + 1].Morph.Class.IsAdjective && !isPers) 
                    {
                        if (res[i].EndToken.Next.IsHiphen) 
                        {
                        }
                        else if (i == 1 && res[0].Typ == StreetItemType.Noun && res.Count == 3) 
                        {
                        }
                        else if (i == 0 && res.Count == 3 && res[2].Typ == StreetItemType.Noun) 
                        {
                        }
                        else 
                            continue;
                    }
                    if (res[i].Chars.Value != res[i + 1].Chars.Value) 
                    {
                        Pullenti.Ner.ReferentToken rt = res[0].Kit.ProcessReferent("ORGANIZATION", res[i + 1].BeginToken, null);
                        if (rt != null) 
                        {
                            res.RemoveRange(i + 1, res.Count - i - 1);
                            continue;
                        }
                    }
                    StreetItemToken r = res[i].Clone();
                    if (r.Value == null) 
                        r.Value = Pullenti.Ner.Core.MiscHelper.GetTextValueOfMetaToken(res[i], Pullenti.Ner.Core.GetTextAttr.No);
                    Pullenti.Ner.Token tt1 = res[i + 1].EndToken;
                    Pullenti.Morph.MorphClass mc1 = res[i].BeginToken.GetMorphClassInDictionary();
                    Pullenti.Morph.MorphClass mc2 = tt1.GetMorphClassInDictionary();
                    if ((tt1.IsValue("БОР", null) || tt1.IsValue("САД", null) || tt1.IsValue("ПАРК", null)) || tt1.Previous.IsHiphen) 
                        r.Value = Pullenti.Ner.Core.MiscHelper.GetTextValue(res[i].BeginToken, res[i + 1].EndToken, Pullenti.Ner.Core.GetTextAttr.No);
                    else if (((mc1.IsProperName && !mc2.IsProperName)) || ((!mc1.IsProperSurname && mc2.IsProperSurname))) 
                    {
                        if (r.Misc == null) 
                            r.Misc = r.Value;
                        r.Value = res[i + 1].Value ?? Pullenti.Ner.Core.MiscHelper.GetTextValueOfMetaToken(res[i + 1], Pullenti.Ner.Core.GetTextAttr.No);
                    }
                    else if (((mc2.IsProperName && !mc1.IsProperName)) || ((!mc2.IsProperSurname && mc1.IsProperSurname))) 
                    {
                        if (r.Misc == null) 
                            r.Misc = Pullenti.Ner.Core.MiscHelper.GetTextValueOfMetaToken(res[i + 1], Pullenti.Ner.Core.GetTextAttr.No);
                    }
                    else 
                        r.Value = Pullenti.Ner.Core.MiscHelper.GetTextValue(res[i].BeginToken, res[i + 1].EndToken, Pullenti.Ner.Core.GetTextAttr.No);
                    if (r.Value.Contains("-")) 
                        r.Value = r.Value.Replace('-', ' ');
                    r.OrtoTerr = res[i + 1].OrtoTerr;
                    r.EndToken = res[i + 1].EndToken;
                    r.ExistStreet = null;
                    r.IsInDictionary = res[i + 1].IsInDictionary || res[i].IsInDictionary;
                    res[i] = r;
                    res.RemoveAt(i + 1);
                    i--;
                }
                else if ((res[i].Typ == StreetItemType.Noun && res[i + 1].Typ == StreetItemType.Noun && res[i].Termin == res[i + 1].Termin) && (res[i].WhitespacesAfterCount < 3)) 
                {
                    StreetItemToken r = res[i].Clone();
                    r.EndToken = res[i + 1].EndToken;
                    res.RemoveAt(i + 1);
                    i--;
                }
            }
            for (int i = 0; i < (res.Count - 1); i++) 
            {
                if (res[i].Typ == StreetItemType.StdAdjective && res[i].EndToken.IsChar('.') && res[i + 1]._isSurname()) 
                {
                    StreetItemToken r = res[i + 1].Clone();
                    r.Value = (res[i + 1].BeginToken as Pullenti.Ner.TextToken).Term;
                    r.AltValue = Pullenti.Ner.Core.MiscHelper.GetTextValue(res[i].BeginToken, res[i + 1].EndToken, Pullenti.Ner.Core.GetTextAttr.No);
                    r.BeginToken = res[i].BeginToken;
                    r.StdAdjVersion = res[i];
                    res[i + 1] = r;
                    res.RemoveAt(i);
                    break;
                }
            }
            for (int i = 0; i < (res.Count - 1); i++) 
            {
                if ((res[i + 1].Typ == StreetItemType.StdAdjective && res[i + 1].EndToken.IsChar('.') && res[i + 1].BeginToken.LengthChar == 1) && !res[i].BeginToken.Chars.IsAllLower) 
                {
                    if (res[i]._isSurname()) 
                    {
                        if (i == (res.Count - 2) || res[i + 2].Typ != StreetItemType.Noun) 
                        {
                            StreetItemToken r = res[i].Clone();
                            if (r.Value == null) 
                                r.Value = Pullenti.Ner.Core.MiscHelper.GetTextValueOfMetaToken(r, Pullenti.Ner.Core.GetTextAttr.No);
                            r.EndToken = res[i + 1].EndToken;
                            r.StdAdjVersion = res[i + 1];
                            res[i] = r;
                            res.RemoveAt(i + 1);
                            break;
                        }
                    }
                }
            }
            for (int i = 0; i < (res.Count - 1); i++) 
            {
                if (res[i].Typ == StreetItemType.Name || res[i].Typ == StreetItemType.StdName || res[i].Typ == StreetItemType.StdAdjective) 
                {
                    if (res[i + 1].Typ == StreetItemType.Noun && !res[i + 1].IsAbridge && res[i + 1].Termin.CanonicText != "УЛИЦА") 
                    {
                        List<StreetItemToken> res0 = new List<StreetItemToken>(res);
                        res0.RemoveRange(0, i + 1);
                        AddressItemToken rtt = StreetDefineHelper.TryParseStreet(res0, false, false, false, null);
                        if (rtt != null) 
                            continue;
                        int i0 = -1;
                        if (i == 1 && res[0].Typ == StreetItemType.Noun && res.Count == 3) 
                            i0 = 0;
                        else if (i == 0 && res.Count == 3 && res[2].Typ == StreetItemType.Noun) 
                            i0 = 2;
                        if (i0 < 0) 
                            continue;
                        if (res[i0].Termin == res[i + 1].Termin) 
                            continue;
                        StreetItemToken r = res[i].Clone();
                        r.AltValue = res[i].Value ?? Pullenti.Ner.Core.MiscHelper.GetTextValue(res[i].BeginToken, res[i].EndToken, Pullenti.Ner.Core.GetTextAttr.No);
                        if (res[i].Typ == StreetItemType.StdAdjective) 
                        {
                            List<string> adjs = Pullenti.Ner.Geo.Internal.MiscLocationHelper.GetStdAdjFull(res[i].BeginToken, res[i + 1].Morph.Gender, res[i + 1].Morph.Number, true);
                            if (adjs != null && adjs.Count > 0) 
                                r.AltValue = adjs[0];
                        }
                        r.Value = string.Format("{0} {1}", r.AltValue, res[i + 1].Termin.CanonicText);
                        r.Typ = StreetItemType.StdName;
                        r.EndToken = res[i + 1].EndToken;
                        res[i] = r;
                        StreetItemToken rr = res[i0].Clone();
                        rr.AltTermin = res[i + 1].Termin;
                        res[i0] = rr;
                        res.RemoveAt(i + 1);
                        i--;
                    }
                }
            }
            if ((res.Count >= 3 && res[0].Typ == StreetItemType.Noun && res[0].Termin.CanonicText == "КВАРТАЛ") && ((res[1].Typ == StreetItemType.Name || res[1].Typ == StreetItemType.StdName)) && res[2].Typ == StreetItemType.Noun) 
            {
                if (res.Count == 3 || res[3].Typ == StreetItemType.Number) 
                {
                    List<StreetItemToken> res0 = new List<StreetItemToken>(res);
                    res0.RemoveRange(0, 2);
                    AddressItemToken rtt = StreetDefineHelper.TryParseStreet(res0, false, false, false, null);
                    if (rtt == null || res0[0].Chars.IsCapitalUpper) 
                    {
                        StreetItemToken r = res[1].Clone();
                        r.Value = string.Format("{0} {1}", Pullenti.Ner.Core.MiscHelper.GetTextValueOfMetaToken(res[1], Pullenti.Ner.Core.GetTextAttr.No), res[2].Termin.CanonicText);
                        r.EndToken = res[2].EndToken;
                        res[1] = r;
                        res.RemoveAt(2);
                    }
                }
            }
            if ((res.Count >= 3 && res[0].Typ == StreetItemType.Noun && res[0].Termin.CanonicText == "КВАРТАЛ") && ((res[2].Typ == StreetItemType.Name || res[2].Typ == StreetItemType.StdName)) && res[1].Typ == StreetItemType.Noun) 
            {
                if (res.Count == 3 || res[3].Typ == StreetItemType.Number) 
                {
                    StreetItemToken r = res[1].Clone();
                    r.Value = string.Format("{0} {1}", Pullenti.Ner.Core.MiscHelper.GetTextValueOfMetaToken(res[2], Pullenti.Ner.Core.GetTextAttr.No), res[1].Termin.CanonicText);
                    r.EndToken = res[2].EndToken;
                    r.Typ = StreetItemType.Name;
                    res[1] = r;
                    res.RemoveAt(2);
                }
            }
            if ((res.Count >= 3 && res[0].Typ == StreetItemType.Number && !res[0].IsNumberKm) && res[1].Typ == StreetItemType.Noun) 
            {
                if (!Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(res[0]) && res[2].Typ != StreetItemType.StdName && res[2].Typ != StreetItemType.Fix) 
                {
                    Pullenti.Ner.NumberToken nt = res[0].BeginToken as Pullenti.Ner.NumberToken;
                    if (nt != null && nt.Typ == Pullenti.Ner.NumberSpellingType.Digit && nt.Morph.Class.IsUndefined) 
                        return null;
                }
            }
            int ii0 = -1;
            int ii1 = -1;
            if (res.Count > 0 && res[0].Typ == StreetItemType.Noun && res[0].IsRoad) 
            {
                ii0 = (ii1 = 0);
                if (((ii0 + 1) < res.Count) && res[ii0 + 1].Typ == StreetItemType.Number && res[ii0 + 1].IsNumberKm) 
                    ii0++;
            }
            else if ((res.Count > 1 && res[0].Typ == StreetItemType.Number && res[0].IsNumberKm) && res[1].Typ == StreetItemType.Noun && res[1].IsRoad) 
                ii0 = (ii1 = 1);
            if (ii0 >= 0) 
            {
                if (res.Count == (ii0 + 1)) 
                {
                    Pullenti.Ner.Token tt = res[ii0].EndToken.Next;
                    StreetItemToken num = _tryAttachRoadNum(tt);
                    if (num != null) 
                    {
                        res.Add(num);
                        tt = num.EndToken.Next;
                        res[0].IsAbridge = false;
                    }
                    if (tt != null && (tt.GetReferent() is Pullenti.Ner.Geo.GeoReferent)) 
                    {
                        Pullenti.Ner.Geo.GeoReferent g1 = tt.GetReferent() as Pullenti.Ner.Geo.GeoReferent;
                        tt = tt.Next;
                        if (tt != null && tt.IsHiphen) 
                            tt = tt.Next;
                        Pullenti.Ner.Geo.GeoReferent g2 = (tt == null ? null : tt.GetReferent() as Pullenti.Ner.Geo.GeoReferent);
                        if (g2 != null) 
                        {
                            if (g1.IsCity && g2.IsCity) 
                            {
                                StreetItemToken nam = new StreetItemToken(res[0].EndToken.Next, tt) { Typ = StreetItemType.Name };
                                nam.Value = string.Format("{0} - {1}", g1.ToStringEx(true, tt.Kit.BaseLanguage, 0), g2.ToStringEx(true, tt.Kit.BaseLanguage, 0)).ToUpper();
                                nam.AltValue = string.Format("{0} - {1}", g2.ToStringEx(true, tt.Kit.BaseLanguage, 0), g1.ToStringEx(true, tt.Kit.BaseLanguage, 0)).ToUpper();
                                res.Add(nam);
                            }
                        }
                    }
                    else if (Pullenti.Ner.Core.BracketHelper.IsBracket(tt, false)) 
                    {
                        Pullenti.Ner.Core.BracketSequenceToken br = Pullenti.Ner.Core.BracketHelper.TryParse(tt, Pullenti.Ner.Core.BracketParseAttr.No, 100);
                        if (br != null) 
                        {
                            StreetItemToken nam = new StreetItemToken(tt, br.EndToken) { Typ = StreetItemType.Name, IsInBrackets = true };
                            nam.Value = Pullenti.Ner.Core.MiscHelper.GetTextValue(tt.Next, br.EndToken, Pullenti.Ner.Core.GetTextAttr.No);
                            res.Add(nam);
                        }
                    }
                }
                else if ((res.Count == (ii0 + 2) && res[ii0 + 1].Typ == StreetItemType.Name && res[ii0 + 1].EndToken.Next != null) && res[ii0 + 1].EndToken.Next.IsHiphen) 
                {
                    Pullenti.Ner.Token tt = res[ii0 + 1].EndToken.Next.Next;
                    Pullenti.Ner.Geo.GeoReferent g2 = (tt == null ? null : tt.GetReferent() as Pullenti.Ner.Geo.GeoReferent);
                    Pullenti.Ner.Token te = null;
                    string name2 = null;
                    if (g2 == null && tt != null) 
                    {
                        Pullenti.Ner.ReferentToken rt = tt.Kit.ProcessReferent("GEO", tt, null);
                        if (rt != null) 
                        {
                            te = rt.EndToken;
                            name2 = rt.Referent.ToStringEx(true, te.Kit.BaseLanguage, 0);
                        }
                        else 
                        {
                            List<Pullenti.Ner.Geo.Internal.CityItemToken> cits2 = Pullenti.Ner.Geo.Internal.CityItemToken.TryParseList(tt, 2, null);
                            if (cits2 != null) 
                            {
                                if (cits2.Count == 1 && ((cits2[0].Typ == Pullenti.Ner.Geo.Internal.CityItemToken.ItemType.ProperName || cits2[0].Typ == Pullenti.Ner.Geo.Internal.CityItemToken.ItemType.City))) 
                                {
                                    if (cits2[0].OntoItem != null) 
                                        name2 = cits2[0].OntoItem.CanonicText;
                                    else 
                                        name2 = cits2[0].Value;
                                    te = cits2[0].EndToken;
                                }
                            }
                        }
                    }
                    else if (g2 != null) 
                    {
                        te = tt;
                        name2 = g2.ToStringEx(true, te.Kit.BaseLanguage, 0);
                    }
                    if (((g2 != null && g2.IsCity)) || ((g2 == null && name2 != null))) 
                    {
                        StreetItemToken r = res[ii0 + 1].Clone();
                        r.AltValue = string.Format("{0} - {1}", name2, res[ii0 + 1].Value ?? res[ii0 + 1].GetSourceText()).ToUpper();
                        r.Value = string.Format("{0} - {1}", res[ii0 + 1].Value ?? res[ii0 + 1].GetSourceText(), name2).ToUpper();
                        r.EndToken = te;
                        res[ii0 + 1] = r;
                    }
                }
                StreetItemToken nn = _tryAttachRoadNum(res[res.Count - 1].EndToken.Next);
                if (nn != null) 
                {
                    res.Add(nn);
                    res[ii1].IsAbridge = false;
                }
                if (res.Count > (ii0 + 1) && res[ii0 + 1].Typ == StreetItemType.Name && res[ii1].Termin.CanonicText == "АВТОДОРОГА") 
                {
                    if (res[ii0 + 1].BeginToken.IsValue("ФЕДЕРАЛЬНЫЙ", null)) 
                        return null;
                    Pullenti.Ner.Core.NounPhraseToken npt = Pullenti.Ner.Geo.Internal.MiscLocationHelper.TryParseNpt(res[ii0 + 1].BeginToken);
                    if (npt != null && npt.Adjectives.Count > 0) 
                    {
                        if (npt.EndToken.IsValue("ЗНАЧЕНИЕ", null)) 
                            return null;
                    }
                }
            }
            while (res.Count > 1) 
            {
                StreetItemToken it = res[res.Count - 1];
                if (!it.IsWhitespaceBefore) 
                    break;
                StreetItemToken it0 = (res.Count > 1 ? res[res.Count - 2] : null);
                if (it.Typ == StreetItemType.Number && !it.NumberHasPrefix && !it.IsNumberKm) 
                {
                    if (it.BeginToken is Pullenti.Ner.NumberToken) 
                    {
                        if (res.Count == 2 && res[0].Typ == StreetItemType.Noun) 
                            break;
                        if (!it.BeginToken.Morph.Class.IsAdjective || it.BeginToken.Morph.Class.IsNoun) 
                        {
                            if (AddressItemToken.CheckHouseAfter(it.EndToken.Next, false, true)) 
                                it.NumberHasPrefix = true;
                            else if (it0 != null && it0.Typ == StreetItemType.Noun && (((it0.Termin.CanonicText == "МИКРОРАЙОН" || it0.Termin.CanonicText == "МІКРОРАЙОН" || it0.Termin.CanonicText == "КВАРТАЛ") || it0.Termin.CanonicText == "ГОРОДОК"))) 
                            {
                                AddressItemToken ait = AddressItemToken.TryParsePureItem(it.BeginToken, null, null);
                                if (ait != null && ait.Typ == AddressItemType.Number && ait.EndChar > it.EndChar) 
                                {
                                    it.NumberType = Pullenti.Ner.NumberSpellingType.Undefined;
                                    it.Value = ait.Value;
                                    it.EndToken = ait.EndToken;
                                    it.Typ = StreetItemType.Name;
                                }
                            }
                            else if (it0 != null && it0.Termin != null && it0.Termin.CanonicText == "ПОЧТОВОЕ ОТДЕЛЕНИЕ") 
                                it.NumberHasPrefix = true;
                            else if (it0 != null && it0.BeginToken.IsValue("ЛИНИЯ", null)) 
                                it.NumberHasPrefix = true;
                            else if (res.Count == 2 && res[0].Typ == StreetItemType.Noun && (res[0].WhitespacesAfterCount < 2)) 
                            {
                            }
                            else if (it.BeginToken.Morph.Class.IsAdjective && (it.BeginToken as Pullenti.Ner.NumberToken).Typ == Pullenti.Ner.NumberSpellingType.Words && it.BeginToken.Chars.IsCapitalUpper) 
                                it.NumberHasPrefix = true;
                            else if (it.BeginToken.Previous.IsHiphen) 
                                it.NumberHasPrefix = true;
                            else 
                            {
                                res.RemoveAt(res.Count - 1);
                                continue;
                            }
                        }
                        else 
                            it.NumberHasPrefix = true;
                    }
                }
                break;
            }
            if (res.Count == 0) 
                return null;
            for (int i = 0; i < res.Count; i++) 
            {
                if (res[i].NextItem != null) 
                    res.Insert(i + 1, res[i].NextItem);
            }
            for (int i = 0; i < res.Count; i++) 
            {
                if ((res[i].Typ == StreetItemType.Noun && res[i].Chars.IsCapitalUpper && (((res[i].Termin.CanonicText == "НАБЕРЕЖНАЯ" || res[i].Termin.CanonicText == "МИКРОРАЙОН" || res[i].Termin.CanonicText == "НАБЕРЕЖНА") || res[i].Termin.CanonicText == "МІКРОРАЙОН" || res[i].Termin.CanonicText == "ГОРОДОК"))) && res[i].BeginToken.IsValue(res[i].Termin.CanonicText, null)) 
                {
                    bool ok = false;
                    if (i > 0 && ((res[i - 1].Typ == StreetItemType.Noun || res[i - 1].Typ == StreetItemType.StdAdjective))) 
                        ok = true;
                    else if (i > 1 && ((res[i - 1].Typ == StreetItemType.StdAdjective || res[i - 1].Typ == StreetItemType.Number)) && res[i - 2].Typ == StreetItemType.Noun) 
                        ok = true;
                    if (ok) 
                    {
                        StreetItemToken r = res[i].Clone();
                        r.Typ = StreetItemType.Name;
                        res[i] = r;
                    }
                }
            }
            StreetItemToken last = res[res.Count - 1];
            for (int kk = 0; kk < 2; kk++) 
            {
                Pullenti.Ner.Token ttt = last.EndToken.Next;
                if (((last.Typ == StreetItemType.Name && ttt != null && ttt.LengthChar == 1) && ttt.Chars.IsAllUpper && (ttt.WhitespacesBeforeCount < 2)) && ttt.Next != null && ttt.Next.IsChar('.')) 
                {
                    if (AddressItemToken.TryParsePureItem(ttt, null, null) != null) 
                        break;
                    last = last.Clone();
                    last.EndToken = ttt.Next;
                    res[res.Count - 1] = last;
                }
            }
            if (res.Count > 1) 
            {
                if (res[res.Count - 1].Org != null) 
                {
                    if (res.Count == 2 && res[0].Typ == StreetItemType.Noun) 
                    {
                    }
                    else 
                        res.RemoveAt(res.Count - 1);
                }
            }
            if (res.Count == 0) 
                return null;
            return res;
        }
        public static void Initialize()
        {
            if (m_Ontology != null) 
                return;
            m_Ontology = new Pullenti.Ner.Core.TerminCollection();
            m_OntologyEx = new Pullenti.Ner.Core.TerminCollection();
            m_StdOntMisc = new Pullenti.Ner.Core.TerminCollection();
            m_StdAdj = new Pullenti.Ner.Core.TerminCollection();
            Pullenti.Ner.Core.Termin t;
            t = new Pullenti.Ner.Core.Termin("УЛИЦА") { Tag = StreetItemType.Noun, Gender = Pullenti.Morph.MorphGender.Feminie };
            t.AddAbridge("УЛ.");
            t.AddAbridge("УЛЮ");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ВУЛИЦЯ") { Tag = StreetItemType.Noun, Lang = Pullenti.Morph.MorphLang.UA, Gender = Pullenti.Morph.MorphGender.Feminie };
            t.AddAbridge("ВУЛ.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("STREET") { Tag = StreetItemType.Noun };
            t.AddAbridge("ST.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПЛОЩАДЬ") { Tag = StreetItemType.Noun, Tag2 = 1, Gender = Pullenti.Morph.MorphGender.Feminie };
            t.AddAbridge("ПЛ.");
            t.AddAbridge("ПЛОЩ.");
            t.AddAbridge("ПЛ-ДЬ");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПЛОЩА") { Tag = StreetItemType.Noun, Lang = Pullenti.Morph.MorphLang.UA, Tag2 = 1, Gender = Pullenti.Morph.MorphGender.Feminie };
            t.AddAbridge("ПЛ.");
            t.AddAbridge("ПЛОЩ.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("МАЙДАН") { Tag = StreetItemType.Noun, Tag2 = 0, Gender = Pullenti.Morph.MorphGender.Masculine };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("SQUARE") { Tag = StreetItemType.Noun };
            t.AddAbridge("SQ.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПРОЕЗД") { Tag = StreetItemType.Noun, Tag2 = 1, Gender = Pullenti.Morph.MorphGender.Masculine };
            t.AddAbridge("ПР.");
            t.AddAbridge("П-Д");
            t.AddAbridge("ПР-Д");
            t.AddAbridge("ПР-ЗД");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПРОЕЗД") { Tag = StreetItemType.Noun, Lang = Pullenti.Morph.MorphLang.UA, Tag2 = 1, Gender = Pullenti.Morph.MorphGender.Masculine };
            t.AddAbridge("ПР.");
            t.AddAbridge("П-Д");
            t.AddAbridge("ПР-Д");
            t.AddAbridge("ПР-ЗД");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ЛИНИЯ") { Tag = StreetItemType.Noun, Tag2 = 2, Gender = Pullenti.Morph.MorphGender.Feminie };
            t.AddAbridge("ЛИН.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ЛІНІЯ") { Tag = StreetItemType.Noun, Lang = Pullenti.Morph.MorphLang.UA, Tag2 = 2, Gender = Pullenti.Morph.MorphGender.Feminie };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("РЯД") { Tag = StreetItemType.Noun, Tag2 = 2, Gender = Pullenti.Morph.MorphGender.Masculine };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ОЧЕРЕДЬ") { Tag = StreetItemType.Noun, Tag2 = 2, Gender = Pullenti.Morph.MorphGender.Feminie };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПАНЕЛЬ") { Tag = StreetItemType.Noun, Tag2 = 2, Gender = Pullenti.Morph.MorphGender.Feminie };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("КУСТ") { Tag = StreetItemType.Noun, Tag2 = 2, Gender = Pullenti.Morph.MorphGender.Masculine };
            t.AddVariant("КУСТ ГАЗОВЫХ СКВАЖИН", false);
            t.AddVariant("КУСТОВАЯ ПЛОЩАДКА СКВАЖИН", false);
            t.AddVariant("КУСТ СКВАЖИН", false);
            m_Ontology.Add(t);
            m_Prospect = (t = new Pullenti.Ner.Core.Termin("ПРОСПЕКТ") { Tag = StreetItemType.Noun, Tag2 = 0, Gender = Pullenti.Morph.MorphGender.Masculine });
            t.AddAbridge("ПРОС.");
            t.AddAbridge("ПРКТ");
            t.AddAbridge("ПРОСП.");
            t.AddAbridge("ПР-Т");
            t.AddAbridge("ПР-КТ");
            t.AddAbridge("П-Т");
            t.AddAbridge("П-КТ");
            t.AddAbridge("ПР Т");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПЕРЕУЛОК") { Tag = StreetItemType.Noun, Tag2 = 0, Gender = Pullenti.Morph.MorphGender.Masculine };
            t.AddAbridge("ПЕР.");
            t.AddAbridge("ПЕР-К");
            t.AddAbridge("П-К");
            t.AddVariant("ПРЕУЛОК", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПРОУЛОК") { Tag = StreetItemType.Noun, Tag2 = 0, Gender = Pullenti.Morph.MorphGender.Masculine };
            t.AddAbridge("ПРОУЛ.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПРОВУЛОК") { Tag = StreetItemType.Noun, Lang = Pullenti.Morph.MorphLang.UA, Tag2 = 0, Gender = Pullenti.Morph.MorphGender.Masculine };
            t.AddAbridge("ПРОВ.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("LANE") { Tag = StreetItemType.Noun, Tag2 = 0 };
            t.AddAbridge("LN.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ТУПИК") { Tag = StreetItemType.Noun, Tag2 = 1, Gender = Pullenti.Morph.MorphGender.Masculine };
            t.AddAbridge("ТУП.");
            t.AddAbridge("Т.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("БУЛЬВАР") { Tag = StreetItemType.Noun, Tag2 = 0, Gender = Pullenti.Morph.MorphGender.Masculine };
            t.AddAbridge("БУЛЬВ.");
            t.AddAbridge("БУЛ.");
            t.AddAbridge("Б-Р");
            t.AddAbridge("Б-РЕ");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("BOULEVARD") { Tag = StreetItemType.Noun, Tag2 = 0 };
            t.AddAbridge("BLVD");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("СКВЕР") { Tag = StreetItemType.Noun, Tag2 = 1 };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("НАБЕРЕЖНАЯ") { Tag = StreetItemType.Noun, Tag2 = 0, Gender = Pullenti.Morph.MorphGender.Feminie };
            t.AddAbridge("НАБ.");
            t.AddAbridge("НАБЕР.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("НАБЕРЕЖНА") { Tag = StreetItemType.Noun, Lang = Pullenti.Morph.MorphLang.UA, Tag2 = 0, Gender = Pullenti.Morph.MorphGender.Feminie };
            t.AddAbridge("НАБ.");
            t.AddAbridge("НАБЕР.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("АЛЛЕЯ") { Tag = StreetItemType.Noun, Tag2 = 0, Gender = Pullenti.Morph.MorphGender.Feminie };
            t.AddAbridge("АЛ.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("АЛЕЯ") { Tag = StreetItemType.Noun, Lang = Pullenti.Morph.MorphLang.UA, Tag2 = 0, Gender = Pullenti.Morph.MorphGender.Feminie };
            t.AddAbridge("АЛ.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ALLEY") { Tag = StreetItemType.Noun, Tag2 = 0 };
            t.AddAbridge("ALY.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("АВЕНЮ") { Tag = StreetItemType.Noun, Tag2 = 0, Gender = Pullenti.Morph.MorphGender.Feminie };
            t.AddVariant("АВЕНЬЮ", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПРОСЕКА") { Tag = StreetItemType.Noun, Tag2 = 1, Gender = Pullenti.Morph.MorphGender.Feminie };
            t.AddVariant("ПРОСЕК", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПРОСІКА") { Tag = StreetItemType.Noun, Lang = Pullenti.Morph.MorphLang.UA, Tag2 = 1, Gender = Pullenti.Morph.MorphGender.Feminie };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ТРАКТ") { Tag = StreetItemType.Noun, Tag2 = 1, Gender = Pullenti.Morph.MorphGender.Neuter };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ШОССЕ") { Tag = StreetItemType.Noun, Tag2 = 1, Gender = Pullenti.Morph.MorphGender.Neuter };
            t.AddAbridge("Ш.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ШОСЕ") { Tag = StreetItemType.Noun, Lang = Pullenti.Morph.MorphLang.UA, Tag2 = 1, Gender = Pullenti.Morph.MorphGender.Neuter };
            t.AddAbridge("Ш.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ROAD") { Tag = StreetItemType.Noun, Tag2 = 1 };
            t.AddAbridge("RD.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("МИКРОРАЙОН") { Tag = StreetItemType.Noun, Tag2 = 0, Gender = Pullenti.Morph.MorphGender.Masculine };
            t.AddAbridge("МКР.");
            t.AddAbridge("МИКР-Н");
            t.AddAbridge("МИКР.");
            t.AddAbridge("МКР-Н");
            t.AddAbridge("МКР-ОН");
            t.AddAbridge("МКРН.");
            t.AddAbridge("М-Н");
            t.AddAbridge("М-ОН");
            t.AddAbridge("М.Р-Н");
            t.AddAbridge("МИКР-ОН");
            t.AddVariant("МИКРОН", false);
            t.AddAbridge("М/Р");
            t.AddVariant("МІКРОРАЙОН", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("КВАРТАЛ") { Tag = StreetItemType.Noun, Tag2 = 2, Gender = Pullenti.Morph.MorphGender.Masculine };
            t.AddAbridge("КВАРТ.");
            t.AddAbridge("КВ-Л");
            t.AddAbridge("КВ.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("КАДАСТРОВЫЙ КВАРТАЛ") { Tag = StreetItemType.Noun, Tag2 = 2, Gender = Pullenti.Morph.MorphGender.Masculine };
            t.AddAbridge("КАД.КВАРТ.");
            t.AddAbridge("КАД.КВ-Л");
            t.AddAbridge("КАД.КВ.");
            t.AddAbridge("КАД.КВАРТАЛ");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ТОРФЯНОЙ УЧАСТОК") { Tag = StreetItemType.Noun, Tag2 = 2, Gender = Pullenti.Morph.MorphGender.Masculine };
            t.AddVariant("ТОРФУЧАСТОК", false);
            t.AddVariant("ТОРФОУЧАСТОК", false);
            t.AddAbridge("ТОРФ.УЧАСТОК");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("МОСТ") { Tag = StreetItemType.Noun, Tag2 = 2, Gender = Pullenti.Morph.MorphGender.Masculine };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("МІСТ") { Tag = StreetItemType.Noun, Lang = Pullenti.Morph.MorphLang.UA, Tag2 = 2, Gender = Pullenti.Morph.MorphGender.Masculine };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("PLAZA") { Tag = StreetItemType.Noun, Tag2 = 1 };
            t.AddAbridge("PLZ");
            m_Ontology.Add(t);
            m_Metro = (t = new Pullenti.Ner.Core.Termin("СТАНЦИЯ МЕТРО") { CanonicText = "МЕТРО", Tag = StreetItemType.Noun, Tag2 = 0, Gender = Pullenti.Morph.MorphGender.Feminie });
            t.AddVariant("СТАНЦІЯ МЕТРО", false);
            t.AddAbridge("СТ.МЕТРО");
            t.AddAbridge("СТ.М.");
            t.AddAbridge("МЕТРО");
            m_Ontology.Add(t);
            m_Road = (t = new Pullenti.Ner.Core.Termin("АВТОДОРОГА") { Tag = StreetItemType.Noun, Acronym = "ФАД", Tag2 = 0, Gender = Pullenti.Morph.MorphGender.Feminie });
            t.AddVariant("ФЕДЕРАЛЬНАЯ АВТОДОРОГА", false);
            t.AddVariant("АВТОМОБИЛЬНАЯ ДОРОГА", false);
            t.AddVariant("АВТОТРАССА", false);
            t.AddVariant("ФЕДЕРАЛЬНАЯ ТРАССА", false);
            t.AddVariant("ФЕДЕР ТРАССА", false);
            t.AddVariant("АВТОМАГИСТРАЛЬ", false);
            t.AddAbridge("А/Д");
            t.AddAbridge("ФЕДЕР.ТРАССА");
            t.AddAbridge("ФЕД.ТРАССА");
            t.AddVariant("ГОСТРАССА", false);
            t.AddVariant("ГОС.ТРАССА", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ДОРОГА") { CanonicText = "АВТОДОРОГА", Tag = StreetItemType.Noun, Tag2 = 1, Gender = Pullenti.Morph.MorphGender.Feminie };
            t.AddVariant("ТРАССА", false);
            t.AddVariant("МАГИСТРАЛЬ", false);
            t.AddAbridge("ДОР.");
            t.AddVariant("ДОР", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("АВТОДОРОГА") { Tag = StreetItemType.Noun, Lang = Pullenti.Morph.MorphLang.UA, Tag2 = 0, Gender = Pullenti.Morph.MorphGender.Feminie };
            t.AddVariant("ФЕДЕРАЛЬНА АВТОДОРОГА", false);
            t.AddVariant("АВТОМОБІЛЬНА ДОРОГА", false);
            t.AddVariant("АВТОТРАСА", false);
            t.AddVariant("ФЕДЕРАЛЬНА ТРАСА", false);
            t.AddVariant("АВТОМАГІСТРАЛЬ", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ДОРОГА") { CanonicText = "АВТОДОРОГА", Tag = StreetItemType.Noun, Lang = Pullenti.Morph.MorphLang.UA, Tag2 = 1, Gender = Pullenti.Morph.MorphGender.Feminie };
            t.AddVariant("ТРАСА", false);
            t.AddVariant("МАГІСТРАЛЬ", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("МОСКОВСКАЯ КОЛЬЦЕВАЯ АВТОМОБИЛЬНАЯ ДОРОГА") { Acronym = "МКАД", Tag = StreetItemType.Fix, Gender = Pullenti.Morph.MorphGender.Feminie };
            t.AddVariant("МОСКОВСКАЯ КОЛЬЦЕВАЯ АВТОДОРОГА", false);
            m_Ontology.Add(t);
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("САДОВОЕ КОЛЬЦО") { Tag = StreetItemType.Fix });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("БУЛЬВАРНОЕ КОЛЬЦО") { Tag = StreetItemType.Fix });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("ТРАНСПОРТНОЕ КОЛЬЦО") { Tag = StreetItemType.Fix });
            t = new Pullenti.Ner.Core.Termin("ПОЧТОВОЕ ОТДЕЛЕНИЕ") { Tag = StreetItemType.Noun, Acronym = "ОПС", Gender = Pullenti.Morph.MorphGender.Neuter };
            t.AddAbridge("П.О.");
            t.AddAbridge("ПОЧТ.ОТД.");
            t.AddAbridge("ПОЧТОВ.ОТД.");
            t.AddAbridge("ПОЧТОВОЕ ОТД.");
            t.AddAbridge("П/О");
            t.AddVariant("ОТДЕЛЕНИЕ ПОЧТОВОЙ СВЯЗИ", false);
            t.AddVariant("ПОЧТАМТ", false);
            t.AddVariant("ГЛАВПОЧТАМТ", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("БУДКА") { Tag = StreetItemType.Noun, Gender = Pullenti.Morph.MorphGender.Feminie };
            t.AddVariant("ЖЕЛЕЗНОДОРОЖНАЯ БУДКА", false);
            t.AddAbridge("Ж/Д БУДКА");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("КАЗАРМА") { Tag = StreetItemType.Noun, Gender = Pullenti.Morph.MorphGender.Feminie };
            t.AddVariant("ЖЕЛЕЗНОДОРОЖНАЯ КАЗАРМА", false);
            t.AddAbridge("Ж/Д КАЗАРМА");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("СТОЯНКА") { Tag = StreetItemType.Noun, Gender = Pullenti.Morph.MorphGender.Feminie };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПУНКТ") { Tag = StreetItemType.Noun, Gender = Pullenti.Morph.MorphGender.Masculine };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("РАЗЪЕЗД") { Tag = StreetItemType.Noun, Gender = Pullenti.Morph.MorphGender.Masculine };
            t.AddAbridge("РЗД");
            t.AddAbridge("Ж/Д РАЗЪЕЗД");
            t.AddVariant("ЖЕЛЕЗНОДОРОЖНЫЙ РАЗЪЕЗД", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ЗАЕЗД") { Tag = StreetItemType.Noun, Gender = Pullenti.Morph.MorphGender.Masculine };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПЕРЕЕЗД") { Tag = StreetItemType.Noun, Gender = Pullenti.Morph.MorphGender.Masculine };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("БОЛЬШОЙ") { Tag = StreetItemType.StdAdjective };
            t.AddAbridge("БОЛ.");
            t.AddAbridge("Б.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ВЕЛИКИЙ") { Tag = StreetItemType.StdAdjective };
            t.AddAbridge("ВЕЛ.");
            t.AddAbridge("В.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("МАЛЫЙ") { Tag = StreetItemType.StdAdjective };
            t.AddAbridge("МАЛ.");
            t.AddAbridge("М.");
            t.AddVariant("МАЛИЙ", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("СРЕДНИЙ") { Tag = StreetItemType.StdAdjective };
            t.AddAbridge("СРЕД.");
            t.AddAbridge("СР.");
            t.AddAbridge("С.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("СЕРЕДНІЙ") { Tag = StreetItemType.StdAdjective, Lang = Pullenti.Morph.MorphLang.UA };
            t.AddAbridge("СЕРЕД.");
            t.AddAbridge("СЕР.");
            t.AddAbridge("С.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ВЕРХНИЙ") { Tag = StreetItemType.StdAdjective };
            t.AddAbridge("ВЕРХН.");
            t.AddAbridge("ВЕРХ.");
            t.AddAbridge("ВЕР.");
            t.AddAbridge("В.");
            t.AddVariant("ВЕРХНІЙ", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("НИЖНИЙ") { Tag = StreetItemType.StdAdjective };
            t.AddAbridge("НИЖН.");
            t.AddAbridge("НИЖ.");
            t.AddAbridge("Н.");
            t.AddVariant("НИЖНІЙ", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("СТАРЫЙ") { Tag = StreetItemType.StdAdjective };
            t.AddAbridge("СТАР.");
            t.AddAbridge("СТ.");
            t.AddVariant("СТАРИЙ", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("НОВЫЙ") { Tag = StreetItemType.StdAdjective };
            t.AddAbridge("НОВ.");
            t.AddAbridge("Н.");
            t.AddVariant("НОВИЙ", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("КРАСНЫЙ") { Tag = StreetItemType.StdAdjective };
            t.AddAbridge("КРАСН.");
            t.AddAbridge("КР.");
            t.AddAbridge("КРАС.");
            t.AddVariant("ЧЕРВОНИЙ", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("НОМЕР") { Tag = StreetItemType.StdAdjective };
            t.AddAbridge("N");
            t.AddAbridge("№");
            t.AddAbridge("НОМ.");
            m_Ontology.Add(t);
            foreach (string s in new string[] {"ПРОЕКТИРУЕМЫЙ", "ЮНЫХ ЛЕНИНЦЕВ;ЮН. ЛЕНИНЦЕВ", "МАРКСА И ЭНГЕЛЬСА;КАРЛА МАРКСА И ФРИДРИХА ЭНГЕЛЬСА", "БАКИНСКИХ КОМИССАРОВ;БАК.КОМИССАРОВ;Б.КОМИССАРОВ", "САККО И ВАНЦЕТТИ", "СЕРП И МОЛОТ", "ЗАВОДА СЕРП И МОЛОТ", "ШАРЛЯ ДЕ ГОЛЛЯ;ДЕ ГОЛЛЯ", "МИНИНА И ПОЖАРСКОГО", "ХО ШИ МИНА;ХОШИМИНА", "ЗОИ И АЛЕКСАНДРА КОСМОДЕМЬЯНСКИХ;З.И А.КОСМОДЕМЬЯНСКИХ;З.А.КОСМОДЕМЬЯНСКИХ", "АРМАНД;ИНЕССЫ АРМАНД", "МИРА", "СВОБОДЫ", "РИМСКОГО-КОРСАКОВА", "ПЕТРА И ПАВЛА"}) 
            {
                string[] pp = s.Split(';');
                t = new Pullenti.Ner.Core.Termin(pp[0]) { Tag = StreetItemType.StdName, IgnoreTermsOrder = true };
                for (int kk = 1; kk < pp.Length; kk++) 
                {
                    if (pp[kk].IndexOf('.') > 0) 
                        t.AddAbridge(pp[kk]);
                    else 
                        t.AddVariant(pp[kk], false);
                }
                m_Ontology.Add(t);
            }
            foreach (string s in Pullenti.Ner.Geo.Internal.NameToken.StandardNames) 
            {
                string[] pp = s.Split(';');
                t = new Pullenti.Ner.Core.Termin(pp[0]) { Tag = StreetItemType.StdName, IgnoreTermsOrder = true };
                for (int kk = 1; kk < pp.Length; kk++) 
                {
                    if (pp[kk].IndexOf('.') > 0) 
                        t.AddAbridge(pp[kk]);
                    else if (t.Acronym == null && (pp[kk].Length < 4)) 
                        t.Acronym = pp[kk];
                    else 
                        t.AddVariant(pp[kk], false);
                }
                m_Ontology.Add(t);
            }
            foreach (string s in new string[] {"МАРТА", "МАЯ", "ОКТЯБРЯ", "НОЯБРЯ", "БЕРЕЗНЯ", "ТРАВНЯ", "ЖОВТНЯ", "ЛИСТОПАДА", "ДОРОЖКА", "ЛУЧ", "НАДЕЛ", "ПОЛЕ", "СКЛОН"}) 
            {
                m_Ontology.Add(new Pullenti.Ner.Core.Termin(s) { Tag = StreetItemType.StdName });
            }
            foreach (string s in new string[] {"МАРШАЛА", "ГЕНЕРАЛА", "ГЕНЕРАЛ-МАЙОРА", "ГЕНЕРАЛ-ЛЕЙТЕНАНТА", "ГЕНЕРАЛ-ПОЛКОВНИКА", "АДМИРАЛА", "КОНТРАДМИРАЛА", "КОСМОНАВТА", "ЛЕТЧИКА", "ПОГРАНИЧНИКА", "ПУТЕШЕСТВЕННИКА", "ПАРТИЗАНА", "АТАМАНА", "ТАНКИСТА", "АВИАКОНСТРУКТОРА", "АРХИТЕКТОРА", "ГЛАВНОГО АРХИТЕКТОРА", "СКУЛЬПТОРА", "ХУДОЖНИКА", "КОНСТРУКТОРА", "ГЛАВНОГО КОНСТРУКТОРА", "АКАДЕМИКА", "ПРОФЕССОРА", "КОМПОЗИТОРА", "ПИСАТЕЛЯ", "ПОЭТА", "ДИРИЖЕРА", "ГЕРОЯ", "БРАТЬЕВ", "ЛЕЙТЕНАНТА", "СТАРШЕГО ЛЕЙТЕНАНТА", "КАПИТАНА", "КАПИТАНА-ЛЕЙТЕНАНТА", "МАЙОРА", "ПОДПОЛКОВНИКА", "ПОЛКОВНИКА", "СЕРЖАНТА", "МЛАДШЕГО СЕРЖАНТА", "СТАРШЕГО СЕРЖАНТА", "ЕФРЕЙТОРА", "СТАРШИНЫ", "ПРАПОРЩИКА", "СТАРШЕГО ПРАПОРЩИКА", "ПОЛИТРУКА", "ПОЛИЦИИ", "МИЛИЦИИ", "ГВАРДИИ", "АРМИИ", "МИТРОПОЛИТА", "ПАТРИАРХА", "ИЕРЕЯ", "ПРОТОИЕРЕЯ", "МОНАХА", "СВЯТОГО", "СВЯТИТЕЛЯ"}) 
            {
                m_StdOntMisc.Add(new Pullenti.Ner.Core.Termin(s));
                t = new Pullenti.Ner.Core.Termin(s) { Tag = StreetItemType.StdPartOfName };
                if (s == "СВЯТОГО" || s == "СВЯТИТЕЛЯ") 
                {
                    t.AddAbridge("СВ.");
                    t.AddAbridge("СВЯТ.");
                }
                else 
                {
                    t.AddAllAbridges(0, 0, 2);
                    t.AddAllAbridges(2, 5, 0);
                    if (s == "ПРОФЕССОРА") 
                        t.AddVariant("ПРОФЕСОРА", false);
                }
                m_Ontology.Add(t);
            }
            foreach (string s in new string[] {"МАРШАЛА", "ГЕНЕРАЛА", "ГЕНЕРАЛ-МАЙОРА", "ГЕНЕРАЛ-ЛЕЙТЕНАНТА", "ГЕНЕРАЛ-ПОЛКОВНИКА", "АДМІРАЛА", "КОНТРАДМІРАЛА", "КОСМОНАВТА", "ЛЬОТЧИКА", " ПРИКОРДОННИКА", " МАНДРІВНИКА", "ПАРТИЗАНА", "ОТАМАНА", "ТАНКІСТА", "АВІАКОНСТРУКТОРА", "АРХІТЕКТОРА", "СКУЛЬПТОРА", "ХУДОЖНИКА", "КОНСТРУКТОРА", "АКАДЕМІКА", "ПРОФЕСОРА", "КОМПОЗИТОРА", "ПИСЬМЕННИКА", "ПОЕТА", "ДИРИГЕНТА", "ГЕРОЯ", "ЛЕЙТЕНАНТА", "КАПІТАНА", "КАПІТАНА-ЛЕЙТЕНАНТА", "МАЙОРА", "ПІДПОЛКОВНИКА", "ПОЛКОВНИКА", "СЕРЖАНТА", "ЄФРЕЙТОРА", " СТАРШИНИ", " ПРАПОРЩИКА", "ПОЛІТРУКА", "ПОЛІЦІЇ", "МІЛІЦІЇ", "ГВАРДІЇ", "АРМІЇ", "МИТРОПОЛИТА", "ПАТРІАРХА", "ІЄРЕЯ", "ПРОТОІЄРЕЯ", "ЧЕНЦЯ", "СВЯТОГО", "СВЯТИТЕЛЯ"}) 
            {
                m_StdOntMisc.Add(new Pullenti.Ner.Core.Termin(s));
                t = new Pullenti.Ner.Core.Termin(s) { Tag = StreetItemType.StdPartOfName, Lang = Pullenti.Morph.MorphLang.UA };
                if (s == "СВЯТОГО" || s == "СВЯТИТЕЛЯ") 
                {
                    t.AddAbridge("СВ.");
                    t.AddAbridge("СВЯТ.");
                }
                else 
                {
                    t.AddAllAbridges(0, 0, 2);
                    t.AddAllAbridges(2, 5, 0);
                    t.AddAbridge("ГЛ." + s);
                    t.AddAbridge("ГЛАВ." + s);
                }
                m_Ontology.Add(t);
            }
            t = new Pullenti.Ner.Core.Termin("ЛЕНИНСКИЕ ГОРЫ") { Tag = StreetItemType.Fix };
            m_Ontology.Add(t);
            foreach (string s in new string[] {"КРАСНЫЙ", "СОВЕТСТКИЙ", "ЛЕНИНСКИЙ"}) 
            {
                m_StdAdj.Add(new Pullenti.Ner.Core.Termin(s));
            }
        }
        public static Pullenti.Ner.Token CheckStdName(Pullenti.Ner.Token t)
        {
            if (t == null) 
                return null;
            if (m_StdAdj.TryParse(t, Pullenti.Ner.Core.TerminParseAttr.No) != null) 
                return t;
            Pullenti.Ner.Core.TerminToken tok = m_Ontology.TryParse(t, Pullenti.Ner.Core.TerminParseAttr.No);
            if (tok == null) 
                return null;
            if (((StreetItemType)tok.Termin.Tag) == StreetItemType.StdName) 
                return tok.EndToken;
            return null;
        }
        public static bool CheckKeyword(Pullenti.Ner.Token t)
        {
            if (t == null) 
                return false;
            Pullenti.Ner.Core.TerminToken tok = m_Ontology.TryParse(t, Pullenti.Ner.Core.TerminParseAttr.No);
            if (tok == null) 
                return false;
            return ((StreetItemType)tok.Termin.Tag) == StreetItemType.Noun;
        }
        public static bool CheckOnto(Pullenti.Ner.Token t)
        {
            if (t == null) 
                return false;
            Pullenti.Ner.Core.TerminToken tok = m_OntologyEx.TryParse(t, Pullenti.Ner.Core.TerminParseAttr.No);
            if (tok == null) 
                return false;
            return true;
        }
        internal static Pullenti.Ner.Core.TerminCollection m_Ontology;
        internal static Pullenti.Ner.Core.TerminCollection m_OntologyEx;
        static Pullenti.Ner.Core.TerminCollection m_StdOntMisc;
        static Pullenti.Ner.Core.TerminCollection m_StdAdj;
        static Pullenti.Ner.Core.Termin m_Prospect;
        static Pullenti.Ner.Core.Termin m_Metro;
        static Pullenti.Ner.Core.Termin m_Road;
        static Pullenti.Ner.Core.Termin m_Block;
        static string[] m_RegTails = new string[] {"ГОРОДОК", "РАЙОН", "МАССИВ", "МАСИВ", "КОМПЛЕКС", "ЗОНА", "КВАРТАЛ", "ОТДЕЛЕНИЕ", "ПАРК", "МЕСТНОСТЬ", "РАЗЪЕЗД", "УРОЧИЩЕ", "САД", "МЕСТОРОЖДЕНИЕ"};
        public static bool _isRegion(string txt)
        {
            txt = txt.ToUpper();
            foreach (string v in m_RegTails) 
            {
                if (Pullenti.Morph.LanguageHelper.EndsWith(txt, v)) 
                    return true;
            }
            return false;
        }
        static string[] m_SpecTails = new string[] {"БУДКА", "КАЗАРМА"};
        public static bool _isSpec(string txt)
        {
            txt = txt.ToUpper();
            foreach (string v in m_SpecTails) 
            {
                if (Pullenti.Morph.LanguageHelper.EndsWith(txt, v)) 
                    return true;
            }
            return false;
        }
        public StreetItemToken(Pullenti.Ner.Token begin, Pullenti.Ner.Token end) : base(begin, end, null)
        {
        }
        public StreetItemType Typ;
        public StreetItemType AltTyp;
        public Pullenti.Ner.Core.Termin Termin;
        public Pullenti.Ner.Core.Termin AltTermin;
        public Pullenti.Ner.Address.StreetReferent ExistStreet;
        public Pullenti.Ner.NumberSpellingType NumberType = Pullenti.Ner.NumberSpellingType.Undefined;
        public bool NumberHasPrefix;
        public bool IsNumberKm;
        public string Value;
        public string AltValue;
        public string AltValue2;
        public string Misc;
        public bool IsAbridge;
        public bool IsInDictionary;
        public bool IsInBrackets;
        public bool HasStdSuffix;
        public int NounIsDoubtCoef;
        public bool NounCanBeName;
        public bool IsRoadName;
        public StreetItemToken StdAdjVersion;
        public StreetItemToken NextItem;
        public bool IsRoad
        {
            get
            {
                if (Termin == null) 
                    return false;
                if ((Termin.CanonicText == "АВТОДОРОГА" || Termin.CanonicText == "ШОССЕ" || Termin.CanonicText == "ТРАКТ") || Termin.CanonicText == "АВТОШЛЯХ" || Termin.CanonicText == "ШОСЕ") 
                    return true;
                return false;
            }
        }
        public bool IsRailway;
        internal Pullenti.Ner.Geo.Internal.Condition Cond;
        internal bool NoGeoInThisToken;
        internal Pullenti.Ner.Geo.Internal.OrgItemToken Org;
        internal AddressItemToken OrtoTerr;
        internal Pullenti.Ner.Geo.GeoReferent City;
        public StreetItemToken Clone()
        {
            StreetItemToken res = new StreetItemToken(BeginToken, EndToken);
            res.Morph = Morph;
            res.Typ = Typ;
            res.AltTyp = AltTyp;
            res.Termin = Termin;
            res.AltTermin = AltTermin;
            res.Value = Value;
            res.AltValue = AltValue;
            res.AltValue2 = AltValue2;
            res.IsRailway = IsRailway;
            res.IsRoadName = IsRoadName;
            res.NounCanBeName = NounCanBeName;
            res.NounIsDoubtCoef = NounIsDoubtCoef;
            res.HasStdSuffix = HasStdSuffix;
            res.IsInBrackets = IsInBrackets;
            res.IsAbridge = IsAbridge;
            res.IsInDictionary = IsInDictionary;
            res.ExistStreet = ExistStreet;
            res.Misc = Misc;
            res.NumberType = NumberType;
            res.NumberHasPrefix = NumberHasPrefix;
            res.IsNumberKm = IsNumberKm;
            res.Cond = Cond;
            res.Org = Org;
            if (OrtoTerr != null) 
                res.OrtoTerr = OrtoTerr.Clone();
            res.City = City;
            if (StdAdjVersion != null) 
                res.StdAdjVersion = StdAdjVersion.Clone();
            if (NextItem != null) 
                res.NextItem = NextItem.Clone();
            return res;
        }
        public override string ToString()
        {
            StringBuilder res = new StringBuilder();
            res.AppendFormat("{0}", Typ.ToString());
            if (Value != null) 
            {
                res.AppendFormat(" {0}", Value);
                if (AltValue != null) 
                    res.AppendFormat("/{0}", AltValue);
                if (IsNumberKm) 
                    res.Append("км");
            }
            if (Misc != null) 
                res.AppendFormat(" <{0}>", Misc);
            if (ExistStreet != null) 
                res.AppendFormat(" {0}", ExistStreet.ToString());
            if (Termin != null) 
            {
                res.AppendFormat(" {0}", Termin.ToString());
                if (AltTermin != null) 
                    res.AppendFormat("/{0}", AltTermin.ToString());
            }
            else 
                res.AppendFormat(" {0}", base.ToString());
            if (Org != null) 
                res.AppendFormat(" Org: {0}", Org);
            if (IsAbridge) 
                res.Append(" (?)");
            if (OrtoTerr != null) 
                res.AppendFormat(" TERR: {0}", OrtoTerr);
            if (StdAdjVersion != null) 
                res.AppendFormat(" + (?) {0}", StdAdjVersion.ToString());
            if (NextItem != null) 
                res.AppendFormat(" + {0}", NextItem.ToString());
            return res.ToString();
        }
        bool _isSurname()
        {
            if (Typ != StreetItemType.Name) 
                return false;
            if (!(EndToken is Pullenti.Ner.TextToken)) 
                return false;
            string nam = (EndToken as Pullenti.Ner.TextToken).Term;
            if (nam.Length > 4) 
            {
                if (Pullenti.Morph.LanguageHelper.EndsWithEx(nam, "А", "Я", "КО", "ЧУКА")) 
                {
                    if (!Pullenti.Morph.LanguageHelper.EndsWithEx(nam, "АЯ", "ЯЯ", null, null)) 
                    {
                        Pullenti.Morph.MorphClass mc = EndToken.GetMorphClassInDictionary();
                        if (!mc.IsNoun) 
                            return true;
                    }
                }
            }
            return false;
        }
        public static bool SpeedRegime = false;
        internal static void PrepareAllData(Pullenti.Ner.Token t0)
        {
            if (!SpeedRegime) 
                return;
            Pullenti.Ner.Geo.Internal.GeoAnalyzerData ad = Pullenti.Ner.Geo.GeoAnalyzer.GetData(t0);
            if (ad == null) 
                return;
            ad.SRegime = false;
            for (Pullenti.Ner.Token t = t0; t != null; t = t.Next) 
            {
                Pullenti.Ner.Geo.Internal.GeoTokenData d = t.Tag as Pullenti.Ner.Geo.Internal.GeoTokenData;
                StreetItemToken prev = null;
                int kk = 0;
                for (Pullenti.Ner.Token tt = t.Previous; tt != null && (kk < 10); tt = tt.Previous,kk++) 
                {
                    Pullenti.Ner.Geo.Internal.GeoTokenData dd = tt.Tag as Pullenti.Ner.Geo.Internal.GeoTokenData;
                    if (dd == null || dd.Street == null) 
                        continue;
                    if (dd.Street.EndToken.Next == t) 
                        prev = dd.Street;
                    if (t.Previous != null && t.Previous.IsHiphen && dd.Street.EndToken.Next == t.Previous) 
                        prev = dd.Street;
                }
                StreetItemToken str = TryParse(t, prev, false, ad);
                if (str != null) 
                {
                    if (d == null) 
                        d = new Pullenti.Ner.Geo.Internal.GeoTokenData(t);
                    d.Street = str;
                    if (str.NoGeoInThisToken) 
                    {
                        if (((prev != null && prev.Typ == StreetItemType.Noun)) || StreetItemToken.CheckKeyword(str.EndToken.Next)) 
                        {
                            for (Pullenti.Ner.Token tt = str.BeginToken; tt != null && tt.EndChar <= str.EndChar; tt = tt.Next) 
                            {
                                Pullenti.Ner.Geo.Internal.GeoTokenData dd = tt.Tag as Pullenti.Ner.Geo.Internal.GeoTokenData;
                                if (dd == null) 
                                    dd = new Pullenti.Ner.Geo.Internal.GeoTokenData(tt);
                                dd.NoGeo = true;
                            }
                        }
                    }
                    if ((prev != null && prev.Typ == StreetItemType.Noun && str.Typ == StreetItemType.Noun) && str.Termin == prev.Termin) 
                        prev.EndToken = str.EndToken;
                }
            }
            ad.SRegime = true;
        }
        public static StreetItemToken TryParse(Pullenti.Ner.Token t, StreetItemToken prev = null, bool inSearch = false, Pullenti.Ner.Geo.Internal.GeoAnalyzerData ad = null)
        {
            if (t == null) 
                return null;
            if ((t is Pullenti.Ner.TextToken) && t.LengthChar == 1 && t.IsCharOf(",.:")) 
                return null;
            if (ad == null) 
                ad = Pullenti.Ner.Geo.GeoAnalyzer.GetData(t) as Pullenti.Ner.Geo.Internal.GeoAnalyzerData;
            if (ad == null) 
                return null;
            if ((SpeedRegime && ((ad.SRegime || ad.AllRegime)) && !inSearch) && !(t is Pullenti.Ner.ReferentToken)) 
            {
                if ((t is Pullenti.Ner.TextToken) && t.IsChar('м')) 
                {
                }
                else 
                {
                    Pullenti.Ner.Geo.Internal.GeoTokenData d = t.Tag as Pullenti.Ner.Geo.Internal.GeoTokenData;
                    if (d == null) 
                        return null;
                    if (d.Street != null) 
                    {
                        if (d.Street.Cond == null) 
                            return d.Street;
                        if (d.Street.Cond.Check()) 
                            return d.Street;
                        return null;
                    }
                    if (d.Org != null) 
                        return new StreetItemToken(t, d.Org.EndToken) { Typ = StreetItemType.Fix, Org = d.Org };
                    return null;
                }
            }
            if (ad.SLevel > 3) 
                return null;
            ad.SLevel++;
            StreetItemToken res = _tryParse(t, false, prev, inSearch);
            if (res != null && res.Typ != StreetItemType.Noun) 
            {
                if (res.Typ == StreetItemType.Name && res.BeginToken == res.EndToken && (res.BeginToken is Pullenti.Ner.TextToken)) 
                {
                    Pullenti.Ner.TextToken tt2 = res.BeginToken as Pullenti.Ner.TextToken;
                    if (tt2.Term == "ИЖС" || tt2.Term == "ЛПХ" || tt2.Term == "ДУБЛЬ") 
                    {
                        ad.SLevel--;
                        return null;
                    }
                    AddressItemToken ait = AddressItemToken.TryParsePureItem(t, null, null);
                    if ((ait != null && ait.Typ == AddressItemType.House && ait.Value != null) && ait.Value != "0") 
                    {
                        ad.SLevel--;
                        return null;
                    }
                }
                Pullenti.Ner.TextToken tt = res.EndToken.Next as Pullenti.Ner.TextToken;
                if (tt != null && res.Typ == StreetItemType.Number && tt.IsValue("ОТДЕЛЕНИЕ", null)) 
                {
                    res.EndToken = tt;
                    res.NumberHasPrefix = true;
                }
                else if (tt != null && res.Typ == StreetItemType.Number && tt.IsChar('+')) 
                {
                    StreetItemToken res2 = _tryParse(tt.Next, false, prev, inSearch);
                    if (res2 != null && res2.Typ == StreetItemType.Number) 
                    {
                        res.EndToken = res2.EndToken;
                        res.Value = string.Format("{0}+{1}", res.Value, res2.Value);
                    }
                }
                else if (tt != null && tt.IsChar('(')) 
                {
                    if (res.Value == null) 
                        res.Value = Pullenti.Ner.Core.MiscHelper.GetTextValue(res.BeginToken, res.EndToken, Pullenti.Ner.Core.GetTextAttr.No);
                    AddressItemToken ait = AddressItemToken.TryParse(tt.Next, false, null, null);
                    if ((ait != null && ait.Typ == AddressItemType.Street && ait.EndToken.Next != null) && ait.EndToken.Next.IsChar(')')) 
                    {
                        res.OrtoTerr = ait.Clone();
                        res.OrtoTerr.EndToken = ait.EndToken.Next;
                        res.EndToken = res.OrtoTerr.EndToken;
                    }
                    else 
                    {
                        StreetItemToken sit = StreetItemToken.TryParse(tt.Next, null, false, null);
                        if ((sit != null && ((sit.Typ == StreetItemType.Name || sit.Typ == StreetItemType.StdName)) && sit.EndToken.Next != null) && sit.EndToken.Next.IsChar(')')) 
                        {
                            ait = new AddressItemToken(AddressItemType.Street, tt.Next, sit.EndToken);
                            Pullenti.Ner.Address.StreetReferent stre = new Pullenti.Ner.Address.StreetReferent();
                            stre.Kind = Pullenti.Ner.Address.StreetKind.Area;
                            stre.AddTyp("территория");
                            stre.AddName(sit);
                            ait.Referent = stre;
                            res.OrtoTerr = ait;
                            res.EndToken = sit.EndToken.Next;
                        }
                    }
                }
                if (res.BeginToken == res.EndToken && ((res.Typ == StreetItemType.Name || res.Typ == StreetItemType.StdName || res.Typ == StreetItemType.StdPartOfName))) 
                {
                    Pullenti.Ner.Token end = Pullenti.Ner.Geo.Internal.MiscLocationHelper.CheckNameLong(res);
                    if ((end is Pullenti.Ner.ReferentToken) && (end.GetReferent() is Pullenti.Ner.Date.DateReferent)) 
                        end = null;
                    if (end != null && CheckKeyword(end)) 
                        end = null;
                    if (end != null) 
                    {
                        res.EndToken = end;
                        if (res.Value == null) 
                            res.Value = Pullenti.Ner.Core.MiscHelper.GetTextValue(res.BeginToken, end, Pullenti.Ner.Core.GetTextAttr.No);
                        else 
                        {
                            res.Value = string.Format("{0} {1}", res.Value, Pullenti.Ner.Core.MiscHelper.GetTextValue(res.BeginToken.Next, end, Pullenti.Ner.Core.GetTextAttr.No));
                            if (res.AltValue != null) 
                                res.AltValue = string.Format("{0} {1}", res.AltValue, Pullenti.Ner.Core.MiscHelper.GetTextValue(res.BeginToken.Next, end, Pullenti.Ner.Core.GetTextAttr.No));
                        }
                        if (res.BeginToken.Next == res.EndToken) 
                        {
                            Pullenti.Morph.MorphClass mc = res.BeginToken.GetMorphClassInDictionary();
                            Pullenti.Morph.MorphClass mc1 = res.EndToken.GetMorphClassInDictionary();
                            if (((mc.IsProperName && !res.BeginToken.IsValue("СЛАВА", null))) || mc1.IsProperSurname) 
                            {
                                res.AltValue2 = res.AltValue;
                                res.AltValue = Pullenti.Ner.Core.MiscHelper.GetTextValue(end, end, Pullenti.Ner.Core.GetTextAttr.No);
                            }
                            else if (mc.IsProperSurname && mc1.IsProperName) 
                            {
                                res.AltValue2 = res.AltValue;
                                res.AltValue = Pullenti.Ner.Core.MiscHelper.GetTextValue(res.BeginToken, res.BeginToken, Pullenti.Ner.Core.GetTextAttr.No);
                            }
                        }
                    }
                }
            }
            if ((res != null && res.Typ == StreetItemType.Number && prev != null) && prev.Typ == StreetItemType.Noun && prev.Termin.CanonicText == "РЯД") 
            {
                if (res.EndToken.Next != null && ((res.EndToken.Next.IsValue("ЛИНИЯ", null) || res.EndToken.Next.IsValue("БЛОК", null)))) 
                {
                    StreetItemToken next = _tryParse(res.EndToken.Next.Next, true, prev, inSearch);
                    if (next != null && next.Typ == StreetItemType.Number && !string.IsNullOrEmpty(next.Value)) 
                    {
                        if (char.IsLetter(next.Value[0])) 
                            res.Value += next.Value;
                        else 
                            res.Value = string.Format("{0}/{1}", res.Value, next.Value);
                        res.EndToken = next.EndToken;
                    }
                }
            }
            if ((res != null && res.Typ == StreetItemType.Number && prev != null) && prev.Typ == StreetItemType.Noun && ((prev.Termin.CanonicText == "БЛОК" || prev.Termin.CanonicText == "ЛИНИЯ"))) 
            {
                if (res.EndToken.Next != null && res.EndToken.Next.IsValue("РЯД", null)) 
                {
                    StreetItemToken next = _tryParse(res.EndToken.Next.Next, true, prev, inSearch);
                    if (next != null && next.Typ == StreetItemType.Number && !string.IsNullOrEmpty(next.Value)) 
                    {
                        Pullenti.Ner.Core.TerminToken tok = m_Ontology.TryParse(res.EndToken.Next, Pullenti.Ner.Core.TerminParseAttr.No);
                        if (tok != null && tok.Termin.CanonicText == "РЯД") 
                            prev.Termin = tok.Termin;
                        if (char.IsDigit(next.Value[0]) == char.IsDigit(res.Value[res.Value.Length - 1])) 
                            res.Value = string.Format("{0}/{1}", next.Value, res.Value);
                        else 
                            res.Value = string.Format("{0}{1}", next.Value, res.Value);
                        res.EndToken = next.EndToken;
                    }
                }
            }
            if (res != null && res.IsRoad) 
            {
                for (Pullenti.Ner.Token tt = res.EndToken.Next; tt != null; tt = tt.Next) 
                {
                    Pullenti.Ner.Core.NounPhraseToken npt = Pullenti.Ner.Core.NounPhraseHelper.TryParse(tt, Pullenti.Ner.Core.NounPhraseParseAttr.No, 0, null);
                    if (npt != null) 
                    {
                        if (npt.EndToken.IsValue("ПОЛЬЗОВАНИЕ", null) || npt.EndToken.IsValue("ЗНАЧЕНИЕ", null)) 
                        {
                            res.EndToken = (tt = npt.EndToken);
                            continue;
                        }
                    }
                    break;
                }
            }
            if ((res == null && t.IsChar('(') && Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(t)) && m_Ontology.TryParse(t.Next, Pullenti.Ner.Core.TerminParseAttr.No) != null) 
            {
                StreetItemToken next = TryParse(t.Next, null, false, null);
                if ((next != null && next.Typ == StreetItemType.Noun && next.EndToken.Next != null) && next.EndToken.Next.IsChar(')')) 
                {
                    next.BeginToken = t;
                    next.EndToken = next.EndToken.Next;
                    res = next;
                }
            }
            ad.SLevel--;
            return res;
        }
        public static StreetItemToken _tryParse(Pullenti.Ner.Token t, bool ignoreOnto, StreetItemToken prev, bool inSearch)
        {
            if (t == null) 
                return null;
            if (prev != null && prev.IsRoad) 
            {
                List<StreetItemToken> res1 = TryParseSpec(t, prev);
                if (res1 != null && res1[0].Typ == StreetItemType.Name) 
                    return res1[0];
            }
            if ((prev == null && t.Next != null && t.Next.IsHiphen) && Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(t)) 
            {
                List<StreetItemToken> res1 = TryParseSpec(t, prev);
                if (res1 != null && res1[0].Typ == StreetItemType.Name) 
                    return res1[0];
            }
            if (t.IsValue("ТЕР", null)) 
            {
            }
            if ((t.IsValue("А", null) || t.IsValue("АД", null) || t.IsValue("АВТ", null)) || t.IsValue("АВТОДОР", null)) 
            {
                Pullenti.Ner.Token tt1 = t;
                if (t.IsValue("А", null)) 
                {
                    tt1 = t.Next;
                    if (tt1 != null && tt1.IsCharOf("\\/")) 
                        tt1 = tt1.Next;
                    if (tt1 != null && ((tt1.IsValue("Д", null) || tt1.IsValue("М", null)))) 
                    {
                    }
                    else 
                        tt1 = null;
                }
                else if (tt1.Next != null && tt1.Next.IsChar('.')) 
                    tt1 = tt1.Next;
                if (tt1 != null) 
                {
                    StreetItemToken res = new StreetItemToken(t, tt1) { Typ = StreetItemType.Noun, Termin = m_Road };
                    if (prev != null && ((prev.IsRoadName || prev.IsRoad))) 
                        return res;
                    StreetItemToken next = TryParse(tt1.Next, res, false, null);
                    if (next != null && next.IsRoadName) 
                        return res;
                    if (t.Previous != null) 
                    {
                        if (t.Previous.IsValue("КМ", null) || t.Previous.IsValue("КИЛОМЕТР", null)) 
                            return res;
                    }
                }
            }
            if ((((t.IsValue("Ж", null) || t.IsValue("ЖЕЛ", null))) && t.Next != null && t.Next.IsCharOf("\\/")) && (t.Next.Next is Pullenti.Ner.TextToken) && t.Next.Next.IsValue("ДОРОЖНЫЙ", null)) 
                return new StreetItemToken(t, t.Next.Next) { Typ = StreetItemType.Name, Value = "ЖЕЛЕЗНО" + (t.Next.Next as Pullenti.Ner.TextToken).Term };
            if ((((t.IsValue("ФЕДЕРАЛЬНЫЙ", null) || t.IsValue("ГОСУДАРСТВЕННЫЙ", null) || t.IsValue("АВТОМОБИЛЬНЫЙ", null)) || t.IsValue("ФЕД", null) || t.IsValue("ФЕДЕРАЛ", null)) || t.IsValue("ГОС", null) || t.IsValue("АВТО", null)) || t.IsValue("АВТОМОБ", null)) 
            {
                Pullenti.Ner.Token tt2 = t.Next;
                if (tt2 != null && tt2.IsChar('.')) 
                    tt2 = tt2.Next;
                Pullenti.Ner.Core.TerminToken tok2 = m_Ontology.TryParse(tt2, Pullenti.Ner.Core.TerminParseAttr.No);
                if (tok2 != null && tok2.Termin.CanonicText == "АВТОДОРОГА") 
                    return new StreetItemToken(t, tok2.EndToken) { Typ = StreetItemType.Noun, Termin = tok2.Termin };
            }
            if (t.IsHiphen && prev != null && prev.Typ == StreetItemType.Name) 
            {
                Pullenti.Ner.NumberToken num = Pullenti.Ner.Core.NumberHelper.TryParseRoman(t.Next);
                if (num != null) 
                    return new StreetItemToken(t, num.EndToken) { Typ = StreetItemType.Number, Value = num.Value, NumberHasPrefix = true };
            }
            Pullenti.Ner.Token t0 = t;
            Pullenti.Ner.Token tn = null;
            Pullenti.Ner.Geo.Internal.OrgItemToken org1 = Pullenti.Ner.Geo.Internal.OrgItemToken.TryParse(t, null);
            if (org1 != null) 
            {
                if (org1.IsGsk || !org1.IsDoubt || org1.HasTerrKeyword) 
                    return new StreetItemToken(t0, org1.EndToken) { Typ = StreetItemType.Fix, Org = org1 };
                if (Pullenti.Ner.Core.BracketHelper.IsBracket(t, true)) 
                {
                    StreetItemToken next = TryParse(t.Next, prev, false, null);
                    if (next != null) 
                    {
                        if (Pullenti.Ner.Core.BracketHelper.IsBracket(next.EndToken.Next, true)) 
                        {
                            if (next.Typ == StreetItemType.Name && next.Value == null) 
                                next.Value = Pullenti.Ner.Core.MiscHelper.GetTextValueOfMetaToken(next, Pullenti.Ner.Core.GetTextAttr.No);
                            next.BeginToken = t0;
                            next.EndToken = next.EndToken.Next;
                        }
                        return next;
                    }
                }
            }
            Pullenti.Ner.Geo.GeoReferent geo1 = t.GetReferent() as Pullenti.Ner.Geo.GeoReferent;
            if (geo1 != null && geo1.IsCity && Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(t)) 
            {
                foreach (string ty in geo1.Typs) 
                {
                    if ((ty == "поселок" || ty == "станция" || ty == "слобода") || ty == "хутор") 
                        return new StreetItemToken(t0, t) { Typ = StreetItemType.Fix, City = geo1 };
                }
            }
            if (t is Pullenti.Ner.TextToken) 
            {
                if (t.IsValue("ТЕРРИТОРИЯ", null) || (t as Pullenti.Ner.TextToken).Term == "ТЕР" || (t as Pullenti.Ner.TextToken).Term == "ТЕРР") 
                    return null;
            }
            Pullenti.Ner.Geo.Internal.NumToken nnn2 = Pullenti.Ner.Geo.Internal.NumToken.TryParse(t, Pullenti.Ner.Geo.Internal.GeoTokenType.Street);
            if (nnn2 != null && ((nnn2.HasPrefix || nnn2.IsCadasterNumber))) 
                return new StreetItemToken(t, nnn2.EndToken) { Value = nnn2.Value, Typ = StreetItemType.Number, NumberHasPrefix = true };
            if (prev != null) 
            {
            }
            bool hasNamed = false;
            if (t.IsValue("ИМЕНИ", "ІМЕНІ")) 
                tn = t;
            else if (t.IsValue("ПАМЯТИ", "ПАМЯТІ")) 
            {
                Pullenti.Ner.Geo.Internal.NameToken nam = Pullenti.Ner.Geo.Internal.NameToken.TryParse(t, Pullenti.Ner.Geo.Internal.GeoTokenType.Street, 0, false);
                if (nam != null && nam.EndToken != t && nam.Number == null) 
                {
                }
                else if (t.IsNewlineAfter || ((t.Next != null && t.Next.IsComma))) 
                {
                }
                else 
                    tn = t;
            }
            else if (t.IsValue("ИМ", null) || t.IsValue("ІМ", null)) 
            {
                tn = t;
                if (tn.Next != null && tn.Next.IsChar('.')) 
                    tn = tn.Next;
            }
            if (tn != null) 
            {
                if (tn.Next == null || tn.NewlinesAfterCount > 1) 
                    return null;
                t = tn.Next;
                tn = t;
                hasNamed = true;
            }
            if (t.IsValue("ДВАЖДЫ", null) || t.IsValue("ТРИЖДЫ", null) || t.IsValue("ЧЕТЫРЕЖДЫ", null)) 
            {
                if (t.Next != null) 
                    t = t.Next;
            }
            if (t.IsValue("ГЕРОЙ", null)) 
            {
                List<Pullenti.Ner.Geo.Internal.TerrItemToken> ters = Pullenti.Ner.Geo.Internal.TerrItemToken.TryParseList(t.Next, 3, null);
                if (ters != null && ters.Count > 0) 
                {
                    Pullenti.Ner.Token tt1 = null;
                    if (ters[0].OntoItem != null) 
                        tt1 = ters[0].EndToken.Next;
                    else if (ters[0].TerminItem != null && ters.Count > 1 && ters[1].OntoItem != null) 
                        tt1 = ters[1].EndToken.Next;
                    StreetItemToken nnn = TryParse(tt1, prev, inSearch, null);
                    if (nnn != null && nnn.Typ == StreetItemType.Name) 
                        return nnn;
                }
            }
            if (t.IsValue("НЕЗАВИСИМОСТЬ", null)) 
            {
                List<Pullenti.Ner.Geo.Internal.TerrItemToken> ters = Pullenti.Ner.Geo.Internal.TerrItemToken.TryParseList(t.Next, 3, null);
                if (ters != null && ters.Count > 0) 
                {
                    Pullenti.Ner.Geo.Internal.TerrItemToken tok2 = null;
                    if (ters[0].OntoItem != null) 
                        tok2 = ters[0];
                    else if (ters[0].TerminItem != null && ters.Count > 1 && ters[1].OntoItem != null) 
                        tok2 = ters[1];
                    if (tok2 != null) 
                    {
                        StreetItemToken res = new StreetItemToken(t, tok2.EndToken) { Typ = StreetItemType.Name };
                        res.Value = string.Format("НЕЗАВИСИМОСТИ {0}", tok2.OntoItem.CanonicText);
                        return res;
                    }
                }
            }
            if (t.IsValue("ЖУКОВА", null)) 
            {
            }
            if (t is Pullenti.Ner.ReferentToken) 
            {
                List<StreetItemToken> res1 = TryParseSpec(t, prev);
                if (res1 != null && ((res1.Count == 1 || res1[0].Typ == StreetItemType.Name))) 
                    return res1[0];
                if ((res1 != null && res1.Count == 2 && res1[0].Typ == StreetItemType.Number) && ((res1[1].Typ == StreetItemType.Name || res1[1].Typ == StreetItemType.StdName))) 
                {
                    res1[0].NextItem = res1[1];
                    return res1[0];
                }
            }
            Pullenti.Ner.NumberToken nt = Pullenti.Ner.Core.NumberHelper.TryParseAge(t);
            if (nt != null && nt.IntValue != null) 
                return new StreetItemToken(nt.BeginToken, nt.EndToken) { Typ = StreetItemType.Age, Value = nt.Value, NumberType = Pullenti.Ner.NumberSpellingType.Age };
            if ((((nt = t as Pullenti.Ner.NumberToken))) != null) 
            {
                if ((nt as Pullenti.Ner.NumberToken).IntValue == null) 
                {
                    if (prev != null && prev.Typ == StreetItemType.Noun) 
                    {
                    }
                    else 
                        return null;
                }
                else if ((nt as Pullenti.Ner.NumberToken).IntValue.Value == 0) 
                {
                    if (prev != null && prev.Typ == StreetItemType.Noun) 
                    {
                    }
                    else 
                    {
                        StreetItemToken next = TryParse(nt.Next, null, false, null);
                        if (next != null && next.Typ == StreetItemType.Noun) 
                        {
                        }
                        else 
                            return null;
                    }
                }
                StreetItemToken res = new StreetItemToken(nt, nt) { Typ = StreetItemType.Number, Value = nt.Value, NumberType = nt.Typ, Morph = nt.Morph };
                res.Value = nt.Value;
                if (prev != null && prev.Typ == StreetItemType.Noun && ((prev.Termin.CanonicText == "РЯД" || prev.Termin.CanonicText == "ЛИНИЯ"))) 
                {
                    AddressItemToken ait = AddressItemToken.TryParsePureItem(t, null, null);
                    if (ait != null && ait.Typ == AddressItemType.Number) 
                    {
                        res.Value = ait.Value;
                        res.EndToken = ait.EndToken;
                        return res;
                    }
                }
                Pullenti.Ner.Geo.Internal.NumToken nnn = Pullenti.Ner.Geo.Internal.NumToken.TryParse(t, Pullenti.Ner.Geo.Internal.GeoTokenType.Street);
                if (nnn != null) 
                {
                    res.Value = nnn.Value;
                    t = (res.EndToken = nnn.EndToken);
                }
                Pullenti.Ner.Core.NumberExToken nex = Pullenti.Ner.Core.NumberHelper.TryParseNumberWithPostfix(t);
                if (nex != null) 
                {
                    if (nex.ExTyp == Pullenti.Ner.Core.NumberExType.Kilometer) 
                    {
                        res.IsNumberKm = true;
                        res.EndToken = nex.EndToken;
                        Pullenti.Ner.Token tt2 = res.EndToken.Next;
                        bool hasBr = false;
                        while (tt2 != null) 
                        {
                            if (tt2.IsHiphen || tt2.IsChar('+')) 
                                tt2 = tt2.Next;
                            else if (tt2.IsChar('(')) 
                            {
                                hasBr = true;
                                tt2 = tt2.Next;
                            }
                            else 
                                break;
                        }
                        Pullenti.Ner.Core.NumberExToken nex2 = Pullenti.Ner.Core.NumberHelper.TryParseNumberWithPostfix(tt2);
                        if (nex2 != null && nex2.ExTyp == Pullenti.Ner.Core.NumberExType.Meter) 
                        {
                            res.EndToken = nex2.EndToken;
                            if (hasBr && res.EndToken.Next != null && res.EndToken.Next.IsChar(')')) 
                                res.EndToken = res.EndToken.Next;
                            string mm = Pullenti.Ner.Core.NumberHelper.DoubleToString(nex2.RealValue / 1000);
                            if (mm.StartsWith("0.")) 
                                res.Value += mm.Substring(1);
                        }
                        AddressItemToken ait = AddressItemToken.TryParsePureItem(t, null, null);
                        if (ait != null && ait.Typ == AddressItemType.Detail) 
                            return null;
                    }
                    else 
                    {
                        StreetItemToken nex2 = TryParse(res.EndToken.Next, null, false, null);
                        if (nex2 != null && nex2.Typ == StreetItemType.Noun && nex2.EndChar > nex.EndChar) 
                        {
                        }
                        else 
                            return null;
                    }
                }
                if (t.Next != null && t.Next.IsHiphen && (t.Next.Next is Pullenti.Ner.NumberToken)) 
                {
                    nex = Pullenti.Ner.Core.NumberHelper.TryParseNumberWithPostfix(t.Next.Next);
                    if (nex != null) 
                    {
                        if (nex.ExTyp == Pullenti.Ner.Core.NumberExType.Kilometer) 
                        {
                            res.IsNumberKm = true;
                            res.EndToken = nex.EndToken;
                            res.Value = string.Format("{0}-{1}", res.Value, nex.Value);
                        }
                        else 
                            return null;
                    }
                }
                AddressItemToken aaa = AddressItemToken.TryParsePureItem(t, null, null);
                if (aaa != null && aaa.Typ == AddressItemType.Number && aaa.EndChar > (t.EndChar + 1)) 
                {
                    StreetItemToken next = TryParse(res.EndToken.Next, null, false, null);
                    if (next != null && ((next.Typ == StreetItemType.Name || next.Typ == StreetItemType.StdName))) 
                    {
                    }
                    else if (prev != null && prev.Typ == StreetItemType.Noun && (((t.Next.IsHiphen || prev.Termin.CanonicText == "КВАРТАЛ" || prev.Termin.CanonicText == "ЛИНИЯ") || prev.Termin.CanonicText == "АЛЛЕЯ" || prev.Termin.CanonicText == "ДОРОГА"))) 
                    {
                        if (m_Ontology.TryParse(aaa.EndToken, Pullenti.Ner.Core.TerminParseAttr.No) != null) 
                        {
                        }
                        else 
                        {
                            res.EndToken = aaa.EndToken;
                            res.Value = aaa.Value;
                            res.NumberType = Pullenti.Ner.NumberSpellingType.Undefined;
                        }
                    }
                    else 
                        return null;
                }
                if (nt.Typ == Pullenti.Ner.NumberSpellingType.Words && nt.Morph.Class.IsAdjective) 
                {
                    Pullenti.Ner.Core.NounPhraseToken npt2 = Pullenti.Ner.Geo.Internal.MiscLocationHelper.TryParseNpt(t);
                    if (npt2 != null && npt2.EndChar > t.EndChar && npt2.Morph.Number != Pullenti.Morph.MorphNumber.Singular) 
                    {
                        if (t.Next != null && !t.Next.Chars.IsAllLower) 
                        {
                        }
                        else 
                            return null;
                    }
                }
                if (!res.IsNumberKm && prev != null && prev.BeginToken.IsValue("КИЛОМЕТР", null)) 
                    res.IsNumberKm = true;
                else if (prev != null && prev.Typ == StreetItemType.Noun && !res.IsWhitespaceAfter) 
                {
                    for (Pullenti.Ner.Token tt1 = res.EndToken.Next; tt1 != null; tt1 = tt1.Next) 
                    {
                        if (tt1.IsWhitespaceBefore) 
                            break;
                        if (tt1 is Pullenti.Ner.NumberToken) 
                        {
                            res.Value += (tt1 as Pullenti.Ner.NumberToken).Value;
                            res.EndToken = tt1;
                        }
                        else if (tt1.IsHiphen) 
                        {
                            res.Value += "-";
                            res.EndToken = tt1;
                        }
                        else if ((tt1 is Pullenti.Ner.TextToken) && tt1.Chars.IsLetter && tt1.LengthChar == 1) 
                        {
                            char ch = (tt1 as Pullenti.Ner.TextToken).Term[0];
                            char ch1 = Pullenti.Morph.LanguageHelper.GetCyrForLat(ch);
                            if (ch1 != 0) 
                                ch = ch1;
                            res.Value = string.Format("{0}{1}", res.Value, ch);
                            res.EndToken = tt1;
                        }
                        else 
                            break;
                    }
                }
                if (res.Value.EndsWith("-")) 
                    res.Value = res.Value.Substring(0, res.Value.Length - 1);
                if (res.EndToken.Next != null && ((res.EndToken.Next.IsValue("СЕКТОР", null) || res.EndToken.Next.IsValue("ЗОНА", null)))) 
                {
                    Pullenti.Ner.Token tt1 = res.EndToken.Next.Next;
                    if ((tt1 is Pullenti.Ner.TextToken) && tt1.LengthChar == 1 && tt1.Chars.IsLetter) 
                    {
                        char ch = (tt1 as Pullenti.Ner.TextToken).Term[0];
                        char ch1 = Pullenti.Morph.LanguageHelper.GetCyrForLat(ch);
                        if (ch1 != 0) 
                            ch = ch1;
                        res.Value = string.Format("{0}{1}", res.Value, ch);
                        res.EndToken = tt1;
                    }
                    else if (tt1 is Pullenti.Ner.NumberToken) 
                    {
                        res.Value = string.Format("{0}-{1}", res.Value, (tt1 as Pullenti.Ner.NumberToken).Value);
                        res.EndToken = tt1;
                    }
                }
                return res;
            }
            Pullenti.Ner.Token ntt = Pullenti.Ner.Core.MiscHelper.CheckNumberPrefix(t);
            if ((ntt != null && (ntt is Pullenti.Ner.NumberToken) && prev != null) && (ntt as Pullenti.Ner.NumberToken).IntValue != null) 
                return new StreetItemToken(t, ntt) { Typ = StreetItemType.Number, Value = (ntt as Pullenti.Ner.NumberToken).Value, NumberType = (ntt as Pullenti.Ner.NumberToken).Typ, NumberHasPrefix = true };
            StreetItemToken rrr = Pullenti.Ner.Geo.Internal.OrgItemToken.TryParseRailway(t);
            if (rrr != null) 
                return rrr;
            if ((t is Pullenti.Ner.ReferentToken) && (t as Pullenti.Ner.ReferentToken).BeginToken == (t as Pullenti.Ner.ReferentToken).EndToken && !t.Chars.IsAllLower) 
            {
                if (prev != null && prev.Typ == StreetItemType.Noun) 
                {
                    if (((prev.Morph.Number & Pullenti.Morph.MorphNumber.Plural)) == Pullenti.Morph.MorphNumber.Undefined) 
                        return new StreetItemToken(t, t) { Typ = StreetItemType.Name, Value = Pullenti.Ner.Core.MiscHelper.GetTextValueOfMetaToken(t as Pullenti.Ner.ReferentToken, Pullenti.Ner.Core.GetTextAttr.No) };
                }
            }
            if (t.IsValue("ЧАСТЬ", null) || t.IsValue("УГОЛ", null)) 
                return null;
            Pullenti.Ner.TextToken tt = t as Pullenti.Ner.TextToken;
            Pullenti.Ner.Core.NounPhraseToken npt = null;
            if (tt != null && tt.Morph.Class.IsAdjective) 
            {
                if (tt.Chars.IsCapitalUpper || Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(tt) || ((prev != null && prev.Typ == StreetItemType.Number && tt.IsValue("ТРАНСПОРТНЫЙ", null)))) 
                {
                    npt = Pullenti.Ner.Geo.Internal.MiscLocationHelper.TryParseNpt(tt);
                    if (npt != null && Pullenti.Ner.Core.MiscHelper.GetTextValueOfMetaToken(npt.Noun, Pullenti.Ner.Core.GetTextAttr.No).Contains("-")) 
                        npt = null;
                    else if (npt != null && npt.Adjectives.Count > 0 && ((npt.Adjectives[0].IsNewlineAfter || npt.Noun.IsNewlineBefore))) 
                        npt = null;
                    if (npt != null && AddressItemToken.TryParsePureItem(npt.EndToken, null, null) != null) 
                        npt = null;
                    Pullenti.Ner.Token tte = tt.Next;
                    if (npt != null && npt.Adjectives.Count == 1) 
                        tte = npt.EndToken;
                    if (tte != null) 
                    {
                        if (((((((((tte.IsValue("ВАЛ", null) || tte.IsValue("ПОЛЕ", null) || tte.IsValue("МАГИСТРАЛЬ", null)) || tte.IsValue("СПУСК", null) || tte.IsValue("ВЗВОЗ", null)) || tte.IsValue("РЯД", null) || tte.IsValue("СЛОБОДА", null)) || tte.IsValue("РОЩА", null) || tte.IsValue("ПРУД", null)) || tte.IsValue("СЪЕЗД", null) || tte.IsValue("КОЛЬЦО", null)) || tte.IsValue("МАГІСТРАЛЬ", null) || tte.IsValue("УЗВІЗ", null)) || tte.IsValue("ЛІНІЯ", null) || tte.IsValue("УЗВІЗ", null)) || tte.IsValue("ГАЙ", null) || tte.IsValue("СТАВОК", null)) || tte.IsValue("ЗЇЗД", null) || tte.IsValue("КІЛЬЦЕ", null)) 
                        {
                            StreetItemToken sit = new StreetItemToken(tt, tte) { HasStdSuffix = true };
                            sit.Typ = StreetItemType.Name;
                            if (npt == null || npt.Adjectives.Count == 0) 
                                sit.Value = Pullenti.Ner.Core.MiscHelper.GetTextValue(tt, tte, Pullenti.Ner.Core.GetTextAttr.No);
                            else if (npt.Morph.Case.IsGenitive) 
                            {
                                sit.Value = Pullenti.Ner.Core.MiscHelper.GetTextValue(tt, tte, Pullenti.Ner.Core.GetTextAttr.No);
                                sit.AltValue = npt.GetNormalCaseText(null, Pullenti.Morph.MorphNumber.Undefined, Pullenti.Morph.MorphGender.Undefined, false);
                            }
                            else 
                                sit.Value = npt.GetNormalCaseText(null, Pullenti.Morph.MorphNumber.Undefined, Pullenti.Morph.MorphGender.Undefined, false);
                            Pullenti.Ner.Core.TerminToken tok2 = m_Ontology.TryParse(tt, Pullenti.Ner.Core.TerminParseAttr.No);
                            if (tok2 != null && tok2.Termin != null && tok2.EndToken == tte) 
                                sit.Termin = tok2.Termin;
                            return sit;
                        }
                    }
                    if (npt != null && npt.BeginToken != npt.EndToken && npt.Adjectives.Count <= 1) 
                    {
                        Pullenti.Ner.Core.TerminToken oo = m_Ontology.TryParse(t, Pullenti.Ner.Core.TerminParseAttr.No);
                        if (oo != null && ((StreetItemType)oo.Termin.Tag) == StreetItemType.Noun) 
                            npt = null;
                    }
                    if (npt != null && npt.EndToken.IsValue("ВЕРХ", null)) 
                        npt = null;
                    if (npt != null) 
                    {
                        AddressItemToken ait = AddressItemToken.TryParsePureItem(t, null, null);
                        if (ait != null && ait.DetailType != Pullenti.Ner.Address.AddressDetailType.Undefined) 
                            npt = null;
                    }
                    if (npt != null && npt.BeginToken != npt.EndToken && npt.Adjectives.Count <= 1) 
                    {
                        Pullenti.Ner.Token tt1 = npt.EndToken.Next;
                        bool ok = Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(npt);
                        if (npt.IsNewlineAfter) 
                            ok = true;
                        else if (tt1 != null && tt1.IsComma) 
                        {
                            ok = true;
                            tt1 = tt1.Next;
                        }
                        StreetItemToken sti1 = TryParse(tt1, null, false, null);
                        if (sti1 != null && sti1.Typ == StreetItemType.Noun) 
                            ok = true;
                        else if (tt1 != null && tt1.IsHiphen && (tt1.Next is Pullenti.Ner.NumberToken)) 
                            ok = true;
                        else 
                        {
                            AddressItemToken ait = AddressItemToken.TryParsePureItem(tt1, null, null);
                            if (ait != null) 
                            {
                                if (ait.Typ == AddressItemType.House) 
                                    ok = true;
                                else if (ait.Typ == AddressItemType.Number) 
                                {
                                    AddressItemToken ait2 = AddressItemToken.TryParsePureItem(npt.EndToken, null, null);
                                    if (ait2 == null) 
                                        ok = true;
                                }
                            }
                        }
                        if (ok) 
                        {
                            sti1 = TryParse(npt.EndToken, null, false, null);
                            if (sti1 != null && sti1.Typ == StreetItemType.Noun) 
                                ok = sti1.NounIsDoubtCoef >= 2 && sti1.Termin.CanonicText != "КВАРТАЛ";
                            else 
                            {
                                Pullenti.Ner.Core.TerminToken tok2 = m_Ontology.TryParse(npt.EndToken, Pullenti.Ner.Core.TerminParseAttr.No);
                                if (tok2 != null) 
                                {
                                    StreetItemType typ = (StreetItemType)tok2.Termin.Tag;
                                    if (typ == StreetItemType.Noun || typ == StreetItemType.StdPartOfName || typ == StreetItemType.StdAdjective) 
                                        ok = false;
                                }
                            }
                        }
                        if (ok) 
                        {
                            StreetItemToken sit = new StreetItemToken(tt, npt.EndToken);
                            sit.Typ = StreetItemType.Name;
                            sit.Value = Pullenti.Ner.Core.MiscHelper.GetTextValue(tt, npt.EndToken, Pullenti.Ner.Core.GetTextAttr.No);
                            sit.AltValue = npt.GetNormalCaseText(null, Pullenti.Morph.MorphNumber.Undefined, Pullenti.Morph.MorphGender.Undefined, false);
                            return sit;
                        }
                    }
                }
            }
            if (tt != null && (tt.Next is Pullenti.Ner.TextToken) && ((tt.Next.Chars.IsCapitalUpper || Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(tt)))) 
            {
                if ((tt.IsValue("ВАЛ", null) || tt.IsValue("ПОЛЕ", null) || tt.IsValue("КОЛЬЦО", null)) || tt.IsValue("КІЛЬЦЕ", null)) 
                {
                    StreetItemToken sit = TryParse(tt.Next, null, false, null);
                    if (sit != null && sit.Typ == StreetItemType.Name) 
                    {
                        if (sit.Value != null) 
                            sit.Value = string.Format("{0} {1}", sit.Value, tt.GetNormalCaseText(null, Pullenti.Morph.MorphNumber.Undefined, Pullenti.Morph.MorphGender.Undefined, false));
                        else 
                            sit.Value = string.Format("{0} {1}", sit.GetSourceText().ToUpper(), tt.GetNormalCaseText(null, Pullenti.Morph.MorphNumber.Undefined, Pullenti.Morph.MorphGender.Undefined, false));
                        if (sit.AltValue != null) 
                            sit.AltValue = string.Format("{0} {1}", sit.AltValue, tt.GetNormalCaseText(null, Pullenti.Morph.MorphNumber.Undefined, Pullenti.Morph.MorphGender.Undefined, false));
                        sit.BeginToken = tt;
                        return sit;
                    }
                }
            }
            if (((tt != null && tt.LengthChar == 1 && tt.Chars.IsAllLower) && tt.Next != null && tt.Next.IsChar('.')) && tt.Kit.BaseLanguage.IsRu) 
            {
                if (tt.IsValue("М", null) || tt.IsValue("M", null)) 
                {
                    if (prev != null && prev.Typ == StreetItemType.Noun) 
                    {
                    }
                    else if (!inSearch) 
                    {
                        Pullenti.Ner.Core.TerminToken tok1 = m_Ontology.TryParse(tt, Pullenti.Ner.Core.TerminParseAttr.No);
                        if (tok1 != null && tok1.Termin.CanonicText == "МИКРОРАЙОН") 
                            return new StreetItemToken(tt, tok1.EndToken) { Termin = tok1.Termin, Typ = StreetItemType.Noun };
                        if (Pullenti.Ner.Geo.Internal.NameToken.CheckInitial(tt) != null || Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(tt)) 
                        {
                        }
                        else 
                            return new StreetItemToken(tt, tt.Next) { Termin = m_Metro, Typ = StreetItemType.Noun, IsAbridge = true };
                    }
                }
            }
            Pullenti.Ner.Core.IntOntologyToken ot = null;
            if (!Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(t)) 
            {
                if (t.Kit.Ontology != null && ot == null) 
                {
                    List<Pullenti.Ner.Core.IntOntologyToken> ots = t.Kit.Ontology.AttachToken(Pullenti.Ner.Address.AddressReferent.OBJ_TYPENAME, t);
                    if (ots != null) 
                        ot = ots[0];
                }
                if (ot != null && ot.BeginToken == ot.EndToken && ot.Morph.Class.IsAdjective) 
                {
                    Pullenti.Ner.Core.TerminToken tok0 = m_Ontology.TryParse(t, Pullenti.Ner.Core.TerminParseAttr.No);
                    if (tok0 != null) 
                    {
                        if (((StreetItemType)tok0.Termin.Tag) == StreetItemType.StdAdjective) 
                            ot = null;
                    }
                }
            }
            if (ot != null) 
            {
                StreetItemToken res0 = new StreetItemToken(ot.BeginToken, ot.EndToken) { Typ = StreetItemType.Name, ExistStreet = ot.Item.Referent as Pullenti.Ner.Address.StreetReferent, Morph = ot.Morph, IsInDictionary = true };
                return res0;
            }
            if (prev != null && prev.Typ == StreetItemType.Noun && prev.Termin.CanonicText == "ПРОЕЗД") 
            {
                if (t.IsValue("ПР", null)) 
                {
                    StreetItemToken res1 = new StreetItemToken(t, t) { Typ = StreetItemType.Name, Value = "ПРОЕКТИРУЕМЫЙ" };
                    if (t.Next != null && t.Next.IsChar('.')) 
                        res1.EndToken = t.Next;
                    return res1;
                }
            }
            Pullenti.Ner.Core.TerminToken tok = (ignoreOnto ? null : m_Ontology.TryParse(t, Pullenti.Ner.Core.TerminParseAttr.No));
            Pullenti.Ner.Core.TerminToken tokEx = (ignoreOnto ? null : m_OntologyEx.TryParse(t, Pullenti.Ner.Core.TerminParseAttr.No));
            if (tok == null) 
                tok = tokEx;
            else if (tokEx != null && tokEx.EndChar > tok.EndChar) 
                tok = tokEx;
            if (tok != null && (t is Pullenti.Ner.TextToken) && (((t as Pullenti.Ner.TextToken).Term == "ДОРОГАЯ" || (t as Pullenti.Ner.TextToken).Term == "ДОРОГОЙ"))) 
                tok = null;
            if ((tok != null && t.LengthChar == 1 && t.IsValue("Б", null)) && (t.Previous is Pullenti.Ner.NumberToken) && (t.Previous as Pullenti.Ner.NumberToken).Value == "26") 
                tok = null;
            if ((tok != null && tok.Termin.CanonicText == "БЛОК" && (t is Pullenti.Ner.TextToken)) && tok.EndToken == t) 
            {
                if ((t as Pullenti.Ner.TextToken).Term == "БЛОКА") 
                    tok = null;
                else if (t.Chars.IsAllLower) 
                {
                }
                else if (prev != null && prev.Typ == StreetItemType.Noun && prev.Termin.CanonicText == "РЯД") 
                {
                }
                else 
                {
                    AddressItemToken ait = AddressItemToken.TryParsePureItem(t.Next, null, null);
                    if (ait != null && ait.Typ == AddressItemType.Number) 
                    {
                    }
                    else 
                        tok = null;
                }
            }
            if (tok != null && tok.Termin.CanonicText == "СЕКЦИЯ") 
            {
                if (prev != null) 
                    tok = null;
            }
            if (tok != null && tok.BeginToken == tok.EndToken) 
            {
                if ((((StreetItemType)tok.Termin.Tag) == StreetItemType.Name || t.IsValue("ГАРАЖНО", null) || t.LengthChar == 1) || t.IsValue("СТ", null)) 
                {
                    Pullenti.Ner.Geo.Internal.OrgItemToken org = Pullenti.Ner.Geo.Internal.OrgItemToken.TryParse(t, null);
                    if (org != null) 
                    {
                        tok = null;
                        if (t.LengthChar < 3) 
                            return new StreetItemToken(t, org.EndToken) { Typ = StreetItemType.Fix, Org = org };
                    }
                }
                else if (((StreetItemType)tok.Termin.Tag) == StreetItemType.StdAdjective && (t is Pullenti.Ner.TextToken) && (t as Pullenti.Ner.TextToken).Term.EndsWith("О")) 
                    tok = null;
                else if (((StreetItemType)tok.Termin.Tag) == StreetItemType.Noun && t.IsValue("САД", null) && t.Previous != null) 
                {
                    if (t.Previous.IsValue("ДЕТСКИЙ", null)) 
                        tok = null;
                    else if (t.Previous.IsHiphen && t.Previous.Previous != null && t.Previous.Previous.IsValue("ЯСЛИ", null)) 
                        tok = null;
                }
            }
            if (tok != null && !ignoreOnto) 
            {
                if (((StreetItemType)tok.Termin.Tag) == StreetItemType.Number) 
                {
                    if ((tok.EndToken.Next is Pullenti.Ner.NumberToken) && (tok.EndToken.Next as Pullenti.Ner.NumberToken).IntValue != null) 
                        return new StreetItemToken(t, tok.EndToken.Next) { Typ = StreetItemType.Number, Value = (tok.EndToken.Next as Pullenti.Ner.NumberToken).Value, NumberType = (tok.EndToken.Next as Pullenti.Ner.NumberToken).Typ, NumberHasPrefix = true, Morph = tok.Morph };
                    return null;
                }
                if (tt == null) 
                    return null;
                bool abr = true;
                switch ((StreetItemType)tok.Termin.Tag)
                {
                    case StreetItemType.StdAdjective:
                        Pullenti.Ner.Token tt3 = Pullenti.Ner.Geo.Internal.NameToken.CheckInitial(tok.BeginToken);
                        if (tt3 != null) 
                        {
                            StreetItemToken next = TryParse(tt3, prev, inSearch, null);
                            if (next != null && next.Typ != StreetItemType.Noun) 
                            {
                                if (next.Value == null) 
                                    next.Value = Pullenti.Ner.Core.MiscHelper.GetTextValueOfMetaToken(next, Pullenti.Ner.Core.GetTextAttr.No);
                                next.BeginToken = t0;
                                return next;
                            }
                        }
                        if (tt.Chars.IsAllLower && prev == null && !inSearch) 
                        {
                            if (!Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(tok)) 
                                return null;
                        }
                        if (tt.IsValue(tok.Termin.CanonicText, null)) 
                            abr = false;
                        else if (tt.LengthChar == 1) 
                        {
                            if (!tt.IsWhitespaceBefore && !tt.Previous.IsCharOf(":,.")) 
                                break;
                            if (!tok.EndToken.IsChar('.')) 
                            {
                                bool oo2 = false;
                                if (!tt.Chars.IsAllUpper && !inSearch) 
                                {
                                    if ((tt.IsCharOf("мб") && (tt.Previous is Pullenti.Ner.TextToken) && tt.Previous.Chars.IsCapitalUpper) && AddressItemToken.CheckHouseAfter(tt.Next, false, false)) 
                                        oo2 = true;
                                    else 
                                        break;
                                }
                                if (tok.EndToken.IsNewlineAfter && prev != null && prev.Typ != StreetItemType.Noun) 
                                    oo2 = true;
                                else if (inSearch) 
                                    oo2 = true;
                                else 
                                {
                                    StreetItemToken next = TryParse(tok.EndToken.Next, null, false, null);
                                    if (next != null && ((next.Typ == StreetItemType.Name || next.Typ == StreetItemType.Noun))) 
                                        oo2 = true;
                                    else if (AddressItemToken.CheckHouseAfter(tok.EndToken.Next, false, true) && prev != null) 
                                        oo2 = true;
                                }
                                if (oo2) 
                                    return new StreetItemToken(tok.BeginToken, tok.EndToken) { Typ = StreetItemType.StdAdjective, Termin = tok.Termin, IsAbridge = abr, Morph = tok.Morph };
                                break;
                            }
                            Pullenti.Ner.Token tt2 = tok.EndToken.Next;
                            if (tt2 != null && tt2.IsHiphen) 
                                tt2 = tt2.Next;
                            if (tt2 is Pullenti.Ner.TextToken) 
                            {
                                if (tt2.LengthChar == 1 && tt2.Chars.IsAllUpper) 
                                    break;
                                if (tt2.Chars.IsCapitalUpper) 
                                {
                                    bool isSur = false;
                                    string txt = (tt2 as Pullenti.Ner.TextToken).Term;
                                    if (Pullenti.Morph.LanguageHelper.EndsWith(txt, "ОГО")) 
                                        isSur = true;
                                    else 
                                        foreach (Pullenti.Morph.MorphBaseInfo wf in tt2.Morph.Items) 
                                        {
                                            if (wf.Class.IsProperSurname && (wf as Pullenti.Morph.MorphWordForm).IsInDictionary) 
                                            {
                                                if (wf.Case.IsGenitive) 
                                                {
                                                    isSur = true;
                                                    break;
                                                }
                                            }
                                        }
                                    if (isSur) 
                                        break;
                                }
                            }
                        }
                        StreetItemToken res1 = new StreetItemToken(tok.BeginToken, tok.EndToken) { Typ = StreetItemType.StdAdjective, Termin = tok.Termin, IsAbridge = abr, Morph = tok.Morph };
                        List<Pullenti.Ner.Core.TerminToken> toks = m_Ontology.TryParseAll(tok.BeginToken, Pullenti.Ner.Core.TerminParseAttr.No);
                        if (toks != null && toks.Count > 1) 
                            res1.AltTermin = toks[1].Termin;
                        return res1;
                    case StreetItemType.Noun:
                        if ((tt.IsValue(tok.Termin.CanonicText, null) || tok.EndToken.IsValue(tok.Termin.CanonicText, null) || tt.IsValue("УЛ", null)) || tok.Termin.CanonicText == "НАБЕРЕЖНАЯ") 
                            abr = false;
                        else if (tok.BeginToken != tok.EndToken && ((tok.BeginToken.Next.IsHiphen || tok.BeginToken.Next.IsCharOf("/\\")))) 
                        {
                        }
                        else if (!tt.Chars.IsAllLower && tt.LengthChar == 1) 
                            break;
                        else if (tt.LengthChar == 1) 
                        {
                            if (!tt.IsWhitespaceBefore) 
                            {
                                if (tt.Previous != null && tt.Previous.IsCharOf(",")) 
                                {
                                }
                                else 
                                    return null;
                            }
                            if (tok.EndToken.IsChar('.')) 
                            {
                            }
                            else if (tok.BeginToken != tok.EndToken && tok.BeginToken.Next != null && ((tok.BeginToken.Next.IsHiphen || tok.BeginToken.Next.IsCharOf("/\\")))) 
                            {
                            }
                            else if (tok.LengthChar > 5) 
                            {
                            }
                            else if (tok.BeginToken == tok.EndToken && tt.IsValue("Ш", null) && tt.Chars.IsAllLower) 
                            {
                                if (prev != null && ((prev.Typ == StreetItemType.Name || prev.Typ == StreetItemType.StdName || prev.Typ == StreetItemType.StdPartOfName))) 
                                {
                                }
                                else 
                                {
                                    StreetItemToken sii = TryParse(tt.Next, null, false, null);
                                    if (sii != null && (((sii.Typ == StreetItemType.Name || sii.Typ == StreetItemType.StdName || sii.Typ == StreetItemType.StdPartOfName) || sii.Typ == StreetItemType.Age))) 
                                    {
                                    }
                                    else 
                                        return null;
                                }
                            }
                            else 
                                return null;
                        }
                        else if (tt.Term == "КВ" && !tok.EndToken.IsValue("Л", null)) 
                        {
                            if (prev != null && prev.Typ == StreetItemType.Number) 
                                return null;
                            AddressItemToken ait = AddressItemToken.TryParsePureItem(tok.EndToken.Next, null, null);
                            if (ait != null && ait.Typ == AddressItemType.Number) 
                            {
                                if (AddressItemToken.CheckHouseAfter(ait.EndToken.Next, false, false)) 
                                {
                                }
                                else if (AddressItemToken.CheckStreetAfter(ait.EndToken.Next, false)) 
                                {
                                }
                                else 
                                    return null;
                            }
                            else if (tok.EndToken.Next != null && tok.EndToken.Next.IsValue("НЕТ", null)) 
                                return null;
                        }
                        if ((tok.EndToken == tok.BeginToken && !t.Chars.IsAllLower && t.Morph.Class.IsProperSurname) && t.Chars.IsCyrillicLetter) 
                        {
                            if (((t.Morph.Number & Pullenti.Morph.MorphNumber.Plural)) != Pullenti.Morph.MorphNumber.Undefined && !Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(t)) 
                                return null;
                        }
                        if (tt.Term == "ДОРОГОЙ" || tt.Term == "РЯДОМ") 
                            return null;
                        Pullenti.Ner.Core.Termin alt = null;
                        if (tok.BeginToken.IsValue("ПР", null) && ((tok.BeginToken == tok.EndToken || tok.BeginToken.Next.IsChar('.')))) 
                            alt = m_Prospect;
                        StreetItemToken res = new StreetItemToken(tok.BeginToken, tok.EndToken) { Typ = StreetItemType.Noun, Termin = tok.Termin, AltTermin = alt, IsAbridge = abr, Morph = tok.Morph, NounIsDoubtCoef = (tok.Termin.Tag2 is int ? (int)tok.Termin.Tag2 : 0) };
                        Pullenti.Morph.MorphClass mc = tok.BeginToken.GetMorphClassInDictionary();
                        if ((!abr && tok.BeginToken == tok.EndToken && tok.BeginToken.Chars.IsCapitalUpper) && ((mc.IsNoun || mc.IsAdjective))) 
                        {
                            if (tok.Morph.Case.IsNominative && !tok.Morph.Case.IsGenitive) 
                                res.NounCanBeName = true;
                            else if ((t.Next != null && t.Next.IsHiphen && (t.Next.Next is Pullenti.Ner.NumberToken)) && !t.Chars.IsAllLower) 
                                res.NounCanBeName = true;
                        }
                        if (res.IsRoad) 
                        {
                            StreetItemToken next = _tryParse(res.EndToken.Next, false, null, false);
                            if (next != null && next.IsRoad) 
                            {
                                res.EndToken = next.EndToken;
                                res.NounIsDoubtCoef = 0;
                                res.IsAbridge = false;
                            }
                        }
                        return res;
                    case StreetItemType.StdName:
                        bool isPostOff = tok.Termin.CanonicText == "ПОЧТОВОЕ ОТДЕЛЕНИЕ" || tok.Termin.CanonicText == "ПРОЕКТИРУЕМЫЙ";
                        if (((t.LengthChar == 1 && t.Next != null && t.Next.IsChar('.')) && t.Next.Next != null && t.Next.Next.LengthChar == 1) && t.Next.Next.Next.EndChar <= tok.EndChar) 
                        {
                            StreetItemToken next = _tryParse(tok.EndToken.Next, ignoreOnto, prev, inSearch);
                            if (next != null && ((next.Typ == StreetItemType.Name || next.Typ == StreetItemType.StdName))) 
                            {
                                next.Value = Pullenti.Ner.Core.MiscHelper.GetTextValueOfMetaToken(next, Pullenti.Ner.Core.GetTextAttr.No);
                                next.BeginToken = t;
                                return next;
                            }
                        }
                        if (tok.BeginToken.Chars.IsAllLower && !isPostOff && tok.EndToken.Chars.IsAllLower) 
                        {
                            if (CheckKeyword(tok.EndToken.Next)) 
                            {
                            }
                            else if (prev != null && ((prev.Typ == StreetItemType.Number || prev.Typ == StreetItemType.Noun || prev.Typ == StreetItemType.Age))) 
                            {
                            }
                            else if (Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(tok)) 
                            {
                            }
                            else 
                                return null;
                        }
                        StreetItemToken sits = new StreetItemToken(tok.BeginToken, tok.EndToken) { Typ = StreetItemType.StdName, Morph = tok.Morph, Value = tok.Termin.CanonicText };
                        if (tok.Termin.AdditionalVars != null && tok.Termin.AdditionalVars.Count > 0 && !tok.Termin.AdditionalVars[0].CanonicText.StartsWith("ПАРТ")) 
                        {
                            if (tok.Termin.AdditionalVars[0].CanonicText.IndexOf(' ') < 0) 
                            {
                                sits.AltValue = sits.Value;
                                sits.Value = tok.Termin.AdditionalVars[0].CanonicText;
                            }
                            else 
                                sits.AltValue = tok.Termin.AdditionalVars[0].CanonicText;
                            int ii = sits.AltValue.IndexOf(sits.Value);
                            if (ii >= 0) 
                            {
                                if (ii > 0) 
                                    sits.Misc = sits.AltValue.Substring(0, ii).Trim();
                                else 
                                    sits.Misc = sits.AltValue.Substring(sits.Value.Length).Trim();
                                sits.AltValue = null;
                            }
                        }
                        if (tok.BeginToken != tok.EndToken && !isPostOff) 
                        {
                            if (tok.BeginToken.Next == tok.EndToken) 
                            {
                                if (tok.EndToken.GetMorphClassInDictionary().IsNoun && tok.BeginToken.GetMorphClassInDictionary().IsAdjective) 
                                {
                                }
                                else if (((m_StdOntMisc.TryParse(tok.BeginToken, Pullenti.Ner.Core.TerminParseAttr.No) != null || tok.BeginToken.GetMorphClassInDictionary().IsProperName || (tok.BeginToken.LengthChar < 4))) && tok.EndToken.LengthChar > 2 && ((tok.EndToken.Morph.Class.IsProperSurname || !tok.EndToken.GetMorphClassInDictionary().IsProperName))) 
                                    sits.AltValue2 = Pullenti.Ner.Core.MiscHelper.GetTextValue(tok.EndToken, tok.EndToken, Pullenti.Ner.Core.GetTextAttr.No);
                                else if (((tok.EndToken.GetMorphClassInDictionary().IsProperName || m_StdOntMisc.TryParse(tok.EndToken, Pullenti.Ner.Core.TerminParseAttr.No) != null)) && (tok.BeginToken.Morph.Class.IsProperSurname)) 
                                    sits.AltValue2 = Pullenti.Ner.Core.MiscHelper.GetTextValue(tok.BeginToken, tok.BeginToken, Pullenti.Ner.Core.GetTextAttr.No);
                            }
                        }
                        return sits;
                    case StreetItemType.StdPartOfName:
                        Pullenti.Ner.Token tt1 = tok.EndToken;
                        string vvv = tok.Termin.CanonicText;
                        if ((tt1.Next is Pullenti.Ner.NumberToken) && tt1.Next.Next != null && tt1.Next.Next.IsValue("РАНГ", null)) 
                            tt1 = tt1.Next.Next;
                        Pullenti.Ner.Core.TerminToken tok2 = m_Ontology.TryParse(tt1.Next, Pullenti.Ner.Core.TerminParseAttr.No);
                        if (tok2 != null && ((StreetItemType)tok2.Termin.Tag) == StreetItemType.StdPartOfName) 
                        {
                            tt1 = tok2.EndToken;
                            vvv = string.Format("{0} {1}", vvv, tok2.Termin.CanonicText);
                        }
                        StreetItemToken sit = TryParse(tt1.Next, null, false, null);
                        if (sit != null && sit.Typ == StreetItemType.StdAdjective && tt1.Next.LengthChar == 1) 
                            sit = TryParse(sit.EndToken.Next, null, false, null);
                        if (sit == null || tt1.WhitespacesAfterCount > 3) 
                        {
                            foreach (Pullenti.Morph.MorphBaseInfo m in tok.Morph.Items) 
                            {
                                if (m.Number == Pullenti.Morph.MorphNumber.Plural && m.Case.IsGenitive) 
                                    return new StreetItemToken(tok.BeginToken, tt1) { Typ = StreetItemType.Name, Morph = tok.Morph, Value = Pullenti.Ner.Core.MiscHelper.GetTextValueOfMetaToken(tok, Pullenti.Ner.Core.GetTextAttr.No) };
                            }
                            return new StreetItemToken(tok.BeginToken, tt1) { Typ = StreetItemType.StdPartOfName, Morph = tok.Morph, Termin = tok.Termin };
                        }
                        if (sit.Typ != StreetItemType.Name && sit.Typ != StreetItemType.Noun && sit.Typ != StreetItemType.StdName) 
                            return null;
                        if (sit.Typ == StreetItemType.Noun) 
                        {
                            if (tok.Morph.Number == Pullenti.Morph.MorphNumber.Plural) 
                                return new StreetItemToken(tok.BeginToken, tt1) { Typ = StreetItemType.Name, AltTyp = StreetItemType.StdPartOfName, Morph = tok.Morph, Value = Pullenti.Ner.Core.MiscHelper.GetTextValueOfMetaToken(tok, Pullenti.Ner.Core.GetTextAttr.No) };
                            else 
                                return new StreetItemToken(tok.BeginToken, tt1) { Typ = StreetItemType.Name, AltTyp = StreetItemType.StdPartOfName, Morph = tok.Morph, Termin = tok.Termin };
                        }
                        if (sit.Value != null) 
                            sit.Misc = vvv;
                        else if (sit.ExistStreet == null) 
                        {
                            if (vvv == "ГЕРОЯ") 
                            {
                                if (sit.BeginToken.GetMorphClassInDictionary().IsProperSurname) 
                                    sit.Value = Pullenti.Ner.Core.MiscHelper.GetTextValueOfMetaToken(sit, Pullenti.Ner.Core.GetTextAttr.No);
                                else 
                                    sit.Value = "ГЕРОЕВ " + Pullenti.Ner.Core.MiscHelper.GetTextValueOfMetaToken(sit, Pullenti.Ner.Core.GetTextAttr.No);
                            }
                            else 
                            {
                                sit.Misc = vvv;
                                sit.Value = Pullenti.Ner.Core.MiscHelper.GetTextValueOfMetaToken(sit, Pullenti.Ner.Core.GetTextAttr.No);
                            }
                        }
                        sit.BeginToken = tok.BeginToken;
                        return sit;
                    case StreetItemType.Name:
                        if (tok.BeginToken.Chars.IsAllLower) 
                        {
                            if (prev != null && prev.Typ == StreetItemType.StdAdjective) 
                            {
                            }
                            else if (prev != null && prev.Typ == StreetItemType.Noun && AddressItemToken.CheckHouseAfter(tok.EndToken.Next, true, false)) 
                            {
                            }
                            else if (t.IsValue("ПРОЕКТИРУЕМЫЙ", null) || t.IsValue("МИРА", null)) 
                            {
                            }
                            else 
                            {
                                StreetItemToken nex = TryParse(tok.EndToken.Next, null, false, null);
                                if (nex != null && nex.Typ == StreetItemType.Noun) 
                                {
                                    Pullenti.Ner.Token tt2 = nex.EndToken.Next;
                                    while (tt2 != null && tt2.IsCharOf(",.")) 
                                    {
                                        tt2 = tt2.Next;
                                    }
                                    if (tt2 == null || tt2.WhitespacesBeforeCount > 1) 
                                        return null;
                                    if (AddressItemToken.CheckHouseAfter(tt2, false, true)) 
                                    {
                                    }
                                    else 
                                        return null;
                                }
                                else 
                                    return null;
                            }
                        }
                        StreetItemToken sit0 = TryParse(tok.BeginToken, prev, true, null);
                        if (sit0 != null && sit0.Typ == StreetItemType.Name && sit0.EndChar > tok.EndChar) 
                        {
                            sit0.IsInDictionary = true;
                            return sit0;
                        }
                        StreetItemToken sit1 = new StreetItemToken(tok.BeginToken, tok.EndToken) { Typ = StreetItemType.Name, Morph = tok.Morph, IsInDictionary = true };
                        if ((!tok.IsWhitespaceAfter && tok.EndToken.Next != null && tok.EndToken.Next.IsHiphen) && !tok.EndToken.Next.IsWhitespaceAfter) 
                        {
                            StreetItemToken sit2 = TryParse(tok.EndToken.Next.Next, null, false, null);
                            if (sit2 != null && ((sit2.Typ == StreetItemType.Name || sit2.Typ == StreetItemType.StdPartOfName || sit2.Typ == StreetItemType.StdName))) 
                                sit1.EndToken = sit2.EndToken;
                        }
                        if (npt != null && (sit1.EndChar < npt.EndChar) && m_Ontology.TryParse(npt.EndToken, Pullenti.Ner.Core.TerminParseAttr.No) == null) 
                        {
                            StreetItemToken sit2 = _tryParse(t, true, prev, inSearch);
                            if (sit2 != null && sit2.EndChar > sit1.EndChar) 
                                return sit2;
                        }
                        return sit1;
                    case StreetItemType.Fix:
                        return new StreetItemToken(tok.BeginToken, tok.EndToken) { Typ = StreetItemType.Fix, Morph = tok.Morph, IsInDictionary = true, Termin = tok.Termin };
                    }
            }
            if (tt != null && ((tt.IsValue("КИЛОМЕТР", null) || tt.IsValue("КМ", null)))) 
            {
                Pullenti.Ner.Token tt1 = (Pullenti.Ner.Token)tt;
                if (tt1.Next != null && tt1.Next.IsChar('.')) 
                    tt1 = tt1.Next;
                if ((tt1.WhitespacesAfterCount < 3) && (tt1.Next is Pullenti.Ner.NumberToken)) 
                {
                    StreetItemToken sit = new StreetItemToken(tt, tt1.Next) { Typ = StreetItemType.Number };
                    sit.Value = (tt1.Next as Pullenti.Ner.NumberToken).Value;
                    sit.NumberType = (tt1.Next as Pullenti.Ner.NumberToken).Typ;
                    sit.IsNumberKm = true;
                    bool isPlus = false;
                    Pullenti.Ner.Token tt2 = sit.EndToken.Next;
                    if (tt2 != null && ((tt2.IsHiphen || tt2.IsChar('+')))) 
                    {
                        isPlus = tt2.IsChar('+');
                        tt2 = tt2.Next;
                    }
                    Pullenti.Ner.Core.NumberExToken nex2 = Pullenti.Ner.Core.NumberHelper.TryParseNumberWithPostfix(tt2);
                    if (nex2 != null && nex2.ExTyp == Pullenti.Ner.Core.NumberExType.Meter) 
                    {
                        sit.EndToken = nex2.EndToken;
                        string mm = Pullenti.Ner.Core.NumberHelper.DoubleToString(nex2.RealValue / 1000);
                        if (mm.StartsWith("0.")) 
                            sit.Value += mm.Substring(1);
                    }
                    else if ((tt2 is Pullenti.Ner.NumberToken) && isPlus) 
                    {
                        double dw = (tt2 as Pullenti.Ner.NumberToken).RealValue;
                        if (dw > 0 && (dw < 1000)) 
                        {
                            sit.EndToken = tt2;
                            string mm = Pullenti.Ner.Core.NumberHelper.DoubleToString(dw / 1000);
                            if (mm.StartsWith("0.")) 
                                sit.Value += mm.Substring(1);
                        }
                    }
                    return sit;
                }
                StreetItemToken next = TryParse(tt.Next, null, inSearch, null);
                if (next != null && ((next.IsRailway || next.IsRoad))) 
                {
                    next.BeginToken = tt;
                    return next;
                }
            }
            Pullenti.Ner.Core.TerminToken tokn = Pullenti.Ner.Geo.Internal.NameToken.m_Onto.TryParse(tt, Pullenti.Ner.Core.TerminParseAttr.No);
            if (tokn != null) 
                return new StreetItemToken(tt, tokn.EndToken) { Typ = StreetItemType.Name, Value = tokn.Termin.CanonicText };
            if (tt != null) 
            {
                if (((tt.IsValue("РЕКА", null) || tt.IsValue("РЕЧКА", "РІЧКА"))) && tt.Next != null && ((!tt.Next.Chars.IsAllLower || Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(tt)))) 
                {
                    Pullenti.Ner.Geo.Internal.NameToken nam = Pullenti.Ner.Geo.Internal.NameToken.TryParse(tt.Next, Pullenti.Ner.Geo.Internal.GeoTokenType.City, 0, false);
                    if (nam != null && nam.Name != null && nam.Number == null) 
                        return new StreetItemToken(tt, nam.EndToken) { Typ = StreetItemType.Name, Morph = tt.Morph, Misc = "реки", Value = nam.Name };
                }
                if ((tt.IsValue("Р", null) && prev != null && prev.Termin != null) && prev.Termin.CanonicText == "НАБЕРЕЖНАЯ") 
                {
                    Pullenti.Ner.Token tt2 = tt.Next;
                    if (tt2 != null && tt2.IsChar('.')) 
                        tt2 = tt2.Next;
                    Pullenti.Ner.Geo.Internal.NameToken nam = Pullenti.Ner.Geo.Internal.NameToken.TryParse(tt2, Pullenti.Ner.Geo.Internal.GeoTokenType.City, 0, false);
                    if (nam != null && nam.Name != null && nam.Number == null) 
                        return new StreetItemToken(tt, nam.EndToken) { Typ = StreetItemType.Name, Morph = tt.Morph, Misc = "реки", Value = nam.Name };
                }
                if (tt.IsValue("КАДАСТРОВЫЙ", null)) 
                {
                    StreetItemToken next = TryParse(tt.Next, prev, inSearch, null);
                    if (next != null && next.Typ == StreetItemType.Noun && next.Termin.CanonicText == "КВАРТАЛ") 
                    {
                        next.BeginToken = tt;
                        return next;
                    }
                }
                if ((t.Previous is Pullenti.Ner.NumberToken) && (t.Previous as Pullenti.Ner.NumberToken).Value == "26") 
                {
                    if (tt.IsValue("БАКИНСКИЙ", null) || "БАКИНСК".StartsWith((tt as Pullenti.Ner.TextToken).Term)) 
                    {
                        Pullenti.Ner.Token tt2 = (Pullenti.Ner.Token)tt;
                        if (tt2.Next != null && tt2.Next.IsChar('.')) 
                            tt2 = tt2.Next;
                        if (tt2.Next is Pullenti.Ner.TextToken) 
                        {
                            tt2 = tt2.Next;
                            if (tt2.IsValue("КОМИССАР", null) || tt2.IsValue("КОММИССАР", null) || "КОМИС".StartsWith((tt2 as Pullenti.Ner.TextToken).Term)) 
                            {
                                if (tt2.Next != null && tt2.Next.IsChar('.')) 
                                    tt2 = tt2.Next;
                                StreetItemToken sit = new StreetItemToken(tt, tt2) { Typ = StreetItemType.StdName, IsInDictionary = true, Value = "БАКИНСКИХ КОМИССАРОВ", Morph = tt2.Morph };
                                return sit;
                            }
                        }
                    }
                }
                if ((tt.Next != null && ((tt.Next.IsChar('.') || ((tt.Next.IsHiphen && tt.LengthChar == 1)))) && ((!tt.Chars.IsAllLower || Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(tt)))) && (tt.Next.WhitespacesAfterCount < 3) && (tt.Next.Next is Pullenti.Ner.TextToken)) 
                {
                    Pullenti.Ner.Token tt1 = tt.Next.Next;
                    if (tt1 != null && tt1.IsHiphen && tt1.Next != null) 
                        tt1 = tt1.Next;
                    if (tt.LengthChar == 1 && tt1.LengthChar == 1 && (tt1.Next is Pullenti.Ner.TextToken)) 
                    {
                        if (tt1.IsAnd && tt1.Next.Chars.IsAllUpper && tt1.Next.LengthChar == 1) 
                            tt1 = tt1.Next;
                        if ((tt1.Chars.IsAllUpper && tt1.Next.IsChar('.') && (tt1.Next.WhitespacesAfterCount < 3)) && (tt1.Next.Next is Pullenti.Ner.TextToken)) 
                            tt1 = tt1.Next.Next;
                        else if ((tt1.Chars.IsAllUpper && (tt1.WhitespacesAfterCount < 3) && (tt1.Next is Pullenti.Ner.TextToken)) && !tt1.Next.Chars.IsAllLower) 
                            tt1 = tt1.Next;
                    }
                    StreetItemToken sit = StreetItemToken.TryParse(tt1, null, false, null);
                    if (sit != null) 
                    {
                        AddressItemToken ait = AddressItemToken.TryParsePureItem(tt, null, null);
                        if (ait != null) 
                        {
                            if (ait.Value != null && ait.Value != "0") 
                                sit = null;
                        }
                    }
                    if (sit != null && (tt1 is Pullenti.Ner.TextToken)) 
                    {
                        string str = (tt1 as Pullenti.Ner.TextToken).Term;
                        bool ok = false;
                        Pullenti.Morph.MorphClass mc = tt1.GetMorphClassInDictionary();
                        Pullenti.Morph.MorphClass cla = tt.Next.Next.GetMorphClassInDictionary();
                        if (sit.IsInDictionary) 
                            ok = true;
                        else if (sit._isSurname() || cla.IsProperSurname) 
                            ok = true;
                        else if (Pullenti.Morph.LanguageHelper.EndsWith(str, "ОЙ") && ((cla.IsProperSurname || ((sit.Typ == StreetItemType.Name && sit.IsInDictionary))))) 
                            ok = true;
                        else if (Pullenti.Morph.LanguageHelper.EndsWithEx(str, "ГО", "ИХ", "ЫХ", null)) 
                            ok = true;
                        else if ((tt1.IsWhitespaceBefore && !mc.IsUndefined && !mc.IsProperSurname) && !mc.IsProperName) 
                        {
                            if (AddressItemToken.CheckHouseAfter(sit.EndToken.Next, false, true)) 
                                ok = true;
                        }
                        else if (prev != null && prev.Typ == StreetItemType.Noun && ((!prev.IsAbridge || prev.LengthChar > 2))) 
                            ok = true;
                        else if ((prev != null && prev.Typ == StreetItemType.Name && sit.Typ == StreetItemType.Noun) && AddressItemToken.CheckHouseAfter(sit.EndToken.Next, false, true)) 
                            ok = true;
                        else if (sit.Typ == StreetItemType.Name && AddressItemToken.CheckHouseAfter(sit.EndToken.Next, false, true)) 
                        {
                            if (Pullenti.Ner.Geo.Internal.MiscLocationHelper.CheckGeoObjectBefore(tt, false)) 
                                ok = true;
                            else 
                            {
                                Pullenti.Ner.Geo.Internal.GeoAnalyzerData ad = Pullenti.Ner.Geo.GeoAnalyzer.GetData(t);
                                if (!ad.SRegime && SpeedRegime) 
                                {
                                    ok = true;
                                    sit.Cond = new Pullenti.Ner.Geo.Internal.Condition() { GeoBeforeToken = tt, PureGeoBefore = true };
                                }
                            }
                        }
                        if (!ok && Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(tt) && ((sit.Typ == StreetItemType.Name || sit.Typ == StreetItemType.StdAdjective))) 
                        {
                            StreetItemToken sit1 = TryParse(sit.EndToken.Next, null, false, null);
                            if (sit1 != null && sit1.Typ == StreetItemType.Noun) 
                                ok = true;
                            else if (AddressItemToken.CheckHouseAfter(sit.EndToken.Next, true, false)) 
                                ok = true;
                            else if (sit.EndToken.IsNewlineAfter) 
                                ok = true;
                        }
                        if (ok) 
                        {
                            sit.BeginToken = tt;
                            if (sit.Value == null) 
                                sit.Value = str;
                            if (tok != null && ((StreetItemType)tok.Termin.Tag) == StreetItemType.StdAdjective && !hasNamed) 
                            {
                                if ((tok.EndToken.Next is Pullenti.Ner.TextToken) && tok.EndToken.Next.LengthChar == 1 && tok.EndToken.Next.Chars.IsLetter) 
                                {
                                }
                                else 
                                {
                                    sit.StdAdjVersion = new StreetItemToken(tok.BeginToken, tok.EndToken) { Typ = StreetItemType.StdAdjective, Termin = tok.Termin, IsAbridge = true };
                                    List<Pullenti.Ner.Core.TerminToken> toks2 = m_Ontology.TryParseAll(tok.BeginToken, Pullenti.Ner.Core.TerminParseAttr.No);
                                    if (toks2 != null && toks2.Count > 1) 
                                        sit.StdAdjVersion.AltTermin = toks2[1].Termin;
                                }
                            }
                            return sit;
                        }
                    }
                }
                if (tt.Chars.IsCyrillicLetter && tt.LengthChar > 1 && !tt.Morph.Class.IsPreposition) 
                {
                    if (((tt.IsValue("ОБЪЕЗД", null) || tt.IsValue("ОБХОД", null))) && tt.Next != null) 
                    {
                        if (prev == null) 
                            return null;
                        if (prev.IsRoad) 
                        {
                            List<Pullenti.Ner.Geo.Internal.CityItemToken> cits = Pullenti.Ner.Geo.Internal.CityItemToken.TryParseList(tt.Next, 3, null);
                            if (cits != null && (cits.Count < 3)) 
                            {
                                StreetItemToken resr = new StreetItemToken(tt, cits[cits.Count - 1].EndToken) { Typ = StreetItemType.Name, IsRoadName = true };
                                foreach (Pullenti.Ner.Geo.Internal.CityItemToken ci in cits) 
                                {
                                    if (ci.Typ == Pullenti.Ner.Geo.Internal.CityItemToken.ItemType.City || ci.Typ == Pullenti.Ner.Geo.Internal.CityItemToken.ItemType.ProperName) 
                                    {
                                        resr.Value = "ОБЪЕЗД " + Pullenti.Ner.Core.MiscHelper.GetTextValueOfMetaToken(ci, Pullenti.Ner.Core.GetTextAttr.No);
                                        break;
                                    }
                                }
                                if (resr.Value != null) 
                                    return resr;
                            }
                        }
                    }
                    if ((tt.IsValue("ГЕРОЙ", null) || tt.IsValue("ЗАЩИТНИК", "ЗАХИСНИК") || tt.IsValue("ОБРАЗОВАНИЕ", null)) || tt.IsValue("ОСВОБОДИТЕЛЬ", "ВИЗВОЛИТЕЛЬ") || tt.IsValue("КОНСТИТУЦИЯ", null)) 
                    {
                        Pullenti.Ner.Token tt2 = null;
                        if ((tt.Next is Pullenti.Ner.ReferentToken) && (tt.Next.GetReferent() is Pullenti.Ner.Geo.GeoReferent)) 
                            tt2 = tt.Next;
                        else 
                        {
                            Pullenti.Ner.Core.NounPhraseToken npt2 = Pullenti.Ner.Geo.Internal.MiscLocationHelper.TryParseNpt(tt.Next);
                            if (npt2 != null && npt2.Morph.Case.IsGenitive) 
                                tt2 = npt2.EndToken;
                            else 
                            {
                                Pullenti.Ner.Core.IntOntologyToken tee = Pullenti.Ner.Geo.Internal.TerrItemToken.CheckOntoItem(tt.Next);
                                if (tee != null) 
                                    tt2 = tee.EndToken;
                                else if ((((tee = Pullenti.Ner.Geo.Internal.CityItemToken.CheckOntoItem(tt.Next)))) != null) 
                                    tt2 = tee.EndToken;
                            }
                        }
                        if (tt2 != null) 
                        {
                            StreetItemToken re = new StreetItemToken(tt, tt2) { Typ = StreetItemType.StdPartOfName, Value = Pullenti.Ner.Core.MiscHelper.GetTextValue(tt, tt2, Pullenti.Ner.Core.GetTextAttr.No), NoGeoInThisToken = true };
                            StreetItemToken sit = TryParse(tt2.Next, null, false, null);
                            if (sit == null || sit.Typ != StreetItemType.Name) 
                            {
                                bool ok2 = false;
                                if (sit != null && ((sit.Typ == StreetItemType.StdAdjective || sit.Typ == StreetItemType.Noun))) 
                                    ok2 = true;
                                else if (AddressItemToken.CheckHouseAfter(tt2.Next, false, true)) 
                                    ok2 = true;
                                else if (tt2.IsNewlineAfter) 
                                    ok2 = true;
                                if (ok2) 
                                {
                                    sit = new StreetItemToken(tt, tt2) { Typ = StreetItemType.Name };
                                    sit.Value = Pullenti.Ner.Core.MiscHelper.GetTextValue(tt, tt2, Pullenti.Ner.Core.GetTextAttr.No);
                                    if (!tt.IsValue("ОБРАЗОВАНИЕ", null)) 
                                        sit.NoGeoInThisToken = true;
                                    return sit;
                                }
                                return re;
                            }
                            if (sit.Value == null) 
                                sit.Value = Pullenti.Ner.Core.MiscHelper.GetTextValueOfMetaToken(sit, Pullenti.Ner.Core.GetTextAttr.No);
                            if (sit.AltValue == null) 
                            {
                                sit.AltValue = sit.Value;
                                sit.Value = string.Format("{0} {1}", re.Value, sit.Value);
                            }
                            else 
                                sit.Value = string.Format("{0} {1}", re.Value, sit.Value);
                            sit.BeginToken = tt;
                            return sit;
                        }
                    }
                    Pullenti.Ner.NumberToken ani = Pullenti.Ner.Core.NumberHelper.TryParseAnniversary(t);
                    if (ani != null) 
                        return new StreetItemToken(t, ani.EndToken) { Typ = StreetItemType.Age, NumberType = Pullenti.Ner.NumberSpellingType.Age, Value = ani.Value };
                    Pullenti.Ner.Geo.Internal.NumToken num1 = Pullenti.Ner.Geo.Internal.NumToken.TryParse(t, Pullenti.Ner.Geo.Internal.GeoTokenType.Street);
                    if (num1 != null) 
                        return new StreetItemToken(t, num1.EndToken) { Typ = StreetItemType.Number, NumberType = Pullenti.Ner.NumberSpellingType.Roman, Value = num1.Value };
                    if (prev != null && prev.Typ == StreetItemType.Noun) 
                    {
                    }
                    else 
                    {
                        Pullenti.Ner.Geo.Internal.OrgItemToken org = Pullenti.Ner.Geo.Internal.OrgItemToken.TryParse(t, null);
                        if (org != null) 
                        {
                            if (org.IsGsk || org.HasTerrKeyword) 
                                return new StreetItemToken(t, org.EndToken) { Typ = StreetItemType.Fix, Org = org };
                        }
                    }
                    bool ok1 = false;
                    Pullenti.Ner.Geo.Internal.Condition cond = null;
                    if (!tt.Chars.IsAllLower) 
                    {
                        AddressItemToken ait = AddressItemToken.TryParsePureItem(tt, null, null);
                        if (ait != null) 
                        {
                            if (tt.Next != null && tt.Next.IsHiphen) 
                                ok1 = true;
                            else if (tt.IsValue("БЛОК", null) || tt.IsValue("ДОС", null) || ait.EndToken.IsValue("БЛОК", null)) 
                                ok1 = true;
                        }
                        else 
                            ok1 = true;
                    }
                    else if (prev != null && ((prev.Typ == StreetItemType.Noun || ((prev.Typ == StreetItemType.StdAdjective && t.Previous.IsHiphen)) || ((prev.Typ == StreetItemType.Number && Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(prev)))))) 
                    {
                        if (AddressItemToken.CheckHouseAfter(tt.Next, false, false)) 
                        {
                            if (!AddressItemToken.CheckHouseAfter(tt, false, false)) 
                                ok1 = true;
                        }
                        if (!ok1) 
                        {
                            Pullenti.Ner.Token tt1 = prev.BeginToken.Previous;
                            if (tt1 != null && tt1.IsComma) 
                                tt1 = tt1.Previous;
                            if (tt1 != null && (tt1.GetReferent() is Pullenti.Ner.Geo.GeoReferent)) 
                                ok1 = true;
                            else if (Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(prev) && !AddressItemToken.CheckHouseAfter(tt, false, false)) 
                                ok1 = true;
                            else if (t.Previous != null && t.Previous.IsHiphen) 
                                ok1 = true;
                            else 
                            {
                                Pullenti.Ner.Geo.Internal.GeoAnalyzerData ad = Pullenti.Ner.Geo.GeoAnalyzer.GetData(t);
                                if (!ad.SRegime && SpeedRegime) 
                                {
                                    ok1 = true;
                                    cond = new Pullenti.Ner.Geo.Internal.Condition() { GeoBeforeToken = prev.BeginToken, PureGeoBefore = true };
                                }
                            }
                        }
                    }
                    else if (tt.WhitespacesAfterCount < 2) 
                    {
                        Pullenti.Ner.Core.TerminToken nex = m_Ontology.TryParse(tt.Next, Pullenti.Ner.Core.TerminParseAttr.No);
                        if (nex != null && nex.Termin != null) 
                        {
                            if (nex.Termin.CanonicText == "ПЛОЩАДЬ") 
                            {
                                if (tt.IsValue("ОБЩИЙ", null)) 
                                    return null;
                            }
                            Pullenti.Ner.Token tt1 = tt.Previous;
                            if (tt1 != null && tt1.IsComma) 
                                tt1 = tt1.Previous;
                            if (tt1 != null && (tt1.GetReferent() is Pullenti.Ner.Geo.GeoReferent)) 
                                ok1 = true;
                            else if (AddressItemToken.CheckHouseAfter(nex.EndToken.Next, false, false)) 
                                ok1 = true;
                            else if (Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(tt)) 
                                ok1 = true;
                        }
                        else if (Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(tt) && tt.LengthChar > 3) 
                        {
                            if (AddressItemToken.TryParsePureItem(tt, null, null) == null) 
                                ok1 = true;
                        }
                    }
                    else if (tt.IsNewlineAfter && tt.LengthChar > 2 && Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(tt)) 
                        ok1 = true;
                    if (ok1) 
                    {
                        Pullenti.Morph.MorphClass dc = tt.GetMorphClassInDictionary();
                        if (dc.IsAdverb && !Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(tt)) 
                        {
                            if (!(dc.IsProper)) 
                            {
                                if (tt.Next != null && tt.Next.IsHiphen) 
                                {
                                }
                                else 
                                    return null;
                            }
                        }
                        StreetItemToken res = new StreetItemToken(tt, tt) { Typ = StreetItemType.Name, Morph = tt.Morph, Cond = cond };
                        if ((tt.Next != null && (tt.Next.IsHiphen) && (tt.Next.Next is Pullenti.Ner.TextToken)) && !tt.IsWhitespaceAfter && !tt.Next.IsWhitespaceAfter) 
                        {
                            bool ok2 = AddressItemToken.CheckHouseAfter(tt.Next.Next.Next, false, false) || tt.Next.Next.IsNewlineAfter;
                            if (!ok2) 
                            {
                                StreetItemToken te2 = TryParse(tt.Next.Next.Next, null, false, null);
                                if (te2 != null && te2.Typ == StreetItemType.Noun) 
                                    ok2 = true;
                            }
                            if (((!ok2 && tt.Next.IsHiphen && !tt.IsWhitespaceAfter) && !tt.Next.IsWhitespaceAfter && (tt.Next.Next is Pullenti.Ner.TextToken)) && tt.Next.Next.LengthChar > 3) 
                                ok2 = true;
                            if (ok2) 
                            {
                                res.EndToken = tt.Next.Next;
                                res.Value = string.Format("{0} {1}", Pullenti.Ner.Core.MiscHelper.GetTextValue(tt, tt, Pullenti.Ner.Core.GetTextAttr.No), Pullenti.Ner.Core.MiscHelper.GetTextValue(res.EndToken, res.EndToken, Pullenti.Ner.Core.GetTextAttr.No));
                            }
                        }
                        else if ((tt.WhitespacesAfterCount < 2) && (tt.Next is Pullenti.Ner.TextToken) && tt.Next.Chars.IsLetter) 
                        {
                            if (tt.Next.IsValue("БИ", null)) 
                            {
                                if (res.Value == null) 
                                    res.Value = Pullenti.Ner.Core.MiscHelper.GetTextValue(tt, tt, Pullenti.Ner.Core.GetTextAttr.No);
                                res.EndToken = tt.Next;
                                res.Value = string.Format("{0} {1}", res.Value, (tt.Next as Pullenti.Ner.TextToken).Term);
                                if (res.AltValue != null) 
                                    res.AltValue = string.Format("{0} {1}", res.AltValue, (tt.Next as Pullenti.Ner.TextToken).Term);
                            }
                            else if (!AddressItemToken.CheckHouseAfter(tt.Next, false, false) || tt.Next.IsNewlineAfter) 
                            {
                                Pullenti.Ner.Token tt1 = tt.Next;
                                bool isPref = false;
                                if ((tt1 is Pullenti.Ner.TextToken) && tt1.Chars.IsAllLower) 
                                {
                                    if (tt1.IsValue("ДЕ", null) || tt1.IsValue("ЛА", null)) 
                                    {
                                        tt1 = tt1.Next;
                                        isPref = true;
                                    }
                                }
                                StreetItemToken nn = TryParse(tt1, null, false, null);
                                if (nn == null || nn.Typ == StreetItemType.Name) 
                                {
                                    npt = Pullenti.Ner.Geo.Internal.MiscLocationHelper.TryParseNpt(tt);
                                    if (npt != null) 
                                    {
                                        if (npt.BeginToken == npt.EndToken) 
                                            npt = null;
                                        else if (m_Ontology.TryParse(npt.EndToken, Pullenti.Ner.Core.TerminParseAttr.No) != null) 
                                            npt = null;
                                    }
                                    if (npt != null && ((npt.IsNewlineAfter || AddressItemToken.CheckHouseAfter(npt.EndToken.Next, false, false) || ((npt.EndToken.Next != null && npt.EndToken.Next.IsCommaAnd))))) 
                                    {
                                        res.EndToken = npt.EndToken;
                                        if (npt.Morph.Case.IsGenitive) 
                                        {
                                            res.Value = Pullenti.Ner.Core.MiscHelper.GetTextValueOfMetaToken(npt, Pullenti.Ner.Core.GetTextAttr.No);
                                            res.AltValue = npt.GetNormalCaseText(null, Pullenti.Morph.MorphNumber.Undefined, Pullenti.Morph.MorphGender.Undefined, false);
                                        }
                                        else 
                                        {
                                            res.Value = npt.GetNormalCaseText(null, Pullenti.Morph.MorphNumber.Undefined, Pullenti.Morph.MorphGender.Undefined, false);
                                            res.AltValue = Pullenti.Ner.Core.MiscHelper.GetTextValueOfMetaToken(npt, Pullenti.Ner.Core.GetTextAttr.No);
                                        }
                                    }
                                    else if ((tt1.LengthChar > 2 && AddressItemToken.CheckHouseAfter(tt1.Next, false, false) && tt1.Chars.IsCyrillicLetter == tt.Chars.IsCyrillicLetter) && (t.WhitespacesAfterCount < 2)) 
                                    {
                                        if (tt1.Morph.Class.IsVerb && !tt1.IsValue("ДАЛИ", null)) 
                                        {
                                        }
                                        else if (npt == null && !tt1.Chars.IsAllLower && !isPref) 
                                        {
                                        }
                                        else 
                                        {
                                            res.EndToken = tt1;
                                            res.Value = string.Format("{0} {1}", Pullenti.Ner.Core.MiscHelper.GetTextValue(res.BeginToken, res.BeginToken, Pullenti.Ner.Core.GetTextAttr.No), Pullenti.Ner.Core.MiscHelper.GetTextValue(res.EndToken, res.EndToken, Pullenti.Ner.Core.GetTextAttr.No));
                                        }
                                    }
                                    else if ((nn != null && t.LengthChar > 3 && nn.BeginToken == nn.EndToken) && t.GetMorphClassInDictionary().IsProperName && nn.BeginToken.Morph.Class.IsProperSurname) 
                                    {
                                        res.EndToken = nn.EndToken;
                                        res.Value = Pullenti.Ner.Core.MiscHelper.GetTextValueOfMetaToken(nn, Pullenti.Ner.Core.GetTextAttr.No);
                                        res.Misc = Pullenti.Ner.Core.MiscHelper.GetTextValue(t, t, Pullenti.Ner.Core.GetTextAttr.No);
                                    }
                                }
                                else if (nn.Typ == StreetItemType.Noun) 
                                {
                                    Pullenti.Morph.MorphGender gen = nn.Termin.Gender;
                                    if (gen == Pullenti.Morph.MorphGender.Undefined) 
                                    {
                                        npt = Pullenti.Ner.Geo.Internal.MiscLocationHelper.TryParseNpt(tt);
                                        if (npt != null && npt.EndToken == nn.EndToken) 
                                            gen = npt.Morph.Gender;
                                        else if (prev != null && prev.Typ == StreetItemType.Noun) 
                                            gen = prev.Termin.Gender;
                                    }
                                    else 
                                        foreach (Pullenti.Morph.MorphBaseInfo ii in tt.Morph.Items) 
                                        {
                                            if (((ii.Class.IsProperSurname || ii.Class.IsNoun)) && ii.Case.IsGenitive && (ii is Pullenti.Morph.MorphWordForm)) 
                                            {
                                                if ((ii as Pullenti.Morph.MorphWordForm).IsInDictionary) 
                                                {
                                                    gen = Pullenti.Morph.MorphGender.Undefined;
                                                    break;
                                                }
                                            }
                                        }
                                    if (gen != Pullenti.Morph.MorphGender.Undefined && ((!nn.Morph.Case.IsNominative || nn.Morph.Number != Pullenti.Morph.MorphNumber.Singular))) 
                                    {
                                        res.Value = Pullenti.Ner.Core.MiscHelper.GetTextValue(res.BeginToken, res.EndToken, Pullenti.Ner.Core.GetTextAttr.No);
                                        string var = null;
                                        try 
                                        {
                                            var = Pullenti.Morph.MorphologyService.GetWordform(res.Value, new Pullenti.Morph.MorphBaseInfo() { Case = Pullenti.Morph.MorphCase.Nominative, Class = Pullenti.Morph.MorphClass.Adjective, Number = Pullenti.Morph.MorphNumber.Singular, Gender = gen });
                                        }
                                        catch(Exception ex) 
                                        {
                                        }
                                        if (var != null && var.EndsWith("ОЙ") && !res.BeginToken.GetMorphClassInDictionary().IsAdjective) 
                                        {
                                            if (gen == Pullenti.Morph.MorphGender.Masculine) 
                                                var = var.Substring(0, var.Length - 2) + "ЫЙ";
                                            else if (gen == Pullenti.Morph.MorphGender.Neuter) 
                                                var = var.Substring(0, var.Length - 2) + "ОЕ";
                                            else if (gen == Pullenti.Morph.MorphGender.Feminie) 
                                                var = var.Substring(0, var.Length - 2) + "АЯ";
                                        }
                                        if (var != null && var != res.Value) 
                                        {
                                            res.AltValue = res.Value;
                                            res.Value = var;
                                        }
                                    }
                                }
                            }
                        }
                        if (res != null && res.Typ == StreetItemType.Name && (res.WhitespacesAfterCount < 2)) 
                        {
                            tt = res.EndToken.Next as Pullenti.Ner.TextToken;
                            if (Pullenti.Ner.Geo.Internal.NameToken.CheckInitialBack(tt)) 
                            {
                                if (res.Value == null) 
                                    res.Value = Pullenti.Ner.Core.MiscHelper.GetTextValueOfMetaToken(res, Pullenti.Ner.Core.GetTextAttr.No);
                                if (tt.Next != null && tt.Next.IsChar('.')) 
                                    tt = tt.Next as Pullenti.Ner.TextToken;
                                res.EndToken = tt;
                                tt = res.EndToken.Next as Pullenti.Ner.TextToken;
                                if ((((res.WhitespacesAfterCount < 2) && tt != null && tt.LengthChar == 1) && tt.Chars.IsAllUpper && tt.Next != null) && tt.Next.IsChar('.')) 
                                    res.EndToken = tt.Next;
                                tt = res.EndToken.Next as Pullenti.Ner.TextToken;
                            }
                            else if ((tt != null && tt.LengthChar == 1 && tt.Chars.IsAllUpper) && tt.Next != null && tt.Next.IsChar('.')) 
                            {
                                if (TryParse(tt, null, false, null) != null || AddressItemToken.CheckHouseAfter(tt, false, false)) 
                                {
                                }
                                else 
                                {
                                    Pullenti.Ner.ReferentToken rt = tt.Kit.ProcessReferent("PERSON", tt, null);
                                    if (rt == null) 
                                    {
                                        if (res.Value == null) 
                                            res.Value = Pullenti.Ner.Core.MiscHelper.GetTextValueOfMetaToken(res, Pullenti.Ner.Core.GetTextAttr.No);
                                        res.EndToken = tt.Next;
                                        tt = res.EndToken.Next as Pullenti.Ner.TextToken;
                                        if (((res.WhitespacesAfterCount < 2) && tt != null && tt.LengthChar == 1) && tt.Chars.IsAllUpper) 
                                        {
                                            if (tt.Next != null && tt.Next.IsChar('.')) 
                                                res.EndToken = tt.Next;
                                            else if (tt.Next == null || tt.Next.IsComma) 
                                                res.EndToken = tt;
                                        }
                                    }
                                }
                            }
                            if (tt != null && tt.GetMorphClassInDictionary().IsProperName) 
                            {
                                Pullenti.Ner.ReferentToken rt = tt.Kit.ProcessReferent("PERSON", res.BeginToken, null);
                                if (rt != null) 
                                {
                                    bool ok2 = false;
                                    if (rt.EndToken == tt) 
                                        ok2 = true;
                                    else if (rt.EndToken == tt.Next && tt.Next.GetMorphClassInDictionary().IsProperSecname) 
                                        ok2 = true;
                                    if (ok2) 
                                    {
                                        if (res.Value == null) 
                                            res.Value = Pullenti.Ner.Core.MiscHelper.GetTextValueOfMetaToken(res, Pullenti.Ner.Core.GetTextAttr.No);
                                        res.EndToken = rt.EndToken;
                                        if (res.BeginToken != res.EndToken) 
                                        {
                                            Pullenti.Morph.MorphClass mc1 = res.BeginToken.GetMorphClassInDictionary();
                                            Pullenti.Morph.MorphClass mc2 = res.EndToken.GetMorphClassInDictionary();
                                            if (((mc1.IsProperName && !mc2.IsProperName)) || ((!mc1.IsProperSurname && mc2.IsProperSurname))) 
                                            {
                                                res.Misc = res.Value;
                                                res.Value = Pullenti.Ner.Core.MiscHelper.GetTextValue(res.EndToken, res.EndToken, Pullenti.Ner.Core.GetTextAttr.No);
                                            }
                                            else if (mc1.IsProperName && mc2.IsProperSurname) 
                                            {
                                                res.Misc = res.Value;
                                                res.Value = Pullenti.Ner.Core.MiscHelper.GetTextValue(res.EndToken, res.EndToken, Pullenti.Ner.Core.GetTextAttr.No);
                                            }
                                            else 
                                                res.Misc = Pullenti.Ner.Core.MiscHelper.GetTextValue(res.EndToken, res.EndToken, Pullenti.Ner.Core.GetTextAttr.No);
                                        }
                                    }
                                }
                            }
                        }
                        if (res.BeginToken == res.EndToken) 
                        {
                            Pullenti.Ner.MetaToken nn = Pullenti.Ner.Geo.Internal.MiscLocationHelper.TryAttachNordWest(res.BeginToken);
                            if (nn != null && nn.EndChar > res.EndChar) 
                            {
                                res.EndToken = nn.EndToken;
                                res.Value = Pullenti.Ner.Core.MiscHelper.GetTextValueOfMetaToken(res, Pullenti.Ner.Core.GetTextAttr.No).Replace(" - ", " ");
                            }
                        }
                        return res;
                    }
                }
                if (tt.IsValue("№", null) || tt.IsValue("НОМЕР", null) || tt.IsValue("НОМ", null)) 
                {
                    Pullenti.Ner.Token tt1 = tt.Next;
                    if (tt1 != null && tt1.IsChar('.')) 
                        tt1 = tt1.Next;
                    if ((tt1 is Pullenti.Ner.NumberToken) && (tt1 as Pullenti.Ner.NumberToken).IntValue != null) 
                        return new StreetItemToken(tt, tt1) { Typ = StreetItemType.Number, NumberType = (tt1 as Pullenti.Ner.NumberToken).Typ, Value = (tt1 as Pullenti.Ner.NumberToken).Value, NumberHasPrefix = true };
                }
                if (tt.IsHiphen && (tt.Next is Pullenti.Ner.NumberToken) && (tt.Next as Pullenti.Ner.NumberToken).IntValue != null) 
                {
                    if (prev != null && prev.Typ == StreetItemType.Noun) 
                    {
                        if ((prev.NounCanBeName || prev.Termin.CanonicText == "МИКРОРАЙОН" || prev.Termin.CanonicText == "КВАРТАЛ") || Pullenti.Morph.LanguageHelper.EndsWith(prev.Termin.CanonicText, "ГОРОДОК")) 
                            return new StreetItemToken(tt, tt.Next) { Typ = StreetItemType.Number, NumberType = (tt.Next as Pullenti.Ner.NumberToken).Typ, Value = (tt.Next as Pullenti.Ner.NumberToken).Value, NumberHasPrefix = true };
                    }
                }
                if (((tt is Pullenti.Ner.TextToken) && tt.LengthChar == 1 && (tt.WhitespacesBeforeCount < 2)) && tt.Chars.IsLetter && tt.Chars.IsAllUpper) 
                {
                    if (prev != null && prev.Typ == StreetItemType.Noun) 
                    {
                        if (prev.Termin.CanonicText == "МИКРОРАЙОН" || prev.Termin.CanonicText == "КВАРТАЛ" || Pullenti.Morph.LanguageHelper.EndsWith(prev.Termin.CanonicText, "ГОРОДОК")) 
                            return new StreetItemToken(tt, tt) { Typ = StreetItemType.Name, Value = (tt as Pullenti.Ner.TextToken).Term };
                        if ((prev.Termin.CanonicText == "РЯД" || prev.Termin.CanonicText == "БЛОК" || prev.Termin.CanonicText == "ЛИНИЯ") || prev.Termin.CanonicText == "ПАНЕЛЬ") 
                        {
                            StreetItemToken res = new StreetItemToken(tt, tt) { Typ = StreetItemType.Number, Value = (tt as Pullenti.Ner.TextToken).Term };
                            Pullenti.Ner.Token tt2 = tt.Next;
                            if (tt2 != null && tt2.IsHiphen) 
                                tt2 = tt2.Next;
                            if ((tt2 is Pullenti.Ner.NumberToken) && (tt.WhitespacesAfterCount < 3)) 
                            {
                                AddressItemToken ait = AddressItemToken.TryParsePureItem(tt2, null, null);
                                if (ait != null && ait.Typ == AddressItemType.Number) 
                                {
                                    res.Value = string.Format("{0}{1}", ait.Value, res.Value);
                                    res.EndToken = ait.EndToken;
                                }
                            }
                            return res;
                        }
                        if (Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(tt)) 
                        {
                            StreetItemToken next = TryParse(tt.Next, prev, inSearch, null);
                            if (next != null && next.Typ == StreetItemType.Name) 
                            {
                                next = next.Clone();
                                next.Value = Pullenti.Ner.Core.MiscHelper.GetTextValueOfMetaToken(next, Pullenti.Ner.Core.GetTextAttr.No);
                                next.BeginToken = tt;
                                return next;
                            }
                        }
                    }
                }
            }
            Pullenti.Ner.Referent r = (t == null ? null : t.GetReferent());
            if (r is Pullenti.Ner.Geo.GeoReferent) 
            {
                Pullenti.Ner.Geo.GeoReferent geo = r as Pullenti.Ner.Geo.GeoReferent;
                if (prev != null && prev.Typ == StreetItemType.Noun) 
                {
                    if (AddressItemToken.CheckHouseAfter(t.Next, false, false)) 
                    {
                        StreetItemToken res1 = TryParse((t as Pullenti.Ner.ReferentToken).BeginToken, prev, false, null);
                        if (res1 != null && res1.EndChar == t.EndChar) 
                        {
                            res1 = res1.Clone();
                            res1.BeginToken = (res1.EndToken = t);
                            return res1;
                        }
                        StreetItemToken res = new StreetItemToken(t, t) { Typ = StreetItemType.Name, Value = Pullenti.Ner.Core.MiscHelper.GetTextValue(t, t, Pullenti.Ner.Core.GetTextAttr.No) };
                        return res;
                    }
                }
            }
            if (((tt is Pullenti.Ner.TextToken) && tt.Chars.IsCapitalUpper && tt.Chars.IsLatinLetter) && (tt.WhitespacesAfterCount < 2)) 
            {
                if (Pullenti.Ner.Core.MiscHelper.IsEngArticle(tt)) 
                    return null;
                Pullenti.Ner.Token tt2 = tt.Next;
                if (Pullenti.Ner.Core.MiscHelper.IsEngAdjSuffix(tt2)) 
                    tt2 = tt2.Next.Next;
                Pullenti.Ner.Core.TerminToken tok1 = m_Ontology.TryParse(tt2, Pullenti.Ner.Core.TerminParseAttr.No);
                if (tok1 != null) 
                    return new StreetItemToken(tt, tt2.Previous) { Typ = StreetItemType.Name, Morph = tt.Morph, Value = (tt as Pullenti.Ner.TextToken).Term };
            }
            if (((tt != null && tt.IsValue("ПОДЪЕЗД", null) && prev != null) && prev.IsRoad && tt.Next != null) && tt.Next.IsValue("К", null) && tt.Next.Next != null) 
            {
                StreetItemToken sit = new StreetItemToken(tt, tt.Next) { Typ = StreetItemType.Name };
                sit.IsRoadName = true;
                Pullenti.Ner.Token t1 = tt.Next.Next;
                Pullenti.Ner.Geo.GeoReferent g1 = null;
                for (; t1 != null; t1 = t1.Next) 
                {
                    if (t1.WhitespacesBeforeCount > 3) 
                        break;
                    if ((((g1 = t1.GetReferent() as Pullenti.Ner.Geo.GeoReferent))) != null) 
                        break;
                    if (t1.IsChar('.') || (t1.LengthChar < 3)) 
                        continue;
                    if ((t1.LengthChar < 4) && t1.Chars.IsAllLower) 
                        continue;
                    break;
                }
                if (g1 != null) 
                {
                    sit.EndToken = t1;
                    List<string> nams = g1.GetStringValues(Pullenti.Ner.Geo.GeoReferent.ATTR_NAME);
                    if (nams == null || nams.Count == 0) 
                        return null;
                    sit.Value = "ПОДЪЕЗД - " + nams[0];
                    if (nams.Count > 1) 
                        sit.AltValue = "ПОДЪЕЗД - " + nams[1];
                    return sit;
                }
                if ((t1 is Pullenti.Ner.TextToken) && (t1.WhitespacesBeforeCount < 2) && t1.Chars.IsCapitalUpper) 
                {
                    Pullenti.Ner.Geo.Internal.CityItemToken cit = Pullenti.Ner.Geo.Internal.CityItemToken.TryParse(t1, null, true, null);
                    if (cit != null && ((cit.Typ == Pullenti.Ner.Geo.Internal.CityItemToken.ItemType.ProperName || cit.Typ == Pullenti.Ner.Geo.Internal.CityItemToken.ItemType.City))) 
                    {
                        sit.EndToken = cit.EndToken;
                        sit.Value = "ПОДЪЕЗД - " + cit.Value;
                        return sit;
                    }
                }
            }
            if (tt != null && tt.LengthChar == 1) 
            {
                Pullenti.Ner.Token t1 = Pullenti.Ner.Geo.Internal.NameToken.CheckInitial(tt);
                if (t1 != null) 
                {
                    StreetItemToken res = TryParse(t1, null, false, null);
                    if (res != null) 
                    {
                        if (res.Value == null) 
                            res.Value = Pullenti.Ner.Core.MiscHelper.GetTextValueOfMetaToken(res, Pullenti.Ner.Core.GetTextAttr.No);
                        res.BeginToken = tt;
                        return res;
                    }
                }
            }
            return null;
        }
        internal static List<StreetItemToken> TryParseSpec(Pullenti.Ner.Token t, StreetItemToken prev)
        {
            if (t == null) 
                return null;
            List<StreetItemToken> res = null;
            StreetItemToken sit;
            if (t.GetReferent() is Pullenti.Ner.Date.DateReferent) 
            {
                Pullenti.Ner.Date.DateReferent dr = t.GetReferent() as Pullenti.Ner.Date.DateReferent;
                if (!((t as Pullenti.Ner.ReferentToken).BeginToken is Pullenti.Ner.NumberToken)) 
                    return null;
                if (dr.Year == 0 && dr.Day > 0 && dr.Month > 0) 
                {
                    res = new List<StreetItemToken>();
                    res.Add(new StreetItemToken(t, t) { Typ = StreetItemType.Number, NumberHasPrefix = true, NumberType = Pullenti.Ner.NumberSpellingType.Digit, Value = dr.Day.ToString() });
                    string tmp = dr.ToStringEx(false, t.Morph.Language, 0);
                    int i = tmp.IndexOf(' ');
                    res.Add((sit = new StreetItemToken(t, t) { Typ = StreetItemType.StdName, Value = tmp.Substring(i + 1).ToUpper() }));
                    sit.Chars.IsCapitalUpper = true;
                    return res;
                }
                if (dr.Year > 0 && dr.Month == 0) 
                {
                    res = new List<StreetItemToken>();
                    res.Add(new StreetItemToken(t, t) { Typ = StreetItemType.Number, NumberHasPrefix = true, NumberType = Pullenti.Ner.NumberSpellingType.Digit, Value = dr.Year.ToString() });
                    res.Add((sit = new StreetItemToken(t, t) { Typ = StreetItemType.StdName, Value = (t.Morph.Language.IsUa ? "РОКУ" : "ГОДА") }));
                    sit.Chars.IsCapitalUpper = true;
                    return res;
                }
                return null;
            }
            if (prev != null && prev.Typ == StreetItemType.Age) 
            {
                res = new List<StreetItemToken>();
                if (t.GetReferent() is Pullenti.Ner.Geo.GeoReferent) 
                    res.Add((sit = new StreetItemToken(t, t) { Typ = StreetItemType.Name, Value = t.GetSourceText().ToUpper(), AltValue = t.GetReferent().ToStringEx(true, t.Kit.BaseLanguage, 0).ToUpper() }));
                else if (t.IsValue("ГОРОД", null) || t.IsValue("МІСТО", null)) 
                    res.Add((sit = new StreetItemToken(t, t) { Typ = StreetItemType.Name, Value = "ГОРОДА" }));
                else 
                    return null;
                return res;
            }
            if (prev != null && prev.Typ == StreetItemType.Noun) 
            {
                Pullenti.Ner.Geo.Internal.NumToken num = Pullenti.Ner.Geo.Internal.NumToken.TryParse(t, Pullenti.Ner.Geo.Internal.GeoTokenType.Street);
                if (num != null) 
                {
                    res = new List<StreetItemToken>();
                    res.Add((sit = new StreetItemToken(num.BeginToken, num.EndToken) { Typ = StreetItemType.Number, Value = num.Value }));
                    return res;
                }
            }
            bool canBeRoad = false;
            if (prev != null && prev.IsRoad && (t.WhitespacesBeforeCount < 3)) 
                canBeRoad = true;
            else if ((prev == null && t.Next != null && t.Next.IsHiphen) && Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(t)) 
            {
                int cou = 5;
                for (Pullenti.Ner.Token tt = t.Next; tt != null && cou > 0; tt = tt.Next,cou--) 
                {
                    if (tt.WhitespacesBeforeCount > 3) 
                        break;
                    if ((tt is Pullenti.Ner.NumberToken) || tt.IsComma) 
                        break;
                    StreetItemToken sit1 = TryParse(tt, null, false, null);
                    if (sit1 != null && sit1.Typ == StreetItemType.Noun) 
                    {
                        if (sit1.IsRoad) 
                            canBeRoad = true;
                        break;
                    }
                }
            }
            if (canBeRoad) 
            {
                List<string> vals = null;
                Pullenti.Ner.Token t1 = null;
                bool br = false;
                for (Pullenti.Ner.Token tt = t; tt != null; tt = tt.Next) 
                {
                    if (tt.WhitespacesBeforeCount > 3) 
                        break;
                    if (Pullenti.Ner.Core.BracketHelper.IsBracket(tt, false)) 
                    {
                        if (tt == t) 
                        {
                            br = true;
                            continue;
                        }
                        break;
                    }
                    string val = null;
                    if (tt.GetReferent() is Pullenti.Ner.Geo.GeoReferent) 
                    {
                        Pullenti.Ner.ReferentToken rt = tt as Pullenti.Ner.ReferentToken;
                        if (rt.BeginToken == rt.EndToken && (rt.EndToken is Pullenti.Ner.TextToken)) 
                            val = (rt.EndToken as Pullenti.Ner.TextToken).Term;
                        else 
                            val = tt.GetReferent().ToStringEx(true, tt.Kit.BaseLanguage, 0).ToUpper();
                        t1 = tt;
                    }
                    else if ((tt is Pullenti.Ner.TextToken) && tt.Chars.IsCapitalUpper) 
                    {
                        Pullenti.Ner.Geo.Internal.CityItemToken cit = Pullenti.Ner.Geo.Internal.CityItemToken.TryParse(tt, null, true, null);
                        if (cit != null && cit.OrgRef == null && ((cit.Typ == Pullenti.Ner.Geo.Internal.CityItemToken.ItemType.ProperName || cit.Typ == Pullenti.Ner.Geo.Internal.CityItemToken.ItemType.City))) 
                        {
                            val = cit.Value ?? cit.OntoItem.CanonicText;
                            t1 = (tt = cit.EndToken);
                        }
                        else 
                            break;
                    }
                    else 
                        break;
                    if (vals == null) 
                        vals = new List<string>();
                    if (val.IndexOf('-') > 0 && (tt is Pullenti.Ner.TextToken)) 
                        vals.AddRange(val.Split('-'));
                    else 
                        vals.Add(val);
                    if (tt.Next != null && tt.Next.IsHiphen) 
                        tt = tt.Next;
                    else 
                        break;
                }
                if (vals != null) 
                {
                    bool ok = false;
                    if (vals.Count > 1) 
                        ok = true;
                    else if (Pullenti.Ner.Geo.Internal.MiscLocationHelper.CheckGeoObjectBefore(t, false)) 
                        ok = true;
                    else 
                    {
                        StreetItemToken sit1 = TryParse(t1.Next, null, false, null);
                        if (sit1 != null && sit1.Typ == StreetItemType.Number && sit1.IsNumberKm) 
                            ok = true;
                    }
                    if (ok) 
                    {
                        if (br) 
                        {
                            if (Pullenti.Ner.Core.BracketHelper.IsBracket(t1.Next, false)) 
                                t1 = t1.Next;
                        }
                        res = new List<StreetItemToken>();
                        if (prev != null) 
                        {
                            prev.NounIsDoubtCoef = 0;
                            prev.IsAbridge = false;
                        }
                        res.Add((sit = new StreetItemToken(t, t1) { Typ = StreetItemType.Name }));
                        if (vals.Count == 1) 
                            sit.Value = vals[0];
                        else if (vals.Count == 2) 
                        {
                            sit.Value = string.Format("{0} - {1}", vals[0], vals[1]);
                            sit.AltValue = string.Format("{0} - {1}", vals[1], vals[0]);
                        }
                        else if (vals.Count == 3) 
                        {
                            sit.Value = string.Format("{0} - {1} - {2}", vals[0], vals[1], vals[2]);
                            sit.AltValue = string.Format("{0} - {1} - {2}", vals[2], vals[1], vals[0]);
                        }
                        else if (vals.Count == 4) 
                        {
                            sit.Value = string.Format("{0} - {1} - {2} - {3}", vals[0], vals[1], vals[2], vals[3]);
                            sit.AltValue = string.Format("{0} - {1} - {2} - {3}", vals[3], vals[2], vals[1], vals[0]);
                        }
                        else 
                            return null;
                        return res;
                    }
                }
                if (((t is Pullenti.Ner.TextToken) && t.LengthChar == 1 && t.Chars.IsLetter) && t.Next != null) 
                {
                    if (t.IsValue("К", null) || t.IsValue("Д", null)) 
                        return null;
                    Pullenti.Ner.Token tt = t.Next;
                    if (tt.IsHiphen && tt.Next != null) 
                        tt = tt.Next;
                    if (tt is Pullenti.Ner.NumberToken) 
                    {
                        res = new List<StreetItemToken>();
                        prev.NounIsDoubtCoef = 0;
                        res.Add((sit = new StreetItemToken(t, tt) { Typ = StreetItemType.Name }));
                        char ch = (t as Pullenti.Ner.TextToken).Term[0];
                        char ch0 = Pullenti.Morph.LanguageHelper.GetCyrForLat(ch);
                        if (ch0 != 0) 
                            ch = ch0;
                        sit.Value = string.Format("{0}{1}", ch, (tt as Pullenti.Ner.NumberToken).Value);
                        sit.IsRoadName = true;
                        tt = tt.Next;
                        if (tt != null && tt.IsHiphen && (tt.Next is Pullenti.Ner.NumberToken)) 
                        {
                            sit.EndToken = tt.Next;
                            tt = tt.Next.Next;
                        }
                        Pullenti.Ner.Core.BracketSequenceToken br1 = Pullenti.Ner.Core.BracketHelper.TryParse(tt, Pullenti.Ner.Core.BracketParseAttr.No, 100);
                        if (br1 != null && (br1.LengthChar < 15)) 
                        {
                            sit.EndToken = br1.EndToken;
                            sit.AltValue = Pullenti.Ner.Core.MiscHelper.GetTextValue(tt.Next, sit.EndToken.Previous, Pullenti.Ner.Core.GetTextAttr.No);
                        }
                        else if (tt != null && tt.LengthChar > 2 && !tt.Chars.IsAllLower) 
                        {
                            if ((((((tt.IsValue("ДОН", null) || tt.IsValue("КАВКАЗ", null) || tt.IsValue("УРАЛ", null)) || tt.IsValue("БЕЛАРУСЬ", null) || tt.IsValue("УКРАИНА", null)) || tt.IsValue("КРЫМ", null) || tt.IsValue("ВОЛГА", null)) || tt.IsValue("ХОЛМОГОРЫ", null) || tt.IsValue("БАЛТИЯ", null)) || tt.IsValue("РОССИЯ", null) || tt.IsValue("НЕВА", null)) || tt.IsValue("КОЛА", null) || tt.IsValue("КАСПИЙ", null)) 
                            {
                                sit.EndToken = tt;
                                sit.AltValue = Pullenti.Ner.Core.MiscHelper.GetTextValue(tt, tt, Pullenti.Ner.Core.GetTextAttr.No);
                            }
                            else 
                            {
                                List<StreetItemToken> nnn = TryParseSpec(tt, prev);
                                if (nnn != null && nnn.Count == 1 && nnn[0].Typ == StreetItemType.Name) 
                                {
                                    sit.EndToken = nnn[0].EndToken;
                                    sit.AltValue = nnn[0].Value;
                                    sit.AltValue2 = nnn[0].AltValue;
                                }
                            }
                        }
                        return res;
                    }
                }
            }
            return null;
        }
        static StreetItemToken _tryAttachRoadNum(Pullenti.Ner.Token t)
        {
            if (t == null) 
                return null;
            if (!t.Chars.IsLetter || t.LengthChar != 1) 
                return null;
            Pullenti.Ner.Token tt = t.Next;
            if (tt != null && tt.IsHiphen) 
                tt = tt.Next;
            if (!(tt is Pullenti.Ner.NumberToken)) 
                return null;
            StreetItemToken res = new StreetItemToken(t, tt) { Typ = StreetItemType.Name };
            res.Value = string.Format("{0}{1}", t.GetSourceText().ToUpper(), (tt as Pullenti.Ner.NumberToken).Value);
            return res;
        }
    }
}