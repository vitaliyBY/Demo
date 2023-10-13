/*
 * SDK Pullenti Address, version 4.20, august 2023. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Collections;
using System.Collections.Generic;

namespace Pullenti.Address.Internal
{
    class RepObjTable : Pullenti.Util.Repository.KeyBaseTable
    {
        internal RepObjTable(RepTypTable typs, string baseDir, string name = "objs") : base(null, name, baseDir)
        {
            m_Typs = typs;
        }
        RepTypTable m_Typs;
        public void Add(int id, Pullenti.Address.RepaddrObject r)
        {
            r.Id = id;
            List<byte> res = new List<byte>();
            this._store(res, r);
            byte[] dat = res.ToArray();
            this.WriteKeyData(id, dat);
        }
        void _store(List<byte> res, Pullenti.Address.RepaddrObject ao)
        {
            byte attr = (byte)0;
            res.Add(attr);
            res.Add((byte)((int)ao.Level));
            Pullenti.Util.Repository.BaseTable.GetBytesForString(res, ao.Spelling, null);
            res.AddRange(BitConverter.GetBytes((short)ao.Types.Count));
            foreach (string ty in ao.Types) 
            {
                res.AddRange(BitConverter.GetBytes((short)m_Typs.GetId(ty)));
            }
            if (ao.Parents == null || ao.Parents.Count == 0) 
                res.AddRange(BitConverter.GetBytes((short)0));
            else 
            {
                res.AddRange(BitConverter.GetBytes((short)ao.Parents.Count));
                foreach (int p in ao.Parents) 
                {
                    res.AddRange(BitConverter.GetBytes(p));
                }
            }
            if (ao.GarGuids == null || ao.GarGuids.Count == 0) 
                res.AddRange(BitConverter.GetBytes((short)0));
            else 
            {
                res.AddRange(BitConverter.GetBytes((short)ao.GarGuids.Count));
                foreach (string p in ao.GarGuids) 
                {
                    Pullenti.Util.Repository.BaseTable.GetBytesForString(res, p, null);
                }
            }
        }
        public Pullenti.Address.RepaddrObject Get(int id)
        {
            byte[] dat = this.ReadKeyData(id, 0);
            if (dat == null) 
                return null;
            Pullenti.Address.RepaddrObject r = new Pullenti.Address.RepaddrObject();
            r.Id = id;
            int ind = 0;
            this._restore(dat, r, ref ind);
            return r;
        }
        bool _restore(byte[] data, Pullenti.Address.RepaddrObject ao, ref int ind)
        {
            byte attr = data[ind];
            ind++;
            int cou = (int)data[ind];
            ind++;
            ao.Level = (Pullenti.Address.AddrLevel)cou;
            ao.Spelling = Pullenti.Util.Repository.BaseTable.GetStringForBytes(data, ref ind, false, null);
            cou = BitConverter.ToInt16(data, ind);
            ind += 2;
            for (; cou > 0; cou--) 
            {
                int id = (int)BitConverter.ToInt16(data, ind);
                ind += 2;
                string ty = m_Typs.GetTyp(id);
                if (ty != null) 
                    ao.Types.Add(ty);
            }
            cou = BitConverter.ToInt16(data, ind);
            ind += 2;
            for (; cou > 0; cou--) 
            {
                if (ao.Parents == null) 
                    ao.Parents = new List<int>();
                ao.Parents.Add(BitConverter.ToInt32(data, ind));
                ind += 4;
            }
            cou = BitConverter.ToInt16(data, ind);
            ind += 2;
            for (; cou > 0; cou--) 
            {
                if (ao.GarGuids == null) 
                    ao.GarGuids = new List<string>();
                string s = Pullenti.Util.Repository.BaseTable.GetStringForBytes(data, ref ind, false, null);
                ao.GarGuids.Add(s);
            }
            return true;
        }
    }
}