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

namespace Pullenti.Util.Repository
{
    public class StringDictionaryTable : BaseTable
    {
        public StringDictionaryTable(IRepository index, string name) : base(index)
        {
            m_FileName = Path.Combine(index.BaseDir, name + ".dic");
            Name = name;
        }
        public bool IsExists
        {
            get
            {
                if (!File.Exists(m_FileName)) 
                    return false;
                return true;
            }
        }
        public override long Size
        {
            get
            {
                FileInfo fi = new FileInfo(m_FileName);
                if (fi.Exists) 
                    return fi.Length;
                else 
                    return 0;
            }
        }
        public override bool Backup(string path)
        {
            this._Close();
            if (!BaseTable.BackupFile(m_FileName, path)) 
                return false;
            return base.Backup(path);
        }
        public override bool Restore(string path, bool remove)
        {
            this._Close();
            if (!BaseTable.RestoreFile(m_FileName, path, remove)) 
                return false;
            return base.Restore(path, remove);
        }
        public override void _Close()
        {
            this._saveNew();
            if (m_Stream != null) 
            {
                m_Stream.Dispose();
                m_Stream = null;
            }
            m_Hash.Clear();
            m_Strings.Clear();
        }
        public override void Flush()
        {
            this._saveNew();
            base.Flush();
            if (m_Stream != null) 
                m_Stream.Flush();
        }
        Dictionary<string, int> m_Hash = new Dictionary<string, int>();
        List<string> m_Strings = new List<string>();
        List<string> m_New = new List<string>();
        void _saveNew()
        {
            if (m_New.Count < 1) 
                return;
            List<byte> buf = new List<byte>();
            foreach (string s in m_New) 
            {
                BaseTable.GetBytesForString(buf, s, null);
            }
            m_Stream.Position = m_Stream.Length;
            m_Stream.Write(buf.ToArray(), 0, buf.Count);
            m_New.Clear();
        }
        string m_FileName;
        Stream m_Stream;
        public override void Clear()
        {
            this._Close();
            if (File.Exists(m_FileName)) 
                File.Delete(m_FileName);
        }
        public override bool Open(bool readOnly, int indexInMemoryMaxLength = 0)
        {
            if (m_Stream != null) 
            {
                if (readOnly || m_Stream.CanWrite) 
                    return true;
            }
            this._Close();
            if (readOnly) 
            {
                if (!File.Exists(m_FileName)) 
                    return false;
            }
            m_Stream = this.CreateFileStream(m_FileName, readOnly, -1);
            if (m_Stream.Length > 0) 
            {
                byte[] buf = new byte[(int)((int)m_Stream.Length)];
                m_Stream.Position = 0;
                m_Stream.Read(buf, 0, buf.Length);
                this._restore(buf);
            }
            return true;
        }
        void _restore(byte[] data)
        {
            int ind = 0;
            while (ind < data.Length) 
            {
                string s = BaseTable.GetStringForBytes(data, ref ind, false, null);
                if (s == null) 
                    break;
                if (!m_Hash.ContainsKey(s)) 
                {
                    m_Hash.Add(s, m_Hash.Count + 1);
                    m_Strings.Add(s);
                }
            }
        }
        public int GetCodeByString(string val, bool addIfNotExist)
        {
            if (string.IsNullOrEmpty(val)) 
                return 0;
            int id;
            if (m_Hash.TryGetValue(val, out id)) 
                return id;
            if (!addIfNotExist) 
                return 0;
            id = m_Hash.Count + 1;
            m_Hash.Add(val, id);
            m_Strings.Add(val);
            m_New.Add(val);
            return id;
        }
        public string GetStringByCode(int id)
        {
            if ((id < 1) || id > m_Strings.Count) 
                return null;
            else 
                return m_Strings[id - 1];
        }
    }
}