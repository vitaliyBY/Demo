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
    /// Парсинг файлов формата JSON
    /// </summary>
    public class JsonParser
    {
        public JsonParser(Stream stream)
        {
            m_Stream = stream;
        }
        Stream m_Stream;
        public JsonType Typ;
        public string Key;
        public object Value;
        public override string ToString()
        {
            if (Typ != JsonType.KeyValue) 
                return Typ.ToString();
            return string.Format("{0} : {1}", Key ?? "?", (Value == null ? "null" : Value.ToString()));
        }
        public bool Next()
        {
            Typ = JsonType.Error;
            Key = null;
            Value = null;
            while (true) 
            {
                int i = m_Stream.ReadByte();
                if (i < 0) 
                    break;
                char ch = (char)i;
                if (char.IsWhiteSpace(ch)) 
                    continue;
                if (ch == '[') 
                {
                    if (Typ == JsonType.Error) 
                        Typ = JsonType.ArrayStart;
                    else 
                        Value = JsonType.ArrayStart;
                    return true;
                }
                if (ch == ']') 
                {
                    if (Typ == JsonType.Error && Key != null) 
                        m_Stream.Position--;
                    else 
                        Typ = JsonType.ArrayEnd;
                    return true;
                }
                if (ch == '{') 
                {
                    if (Typ == JsonType.Error) 
                        Typ = JsonType.ObjectStart;
                    else 
                        Value = JsonType.ObjectStart;
                    return true;
                }
                if (ch == '}') 
                {
                    Typ = JsonType.ObjectEnd;
                    return true;
                }
                if (ch == ',') 
                {
                    if (Typ == JsonType.Error && Key != null) 
                        return true;
                    continue;
                }
                if (ch == '/') 
                {
                    char ch0 = (char)m_Stream.ReadByte();
                    if (ch0 == '*') 
                    {
                        ch0 = (char)0;
                        while (true) 
                        {
                            i = m_Stream.ReadByte();
                            if (i < 0) 
                                break;
                            ch = (char)i;
                            if (ch == '/' && ch0 == '*') 
                                break;
                            ch0 = ch;
                        }
                    }
                    continue;
                }
                if (ch == '"') 
                {
                    string str = this._readString();
                    if (str == null) 
                        break;
                    if (Typ == JsonType.Error) 
                        Key = str;
                    else if (Typ == JsonType.KeyValue) 
                    {
                        Value = str;
                        return true;
                    }
                    continue;
                }
                if (ch == ':') 
                {
                    if (Key == null) 
                        return false;
                    Typ = JsonType.KeyValue;
                    continue;
                }
                if (char.IsDigit(ch) && Typ == JsonType.Error) 
                {
                    Key = this._readWord(ch);
                    return true;
                }
                if (char.IsDigit(ch) && Typ == JsonType.KeyValue) 
                {
                    Value = this._readWord(ch);
                    return true;
                }
                if (char.IsLetter(ch)) 
                {
                    string str = this._readWord(ch);
                    if (str == null) 
                        break;
                    if (Typ == JsonType.Error) 
                        Key = str;
                    else if (Typ == JsonType.KeyValue) 
                    {
                        if (str == "null") 
                            str = null;
                        Value = str;
                        return true;
                    }
                }
            }
            return false;
        }
        string _readString()
        {
            List<byte> li = new List<byte>();
            StringBuilder res = null;
            while (true) 
            {
                int i = m_Stream.ReadByte();
                if (i < 0) 
                    break;
                byte ch = (byte)i;
                if (ch == '"') 
                    break;
                if (ch == '\\') 
                {
                    ch = (byte)m_Stream.ReadByte();
                    if (ch == 'r') 
                        ch = 0xD;
                    else if (ch == 'n') 
                        ch = 0xA;
                    else if (ch == 't') 
                        ch = 9;
                    else if (ch == 'u') 
                    {
                        if (res == null) 
                        {
                            res = new StringBuilder();
                            if (li.Count > 0) 
                                res.Append(Encoding.UTF8.GetString(li.ToArray()));
                        }
                        int cod = 0;
                        for (int k = 0; k < 4; k++) 
                        {
                            i = m_Stream.ReadByte();
                            if (i < 0) 
                                break;
                            char c = (char)i;
                            if (i >= 0x30 && i <= 0x39) 
                                cod = (cod * 16) + ((i - 0x30));
                            else if ("ABCDEF".IndexOf(c) >= 0) 
                                cod = (cod * 16) + 10 + ((i - ((int)'A')));
                            else if ("abcdef".IndexOf(c) >= 0) 
                                cod = (cod * 16) + 10 + ((i - ((int)'a')));
                            else 
                                break;
                        }
                        res.Append((char)cod);
                        continue;
                    }
                }
                if (res != null) 
                    res.Append((char)ch);
                else 
                    li.Add(ch);
            }
            if (res != null) 
                return res.ToString();
            if (li.Count == 0) 
                return "";
            try 
            {
                return Encoding.UTF8.GetString(li.ToArray());
            }
            catch(Exception ex4377) 
            {
                return null;
            }
        }
        string _readWord(char ch0)
        {
            StringBuilder res = new StringBuilder();
            res.Append(ch0);
            while (true) 
            {
                int i = m_Stream.ReadByte();
                if (i < 0) 
                    break;
                char ch = (char)i;
                if (!char.IsLetterOrDigit(ch)) 
                {
                    m_Stream.Position--;
                    break;
                }
                res.Append(ch);
            }
            return res.ToString();
        }
    }
}