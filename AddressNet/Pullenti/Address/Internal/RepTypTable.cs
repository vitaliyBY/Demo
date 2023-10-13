/*
 * SDK Pullenti Address, version 4.20, august 2023. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Pullenti.Address.Internal
{
    class RepTypTable : Pullenti.Util.Repository.KeyBaseTable
    {
        internal RepTypTable(string baseDir, string name = "typs") : base(null, name, baseDir)
        {
            int max = this.GetMaxKey();
            for (int id = 1; id <= max; id++) 
            {
                byte[] dat = this.ReadKeyData(id, 0);
                if (dat == null) 
                    continue;
                string typ = Encoding.UTF8.GetString(dat);
                if (m_TypesByName.ContainsKey(typ)) 
                    continue;
                m_TypesByName.Add(typ, id);
                m_TypesById.Add(id, typ);
            }
        }
        Dictionary<string, int> m_TypesByName = new Dictionary<string, int>();
        Dictionary<int, string> m_TypesById = new Dictionary<int, string>();
        public int GetId(string typ)
        {
            if (string.IsNullOrEmpty(typ)) 
                return 0;
            int id;
            if (m_TypesByName.TryGetValue(typ, out id)) 
                return id;
            id = this.GetMaxKey() + 1;
            this.WriteKeyData(id, Encoding.UTF8.GetBytes(typ));
            this.Flush();
            m_TypesById.Add(id, typ);
            m_TypesByName.Add(typ, id);
            return id;
        }
        public string GetTyp(int id)
        {
            string typ;
            if (m_TypesById.TryGetValue(id, out typ)) 
                return typ;
            return null;
        }
    }
}