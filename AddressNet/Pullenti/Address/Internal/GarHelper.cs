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

namespace Pullenti.Address.Internal
{
    public static class GarHelper
    {
        public static List<Pullenti.Address.GarObject> Regions = new List<Pullenti.Address.GarObject>();
        static object m_Lock = new object();
        public static Pullenti.Address.Internal.Gar.FiasDatabase GarIndex;
        public static void Init(string indexPath)
        {
            Regions = new List<Pullenti.Address.GarObject>();
            if (GarIndex != null) 
            {
                GarIndex.Dispose();
                GarIndex = null;
            }
            if (indexPath != null) 
            {
                if (!Directory.Exists(indexPath)) 
                    throw new Exception(string.Format("Directory '{0}' not exists", indexPath));
                GarIndex = new Pullenti.Address.Internal.Gar.FiasDatabase();
                GarIndex.Initialize(indexPath);
            }
            if (GarIndex == null) 
                return;
            Pullenti.Address.Internal.Gar.AreaObject robj = GarIndex.GetAO(1);
            if (robj == null) 
                return;
            List<Pullenti.Address.GarObject> ga = new List<Pullenti.Address.GarObject>();
            foreach (uint id in robj.ChildrenIds) 
            {
                Pullenti.Address.Internal.Gar.AreaObject ao = GarIndex.GetAO((int)id);
                if (ao == null) 
                    continue;
                if (ao.Level != 1) 
                    continue;
                Pullenti.Address.GarObject g = CreateGarArea(ao);
                if ((g.Attrs is Pullenti.Address.AreaAttributes) && g.Level == Pullenti.Address.GarLevel.Region) 
                    ga.Add(g);
            }
            ga.Sort();
            foreach (Pullenti.Address.GarObject g in ga) 
            {
                Regions.Add(g);
            }
        }
        public static Pullenti.Address.GarObject GetObject(string sid)
        {
            if (sid == null || GarIndex == null) 
                return null;
            int iid;
            if (!int.TryParse(sid.Substring(1), out iid)) 
                return null;
            if (sid[0] == 'a') 
            {
                if (iid < 1) 
                    return null;
                string nam = GarIndex.GetAOName(iid);
                Pullenti.Address.Internal.Gar.AreaTreeObject prox = GarIndex.GetAOProxy(iid);
                if (nam == null || prox == null) 
                {
                    Pullenti.Address.Internal.Gar.AreaObject ao = GarIndex.GetAO(iid);
                    if (ao == null) 
                        return null;
                    return CreateGarArea(ao);
                }
                Pullenti.Address.AreaAttributes aa = new Pullenti.Address.AreaAttributes();
                Pullenti.Address.GarObject res = new Pullenti.Address.GarObject(aa);
                if (nam.IndexOf('+') < 0) 
                    aa.Names.Add(nam);
                else 
                    aa.Names.AddRange(nam.Split('+'));
                res.RegionNumber = prox.Region;
                Pullenti.Address.Internal.Gar.AreaType ty = GarIndex.GetAddrType(prox.TypId);
                if (ty != null) 
                    aa.Types.Add(ty.Name);
                if (prox.AltTypId > 0) 
                {
                    ty = GarIndex.GetAddrType(prox.AltTypId);
                    if (ty != null) 
                        aa.Types.Add(ty.Name);
                }
                res.Status = prox.Status;
                foreach (int pid in prox.ParentIds) 
                {
                    res.ParentIds.Add(string.Format("a{0}", pid));
                }
                res.Expired = prox.Expired;
                res.Level = prox.GLevel;
                res.Guid = GarIndex.GetAOGuid(iid);
                res.Id = sid;
                res.ChildrenCount = prox.ChCount;
                return res;
            }
            if (sid[0] == 'h') 
            {
                Pullenti.Address.Internal.Gar.HouseObject ho = GarIndex.GetHouse(iid);
                if (ho == null) 
                    return null;
                return CreateGarHouse(ho);
            }
            if (sid[0] == 'r') 
            {
                Pullenti.Address.Internal.Gar.RoomObject ho = GarIndex.GetRoom(iid);
                if (ho == null) 
                    return null;
                return CreateGarRoom(ho);
            }
            return null;
        }
        public static Dictionary<Pullenti.Address.GarParam, string> GetObjectParams(string sid)
        {
            if (GarIndex == null) 
                return null;
            int iid;
            if (!int.TryParse(sid.Substring(1), out iid)) 
                return null;
            if (sid[0] == 'a') 
                return GarIndex.GetAOParams(iid);
            if (sid[0] == 'h') 
                return GarIndex.GetHouseParams(iid);
            if (sid[0] == 'r') 
                return GarIndex.GetRoomParams(iid);
            return null;
        }
        public static List<Pullenti.Address.GarObject> GetChildrenObjects(string id, bool ignoreHouses = false)
        {
            if (string.IsNullOrEmpty(id)) 
                return Regions;
            List<Pullenti.Address.GarObject> res = GetChildrenObjectsById(id, ignoreHouses);
            if (res != null) 
            {
                foreach (Pullenti.Address.GarObject r in res) 
                {
                    if (id != null && !r.ParentIds.Contains(id)) 
                        r.ParentIds.Add(id);
                }
            }
            return res;
        }
        public static List<Pullenti.Address.GarObject> GetChildrenObjectsById(string sid, bool ignoreHouses = false)
        {
            if (GarIndex == null || string.IsNullOrEmpty(sid)) 
                return null;
            List<Pullenti.Address.GarObject> res = new List<Pullenti.Address.GarObject>();
            int iid;
            if (!int.TryParse(sid.Substring(1), out iid)) 
                return null;
            if (sid[0] == 'a') 
            {
                Pullenti.Address.Internal.Gar.AreaObject ao = GarIndex.GetAO(iid);
                if (ao == null) 
                    return null;
                if (ao.ChildrenIds != null) 
                {
                    List<Pullenti.Address.GarObject> areas = new List<Pullenti.Address.GarObject>();
                    List<Pullenti.Address.Internal.Gar.HouseObject> houses = new List<Pullenti.Address.Internal.Gar.HouseObject>();
                    List<Pullenti.Address.Internal.Gar.RoomObject> rooms = new List<Pullenti.Address.Internal.Gar.RoomObject>();
                    foreach (uint id in ao.ChildrenIds) 
                    {
                        uint mm = (uint)(id & Pullenti.Address.Internal.Gar.AreaObject.ROOMMASK);
                        if (mm == Pullenti.Address.Internal.Gar.AreaObject.ROOMMASK) 
                        {
                            if (ignoreHouses) 
                                continue;
                            Pullenti.Address.Internal.Gar.RoomObject ro = GarIndex.GetRoom((int)((id ^ Pullenti.Address.Internal.Gar.AreaObject.ROOMMASK)));
                            if (ro != null) 
                                rooms.Add(ro);
                        }
                        else if (mm == Pullenti.Address.Internal.Gar.AreaObject.HOUSEMASK) 
                        {
                            if (ignoreHouses) 
                                continue;
                            Pullenti.Address.Internal.Gar.HouseObject ho = GarIndex.GetHouse((int)((id ^ Pullenti.Address.Internal.Gar.AreaObject.HOUSEMASK)));
                            if (ho != null) 
                                houses.Add(ho);
                        }
                        else 
                        {
                            Pullenti.Address.GarObject ch = CreateGarAById((int)id);
                            if (ch != null) 
                                areas.Add(ch);
                        }
                    }
                    areas.Sort();
                    houses.Sort();
                    foreach (Pullenti.Address.GarObject a in areas) 
                    {
                        res.Add(a);
                    }
                    foreach (Pullenti.Address.Internal.Gar.HouseObject h in houses) 
                    {
                        Pullenti.Address.GarObject gh = CreateGarHouse(h);
                        if (gh != null) 
                            res.Add(gh);
                    }
                    foreach (Pullenti.Address.Internal.Gar.RoomObject r in rooms) 
                    {
                        Pullenti.Address.GarObject rh = CreateGarRoom(r);
                        if (rh != null) 
                            res.Add(rh);
                    }
                }
                return res;
            }
            if (sid[0] == 'h') 
            {
                Pullenti.Address.Internal.Gar.HouseObject ho = GarIndex.GetHouse(iid);
                if (ho == null || ho.RoomIds == null) 
                    return null;
                List<Pullenti.Address.Internal.Gar.RoomObject> rooms = new List<Pullenti.Address.Internal.Gar.RoomObject>();
                foreach (int id in ho.RoomIds) 
                {
                    Pullenti.Address.Internal.Gar.RoomObject ro = GarIndex.GetRoom(id);
                    if (ro != null) 
                        rooms.Add(ro);
                }
                rooms.Sort();
                foreach (Pullenti.Address.Internal.Gar.RoomObject r in rooms) 
                {
                    Pullenti.Address.GarObject gr = CreateGarRoom(r);
                    if (gr != null) 
                        res.Add(gr);
                }
            }
            if (sid[0] == 'r') 
            {
                Pullenti.Address.Internal.Gar.RoomObject ho = GarIndex.GetRoom(iid);
                if (ho == null || ho.ChildrenIds == null) 
                    return null;
                List<Pullenti.Address.Internal.Gar.RoomObject> rooms = new List<Pullenti.Address.Internal.Gar.RoomObject>();
                foreach (uint id in ho.ChildrenIds) 
                {
                    Pullenti.Address.Internal.Gar.RoomObject ro = GarIndex.GetRoom((int)id);
                    if (ro != null) 
                        rooms.Add(ro);
                }
                rooms.Sort();
                foreach (Pullenti.Address.Internal.Gar.RoomObject r in rooms) 
                {
                    Pullenti.Address.GarObject gr = CreateGarRoom(r);
                    if (gr != null) 
                        res.Add(gr);
                }
            }
            return res;
        }
        public static Pullenti.Address.GarObject CreateGarAById(int id)
        {
            Pullenti.Address.Internal.Gar.AreaObject aa = GarIndex.GetAO(id);
            if (aa == null) 
                return null;
            return CreateGarArea(aa);
        }
        public static Pullenti.Address.GarObject CreateGarArea(Pullenti.Address.Internal.Gar.AreaObject a)
        {
            Pullenti.Address.AreaAttributes aa = new Pullenti.Address.AreaAttributes();
            Pullenti.Address.GarObject ga = new Pullenti.Address.GarObject(aa);
            ga.Id = string.Format("a{0}", a.Id);
            ga.Status = (Pullenti.Address.GarStatus)a.Status;
            aa.Names.AddRange(a.Names);
            if (a.Typ != null) 
                aa.Types.Add(a.Typ.Name);
            if (a.OldTyp != null) 
                aa.Types.Add(a.OldTyp.Name);
            ga.Level = (Pullenti.Address.GarLevel)a.Level;
            ga.Expired = !a.Actual;
            ga.Guid = a.Guid;
            ga.RegionNumber = a.Region;
            if (a.ChildrenIds != null) 
                ga.ChildrenCount = a.ChildrenIds.Count;
            foreach (int ii in a.ParentIds) 
            {
                ga.ParentIds.Add(string.Format("a{0}", ii));
            }
            return ga;
        }
        public static Pullenti.Address.GarObject CreateGarHouse(Pullenti.Address.Internal.Gar.HouseObject a)
        {
            if (a == null) 
                return null;
            string sid = "h" + a.Id;
            Pullenti.Address.HouseAttributes ha = new Pullenti.Address.HouseAttributes();
            Pullenti.Address.GarObject ga = new Pullenti.Address.GarObject(ha);
            ga.Id = sid;
            ha.Number = a.HouseNumber;
            if (a.HouseTyp == 1) 
                ha.Typ = Pullenti.Address.HouseType.Estate;
            else if (a.HouseTyp == 2) 
                ha.Typ = Pullenti.Address.HouseType.House;
            else if (a.HouseTyp == 3) 
                ha.Typ = Pullenti.Address.HouseType.HouseEstate;
            else if (a.HouseTyp == 4) 
                ha.Typ = Pullenti.Address.HouseType.Garage;
            else if (a.HouseTyp == 5) 
                ha.Typ = Pullenti.Address.HouseType.Plot;
            ha.BuildNumber = a.BuildNumber;
            ha.StroenNumber = a.StrucNumber;
            ha.StroenTyp = (Pullenti.Address.StroenType)((int)a.StrucTyp);
            ga.Level = (ha.Typ == Pullenti.Address.HouseType.Plot ? Pullenti.Address.GarLevel.Plot : Pullenti.Address.GarLevel.Building);
            ga.Expired = !a.Actual;
            ga.Guid = a.Guid;
            ga.Status = a.Status;
            if (a.ParentId > 0) 
                ga.ParentIds.Add("a" + a.ParentId);
            if (a.AltParentId > 0) 
                ga.ParentIds.Add("a" + a.AltParentId);
            ga.ChildrenCount = (a.RoomIds == null ? 0 : a.RoomIds.Count);
            return ga;
        }
        public static Pullenti.Address.GarObject CreateGarRoom(Pullenti.Address.Internal.Gar.RoomObject a)
        {
            string sid = "r" + a.Id;
            Pullenti.Address.RoomAttributes ra = new Pullenti.Address.RoomAttributes();
            Pullenti.Address.GarObject ga = new Pullenti.Address.GarObject(ra);
            ga.Id = sid;
            ra.Number = a.Number;
            ra.Typ = a.Typ;
            ga.Level = Pullenti.Address.GarLevel.Room;
            ga.Expired = !a.Actual;
            ga.Guid = a.Guid;
            if (a.ChildrenIds != null) 
                ga.ChildrenCount = a.ChildrenIds.Count;
            if (a.HouseId != 0 && ((a.HouseId & 0x80000000)) == 0) 
                ga.ParentIds.Add("h" + a.HouseId);
            else if (a.HouseId != 0 && ((a.HouseId & 0x80000000)) != 0) 
            {
                uint id = (uint)(a.HouseId & 0x7FFFFFFF);
                ga.ParentIds.Add("h" + id);
            }
            return ga;
        }
        public static Pullenti.Address.AddrObject CreateAddrObject(Pullenti.Address.GarObject g)
        {
            Pullenti.Address.AddrObject res = new Pullenti.Address.AddrObject(g.Attrs);
            res.IsReconstructed = true;
            res.Gars.Add(g);
            if (g.Level == Pullenti.Address.GarLevel.Region) 
            {
                res.Level = Pullenti.Address.AddrLevel.RegionArea;
                if ((g.Attrs is Pullenti.Address.AreaAttributes) && (g.Attrs as Pullenti.Address.AreaAttributes).Types.Contains("город")) 
                    res.Level = Pullenti.Address.AddrLevel.RegionCity;
            }
            else if (g.Level == Pullenti.Address.GarLevel.AdminArea || g.Level == Pullenti.Address.GarLevel.MunicipalArea) 
                res.Level = Pullenti.Address.AddrLevel.District;
            else if (g.Level == Pullenti.Address.GarLevel.Settlement) 
                res.Level = Pullenti.Address.AddrLevel.Settlement;
            else if (g.Level == Pullenti.Address.GarLevel.City) 
                res.Level = Pullenti.Address.AddrLevel.City;
            else if (g.Level == Pullenti.Address.GarLevel.Locality) 
                res.Level = Pullenti.Address.AddrLevel.Locality;
            else if (g.Level == Pullenti.Address.GarLevel.Area) 
                res.Level = Pullenti.Address.AddrLevel.Territory;
            else if (g.Level == Pullenti.Address.GarLevel.Street) 
                res.Level = Pullenti.Address.AddrLevel.Street;
            else if (g.Level == Pullenti.Address.GarLevel.Plot) 
            {
                res.Level = Pullenti.Address.AddrLevel.Plot;
                (res.Attrs as Pullenti.Address.HouseAttributes).Typ = Pullenti.Address.HouseType.Plot;
            }
            else if (g.Level == Pullenti.Address.GarLevel.Building) 
                res.Level = Pullenti.Address.AddrLevel.Building;
            else if (g.Level == Pullenti.Address.GarLevel.Room) 
                res.Level = Pullenti.Address.AddrLevel.Apartment;
            else 
                return null;
            return res;
        }
    }
}