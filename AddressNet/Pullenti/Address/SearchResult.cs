/*
 * SDK Pullenti Address, version 4.20, august 2023. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace Pullenti.Address
{
    /// <summary>
    /// Результат поискового запроса
    /// </summary>
    public class SearchResult
    {
        /// <summary>
        /// Параметры запроса
        /// </summary>
        public SearchParams Params;
        /// <summary>
        /// Всего объектов, соответствующих запросу
        /// </summary>
        public int TotalCount;
        /// <summary>
        /// Найденные объекты, соответствующие запросу
        /// </summary>
        public List<GarObject> Objects = new List<GarObject>();
        public override string ToString()
        {
            return string.Format("{0} = {1} item(s)", (Params == null ? "?" : Params.ToString()), TotalCount);
        }
        public void Serialize(XmlWriter xml)
        {
            xml.WriteStartElement("searchresult");
            if (Params != null) 
                Params.Serialize(xml);
            xml.WriteElementString("total", TotalCount.ToString());
            foreach (GarObject o in Objects) 
            {
                o.Serialize(xml);
            }
            xml.WriteEndElement();
        }
        public void Deserialize(XmlNode xml)
        {
            foreach (XmlNode x in xml.ChildNodes) 
            {
                if (x.LocalName == "seearchparams") 
                {
                    Params = new SearchParams();
                    Params.Deserialize(x);
                }
                else if (x.LocalName == "total") 
                    TotalCount = int.Parse(x.InnerText);
                else 
                {
                    GarObject go = new GarObject(null);
                    go.Deserialize(x);
                    Objects.Add(go);
                }
            }
        }
    }
}