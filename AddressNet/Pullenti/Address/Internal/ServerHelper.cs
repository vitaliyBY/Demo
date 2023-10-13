/*
 * SDK Pullenti Address, version 4.20, august 2023. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Xml;

namespace Pullenti.Address.Internal
{
    static class ServerHelper
    {
        static object m_Lock = new object();
        public static string GetServerVersion(string address)
        {
            if (address == null) 
                address = ServerUri;
            try 
            {
                lock (m_Lock) 
                {
                    WebClient web = new WebClient();
                    byte[] res = web.DownloadData(address ?? "http://localhost:2222");
                    if (res == null || res.Length == 0) 
                        return null;
                    return Encoding.UTF8.GetString(res);
                }
            }
            catch(Exception ex) 
            {
                return null;
            }
        }
        public static string ServerUri;
        public static Pullenti.Address.GarStatistic GetGarStatistic()
        {
            byte[] dat = null;
            StringBuilder tmp = new StringBuilder();
            using (XmlWriter wxml = XmlWriter.Create(tmp)) 
            {
                wxml.WriteStartElement("GetGarStatistic");
                wxml.WriteEndElement();
            }
            dat = _getDatFromXml(tmp);
            try 
            {
                WebClient web = new WebClient();
                byte[] dat1;
                lock (m_Lock) 
                {
                    dat1 = web.UploadData(ServerUri, dat);
                }
                if (dat1 == null || dat1.Length == 0) 
                    return null;
                XmlDocument xml = new XmlDocument();
                string rstr = Encoding.UTF8.GetString(dat1);
                if (rstr.Length < 10) 
                    return null;
                xml.LoadXml(rstr);
                Pullenti.Address.GarStatistic res = new Pullenti.Address.GarStatistic();
                res.Deserialize(xml.DocumentElement);
                return res;
            }
            catch(Exception ex) 
            {
                return null;
            }
        }
        static byte[] _getDatFromXml(StringBuilder tmp)
        {
            for (int i = 10; (i < tmp.Length) && (i < 100); i++) 
            {
                if (tmp[i] == '-' && tmp[i + 1] == '1' && tmp[i + 2] == '6') 
                {
                    tmp[i + 1] = '8';
                    tmp.Remove(i + 2, 1);
                    break;
                }
            }
            return Encoding.UTF8.GetBytes(tmp.ToString());
        }
        static void _outPars(XmlWriter xml, Pullenti.Address.ProcessTextParams pars)
        {
            if (pars.DefaultRegions.Count > 0) 
            {
                string val = pars.DefaultRegions[0].ToString();
                for (int i = 1; i < pars.DefaultRegions.Count; i++) 
                {
                    val = string.Format("{0} {1}", val, pars.DefaultRegions);
                }
                xml.WriteAttributeString("regs", val);
            }
            if (pars.DefaultObject != null) 
                xml.WriteAttributeString("defobj", pars.DefaultObject.Id);
        }
        public static List<Pullenti.Address.TextAddress> ProcessText(string txt, Pullenti.Address.ProcessTextParams pars)
        {
            byte[] dat = null;
            StringBuilder tmp = new StringBuilder();
            using (XmlWriter wxml = XmlWriter.Create(tmp)) 
            {
                wxml.WriteStartElement("ProcessText");
                if (pars != null) 
                    _outPars(wxml, pars);
                wxml.WriteString(txt ?? "");
                wxml.WriteEndElement();
            }
            dat = _getDatFromXml(tmp);
            try 
            {
                WebClient web = new WebClient();
                byte[] dat1;
                lock (m_Lock) 
                {
                    dat1 = web.UploadData(ServerUri, dat);
                }
                if (dat1 == null || dat1.Length == 0) 
                    return null;
                XmlDocument xml = new XmlDocument();
                string rstr = Encoding.UTF8.GetString(dat1);
                xml.LoadXml(rstr);
                List<Pullenti.Address.TextAddress> res = new List<Pullenti.Address.TextAddress>();
                foreach (XmlNode x in xml.DocumentElement.ChildNodes) 
                {
                    if (x.ChildNodes.Count == 0) 
                        continue;
                    Pullenti.Address.TextAddress to = new Pullenti.Address.TextAddress();
                    to.Deserialize(x);
                    res.Add(to);
                }
                return res;
            }
            catch(Exception ex) 
            {
                return null;
            }
        }
        public static List<Pullenti.Address.TextAddress> ProcessSingleAddressTexts(List<string> txts, Pullenti.Address.ProcessTextParams pars)
        {
            byte[] dat = null;
            StringBuilder tmp = new StringBuilder();
            using (XmlWriter wxml = XmlWriter.Create(tmp)) 
            {
                wxml.WriteStartElement("ProcessSingleAddressTexts");
                if (pars != null) 
                    _outPars(wxml, pars);
                foreach (string txt in txts) 
                {
                    wxml.WriteElementString("text", txt);
                }
                wxml.WriteEndElement();
            }
            dat = _getDatFromXml(tmp);
            try 
            {
                WebClient web = new WebClient();
                byte[] dat1;
                lock (m_Lock) 
                {
                    dat1 = web.UploadData(ServerUri, dat);
                }
                if (dat1 == null || dat1.Length == 0) 
                    return null;
                XmlDocument xml = new XmlDocument();
                string rstr = Encoding.UTF8.GetString(dat1);
                if (rstr.Length < 5) 
                    return null;
                xml.LoadXml(rstr);
                List<Pullenti.Address.TextAddress> res = new List<Pullenti.Address.TextAddress>();
                foreach (XmlNode x in xml.DocumentElement.ChildNodes) 
                {
                    if (x.ChildNodes.Count == 0) 
                        continue;
                    Pullenti.Address.TextAddress r = new Pullenti.Address.TextAddress();
                    r.Deserialize(x);
                    res.Add(r);
                }
                if (res.Count != txts.Count) 
                    return null;
                return res;
            }
            catch(Exception ex) 
            {
                return null;
            }
        }
        public static Pullenti.Address.TextAddress ProcessSingleAddressText(string txt, Pullenti.Address.ProcessTextParams pars)
        {
            byte[] dat = null;
            StringBuilder tmp = new StringBuilder();
            using (XmlWriter wxml = XmlWriter.Create(tmp)) 
            {
                wxml.WriteStartElement("ProcessSingleAddressText");
                if (pars != null) 
                    _outPars(wxml, pars);
                wxml.WriteString(txt ?? "");
                wxml.WriteEndElement();
            }
            dat = _getDatFromXml(tmp);
            try 
            {
                WebClient web = new WebClient();
                byte[] dat1;
                lock (m_Lock) 
                {
                    dat1 = web.UploadData(ServerUri, dat);
                }
                if (dat1 == null || dat1.Length == 0) 
                    return null;
                XmlDocument xml = new XmlDocument();
                string rstr = Encoding.UTF8.GetString(dat1);
                if (rstr.Length < 5) 
                    return null;
                xml.LoadXml(rstr);
                Pullenti.Address.TextAddress res = new Pullenti.Address.TextAddress();
                res.Deserialize(xml.DocumentElement);
                return res;
            }
            catch(Exception ex) 
            {
                return null;
            }
        }
        public static Pullenti.Address.SearchResult SearchObjects(Pullenti.Address.SearchParams searchPars)
        {
            byte[] dat = null;
            StringBuilder tmp = new StringBuilder();
            using (XmlWriter wxml = XmlWriter.Create(tmp)) 
            {
                wxml.WriteStartElement("SearchObjects");
                searchPars.Serialize(wxml);
                wxml.WriteEndElement();
            }
            dat = _getDatFromXml(tmp);
            try 
            {
                WebClient web = new WebClient();
                byte[] dat1;
                lock (m_Lock) 
                {
                    dat1 = web.UploadData(ServerUri, dat);
                }
                if (dat1 == null || dat1.Length == 0) 
                    return null;
                XmlDocument xml = new XmlDocument();
                string rstr = Encoding.UTF8.GetString(dat1);
                if (rstr.Length < 5) 
                    return null;
                xml.LoadXml(rstr);
                Pullenti.Address.SearchResult res = new Pullenti.Address.SearchResult();
                res.Deserialize(xml.DocumentElement);
                return res;
            }
            catch(Exception ex) 
            {
                return null;
            }
        }
        public static List<Pullenti.Address.GarObject> GetChildrenObjects(string id, bool ignoreHouses = false)
        {
            byte[] dat = null;
            StringBuilder tmp = new StringBuilder();
            using (XmlWriter wxml = XmlWriter.Create(tmp)) 
            {
                wxml.WriteStartElement("GetObjects");
                if (ignoreHouses) 
                    wxml.WriteAttributeString("ignoreHouses", "true");
                if (id != null) 
                    wxml.WriteString(id);
                wxml.WriteEndElement();
            }
            dat = _getDatFromXml(tmp);
            try 
            {
                WebClient web = new WebClient();
                byte[] dat1;
                lock (m_Lock) 
                {
                    dat1 = web.UploadData(ServerUri, dat);
                }
                if (dat1 == null || dat1.Length == 0) 
                    return null;
                XmlDocument xml = new XmlDocument();
                string rstr = Encoding.UTF8.GetString(dat1);
                xml.LoadXml(rstr);
                List<Pullenti.Address.GarObject> res = new List<Pullenti.Address.GarObject>();
                if (rstr.Length < 10) 
                    return res;
                foreach (XmlNode x in xml.DocumentElement.ChildNodes) 
                {
                    Pullenti.Address.GarObject go = new Pullenti.Address.GarObject(null);
                    go.Deserialize(x);
                    if (go.Attrs != null) 
                        res.Add(go);
                }
                return res;
            }
            catch(Exception ex) 
            {
                return null;
            }
        }
        public static Pullenti.Address.GarObject GetObject(string objId)
        {
            byte[] dat = null;
            StringBuilder tmp = new StringBuilder();
            using (XmlWriter wxml = XmlWriter.Create(tmp)) 
            {
                wxml.WriteStartElement("GetObject");
                wxml.WriteString(objId);
                wxml.WriteEndElement();
            }
            dat = _getDatFromXml(tmp);
            try 
            {
                WebClient web = new WebClient();
                byte[] dat1;
                lock (m_Lock) 
                {
                    dat1 = web.UploadData(ServerUri, dat);
                }
                if (dat1 == null || dat1.Length == 0) 
                    return null;
                XmlDocument xml = new XmlDocument();
                string rstr = Encoding.UTF8.GetString(dat1);
                if (rstr.Length < 10) 
                    return null;
                xml.LoadXml(rstr);
                Pullenti.Address.GarObject res = new Pullenti.Address.GarObject(null);
                res.Deserialize(xml.DocumentElement);
                return res;
            }
            catch(Exception ex) 
            {
                return null;
            }
        }
        public static Dictionary<Pullenti.Address.GarParam, string> GetObjectParams(string sid)
        {
            byte[] dat = null;
            StringBuilder tmp = new StringBuilder();
            using (XmlWriter wxml = XmlWriter.Create(tmp)) 
            {
                wxml.WriteStartElement("GetObjectParams");
                wxml.WriteString(sid);
                wxml.WriteEndElement();
            }
            dat = _getDatFromXml(tmp);
            try 
            {
                WebClient web = new WebClient();
                byte[] dat1;
                lock (m_Lock) 
                {
                    dat1 = web.UploadData(ServerUri, dat);
                }
                if (dat1 == null || dat1.Length == 0) 
                    return null;
                XmlDocument xml = new XmlDocument();
                string rstr = Encoding.UTF8.GetString(dat1);
                if (rstr.Length < 10) 
                    return null;
                xml.LoadXml(rstr);
                Dictionary<Pullenti.Address.GarParam, string> res = new Dictionary<Pullenti.Address.GarParam, string>();
                foreach (XmlNode x in xml.DocumentElement.ChildNodes) 
                {
                    try 
                    {
                        Pullenti.Address.GarParam ty = (Pullenti.Address.GarParam)Enum.Parse(typeof(Pullenti.Address.GarParam), x.LocalName, true);
                        if (ty != null && ty != Pullenti.Address.GarParam.Undefined) 
                            res.Add(ty, x.InnerText);
                    }
                    catch(Exception ex101) 
                    {
                    }
                }
                return res;
            }
            catch(Exception ex) 
            {
                return null;
            }
        }
    }
}