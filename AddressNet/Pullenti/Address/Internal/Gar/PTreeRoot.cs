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

namespace Pullenti.Address.Internal.Gar
{
    class PTreeRoot
    {
        public int MaxLength = 6;
        public Dictionary<char, PTreeNode> Children = new Dictionary<char, PTreeNode>();
        object m_Lock = new object();
        Stream m_Data;
        public void Close()
        {
            Children.Clear();
            if (m_Data != null) 
            {
                m_Data.Dispose();
                m_Data = null;
            }
        }
        public void Collect()
        {
            lock (m_Lock) 
            {
                foreach (KeyValuePair<char, PTreeNode> ch in Children) 
                {
                    ch.Value.Children.Clear();
                    ch.Value.Loaded = false;
                }
            }
        }
        public PTreeNode Add(string path, uint id)
        {
            if (string.IsNullOrEmpty(path)) 
                return null;
            PTreeNode res = null;
            if (!Children.TryGetValue(path[0], out res)) 
                Children.Add(path[0], (res = new PTreeNode()));
            int j = 1;
            for (int i = 1; (i < path.Length) && (j < MaxLength); i++) 
            {
                if (!char.IsLetterOrDigit(path[i]) || path[i] == '0') 
                    continue;
                PTreeNode rr;
                if (res.Children == null) 
                    res.Children = new Dictionary<char, PTreeNode>();
                if (!res.Children.TryGetValue(path[i], out rr)) 
                    res.Children.Add(path[i], (rr = new PTreeNode()));
                j++;
                res = rr;
            }
            if (res.Ids == null) 
                res.Ids = new List<uint>();
            if (res.Ids.Count >= 10000) 
            {
            }
            else if (!res.Ids.Contains(id)) 
                res.Ids.Add(id);
            return res;
        }
        public PTreeNode Find(string path)
        {
            if (string.IsNullOrEmpty(path)) 
                return null;
            PTreeNode res = null;
            if (!Children.TryGetValue(path[0], out res)) 
                return null;
            int j = 1;
            for (int i = 1; (i < path.Length) && (j < MaxLength); i++) 
            {
                if (!char.IsLetterOrDigit(path[i]) || path[i] == '0') 
                    continue;
                if (!res.Loaded) 
                    this.LoadNode(res);
                PTreeNode rr;
                if (res.Children == null) 
                    return null;
                if (!res.Children.TryGetValue(path[i], out rr)) 
                    return null;
                res = rr;
                j++;
            }
            if (!res.Loaded) 
                this.LoadNode(res);
            return res;
        }
        public void LoadNode(PTreeNode res)
        {
            lock (m_Lock) 
            {
                if (!res.Loaded && res.LazyPos > 0) 
                {
                    m_Data.Position = res.LazyPos;
                    this._deserializeNode(res);
                }
                res.Loaded = true;
            }
        }
        public void Save(string fname)
        {
            if (m_Data != null) 
            {
                m_Data.Dispose();
                m_Data = null;
            }
            using (FileStream f = new FileStream(fname, FileMode.Create, FileAccess.Write)) 
            {
                Pullenti.Address.Internal.FiasHelper.SerializeInt(f, Children.Count);
                foreach (KeyValuePair<char, PTreeNode> kp in Children) 
                {
                    Pullenti.Address.Internal.FiasHelper.SerializeShort(f, (short)kp.Key);
                    _serializeNode(f, kp.Value);
                }
            }
            this.Close();
            GC.Collect();
        }
        static void _serializeNode(Stream f, PTreeNode nod)
        {
            Pullenti.Address.Internal.FiasHelper.SerializeShort(f, (short)((nod.Ids == null ? 0 : nod.Ids.Count)));
            if (nod.Ids != null) 
            {
                nod.Ids.Sort();
                foreach (uint v in nod.Ids) 
                {
                    Pullenti.Address.Internal.FiasHelper.SerializeInt(f, (int)v);
                }
            }
            Pullenti.Address.Internal.FiasHelper.SerializeInt(f, (nod.Children == null ? 0 : nod.Children.Count));
            if (nod.Children != null) 
            {
                if (nod.Children.Count > 0x1000) 
                {
                }
                foreach (KeyValuePair<char, PTreeNode> kp in nod.Children) 
                {
                    Pullenti.Address.Internal.FiasHelper.SerializeShort(f, (short)kp.Key);
                    int p0 = (int)f.Position;
                    Pullenti.Address.Internal.FiasHelper.SerializeInt(f, 0);
                    _serializeNode(f, kp.Value);
                    int p1 = (int)f.Position;
                    f.Position = p0;
                    Pullenti.Address.Internal.FiasHelper.SerializeInt(f, p1);
                    f.Position = p1;
                }
            }
        }
        public void Load(string fname)
        {
            m_Data = new FileStream(fname, FileMode.Open, FileAccess.Read);
            m_Data.Position = 0;
            int cou = Pullenti.Address.Internal.FiasHelper.DeserializeInt(m_Data);
            if (cou == 0) 
                return;
            for (int i = 0; i < cou; i++) 
            {
                char ch = (char)Pullenti.Address.Internal.FiasHelper.DeserializeShort(m_Data);
                PTreeNode tn = new PTreeNode();
                this._deserializeNode(tn);
                Children.Add(ch, tn);
            }
        }
        void _deserializeNode(PTreeNode res)
        {
            int cou = Pullenti.Address.Internal.FiasHelper.DeserializeShort(m_Data);
            if (cou > 0x1000 || (cou < 0)) 
            {
            }
            if (cou > 0) 
            {
                res.Ids = new List<uint>(cou);
                for (int i = 0; i < cou; i++) 
                {
                    int id = Pullenti.Address.Internal.FiasHelper.DeserializeInt(m_Data);
                    res.Ids.Add((uint)id);
                }
            }
            cou = Pullenti.Address.Internal.FiasHelper.DeserializeInt(m_Data);
            if (cou == 0) 
                return;
            if (cou > 0x1000 || (cou < 0)) 
            {
            }
            for (int i = 0; i < cou; i++) 
            {
                char ch = (char)Pullenti.Address.Internal.FiasHelper.DeserializeShort(m_Data);
                int p1 = Pullenti.Address.Internal.FiasHelper.DeserializeInt(m_Data);
                PTreeNode tn = new PTreeNode();
                tn.LazyPos = (int)m_Data.Position;
                tn.Loaded = false;
                if (res.Children == null) 
                    res.Children = new Dictionary<char, PTreeNode>();
                res.Children.Add(ch, tn);
                m_Data.Position = p1;
            }
            res.Loaded = true;
        }
    }
}