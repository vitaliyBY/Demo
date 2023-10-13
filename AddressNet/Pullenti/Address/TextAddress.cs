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
    /// Полный адрес, выделяемый из текста
    /// </summary>
    public class TextAddress
    {
        /// <summary>
        /// Элементы адреса в порядке убывания (от региона до квартир/комнат)
        /// </summary>
        public List<AddrObject> Items = new List<AddrObject>();
        /// <summary>
        /// Список дополнительных элементов (самых низкоуровневых), если есть. 
        /// При перечислении одноуровневых элементов в адресе (например, несколько квартир) 
        /// элементы начиная со второго заносятся в этот список. Если таких элементов нет, то null. 
        /// Например, ... д.10 кв.1,2,3 - кв.1 будет в LastItem, а кв.2 и 3 попадут в этот список. 
        /// Работает только для режима "текста одного адреса".
        /// </summary>
        public List<AddrObject> AdditionalItems = null;
        /// <summary>
        /// Дополнительные параметры (индексы, этажи, номер генплана и пр.)
        /// </summary>
        public Dictionary<ParamType, string> Params = new Dictionary<ParamType, string>();
        /// <summary>
        /// Последний (самый низкоуровневый) элемент адреса
        /// </summary>
        public AddrObject LastItem
        {
            get
            {
                if (Items.Count == 0) 
                    return null;
                return Items[Items.Count - 1];
            }
        }
        /// <summary>
        /// Самый низкоуровневый объект, который удалось привязать к ГАР
        /// </summary>
        public AddrObject LastItemWithGar
        {
            get
            {
                for (int i = Items.Count - 1; i >= 0; i--) 
                {
                    if (Items[i].Gars.Count > 0) 
                        return Items[i];
                }
                return null;
            }
        }
        /// <summary>
        /// Найти элемент конкретного уровня
        /// </summary>
        public AddrObject FindItemByLevel(AddrLevel lev)
        {
            AddrObject res = null;
            foreach (AddrObject it in Items) 
            {
                if (it.Level == lev || ((lev == AddrLevel.RegionArea && it.Level == AddrLevel.RegionCity)) || ((lev == AddrLevel.RegionCity && it.Level == AddrLevel.RegionArea))) 
                {
                    if (res == null || it.Gars.Count > 0) 
                        res = it;
                }
            }
            return res;
        }
        public AddrObject FindItemByGarLevel(GarLevel lev)
        {
            AddrObject res = null;
            foreach (AddrObject it in Items) 
            {
                if (AddressHelper.CanBeEqualLevels(it.Level, lev)) 
                {
                    if (res == null || it.Gars.Count > 0) 
                        res = it;
                }
            }
            return res;
        }
        public GarObject FindGarByIds(List<string> ids)
        {
            if (ids == null) 
                return null;
            foreach (AddrObject it in Items) 
            {
                if (it == null) 
                    continue;
                GarObject g = it.FindGarByIds(ids);
                if (g != null) 
                    return g;
            }
            return null;
        }
        public void SortItems()
        {
            for (int k = 0; k < Items.Count; k++) 
            {
                bool ch = false;
                for (int i = 0; i < (Items.Count - 1); i++) 
                {
                    if (AddressHelper.CompareLevels(Items[i].Level, Items[i + 1].Level) > 0) 
                    {
                        AddrObject it = Items[i];
                        Items[i] = Items[i + 1];
                        Items[i + 1] = it;
                        ch = true;
                    }
                }
                if (!ch) 
                    break;
            }
        }
        /// <summary>
        /// Начальная позиция в тексте
        /// </summary>
        public int BeginChar;
        /// <summary>
        /// Конечная позиция в тексте
        /// </summary>
        public int EndChar;
        /// <summary>
        /// Коэффициент качества выделения (от 0 до 100). 
        /// 100 - идеальное качество, привязалось всё к одному ГАР-объекту 
        /// и нет непонятных элементов в тексте
        /// </summary>
        public int Coef;
        /// <summary>
        /// Сообщение, описывающее ошибки (если null, то замечаний и ошибок нет)
        /// </summary>
        public string ErrorMessage;
        /// <summary>
        /// Миллисекунд обрабатывалось (в случае обработки одиночного адреса)
        /// </summary>
        public int Milliseconds;
        /// <summary>
        /// Количество чтений объектов из индекса ГАР (для отладки производительности)
        /// </summary>
        public int ReadCount;
        /// <summary>
        /// Анализируемый текст адреса в случае одиночной обработки или фрагмент исходного текста 
        /// для множественной обработки
        /// </summary>
        public string Text;
        public override string ToString()
        {
            StringBuilder res = new StringBuilder();
            res.AppendFormat("Coef={0}", Coef);
            for (int i = 0; i < Items.Count; i++) 
            {
                res.Append((i > 0 ? ", " : ": "));
                res.Append(Items[i].ToString());
            }
            foreach (KeyValuePair<ParamType, string> kp in Params) 
            {
                if (kp.Key != ParamType.Zip) 
                    res.AppendFormat(", {0} {1}", AddressHelper.GetParamTypeString(kp.Key), kp.Value ?? "");
            }
            return res.ToString();
        }
        /// <summary>
        /// Вывести полный путь
        /// </summary>
        /// <param name="delim">разделитель, пробел по умолчанию</param>
        public string GetFullPath(string delim = " ")
        {
            StringBuilder tmp = new StringBuilder();
            for (int i = 0; i < Items.Count; i++) 
            {
                if (i > 0) 
                    tmp.Append(delim);
                tmp.Append(Items[i].ToString());
            }
            foreach (KeyValuePair<ParamType, string> kp in Params) 
            {
                if (kp.Key != ParamType.Zip) 
                    tmp.AppendFormat("{0}{1} {2}", delim, AddressHelper.GetParamTypeString(kp.Key), kp.Value ?? "");
            }
            return tmp.ToString();
        }
        /// <summary>
        /// Вывести подробную текстовую информацию об объекте (для отладки)
        /// </summary>
        public virtual void OutInfo(StringBuilder res)
        {
            res.AppendFormat("Позиция в тексте: [{0}..{1}]\r\n", BeginChar, EndChar);
            res.AppendFormat("Коэффициент качества: {0}\r\n", Coef);
            if (ErrorMessage != null) 
                res.AppendFormat("Ошибка: {0}\r\n", ErrorMessage);
            for (int i = Items.Count - 1; i >= 0; i--) 
            {
                res.Append("\r\n");
                Items[i].OutInfo(res);
                if (AdditionalItems != null && i == (Items.Count - 1)) 
                {
                    foreach (AddrObject it in AdditionalItems) 
                    {
                        res.Append("\r\n");
                        it.OutInfo(res);
                    }
                }
            }
            foreach (KeyValuePair<ParamType, string> kp in Params) 
            {
                res.AppendFormat("\r\n{0}: {1}", AddressHelper.GetParamTypeString(kp.Key), kp.Value ?? "");
            }
        }
        public void Serialize(XmlWriter xml, string tag = null)
        {
            xml.WriteStartElement("textaddr");
            xml.WriteElementString("coef", Coef.ToString());
            if (ErrorMessage != null) 
                xml.WriteElementString("message", ErrorMessage);
            xml.WriteElementString("text", Text ?? "");
            xml.WriteElementString("ms", Milliseconds.ToString());
            xml.WriteElementString("rd", ReadCount.ToString());
            xml.WriteElementString("begin", BeginChar.ToString());
            xml.WriteElementString("end", EndChar.ToString());
            foreach (AddrObject o in Items) 
            {
                o.Serialize(xml);
            }
            if (AdditionalItems != null) 
            {
                xml.WriteStartElement("additional");
                foreach (AddrObject it in AdditionalItems) 
                {
                    it.Serialize(xml);
                }
                xml.WriteEndElement();
            }
            foreach (KeyValuePair<ParamType, string> kp in Params) 
            {
                xml.WriteStartElement("param");
                xml.WriteAttributeString("typ", kp.Key.ToString().ToLower());
                if (kp.Value != null) 
                    xml.WriteAttributeString("val", kp.Value);
                xml.WriteEndElement();
            }
            xml.WriteEndElement();
        }
        public void Deserialize(XmlNode xml)
        {
            foreach (XmlNode x in xml.ChildNodes) 
            {
                if (x.LocalName == "coef") 
                    Coef = int.Parse(x.InnerText);
                else if (x.LocalName == "ms") 
                    Milliseconds = int.Parse(x.InnerText);
                else if (x.LocalName == "rd") 
                    ReadCount = int.Parse(x.InnerText);
                else if (x.LocalName == "message") 
                    ErrorMessage = x.InnerText;
                else if (x.LocalName == "text") 
                    Text = x.InnerText;
                else if (x.LocalName == "begin") 
                    BeginChar = int.Parse(x.InnerText);
                else if (x.LocalName == "end") 
                    EndChar = int.Parse(x.InnerText);
                else if (x.LocalName == "textobj") 
                {
                    AddrObject to = new AddrObject(null);
                    to.Deserialize(x);
                    Items.Add(to);
                }
                else if (x.LocalName == "additional") 
                {
                    AdditionalItems = new List<AddrObject>();
                    foreach (XmlNode xx in x.ChildNodes) 
                    {
                        AddrObject it = new AddrObject(null);
                        it.Deserialize(xx);
                        AdditionalItems.Add(it);
                    }
                }
                else if (x.LocalName == "param") 
                {
                    ParamType ty = ParamType.Undefined;
                    string val = null;
                    foreach (XmlAttribute a in x.Attributes) 
                    {
                        if (a.LocalName == "typ") 
                        {
                            try 
                            {
                                ty = (ParamType)Enum.Parse(typeof(ParamType), a.Value, true);
                            }
                            catch(Exception ex115) 
                            {
                            }
                        }
                        else if (a.LocalName == "val") 
                            val = a.Value;
                    }
                    if (ty != ParamType.Undefined) 
                        Params.Add(ty, val);
                }
            }
        }
        public TextAddress Clone()
        {
            TextAddress res = new TextAddress();
            foreach (AddrObject it in Items) 
            {
                res.Items.Add(it.Clone());
            }
            if (AdditionalItems != null) 
            {
                res.AdditionalItems = new List<AddrObject>();
                foreach (AddrObject it in AdditionalItems) 
                {
                    res.AdditionalItems.Add(it.Clone());
                }
            }
            res.BeginChar = BeginChar;
            res.EndChar = EndChar;
            res.Coef = Coef;
            res.Milliseconds = Milliseconds;
            res.Text = Text;
            res.ErrorMessage = ErrorMessage;
            foreach (KeyValuePair<ParamType, string> kp in Params) 
            {
                res.Params.Add(kp.Key, kp.Value);
            }
            return res;
        }
    }
}