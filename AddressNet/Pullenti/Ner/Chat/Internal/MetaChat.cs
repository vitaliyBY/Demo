/*
 * SDK Pullenti Address, version 4.20, august 2023. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Ner.Chat.Internal
{
    class MetaChat : Pullenti.Ner.Metadata.ReferentClass
    {
        public static void Initialize()
        {
            GlobalMeta = new MetaChat();
            GlobalMeta.AddFeature(Pullenti.Ner.Chat.ChatReferent.ATTR_TYPE, "Тип", 1, 1);
            GlobalMeta.AddFeature(Pullenti.Ner.Chat.ChatReferent.ATTR_VALUE, "Значение", 0, 1);
            GlobalMeta.AddFeature(Pullenti.Ner.Chat.ChatReferent.ATTR_NOT, "Отрицание", 0, 1);
            GlobalMeta.AddFeature(Pullenti.Ner.Chat.ChatReferent.ATTR_VERBTYPE, "Тип глагола", 0, 0);
        }
        public override string Name
        {
            get
            {
                return Pullenti.Ner.Chat.ChatReferent.OBJ_TYPENAME;
            }
        }
        public override string Caption
        {
            get
            {
                return "Элемент диалога";
            }
        }
        public static string ImageId = "chat";
        public override string GetImageId(Pullenti.Ner.Referent obj = null)
        {
            return ImageId;
        }
        internal static MetaChat GlobalMeta;
    }
}