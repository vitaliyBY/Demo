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

namespace Pullenti.Address.Internal
{
    public static class FiasHelper
    {
        public static void SerializeByte(Stream res, byte val)
        {
            res.WriteByte(val);
        }
        public static void SerializeShort(Stream res, int val)
        {
            res.WriteByte((byte)val);
            res.WriteByte((byte)((val >> 8)));
        }
        public static void SerializeInt(Stream res, int val)
        {
            res.WriteByte((byte)val);
            res.WriteByte((byte)((val >> 8)));
            res.WriteByte((byte)((val >> 16)));
            res.WriteByte((byte)((val >> 24)));
        }
        public static byte DeserializeByte(Stream str)
        {
            return (byte)str.ReadByte();
        }
        public static int DeserializeShort(Stream str)
        {
            int b0 = str.ReadByte();
            int b1 = str.ReadByte();
            int res = b1;
            res <<= 8;
            return (res | b0);
        }
        public static int DeserializeInt(Stream str)
        {
            int b0 = str.ReadByte();
            int b1 = str.ReadByte();
            int b2 = str.ReadByte();
            int b3 = str.ReadByte();
            int res = b3;
            res <<= 8;
            res |= b2;
            res <<= 8;
            res |= b1;
            res <<= 8;
            return (res | b0);
        }
        public static void SerializeString(Stream res, string s, bool utf8 = false)
        {
            if (s == null) 
                res.WriteByte((byte)0xFF);
            else if (s.Length == 0) 
                res.WriteByte((byte)0);
            else 
            {
                byte[] data = (utf8 ? Encoding.UTF8.GetBytes(s) : EncodeString1251(s));
                res.WriteByte((byte)data.Length);
                res.Write(data, 0, data.Length);
            }
        }
        public static string DeserializeStringFromBytes(byte[] dat, ref int ind, bool utf8 = false)
        {
            byte len = dat[ind];
            ind++;
            if (len == 0xFF) 
                return null;
            if (len == 0) 
                return string.Empty;
            string res = (utf8 ? Encoding.UTF8.GetString(dat, ind, len) : DecodeString1251(dat, ind, len, false));
            ind += len;
            return res;
        }
        public static string DeserializeString(Stream str)
        {
            byte len = (byte)str.ReadByte();
            if (len == 0xFF) 
                return null;
            if (len == 0) 
                return string.Empty;
            byte[] buf = new byte[(int)len];
            str.Read(buf, 0, len);
            return DecodeString1251(buf, 0, -1, false);
        }
        static int[] m_1251_utf;
        static Dictionary<int, byte> m_utf_1251;
        static FiasHelper()
        {
            m_1251_utf = new int[(int)256];
            m_utf_1251 = new Dictionary<int, byte>();
            for (int i = 0; i < 0x80; i++) 
            {
                m_1251_utf[i] = i;
            }
            int[] m_1251_80_BF = new int[] {0x0402, 0x0403, 0x201A, 0x0453, 0x201E, 0x2026, 0x2020, 0x2021, 0x20AC, 0x2030, 0x0409, 0x2039, 0x040A, 0x040C, 0x040B, 0x040F, 0x0452, 0x2018, 0x2019, 0x201C, 0x201D, 0x2022, 0x2013, 0x2014, 0x0000, 0x2122, 0x0459, 0x203A, 0x045A, 0x045C, 0x045B, 0x045F, 0x00A0, 0x040E, 0x045E, 0x0408, 0x00A4, 0x0490, 0x00A6, 0x00A7, 0x0401, 0x00A9, 0x0404, 0x00AB, 0x00AC, 0x00AD, 0x00AE, 0x0407, 0x00B0, 0x00B1, 0x0406, 0x0456, 0x0491, 0x00B5, 0x00B6, 0x00B7, 0x0451, 0x2116, 0x0454, 0x00BB, 0x0458, 0x0405, 0x0455, 0x0457};
            for (int i = 0; i < 0x40; i++) 
            {
                m_1251_utf[i + 0x80] = m_1251_80_BF[i];
                m_utf_1251.Add(m_1251_80_BF[i], (byte)((i + 0x80)));
            }
            for (int i = 0; i < 0x20; i++) 
            {
                m_1251_utf[i + 0xC0] = ((int)'А') + i;
                m_utf_1251.Add(((int)'А') + i, (byte)((i + 0xC0)));
            }
            for (int i = 0; i < 0x20; i++) 
            {
                m_1251_utf[i + 0xE0] = ((int)'а') + i;
                m_utf_1251.Add(((int)'а') + i, (byte)((i + 0xE0)));
            }
        }
        public static byte[] EncodeString1251(string str)
        {
            if (str == null) 
                return new byte[(int)0];
            byte[] res = new byte[(int)str.Length];
            for (int j = 0; j < str.Length; j++) 
            {
                int i = (int)str[j];
                if (i < 0x80) 
                    res[j] = (byte)i;
                else 
                {
                    byte b;
                    if (m_utf_1251.TryGetValue(i, out b)) 
                        res[j] = b;
                    else 
                        res[j] = (byte)'?';
                }
            }
            return res;
        }
        public static string DecodeString1251(byte[] dat, int pos = 0, int len = -1, bool zeroTerm = false)
        {
            if (dat == null) 
                return null;
            if (dat.Length == 0) 
                return "";
            if (len < 0) 
            {
                len = dat.Length - pos;
                if (zeroTerm && len > 300) 
                    len = 300;
            }
            StringBuilder tmp = new StringBuilder(len);
            for (int j = pos; (j < (pos + len)) && (j < dat.Length); j++) 
            {
                int i = (int)((byte)dat[j]);
                if (zeroTerm && i == 0) 
                    break;
                if (i < 0x80) 
                    tmp.Append((char)i);
                else if (m_1251_utf[i] == 0) 
                    tmp.Append('?');
                else 
                    tmp.Append((char)m_1251_utf[i]);
            }
            return tmp.ToString();
        }
    }
}