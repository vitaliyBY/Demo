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
    /// Адресный объект, выделяемый из текста (элемент TextAddress)
    /// </summary>
    public class AddrObject
    {
        public AddrObject(BaseAttributes attrs)
        {
            Attrs = attrs;
        }
        /// <summary>
        /// Атрибуты объекта (класс AreaAttributes, HouseAttributes или RoomAttributes)
        /// </summary>
        public BaseAttributes Attrs;
        /// <summary>
        /// Уровень объекта
        /// </summary>
        public AddrLevel Level;
        /// <summary>
        /// Объект(ы) ГАР, к которым удалось привязать элемент 
        /// (возможна привязка к нескольким объектам и вообще непривязка)
        /// </summary>
        public List<GarObject> Gars = new List<GarObject>();
        /// <summary>
        /// Второй объект в случае пересечения улиц (для улицы и для дома, когда через дробь)
        /// </summary>
        public AddrObject CrossObject;
        /// <summary>
        /// Ссылка на объект репозитория (устанавливается при поиске и сохранении в репозитории)
        /// </summary>
        public RepaddrObject RepObject;
        /// <summary>
        /// Признак того, что данный элемент восстановлен из иерархии ГАР-объектов, а в тексте адреса пропущен
        /// </summary>
        public bool IsReconstructed;
        /// <summary>
        /// Дополнительная детализация (на север 100 м)
        /// </summary>
        public DetailType DetailTyp = DetailType.Undefined;
        /// <summary>
        /// Параметр детализации (если есть, обычно расстояние)
        /// </summary>
        public string DetailParam;
        // Некоторый внешний идентификатор (используется в специфических приложениях)
        public string ExtId;
        /// <summary>
        /// Используйте произвольным образом
        /// </summary>
        public object Tag;
        public override string ToString()
        {
            if (DetailTyp == DetailType.Undefined) 
                return this.ToStringMin();
            if (DetailTyp == DetailType.KmRange) 
                return string.Format("{0} {1}", this.ToStringMin(), DetailParam ?? "");
            string res = null;
            if (DetailParam == "часть") 
                res = string.Format("{0} ", AddressHelper.GetDetailPartParamString(DetailTyp));
            else 
            {
                res = AddressHelper.GetDetailTypeString(DetailTyp);
                if (DetailParam != null) 
                    res = string.Format("{0} {1}", res, DetailParam);
                res += " от ";
            }
            return res + this.ToStringMin();
        }
        public string ToStringMin()
        {
            if (Attrs == null) 
                return "?";
            string res = null;
            if (CrossObject != null) 
                res = string.Format("{0} / {1}", Attrs.ToString(), CrossObject.ToStringMin());
            else if (Attrs is AreaAttributes) 
                res = (Attrs as AreaAttributes).ToStringEx(Level, false);
            else 
                res = Attrs.ToString();
            return res;
        }
        internal GarObject FindGarById(string id)
        {
            if (id == null) 
                return null;
            foreach (GarObject g in Gars) 
            {
                if (g.Id == id) 
                    return g;
            }
            return null;
        }
        internal GarObject FindGarByIds(List<string> ids)
        {
            if (ids == null) 
                return null;
            foreach (GarObject g in Gars) 
            {
                if (ids.Contains(g.Id)) 
                    return g;
            }
            return null;
        }
        public GarObject FindGarByLevel(GarLevel level)
        {
            foreach (GarObject g in Gars) 
            {
                if (g.Level == level) 
                    return g;
            }
            return null;
        }
        internal void SortGars()
        {
            for (int k = 0; k < Gars.Count; k++) 
            {
                for (int i = 0; i < (Gars.Count - 1); i++) 
                {
                    if (Gars[i].Expired && !Gars[i + 1].Expired) 
                    {
                        GarObject g = Gars[i];
                        Gars[i] = Gars[i + 1];
                        Gars[i + 1] = g;
                    }
                }
            }
        }
        /// <summary>
        /// Вывести подробную текстовую информацию об объекте (для отладки)
        /// </summary>
        public void OutInfo(StringBuilder res)
        {
            Attrs.OutInfo(res);
            if (DetailTyp != DetailType.Undefined) 
            {
                if (DetailParam == "часть") 
                    res.AppendFormat("Детализация: {0}\r\n", AddressHelper.GetDetailPartParamString(DetailTyp));
                else 
                {
                    res.AppendFormat("Детализация: {0}", AddressHelper.GetDetailTypeString(DetailTyp));
                    if (DetailParam != null) 
                        res.AppendFormat(" {0}", DetailParam);
                    res.Append("\r\n");
                }
            }
            res.AppendFormat("\r\nУровень: {0}", AddressHelper.GetAddrLevelString(Level));
            if (RepObject != null) 
                res.AppendFormat("\r\nОбъект адрессария: {0} (ID={1})", RepObject.Spelling, RepObject.Id);
            if (ExtId != null) 
                res.AppendFormat("\r\nВнешний идентификатор: {0}", ExtId);
            res.AppendFormat("\r\nПривязка к ГАР: ");
            if (Gars.Count == 0) 
                res.Append("НЕТ");
            else 
                for (int i = 0; i < Gars.Count; i++) 
                {
                    if (i > 0) 
                        res.Append("; ");
                    res.Append(Gars[i].ToString());
                }
            if (CrossObject != null) 
            {
                res.AppendFormat("\r\n\r\nОбъект пересечения:\r\n");
                CrossObject.OutInfo(res);
            }
        }
        public bool CanBeParentFor(AddrObject child, AddrObject parForParent = null)
        {
            if (child == null) 
                return false;
            if (!AddressHelper.CanBeParent(child.Level, Level)) 
            {
                if (Level == AddrLevel.Building && child.Level == AddrLevel.Building) 
                {
                    if ((child.Attrs as HouseAttributes).Typ == HouseType.Garage && (Attrs as HouseAttributes).Typ != HouseType.Garage) 
                        return true;
                }
                if (child.Level == AddrLevel.City && Level == AddrLevel.City) 
                {
                    AreaAttributes cha = child.Attrs as AreaAttributes;
                    AreaAttributes a = Attrs as AreaAttributes;
                    if (a.Names.Count > 0 && cha.Names.Contains(a.Names[0])) 
                    {
                        if (a.Number == null && cha.Number != null) 
                            return true;
                    }
                }
                if (Level == AddrLevel.Street && child.Level == AddrLevel.Street) 
                {
                    if ((child.Attrs as AreaAttributes).Types.Contains("километр")) 
                        return true;
                    if ((Attrs as AreaAttributes).Types.Contains("километр")) 
                        return true;
                }
                return false;
            }
            if (child.Level == AddrLevel.Street || child.Level == AddrLevel.Territory) 
            {
                if (Level == AddrLevel.District) 
                {
                    if (parForParent == null) 
                        return false;
                    if (parForParent.Level != AddrLevel.City && parForParent.Level != AddrLevel.RegionCity && parForParent.Level != AddrLevel.RegionArea) 
                        return false;
                }
            }
            if (child.Level == AddrLevel.Building && Gars.Count == 1) 
            {
            }
            return true;
        }
        public bool CanBeEqualsLevel(AddrObject obj)
        {
            if (Level == obj.Level) 
                return true;
            if (Level == AddrLevel.Street && obj.Level == AddrLevel.Territory) 
                return (obj.Attrs as AreaAttributes).Miscs.Contains("дорога");
            if (Level == AddrLevel.Territory && obj.Level == AddrLevel.Street) 
                return (Attrs as AreaAttributes).Miscs.Contains("дорога");
            return false;
        }
        public bool CanBeEqualsGLevel(GarObject gar)
        {
            AreaAttributes a = Attrs as AreaAttributes;
            AreaAttributes ga = gar.Attrs as AreaAttributes;
            if (((Level == AddrLevel.Locality && gar.Level == GarLevel.Area)) || ((Level == AddrLevel.Territory && ((gar.Level == GarLevel.Locality || gar.Level == GarLevel.Street))))) 
            {
                foreach (string mi in a.Miscs) 
                {
                    if (ga.Miscs.Contains(mi)) 
                        return true;
                    if (mi == "дорога" && ((gar.Level == GarLevel.Street || gar.Level == GarLevel.Area))) 
                        return true;
                }
                foreach (string ty in a.Types) 
                {
                    if (ga.Types.Contains(ty)) 
                        return true;
                }
                if (Level == AddrLevel.Locality && gar.Level == GarLevel.Area) 
                {
                    if (ga.Types.Contains("микрорайон")) 
                        return true;
                    if (a.Types.Count > 0) 
                    {
                        if (ga.ToString().ToLower().Contains(a.Types[0])) 
                            return true;
                    }
                }
                if (Level == AddrLevel.Territory && gar.Level == GarLevel.Locality) 
                {
                    if (a.Miscs.Contains("совхоз") || a.Miscs.Contains("колхоз")) 
                        return true;
                }
                if (Level == AddrLevel.Territory && gar.Level == GarLevel.Street) 
                {
                    if (a.Types.Contains("микрорайон")) 
                    {
                        if (ga.ToString().ToUpper().Contains("МИКРОРАЙОН")) 
                            return true;
                    }
                }
                return false;
            }
            if (Level == AddrLevel.City && ((gar.Level == GarLevel.MunicipalArea || gar.Level == GarLevel.AdminArea))) 
            {
                if (ga.Types.Contains("город")) 
                    return true;
            }
            if (Level == AddrLevel.District && gar.Level == GarLevel.Settlement) 
            {
                foreach (string ty in (gar.Attrs as AreaAttributes).Types) 
                {
                    if (ty.Contains("район")) 
                        return true;
                }
                foreach (string nam in (gar.Attrs as AreaAttributes).Names) 
                {
                    if (nam.Contains("район")) 
                        return true;
                }
            }
            if (AddressHelper.CanBeEqualLevels(Level, gar.Level)) 
                return true;
            if (Level == AddrLevel.Locality && gar.Level == GarLevel.Settlement) 
                return true;
            if (Level == AddrLevel.Settlement && gar.Level == GarLevel.Locality) 
                return true;
            if (Level == AddrLevel.District && gar.Level == GarLevel.Locality) 
            {
                if (a.Types.Contains("улус")) 
                    return true;
            }
            if (Level == AddrLevel.CityDistrict) 
            {
                if (ga.Types.Count > 0 && ga.Types[0].Contains("район")) 
                    return true;
            }
            if (Level == AddrLevel.Street && gar.Level == GarLevel.Area) 
            {
                if (a.Types.Count == 1 && a.Types[0] == "улица") 
                    return true;
            }
            if (((Level == AddrLevel.Settlement || Level == AddrLevel.Locality)) && gar.Level == GarLevel.City) 
            {
                if (gar.ToString().Contains("поселок")) 
                    return true;
                if (this.ToString().Contains("поселок")) 
                    return true;
                foreach (string ty in a.Types) 
                {
                    if (ga.Types.Contains(ty)) 
                        return true;
                }
            }
            if (Level == AddrLevel.City && gar.Level == GarLevel.Locality) 
            {
                if (!a.Types.Contains("город")) 
                    return true;
            }
            return false;
        }
        public void Serialize(XmlWriter xml)
        {
            xml.WriteStartElement("textobj");
            Attrs.Serialize(xml);
            xml.WriteElementString("level", Level.ToString().ToLower());
            if (IsReconstructed) 
                xml.WriteElementString("reconstr", "true");
            if (ExtId != null) 
                xml.WriteElementString("extid", ExtId);
            foreach (GarObject g in Gars) 
            {
                g.Serialize(xml);
            }
            if (CrossObject != null) 
            {
                xml.WriteStartElement("cross");
                CrossObject.Serialize(xml);
                xml.WriteEndElement();
            }
            if (DetailTyp != DetailType.Undefined) 
            {
                xml.WriteElementString("detailtyp", DetailTyp.ToString().ToLower());
                if (DetailParam != null) 
                    xml.WriteElementString("detailparame", DetailParam);
            }
            xml.WriteEndElement();
        }
        public void Deserialize(XmlNode xml)
        {
            foreach (XmlNode x in xml.ChildNodes) 
            {
                if (x.LocalName == "gar") 
                {
                    GarObject g = new GarObject(null);
                    g.Deserialize(x);
                    Gars.Add(g);
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
                else if (x.LocalName == "cross") 
                {
                    foreach (XmlNode xx in x.ChildNodes) 
                    {
                        CrossObject = new AddrObject(null);
                        CrossObject.Deserialize(xx);
                        break;
                    }
                }
                else if (x.LocalName == "level") 
                {
                    try 
                    {
                        Level = (AddrLevel)Enum.Parse(typeof(AddrLevel), x.InnerText, true);
                    }
                    catch(Exception ex107) 
                    {
                    }
                }
                else if (x.LocalName == "reconstr") 
                    IsReconstructed = x.InnerText == "true";
                else if (x.LocalName == "extid") 
                    ExtId = x.InnerText;
                else if (x.LocalName == "detailtyp") 
                {
                    try 
                    {
                        DetailTyp = (DetailType)Enum.Parse(typeof(DetailType), x.InnerText, true);
                    }
                    catch(Exception ex108) 
                    {
                    }
                }
                else if (x.LocalName == "detailparam") 
                    DetailParam = x.InnerText;
            }
        }
        public AddrObject Clone()
        {
            AddrObject res = new AddrObject(Attrs.Clone());
            res.Gars.AddRange(Gars);
            res.Level = Level;
            res.Tag = Tag;
            res.RepObject = RepObject;
            if (CrossObject != null) 
                res.CrossObject = CrossObject.Clone();
            res.DetailTyp = DetailTyp;
            res.DetailParam = DetailParam;
            return res;
        }
    }
}