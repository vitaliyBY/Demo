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

namespace Pullenti.Util.Repository
{
    public class FixRecordBaseTable : KeyBaseTable
    {
        public FixRecordBaseTable(IRepository index, string name) : base(index, name, null)
        {
        }
        public int RecordSize
        {
            get
            {
                return m_FixRecordFields.Count * 4;
            }
        }
        public int MaxCount = 0;
        public int MaxCountFloatIndex;
        /// <summary>
        /// Описание поля фиксированной записи
        /// </summary>
        public class FieldDefinition
        {
            public bool IsKey;
            public bool IsFloat;
            public bool MergeAdd;
            public string Name;
            public override string ToString()
            {
                StringBuilder res = new StringBuilder();
                if (Name != null) 
                    res.AppendFormat("{0} ", Name);
                if (IsKey) 
                    res.Append("Key ");
                res.Append((IsFloat ? "Float" : "Int"));
                if (MergeAdd) 
                    res.Append(" MergeAdd");
                return res.ToString();
            }
        }

        protected List<FieldDefinition> m_FixRecordFields = new List<FieldDefinition>();
        public override long RecordsCount
        {
            get
            {
                if (m_Data != null) 
                    return m_Data.Length / RecordSize;
                FileInfo fi = new FileInfo(m_DataFileName);
                if (fi.Exists) 
                    return fi.Length / RecordSize;
                return 0;
            }
        }
        public override void _Close()
        {
            m_LastReadedFixRecords = null;
            try 
            {
                this._saveBuffer();
            }
            catch(Exception ex) 
            {
            }
            base._Close();
        }
        public override void Flush()
        {
            try 
            {
                this._saveBuffer();
            }
            catch(Exception ex) 
            {
            }
            base.Flush();
        }
        protected void AddFixRecord(int key, int i1, int i2, int i3, float f1, float f2, float f3, int i4 = 0)
        {
            if (m_LastReadedFixRecords != null && m_LastReadedFixRecords.BaseKey == key) 
                m_LastReadedFixRecords = null;
            if (m_AddBuffer != null && m_AddBuffer.BaseKey != key) 
                this._saveBuffer();
            if (m_AddBuffer == null) 
                m_AddBuffer = new FixRecordsBuffer(key, m_FixRecordFields);
            if (!m_AddBuffer.Add(i1, i2, i3, f1, f2, f3, i4)) 
            {
                this._saveBuffer();
                m_AddBuffer = new FixRecordsBuffer(key, m_FixRecordFields);
                m_AddBuffer.Add(i1, i2, i3, f1, f2, f3, i4);
            }
        }
        FixRecordsBuffer m_AddBuffer;
        public FixRecordsBuffer GetFixRecords(int key)
        {
            if (m_AddBuffer != null) 
            {
                if (key == m_AddBuffer.BaseKey) 
                    this._saveBuffer();
            }
            if (m_LastReadedFixRecords != null && m_LastReadedFixRecords.BaseKey == key) 
                return m_LastReadedFixRecords;
            FixRecordsBuffer res = new FixRecordsBuffer(key, m_FixRecordFields);
            byte[] data = this.ReadKeyData(key, 0);
            if (data != null) 
                res.Restore(data);
            m_LastReadedFixRecords = res;
            return res;
        }
        FixRecordsBuffer m_LastReadedFixRecords;
        public void SaveFixRecords(FixRecordsBuffer buf)
        {
            m_LastReadedFixRecords = null;
            this.WriteKeyData(buf.BaseKey, buf.GetBytesArray());
        }
        void _saveBuffer()
        {
            if (m_AddBuffer == null) 
                return;
            byte[] exData = this.ReadKeyData(m_AddBuffer.BaseKey, 0);
            if (exData == null || exData.Length == 0) 
            {
                if (MaxCount > 0 && (MaxCount < m_AddBuffer.Count)) 
                    m_AddBuffer.Cut(MaxCount, MaxCountFloatIndex);
                this.WriteKeyData(m_AddBuffer.BaseKey, m_AddBuffer.GetBytesArray());
                m_AddBuffer = null;
                return;
            }
            FixRecordsBuffer exRecs = new FixRecordsBuffer(m_AddBuffer.BaseKey, m_FixRecordFields);
            exRecs.Restore(exData);
            List<byte> res = exRecs.MergeWith(m_AddBuffer);
            byte[] bytes = null;
            if (MaxCount > 0) 
            {
                int cou = res.Count / exRecs.RecordSize;
                if (cou > MaxCount) 
                {
                    FixRecordsBuffer tmp = new FixRecordsBuffer(m_AddBuffer.BaseKey, m_FixRecordFields);
                    tmp.Restore(res.ToArray());
                    tmp.Cut(MaxCount, MaxCountFloatIndex);
                    bytes = tmp.GetBytesArray();
                }
            }
            if (bytes == null) 
                bytes = res.ToArray();
            this.WriteKeyData(m_AddBuffer.BaseKey, bytes);
            m_AddBuffer = null;
        }
        public class FixRecordsBuffer
        {
            internal FixRecordsBuffer(int baseKey, List<Pullenti.Util.Repository.FixRecordBaseTable.FieldDefinition> fields)
            {
                BaseKey = baseKey;
                KeyCount = 0;
                RecordSize = 0;
                Fields = fields;
                foreach (Pullenti.Util.Repository.FixRecordBaseTable.FieldDefinition f in fields) 
                {
                    if (f.IsFloat) 
                    {
                        if (m_Floats == null) 
                            m_Floats = new List<List<float>>();
                        m_Floats.Add(new List<float>());
                    }
                    else 
                    {
                        if (m_Ints == null) 
                            m_Ints = new List<List<int>>();
                        m_Ints.Add(new List<int>());
                        if (f.IsKey) 
                            KeyCount++;
                    }
                    RecordSize += 4;
                }
            }
            public int BaseKey;
            internal int RecordSize;
            internal int KeyCount;
            public List<Pullenti.Util.Repository.FixRecordBaseTable.FieldDefinition> Fields;
            List<List<int>> m_Ints;
            List<List<float>> m_Floats;
            public int Count
            {
                get
                {
                    return (m_Ints == null ? 0 : m_Ints[0].Count);
                }
            }
            public int Find(int i1)
            {
                return (m_Ints == null ? -1 : m_Ints[0].IndexOf(i1));
            }
            public float GetFloat(int ind, int i)
            {
                if (m_Floats == null || (ind < 0) || ind >= m_Floats.Count) 
                    return 0;
                if ((i < 0) || i >= m_Floats[ind].Count) 
                    return 0;
                else 
                    return m_Floats[ind][i];
            }
            public int GetInt(int ind, int i)
            {
                if (m_Ints == null || (ind < 0) || ind >= m_Ints.Count) 
                    return 0;
                if ((i < 0) || i >= m_Ints[ind].Count) 
                    return 0;
                else 
                    return m_Ints[ind][i];
            }
            public void SetFloat(int ind, int i, float val)
            {
                if ((m_Floats == null || (ind < 0) || ind >= m_Floats.Count) || (i < 0)) 
                    return;
                if (i == m_Floats[ind].Count) 
                    m_Floats[ind].Add(val);
                else 
                    m_Floats[ind][i] = val;
            }
            public void SetInt(int ind, int i, int val)
            {
                if ((m_Ints == null || (ind < 0) || ind >= m_Ints.Count) || (i < 0)) 
                    return;
                if (i == m_Ints[ind].Count) 
                    m_Ints[ind].Add(val);
                else 
                    m_Ints[ind][i] = val;
            }
            public byte[] GetBytesArray()
            {
                List<byte> res = new List<byte>(RecordSize * Count);
                for (int i = 0; i < Count; i++) 
                {
                    this.GetBytes(i, res);
                }
                return res.ToArray();
            }
            public void GetBytes(int i, List<byte> res)
            {
                if (m_Ints != null) 
                {
                    for (int j = 0; j < m_Ints.Count; j++) 
                    {
                        res.AddRange(BitConverter.GetBytes(m_Ints[j][i]));
                    }
                }
                if (m_Floats != null) 
                {
                    for (int j = 0; j < m_Floats.Count; j++) 
                    {
                        res.AddRange(BitConverter.GetBytes(m_Floats[j][i]));
                    }
                }
            }
            internal bool Add(int i1, int i2, int i3, float f1, float f2, float f3, int i4)
            {
                if (Count > 0) 
                {
                    if (this.CompareWith(Count - 1, i1, i2) >= 0) 
                        return false;
                }
                if (m_Ints != null) 
                {
                    m_Ints[0].Add(i1);
                    if (m_Ints.Count > 1) 
                        m_Ints[1].Add(i2);
                    if (m_Ints.Count > 2) 
                        m_Ints[2].Add(i3);
                    if (m_Ints.Count > 3) 
                        m_Ints[3].Add(i4);
                }
                if (m_Floats != null) 
                {
                    m_Floats[0].Add(f1);
                    if (m_Floats.Count > 1) 
                        m_Floats[1].Add(f2);
                    if (m_Floats.Count > 2) 
                        m_Floats[2].Add(f3);
                }
                return true;
            }
            internal void Remove(int ind)
            {
                if (m_Ints != null) 
                {
                    foreach (List<int> li in m_Ints) 
                    {
                        li.RemoveAt(ind);
                    }
                }
                if (m_Floats != null) 
                {
                    foreach (List<float> li in m_Floats) 
                    {
                        li.RemoveAt(ind);
                    }
                }
            }
            internal void Restore(byte[] data)
            {
                int cou = data.Length / RecordSize;
                this._Clear(cou);
                int ind = 0;
                for (int i = 0; i < cou; i++) 
                {
                    if (m_Ints != null) 
                    {
                        for (int j = 0; j < m_Ints.Count; j++) 
                        {
                            m_Ints[j].Add(BitConverter.ToInt32(data, ind));
                            ind += 4;
                        }
                    }
                    if (m_Floats != null) 
                    {
                        for (int j = 0; j < m_Floats.Count; j++) 
                        {
                            m_Floats[j].Add(BitConverter.ToSingle(data, ind));
                            ind += 4;
                        }
                    }
                }
            }
            public bool Cut(int maxCount, int floatInd)
            {
                if (Count <= maxCount) 
                    return false;
                if ((floatInd < 0) || floatInd >= m_Floats.Count) 
                    return false;
                List<Pullenti.Util.Repository.FixRecordBaseTable.Temp> li = new List<Pullenti.Util.Repository.FixRecordBaseTable.Temp>();
                for (int i = 0; i < m_Floats[floatInd].Count; i++) 
                {
                    li.Add(new Pullenti.Util.Repository.FixRecordBaseTable.Temp() { Ind = i, Val = m_Floats[floatInd][i] });
                }
                // PYTHON: sort(key=attrgetter('val'))
                li.Sort();
                List<int> inds = new List<int>(li.Count - maxCount);
                for (int i = li.Count - 1; i >= maxCount; i--) 
                {
                    inds.Add(li[i].Ind);
                }
                inds.Sort();
                for (int i = inds.Count - 1; i >= 0; i--) 
                {
                    this.Remove(inds[i]);
                }
                return true;
            }
            void _Clear(int capacity)
            {
                if (m_Ints != null) 
                {
                    foreach (List<int> li in m_Ints) 
                    {
                        li.Clear();
                        if (li.Capacity < capacity) 
                            li.Capacity = capacity;
                    }
                }
                if (m_Floats != null) 
                {
                    foreach (List<float> li in m_Floats) 
                    {
                        li.Clear();
                        if (li.Capacity < capacity) 
                            li.Capacity = capacity;
                    }
                }
            }
            internal int CompareWithBuf(int ind, FixRecordsBuffer rd, int rbInd)
            {
                if (m_Ints == null) 
                    return 0;
                if (m_Ints[0][ind] < rd.m_Ints[0][rbInd]) 
                    return -1;
                if (m_Ints[0][ind] > rd.m_Ints[0][rbInd]) 
                    return 1;
                if ((KeyCount < 2) || (m_Ints.Count < 2)) 
                    return 0;
                if (m_Ints[1][ind] < rd.m_Ints[1][rbInd]) 
                    return -1;
                if (m_Ints[1][ind] > rd.m_Ints[1][rbInd]) 
                    return 1;
                return 0;
            }
            internal int CompareWith(int ind, int i1, int i2)
            {
                if (m_Ints == null) 
                    return 0;
                if (m_Ints[0][ind] < i1) 
                    return -1;
                if (m_Ints[0][ind] > i1) 
                    return 1;
                if ((KeyCount < 2) || (m_Ints.Count < 2)) 
                    return 0;
                if (m_Ints[1][ind] < i2) 
                    return -1;
                if (m_Ints[1][ind] > i2) 
                    return 1;
                return 0;
            }
            public bool Check()
            {
                for (int i = 0; i < (Count - 1); i++) 
                {
                    int cmp = this.CompareWithBuf(i, this, i + 1);
                    if (cmp >= 0) 
                        return false;
                }
                return true;
            }
            public static FixRecordsBuffer Merge(FixRecordsBuffer buf1, FixRecordsBuffer buf2)
            {
                List<byte> buf = buf1.MergeWith(buf2);
                FixRecordsBuffer res = new FixRecordsBuffer(buf1.BaseKey, buf1.Fields);
                res.Restore(buf.ToArray());
                return res;
            }
            internal List<byte> MergeWith(FixRecordsBuffer buf)
            {
                int cou = Count + ((buf.Count / 2));
                int i = ((Count / 2)) + buf.Count;
                if (cou < i) 
                    cou = i;
                List<byte> res = new List<byte>(cou * RecordSize);
                i = 0;
                int j = 0;
                while ((i < Count) || (j < buf.Count)) 
                {
                    if (i >= Count) 
                    {
                        buf.GetBytes(j, res);
                        j++;
                        continue;
                    }
                    if (j >= buf.Count) 
                    {
                        this.GetBytes(i, res);
                        i++;
                        continue;
                    }
                    int cmp = this.CompareWithBuf(i, buf, j);
                    if (cmp < 0) 
                    {
                        this.GetBytes(i, res);
                        i++;
                        continue;
                    }
                    if (cmp > 0) 
                    {
                        buf.GetBytes(j, res);
                        j++;
                        continue;
                    }
                    int ii = 0;
                    int fi = 0;
                    for (int ff = 0; ff < Fields.Count; ff++) 
                    {
                        if (Fields[ff].IsFloat) 
                        {
                            float f = buf.GetFloat(fi, j);
                            if (Fields[ff].MergeAdd) 
                                this.SetFloat(fi, i, this.GetFloat(fi, i) + f);
                            else if (f > 0) 
                                this.SetFloat(fi, i, f);
                            fi++;
                        }
                        else 
                        {
                            if (Fields[ff].MergeAdd) 
                                this.SetInt(ii, i, this.GetInt(ii, i) + buf.GetInt(ii, j));
                            ii++;
                        }
                    }
                    this.GetBytes(i, res);
                    i++;
                    j++;
                }
                return res;
            }
        }

        class Temp : IComparable
        {
            public int Ind;
            public float Val;
            public int CompareTo(object obj)
            {
                float f = (obj as Temp).Val;
                if (Val > f) 
                    return -1;
                if (Val < f) 
                    return 1;
                return 0;
            }
        }

        public void BeginFetchAllFixRecordsBuffer(int firstId = 1)
        {
            m_FetchLastId = firstId - 1;
        }
        int m_FetchLastId = 0;
        public List<FixRecordsBuffer> FetchFixRecordsBuffer(int maxCount = 1000)
        {
            m_FetchLastId++;
            if (m_FetchLastId > this.GetMaxKey()) 
                return null;
            Dictionary<int, byte[]> dats = this.ReadKeysData(m_FetchLastId, maxCount, 10000000);
            List<FixRecordsBuffer> res = new List<FixRecordsBuffer>();
            if (dats == null) 
            {
                m_FetchLastId += maxCount;
                return res;
            }
            foreach (KeyValuePair<int, byte[]> kp in dats) 
            {
                if (kp.Value != null) 
                {
                    FixRecordsBuffer buf = new FixRecordsBuffer(kp.Key, m_FixRecordFields);
                    buf.Restore(kp.Value);
                    res.Add(buf);
                    m_FetchLastId = kp.Key;
                }
            }
            return res;
        }
    }
}