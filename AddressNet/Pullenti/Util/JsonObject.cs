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

namespace Pullenti.Util
{
    /// <summary>
    /// Используется как при парсинге, так и для генерации
    /// </summary>
    public class JsonObject
    {
        /// <summary>
        /// Здесь в качестве значений могут быть string, JSonNode, List(object)
        /// </summary>
        public Dictionary<string, object> Attrs = new Dictionary<string, object>();
        public bool Multiline = false;
        public bool NewlineBefore = false;
        public static JsonObject Parse(byte[] data, bool jsonl = false)
        {
            if (data == null) 
                return null;
            using (MemoryStream mem = new MemoryStream(data)) 
            {
                return Parse(mem, jsonl);
            }
        }
        public static JsonObject Parse(Stream stream, bool jsonl = false)
        {
            JsonParser jsp = new JsonParser(stream);
            if (!jsp.Next()) 
                return null;
            if (jsp.Typ == JsonType.ObjectStart) 
            {
                JsonObject res = new JsonObject();
                if (!res._parseObject(jsp)) 
                    return null;
                if (!jsonl) 
                    return res;
                List<object> arr = new List<object>();
                arr.Add(res);
                while (true) 
                {
                    JsonObject jo = new JsonObject();
                    if (!jo._parseObject(jsp)) 
                        break;
                    arr.Add(jo);
                }
                if (arr.Count == 1) 
                    return res;
                res = new JsonObject();
                res.Attrs.Add("", arr);
                return res;
            }
            if (jsp.Typ == JsonType.ArrayStart) 
            {
                List<object> arr = _parseArray(jsp);
                if (arr == null) 
                    return null;
                JsonObject res = new JsonObject();
                res.Attrs.Add("", arr);
                return res;
            }
            return null;
        }
        static List<object> _parseArray(JsonParser jsp)
        {
            List<object> res = new List<object>();
            while (jsp.Next()) 
            {
                if (jsp.Typ == JsonType.ArrayEnd) 
                    break;
                if (jsp.Typ == JsonType.Error & jsp.Key != null) 
                {
                    res.Add(jsp.Key);
                    continue;
                }
                if (jsp.Typ == JsonType.ObjectStart) 
                {
                    JsonObject ch = new JsonObject();
                    if (!ch._parseObject(jsp)) 
                        return null;
                    res.Add(ch);
                    continue;
                }
                if (jsp.Typ == JsonType.ArrayStart) 
                {
                    List<object> ch = _parseArray(jsp);
                    if (ch == null) 
                        return null;
                    res.Add(ch);
                    continue;
                }
            }
            return res;
        }
        bool _parseObject(JsonParser jsp)
        {
            while (jsp.Next()) 
            {
                if (jsp.Typ == JsonType.ObjectEnd) 
                    return true;
                if (jsp.Typ == JsonType.KeyValue) 
                {
                    string key = jsp.Key;
                    object val = null;
                    if (jsp.Value is string) 
                        val = jsp.Value as string;
                    else if (jsp.Value is JsonType) 
                    {
                        if (((JsonType)jsp.Value) == JsonType.ArrayStart) 
                        {
                            List<object> arr = _parseArray(jsp);
                            if (arr == null) 
                                return false;
                            val = arr;
                        }
                        else if (((JsonType)jsp.Value) == JsonType.ObjectStart) 
                        {
                            JsonObject nn = new JsonObject();
                            if (!nn._parseObject(jsp)) 
                                return false;
                            val = nn;
                        }
                    }
                    if (key != null) 
                    {
                        if (!Attrs.ContainsKey(key)) 
                            Attrs.Add(key, val);
                    }
                    continue;
                }
            }
            return false;
        }
        public void Serialize(StringBuilder res, int lev = 0)
        {
            if (Attrs.ContainsKey("") && (Attrs[""] is List<object>)) 
            {
                _outArray(res, lev, Attrs[""] as List<object>, false);
                return;
            }
            _outObject(res, lev, this);
        }
        static void _outString(StringBuilder res, string txt)
        {
            res.Append('"');
            if (txt != null) 
            {
                foreach (char ch in txt) 
                {
                    if (ch == '"') 
                        res.Append("\\\"");
                    else if (ch == '\\') 
                        res.Append("\\\\");
                    else if (ch == '/') 
                        res.Append("\\/");
                    else if (ch == 0xD) 
                        res.Append("\\r");
                    else if (ch == 0xA) 
                        res.Append("\\n");
                    else if (ch == '\t') 
                        res.Append("\\t");
                    else if (((int)ch) < 0x20) 
                        res.Append(' ');
                    else 
                        res.Append(ch);
                }
            }
            res.Append('"');
        }
        static void _outObject(StringBuilder res, int lev, JsonObject obj)
        {
            if (obj.Multiline || obj.NewlineBefore) 
            {
                res.Append("\r\n");
                if (lev > 0) 
                    res.Append(' ', lev);
            }
            res.Append("{");
            bool fi = true;
            foreach (KeyValuePair<string, object> a in obj.Attrs) 
            {
                if (fi) 
                    fi = false;
                else 
                {
                    res.Append(", ");
                    if (obj.Multiline) 
                    {
                        res.Append("\r\n");
                        res.Append(' ', lev + 1);
                    }
                }
                _outString(res, a.Key);
                res.Append(":");
                if (a.Value is string) 
                {
                    _outString(res, a.Value as string);
                    continue;
                }
                if (a.Value is JsonObject) 
                {
                    res.Append(' ');
                    _outObject(res, lev + 1, a.Value as JsonObject);
                    continue;
                }
                if (a.Value is List<object>) 
                {
                    res.Append(' ');
                    _outArray(res, lev + 2, a.Value as List<object>, obj.Multiline);
                    continue;
                }
                if (a.Value is List<string>) 
                {
                    List<object> li = new List<object>();
                    foreach (string v in a.Value as List<string>) 
                    {
                        li.Add(v);
                    }
                    res.Append(' ');
                    _outArray(res, lev + 2, li, obj.Multiline);
                    continue;
                }
                if (a.Value is List<JsonObject>) 
                {
                    List<object> li = new List<object>();
                    foreach (JsonObject v in a.Value as List<JsonObject>) 
                    {
                        li.Add(v);
                    }
                    res.Append(' ');
                    _outArray(res, lev + 2, li, obj.Multiline);
                    continue;
                }
                res.Append("null");
            }
            if (lev == 0) 
                res.Append("\r\n");
            if (obj.Multiline) 
            {
                res.Append("\r\n");
                if (lev > 0) 
                    res.Append(' ', lev);
            }
            res.Append("}");
        }
        static void _outArray(StringBuilder res, int lev, List<object> arr, bool multiline = false)
        {
            res.Append("[ ");
            for (int i = 0; i < arr.Count; i++) 
            {
                if (i > 0) 
                    res.Append(", ");
                if (arr[i] is JsonObject) 
                {
                    if (multiline && !(arr[i] as JsonObject).Multiline && !(arr[i] as JsonObject).NewlineBefore) 
                    {
                        res.Append("\r\n");
                        if (lev > 0) 
                            res.Append(' ', lev);
                    }
                    _outObject(res, lev + 1, arr[i] as JsonObject);
                }
                else if (arr[i] is string) 
                    _outString(res, arr[i] as string);
                else if (arr[i] is List<object>) 
                    _outArray(res, lev + 1, arr[i] as List<object>, false);
            }
            res.Append(" ] ");
        }
    }
}