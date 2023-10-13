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
using System.Xml;

namespace Pullenti.Address
{
    /// <summary>
    /// Атрибуты города, региона, района, квартала, улиц и т.п.
    /// </summary>
    public class AreaAttributes : BaseAttributes
    {
        /// <summary>
        /// Тип объекта (может быть несколько вариантов)
        /// </summary>
        public List<string> Types = new List<string>();
        /// <summary>
        /// Наименование (может быть несколько вариантов или пусто - только номер)
        /// </summary>
        public List<string> Names = new List<string>();
        /// <summary>
        /// Номер или километр (если есть)
        /// </summary>
        public string Number;
        /// <summary>
        /// Это дополнительные атрибуты (например, для пос. д\о Заря - "дом отдыха")
        /// </summary>
        public List<string> Miscs = new List<string>();
        public override string ToString()
        {
            return this.ToStringEx(AddrLevel.Undefined, false);
        }
        public string ToStringEx(AddrLevel lev, bool isGar)
        {
            string res = (Types.Count > 0 ? Types[0] : "");
            bool outNum = false;
            if ((lev == AddrLevel.Street && Types.Count > 1 && Types[1] != "улица") && res == "улица") 
                res = Types[1];
            bool br = false;
            if (res == "территория" || lev == AddrLevel.Territory) 
            {
                if (Miscs.Count > 0 && !isGar && Miscs[0] != "дорога") 
                {
                    if (Names.Count > 0 && Names[0].Contains(Miscs[0])) 
                    {
                    }
                    else if (Names.Count > 0 || Number != null) 
                    {
                        res = string.Format("{0} {1}", res, Miscs[0]);
                        if (Names.Count > 0) 
                            br = true;
                    }
                }
            }
            if (Number != null && lev == AddrLevel.Street && !Number.EndsWith("км")) 
            {
                res = string.Format("{0} {1}", res, Number);
                outNum = true;
            }
            if (Names.Count > 0) 
            {
                if (res == "километр" && !char.IsDigit(Names[0][0])) 
                    res = string.Format("{0} километр", Names[0]);
                else if (br) 
                {
                    if (Number != null && !outNum) 
                    {
                        res = string.Format("{0} \"{1}-{2}\"", res, Names[0], Number);
                        outNum = true;
                    }
                    else 
                        res = string.Format("{0} \"{1}\"", res, Names[0]);
                }
                else 
                {
                    res = string.Format("{0} {1}", res, Names[0]);
                    if (lev == AddrLevel.Locality || lev == AddrLevel.Settlement) 
                    {
                        if (Miscs.Count > 0 && !Types.Contains(Miscs[0])) 
                            res = string.Format("{0} {1}", res, Pullenti.Ner.Core.MiscHelper.ConvertFirstCharUpperAndOtherLower(Miscs[0]));
                    }
                }
            }
            else if (((lev == AddrLevel.Street || lev == AddrLevel.Territory)) && Types.Count > 1) 
            {
                if (Types[1] != "улица") 
                    res = string.Format("{0} {1}", res, Pullenti.Ner.Core.MiscHelper.ConvertFirstCharUpperAndOtherLower(Types[1]));
                else 
                {
                    res = Types[1];
                    if (Number != null && lev == AddrLevel.Street) 
                    {
                        res = string.Format("{0} {1}", res, Number);
                        outNum = true;
                    }
                    res = string.Format("{0} {1}", res, Pullenti.Ner.Core.MiscHelper.ConvertFirstCharUpperAndOtherLower(Types[0]));
                }
            }
            if (Number != null && !outNum) 
            {
                if (res == "километр") 
                    res = string.Format("{0} километр", Number);
                else 
                {
                    int nnn;
                    if (lev == AddrLevel.Territory) 
                    {
                        if (Number.Length > 3) 
                            res = string.Format("{0} №{1}", res, Number);
                        else 
                            res = string.Format("{0}-{1}", res, Number);
                    }
                    else if (int.TryParse(Number, out nnn)) 
                        res = string.Format("{0}-{1}", res, Number);
                    else 
                        res = string.Format("{0} {1}", res, Number);
                }
            }
            if (Names.Count == 0 && Number == null && Miscs.Count > 0) 
                res = string.Format("{0} {1}", res, Pullenti.Ner.Core.MiscHelper.ConvertFirstCharUpperAndOtherLower(Miscs[0]));
            return res.Trim();
        }
        public override void OutInfo(StringBuilder res)
        {
            if (Types.Count > 0) 
            {
                res.AppendFormat("Тип: {0}", Types[0]);
                for (int i = 1; i < Types.Count; i++) 
                {
                    res.AppendFormat(" / {0}", Types[i]);
                }
                res.Append("\r\n");
            }
            if (Names.Count > 0) 
            {
                res.AppendFormat("Наименование: {0}", Names[0]);
                for (int i = 1; i < Names.Count; i++) 
                {
                    res.AppendFormat(" / {0}", Names[i]);
                }
                res.Append("\r\n");
            }
            if (Number != null) 
                res.AppendFormat("Номер: {0}\r\n", Number);
            if (Miscs.Count > 0) 
            {
                res.AppendFormat("Дополнительно: {0}", Miscs[0]);
                for (int i = 1; i < Miscs.Count; i++) 
                {
                    res.AppendFormat(" / {0}", Miscs[i]);
                }
                res.Append("\r\n");
            }
        }
        public override void Serialize(XmlWriter xml)
        {
            xml.WriteStartElement("area");
            foreach (string ty in Types) 
            {
                xml.WriteElementString("type", ty);
            }
            foreach (string nam in Names) 
            {
                xml.WriteElementString("name", nam);
            }
            foreach (string misc in Miscs) 
            {
                xml.WriteElementString("misc", misc);
            }
            if (Number != null) 
                xml.WriteElementString("num", Number);
            xml.WriteEndElement();
        }
        public override void Deserialize(XmlNode xml)
        {
            foreach (XmlNode x in xml.ChildNodes) 
            {
                if (x.LocalName == "type") 
                    Types.Add(x.InnerText);
                else if (x.LocalName == "name") 
                    Names.Add(x.InnerText);
                else if (x.LocalName == "misc") 
                    Miscs.Add(x.InnerText);
                else if (x.LocalName == "num") 
                    Number = x.InnerText;
            }
        }
        public bool HasEqualType(List<string> typs)
        {
            if (typs == null) 
                return false;
            foreach (string ty in Types) 
            {
                if (typs.Contains(ty)) 
                    return true;
                if (ty.Contains("поселок")) 
                {
                    foreach (string tyy in typs) 
                    {
                        if (tyy.Contains("поселок")) 
                            return true;
                    }
                }
            }
            return false;
        }
        public string FindMisc(List<string> miscs)
        {
            if (miscs != null) 
            {
                foreach (string m in miscs) 
                {
                    if (Miscs.Contains(m)) 
                        return m;
                }
            }
            return null;
        }
        public bool ContainsName(string subName)
        {
            foreach (string n in Names) 
            {
                if (n.Contains(subName)) 
                    return true;
                else if (string.Compare(n, subName, true) == 0) 
                    return true;
            }
            return false;
        }
        public override BaseAttributes Clone()
        {
            AreaAttributes res = new AreaAttributes();
            res.Types.AddRange(Types);
            res.Names.AddRange(Names);
            res.Miscs.AddRange(Miscs);
            res.Number = Number;
            return res;
        }
    }
}