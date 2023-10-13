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
    class RepChildrenTable : Pullenti.Util.Repository.KeyBaseTable
    {
        internal RepChildrenTable(string baseDir, string name = "chils") : base(null, name, baseDir)
        {
        }
        public void Add(int id, List<int> li)
        {
            if (li == null || li.Count == 0) 
                this.WriteKeyData(id, null);
            else 
            {
                List<byte> res = new List<byte>();
                foreach (int i in li) 
                {
                    res.AddRange(BitConverter.GetBytes(i));
                }
                this.WriteKeyData(id, res.ToArray());
            }
        }
        public List<int> Get(int id)
        {
            byte[] dat = this.ReadKeyData(id, 0);
            if (dat == null) 
                return null;
            List<int> res = new List<int>();
            int ind = 0;
            for (; ind < dat.Length; ind += 4) 
            {
                res.Add(BitConverter.ToInt32(dat, ind));
            }
            return res;
        }
    }
}