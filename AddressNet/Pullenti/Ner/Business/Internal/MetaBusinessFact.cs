/*
 * SDK Pullenti Address, version 4.20, august 2023. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Ner.Business.Internal
{
    internal class MetaBusinessFact : Pullenti.Ner.Metadata.ReferentClass
    {
        public static void Initialize()
        {
            GlobalMeta = new MetaBusinessFact();
            Pullenti.Ner.Metadata.Feature f = GlobalMeta.AddFeature(Pullenti.Ner.Business.BusinessFactReferent.ATTR_KIND, "Класс", 0, 1);
            GlobalMeta.KindFeature = f;
            f.AddValue(Pullenti.Ner.Business.BusinessFactKind.Create.ToString(), "Создавать", null, null);
            f.AddValue(Pullenti.Ner.Business.BusinessFactKind.Delete.ToString(), "Удалять", null, null);
            f.AddValue(Pullenti.Ner.Business.BusinessFactKind.Have.ToString(), "Иметь", null, null);
            f.AddValue(Pullenti.Ner.Business.BusinessFactKind.Get.ToString(), "Приобретать", null, null);
            f.AddValue(Pullenti.Ner.Business.BusinessFactKind.Sell.ToString(), "Продавать", null, null);
            f.AddValue(Pullenti.Ner.Business.BusinessFactKind.Profit.ToString(), "Доход", null, null);
            f.AddValue(Pullenti.Ner.Business.BusinessFactKind.Damages.ToString(), "Убытки", null, null);
            f.AddValue(Pullenti.Ner.Business.BusinessFactKind.Agreement.ToString(), "Соглашение", null, null);
            f.AddValue(Pullenti.Ner.Business.BusinessFactKind.Subsidiary.ToString(), "Дочернее предприятие", null, null);
            f.AddValue(Pullenti.Ner.Business.BusinessFactKind.Finance.ToString(), "Финансировать", null, null);
            f.AddValue(Pullenti.Ner.Business.BusinessFactKind.Lawsuit.ToString(), "Судебный иск", null, null);
            GlobalMeta.AddFeature(Pullenti.Ner.Business.BusinessFactReferent.ATTR_TYPE, "Тип", 0, 1);
            GlobalMeta.AddFeature(Pullenti.Ner.Business.BusinessFactReferent.ATTR_WHO, "Кто", 0, 1).ShowAsParent = true;
            GlobalMeta.AddFeature(Pullenti.Ner.Business.BusinessFactReferent.ATTR_WHOM, "Кого\\Кому", 0, 1).ShowAsParent = true;
            GlobalMeta.AddFeature(Pullenti.Ner.Business.BusinessFactReferent.ATTR_WHEN, "Когда", 0, 1).ShowAsParent = true;
            GlobalMeta.AddFeature(Pullenti.Ner.Business.BusinessFactReferent.ATTR_WHAT, "Что", 0, 0).ShowAsParent = true;
            GlobalMeta.AddFeature(Pullenti.Ner.Business.BusinessFactReferent.ATTR_MISC, "Дополнительная информация", 0, 0).ShowAsParent = true;
        }
        public Pullenti.Ner.Metadata.Feature KindFeature;
        public override string Name
        {
            get
            {
                return Pullenti.Ner.Business.BusinessFactReferent.OBJ_TYPENAME;
            }
        }
        public override string Caption
        {
            get
            {
                return "Бизнес-факт";
            }
        }
        public static string ImageId = "businessfact";
        public override string GetImageId(Pullenti.Ner.Referent obj = null)
        {
            return ImageId;
        }
        public static MetaBusinessFact GlobalMeta;
    }
}