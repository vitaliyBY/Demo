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
    /// Атрибуты внутридомовых помещений (квартиры, комнаты), гаражей и машиномест
    /// </summary>
    public class RoomAttributes : BaseAttributes
    {
        /// <summary>
        /// Тип помещения
        /// </summary>
        public RoomType Typ;
        /// <summary>
        /// Номер помещения
        /// </summary>
        public string Number;
        public override string ToString()
        {
            return string.Format("{0}{1}", AddressHelper.GetRoomTypeString(Typ, true), Number ?? "б/н");
        }
        public override void OutInfo(StringBuilder res)
        {
            string typ = AddressHelper.GetRoomTypeString(Typ, false);
            res.AppendFormat("{0}{1}: {2}\r\n", char.ToUpper(typ[0]), typ.Substring(1), Number ?? "б/н");
        }
        public override void Serialize(XmlWriter xml)
        {
            xml.WriteStartElement("room");
            if (Typ != RoomType.Undefined) 
                xml.WriteElementString("type", Typ.ToString().ToLower());
            if (Number != null) 
                xml.WriteElementString("num", Number);
            xml.WriteEndElement();
        }
        public override void Deserialize(XmlNode xml)
        {
            foreach (XmlNode x in xml.ChildNodes) 
            {
                if (x.LocalName == "type") 
                {
                    try 
                    {
                        Typ = (RoomType)Enum.Parse(typeof(RoomType), x.InnerText, true);
                    }
                    catch(Exception ex113) 
                    {
                    }
                }
                else if (x.LocalName == "num") 
                    Number = x.InnerText;
            }
        }
        public override BaseAttributes Clone()
        {
            RoomAttributes res = new RoomAttributes();
            res.Number = Number;
            res.Typ = Typ;
            return res;
        }
    }
}