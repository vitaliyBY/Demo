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
    public static class StreetDefineHelper
    {
        public static bool CheckStreetAfter(Pullenti.Ner.Token t)
        {
            if (t == null) 
                return false;
            while (t != null && ((t.IsCharOf(",;") || t.Morph.Class.IsPreposition))) 
            {
                t = t.Next;
            }
            List<StreetItemToken> li = StreetItemToken.TryParseList(t, 10, null);
            if (li == null) 
                return false;
            AddressItemToken rt = TryParseStreet(li, false, false, false, null);
            if (rt != null && rt.BeginToken == t) 
                return true;
            else 
                return false;
        }
        public static Pullenti.Ner.ReferentToken TryParseExtStreet(List<StreetItemToken> sli)
        {
            AddressItemToken a = TryParseStreet(sli, true, false, false, null);
            if (a != null) 
                return new Pullenti.Ner.ReferentToken(a.Referent, a.BeginToken, a.EndToken);
            return null;
        }
        internal static AddressItemToken TryParseStreet(List<StreetItemToken> sli, bool extOntoRegim = false, bool forMetro = false, bool streetBefore = false, Pullenti.Ner.Address.StreetReferent crossStreet = null)
        {
            if (sli == null || sli.Count == 0) 
                return null;
            if ((sli.Count == 2 && sli[0].Typ == StreetItemType.Number && sli[1].Typ == StreetItemType.Noun) && sli[1].IsAbridge) 
            {
                if (Pullenti.Ner.Geo.Internal.MiscLocationHelper.CheckGeoObjectBefore(sli[0].BeginToken, false)) 
                {
                }
                else if (StreetItemToken._isRegion(sli[1].Termin.CanonicText) && Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(sli[1])) 
                {
                }
                else 
                    return null;
            }
            if ((sli.Count == 2 && sli[0].Typ == StreetItemType.Noun && sli[0].NounIsDoubtCoef > 1) && sli[0].BeginToken.IsValue("КВ", null) && sli[1].Typ == StreetItemType.Number) 
            {
                AddressItemToken at = AddressItemToken.TryParsePureItem(sli[0].BeginToken, null, null);
                if (at != null && at.Value != null) 
                {
                    Pullenti.Ner.Token ttt = at.EndToken.Next;
                    if (ttt != null && ttt.IsComma) 
                        ttt = ttt.Next;
                    AddressItemToken next = AddressItemToken.TryParsePureItem(ttt, null, null);
                    if (next != null && next.Typ == AddressItemType.Plot) 
                    {
                    }
                    else 
                        return null;
                }
            }
            if (((sli.Count == 4 && sli[0].Typ == StreetItemType.Noun && sli[1].Typ == StreetItemType.Number) && sli[2].Typ == StreetItemType.Noun && sli[0].Termin == sli[2].Termin) && ((sli[3].Typ == StreetItemType.Name || sli[3].Typ == StreetItemType.StdName || sli[3].Typ == StreetItemType.StdAdjective))) 
                sli.RemoveAt(2);
            if (sli.Count == 2 && sli[0].Typ == StreetItemType.Noun && sli[1].Typ == StreetItemType.Fix) 
                return _tryParseFix(sli);
            if ((sli.Count == 3 && sli[1].Typ == StreetItemType.Fix && sli[2].Typ == StreetItemType.Noun) && (((sli[0].Typ == StreetItemType.Number || sli[0].Typ == StreetItemType.Name || sli[0].Typ == StreetItemType.StdAdjective) || sli[0].Typ == StreetItemType.StdName))) 
            {
                List<StreetItemToken> tmp = new List<StreetItemToken>();
                tmp.Add(sli[0]);
                tmp.Add(sli[2]);
                AddressItemToken res1 = TryParseStreet(tmp, extOntoRegim, forMetro, streetBefore, crossStreet);
                if (res1 == null) 
                    return null;
                tmp.Clear();
                tmp.Add(sli[1]);
                AddressItemToken res2 = TryParseStreet(tmp, extOntoRegim, forMetro, streetBefore, crossStreet);
                if (res2 != null) 
                    res1.OrtoTerr = res2;
                return res1;
            }
            int i;
            int j;
            bool notDoubt = false;
            bool isTerr = false;
            for (i = 0; i < sli.Count; i++) 
            {
                if (i == 0 && sli[i].Typ == StreetItemType.Fix && ((sli.Count == 1 || sli[1].Typ != StreetItemType.Noun || sli[0].Org != null))) 
                    return _tryParseFix(sli);
                else if (sli[i].Typ == StreetItemType.Noun) 
                {
                    if (sli.Count == 1 && sli[0].NounCanBeName && Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(sli[0])) 
                        continue;
                    if (sli[i].Termin.CanonicText == "МЕТРО") 
                    {
                        if ((i + 1) < sli.Count) 
                        {
                            List<StreetItemToken> sli1 = new List<StreetItemToken>();
                            for (int ii = i + 1; ii < sli.Count; ii++) 
                            {
                                sli1.Add(sli[ii]);
                            }
                            AddressItemToken str1 = TryParseStreet(sli1, extOntoRegim, true, false, null);
                            if (str1 != null) 
                            {
                                str1.BeginToken = sli[i].BeginToken;
                                str1.IsDoubt = sli[i].IsAbridge;
                                if (sli[i + 1].IsInBrackets) 
                                    str1.IsDoubt = false;
                                return str1;
                            }
                        }
                        else if (i == 1 && sli[0].Typ == StreetItemType.Name) 
                        {
                            forMetro = true;
                            break;
                        }
                        if (i == 0 && sli.Count > 0) 
                        {
                            forMetro = true;
                            break;
                        }
                        return null;
                    }
                    if (i == 0 && (i + 1) >= sli.Count && ((sli[i].Termin.CanonicText == "ВОЕННЫЙ ГОРОДОК" || sli[i].Termin.CanonicText == "ПРОМЗОНА"))) 
                    {
                        Pullenti.Ner.Address.StreetReferent stri0 = new Pullenti.Ner.Address.StreetReferent();
                        stri0.AddTyp("микрорайон");
                        stri0.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NAME, sli[i].Termin.CanonicText, false, 0);
                        return new AddressItemToken(AddressItemType.Street, sli[0].BeginToken, sli[0].EndToken) { Referent = stri0, IsDoubt = true };
                    }
                    if (i == 0 && (i + 1) >= sli.Count && sli[i].Termin.CanonicText == "МИКРОРАЙОН") 
                    {
                        Pullenti.Ner.Address.StreetReferent stri0 = new Pullenti.Ner.Address.StreetReferent();
                        stri0.Kind = Pullenti.Ner.Address.StreetKind.Area;
                        stri0.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_TYPE, sli[i].Termin.CanonicText.ToLower(), false, 0);
                        return new AddressItemToken(AddressItemType.Street, sli[0].BeginToken, sli[0].EndToken) { Referent = stri0, IsDoubt = true };
                    }
                    if (sli[i].Termin.CanonicText == "ПЛОЩАДЬ" || sli[i].Termin.CanonicText == "ПЛОЩА") 
                    {
                        Pullenti.Ner.Token tt = sli[i].EndToken.Next;
                        if (tt != null && ((tt.IsHiphen || tt.IsChar(':')))) 
                            tt = tt.Next;
                        Pullenti.Ner.Core.NumberExToken nex = Pullenti.Ner.Core.NumberHelper.TryParseNumberWithPostfix(tt);
                        if (nex != null) 
                            return null;
                        if (i > 0 && sli[i - 1].Value == "ПРОЕКТИРУЕМЫЙ") 
                            return null;
                    }
                    break;
                }
            }
            if (i >= sli.Count) 
                return TryDetectNonNoun(sli, extOntoRegim, forMetro, streetBefore, crossStreet);
            StreetItemToken name = null;
            string number = null;
            string age = null;
            StreetItemToken adj = null;
            StreetItemToken noun = sli[i];
            StreetItemToken altNoun = null;
            bool isMicroRaion = StreetItemToken._isRegion(noun.Termin.CanonicText);
            bool isProezd = false;
            int before = 0;
            int after = 0;
            for (j = 0; j < i; j++) 
            {
                if (((sli[j].Typ == StreetItemType.Name || sli[j].Typ == StreetItemType.StdName || sli[j].Typ == StreetItemType.Fix) || sli[j].Typ == StreetItemType.StdAdjective || sli[j].Typ == StreetItemType.StdPartOfName) || sli[j].Typ == StreetItemType.Age) 
                    before++;
                else if (sli[j].Typ == StreetItemType.Number) 
                {
                    if (sli[j].IsNewlineAfter) 
                        return null;
                    if (sli[j].NumberType != Pullenti.Ner.NumberSpellingType.Undefined && sli[j].BeginToken.Morph.Class.IsAdjective) 
                        before++;
                    else if (isMicroRaion || notDoubt) 
                        before++;
                    else if (sli[i].NumberHasPrefix || sli[i].IsNumberKm) 
                        before++;
                    else if (Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(sli[i])) 
                        before++;
                }
                else 
                    before++;
            }
            for (j = i + 1; j < sli.Count; j++) 
            {
                if (before > 0 && sli[j].IsNewlineBefore) 
                    break;
                else if (((sli[j].Typ == StreetItemType.Name || sli[j].Typ == StreetItemType.StdName || sli[j].Typ == StreetItemType.Fix) || sli[j].Typ == StreetItemType.StdAdjective || sli[j].Typ == StreetItemType.StdPartOfName) || sli[j].Typ == StreetItemType.Age) 
                    after++;
                else if (sli[j].Typ == StreetItemType.Number) 
                {
                    if (sli[j].NumberType != Pullenti.Ner.NumberSpellingType.Undefined && sli[j].BeginToken.Morph.Class.IsAdjective) 
                        after++;
                    else if (isMicroRaion || notDoubt) 
                        after++;
                    else if (sli[j].NumberHasPrefix || sli[j].IsNumberKm) 
                        after++;
                    else if (extOntoRegim) 
                        after++;
                    else if (sli.Count == 2 && sli[0].Typ == StreetItemType.Noun && j == 1) 
                        after++;
                    else if ((sli.Count > 2 && sli[0].Typ == StreetItemType.Noun && sli[1].Typ == StreetItemType.Noun) && j == 2) 
                        after++;
                    else if ((sli.Count == 3 && sli[0].Typ == StreetItemType.Noun && sli[2].Typ == StreetItemType.Noun) && j == 1) 
                        after++;
                    else if (((j + 1) < sli.Count) && sli[j + 1].Typ == StreetItemType.Noun) 
                        after++;
                }
                else if (sli[j].Typ == StreetItemType.Noun) 
                {
                    bool isReg = StreetItemToken._isRegion(sli[j].Termin.CanonicText);
                    if (((j == (i + 1) || j == (sli.Count - 1))) && altNoun == null && !isReg) 
                        altNoun = sli[j];
                    else if (altNoun == null && ((sli[i].Termin.CanonicText == "ПРОЕЗД" || ((sli[j].Termin.CanonicText == "ПРОЕЗД" || sli[j].Termin.CanonicText == "НАБЕРЕЖНАЯ")))) && !isMicroRaion) 
                    {
                        altNoun = sli[j];
                        isProezd = true;
                    }
                    else if (j == 1 && sli.Count == 3 && sli[2].Typ == StreetItemType.Number) 
                        altNoun = sli[j];
                    else if (((j + 1) < sli.Count) && !sli[j].IsNewlineAfter) 
                        break;
                    else 
                        altNoun = sli[j];
                }
                else 
                    after++;
            }
            List<StreetItemToken> rli = new List<StreetItemToken>();
            int n0 = 0;
            int n1 = 0;
            if (before > after) 
            {
                if (noun.Termin.CanonicText == "МЕТРО") 
                    return null;
                if (noun.Termin.CanonicText == "КВАРТАЛ" && !extOntoRegim && !streetBefore) 
                {
                    if (sli[0].Typ == StreetItemType.Number && sli.Count == 2) 
                    {
                        if (!AddressItemToken.CheckHouseAfter(sli[1].EndToken.Next, false, false)) 
                        {
                            if (!Pullenti.Ner.Geo.Internal.MiscLocationHelper.CheckGeoObjectBefore(sli[0].BeginToken, false)) 
                                return null;
                            if (sli[0].BeginToken.Previous != null && sli[0].BeginToken.Previous.GetMorphClassInDictionary().IsPreposition) 
                                return null;
                        }
                    }
                }
                Pullenti.Ner.Token tt = sli[0].BeginToken;
                if (tt == sli[0].EndToken && noun.BeginToken == sli[0].EndToken.Next && !Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(sli[0])) 
                {
                    if (!tt.Morph.Class.IsAdjective && !(tt is Pullenti.Ner.NumberToken)) 
                    {
                        if ((sli[0].IsNewlineBefore || !Pullenti.Ner.Geo.Internal.MiscLocationHelper.CheckGeoObjectBefore(sli[0].BeginToken, false) || noun.Morph.Case.IsGenitive) || noun.Morph.Case.IsInstrumental) 
                        {
                            bool ok = false;
                            if (AddressItemToken.CheckHouseAfter(noun.EndToken.Next, false, true)) 
                                ok = true;
                            else if (noun.EndToken.Next == null) 
                                ok = true;
                            else if (noun.IsNewlineAfter && Pullenti.Ner.Geo.Internal.MiscLocationHelper.CheckGeoObjectBefore(sli[0].BeginToken, false)) 
                                ok = true;
                            if (!ok) 
                            {
                                if ((noun.Chars.IsLatinLetter && noun.Chars.IsCapitalUpper && sli[0].Chars.IsLatinLetter) && sli[0].Chars.IsCapitalUpper) 
                                    ok = true;
                            }
                            if (!ok) 
                                return null;
                        }
                    }
                }
                n0 = 0;
                n1 = i - 1;
            }
            else if (i == 1 && sli[0].Typ == StreetItemType.Number) 
            {
                if (!sli[0].IsWhitespaceAfter) 
                    return null;
                number = sli[0].Value;
                if (sli[0].IsNumberKm) 
                    number += "км";
                n0 = i + 1;
                n1 = sli.Count - 1;
                rli.Add(sli[0]);
                rli.Add(sli[i]);
            }
            else if (after > before) 
            {
                n0 = i + 1;
                n1 = sli.Count - 1;
                rli.Add(sli[i]);
                if (altNoun != null && altNoun == sli[i + 1]) 
                {
                    rli.Add(sli[i + 1]);
                    n0++;
                }
            }
            else if (after == 0) 
            {
                if (altNoun == null || sli.Count != 2) 
                    return null;
                n0 = 1;
                n1 = 0;
            }
            else if ((sli.Count > 2 && ((sli[0].Typ == StreetItemType.Name || sli[0].Typ == StreetItemType.StdAdjective || sli[0].Typ == StreetItemType.StdName)) && sli[1].Typ == StreetItemType.Noun) && sli[2].Typ == StreetItemType.Number) 
            {
                n0 = 0;
                n1 = 0;
                bool num = false;
                Pullenti.Ner.Token tt2 = sli[2].EndToken.Next;
                if (sli[2].IsNumberKm) 
                    num = true;
                else if (sli[0].BeginToken.Previous != null && sli[0].BeginToken.Previous.IsValue("КИЛОМЕТР", null)) 
                {
                    sli[2].IsNumberKm = true;
                    num = true;
                }
                else if (sli[2].BeginToken.Previous.IsComma) 
                {
                }
                else if (sli[2].BeginToken != sli[2].EndToken) 
                    num = true;
                else if (AddressItemToken.CheckHouseAfter(sli[2].EndToken.Next, false, true)) 
                    num = true;
                else if (sli[2].Morph.Class.IsAdjective && (sli[2].WhitespacesBeforeCount < 2)) 
                {
                    if (sli[2].EndToken.Next == null || sli[2].EndToken.IsComma || sli[2].IsNewlineAfter) 
                        num = true;
                }
                if (num) 
                {
                    number = sli[2].Value;
                    if (sli[2].IsNumberKm) 
                        number += "км";
                    rli.Add(sli[2]);
                }
                else 
                    sli.RemoveRange(2, sli.Count - 2);
            }
            else if ((sli.Count > 2 && sli[0].Typ == StreetItemType.StdAdjective && sli[1].Typ == StreetItemType.Noun) && sli[2].Typ == StreetItemType.StdName) 
            {
                n0 = 0;
                n1 = -1;
                rli.Add(sli[0]);
                rli.Add(sli[2]);
                adj = sli[0];
                name = sli[2];
            }
            else 
                return null;
            string secNumber = null;
            for (j = n0; j <= n1; j++) 
            {
                if (sli[j].Typ == StreetItemType.Number) 
                {
                    if (sli[j].IsNewlineBefore && j > 0) 
                        break;
                    if (number != null) 
                    {
                        if (name != null && name.Typ == StreetItemType.StdName) 
                        {
                            secNumber = sli[j].Value;
                            if (sli[j].IsNumberKm) 
                                secNumber += "км";
                            rli.Add(sli[j]);
                            continue;
                        }
                        if (((j + 1) < sli.Count) && sli[j + 1].Typ == StreetItemType.StdName) 
                        {
                            secNumber = sli[j].Value;
                            if (sli[j].IsNumberKm) 
                                secNumber += "км";
                            rli.Add(sli[j]);
                            continue;
                        }
                        break;
                    }
                    if (sli[j].NumberType == Pullenti.Ner.NumberSpellingType.Digit && !sli[j].BeginToken.Morph.Class.IsAdjective && !sli[j].EndToken.Morph.Class.IsAdjective) 
                    {
                        if (sli[j].WhitespacesBeforeCount > 2 && j > 0) 
                            break;
                        int nval = 0;
                        int.TryParse(sli[j].Value, out nval);
                        if (nval > 20) 
                        {
                            if (j > n0) 
                            {
                                if (((j + 1) < sli.Count) && ((sli[j + 1].Typ == StreetItemType.Noun || sli[j + 1].Value == "ГОДА"))) 
                                {
                                }
                                else if ((j + 1) == sli.Count && sli[j].BeginToken.Previous.IsHiphen) 
                                {
                                    Pullenti.Ner.Token tt = sli[j].EndToken.Next;
                                    if (tt != null && tt.IsComma) 
                                        tt = tt.Next;
                                    AddressItemToken ait = AddressItemToken.TryParsePureItem(tt, null, null);
                                    if (ait != null && ait.Typ == AddressItemType.House) 
                                    {
                                    }
                                    else if (Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamGarAddress(sli[j])) 
                                    {
                                    }
                                    else 
                                        break;
                                }
                                else 
                                    break;
                            }
                        }
                        if (j == n0 && n0 > 0) 
                        {
                        }
                        else if (j == n0 && n0 == 0 && sli[j].WhitespacesAfterCount == 1) 
                        {
                        }
                        else if (sli[j].NumberHasPrefix || sli[j].IsNumberKm) 
                        {
                        }
                        else if (j == n1 && ((n1 + 1) < sli.Count) && sli[n1 + 1].Typ == StreetItemType.Noun) 
                        {
                        }
                        else if (!sli[j].IsWhitespaceBefore) 
                        {
                        }
                        else 
                            break;
                    }
                    number = sli[j].Value;
                    if (sli[j].IsNumberKm) 
                        number += "км";
                    rli.Add(sli[j]);
                }
                else if (sli[j].Typ == StreetItemType.Age) 
                {
                    if (age != null) 
                        break;
                    age = sli[j].Value;
                    rli.Add(sli[j]);
                }
                else if (sli[j].Typ == StreetItemType.StdAdjective) 
                {
                    if (adj != null) 
                    {
                        if (j == (sli.Count - 1) && !sli[j].IsAbridge && name == null) 
                        {
                            name = sli[j];
                            rli.Add(sli[j]);
                            continue;
                        }
                        else 
                            return null;
                    }
                    adj = sli[j];
                    rli.Add(sli[j]);
                }
                else if (sli[j].Typ == StreetItemType.Name || sli[j].Typ == StreetItemType.StdName || sli[j].Typ == StreetItemType.Fix) 
                {
                    if (name != null) 
                    {
                        if (j > 1 && sli[j - 2].Typ == StreetItemType.Noun) 
                        {
                            if (name.NounCanBeName && sli[j - 2].Termin.CanonicText == "УЛИЦА" && j == (sli.Count - 1)) 
                                noun = name;
                            else if ((isMicroRaion && sli[j - 1].Termin != null && StreetItemToken._isRegion(sli[j - 1].Termin.CanonicText)) && j == (sli.Count - 1)) 
                                noun = name;
                            else 
                                break;
                        }
                        else if (i < j) 
                            break;
                        else 
                            return null;
                    }
                    name = sli[j];
                    rli.Add(sli[j]);
                }
                else if (sli[j].Typ == StreetItemType.StdPartOfName && j == n1) 
                {
                    if (name != null) 
                        break;
                    name = sli[j];
                    rli.Add(sli[j]);
                }
                else if (sli[j].Typ == StreetItemType.Noun) 
                {
                    if ((sli[0] == noun && ((noun.Termin.CanonicText == "УЛИЦА" || noun.Termin.CanonicText == "ВУЛИЦЯ")) && j > 0) && name == null) 
                    {
                        altNoun = noun;
                        noun = sli[j];
                        rli.Add(sli[j]);
                    }
                    else if (sli[j] == altNoun) 
                        rli.Add(sli[j]);
                    else 
                        break;
                }
            }
            if (((n1 < i) && number == null && ((i + 1) < sli.Count)) && sli[i + 1].Typ == StreetItemType.Number && sli[i + 1].NumberHasPrefix) 
            {
                number = sli[i + 1].Value;
                rli.Add(sli[i + 1]);
            }
            else if ((((i < n0) && ((name != null || adj != null)) && (j < sli.Count)) && sli[j].Typ == StreetItemType.Noun && ((noun.Termin.CanonicText == "УЛИЦА" || noun.Termin.CanonicText == "ВУЛИЦЯ"))) && (((sli[j].Termin.CanonicText == "ПЛОЩАДЬ" || sli[j].Termin.CanonicText == "БУЛЬВАР" || sli[j].Termin.CanonicText == "ПЛОЩА") || sli[j].Termin.CanonicText == "МАЙДАН" || (j + 1) == sli.Count))) 
            {
                altNoun = noun;
                noun = sli[j];
                rli.Add(sli[j]);
            }
            if ((altNoun != null && name == null && number == null) && age == null && adj == null) 
            {
                if (noun.Termin.CanonicText == "УЛИЦА") 
                {
                    name = altNoun;
                    altNoun = null;
                }
                else if (altNoun.Termin.CanonicText == "УЛИЦА") 
                {
                    name = noun;
                    noun = altNoun;
                    altNoun = null;
                }
                else if (altNoun.NounCanBeName) 
                {
                    name = altNoun;
                    altNoun = null;
                }
                if (name != null) 
                    rli.Add(name);
            }
            if (altNoun != null && altNoun.Termin.CanonicText == "УЛИЦА" && StreetItemToken._isRegion(noun.Termin.CanonicText)) 
            {
                altNoun = null;
                isMicroRaion = true;
            }
            if (name == null) 
            {
                if (number == null && age == null && adj == null) 
                    return null;
                if (noun.IsAbridge && !Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(noun)) 
                {
                    if (isMicroRaion || notDoubt) 
                    {
                    }
                    else if (noun.Termin != null && ((noun.Termin.CanonicText == "ПРОЕЗД" || noun.Termin.CanonicText == "ПРОЇЗД"))) 
                    {
                    }
                    else if (adj == null || adj.IsAbridge) 
                        return null;
                }
                if (adj != null && adj.IsAbridge) 
                {
                    if (!noun.IsAbridge && Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(adj)) 
                    {
                    }
                    else if (altNoun != null) 
                    {
                    }
                    else 
                        return null;
                }
            }
            if (!rli.Contains(noun)) 
                rli.Add(noun);
            if (altNoun != null && !rli.Contains(altNoun)) 
                rli.Add(altNoun);
            Pullenti.Ner.Address.StreetReferent street = new Pullenti.Ner.Address.StreetReferent();
            if (!forMetro) 
            {
                street.AddTyp(noun.Termin.CanonicText.ToLower());
                if (noun.AltTermin != null) 
                {
                    if (noun.AltTermin.CanonicText == "ПРОСПЕКТ" && number != null) 
                    {
                    }
                    else 
                        street.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_TYPE, noun.AltTermin.CanonicText.ToLower(), false, 0);
                }
                if (altNoun != null) 
                {
                    street.AddTyp(altNoun.Termin.CanonicText.ToLower());
                    if (altNoun.AltTermin != null) 
                        street.AddTyp(altNoun.AltTermin.CanonicText.ToLower());
                }
            }
            else 
                street.AddTyp("метро");
            street.Tag = noun;
            AddressItemToken res = new AddressItemToken(AddressItemType.Street, rli[0].BeginToken, rli[0].EndToken) { Referent = street };
            if (noun.Termin.CanonicText == "ЛИНИЯ") 
            {
                if (number == null) 
                {
                    if (Pullenti.Ner.Geo.Internal.MiscLocationHelper.CheckGeoObjectBefore(sli[0].BeginToken, false)) 
                    {
                    }
                    else 
                        return null;
                }
                res.IsDoubt = true;
            }
            else if (noun.Termin.CanonicText == "ПУНКТ") 
            {
                if (!Pullenti.Ner.Geo.Internal.MiscLocationHelper.CheckGeoObjectBefore(sli[0].BeginToken, false)) 
                    return null;
                if (name == null || number != null) 
                    return null;
            }
            foreach (StreetItemToken r in rli) 
            {
                if (res.BeginChar > r.BeginChar) 
                    res.BeginToken = r.BeginToken;
                if (res.EndChar < r.EndChar) 
                    res.EndToken = r.EndToken;
            }
            if (forMetro && rli.Contains(noun) && noun.Termin.CanonicText == "МЕТРО") 
                rli.Remove(noun);
            if (noun.IsAbridge && (noun.LengthChar < 4)) 
                res.IsDoubt = true;
            else if (noun.NounIsDoubtCoef > 0 && !notDoubt && !Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(noun)) 
            {
                res.IsDoubt = true;
                if ((name != null && name.EndChar > noun.EndChar && noun.Chars.IsAllLower) && !name.Chars.IsAllLower && !(name.BeginToken is Pullenti.Ner.ReferentToken)) 
                {
                    Pullenti.Ner.Core.NounPhraseToken npt2 = Pullenti.Ner.Geo.Internal.MiscLocationHelper.TryParseNpt(name.BeginToken);
                    if (npt2 != null && npt2.EndChar > name.EndChar) 
                    {
                    }
                    else if (AddressItemToken.CheckHouseAfter(res.EndToken.Next, false, false)) 
                        res.IsDoubt = false;
                    else if (name.Chars.IsCapitalUpper && noun.NounIsDoubtCoef == 1) 
                        res.IsDoubt = false;
                }
            }
            StringBuilder nameBase = new StringBuilder();
            StringBuilder nameAlt = new StringBuilder();
            string nameAlt2 = null;
            Pullenti.Morph.MorphGender gen = noun.Termin.Gender;
            Pullenti.Morph.MorphGender adjGen = Pullenti.Morph.MorphGender.Undefined;
            if (number != null) 
            {
                street.Numbers = number;
                if (secNumber != null) 
                    street.Numbers = secNumber;
            }
            if (age != null) 
                street.Numbers = age;
            List<string> miscs = null;
            if (name != null && name.Value != null) 
            {
                if (name.Value.IndexOf(' ') > 0 && name.BeginToken.Next == name.EndToken) 
                {
                    Pullenti.Ner.Geo.Internal.OrgTypToken ty = Pullenti.Ner.Geo.Internal.OrgTypToken.TryParse(name.EndToken, false, null);
                    if (ty != null && ty.Vals.Count > 0) 
                    {
                        name = name.Clone();
                        name.AltValue = null;
                        name.EndToken = name.EndToken.Previous;
                        miscs = ty.Vals;
                        name.Value = name.Value.Substring(0, name.Value.IndexOf(' '));
                    }
                    else 
                    {
                        StreetItemToken nn = StreetItemToken.TryParse(name.EndToken, null, false, null);
                        if (nn != null && nn.Typ == StreetItemType.Noun && nn.Termin.CanonicText != "МОСТ") 
                        {
                            name = name.Clone();
                            name.AltValue = null;
                            name.Value = name.Value.Substring(0, name.Value.IndexOf(' '));
                            name.EndToken = name.EndToken.Previous;
                            street.AddTyp(nn.Termin.CanonicText.ToLower());
                            Pullenti.Ner.Slot ss = street.FindSlot(Pullenti.Ner.Address.StreetReferent.ATTR_TYPE, "улица", true);
                            if (ss != null) 
                                street.Slots.Remove(ss);
                        }
                    }
                }
                if (name.AltValue != null && nameAlt.Length == 0) 
                    nameAlt.AppendFormat("{0} {1}", nameBase.ToString(), name.AltValue);
                nameBase.AppendFormat(" {0}", name.Value);
            }
            else if ((name != null && name.Termin != null && number != null) && ((StreetItemType)name.Termin.Tag) == StreetItemType.Noun) 
                street.AddTyp(name.Termin.CanonicText.ToLower());
            else if (name != null) 
            {
                bool isAdj = false;
                if (name.EndToken is Pullenti.Ner.TextToken) 
                {
                    foreach (Pullenti.Morph.MorphBaseInfo wf in name.EndToken.Morph.Items) 
                    {
                        if ((wf is Pullenti.Morph.MorphWordForm) && (wf as Pullenti.Morph.MorphWordForm).IsInDictionary) 
                        {
                            isAdj = wf.Class.IsAdjective | wf.Class.IsProperGeo;
                            adjGen = wf.Gender;
                            break;
                        }
                        else if (wf.Class.IsAdjective | wf.Class.IsProperGeo) 
                            isAdj = true;
                    }
                }
                if (isAdj) 
                {
                    StringBuilder tmp = new StringBuilder();
                    List<string> vars = new List<string>();
                    for (Pullenti.Ner.Token t = name.BeginToken; t != null; t = t.Next) 
                    {
                        Pullenti.Ner.TextToken tt = t as Pullenti.Ner.TextToken;
                        if (tt == null) 
                            break;
                        if (tmp.Length > 0 && tmp[tmp.Length - 1] != ' ') 
                            tmp.Append(' ');
                        if (t == name.EndToken) 
                        {
                            bool isPadez = false;
                            if (!noun.IsAbridge) 
                            {
                                if (!noun.Morph.Case.IsUndefined && !noun.Morph.Case.IsNominative) 
                                    isPadez = true;
                                else if (noun.Termin.CanonicText == "ШОССЕ" || noun.Termin.CanonicText == "ШОСЕ") 
                                    isPadez = true;
                            }
                            if (res.BeginToken.Previous != null && res.BeginToken.Previous.Morph.Class.IsPreposition) 
                                isPadez = true;
                            if (isProezd) 
                                isPadez = true;
                            if (!isPadez) 
                            {
                                tmp.Append(tt.Term);
                                break;
                            }
                            foreach (Pullenti.Morph.MorphBaseInfo wf in tt.Morph.Items) 
                            {
                                if (((wf.Class.IsAdjective || wf.Class.IsProperGeo)) && ((wf.Gender & gen)) != Pullenti.Morph.MorphGender.Undefined) 
                                {
                                    if (noun.Morph.Case.IsUndefined || !((wf.Case & noun.Morph.Case)).IsUndefined) 
                                    {
                                        Pullenti.Morph.MorphWordForm wff = wf as Pullenti.Morph.MorphWordForm;
                                        if (wff == null) 
                                            continue;
                                        if (gen == Pullenti.Morph.MorphGender.Masculine && wff.NormalCase.EndsWith("ОЙ")) 
                                            continue;
                                        if (wff.NormalCase.EndsWith("СКИ")) 
                                            continue;
                                        if (!vars.Contains(wff.NormalCase)) 
                                            vars.Add(wff.NormalCase);
                                    }
                                }
                            }
                            if (!vars.Contains(tt.Term) && sli.IndexOf(name) > sli.IndexOf(noun)) 
                                vars.Add(tt.Term);
                            if (vars.Count == 0) 
                                vars.Add(tt.Term);
                            if (isProezd) 
                            {
                                string nnn = tt.GetNormalCaseText(Pullenti.Morph.MorphClass.Adjective, Pullenti.Morph.MorphNumber.Undefined, Pullenti.Morph.MorphGender.Undefined, false);
                                if (nnn == null) 
                                    nnn = tt.Lemma;
                                if (!vars.Contains(nnn)) 
                                    vars.Add(nnn);
                            }
                            break;
                        }
                        if (!tt.IsHiphen) 
                            tmp.Append(tt.Term);
                    }
                    if (vars.Count == 0) 
                        nameBase.AppendFormat(" {0}", tmp.ToString());
                    else 
                    {
                        string head = nameBase.ToString();
                        nameBase.AppendFormat(" {0}{1}", tmp.ToString(), vars[0]);
                        string src = Pullenti.Ner.Core.MiscHelper.GetTextValueOfMetaToken(name, Pullenti.Ner.Core.GetTextAttr.No);
                        int ii = vars.IndexOf(src);
                        if (ii > 1) 
                        {
                            vars.RemoveAt(ii);
                            vars.Insert(1, src);
                        }
                        else if (ii < 0) 
                            vars.Insert(1, src);
                        if (vars.Count > 1) 
                        {
                            nameAlt.Length = 0;
                            nameAlt.AppendFormat("{0} {1}{2}", head, tmp.ToString(), vars[1]);
                        }
                        if (vars.Count > 2) 
                            nameAlt2 = string.Format("{0} {1}{2}", head, tmp.ToString(), vars[2]);
                    }
                }
                else 
                {
                    string strNam = null;
                    List<string> nits = new List<string>();
                    bool hasAdj = false;
                    bool hasProperName = false;
                    for (Pullenti.Ner.Token t = name.BeginToken; t != null && t.EndChar <= name.EndChar; t = t.Next) 
                    {
                        if (t.Morph.Class.IsAdjective || t.Morph.Class.IsConjunction) 
                            hasAdj = true;
                        if ((t is Pullenti.Ner.TextToken) && !t.IsHiphen) 
                        {
                            if (name.Termin != null) 
                            {
                                nits.Add(name.Termin.CanonicText);
                                break;
                            }
                            else if (!t.Chars.IsLetter && nits.Count > 0) 
                                nits[nits.Count - 1] += (t as Pullenti.Ner.TextToken).Term;
                            else 
                            {
                                nits.Add((t as Pullenti.Ner.TextToken).Term);
                                if (t == name.BeginToken && t.GetMorphClassInDictionary().IsProperName) 
                                    hasProperName = true;
                            }
                        }
                        else if ((t is Pullenti.Ner.ReferentToken) && name.Termin == null) 
                            nits.Add(t.GetSourceText().ToUpper());
                    }
                    if (!hasAdj && !hasProperName && !name.IsInDictionary) 
                        nits.Sort();
                    strNam = string.Join(" ", nits.ToArray());
                    if (hasProperName && nits.Count == 2) 
                    {
                        nameAlt.Length = 0;
                        nameAlt.AppendFormat("{0} {1}", nameBase.ToString(), nits[1]);
                    }
                    nameBase.AppendFormat(" {0}", strNam);
                    if (name.Org != null && name.Org.Referent.FindSlot("NUMBER", null, true) != null) 
                        street.AddSlot("NUMBER", name.Org.Referent.GetStringValue("NUMBER"), false, 0);
                }
            }
            string adjStr = null;
            string adjStr2 = null;
            bool adjCanBeInitial = false;
            if (adj != null) 
            {
                string s = null;
                string ss = null;
                if (adjGen == Pullenti.Morph.MorphGender.Undefined && name != null && ((name.Morph.Number & Pullenti.Morph.MorphNumber.Plural)) == Pullenti.Morph.MorphNumber.Undefined) 
                {
                    if (name.Morph.Gender == Pullenti.Morph.MorphGender.Feminie || name.Morph.Gender == Pullenti.Morph.MorphGender.Masculine || name.Morph.Gender == Pullenti.Morph.MorphGender.Neuter) 
                        adjGen = name.Morph.Gender;
                }
                if (name != null && ((name.Morph.Number & Pullenti.Morph.MorphNumber.Plural)) != Pullenti.Morph.MorphNumber.Undefined) 
                {
                    try 
                    {
                        s = Pullenti.Morph.MorphologyService.GetWordform(adj.Termin.CanonicText, new Pullenti.Morph.MorphBaseInfo() { Class = Pullenti.Morph.MorphClass.Adjective, Number = Pullenti.Morph.MorphNumber.Plural });
                        if (adj.AltTermin != null) 
                            ss = Pullenti.Morph.MorphologyService.GetWordform(adj.AltTermin.CanonicText, new Pullenti.Morph.MorphBaseInfo() { Class = Pullenti.Morph.MorphClass.Adjective, Number = Pullenti.Morph.MorphNumber.Plural });
                    }
                    catch(Exception ex) 
                    {
                    }
                }
                else if (adjGen != Pullenti.Morph.MorphGender.Undefined) 
                {
                    try 
                    {
                        s = Pullenti.Morph.MorphologyService.GetWordform(adj.Termin.CanonicText, new Pullenti.Morph.MorphBaseInfo() { Class = Pullenti.Morph.MorphClass.Adjective, Gender = adjGen });
                        if (adj.AltTermin != null) 
                            ss = Pullenti.Morph.MorphologyService.GetWordform(adj.AltTermin.CanonicText, new Pullenti.Morph.MorphBaseInfo() { Class = Pullenti.Morph.MorphClass.Adjective, Gender = adjGen });
                    }
                    catch(Exception ex) 
                    {
                    }
                }
                else if (((adj.Morph.Gender & gen)) == Pullenti.Morph.MorphGender.Undefined) 
                {
                    try 
                    {
                        Pullenti.Morph.MorphGender ggg = noun.Termin.Gender;
                        s = Pullenti.Morph.MorphologyService.GetWordform(adj.Termin.CanonicText, new Pullenti.Morph.MorphBaseInfo() { Class = Pullenti.Morph.MorphClass.Adjective, Gender = ggg });
                        if (adj.AltTermin != null) 
                            ss = Pullenti.Morph.MorphologyService.GetWordform(adj.AltTermin.CanonicText, new Pullenti.Morph.MorphBaseInfo() { Class = Pullenti.Morph.MorphClass.Adjective, Gender = ggg });
                    }
                    catch(Exception ex) 
                    {
                    }
                }
                else 
                    try 
                    {
                        s = Pullenti.Morph.MorphologyService.GetWordform(adj.Termin.CanonicText, new Pullenti.Morph.MorphBaseInfo() { Class = Pullenti.Morph.MorphClass.Adjective, Gender = gen });
                        if (adj.AltTermin != null) 
                            ss = Pullenti.Morph.MorphologyService.GetWordform(adj.AltTermin.CanonicText, new Pullenti.Morph.MorphBaseInfo() { Class = Pullenti.Morph.MorphClass.Adjective, Gender = gen });
                    }
                    catch(Exception ex) 
                    {
                    }
                adjStr = s;
                adjStr2 = ss;
                if (name != null) 
                {
                    if (adj.EndToken.IsChar('.') && adj.LengthChar <= 3 && !adj.BeginToken.Chars.IsAllLower) 
                        adjCanBeInitial = true;
                }
            }
            string s1 = nameBase.ToString().Trim();
            string s2 = nameAlt.ToString().Trim();
            if ((s1.Length < 3) && street.Kind != Pullenti.Ner.Address.StreetKind.Road) 
            {
                if (street.Numbers != null) 
                {
                    if (adjStr != null) 
                    {
                        if (adj.IsAbridge) 
                            return null;
                        street.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NAME, adjStr, false, 0);
                    }
                    else if (Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(res) && s1.Length > 0) 
                        street.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NAME, s1, false, 0);
                }
                else if (adjStr == null) 
                {
                    if (s1.Length < 1) 
                        return null;
                    if (isMicroRaion || Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(res)) 
                    {
                        street.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NAME, s1, false, 0);
                        if (!string.IsNullOrEmpty(s2)) 
                            street.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NAME, s2, false, 0);
                    }
                    else 
                        return null;
                }
                else 
                {
                    if (adj.IsAbridge && !Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(adj) && altNoun == null) 
                        return null;
                    street.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NAME, adjStr, false, 0);
                }
            }
            else if (adjCanBeInitial) 
            {
                street.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NAME, s1, false, 0);
                street.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NAME, Pullenti.Ner.Core.MiscHelper.GetTextValue(adj.BeginToken, name.EndToken, Pullenti.Ner.Core.GetTextAttr.No), false, 0);
                street.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NAME, string.Format("{0} {1}", adjStr, s1), false, 0);
                if (adjStr2 != null) 
                    street.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NAME, string.Format("{0} {1}", adjStr2, s1), false, 0);
            }
            else if (adjStr == null) 
            {
                if (!string.IsNullOrEmpty(s1)) 
                    street.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NAME, s1, false, 0);
            }
            else 
            {
                street.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NAME, string.Format("{0} {1}", adjStr, s1), false, 0);
                if (adjStr2 != null) 
                    street.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NAME, string.Format("{0} {1}", adjStr2, s1), false, 0);
            }
            if (nameAlt.Length > 0) 
            {
                s1 = nameAlt.ToString().Trim();
                if (adjStr == null) 
                    street.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NAME, s1, false, 0);
                else 
                {
                    street.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NAME, string.Format("{0} {1}", adjStr, s1), false, 0);
                    if (adjStr2 != null) 
                        street.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NAME, string.Format("{0} {1}", adjStr2, s1), false, 0);
                }
            }
            if (nameAlt2 != null) 
            {
                if (adjStr == null) 
                {
                    if (forMetro && noun != null) 
                        street.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NAME, string.Format("{0} {1}", altNoun.Termin.CanonicText, nameAlt2.Trim()), false, 0);
                    else 
                        street.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NAME, nameAlt2.Trim(), false, 0);
                }
                else 
                {
                    street.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NAME, string.Format("{0} {1}", adjStr, nameAlt2.Trim()), false, 0);
                    if (adjStr2 != null) 
                        street.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NAME, string.Format("{0} {1}", adjStr2, nameAlt2.Trim()), false, 0);
                }
            }
            if (name != null && name.AltValue2 != null) 
                street.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NAME, name.AltValue2, false, 0);
            if ((name != null && adj == null && name.ExistStreet != null) && !forMetro) 
            {
                foreach (string n in name.ExistStreet.Names) 
                {
                    street.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NAME, n, false, 0);
                }
            }
            if (miscs != null) 
            {
                foreach (string m in miscs) 
                {
                    street.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_MISC, m, false, 0);
                }
            }
            if (altNoun != null && !forMetro) 
                street.AddTyp(altNoun.Termin.CanonicText.ToLower());
            if (noun.Termin.CanonicText == "ПЛОЩАДЬ" || noun.Termin.CanonicText == "КВАРТАЛ" || noun.Termin.CanonicText == "ПЛОЩА") 
            {
                res.IsDoubt = true;
                if (name != null && name.IsInDictionary) 
                    res.IsDoubt = false;
                else if (altNoun != null || forMetro || adj != null) 
                    res.IsDoubt = false;
                else if (name != null && StreetItemToken.CheckStdName(name.BeginToken) != null) 
                    res.IsDoubt = false;
                else if (res.BeginToken.Previous == null || Pullenti.Ner.Geo.Internal.MiscLocationHelper.CheckGeoObjectBefore(res.BeginToken.Previous, false)) 
                {
                    if (res.EndToken.Next == null || AddressItemToken.CheckHouseAfter(res.EndToken.Next, false, true)) 
                        res.IsDoubt = false;
                }
            }
            if (name != null && adj == null && name.StdAdjVersion != null) 
            {
                List<string> nams = street.Names;
                foreach (string n in nams) 
                {
                    if (n.IndexOf(' ') < 0) 
                    {
                        List<string> adjs = Pullenti.Ner.Geo.Internal.MiscLocationHelper.GetStdAdjFull(name.StdAdjVersion.BeginToken, noun.Termin.Gender, Pullenti.Morph.MorphNumber.Singular, false);
                        if (adjs != null && adjs.Count > 0) 
                        {
                            foreach (string a in adjs) 
                            {
                                street.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NAME, string.Format("{0} {1}", a, n), false, 0);
                            }
                        }
                        else 
                        {
                            street.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NAME, string.Format("{0} {1}", name.StdAdjVersion.Termin.CanonicText, n), false, 0);
                            if (name.StdAdjVersion.AltTermin != null) 
                                street.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NAME, string.Format("{0} {1}", name.StdAdjVersion.AltTermin.CanonicText, n), false, 0);
                        }
                    }
                }
            }
            if (Pullenti.Morph.LanguageHelper.EndsWith(noun.Termin.CanonicText, "ГОРОДОК")) 
            {
                street.Kind = Pullenti.Ner.Address.StreetKind.Area;
                foreach (Pullenti.Ner.Slot s in street.Slots) 
                {
                    if (s.TypeName == Pullenti.Ner.Address.StreetReferent.ATTR_TYPE) 
                        street.UploadSlot(s, "микрорайон");
                    else if (s.TypeName == Pullenti.Ner.Address.StreetReferent.ATTR_NAME) 
                        street.UploadSlot(s, string.Format("{0} {1}", noun.Termin.CanonicText, s.Value));
                }
                if (street.FindSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NAME, null, true) == null) 
                    street.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NAME, noun.Termin.CanonicText, false, 0);
            }
            Pullenti.Ner.Token t1 = res.EndToken.Next;
            if (t1 != null && t1.IsComma) 
                t1 = t1.Next;
            StreetItemToken non = StreetItemToken.TryParse(t1, null, false, null);
            if (non != null && non.Typ == StreetItemType.Noun && street.Typs.Count > 0) 
            {
                if (AddressItemToken.CheckHouseAfter(non.EndToken.Next, false, true)) 
                {
                    street.Correct();
                    List<string> nams = street.Names;
                    foreach (string t in street.Typs) 
                    {
                        if (t != "улица") 
                        {
                            foreach (string n in nams) 
                            {
                                street.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NAME, string.Format("{0} {1}", t.ToUpper(), n), false, 0);
                            }
                        }
                    }
                    street.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_TYPE, non.Termin.CanonicText.ToLower(), false, 0);
                    res.EndToken = non.EndToken;
                }
            }
            if (street.FindSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NAME, "ПРОЕКТИРУЕМЫЙ", true) != null && street.Numbers == null) 
            {
                if (non != null && non.Typ == StreetItemType.Number) 
                {
                    street.Numbers = non.Value;
                    res.EndToken = non.EndToken;
                }
                else 
                {
                    Pullenti.Ner.Token ttt = Pullenti.Ner.Core.MiscHelper.CheckNumberPrefix(res.EndToken.Next);
                    if (ttt != null) 
                    {
                        non = StreetItemToken.TryParse(ttt, null, false, null);
                        if (non != null && non.Typ == StreetItemType.Number) 
                        {
                            street.Numbers = non.Value;
                            res.EndToken = non.EndToken;
                        }
                    }
                }
            }
            if (res.IsDoubt) 
            {
                if (noun.IsRoad) 
                {
                    street.Kind = Pullenti.Ner.Address.StreetKind.Road;
                    string num = street.Numbers;
                    if (num != null && num.Contains("км")) 
                        res.IsDoubt = false;
                    else if (AddressItemToken.CheckKmAfter(res.EndToken.Next)) 
                        res.IsDoubt = false;
                    else if (AddressItemToken.CheckKmBefore(res.BeginToken.Previous)) 
                        res.IsDoubt = false;
                }
                else if (noun.Termin.CanonicText == "ПРОЕЗД" && street.FindSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NAME, "ПРОЕКТИРУЕМЫЙ", true) != null) 
                    res.IsDoubt = false;
                for (Pullenti.Ner.Token tt0 = res.BeginToken.Previous; tt0 != null; tt0 = tt0.Previous) 
                {
                    if (tt0.IsCharOf(",.") || tt0.IsCommaAnd) 
                        continue;
                    Pullenti.Ner.Address.StreetReferent str0 = tt0.GetReferent() as Pullenti.Ner.Address.StreetReferent;
                    if (str0 != null) 
                        res.IsDoubt = false;
                    break;
                }
                if (res.IsDoubt) 
                {
                    if (Pullenti.Ner.Core.BracketHelper.CanBeStartOfSequence(res.BeginToken.Previous, true, false) && Pullenti.Ner.Core.BracketHelper.CanBeEndOfSequence(res.EndToken.Next, true, null, false)) 
                        return null;
                    if (isProezd) 
                        res.IsDoubt = false;
                    else if (AddressItemToken.CheckHouseAfter(res.EndToken.Next, false, false)) 
                        res.IsDoubt = false;
                    else if (AddressItemToken.CheckStreetAfter(res.EndToken.Next, false)) 
                        res.IsDoubt = false;
                    else if (Pullenti.Ner.Geo.Internal.MiscLocationHelper.CheckGeoObjectBefore(res.BeginToken, false)) 
                        res.IsDoubt = false;
                    for (Pullenti.Ner.Token ttt = res.BeginToken.Next; ttt != null && ttt.EndChar <= res.EndChar; ttt = ttt.Next) 
                    {
                        if (ttt.IsNewlineBefore) 
                            res.IsDoubt = true;
                    }
                }
            }
            if (noun.Termin.CanonicText == "КВАРТАЛ" && (res.WhitespacesAfterCount < 2) && number == null) 
            {
                AddressItemToken ait = AddressItemToken.TryParsePureItem(res.EndToken.Next, null, null);
                if (ait != null && ait.Typ == AddressItemType.Number && ait.Value != null) 
                {
                    street.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NUMBER, ait.Value, false, 0);
                    res.EndToken = ait.EndToken;
                }
            }
            if (age != null && street.FindSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NAME, null, true) == null) 
                street.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NAME, "ЛЕТ", false, 0);
            if (name != null) 
                street.AddMisc(name.Misc);
            if (street.Numbers == null && ((street.Kind == Pullenti.Ner.Address.StreetKind.Road || street.Kind == Pullenti.Ner.Address.StreetKind.Railway))) 
            {
                t1 = res.EndToken.Next;
                if (t1 != null && t1.IsComma) 
                    t1 = t1.Next;
                StreetItemToken sit = StreetItemToken.TryParse(t1, null, false, null);
                if (sit != null && sit.IsNumberKm) 
                {
                    res.EndToken = sit.EndToken;
                    street.Numbers = sit.Value + "км";
                }
            }
            foreach (StreetItemToken r in rli) 
            {
                if (r.OrtoTerr != null) 
                {
                    res.OrtoTerr = r.OrtoTerr;
                    break;
                }
            }
            if (noun.IsRoad) 
            {
                List<StreetItemToken> nam2 = StreetItemToken.TryParseSpec(res.EndToken.Next, noun);
                if (nam2 != null && nam2.Count == 1 && nam2[0].IsRoadName) 
                {
                    res.EndToken = nam2[0].EndToken;
                    street.AddName(nam2[0]);
                }
            }
            return res;
        }
        static AddressItemToken TryDetectNonNoun(List<StreetItemToken> sli, bool ontoRegim, bool forMetro, bool streetBefore, Pullenti.Ner.Address.StreetReferent crossStreet)
        {
            if (sli.Count > 1 && sli[sli.Count - 1].Typ == StreetItemType.Number && !sli[sli.Count - 1].NumberHasPrefix) 
                sli.RemoveAt(sli.Count - 1);
            Pullenti.Ner.Address.StreetReferent street;
            if (sli.Count == 1 && ((sli[0].Typ == StreetItemType.Name || sli[0].Typ == StreetItemType.StdName || sli[0].Typ == StreetItemType.StdAdjective)) && ((ontoRegim || forMetro))) 
            {
                string s = Pullenti.Ner.Core.MiscHelper.GetTextValue(sli[0].BeginToken, sli[0].EndToken, Pullenti.Ner.Core.GetTextAttr.No);
                if (s == null) 
                    return null;
                if (!forMetro && !sli[0].IsInDictionary && sli[0].ExistStreet == null) 
                {
                    Pullenti.Ner.Token tt = sli[0].EndToken.Next;
                    if (tt != null && tt.IsComma) 
                        tt = tt.Next;
                    AddressItemToken ait1 = AddressItemToken.TryParsePureItem(tt, null, null);
                    if (ait1 != null && ((ait1.Typ == AddressItemType.Number || ait1.Typ == AddressItemType.House))) 
                    {
                    }
                    else if (((tt == null || tt.IsComma || tt.IsNewlineBefore)) && Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(sli[0])) 
                    {
                    }
                    else 
                        return null;
                }
                street = new Pullenti.Ner.Address.StreetReferent();
                if (forMetro) 
                    street.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_TYPE, "метро", false, 0);
                if (sli[0].Value != null) 
                    street.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NAME, sli[0].Value, false, 0);
                if (sli[0].AltValue != null) 
                    street.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NAME, sli[0].AltValue, false, 0);
                if (sli[0].AltValue2 != null) 
                    street.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NAME, sli[0].AltValue2, false, 0);
                street.AddMisc(sli[0].Misc);
                street.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NAME, s, false, 0);
                AddressItemToken res0 = new AddressItemToken(AddressItemType.Street, sli[0].BeginToken, sli[0].EndToken) { Referent = street, IsDoubt = true };
                if (sli[0].IsInBrackets) 
                    res0.IsDoubt = false;
                return res0;
            }
            if ((sli.Count == 1 && sli[0].Typ == StreetItemType.Number && sli[0].IsNumberKm) && Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(sli[0])) 
            {
                street = new Pullenti.Ner.Address.StreetReferent();
                street.Numbers = sli[0].Value + "км";
                AddressItemToken res0 = new AddressItemToken(AddressItemType.Street, sli[0].BeginToken, sli[0].EndToken) { Referent = street, IsDoubt = true };
                return res0;
            }
            if ((sli.Count == 1 && sli[0].Typ == StreetItemType.Number && sli[0].BeginToken.Morph.Class.IsAdjective) && Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(sli[0])) 
            {
                if (streetBefore) 
                    return null;
                street = new Pullenti.Ner.Address.StreetReferent();
                street.Numbers = sli[0].Value;
                AddressItemToken res0 = new AddressItemToken(AddressItemType.Street, sli[0].BeginToken, sli[0].EndToken) { Referent = street, IsDoubt = true };
                return res0;
            }
            int i1 = 0;
            if (sli.Count == 1 && (((sli[0].Typ == StreetItemType.StdName || sli[0].Typ == StreetItemType.Name || sli[0].Typ == StreetItemType.StdAdjective) || ((sli[0].Typ == StreetItemType.Noun && sli[0].NounCanBeName))))) 
            {
                if (!ontoRegim) 
                {
                    bool isStreetBefore = streetBefore;
                    Pullenti.Ner.Token tt = sli[0].BeginToken.Previous;
                    Pullenti.Ner.Address.StreetReferent sBefor = null;
                    if ((tt != null && tt.IsCommaAnd && tt.Previous != null) && (tt.Previous.GetReferent() is Pullenti.Ner.Address.StreetReferent)) 
                    {
                        isStreetBefore = true;
                        sBefor = tt.Previous.GetReferent() as Pullenti.Ner.Address.StreetReferent;
                    }
                    int cou = 0;
                    for (tt = sli[0].EndToken.Next; tt != null; tt = tt.Next) 
                    {
                        if (!tt.IsCommaAnd || tt.Next == null) 
                            break;
                        List<StreetItemToken> sli2 = StreetItemToken.TryParseList(tt.Next, 10, null);
                        if (sli2 == null) 
                            break;
                        StreetItemToken noun = null;
                        bool empty = true;
                        foreach (StreetItemToken si in sli2) 
                        {
                            if (si.Typ == StreetItemType.Noun) 
                                noun = si;
                            else if ((si.Typ == StreetItemType.Name || si.Typ == StreetItemType.StdName || si.Typ == StreetItemType.Number) || si.Typ == StreetItemType.StdAdjective) 
                                empty = false;
                        }
                        if (empty) 
                            break;
                        if (noun == null) 
                        {
                            if (tt.IsAnd && !isStreetBefore) 
                                break;
                            if ((++cou) > 4) 
                                break;
                            tt = sli2[sli2.Count - 1].EndToken;
                            continue;
                        }
                        if (!tt.IsAnd && !isStreetBefore) 
                            break;
                        if (noun == sli2[0]) 
                        {
                            if (sBefor != null && (sBefor.Tag is StreetItemToken)) 
                                noun = sBefor.Tag as StreetItemToken;
                            else if (sBefor != null && sBefor.Typs.Count > 0) 
                            {
                                noun = new StreetItemToken(tt, tt) { Typ = StreetItemType.Noun, Value = sBefor.Typs[0] };
                                noun.Termin = new Pullenti.Ner.Core.Termin(sBefor.Typs[0]);
                            }
                            else 
                                break;
                        }
                        List<StreetItemToken> tmp = new List<StreetItemToken>();
                        tmp.Add(sli[0]);
                        tmp.Add(noun);
                        AddressItemToken re = TryParseStreet(tmp, false, forMetro, false, null);
                        if (re != null) 
                        {
                            re.BeginToken = sli[0].BeginToken;
                            re.EndToken = sli[0].EndToken;
                            return re;
                        }
                    }
                    if (crossStreet != null) 
                        i1 = 0;
                    else if (sBefor != null && (sBefor.Tag is StreetItemToken)) 
                    {
                        List<StreetItemToken> tmp = new List<StreetItemToken>();
                        tmp.Add(sli[0]);
                        tmp.Add(sBefor.Tag as StreetItemToken);
                        AddressItemToken re = TryParseStreet(tmp, false, forMetro, false, null);
                        if (re != null) 
                        {
                            re.BeginToken = sli[0].BeginToken;
                            re.EndToken = sli[0].EndToken;
                            return re;
                        }
                    }
                }
                if (sli[0].WhitespacesAfterCount < 2) 
                {
                    Pullenti.Ner.Token tt = Pullenti.Ner.Geo.Internal.MiscLocationHelper.CheckTerritory(sli[0].EndToken.Next);
                    if (tt != null) 
                    {
                        bool ok1 = false;
                        if ((tt.IsNewlineAfter || tt.Next == null || tt.Next.IsComma) || tt.Next.IsChar(')')) 
                            ok1 = true;
                        else if (AddressItemToken.CheckHouseAfter(tt.Next, false, false)) 
                            ok1 = true;
                        else if (AddressItemToken.CheckStreetAfter(tt.Next, false)) 
                            ok1 = true;
                        if (ok1) 
                        {
                            street = new Pullenti.Ner.Address.StreetReferent();
                            street.AddTyp("территория");
                            street.Kind = Pullenti.Ner.Address.StreetKind.Area;
                            street.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NAME, sli[0].Value ?? Pullenti.Ner.Core.MiscHelper.GetTextValueOfMetaToken(sli[0], Pullenti.Ner.Core.GetTextAttr.No), false, 0);
                            if (sli[0].AltValue != null) 
                                street.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NAME, sli[0].AltValue, false, 0);
                            if (sli[0].AltValue2 != null) 
                                street.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NAME, sli[0].AltValue2, false, 0);
                            street.AddMisc(sli[0].Misc);
                            return new AddressItemToken(AddressItemType.Street, sli[0].BeginToken, tt) { Referent = street };
                        }
                    }
                }
                if (!Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(sli[0]) && !streetBefore) 
                {
                    if (Pullenti.Ner.Geo.Internal.MiscLocationHelper.CheckGeoObjectBefore(sli[0].BeginToken, false)) 
                    {
                    }
                    else if (AddressItemToken.CheckHouseAfter(sli[0].EndToken.Next, false, false)) 
                    {
                        Pullenti.Ner.Token tt2 = sli[0].EndToken.Next;
                        if (tt2.IsComma) 
                            tt2 = tt2.Next;
                        AddressItemToken ait = AddressItemToken.TryParsePureItem(tt2, null, null);
                        if ((ait != null && ((ait.Typ == AddressItemType.House || ait.Typ == AddressItemType.Building || ait.Typ == AddressItemType.Corpus)) && !string.IsNullOrEmpty(ait.Value)) && ait.Value != "0" && char.IsDigit(ait.Value[0])) 
                        {
                        }
                        else 
                            return null;
                    }
                    else 
                        return null;
                }
            }
            else if (sli.Count == 2 && ((sli[0].Typ == StreetItemType.StdAdjective || sli[0].Typ == StreetItemType.Number || sli[0].Typ == StreetItemType.Age)) && ((sli[1].Typ == StreetItemType.StdName || sli[1].Typ == StreetItemType.Name))) 
            {
                if (streetBefore) 
                {
                    AddressItemToken ait = AddressItemToken.TryParsePureItem(sli[0].BeginToken, null, null);
                    if (ait != null && ait.Value != null) 
                        return null;
                }
                if (sli[0].Typ == StreetItemType.Number && sli[1].Typ == StreetItemType.Name) 
                {
                    if (AddressItemToken.TryParsePureItem(sli[1].BeginToken, null, null) != null) 
                        return null;
                }
                i1 = 1;
            }
            else if (sli.Count == 2 && ((sli[0].Typ == StreetItemType.StdName || sli[0].Typ == StreetItemType.Name)) && ((sli[1].Typ == StreetItemType.Number || sli[1].Typ == StreetItemType.StdAdjective))) 
            {
                if (!Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(sli[0])) 
                    return null;
                i1 = 0;
            }
            else if ((sli.Count == 3 && ((sli[0].Typ == StreetItemType.StdName || sli[0].Typ == StreetItemType.Name)) && sli[1].Typ == StreetItemType.Number) && sli[2].Typ == StreetItemType.StdName) 
                i1 = 0;
            else if (sli.Count == 1 && sli[0].Typ == StreetItemType.Number && sli[0].IsNumberKm) 
            {
                for (Pullenti.Ner.Token tt = sli[0].BeginToken.Previous; tt != null; tt = tt.Previous) 
                {
                    if (tt.LengthChar == 1) 
                        continue;
                    Pullenti.Ner.Geo.GeoReferent geo = tt.GetReferent() as Pullenti.Ner.Geo.GeoReferent;
                    if (geo == null) 
                        break;
                    bool ok1 = false;
                    if (geo.FindSlot(Pullenti.Ner.Geo.GeoReferent.ATTR_TYPE, "станция", true) != null) 
                        ok1 = true;
                    if (ok1) 
                    {
                        street = new Pullenti.Ner.Address.StreetReferent();
                        street.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NUMBER, string.Format("{0}км", sli[0].Value), false, 0);
                        AddressItemToken res0 = new AddressItemToken(AddressItemType.Street, sli[0].BeginToken, sli[0].EndToken) { Referent = street, IsDoubt = true };
                        if (sli[0].IsInBrackets) 
                            res0.IsDoubt = false;
                        return res0;
                    }
                }
                return null;
            }
            else 
                return null;
            string val = sli[i1].Value;
            string altVal = sli[i1].AltValue;
            if (altVal == val) 
                altVal = null;
            List<string> miscs = null;
            if (val != null && val.IndexOf(' ') > 0 && sli[i1].BeginToken.Next == sli[i1].EndToken) 
            {
                Pullenti.Ner.Geo.Internal.OrgTypToken ty = Pullenti.Ner.Geo.Internal.OrgTypToken.TryParse(sli[i1].EndToken, false, null);
                if (ty != null && ty.Vals.Count > 0) 
                {
                    altVal = null;
                    miscs = ty.Vals;
                    val = val.Substring(0, val.IndexOf(' '));
                }
            }
            if (sli[i1].Value == null && sli[i1].Termin != null && sli[i1].Typ == StreetItemType.Noun) 
                val = sli[i1].Termin.CanonicText;
            StreetItemToken stdAdjProb = sli[i1].StdAdjVersion;
            if (val == null) 
            {
                if (sli[i1].ExistStreet != null) 
                {
                    List<string> names = sli[i1].ExistStreet.Names;
                    if (names.Count > 0) 
                    {
                        val = names[0];
                        if (names.Count > 1) 
                            altVal = names[1];
                    }
                }
                else 
                {
                    Pullenti.Ner.TextToken te = sli[i1].BeginToken as Pullenti.Ner.TextToken;
                    if (te != null) 
                    {
                        foreach (Pullenti.Morph.MorphBaseInfo wf in te.Morph.Items) 
                        {
                            if (wf.Class.IsAdjective && wf.Gender == Pullenti.Morph.MorphGender.Feminie && !wf.ContainsAttr("к.ф.", null)) 
                            {
                                val = (wf as Pullenti.Morph.MorphWordForm).NormalCase;
                                break;
                            }
                        }
                    }
                    if (i1 > 0 && sli[0].Typ == StreetItemType.Age) 
                        val = Pullenti.Ner.Core.MiscHelper.GetTextValueOfMetaToken(sli[i1], Pullenti.Ner.Core.GetTextAttr.No);
                    else 
                    {
                        altVal = Pullenti.Ner.Core.MiscHelper.GetTextValueOfMetaToken(sli[i1], Pullenti.Ner.Core.GetTextAttr.No);
                        if (val == null && te.Morph.Class.IsAdjective) 
                        {
                            val = altVal;
                            altVal = null;
                        }
                    }
                    if (sli.Count > 1 && val == null && altVal != null) 
                    {
                        val = altVal;
                        altVal = null;
                    }
                }
            }
            bool veryDoubt = false;
            if (val == null && sli.Count == 1 && sli[0].Chars.IsCapitalUpper) 
            {
                veryDoubt = true;
                Pullenti.Ner.Token t0 = sli[0].BeginToken.Previous;
                if (t0 != null && t0.IsChar(',')) 
                    t0 = t0.Previous;
                if ((t0 is Pullenti.Ner.ReferentToken) && (t0.GetReferent() is Pullenti.Ner.Geo.GeoReferent)) 
                    val = Pullenti.Ner.Core.MiscHelper.GetTextValue(sli[0].BeginToken, sli[0].EndToken, Pullenti.Ner.Core.GetTextAttr.No);
            }
            if (val == null) 
                return null;
            Pullenti.Ner.Token t = sli[sli.Count - 1].EndToken.Next;
            if (t != null && t.IsChar(',')) 
                t = t.Next;
            if (t == null) 
            {
                if (!Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(sli[0])) 
                    return null;
            }
            bool ok = false;
            bool doubt = true;
            if (sli[i1].Termin != null && ((StreetItemType)sli[i1].Termin.Tag) == StreetItemType.Fix) 
            {
                ok = true;
                doubt = false;
            }
            else if (((sli[i1].ExistStreet != null || sli[0].ExistStreet != null)) && sli[0].BeginToken != sli[i1].EndToken) 
            {
                ok = true;
                doubt = false;
                if (t.Kit.ProcessReferent("PERSON", sli[0].BeginToken, null) != null) 
                {
                    if (AddressItemToken.CheckHouseAfter(t, false, false)) 
                    {
                    }
                    else 
                        doubt = true;
                }
            }
            else if (crossStreet != null) 
                ok = true;
            else if (t == null) 
                ok = true;
            else if (t.IsCharOf("\\/")) 
                ok = true;
            else if (AddressItemToken.CheckHouseAfter(t, false, false)) 
            {
                if (t.Previous != null) 
                {
                    if (t.Previous.IsValue("АРЕНДА", "ОРЕНДА") || t.Previous.IsValue("СДАЧА", "ЗДАЧА") || t.Previous.IsValue("СЪЕМ", "ЗНІМАННЯ")) 
                        return null;
                }
                Pullenti.Ner.Core.NounPhraseToken vv = Pullenti.Ner.Geo.Internal.MiscLocationHelper.TryParseNpt(t.Previous);
                if (vv != null && vv.EndChar >= t.BeginChar) 
                    return null;
                ok = true;
            }
            else if (Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(t) && ((t.IsNewlineBefore || t.IsValue("Д", null)))) 
                ok = true;
            else if (sli[0].Typ == StreetItemType.Age && Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(sli[0])) 
                ok = true;
            else if ((t.IsChar('(') && (t.Next is Pullenti.Ner.ReferentToken) && (t.Next.GetReferent() is Pullenti.Ner.Geo.GeoReferent)) && (t.Next.GetReferent() as Pullenti.Ner.Geo.GeoReferent).IsCity) 
                ok = true;
            else 
            {
                AddressItemToken ait = AddressItemToken.TryParsePureItem(t, null, null);
                if (ait == null) 
                    return null;
                if (ait.Typ == AddressItemType.House && ait.Value != null) 
                    ok = true;
                else if (veryDoubt) 
                    return null;
                else if (((val == "ТАБЛИЦА" || val == "РИСУНОК" || val == "ДИАГРАММА") || val == "ТАБЛИЦЯ" || val == "МАЛЮНОК") || val == "ДІАГРАМА") 
                    return null;
                else if ((ait.Typ == AddressItemType.Number && (ait.BeginToken.WhitespacesBeforeCount < 4) && crossStreet == null) && sli[0].Typ != StreetItemType.Age) 
                {
                    Pullenti.Ner.NumberToken nt = ait.BeginToken as Pullenti.Ner.NumberToken;
                    if ((nt == null || nt.IntValue == null || nt.Typ != Pullenti.Ner.NumberSpellingType.Digit) || nt.Morph.Class.IsAdjective) 
                        return null;
                    if (ait.EndToken.Next != null && !ait.EndToken.IsNewlineAfter) 
                    {
                        Pullenti.Morph.MorphClass mc = ait.EndToken.Next.GetMorphClassInDictionary();
                        if (mc.IsAdjective || mc.IsNoun) 
                            return null;
                    }
                    if (nt.IntValue.Value > 100) 
                        return null;
                    if (!Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(ait)) 
                    {
                        Pullenti.Ner.Core.NumberExToken nex = Pullenti.Ner.Core.NumberHelper.TryParseNumberWithPostfix(ait.BeginToken);
                        if (nex != null) 
                            return null;
                    }
                    for (t = sli[0].BeginToken.Previous; t != null; t = t.Previous) 
                    {
                        if (t.IsNewlineAfter) 
                            break;
                        if (t.GetReferent() is Pullenti.Ner.Geo.GeoReferent) 
                        {
                            ok = true;
                            break;
                        }
                        if (t.IsChar(',')) 
                            continue;
                        if (t.IsChar('.')) 
                            break;
                        AddressItemToken ait0 = AddressItemToken.TryParsePureItem(t, null, null);
                        if (ait != null) 
                        {
                            if (ait.Typ == AddressItemType.Prefix) 
                            {
                                ok = true;
                                break;
                            }
                        }
                        if (t.Chars.IsLetter) 
                            break;
                    }
                    if (!ok) 
                    {
                        if (Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(sli[0])) 
                            ok = true;
                    }
                }
            }
            if (!ok) 
                return null;
            Pullenti.Ner.Geo.Internal.OrgItemToken ooo = Pullenti.Ner.Geo.Internal.OrgItemToken.TryParse(sli[0].BeginToken, null);
            if (ooo == null && sli.Count > 1) 
                ooo = Pullenti.Ner.Geo.Internal.OrgItemToken.TryParse(sli[1].BeginToken, null);
            if (ooo != null) 
                return null;
            street = new Pullenti.Ner.Address.StreetReferent();
            if (crossStreet != null) 
            {
                foreach (string ty in crossStreet.Typs) 
                {
                    street.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_TYPE, ty, false, 0);
                }
            }
            if (sli.Count > 1) 
            {
                if (sli[0].Typ == StreetItemType.Number || sli[0].Typ == StreetItemType.Age) 
                    street.Numbers = sli[0].Value;
                else if (sli[1].Typ == StreetItemType.Number || sli[1].Typ == StreetItemType.Age) 
                    street.Numbers = sli[1].Value;
                else 
                {
                    List<string> adjs = null;
                    if (sli[0].Typ == StreetItemType.StdAdjective) 
                    {
                        adjs = Pullenti.Ner.Geo.Internal.MiscLocationHelper.GetStdAdjFull(sli[0].BeginToken, sli[1].Morph.Gender, sli[1].Morph.Number, true);
                        if (adjs == null) 
                            adjs = Pullenti.Ner.Geo.Internal.MiscLocationHelper.GetStdAdjFull(sli[0].BeginToken, Pullenti.Morph.MorphGender.Feminie, Pullenti.Morph.MorphNumber.Singular, false);
                    }
                    else if (sli[1].Typ == StreetItemType.StdAdjective) 
                    {
                        adjs = Pullenti.Ner.Geo.Internal.MiscLocationHelper.GetStdAdjFull(sli[1].BeginToken, sli[0].Morph.Gender, sli[0].Morph.Number, true);
                        if (adjs == null) 
                            adjs = Pullenti.Ner.Geo.Internal.MiscLocationHelper.GetStdAdjFull(sli[1].BeginToken, Pullenti.Morph.MorphGender.Feminie, Pullenti.Morph.MorphNumber.Singular, false);
                    }
                    if (adjs != null) 
                    {
                        if (adjs.Count > 1) 
                            altVal = string.Format("{0} {1}", adjs[1], val);
                        if (sli[0].IsAbridge) 
                            altVal = string.Format("{0} {1}", adjs[0], val);
                        else 
                            val = string.Format("{0} {1}", adjs[0], val);
                    }
                }
            }
            street.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NAME, val, false, 0);
            if (altVal != null) 
                street.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NAME, altVal, false, 0);
            if (miscs != null) 
            {
                foreach (string m in miscs) 
                {
                    street.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_MISC, m, false, 0);
                }
            }
            if (stdAdjProb != null) 
            {
                List<string> adjs = Pullenti.Ner.Geo.Internal.MiscLocationHelper.GetStdAdjFull(stdAdjProb.BeginToken, Pullenti.Morph.MorphGender.Undefined, Pullenti.Morph.MorphNumber.Undefined, true);
                if (adjs != null) 
                {
                    foreach (string a in adjs) 
                    {
                        street.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NAME, string.Format("{0} {1}", a, val), false, 0);
                        if (altVal != null) 
                            street.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NAME, string.Format("{0} {1}", a, altVal), false, 0);
                    }
                }
                else 
                {
                    street.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NAME, string.Format("{0} {1}", stdAdjProb.Termin.CanonicText, val), false, 0);
                    if (altVal != null) 
                        street.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NAME, string.Format("{0} {1}", stdAdjProb.Termin.CanonicText, altVal), false, 0);
                    if (stdAdjProb.AltTermin != null) 
                    {
                        street.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NAME, string.Format("{0} {1}", stdAdjProb.AltTermin.CanonicText, val), false, 0);
                        if (altVal != null) 
                            street.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NAME, string.Format("{0} {1}", stdAdjProb.AltTermin.CanonicText, altVal), false, 0);
                    }
                }
            }
            street.AddMisc(sli[0].Misc);
            if (sli.Count > 1) 
                street.AddMisc(sli[1].Misc);
            Pullenti.Ner.Token t00 = sli[0].BeginToken;
            if (street.Kind == Pullenti.Ner.Address.StreetKind.Undefined) 
            {
                int cou = 0;
                for (Pullenti.Ner.Token tt = sli[0].BeginToken.Previous; tt != null && (cou < 4); tt = tt.Previous,cou++) 
                {
                    if (tt.WhitespacesAfterCount > 2) 
                        break;
                    Pullenti.Ner.Token te = Pullenti.Ner.Geo.Internal.MiscLocationHelper.CheckTerritory(tt);
                    if (te != null && te.Next == sli[0].BeginToken) 
                    {
                        street.AddTyp("территория");
                        street.Kind = Pullenti.Ner.Address.StreetKind.Area;
                        t00 = tt;
                        break;
                    }
                }
            }
            return new AddressItemToken(AddressItemType.Street, t00, sli[sli.Count - 1].EndToken) { Referent = street, IsDoubt = doubt };
        }
        static AddressItemToken _tryParseFix(List<StreetItemToken> sits)
        {
            if ((sits.Count == 2 && sits[0].Typ == StreetItemType.Noun && sits[1].Typ == StreetItemType.Fix) && sits[1].City != null) 
            {
                Pullenti.Ner.Address.StreetReferent str = new Pullenti.Ner.Address.StreetReferent();
                str.AddTyp(sits[0].Termin.CanonicText.ToLower());
                if (sits[0].AltTermin != null) 
                    str.AddTyp(sits[0].AltTermin.CanonicText.ToLower());
                foreach (Pullenti.Ner.Slot s in sits[1].City.Slots) 
                {
                    if (s.TypeName == Pullenti.Ner.Geo.GeoReferent.ATTR_NAME) 
                        str.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NAME, s.Value, false, 0);
                    else if (s.TypeName == Pullenti.Ner.Geo.GeoReferent.ATTR_TYPE || s.TypeName == Pullenti.Ner.Geo.GeoReferent.ATTR_MISC) 
                        str.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_MISC, s.Value, false, 0);
                }
                return new AddressItemToken(AddressItemType.Street, sits[0].BeginToken, sits[1].EndToken) { Referent = str };
            }
            if (sits.Count < 1) 
                return null;
            if ((sits.Count == 2 && !sits[0].IsRoad && sits[0].Typ == StreetItemType.Noun) && sits[1].Org != null) 
            {
                if (sits[0].Termin.CanonicText == "ПЛОЩАДЬ" && !Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(sits[0])) 
                    return null;
                Pullenti.Ner.Geo.Internal.OrgItemToken o = sits[1].Org;
                Pullenti.Ner.Address.StreetReferent str = new Pullenti.Ner.Address.StreetReferent();
                str.AddTyp(sits[0].Termin.CanonicText.ToLower());
                bool noOrg = false;
                foreach (Pullenti.Ner.Slot s in o.Referent.Slots) 
                {
                    if (s.TypeName == "NAME" || s.TypeName == "NUMBER") 
                        str.AddSlot(s.TypeName, s.Value, false, 0);
                    else if (s.TypeName == "TYPE") 
                    {
                        string ty = s.Value as string;
                        if (ty == "кадастровый квартал") 
                        {
                            noOrg = true;
                            str.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_TYPE, null, true, 0);
                            str.AddTyp(ty);
                            continue;
                        }
                        if (ty == "владение" || ty == "участок") 
                            noOrg = true;
                        str.AddMisc(ty);
                    }
                }
                if (StreetItemToken._isRegion(sits[0].Termin.CanonicText)) 
                    str.Kind = Pullenti.Ner.Address.StreetKind.Area;
                AddressItemToken re = new AddressItemToken(AddressItemType.Street, sits[0].BeginToken, sits[1].EndToken);
                re.Referent = str;
                return re;
            }
            if (sits[0].Org != null) 
            {
                Pullenti.Ner.Geo.Internal.OrgItemToken o = sits[0].Org;
                Pullenti.Ner.Address.StreetReferent str = new Pullenti.Ner.Address.StreetReferent();
                str.AddTyp("территория");
                bool noOrg = o.NotOrg;
                foreach (Pullenti.Ner.Slot s in o.Referent.Slots) 
                {
                    if (s.TypeName == "NAME" || s.TypeName == "NUMBER") 
                        str.AddSlot(s.TypeName, s.Value, false, 0);
                    else if (s.TypeName == "TYPE") 
                    {
                        string ty = s.Value as string;
                        if (ty == "кадастровый квартал") 
                        {
                            noOrg = true;
                            str.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_TYPE, null, true, 0);
                            str.AddTyp(ty);
                            continue;
                        }
                        if (ty == "владение" || ty == "участок") 
                            noOrg = true;
                        str.AddMisc(ty);
                    }
                }
                Pullenti.Ner.Token b = sits[0].BeginToken;
                Pullenti.Ner.Token e = sits[0].EndToken;
                if (sits.Count == 2 && sits[1].Typ == StreetItemType.Noun) 
                {
                    if (AddressItemToken.CheckStreetAfter(e.Next, false)) 
                    {
                    }
                    else 
                    {
                        str.Kind = Pullenti.Ner.Address.StreetKind.Undefined;
                        str.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_TYPE, null, true, 0);
                        str.AddTyp(sits[1].Termin.CanonicText.ToLower());
                        if (sits[1].AltTermin != null) 
                            str.AddTyp(sits[1].AltTermin.CanonicText.ToLower());
                        e = sits[1].EndToken;
                        if (str.FindSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NAME, null, true) == null && str.FindSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NUMBER, null, true) == null) 
                        {
                            Pullenti.Ner.Slot mi = str.FindSlot(Pullenti.Ner.Address.StreetReferent.ATTR_MISC, null, true);
                            if (mi != null) 
                            {
                                str.Slots.Remove(mi);
                                str.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NAME, (mi.Value as string).ToUpper(), false, 0);
                            }
                        }
                        return new AddressItemToken(AddressItemType.Street, b, e) { Referent = str };
                    }
                }
                if (noOrg || o.Referent.FindSlot("TYPE", null, true) == null) 
                    str.Kind = Pullenti.Ner.Address.StreetKind.Area;
                else 
                {
                    str.Kind = Pullenti.Ner.Address.StreetKind.Org;
                    str.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_REF, o.Referent, false, 0);
                    str.AddExtReferent(sits[0].Org);
                }
                if (sits[0].LengthChar > 500) 
                {
                }
                AddressItemToken re = new AddressItemToken(AddressItemType.Street, b, e);
                re.Referent = str;
                if (o.NotOrg) 
                    str.Kind = Pullenti.Ner.Address.StreetKind.Area;
                re.RefToken = o;
                re.RefTokenIsGsk = o.IsGsk || o.HasTerrKeyword;
                re.RefTokenIsMassive = o.NotOrg;
                re.IsDoubt = o.IsDoubt;
                if (!o.IsGsk && !o.HasTerrKeyword) 
                {
                    if (!AddressItemToken.CheckHouseAfter(sits[0].EndToken.Next, false, false)) 
                    {
                        if (!Pullenti.Ner.Geo.Internal.MiscLocationHelper.IsUserParamAddress(sits[0])) 
                            re.IsDoubt = true;
                    }
                }
                return re;
            }
            if (sits[0].IsRailway) 
            {
                Pullenti.Ner.Address.StreetReferent str = new Pullenti.Ner.Address.StreetReferent();
                str.Kind = Pullenti.Ner.Address.StreetKind.Railway;
                str.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_TYPE, "железная дорога", false, 0);
                str.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NAME, sits[0].Value.Replace(" ЖЕЛЕЗНАЯ ДОРОГА", ""), false, 0);
                Pullenti.Ner.Token t0 = sits[0].BeginToken;
                Pullenti.Ner.Token t1 = sits[0].EndToken;
                if (sits.Count > 1 && sits[1].Typ == StreetItemType.Number) 
                {
                    string num = sits[1].Value;
                    if (t0.Previous != null && ((t0.Previous.IsValue("КИЛОМЕТР", null) || t0.Previous.IsValue("КМ", null)))) 
                    {
                        t0 = t0.Previous;
                        str.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NUMBER, num + "км", false, 0);
                        t1 = sits[1].EndToken;
                    }
                    else if (sits[1].IsNumberKm) 
                    {
                        str.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NUMBER, num + "км", false, 0);
                        t1 = sits[1].EndToken;
                    }
                }
                else if (sits[0].NounIsDoubtCoef > 1) 
                    return null;
                return new AddressItemToken(AddressItemType.Street, t0, t1) { Referent = str };
            }
            if (sits[0].Termin == null) 
                return null;
            if (sits[0].Termin.Acronym == "МКАД") 
            {
                Pullenti.Ner.Address.StreetReferent str = new Pullenti.Ner.Address.StreetReferent();
                str.Kind = Pullenti.Ner.Address.StreetKind.Road;
                str.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_TYPE, "автодорога", false, 0);
                str.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NAME, "МОСКОВСКАЯ КОЛЬЦЕВАЯ", false, 0);
                Pullenti.Ner.Token t0 = sits[0].BeginToken;
                Pullenti.Ner.Token t1 = sits[0].EndToken;
                if (sits.Count > 1 && sits[1].Typ == StreetItemType.Number) 
                {
                    string num = sits[1].Value;
                    if (t0.Previous != null && ((t0.Previous.IsValue("КИЛОМЕТР", null) || t0.Previous.IsValue("КМ", null)))) 
                    {
                        t0 = t0.Previous;
                        str.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NUMBER, num + "км", false, 0);
                        t1 = sits[1].EndToken;
                    }
                    else if (sits[1].IsNumberKm) 
                    {
                        str.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NUMBER, num + "км", false, 0);
                        t1 = sits[1].EndToken;
                    }
                }
                return new AddressItemToken(AddressItemType.Street, t0, t1) { Referent = str };
            }
            if (Pullenti.Ner.Geo.Internal.MiscLocationHelper.CheckGeoObjectBefore(sits[0].BeginToken, false) || AddressItemToken.CheckHouseAfter(sits[0].EndToken.Next, false, true)) 
            {
                Pullenti.Ner.Address.StreetReferent str = new Pullenti.Ner.Address.StreetReferent();
                str.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NAME, sits[0].Termin.CanonicText, false, 0);
                return new AddressItemToken(AddressItemType.Street, sits[0].BeginToken, sits[0].EndToken) { Referent = str };
            }
            return null;
        }
        internal static AddressItemToken TryParseSecondStreet(Pullenti.Ner.Token t1, Pullenti.Ner.Token t2)
        {
            List<StreetItemToken> sli = StreetItemToken.TryParseList(t1, 10, null);
            if (sli == null || (sli.Count < 1) || sli[0].Typ != StreetItemType.Noun) 
                return null;
            List<StreetItemToken> sli2 = StreetItemToken.TryParseList(t2, 10, null);
            if (sli2 == null || sli2.Count == 0) 
                return null;
            sli2.Insert(0, sli[0]);
            AddressItemToken res = TryParseStreet(sli2, true, false, false, null);
            if (res == null) 
                return null;
            res.BeginToken = sli2[1].BeginToken;
            return res;
        }
    }
}