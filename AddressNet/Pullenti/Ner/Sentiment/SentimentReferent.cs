/*
 * SDK Pullenti Address, version 4.20, august 2023. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Text;

namespace Pullenti.Ner.Sentiment
{
    /// <summary>
    /// Фрагмент, соответсвующий сентиментной оценке
    /// </summary>
    public class SentimentReferent : Pullenti.Ner.Referent
    {
        public SentimentReferent() : base(OBJ_TYPENAME)
        {
            InstanceOf = Pullenti.Ner.Sentiment.Internal.MetaSentiment.GlobalMeta;
        }
        public const string OBJ_TYPENAME = "SENTIMENT";
        public const string ATTR_KIND = "KIND";
        public const string ATTR_COEF = "COEF";
        public const string ATTR_REF = "REF";
        public const string ATTR_SPELLING = "SPELLING";
        public override string ToStringEx(bool shortVariant, Pullenti.Morph.MorphLang lang, int lev = 0)
        {
            StringBuilder res = new StringBuilder();
            res.Append(Pullenti.Ner.Sentiment.Internal.MetaSentiment.FTyp.ConvertInnerValueToOuterValue(this.GetStringValue(ATTR_KIND), lang));
            res.AppendFormat(" {0}", Spelling ?? "");
            if (Coef > 0) 
                res.AppendFormat(" (coef={0})", Coef);
            object r = this.GetSlotValue(ATTR_REF);
            if (r != null && !shortVariant) 
                res.AppendFormat(" -> {0}", r);
            return res.ToString();
        }
        public SentimentKind Kind
        {
            get
            {
                string s = this.GetStringValue(ATTR_KIND);
                if (s == null) 
                    return SentimentKind.Undefined;
                try 
                {
                    object res = Enum.Parse(typeof(SentimentKind), s, true);
                    if (res is SentimentKind) 
                        return (SentimentKind)res;
                }
                catch(Exception ex4010) 
                {
                }
                return SentimentKind.Undefined;
            }
            set
            {
                if (value != SentimentKind.Undefined) 
                    this.AddSlot(ATTR_KIND, value.ToString(), true, 0);
            }
        }
        public string Spelling
        {
            get
            {
                return this.GetStringValue(ATTR_SPELLING);
            }
            set
            {
                this.AddSlot(ATTR_SPELLING, value, true, 0);
            }
        }
        public int Coef
        {
            get
            {
                string val = this.GetStringValue(ATTR_COEF);
                if (val == null) 
                    return 0;
                int i;
                if (!int.TryParse(val, out i)) 
                    return 0;
                return i;
            }
            set
            {
                this.AddSlot(ATTR_COEF, value.ToString(), true, 0);
            }
        }
        public override bool CanBeEquals(Pullenti.Ner.Referent obj, Pullenti.Ner.Core.ReferentsEqualType typ = Pullenti.Ner.Core.ReferentsEqualType.WithinOneText)
        {
            SentimentReferent sr = obj as SentimentReferent;
            if (sr == null) 
                return false;
            if (sr.Kind != Kind) 
                return false;
            if (sr.Spelling != Spelling) 
                return false;
            return true;
        }
        public override bool CanBeGeneralFor(Pullenti.Ner.Referent obj)
        {
            return false;
        }
        public override Pullenti.Ner.Core.IntOntologyItem CreateOntologyItem()
        {
            Pullenti.Ner.Core.IntOntologyItem oi = new Pullenti.Ner.Core.IntOntologyItem(this);
            oi.Termins.Add(new Pullenti.Ner.Core.Termin(Spelling));
            return oi;
        }
    }
}