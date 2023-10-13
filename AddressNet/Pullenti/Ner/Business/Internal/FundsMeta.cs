/*
 * SDK Pullenti Address, version 4.20, august 2023. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Ner.Business.Internal
{
    internal class FundsMeta : Pullenti.Ner.Metadata.ReferentClass
    {
        public static void Initialize()
        {
            GlobalMeta = new FundsMeta();
            Pullenti.Ner.Metadata.Feature f = GlobalMeta.AddFeature(Pullenti.Ner.Business.FundsReferent.ATTR_KIND, "Класс", 0, 1);
            GlobalMeta.KindFeature = f;
            f.AddValue(Pullenti.Ner.Business.FundsKind.Stock.ToString(), "Акция", null, null);
            f.AddValue(Pullenti.Ner.Business.FundsKind.Capital.ToString(), "Уставной капитал", null, null);
            GlobalMeta.AddFeature(Pullenti.Ner.Business.FundsReferent.ATTR_TYPE, "Тип", 0, 1);
            GlobalMeta.AddFeature(Pullenti.Ner.Business.FundsReferent.ATTR_SOURCE, "Эмитент", 0, 1);
            GlobalMeta.AddFeature(Pullenti.Ner.Business.FundsReferent.ATTR_PERCENT, "Процент", 0, 1);
            GlobalMeta.AddFeature(Pullenti.Ner.Business.FundsReferent.ATTR_COUNT, "Количество", 0, 1);
            GlobalMeta.AddFeature(Pullenti.Ner.Business.FundsReferent.ATTR_PRICE, "Номинал", 0, 1);
            GlobalMeta.AddFeature(Pullenti.Ner.Business.FundsReferent.ATTR_SUM, "Денежная сумма", 0, 1);
        }
        public override string Name
        {
            get
            {
                return Pullenti.Ner.Business.FundsReferent.OBJ_TYPENAME;
            }
        }
        public override string Caption
        {
            get
            {
                return "Ценная бумага";
            }
        }
        public static string ImageId = "funds";
        public Pullenti.Ner.Metadata.Feature KindFeature;
        public override string GetImageId(Pullenti.Ner.Referent obj = null)
        {
            return ImageId;
        }
        public static FundsMeta GlobalMeta;
    }
}