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

namespace Pullenti.Address.Internal.Gar
{
    public class AreaTreeObject : IComparable<AreaTreeObject>
    {
        public int Id;
        public byte Region;
        public Pullenti.Address.AddrLevel Level;
        public Pullenti.Address.GarLevel GLevel;
        public List<string> Typs;
        public List<string> Miscs;
        public List<int> ParentIds = new List<int>();
        public List<int> ParentParentIds;
        public bool Expired;
        public Pullenti.Address.GarStatus Status;
        public short ChCount;
        public short TypId;
        public short AltTypId;
        public int CheckType(Pullenti.Address.Internal.NameAnalyzer na)
        {
            Pullenti.Address.AddrLevel alev = Level;
            if (alev == Pullenti.Address.AddrLevel.City || alev == Pullenti.Address.AddrLevel.Country) 
            {
                if (GLevel == Pullenti.Address.GarLevel.Street) 
                    alev = Pullenti.Address.AddrLevel.Street;
            }
            if (alev != na.Level) 
            {
                if (alev == Pullenti.Address.AddrLevel.Settlement && (na.Level == Pullenti.Address.AddrLevel.Locality)) 
                {
                }
                else if (alev == Pullenti.Address.AddrLevel.Locality && na.Level == Pullenti.Address.AddrLevel.Territory) 
                {
                }
                else if (alev == Pullenti.Address.AddrLevel.Locality && na.Level == Pullenti.Address.AddrLevel.City && na.Types.Contains("населенный пункт")) 
                {
                }
                else if (alev == Pullenti.Address.AddrLevel.Territory && (na.Level == Pullenti.Address.AddrLevel.Locality)) 
                {
                }
                else if ((alev == Pullenti.Address.AddrLevel.Territory && na.Level == Pullenti.Address.AddrLevel.District && na.Types.Count == 1) && na.Types[0] == "муниципальный район") 
                {
                }
                else if (alev == Pullenti.Address.AddrLevel.Street && na.Level == Pullenti.Address.AddrLevel.Territory) 
                {
                }
                else if (alev == Pullenti.Address.AddrLevel.City && ((na.Level == Pullenti.Address.AddrLevel.RegionCity || na.Level == Pullenti.Address.AddrLevel.Locality))) 
                {
                }
                else if ((alev == Pullenti.Address.AddrLevel.City && na.Level == Pullenti.Address.AddrLevel.Locality && na.Types.Count == 1) && na.Types[0] == "населенный пункт") 
                {
                }
                else if (alev == Pullenti.Address.AddrLevel.Territory && na.Level == Pullenti.Address.AddrLevel.Street) 
                {
                    if (na.Types.Count == 0 || ((na.Types.Count == 1 && na.Types[0] == "улица"))) 
                    {
                        if (Miscs != null) 
                        {
                            if (Miscs.Contains("гаражи") || Miscs.Contains("месторождение") || Miscs.Contains("дачи")) 
                                return -1;
                        }
                    }
                    else 
                    {
                        bool ok = false;
                        if (Miscs != null && na.Miscs != null) 
                        {
                            foreach (string m in Miscs) 
                            {
                                if (na.Miscs.Contains(m)) 
                                    ok = true;
                            }
                        }
                        if (!ok) 
                            return -1;
                    }
                }
                else if ((alev == Pullenti.Address.AddrLevel.District && na.Level == Pullenti.Address.AddrLevel.Settlement && Typs.Count > 0) && Typs[0].Contains("округ")) 
                {
                }
                else if (alev == Pullenti.Address.AddrLevel.Locality && na.Types.Count > 0 && na.Types.Contains("улус")) 
                {
                }
                else 
                    return -1;
            }
            if (Typs != null && na.Types != null && na.Types.Count > 0) 
            {
                foreach (string ty in Typs) 
                {
                    if (na.Types.Contains(ty)) 
                    {
                        if (ty != "территория") 
                            return (alev == na.Level ? 1 : 0);
                        if (Miscs != null && na.Miscs != null) 
                        {
                            foreach (string m in Miscs) 
                            {
                                if (na.Miscs.Contains(m)) 
                                    return (alev == na.Level ? 1 : 0);
                                if (char.IsLower(m[0])) 
                                {
                                    char ch0 = char.ToUpper(m[0]);
                                    foreach (string mm in na.Miscs) 
                                    {
                                        if (mm[mm.Length - 1] == ch0) 
                                            return (alev == na.Level ? 1 : 0);
                                    }
                                }
                            }
                        }
                    }
                }
                if (alev == Pullenti.Address.AddrLevel.Territory && na.Level == Pullenti.Address.AddrLevel.Territory) 
                {
                    if ((Typs.Count == 1 && na.Types.Count == 1 && Miscs == null) && na.Miscs.Count == 0) 
                        return 0;
                    return -1;
                }
                if (alev == Pullenti.Address.AddrLevel.Street && na.Level == Pullenti.Address.AddrLevel.Street) 
                {
                    if (Typs.Count > 0 && na.Types.Count > 0) 
                        return -1;
                }
                if (((alev == Pullenti.Address.AddrLevel.Street && na.Level == Pullenti.Address.AddrLevel.Territory)) || ((alev == Pullenti.Address.AddrLevel.Territory && na.Level == Pullenti.Address.AddrLevel.Street))) 
                {
                    if (Miscs != null && na.Miscs != null) 
                    {
                        foreach (string m in Miscs) 
                        {
                            if (na.Miscs.Contains(m)) 
                                return 0;
                        }
                    }
                    if (na.Types.Count == 1 && na.Types[0] == "улица") 
                        return 0;
                    if (Status == Pullenti.Address.GarStatus.Ok2) 
                        return 0;
                    return -1;
                }
            }
            return 0;
        }
        public override string ToString()
        {
            StringBuilder tmp = new StringBuilder();
            tmp.AppendFormat("{0}", Id);
            if (Typs != null) 
            {
                tmp.Append(" (");
                foreach (string ty in Typs) 
                {
                    if (ty != Typs[0]) 
                        tmp.Append("/");
                    tmp.Append(ty);
                }
                tmp.Append(")");
            }
            if (Miscs != null) 
            {
                tmp.Append(" [");
                foreach (string ty in Miscs) 
                {
                    if (ty != Miscs[0]) 
                        tmp.Append("/");
                    tmp.Append(ty);
                }
                tmp.Append("]");
            }
            foreach (int p in ParentIds) 
            {
                tmp.AppendFormat("{0}{1}", (p == ParentIds[0] ? " " : "/"), p);
            }
            if (ParentParentIds != null) 
            {
                foreach (int id in ParentParentIds) 
                {
                    tmp.AppendFormat("{0}{1}", (id == ParentParentIds[0] ? " -" : ","), id);
                }
            }
            tmp.AppendFormat(",r={0},l={1}", Region, Level);
            if (Expired) 
                tmp.Append(",expired");
            if (Status != Pullenti.Address.GarStatus.Ok) 
                tmp.AppendFormat(",{0}", Status);
            return tmp.ToString();
        }
        public int CompareTo(AreaTreeObject other)
        {
            if (Id < other.Id) 
                return -1;
            if (Id > other.Id) 
                return 1;
            return 0;
        }
        internal void Serialize(Stream f, AreaTree tr)
        {
            Pullenti.Address.Internal.FiasHelper.SerializeInt(f, Id);
            f.WriteByte(Region);
            byte b = (byte)((int)Level);
            if (Expired) 
                b |= 0x80;
            if (Status == Pullenti.Address.GarStatus.Error) 
                b |= 0x40;
            if (Status == Pullenti.Address.GarStatus.Warning) 
                b |= 0x20;
            if (Status == Pullenti.Address.GarStatus.Ok2) 
                b |= 0x60;
            f.WriteByte(b);
            f.WriteByte((byte)((Typs == null ? 0 : Typs.Count)));
            if (Typs != null) 
            {
                foreach (string ty in Typs) 
                {
                    Pullenti.Address.Internal.FiasHelper.SerializeShort(f, tr.GetStringId(ty));
                }
            }
            f.WriteByte((byte)((Miscs == null ? 0 : Miscs.Count)));
            if (Miscs != null) 
            {
                foreach (string ty in Miscs) 
                {
                    Pullenti.Address.Internal.FiasHelper.SerializeShort(f, tr.GetStringId(ty));
                }
            }
            f.WriteByte((byte)ParentIds.Count);
            if (ParentIds.Count > 0) 
            {
                foreach (int p in ParentIds) 
                {
                    Pullenti.Address.Internal.FiasHelper.SerializeInt(f, p);
                }
                f.WriteByte((byte)((ParentParentIds == null ? 0 : ParentParentIds.Count)));
                if (ParentParentIds != null) 
                {
                    foreach (int id in ParentParentIds) 
                    {
                        Pullenti.Address.Internal.FiasHelper.SerializeInt(f, id);
                    }
                }
            }
            f.WriteByte((byte)((int)GLevel));
            Pullenti.Address.Internal.FiasHelper.SerializeShort(f, ChCount);
            Pullenti.Address.Internal.FiasHelper.SerializeShort(f, TypId);
            Pullenti.Address.Internal.FiasHelper.SerializeShort(f, AltTypId);
        }
        internal void Deserialize(byte[] dat, int pos, AreaTree tr)
        {
            Id = BitConverter.ToInt32(dat, pos);
            pos += 4;
            Region = dat[pos];
            pos++;
            byte b = dat[pos];
            pos++;
            if (((b & 0x80)) != 0) 
            {
                Expired = true;
                b &= 0x7F;
            }
            if (((b & 0x40)) != 0) 
            {
                if (((b & 0x20)) != 0) 
                {
                    Status = Pullenti.Address.GarStatus.Ok2;
                    b &= 0x1F;
                }
                else 
                {
                    Status = Pullenti.Address.GarStatus.Error;
                    b &= 0x3F;
                }
            }
            if (((b & 0x20)) != 0) 
            {
                Status = Pullenti.Address.GarStatus.Warning;
                b &= 0x1F;
            }
            Level = (Pullenti.Address.AddrLevel)((int)b);
            int cou1 = (int)dat[pos];
            pos++;
            if (cou1 > 0) 
            {
                Typs = new List<string>();
                for (; cou1 > 0; cou1--) 
                {
                    string s = tr.GetString(BitConverter.ToInt16(dat, pos));
                    pos += 2;
                    if (s != null) 
                        Typs.Add(s);
                }
            }
            cou1 = dat[pos];
            pos++;
            if (cou1 > 0) 
            {
                Miscs = new List<string>();
                for (; cou1 > 0; cou1--) 
                {
                    string s = tr.GetString(BitConverter.ToInt16(dat, pos));
                    pos += 2;
                    if (s != null) 
                        Miscs.Add(s);
                }
            }
            cou1 = dat[pos];
            pos++;
            if (cou1 > 0) 
            {
                for (; cou1 > 0; cou1--,pos += 4) 
                {
                    ParentIds.Add(BitConverter.ToInt32(dat, pos));
                }
                cou1 = dat[pos];
                pos++;
                if (cou1 > 0) 
                {
                    ParentParentIds = new List<int>();
                    for (; cou1 > 0; cou1--,pos += 4) 
                    {
                        ParentParentIds.Add(BitConverter.ToInt32(dat, pos));
                    }
                }
            }
            if (pos >= dat.Length) 
                return;
            b = dat[pos];
            pos++;
            GLevel = (Pullenti.Address.GarLevel)((int)b);
            ChCount = BitConverter.ToInt16(dat, pos);
            pos += 2;
            TypId = BitConverter.ToInt16(dat, pos);
            pos += 2;
            AltTypId = BitConverter.ToInt16(dat, pos);
            pos += 2;
        }
    }
}