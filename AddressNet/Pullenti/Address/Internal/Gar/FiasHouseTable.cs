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
    class FiasHouseTable : Pullenti.Util.Repository.KeyBaseTable
    {
        internal FiasHouseTable(Pullenti.Util.Repository.IRepository rep, string name = "houseobjects") : base(rep, name, null)
        {
        }
        public void Add(int id, HouseObject doc)
        {
            byte[] dat = _store(doc);
            this.WriteKeyData(id, dat);
        }
        internal static byte[] _store(HouseObject ao)
        {
            List<byte> res = new List<byte>();
            byte attr = (byte)((ao.Actual ? 0 : 1));
            if (ao.AltParentId > 0) 
                attr |= 2;
            if (ao.Status == Pullenti.Address.GarStatus.Warning) 
                attr |= ((byte)4);
            else if (ao.Status == Pullenti.Address.GarStatus.Error) 
                attr |= ((byte)8);
            else if (ao.Status == Pullenti.Address.GarStatus.Ok2) 
                attr |= ((byte)0x10);
            res.Add(attr);
            res.AddRange(BitConverter.GetBytes(ao.ParentId));
            res.Add(ao.HouseTyp);
            res.Add(ao.StrucTyp);
            Pullenti.Util.Repository.BaseTable.GetBytesForString(res, ao.HouseNumber, null);
            Pullenti.Util.Repository.BaseTable.GetBytesForString(res, ao.BuildNumber, null);
            Pullenti.Util.Repository.BaseTable.GetBytesForString(res, ao.StrucNumber, null);
            res.AddRange(BitConverter.GetBytes((ao.RoomIds == null ? 0 : ao.RoomIds.Count)));
            if (ao.RoomIds != null) 
            {
                foreach (int ii in ao.RoomIds) 
                {
                    res.AddRange(BitConverter.GetBytes(ii));
                }
            }
            if (ao.AltParentId > 0) 
                res.AddRange(BitConverter.GetBytes(ao.AltParentId));
            Guid gg = new Guid(ao.Guid);
            res.AddRange(gg.ToByteArray());
            return res.ToArray();
        }
        public bool Get(int id, HouseObject ao)
        {
            byte[] dat = this.ReadKeyData(id, 0);
            if (dat == null) 
                return false;
            ao.Id = id;
            return _restore(dat, ao, 0);
        }
        public int GetParentId(int id)
        {
            byte[] data = this.ReadKeyData(id, 5);
            if (data == null) 
                return 0;
            int ind = 1;
            return BitConverter.ToInt32(data, ind);
        }
        public int GetActual(int id)
        {
            byte[] data = this.ReadKeyData(id, 1);
            if (data == null) 
                return -1;
            return (((data[0] & 1)) != 0 ? 0 : 1);
        }
        public static bool _restore(byte[] data, HouseObject ao, int ind = 0)
        {
            if (((data[ind] & 1)) != 0) 
                ao.Actual = false;
            else 
                ao.Actual = true;
            bool isUnom = ((data[ind] & 2)) != 0;
            if (((data[ind] & 4)) != 0) 
                ao.Status = Pullenti.Address.GarStatus.Warning;
            if (((data[ind] & 8)) != 0) 
                ao.Status = Pullenti.Address.GarStatus.Error;
            if (((data[ind] & 0x10)) != 0) 
                ao.Status = Pullenti.Address.GarStatus.Ok2;
            ind++;
            ao.ParentId = BitConverter.ToInt32(data, ind);
            ind += 4;
            ao.HouseTyp = data[ind++];
            ao.StrucTyp = data[ind++];
            ao.HouseNumber = Pullenti.Util.Repository.BaseTable.GetStringForBytes(data, ref ind, false, null);
            ao.BuildNumber = Pullenti.Util.Repository.BaseTable.GetStringForBytes(data, ref ind, false, null);
            ao.StrucNumber = Pullenti.Util.Repository.BaseTable.GetStringForBytes(data, ref ind, false, null);
            int cou = BitConverter.ToInt32(data, ind);
            ind += 4;
            if (cou > 0) 
            {
                ao.RoomIds = new List<int>();
                for (; cou > 0; cou--) 
                {
                    ao.RoomIds.Add(BitConverter.ToInt32(data, ind));
                    ind += 4;
                }
            }
            if (isUnom) 
            {
                ao.AltParentId = BitConverter.ToInt32(data, ind);
                ind += 4;
            }
            if ((ind + 16) <= data.Length) 
            {
                byte[] dat = new byte[(int)16];
                for (int i = 0; i < 16; i++) 
                {
                    dat[i] = data[ind + i];
                }
                Guid gg = new Guid(dat);
                ao.Guid = gg.ToString();
            }
            return true;
        }
    }
}