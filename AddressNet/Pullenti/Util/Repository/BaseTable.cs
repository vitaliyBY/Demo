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
using System.Threading;

namespace Pullenti.Util.Repository
{
    public class BaseTable : IDisposable
    {
        public BaseTable(IRepository index)
        {
            Index = index;
        }
        public IRepository Index;
        public string Name;
        public override string ToString()
        {
            return Name ?? base.ToString();
        }
        public object m_Lock = new object();
        public virtual long RecordsCount
        {
            get
            {
                return 0;
            }
        }
        public virtual long Size
        {
            get
            {
                return 0;
            }
        }
        public void Dispose()
        {
            this._Close();
        }
        public virtual bool Open(bool readOnly, int indexInMemoryMaxLength = 0)
        {
            return false;
        }
        public virtual void _Close()
        {
        }
        public virtual void Clear()
        {
        }
        public virtual void Flush()
        {
        }
        public virtual bool Optimize(int minPercent = 10)
        {
            return false;
        }
        public virtual bool NeedOptimize(int minPercent = 10, bool analyzeDiskSpace = true)
        {
            return false;
        }
        protected static bool BackupFile(string fname, string path)
        {
            try 
            {
                if (!File.Exists(fname)) 
                    return true;
                File.Copy(fname, Path.Combine(path, Path.GetFileName(fname)), true);
                return true;
            }
            catch(Exception ex) 
            {
                return false;
            }
        }
        protected static bool RestoreFile(string fname, string path, bool remove)
        {
            try 
            {
                string src = Path.Combine(path, Path.GetFileName(fname));
                if (!File.Exists(src)) 
                    return true;
                File.Copy(src, fname, true);
                if (remove) 
                    File.Delete(src);
                return true;
            }
            catch(Exception ex) 
            {
                return false;
            }
        }
        public virtual bool Backup(string path)
        {
            return true;
        }
        public virtual bool Restore(string path, bool remove)
        {
            return true;
        }
        protected FileStream CreateFileStream(string fileName, bool readOnly, int bufLen = -1)
        {
            Exception resEx = null;
            for (int k = 0; k < 5; k++) 
            {
                try 
                {
                    if (readOnly) 
                    {
                        if (bufLen > 0) 
                            return new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, bufLen);
                        else 
                            return new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    }
                    else if (bufLen > 0) 
                        return new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read, bufLen);
                    else 
                        return new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
                }
                catch(Exception ex) 
                {
                    resEx = ex;
                }
                if (k == 0) 
                {
                    if (!File.Exists(fileName)) 
                    {
                        if (bufLen > 0) 
                            return new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, bufLen);
                        else 
                            return new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                    }
                }
                Thread.Sleep(2000);
            }
            throw resEx;
        }
        public static void GetBytesForString(List<byte> res, string str, Encoding enc = null)
        {
            if (string.IsNullOrEmpty(str)) 
                res.AddRange(BitConverter.GetBytes((short)0));
            else 
            {
                byte[] b = (enc == null ? Encoding.UTF8.GetBytes(str) : enc.GetBytes(str));
                res.AddRange(BitConverter.GetBytes((short)b.Length));
                res.AddRange(b);
            }
        }
        public static string GetStringForBytes(byte[] data, ref int ind, bool dontCreate = false, Encoding enc = null)
        {
            if ((ind + 2) > data.Length) 
                return null;
            short len = BitConverter.ToInt16(data, ind);
            ind += 2;
            if (len <= 0) 
                return null;
            if ((ind + len) > data.Length) 
                return null;
            string res = null;
            if (!dontCreate) 
            {
                if (enc == null) 
                    res = Encoding.UTF8.GetString(data, ind, len);
                else 
                    res = enc.GetString(data, ind, len);
            }
            ind += len;
            return res;
        }
        protected static void GetBytesForDate0(List<byte> res, DateTime? dt)
        {
            if (dt != null) 
                GetBytesForDate(res, dt.Value);
            else 
                res.AddRange(BitConverter.GetBytes((short)0));
        }
        protected static void GetBytesForDate(List<byte> res, DateTime dt)
        {
            res.AddRange(BitConverter.GetBytes((short)dt.Year));
            res.Add((byte)dt.Month);
            res.Add((byte)dt.Day);
            res.Add((byte)dt.Hour);
            res.Add((byte)dt.Minute);
            res.Add((byte)dt.Second);
            res.Add((byte)0);
        }
        protected static DateTime? ToDate(byte[] data, ref int ind)
        {
            if ((ind + 2) > data.Length) 
                return null;
            int year = (int)BitConverter.ToInt16(data, ind);
            ind += 2;
            if (year == 0) 
                return null;
            if ((ind + 8) > data.Length) 
                return null;
            int mon = (int)data[ind++];
            int day = (int)data[ind++];
            int hour = (int)data[ind++];
            int min = (int)data[ind++];
            int sec = (int)data[ind++];
            ind++;
            if (year == 0) 
                return null;
            try 
            {
                return new DateTime(year, mon, day, hour, min, sec);
            }
            catch(Exception ex4363) 
            {
                return null;
            }
        }
    }
}