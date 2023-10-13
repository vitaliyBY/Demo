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
    class FiasRoomTable : Pullenti.Util.Repository.KeyBaseTable
    {
        internal FiasRoomTable(Pullenti.Util.Repository.IRepository rep, string name = "roomobjects") : base(rep, name, null)
        {
        }
        public void Add(int id, RoomObject r)
        {
            List<byte> res = new List<byte>();
            _store(res, r);
            byte[] dat = res.ToArray();
            this.WriteKeyData(id, dat);
        }
        public static void _store(List<byte> res, RoomObject ao)
        {
            byte attr = (byte)((ao.Actual ? 0 : 1));
            res.Add(attr);
            res.AddRange(BitConverter.GetBytes(ao.HouseId));
            res.Add((byte)((int)ao.Typ));
            Pullenti.Util.Repository.BaseTable.GetBytesForString(res, ao.Number, null);
            res.AddRange(BitConverter.GetBytes((int)((ao.ChildrenIds == null ? 0 : ao.ChildrenIds.Count))));
            if (ao.ChildrenIds != null) 
            {
                foreach (uint id in ao.ChildrenIds) 
                {
                    res.AddRange(BitConverter.GetBytes(id));
                }
            }
            Guid gg = new Guid(ao.Guid);
            res.AddRange(gg.ToByteArray());
        }
        public RoomObject Get(int id)
        {
            byte[] dat = this.ReadKeyData(id, 0);
            if (dat == null) 
                return null;
            RoomObject r = new RoomObject();
            r.Id = id;
            int ind = 0;
            _restore(dat, r, ref ind);
            return r;
        }
        public int GetParentId(int id)
        {
            byte[] data = this.ReadKeyData(id, 11);
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
        public static Pullenti.Address.RoomType _getRoomTyp(byte[] data, int ind)
        {
            ind += 5;
            return (Pullenti.Address.RoomType)data[ind];
        }
        public static bool _restore(byte[] data, RoomObject ao, ref int ind)
        {
            if (((data[ind] & 1)) != 0) 
                ao.Actual = false;
            else 
                ao.Actual = true;
            ind++;
            ao.HouseId = BitConverter.ToUInt32(data, ind);
            ind += 4;
            byte typ = data[ind++];
            ao.Typ = (Pullenti.Address.RoomType)typ;
            ao.Number = Pullenti.Util.Repository.BaseTable.GetStringForBytes(data, ref ind, false, null);
            int cou = BitConverter.ToInt32(data, ind);
            ind += 4;
            for (; cou > 0; cou--) 
            {
                if (ao.ChildrenIds == null) 
                    ao.ChildrenIds = new List<uint>();
                ao.ChildrenIds.Add(BitConverter.ToUInt32(data, ind));
                ind += 4;
            }
            byte[] dat = new byte[(int)16];
            for (int i = 0; i < 16; i++) 
            {
                dat[i] = data[ind + i];
            }
            Guid gg = new Guid(dat);
            ao.Guid = gg.ToString();
            return true;
        }
    }
}