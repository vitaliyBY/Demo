/*
 * SDK Pullenti Address, version 4.20, august 2023. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Collections;
using System.Collections.Generic;

namespace Pullenti.Address.Internal.Gar
{
    class FiasAddrTable : Pullenti.Util.Repository.KeyBaseTable
    {
        internal FiasAddrTable(Pullenti.Util.Repository.IRepository rep, string name = "areaobjects") : base(rep, name, null)
        {
        }
        public void Add(int id, AreaObject doc, bool onlyAttrs)
        {
            byte[] dat = this._store(doc);
            this.WriteKeyData(id, dat);
        }
        byte[] _store(AreaObject ao)
        {
            List<byte> res = new List<byte>();
            byte attr = (byte)((ao.Actual ? 0 : 1));
            if (ao.Status == Pullenti.Address.GarStatus.Warning) 
                attr |= ((byte)2);
            else if (ao.Status == Pullenti.Address.GarStatus.Error) 
                attr |= ((byte)4);
            else if (ao.Status == Pullenti.Address.GarStatus.Ok2) 
                attr |= ((byte)8);
            res.Add(attr);
            res.AddRange(BitConverter.GetBytes((short)((ao.Typ == null ? 0 : ao.Typ.Id))));
            res.AddRange(BitConverter.GetBytes((short)((ao.OldTyp == null ? 0 : ao.OldTyp.Id))));
            res.AddRange(BitConverter.GetBytes((short)ao.ParentIds.Count));
            if (ao.ParentIds.Count > 0) 
            {
                foreach (int id in ao.ParentIds) 
                {
                    res.AddRange(BitConverter.GetBytes(id));
                }
                res.Add((byte)((ao.ParentParentIds == null ? 0 : ao.ParentParentIds.Count)));
                if (ao.ParentParentIds != null) 
                {
                    foreach (int id in ao.ParentParentIds) 
                    {
                        res.AddRange(BitConverter.GetBytes(id));
                    }
                }
            }
            res.AddRange(BitConverter.GetBytes((short)ao.Level));
            res.AddRange(BitConverter.GetBytes((short)ao.Names.Count));
            foreach (string n in ao.Names) 
            {
                GetBytesForString1251(res, n);
            }
            res.AddRange(BitConverter.GetBytes(ao.ChildrenIds.Count));
            foreach (uint id in ao.ChildrenIds) 
            {
                res.AddRange(BitConverter.GetBytes(id));
            }
            res.AddRange(BitConverter.GetBytes(0));
            res.Add(ao.Region);
            GetBytesForString1251(res, ao.Guid);
            return res.ToArray();
        }
        public bool Get(int id, AreaObject ao, Dictionary<int, AreaType> typs)
        {
            byte[] dat = this.ReadKeyData(id, 0);
            if (dat == null) 
                return false;
            ao.Id = id;
            return _restore(dat, ao, typs);
        }
        public int GetParentId(int id)
        {
            byte[] data = this.ReadKeyData(id, 11);
            if (data == null) 
                return 0;
            int ind = 5;
            int cou = (int)BitConverter.ToInt16(data, ind);
            ind += 2;
            if (cou == 0) 
                return 0;
            return BitConverter.ToInt32(data, ind);
        }
        public int GetActual(int id)
        {
            byte[] data = this.ReadKeyData(id, 1);
            if (data == null) 
                return -1;
            return (((data[0] & 1)) != 0 ? 0 : 1);
        }
        public Pullenti.Address.GarStatus GetStatus(int id)
        {
            byte[] data = this.ReadKeyData(id, 1);
            if (data == null) 
                return Pullenti.Address.GarStatus.Error;
            if (((data[0] & 2)) != 0) 
                return Pullenti.Address.GarStatus.Warning;
            if (((data[0] & 4)) != 0) 
                return Pullenti.Address.GarStatus.Error;
            if (((data[0] & 8)) != 0) 
                return Pullenti.Address.GarStatus.Ok2;
            return Pullenti.Address.GarStatus.Ok;
        }
        public static bool _restore(byte[] data, AreaObject ao, Dictionary<int, AreaType> typs)
        {
            if (((data[0] & 1)) != 0) 
                ao.Actual = false;
            else 
                ao.Actual = true;
            if (((data[0] & 2)) != 0) 
                ao.Status = Pullenti.Address.GarStatus.Warning;
            if (((data[0] & 4)) != 0) 
                ao.Status = Pullenti.Address.GarStatus.Error;
            if (((data[0] & 8)) != 0) 
                ao.Status = Pullenti.Address.GarStatus.Ok2;
            int ind = 1;
            int id = (int)BitConverter.ToInt16(data, ind);
            ind += 2;
            AreaType ty;
            if (typs.TryGetValue(id, out ty)) 
                ao.Typ = ty;
            id = BitConverter.ToInt16(data, ind);
            ind += 2;
            if (id != 0) 
            {
                if (typs.TryGetValue(id, out ty)) 
                    ao.OldTyp = ty;
            }
            int cou = (int)BitConverter.ToInt16(data, ind);
            ind += 2;
            if (cou > 0) 
            {
                for (; cou > 0; cou--) 
                {
                    ao.ParentIds.Add(BitConverter.ToInt32(data, ind));
                    ind += 4;
                }
                cou = data[ind];
                ind += 1;
                if (cou > 0) 
                {
                    ao.ParentParentIds = new List<int>();
                    for (; cou > 0; cou--) 
                    {
                        ao.ParentParentIds.Add(BitConverter.ToInt32(data, ind));
                        ind += 4;
                    }
                }
            }
            ao.Level = BitConverter.ToInt16(data, ind);
            ind += 2;
            cou = BitConverter.ToInt16(data, ind);
            ind += 2;
            for (; cou > 0; cou--) 
            {
                ao.Names.Add(ToString1251(data, ref ind));
            }
            cou = BitConverter.ToInt32(data, ind);
            ind += 4;
            for (; cou > 0; cou--) 
            {
                ao.ChildrenIds.Add(BitConverter.ToUInt32(data, ind));
                ind += 4;
            }
            ind += 4;
            ao.Region = data[ind];
            ind++;
            if (ind < data.Length) 
                ao.Guid = ToString1251(data, ref ind);
            return true;
        }
        static string ToString1251(byte[] data, ref int ind)
        {
            if ((ind + 2) > data.Length) 
                return null;
            short len = BitConverter.ToInt16(data, ind);
            ind += 2;
            if (len <= 0) 
                return null;
            if ((ind + len) > data.Length) 
                return null;
            string res = Pullenti.Address.Internal.FiasHelper.DecodeString1251(data, ind, len, false);
            ind += len;
            return res;
        }
        static void GetBytesForString1251(List<byte> res, string str)
        {
            if (string.IsNullOrEmpty(str)) 
                res.AddRange(BitConverter.GetBytes((short)0));
            else 
            {
                byte[] b = Pullenti.Address.Internal.FiasHelper.EncodeString1251(str);
                res.AddRange(BitConverter.GetBytes((short)b.Length));
                res.AddRange(b);
            }
        }
    }
}