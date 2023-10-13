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
    public class HousesInStreet
    {
        List<HouseObject> m_Houses = null;
        Dictionary<string, List<int>> Refs = new Dictionary<string, List<int>>();
        byte[] m_Data;
        int[] m_HousesPos;
        public object m_Lock = new object();
        public override string ToString()
        {
            return string.Format("{0} houses, {1} refs", (m_Houses != null ? m_Houses.Count : (m_HousesPos != null ? m_HousesPos.Length : 0)), Refs.Count);
        }
        public void Load(byte[] dat)
        {
            if (dat == null || (dat.Length < 8)) 
                return;
            m_Data = dat;
            int ind = 4;
            int cou = BitConverter.ToInt32(dat, ind);
            ind += 4;
            int pos0 = BitConverter.ToInt32(dat, ind);
            ind += 4;
            m_HousesPos = new int[(int)cou];
            for (int i = 0; i < cou; i++,ind += 4) 
            {
                m_HousesPos[i] = BitConverter.ToInt32(dat, ind);
            }
            ind = pos0;
            cou = BitConverter.ToInt32(dat, ind);
            ind += 4;
            Refs.Clear();
            for (int i = 0; i < cou; i++) 
            {
                string key = Pullenti.Address.Internal.FiasHelper.DeserializeStringFromBytes(dat, ref ind, false);
                short cou2 = BitConverter.ToInt16(dat, ind);
                ind += 2;
                List<int> li = new List<int>();
                for (int j = 0; j < cou2; j++,ind += 2) 
                {
                    li.Add(BitConverter.ToInt16(dat, ind));
                }
                Refs.Add(key, li);
            }
        }
        public List<HouseObject> GetHouses(Pullenti.Address.Internal.NumberAnalyzer na)
        {
            List<int> inds = new List<int>();
            foreach (Pullenti.Address.Internal.NumberItem it in na.Items) 
            {
                List<int> li;
                if (!Refs.TryGetValue(it.Value, out li)) 
                    continue;
                if (inds.Count == 0) 
                    inds.AddRange(li);
                else 
                    foreach (int v in li) 
                    {
                        if (!inds.Contains(v)) 
                            inds.Add(v);
                    }
            }
            if (inds.Count == 0) 
                return null;
            List<HouseObject> res = new List<HouseObject>();
            foreach (int i in inds) 
            {
                if ((i < 1) || i > m_HousesPos.Length) 
                    continue;
                HouseObject ho = new HouseObject();
                ho.Id = BitConverter.ToInt32(m_Data, m_HousesPos[i - 1]);
                if (FiasHouseTable._restore(m_Data, ho, m_HousesPos[i - 1] + 4)) 
                    res.Add(ho);
            }
            return res;
        }
        public byte[] Save()
        {
            if (m_Houses == null || m_Houses.Count == 0) 
                return null;
            using (MemoryStream mem = new MemoryStream()) 
            {
                Pullenti.Address.Internal.FiasHelper.SerializeInt(mem, 0);
                Pullenti.Address.Internal.FiasHelper.SerializeInt(mem, m_Houses.Count);
                Pullenti.Address.Internal.FiasHelper.SerializeInt(mem, 0);
                for (int i = 0; i < m_Houses.Count; i++) 
                {
                    Pullenti.Address.Internal.FiasHelper.SerializeInt(mem, 0);
                }
                m_HousesPos = new int[(int)m_Houses.Count];
                for (int i = 0; i < m_Houses.Count; i++) 
                {
                    m_HousesPos[i] = (int)mem.Position;
                    Pullenti.Address.Internal.FiasHelper.SerializeInt(mem, m_Houses[i].Id);
                    byte[] dat = FiasHouseTable._store(m_Houses[i]);
                    mem.Write(dat, 0, dat.Length);
                }
                int pos = (int)mem.Position;
                mem.Position = 8;
                Pullenti.Address.Internal.FiasHelper.SerializeInt(mem, pos);
                for (int i = 0; i < m_HousesPos.Length; i++) 
                {
                    Pullenti.Address.Internal.FiasHelper.SerializeInt(mem, m_HousesPos[i]);
                }
                mem.Position = mem.Length;
                Pullenti.Address.Internal.FiasHelper.SerializeInt(mem, Refs.Count);
                foreach (KeyValuePair<string, List<int>> r in Refs) 
                {
                    Pullenti.Address.Internal.FiasHelper.SerializeString(mem, r.Key, false);
                    Pullenti.Address.Internal.FiasHelper.SerializeShort(mem, r.Value.Count);
                    foreach (int v in r.Value) 
                    {
                        Pullenti.Address.Internal.FiasHelper.SerializeShort(mem, v);
                    }
                }
                return mem.ToArray();
            }
        }
        public bool AddHO(HouseObject ho)
        {
            Pullenti.Address.Internal.NumberAnalyzer num = Pullenti.Address.Internal.NumberAnalyzer.TryParseHO(ho);
            if (num == null || num.Items.Count == 0) 
                return false;
            if (m_Houses == null) 
                m_Houses = new List<HouseObject>();
            m_Houses.Add(ho);
            int ind = m_Houses.Count;
            foreach (Pullenti.Address.Internal.NumberItem it in num.Items) 
            {
                List<int> li;
                if (!Refs.TryGetValue(it.Value, out li)) 
                    Refs.Add(it.Value, (li = new List<int>()));
                if (!li.Contains(ind)) 
                    li.Add(ind);
            }
            return true;
        }
    }
}