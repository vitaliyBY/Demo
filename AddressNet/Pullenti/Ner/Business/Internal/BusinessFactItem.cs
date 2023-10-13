/*
 * SDK Pullenti Address, version 4.20, august 2023. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Ner.Business.Internal
{
    class BusinessFactItem : Pullenti.Ner.MetaToken
    {
        public BusinessFactItem(Pullenti.Ner.Token b, Pullenti.Ner.Token e) : base(b, e, null)
        {
        }
        public BusinessFactItemTyp Typ;
        public Pullenti.Ner.Business.BusinessFactKind BaseKind;
        public bool IsBasePassive;
        public static BusinessFactItem TryParse(Pullenti.Ner.Token t)
        {
            if (t == null) 
                return null;
            BusinessFactItem res = _tryParse(t);
            if (res == null) 
                return null;
            for (Pullenti.Ner.Token tt = res.EndToken.Next; tt != null; tt = tt.Next) 
            {
                if (tt.Morph.Class.IsPreposition) 
                    continue;
                if (!(tt is Pullenti.Ner.TextToken)) 
                    break;
                Pullenti.Ner.Core.NounPhraseToken npt = Pullenti.Ner.Core.NounPhraseHelper.TryParse(tt, Pullenti.Ner.Core.NounPhraseParseAttr.No, 0, null);
                if (npt == null) 
                    break;
                BusinessFactItem rr = _tryParse(tt);
                if (rr != null) 
                {
                    if (rr.BaseKind == res.BaseKind) 
                    {
                    }
                    else if (rr.BaseKind == Pullenti.Ner.Business.BusinessFactKind.Get && res.BaseKind == Pullenti.Ner.Business.BusinessFactKind.Finance) 
                        res.BaseKind = rr.BaseKind;
                    else 
                        break;
                    tt = (res.EndToken = rr.EndToken);
                    continue;
                }
                if ((res.BaseKind == Pullenti.Ner.Business.BusinessFactKind.Finance || npt.Noun.IsValue("РЫНОК", null) || npt.Noun.IsValue("СДЕЛКА", null)) || npt.Noun.IsValue("РИНОК", null) || npt.Noun.IsValue("УГОДА", null)) 
                {
                    res.EndToken = tt;
                    continue;
                }
                break;
            }
            return res;
        }
        static BusinessFactItem _tryParse(Pullenti.Ner.Token t)
        {
            Pullenti.Ner.Core.TerminToken tok = m_BaseOnto.TryParse(t, Pullenti.Ner.Core.TerminParseAttr.No);
            if (tok == null && t.Morph.Class.IsVerb && t.Next != null) 
                tok = m_BaseOnto.TryParse(t.Next, Pullenti.Ner.Core.TerminParseAttr.No);
            if (tok != null) 
            {
                Pullenti.Ner.Business.BusinessFactKind ki = (Pullenti.Ner.Business.BusinessFactKind)tok.Termin.Tag;
                if (ki != Pullenti.Ner.Business.BusinessFactKind.Undefined) 
                    return new BusinessFactItem(t, tok.EndToken) { Typ = BusinessFactItemTyp.Base, BaseKind = ki, Morph = tok.Morph, IsBasePassive = tok.Termin.Tag2 != null };
                for (Pullenti.Ner.Token tt = tok.EndToken.Next; tt != null; tt = tt.Next) 
                {
                    if (tt.Morph.Class.IsPreposition) 
                        continue;
                    tok = m_BaseOnto.TryParse(tt, Pullenti.Ner.Core.TerminParseAttr.No);
                    if (tok == null) 
                        continue;
                    ki = (Pullenti.Ner.Business.BusinessFactKind)tok.Termin.Tag;
                    if (ki != Pullenti.Ner.Business.BusinessFactKind.Undefined) 
                        return new BusinessFactItem(t, tok.EndToken) { Typ = BusinessFactItemTyp.Base, BaseKind = ki, Morph = tok.Morph };
                    tt = tok.EndToken;
                }
            }
            Pullenti.Ner.Core.NounPhraseToken npt = Pullenti.Ner.Core.NounPhraseHelper.TryParse(t, Pullenti.Ner.Core.NounPhraseParseAttr.No, 0, null);
            if (npt != null) 
            {
                if (((((npt.Noun.IsValue("АКЦИОНЕР", null) || npt.Noun.IsValue("ВЛАДЕЛЕЦ", null) || npt.Noun.IsValue("ВЛАДЕЛИЦА", null)) || npt.Noun.IsValue("СОВЛАДЕЛЕЦ", null) || npt.Noun.IsValue("СОВЛАДЕЛИЦА", null)) || npt.Noun.IsValue("АКЦІОНЕР", null) || npt.Noun.IsValue("ВЛАСНИК", null)) || npt.Noun.IsValue("ВЛАСНИЦЯ", null) || npt.Noun.IsValue("СПІВВЛАСНИК", null)) || npt.Noun.IsValue("СПІВВЛАСНИЦЯ", null)) 
                    return new BusinessFactItem(t, npt.EndToken) { Typ = BusinessFactItemTyp.Base, BaseKind = Pullenti.Ner.Business.BusinessFactKind.Have, Morph = npt.Morph };
            }
            if (npt != null) 
            {
                if ((npt.Noun.IsValue("ОСНОВАТЕЛЬ", null) || npt.Noun.IsValue("ОСНОВАТЕЛЬНИЦА", null) || npt.Noun.IsValue("ЗАСНОВНИК", null)) || npt.Noun.IsValue("ЗАСНОВНИЦЯ", null)) 
                    return new BusinessFactItem(t, npt.EndToken) { Typ = BusinessFactItemTyp.Base, BaseKind = Pullenti.Ner.Business.BusinessFactKind.Create, Morph = npt.Morph };
            }
            return null;
        }
        public static void Initialize()
        {
            if (m_BaseOnto != null) 
                return;
            m_BaseOnto = new Pullenti.Ner.Core.TerminCollection();
            foreach (string s in new string[] {"КУПИТЬ", "ПОКУПАТЬ", "ПРИОБРЕТАТЬ", "ПРИОБРЕСТИ", "ПОКУПКА", "ПРИОБРЕТЕНИЕ"}) 
            {
                m_BaseOnto.Add(new Pullenti.Ner.Core.Termin(s) { Tag = Pullenti.Ner.Business.BusinessFactKind.Get });
            }
            foreach (string s in new string[] {"КУПИТИ", "КУПУВАТИ", "КУПУВАТИ", "ПРИДБАТИ", "ПОКУПКА", "ПРИДБАННЯ"}) 
            {
                m_BaseOnto.Add(new Pullenti.Ner.Core.Termin(s) { Tag = Pullenti.Ner.Business.BusinessFactKind.Get, Lang = Pullenti.Morph.MorphLang.UA });
            }
            foreach (string s in new string[] {"ПРОДАТЬ", "ПРОДАВАТЬ", "ПРОДАЖА"}) 
            {
                m_BaseOnto.Add(new Pullenti.Ner.Core.Termin(s) { Tag = Pullenti.Ner.Business.BusinessFactKind.Sell });
            }
            foreach (string s in new string[] {"ПРОДАТИ", "ПРОДАВАТИ", "ПРОДАЖ"}) 
            {
                m_BaseOnto.Add(new Pullenti.Ner.Core.Termin(s) { Tag = Pullenti.Ner.Business.BusinessFactKind.Sell, Lang = Pullenti.Morph.MorphLang.UA });
            }
            foreach (string s in new string[] {"ФИНАНСИРОВАТЬ", "СПОНСИРОВАТЬ", "ПРОФИНАНСИРОВАТЬ"}) 
            {
                m_BaseOnto.Add(new Pullenti.Ner.Core.Termin(s) { Tag = Pullenti.Ner.Business.BusinessFactKind.Finance });
            }
            foreach (string s in new string[] {"ФІНАНСУВАТИ", "СПОНСОРУВАТИ", "ПРОФІНАНСУВАТИ"}) 
            {
                m_BaseOnto.Add(new Pullenti.Ner.Core.Termin(s) { Tag = Pullenti.Ner.Business.BusinessFactKind.Finance, Lang = Pullenti.Morph.MorphLang.UA });
            }
            foreach (string s in new string[] {"ВЛАДЕТЬ", "РАСПОРЯЖАТЬСЯ", "КОНТРОЛИРОВАТЬ", "ПРИНАДЛЕЖАТЬ", "СТАТЬ ВЛАДЕЛЬЦЕМ", "КОНСОЛИДИРОВАТЬ"}) 
            {
                m_BaseOnto.Add(new Pullenti.Ner.Core.Termin(s) { Tag = Pullenti.Ner.Business.BusinessFactKind.Have });
            }
            foreach (string s in new string[] {"ВОЛОДІТИ", "РОЗПОРЯДЖАТИСЯ", "КОНТРОЛЮВАТИ", "НАЛЕЖАТИ", "СТАТИ ВЛАСНИКОМ", "КОНСОЛІДУВАТИ"}) 
            {
                m_BaseOnto.Add(new Pullenti.Ner.Core.Termin(s) { Tag = Pullenti.Ner.Business.BusinessFactKind.Have, Lang = Pullenti.Morph.MorphLang.UA });
            }
            foreach (string s in new string[] {"ПРИНАДЛЕЖАЩИЙ", "КОНТРОЛИРУЕМЫЙ", "ВЛАДЕЕМЫЙ", "ПЕРЕЙТИ ПОД КОНТРОЛЬ"}) 
            {
                m_BaseOnto.Add(new Pullenti.Ner.Core.Termin(s) { Tag = Pullenti.Ner.Business.BusinessFactKind.Have, Tag2 = s });
            }
            foreach (string s in new string[] {"НАЛЕЖНИЙ", "КОНТРОЛЬОВАНИЙ", "ВЛАДЕЕМЫЙ", "ПЕРЕЙТИ ПІД КОНТРОЛЬ"}) 
            {
                m_BaseOnto.Add(new Pullenti.Ner.Core.Termin(s) { Tag = Pullenti.Ner.Business.BusinessFactKind.Have, Tag2 = s, Lang = Pullenti.Morph.MorphLang.UA });
            }
            foreach (string s in new string[] {"ЗАКРЫТЬ СДЕЛКУ", "СОВЕРШИТЬ СДЕЛКУ", "ЗАВЕРШИТЬ СДЕЛКУ", "ЗАКЛЮЧИТЬ"}) 
            {
                m_BaseOnto.Add(new Pullenti.Ner.Core.Termin(s) { Tag = Pullenti.Ner.Business.BusinessFactKind.Undefined });
            }
            foreach (string s in new string[] {"ЗАКРИТИ ОПЕРАЦІЮ", "ЗДІЙСНИТИ ОПЕРАЦІЮ", "ЗАВЕРШИТИ ОПЕРАЦІЮ", "УКЛАСТИ"}) 
            {
                m_BaseOnto.Add(new Pullenti.Ner.Core.Termin(s) { Tag = Pullenti.Ner.Business.BusinessFactKind.Undefined, Lang = Pullenti.Morph.MorphLang.UA });
            }
            foreach (string s in new string[] {"ДОХОД", "ПРИБЫЛЬ", "ВЫРУЧКА"}) 
            {
                m_BaseOnto.Add(new Pullenti.Ner.Core.Termin(s) { Tag = Pullenti.Ner.Business.BusinessFactKind.Profit });
            }
            foreach (string s in new string[] {"ДОХІД", "ПРИБУТОК", "ВИРУЧКА"}) 
            {
                m_BaseOnto.Add(new Pullenti.Ner.Core.Termin(s) { Tag = Pullenti.Ner.Business.BusinessFactKind.Profit, Lang = Pullenti.Morph.MorphLang.UA });
            }
            foreach (string s in new string[] {"УБЫТОК"}) 
            {
                m_BaseOnto.Add(new Pullenti.Ner.Core.Termin(s) { Tag = Pullenti.Ner.Business.BusinessFactKind.Damages });
            }
            foreach (string s in new string[] {"ЗБИТОК"}) 
            {
                m_BaseOnto.Add(new Pullenti.Ner.Core.Termin(s) { Tag = Pullenti.Ner.Business.BusinessFactKind.Damages, Lang = Pullenti.Morph.MorphLang.UA });
            }
            foreach (string s in new string[] {"СОГЛАШЕНИЕ", "ДОГОВОР"}) 
            {
                m_BaseOnto.Add(new Pullenti.Ner.Core.Termin(s) { Tag = Pullenti.Ner.Business.BusinessFactKind.Agreement });
            }
            foreach (string s in new string[] {"УГОДА", "ДОГОВІР"}) 
            {
                m_BaseOnto.Add(new Pullenti.Ner.Core.Termin(s) { Tag = Pullenti.Ner.Business.BusinessFactKind.Agreement, Lang = Pullenti.Morph.MorphLang.UA });
            }
            foreach (string s in new string[] {"ИСК", "СУДЕБНЫЙ ИСК"}) 
            {
                m_BaseOnto.Add(new Pullenti.Ner.Core.Termin(s) { Tag = Pullenti.Ner.Business.BusinessFactKind.Lawsuit });
            }
            foreach (string s in new string[] {"ПОЗОВ", "СУДОВИЙ ПОЗОВ"}) 
            {
                m_BaseOnto.Add(new Pullenti.Ner.Core.Termin(s) { Tag = Pullenti.Ner.Business.BusinessFactKind.Lawsuit, Lang = Pullenti.Morph.MorphLang.UA });
            }
            foreach (string s in new string[] {"ДОЧЕРНЕЕ ПРЕДПРИЯТИЕ", "ДОЧЕРНЕЕ ПОДРАЗДЕЛЕНИЕ", "ДОЧЕРНЯЯ КОМПАНИЯ"}) 
            {
                m_BaseOnto.Add(new Pullenti.Ner.Core.Termin(s) { Tag = Pullenti.Ner.Business.BusinessFactKind.Subsidiary });
            }
            foreach (string s in new string[] {"ДОЧІРНЄ ПІДПРИЄМСТВО", "ДОЧІРНІЙ ПІДРОЗДІЛ", "ДОЧІРНЯ КОМПАНІЯ"}) 
            {
                m_BaseOnto.Add(new Pullenti.Ner.Core.Termin(s) { Tag = Pullenti.Ner.Business.BusinessFactKind.Subsidiary, Lang = Pullenti.Morph.MorphLang.UA });
            }
        }
        static Pullenti.Ner.Core.TerminCollection m_BaseOnto;
    }
}