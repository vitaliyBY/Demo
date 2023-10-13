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
    class AreaTreeNode
    {
        public List<int> ObjIds = null;
        public AreaTreeNode Parent;
        public Dictionary<char, AreaTreeNode> Children = null;
        public int LazyPos;
        public bool Loaded;
        public override string ToString()
        {
            return string.Format("Objs={0}, Chils={1}{2}", (ObjIds == null ? 0 : ObjIds.Count), (Children == null ? 0 : Children.Count), (Loaded ? " (loaded)" : ""));
        }
        public void Serialize(Stream f)
        {
            Pullenti.Address.Internal.FiasHelper.SerializeInt(f, (ObjIds == null ? 0 : ObjIds.Count));
            if (ObjIds != null) 
            {
                foreach (int v in ObjIds) 
                {
                    Pullenti.Address.Internal.FiasHelper.SerializeInt(f, v);
                }
            }
            Pullenti.Address.Internal.FiasHelper.SerializeShort(f, (short)((Children == null ? 0 : Children.Count)));
            if (Children != null) 
            {
                foreach (KeyValuePair<char, AreaTreeNode> kp in Children) 
                {
                    Pullenti.Address.Internal.FiasHelper.SerializeShort(f, (short)kp.Key);
                    int p0 = (int)f.Position;
                    Pullenti.Address.Internal.FiasHelper.SerializeInt(f, 0);
                    kp.Value.Serialize(f);
                    int p1 = (int)f.Position;
                    f.Position = p0;
                    Pullenti.Address.Internal.FiasHelper.SerializeInt(f, p1);
                    f.Position = p1;
                }
            }
        }
        public int Deserialize(byte[] dat, int pos)
        {
            int cou = BitConverter.ToInt32(dat, pos);
            pos += 4;
            if (cou > 0x7000 || (cou < 0)) 
            {
            }
            if (cou > 0) 
            {
                ObjIds = new List<int>(cou);
                for (int i = 0; i < cou; i++,pos += 4) 
                {
                    ObjIds.Add(BitConverter.ToInt32(dat, pos));
                }
            }
            cou = BitConverter.ToInt16(dat, pos);
            pos += 2;
            if (cou == 0) 
                return pos;
            if (cou > 0x1000 || (cou < 0)) 
            {
            }
            for (int i = 0; i < cou; i++) 
            {
                char ch = (char)BitConverter.ToInt16(dat, pos);
                pos += 2;
                int p1 = BitConverter.ToInt32(dat, pos);
                pos += 4;
                AreaTreeNode tn = new AreaTreeNode();
                tn.LazyPos = pos;
                tn.Loaded = false;
                if (Children == null) 
                    Children = new Dictionary<char, AreaTreeNode>();
                Children.Add(ch, tn);
                tn.Parent = this;
                pos = p1;
            }
            Loaded = true;
            return pos;
        }
    }
}