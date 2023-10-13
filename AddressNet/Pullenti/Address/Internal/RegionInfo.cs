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

namespace Pullenti.Address.Internal
{
    class RegionInfo
    {
        public Pullenti.Address.AreaAttributes Attrs = new Pullenti.Address.AreaAttributes();
        public NameAnalyzer Names;
        public List<string> Acronims = new List<string>();
        public string Capital;
        public List<string> Cities = new List<string>();
        public List<string> Districts = new List<string>();
        public override string ToString()
        {
            return string.Format("{0} ({1}) - {2} ({3}/{4})", Attrs.ToString(), (Acronims.Count > 0 ? Acronims[0] : "?"), Capital ?? "?", Cities.Count, Districts.Count);
        }
        public Pullenti.Ner.Core.TerminCollection TermCities;
        public void AddCity(string nam)
        {
            nam = nam.Replace('ё', 'е');
            if (!Cities.Contains(nam)) 
                Cities.Add(nam);
        }
        public void AddDistrict(string nam)
        {
            nam = nam.Replace('ё', 'е');
            if (!Districts.Contains(nam)) 
                Districts.Add(nam);
        }
        public void Serialize(XmlWriter xml)
        {
            xml.WriteStartElement("reg");
            Attrs.Serialize(xml);
            foreach (string a in Acronims) 
            {
                xml.WriteElementString("acr", a);
            }
            if (Capital != null) 
                xml.WriteElementString("capital", Capital);
            foreach (string c in Cities) 
            {
                xml.WriteElementString("city", c);
            }
            foreach (string d in Districts) 
            {
                xml.WriteElementString("distr", d);
            }
            xml.WriteEndElement();
        }
        public void Deserialize(XmlNode xml)
        {
            foreach (XmlNode x in xml.ChildNodes) 
            {
                if (x.LocalName == "area") 
                    Attrs.Deserialize(x);
                else if (x.LocalName == "acr") 
                    Acronims.Add(x.InnerText);
                else if (x.LocalName == "capital") 
                    Capital = x.InnerText;
                else if (x.LocalName == "city") 
                    Cities.Add(x.InnerText);
                else if (x.LocalName == "distr") 
                    Districts.Add(x.InnerText);
            }
            Names = new NameAnalyzer();
            if (Attrs.Types.Count > 0) 
                Names.Process(Attrs.Names, Attrs.Types[0]);
            else 
            {
            }
        }
        public string ReplaceCapitalByRegion(string txt)
        {
            if (Capital == null) 
                return null;
            int ii = txt.ToUpper().IndexOf(Capital.ToUpper());
            if (ii < 0) 
                return null;
            if (ii > 0 && (ii < 7) && char.ToUpper(txt[0]) == 'Г') 
                ii += Capital.Length;
            else if (ii == 0) 
            {
                ii += Capital.Length;
                for (int j = ii + 1; j < txt.Length; j++) 
                {
                    if (txt[j] == ',') 
                        break;
                    else if (txt[j] == ' ') 
                    {
                    }
                    else if (txt[j] == 'Г' || txt[j] == 'г') 
                    {
                        string ss = txt.Substring(j).ToUpper();
                        if (ss.StartsWith("ГОРОД")) 
                            ii = j + 5;
                        else if (ss.StartsWith("ГОР")) 
                            ii = j + 3;
                        else if (ss.StartsWith("Г") && ss.Length > 1 && !char.IsLetter(ss[1])) 
                            ii = j + 1;
                        break;
                    }
                    else 
                        break;
                }
            }
            else 
                return null;
            string res = string.Format("{0}, {1}", Attrs.ToString(), txt.Substring(ii));
            return res;
        }
    }
}