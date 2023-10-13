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

namespace Pullenti.Address.Internal.Gar
{
    class AreaTree
    {
        List<int> m_ObjPos = new List<int>();
        List<AreaTreeObject> m_Objs = new List<AreaTreeObject>();
        List<string> m_Strings = new List<string>();
        public int GetStringId(string str)
        {
            if (str == null) 
                return 0;
            int i = m_Strings.IndexOf(str);
            if (i < 0) 
            {
                m_Strings.Add(str);
                i = m_Strings.Count;
            }
            else 
                i++;
            return i;
        }
        public string GetString(int id)
        {
            if (id > 0 && id <= m_Strings.Count) 
                return m_Strings[id - 1];
            return null;
        }
        public Dictionary<char, AreaTreeNode> Children = new Dictionary<char, AreaTreeNode>();
        public object m_Lock = new object();
        byte[] m_Data;
        public void Close()
        {
            Children.Clear();
            m_ObjPos.Clear();
            m_Objs.Clear();
            m_Strings.Clear();
        }
        public void Collect()
        {
            foreach (KeyValuePair<char, AreaTreeNode> ch in Children) 
            {
                ch.Value.Children.Clear();
                ch.Value.Loaded = false;
            }
        }
        public void LoadAllObjects()
        {
            for (int id = 1; id <= m_Objs.Count; id++) 
            {
                this.GetObj(id);
            }
        }
        public void LoadAllData()
        {
            this.LoadAllObjects();
            foreach (KeyValuePair<char, AreaTreeNode> ch in Children) 
            {
                this._loadAllNodes(ch.Value);
            }
        }
        void _loadAllNodes(AreaTreeNode node)
        {
            if (!node.Loaded) 
                this._loadNode(node);
            if (node.Children != null) 
            {
                foreach (KeyValuePair<char, AreaTreeNode> ch in node.Children) 
                {
                    this._loadAllNodes(ch.Value);
                }
            }
        }
        public AreaTreeObject GetObj(int id)
        {
            if ((id < 1) || id >= m_Objs.Count) 
                return null;
            if (m_Objs[id] != null) 
                return m_Objs[id];
            if (m_ObjPos[id] == 0) 
                return null;
            AreaTreeObject ao = new AreaTreeObject();
            ao.Deserialize(m_Data, m_ObjPos[id], this);
            m_Objs[id] = ao;
            return ao;
        }
        public AreaTreeNode Add(string path, AreaObject ao, Pullenti.Address.Internal.NameAnalyzer na)
        {
            if (string.IsNullOrEmpty(path)) 
                return null;
            while (m_ObjPos.Count <= ao.Id) 
            {
                m_ObjPos.Add(0);
                m_Objs.Add(null);
            }
            if (ao.Id == 49599) 
            {
            }
            AreaTreeObject o = m_Objs[ao.Id];
            if (o == null) 
            {
                o = new AreaTreeObject() { Id = ao.Id };
                m_Objs[ao.Id] = o;
            }
            o.Region = ao.Region;
            o.ParentIds = ao.ParentIds;
            o.ParentParentIds = ao.ParentParentIds;
            o.Level = na.Level;
            o.Expired = !ao.Actual;
            o.Status = ao.Status;
            o.GLevel = (Pullenti.Address.GarLevel)((int)ao.Level);
            o.ChCount = (short)((ao.ChildrenIds == null ? 0 : ao.ChildrenIds.Count));
            o.TypId = (short)ao.Typ.Id;
            if (ao.OldTyp != null) 
                o.AltTypId = (short)ao.OldTyp.Id;
            if (na.Miscs != null && na.Miscs.Count > 0) 
                o.Miscs = na.Miscs;
            if (na.Types != null) 
                o.Typs = na.Types;
            AreaTreeNode res = null;
            if (!Children.TryGetValue(path[0], out res)) 
                Children.Add(path[0], (res = new AreaTreeNode()));
            for (int i = 1; i < path.Length; i++) 
            {
                AreaTreeNode rr;
                if (res.Children == null) 
                    res.Children = new Dictionary<char, AreaTreeNode>();
                if (!res.Children.TryGetValue(path[i], out rr)) 
                    res.Children.Add(path[i], (rr = new AreaTreeNode() { Parent = res }));
                res = rr;
            }
            if (res.ObjIds == null) 
                res.ObjIds = new List<int>();
            if (!res.ObjIds.Contains(ao.Id)) 
                res.ObjIds.Add(ao.Id);
            return res;
        }
        public AreaTreeNode Find(string path, bool correct = false, bool forSearch = false, bool ignoreNonCorrect = false)
        {
            if (string.IsNullOrEmpty(path)) 
                return null;
            AreaTreeNode res = null;
            if (!ignoreNonCorrect) 
            {
                res = this._find(null, path, 0);
                if (res != null) 
                {
                    if (((res.ObjIds != null && res.ObjIds.Count > 0)) || forSearch) 
                        return res;
                }
                if (!correct || (path.Length < 4)) 
                    return null;
            }
            if (!Children.TryGetValue(path[0], out res)) 
                return null;
            int j = 1;
            AreaTreeNode res1 = null;
            for (; j < path.Length; j++) 
            {
                AreaTreeNode rr;
                if (!res.Loaded) 
                    this._loadNode(res);
                if (res.Children == null) 
                    break;
                if (char.IsLetter(path[j]) && !char.IsLower(path[j]) && ((j + 1) < path.Length)) 
                {
                    foreach (KeyValuePair<char, AreaTreeNode> ch in res.Children) 
                    {
                        if (!char.IsLetter(ch.Key)) 
                            continue;
                        if (char.IsLower(ch.Key)) 
                            continue;
                        rr = this._find(ch.Value, path, j);
                        if (rr == null || rr.ObjIds == null || rr.ObjIds.Count == 0) 
                        {
                            if (j >= 2) 
                                rr = this._find(ch.Value, path, j - 1);
                        }
                        if (rr == null || rr.ObjIds == null || rr.ObjIds.Count == 0) 
                            continue;
                        if (res1 == null) 
                            res1 = rr;
                        else 
                        {
                            AreaTreeNode res2 = new AreaTreeNode();
                            res2.ObjIds = new List<int>(res1.ObjIds);
                            foreach (int id in rr.ObjIds) 
                            {
                                if (!res1.ObjIds.Contains(id)) 
                                    res2.ObjIds.Add(id);
                            }
                            res1 = res2;
                        }
                    }
                }
                if (path[j] == '$' && path[j - 1] != '@') 
                {
                    if (res.Children.ContainsKey('@')) 
                    {
                        rr = this._find(res.Children['@'], path, j - 1);
                        if (rr != null && rr.ObjIds != null && rr.ObjIds.Count > 0) 
                        {
                            if (res1 == null) 
                                res1 = rr;
                            else 
                            {
                                AreaTreeNode res2 = new AreaTreeNode();
                                res2.ObjIds = new List<int>(res1.ObjIds);
                                foreach (int id in rr.ObjIds) 
                                {
                                    if (!res1.ObjIds.Contains(id)) 
                                        res2.ObjIds.Add(id);
                                }
                                res1 = res2;
                            }
                        }
                    }
                }
                if (!res.Children.TryGetValue(path[j], out rr)) 
                    break;
                res = rr;
            }
            if (res1 != null) 
                return res1;
            StringBuilder tmp = new StringBuilder();
            for (int i = 0; i < path.Length; i++) 
            {
                if (!char.IsLetter(path[i])) 
                    continue;
                if (char.IsLower(path[i])) 
                    continue;
                if (i == 0 || (i + 1) == path.Length) 
                    continue;
                tmp.Length = 0;
                tmp.Append(path);
                tmp.Remove(i, 1);
                res = this._find(null, tmp.ToString(), 0);
                if (res != null && res.ObjIds != null && res.ObjIds.Count > 0) 
                    return res;
            }
            return null;
        }
        AreaTreeNode _find(AreaTreeNode tn, string path, int i)
        {
            AreaTreeNode res = null;
            if (tn == null) 
            {
                if (!Children.TryGetValue(path[i], out res)) 
                    return null;
            }
            else 
                res = tn;
            for (int j = i + 1; j < path.Length; j++) 
            {
                AreaTreeNode rr;
                if (!res.Loaded) 
                    this._loadNode(res);
                if (res.Children == null) 
                    return null;
                if (!res.Children.TryGetValue(path[j], out rr)) 
                    return null;
                res = rr;
            }
            if (!res.Loaded) 
                this._loadNode(res);
            return res;
        }
        public void _getAllObjIdsTotal(AreaTreeNode n, List<int> res)
        {
            if (!n.Loaded) 
                this._loadNode(n);
            if (n.ObjIds != null) 
                res.AddRange(n.ObjIds);
            if (n.Children != null) 
            {
                foreach (KeyValuePair<char, AreaTreeNode> kp in n.Children) 
                {
                    this._getAllObjIdsTotal(kp.Value, res);
                }
            }
        }
        public void GetAllObjIds(AreaTreeNode n, string suffix, bool street, List<int> res)
        {
            if (!n.Loaded) 
                this._loadNode(n);
            if (n.Children == null) 
            {
                if (n.ObjIds != null) 
                    res.AddRange(n.ObjIds);
            }
            else 
                foreach (KeyValuePair<char, AreaTreeNode> kp in n.Children) 
                {
                    if (suffix != null) 
                    {
                        if (kp.Key == '$') 
                            continue;
                        if (kp.Key == '_') 
                        {
                            if (!char.IsDigit(suffix[0])) 
                                this._getAllObjIdsAfterSuffix(kp.Value, suffix, street, res);
                            continue;
                        }
                        if (char.IsDigit(kp.Key) && char.IsDigit(suffix[0])) 
                        {
                            if (kp.Key == suffix[0]) 
                                this._getAllObjIdsAfterSuffix(kp.Value, suffix.Substring(1), street, res);
                            continue;
                        }
                    }
                    else if (kp.Key == '$') 
                    {
                        this._getAllObjIdsTotal(kp.Value, res);
                        continue;
                    }
                    else if (kp.Key == '@') 
                    {
                        if (!kp.Value.Loaded) 
                            this._loadNode(kp.Value);
                        if (kp.Value.ObjIds != null) 
                            res.AddRange(kp.Value.ObjIds);
                    }
                    this.GetAllObjIds(kp.Value, suffix, street, res);
                }
        }
        void _getAllObjIdsAfterSuffix(AreaTreeNode n, string suffix, bool street, List<int> res)
        {
            for (int i = 0; i < suffix.Length; i++) 
            {
                if (!n.Loaded) 
                    this._loadNode(n);
                AreaTreeNode tn;
                if (n.Children == null) 
                    return;
                if (!n.Children.TryGetValue(suffix[i], out tn)) 
                    return;
                n = tn;
            }
            if (n != null) 
                this.GetAllObjIds(n, null, street, res);
        }
        void _loadNode(AreaTreeNode res)
        {
            if (!res.Loaded && res.LazyPos > 0) 
                res.Deserialize(m_Data, res.LazyPos);
            res.Loaded = true;
        }
        public void Save(string fname)
        {
            if (m_Data != null) 
                m_Data = null;
            m_Strings.Clear();
            foreach (AreaTreeObject o in m_Objs) 
            {
                if (o != null) 
                {
                    if (o.Typs != null) 
                    {
                        foreach (string ty in o.Typs) 
                        {
                            this.GetStringId(ty);
                        }
                    }
                    if (o.Miscs != null) 
                    {
                        foreach (string mi in o.Miscs) 
                        {
                            this.GetStringId(mi);
                        }
                    }
                }
            }
            using (FileStream f = new FileStream(fname, FileMode.Create, FileAccess.Write)) 
            {
                Pullenti.Address.Internal.FiasHelper.SerializeInt(f, 0);
                Pullenti.Address.Internal.FiasHelper.SerializeInt(f, 0);
                Pullenti.Address.Internal.FiasHelper.SerializeInt(f, m_Strings.Count);
                foreach (string s in m_Strings) 
                {
                    Pullenti.Address.Internal.FiasHelper.SerializeString(f, s, false);
                }
                Pullenti.Address.Internal.FiasHelper.SerializeInt(f, m_Objs.Count);
                int pos0 = (int)f.Position;
                for (int i = 0; i < m_Objs.Count; i++) 
                {
                    Pullenti.Address.Internal.FiasHelper.SerializeInt(f, 0);
                }
                for (int i = 0; i < m_Objs.Count; i++) 
                {
                    if (m_Objs[i] != null) 
                    {
                        m_ObjPos[i] = (int)f.Position;
                        m_Objs[i].Serialize(f, this);
                    }
                }
                f.Position = pos0;
                for (int i = 0; i < m_ObjPos.Count; i++) 
                {
                    Pullenti.Address.Internal.FiasHelper.SerializeInt(f, m_ObjPos[i]);
                }
                f.Position = 4;
                Pullenti.Address.Internal.FiasHelper.SerializeInt(f, (int)f.Length);
                f.Position = f.Length;
                Pullenti.Address.Internal.FiasHelper.SerializeInt(f, Children.Count);
                foreach (KeyValuePair<char, AreaTreeNode> kp in Children) 
                {
                    Pullenti.Address.Internal.FiasHelper.SerializeShort(f, (short)kp.Key);
                    kp.Value.Serialize(f);
                }
            }
            this.Close();
            GC.Collect();
        }
        public void Load(string fname)
        {
            m_Data = Pullenti.Util.FileHelper.LoadDataFromFile(fname, 0);
            int pos = 0;
            int a0 = BitConverter.ToInt32(m_Data, pos);
            pos += 4;
            int pos0 = BitConverter.ToInt32(m_Data, pos);
            pos += 4;
            int cou = BitConverter.ToInt32(m_Data, pos);
            pos += 4;
            m_Strings.Clear();
            for (; cou > 0; cou--) 
            {
                string s = Pullenti.Address.Internal.FiasHelper.DeserializeStringFromBytes(m_Data, ref pos, false);
                m_Strings.Add(s);
            }
            cou = BitConverter.ToInt32(m_Data, pos);
            pos += 4;
            if (cou > 0) 
            {
                m_Objs = new List<AreaTreeObject>(cou);
                m_ObjPos = new List<int>(cou);
                for (; cou > 0; cou--,pos += 4) 
                {
                    m_ObjPos.Add(BitConverter.ToInt32(m_Data, pos));
                    m_Objs.Add(null);
                }
            }
            pos = pos0;
            cou = BitConverter.ToInt32(m_Data, pos);
            pos += 4;
            if (cou == 0) 
                return;
            for (int i = 0; i < cou; i++) 
            {
                char ch = (char)BitConverter.ToInt16(m_Data, pos);
                pos += 2;
                AreaTreeNode tn = new AreaTreeNode();
                pos = tn.Deserialize(m_Data, pos);
                Children.Add(ch, tn);
            }
        }
    }
}