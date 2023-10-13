/*
 * SDK Pullenti Address, version 4.20, august 2023. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Text;

namespace Pullenti.Ner.Business
{
    /// <summary>
    /// Ценные бумаги (акции, доли в уставном капитале и пр.)
    /// </summary>
    public class FundsReferent : Pullenti.Ner.Referent
    {
        public FundsReferent() : base(OBJ_TYPENAME)
        {
            InstanceOf = Pullenti.Ner.Business.Internal.FundsMeta.GlobalMeta;
        }
        /// <summary>
        /// Имя типа сущности TypeName ("FUNDS")
        /// </summary>
        public const string OBJ_TYPENAME = "FUNDS";
        /// <summary>
        /// Имя атрибута - класс (FundsKind)
        /// </summary>
        public const string ATTR_KIND = "KIND";
        /// <summary>
        /// Имя атрибута - тип
        /// </summary>
        public const string ATTR_TYPE = "TYPE";
        /// <summary>
        /// Имя атрибута - эмитент
        /// </summary>
        public const string ATTR_SOURCE = "SOURCE";
        /// <summary>
        /// Имя атрибута - процент от общего количества
        /// </summary>
        public const string ATTR_PERCENT = "PERCENT";
        /// <summary>
        /// Имя атрибута - количество
        /// </summary>
        public const string ATTR_COUNT = "COUNT";
        /// <summary>
        /// Имя атрибута - общая цена за всё (MoneyReferent)
        /// </summary>
        public const string ATTR_SUM = "SUM";
        /// <summary>
        /// Имя атрибута - цена за одну бумагу (MoneyReferent)
        /// </summary>
        public const string ATTR_PRICE = "PRICE";
        public override string ToStringEx(bool shortVariant, Pullenti.Morph.MorphLang lang, int lev = 0)
        {
            StringBuilder res = new StringBuilder();
            if (Typ != null) 
                res.Append(Pullenti.Ner.Core.MiscHelper.ConvertFirstCharUpperAndOtherLower(Typ));
            else 
            {
                string kind = this.GetStringValue(ATTR_KIND);
                if (kind != null) 
                    kind = Pullenti.Ner.Business.Internal.FundsMeta.GlobalMeta.KindFeature.ConvertInnerValueToOuterValue(kind, null) as string;
                if (kind != null) 
                    res.Append(Pullenti.Ner.Core.MiscHelper.ConvertFirstCharUpperAndOtherLower(kind));
                else 
                    res.Append("?");
            }
            if (Source != null) 
                res.AppendFormat("; {0}", Source.ToStringEx(shortVariant, lang, 0));
            if (Count > 0) 
                res.AppendFormat("; кол-во {0}", Count);
            if (Percent > 0) 
                res.AppendFormat("; {0}%", Percent);
            if (!shortVariant) 
            {
                if (Sum != null) 
                    res.AppendFormat("; {0}", Sum.ToStringEx(false, lang, 0));
                if (Price != null) 
                    res.AppendFormat("; номинал {0}", Price.ToStringEx(false, lang, 0));
            }
            return res.ToString();
        }
        public override Pullenti.Ner.Referent ParentReferent
        {
            get
            {
                return Source;
            }
        }
        /// <summary>
        /// Классификатор ценной бумаги
        /// </summary>
        public FundsKind Kind
        {
            get
            {
                string s = this.GetStringValue(ATTR_KIND);
                if (s == null) 
                    return FundsKind.Undefined;
                try 
                {
                    object res = Enum.Parse(typeof(FundsKind), s, true);
                    if (res is FundsKind) 
                        return (FundsKind)res;
                }
                catch(Exception ex763) 
                {
                }
                return FundsKind.Undefined;
            }
            set
            {
                if (value != FundsKind.Undefined) 
                    this.AddSlot(ATTR_KIND, value.ToString(), true, 0);
                else 
                    this.AddSlot(ATTR_KIND, null, true, 0);
            }
        }
        /// <summary>
        /// Эмитент
        /// </summary>
        public Pullenti.Ner.Org.OrganizationReferent Source
        {
            get
            {
                return this.GetSlotValue(ATTR_SOURCE) as Pullenti.Ner.Org.OrganizationReferent;
            }
            set
            {
                this.AddSlot(ATTR_SOURCE, value, true, 0);
            }
        }
        /// <summary>
        /// Тип (например, привелигированная акция)
        /// </summary>
        public string Typ
        {
            get
            {
                return this.GetStringValue(ATTR_TYPE);
            }
            set
            {
                this.AddSlot(ATTR_TYPE, value, true, 0);
            }
        }
        /// <summary>
        /// Процент от общего количества
        /// </summary>
        public double Percent
        {
            get
            {
                string val = this.GetStringValue(ATTR_PERCENT);
                if (val == null) 
                    return 0;
                double? res = Pullenti.Ner.Core.NumberHelper.StringToDouble(val);
                if (res == null) 
                    return 0;
                return res.Value;
            }
            set
            {
                if (value > 0) 
                    this.AddSlot(ATTR_PERCENT, Pullenti.Ner.Core.NumberHelper.DoubleToString(value), true, 0);
                else 
                    this.AddSlot(ATTR_PERCENT, null, true, 0);
            }
        }
        /// <summary>
        /// Количество
        /// </summary>
        public int Count
        {
            get
            {
                string val = this.GetStringValue(ATTR_COUNT);
                if (val == null) 
                    return 0;
                int v;
                if (!int.TryParse(val, out v)) 
                    return 0;
                return v;
            }
            set
            {
                this.AddSlot(ATTR_COUNT, value.ToString(), true, 0);
            }
        }
        /// <summary>
        /// Сумма за все акции
        /// </summary>
        public Pullenti.Ner.Money.MoneyReferent Sum
        {
            get
            {
                return this.GetSlotValue(ATTR_SUM) as Pullenti.Ner.Money.MoneyReferent;
            }
            set
            {
                this.AddSlot(ATTR_SUM, value, true, 0);
            }
        }
        /// <summary>
        /// Сумма за одну акцию
        /// </summary>
        public Pullenti.Ner.Money.MoneyReferent Price
        {
            get
            {
                return this.GetSlotValue(ATTR_PRICE) as Pullenti.Ner.Money.MoneyReferent;
            }
            set
            {
                this.AddSlot(ATTR_PRICE, value, true, 0);
            }
        }
        public override bool CanBeEquals(Pullenti.Ner.Referent obj, Pullenti.Ner.Core.ReferentsEqualType typ = Pullenti.Ner.Core.ReferentsEqualType.WithinOneText)
        {
            FundsReferent f = obj as FundsReferent;
            if (f == null) 
                return false;
            if (Kind != f.Kind) 
                return false;
            if (Typ != null && f.Typ != null) 
            {
                if (Typ != f.Typ) 
                    return false;
            }
            if (Source != f.Source) 
                return false;
            if (Count != f.Count) 
                return false;
            if (Percent != f.Percent) 
                return false;
            if (Sum != f.Sum) 
                return false;
            return true;
        }
        internal bool CheckCorrect()
        {
            if (Kind == FundsKind.Undefined) 
                return false;
            foreach (Pullenti.Ner.Slot s in Slots) 
            {
                if (s.TypeName != ATTR_TYPE && s.TypeName != ATTR_KIND) 
                    return true;
            }
            return false;
        }
    }
}