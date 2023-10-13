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

namespace Pullenti.Address.Internal
{
    class RepObjTree
    {
        byte[] m_Data = null;
        Dictionary<char, RepObjTreeNode> Children = new Dictionary<char, RepObjTreeNode>();
        public void Clear()
        {
            foreach (KeyValuePair<char, RepObjTreeNode> kp in Children) 
            {
                kp.Value.Unload();
            }
            Children.Clear();
            m_Data = null;
        }
        public bool Modified = false;
        public int MaxLength = 8;
        public void Open(byte[] dat)
        {
            m_Data = dat;
            Children.Clear();
            int ind = 0;
            int cou = BitConverter.ToInt32(dat, ind);
            ind += 4;
            if (cou == 0) 
                return;
            for (int i = 0; i < cou; i++) 
            {
                char ch = (char)BitConverter.ToInt16(m_Data, ind);
                ind += 2;
                RepObjTreeNode tn = new RepObjTreeNode();
                this._deserializeNode(tn, ref ind);
                Children.Add(ch, tn);
            }
            Modified = false;
        }
        void _deserializeNode(RepObjTreeNode res, ref int ind)
        {
            int cou = (int)BitConverter.ToInt16(m_Data, ind);
            ind += 2;
            if (cou > 0) 
            {
                res.Objs = new Dictionary<string, int>();
                for (; cou > 0; cou--) 
                {
                    int id = BitConverter.ToInt32(m_Data, ind);
                    ind += 4;
                    string rest = FiasHelper.DeserializeStringFromBytes(m_Data, ref ind, true);
                    res.Objs.Add(rest, id);
                }
            }
            cou = BitConverter.ToInt16(m_Data, ind);
            ind += 2;
            for (; cou > 0; cou--) 
            {
                char ch = (char)BitConverter.ToInt16(m_Data, ind);
                ind += 2;
                RepObjTreeNode tn = new RepObjTreeNode();
                tn.LazyPos = ind;
                tn.Loaded = false;
                int len = BitConverter.ToInt32(m_Data, ind);
                ind += 4;
                if (res.Children == null) 
                    res.Children = new Dictionary<char, RepObjTreeNode>();
                res.Children.Add(ch, tn);
                ind = tn.LazyPos + len;
            }
            res.Loaded = true;
        }
        void _loadNode(RepObjTreeNode res)
        {
            if (!res.Loaded && res.LazyPos > 0) 
            {
                int ind = res.LazyPos + 4;
                this._deserializeNode(res, ref ind);
            }
            res.Loaded = true;
        }
        public void Save(Stream f)
        {
            FiasHelper.SerializeInt(f, Children.Count);
            foreach (KeyValuePair<char, RepObjTreeNode> kp in Children) 
            {
                FiasHelper.SerializeShort(f, (int)kp.Key);
                this._serializeNode(f, kp.Value);
            }
            Modified = false;
        }
        void _serializeNode(Stream s, RepObjTreeNode tn)
        {
            if (!tn.Loaded) 
            {
                int ind = tn.LazyPos;
                int len = BitConverter.ToInt32(m_Data, ind);
                ind += 4;
                len -= 4;
                s.Write(m_Data, ind, len);
                return;
            }
            if (tn.Objs == null || tn.Objs.Count == 0) 
                FiasHelper.SerializeShort(s, 0);
            else 
            {
                FiasHelper.SerializeShort(s, tn.Objs.Count);
                foreach (KeyValuePair<string, int> o in tn.Objs) 
                {
                    FiasHelper.SerializeInt(s, o.Value);
                    FiasHelper.SerializeString(s, o.Key, true);
                }
            }
            FiasHelper.SerializeShort(s, (tn.Children == null ? 0 : tn.Children.Count));
            if (tn.Children != null) 
            {
                foreach (KeyValuePair<char, RepObjTreeNode> ch in tn.Children) 
                {
                    FiasHelper.SerializeShort(s, (int)ch.Key);
                    int p0 = (int)s.Position;
                    FiasHelper.SerializeInt(s, 0);
                    this._serializeNode(s, ch.Value);
                    int p1 = (int)s.Position;
                    s.Position = p0;
                    FiasHelper.SerializeInt(s, p1 - p0);
                    s.Position = p1;
                }
            }
        }
        public int Find(string str)
        {
            Dictionary<char, RepObjTreeNode> dic = Children;
            RepObjTreeNode gtn = null;
            int i;
            for (i = 0; i < str.Length; i++) 
            {
                if (dic == null) 
                    return 0;
                RepObjTreeNode tn;
                char ch = str[i];
                if (!dic.TryGetValue(ch, out tn)) 
                    return 0;
                if (!tn.Loaded) 
                    this._loadNode(tn);
                gtn = tn;
                if (tn.Children == null || tn.Children.Count == 0) 
                {
                    i++;
                    break;
                }
                dic = tn.Children;
            }
            if (gtn == null || gtn.Objs == null) 
                return 0;
            string rest = (i >= str.Length ? "" : str.Substring(i));
            int res;
            if (gtn.Objs.TryGetValue(rest, out res)) 
                return res;
            return 0;
        }
        public bool Add(string str, int id)
        {
            Dictionary<char, RepObjTreeNode> dic = Children;
            RepObjTreeNode gtn = null;
            int i;
            for (i = 0; (i < str.Length) && (i < MaxLength); i++) 
            {
                RepObjTreeNode tn;
                char ch = str[i];
                if (!dic.TryGetValue(ch, out tn)) 
                {
                    tn = new RepObjTreeNode();
                    tn.Loaded = true;
                    dic.Add(ch, tn);
                }
                else if (!tn.Loaded) 
                    this._loadNode(tn);
                gtn = tn;
                if (tn.Children == null) 
                    tn.Children = new Dictionary<char, RepObjTreeNode>();
                dic = tn.Children;
            }
            if (gtn.Objs == null) 
                gtn.Objs = new Dictionary<string, int>();
            string rest = (i >= str.Length ? "" : str.Substring(i));
            if (!gtn.Objs.ContainsKey(rest)) 
            {
                gtn.Objs.Add(rest, id);
                Modified = true;
            }
            else if (gtn.Objs[rest] != id) 
            {
                gtn.Objs[rest] = id;
                Modified = true;
            }
            return true;
        }
    }
}