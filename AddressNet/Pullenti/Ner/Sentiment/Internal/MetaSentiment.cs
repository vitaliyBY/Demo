/*
 * SDK Pullenti Address, version 4.20, august 2023. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Ner.Sentiment.Internal
{
    class MetaSentiment : Pullenti.Ner.Metadata.ReferentClass
    {
        public static void Initialize()
        {
            GlobalMeta = new MetaSentiment();
            Pullenti.Ner.Metadata.Feature f = GlobalMeta.AddFeature(Pullenti.Ner.Sentiment.SentimentReferent.ATTR_KIND, "Тип", 1, 1);
            FTyp = f;
            f.AddValue(Pullenti.Ner.Sentiment.SentimentKind.Undefined.ToString(), "Неизвестно", null, null);
            f.AddValue(Pullenti.Ner.Sentiment.SentimentKind.Positive.ToString(), "Положительно", null, null);
            f.AddValue(Pullenti.Ner.Sentiment.SentimentKind.Negative.ToString(), "Отрицательно", null, null);
            GlobalMeta.AddFeature(Pullenti.Ner.Sentiment.SentimentReferent.ATTR_SPELLING, "Текст", 0, 0);
            GlobalMeta.AddFeature(Pullenti.Ner.Sentiment.SentimentReferent.ATTR_REF, "Ссылка", 0, 0);
            GlobalMeta.AddFeature(Pullenti.Ner.Sentiment.SentimentReferent.ATTR_COEF, "Коэффициент", 0, 0);
        }
        public static Pullenti.Ner.Metadata.Feature FTyp;
        public override string Name
        {
            get
            {
                return Pullenti.Ner.Sentiment.SentimentReferent.OBJ_TYPENAME;
            }
        }
        public override string Caption
        {
            get
            {
                return "Сентимент";
            }
        }
        public static string ImageIdGood = "good";
        public static string ImageIdBad = "bad";
        public static string ImageId = "unknown";
        public override string GetImageId(Pullenti.Ner.Referent obj = null)
        {
            Pullenti.Ner.Sentiment.SentimentReferent sy = obj as Pullenti.Ner.Sentiment.SentimentReferent;
            if (sy != null) 
            {
                if (sy.Kind == Pullenti.Ner.Sentiment.SentimentKind.Positive) 
                    return ImageIdGood;
                if (sy.Kind == Pullenti.Ner.Sentiment.SentimentKind.Negative) 
                    return ImageIdBad;
            }
            return ImageId;
        }
        internal static MetaSentiment GlobalMeta;
    }
}