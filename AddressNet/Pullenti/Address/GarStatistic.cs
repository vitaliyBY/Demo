/*
 * SDK Pullenti Address, version 4.20, august 2023. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Xml;

namespace Pullenti.Address
{
    /// <summary>
    /// Статистика по объектам ГАР
    /// </summary>
    public class GarStatistic
    {
        /// <summary>
        /// Имя локальной дорожки индекса
        /// </summary>
        public string IndexPath;
        /// <summary>
        /// Количество адресных объектов (GarArea)
        /// </summary>
        public int AreaCount;
        /// <summary>
        /// Количество домов и участков (GarHouse)
        /// </summary>
        public int HouseCount;
        /// <summary>
        /// Количество помещений (GarRoom)
        /// </summary>
        public int RoomCount;
        public override string ToString()
        {
            return string.Format("IndexPath: {0}, AddrObjs: {1}, Houses: {2}, Rooms: {3}", IndexPath, AreaCount, HouseCount, RoomCount);
        }
        public void Serialize(XmlWriter xml)
        {
            xml.WriteStartElement("GarStatistic");
            if (IndexPath != null) 
                xml.WriteElementString("path", IndexPath);
            xml.WriteElementString("areas", AreaCount.ToString());
            xml.WriteElementString("houses", HouseCount.ToString());
            xml.WriteElementString("rooms", RoomCount.ToString());
            xml.WriteEndElement();
        }
        public void Deserialize(XmlNode xml)
        {
            foreach (XmlNode x in xml.ChildNodes) 
            {
                if (x.LocalName == "path") 
                    IndexPath = x.InnerText;
                else if (x.LocalName == "areas") 
                    AreaCount = int.Parse(x.InnerText);
                else if (x.LocalName == "houses") 
                    HouseCount = int.Parse(x.InnerText);
                else if (x.LocalName == "rooms") 
                    RoomCount = int.Parse(x.InnerText);
            }
        }
    }
}