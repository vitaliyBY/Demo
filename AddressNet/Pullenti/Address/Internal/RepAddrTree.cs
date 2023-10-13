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
    class RepAddrTree
    {
        byte[] m_Data = null;
        Dictionary<char, RepAddrTreeNode> Children = new Dictionary<char, RepAddrTreeNode>();
        public void Clear()
        {
            foreach (KeyValuePair<char, RepAddrTreeNode> kp in Children) 
            {
                kp.Value.Unload();
            }
            Children.Clear();
            m_Data = null;
        }
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
                char ch = (char)BitConverter.ToInt16(dat, ind);
                ind += 2;
                RepAddrTreeNode tn = new RepAddrTreeNode();
                this._deserializeNode(tn, ref ind);
                Children.Add(ch, tn);
            }
        }
        void _deserializeNode(RepAddrTreeNode res, ref int ind)
        {
            int cou = (int)BitConverter.ToInt16(m_Data, ind);
            ind += 2;
            if (cou > 0) 
            {
                res.Objs = new List<RepAddrTreeNodeObj>();
                for (; cou > 0; cou--) 
                {
                    RepAddrTreeNodeObj o = new RepAddrTreeNodeObj();
                    o.Id = BitConverter.ToInt32(m_Data, ind);
                    ind += 4;
                    o.Lev = (Pullenti.Address.AddrLevel)((int)m_Data[ind]);
                    ind++;
                    int tt = (int)m_Data[ind];
                    ind++;
                    for (; tt > 0; tt--) 
                    {
                        o.TypIds.Add(BitConverter.ToInt16(m_Data, ind));
                        ind += 2;
                    }
                    int cc = (int)BitConverter.ToInt16(m_Data, ind);
                    ind += 2;
                    for (; cc > 0; cc--) 
                    {
                        if (o.Parents == null) 
                            o.Parents = new List<int>();
                        o.Parents.Add(BitConverter.ToInt32(m_Data, ind));
                        ind += 4;
                    }
                    res.Objs.Add(o);
                }
            }
            cou = BitConverter.ToInt16(m_Data, ind);
            ind += 2;
            for (; cou > 0; cou--) 
            {
                char ch = (char)BitConverter.ToInt16(m_Data, ind);
                ind += 2;
                RepAddrTreeNode tn = new RepAddrTreeNode();
                tn.LazyPos = ind;
                tn.Loaded = false;
                int len = BitConverter.ToInt32(m_Data, ind);
                ind += 4;
                if (res.Children == null) 
                    res.Children = new Dictionary<char, RepAddrTreeNode>();
                res.Children.Add(ch, tn);
                ind = tn.LazyPos + len;
            }
            res.Loaded = true;
        }
        void _loadNode(RepAddrTreeNode res)
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
            foreach (KeyValuePair<char, RepAddrTreeNode> kp in Children) 
            {
                FiasHelper.SerializeShort(f, (short)kp.Key);
                this._serializeNode(f, kp.Value);
            }
        }
        void _serializeNode(Stream s, RepAddrTreeNode tn)
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
                foreach (RepAddrTreeNodeObj o in tn.Objs) 
                {
                    FiasHelper.SerializeInt(s, o.Id);
                    FiasHelper.SerializeByte(s, (byte)((int)o.Lev));
                    FiasHelper.SerializeByte(s, (byte)o.TypIds.Count);
                    foreach (short ii in o.TypIds) 
                    {
                        FiasHelper.SerializeShort(s, ii);
                    }
                    if (o.Parents == null || o.Parents.Count == 0) 
                        FiasHelper.SerializeShort(s, 0);
                    else 
                    {
                        FiasHelper.SerializeShort(s, o.Parents.Count);
                        foreach (int p in o.Parents) 
                        {
                            FiasHelper.SerializeInt(s, p);
                        }
                    }
                }
            }
            FiasHelper.SerializeShort(s, (short)((tn.Children == null ? 0 : tn.Children.Count)));
            if (tn.Children != null) 
            {
                foreach (KeyValuePair<char, RepAddrTreeNode> ch in tn.Children) 
                {
                    FiasHelper.SerializeShort(s, (short)ch.Key);
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
        public List<RepAddrTreeNodeObj> Find(string path)
        {
            Dictionary<char, RepAddrTreeNode> dic = Children;
            RepAddrTreeNode gtn = null;
            for (int i = 0; i < path.Length; i++) 
            {
                if (dic == null) 
                    return null;
                RepAddrTreeNode tn;
                char ch = path[i];
                if (!dic.TryGetValue(ch, out tn)) 
                    return null;
                if (!tn.Loaded) 
                    this._loadNode(tn);
                if ((i + 1) == path.Length) 
                {
                    gtn = tn;
                    break;
                }
                if (tn.Children == null || tn.Children.Count == 0) 
                    return null;
                dic = tn.Children;
            }
            if (gtn == null) 
                return null;
            return gtn.Objs;
        }
        public bool Add(string path, RepAddrTreeNodeObj obj)
        {
            Dictionary<char, RepAddrTreeNode> dic = Children;
            RepAddrTreeNode gtn = null;
            for (int i = 0; i < path.Length; i++) 
            {
                RepAddrTreeNode tn;
                char ch = path[i];
                if (!dic.TryGetValue(ch, out tn)) 
                {
                    tn = new RepAddrTreeNode();
                    tn.Loaded = true;
                    dic.Add(ch, tn);
                }
                if (!tn.Loaded) 
                    this._loadNode(tn);
                if ((i + 1) == path.Length) 
                {
                    gtn = tn;
                    break;
                }
                if (tn.Children == null) 
                    tn.Children = new Dictionary<char, RepAddrTreeNode>();
                dic = tn.Children;
            }
            if (gtn.Objs == null) 
                gtn.Objs = new List<RepAddrTreeNodeObj>();
            foreach (RepAddrTreeNodeObj o in gtn.Objs) 
            {
                if (o.Id == obj.Id) 
                {
                    bool ret = false;
                    if (obj.Parents != null) 
                    {
                        if (o.Parents == null) 
                        {
                            o.Parents = obj.Parents;
                            ret = true;
                        }
                        else 
                            foreach (int p in obj.Parents) 
                            {
                                if (!o.Parents.Contains(p)) 
                                {
                                    o.Parents = obj.Parents;
                                    ret = true;
                                }
                            }
                    }
                    return ret;
                }
            }
            gtn.Objs.Add(obj);
            return true;
        }
    }
}