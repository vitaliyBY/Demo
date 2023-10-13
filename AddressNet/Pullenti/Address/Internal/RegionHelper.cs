/*
 * SDK Pullenti Address, version 4.20, august 2023. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Pullenti.Address.Internal
{
    static class RegionHelper
    {
        public static List<RegionInfo> Regions = new List<RegionInfo>();
        public static void LoadFromFile(string fname)
        {
            if (!File.Exists(fname)) 
                return;
            Regions.Clear();
            XmlDocument xml = new XmlDocument();
            xml.Load(fname);
            foreach (XmlNode x in xml.DocumentElement.ChildNodes) 
            {
                if (x.LocalName == "reg") 
                {
                    RegionInfo r = new RegionInfo();
                    r.Deserialize(x);
                    Regions.Add(r);
                }
            }
            _init();
        }
        static void _init()
        {
            m_CityRegs.Clear();
            m_AdjRegs.Clear();
            foreach (RegionInfo r in Regions) 
            {
                r.TermCities = new Pullenti.Ner.Core.TerminCollection();
                foreach (string c in r.Cities) 
                {
                    string city = c.ToUpper();
                    if (!m_CityRegs.ContainsKey(city)) 
                        m_CityRegs.Add(city, r);
                    r.TermCities.Add(new Pullenti.Ner.Core.Termin(city));
                }
                foreach (string d in r.Districts) 
                {
                    string nam = d.ToUpper();
                    r.TermCities.Add(new Pullenti.Ner.Core.Termin(nam) { Tag = d });
                }
                foreach (Pullenti.Ner.Slot s in r.Names.Ref.Slots) 
                {
                    if (s.TypeName == "NAME") 
                    {
                        if (!m_AdjRegs.ContainsKey(s.Value as string)) 
                            m_AdjRegs.Add(s.Value as string, r);
                    }
                }
            }
        }
        static Dictionary<string, RegionInfo> m_CityRegs = new Dictionary<string, RegionInfo>();
        static Dictionary<string, RegionInfo> m_AdjRegs = new Dictionary<string, RegionInfo>();
        public static RegionInfo IsBigCity(string nam)
        {
            if (nam == null) 
                return null;
            RegionInfo res;
            if (m_CityRegs.TryGetValue(nam.ToUpper(), out res)) 
                return res;
            return null;
        }
        public static RegionInfo IsBigCityA(Pullenti.Address.AddrObject ao)
        {
            if (ao.Level != Pullenti.Address.AddrLevel.City && ao.Level != Pullenti.Address.AddrLevel.RegionCity) 
                return null;
            Pullenti.Address.AreaAttributes aa = ao.Attrs as Pullenti.Address.AreaAttributes;
            if (aa == null || aa.Names.Count == 0) 
                return null;
            if (aa.Number != null) 
                return null;
            foreach (string n in aa.Names) 
            {
                RegionInfo r = IsBigCity(n);
                if (r != null) 
                    return r;
            }
            return null;
        }
        public static List<RegionInfo> GetRegionsByAbbr(string abbr)
        {
            List<RegionInfo> res = null;
            foreach (RegionInfo r in Regions) 
            {
                if (r.Acronims.Contains(abbr)) 
                {
                    if (res == null) 
                        res = new List<RegionInfo>();
                    res.Add(r);
                }
            }
            return res;
        }
        public static RegionInfo FindRegionByAdj(string adj)
        {
            adj = adj.ToUpper();
            RegionInfo ri;
            if (!m_AdjRegs.TryGetValue(adj, out ri)) 
                return null;
            return ri;
        }
        public static void Create()
        {
            List<Pullenti.Address.GarObject> gars = Pullenti.Address.AddressService.GetObjects(null, false);
            if (gars == null || gars.Count == 0) 
                return;
            Regions.Clear();
            string[] regInfo = new string[] {"Архангельская;Архангельск", "Амурская;Благовещенск", "Алтайский;Барнаул", "Астраханская;Астрахань", "Белгородская;Белгород", "Брянская;Брянск", "Владимирская;Владимир", "Волгоградская;Волгоград", "Вологодская;Вологда", "Воронежская;Воронеж", "Донецкая;Донецк", "Забайкальский;Чита", "Ивановская;Иваново", "Иркутская;Иркутск", "Камчатский;Петропавловск-Камчатский", "Калининградская;Калининград", "Калужская;Калуга", "Кемеровская;Кемерово", "Кировская;Киров", "Костромская;Кострома", "Краснодарский;Краснодар", "Красноярский;Красноярск", "Курганская;Курган", "Курская;Курск", "Ленинградская;Санкт-Петербург", "Липецкая;Липецк", "Луганская;Луганск", "Магаданская;Магадан", "Мурманская;Мурманск", "Московская;Москва", "Нижегородская;Нижний Новгород", "Новгородская;Великий Новгород", "Новосибирская;Новосибирск", "Омская;Омск", "Оренбургская;Оренбург", "Орловская;Орел", "Пензенская;Пенза", "Пермский;Пермь", "Приморский;Владивосток", "Псковская;Псков", "Ростовская;Ростов-на-Дону", "Рязанская;Рязань", "Самарская;Самара", "Саратовская;Саратов", "Сахалинская;Южно-Сахалинск", "Свердловская;Екатеринбург", "Смоленская;Смоленск", "Ставропольский;Ставрополь", "Тамбовская;Тамбов", "Тверская;Тверь", "Томская;Томск", "Тульская;Тула", "Тюменская;Тюмень", "Ульяновская;Ульяновск", "Хабаровский;Хабаровск", "Челябинская;Челябинск", "Ярославская;Ярославль", "Адыгея;Майкоп", "Алтай;Горно-Алтайск", "Башкортостан;Уфа", "Бурятия;Улан-Удэ", "Дагестан;Махачкала", "Ингушетия;Магас", "Кабардино-Балкарская;Нальчик;КБР", "Калмыкия;Элиста", "Карачаево-Черкесская;Черкесск;КЧР", "Карелия;Петрозаводск", "Коми;Сыктывкар", "Крым;Симферополь", "Марий Эл;Йошкар-Ола;РМЭ", "Мордовия;Саранск", "Якутия;Якутск", "Северная Осетия;Владикавказ", "Татарстан;Казань", "Тыва;Кызыл", "Хакасия;Абакан", "Удмуртская;Ижевск", "Чеченская;Грозный", "Чувашская;Чебоксары", "Еврейская;Биробиджан;ЕАО", "Ханты-Мансийский;Ханты-Мансийск;ЕАО", "Чукотский;Анадырь;ЧАО", "Ямало-Ненецкий;Салехард;ЯНАО", "Ненецкий;Нарьян-Мар;НАО"};
            foreach (Pullenti.Address.GarObject g in gars) 
            {
                Pullenti.Address.AreaAttributes a = g.Attrs as Pullenti.Address.AreaAttributes;
                if (a == null) 
                    continue;
                if (a.Types.Count == 0) 
                    continue;
                if (a.Names.Count == 0) 
                    continue;
                RegionInfo ri = new RegionInfo();
                ri.Attrs = g.Attrs.Clone() as Pullenti.Address.AreaAttributes;
                Regions.Add(ri);
                ri.Acronims.Add(string.Format("{0}{1}", a.Names[0][0], a.Types[0][0]).ToUpper());
                ri.Acronims.Add(string.Format("{1}{0}", a.Names[0][0], a.Types[0][0]).ToUpper());
                ri.Names = new NameAnalyzer();
                ri.Names.Process(ri.Attrs.Names, ri.Attrs.Types[0]);
                foreach (Pullenti.Address.GarObject ch in Pullenti.Address.AddressService.GetObjects(g.Id, true)) 
                {
                    if (ch.Level == Pullenti.Address.GarLevel.City && (ch.Attrs as Pullenti.Address.AreaAttributes).Types.Contains("город")) 
                        ri.AddCity((ch.Attrs as Pullenti.Address.AreaAttributes).Names[0]);
                    if (ch.Level == Pullenti.Address.GarLevel.AdminArea) 
                        ri.AddDistrict((ch.Attrs as Pullenti.Address.AreaAttributes).Names[0]);
                    foreach (Pullenti.Address.GarObject ch2 in Pullenti.Address.AddressService.GetObjects(ch.Id, true)) 
                    {
                        if (ch2.Level == Pullenti.Address.GarLevel.City && (ch2.Attrs as Pullenti.Address.AreaAttributes).Types.Contains("город")) 
                            ri.AddCity((ch2.Attrs as Pullenti.Address.AreaAttributes).Names[0]);
                    }
                }
                ri.Cities.Sort();
                ri.Districts.Sort();
                foreach (string s in regInfo) 
                {
                    string[] pp = s.Split(';');
                    if (!a.ContainsName(pp[0])) 
                        continue;
                    ri.Capital = pp[1];
                    for (int i = 2; i < pp.Length; i++) 
                    {
                        ri.Acronims.Add(pp[i]);
                    }
                    break;
                }
                if (ri.Capital != null && !ri.Cities.Contains(ri.Capital)) 
                {
                }
            }
            _init();
        }
        public static void SaveToFile(string fname)
        {
            using (XmlWriter xml = XmlWriter.Create(fname, new XmlWriterSettings() { Encoding = Encoding.UTF8, Indent = true })) 
            {
                xml.WriteStartDocument();
                xml.WriteStartElement("regions");
                foreach (RegionInfo r in Regions) 
                {
                    r.Serialize(xml);
                }
                xml.WriteEndElement();
                xml.WriteEndDocument();
            }
        }
    }
}