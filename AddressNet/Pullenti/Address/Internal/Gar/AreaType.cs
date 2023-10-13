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

namespace Pullenti.Address.Internal.Gar
{
    public class AreaType
    {
        public string Name;
        public enum Typs : int
        {
            Undefined = 0,
            Region = 1,
            City = 2,
            Village = 3,
            Org = 4,
            Street = 5,
        }

        public Typs Typ = Typs.Undefined;
        public int Id;
        public int Count;
        public override string ToString()
        {
            return string.Format("{0}: {1}", Typ, Name);
        }
        public Dictionary<Typs, int> Stat = new Dictionary<Typs, int>();
        public void CalcTyp()
        {
            if (Name == "территория") 
                return;
            int max = 10;
            foreach (KeyValuePair<Typs, int> s in Stat) 
            {
                if (s.Value > max) 
                {
                    max = s.Value;
                    Typ = s.Key;
                }
            }
        }
        internal static void Save(string fname, Dictionary<int, AreaType> typs, string id, string dt)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = " ";
            settings.Encoding = Encoding.UTF8;
            using (FileStream f = new FileStream(fname, FileMode.Create, FileAccess.Write)) 
            {
                XmlWriter xml = XmlWriter.Create(f, settings);
                xml.WriteStartDocument();
                xml.WriteStartElement("types");
                if (id != null) 
                    xml.WriteAttributeString("guid", id);
                if (dt != null) 
                    xml.WriteAttributeString("date", dt);
                foreach (AreaType ty in typs.Values) 
                {
                    xml.WriteStartElement("type");
                    xml.WriteAttributeString("id", ty.Id.ToString());
                    xml.WriteAttributeString("class", ty.Typ.ToString());
                    xml.WriteAttributeString("name", ty.Name ?? "?");
                    xml.WriteAttributeString("count", ty.Count.ToString());
                    xml.WriteEndElement();
                }
                xml.WriteEndElement();
                xml.WriteEndDocument();
                xml.Close();
            }
        }
        internal static Dictionary<int, AreaType> Load(string fname, ref string id, ref string dt)
        {
            Dictionary<int, AreaType> res = new Dictionary<int, AreaType>();
            XmlDocument xdoc = new XmlDocument();
            using (FileStream f = new FileStream(fname, FileMode.Open, FileAccess.Read)) 
            {
                xdoc.Load(f);
            }
            foreach (XmlAttribute a in xdoc.DocumentElement.Attributes) 
            {
                if (a.LocalName == "guid") 
                    id = a.Value;
                else if (a.LocalName == "date") 
                    dt = a.Value;
            }
            foreach (XmlNode x in xdoc.DocumentElement.ChildNodes) 
            {
                if (x.LocalName == "type") 
                {
                    AreaType ty = new AreaType();
                    foreach (XmlAttribute a in x.Attributes) 
                    {
                        if (a.LocalName == "id") 
                        {
                            int n;
                            if (int.TryParse(a.Value, out n)) 
                                ty.Id = n;
                        }
                        else if (a.LocalName == "name") 
                            ty.Name = a.Value;
                        else if (a.LocalName == "count") 
                        {
                            int n;
                            if (int.TryParse(a.Value, out n)) 
                                ty.Count = n;
                        }
                        else if (a.LocalName == "class") 
                        {
                            try 
                            {
                                ty.Typ = (Typs)Enum.Parse(typeof(Typs), a.Value, true);
                            }
                            catch(Exception ex5) 
                            {
                            }
                        }
                    }
                    if (ty.Id > 0 && !res.ContainsKey(ty.Id)) 
                        res.Add(ty.Id, ty);
                }
            }
            return res;
        }
    }
}