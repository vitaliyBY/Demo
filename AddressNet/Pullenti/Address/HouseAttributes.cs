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
    /// Атрибуты строений и участков
    /// </summary>
    public class HouseAttributes : BaseAttributes
    {
        /// <summary>
        /// Тип объекта
        /// </summary>
        public HouseType Typ;
        /// <summary>
        /// Номер объекта
        /// </summary>
        public string Number;
        /// <summary>
        /// Номер корпуса
        /// </summary>
        public string BuildNumber;
        /// <summary>
        /// Тип строения
        /// </summary>
        public StroenType StroenTyp = StroenType.Undefined;
        /// <summary>
        /// Номер строения
        /// </summary>
        public string StroenNumber;
        public override string ToString()
        {
            StringBuilder res = new StringBuilder();
            if (Number != null || Typ != HouseType.Undefined) 
            {
                if (Typ != HouseType.Undefined) 
                    res.AppendFormat("{0}{1}", AddressHelper.GetHouseTypeString(Typ, Number != null), Number ?? " б/н");
                else 
                    res.Append(Number);
            }
            if (BuildNumber != null) 
            {
                if (res.Length > 0) 
                    res.Append(" ");
                res.AppendFormat("корп.{0}", BuildNumber);
            }
            if (StroenNumber != null) 
            {
                if (res.Length > 0) 
                    res.Append(" ");
                res.AppendFormat("{0}{1}", AddressHelper.GetStroenTypeString(StroenTyp, true), StroenNumber);
            }
            return res.ToString();
        }
        public override void OutInfo(StringBuilder res)
        {
            if (Number != null || Typ != HouseType.Undefined) 
            {
                if (Typ != HouseType.Undefined) 
                {
                    string typ = AddressHelper.GetHouseTypeString(Typ, false);
                    res.AppendFormat("{0}{1}: {2}\r\n", char.ToUpper(typ[0]), typ.Substring(1), Number ?? "б/н");
                }
                else 
                    res.AppendFormat("Номер: {0}\r\n", Number);
            }
            if (BuildNumber != null) 
                res.AppendFormat("Корпус: {0}\r\n", BuildNumber);
            if (StroenNumber != null) 
            {
                string typ = AddressHelper.GetStroenTypeString(StroenTyp, false);
                res.AppendFormat("{0}{1}: {2}\r\n", char.ToUpper(typ[0]), typ.Substring(1), StroenNumber);
            }
        }
        public override void Serialize(XmlWriter xml)
        {
            xml.WriteStartElement("house");
            if (Typ != HouseType.Undefined) 
                xml.WriteElementString("type", Typ.ToString().ToLower());
            if (Number != null) 
                xml.WriteElementString("num", Number);
            if (BuildNumber != null) 
                xml.WriteElementString("bnum", BuildNumber);
            if (StroenNumber != null) 
            {
                xml.WriteElementString("stype", StroenTyp.ToString().ToLower());
                xml.WriteElementString("snum", StroenNumber);
            }
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
                        Typ = (HouseType)Enum.Parse(typeof(HouseType), x.InnerText, true);
                    }
                    catch(Exception ex111) 
                    {
                    }
                }
                else if (x.LocalName == "num") 
                    Number = x.InnerText;
                else if (x.LocalName == "stype") 
                {
                    try 
                    {
                        StroenTyp = (StroenType)Enum.Parse(typeof(StroenType), x.InnerText, true);
                    }
                    catch(Exception ex112) 
                    {
                    }
                }
                else if (x.LocalName == "snum") 
                    StroenNumber = x.InnerText;
                else if (x.LocalName == "bnum") 
                    BuildNumber = x.InnerText;
            }
        }
        public override BaseAttributes Clone()
        {
            HouseAttributes res = new HouseAttributes();
            res.Typ = Typ;
            res.Number = Number;
            res.BuildNumber = BuildNumber;
            res.StroenTyp = StroenTyp;
            res.StroenNumber = StroenNumber;
            return res;
        }
    }
}