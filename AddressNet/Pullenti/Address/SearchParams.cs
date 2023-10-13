/*
 * SDK Pullenti Address, version 4.20, august 2023. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Text;
using System.Xml;

namespace Pullenti.Address
{
    /// <summary>
    /// Параметры для поиска
    /// </summary>
    public class SearchParams
    {
        /// <summary>
        /// Номер региона (0 - все)
        /// </summary>
        public int Region;
        /// <summary>
        /// Строка поиска района (null - любой)
        /// </summary>
        public string Area;
        /// <summary>
        /// Строка поиска населенного пункта (null - любой)
        /// </summary>
        public string City;
        /// <summary>
        /// Строка поиска улицы (null - не искать)
        /// </summary>
        public string Street;
        /// <summary>
        /// Тип параметра для поиска
        /// </summary>
        public GarParam ParamTyp = GarParam.Undefined;
        /// <summary>
        /// Значение параметра для поиска (при заданном ParamTyp)
        /// </summary>
        public string ParamValue;
        /// <summary>
        /// Ограничение на число возвращаемых объектов, подходящих под запрос
        /// </summary>
        public int MaxCount = 100;
        public override string ToString()
        {
            StringBuilder res = new StringBuilder();
            if (Region > 0) 
                res.AppendFormat("Region:{0} ");
            if (!string.IsNullOrEmpty(Area)) 
                res.AppendFormat("Area:'{0}' ", Area);
            if (!string.IsNullOrEmpty(City)) 
                res.AppendFormat("City:'{0}' ", City);
            if (!string.IsNullOrEmpty(Street)) 
                res.AppendFormat("Street:'{0}' ", Street);
            if (ParamTyp != GarParam.Undefined) 
                res.AppendFormat("{0}:'{1}'", ParamTyp, ParamValue ?? "");
            return res.ToString();
        }
        public void Serialize(XmlWriter xml)
        {
            xml.WriteStartElement("searchparams");
            if (Region > 0) 
                xml.WriteElementString("region", Region.ToString());
            if (Area != null) 
                xml.WriteElementString("area", Area);
            if (City != null) 
                xml.WriteElementString("city", City);
            if (Street != null) 
                xml.WriteElementString("street", Street);
            if (ParamTyp != GarParam.Undefined) 
                xml.WriteElementString("paramtype", ParamTyp.ToString().ToLower());
            if (ParamValue != null) 
                xml.WriteElementString("paramvalue", ParamValue);
            if (MaxCount > 0) 
                xml.WriteElementString("maxcount", MaxCount.ToString());
            xml.WriteEndElement();
        }
        public void Deserialize(XmlNode xml)
        {
            foreach (XmlNode x in xml.ChildNodes) 
            {
                if (x.LocalName == "region") 
                    Region = int.Parse(x.InnerText);
                else if (x.LocalName == "area") 
                    Area = x.InnerText;
                else if (x.LocalName == "city") 
                    City = x.InnerText;
                else if (x.LocalName == "street") 
                    Street = x.InnerText;
                else if (x.LocalName == "paramtype") 
                {
                    try 
                    {
                        ParamTyp = (GarParam)Enum.Parse(typeof(GarParam), x.InnerText, true);
                    }
                    catch(Exception ex114) 
                    {
                    }
                }
                else if (x.LocalName == "paramvalue") 
                    ParamValue = x.InnerText;
                else if (x.LocalName == "maxcount") 
                    MaxCount = int.Parse(x.InnerText);
            }
        }
    }
}