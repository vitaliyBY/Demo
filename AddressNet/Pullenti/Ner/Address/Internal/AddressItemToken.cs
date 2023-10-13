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

namespace Pullenti.Ner.Address.Internal
{
    public class AddressItemToken : Pullenti.Ner.MetaToken
    {
        public static List<AddressItemToken> TryParseList(Pullenti.Ner.Token t, int maxCount = 20)
        {
            if (t == null) 
                return null;
            Pullenti.Ner.Geo.Internal.GeoAnalyzerData ad = Pullenti.Ner.Geo.GeoAnalyzer.GetData(t);
            if (ad != null) 
            {
                if (ad.Level > 0) 
                    return null;
                ad.Level++;
            }
            List<AddressItemToken> res = TryParseListInt(t, maxCount);
            if (ad != null) 
                ad.Level--;
            if (res != null && res.Count == 0) 
                return null;
            return res;
        }
        static List<AddressItemToken> TryParseListInt(Pullenti.Ner.Token t, int maxCount = 20)
        {
            if (t is Pullenti.Ner.NumberToken) 
            {
                if ((t as Pullenti.Ner.NumberToken).IntValue == null) 
                    return null;
                int v = (t as Pullenti.Ner.NumberToken).IntValue.Value;
                if ((v < 100000) || v >= 10000000) 
                {
                    if ((t as Pullenti.Ner.NumberToken).Typ == Pullenti.Ner.NumberSpellingType.Digit && !t.Morph.Class.IsAdjective) 
                    {
                        if (t.Next == null || (t.Next is Pullenti.Ner.NumberToken)) 
                        {
                            if (t.Previous == null || !t.Previous.Morph.Class.IsPreposition) 
                                return null;
                        }
                    }
                }
            }
            AddressItemToken it = TryParse(t, false, null, null);
            if (it == null) 
                return null;
            if (it.Typ == AddressItemType.Number) 
                return null;
            if (it.Typ == AddressItemType.Kilometer && (it.BeginToken.Previous is Pullenti.Ner.NumberToken)) 
            {
                it = it.Clone();
                it.BeginToken = it.BeginToken.Previous;
                it.Value = (it.BeginToken as Pullenti.Ner.NumberToken).Value.ToString();
                if (it.BeginToken.Previous != null && it.BeginToken.Previous.Morph.Class.IsPreposition) 
                    it.BeginToken = it.BeginToken.Previous;
            }
            if (it.Typ == AddressItemType.Street && it.RefToken != null && !Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(it)) 
                return null;
            List<AddressItemToken> res = new List<AddressItemToken>();
            res.Add(it);
            if (it.AltTyp != null) 
                res.Add(it.AltTyp);
            bool pref = it.Typ == AddressItemType.Prefix;
            for (t = it.EndToken.Next; t != null; t = t.Next) 
            {
                if (maxCount > 0 && res.Count >= maxCount) 
                    break;
                AddressItemToken last = res[res.Count - 1];
                if (res.Count > 1) 
                {
                    if (last.IsNewlineBefore && res[res.Count - 2].Typ != AddressItemType.Prefix) 
                    {
                        int i;
                        for (i = 0; i < (res.Count - 1); i++) 
                        {
                            if (res[i].Typ == last.Typ) 
                            {
                                if (i == (res.Count - 2) && ((last.Typ == AddressItemType.City || last.Typ == AddressItemType.Region))) 
                                {
                                    int jj;
                                    for (jj = 0; jj < i; jj++) 
                                    {
                                        if ((res[jj].Typ != AddressItemType.Prefix && res[jj].Typ != AddressItemType.Zip && res[jj].Typ != AddressItemType.Region) && res[jj].Typ != AddressItemType.Country) 
                                            break;
                                    }
                                    if (jj >= i) 
                                        continue;
                                }
                                break;
                            }
                        }
                        if ((i < (res.Count - 1)) || last.Typ == AddressItemType.Zip) 
                        {
                            res.Remove(last);
                            break;
                        }
                    }
                }
                if (t.IsTableControlChar) 
                    break;
                if (t.IsChar(',') || t.IsChar('|')) 
                    continue;
                if (t.IsValue("ДУБЛЬ", null)) 
                    continue;
                if (t.IsCharOf("\\/")) 
                {
                    if (t.IsNewlineBefore || t.IsNewlineAfter) 
                        break;
                    if (t.Previous != null && t.Previous.IsComma) 
                        continue;
                    if (last.Typ == AddressItemType.Street && last.IsDoubt) 
                        break;
                    res.Add(new AddressItemToken(AddressItemType.Detail, t, t) { DetailType = Pullenti.Ner.Address.AddressDetailType.Cross });
                    continue;
                }
                if (t.IsCharOf(":;") && Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(t)) 
                    continue;
                if (Pullenti.Ner.Core.BracketHelper.IsBracket(t, false) && t.Next != null && t.Next.IsComma) 
                    continue;
                if (t.IsChar('.')) 
                {
                    if (t.IsNewlineAfter) 
                    {
                        if (last.Typ == AddressItemType.City) 
                        {
                            AddressItemToken next = TryParse(t.Next, false, null, null);
                            if (next != null && next.Typ == AddressItemType.Street) 
                                continue;
                        }
                        break;
                    }
                    if (t.Previous != null && t.Previous.IsChar('.')) 
                    {
                        if (t.Previous.Previous != null && t.Previous.Previous.IsChar('.')) 
                            break;
                    }
                    continue;
                }
                if (t.IsHiphen || t.IsChar('_')) 
                {
                    if (((it.Typ == AddressItemType.Number || it.Typ == AddressItemType.Street)) && (t.Next is Pullenti.Ner.NumberToken)) 
                        continue;
                    if (Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(it)) 
                        continue;
                    if (it.Typ == AddressItemType.City) 
                        continue;
                }
                if (it.Typ == AddressItemType.Detail && it.DetailType == Pullenti.Ner.Address.AddressDetailType.Cross) 
                {
                    AddressItemToken str1 = TryParse(t, true, null, null);
                    if (str1 != null && str1.Typ == AddressItemType.Street) 
                    {
                        if (str1.EndToken.Next != null && ((str1.EndToken.Next.IsAnd || str1.EndToken.Next.IsHiphen))) 
                        {
                            AddressItemToken str2 = TryParse(str1.EndToken.Next.Next, true, null, null);
                            if (str2 == null || str2.Typ != AddressItemType.Street) 
                            {
                                str2 = StreetDefineHelper.TryParseSecondStreet(str1.BeginToken, str1.EndToken.Next.Next);
                                if (str2 != null && str2.IsDoubt) 
                                {
                                    str2 = str2.Clone();
                                    str2.IsDoubt = false;
                                }
                            }
                            if (str2 != null && str2.Typ == AddressItemType.Street) 
                            {
                                res.Add(str1);
                                res.Add(str2);
                                t = str2.EndToken;
                                it = str2;
                                continue;
                            }
                        }
                    }
                }
                bool pre = pref;
                if (it.Typ == AddressItemType.Kilometer || ((it.Typ == AddressItemType.House && it.Value != null))) 
                {
                    if (!t.IsNewlineBefore) 
                        pre = true;
                }
                AddressItemToken it0 = TryParse(t, pre, it, null);
                if (it0 == null) 
                {
                    bool hous = false;
                    Pullenti.Ner.Token tt = GotoEndOfAddress(t.Previous, out hous);
                    if (tt != null && tt.EndChar >= t.EndChar && tt.Next != null) 
                    {
                        if (tt.Next.IsComma) 
                            tt = tt.Next;
                        it0 = TryParse(tt.Next, pre, it, null);
                        if (it0 == null && hous && it.Typ == AddressItemType.Street) 
                            res.Add(new AddressItemToken(AddressItemType.House, t, tt) { Value = "0" });
                    }
                }
                if (it0 == null && t.GetMorphClassInDictionary().IsPreposition && (t.WhitespacesAfterCount < 3)) 
                {
                    it0 = TryParse(t.Next, pre, it, null);
                    if (it0 != null) 
                    {
                        if (it0.Typ == AddressItemType.Number) 
                            it0 = null;
                        else if (it0.Typ == AddressItemType.Building && t.Next.IsValue("СТ", null)) 
                            it0 = null;
                    }
                }
                if (it0 == null) 
                {
                    if (Pullenti.Ner.Core.BracketHelper.CanBeEndOfSequence(t, true, null, false) && last.Typ == AddressItemType.Street) 
                        continue;
                    if (t.IsCharOf("\\/") && last.Typ == AddressItemType.Street) 
                        continue;
                }
                if (((it0 == null && t.IsChar('(') && (t.Next is Pullenti.Ner.ReferentToken)) && (t.Next.GetReferent() is Pullenti.Ner.Geo.GeoReferent) && t.Next.Next != null) && t.Next.Next.IsChar(')')) 
                {
                    it0 = TryParse(t.Next, pre, it, null);
                    if (it0 != null) 
                    {
                        it0 = it0.Clone();
                        it0.BeginToken = t;
                        it0.EndToken = it0.EndToken.Next;
                        Pullenti.Ner.Geo.GeoReferent geo0 = t.Next.GetReferent() as Pullenti.Ner.Geo.GeoReferent;
                        if (geo0.Higher == null) 
                        {
                            for (int kk = res.Count - 1; kk >= 0; kk--) 
                            {
                                if (res[kk].Typ == AddressItemType.City && (res[kk].Referent is Pullenti.Ner.Geo.GeoReferent)) 
                                {
                                    if (Pullenti.Ner.Geo.Internal.GeoOwnerHelper.CanBeHigher(res[kk].Referent as Pullenti.Ner.Geo.GeoReferent, geo0, null, null) || ((geo0.FindSlot(Pullenti.Ner.Geo.GeoReferent.ATTR_TYPE, "город", true) != null && res[kk].Referent.FindSlot(Pullenti.Ner.Geo.GeoReferent.ATTR_TYPE, "город", true) != null))) 
                                    {
                                        geo0.Higher = res[kk].Referent as Pullenti.Ner.Geo.GeoReferent;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                if (it0 == null) 
                {
                    if (t.NewlinesBeforeCount > 2) 
                        break;
                    if (it.Typ == AddressItemType.PostOfficeBox) 
                        break;
                    if (t.IsHiphen && t.Next != null && t.Next.IsComma) 
                        continue;
                    if (t.IsHiphen && (t.Next is Pullenti.Ner.NumberToken) && Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(t)) 
                        continue;
                    if (t.IsValue("НЕТ", null) || t.IsValue("ТЕР", null) || t.IsValue("ТЕРРИТОРИЯ", null)) 
                        continue;
                    Pullenti.Ner.Token tt1 = StreetItemToken.CheckStdName(t);
                    if (tt1 != null) 
                    {
                        t = tt1;
                        continue;
                    }
                    if (t.Morph.Class.IsPreposition) 
                    {
                        it0 = TryParse(t.Next, false, it, null);
                        if (it0 != null && it0.Typ == AddressItemType.Building && it0.BeginToken.IsValue("СТ", null)) 
                        {
                            it0 = null;
                            break;
                        }
                        if (it0 != null) 
                        {
                            if ((it0.Typ == AddressItemType.Detail && it.Typ == AddressItemType.City && it.DetailMeters > 0) && it.DetailType == Pullenti.Ner.Address.AddressDetailType.Undefined) 
                            {
                                it.DetailType = it0.DetailType;
                                t = (it.EndToken = it0.EndToken);
                                continue;
                            }
                            if ((it0.Typ == AddressItemType.House || it0.Typ == AddressItemType.Building || it0.Typ == AddressItemType.Corpus) || it0.Typ == AddressItemType.Street || it0.Typ == AddressItemType.Detail) 
                            {
                                res.Add((it = it0));
                                t = it.EndToken;
                                continue;
                            }
                        }
                    }
                    if (it.Typ == AddressItemType.House || it.Typ == AddressItemType.Building || it.Typ == AddressItemType.Number) 
                    {
                        if ((!t.IsWhitespaceBefore && t.LengthChar == 1 && t.Chars.IsLetter) && !t.IsWhitespaceAfter && (t.Next is Pullenti.Ner.NumberToken)) 
                        {
                            string ch = CorrectCharToken(t);
                            if (ch == "К" || ch == "С") 
                            {
                                it0 = new AddressItemToken((ch == "К" ? AddressItemType.Corpus : AddressItemType.Building), t, t.Next) { Value = (t.Next as Pullenti.Ner.NumberToken).Value.ToString() };
                                it = it0;
                                res.Add(it);
                                t = it.EndToken;
                                Pullenti.Ner.Token tt = t.Next;
                                if (((tt != null && !tt.IsWhitespaceBefore && tt.LengthChar == 1) && tt.Chars.IsLetter && !tt.IsWhitespaceAfter) && (tt.Next is Pullenti.Ner.NumberToken)) 
                                {
                                    ch = CorrectCharToken(tt);
                                    if (ch == "К" || ch == "С") 
                                    {
                                        it = new AddressItemToken((ch == "К" ? AddressItemType.Corpus : AddressItemType.Building), tt, tt.Next) { Value = (tt.Next as Pullenti.Ner.NumberToken).Value.ToString() };
                                        res.Add(it);
                                        t = it.EndToken;
                                    }
                                }
                                continue;
                            }
                        }
                    }
                    if (t.Morph.Class.IsPreposition) 
                    {
                        if ((((t.IsValue("У", null) || t.IsValue("ВОЗЛЕ", null) || t.IsValue("НАПРОТИВ", null)) || t.IsValue("НА", null) || t.IsValue("В", null)) || t.IsValue("ВО", null) || t.IsValue("ПО", null)) || t.IsValue("ОКОЛО", null)) 
                        {
                            if (it0 != null && it0.Typ == AddressItemType.Number) 
                                break;
                            continue;
                        }
                    }
                    if (t.Morph.Class.IsNoun) 
                    {
                        if ((t.IsValue("ДВОР", null) || t.IsValue("ПОДЪЕЗД", null) || t.IsValue("КРЫША", null)) || t.IsValue("ПОДВАЛ", null)) 
                            continue;
                    }
                    if (t.IsValue("ТЕРРИТОРИЯ", "ТЕРИТОРІЯ")) 
                        continue;
                    if (t.IsChar('(') && t.Next != null) 
                    {
                        it0 = TryParse(t.Next, pre, null, null);
                        if (it0 != null && it0.EndToken.Next != null && it0.EndToken.Next.IsChar(')')) 
                        {
                            it0 = it0.Clone();
                            it0.BeginToken = t;
                            it0.EndToken = it0.EndToken.Next;
                            it = it0;
                            res.Add(it);
                            t = it.EndToken;
                            continue;
                        }
                        List<AddressItemToken> li0 = TryParseListInt(t.Next, 3);
                        if ((li0 != null && li0.Count > 1 && li0[0].Typ != AddressItemType.Detail) && li0[li0.Count - 1].EndToken.Next != null && li0[li0.Count - 1].EndToken.Next.IsChar(')')) 
                        {
                            li0[0] = li0[0].Clone();
                            li0[0].BeginToken = t;
                            li0[li0.Count - 1] = li0[li0.Count - 1].Clone();
                            li0[li0.Count - 1].EndToken = li0[li0.Count - 1].EndToken.Next;
                            res.AddRange(li0);
                            it = li0[li0.Count - 1];
                            t = it.EndToken;
                            continue;
                        }
                        Pullenti.Ner.Core.BracketSequenceToken br = Pullenti.Ner.Core.BracketHelper.TryParse(t, Pullenti.Ner.Core.BracketParseAttr.No, 100);
                        if (br != null && (br.LengthChar < 100)) 
                        {
                            if (t.Next.IsValue("БЫВШИЙ", null) || t.Next.IsValue("БЫВШ", null)) 
                            {
                                it = new AddressItemToken(AddressItemType.Detail, t, br.EndToken);
                                res.Add(it);
                            }
                            t = br.EndToken;
                            continue;
                        }
                    }
                    bool checkKv = false;
                    if (t.IsValue("КВ", null) || t.IsValue("KB", null)) 
                    {
                        if (it.Typ == AddressItemType.Number && res.Count > 1 && res[res.Count - 2].Typ == AddressItemType.Street) 
                            checkKv = true;
                        else if ((it.Typ == AddressItemType.House || it.Typ == AddressItemType.Building || it.Typ == AddressItemType.Corpus) || it.Typ == AddressItemType.CorpusOrFlat) 
                        {
                            for (int jj = res.Count - 2; jj >= 0; jj--) 
                            {
                                if (res[jj].Typ == AddressItemType.Street || res[jj].Typ == AddressItemType.City) 
                                    checkKv = true;
                            }
                        }
                        if (checkKv) 
                        {
                            Pullenti.Ner.Token tt2 = t.Next;
                            if (tt2 != null && tt2.IsChar('.')) 
                                tt2 = tt2.Next;
                            AddressItemToken it22 = TryParsePureItem(tt2, null, null);
                            if (it22 != null && it22.Typ == AddressItemType.Number) 
                            {
                                it22 = it22.Clone();
                                it22.BeginToken = t;
                                it22.Typ = AddressItemType.Flat;
                                res.Add(it22);
                                t = it22.EndToken;
                                continue;
                            }
                        }
                    }
                    if (res[res.Count - 1].Typ == AddressItemType.City) 
                    {
                        if (((t.IsHiphen || t.IsChar('_') || t.IsValue("НЕТ", null))) && t.Next != null && t.Next.IsComma) 
                        {
                            AddressItemToken att = TryParsePureItem(t.Next.Next, null, null);
                            if (att != null) 
                            {
                                if (att.Typ == AddressItemType.House || att.Typ == AddressItemType.Building || att.Typ == AddressItemType.Corpus) 
                                {
                                    it = new AddressItemToken(AddressItemType.Street, t, t);
                                    res.Add(it);
                                    continue;
                                }
                            }
                        }
                    }
                    if (t.LengthChar == 2 && (t is Pullenti.Ner.TextToken) && t.Chars.IsAllUpper) 
                    {
                        string term = (t as Pullenti.Ner.TextToken).Term;
                        if (!string.IsNullOrEmpty(term) && term[0] == 'Р') 
                            continue;
                    }
                    break;
                }
                if (t.WhitespacesBeforeCount > 15) 
                {
                    if (it0.Typ == AddressItemType.Street && last.Typ == AddressItemType.City) 
                    {
                    }
                    else 
                        break;
                }
                if (t.IsNewlineBefore && it0.Typ == AddressItemType.Street && it0.RefToken != null) 
                {
                    if (!it0.RefTokenIsGsk) 
                        break;
                }
                if (it0.Typ == AddressItemType.Street && t.IsValue("КВ", null)) 
                {
                    if (it != null) 
                    {
                        if (it.Typ == AddressItemType.House || it.Typ == AddressItemType.Building || it.Typ == AddressItemType.Corpus) 
                        {
                            AddressItemToken it2 = TryParsePureItem(t, null, null);
                            if (it2 != null && it2.Typ == AddressItemType.Flat) 
                                it0 = it2;
                        }
                    }
                }
                if (it0.Typ == AddressItemType.Prefix) 
                    break;
                if (it0.Typ == AddressItemType.Number) 
                {
                    if (string.IsNullOrEmpty(it0.Value)) 
                        break;
                    if (!char.IsDigit(it0.Value[0])) 
                        break;
                    int cou = 0;
                    for (int i = res.Count - 1; i >= 0; i--) 
                    {
                        if (res[i].Typ == AddressItemType.Number) 
                            cou++;
                        else 
                            break;
                    }
                    if (cou > 5) 
                        break;
                    if (it.IsDoubt && t.IsNewlineBefore) 
                        break;
                }
                if (it0.Typ == AddressItemType.CorpusOrFlat && it != null && it.Typ == AddressItemType.Flat) 
                    it0.Typ = AddressItemType.Room;
                if (((((it0.Typ == AddressItemType.Floor || it0.Typ == AddressItemType.Potch || it0.Typ == AddressItemType.Block) || it0.Typ == AddressItemType.Kilometer)) && string.IsNullOrEmpty(it0.Value) && it.Typ == AddressItemType.Number) && it.EndToken.Next == it0.BeginToken) 
                {
                    it = it.Clone();
                    res[res.Count - 1] = it;
                    it.Typ = it0.Typ;
                    it.EndToken = it0.EndToken;
                }
                else if ((((it.Typ == AddressItemType.Floor || it.Typ == AddressItemType.Potch)) && string.IsNullOrEmpty(it.Value) && it0.Typ == AddressItemType.Number) && it.EndToken.Next == it0.BeginToken) 
                {
                    it = it.Clone();
                    res[res.Count - 1] = it;
                    it.Value = it0.Value;
                    it.EndToken = it0.EndToken;
                }
                else 
                {
                    it = it0;
                    res.Add(it);
                    if (it.AltTyp != null) 
                        res.Add(it.AltTyp);
                }
                t = it.EndToken;
            }
            if (res.Count > 0) 
            {
                it = res[res.Count - 1];
                AddressItemToken it0 = (res.Count > 1 ? res[res.Count - 2] : null);
                if (it.Typ == AddressItemType.Number && it0 != null && it0.RefToken != null) 
                {
                    foreach (Pullenti.Ner.Slot s in it0.RefToken.Referent.Slots) 
                    {
                        if (s.TypeName == "TYPE") 
                        {
                            string ss = s.Value as string;
                            if (ss.Contains("гараж") || ((ss[0] == 'Г' && ss[ss.Length - 1] == 'К'))) 
                            {
                                if (it0.RefToken.Referent.FindSlot("NAME", "РОСАТОМ", true) != null) 
                                    break;
                                it.Typ = AddressItemType.Box;
                                break;
                            }
                        }
                    }
                }
                if (it.Typ == AddressItemType.Number || it.Typ == AddressItemType.Zip) 
                {
                    bool del = false;
                    if (it.BeginToken.Previous != null && it.BeginToken.Previous.Morph.Class.IsPreposition) 
                        del = true;
                    else if (it.Morph.Class.IsNoun) 
                        del = true;
                    if ((!del && it.EndToken.WhitespacesAfterCount == 1 && it.WhitespacesBeforeCount > 0) && it.Typ == AddressItemType.Number) 
                    {
                        Pullenti.Ner.Core.NounPhraseToken npt = Pullenti.Ner.Geo.Internal.MiscLocationHelper.TryParseNpt(it.EndToken.Next);
                        if (npt != null) 
                            del = true;
                    }
                    if (del) 
                        res.RemoveAt(res.Count - 1);
                    else if ((it.Typ == AddressItemType.Number && it0 != null && it0.Typ == AddressItemType.Street) && it0.RefToken == null) 
                    {
                        if (it.BeginToken.Previous.IsChar(',') || it.IsNewlineAfter) 
                        {
                            it = it.Clone();
                            res[res.Count - 1] = it;
                            it.Typ = AddressItemType.House;
                            it.IsDoubt = true;
                        }
                    }
                }
            }
            if (res.Count == 0) 
                return null;
            foreach (AddressItemToken r in res) 
            {
                if (r.Typ == AddressItemType.City || r.Typ == AddressItemType.Street) 
                {
                    AddressItemToken ty = _findAddrTyp(r.BeginToken, r.EndChar, 0);
                    if (ty != null) 
                    {
                        if (r.DetailType == Pullenti.Ner.Address.AddressDetailType.Undefined) 
                            r.DetailType = ty.DetailType;
                        if (ty.DetailMeters > 0) 
                            r.DetailMeters = ty.DetailMeters;
                        if (ty.DetailParam != null) 
                            r.DetailParam = ty.DetailParam;
                    }
                }
            }
            for (int i = 0; i < (res.Count - 2); i++) 
            {
                if (res[i].Typ == AddressItemType.Street && res[i + 1].Typ == AddressItemType.Number) 
                {
                    if ((res[i + 2].Typ == AddressItemType.Building || res[i + 2].Typ == AddressItemType.Corpus || res[i + 2].Typ == AddressItemType.Office) || res[i + 2].Typ == AddressItemType.Flat) 
                    {
                        res[i + 1] = res[i + 1].Clone();
                        res[i + 1].Typ = AddressItemType.House;
                    }
                }
            }
            for (int i = 0; i < (res.Count - 1); i++) 
            {
                if (res[i].Typ == AddressItemType.Street && res[i + 1].Typ == AddressItemType.City && (res[i].Referent is Pullenti.Ner.Address.StreetReferent)) 
                {
                    Pullenti.Ner.Address.StreetReferent sr = res[i].Referent as Pullenti.Ner.Address.StreetReferent;
                    if (sr.Slots.Count != 2 || sr.Kind != Pullenti.Ner.Address.StreetKind.Area || sr.Typs.Count != 1) 
                        continue;
                    if (i == 0 && Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(res[0])) 
                    {
                    }
                    else if (i > 0 && res[i - 1].Typ == AddressItemType.City) 
                    {
                    }
                    else 
                        continue;
                    Pullenti.Ner.Token tt = res[i + 1].BeginToken;
                    if (tt is Pullenti.Ner.ReferentToken) 
                        tt = (tt as Pullenti.Ner.ReferentToken).BeginToken;
                    Pullenti.Ner.Core.NounPhraseToken npt = Pullenti.Ner.Core.NounPhraseHelper.TryParse(tt, Pullenti.Ner.Core.NounPhraseParseAttr.No, 0, null);
                    if (npt != null && npt.EndChar == res[i + 1].EndChar) 
                    {
                        res[i].EndToken = res[i + 1].EndToken;
                        sr.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NAME, npt.GetNormalCaseText(null, Pullenti.Morph.MorphNumber.Undefined, Pullenti.Morph.MorphGender.Undefined, false), false, 0);
                        res.RemoveAt(i + 1);
                        break;
                    }
                }
            }
            for (int i = 0; i < (res.Count - 1); i++) 
            {
                if (res[i].Typ == AddressItemType.Building && res[i].BeginToken == res[i].EndToken && res[i].BeginToken.LengthChar == 1) 
                {
                    if (res[i + 1].Typ == AddressItemType.City) 
                    {
                        res.RemoveAt(i);
                        i--;
                    }
                }
            }
            for (int i = 0; i < (res.Count - 1); i++) 
            {
                if (res[i].Typ == AddressItemType.Flat && res[i + 1].Typ == AddressItemType.Street && (res[i + 1].RefToken is Pullenti.Ner.Geo.Internal.OrgItemToken)) 
                {
                    string str = res[i + 1].RefToken.ToString().ToUpper();
                    if (str.Contains("ЛЕСНИЧ")) 
                    {
                        res[i + 1].BeginToken = res[i].BeginToken;
                        res[i + 1].Referent.AddSlot("NUMBER", res[i].Value, false, 0);
                        res.RemoveAt(i);
                        break;
                    }
                }
            }
            for (int i = 0; i < (res.Count - 1); i++) 
            {
                if ((res[i].Typ == AddressItemType.Street && (res[i].Referent is Pullenti.Ner.Address.StreetReferent) && res[i + 1].Typ == AddressItemType.Street) && (res[i + 1].RefToken is Pullenti.Ner.Geo.Internal.OrgItemToken)) 
                {
                    Pullenti.Ner.Address.StreetReferent ss = res[i].Referent as Pullenti.Ner.Address.StreetReferent;
                    if (ss.Numbers == null || ss.Names.Count > 0) 
                        continue;
                    if (!ss.ToString().Contains("квартал")) 
                        continue;
                    string str = res[i + 1].RefToken.ToString().ToUpper();
                    if (!str.Contains("ЛЕСНИЧ")) 
                        continue;
                    res[i + 1].BeginToken = res[i].BeginToken;
                    res[i + 1].Referent.AddSlot("NUMBER", ss.Numbers, false, 0);
                    res.RemoveAt(i);
                    break;
                }
            }
            for (int i = 0; i < (res.Count - 1); i++) 
            {
                if ((res[i].Typ == AddressItemType.Street && res[i + 1].Typ == AddressItemType.Kilometer && (res[i].Referent is Pullenti.Ner.Address.StreetReferent)) && (res[i].Referent as Pullenti.Ner.Address.StreetReferent).Numbers == null) 
                {
                    res[i] = res[i].Clone();
                    (res[i].Referent as Pullenti.Ner.Address.StreetReferent).Numbers = res[i + 1].Value + "км";
                    res[i].EndToken = res[i + 1].EndToken;
                    res.RemoveAt(i + 1);
                }
            }
            for (int i = 0; i < (res.Count - 1); i++) 
            {
                if ((res[i + 1].Typ == AddressItemType.Street && res[i].Typ == AddressItemType.Kilometer && (res[i + 1].Referent is Pullenti.Ner.Address.StreetReferent)) && (res[i + 1].Referent as Pullenti.Ner.Address.StreetReferent).Numbers == null) 
                {
                    res[i + 1] = res[i + 1].Clone();
                    (res[i + 1].Referent as Pullenti.Ner.Address.StreetReferent).Numbers = res[i].Value + "км";
                    res[i + 1].BeginToken = res[i].BeginToken;
                    res.RemoveAt(i);
                    break;
                }
            }
            for (int i = 0; i < (res.Count - 1); i++) 
            {
                if (res[i].Typ == AddressItemType.Building && res[i + 1].Typ == AddressItemType.Building && (res[i].BeginToken is Pullenti.Ner.TextToken)) 
                {
                    if ((res[i].BeginToken as Pullenti.Ner.TextToken).Term.StartsWith("ЗД")) 
                    {
                        res[i] = res[i].Clone();
                        res[i].Typ = AddressItemType.House;
                    }
                }
            }
            for (int i = 0; i < res.Count; i++) 
            {
                if (res[i].Typ == AddressItemType.Part) 
                {
                    if (i > 0 && ((res[i - 1].Typ == AddressItemType.House || res[i - 1].Typ == AddressItemType.Plot))) 
                        continue;
                    if (((i + 1) < res.Count) && ((res[i + 1].Typ == AddressItemType.House || res[i + 1].Typ == AddressItemType.Plot))) 
                        continue;
                    if (i == 0) 
                        return null;
                    res.RemoveRange(i, res.Count - i);
                    break;
                }
                else if ((res[i].Typ == AddressItemType.NoNumber && i == (res.Count - 1) && i > 0) && res[i - 1].Typ == AddressItemType.City) 
                {
                    res[i] = res[i].Clone();
                    res[i].Typ = AddressItemType.House;
                }
            }
            if (res.Count > 0 && Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(res[0])) 
            {
                for (int i = 0; i < (res.Count - 1); i++) 
                {
                    for (int j = i + 1; j < res.Count; j++) 
                    {
                        if (res[j].IsNewlineBefore) 
                            break;
                        if (res[i].Typ == res[j].Typ && (res[i].Referent is Pullenti.Ner.Geo.GeoReferent) && res[j].Referent == res[i].Referent) 
                        {
                            res.RemoveAt(j);
                            j--;
                        }
                    }
                }
            }
            while (res.Count > 0) 
            {
                AddressItemToken last = res[res.Count - 1];
                if (last.Typ == AddressItemType.Detail && last.DetailType == Pullenti.Ner.Address.AddressDetailType.Cross && last.LengthChar == 1) 
                {
                    res.RemoveAt(res.Count - 1);
                    continue;
                }
                if (last.Typ == AddressItemType.City && res.Count > 4) 
                {
                    bool ok = false;
                    for (int ii = 0; ii < 3; ii++) 
                    {
                        if (res[ii].Typ == AddressItemType.City) 
                            ok = true;
                    }
                    if (ok) 
                    {
                        res.RemoveAt(res.Count - 1);
                        continue;
                    }
                }
                if (last.Typ != AddressItemType.Street || !(last.RefToken is Pullenti.Ner.Geo.Internal.OrgItemToken)) 
                    break;
                if ((last.RefToken as Pullenti.Ner.Geo.Internal.OrgItemToken).IsGsk || (last.RefToken as Pullenti.Ner.Geo.Internal.OrgItemToken).HasTerrKeyword) 
                    break;
                if (Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(last)) 
                    break;
                res.RemoveAt(res.Count - 1);
            }
            if (res.Count > 2 && Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(res[0])) 
            {
                for (int i = 1; i < res.Count; i++) 
                {
                    if (((res[i - 1].Typ == AddressItemType.Street || res[i - 1].Typ == AddressItemType.City)) && res[i].Typ == AddressItemType.Street) 
                    {
                        Pullenti.Ner.Address.StreetReferent sr = res[i].Referent as Pullenti.Ner.Address.StreetReferent;
                        if (sr == null) 
                            continue;
                        if ((sr.Numbers == null || sr.Names.Count > 0 || sr.Typs.Count != 1) || sr.Typs[0] != "улица") 
                            continue;
                        if ((i + 1) < res.Count) 
                            continue;
                        if (res[i - 1].Typ == AddressItemType.City) 
                        {
                            Pullenti.Ner.Geo.GeoReferent geo = res[i - 1].Referent as Pullenti.Ner.Geo.GeoReferent;
                            if (geo == null) 
                                continue;
                            if (geo.Typs.Contains("город")) 
                                continue;
                        }
                        res[i] = res[i].Clone();
                        res[i].Typ = AddressItemType.House;
                        res[i].Value = sr.Numbers;
                        res[i].Referent = null;
                    }
                }
            }
            for (int i = 0; i < (res.Count - 2); i++) 
            {
                if (res[i].Typ == AddressItemType.Region && res[i + 1].Typ == AddressItemType.Number && res[i + 2].Typ == AddressItemType.City) 
                {
                    bool ok = false;
                    for (int j = i + 3; j < res.Count; j++) 
                    {
                        if (res[j].Typ == AddressItemType.Street || res[j].Value != null) 
                            ok = true;
                    }
                    if (ok) 
                    {
                        res.RemoveAt(i + 1);
                        break;
                    }
                }
            }
            return res;
        }
        public static void Initialize()
        {
            if (m_Ontology != null) 
                return;
            StreetItemToken.Initialize();
            m_Ontology = new Pullenti.Ner.Core.TerminCollection();
            Pullenti.Ner.Core.Termin t;
            t = new Pullenti.Ner.Core.Termin("ДОМ") { Tag = AddressItemType.House };
            t.AddAbridge("Д.");
            t.AddVariant("КОТТЕДЖ", false);
            t.AddAbridge("КОТ.");
            t.AddVariant("ДАЧА", false);
            t.AddVariant("ЖИЛОЙ ДОМ", false);
            t.AddAbridge("ЖИЛ.ДОМ");
            t.AddVariant("ДО ДОМА", false);
            t.AddVariant("ДОМ ОФИЦЕРСКОГО СОСТАВА", false);
            t.AddVariant("ДОС", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("БУДИНОК") { Tag = AddressItemType.House, Lang = Pullenti.Morph.MorphLang.UA };
            t.AddAbridge("Б.");
            t.AddVariant("КОТЕДЖ", false);
            t.AddAbridge("БУД.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ВЛАДЕНИЕ") { Tag = AddressItemType.House, Tag2 = Pullenti.Ner.Address.AddressHouseType.Estate };
            t.AddAbridge("ВЛАД.");
            t.AddAbridge("ВЛД.");
            t.AddAbridge("ВЛ.");
            m_Ontology.Add(t);
            m_Owner = t;
            t = new Pullenti.Ner.Core.Termin("ДОМОВЛАДЕНИЕ") { Tag = AddressItemType.House, Tag2 = Pullenti.Ner.Address.AddressHouseType.HouseEstate };
            t.AddVariant("ДОМОВЛАДЕНИЕ", false);
            t.AddAbridge("ДВЛД.");
            t.AddAbridge("ДМВЛД.");
            t.AddVariant("ДОМОВЛ", false);
            t.AddVariant("ДОМОВА", false);
            t.AddVariant("ДОМОВЛАД", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПОДЪЕЗД ДОМА") { Tag = AddressItemType.House };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ЭТАЖ") { Tag = AddressItemType.Floor };
            t.AddAbridge("ЭТ.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПОДЪЕЗД") { Tag = AddressItemType.Potch };
            t.AddAbridge("ПОД.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("КОРПУС") { Tag = AddressItemType.Corpus };
            t.AddAbridge("КОРП.");
            t.AddAbridge("КОР.");
            t.AddAbridge("Д.КОРП.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("К") { Tag = AddressItemType.CorpusOrFlat };
            t.AddAbridge("К.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("СТРОЕНИЕ") { Tag = AddressItemType.Building };
            t.AddAbridge("СТРОЕН.");
            t.AddAbridge("СТР.");
            t.AddAbridge("СТ.");
            t.AddAbridge("ПОМ.СТР.");
            t.AddAbridge("Д.СТР.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("СООРУЖЕНИЕ") { Acronym = "РК", Tag = AddressItemType.Building, Tag2 = Pullenti.Ner.Address.AddressBuildingType.Construction };
            t.AddAbridge("СООР.");
            t.AddAbridge("СООРУЖ.");
            t.AddAbridge("СООРУЖЕН.");
            t.AddVariant("БАШНЯ", false);
            t.AddVariant("ЗДАНИЕ", false);
            t.AddAbridge("ЗД.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ЛИТЕРА") { Tag = AddressItemType.Building, Tag2 = Pullenti.Ner.Address.AddressBuildingType.Liter };
            t.AddAbridge("ЛИТ.");
            t.AddVariant("ЛИТЕР", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("УЧАСТОК") { Tag = AddressItemType.Plot };
            t.AddAbridge("УЧАСТ.");
            t.AddAbridge("УЧ.");
            t.AddAbridge("УЧ-К");
            t.AddAbridge("ДОМ УЧ.");
            t.AddAbridge("ДОМ.УЧ.");
            t.AddAbridge("У-К");
            t.AddVariant("ЗЕМЕЛЬНЫЙ УЧАСТОК", false);
            t.AddAbridge("ЗЕМ.УЧ.");
            t.AddAbridge("ЗЕМ.УЧ-К");
            t.AddAbridge("З/У");
            t.AddVariant("ЧАСТЬ ВЫДЕЛА", false);
            t.AddVariant("ВЫДЕЛ", false);
            t.AddVariant("НАДЕЛ", false);
            t.AddVariant("КОНТУР", false);
            t.AddAbridge("ЗУ");
            t.AddAbridge("ВЫД.");
            m_Ontology.Add(t);
            m_Plot = t;
            t = new Pullenti.Ner.Core.Termin("ПОЛЕ") { Tag = AddressItemType.Field };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ГЕНЕРАЛЬНЫЙ ПЛАН") { Acronym = "ГП", Tag = AddressItemType.Genplan };
            t.AddVariant("ГЕНПЛАН", false);
            t.AddAbridge("ГЕН.ПЛАН");
            t.AddAbridge("Г/П");
            t.AddAbridge("Г.П.");
            t.AddVariant("ПО ГП", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("КВАРТИРА") { Tag = AddressItemType.Flat };
            t.AddAbridge("КВАРТ.");
            t.AddAbridge("КВАР.");
            t.AddAbridge("КВ.");
            t.AddAbridge("KB.");
            t.AddAbridge("КВ-РА");
            t.AddAbridge("КВ.КОМ");
            t.AddAbridge("КВ.ОБЩ");
            t.AddAbridge("КВ.Ч.");
            t.AddAbridge("КВЮ");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ОФИС") { Tag = AddressItemType.Office };
            t.AddAbridge("ОФ.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ОФІС") { Tag = AddressItemType.Office, Lang = Pullenti.Morph.MorphLang.UA };
            t.AddAbridge("ОФ.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПАВИЛЬОН") { Tag = AddressItemType.Pavilion };
            t.AddAbridge("ПАВ.");
            t.AddVariant("ТОРГОВЫЙ ПАВИЛЬОН", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПАВІЛЬЙОН") { Tag = AddressItemType.Pavilion, Lang = Pullenti.Morph.MorphLang.UA };
            t.AddAbridge("ПАВ.");
            t.AddVariant("ТОРГОВИЙ ПАВІЛЬЙОН", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("КЛАДОВКА") { Tag = AddressItemType.Pantry };
            t.AddAbridge("КЛАД.");
            t.AddVariant("КЛАДОВАЯ", false);
            t.AddVariant("КЛАДОВОЕ ПОМЕЩЕНИЕ", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("СЕКЦИЯ") { Tag = AddressItemType.Block };
            t.AddVariant("БЛОК", false);
            t.AddVariant("БЛОК БОКС", false);
            t.AddAbridge("БЛ.");
            t.AddVariant("БЛОК ГАРАЖЕЙ", false);
            t.AddVariant("ГАРАЖНЫЙ БЛОК", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("БОКС") { Tag = AddressItemType.Box };
            t.AddVariant("ГАРАЖ", false);
            t.AddAbridge("ГАР.");
            t.AddVariant("ГАРАЖНАЯ ЯЧЕЙКА", false);
            t.AddAbridge("Г-Ж");
            t.AddVariant("ПОДЪЕЗД", false);
            t.AddAbridge("ГАРАЖ-БОКС");
            t.AddVariant("ИНДИВИДУАЛЬНЫЙ ГАРАЖ", false);
            t.AddVariant("ГАРАЖНЫЙ БОКС", false);
            t.AddAbridge("ГБ.");
            t.AddAbridge("Г.Б.");
            t.AddVariant("ЭЛЛИНГ", false);
            t.AddVariant("ЭЛИНГ", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ЧАСТЬ") { Tag = AddressItemType.Part };
            t.AddAbridge("Ч.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("СКВАЖИНА") { Tag = AddressItemType.Well };
            t.AddAbridge("СКВАЖ.");
            t.AddVariant("СКВАЖИНА ГАЗОКОНДЕНСАТНАЯ ЭКСПЛУАТАЦИОННАЯ", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПОМЕЩЕНИЕ") { Tag = AddressItemType.Space };
            t.AddVariant("ПОМЕЩЕНИЕ", false);
            t.AddAbridge("ПОМ.");
            t.AddAbridge("ПОМЕЩ.");
            t.AddVariant("НЕЖИЛОЕ ПОМЕЩЕНИЕ", false);
            t.AddAbridge("Н.П.");
            t.AddAbridge("НП");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПОДВАЛ") { Tag = AddressItemType.Space, Tag2 = 1 };
            t.AddVariant("ПОДВАЛЬНОЕ ПОМЕЩЕНИЕ", false);
            t.AddAbridge("ПОДВ.ПОМ.");
            t.AddAbridge("ПОДВАЛ.ПОМ.");
            t.AddAbridge("ПОДВ.");
            t.AddVariant("ПОГРЕБ", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("МАСТЕРСКАЯ") { Tag = AddressItemType.Space, Tag2 = 1 };
            m_Ontology.Add(t);
            foreach (string s in new string[] {"АПТЕКА", "МАНСАРДА", "АТЕЛЬЕ", "ЧЕРДАК", "КРЫША", "ОТЕЛЬ", "ГОСТИНИЦА", "САРАЙ", "ПАРИКМАХЕРСКАЯ", "СТОЛОВАЯ", "КАФЕ"}) 
            {
                m_Ontology.Add(new Pullenti.Ner.Core.Termin(s) { Tag = AddressItemType.Space, Tag2 = 1 });
            }
            t = new Pullenti.Ner.Core.Termin("МАГАЗИН") { Tag = AddressItemType.Space, Tag2 = 1 };
            t.AddAbridge("МАГ.");
            t.AddAbridge("МАГ-Н");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("МАШИНОМЕСТО") { Tag = AddressItemType.Carplace };
            t.AddAbridge("М/М");
            t.AddAbridge("МАШ.МЕСТО");
            t.AddAbridge("М.МЕСТО");
            t.AddAbridge("МАШ.М.");
            t.AddVariant("МАШИНО-МЕСТО", false);
            t.AddVariant("ПАРКОВОЧНОЕ МЕСТО", false);
            t.AddAbridge("ММ");
            t.AddAbridge("MM");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("КОМНАТА") { Tag = AddressItemType.Room };
            t.AddAbridge("КОМ.");
            t.AddAbridge("КОМН.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("КАБИНЕТ") { Tag = AddressItemType.Office };
            t.AddAbridge("КАБ.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("НОМЕР") { Tag = AddressItemType.Number };
            t.AddAbridge("НОМ.");
            t.AddAbridge("№");
            t.AddAbridge("N");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("БЕЗ НОМЕРА") { CanonicText = "Б/Н", Tag = AddressItemType.NoNumber };
            t.AddVariant("НЕ ОПРЕДЕЛЕНО", false);
            t.AddVariant("НЕОПРЕДЕЛЕНО", false);
            t.AddVariant("НЕ ЗАДАН", false);
            t.AddAbridge("Б.Н.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("АБОНЕНТСКИЙ ЯЩИК") { Tag = AddressItemType.PostOfficeBox };
            t.AddAbridge("А.Я.");
            t.AddVariant("ПОЧТОВЫЙ ЯЩИК", false);
            t.AddAbridge("П.Я.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ГОРОДСКАЯ СЛУЖЕБНАЯ ПОЧТА") { Tag = AddressItemType.CSP, Acronym = "ГСП" };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ДОСТАВОЧНЫЙ УЧАСТОК") { Tag = AddressItemType.DeliveryArea };
            t.AddAbridge("ДОСТ.УЧАСТОК");
            t.AddAbridge("ДОСТ.УЧ.");
            t.AddAbridge("ДОСТ.УЧ-К");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("АДРЕС") { Tag = AddressItemType.Prefix, IgnoreTermsOrder = true };
            t.AddVariant("ЮРИДИЧЕСКИЙ АДРЕС", false);
            t.AddVariant("ФАКТИЧЕСКИЙ АДРЕС", false);
            t.AddAbridge("ЮР.АДРЕС");
            t.AddAbridge("ПОЧТ.АДРЕС");
            t.AddAbridge("ФАКТ.АДРЕС");
            t.AddAbridge("П.АДРЕС");
            t.AddVariant("ЮРИДИЧЕСКИЙ/ФАКТИЧЕСКИЙ АДРЕС", false);
            t.AddVariant("ЮРИДИЧЕСКИЙ И ФАКТИЧЕСКИЙ АДРЕС", false);
            t.AddVariant("ПОЧТОВЫЙ АДРЕС", false);
            t.AddVariant("АДРЕС ПРОЖИВАНИЯ", false);
            t.AddVariant("МЕСТО НАХОЖДЕНИЯ", false);
            t.AddVariant("МЕСТОНАХОЖДЕНИЕ", false);
            t.AddVariant("МЕСТОПОЛОЖЕНИЕ", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("АДРЕСА") { Tag = AddressItemType.Prefix, IgnoreTermsOrder = true };
            t.AddVariant("ЮРИДИЧНА АДРЕСА", false);
            t.AddVariant("ФАКТИЧНА АДРЕСА", false);
            t.AddVariant("ПОШТОВА АДРЕСА", false);
            t.AddVariant("АДРЕСА ПРОЖИВАННЯ", false);
            t.AddVariant("МІСЦЕ ПЕРЕБУВАННЯ", false);
            t.AddVariant("ПРОПИСКА", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("КИЛОМЕТР") { Tag = AddressItemType.Kilometer };
            t.AddAbridge("КИЛОМ.");
            t.AddAbridge("КМ.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПЕРЕСЕЧЕНИЕ") { Tag = Pullenti.Ner.Address.AddressDetailType.Cross };
            t.AddVariant("НА ПЕРЕСЕЧЕНИИ", false);
            t.AddVariant("ПЕРЕКРЕСТОК", false);
            t.AddVariant("УГОЛ", false);
            t.AddVariant("НА ПЕРЕКРЕСТКЕ", false);
            m_Ontology.Add(t);
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("НА ТЕРРИТОРИИ") { Tag = Pullenti.Ner.Address.AddressDetailType.Near });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("СЕРЕДИНА") { Tag = Pullenti.Ner.Address.AddressDetailType.Near });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("ПРИМЫКАТЬ") { Tag = Pullenti.Ner.Address.AddressDetailType.Near });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("ГРАНИЧИТЬ") { Tag = Pullenti.Ner.Address.AddressDetailType.Near });
            t = new Pullenti.Ner.Core.Termin("ВБЛИЗИ") { Tag = Pullenti.Ner.Address.AddressDetailType.Near };
            t.AddVariant("У", false);
            t.AddAbridge("ВБЛ.");
            t.AddVariant("В БЛИЗИ", false);
            t.AddVariant("ВОЗЛЕ", false);
            t.AddVariant("ОКОЛО", false);
            t.AddVariant("НЕДАЛЕКО ОТ", false);
            t.AddVariant("РЯДОМ С", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("РАЙОН") { Tag = Pullenti.Ner.Address.AddressDetailType.Near };
            t.AddAbridge("Р-Н");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("В РАЙОНЕ") { CanonicText = "РАЙОН", Tag = Pullenti.Ner.Address.AddressDetailType.Near };
            t.AddAbridge("В Р-НЕ");
            m_Ontology.Add(t);
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("ПРИМЕРНО") { Tag = Pullenti.Ner.Address.AddressDetailType.Undefined });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("ПОРЯДКА") { Tag = Pullenti.Ner.Address.AddressDetailType.Undefined });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("ПРИБЛИЗИТЕЛЬНО") { Tag = Pullenti.Ner.Address.AddressDetailType.Undefined });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("ОРИЕНТИР") { Tag = Pullenti.Ner.Address.AddressDetailType.Undefined });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("НАПРАВЛЕНИЕ") { Tag = Pullenti.Ner.Address.AddressDetailType.Undefined });
            Pullenti.Ner.Core.Termin.AssignAllTextsAsNormal = true;
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("СЕВЕРНЕЕ") { Tag = Pullenti.Ner.Address.AddressDetailType.North });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("СЕВЕР") { Tag = Pullenti.Ner.Address.AddressDetailType.North });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("ЮЖНЕЕ") { Tag = Pullenti.Ner.Address.AddressDetailType.South });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("ЮГ") { Tag = Pullenti.Ner.Address.AddressDetailType.South });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("ЗАПАДНЕЕ") { Tag = Pullenti.Ner.Address.AddressDetailType.West });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("ЗАПАД") { Tag = Pullenti.Ner.Address.AddressDetailType.West });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("ВОСТОЧНЕЕ") { Tag = Pullenti.Ner.Address.AddressDetailType.East });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("ВОСТОК") { Tag = Pullenti.Ner.Address.AddressDetailType.East });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("СЕВЕРО-ЗАПАДНЕЕ") { Tag = Pullenti.Ner.Address.AddressDetailType.NorthWest });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("СЕВЕРО-ЗАПАД") { Tag = Pullenti.Ner.Address.AddressDetailType.NorthWest });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("СЕВЕРО-ВОСТОЧНЕЕ") { Tag = Pullenti.Ner.Address.AddressDetailType.NorthEast });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("СЕВЕРО-ВОСТОК") { Tag = Pullenti.Ner.Address.AddressDetailType.NorthEast });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("ЮГО-ЗАПАДНЕЕ") { Tag = Pullenti.Ner.Address.AddressDetailType.SouthWest });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("ЮГО-ЗАПАД") { Tag = Pullenti.Ner.Address.AddressDetailType.SouthWest });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("ЮГО-ВОСТОЧНЕЕ") { Tag = Pullenti.Ner.Address.AddressDetailType.SouthEast });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("ЮГО-ВОСТОК") { Tag = Pullenti.Ner.Address.AddressDetailType.SouthEast });
            t = new Pullenti.Ner.Core.Termin("ЦЕНТРАЛЬНАЯ ЧАСТЬ") { Tag = Pullenti.Ner.Address.AddressDetailType.Central, Tag2 = 1 };
            t.AddAbridge("ЦЕНТР.ЧАСТЬ");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("СЕВЕРНАЯ ЧАСТЬ") { Tag = Pullenti.Ner.Address.AddressDetailType.North, Tag2 = 1 };
            t.AddAbridge("СЕВ.ЧАСТЬ");
            t.AddAbridge("СЕВЕРН.ЧАСТЬ");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("СЕВЕРО-ВОСТОЧНАЯ ЧАСТЬ") { Tag = Pullenti.Ner.Address.AddressDetailType.NorthEast, Tag2 = 1 };
            t.AddVariant("СЕВЕРОВОСТОЧНАЯ ЧАСТЬ", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("СЕВЕРО-ЗАПАДНАЯ ЧАСТЬ") { Tag = Pullenti.Ner.Address.AddressDetailType.NorthWest, Tag2 = 1 };
            t.AddVariant("СЕВЕРОЗАПАДНАЯ ЧАСТЬ", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ЮЖНАЯ ЧАСТЬ") { Tag = Pullenti.Ner.Address.AddressDetailType.South, Tag2 = 1 };
            t.AddAbridge("ЮЖН.ЧАСТЬ");
            t.AddAbridge("ЮЖ.ЧАСТЬ");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ЮГО-ВОСТОЧНАЯ ЧАСТЬ") { Tag = Pullenti.Ner.Address.AddressDetailType.SouthEast, Tag2 = 1 };
            t.AddVariant("ЮГОВОСТОЧНАЯ ЧАСТЬ", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ЮГО-ЗАПАДНАЯ ЧАСТЬ") { Tag = Pullenti.Ner.Address.AddressDetailType.SouthWest, Tag2 = 1 };
            t.AddVariant("ЮГОЗАПАДНАЯ ЧАСТЬ", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ЗАПАДНАЯ ЧАСТЬ") { Tag = Pullenti.Ner.Address.AddressDetailType.West, Tag2 = 1 };
            t.AddAbridge("ЗАП.ЧАСТЬ");
            t.AddAbridge("ЗАПАД.ЧАСТЬ");
            t.AddAbridge("ЗАПАДН.ЧАСТЬ");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ВОСТОЧНАЯ ЧАСТЬ") { Tag = Pullenti.Ner.Address.AddressDetailType.East, Tag2 = 1 };
            t.AddAbridge("ВОСТ.ЧАСТЬ");
            t.AddAbridge("ВОСТОЧ.ЧАСТЬ");
            t.AddAbridge("ВОСТОЧН.ЧАСТЬ");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПРАВАЯ ЧАСТЬ") { Tag = Pullenti.Ner.Address.AddressDetailType.Right, Tag2 = 1 };
            t.AddAbridge("СПРАВА");
            t.AddAbridge("ПРАВ.ЧАСТЬ");
            t.AddVariant("ПРАВАЯ СТОРОНА", false);
            t.AddAbridge("ПРАВ.СТОРОНА");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ЛЕВАЯ ЧАСТЬ") { Tag = Pullenti.Ner.Address.AddressDetailType.Left, Tag2 = 1 };
            t.AddAbridge("СЛЕВА");
            t.AddAbridge("ЛЕВ.ЧАСТЬ");
            t.AddVariant("ЛЕВАЯ СТОРОНА", false);
            t.AddAbridge("ЛЕВ.СТОРОНА");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ТАМ ЖЕ");
            t.AddAbridge("ТАМЖЕ");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("АВТОЗАПРАВОЧНАЯ СТАНЦИЯ") { Tag = AddressItemType.House, Tag2 = Pullenti.Ner.Address.AddressHouseType.Special, Acronym = "АЗС", AcronymCanBeLower = true, AcronymCanBeSmart = true };
            t.AddVariant("АВТО ЗАПРАВОЧНАЯ СТАНЦИЯ", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("АВТОНОМНАЯ ТЕПЛОВАЯ СТАНЦИЯ") { Tag = AddressItemType.House, Tag2 = Pullenti.Ner.Address.AddressHouseType.Special, Acronym = "АТС", AcronymCanBeLower = true, AcronymCanBeSmart = true };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ДОРОЖНО РЕМОНТНЫЙ ПУНКТ") { Tag = AddressItemType.House, Tag2 = Pullenti.Ner.Address.AddressHouseType.Special, Acronym = "ДРП", AcronymCanBeLower = true, AcronymCanBeSmart = true };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("УСТАНОВКА КОМПЛЕКСНОЙ ПОДГОТОВКИ ГАЗА") { Tag = AddressItemType.House, Tag2 = Pullenti.Ner.Address.AddressHouseType.Special, Acronym = "УКПГ", AcronymCanBeLower = true, AcronymCanBeSmart = true };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("УСТАНОВКА ПРЕДВАРИТЕЛЬНОЙ ПОДГОТОВКИ ГАЗА") { Tag = AddressItemType.House, Tag2 = Pullenti.Ner.Address.AddressHouseType.Special, Acronym = "УППГ", AcronymCanBeLower = true, AcronymCanBeSmart = true };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ЦЕНТРАЛЬНЫЙ ПУНКТ СБОРА НЕФТИ") { Tag = AddressItemType.House, Tag2 = Pullenti.Ner.Address.AddressHouseType.Special, Acronym = "ЦПС", AcronymCanBeLower = true, AcronymCanBeSmart = true };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("КОМПЛЕКТНАЯ ТРАНСФОРМАТОРНАЯ ПОДСТАНЦИЯ") { Tag = AddressItemType.House, Tag2 = Pullenti.Ner.Address.AddressHouseType.Special, Acronym = "КТП", AcronymCanBeLower = true, AcronymCanBeSmart = true };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ТРАНСФОРМАТОРНАЯ ПОДСТАНЦИЯ") { Tag = AddressItemType.House, Tag2 = Pullenti.Ner.Address.AddressHouseType.Special, Acronym = "ТП", AcronymCanBeLower = true, AcronymCanBeSmart = true };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ДИСПЕТЧЕРСКАЯ НЕФТЕПРОВОДНАЯ СЛУЖБА") { Tag = AddressItemType.House, Tag2 = Pullenti.Ner.Address.AddressHouseType.Special, Acronym = "ДНС", AcronymCanBeLower = true, AcronymCanBeSmart = true };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("КУСТОВАЯ НАСОСНАЯ СТАНЦИЯ") { Tag = AddressItemType.House, Tag2 = Pullenti.Ner.Address.AddressHouseType.Special, Acronym = "КНС", AcronymCanBeLower = true, AcronymCanBeSmart = true };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ЦЕНТРАЛЬНЫЙ РАСПРЕДЕЛИТЕЛЬНЫЙ ПУНКТ") { Tag = AddressItemType.House, Tag2 = Pullenti.Ner.Address.AddressHouseType.Special, Acronym = "ЦРП", AcronymCanBeLower = true, AcronymCanBeSmart = true };
            t.AddVariant("ЦРП ТП", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ТРАНСФОРМАТОРНАЯ ПОДСТАНЦИЯ") { Tag = AddressItemType.House, Tag2 = Pullenti.Ner.Address.AddressHouseType.Special, Acronym = "ТП", AcronymCanBeLower = true, AcronymCanBeSmart = true };
            m_Ontology.Add(t);
            Pullenti.Ner.Core.Termin.AssignAllTextsAsNormal = false;
        }
        static Pullenti.Ner.Core.TerminCollection m_Ontology;
        public static Pullenti.Ner.Core.Termin m_Plot;
        public static Pullenti.Ner.Core.Termin m_Owner;
        public static Pullenti.Ner.Token GotoEndOfAddress(Pullenti.Ner.Token t, out bool isHouse)
        {
            isHouse = false;
            Pullenti.Ner.Token brStart = null;
            Pullenti.Ner.Token brEnd = null;
            Pullenti.Ner.Token t1 = t;
            for (Pullenti.Ner.Token tt = t.Next; tt != null; tt = tt.Next) 
            {
                if ((tt is Pullenti.Ner.NumberToken) || tt.IsNewlineBefore) 
                    break;
                Pullenti.Ner.TextToken ttt = tt as Pullenti.Ner.TextToken;
                if (ttt == null) 
                    break;
                if (ttt.IsChar('(')) 
                    brStart = tt;
                if (ttt.IsChar(')')) 
                    brEnd = tt;
                if (!ttt.Chars.IsLetter) 
                    continue;
                if ((ttt.IsValue("ЧАСНЫЙ", null) || ttt.IsValue("ЧАСТНЫЙ", null) || ttt.Term == "Ч") || ttt.Term == "ЧАСТН") 
                {
                    Pullenti.Ner.Token tt2 = ttt.Next;
                    if (tt2 != null && tt2.IsCharOf(".\\/-")) 
                        tt2 = tt2.Next;
                    if (tt2 is Pullenti.Ner.TextToken) 
                    {
                        if ((tt2.IsValue("С", null) || tt2.IsValue("ДОМ", null) || tt2.IsValue("СЕКТ", null)) || tt2.IsValue("СЕКТОР", null) || tt2.IsValue("Д", null)) 
                        {
                            if (tt2.IsValue("ДОМ", null)) 
                                isHouse = true;
                            t1 = (tt = tt2);
                            continue;
                        }
                    }
                }
                if ((ttt.Term == "ЛПХ" || ttt.Term == "ИЖС" || ttt.Term == "ЖД") || ttt.Term == "ПС" || ttt.Term == "ВЧ") 
                {
                    t1 = tt;
                    continue;
                }
                if (ttt.Term.StartsWith("ОБЩ")) 
                {
                    isHouse = true;
                    t1 = tt;
                    continue;
                }
                if (ttt.Term.StartsWith("СЕМ") || ttt.Term.StartsWith("ВЕД")) 
                    continue;
                if ((ttt.LengthChar == 3 && ttt.Chars.IsAllUpper && Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(ttt)) && Pullenti.Ner.Core.NumberHelper.TryParseRoman(ttt) == null) 
                {
                    t1 = tt;
                    continue;
                }
                if ((ttt.LengthChar == 1 && ttt.Next != null && ttt.Next.IsCharOf("\\/")) && (ttt.Next.Next is Pullenti.Ner.TextToken) && ttt.Next.Next.LengthChar == 1) 
                {
                    tt = tt.Next.Next;
                    t1 = tt;
                    continue;
                }
                break;
            }
            if (brStart != null && t1.EndChar > brStart.BeginChar) 
            {
                if (brEnd != null && brEnd.EndChar > t1.EndChar) 
                    t1 = brEnd;
                else 
                {
                    Pullenti.Ner.Core.BracketSequenceToken br = Pullenti.Ner.Core.BracketHelper.TryParse(brStart, Pullenti.Ner.Core.BracketParseAttr.No, 100);
                    if (br != null && br.EndChar > t1.EndChar) 
                        t1 = br.EndToken;
                }
            }
            return t1;
        }
        public AddressItemToken(AddressItemType typ, Pullenti.Ner.Token begin, Pullenti.Ner.Token end) : base(begin, end, null)
        {
            Typ = typ;
        }
        public AddressItemType Typ
        {
            get
            {
                return m_Typ;
            }
            set
            {
                m_Typ = value;
                if (value == AddressItemType.House) 
                {
                }
            }
        }
        AddressItemType m_Typ;
        public string Value;
        public Pullenti.Ner.Referent Referent;
        public Pullenti.Ner.ReferentToken RefToken;
        public Pullenti.Ner.Referent Referent2;
        public Pullenti.Ner.ReferentToken RefToken2;
        public bool RefTokenIsGsk;
        public bool RefTokenIsMassive;
        public bool IsDoubt;
        public bool IsGenplan;
        public Pullenti.Ner.Address.AddressDetailType DetailType = Pullenti.Ner.Address.AddressDetailType.Undefined;
        public Pullenti.Ner.Address.AddressBuildingType BuildingType = Pullenti.Ner.Address.AddressBuildingType.Undefined;
        public Pullenti.Ner.Address.AddressHouseType HouseType = Pullenti.Ner.Address.AddressHouseType.Undefined;
        public int DetailMeters = 0;
        public string DetailParam;
        public AddressItemToken OrtoTerr;
        public AddressItemToken AltTyp;
        public AddressItemToken Clone()
        {
            AddressItemToken res = new AddressItemToken(Typ, BeginToken, EndToken);
            res.Morph = Morph;
            res.Value = Value;
            res.Referent = Referent;
            res.RefToken = RefToken;
            res.Referent2 = Referent2;
            res.RefToken2 = RefToken2;
            res.RefTokenIsGsk = RefTokenIsGsk;
            res.RefTokenIsMassive = RefTokenIsMassive;
            res.IsDoubt = IsDoubt;
            res.DetailType = DetailType;
            res.BuildingType = BuildingType;
            res.HouseType = HouseType;
            res.DetailMeters = DetailMeters;
            res.DetailParam = DetailParam;
            res.IsGenplan = IsGenplan;
            if (OrtoTerr != null) 
                res.OrtoTerr = OrtoTerr.Clone();
            if (AltTyp != null) 
                res.AltTyp = AltTyp.Clone();
            return res;
        }
        public bool IsStreetRoad
        {
            get
            {
                if (Typ != AddressItemType.Street) 
                    return false;
                if (!(Referent is Pullenti.Ner.Address.StreetReferent)) 
                    return false;
                return (Referent as Pullenti.Ner.Address.StreetReferent).Kind == Pullenti.Ner.Address.StreetKind.Road;
            }
        }
        public bool IsStreetDetail
        {
            get
            {
                if (Typ != AddressItemType.Street) 
                    return false;
                if (!(Referent is Pullenti.Ner.Address.StreetReferent)) 
                    return false;
                foreach (string s in Referent.GetStringValues("MISC")) 
                {
                    if (s.Contains("бизнес") || s.Contains("делов") || s.Contains("офис")) 
                        return true;
                }
                return false;
            }
        }
        public bool IsDigit
        {
            get
            {
                if (Value == "Б/Н" || Value == "НЕТ") 
                    return true;
                if (string.IsNullOrEmpty(Value)) 
                    return false;
                if (char.IsDigit(Value[0]) || Value[0] == '-') 
                    return true;
                if (Value.Length > 1) 
                {
                    if (char.IsLetter(Value[0]) && char.IsDigit(Value[1])) 
                        return true;
                }
                if (Value.Length != 1 || !char.IsLetter(Value[0])) 
                    return false;
                if (!BeginToken.Chars.IsAllLower) 
                    return false;
                return true;
            }
        }
        public bool IsHouse
        {
            get
            {
                return (Typ == AddressItemType.House || Typ == AddressItemType.Plot || Typ == AddressItemType.Box) || Typ == AddressItemType.Building || Typ == AddressItemType.Corpus;
            }
        }
        public override string ToString()
        {
            StringBuilder res = new StringBuilder();
            res.AppendFormat("{0} {1}", Typ.ToString(), Value ?? "");
            if (Referent != null) 
                res.AppendFormat(" <{0}>", Referent.ToString());
            if (Referent2 != null) 
                res.AppendFormat(" / <{0}>", Referent2.ToString());
            if (DetailType != Pullenti.Ner.Address.AddressDetailType.Undefined || DetailMeters > 0) 
                res.AppendFormat(" [{0}, {1}]", DetailType, DetailMeters);
            if (OrtoTerr != null) 
                res.AppendFormat(" TERR: {0}", OrtoTerr);
            if (AltTyp != null) 
                res.AppendFormat(" ALT: {0}", AltTyp);
            return res.ToString();
        }
        static AddressItemToken _findAddrTyp(Pullenti.Ner.Token t, int maxChar, int lev = 0)
        {
            if (t == null || t.EndChar > maxChar) 
                return null;
            if (lev > 5) 
                return null;
            if (t is Pullenti.Ner.ReferentToken) 
            {
                Pullenti.Ner.Geo.GeoReferent geo = t.GetReferent() as Pullenti.Ner.Geo.GeoReferent;
                if (geo != null) 
                {
                    foreach (Pullenti.Ner.Slot s in geo.Slots) 
                    {
                        if (s.TypeName == Pullenti.Ner.Geo.GeoReferent.ATTR_TYPE) 
                        {
                            string ty = (string)s.Value;
                            if (ty.Contains("район")) 
                                return null;
                        }
                    }
                }
                for (Pullenti.Ner.Token tt = (t as Pullenti.Ner.ReferentToken).BeginToken; tt != null && tt.EndChar <= t.EndChar; tt = tt.Next) 
                {
                    if (tt.EndChar > maxChar) 
                        break;
                    if (tt.IsValue("У", null)) 
                    {
                        if (geo.FindSlot(Pullenti.Ner.Geo.GeoReferent.ATTR_TYPE, "улус", true) != null) 
                            continue;
                    }
                    AddressItemToken ty = _findAddrTyp(tt, maxChar, lev + 1);
                    if (ty != null) 
                        return ty;
                }
            }
            else 
            {
                AddressItemToken ai = _tryAttachDetail(t, null);
                if (ai != null) 
                {
                    if (ai.DetailType != Pullenti.Ner.Address.AddressDetailType.Undefined || ai.DetailMeters > 0) 
                        return ai;
                }
            }
            return null;
        }
        public static AddressItemToken TryParse(Pullenti.Ner.Token t, bool prefixBefore = false, AddressItemToken prev = null, Pullenti.Ner.Geo.Internal.GeoAnalyzerData ad = null)
        {
            if (t == null) 
                return null;
            if (ad == null) 
                ad = Pullenti.Ner.Geo.GeoAnalyzer.GetData(t);
            if (ad == null) 
                return null;
            if (ad.ALevel > 1) 
                return null;
            ad.ALevel++;
            AddressItemToken res = _TryParse(t, prefixBefore, prev, ad);
            ad.ALevel--;
            if (((res != null && !res.IsWhitespaceAfter && res.EndToken.Next != null) && ((res.EndToken.Next.IsHiphen || res.EndToken.Next.IsCharOf("\\/"))) && !res.EndToken.Next.IsWhitespaceAfter) && res.Value != null) 
            {
                if ((res.Typ == AddressItemType.House || res.Typ == AddressItemType.Building || res.Typ == AddressItemType.Corpus) || res.Typ == AddressItemType.Plot) 
                {
                    Pullenti.Ner.Token tt = res.EndToken.Next.Next;
                    AddressItemToken next = TryParsePureItem(tt, null, null);
                    if (next != null && next.Typ == AddressItemType.Number) 
                    {
                        res = res.Clone();
                        res.Value = string.Format("{0}{1}{2}", res.Value, (res.EndToken.Next.IsHiphen ? "-" : "/"), next.Value);
                        res.EndToken = next.EndToken;
                        tt = res.EndToken.Next;
                        if ((tt != null && ((tt.IsHiphen || tt.IsCharOf("\\/"))) && !tt.IsWhitespaceBefore) && !tt.IsWhitespaceAfter) 
                        {
                            next = TryParsePureItem(tt.Next, null, null);
                            if (next != null && next.Typ == AddressItemType.Number) 
                            {
                                res.Value = string.Format("{0}{1}{2}", res.Value, (tt.IsHiphen ? "-" : "/"), next.Value);
                                res.EndToken = next.EndToken;
                            }
                        }
                    }
                    else if ((tt is Pullenti.Ner.TextToken) && tt.LengthChar == 1 && tt.Chars.IsAllUpper) 
                    {
                        res.Value = string.Format("{0}-{1}", res.Value, (tt as Pullenti.Ner.TextToken).Term);
                        res.EndToken = tt;
                    }
                }
            }
            return res;
        }
        static AddressItemToken _TryParse(Pullenti.Ner.Token t, bool prefixBefore, AddressItemToken prev, Pullenti.Ner.Geo.Internal.GeoAnalyzerData ad)
        {
            if (t == null) 
                return null;
            if (t is Pullenti.Ner.ReferentToken) 
            {
                Pullenti.Ner.ReferentToken rt = t as Pullenti.Ner.ReferentToken;
                AddressItemType ty;
                Pullenti.Ner.Geo.GeoReferent geo = rt.Referent as Pullenti.Ner.Geo.GeoReferent;
                if ((geo != null && t.Next != null && t.Next.IsHiphen) && Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(t)) 
                {
                    List<StreetItemToken> sit = StreetItemToken.TryParseSpec(t, null);
                    if (sit != null && sit[0].Typ == StreetItemType.Name) 
                        geo = null;
                }
                if (geo != null) 
                {
                    if (geo.IsCity) 
                        ty = AddressItemType.City;
                    else if (geo.IsState) 
                        ty = AddressItemType.Country;
                    else 
                        ty = AddressItemType.Region;
                    AddressItemToken res = new AddressItemToken(ty, t, t) { Referent = rt.Referent };
                    if (ty != AddressItemType.City) 
                        return res;
                    for (Pullenti.Ner.Token tt = (t as Pullenti.Ner.ReferentToken).BeginToken; tt != null && tt.EndChar <= t.EndChar; tt = tt.Next) 
                    {
                        if (tt is Pullenti.Ner.ReferentToken) 
                        {
                            if (tt.GetReferent() == geo) 
                            {
                                AddressItemToken res1 = _TryParse(tt, false, prev, ad);
                                if (res1 != null && ((res1.DetailMeters > 0 || res1.DetailType != Pullenti.Ner.Address.AddressDetailType.Undefined))) 
                                {
                                    res1.BeginToken = (res1.EndToken = t);
                                    return res1;
                                }
                            }
                            continue;
                        }
                        AddressItemToken det = _tryParsePureItem(tt, false, null);
                        if (det != null) 
                        {
                            if (tt.IsValue("У", null) && geo.FindSlot(Pullenti.Ner.Geo.GeoReferent.ATTR_TYPE, "улус", true) != null) 
                            {
                            }
                            else 
                            {
                                if (det.DetailType != Pullenti.Ner.Address.AddressDetailType.Undefined && res.DetailType == Pullenti.Ner.Address.AddressDetailType.Undefined) 
                                    res.DetailType = det.DetailType;
                                if (det.DetailMeters > 0) 
                                    res.DetailMeters = det.DetailMeters;
                            }
                        }
                    }
                    return res;
                }
            }
            bool kvart = false;
            if (prev != null) 
            {
                if (t.IsValue("КВ", null) || t.IsValue("КВАРТ", null)) 
                {
                    if ((((prev.Typ == AddressItemType.House || prev.Typ == AddressItemType.Number || prev.Typ == AddressItemType.Building) || prev.Typ == AddressItemType.Floor || prev.Typ == AddressItemType.Potch) || prev.Typ == AddressItemType.Corpus || prev.Typ == AddressItemType.CorpusOrFlat) || prev.Typ == AddressItemType.Detail) 
                        return TryParsePureItem(t, prev, null);
                    kvart = true;
                }
            }
            if (prev != null && ((t.IsValue("П", null) || t.IsValue("ПОЗ", null) || t.IsValue("ПОЗИЦИЯ", null)))) 
            {
                if (((Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(t) || prev.Typ == AddressItemType.Street || prev.Typ == AddressItemType.City) || prev.Typ == AddressItemType.Genplan || prev.Typ == AddressItemType.Plot) || prev.Typ == AddressItemType.City) 
                {
                    Pullenti.Ner.Token tt = t.Next;
                    if (tt != null && tt.IsChar('.')) 
                        tt = tt.Next;
                    AddressItemToken next = TryParsePureItem(tt, null, null);
                    if (next != null && ((next.Typ == AddressItemType.Number || next.Typ == AddressItemType.Genplan))) 
                    {
                        next = next.Clone();
                        next.BeginToken = t;
                        next.IsGenplan = true;
                        next.Typ = AddressItemType.Number;
                        return next;
                    }
                }
            }
            AddressItemToken pure = TryParsePureItem(t, prev, ad);
            if ((pure != null && pure.Typ != AddressItemType.Number && pure.Typ != AddressItemType.Kilometer) && pure.Value != null) 
            {
                if (t.IsValue("СТ", null) && Pullenti.Ner.Geo.Internal.OrgItemToken.TryParse(t, null) != null) 
                {
                }
                else if (kvart) 
                {
                    Pullenti.Ner.Token ttt = pure.EndToken.Next;
                    if (ttt != null && ttt.IsComma) 
                        ttt = ttt.Next;
                    AddressItemToken next = TryParsePureItem(ttt, null, null);
                    if (next != null && next.Typ == AddressItemType.Plot) 
                    {
                    }
                    else 
                        return pure;
                }
                else 
                    return pure;
            }
            Pullenti.Ner.Token tt2 = Pullenti.Ner.Geo.Internal.MiscLocationHelper.CheckTerritory(t);
            if (tt2 != null) 
            {
                AddressItemToken next = TryParse(tt2.Next, false, null, null);
                if (next != null && next.Typ == AddressItemType.Street) 
                {
                    Pullenti.Ner.Address.StreetReferent ss = next.Referent as Pullenti.Ner.Address.StreetReferent;
                    if (ss.Kind == Pullenti.Ner.Address.StreetKind.Road || ss.Kind == Pullenti.Ner.Address.StreetKind.Railway) 
                    {
                        next.BeginToken = t;
                        return next;
                    }
                }
            }
            List<StreetItemToken> sli = StreetItemToken.TryParseList(t, 10, ad);
            if (sli != null) 
            {
                AddressItemToken rt = StreetDefineHelper.TryParseStreet(sli, prefixBefore, false, (prev != null && prev.Typ == AddressItemType.Street), null);
                if (rt == null && sli[0].Typ != StreetItemType.Fix) 
                {
                    Pullenti.Ner.Geo.Internal.OrgItemToken org = Pullenti.Ner.Geo.Internal.OrgItemToken.TryParse(t, null);
                    if (org != null) 
                    {
                        StreetItemToken si = new StreetItemToken(t, org.EndToken) { Typ = StreetItemType.Fix, Org = org };
                        sli.Clear();
                        sli.Add(si);
                        rt = StreetDefineHelper.TryParseStreet(sli, prefixBefore || prev != null, false, false, null);
                    }
                    else if (sli.Count == 1 && sli[0].Typ == StreetItemType.Noun && !sli[0].IsNewlineAfter) 
                    {
                        org = Pullenti.Ner.Geo.Internal.OrgItemToken.TryParse(sli[0].EndToken.Next, null);
                        if (org != null) 
                        {
                            string typ = sli[0].Termin.CanonicText.ToLower();
                            StreetItemToken si = new StreetItemToken(t, org.EndToken) { Typ = StreetItemType.Fix, Org = org };
                            sli.Clear();
                            sli.Add(si);
                            rt = StreetDefineHelper.TryParseStreet(sli, prefixBefore || prev != null, false, false, null);
                            if (rt != null) 
                            {
                                Pullenti.Ner.Address.StreetReferent sr = rt.Referent as Pullenti.Ner.Address.StreetReferent;
                                sr.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_TYPE, null, true, 0);
                                sr.AddTyp(typ);
                                sr.Kind = Pullenti.Ner.Address.StreetKind.Undefined;
                            }
                        }
                    }
                }
                if ((rt == null && prev != null && prev.Typ == AddressItemType.City) && Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(sli[0])) 
                {
                    if (sli.Count == 1 && (((sli[0].Typ == StreetItemType.Name || sli[0].Typ == StreetItemType.StdName || sli[0].Typ == StreetItemType.StdAdjective) || ((sli[0].Typ == StreetItemType.Number && sli[0].BeginToken.Morph.Class.IsAdjective))))) 
                        rt = StreetDefineHelper.TryParseStreet(sli, true, false, false, null);
                }
                if (rt != null) 
                {
                    if (sli.Count > 2) 
                    {
                    }
                    if (rt.BeginChar > sli[0].BeginChar) 
                        return null;
                    bool crlf = false;
                    for (Pullenti.Ner.Token ttt = rt.BeginToken; ttt != rt.EndToken && (ttt.EndChar < rt.EndChar); ttt = ttt.Next) 
                    {
                        if (ttt.IsNewlineAfter) 
                        {
                            crlf = true;
                            break;
                        }
                    }
                    if (crlf) 
                    {
                        for (Pullenti.Ner.Token ttt = rt.BeginToken.Previous; ttt != null; ttt = ttt.Previous) 
                        {
                            if (ttt.Morph.Class.IsPreposition || ttt.IsComma) 
                                continue;
                            if (ttt.GetReferent() is Pullenti.Ner.Geo.GeoReferent) 
                                crlf = false;
                            break;
                        }
                        if (sli[0].Typ == StreetItemType.Noun && sli[0].Termin.CanonicText.Contains("ДОРОГА")) 
                            crlf = false;
                    }
                    if (crlf) 
                    {
                        AddressItemToken aat = TryParsePureItem(rt.EndToken.Next, null, null);
                        if (aat == null) 
                            return null;
                        if (aat.Typ != AddressItemType.House) 
                            return null;
                    }
                    if (rt.EndToken.Next != null && rt.EndToken.Next.IsCharOf("\\/")) 
                    {
                        if (!AddressItemToken.CheckHouseAfter(rt.EndToken.Next.Next, false, false)) 
                        {
                            List<StreetItemToken> sli2 = StreetItemToken.TryParseList(rt.EndToken.Next.Next, 10, ad);
                            if (sli2 != null && sli2.Count > 0) 
                            {
                                AddressItemToken rt2 = StreetDefineHelper.TryParseStreet(sli2, prefixBefore, false, true, rt.Referent as Pullenti.Ner.Address.StreetReferent);
                                if (rt2 != null) 
                                {
                                    rt.EndToken = rt2.EndToken;
                                    rt.Referent2 = rt2.Referent;
                                }
                            }
                        }
                    }
                    return rt;
                }
                if (sli.Count == 1 && sli[0].Typ == StreetItemType.Noun) 
                {
                    Pullenti.Ner.Token tt = sli[0].EndToken.Next;
                    if (tt != null && ((tt.IsHiphen || tt.IsChar('_') || tt.IsValue("НЕТ", null)))) 
                    {
                        Pullenti.Ner.Token ttt = tt.Next;
                        if (ttt != null && ttt.IsComma) 
                            ttt = ttt.Next;
                        AddressItemToken att = TryParsePureItem(ttt, null, null);
                        if (att != null) 
                        {
                            if (att.Typ == AddressItemType.House || att.Typ == AddressItemType.Corpus || att.Typ == AddressItemType.Building) 
                                return new AddressItemToken(AddressItemType.Street, t, tt);
                        }
                    }
                }
            }
            if (t == null || pure != null) 
                return pure;
            if ((t.LengthChar == 1 && t.Chars.IsLetter && prev != null) && prev.Typ == AddressItemType.City && Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(t)) 
            {
                Pullenti.Ner.Token tt = t.Next;
                if (tt != null && tt.IsHiphen) 
                    tt = tt.Next;
                string ch = CorrectCharToken(t);
                if ((tt is Pullenti.Ner.NumberToken) && ch != null) 
                {
                    Pullenti.Ner.Address.StreetReferent micr = new Pullenti.Ner.Address.StreetReferent();
                    micr.AddTyp("микрорайон");
                    micr.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NAME, ch, false, 0);
                    micr.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NUMBER, (tt as Pullenti.Ner.NumberToken).Value, false, 0);
                    micr.Kind = Pullenti.Ner.Address.StreetKind.Area;
                    return new AddressItemToken(AddressItemType.Street, t, tt) { Referent = micr, RefToken = new Pullenti.Ner.ReferentToken(micr, t, tt) };
                }
            }
            return null;
        }
        public static bool SpeedRegime = false;
        internal static void PrepareAllData(Pullenti.Ner.Token t0)
        {
            if (!SpeedRegime) 
                return;
            Pullenti.Ner.Geo.Internal.GeoAnalyzerData ad = Pullenti.Ner.Geo.GeoAnalyzer.GetData(t0);
            if (ad == null) 
                return;
            ad.ARegime = false;
            for (Pullenti.Ner.Token t = t0; t != null; t = t.Next) 
            {
                Pullenti.Ner.Geo.Internal.GeoTokenData d = t.Tag as Pullenti.Ner.Geo.Internal.GeoTokenData;
                AddressItemToken prev = null;
                int kk = 0;
                for (Pullenti.Ner.Token tt = t.Previous; tt != null && (kk < 10); tt = tt.Previous,kk++) 
                {
                    Pullenti.Ner.Geo.Internal.GeoTokenData dd = tt.Tag as Pullenti.Ner.Geo.Internal.GeoTokenData;
                    if (dd == null) 
                        continue;
                    if (dd.Street != null) 
                    {
                        if (dd.Street.EndToken.Next == t) 
                            prev = dd.Addr;
                        else if (t.Previous != null && t.Previous.IsComma && dd.Street.EndToken.Next == t.Previous) 
                            prev = dd.Addr;
                    }
                    else if (dd.Addr != null && (((dd.Addr.Typ == AddressItemType.House || dd.Addr.Typ == AddressItemType.Flat || dd.Addr.Typ == AddressItemType.Corpus) || dd.Addr.Typ == AddressItemType.Building))) 
                    {
                        if (dd.Addr.EndToken.Next == t) 
                            prev = dd.Addr;
                        else if (t.Previous != null && t.Previous.IsComma && dd.Addr.EndToken.Next == t.Previous) 
                            prev = dd.Addr;
                    }
                }
                AddressItemToken str = TryParsePureItem(t, prev, null);
                if (str != null) 
                {
                    if (d == null) 
                        d = new Pullenti.Ner.Geo.Internal.GeoTokenData(t);
                    d.Addr = str;
                }
            }
            ad.ARegime = true;
        }
        public static AddressItemToken TryParsePureItem(Pullenti.Ner.Token t, AddressItemToken prev = null, Pullenti.Ner.Geo.Internal.GeoAnalyzerData ad = null)
        {
            if (t == null) 
                return null;
            if (t.IsChar(',')) 
                return null;
            if (ad == null) 
                ad = Pullenti.Ner.Geo.GeoAnalyzer.GetData(t);
            if (ad == null) 
                return null;
            int maxLevel = 0;
            if ((prev != null && prev.Typ == AddressItemType.Street && t.LengthChar == 1) && ((t.IsValue("С", null) || t.IsValue("Д", null)))) 
                maxLevel = 1;
            else if (SpeedRegime && ((ad.ARegime || ad.AllRegime)) && !(t is Pullenti.Ner.ReferentToken)) 
            {
                Pullenti.Ner.Geo.Internal.GeoTokenData d = t.Tag as Pullenti.Ner.Geo.Internal.GeoTokenData;
                if (d == null) 
                    return null;
                if (d.Addr == null) 
                    return null;
                if (d.Addr.HouseType == Pullenti.Ner.Address.AddressHouseType.Estate && d.NoGeo) 
                    return null;
                bool ok = true;
                for (Pullenti.Ner.Token tt = t; tt != null && tt.BeginChar <= d.Addr.EndChar; tt = tt.Next) 
                {
                    if (tt is Pullenti.Ner.ReferentToken) 
                    {
                        ok = false;
                        maxLevel = 1;
                        break;
                    }
                }
                if (ok) 
                    return d.Addr;
            }
            if (ad.ALevel > (maxLevel + 1)) 
                return null;
            if (ad.Level > 1) 
                return null;
            ad.Level++;
            AddressItemToken res = _tryParsePureItem(t, false, prev);
            if (res == null && Pullenti.Ner.Core.BracketHelper.IsBracket(t, false) && (t.WhitespacesAfterCount < 2)) 
            {
                AddressItemToken res1 = _tryParsePureItem(t.Next, false, prev);
                if (res1 != null && Pullenti.Ner.Core.BracketHelper.IsBracket(res1.EndToken.Next, false)) 
                {
                    res = res1;
                    res.BeginToken = t;
                    res.EndToken = res1.EndToken.Next;
                }
            }
            if ((res == null && prev != null && t.LengthChar == 1) && t.IsValue("С", null)) 
            {
                if (prev.Typ == AddressItemType.Corpus || prev.Typ == AddressItemType.House || prev.Typ == AddressItemType.Street) 
                {
                    AddressItemToken next = _tryParsePureItem(t.Next, false, null);
                    if (next != null && next.Typ == AddressItemType.Number) 
                    {
                        next.Typ = AddressItemType.Building;
                        next.BeginToken = t;
                        res = next;
                    }
                }
            }
            if (res != null && res.Typ == AddressItemType.Detail) 
            {
            }
            else 
            {
                AddressItemToken det = _tryAttachDetail(t, null);
                if (res == null) 
                    res = det;
                else if (det != null && det.EndChar > res.EndChar) 
                    res = det;
            }
            if ((res != null && !string.IsNullOrEmpty(res.Value) && char.IsDigit(res.Value[res.Value.Length - 1])) && Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(res)) 
            {
                Pullenti.Ner.Token t1 = res.EndToken.Next;
                if (((t1 is Pullenti.Ner.TextToken) && (t1.WhitespacesBeforeCount < 3) && t1.Chars.IsLetter) && t1.LengthChar == 1) 
                {
                    AddressItemToken res2 = _tryParsePureItem(t1, false, null);
                    if (res2 == null) 
                    {
                        StreetItemToken sit = StreetItemToken.TryParse(t1, null, false, null);
                        if (sit != null && sit.Typ == StreetItemType.Noun) 
                        {
                        }
                        else 
                        {
                            string ch = CorrectCharToken(t1);
                            if (Pullenti.Ner.Geo.Internal.OrgTypToken.TryParse(t1, false, null) != null) 
                                ch = null;
                            if (ch != null && ch != "К" && ch != "С") 
                            {
                                res.Value = string.Format("{0}{1}", res.Value, ch);
                                res.EndToken = t1;
                            }
                        }
                    }
                }
            }
            if ((res != null && res.Typ == AddressItemType.Number && res.EndToken.Next != null) && res.EndToken.Next.IsValue("ДОЛЯ", null)) 
            {
                res.EndToken = res.EndToken.Next;
                res.Typ = AddressItemType.Part;
                res.Value = "1";
            }
            if (res == null && t.GetMorphClassInDictionary().IsPreposition) 
            {
                AddressItemToken next = TryParsePureItem(t.Next, null, null);
                if (next != null && next.Typ != AddressItemType.Number && !t.Next.IsValue("СТ", null)) 
                {
                    next.BeginToken = t;
                    res = next;
                }
            }
            ad.Level--;
            return res;
        }
        static AddressItemToken _tryParsePureItem(Pullenti.Ner.Token t, bool prefixBefore, AddressItemToken prev)
        {
            if (t is Pullenti.Ner.NumberToken) 
            {
                Pullenti.Ner.NumberToken n = t as Pullenti.Ner.NumberToken;
                if (((n.LengthChar == 6 || n.LengthChar == 5)) && n.Typ == Pullenti.Ner.NumberSpellingType.Digit && !n.Morph.Class.IsAdjective) 
                    return new AddressItemToken(AddressItemType.Zip, t, t) { Value = n.Value.ToString() };
                bool ok = false;
                if ((t.Previous != null && t.Previous.Morph.Class.IsPreposition && t.Next != null) && t.Next.Chars.IsLetter && t.Next.Chars.IsAllLower) 
                    ok = true;
                else if (t.Morph.Class.IsAdjective && !t.Morph.Class.IsNoun) 
                    ok = true;
                Pullenti.Ner.Core.TerminToken tok0 = m_Ontology.TryParse(t.Next, Pullenti.Ner.Core.TerminParseAttr.No);
                if (tok0 != null && (tok0.Termin.Tag is AddressItemType)) 
                {
                    AddressItemType typ0 = (AddressItemType)tok0.Termin.Tag;
                    if (tok0.EndToken.Next == null || tok0.EndToken.IsNewlineAfter) 
                        ok = true;
                    else if (tok0.EndToken.Next.IsComma && (tok0.EndToken.Next.Next is Pullenti.Ner.NumberToken) && typ0 == AddressItemType.Flat) 
                        return new AddressItemToken(AddressItemType.House, t, t) { Value = n.Value };
                    if (typ0 == AddressItemType.Flat) 
                    {
                        if ((t.Next is Pullenti.Ner.TextToken) && t.Next.IsValue("КВ", null)) 
                        {
                            if (t.Next.GetSourceText() == "кВ") 
                                return null;
                            StreetItemToken si = StreetItemToken.TryParse(t.Next, null, false, null);
                            if (si != null && si.Typ == StreetItemType.Noun && si.EndChar > tok0.EndChar) 
                                return null;
                            Pullenti.Ner.Core.NumberExToken suf = Pullenti.Ner.Core.NumberHelper.TryParsePostfixOnly(t.Next);
                            if (suf != null) 
                                return null;
                        }
                        if ((tok0.EndToken.Next is Pullenti.Ner.NumberToken) && (tok0.EndToken.WhitespacesAfterCount < 3)) 
                        {
                            if (prev != null && ((prev.Typ == AddressItemType.Street || prev.Typ == AddressItemType.City))) 
                                return new AddressItemToken(AddressItemType.Number, t, t) { Value = n.Value.ToString() };
                        }
                    }
                    if (tok0.EndToken.Next is Pullenti.Ner.NumberToken) 
                    {
                    }
                    else if (tok0.EndToken.Next != null && tok0.EndToken.Next.IsValue("НЕТ", null)) 
                    {
                    }
                    else if ((((typ0 == AddressItemType.Kilometer || typ0 == AddressItemType.Floor || typ0 == AddressItemType.Block) || typ0 == AddressItemType.Potch || typ0 == AddressItemType.Flat) || typ0 == AddressItemType.Plot || typ0 == AddressItemType.Box) || typ0 == AddressItemType.Office) 
                    {
                        AddressItemToken next = _tryParsePureItem(tok0.EndToken.Next, false, null);
                        if (next != null && next.Typ == AddressItemType.Number) 
                        {
                        }
                        else 
                        {
                            next = _tryParsePureItem(tok0.EndToken, false, null);
                            if (next != null && next.Value != null && next.Value != "0") 
                            {
                            }
                            else 
                                return new AddressItemToken(typ0, t, tok0.EndToken) { Value = n.Value.ToString() };
                        }
                    }
                }
            }
            bool prepos = false;
            Pullenti.Ner.Core.TerminToken tok = null;
            if (t != null && t.Morph.Class.IsPreposition) 
            {
                if ((((tok = m_Ontology.TryParse(t, Pullenti.Ner.Core.TerminParseAttr.No)))) == null) 
                {
                    if (t.BeginChar < t.EndChar) 
                        return null;
                    if (t.IsValue("В", null) && Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(t)) 
                    {
                        Pullenti.Ner.Token tt = t.Next;
                        if (tt != null && tt.IsChar('.')) 
                            tt = tt.Next;
                        Pullenti.Ner.Geo.Internal.NumToken num1 = Pullenti.Ner.Geo.Internal.NumToken.TryParse(tt, Pullenti.Ner.Geo.Internal.GeoTokenType.House);
                        if (num1 != null) 
                        {
                            for (Pullenti.Ner.Token tt0 = t.Previous; tt0 != null; tt0 = tt0.Previous) 
                            {
                                if (tt0.IsValue("КВАРТАЛ", null) || tt0.IsValue("КВ", null) || tt0.IsValue("ЛЕСНИЧЕСТВО", null)) 
                                    return new AddressItemToken(AddressItemType.Plot, t, num1.EndToken) { Value = num1.Value };
                                if (tt0.IsNewlineBefore) 
                                    break;
                            }
                        }
                    }
                    if (!t.IsCharOf("КСкс")) 
                        t = t.Next;
                    prepos = true;
                }
            }
            if (t == null) 
                return null;
            if ((((t is Pullenti.Ner.TextToken) && t.LengthChar == 1 && t.Chars.IsLetter) && !t.IsValue("V", null) && !t.IsValue("I", null)) && !t.IsValue("X", null)) 
            {
                if (t.Previous != null && t.Previous.IsComma) 
                {
                    if (t.IsNewlineAfter || t.Next.IsComma) 
                        return new AddressItemToken(AddressItemType.Building, t, t) { BuildingType = Pullenti.Ner.Address.AddressBuildingType.Liter, Value = (t as Pullenti.Ner.TextToken).Term };
                }
            }
            if (t.IsChar('/')) 
            {
                AddressItemToken next = TryParsePureItem(t.Next, prev, null);
                if (next != null && next.EndToken.Next != null && next.EndToken.Next.IsChar('/')) 
                {
                    next.BeginToken = t;
                    next.EndToken = next.EndToken.Next;
                    return next;
                }
                if (next != null && next.EndToken.IsChar('/')) 
                {
                    next.BeginToken = t;
                    return next;
                }
            }
            if (tok == null) 
                tok = m_Ontology.TryParse(t, Pullenti.Ner.Core.TerminParseAttr.No);
            Pullenti.Ner.Token t1 = t;
            AddressItemType typ = AddressItemType.Number;
            Pullenti.Ner.Address.AddressHouseType houseTyp = Pullenti.Ner.Address.AddressHouseType.Undefined;
            Pullenti.Ner.Address.AddressBuildingType buildTyp = Pullenti.Ner.Address.AddressBuildingType.Undefined;
            Pullenti.Ner.Core.TerminToken tok00 = tok;
            if (tok != null) 
            {
                if (t.IsValue("УЖЕ", null)) 
                    return null;
                if (t.IsValue("ЛИТЕРА", null)) 
                {
                    string str = t.GetSourceText();
                    if (char.IsUpper(str[str.Length - 1]) && char.IsLower(str[str.Length - 2])) 
                        return new AddressItemToken(AddressItemType.Building, t, t) { BuildingType = Pullenti.Ner.Address.AddressBuildingType.Liter, Value = str.Substring(str.Length - 1) };
                }
                if (tok.Termin.CanonicText == "ТАМ ЖЕ") 
                {
                    int cou = 0;
                    for (Pullenti.Ner.Token tt = t.Previous; tt != null; tt = tt.Previous) 
                    {
                        if (cou > 1000) 
                            break;
                        Pullenti.Ner.Referent r = tt.GetReferent();
                        if (r == null) 
                            continue;
                        if (r is Pullenti.Ner.Address.AddressReferent) 
                        {
                            Pullenti.Ner.Geo.GeoReferent g = r.GetSlotValue(Pullenti.Ner.Address.AddressReferent.ATTR_GEO) as Pullenti.Ner.Geo.GeoReferent;
                            if (g != null) 
                                return new AddressItemToken(AddressItemType.City, t, tok.EndToken) { Referent = g };
                            break;
                        }
                        else if (r is Pullenti.Ner.Geo.GeoReferent) 
                        {
                            Pullenti.Ner.Geo.GeoReferent g = r as Pullenti.Ner.Geo.GeoReferent;
                            if (!g.IsState) 
                                return new AddressItemToken(AddressItemType.City, t, tok.EndToken) { Referent = g };
                        }
                    }
                    return null;
                }
                if (tok.Termin.Tag is Pullenti.Ner.Address.AddressDetailType) 
                    return _tryAttachDetail(t, tok);
                t1 = tok.EndToken.Next;
                if ((t1 is Pullenti.Ner.TextToken) && (t1 as Pullenti.Ner.TextToken).Term.StartsWith("ОБ")) 
                {
                    tok.EndToken = t1;
                    t1 = t1.Next;
                    if (t1 != null && t1.IsChar('.')) 
                    {
                        tok.EndToken = t1;
                        t1 = t1.Next;
                    }
                }
                if ((t1 != null && t1.IsChar('(') && (t1.Next is Pullenti.Ner.TextToken)) && (t1.Next as Pullenti.Ner.TextToken).Term.StartsWith("ОБ")) 
                {
                    tok.EndToken = t1.Next;
                    t1 = t1.Next.Next;
                    while (t1 != null) 
                    {
                        if (t1.IsCharOf(".)")) 
                        {
                            tok.EndToken = t1;
                            t1 = t1.Next;
                        }
                        else 
                            break;
                    }
                }
                if (tok.Termin.Tag is AddressItemType) 
                {
                    if (tok.Termin.Tag2 is Pullenti.Ner.Address.AddressHouseType) 
                        houseTyp = (Pullenti.Ner.Address.AddressHouseType)tok.Termin.Tag2;
                    if (tok.Termin.Tag2 is Pullenti.Ner.Address.AddressBuildingType) 
                        buildTyp = (Pullenti.Ner.Address.AddressBuildingType)tok.Termin.Tag2;
                    typ = (AddressItemType)tok.Termin.Tag;
                    if (typ == AddressItemType.Plot) 
                    {
                        if (t.Previous != null && ((t.Previous.IsValue("СУДЕБНЫЙ", "СУДОВИЙ") || t.Previous.IsValue("ИЗБИРАТЕЛЬНЫЙ", "ВИБОРЧИЙ")))) 
                            return null;
                    }
                    if (t1 != null && t1.IsCharOf("\\/") && m_Ontology.TryParse(t1.Next, Pullenti.Ner.Core.TerminParseAttr.No) != null) 
                    {
                        AddressItemToken aa = TryParsePureItem(t1.Next, null, null);
                        if (aa != null && aa.Typ != AddressItemType.Number && aa.Value != null) 
                        {
                            int ii = aa.Value.IndexOf('/');
                            if (ii < 0) 
                                ii = aa.Value.IndexOf('\\');
                            if (ii > 0) 
                            {
                                AddressItemToken res = new AddressItemToken(typ, t, aa.EndToken);
                                res.Value = aa.Value.Substring(0, ii);
                                res.AltTyp = aa;
                                aa.Value = aa.Value.Substring(ii + 1);
                                return res;
                            }
                        }
                    }
                    if (typ == AddressItemType.House && houseTyp == Pullenti.Ner.Address.AddressHouseType.Special) 
                    {
                        Pullenti.Ner.Token tt2 = tok.EndToken.Next;
                        if (tt2 != null && tt2.IsHiphen) 
                            tt2 = tt2.Next;
                        AddressItemToken res = new AddressItemToken(typ, t, tok.EndToken) { HouseType = houseTyp, Value = tok.Termin.Acronym ?? tok.Termin.CanonicText };
                        Pullenti.Ner.Geo.Internal.NumToken num2 = Pullenti.Ner.Geo.Internal.NumToken.TryParse(tt2, Pullenti.Ner.Geo.Internal.GeoTokenType.Any);
                        if (num2 != null && (tt2.WhitespacesBeforeCount < 2)) 
                        {
                            res.Value = string.Format("{0}-{1}", res.Value, num2.Value);
                            res.EndToken = num2.EndToken;
                        }
                        return res;
                    }
                    if (typ == AddressItemType.Prefix) 
                    {
                        for (; t1 != null; t1 = t1.Next) 
                        {
                            if (((t1.Morph.Class.IsPreposition || t1.Morph.Class.IsConjunction)) && t1.WhitespacesAfterCount == 1) 
                                continue;
                            if (t1.IsChar(':')) 
                            {
                                t1 = t1.Next;
                                break;
                            }
                            if (t1.IsChar('(')) 
                            {
                                Pullenti.Ner.Core.BracketSequenceToken br = Pullenti.Ner.Core.BracketHelper.TryParse(t1, Pullenti.Ner.Core.BracketParseAttr.No, 100);
                                if (br != null && (br.LengthChar < 50)) 
                                {
                                    t1 = br.EndToken;
                                    continue;
                                }
                            }
                            if (t1 is Pullenti.Ner.TextToken) 
                            {
                                if (t1.Chars.IsAllLower || (t1.WhitespacesBeforeCount < 3)) 
                                {
                                    Pullenti.Ner.Core.NounPhraseToken npt = Pullenti.Ner.Geo.Internal.MiscLocationHelper.TryParseNpt(t1);
                                    if (npt != null && ((npt.Chars.IsAllLower || npt.Morph.Case.IsGenitive))) 
                                    {
                                        if (Pullenti.Ner.Geo.Internal.CityItemToken.CheckKeyword(npt.EndToken) == null && Pullenti.Ner.Geo.Internal.TerrItemToken.CheckKeyword(npt.EndToken) == null) 
                                        {
                                            t1 = npt.EndToken;
                                            continue;
                                        }
                                    }
                                }
                            }
                            if (t1.IsValue("УКАЗАННЫЙ", null) || t1.IsValue("ЕГРИП", null) || t1.IsValue("ФАКТИЧЕСКИЙ", null)) 
                                continue;
                            if (t1.IsComma) 
                            {
                                if (t1.Next != null && t1.Next.IsValue("УКАЗАННЫЙ", null)) 
                                    continue;
                            }
                            break;
                        }
                        if (t1 != null) 
                        {
                            Pullenti.Ner.Token t0 = t;
                            if (((t0.Previous != null && !t0.IsNewlineBefore && t0.Previous.IsChar(')')) && (t0.Previous.Previous is Pullenti.Ner.TextToken) && t0.Previous.Previous.Previous != null) && t0.Previous.Previous.Previous.IsChar('(')) 
                            {
                                t = t0.Previous.Previous.Previous.Previous;
                                if (t != null && t.GetMorphClassInDictionary().IsAdjective && !t.IsNewlineAfter) 
                                    t0 = t;
                            }
                            AddressItemToken res = new AddressItemToken(AddressItemType.Prefix, t0, t1.Previous);
                            for (Pullenti.Ner.Token tt = t0.Previous; tt != null; tt = tt.Previous) 
                            {
                                if (tt.NewlinesAfterCount > 3) 
                                    break;
                                if (tt.IsCommaAnd || tt.IsCharOf("().")) 
                                    continue;
                                if (!(tt is Pullenti.Ner.TextToken)) 
                                    break;
                                if (((tt.IsValue("ПОЧТОВЫЙ", null) || tt.IsValue("ЮРИДИЧЕСКИЙ", null) || tt.IsValue("ЮР", null)) || tt.IsValue("ФАКТИЧЕСКИЙ", null) || tt.IsValue("ФАКТ", null)) || tt.IsValue("ПОЧТ", null) || tt.IsValue("АДРЕС", null)) 
                                    res.BeginToken = tt;
                                else 
                                    break;
                            }
                            return res;
                        }
                        else 
                            return null;
                    }
                    else if ((typ == AddressItemType.CorpusOrFlat && !tok.IsWhitespaceBefore && !tok.IsWhitespaceAfter) && tok.BeginToken == tok.EndToken && tok.BeginToken.IsValue("К", null)) 
                    {
                        if (prev != null && prev.Typ == AddressItemType.Flat) 
                            typ = AddressItemType.Room;
                        else 
                            typ = AddressItemType.Corpus;
                    }
                    if (typ == AddressItemType.Detail && t.IsValue("У", null)) 
                    {
                        if (!Pullenti.Ner.Geo.Internal.MiscLocationHelper.CheckGeoObjectBefore(t, false)) 
                            return null;
                    }
                    if (typ == AddressItemType.Flat && t.IsValue("КВ", null)) 
                    {
                        if (t.GetSourceText() == "кВ") 
                            return null;
                    }
                    if (((typ == AddressItemType.Flat || typ == AddressItemType.Space || typ == AddressItemType.Office)) && !(tok.EndToken.Next is Pullenti.Ner.NumberToken)) 
                    {
                        AddressItemToken next = _tryParsePureItem(tok.EndToken.Next, false, null);
                        if (next != null && typ != AddressItemType.Office && ((next.Typ == AddressItemType.Pantry || next.Typ == AddressItemType.Flat))) 
                        {
                            if (typ != next.Typ) 
                                next.Typ = AddressItemType.Pantry;
                            next.BeginToken = t;
                            return next;
                        }
                        if (next != null && typ == AddressItemType.Office && ((next.Typ == AddressItemType.Space || next.Typ == AddressItemType.Flat))) 
                        {
                            next.Typ = AddressItemType.Office;
                            next.BeginToken = t;
                            return next;
                        }
                        if (tok.EndToken.Next != null && tok.EndToken.Next.IsChar('(')) 
                        {
                            Pullenti.Ner.Core.TerminToken tok2 = m_Ontology.TryParse(tok.EndToken.Next.Next, Pullenti.Ner.Core.TerminParseAttr.No);
                            if (tok2 != null && tok2.EndToken.Next != null && tok2.EndToken.Next.IsChar(')')) 
                                t1 = tok2.EndToken.Next.Next;
                        }
                    }
                    if (typ == AddressItemType.Pantry && !(tok.EndToken.Next is Pullenti.Ner.NumberToken)) 
                    {
                        AddressItemToken next = _tryParsePureItem(tok.EndToken.Next, false, null);
                        if (next != null && ((next.Typ == AddressItemType.Space || next.Typ == AddressItemType.Flat))) 
                        {
                            next.Typ = AddressItemType.Pantry;
                            next.BeginToken = t;
                            return next;
                        }
                    }
                    if (typ == AddressItemType.Floor && t1 != null) 
                    {
                        Pullenti.Ner.Token tt2 = t1;
                        if (tt2 != null && tt2.IsCharOf("\\/")) 
                            tt2 = tt2.Next;
                        AddressItemToken next = _tryParsePureItem(tt2, prefixBefore, prev);
                        if (next != null && next.Value == "подвал") 
                        {
                            tt2 = next.EndToken.Next;
                            if (tt2 != null && tt2.IsCharOf("\\/")) 
                                tt2 = tt2.Next;
                            AddressItemToken num2 = _tryParsePureItem(tt2, prefixBefore, prev);
                            if (num2 != null && num2.Typ == AddressItemType.Number && num2.Value != null) 
                            {
                                num2.Typ = typ;
                                num2.BeginToken = t;
                                num2.Value = "-" + num2.Value;
                                return num2;
                            }
                        }
                    }
                    if (typ == AddressItemType.Kilometer || typ == AddressItemType.Floor || typ == AddressItemType.Potch) 
                    {
                        if ((tok.EndToken.Next is Pullenti.Ner.NumberToken) || Pullenti.Ner.Core.MiscHelper.CheckNumberPrefix(tok.EndToken.Next) != null) 
                        {
                        }
                        else 
                            return new AddressItemToken(typ, t, tok.EndToken);
                    }
                    if (typ == AddressItemType.Space) 
                    {
                        if (tok.Termin.Tag2 != null) 
                        {
                            AddressItemToken res = new AddressItemToken(typ, t, tok.EndToken) { Value = tok.Termin.CanonicText.ToLower() };
                            if (res.EndToken.Next != null && res.EndToken.Next.IsHiphen) 
                            {
                                AddressItemToken next2 = TryParsePureItem(res.EndToken.Next.Next, null, null);
                                if (next2 != null && next2.Typ == AddressItemType.Space) 
                                    res.EndToken = next2.EndToken;
                            }
                            return res;
                        }
                        AddressItemToken next = TryParsePureItem(tok.EndToken.Next, null, null);
                        if (next != null && next.Typ == AddressItemType.Space) 
                        {
                            next.BeginToken = t;
                            return next;
                        }
                        Pullenti.Ner.Token tt = tok.EndToken.Next;
                        if (tt is Pullenti.Ner.TextToken) 
                        {
                            if ((tt as Pullenti.Ner.TextToken).Term.StartsWith("Н")) 
                            {
                                t1 = tt.Next;
                                if (tt.Next != null && tt.Next.IsChar('.')) 
                                    t1 = tt.Next.Next;
                            }
                        }
                    }
                    if ((typ == AddressItemType.House || typ == AddressItemType.Building || typ == AddressItemType.Corpus) || typ == AddressItemType.Plot || typ == AddressItemType.Box) 
                    {
                        for (Pullenti.Ner.Token tt2 = t1; tt2 != null; tt2 = tt2.Next) 
                        {
                            if (tt2.IsComma) 
                                continue;
                            if (tt2.IsValue("РАСПОЛОЖЕННЫЙ", null) || tt2.IsValue("НАХОДЯЩИЙСЯ", null) || tt2.IsValue("ПРИЛЕГАЮЩИЙ", null)) 
                                continue;
                            if (tt2.IsValue("ПОДВАЛ", null)) 
                            {
                                t1 = tt2.Next;
                                continue;
                            }
                            if (tt2.Morph.Class.IsPreposition) 
                                continue;
                            Pullenti.Ner.Core.TerminToken tok2 = m_Ontology.TryParse(tt2, Pullenti.Ner.Core.TerminParseAttr.No);
                            if (tok2 != null && (tok2.Termin.Tag is AddressItemType)) 
                            {
                                AddressItemType typ2 = (AddressItemType)tok2.Termin.Tag;
                                if (typ2 != typ && ((typ2 == AddressItemType.Plot || ((typ2 == AddressItemType.House && typ == AddressItemType.Plot))))) 
                                    return new AddressItemToken(typ, t, tt2.Previous) { Value = "0", HouseType = houseTyp };
                                if (typ == AddressItemType.Box && typ2 == AddressItemType.Space && tok2.Termin.CanonicText == "ПОДВАЛ") 
                                {
                                    tt2 = tok2.EndToken;
                                    t1 = tt2.Next;
                                    continue;
                                }
                            }
                            if (tt2 is Pullenti.Ner.TextToken) 
                            {
                                if ((tt2 as Pullenti.Ner.TextToken).Term.StartsWith("ДОП")) 
                                {
                                    t1 = tt2.Next;
                                    if (t1 != null && t1.IsChar('.')) 
                                    {
                                        tt2 = tt2.Next;
                                        t1 = t1.Next;
                                    }
                                    continue;
                                }
                            }
                            break;
                        }
                    }
                    if (typ == AddressItemType.House && t1 != null && t1.Chars.IsLetter) 
                    {
                        AddressItemToken next = TryParsePureItem(t1, prev, null);
                        if (next != null && ((next.Typ == typ || next.Typ == AddressItemType.Plot))) 
                        {
                            next.BeginToken = t;
                            return next;
                        }
                    }
                    if (typ == AddressItemType.Flat && (t1 is Pullenti.Ner.TextToken) && ((t1.IsValue("М", null) || t1.IsValue("M", null)))) 
                    {
                        if (t1.Next is Pullenti.Ner.NumberToken) 
                            t1 = t1.Next;
                        else if (t1.Next != null && t1.Next.IsChar('.') && (t1.Next.Next is Pullenti.Ner.NumberToken)) 
                            t1 = t1.Next.Next;
                    }
                    if (typ == AddressItemType.Room && t1 != null && t1.IsCharOf("\\/")) 
                    {
                        AddressItemToken next = _tryParsePureItem(t1.Next, prefixBefore, prev);
                        if (next != null && ((next.Typ == AddressItemType.Room || next.Typ == AddressItemType.Office))) 
                        {
                            next.BeginToken = t;
                            return next;
                        }
                    }
                    if (typ == AddressItemType.Field) 
                    {
                        Pullenti.Ner.Core.NumberExToken nt2 = Pullenti.Ner.Core.NumberHelper.TryParseNumberWithPostfix(t1);
                        if (nt2 != null && ((nt2.ExTyp == Pullenti.Ner.Core.NumberExType.Meter2 || nt2.ExTyp == Pullenti.Ner.Core.NumberExType.Gektar || nt2.ExTyp == Pullenti.Ner.Core.NumberExType.Ar))) 
                            return new AddressItemToken(typ, t, nt2.EndToken) { Value = nt2.ToString() };
                        AddressItemToken re = new AddressItemToken(typ, t, tok.EndToken);
                        StringBuilder nnn = new StringBuilder();
                        for (Pullenti.Ner.Token tt = tok.EndToken.Next; tt != null; tt = tt.Next) 
                        {
                            Pullenti.Ner.NumberToken ll = Pullenti.Ner.Core.NumberHelper.TryParseRoman(tt);
                            if (ll != null && ll.IntValue != null) 
                            {
                                if (nnn.Length > 0) 
                                    nnn.Append("-");
                                nnn.Append(ll.Value);
                                re.EndToken = (tt = ll.EndToken);
                                continue;
                            }
                            if (tt.IsHiphen) 
                                continue;
                            if (tt.IsWhitespaceBefore) 
                                break;
                            if (tt is Pullenti.Ner.NumberToken) 
                            {
                                if (nnn.Length > 0) 
                                    nnn.Append("-");
                                nnn.Append((tt as Pullenti.Ner.NumberToken).Value);
                                re.EndToken = tt;
                                continue;
                            }
                            if ((tt is Pullenti.Ner.TextToken) && tt.Chars.IsAllUpper) 
                            {
                                if (nnn.Length > 0) 
                                    nnn.Append("-");
                                nnn.Append((tt as Pullenti.Ner.TextToken).Term);
                                re.EndToken = tt;
                                continue;
                            }
                            break;
                        }
                        if (nnn.Length > 0) 
                        {
                            re.Value = nnn.ToString();
                            return re;
                        }
                    }
                    if (typ == AddressItemType.NoNumber) 
                        return new AddressItemToken(AddressItemType.NoNumber, t, tok.EndToken) { Value = "0", IsDoubt = false };
                    if (typ == AddressItemType.House || typ == AddressItemType.Plot) 
                    {
                        if (t1 != null && t1.IsValue("ЛПХ", null)) 
                            t1 = t1.Next;
                    }
                    if ((typ != AddressItemType.Number && (t1 is Pullenti.Ner.TextToken) && t1.Chars.IsLetter) && !t1.Chars.IsAllUpper) 
                    {
                        AddressItemToken next = TryParsePureItem(t1, null, null);
                        if ((next != null && next.Typ != AddressItemType.Number && next.Typ != AddressItemType.NoNumber) && next.Value != null) 
                        {
                            next.BeginToken = t;
                            return next;
                        }
                    }
                    if (typ != AddressItemType.Number) 
                    {
                        if ((((t1 == null || tok.IsNewlineAfter)) && t.LengthChar > 1 && ((prev != null || Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(t)))) && !tok.IsNewlineBefore) 
                            return new AddressItemToken(typ, t, tok.EndToken) { HouseType = houseTyp, BuildingType = buildTyp, Value = "0" };
                    }
                    if (typ == AddressItemType.Plot || typ == AddressItemType.Well) 
                    {
                        Pullenti.Ner.Geo.Internal.NumToken num1 = Pullenti.Ner.Geo.Internal.NumToken.TryParse(t1, Pullenti.Ner.Geo.Internal.GeoTokenType.House);
                        if (num1 != null) 
                            return new AddressItemToken(typ, t, num1.EndToken) { Value = num1.Value };
                    }
                }
            }
            if ((t1 != null && t1.IsComma && typ == AddressItemType.Flat) && (t1.Next is Pullenti.Ner.NumberToken)) 
                t1 = t1.Next;
            if (t1 != null && t1.IsChar('.') && t1.Next != null) 
                t1 = t1.Next;
            if ((t1 != null && t1 != t && ((t1.IsHiphen || t1.IsCharOf("_:")))) && (t1.Next is Pullenti.Ner.NumberToken)) 
                t1 = t1.Next;
            tok = m_Ontology.TryParse(t1, Pullenti.Ner.Core.TerminParseAttr.No);
            if (tok != null && (tok.Termin.Tag is AddressItemType) && ((AddressItemType)tok.Termin.Tag) == AddressItemType.Number) 
                t1 = tok.EndToken.Next;
            else if (tok != null && (tok.Termin.Tag is AddressItemType) && ((AddressItemType)tok.Termin.Tag) == AddressItemType.NoNumber) 
            {
                AddressItemToken re0 = new AddressItemToken(typ, t, tok.EndToken) { Value = "0", HouseType = houseTyp, BuildingType = buildTyp };
                if (!re0.IsWhitespaceAfter && (re0.EndToken.Next is Pullenti.Ner.NumberToken)) 
                {
                    re0.EndToken = re0.EndToken.Next;
                    re0.Value = (re0.EndToken as Pullenti.Ner.NumberToken).Value.ToString();
                }
                return re0;
            }
            else if (t1 is Pullenti.Ner.TextToken) 
            {
                string term = (t1 as Pullenti.Ner.TextToken).Term;
                if (((term.Length == 7 && term.StartsWith("ЛИТЕРА"))) || ((term.Length == 6 && term.StartsWith("ЛИТЕР"))) || ((term.Length == 4 && term.StartsWith("ЛИТ")))) 
                {
                    string txt = t1.GetSourceText();
                    if (((char.IsLower(txt[0]) && char.IsUpper(txt[txt.Length - 1]))) || term.Length == 7) 
                    {
                        AddressItemToken res1 = new AddressItemToken(AddressItemType.Building, t, t1);
                        res1.BuildingType = Pullenti.Ner.Address.AddressBuildingType.Liter;
                        res1.Value = term.Substring(term.Length - 1);
                        return res1;
                    }
                }
                if (term.StartsWith("БЛОК") && term.Length > 4) 
                {
                    string txt = t1.GetSourceText();
                    if (char.IsLower(txt[0]) && char.IsUpper(txt[4])) 
                    {
                        Pullenti.Ner.Geo.Internal.NumToken num1 = Pullenti.Ner.Geo.Internal.NumToken.TryParse(t1.Next, Pullenti.Ner.Geo.Internal.GeoTokenType.Org);
                        if (num1 != null) 
                        {
                            AddressItemToken res1 = new AddressItemToken(AddressItemType.Block, t, num1.EndToken);
                            res1.Value = term.Substring(4) + num1.Value;
                            return res1;
                        }
                    }
                }
                if (typ == AddressItemType.Flat && t1 != null) 
                {
                    Pullenti.Ner.Core.TerminToken tok2 = m_Ontology.TryParse(t1, Pullenti.Ner.Core.TerminParseAttr.No);
                    if (tok2 == null && t1.IsComma) 
                        tok2 = m_Ontology.TryParse(t1.Next, Pullenti.Ner.Core.TerminParseAttr.No);
                    if (tok2 != null && ((AddressItemType)tok2.Termin.Tag) == AddressItemType.Flat) 
                        t1 = tok2.EndToken.Next;
                }
                if (t1 != null && t1.IsValue2("СТРОИТЕЛЬНЫЙ", "НОМЕР")) 
                    t1 = t1.Next;
                Pullenti.Ner.Token ttt = Pullenti.Ner.Core.MiscHelper.CheckNumberPrefix(t1);
                if (ttt != null) 
                {
                    t1 = ttt;
                    if (t1.IsHiphen || t1.IsChar('_')) 
                        t1 = t1.Next;
                }
            }
            if (typ != AddressItemType.Number) 
            {
                if (t1 != null && t1.IsChar('.') && Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(t1)) 
                    t1 = t1.Next;
            }
            if (t1 == null) 
            {
                if (typ == AddressItemType.Genplan) 
                    return new AddressItemToken(typ, t, tok00.EndToken) { Value = "0" };
                return null;
            }
            StringBuilder num = new StringBuilder();
            Pullenti.Ner.NumberToken nt = t1 as Pullenti.Ner.NumberToken;
            AddressItemToken re11;
            if (nt != null) 
            {
                if (nt.IntValue == null) 
                    return null;
                if (typ == AddressItemType.Room || typ == AddressItemType.CorpusOrFlat) 
                {
                    Pullenti.Ner.Core.NumberExToken nt2 = Pullenti.Ner.Core.NumberHelper.TryParseNumberWithPostfix(t1);
                    if (nt2 != null && nt2.ExTyp == Pullenti.Ner.Core.NumberExType.Meter2) 
                        return new AddressItemToken(AddressItemType.Room, t, nt2.EndToken) { Value = nt2.ToString() };
                }
                if (typ == AddressItemType.Field || typ == AddressItemType.Plot) 
                {
                    Pullenti.Ner.Core.NumberExToken nt2 = Pullenti.Ner.Core.NumberHelper.TryParseNumberWithPostfix(t1);
                    if (nt2 != null && ((nt2.ExTyp == Pullenti.Ner.Core.NumberExType.Meter2 || nt2.ExTyp == Pullenti.Ner.Core.NumberExType.Gektar || nt2.ExTyp == Pullenti.Ner.Core.NumberExType.Ar))) 
                        return new AddressItemToken(typ, t, nt2.EndToken) { Value = nt2.ToString() };
                }
                num.Append(nt.Value);
                if (nt.Typ == Pullenti.Ner.NumberSpellingType.Digit || nt.Typ == Pullenti.Ner.NumberSpellingType.Words) 
                {
                    if (((nt.EndToken is Pullenti.Ner.TextToken) && (nt.EndToken as Pullenti.Ner.TextToken).Term == "Е" && nt.EndToken.Previous == nt.BeginToken) && !nt.EndToken.IsWhitespaceBefore) 
                        num.Append("Е");
                    bool drob = false;
                    bool hiph = false;
                    bool lit = false;
                    Pullenti.Ner.Token et = nt.Next;
                    if (et != null && ((et.IsCharOf("\\/") || et.IsValue("ДРОБЬ", null) || ((et.IsChar('.') && Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(et) && (et.Next is Pullenti.Ner.NumberToken)))))) 
                    {
                        AddressItemToken next = TryParsePureItem(et.Next, null, null);
                        if (next != null && next.Typ != AddressItemType.Number && typ != AddressItemType.Flat) 
                        {
                            if (next.Typ == typ && next.Value != null) 
                            {
                                next.Value = string.Format("{0}/{1}", num.ToString(), next.Value);
                                next.BeginToken = t;
                                return next;
                            }
                            t1 = et;
                        }
                        else 
                        {
                            drob = true;
                            et = et.Next;
                            if (et != null && et.IsCharOf("\\/")) 
                                et = et.Next;
                        }
                    }
                    else if (et != null && ((et.IsHiphen || et.IsChar('_')))) 
                    {
                        hiph = true;
                        et = et.Next;
                    }
                    else if ((et != null && et.IsChar('.') && (et.Next is Pullenti.Ner.NumberToken)) && !et.IsWhitespaceAfter) 
                    {
                        hiph = true;
                        et = et.Next;
                    }
                    if (et is Pullenti.Ner.NumberToken) 
                    {
                        if (drob) 
                        {
                            AddressItemToken next = TryParsePureItem(et, null, null);
                            if (next != null && next.Typ == AddressItemType.Number) 
                            {
                                num.AppendFormat("/{0}", next.Value);
                                t1 = next.EndToken;
                                et = t1.Next;
                                drob = false;
                            }
                            else 
                            {
                                num.AppendFormat("/{0}", (et as Pullenti.Ner.NumberToken).Value);
                                drob = false;
                                t1 = et;
                                et = et.Next;
                                if (et != null && et.IsCharOf("\\/") && (et.Next is Pullenti.Ner.NumberToken)) 
                                {
                                    t1 = et.Next;
                                    num.AppendFormat("/{0}", (t1 as Pullenti.Ner.NumberToken).Value);
                                    et = t1.Next;
                                }
                            }
                        }
                        else if ((hiph && !t1.IsWhitespaceAfter && (et is Pullenti.Ner.NumberToken)) && !et.IsWhitespaceBefore) 
                        {
                            AddressItemToken numm = TryParsePureItem(et, null, null);
                            if (numm != null && numm.Typ == AddressItemType.Number) 
                            {
                                bool merge = false;
                                if (typ == AddressItemType.Flat || typ == AddressItemType.Plot || typ == AddressItemType.Office) 
                                    merge = true;
                                else if (typ == AddressItemType.House || typ == AddressItemType.Building || typ == AddressItemType.Corpus) 
                                {
                                    Pullenti.Ner.Token ttt = numm.EndToken.Next;
                                    if (ttt != null && ttt.IsComma) 
                                        ttt = ttt.Next;
                                    AddressItemToken numm2 = TryParsePureItem(ttt, null, null);
                                    if (numm2 != null) 
                                    {
                                        if ((numm2.Typ == AddressItemType.Flat || numm2.Typ == AddressItemType.Building || ((numm2.Typ == AddressItemType.CorpusOrFlat && numm2.Value != null))) || numm2.Typ == AddressItemType.Corpus) 
                                            merge = true;
                                    }
                                }
                                if (merge) 
                                {
                                    num.AppendFormat("/{0}", numm.Value);
                                    t1 = numm.EndToken;
                                    et = t1.Next;
                                    hiph = false;
                                }
                            }
                        }
                    }
                    else if (et != null && ((et.IsHiphen || et.IsChar('_') || et.IsValue("НЕТ", null))) && drob) 
                        t1 = et;
                    Pullenti.Ner.Token ett = et;
                    if ((ett != null && ett.IsCharOf(",.") && (ett.WhitespacesAfterCount < 2)) && (ett.Next is Pullenti.Ner.TextToken)) 
                    {
                        if (Pullenti.Ner.Core.BracketHelper.IsBracket(ett.Next, false)) 
                            ett = ett.Next;
                        else if (ett.Next.LengthChar == 1 && ett.Next.Chars.IsLetter && ((ett.Next.Next == null || ett.Next.Next.IsComma))) 
                        {
                            string ch = CorrectCharToken(ett.Next);
                            if (ch != null) 
                            {
                                num.Append(ch);
                                ett = ett.Next;
                                t1 = ett;
                            }
                        }
                    }
                    if ((Pullenti.Ner.Core.BracketHelper.IsBracket(ett, false) && (ett.Next is Pullenti.Ner.TextToken) && ett.Next.LengthChar == 1) && ett.Next.IsLetters && Pullenti.Ner.Core.BracketHelper.IsBracket(ett.Next.Next, false)) 
                    {
                        string ch = CorrectCharToken(ett.Next);
                        if (ch != null) 
                        {
                            num.Append(ch);
                            t1 = ett.Next.Next;
                        }
                        else 
                        {
                            Pullenti.Ner.NumberToken ntt = Pullenti.Ner.Core.NumberHelper.TryParseRoman(ett.Next);
                            if (ntt != null) 
                            {
                                num.AppendFormat("/{0}", ntt.Value);
                                t1 = ett.Next.Next;
                            }
                        }
                    }
                    else if (((Pullenti.Ner.Core.BracketHelper.IsBracket(ett, false) && Pullenti.Ner.Core.BracketHelper.IsBracket(ett.Next, false) && (ett.Next.Next is Pullenti.Ner.TextToken)) && ett.Next.Next.LengthChar == 1 && ett.Next.Next.IsLetters) && Pullenti.Ner.Core.BracketHelper.IsBracket(ett.Next.Next.Next, false)) 
                    {
                        string ch = CorrectCharToken(ett.Next.Next);
                        if (ch != null) 
                        {
                            num.Append(ch);
                            t1 = ett.Next.Next.Next;
                            while (t1.Next != null && Pullenti.Ner.Core.BracketHelper.IsBracket(t1.Next, false)) 
                            {
                                t1 = t1.Next;
                            }
                        }
                    }
                    else if (Pullenti.Ner.Core.BracketHelper.CanBeStartOfSequence(ett, true, false) && (ett.WhitespacesBeforeCount < 2)) 
                    {
                        Pullenti.Ner.Core.BracketSequenceToken br = Pullenti.Ner.Core.BracketHelper.TryParse(ett, Pullenti.Ner.Core.BracketParseAttr.No, 100);
                        if (br != null && (br.BeginToken.Next is Pullenti.Ner.TextToken) && br.BeginToken.Next.Next == br.EndToken) 
                        {
                            string s = CorrectCharToken(br.BeginToken.Next);
                            if (s != null) 
                            {
                                num.Append(s);
                                t1 = br.EndToken;
                            }
                        }
                    }
                    else if ((et is Pullenti.Ner.TextToken) && (((et as Pullenti.Ner.TextToken).LengthChar == 1 || ((et.LengthChar == 2 && et.Chars.IsAllUpper && !et.IsWhitespaceBefore)))) && et.Chars.IsLetter) 
                    {
                        StreetItemToken ttt = StreetItemToken.TryParse(et, null, false, null);
                        string s = CorrectCharToken(et);
                        if (ttt != null && ((ttt.Typ == StreetItemType.StdName || ttt.Typ == StreetItemType.Noun || ttt.Typ == StreetItemType.Fix))) 
                            s = null;
                        else if (Pullenti.Ner.Geo.Internal.TerrItemToken.CheckKeyword(et) != null) 
                            s = null;
                        if (et.IsWhitespaceBefore) 
                        {
                            AddressItemToken next = TryParsePureItem(et, null, null);
                            if (next != null && next.Value != null) 
                                s = null;
                            else if (et.Previous != null && et.Previous.IsHiphen && et.Previous.IsWhitespaceBefore) 
                                s = null;
                        }
                        if (s != null) 
                        {
                            if (((s == "К" || s == "С")) && (et.Next is Pullenti.Ner.NumberToken) && !et.IsWhitespaceAfter) 
                            {
                            }
                            else if ((s == "Б" && et.Next != null && et.Next.IsCharOf("/\\")) && (et.Next.Next is Pullenti.Ner.TextToken) && et.Next.Next.IsValue("Н", null)) 
                                t1 = (et = et.Next.Next);
                            else 
                            {
                                bool ok = false;
                                if (drob || hiph || lit) 
                                    ok = true;
                                else if (!et.IsWhitespaceBefore || ((et.WhitespacesBeforeCount == 1 && ((Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(et) || et.Chars.IsAllUpper || ((et.IsNewlineAfter || ((et.Next != null && et.Next.IsComma))))))))) 
                                {
                                    ok = true;
                                    if (et.Next is Pullenti.Ner.NumberToken) 
                                    {
                                        if (!et.IsWhitespaceBefore && et.IsWhitespaceAfter) 
                                        {
                                        }
                                        else 
                                            ok = false;
                                    }
                                    if (s == "К") 
                                    {
                                        AddressItemToken tmp = new AddressItemToken(typ, t, et.Previous);
                                        AddressItemToken next = TryParsePureItem(et, tmp, null);
                                        if (next != null && next.Value != null) 
                                            ok = false;
                                    }
                                    if (s == "И") 
                                    {
                                        AddressItemToken next = TryParsePureItem(et.Next, prev, null);
                                        if (next != null && next.Typ == typ) 
                                            ok = false;
                                    }
                                }
                                else if (((et.Next == null || et.Next.IsComma)) && (((et.WhitespacesBeforeCount < 2) || Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(et)))) 
                                    ok = true;
                                else if (et.IsWhitespaceBefore && et.Chars.IsAllLower && et.IsValue("В", "У")) 
                                {
                                }
                                else 
                                {
                                    AddressItemToken aitNext = TryParsePureItem(et.Next, null, null);
                                    if (aitNext != null) 
                                    {
                                        if ((aitNext.Typ == AddressItemType.Corpus || aitNext.Typ == AddressItemType.Flat || aitNext.Typ == AddressItemType.Building) || aitNext.Typ == AddressItemType.Office || aitNext.Typ == AddressItemType.Room) 
                                            ok = true;
                                    }
                                }
                                if (ok) 
                                {
                                    num.Append(s);
                                    t1 = et;
                                    if (et.Next != null && et.Next.IsCharOf("\\/") && et.Next.Next != null) 
                                    {
                                        if (et.Next.Next is Pullenti.Ner.NumberToken) 
                                        {
                                            num.AppendFormat("/{0}", (et.Next.Next as Pullenti.Ner.NumberToken).Value);
                                            t1 = (et = et.Next.Next);
                                        }
                                        else if (et.Next.Next.IsHiphen || et.Next.Next.IsChar('_') || et.Next.Next.IsValue("НЕТ", null)) 
                                            t1 = (et = et.Next.Next);
                                    }
                                }
                            }
                        }
                    }
                    else if ((et is Pullenti.Ner.TextToken) && !et.IsWhitespaceBefore) 
                    {
                        string val = (et as Pullenti.Ner.TextToken).Term;
                        if (val == "КМ" && typ == AddressItemType.House) 
                        {
                            t1 = et;
                            num.Append("КМ");
                        }
                        else if (val == "БН") 
                            t1 = et;
                        else if (((val.Length == 2 && val[1] == 'Б' && et.Next != null) && et.Next.IsCharOf("\\/") && et.Next.Next != null) && et.Next.Next.IsValue("Н", null)) 
                        {
                            num.Append(val[0]);
                            t1 = (et = et.Next.Next);
                        }
                    }
                    if (!drob && t1.Next != null && t1.Next.IsCharOf("\\/")) 
                    {
                        AddressItemToken next = _tryParsePureItem(t1.Next.Next, false, null);
                        if (next != null && next.Typ == AddressItemType.Number) 
                        {
                            num.AppendFormat("/{0}", next.Value);
                            t1 = next.EndToken;
                        }
                    }
                }
            }
            else if ((((re11 = _tryAttachVCH(t1, typ)))) != null) 
            {
                re11.BeginToken = t;
                re11.HouseType = houseTyp;
                re11.BuildingType = buildTyp;
                return re11;
            }
            else if (((t1 is Pullenti.Ner.TextToken) && t1.LengthChar == 2 && t1.IsLetters) && !t1.IsWhitespaceBefore && (t1.Previous is Pullenti.Ner.NumberToken)) 
            {
                string src = t1.GetSourceText();
                if ((src != null && src.Length == 2 && ((src[0] == 'к' || src[0] == 'k'))) && char.IsUpper(src[1])) 
                {
                    char ch = CorrectChar(src[1]);
                    if (ch != ((char)0)) 
                        return new AddressItemToken(AddressItemType.Corpus, t1, t1) { Value = string.Format("{0}", ch) };
                }
            }
            else if ((t1 is Pullenti.Ner.TextToken) && t1.LengthChar == 1 && t1.IsLetters) 
            {
                string ch = CorrectCharToken(t1);
                if (ch != null) 
                {
                    if (typ == AddressItemType.Number) 
                        return null;
                    if (ch == "К" || ch == "С") 
                    {
                        if (!t1.IsWhitespaceAfter && (t1.Next is Pullenti.Ner.NumberToken)) 
                            return null;
                    }
                    if (ch == "С") 
                    {
                        Pullenti.Ner.Geo.Internal.NumToken num1 = Pullenti.Ner.Geo.Internal.NumToken.TryParse(t1.Next, Pullenti.Ner.Geo.Internal.GeoTokenType.Any);
                        if (num1 != null && ((num1.HasPrefix || num1.IsCadasterNumber))) 
                            return new AddressItemToken(typ, t, num1.EndToken) { Value = num1.Value };
                    }
                    if (ch == "Д" && typ == AddressItemType.Plot) 
                    {
                        AddressItemToken rrr = TryParsePureItem(t1, null, null);
                        if (rrr != null) 
                        {
                            rrr.Typ = AddressItemType.Plot;
                            rrr.BeginToken = t;
                            return rrr;
                        }
                    }
                    if (ch == "С" && t1.IsWhitespaceAfter) 
                    {
                        AddressItemToken next = TryParsePureItem(t1.Next, null, null);
                        if (next != null && next.Typ == AddressItemType.Number) 
                        {
                            AddressItemToken res1 = new AddressItemToken(typ, t, next.EndToken) { Value = next.Value, Morph = t.Morph, HouseType = houseTyp, BuildingType = buildTyp };
                            return res1;
                        }
                    }
                    if (prev != null && ((prev.Typ == AddressItemType.House || prev.Typ == AddressItemType.Number || prev.Typ == AddressItemType.Flat)) && Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(t1)) 
                    {
                        if (typ == AddressItemType.CorpusOrFlat && prev.Typ == AddressItemType.House) 
                            typ = AddressItemType.Corpus;
                    }
                    else 
                    {
                        if (t1.Chars.IsAllLower && ((t1.Morph.Class.IsPreposition || t1.Morph.Class.IsConjunction))) 
                        {
                            if ((t1.WhitespacesAfterCount < 2) && t1.Next.Chars.IsLetter) 
                            {
                                if (typ == AddressItemType.House || typ == AddressItemType.Plot || typ == AddressItemType.Box) 
                                    return new AddressItemToken(typ, t, t1.Previous) { Value = "0" };
                                return null;
                            }
                        }
                        if (t.Chars.IsAllUpper && t.LengthChar == 1 && t.Next.IsChar('.')) 
                            return null;
                    }
                    num.Append(ch);
                    if ((t1.Next != null && ((t1.Next.IsHiphen || t1.Next.IsChar('_'))) && !t1.IsWhitespaceAfter) && (t1.Next.Next is Pullenti.Ner.NumberToken) && !t1.Next.IsWhitespaceAfter) 
                    {
                        num.Append((t1.Next.Next as Pullenti.Ner.NumberToken).Value);
                        t1 = t1.Next.Next;
                    }
                    else if ((t1.Next is Pullenti.Ner.NumberToken) && !t1.IsWhitespaceAfter) 
                    {
                        num.Append((t1.Next as Pullenti.Ner.NumberToken).Value);
                        t1 = t1.Next;
                    }
                    if (num.Length == 1 && ((typ == AddressItemType.Office || typ == AddressItemType.Room))) 
                        return null;
                }
                if ((((typ == AddressItemType.Box || typ == AddressItemType.Space || typ == AddressItemType.Part) || typ == AddressItemType.Carplace || typ == AddressItemType.Well)) && num.Length == 0) 
                {
                    Pullenti.Ner.NumberToken rom = Pullenti.Ner.Core.NumberHelper.TryParseRoman(t1);
                    if (rom != null) 
                        return new AddressItemToken(typ, t, rom.EndToken) { Value = rom.Value.ToString() };
                }
            }
            else if (((Pullenti.Ner.Core.BracketHelper.IsBracket(t1, false) && (t1.Next is Pullenti.Ner.TextToken) && t1.Next.LengthChar == 1) && t1.Next.IsLetters && Pullenti.Ner.Core.BracketHelper.IsBracket(t1.Next.Next, false)) && !t1.IsWhitespaceAfter && !t1.Next.IsWhitespaceAfter) 
            {
                string ch = CorrectCharToken(t1.Next);
                if (ch == null) 
                    return null;
                num.Append(ch);
                t1 = t1.Next.Next;
            }
            else if ((t1 is Pullenti.Ner.TextToken) && ((((t1.LengthChar == 1 && ((t1.IsHiphen || t1.IsChar('_'))))) || t1.IsValue("НЕТ", null) || t1.IsValue("БН", null))) && (((typ == AddressItemType.Corpus || typ == AddressItemType.CorpusOrFlat || typ == AddressItemType.Building) || typ == AddressItemType.House || typ == AddressItemType.Flat))) 
            {
                while (t1.Next != null && ((t1.Next.IsHiphen || t1.Next.IsChar('_'))) && !t1.IsWhitespaceAfter) 
                {
                    t1 = t1.Next;
                }
                string val = null;
                if (!t1.IsWhitespaceAfter && (t1.Next is Pullenti.Ner.NumberToken)) 
                {
                    t1 = t1.Next;
                    val = (t1 as Pullenti.Ner.NumberToken).Value.ToString();
                }
                if (t1.IsValue("БН", null)) 
                    val = "0";
                else if (t1.IsValue("НЕТ", null)) 
                    val = "НЕТ";
                return new AddressItemToken(typ, t, t1) { Value = val };
            }
            else 
            {
                if (((typ == AddressItemType.Floor || typ == AddressItemType.Kilometer || typ == AddressItemType.Potch)) && (t.Previous is Pullenti.Ner.NumberToken)) 
                    return new AddressItemToken(typ, t, t1.Previous);
                if ((t1 is Pullenti.Ner.ReferentToken) && (t1.GetReferent() is Pullenti.Ner.Date.DateReferent)) 
                {
                    AddressItemToken nn = TryParsePureItem((t1 as Pullenti.Ner.ReferentToken).BeginToken, null, null);
                    if (nn != null && nn.EndChar == t1.EndChar && nn.Typ == AddressItemType.Number) 
                    {
                        nn.BeginToken = t;
                        nn.EndToken = t1;
                        nn.Typ = typ;
                        return nn;
                    }
                }
                if ((t1 is Pullenti.Ner.TextToken) && ((typ == AddressItemType.House || typ == AddressItemType.Building || typ == AddressItemType.Corpus))) 
                {
                    string ter = (t1 as Pullenti.Ner.TextToken).Term;
                    if (ter == "АБ" || ter == "АБВ" || ter == "МГУ") 
                        return new AddressItemToken(typ, t, t1) { Value = ter, HouseType = houseTyp, BuildingType = buildTyp };
                    string ccc = _corrNumber(ter);
                    if (ccc != null) 
                        return new AddressItemToken(typ, t, t1) { Value = ccc, HouseType = houseTyp, BuildingType = buildTyp };
                    if (t1.Chars.IsAllUpper) 
                    {
                        if (prev != null && ((prev.Typ == AddressItemType.Street || prev.Typ == AddressItemType.City))) 
                            return new AddressItemToken(typ, t, t1) { Value = ter, HouseType = houseTyp, BuildingType = buildTyp };
                        if (typ == AddressItemType.Corpus && (t1.LengthChar < 4)) 
                            return new AddressItemToken(typ, t, t1) { Value = ter, HouseType = houseTyp, BuildingType = buildTyp };
                        if (typ == AddressItemType.Building && buildTyp == Pullenti.Ner.Address.AddressBuildingType.Liter && (t1.LengthChar < 4)) 
                            return new AddressItemToken(typ, t, t1) { Value = ter, HouseType = houseTyp, BuildingType = buildTyp };
                    }
                }
                if ((typ == AddressItemType.Box || typ == AddressItemType.Space || typ == AddressItemType.Part) || typ == AddressItemType.Carplace || typ == AddressItemType.Well) 
                {
                    Pullenti.Ner.Geo.Internal.NumToken num1 = Pullenti.Ner.Geo.Internal.NumToken.TryParse(t1, Pullenti.Ner.Geo.Internal.GeoTokenType.Any);
                    if (num1 != null) 
                        return new AddressItemToken(typ, t, num1.EndToken) { Value = num1.Value };
                }
                if (typ == AddressItemType.Plot && t1 != null) 
                {
                    if ((t1.IsValue("ОКОЛО", null) || t1.IsValue("РЯДОМ", null) || t1.IsValue("НАПРОТИВ", null)) || t1.IsValue("БЛИЗЬКО", null) || t1.IsValue("НАВПАКИ", null)) 
                        return new AddressItemToken(typ, t, t1) { Value = t1.GetSourceText().ToLower() };
                    AddressItemToken det = _tryAttachDetail(t1, null);
                    if (det != null) 
                        return new AddressItemToken(typ, t, t1.Previous) { Value = "0" };
                }
                if (t1 != null && t1.IsComma && prev != null) 
                {
                    AddressItemToken next = TryParsePureItem(t1.Next, null, null);
                    if (next != null) 
                    {
                        if (next.Typ == AddressItemType.Number || next.Typ == typ) 
                            return new AddressItemToken(typ, t, next.EndToken) { Value = next.Value };
                        if (prev != null) 
                            return new AddressItemToken(typ, t, t1.Previous);
                    }
                }
                if (t1 != null && t1.IsChar('(')) 
                {
                    AddressItemToken next = TryParsePureItem(t1.Next, prev, null);
                    if ((next != null && next.Typ == AddressItemType.Number && next.EndToken.Next != null) && next.EndToken.Next.IsChar(')')) 
                    {
                        next.Typ = typ;
                        next.BeginToken = t;
                        next.EndToken = next.EndToken.Next;
                        return next;
                    }
                }
                if (Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(t1) || typ != AddressItemType.Number) 
                {
                    Pullenti.Ner.NumberToken nt1 = Pullenti.Ner.Core.NumberHelper.TryParseRoman(t1);
                    if (nt1 != null) 
                        return new AddressItemToken(typ, t, t1) { Value = nt1.Value };
                }
                if (Pullenti.Ner.Core.BracketHelper.IsBracket(t1, false) && (t1.Next is Pullenti.Ner.NumberToken)) 
                {
                    AddressItemToken next = _tryParsePureItem(t1.Next, false, prev);
                    if ((next != null && next.Typ == AddressItemType.Number && next.EndToken.Next != null) && Pullenti.Ner.Core.BracketHelper.IsBracket(next.EndToken.Next, false)) 
                    {
                        next.BeginToken = t;
                        next.Typ = typ;
                        next.EndToken = next.EndToken.Next;
                        return next;
                    }
                }
                if (typ == AddressItemType.Genplan) 
                    return new AddressItemToken(typ, t, tok00.EndToken) { Value = "0" };
                if (typ == AddressItemType.Number && t.IsValue("ОБЩЕЖИТИЕ", null)) 
                {
                    Pullenti.Ner.Geo.Internal.NumToken num1 = Pullenti.Ner.Geo.Internal.NumToken.TryParse(t.Next, Pullenti.Ner.Geo.Internal.GeoTokenType.Any);
                    if (num1 != null) 
                        return new AddressItemToken(AddressItemType.House, t, num1.EndToken) { Value = num1.Value };
                }
                if (t.Chars.IsLatinLetter && !t.Kit.BaseLanguage.IsEn) 
                {
                    Pullenti.Ner.NumberToken num2 = Pullenti.Ner.Core.NumberHelper.TryParseRoman(t);
                    if (num2 != null) 
                        return new AddressItemToken(typ, t, num2.EndToken) { Value = num2.Value };
                }
                return null;
            }
            if (typ == AddressItemType.Number && prepos) 
                return null;
            if (t1 == null) 
            {
                t1 = t;
                while (t1.Next != null) 
                {
                    t1 = t1.Next;
                }
            }
            for (Pullenti.Ner.Token tt = t.Next; tt != null && tt.EndChar <= t1.EndChar; tt = tt.Next) 
            {
                if (tt.IsNewlineBefore && !(tt is Pullenti.Ner.NumberToken)) 
                    return null;
            }
            if (num.Length == 0) 
            {
                if (t.Chars.IsLatinLetter && ((!t.Kit.BaseLanguage.IsEn || Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(t)))) 
                {
                    Pullenti.Ner.NumberToken num2 = Pullenti.Ner.Core.NumberHelper.TryParseRoman(t);
                    if (num2 != null) 
                        return new AddressItemToken(typ, t, num2.EndToken) { Value = num2.Value };
                }
                return null;
            }
            AddressItemToken res0 = new AddressItemToken(typ, t, t1) { Value = num.ToString(), Morph = t.Morph, HouseType = houseTyp, BuildingType = buildTyp };
            t1 = t1.Next;
            if (t1 != null && t1.IsComma) 
                t1 = t1.Next;
            if ((t1 is Pullenti.Ner.TextToken) && (t1 as Pullenti.Ner.TextToken).Term.StartsWith("ОБ")) 
            {
                res0.EndToken = t1;
                t1 = t1.Next;
                if (t1 != null && t1.IsChar('.')) 
                {
                    res0.EndToken = t1;
                    t1 = t1.Next;
                }
                if (res0.Typ == AddressItemType.CorpusOrFlat) 
                    res0.Typ = AddressItemType.Flat;
            }
            if ((t1 != null && t1.IsChar('(') && (t1.Next is Pullenti.Ner.TextToken)) && (t1.Next as Pullenti.Ner.TextToken).Term.StartsWith("ОБ")) 
            {
                res0.EndToken = t1.Next;
                t1 = t1.Next.Next;
                while (t1 != null) 
                {
                    if (t1.IsCharOf(".)")) 
                    {
                        res0.EndToken = t1;
                        t1 = t1.Next;
                    }
                    else 
                        break;
                }
                if (res0.Typ == AddressItemType.CorpusOrFlat) 
                    res0.Typ = AddressItemType.Flat;
            }
            return res0;
        }
        static AddressItemToken _tryAttachVCH(Pullenti.Ner.Token t, AddressItemType ty)
        {
            if (t == null) 
                return null;
            Pullenti.Ner.Token tt = t;
            if ((((tt.IsValue("В", null) || tt.IsValue("B", null))) && tt.Next != null && tt.Next.IsCharOf("./\\")) && (tt.Next.Next is Pullenti.Ner.TextToken) && tt.Next.Next.IsValue("Ч", null)) 
            {
                tt = tt.Next.Next;
                if (tt.Next != null && tt.Next.IsChar('.')) 
                    tt = tt.Next;
                Pullenti.Ner.Token tt2 = Pullenti.Ner.Core.MiscHelper.CheckNumberPrefix(tt.Next);
                if (tt2 != null) 
                    tt = tt2;
                if (tt.Next != null && (tt.Next is Pullenti.Ner.NumberToken) && (tt.WhitespacesAfterCount < 2)) 
                    tt = tt.Next;
                return new AddressItemToken(ty, t, tt) { Value = "В/Ч" };
            }
            if (((tt.IsValue("ВОЙСКОВОЙ", null) || tt.IsValue("ВОИНСКИЙ", null))) && tt.Next != null && tt.Next.IsValue("ЧАСТЬ", null)) 
            {
                tt = tt.Next;
                Pullenti.Ner.Token tt2 = Pullenti.Ner.Core.MiscHelper.CheckNumberPrefix(tt.Next);
                if (tt2 != null) 
                    tt = tt2;
                if (tt.Next != null && (tt.Next is Pullenti.Ner.NumberToken) && (tt.WhitespacesAfterCount < 2)) 
                    tt = tt.Next;
                return new AddressItemToken(ty, t, tt) { Value = "В/Ч" };
            }
            if (ty == AddressItemType.Flat) 
            {
                if (tt.WhitespacesBeforeCount > 1) 
                    return null;
                if (!(tt is Pullenti.Ner.TextToken)) 
                    return null;
                if ((tt as Pullenti.Ner.TextToken).Term.StartsWith("ОБЩ") || (tt as Pullenti.Ner.TextToken).Term.StartsWith("ВЕД")) 
                {
                    if (tt.Next != null && tt.Next.IsChar('.')) 
                        tt = tt.Next;
                    AddressItemToken re = _tryAttachVCH(tt.Next, ty);
                    if (re != null) 
                        return re;
                    return new AddressItemToken(ty, t, tt) { Value = "0" };
                }
                if (tt.Chars.IsAllUpper && tt.LengthChar > 1) 
                {
                    AddressItemToken re = new AddressItemToken(ty, t, tt) { Value = (tt as Pullenti.Ner.TextToken).Term };
                    if ((tt.WhitespacesAfterCount < 2) && (tt.Next is Pullenti.Ner.TextToken) && tt.Next.Chars.IsAllUpper) 
                    {
                        tt = tt.Next;
                        re.EndToken = tt;
                        re.Value += (tt as Pullenti.Ner.TextToken).Term;
                    }
                    return re;
                }
            }
            return null;
        }
        static string _outDoubeKm(Pullenti.Ner.NumberToken n1, Pullenti.Ner.NumberToken n2)
        {
            if (n1.IntValue == null || n2.IntValue == null) 
                return string.Format("{0}+{1}", n1.Value, n2.Value);
            double v = n1.RealValue + ((n2.RealValue / 1000));
            return Pullenti.Ner.Core.NumberHelper.DoubleToString(Math.Round(v, 3));
        }
        static AddressItemToken _tryAttachDetailRange(Pullenti.Ner.Token t)
        {
            Pullenti.Ner.Token t1 = t.Next;
            if (t1 != null && t1.IsChar('.')) 
                t1 = t1.Next;
            if (!(t1 is Pullenti.Ner.NumberToken)) 
                return null;
            if (t1.Next == null || !t1.Next.IsChar('+') || !(t1.Next.Next is Pullenti.Ner.NumberToken)) 
                return null;
            AddressItemToken res = new AddressItemToken(AddressItemType.Detail, t, t1.Next.Next) { DetailType = Pullenti.Ner.Address.AddressDetailType.Range };
            res.Value = string.Format("км{0}", _outDoubeKm(t1 as Pullenti.Ner.NumberToken, t1.Next.Next as Pullenti.Ner.NumberToken));
            t1 = t1.Next.Next.Next;
            if (t1 != null && t1.IsHiphen) 
                t1 = t1.Next;
            if (t1 != null && t1.IsValue("КМ", null)) 
            {
                t1 = t1.Next;
                if (t1 != null && t1.IsChar('.')) 
                    t1 = t1.Next;
            }
            if (!(t1 is Pullenti.Ner.NumberToken)) 
                return null;
            if (t1.Next == null || !t1.Next.IsChar('+') || !(t1.Next.Next is Pullenti.Ner.NumberToken)) 
                return null;
            res.Value = string.Format("{0}-км{1}", res.Value, _outDoubeKm(t1 as Pullenti.Ner.NumberToken, t1.Next.Next as Pullenti.Ner.NumberToken));
            res.EndToken = t1.Next.Next;
            return res;
        }
        static AddressItemToken _tryAttachDetail(Pullenti.Ner.Token t, Pullenti.Ner.Core.TerminToken tok)
        {
            if (t == null || (t is Pullenti.Ner.ReferentToken)) 
                return null;
            if (t.IsValue("КМ", null)) 
            {
                AddressItemToken ran = _tryAttachDetailRange(t);
                if (ran != null) 
                    return ran;
            }
            Pullenti.Ner.Token tt = t;
            if (t.Chars.IsCapitalUpper && !t.Morph.Class.IsPreposition) 
                return null;
            if (tok == null) 
                tok = m_Ontology.TryParse(t, Pullenti.Ner.Core.TerminParseAttr.No);
            if (tok == null && t.Morph.Class.IsPreposition && t.Next != null) 
            {
                tt = t.Next;
                if (tt is Pullenti.Ner.NumberToken) 
                {
                }
                else 
                {
                    if (tt.Chars.IsCapitalUpper && !tt.Morph.Class.IsPreposition) 
                        return null;
                    tok = m_Ontology.TryParse(tt, Pullenti.Ner.Core.TerminParseAttr.No);
                }
            }
            AddressItemToken res = null;
            bool firstNum = false;
            if (tok != null && tok.Termin.Tag2 != null && (tok.Termin.Tag is Pullenti.Ner.Address.AddressDetailType)) 
            {
                res = new AddressItemToken(AddressItemType.Detail, t, tok.EndToken);
                res.DetailType = (Pullenti.Ner.Address.AddressDetailType)tok.Termin.Tag;
                res.DetailParam = "часть";
                return res;
            }
            if (tok == null) 
            {
                Pullenti.Morph.MorphClass mc = tt.GetMorphClassInDictionary();
                if (mc.IsVerb) 
                {
                    AddressItemToken next = _tryAttachDetail(tt.Next, tok);
                    if (next != null) 
                    {
                        next.BeginToken = t;
                        return next;
                    }
                }
                if (tt is Pullenti.Ner.NumberToken) 
                {
                    firstNum = true;
                    Pullenti.Ner.Core.NumberExToken nex = Pullenti.Ner.Core.NumberHelper.TryParseNumberWithPostfix(tt);
                    if (nex != null && ((nex.ExTyp == Pullenti.Ner.Core.NumberExType.Meter || nex.ExTyp == Pullenti.Ner.Core.NumberExType.Kilometer))) 
                    {
                        res = new AddressItemToken(AddressItemType.Detail, t, nex.EndToken);
                        Pullenti.Ner.Core.NumberExType tyy = Pullenti.Ner.Core.NumberExType.Meter;
                        res.DetailMeters = (int)nex.NormalizeValue(ref tyy);
                        Pullenti.Ner.Token tt2 = res.EndToken.Next;
                        if (tt2 != null && tt2.IsHiphen) 
                            tt2 = tt2.Next;
                        Pullenti.Ner.Core.NumberExToken nex2 = Pullenti.Ner.Core.NumberHelper.TryParseNumberWithPostfix(tt2);
                        if (nex2 != null && nex2.ExTyp == Pullenti.Ner.Core.NumberExType.Meter && nex2.IntValue != null) 
                        {
                            res.EndToken = nex2.EndToken;
                            res.DetailMeters += nex2.IntValue.Value;
                        }
                    }
                }
                if (res == null) 
                    return null;
            }
            else 
            {
                if (!(tok.Termin.Tag is Pullenti.Ner.Address.AddressDetailType)) 
                    return null;
                res = new AddressItemToken(AddressItemType.Detail, t, tok.EndToken) { DetailType = (Pullenti.Ner.Address.AddressDetailType)tok.Termin.Tag };
            }
            for (tt = res.EndToken.Next; tt != null; tt = tt.Next) 
            {
                if (tt is Pullenti.Ner.ReferentToken) 
                    break;
                if (!tt.Morph.Class.IsPreposition) 
                {
                    if (tt.Chars.IsCapitalUpper || tt.Chars.IsAllUpper) 
                        break;
                }
                tok = m_Ontology.TryParse(tt, Pullenti.Ner.Core.TerminParseAttr.No);
                if (tok != null && (tok.Termin.Tag is Pullenti.Ner.Address.AddressDetailType)) 
                {
                    Pullenti.Ner.Address.AddressDetailType ty = (Pullenti.Ner.Address.AddressDetailType)tok.Termin.Tag;
                    if (ty != Pullenti.Ner.Address.AddressDetailType.Undefined) 
                    {
                        if (ty == Pullenti.Ner.Address.AddressDetailType.Near && res.DetailType != Pullenti.Ner.Address.AddressDetailType.Undefined && res.DetailType != ty) 
                        {
                        }
                        else 
                            res.DetailType = ty;
                    }
                    res.EndToken = (tt = tok.EndToken);
                    continue;
                }
                Pullenti.Ner.Core.NounPhraseToken npt = Pullenti.Ner.Core.NounPhraseHelper.TryParse(tt, Pullenti.Ner.Core.NounPhraseParseAttr.No, 0, null);
                if (npt != null) 
                    tt = npt.EndToken;
                if (((tt.IsValue("ОРИЕНТИР", null) || tt.IsValue("НАПРАВЛЕНИЕ", null) || tt.IsValue("ОТ", null)) || tt.IsValue("В", null) || tt.IsValue("УСАДЬБА", null)) || tt.IsValue("ДВОР", null)) 
                {
                    res.EndToken = tt;
                    continue;
                }
                if (tt.IsValue("ЗДАНИЕ", null) || tt.IsValue("СТРОЕНИЕ", null) || tt.IsValue("ДОМ", null)) 
                {
                    AddressItemToken ait = TryParsePureItem(tt, null, null);
                    if (ait != null && ait.Value != null) 
                        break;
                    if (Pullenti.Ner.Geo.Internal.OrgItemToken.TryParse(tt.Next, null) != null) 
                        break;
                    res.EndToken = tt;
                    continue;
                }
                if (npt != null && npt.InternalNoun != null) 
                {
                    res.EndToken = (tt = npt.EndToken);
                    continue;
                }
                if (((tt.IsValue("ГРАНИЦА", null) || tt.IsValue("ПРЕДЕЛ", null))) && tt.Next != null) 
                {
                    if (tt.Next.IsValue("УЧАСТОК", null)) 
                    {
                        tt = tt.Next;
                        res.EndToken = tt;
                        continue;
                    }
                }
                Pullenti.Morph.MorphClass mc = tt.GetMorphClassInDictionary();
                if (mc.IsVerb && !mc.IsNoun) 
                    continue;
                if ((tt.IsComma || mc.IsPreposition || tt.IsHiphen) || tt.IsChar(':')) 
                    continue;
                if ((tt is Pullenti.Ner.NumberToken) && tt.Next != null) 
                {
                    Pullenti.Ner.Core.NumberExToken nex = Pullenti.Ner.Core.NumberHelper.TryParseNumberWithPostfix(tt);
                    if (nex != null && ((nex.ExTyp == Pullenti.Ner.Core.NumberExType.Meter || nex.ExTyp == Pullenti.Ner.Core.NumberExType.Kilometer))) 
                    {
                        res.EndToken = (tt = nex.EndToken);
                        Pullenti.Ner.Core.NumberExType tyy = Pullenti.Ner.Core.NumberExType.Meter;
                        res.DetailMeters = (int)nex.NormalizeValue(ref tyy);
                        continue;
                    }
                }
                break;
            }
            if (firstNum && res.DetailType == Pullenti.Ner.Address.AddressDetailType.Undefined) 
                return null;
            if (res != null && res.EndToken.Next != null && res.EndToken.Next.Morph.Class.IsPreposition) 
            {
                if (res.EndToken.WhitespacesAfterCount == 1 && res.EndToken.Next.WhitespacesAfterCount == 1) 
                    res.EndToken = res.EndToken.Next;
            }
            if (res != null && res.EndToken.Next != null) 
            {
                if (res.EndToken.Next.IsHiphen || res.EndToken.Next.IsChar(':')) 
                    res.EndToken = res.EndToken.Next;
            }
            return res;
        }
        public static bool CheckStreetAfter(Pullenti.Ner.Token t, bool checkThisAndNotNext = false)
        {
            int cou = 0;
            for (; t != null && (cou < 4); t = t.Next,cou++) 
            {
                if (t.IsCharOf(",.") || t.IsHiphen || t.Morph.Class.IsPreposition) 
                {
                }
                else 
                    break;
            }
            if (t == null) 
                return false;
            AddressItemToken ait = TryParse(t, false, null, null);
            if (ait == null || ait.Typ != AddressItemType.Street) 
                return false;
            if (ait.RefToken != null) 
            {
                if (!ait.RefTokenIsGsk) 
                    return false;
                Pullenti.Ner.Geo.Internal.OrgItemToken oo = ait.RefToken as Pullenti.Ner.Geo.Internal.OrgItemToken;
                if (oo != null && oo.IsDoubt) 
                    return false;
            }
            if (!checkThisAndNotNext) 
                return true;
            if (t.Next == null || ait.EndChar <= t.EndChar) 
                return true;
            AddressItemToken ait2 = TryParse(t.Next, false, null, null);
            if (ait2 == null) 
                return true;
            List<AddressItemToken> aits1 = TryParseList(t, 20);
            List<AddressItemToken> aits2 = TryParseList(t.Next, 20);
            if (aits1 != null && aits2 != null) 
            {
                if (aits2[aits2.Count - 1].EndChar > aits1[aits1.Count - 1].EndChar) 
                    return false;
            }
            return true;
        }
        public static bool CheckHouseAfter(Pullenti.Ner.Token t, bool leek = false, bool pureHouse = false)
        {
            if (t == null) 
                return false;
            int cou = 0;
            for (; t != null && (cou < 4); t = t.Next,cou++) 
            {
                if (t.IsCharOf(",.") || t.Morph.Class.IsPreposition) 
                {
                }
                else 
                    break;
            }
            if (t == null) 
                return false;
            if (t.IsNewlineBefore) 
                return false;
            AddressItemToken ait = TryParsePureItem(t, null, null);
            if (ait != null) 
            {
                if (ait.Value == null || ait.Value == "0") 
                    return false;
                if (pureHouse) 
                    return ait.Typ == AddressItemType.House || ait.Typ == AddressItemType.Plot;
                if (((ait.Typ == AddressItemType.House || ait.Typ == AddressItemType.Floor || ait.Typ == AddressItemType.Office) || ait.Typ == AddressItemType.Flat || ait.Typ == AddressItemType.Plot) || ait.Typ == AddressItemType.Room || ait.Typ == AddressItemType.Corpus) 
                {
                    if (((t is Pullenti.Ner.TextToken) && t.Chars.IsAllUpper && t.Next != null) && t.Next.IsHiphen && (t.Next.Next is Pullenti.Ner.NumberToken)) 
                        return false;
                    if ((t is Pullenti.Ner.TextToken) && t.Next == ait.EndToken && t.Next.IsHiphen) 
                        return false;
                    return true;
                }
                if (leek) 
                {
                    if (ait.Typ == AddressItemType.Number) 
                        return true;
                }
                if (ait.Typ == AddressItemType.Number) 
                {
                    Pullenti.Ner.Token t1 = t.Next;
                    while (t1 != null && t1.IsCharOf(".,")) 
                    {
                        t1 = t1.Next;
                    }
                    ait = TryParsePureItem(t1, null, null);
                    if (ait != null && ((((ait.Typ == AddressItemType.Building || ait.Typ == AddressItemType.Corpus || ait.Typ == AddressItemType.Flat) || ait.Typ == AddressItemType.Floor || ait.Typ == AddressItemType.Office) || ait.Typ == AddressItemType.Room))) 
                        return true;
                }
            }
            return false;
        }
        public static bool CheckKmAfter(Pullenti.Ner.Token t)
        {
            int cou = 0;
            for (; t != null && (cou < 4); t = t.Next,cou++) 
            {
                if (t.IsCharOf(",.") || t.Morph.Class.IsPreposition) 
                {
                }
                else 
                    break;
            }
            if (t == null) 
                return false;
            AddressItemToken km = TryParsePureItem(t, null, null);
            if (km != null && km.Typ == AddressItemType.Kilometer) 
                return true;
            if (!(t is Pullenti.Ner.NumberToken) || t.Next == null) 
                return false;
            if (t.Next.IsValue("КИЛОМЕТР", null) || t.Next.IsValue("МЕТР", null) || t.Next.IsValue("КМ", null)) 
                return true;
            return false;
        }
        public static bool CheckKmBefore(Pullenti.Ner.Token t)
        {
            int cou = 0;
            for (; t != null && (cou < 4); t = t.Previous,cou++) 
            {
                if (t.IsCharOf(",.")) 
                {
                }
                else if (t.IsValue("КМ", null) || t.IsValue("КИЛОМЕТР", null) || t.IsValue("МЕТР", null)) 
                    return true;
            }
            return false;
        }
        public static char CorrectChar(char v)
        {
            if (v == 'A' || v == 'А') 
                return 'А';
            if (v == 'Б' || v == 'Г') 
                return v;
            if (v == 'B' || v == 'В') 
                return 'В';
            if (v == 'C' || v == 'С') 
                return 'С';
            if (v == 'D' || v == 'Д') 
                return 'Д';
            if (v == 'E' || v == 'Е') 
                return 'Е';
            if (v == 'H' || v == 'Н') 
                return 'Н';
            if (v == 'K' || v == 'К') 
                return 'К';
            return (char)0;
        }
        static string CorrectCharToken(Pullenti.Ner.Token t)
        {
            Pullenti.Ner.TextToken tt = t as Pullenti.Ner.TextToken;
            if (tt == null) 
                return null;
            string v = tt.Term;
            if (v.Length == 1) 
            {
                char corr = CorrectChar(v[0]);
                if (corr != ((char)0)) 
                    return string.Format("{0}", corr);
                if (t.Chars.IsCyrillicLetter) 
                    return v;
            }
            if (v.Length == 2) 
            {
                if (t.Chars.IsCyrillicLetter) 
                    return v;
                char corr = CorrectChar(v[0]);
                char corr2 = CorrectChar(v[1]);
                if (corr != ((char)0) && corr2 != ((char)0)) 
                    return string.Format("{0}{1}", corr, corr2);
            }
            return null;
        }
        static string _corrNumber(string num)
        {
            if (string.IsNullOrEmpty(num)) 
                return null;
            if (num[0] != 'З') 
                return null;
            string res = "3";
            int i;
            for (i = 1; i < num.Length; i++) 
            {
                if (num[i] == 'З') 
                    res += "3";
                else if (num[i] == 'О') 
                    res += "0";
                else 
                    break;
            }
            if (i == num.Length) 
                return res;
            if ((i + 1) < num.Length) 
                return null;
            if (num[i] == 'А' || num[i] == 'Б' || num[i] == 'В') 
                return string.Format("{0}{1}", res, num[i]);
            return null;
        }
        public static Pullenti.Ner.ReferentToken CreateAddress(string txt)
        {
            Pullenti.Ner.AnalysisResult ar = null;
            try 
            {
                ar = Pullenti.Ner.ProcessorService.EmptyProcessor.Process(new Pullenti.Ner.SourceOfAnalysis(txt) { UserParams = "ADDRESS" }, null, null);
            }
            catch(Exception ex) 
            {
                return null;
            }
            if (ar == null) 
                return null;
            PrepareAllData(ar.FirstToken);
            List<AddressItemToken> li = new List<AddressItemToken>();
            for (Pullenti.Ner.Token t = ar.FirstToken; t != null; t = t.Next) 
            {
                if (t.IsCharOf(",.")) 
                    continue;
                AddressItemToken ait = TryParsePureItem(t, (li.Count > 0 ? li[li.Count - 1] : null), null);
                if (ait == null) 
                    break;
                li.Add(ait);
                t = ait.EndToken;
            }
            if (li == null || li.Count == 0) 
                return null;
            Pullenti.Ner.Token rt = AddressDefineHelper.TryDefine(li, ar.FirstToken, null, true);
            return rt as Pullenti.Ner.ReferentToken;
        }
    }
}