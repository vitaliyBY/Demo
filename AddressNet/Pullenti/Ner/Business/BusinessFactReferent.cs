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

namespace Pullenti.Ner.Business
{
    /// <summary>
    /// Сущность для бизнес-факта
    /// </summary>
    public class BusinessFactReferent : Pullenti.Ner.Referent
    {
        public BusinessFactReferent() : base(OBJ_TYPENAME)
        {
            InstanceOf = Pullenti.Ner.Business.Internal.MetaBusinessFact.GlobalMeta;
        }
        /// <summary>
        /// Имя типа сущности TypeName ("BUSINESSFACT")
        /// </summary>
        public const string OBJ_TYPENAME = "BUSINESSFACT";
        /// <summary>
        /// Имя атрибута - класс (BusinessFactKind)
        /// </summary>
        public const string ATTR_KIND = "KIND";
        /// <summary>
        /// Имя атрибута - тип
        /// </summary>
        public const string ATTR_TYPE = "TYPE";
        /// <summary>
        /// Имя атрибута - кто
        /// </summary>
        public const string ATTR_WHO = "WHO";
        /// <summary>
        /// Имя атрибута - кого
        /// </summary>
        public const string ATTR_WHOM = "WHOM";
        /// <summary>
        /// Имя атрибута - когда
        /// </summary>
        public const string ATTR_WHEN = "WHEN";
        /// <summary>
        /// Имя атрибута - что
        /// </summary>
        public const string ATTR_WHAT = "WHAT";
        /// <summary>
        /// Имя атрибута - разное
        /// </summary>
        public const string ATTR_MISC = "MISC";
        /// <summary>
        /// Классификатор бизнес-факта
        /// </summary>
        public BusinessFactKind Kind
        {
            get
            {
                string s = this.GetStringValue(ATTR_KIND);
                if (s == null) 
                    return BusinessFactKind.Undefined;
                try 
                {
                    object res = Enum.Parse(typeof(BusinessFactKind), s, true);
                    if (res is BusinessFactKind) 
                        return (BusinessFactKind)res;
                }
                catch(Exception ex762) 
                {
                }
                return BusinessFactKind.Undefined;
            }
            set
            {
                if (value != BusinessFactKind.Undefined) 
                    this.AddSlot(ATTR_KIND, value.ToString(), true, 0);
            }
        }
        /// <summary>
        /// Краткое описание факта (тип)
        /// </summary>
        public string Typ
        {
            get
            {
                string typ = this.GetStringValue(ATTR_TYPE);
                if (typ != null) 
                    return typ;
                string kind = this.GetStringValue(ATTR_KIND);
                if (kind != null) 
                    typ = Pullenti.Ner.Business.Internal.MetaBusinessFact.GlobalMeta.KindFeature.ConvertInnerValueToOuterValue(kind, null) as string;
                if (typ != null) 
                    return typ.ToLower();
                return null;
            }
            set
            {
                this.AddSlot(ATTR_TYPE, value, true, 0);
            }
        }
        /// <summary>
        /// Кто (действительный залог)
        /// </summary>
        public Pullenti.Ner.Referent Who
        {
            get
            {
                return this.GetSlotValue(ATTR_WHO) as Pullenti.Ner.Referent;
            }
            set
            {
                this.AddSlot(ATTR_WHO, value, true, 0);
            }
        }
        /// <summary>
        /// Второй "Кто" (действительный залог)
        /// </summary>
        public Pullenti.Ner.Referent Who2
        {
            get
            {
                int i = 2;
                foreach (Pullenti.Ner.Slot s in Slots) 
                {
                    if (s.TypeName == ATTR_WHO) 
                    {
                        if ((--i) == 0) 
                            return s.Value as Pullenti.Ner.Referent;
                    }
                }
                return null;
            }
            set
            {
                this.AddSlot(ATTR_WHO, value, false, 0);
            }
        }
        /// <summary>
        /// Кого (страдательный залог)
        /// </summary>
        public Pullenti.Ner.Referent Whom
        {
            get
            {
                return this.GetSlotValue(ATTR_WHOM) as Pullenti.Ner.Referent;
            }
            set
            {
                this.AddSlot(ATTR_WHOM, value, true, 0);
            }
        }
        /// <summary>
        /// Когда (DateReferent или DateRangeReferent)
        /// </summary>
        public Pullenti.Ner.Referent When
        {
            get
            {
                return this.GetSlotValue(ATTR_WHEN) as Pullenti.Ner.Referent;
            }
            set
            {
                this.AddSlot(ATTR_WHEN, value, true, 0);
            }
        }
        /// <summary>
        /// Что (артефакты события) - список Referent
        /// </summary>
        public List<Pullenti.Ner.Referent> Whats
        {
            get
            {
                List<Pullenti.Ner.Referent> res = new List<Pullenti.Ner.Referent>();
                foreach (Pullenti.Ner.Slot s in Slots) 
                {
                    if (s.TypeName == ATTR_WHAT && (s.Value is Pullenti.Ner.Referent)) 
                        res.Add(s.Value as Pullenti.Ner.Referent);
                }
                return res;
            }
        }
        internal void AddWhat(object w)
        {
            if (w is Pullenti.Ner.Referent) 
                this.AddSlot(ATTR_WHAT, w, false, 0);
        }
        public override string ToStringEx(bool shortVariant, Pullenti.Morph.MorphLang lang, int lev = 0)
        {
            StringBuilder res = new StringBuilder();
            string typ = Typ ?? "Бизнес-факт";
            res.Append(Pullenti.Ner.Core.MiscHelper.ConvertFirstCharUpperAndOtherLower(typ));
            object v;
            if ((((v = this.GetSlotValue(ATTR_WHO)))) is Pullenti.Ner.Referent) 
            {
                res.AppendFormat("; Кто: {0}", (v as Pullenti.Ner.Referent).ToStringEx(true, lang, 0));
                if (Who2 != null) 
                    res.AppendFormat(" и {0}", Who2.ToStringEx(true, lang, 0));
            }
            if ((((v = this.GetSlotValue(ATTR_WHOM)))) is Pullenti.Ner.Referent) 
                res.AppendFormat("; Кого: {0}", (v as Pullenti.Ner.Referent).ToStringEx(true, lang, 0));
            if (!shortVariant) 
            {
                if ((((v = this.GetSlotValue(ATTR_WHAT)))) != null) 
                    res.AppendFormat("; Что: {0}", v);
                if ((((v = this.GetSlotValue(ATTR_WHEN)))) is Pullenti.Ner.Referent) 
                    res.AppendFormat("; Когда: {0}", (v as Pullenti.Ner.Referent).ToStringEx(shortVariant, lang, 0));
                foreach (Pullenti.Ner.Slot s in Slots) 
                {
                    if (s.TypeName == ATTR_MISC) 
                        res.AppendFormat("; {0}", s.Value);
                }
            }
            return res.ToString();
        }
        public override bool CanBeEquals(Pullenti.Ner.Referent obj, Pullenti.Ner.Core.ReferentsEqualType typ = Pullenti.Ner.Core.ReferentsEqualType.WithinOneText)
        {
            BusinessFactReferent br = obj as BusinessFactReferent;
            if (br == null) 
                return false;
            if (br.Kind != Kind) 
                return false;
            if (br.Typ != Typ) 
                return false;
            if (br.Who != Who || br.Whom != Whom) 
                return false;
            if (When != null && br.When != null) 
            {
                if (!When.CanBeEquals(br.When, Pullenti.Ner.Core.ReferentsEqualType.WithinOneText)) 
                    return false;
            }
            Pullenti.Ner.Referent mi1 = this.GetSlotValue(ATTR_WHAT) as Pullenti.Ner.Referent;
            Pullenti.Ner.Referent mi2 = br.GetSlotValue(ATTR_WHAT) as Pullenti.Ner.Referent;
            if (mi1 != null && mi2 != null) 
            {
                if (!mi1.CanBeEquals(mi2, Pullenti.Ner.Core.ReferentsEqualType.WithinOneText)) 
                    return false;
            }
            return true;
        }
    }
}