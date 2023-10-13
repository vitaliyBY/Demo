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
    class ParamsTable : Pullenti.Util.Repository.KeyBaseTable
    {
        internal ParamsTable(Pullenti.Util.Repository.IRepository rep, string name) : base(rep, name, null)
        {
        }
        public Dictionary<Pullenti.Address.GarParam, string> GetParams(int id)
        {
            byte[] dat = this.ReadKeyData(id, 0);
            if (dat == null) 
                return null;
            int ind = 0;
            Dictionary<Pullenti.Address.GarParam, string> res = new Dictionary<Pullenti.Address.GarParam, string>();
            _toDic(res, dat, ref ind);
            return res;
        }
        public static void _toDic(Dictionary<Pullenti.Address.GarParam, string> res, byte[] dat, ref int ind)
        {
            short cou = BitConverter.ToInt16(dat, ind);
            ind += 2;
            for (; cou > 0; cou--) 
            {
                Pullenti.Address.GarParam typ = (Pullenti.Address.GarParam)((int)dat[ind]);
                ind++;
                string val = Pullenti.Util.Repository.BaseTable.GetStringForBytes(dat, ref ind, false, null);
                if (val != null && !res.ContainsKey(typ)) 
                    res.Add(typ, val);
            }
        }
        public static string _getVal(byte[] dat, Pullenti.Address.GarParam ty)
        {
            int ind = 0;
            short cou = BitConverter.ToInt16(dat, ind);
            ind += 2;
            for (; cou > 0; cou--) 
            {
                Pullenti.Address.GarParam typ = (Pullenti.Address.GarParam)((int)dat[ind]);
                ind++;
                if (ty == typ) 
                    return Pullenti.Util.Repository.BaseTable.GetStringForBytes(dat, ref ind, false, null);
                Pullenti.Util.Repository.BaseTable.GetStringForBytes(dat, ref ind, true, null);
            }
            return null;
        }
        public static void _fromDic(List<byte> dat, Dictionary<Pullenti.Address.GarParam, string> dic)
        {
            dat.AddRange(BitConverter.GetBytes((short)dic.Count));
            foreach (KeyValuePair<Pullenti.Address.GarParam, string> kp in dic) 
            {
                dat.Add((byte)((int)kp.Key));
                Pullenti.Util.Repository.BaseTable.GetBytesForString(dat, kp.Value, null);
            }
        }
        public void PutParams(int id, Dictionary<Pullenti.Address.GarParam, string> dic, bool zip = false)
        {
            List<byte> dat = new List<byte>();
            _fromDic(dat, dic);
            bool b = AutoZipData;
            if (zip) 
                AutoZipData = true;
            this.WriteKeyData(id, dat.ToArray());
            AutoZipData = b;
        }
    }
}