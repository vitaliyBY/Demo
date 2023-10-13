/*
 * SDK Pullenti Address, version 4.20, august 2023. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Xml;

namespace Pullenti.Address.Internal
{
    public class SearchAddressItem : IComparable<SearchAddressItem>
    {
        public SearchLevel Level = SearchLevel.Undefined;
        public string Id;
        public string Text;
        public SearchAddressItem Parent;
        public bool Search;
        public bool IgnoreTerritories;
        public object Tag;
        public override string ToString()
        {
            return string.Format("{0}{1}: {2}", (Search ? "?" : ""), (int)Level, Text);
        }
        public void Serialize(XmlWriter xml)
        {
            xml.WriteStartElement("item");
            xml.WriteAttributeString("level", ((int)Level).ToString());
            if (Id != null) 
                xml.WriteAttributeString("id", Id);
            if (Text != null) 
                xml.WriteAttributeString("text", Text);
            if (Search) 
                xml.WriteAttributeString("search", "true");
            if (IgnoreTerritories) 
                xml.WriteAttributeString("ignoreterr", "true");
            if (Parent != null) 
                Parent.Serialize(xml);
            xml.WriteEndElement();
        }
        public void Deserialize(XmlNode xml)
        {
            if (xml.Attributes != null) 
            {
                foreach (XmlAttribute a in xml.Attributes) 
                {
                    if (a.LocalName == "level") 
                        Level = (SearchLevel)int.Parse(a.Value);
                    else if (a.LocalName == "id") 
                        Id = a.Value;
                    else if (a.LocalName == "text") 
                        Text = a.Value;
                    else if (a.LocalName == "search") 
                        Search = a.Value == "true";
                    else if (a.LocalName == "ignoreterr") 
                        IgnoreTerritories = a.Value == "true";
                }
            }
            foreach (XmlNode x in xml.ChildNodes) 
            {
                if (x.LocalName == "item") 
                {
                    Parent = new SearchAddressItem();
                    Parent.Deserialize(x);
                }
            }
        }
        public int CompareTo(SearchAddressItem other)
        {
            int i = string.Compare(Text, other.Text);
            if (i != 0) 
                return i;
            if (Parent != null && other.Parent != null) 
                return Parent.CompareTo(other.Parent);
            if (Parent == null && other.Parent != null) 
                return -1;
            if (Parent != null && other.Parent == null) 
                return 1;
            return 0;
        }
    }
}