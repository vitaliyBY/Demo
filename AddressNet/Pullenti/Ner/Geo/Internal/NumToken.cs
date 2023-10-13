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
    class NumToken : Pullenti.Ner.MetaToken
    {
        public NumToken(Pullenti.Ner.Token b, Pullenti.Ner.Token e) : base(b, e, null)
        {
        }
        public string Value;
        public string AltValue;
        public bool HasPrefix;
        public bool HasSpecWord;
        public bool IsCadasterNumber;
        public string Template;
        public override string ToString()
        {
            StringBuilder res = new StringBuilder();
            if (HasPrefix) 
                res.Append("№ ");
            res.Append(Value);
            if (AltValue != null) 
                res.AppendFormat(" / {0}", AltValue);
            if (IsCadasterNumber) 
                res.Append(" cadaster");
            return res.ToString();
        }
        public static NumToken TryParse(Pullenti.Ner.Token t, GeoTokenType typ)
        {
            if (t == null) 
                return null;
            if (t.IsValue("ОТДЕЛЕНИЕ", null) && (t.WhitespacesAfterCount < 3)) 
            {
                NumToken next = TryParse(t.Next, typ);
                if (next != null) 
                {
                    next.BeginToken = t;
                    next.HasPrefix = true;
                    next.HasSpecWord = true;
                    return next;
                }
            }
            Pullenti.Ner.Token tt = Pullenti.Ner.Core.MiscHelper.CheckNumberPrefix(t);
            if (tt == null && (t is Pullenti.Ner.TextToken) && (t as Pullenti.Ner.TextToken).Term.StartsWith("КАД")) 
            {
                Pullenti.Ner.Token tt1 = t.Next;
                if (tt1 != null && tt1.IsChar('.')) 
                    tt1 = tt1.Next;
                tt = Pullenti.Ner.Core.MiscHelper.CheckNumberPrefix(tt1);
                if (tt == null) 
                    tt = tt1;
            }
            if (tt != null) 
            {
                bool hasReest = false;
                for (Pullenti.Ner.Token ttt = tt; ttt != null; ttt = ttt.Next) 
                {
                    if (((ttt.IsCharOf(":") || ttt.IsHiphen)) && (ttt.Next is Pullenti.Ner.NumberToken)) 
                        continue;
                    if (hasReest) 
                    {
                        if (ttt.GetReferent() is Pullenti.Ner.Geo.GeoReferent) 
                            continue;
                    }
                    TerrItemToken ter = TerrItemToken.TryParse(ttt, null, null);
                    if (ter != null && ((ter.OntoItem != null || ter.TerminItem != null))) 
                    {
                        ttt = ter.EndToken;
                        continue;
                    }
                    Pullenti.Ner.Core.NounPhraseToken npt = Pullenti.Ner.Core.NounPhraseHelper.TryParse(ttt, Pullenti.Ner.Core.NounPhraseParseAttr.ParsePreposition, 0, null);
                    if (npt != null) 
                    {
                        if (npt.EndToken.IsValue("ЗАПИСЬ", null) || npt.EndToken.IsValue("РЕЕСТР", null)) 
                        {
                            ttt = npt.EndToken;
                            hasReest = true;
                            continue;
                        }
                        break;
                    }
                    if (!(ttt is Pullenti.Ner.NumberToken)) 
                        break;
                    NumToken res = new NumToken(t, ttt) { HasPrefix = true, Template = "n" };
                    res.Value = (ttt as Pullenti.Ner.NumberToken).Value;
                    res._correct(typ);
                    return res;
                }
            }
            if (tt == null) 
                tt = t;
            if (tt is Pullenti.Ner.NumberToken) 
            {
                NumToken res = new NumToken(t, tt) { HasPrefix = tt != t, Template = "n" };
                res.Value = (tt as Pullenti.Ner.NumberToken).Value;
                res._correct(typ);
                return res;
            }
            if ((tt is Pullenti.Ner.ReferentToken) && (tt.GetReferent() is Pullenti.Ner.Uri.UriReferent)) 
            {
                NumToken res = new NumToken(t, tt) { HasPrefix = tt != t };
                res.Value = tt.GetReferent().GetStringValue("VALUE");
                string sh = tt.GetReferent().GetStringValue("SCHEME");
                if (sh == "КАДАСТР") 
                    res.IsCadasterNumber = true;
                return res;
            }
            if ((tt is Pullenti.Ner.TextToken) && !t.IsValue("C", null)) 
            {
                Pullenti.Ner.NumberToken nt = Pullenti.Ner.Core.NumberHelper.TryParseRoman(tt);
                if (nt != null && nt.Value != "100") 
                {
                    NumToken res = new NumToken(t, nt.EndToken) { HasPrefix = tt != t, Template = "l" };
                    res.Value = nt.Value;
                    res._correct(typ);
                    return res;
                }
            }
            if (Pullenti.Ner.Core.BracketHelper.IsBracket(t, true)) 
            {
                NumToken next = TryParse(t.Next, typ);
                if (next != null) 
                {
                    if (next.EndToken.Next != null && Pullenti.Ner.Core.BracketHelper.IsBracket(next.EndToken.Next, true)) 
                    {
                        next.BeginToken = t;
                        next.EndToken = next.EndToken.Next;
                        return next;
                    }
                }
            }
            if (((t is Pullenti.Ner.TextToken) && t.LengthChar == 1 && t.Chars.IsLetter) && t.Chars.IsAllUpper) 
            {
            }
            return null;
        }
        void _correct(GeoTokenType typ)
        {
            Pullenti.Ner.NumberToken nt = EndToken as Pullenti.Ner.NumberToken;
            if ((nt != null && (nt.EndToken is Pullenti.Ner.TextToken) && (nt.EndToken as Pullenti.Ner.TextToken).Term == "Е") && nt.EndToken.Previous == nt.BeginToken && !nt.EndToken.IsWhitespaceBefore) 
                Value += "Е";
            Pullenti.Ner.Token t = EndToken.Next;
            if (t == null || t.WhitespacesBeforeCount > 1) 
                return;
            if (t.IsValue("ОТДЕЛЕНИЕ", null)) 
            {
                EndToken = t;
                t = t.Next;
                HasPrefix = true;
                HasSpecWord = true;
                if (t == null || t.WhitespacesBeforeCount > 1) 
                    return;
            }
            if (t.IsValue2("ОЧЕРЕДЬ", "ОСВОЕНИЕ")) 
            {
                EndToken = t.Next;
                t = t.Next.Next;
                HasPrefix = true;
                HasSpecWord = true;
                if (t == null || t.WhitespacesBeforeCount > 1) 
                    return;
            }
            if ((t.Next != null && t.Next.IsHiphen && t.Next.Next != null) && t.Next.Next.IsValue("Я", null)) 
            {
                EndToken = t.Next.Next;
                t = EndToken.Next;
                if (t == null || t.WhitespacesBeforeCount > 1) 
                    return;
            }
            if ((EndToken is Pullenti.Ner.NumberToken) && EndToken.LengthChar == 2) 
            {
                Pullenti.Ner.Token tt = t;
                if (tt.IsCharOf(".:") && tt.Next != null) 
                    tt = tt.Next;
                if (((tt is Pullenti.Ner.NumberToken) && (tt as Pullenti.Ner.NumberToken).Typ == Pullenti.Ner.NumberSpellingType.Digit && (tt as Pullenti.Ner.NumberToken).LengthChar == 2) && tt.Next != null) 
                {
                    Pullenti.Ner.Token ttt = tt.Next;
                    if (ttt.IsCharOf(".:") && ttt.Next != null) 
                        ttt = ttt.Next;
                    bool isKv = false;
                    for (Pullenti.Ner.Token tt0 = BeginToken.Previous; tt0 != null; tt0 = tt0.Previous) 
                    {
                        if (tt0.IsValue("КВАРТАЛ", null) || tt0.IsValue("КВ", null)) 
                        {
                            isKv = true;
                            break;
                        }
                        else if (tt0.LengthChar > 1 || !(tt0 is Pullenti.Ner.TextToken)) 
                            break;
                    }
                    if ((ttt is Pullenti.Ner.NumberToken) && (((ttt as Pullenti.Ner.NumberToken).LengthChar == 6 || (ttt as Pullenti.Ner.NumberToken).LengthChar == 7)) && (ttt as Pullenti.Ner.NumberToken).Typ == Pullenti.Ner.NumberSpellingType.Digit) 
                    {
                        Value = string.Format("{0}:{1}:{2}", Value, tt.GetSourceText(), ttt.GetSourceText());
                        EndToken = ttt;
                        ttt = ttt.Next;
                        IsCadasterNumber = true;
                        if (ttt != null && ttt.IsCharOf(".:") && (ttt.Next is Pullenti.Ner.NumberToken)) 
                        {
                            Value = string.Format("{0}:{1}", Value, ttt.Next.GetSourceText());
                            EndToken = ttt.Next;
                        }
                    }
                    else if ((ttt is Pullenti.Ner.NumberToken) && isKv) 
                    {
                        StringBuilder tmp = new StringBuilder();
                        tmp.AppendFormat("{0}:{1}:{2}", Value, tt.GetSourceText(), ttt.GetSourceText());
                        Pullenti.Ner.Token t1 = ttt;
                        for (ttt = ttt.Next; ttt != null; ttt = ttt.Next) 
                        {
                            if (ttt is Pullenti.Ner.NumberToken) 
                            {
                                tmp.Append(ttt.GetSourceText());
                                t1 = ttt;
                                continue;
                            }
                            if (ttt.IsChar(':') && (ttt.Next is Pullenti.Ner.NumberToken)) 
                            {
                                tmp.AppendFormat(":{0}", ttt.Next.GetSourceText());
                                t1 = ttt.Next;
                                break;
                            }
                            break;
                        }
                        if (tmp.Length >= 7) 
                        {
                            Value = tmp.ToString();
                            EndToken = t1;
                            IsCadasterNumber = true;
                        }
                    }
                }
            }
            if (IsCadasterNumber) 
                return;
            for (t = EndToken.Next; t != null; t = t.Next) 
            {
                bool isDrob = false;
                bool isHiph = false;
                bool brAfter = false;
                if (t.IsHiphen && !t.IsWhitespaceAfter && t.Next != null) 
                {
                    isHiph = true;
                    t = t.Next;
                }
                else if (t.IsCharOf("\\/") && !t.IsWhitespaceAfter && t.Next != null) 
                {
                    isDrob = true;
                    t = t.Next;
                }
                else if (t.IsValue("ДРОБЬ", null) && t.Next != null) 
                {
                    isDrob = true;
                    t = t.Next;
                }
                else if (Pullenti.Ner.Core.BracketHelper.IsBracket(t, false)) 
                {
                    if ((((t.Next is Pullenti.Ner.NumberToken) || (((t.Next is Pullenti.Ner.TextToken) && t.Next.Chars.IsLetter && t.Next.LengthChar == 1)))) && t.Next.Next != null && Pullenti.Ner.Core.BracketHelper.IsBracket(t.Next.Next, false)) 
                    {
                        t = t.Next;
                        brAfter = true;
                    }
                    else 
                    {
                        Pullenti.Ner.NumberToken lat0 = Pullenti.Ner.Core.NumberHelper.TryParseRoman(t.Next);
                        if (lat0 != null && lat0.EndToken.Next != null && lat0.EndToken.Next.IsChar(')')) 
                        {
                            t = t.Next;
                            brAfter = true;
                        }
                    }
                }
                char templ0 = Template[Template.Length - 1];
                if (t is Pullenti.Ner.NumberToken) 
                {
                    Pullenti.Ner.NumberToken num = t as Pullenti.Ner.NumberToken;
                    if (num.Typ != Pullenti.Ner.NumberSpellingType.Digit) 
                        break;
                    if (isHiph && ((templ0 != 'n' || typ == GeoTokenType.Org))) 
                    {
                    }
                    else if (isDrob || brAfter) 
                    {
                    }
                    else if (!t.IsWhitespaceBefore) 
                    {
                    }
                    else 
                        break;
                    string val = num.Value;
                    if (!num.Morph.Class.IsAdjective) 
                        val = num.GetSourceText();
                    Value = string.Format("{0}{1}{2}", Value, (isDrob || brAfter ? "/" : (templ0 == 'c' ? "" : "-")), val);
                    Template += "n";
                    if (brAfter) 
                        t = t.Next;
                    EndToken = t;
                    continue;
                }
                if (t is Pullenti.Ner.TextToken) 
                {
                    nt = Pullenti.Ner.Core.NumberHelper.TryParseRoman(t);
                    if (nt != null && nt.Value != "100") 
                    {
                        Value = string.Format("{0}{1}{2}", Value, (isDrob || brAfter ? '/' : '-'), nt.Value);
                        Template += "l";
                        t = nt.EndToken;
                        if (brAfter) 
                            t = t.Next;
                        EndToken = t;
                        continue;
                    }
                }
                if (((t is Pullenti.Ner.TextToken) && t.LengthChar == 1 && t.Chars.IsLetter) && ((!t.IsWhitespaceBefore || t.IsNewlineAfter || ((t.Next != null && t.Next.IsComma))))) 
                {
                    if (templ0 == 'n') 
                    {
                    }
                    else if (templ0 == 'l' && ((isHiph || isDrob))) 
                    {
                    }
                    else 
                        break;
                    char ch = Pullenti.Morph.LanguageHelper.GetCyrForLat((t as Pullenti.Ner.TextToken).Term[0]);
                    if (((int)ch) == 0) 
                        ch = (t as Pullenti.Ner.TextToken).Term[0];
                    Value = string.Format("{0}{1}", Value, ch);
                    Template += "c";
                    if (brAfter) 
                        t = t.Next;
                    EndToken = t;
                    continue;
                }
                break;
            }
        }
        public static char _correctChar(char v)
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
        static string _correctCharToken(Pullenti.Ner.Token t)
        {
            Pullenti.Ner.TextToken tt = t as Pullenti.Ner.TextToken;
            if (tt == null) 
                return null;
            string v = tt.Term;
            if (v.Length == 1) 
            {
                char corr = _correctChar(v[0]);
                if (corr != ((char)0)) 
                    return string.Format("{0}", corr);
                if (t.Chars.IsCyrillicLetter) 
                    return v;
            }
            if (v.Length == 2) 
            {
                if (t.Chars.IsCyrillicLetter) 
                    return v;
                char corr = _correctChar(v[0]);
                char corr2 = _correctChar(v[1]);
                if (corr != ((char)0) && corr2 != ((char)0)) 
                    return string.Format("{0}{1}", corr, corr2);
            }
            return null;
        }
    }
}