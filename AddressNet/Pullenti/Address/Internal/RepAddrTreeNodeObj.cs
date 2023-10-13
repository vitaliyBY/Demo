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
    class RepAddrTreeNodeObj
    {
        public int Id;
        public List<int> Parents = null;
        public Pullenti.Address.AddrLevel Lev = Pullenti.Address.AddrLevel.Undefined;
        public List<short> TypIds = new List<short>();
        public override string ToString()
        {
            StringBuilder tmp = new StringBuilder();
            tmp.AppendFormat("{0} ({1}", Id, Lev);
            for (int i = 0; i < TypIds.Count; i++) 
            {
                tmp.AppendFormat("{0}{1}", (i > 0 ? "," : ":"), TypIds[i]);
            }
            tmp.Append(")");
            if (Parents != null) 
            {
                foreach (int p in Parents) 
                {
                    tmp.AppendFormat(", {0}", p);
                }
            }
            return tmp.ToString();
        }
        public bool Correct(Pullenti.Address.RepaddrObject o, RepTypTable typs, Pullenti.Address.RepaddrObject p)
        {
            bool ret = false;
            foreach (string ty in o.Types) 
            {
                short tid = (short)typs.GetId(ty);
                if (tid != 0 && !TypIds.Contains(tid)) 
                {
                    TypIds.Add(tid);
                    ret = true;
                }
            }
            foreach (short id in TypIds) 
            {
                string ty = typs.GetTyp(id);
                if (ty != null && !o.Types.Contains(ty)) 
                {
                    o.Types.Add(ty);
                    ret = true;
                }
            }
            if (p != null) 
            {
                if (Parents == null) 
                    Parents = new List<int>();
                if (!Parents.Contains(p.Id)) 
                {
                    Parents.Add(p.Id);
                    ret = true;
                }
                if (o.Parents == null) 
                    o.Parents = new List<int>();
                if (!o.Parents.Contains(p.Id)) 
                {
                    o.Parents.Add(p.Id);
                    ret = true;
                }
            }
            return ret;
        }
    }
}