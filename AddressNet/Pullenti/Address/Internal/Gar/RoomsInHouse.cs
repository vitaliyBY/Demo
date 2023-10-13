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
    public class RoomsInHouse
    {
        List<RoomObject> m_Rooms = null;
        Dictionary<string, List<int>> Refs = new Dictionary<string, List<int>>();
        byte[] m_Data;
        int[] m_RoomsPos;
        public object m_Lock = new object();
        public int Count
        {
            get
            {
                return (m_Rooms != null ? m_Rooms.Count : (m_RoomsPos != null ? m_RoomsPos.Length : 0));
            }
        }
        public override string ToString()
        {
            return string.Format("{0} Rooms, {1} refs", Count, Refs.Count);
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
            m_RoomsPos = new int[(int)cou];
            for (int i = 0; i < cou; i++,ind += 4) 
            {
                m_RoomsPos[i] = BitConverter.ToInt32(dat, ind);
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
        public List<RoomObject> GetRooms(Pullenti.Address.Internal.NumberAnalyzer na)
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
            List<RoomObject> res = new List<RoomObject>();
            foreach (int i in inds) 
            {
                if ((i < 1) || i > m_RoomsPos.Length) 
                    continue;
                RoomObject ho = new RoomObject();
                ho.Id = BitConverter.ToInt32(m_Data, m_RoomsPos[i - 1]);
                int ii = m_RoomsPos[i - 1] + 4;
                if (FiasRoomTable._restore(m_Data, ho, ref ii)) 
                    res.Add(ho);
            }
            return res;
        }
        public bool CheckHasFlatsAndSpaces()
        {
            bool hasFlats = false;
            bool hasSpaces = false;
            for (int k = 0; k < m_RoomsPos.Length; k++) 
            {
                Pullenti.Address.RoomType ty = FiasRoomTable._getRoomTyp(m_Data, m_RoomsPos[k] + 4);
                if (ty == Pullenti.Address.RoomType.Flat) 
                    hasFlats = true;
                if (ty == Pullenti.Address.RoomType.Space) 
                    hasSpaces = true;
                if (hasFlats && hasSpaces) 
                    return true;
            }
            return false;
        }
        public byte[] Save()
        {
            if (m_Rooms == null || m_Rooms.Count == 0) 
                return null;
            using (MemoryStream mem = new MemoryStream()) 
            {
                Pullenti.Address.Internal.FiasHelper.SerializeInt(mem, 0);
                Pullenti.Address.Internal.FiasHelper.SerializeInt(mem, m_Rooms.Count);
                Pullenti.Address.Internal.FiasHelper.SerializeInt(mem, 0);
                for (int i = 0; i < m_Rooms.Count; i++) 
                {
                    Pullenti.Address.Internal.FiasHelper.SerializeInt(mem, 0);
                }
                m_RoomsPos = new int[(int)m_Rooms.Count];
                List<byte> buf = new List<byte>();
                for (int i = 0; i < m_Rooms.Count; i++) 
                {
                    m_RoomsPos[i] = (int)mem.Position;
                    Pullenti.Address.Internal.FiasHelper.SerializeInt(mem, m_Rooms[i].Id);
                    buf.Clear();
                    FiasRoomTable._store(buf, m_Rooms[i]);
                    byte[] dat = buf.ToArray();
                    mem.Write(dat, 0, dat.Length);
                }
                int pos = (int)mem.Position;
                mem.Position = 8;
                Pullenti.Address.Internal.FiasHelper.SerializeInt(mem, pos);
                for (int i = 0; i < m_RoomsPos.Length; i++) 
                {
                    Pullenti.Address.Internal.FiasHelper.SerializeInt(mem, m_RoomsPos[i]);
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
        public bool AddRO(RoomObject ho)
        {
            Pullenti.Address.Internal.NumberAnalyzer num = Pullenti.Address.Internal.NumberAnalyzer.TryParseRO(ho);
            if (num == null || num.Items.Count == 0) 
                return false;
            if (m_Rooms == null) 
                m_Rooms = new List<RoomObject>();
            m_Rooms.Add(ho);
            int ind = m_Rooms.Count;
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