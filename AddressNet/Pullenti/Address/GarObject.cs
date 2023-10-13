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
    /// Адресный объект ГАР
    /// </summary>
    public class GarObject : IComparable<GarObject>
    {
        public GarObject(BaseAttributes attrs)
        {
            Attrs = attrs;
        }
        /// <summary>
        /// Атрибуты ГАР-объекта (класс AreaAttributes, HouseAttributes или RoomAttributes)
        /// </summary>
        public BaseAttributes Attrs;
        /// <summary>
        /// Уровень объекта
        /// </summary>
        public GarLevel Level;
        /// <summary>
        /// Признак потери актуальности
        /// </summary>
        public bool Expired;
        /// <summary>
        /// Guid-идентификатор объекта
        /// </summary>
        public string Guid;
        /// <summary>
        /// Номер региона (77 - Москва)
        /// </summary>
        public int RegionNumber;
        /// <summary>
        /// Состояние обработки ГАР-объекта
        /// </summary>
        public GarStatus Status = GarStatus.Ok;
        /// <summary>
        /// Уникальный внутренний идентификатор внутри индекса 
        /// ВНИМАНИЕ! При перестройке индекса идентификаторы могут поменяться, поэтому не 
        /// используйте его для долгострочной инднтификации  - для этого есть Guid!
        /// </summary>
        public string Id;
        /// <summary>
        /// Идентификатор(ы) вышестоящих в иерархии родительских объектов (пустой для уровень региона). 
        /// Отметим, что один объект может одновременно входить в несколько иерархий (муниципальная и административная).
        /// </summary>
        public List<string> ParentIds = new List<string>();
        /// <summary>
        /// Используйте произвольным образом
        /// </summary>
        public object Tag;
        /// <summary>
        /// Для внутреннего использования
        /// </summary>
        internal double InternalTag;
        /// <summary>
        /// Количество дочерних элементов в иерархии
        /// </summary>
        public int ChildrenCount;
        public override string ToString()
        {
            if (Attrs == null) 
                return "?";
            AreaAttributes aa = Attrs as AreaAttributes;
            if (aa != null && aa.Types.Count > 0 && aa.Names.Count > 0) 
                return string.Format("{0} {1}", aa.Types[0], aa.Names[0]);
            return Attrs.ToString();
        }
        /// <summary>
        /// Получить значение параметра (код КЛАДР, почтовый индекс и т.п.)
        /// </summary>
        /// <param name="ty">тип параметра</param>
        /// <return>значение или null</return>
        public string GetParamValue(GarParam ty)
        {
            if (ty == GarParam.Guid) 
                return Guid;
            this._loadParams();
            string res;
            if (m_Params != null && m_Params.TryGetValue(ty, out res)) 
                return res;
            return null;
        }
        /// <summary>
        /// Получить все параметры
        /// </summary>
        public Dictionary<GarParam, string> GetParams()
        {
            this._loadParams();
            return m_Params;
        }
        Dictionary<GarParam, string> m_Params = null;
        /// <summary>
        /// Вывести подробную текстовую информацию об объекте (для отладки)
        /// </summary>
        /// <param name="res">куда выводить</param>
        public void OutInfo(StringBuilder res, bool outNameAnalyzeInfo = true)
        {
            if (Attrs != null) 
                Attrs.OutInfo(res);
            res.AppendFormat("\r\nУровень: {0} - {1}\r\n", (int)Level, AddressHelper.GetGarLevelString(Level));
            if (Expired) 
                res.AppendFormat("Актуальность: НЕТ\r\n");
            if (m_Params == null) 
                this._loadParams();
            if (m_Params != null) 
            {
                foreach (KeyValuePair<GarParam, string> p in m_Params) 
                {
                    res.AppendFormat("{0}: {1}\r\n", p.Key.ToString(), p.Value);
                }
            }
            res.AppendFormat("\r\nПолный путь: {0}\r\n", this.GetFullPath(null, false, null));
            AreaAttributes aa = Attrs as AreaAttributes;
            if (outNameAnalyzeInfo && aa != null && aa.Types.Count > 0) 
            {
                res.Append("\r\nАнализ объекта: ");
                Pullenti.Address.Internal.NameAnalyzer na = new Pullenti.Address.Internal.NameAnalyzer();
                na.ProcessEx(this);
                na.OutInfo(res);
            }
        }
        void _loadParams()
        {
            if (m_Params != null) 
                return;
            m_Params = new Dictionary<GarParam, string>();
            Dictionary<GarParam, string> pars = null;
            if (Pullenti.Address.Internal.ServerHelper.ServerUri != null) 
                pars = Pullenti.Address.Internal.ServerHelper.GetObjectParams(Id);
            else 
                pars = Pullenti.Address.Internal.GarHelper.GetObjectParams(Id);
            if (pars != null) 
            {
                foreach (KeyValuePair<GarParam, string> kp in pars) 
                {
                    if (kp.Key != GarParam.Guid) 
                        m_Params.Add(kp.Key, kp.Value);
                }
            }
            if (!m_Params.ContainsKey(GarParam.Guid)) 
                m_Params.Add(GarParam.Guid, Guid);
        }
        /// <summary>
        /// Получить полную строку адреса с учётом родителей
        /// </summary>
        /// <param name="delim">разделитель (по умолчанию, запятая с пробелом)</param>
        /// <param name="addr">если объект выделен внутри адреса, то для скорости можно указать этот адрес, чтобы родителей искал там , а не в индексе</param>
        /// <return>результат</return>
        public string GetFullPath(string delim = null, bool correct = false, TextAddress addr = null)
        {
            if (delim == null) 
                delim = ", ";
            List<GarObject> path = new List<GarObject>();
            for (GarObject o = this; o != null; ) 
            {
                path.Insert(0, o);
                if (o.ParentIds.Count == 0) 
                    break;
                if (addr != null) 
                {
                    GarObject oo = addr.FindGarByIds(o.ParentIds);
                    if (oo != null) 
                    {
                        o = oo;
                        continue;
                    }
                }
                o = AddressService.GetObject(o.ParentIds[0]);
            }
            StringBuilder tmp = new StringBuilder();
            for (int i = 0; i < path.Count; i++) 
            {
                if (i > 0) 
                    tmp.Append(delim);
                if (correct && (path[i].Attrs is AreaAttributes)) 
                {
                    AreaAttributes a = path[i].Attrs as AreaAttributes;
                    if (a.Names.Count > 0) 
                        tmp.AppendFormat("{0} {1}", (a.Types.Count == 0 ? "?" : a.Types[0]), Pullenti.Address.Internal.NameAnalyzer.CorrectFiasName((a.Names.Count > 0 ? a.Names[0] : "?")));
                }
                else if (path[i].Attrs == null) 
                    tmp.Append("?");
                else 
                    tmp.Append(path[i].Attrs.ToString());
            }
            return tmp.ToString();
        }
        public void Serialize(XmlWriter xml)
        {
            xml.WriteStartElement("gar");
            xml.WriteElementString("id", Id);
            xml.WriteElementString("level", Level.ToString().ToLower());
            foreach (string p in ParentIds) 
            {
                xml.WriteElementString("parent", p);
            }
            xml.WriteElementString("guid", Guid ?? "");
            if (Expired) 
                xml.WriteElementString("expired", "true");
            xml.WriteElementString("reg", RegionNumber.ToString());
            if (Status != GarStatus.Ok) 
                xml.WriteElementString("status", Status.ToString().ToLower());
            if (ChildrenCount > 0) 
                xml.WriteElementString("chcount", ChildrenCount.ToString());
            Attrs.Serialize(xml);
            xml.WriteEndElement();
        }
        public void Deserialize(XmlNode xml)
        {
            foreach (XmlNode x in xml.ChildNodes) 
            {
                if (x.LocalName == "id") 
                    Id = x.InnerText;
                else if (x.LocalName == "parent") 
                    ParentIds.Add(x.InnerText);
                else if (x.LocalName == "guid") 
                    Guid = x.InnerText;
                else if (x.LocalName == "expired") 
                    Expired = x.InnerText == "true";
                else if (x.LocalName == "chcount") 
                    ChildrenCount = int.Parse(x.InnerText);
                else if (x.LocalName == "reg") 
                    RegionNumber = int.Parse(x.InnerText);
                else if (x.LocalName == "status") 
                {
                    try 
                    {
                        Status = (GarStatus)Enum.Parse(typeof(GarStatus), x.InnerText, true);
                    }
                    catch(Exception ex109) 
                    {
                    }
                }
                else if (x.LocalName == "level") 
                {
                    try 
                    {
                        Level = (GarLevel)Enum.Parse(typeof(GarLevel), x.InnerText, true);
                    }
                    catch(Exception ex110) 
                    {
                    }
                }
                else if (x.LocalName == "area") 
                {
                    Attrs = new AreaAttributes();
                    Attrs.Deserialize(x);
                }
                else if (x.LocalName == "house") 
                {
                    Attrs = new HouseAttributes();
                    Attrs.Deserialize(x);
                }
                else if (x.LocalName == "room") 
                {
                    Attrs = new RoomAttributes();
                    Attrs.Deserialize(x);
                }
            }
        }
        public int CompareTo(GarObject other)
        {
            if (((int)Level) < ((int)other.Level)) 
                return -1;
            if (((int)Level) > ((int)other.Level)) 
                return 1;
            AreaAttributes aa1 = Attrs as AreaAttributes;
            AreaAttributes aa2 = other.Attrs as AreaAttributes;
            if (aa1 != null && aa2 != null) 
            {
                if (aa1.Names.Count > 0 && aa2.Names.Count > 0) 
                    return string.Compare(aa1.Names[0], aa2.Names[0]);
            }
            return string.Compare(this.ToString(), other.ToString());
        }
    }
}