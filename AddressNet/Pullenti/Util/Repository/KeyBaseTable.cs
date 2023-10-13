/*
 * SDK Pullenti Address, version 4.20, august 2023. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;

namespace Pullenti.Util.Repository
{
    public class KeyBaseTable : BaseTable
    {
        public KeyBaseTable(IRepository index, string name, string baseDir = null) : base(index)
        {
            if (Index != null) 
            {
                m_IndexFileName = Path.Combine(Index.BaseDir, name + ".ind");
                m_DataFileName = Path.Combine(Index.BaseDir, name + ".dat");
            }
            else if (baseDir != null) 
            {
                m_IndexFileName = Path.Combine(baseDir, name + ".ind");
                m_DataFileName = Path.Combine(baseDir, name + ".dat");
            }
            Name = name;
        }
        public bool AutoZipData = false;
        public bool LoadAllInMemory = false;
        public bool IsExists
        {
            get
            {
                if (!File.Exists(m_DataFileName)) 
                    return false;
                if (!File.Exists(m_IndexFileName)) 
                    return false;
                return true;
            }
        }
        public void Remove()
        {
            this._Close();
            if (File.Exists(m_DataFileName)) 
                File.Delete(m_DataFileName);
            if (File.Exists(m_IndexFileName)) 
                File.Delete(m_IndexFileName);
        }
        public override bool Backup(string path)
        {
            this._Close();
            if (!BaseTable.BackupFile(m_IndexFileName, path)) 
                return false;
            if (!BaseTable.BackupFile(m_DataFileName, path)) 
                return false;
            return base.Backup(path);
        }
        public override bool Restore(string path, bool remove)
        {
            this._Close();
            if (!BaseTable.RestoreFile(m_IndexFileName, path, remove)) 
                return false;
            if (!BaseTable.RestoreFile(m_DataFileName, path, remove)) 
                return false;
            return base.Restore(path, remove);
        }
        public override void _Close()
        {
            if (m_Data != null) 
            {
                m_Data.Dispose();
                m_Data = null;
            }
            if (m_Index != null) 
            {
                m_Index.Dispose();
                m_Index = null;
            }
            m_IndexBuf = null;
            m_DataBuf = null;
        }
        public override void Flush()
        {
            base.Flush();
            if (m_Data != null) 
                m_Data.Flush();
            if (m_Index != null) 
                m_Index.Flush();
        }
        const int IndexRecordSize = 12;
        public override long RecordsCount
        {
            get
            {
                return this.GetMaxKey();
            }
        }
        protected int IndexStreamBufSize = -1;
        protected int DataStreamBufSize = -1;
        public override long Size
        {
            get
            {
                if (m_Data != null) 
                    return m_Data.Length + m_Index.Length;
                long res = (long)0;
                FileInfo fi = new FileInfo(m_DataFileName);
                if (fi.Exists) 
                    res += fi.Length;
                fi = new FileInfo(m_IndexFileName);
                if (fi.Exists) 
                    res += fi.Length;
                return res;
            }
        }
        public int GetMaxKey()
        {
            long res;
            if (m_Index != null) 
                res = m_Index.Length / IndexRecordSize;
            else 
            {
                FileInfo fi = new FileInfo(m_IndexFileName);
                if (!fi.Exists) 
                    return 0;
                res = fi.Length / IndexRecordSize;
            }
            if (res > 0) 
                res--;
            return (int)res;
        }
        public void ResetUniqueKeyPointer()
        {
            m_UniqueKeyPosition = 0;
        }
        int m_UniqueKeyPosition;
        public int GetUniqueKey()
        {
            int max = this.GetMaxKey();
            if (m_UniqueKeyPosition < 0) 
                return max + 1;
            long disp;
            int len;
            if (m_UniqueKeyPosition == 0) 
                m_UniqueKeyPosition = 1;
            for (; m_UniqueKeyPosition < max; m_UniqueKeyPosition++) 
            {
                if (!this._readIndexInfo(m_UniqueKeyPosition, out disp, out len)) 
                    continue;
                if (disp == 0 && len == 0) 
                    return m_UniqueKeyPosition++;
            }
            m_UniqueKeyPosition = -1;
            return max + 1;
        }
        public void SetMaxKey(int maxKey)
        {
            int delta = maxKey - this.GetMaxKey();
            if (delta <= 0) 
                return;
            byte[] buf = new byte[(int)(((delta + 1)) * IndexRecordSize)];
            for (int i = 0; i < buf.Length; i++) 
            {
                buf[i] = 0;
            }
            if (m_Index == null) 
            {
                m_Index = this.CreateFileStream(m_IndexFileName, false, -1);
                m_Index.Position = m_Index.Length;
                m_Index.Write(buf, 0, buf.Length);
                m_Index.Dispose();
            }
            else 
            {
                m_Index.Position = m_Index.Length;
                m_Index.Write(buf, 0, buf.Length);
            }
        }
        public override void Clear()
        {
            this._Close();
            if (File.Exists(m_IndexFileName)) 
                File.Delete(m_IndexFileName);
            if (File.Exists(m_DataFileName)) 
                File.Delete(m_DataFileName);
        }
        public override bool Open(bool readOnly, int indexInMemoryMaxLength = 0)
        {
            if (m_Data != null) 
            {
                if (readOnly || m_Data.CanWrite) 
                    return true;
            }
            this._Close();
            m_UniqueKeyPosition = 0;
            if (readOnly) 
            {
                if (!File.Exists(m_IndexFileName) || !File.Exists(m_DataFileName)) 
                    return false;
            }
            m_Index = this.CreateFileStream(m_IndexFileName, readOnly, IndexStreamBufSize);
            m_Data = this.CreateFileStream(m_DataFileName, readOnly, DataStreamBufSize);
            if (((indexInMemoryMaxLength > 0 && m_Index.Length <= indexInMemoryMaxLength)) || LoadAllInMemory) 
            {
                m_IndexBuf = new byte[(int)m_Index.Length];
                m_Index.Position = 0;
                m_Index.Read(m_IndexBuf, 0, m_IndexBuf.Length);
            }
            if (LoadAllInMemory) 
            {
                m_DataBuf = new byte[(int)m_Data.Length];
                m_Data.Position = 0;
                m_Data.Read(m_DataBuf, 0, m_DataBuf.Length);
            }
            return true;
        }
        protected string m_IndexFileName;
        protected string m_DataFileName;
        protected FileStream m_Index;
        protected byte[] m_IndexBuf;
        protected FileStream m_Data;
        protected byte[] m_DataBuf;
        long calcDataOptimizedLength()
        {
            long res = (long)0;
            if (m_Index != null) 
            {
                byte[] buf = new byte[(int)(10000 * IndexRecordSize)];
                m_Index.Position = 0;
                while (true) 
                {
                    int i = m_Index.Read(buf, 0, buf.Length);
                    if (i < IndexRecordSize) 
                        break;
                    for (int j = 0; j < i; j += IndexRecordSize) 
                    {
                        long lo = BitConverter.ToInt64(buf, j);
                        if (lo > 0) 
                        {
                            int le = BitConverter.ToInt32(buf, j + 8);
                            if (le > 0) 
                                res += le;
                        }
                    }
                }
            }
            else if (m_IndexBuf != null) 
            {
                for (int i = 0; (i + IndexRecordSize) <= m_IndexBuf.Length; i += IndexRecordSize) 
                {
                    long lo = BitConverter.ToInt64(m_IndexBuf, i);
                    if (lo > 0) 
                    {
                        int le = BitConverter.ToInt32(m_IndexBuf, i + 8);
                        if (le > 0) 
                            res += le;
                    }
                }
            }
            return res;
        }
        protected void _shiftIndex(int deltaKey)
        {
            if (m_Index.Length <= IndexRecordSize) 
                return;
            m_IndexBuf = null;
            int len = (int)((m_Index.Length - IndexRecordSize));
            byte[] buf = new byte[(int)len];
            m_Index.Position = IndexRecordSize;
            m_Index.Read(buf, 0, buf.Length);
            byte[] empty = new byte[(int)(deltaKey * IndexRecordSize)];
            for (int i = 0; i < empty.Length; i++) 
            {
                empty[i] = (byte)0;
            }
            m_Index.Position = IndexRecordSize;
            m_Index.Write(empty, 0, empty.Length);
            m_Index.Write(buf, 0, buf.Length);
        }
        protected byte[] _readIndex0()
        {
            byte[] res = new byte[(int)IndexRecordSize];
            m_Index.Position = 0;
            if (m_Index.Read(res, 0, res.Length) != res.Length) 
                return null;
            return res;
        }
        protected void _writeIndex0(byte[] info)
        {
            int len = info.Length;
            if (len > IndexRecordSize) 
                len = IndexRecordSize;
            m_Index.Position = 0;
            m_Index.Write(info, 0, len);
        }
        bool _readIndexInfo(int key, out long disp, out int len)
        {
            disp = 0;
            len = 0;
            long p = (long)key;
            p *= IndexRecordSize;
            if (m_IndexBuf != null) 
            {
                if ((p + IndexRecordSize) > m_IndexBuf.Length) 
                    return false;
                disp = BitConverter.ToInt64(m_IndexBuf, (int)p);
                len = BitConverter.ToInt32(m_IndexBuf, (int)((p + 8)));
            }
            else if (m_Index != null) 
            {
                if ((p + IndexRecordSize) > m_Index.Length) 
                    return false;
                m_Index.Position = p;
                byte[] buf = new byte[(int)IndexRecordSize];
                if (m_Index.Read(buf, 0, IndexRecordSize) != IndexRecordSize) 
                    return false;
                disp = BitConverter.ToInt64(buf, 0);
                len = BitConverter.ToInt32(buf, 8);
            }
            else 
                return false;
            if (len < 0) 
                return false;
            return true;
        }
        void _writeIndexInfo(int key, long disp, int len)
        {
            byte[] dbuf = BitConverter.GetBytes(disp);
            byte[] lbuf = BitConverter.GetBytes(len);
            long p = (long)key;
            p *= IndexRecordSize;
            if (m_IndexBuf != null && (p + IndexRecordSize) <= m_IndexBuf.Length) 
            {
                for (int i = 0; i < 8; i++) 
                {
                    m_IndexBuf[(int)(p + i)] = dbuf[i];
                }
                for (int i = 0; i < 4; i++) 
                {
                    m_IndexBuf[(int)(p + 8 + i)] = lbuf[i];
                }
            }
            if (p > m_Index.Length) 
            {
                byte[] buf = new byte[(int)(p - m_Index.Length)];
                for (int i = 0; i < buf.Length; i++) 
                {
                    buf[i] = (byte)0;
                }
                m_Index.Position = m_Index.Length;
                m_Index.Write(buf, 0, buf.Length);
            }
            m_Index.Position = p;
            m_Index.Write(dbuf, 0, 8);
            m_Index.Write(lbuf, 0, 4);
        }
        public int ReadKeyDataLen(int key)
        {
            if (m_Data == null) 
            {
                if (!this.Open(true, 0)) 
                    return -1;
            }
            long disp;
            int len;
            if (!this._readIndexInfo(key, out disp, out len)) 
                return -1;
            else 
                return len;
        }
        public byte[] ReadKeyData(int key, int maxLen = 0)
        {
            bool log = false;
            if (m_Data == null) 
            {
                if (log) 
                    Pullenti.Util.ConsoleHelper.Write(" m_Data = null ");
                if (!this.Open(true, 0)) 
                {
                    if (log) 
                        Pullenti.Util.ConsoleHelper.Write(" Can't open ");
                    return null;
                }
            }
            long disp;
            int len;
            if (!this._readIndexInfo(key, out disp, out len)) 
            {
                if (log) 
                    Pullenti.Util.ConsoleHelper.Write(" Can't read IndexInfo ");
                return null;
            }
            if (log) 
                Pullenti.Util.ConsoleHelper.Write(string.Format(" Disp={0}; Len = {1} ", disp, len));
            if (len < 1) 
                return null;
            if (disp >= m_Data.Length) 
            {
                if (log) 
                    Pullenti.Util.ConsoleHelper.Write(string.Format(" disp ({0}) >= length ({1}) ", disp, m_Data.Length));
                return null;
            }
            if (maxLen > 0 && len > maxLen) 
                len = maxLen;
            byte[] res = new byte[(int)len];
            if (m_DataBuf != null) 
            {
                for (int i = 0; i < len; i++) 
                {
                    res[i] = m_DataBuf[(int)(disp + i)];
                }
            }
            else 
            {
                m_Data.Position = disp;
                m_Data.Read(res, 0, res.Length);
            }
            if (AutoZipData) 
                return DecompressDeflate(res);
            return res;
        }
        byte[] m_ReadIndBuf;
        public Dictionary<int, byte[]> ReadKeysData(int keyMin, int maxCount, int maxDataSize = 10000000)
        {
            if (m_ReadIndBuf == null || m_ReadIndBuf.Length != (maxCount * IndexRecordSize)) 
                m_ReadIndBuf = new byte[(int)(maxCount * IndexRecordSize)];
            long p = (long)keyMin;
            p *= IndexRecordSize;
            m_Index.Position = p;
            int dlen = m_Index.Read(m_ReadIndBuf, 0, m_ReadIndBuf.Length);
            if (dlen < IndexRecordSize) 
                return null;
            long disp0 = (long)0;
            int len0 = 0;
            int ind = 0;
            for (; (ind + IndexRecordSize) <= m_ReadIndBuf.Length; ind += IndexRecordSize) 
            {
                disp0 = BitConverter.ToInt64(m_ReadIndBuf, ind);
                len0 = BitConverter.ToInt32(m_ReadIndBuf, ind + 8);
                if (len0 > 0) 
                    break;
                keyMin++;
            }
            if (len0 == 0) 
                return null;
            int ind0 = ind;
            long dposMax = disp0 + len0;
            for (ind += IndexRecordSize; (ind + IndexRecordSize) <= dlen; ind += IndexRecordSize) 
            {
                long disp = BitConverter.ToInt64(m_ReadIndBuf, ind);
                int len = BitConverter.ToInt32(m_ReadIndBuf, ind + 8);
                if (len == 0) 
                    continue;
                if (disp > (dposMax + 100) || (disp < disp0)) 
                    break;
                if ((disp + len) > dposMax) 
                {
                    if (((disp + len) - disp0) > maxDataSize) 
                        break;
                    dposMax = disp + len;
                }
                else 
                {
                }
            }
            int ind1 = ind;
            byte[] dats = new byte[(int)(dposMax - disp0)];
            if (m_DataBuf != null) 
            {
                for (int i = 0; i < dats.Length; i++) 
                {
                    dats[i] = m_DataBuf[(int)(disp0 + i)];
                }
            }
            else 
            {
                m_Data.Position = disp0;
                m_Data.Read(dats, 0, dats.Length);
            }
            Dictionary<int, byte[]> res = new Dictionary<int, byte[]>();
            int id = keyMin;
            for (ind = ind0; ind < ind1; ind += IndexRecordSize,id++) 
            {
                long disp = BitConverter.ToInt64(m_ReadIndBuf, ind);
                int len = BitConverter.ToInt32(m_ReadIndBuf, ind + 8);
                if (len == 0) 
                    continue;
                byte[] dat = new byte[(int)len];
                for (int i = 0; i < len; i++) 
                {
                    dat[i] = dats[(int)((disp - disp0) + i)];
                }
                if (AutoZipData) 
                    dat = DecompressDeflate(dat);
                res.Add(id, dat);
            }
            return res;
        }
        public void RemoveKeyData(int key)
        {
            if (!this.Open(false, 0)) 
                return;
            long disp;
            int len;
            if (!this._readIndexInfo(key, out disp, out len)) 
                return;
            if (disp == 0) 
                return;
            this._writeIndexInfo(key, 0, 0);
            m_UniqueKeyPosition = key;
        }
        public void BeginFetch()
        {
            if (m_Index == null) 
                this.Open(true, 0);
            m_FetchPos = 0;
        }
        int m_FetchPos;
        public void FetchDic(Dictionary<int, byte[]> res, int maxCount)
        {
            if (m_Index == null) 
                return;
            while (m_FetchPos < m_Index.Length) 
            {
                int id = m_FetchPos / IndexRecordSize;
                byte[] data = this.ReadKeyData(id, 0);
                m_FetchPos += 12;
                if (data != null) 
                    res.Add(id, data);
                if (res.Count >= maxCount) 
                    break;
            }
        }
        public int FetchPercent()
        {
            if (m_Index.Length > 100000) 
                return m_FetchPos / ((int)((m_Index.Length / 100)));
            else if (((int)m_Index.Length) == 0) 
                return 0;
            else 
                return (m_FetchPos * 100) / ((int)m_Index.Length);
        }
        public Dictionary<int, byte[]> Fetch(int maxCount)
        {
            if (m_Index == null) 
                return null;
            Dictionary<int, byte[]> res = new Dictionary<int, byte[]>();
            while (m_FetchPos < m_Index.Length) 
            {
                int id = m_FetchPos / IndexRecordSize;
                byte[] data = this.ReadKeyData(id, 0);
                m_FetchPos += 12;
                if (data != null) 
                    res.Add(id, data);
                if (res.Count >= maxCount) 
                    break;
            }
            return res;
        }
        protected void EndFetch()
        {
            this._Close();
        }
        public void WriteKeyData(int key, byte[] data)
        {
            if (AutoZipData) 
                data = CompressDeflate(data);
            this._addData(key, data, m_Data);
        }
        public void UpdatePartOfData(int key, byte[] data, int pos)
        {
            long disp;
            int len;
            if (!this._readIndexInfo(key, out disp, out len)) 
                return;
            m_Data.Position = disp + pos;
            m_Data.Write(data, 0, data.Length);
        }
        public void UpdateStartOfData(int key, byte[] data)
        {
            long disp;
            int len;
            if (!this._readIndexInfo(key, out disp, out len)) 
                return;
            m_Data.Position = disp;
            m_Data.Write(data, 0, data.Length);
        }
        void _addData(int key, byte[] data, Stream dst)
        {
            if (data == null || m_Index == null || dst == null) 
                return;
            if (dst == m_Data) 
            {
                long disp;
                int len;
                if (this._readIndexInfo(key, out disp, out len)) 
                {
                    if (len >= data.Length && (disp + len) <= dst.Length) 
                    {
                        dst.Position = disp;
                        dst.Write(data, 0, data.Length);
                        this._writeIndexInfo(key, disp, data.Length);
                        return;
                    }
                }
            }
            if (dst.Length == 0) 
            {
                dst.WriteByte((byte)0xFF);
                dst.WriteByte((byte)0xFF);
            }
            long pos = dst.Length;
            if (data.Length > 0) 
            {
                for (int i = 0; i < 2; i++) 
                {
                    try 
                    {
                        dst.Position = dst.Length;
                        dst.Write(data, 0, data.Length);
                        break;
                    }
                    catch(Exception ex) 
                    {
                        if (i == 0) 
                        {
                        }
                        if (i == 1) 
                            throw ex;
                    }
                }
            }
            this._writeIndexInfo(key, pos, (data == null ? (int)0 : data.Length));
        }
        public override bool NeedOptimize(int minPercent = 10, bool analyzeDiskSpace = true)
        {
            if (m_Data == null) 
                return false;
            long le0 = this.calcDataOptimizedLength();
            if (le0 == 0) 
                return false;
            float ration = (float)(100 + minPercent);
            ration /= 100;
            float d = (float)m_Data.Length;
            d /= le0;
            if (d > ration) 
                return true;
            if (d < 1.05F) 
                return false;
            return false;
        }
        public override bool Optimize(int minPercent = 10)
        {
            bool isOpened = m_Data != null && m_Data.CanWrite;
            if (isOpened) 
                this.Flush();
            else if (!this.Open(false, 10000000)) 
                return false;
            if (minPercent > 0) 
            {
                if (!this.NeedOptimize(minPercent, true)) 
                {
                    if (!isOpened) 
                        this._Close();
                    return false;
                }
            }
            string dir = Path.GetDirectoryName(m_IndexFileName);
            string tempDatFile = Path.Combine(dir, "temp.dat");
            if (File.Exists(tempDatFile)) 
                File.Delete(tempDatFile);
            string tempIndFile = Path.Combine(dir, "temp.ind");
            if (File.Exists(tempIndFile)) 
                File.Delete(tempIndFile);
            KeyBaseTable tmpTable = new KeyBaseTable(Index, "temp", dir);
            tmpTable.Open(false, 0);
            Stopwatch sw = new Stopwatch();
            int max = this.GetMaxKey();
            int id = 1;
            sw.Start();
            int p0 = 0;
            bool autoZip = AutoZipData;
            AutoZipData = false;
            while (id <= max) 
            {
                if (max > 10000) 
                {
                    int p = id / ((max / 100));
                    if (p != p0) 
                        Console.Write(" {0}%", (p0 = p));
                }
                Dictionary<int, byte[]> datas = this.ReadKeysData(id, 1000, 10000000);
                if (datas != null) 
                {
                    foreach (KeyValuePair<int, byte[]> kp in datas) 
                    {
                        tmpTable.WriteKeyData(kp.Key, kp.Value);
                        if (kp.Key > id) 
                            id = kp.Key;
                    }
                }
                id++;
            }
            sw.Stop();
            this._Close();
            tmpTable.Dispose();
            AutoZipData = autoZip;
            File.Delete(m_DataFileName);
            File.Move(tempDatFile, m_DataFileName);
            File.Delete(m_IndexFileName);
            File.Move(tempIndFile, m_IndexFileName);
            if (isOpened) 
                this.Open(false, 0);
            return true;
        }
        public bool UploadDataFromOtherDir(string dirName, bool removeAfterCopy)
        {
            string srcIndexFileName = Path.Combine(dirName, Path.GetFileName(m_IndexFileName));
            string srcDataFileName = Path.Combine(dirName, Path.GetFileName(m_DataFileName));
            if (!File.Exists(srcIndexFileName) || !File.Exists(srcDataFileName)) 
                return false;
            bool isOpened = m_Data != null && m_Data.CanWrite;
            this._Close();
            if (removeAfterCopy) 
            {
                if (File.Exists(m_DataFileName)) 
                    File.Delete(m_DataFileName);
                File.Move(srcDataFileName, m_DataFileName);
                if (File.Exists(m_IndexFileName)) 
                    File.Delete(m_IndexFileName);
                File.Move(srcIndexFileName, m_IndexFileName);
            }
            else 
            {
                File.Copy(srcDataFileName, m_DataFileName, true);
                File.Copy(srcIndexFileName, m_IndexFileName, true);
            }
            if (isOpened) 
                this.Open(false, 0);
            return true;
        }
        public static byte[] CompressDeflate(byte[] dat)
        {
            if (dat == null) 
                return null;
            byte[] zip = null;
            using (MemoryStream res = new MemoryStream()) 
            {
                using (MemoryStream input = new MemoryStream(dat)) 
                {
                    input.Position = 0;
                    using (DeflateStream deflate = new DeflateStream(res, CompressionMode.Compress)) 
                    {
                        input.WriteTo(deflate);
                        deflate.Flush();
                    }
                }
                zip = res.ToArray();
            }
            return zip;
        }
        public static byte[] DecompressDeflate(byte[] zip)
        {
            if (zip == null || (zip.Length < 1)) 
                return null;
            try 
            {
                using (MemoryStream unzip = new MemoryStream()) 
                {
                    byte[] buf = new byte[(int)(zip.Length + zip.Length)];
                    using (MemoryStream data = new MemoryStream(zip)) 
                    {
                        data.Position = 0;
                        using (DeflateStream deflate = new DeflateStream(data, CompressionMode.Decompress)) 
                        {
                            while (true) 
                            {
                                int i = deflate.Read(buf, 0, buf.Length);
                                if (i < 1) 
                                    break;
                                unzip.Write(buf, 0, i);
                            }
                        }
                        return unzip.ToArray();
                    }
                }
            }
            catch(Exception ex) 
            {
                return null;
            }
        }
    }
}