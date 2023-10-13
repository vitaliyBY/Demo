/*
 * SDK Pullenti Address, version 4.20, august 2023. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Text;

namespace Pullenti.Ner.Chat.Internal
{
    class ChatItemToken : Pullenti.Ner.MetaToken
    {
        public ChatItemToken(Pullenti.Ner.Token b, Pullenti.Ner.Token e) : base(b, e, null)
        {
        }
        public bool Not;
        public Pullenti.Ner.Chat.ChatType Typ = Pullenti.Ner.Chat.ChatType.Undefined;
        public Pullenti.Ner.Chat.VerbType VTyp = Pullenti.Ner.Chat.VerbType.Undefined;
        public string Value;
        public override string ToString()
        {
            StringBuilder tmp = new StringBuilder();
            tmp.Append(Typ.ToString());
            if (Not) 
                tmp.Append(" not");
            if (Value != null) 
                tmp.AppendFormat(" {0}", Value);
            if (VTyp != Pullenti.Ner.Chat.VerbType.Undefined) 
                tmp.AppendFormat(" [{0}]", VTyp.ToString());
            return tmp.ToString();
        }
        static bool _isEmptyToken(Pullenti.Ner.Token t)
        {
            if (!(t is Pullenti.Ner.TextToken)) 
                return false;
            if (t.LengthChar == 1) 
                return true;
            Pullenti.Morph.MorphClass mc = t.GetMorphClassInDictionary();
            if ((((mc.IsMisc || mc.IsAdverb || mc.IsConjunction) || mc.IsPreposition || mc.IsPersonalPronoun) || mc.IsPronoun || mc.IsConjunction) || mc.IsPreposition) 
                return true;
            return false;
        }
        public static ChatItemToken TryParse(Pullenti.Ner.Token t)
        {
            Pullenti.Ner.Core.TerminToken tok = null;
            bool not = false;
            Pullenti.Ner.Token tt;
            Pullenti.Ner.Token t0 = null;
            Pullenti.Ner.Token t1 = null;
            bool hasModal = false;
            DateTime dt0 = DateTime.MinValue;
            DateTime dt1 = DateTime.MinValue;
            for (tt = t; tt != null; tt = tt.Next) 
            {
                if (tt != t && tt.IsNewlineBefore) 
                    break;
                if (tt.IsCharOf(".?!")) 
                    break;
                if (tt.LengthChar == 1) 
                    continue;
                bool ok = false;
                if (tt.GetReferent() is Pullenti.Ner.Date.DateReferent) 
                {
                    Pullenti.Ner.Date.DateReferent dr = tt.GetReferent() as Pullenti.Ner.Date.DateReferent;
                    ok = dr.CalculateDateRange((Pullenti.Ner.ProcessorService.DebugCurrentDateTime == null ? DateTime.Now : Pullenti.Ner.ProcessorService.DebugCurrentDateTime.Value), out dt0, out dt1, 0);
                }
                else if (tt.GetReferent() is Pullenti.Ner.Date.DateRangeReferent) 
                {
                    Pullenti.Ner.Date.DateRangeReferent dr = tt.GetReferent() as Pullenti.Ner.Date.DateRangeReferent;
                    ok = dr.CalculateDateRange((Pullenti.Ner.ProcessorService.DebugCurrentDateTime == null ? DateTime.Now : Pullenti.Ner.ProcessorService.DebugCurrentDateTime.Value), out dt0, out dt1, 0);
                }
                if (ok) 
                {
                    if (dt0 != dt1) 
                    {
                        ChatItemToken res = new ChatItemToken(tt, tt) { Typ = Pullenti.Ner.Chat.ChatType.DateRange };
                        res.Value = string.Format("{0}.{1}.{2}", dt0.Year, dt0.Month.ToString("D02"), dt0.Day.ToString("D02"));
                        if (dt0.Hour > 0 || dt0.Minute > 0) 
                            res.Value = string.Format("{0} {1}:{2}", res.Value, dt0.Hour.ToString("D02"), dt0.Minute.ToString("D02"));
                        res.Value = string.Format("{0} - {1}.{2}.{3}", res.Value, dt1.Year, dt1.Month.ToString("D02"), dt1.Day.ToString("D02"));
                        if (dt1.Hour > 0 || dt1.Minute > 0) 
                            res.Value = string.Format("{0} {1}:{2}", res.Value, dt1.Hour.ToString("D02"), dt1.Minute.ToString("D02"));
                        return res;
                    }
                    else 
                    {
                        ChatItemToken res = new ChatItemToken(tt, tt) { Typ = Pullenti.Ner.Chat.ChatType.Date };
                        res.Value = string.Format("{0}.{1}.{2}", dt0.Year, dt0.Month.ToString("D02"), dt0.Day.ToString("D02"));
                        if (dt0.Hour > 0 || dt0.Minute > 0) 
                            res.Value = string.Format("{0} {1}:{2}", res.Value, dt0.Hour.ToString("D02"), dt0.Minute.ToString("D02"));
                        return res;
                    }
                }
                if (!(tt is Pullenti.Ner.TextToken)) 
                    break;
                tok = m_Ontology.TryParse(tt, Pullenti.Ner.Core.TerminParseAttr.No);
                if (tok != null) 
                    break;
                Pullenti.Morph.MorphClass mc = tt.GetMorphClassInDictionary();
                string term = (tt as Pullenti.Ner.TextToken).Term;
                if (term == "НЕ") 
                {
                    not = true;
                    if (t0 == null) 
                        t0 = tt;
                    continue;
                }
                if ((mc.IsPersonalPronoun || mc.IsPronoun || mc.IsConjunction) || mc.IsPreposition) 
                    continue;
                if (tt.IsValue("ХОТЕТЬ", null) || tt.IsValue("ЖЕЛАТЬ", null) || tt.IsValue("МОЧЬ", null)) 
                {
                    hasModal = true;
                    if (t0 == null) 
                        t0 = tt;
                    t1 = tt;
                    continue;
                }
                if (mc.IsAdverb || mc.IsMisc) 
                    continue;
                if (mc.IsVerb) 
                {
                    ChatItemToken res = new ChatItemToken(tt, tt);
                    res.Typ = Pullenti.Ner.Chat.ChatType.Verb;
                    res.Value = (tt as Pullenti.Ner.TextToken).Lemma;
                    if (not) 
                        res.Not = true;
                    if (t0 != null) 
                        res.BeginToken = t0;
                    return res;
                }
            }
            if (tok != null) 
            {
                ChatItemToken res = new ChatItemToken(tok.BeginToken, tok.EndToken);
                res.Typ = (Pullenti.Ner.Chat.ChatType)tok.Termin.Tag;
                if (tok.Termin.Tag2 is Pullenti.Ner.Chat.VerbType) 
                    res.VTyp = (Pullenti.Ner.Chat.VerbType)tok.Termin.Tag2;
                if (res.Typ == Pullenti.Ner.Chat.ChatType.Verb && tok.BeginToken == tok.EndToken && (tok.BeginToken is Pullenti.Ner.TextToken)) 
                    res.Value = (tok.BeginToken as Pullenti.Ner.TextToken).Lemma;
                else 
                    res.Value = Pullenti.Ner.Core.MiscHelper.GetTextValueOfMetaToken(res, Pullenti.Ner.Core.GetTextAttr.No);
                if (not) 
                    res.Not = true;
                if (t0 != null) 
                    res.BeginToken = t0;
                if (res.Typ == Pullenti.Ner.Chat.ChatType.Repeat) 
                {
                    for (tt = tok.EndToken.Next; tt != null; tt = tt.Next) 
                    {
                        if (!(tt is Pullenti.Ner.TextToken)) 
                            break;
                        if (_isEmptyToken(tt)) 
                            continue;
                        Pullenti.Ner.Core.TerminToken tok1 = m_Ontology.TryParse(tt, Pullenti.Ner.Core.TerminParseAttr.No);
                        if (tok1 != null) 
                        {
                            if (((Pullenti.Ner.Chat.ChatType)tok1.Termin.Tag) == Pullenti.Ner.Chat.ChatType.Accept || ((Pullenti.Ner.Chat.ChatType)tok1.Termin.Tag) == Pullenti.Ner.Chat.ChatType.Misc) 
                            {
                                tt = tok1.EndToken;
                                continue;
                            }
                            if (((Pullenti.Ner.Chat.ChatType)tok1.Termin.Tag) == Pullenti.Ner.Chat.ChatType.Repeat) 
                            {
                                tt = (res.EndToken = tok1.EndToken);
                                continue;
                            }
                            break;
                        }
                        Pullenti.Ner.Core.NounPhraseToken npt = Pullenti.Ner.Core.NounPhraseHelper.TryParse(tt, Pullenti.Ner.Core.NounPhraseParseAttr.No, 0, null);
                        if (npt != null) 
                        {
                            if (npt.EndToken.IsValue("ВОПРОС", null) || npt.EndToken.IsValue("ФРАЗА", null) || npt.EndToken.IsValue("ПРЕДЛОЖЕНИЕ", null)) 
                            {
                                tt = (res.EndToken = npt.EndToken);
                                res.Value = npt.GetNormalCaseText(null, Pullenti.Morph.MorphNumber.Undefined, Pullenti.Morph.MorphGender.Undefined, false);
                                continue;
                            }
                        }
                        break;
                    }
                }
                return res;
            }
            if (not && hasModal) 
            {
                ChatItemToken res = new ChatItemToken(t0, t1);
                res.Typ = Pullenti.Ner.Chat.ChatType.Cancel;
                return res;
            }
            return null;
        }
        static Pullenti.Ner.Core.TerminCollection m_Ontology;
        public static void Initialize()
        {
            if (m_Ontology != null) 
                return;
            m_Ontology = new Pullenti.Ner.Core.TerminCollection();
            Pullenti.Ner.Core.Termin t;
            t = new Pullenti.Ner.Core.Termin("ДА") { Tag = Pullenti.Ner.Chat.ChatType.Accept };
            t.AddVariant("КОНЕЧНО", false);
            t.AddVariant("РАЗУМЕЕТСЯ", false);
            t.AddVariant("ПОЖАЛУЙСТА", false);
            t.AddVariant("ПОЖАЛУЙ", false);
            t.AddVariant("ПЛИЗ", false);
            t.AddVariant("НЕПРЕМЕННО", false);
            t.AddVariant("ЕСТЬ", false);
            t.AddVariant("АГА", false);
            t.AddVariant("УГУ", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("НЕТ") { Tag = Pullenti.Ner.Chat.ChatType.Cancel };
            t.AddVariant("ДА НЕТ", false);
            t.AddVariant("НИ ЗА ЧТО", false);
            t.AddVariant("НЕ ХОТЕТЬ", false);
            t.AddVariant("ОТСТАТЬ", false);
            t.AddVariant("НИКТО", false);
            t.AddVariant("НИЧТО", false);
            t.AddVariant("НИЧЕГО", false);
            t.AddVariant("НИГДЕ", false);
            t.AddVariant("НИКОГДА", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("СПАСИБО") { Tag = Pullenti.Ner.Chat.ChatType.Thanks };
            t.AddVariant("БЛАГОДАРИТЬ", false);
            t.AddVariant("БЛАГОДАРСТВОВАТЬ", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("НУ") { Tag = Pullenti.Ner.Chat.ChatType.Misc };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПРИВЕТ") { Tag = Pullenti.Ner.Chat.ChatType.Hello };
            foreach (string s in new string[] {"ЗДРАВСТВУЙ", "ЗДРАВСТВУЙТЕ", "ПРИВЕТИК", "ЗДРАВИЯ ЖЕЛАЮ", "ХЭЛЛОУ", "АЛЛЕ", "ХЭЛО", "АЛЛО", "САЛЮТ", "ДОБРЫЙ ДЕНЬ", "ДОБРЫЙ ВЕЧЕР", "ДОБРОЕ УТРО", "ДОБРАЯ НОЧЬ", "ЗДОРОВО"}) 
            {
                t.AddVariant(s, false);
            }
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПОКА") { Tag = Pullenti.Ner.Chat.ChatType.Bye };
            foreach (string s in new string[] {"ДО СВИДАНИЯ", "ДОСВИДАНИЯ", "ПРОЩАЙ", "ПРОЩАЙТЕ", "ПРОЩЕВАЙ", "ХОРОШЕГО ДНЯ", "ХОРОШЕГО ВЕЧЕРА", "ВСЕГО ХОРОШЕГО", "ВСЕГО ДОБРОГО", "ВСЕХ БЛАГ", "СЧАСТЛИВО", "ДО СКОРОЙ ВСТРЕЧИ", "ДО ЗАВТРА", "ДО ВСТРЕЧИ", "СКОРО УВИДИМСЯ", "ПОКЕДА", "ПОКЕДОВА", "ПРОЩАЙ", "ПРОЩАЙТЕ", "ЧАО", "ГУД БАЙ", "ГУДБАЙ", "ЧАО"}) 
            {
                t.AddVariant(s, false);
            }
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ГОВОРИТЬ") { Tag = Pullenti.Ner.Chat.ChatType.Verb, Tag2 = Pullenti.Ner.Chat.VerbType.Say };
            foreach (string s in new string[] {"СКАЗАТЬ", "РАЗГОВАРИВАТЬ", "ПРОИЗНЕСТИ", "ПРОИЗНОСИТЬ", "ОТВЕТИТЬ", "ОТВЕЧАТЬ", "СПРАШИВАТЬ", "СПРОСИТЬ", "ПОТОВОРИТЬ", "ОБЩАТЬСЯ", "ПООБЩАТЬСЯ"}) 
            {
                t.AddVariant(s, false);
            }
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ЗВОНИТЬ") { Tag = Pullenti.Ner.Chat.ChatType.Verb, Tag2 = Pullenti.Ner.Chat.VerbType.Call };
            foreach (string s in new string[] {"ПЕРЕЗВОНИТЬ", "ПОЗВОНИТЬ", "СДЕЛАТЬ ЗВОНОК", "НАБРАТЬ"}) 
            {
                t.AddVariant(s, false);
            }
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("БЫТЬ") { Tag = Pullenti.Ner.Chat.ChatType.Verb, Tag2 = Pullenti.Ner.Chat.VerbType.Be };
            foreach (string s in new string[] {"ЯВЛЯТЬСЯ"}) 
            {
                t.AddVariant(s, false);
            }
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ИМЕТЬ") { Tag = Pullenti.Ner.Chat.ChatType.Verb, Tag2 = Pullenti.Ner.Chat.VerbType.Have };
            foreach (string s in new string[] {"ОБЛАДАТЬ", "ВЛАДЕТЬ"}) 
            {
                t.AddVariant(s, false);
            }
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПОЗЖЕ") { Tag = Pullenti.Ner.Chat.ChatType.Later };
            foreach (string s in new string[] {"ПОПОЗЖЕ", "ПОЗДНЕЕ", "ПОТОМ", "НЕКОГДА"}) 
            {
                t.AddVariant(s, false);
            }
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ЗАНЯТ") { Tag = Pullenti.Ner.Chat.ChatType.Busy };
            foreach (string s in new string[] {"НЕУДОБНО", "НЕ УДОБНО", "НЕТ ВРЕМЕНИ", "ПАРАЛЛЕЛЬНЫЙ ЗВОНОК", "СОВЕЩАНИЕ", "ОБЕД", "ТРАНСПОРТ", "МЕТРО"}) 
            {
                t.AddVariant(s, false);
            }
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПОВТОРИТЬ") { Tag = Pullenti.Ner.Chat.ChatType.Repeat };
            t.AddVariant("НЕ РАССЛЫШАТЬ", false);
            t.AddVariant("НЕ УСЛЫШАТЬ", false);
            t.AddVariant("ПЛОХО СЛЫШНО", false);
            t.AddVariant("ПЛОХАЯ СВЯЗЬ", false);
            m_Ontology.Add(t);
        }
    }
}