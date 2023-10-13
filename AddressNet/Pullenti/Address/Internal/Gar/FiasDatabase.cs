/*
 * SDK Pullenti Address, version 4.20, august 2023. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Pullenti.Address.Internal.Gar
{
    // База данных ФИАС (ГАР)
    public class FiasDatabase : Pullenti.Util.Repository.IRepository
    {
        public void InitAddrmap()
        {
            m_AreaTree.Close();
            if (m_Types != null) 
            {
                foreach (AreaType ty in m_Types.Values) 
                {
                    ty.Count = 0;
                }
            }
        }
        public void SaveAddrmap()
        {
            string fname = Path.Combine(BaseDir, "areatree.dat");
            m_AreaTree.Save(fname);
            fname = Path.Combine(BaseDir, "types.xml");
            if (File.Exists(fname)) 
                File.Delete(fname);
            if (Id == null) 
                Id = Guid.NewGuid().ToString();
            if (CreateDate == null) 
                CreateDate = string.Format("{0}.{1}.{2}", DateTime.Now.Year, DateTime.Now.Month.ToString("D02"), DateTime.Now.Day.ToString("D02"));
            AreaType.Save(fname, m_Types, Id, CreateDate);
            m_AreaTree.Close();
        }
        public void LoadAllStringEntries()
        {
            m_AreaTree.LoadAllData();
        }
        public void CommitStringEntries()
        {
            string fname = Path.Combine(BaseDir, "areatree.dat");
            m_AreaTree.Save(fname);
            m_AreaTree.Load(fname);
        }
        public void CreateRegions()
        {
            string regFile = Path.Combine(BaseDir, "regions.xml");
            Pullenti.Address.Internal.RegionHelper.Create();
            Pullenti.Address.Internal.RegionHelper.SaveToFile(regFile);
        }
        public void SaveParams(bool dontOptimize = false)
        {
            if (!dontOptimize) 
            {
                m_HousesInAo.Flush();
                m_RoomsInHouse.Flush();
                m_RoomsInRooms.Flush();
                Pullenti.Util.ConsoleHelper.Write("\r\nOptimizing HOUSEPARAMS ... ");
                m_HouseParams.Optimize(0);
                Pullenti.Util.ConsoleHelper.Write(" OK");
                Pullenti.Util.ConsoleHelper.Write("\r\nOptimizing ROOMPARAMS ... ");
                m_RoomParams.Optimize(0);
                Pullenti.Util.ConsoleHelper.Write(" OK");
            }
            Dictionary<int, byte[]> datdic = new Dictionary<int, byte[]>();
            foreach (KeyValuePair<Pullenti.Address.GarParam, PTreeRoot> pm in m_ParamsMaps) 
            {
                Pullenti.Util.ConsoleHelper.Write(string.Format("\r\nCreate param index for {0} ... ", pm.Key));
                Console.Write("\r\n");
                Stopwatch sw = new Stopwatch();
                sw.Start();
                int cou = 0;
                for (int k = 0; k < 3; k++) 
                {
                    if (k == 2) 
                    {
                        if (pm.Key == Pullenti.Address.GarParam.Guid || pm.Key == Pullenti.Address.GarParam.ReesterNumber) 
                        {
                        }
                        else 
                            continue;
                    }
                    else if (k == 1) 
                    {
                        if (pm.Key == Pullenti.Address.GarParam.KladrCode || pm.Key == Pullenti.Address.GarParam.Okato || pm.Key == Pullenti.Address.GarParam.Oktmo) 
                            continue;
                    }
                    Console.Write(string.Format("      {0}\r", (k == 0 ? "ADDROBJS" : (k == 1 ? "HOUSES   " : "ROOMS   "))));
                    Pullenti.Util.Repository.KeyBaseTable ptab = (Pullenti.Util.Repository.KeyBaseTable)(k == 0 ? m_AddrParams : (k == 1 ? m_HouseParams : m_RoomParams));
                    if (k > 0 && pm.Key == Pullenti.Address.GarParam.Guid) 
                    {
                        if (k == 1) 
                            ptab = m_HouseTable;
                        else 
                            ptab = m_RoomTable;
                    }
                    ptab.BeginFetch();
                    HouseObject ho = new HouseObject();
                    RoomObject ro = new RoomObject();
                    while (true) 
                    {
                        Console.Write("{0}%\r", ptab.FetchPercent());
                        datdic.Clear();
                        ptab.FetchDic(datdic, 100000);
                        if (datdic.Count == 0) 
                            break;
                        foreach (KeyValuePair<int, byte[]> kp in datdic) 
                        {
                            string val = null;
                            if (pm.Key == Pullenti.Address.GarParam.Guid && k == 1) 
                            {
                                ho.Guid = null;
                                if (FiasHouseTable._restore(kp.Value, ho, 0)) 
                                    val = ho.Guid;
                            }
                            else if (pm.Key == Pullenti.Address.GarParam.Guid && k == 2) 
                            {
                                ro.Guid = null;
                                int ind = 0;
                                if (FiasRoomTable._restore(kp.Value, ro, ref ind)) 
                                    val = ro.Guid;
                            }
                            else 
                                val = ParamsTable._getVal(kp.Value, pm.Key);
                            if (val == null) 
                                continue;
                            uint ui = (uint)kp.Key;
                            if (k == 1) 
                                ui |= AreaObject.HOUSEMASK;
                            else if (k == 2) 
                                ui |= AreaObject.ROOMMASK;
                            pm.Value.Add(val, ui);
                            cou++;
                        }
                    }
                }
                sw.Stop();
                Pullenti.Util.ConsoleHelper.Write(string.Format("Write {0} values by {1} min {2} sec", cou, (int)sw.Elapsed.TotalMinutes, ((int)sw.Elapsed.TotalSeconds) % 60));
                pm.Value.Save(Path.Combine(BaseDir, string.Format("paramap{0}.dat", (int)pm.Key)));
                Pullenti.Util.ConsoleHelper.WriteMemory(true);
            }
        }
        public void CreateAreaNames()
        {
            m_AddrTable.BeginFetch();
            FileStream guids = new FileStream(Path.Combine(BaseDir, "areaguids.dat"), FileMode.Create, FileAccess.Write);
            FileStream aind = new FileStream(Path.Combine(BaseDir, "areanames.ind"), FileMode.Create, FileAccess.Write);
            FileStream adat = new FileStream(Path.Combine(BaseDir, "areanames.dat"), FileMode.Create, FileAccess.Write);
            int maxid = 0;
            while (true) 
            {
                Dictionary<int, byte[]> dats = m_AddrTable.Fetch(10000);
                if (dats == null || dats.Count == 0) 
                    break;
                foreach (KeyValuePair<int, byte[]> kp in dats) 
                {
                    AreaObject ao = new AreaObject();
                    if (!FiasAddrTable._restore(kp.Value, ao, m_Types)) 
                        continue;
                    if (ao.Guid != null) 
                    {
                        guids.Position = 16 * kp.Key;
                        Guid g = new Guid(ao.Guid);
                        byte[] arr = g.ToByteArray();
                        guids.Write(arr, 0, arr.Length);
                    }
                    maxid = kp.Key;
                    if (ao.Names.Count == 0) 
                        continue;
                    string str = ao.Names[0];
                    if (ao.Names.Count > 1) 
                        str = string.Format("{0}+{1}", str, ao.Names[1]);
                    if (ao.Names.Count > 2) 
                        str = string.Format("{0}+{1}", str, ao.Names[2]);
                    byte[] dat = Pullenti.Address.Internal.FiasHelper.EncodeString1251(str);
                    aind.Position = 4 * kp.Key;
                    aind.Write(BitConverter.GetBytes((int)adat.Length), 0, 4);
                    adat.Write(dat, 0, dat.Length);
                    adat.WriteByte((byte)0);
                }
            }
            guids.Close();
            aind.Close();
            adat.Close();
        }
        public string BaseDir
        {
            get;
            set;
        }
        public string Id;
        public string CreateDate;
        public bool ReadOnly = true;
        public void Initialize(string dirName)
        {
            BaseDir = dirName;
            if (!Directory.Exists(dirName)) 
                Directory.CreateDirectory(dirName);
            m_AddrTable = new FiasAddrTable(this);
            if (ReadOnly) 
            {
                FileInfo fi = new FileInfo(Path.Combine(BaseDir, "areaobjects.ind"));
                m_AddrTable.Open(ReadOnly, (fi.Exists ? (int)fi.Length : -1));
            }
            else 
                m_AddrTable.Open(false, 0);
            m_HouseTable = new FiasHouseTable(this);
            if (!m_HouseTable.Open(ReadOnly, 0)) 
                m_HouseTable = null;
            m_RoomTable = new FiasRoomTable(this);
            if (!m_RoomTable.Open(ReadOnly, 0)) 
                m_RoomTable = null;
            m_HousesInAo = new Pullenti.Util.Repository.KeyBaseTable(this, "houseinareas") { AutoZipData = true };
            if (!m_HousesInAo.Open(ReadOnly, 0)) 
                m_HousesInAo = null;
            m_RoomsInHouse = new Pullenti.Util.Repository.KeyBaseTable(this, "roominhouses") { AutoZipData = true };
            if (!m_RoomsInHouse.Open(ReadOnly, 0)) 
                m_RoomsInHouse = null;
            m_RoomsInRooms = new Pullenti.Util.Repository.KeyBaseTable(this, "roominrooms") { AutoZipData = true };
            if (!m_RoomsInRooms.Open(ReadOnly, 0)) 
                m_RoomsInRooms = null;
            m_AddrParams = new ParamsTable(this, "areaparams");
            if (!m_AddrParams.Open(ReadOnly, 0)) 
                m_AddrParams = null;
            m_HouseParams = new ParamsTable(this, "houseparams");
            if (!m_HouseParams.Open(ReadOnly, 0)) 
                m_HouseParams = null;
            m_RoomParams = new ParamsTable(this, "roomparams");
            if (!m_RoomParams.Open(ReadOnly, 0)) 
                m_RoomParams = null;
            string fname = Path.Combine(BaseDir, "types.xml");
            if (!File.Exists(fname)) 
                fname = Path.Combine(BaseDir, "types.dat");
            if (File.Exists(fname)) 
            {
                string id = null;
                string dt = null;
                Dictionary<int, AreaType> typs = AreaType.Load(fname, ref id, ref dt);
                if (typs != null) 
                    m_Types = typs;
                Id = id;
                CreateDate = dt;
            }
            else 
            {
                Id = Guid.NewGuid().ToString();
                CreateDate = string.Format("{0}.{1}.{2}", DateTime.Now.Year, DateTime.Now.Month.ToString("D02"), DateTime.Now.Day.ToString("D02"));
            }
            m_AreaTree = new AreaTree();
            fname = Path.Combine(BaseDir, "areatree.dat");
            if (File.Exists(fname)) 
                m_AreaTree.Load(fname);
            fname = Path.Combine(BaseDir, "areaguids.dat");
            if (File.Exists(fname)) 
                m_AreaGuids = File.ReadAllBytes(fname);
            fname = Path.Combine(BaseDir, "areanames.ind");
            if (File.Exists(fname)) 
                m_AreaNamePos = File.ReadAllBytes(fname);
            fname = Path.Combine(BaseDir, "areanames.dat");
            if (File.Exists(fname)) 
                m_AreaNameData = File.ReadAllBytes(fname);
            foreach (Pullenti.Address.GarParam ty in m_ParamTypes) 
            {
                fname = Path.Combine(BaseDir, string.Format("paramap{0}.dat", (int)ty));
                PTreeRoot tn = new PTreeRoot();
                if (ty == Pullenti.Address.GarParam.KladrCode || ty == Pullenti.Address.GarParam.KadasterNumber || ty == Pullenti.Address.GarParam.ReesterNumber) 
                    tn.MaxLength = 8;
                else if (ty == Pullenti.Address.GarParam.Guid) 
                    tn.MaxLength = 5;
                try 
                {
                    if (File.Exists(fname)) 
                        tn.Load(fname);
                }
                catch(Exception ex) 
                {
                }
                m_ParamsMaps.Add(ty, tn);
            }
            AreaObject roots = this.GetAO(1);
            if (roots != null && roots.ChildrenIds != null) 
            {
                foreach (uint id in roots.ChildrenIds) 
                {
                    if (((id & AreaObject.ROOMMASK)) != 0) 
                        continue;
                    int uid = (int)id;
                    AreaObject ao = this.GetAO(uid);
                    if (ao == null || ao.Typ == null) 
                        continue;
                    if (ao.Level != 1 || ao.Region == 0) 
                        continue;
                    if (!m_AoByRegs.ContainsKey(ao.Region)) 
                        m_AoByRegs.Add(ao.Region, ao);
                }
            }
        }
        Dictionary<int, AreaObject> m_AoByRegs = new Dictionary<int, AreaObject>();
        Dictionary<int, AreaType> m_Types = new Dictionary<int, AreaType>();
        public AreaType AddAddrType(string typ)
        {
            AreaType ty = null;
            foreach (KeyValuePair<int, AreaType> kp in m_Types) 
            {
                if (kp.Value.Name == typ) 
                {
                    ty = kp.Value;
                    break;
                }
            }
            if (ty == null) 
            {
                ty = new AreaType() { Id = m_Types.Count + 1, Name = typ };
                m_Types.Add(ty.Id, ty);
            }
            return ty;
        }
        public List<AreaType> GetAddrTypes()
        {
            return new List<AreaType>(m_Types.Values);
        }
        public AreaType GetAddrType(short id)
        {
            AreaType res;
            if (!m_Types.TryGetValue(id, out res)) 
                return null;
            else 
                return res;
        }
        public int AreasCount
        {
            get
            {
                if (m_AddrTable == null) 
                    return 0;
                return m_AddrTable.GetMaxKey();
            }
        }
        public int HousesCount
        {
            get
            {
                if (m_HouseTable == null) 
                    return 0;
                return m_HouseTable.GetMaxKey();
            }
        }
        public int RoomsCount
        {
            get
            {
                if (m_RoomTable == null) 
                    return 0;
                return m_RoomTable.GetMaxKey();
            }
        }
        FiasAddrTable m_AddrTable;
        FiasHouseTable m_HouseTable;
        FiasRoomTable m_RoomTable;
        Pullenti.Util.Repository.KeyBaseTable m_HousesInAo;
        Pullenti.Util.Repository.KeyBaseTable m_RoomsInHouse;
        Pullenti.Util.Repository.KeyBaseTable m_RoomsInRooms;
        AreaTree m_AreaTree;
        ParamsTable m_AddrParams;
        ParamsTable m_HouseParams;
        ParamsTable m_RoomParams;
        byte[] m_AreaGuids;
        byte[] m_AreaNamePos;
        byte[] m_AreaNameData;
        Dictionary<Pullenti.Address.GarParam, PTreeRoot> m_ParamsMaps = new Dictionary<Pullenti.Address.GarParam, PTreeRoot>();
        static Pullenti.Address.GarParam[] m_ParamTypes = new Pullenti.Address.GarParam[] {Pullenti.Address.GarParam.KadasterNumber, Pullenti.Address.GarParam.KladrCode, Pullenti.Address.GarParam.Okato, Pullenti.Address.GarParam.Oktmo, Pullenti.Address.GarParam.PostIndex, Pullenti.Address.GarParam.ReesterNumber, Pullenti.Address.GarParam.Guid};
        public void _Close()
        {
            if (m_AddrTable != null) 
            {
                m_AddrTable._Close();
                m_AddrTable = null;
            }
            if (m_AreaTree != null) 
            {
                m_AreaTree.Close();
                m_AreaTree = null;
            }
            if (m_HouseTable != null) 
            {
                m_HouseTable._Close();
                m_HouseTable = null;
            }
            if (m_RoomTable != null) 
            {
                m_RoomTable._Close();
                m_RoomTable = null;
            }
            if (m_HousesInAo != null) 
            {
                m_HousesInAo._Close();
                m_HousesInAo = null;
            }
            if (m_RoomsInHouse != null) 
            {
                m_RoomsInHouse._Close();
                m_RoomsInHouse = null;
            }
            if (m_RoomsInRooms != null) 
            {
                m_RoomsInRooms._Close();
                m_RoomsInRooms = null;
            }
            if (m_AddrParams != null) 
            {
                m_AddrParams._Close();
                m_AddrParams = null;
            }
            if (m_HouseParams != null) 
            {
                m_HouseParams._Close();
                m_HouseParams = null;
            }
            if (m_RoomParams != null) 
            {
                m_RoomParams._Close();
                m_RoomParams = null;
            }
            foreach (KeyValuePair<Pullenti.Address.GarParam, PTreeRoot> kp in m_ParamsMaps) 
            {
                kp.Value.Close();
            }
            m_ParamsMaps.Clear();
            m_AreaGuids = null;
            m_AreaNamePos = null;
            m_AreaNameData = null;
        }
        public void Collect()
        {
            if (m_AreaTree != null) 
                m_AreaTree.Collect();
            foreach (KeyValuePair<Pullenti.Address.GarParam, PTreeRoot> kp in m_ParamsMaps) 
            {
                kp.Value.Collect();
            }
        }
        public void Clear()
        {
        }
        public bool OutLog
        {
            get;
            set;
        }
        public void Dispose()
        {
            this._Close();
        }
        public List<uint> FindByParam(Pullenti.Address.GarParam ty, string value)
        {
            PTreeRoot p;
            if (!m_ParamsMaps.TryGetValue(ty, out p)) 
                return null;
            PTreeNode tn = p.Find(value);
            if (tn == null) 
                return null;
            List<uint> res = new List<uint>();
            foreach (uint ui in tn.Ids) 
            {
                Dictionary<Pullenti.Address.GarParam, string> pars = null;
                if (((ui & 0x80000000)) == 0) 
                    pars = this.GetAOParams((int)ui);
                else if (((ui & 0x40000000)) == 0) 
                {
                    if (ty == Pullenti.Address.GarParam.Guid) 
                    {
                        HouseObject ho = this.GetHouse((int)((ui & 0x3FFFFFFF)));
                        if (ho != null && ho.Guid == value) 
                            res.Add(ui);
                        continue;
                    }
                    pars = this.GetHouseParams((int)((ui & 0x3FFFFFFF)));
                }
                else 
                {
                    if (ty == Pullenti.Address.GarParam.Guid) 
                    {
                        RoomObject ho = this.GetRoom((int)((ui & 0x3FFFFFFF)));
                        if (ho != null && ho.Guid == value) 
                            res.Add(ui);
                        continue;
                    }
                    pars = this.GetRoomParams((int)((ui & 0x3FFFFFFF)));
                }
                if (pars == null) 
                    continue;
                string val;
                if (!pars.TryGetValue(ty, out val)) 
                    continue;
                if (val == value) 
                    res.Add(ui);
            }
            return res;
        }
        public int GetParentId(string sid)
        {
            int iid;
            if (!int.TryParse(sid.Substring(1), out iid)) 
                return 0;
            if (iid < 0) 
                return 0;
            if (sid[0] == 'a') 
            {
                lock (m_AddrTable.m_Lock) 
                {
                    return m_AddrTable.GetParentId(iid);
                }
            }
            if (sid[0] == 'h') 
            {
                lock (m_HouseTable.m_Lock) 
                {
                    return m_HouseTable.GetParentId(iid);
                }
            }
            if (sid[0] == 'r') 
            {
                lock (m_RoomTable.m_Lock) 
                {
                    return m_RoomTable.GetParentId(iid);
                }
            }
            return 0;
        }
        public Pullenti.Address.GarStatus GetStatus(int id)
        {
            return m_AddrTable.GetStatus(id);
        }
        public int GetActual(string sid)
        {
            int iid;
            if (!int.TryParse(sid.Substring(1), out iid)) 
                return -1;
            if (iid < 0) 
                return -1;
            if (sid[0] == 'a') 
            {
                lock (m_AddrTable.m_Lock) 
                {
                    return m_AddrTable.GetActual(iid);
                }
            }
            if (sid[0] == 'h') 
            {
                lock (m_HouseTable.m_Lock) 
                {
                    return m_HouseTable.GetActual(iid);
                }
            }
            if (sid[0] == 'r') 
            {
                lock (m_RoomTable.m_Lock) 
                {
                    return m_RoomTable.GetActual(iid);
                }
            }
            return 0;
        }
        public string GetAOGuid(int id)
        {
            int pos = id * 16;
            if (m_AreaGuids == null || (pos < 0) || (pos + 16) > m_AreaGuids.Length) 
                return null;
            byte[] dat = new byte[(int)16];
            for (int i = 0; i < 16; i++) 
            {
                dat[i] = m_AreaGuids[pos + i];
            }
            Guid g = new Guid(dat);
            return g.ToString();
        }
        public string GetAOName(int id)
        {
            int pos = id * 4;
            if ((pos < 0) || m_AreaNamePos == null || (pos + 4) > m_AreaNamePos.Length) 
                return null;
            int ind = BitConverter.ToInt32(m_AreaNamePos, pos);
            if ((ind < 0) || m_AreaNameData == null || ind >= m_AreaNameData.Length) 
                return null;
            return Pullenti.Address.Internal.FiasHelper.DecodeString1251(m_AreaNameData, ind, -1, true);
        }
        public AreaTreeObject GetAOProxy(int id)
        {
            if (m_AreaTree == null) 
                return null;
            lock (m_AreaTree.m_Lock) 
            {
                return m_AreaTree.GetObj(id);
            }
        }
        public AreaObject GetAO(int id)
        {
            if (m_AddrTable == null) 
                return null;
            lock (m_AddrTable.m_Lock) 
            {
                AreaObject ao = new AreaObject() { Id = id };
                if (m_AddrTable.Get(id, ao, m_Types)) 
                    return ao;
                else 
                    return null;
            }
        }
        public List<AreaObject> GetAOChildren(AreaObject ao)
        {
            if (ao == null || ao.ChildrenIds == null || ao.ChildrenIds.Count == 0) 
                return null;
            List<AreaObject> res = new List<AreaObject>();
            lock (m_AddrTable.m_Lock) 
            {
                foreach (uint uid in ao.ChildrenIds) 
                {
                    uint mm = (uint)(uid & AreaObject.ROOMMASK);
                    if (mm != 0) 
                        continue;
                    AreaObject ao1 = new AreaObject() { Id = (int)uid };
                    if (m_AddrTable.Get(ao1.Id, ao1, m_Types)) 
                        res.Add(ao1);
                }
            }
            return res;
        }
        public AreaObject GetAOByReg(int regId)
        {
            AreaObject res;
            if (m_AoByRegs.TryGetValue(regId, out res)) 
                return res;
            return null;
        }
        public Dictionary<Pullenti.Address.GarParam, string> GetAOParams(int id)
        {
            if (m_AddrParams == null) 
                return null;
            lock (m_AddrParams.m_Lock) 
            {
                return m_AddrParams.GetParams(id);
            }
        }
        public void PutAOParams(int id, Dictionary<Pullenti.Address.GarParam, string> pars)
        {
            if (m_AddrParams == null) 
                return;
            m_AddrParams.PutParams(id, pars, false);
        }
        public void FlushAllParams()
        {
            m_AddrParams.Flush();
            m_HouseParams.Flush();
            m_RoomParams.Flush();
        }
        public bool PutAO(AreaObject ao, bool onlyAttrs = false)
        {
            if (m_AddrTable == null) 
                return false;
            if (ao.Id == 0) 
                ao.Id = m_AddrTable.GetMaxKey() + 1;
            m_AddrTable.Add(ao.Id, ao, onlyAttrs);
            return true;
        }
        public HousesInStreet GetAOHouses(int id)
        {
            if (m_HousesInAo == null) 
                return null;
            byte[] dat = null;
            lock (m_HousesInAo.m_Lock) 
            {
                dat = m_HousesInAo.ReadKeyData(id, 0);
            }
            if (dat == null) 
                return null;
            try 
            {
                HousesInStreet res = new HousesInStreet();
                res.Load(dat);
                return res;
            }
            catch(Exception ex) 
            {
                return null;
            }
        }
        public void PutAOHouses(int id, byte[] dat)
        {
            m_HousesInAo.WriteKeyData(id, dat);
        }
        public HouseObject GetHouse(int id)
        {
            if (m_HouseTable == null) 
                return null;
            lock (m_HouseTable.m_Lock) 
            {
                HouseObject ao = new HouseObject() { Id = id };
                if (m_HouseTable.Get(id, ao)) 
                    return ao;
                else 
                    return null;
            }
        }
        public bool PutHouse(HouseObject ao)
        {
            if (m_HouseTable == null) 
                return false;
            if (ao.ParentId == 0) 
                return false;
            if (ao.Id == 0) 
                ao.Id = m_HouseTable.GetMaxKey() + 1;
            m_HouseTable.Add(ao.Id, ao);
            return true;
        }
        public Dictionary<Pullenti.Address.GarParam, string> GetHouseParams(int id)
        {
            if (m_HouseParams == null) 
                return null;
            lock (m_HouseParams.m_Lock) 
            {
                return m_HouseParams.GetParams(id);
            }
        }
        public void PutHouseParams(int id, Dictionary<Pullenti.Address.GarParam, string> pars)
        {
            if (m_HouseParams == null) 
                return;
            m_HouseParams.PutParams(id, pars, false);
        }
        public RoomsInHouse GetHouseRooms(int id)
        {
            if (m_RoomsInHouse == null) 
                return null;
            byte[] dat = null;
            lock (m_RoomsInHouse.m_Lock) 
            {
                dat = m_RoomsInHouse.ReadKeyData(id, 0);
            }
            if (dat == null) 
                return null;
            try 
            {
                RoomsInHouse res = new RoomsInHouse();
                res.Load(dat);
                return res;
            }
            catch(Exception ex) 
            {
                return null;
            }
        }
        public void PutHouseRooms(int id, RoomsInHouse rih)
        {
            if (rih != null && rih.Count > 0) 
                m_RoomsInHouse.WriteKeyData(id, rih.Save());
        }
        static bool m_SARegime = false;
        public void PutStringEntries(AreaObject ao, Pullenti.Address.Internal.NameAnalyzer na)
        {
            if ((ao.Id == 0 || na.Status == Pullenti.Address.GarStatus.Error || na.Strings == null) || na.Strings.Count == 0) 
                return;
            if (na.Status != Pullenti.Address.GarStatus.Ok2 && na.Sec != null) 
                return;
            if (ao.Id == 25799) 
            {
            }
            ao.Status = na.Status;
            bool isStreet = na.Level == Pullenti.Address.AddrLevel.Street || na.Level == Pullenti.Address.AddrLevel.Territory;
            foreach (string str in na.Strings) 
            {
                if (ao.Region > 0) 
                    m_AreaTree.Add(string.Format("{0}${1}{2}", str, ao.Region, (!m_SARegime ? "" : (isStreet ? "S" : "A"))), ao, na);
                if (!m_SARegime) 
                    m_AreaTree.Add(str, ao, na);
                else if (isStreet) 
                    m_AreaTree.Add(str + "$S", ao, na);
                else 
                    m_AreaTree.Add(str + "$A", ao, na);
            }
            if (na.StringsEx != null) 
            {
                foreach (string str in na.StringsEx) 
                {
                    if (!m_SARegime) 
                        m_AreaTree.Add(str, ao, na);
                    else if (isStreet) 
                        m_AreaTree.Add(str + "$S", ao, na);
                    else 
                        m_AreaTree.Add(str + "$A", ao, na);
                }
            }
        }
        public void ClearStringEntries()
        {
            m_AreaTree.Children = new Dictionary<char, AreaTreeNode>();
        }
        public List<AreaTreeObject> GetAllStringEntriesByStart(string start, string adj, string number, bool street, int regId)
        {
            List<AreaTreeObject> res = new List<AreaTreeObject>();
            string suff = null;
            if (adj != null && number != null) 
                suff = string.Format("{0}{1}", adj, number);
            else if (adj != null) 
                suff = string.Format("{0}", adj);
            else if (number != null) 
                suff = number;
            AreaTreeNode root = null;
            lock (m_AreaTree.m_Lock) 
            {
                if (suff != null) 
                {
                    root = m_AreaTree.Find(suff + start, false, true, false);
                    if (root == null) 
                        root = m_AreaTree.Find(suff + start, true, false, false);
                    if (root != null) 
                        suff = null;
                }
                if (root == null) 
                    root = m_AreaTree.Find(start, false, true, false);
                if (root == null) 
                    root = m_AreaTree.Find(start, true, false, false);
                List<int> ids = new List<int>();
                if (root != null) 
                    m_AreaTree.GetAllObjIds(root, suff, street, ids);
                ids.Sort();
                for (int i = 0; i < ids.Count; i++) 
                {
                    if (i > 0 && ids[i] == ids[i - 1]) 
                        continue;
                    AreaTreeObject o = m_AreaTree.GetObj(ids[i]);
                    if (o == null) 
                        continue;
                    if (regId != 0 && o.Region != ((byte)regId)) 
                        continue;
                    if (street) 
                    {
                        if (Pullenti.Address.AddressHelper.CompareLevels(o.Level, Pullenti.Address.AddrLevel.Territory) < 0) 
                            continue;
                    }
                    else if (Pullenti.Address.AddressHelper.CompareLevels(o.Level, Pullenti.Address.AddrLevel.Territory) > 0) 
                        continue;
                    res.Add(o);
                }
            }
            return res;
        }
        internal bool CheckName(string name, bool isStreet)
        {
            if (m_AreaTree == null) 
                return false;
            lock (m_AreaTree.m_Lock) 
            {
                if (m_SARegime) 
                    name = string.Format("{0}${1}", name, (isStreet ? 'S' : 'A'));
                AreaTreeNode li = m_AreaTree.Find(name, false, false, false);
                if (li != null && li.ObjIds != null && li.ObjIds.Count > 0) 
                    return true;
            }
            return false;
        }
        internal List<AreaTreeObject> GetStringEntries(Pullenti.Address.Internal.NameAnalyzer na, List<byte> regions, List<int> parIds, int maxCount)
        {
            if (na == null || m_AreaTree == null) 
                return null;
            if (regions == null || regions.Count == 0) 
                return this._getStringEntriesR(na, 0, parIds, maxCount);
            if (regions.Count == 1) 
                return this._getStringEntriesR(na, regions[0], parIds, maxCount);
            List<AreaTreeObject> res = null;
            foreach (byte reg in regions) 
            {
                List<AreaTreeObject> re = this._getStringEntriesR(na, reg, parIds, maxCount);
                if (re == null) 
                    continue;
                if (res == null) 
                    res = re;
                else 
                    res.AddRange(re);
                if (res.Count >= maxCount) 
                    break;
            }
            return res;
        }
        List<AreaTreeObject> _getStringEntriesR(Pullenti.Address.Internal.NameAnalyzer na, byte region, List<int> parIds, int maxCount)
        {
            if (na.Strings == null) 
                return null;
            bool isStreet = na.Level == Pullenti.Address.AddrLevel.Street || na.Level == Pullenti.Address.AddrLevel.Territory;
            List<AreaTreeObject> res = null;
            List<AreaTreeObject> res2 = null;
            lock (m_AreaTree.m_Lock) 
            {
                for (int k = 0; k < 3; k++) 
                {
                    List<string> strs = (k == 2 ? na.DoubtStrings : na.Strings);
                    foreach (string s in strs) 
                    {
                        AreaTreeNode li;
                        string ss = s;
                        if (!m_SARegime) 
                        {
                            if (region != 0) 
                                ss = string.Format("{0}${1}", s, region);
                        }
                        else if (region != 0) 
                            ss = string.Format("{0}${1}{2}", s, region, (isStreet ? "S" : "A"));
                        else if (isStreet) 
                            ss = s + "$S";
                        else 
                            ss = s + "$A";
                        li = m_AreaTree.Find(ss, k == 1, false, k == 1);
                        for (int pp = 0; pp < 2; pp++) 
                        {
                            if (li != null && li.ObjIds != null) 
                            {
                                foreach (int oid in li.ObjIds) 
                                {
                                    if (li.ObjIds.Count > 1000 && region == 0) 
                                        return null;
                                    AreaTreeObject o = m_AreaTree.GetObj(oid);
                                    if (o == null) 
                                        continue;
                                    if (oid == 825641) 
                                    {
                                    }
                                    if (region != 0 && o.Region != region && o.Region != 0) 
                                        continue;
                                    bool ok = false;
                                    bool ok2 = false;
                                    if (parIds == null || parIds.Count == 0) 
                                        ok = true;
                                    else if (parIds.Contains(o.Id)) 
                                    {
                                    }
                                    else 
                                        foreach (int id in parIds) 
                                        {
                                            if (o.ParentIds.Contains(id)) 
                                            {
                                                ok = true;
                                                break;
                                            }
                                            else if (o.ParentParentIds != null && o.ParentParentIds.Contains(id)) 
                                                ok2 = true;
                                        }
                                    if (!ok && !ok2) 
                                        continue;
                                    if (o.Id == 1944641) 
                                    {
                                    }
                                    int co = o.CheckType(na);
                                    if (co >= 0) 
                                    {
                                        if (!ok) 
                                        {
                                            if (res2 == null) 
                                                res2 = new List<AreaTreeObject>();
                                            bool exi = false;
                                            foreach (AreaTreeObject oo in res2) 
                                            {
                                                if (oo.Id == o.Id) 
                                                {
                                                    exi = true;
                                                    break;
                                                }
                                            }
                                            if (!exi) 
                                                res2.Add(o);
                                        }
                                        else 
                                        {
                                            if (res == null) 
                                                res = new List<AreaTreeObject>();
                                            res.Add(o);
                                            if (res.Count >= maxCount) 
                                                return res;
                                        }
                                    }
                                    else if (na.Level == Pullenti.Address.AddrLevel.Street && o.Level == Pullenti.Address.AddrLevel.Street) 
                                    {
                                        ok2 = false;
                                        if (na.Types.Count == 1 && na.Types[0] == "улица") 
                                        {
                                            if (o.Typs.Contains("блок") || o.Typs.Contains("ряд") || o.Typs.Contains("линия")) 
                                            {
                                            }
                                            else 
                                                ok2 = true;
                                        }
                                        else if (o.Typs.Count == 1 && o.Typs[0] == "улица") 
                                        {
                                            if (na.Types.Contains("блок") || na.Types.Contains("ряд") || na.Types.Contains("линия")) 
                                            {
                                            }
                                            else 
                                                ok2 = true;
                                        }
                                        else if (na.Types.Count == 1 && o.Typs.Count == 1) 
                                        {
                                            if (na.Types[0] == "проезд" || na.Types[0] == "переулок") 
                                            {
                                                if (o.Typs[0] == "проезд" || o.Typs[0] == "переулок") 
                                                    ok2 = true;
                                            }
                                        }
                                        if (ok2) 
                                        {
                                            if (res2 == null) 
                                                res2 = new List<AreaTreeObject>();
                                            bool exi = false;
                                            foreach (AreaTreeObject oo in res2) 
                                            {
                                                if (oo.Id == o.Id) 
                                                {
                                                    exi = true;
                                                    break;
                                                }
                                            }
                                            if (!exi) 
                                                res2.Add(o);
                                        }
                                    }
                                }
                            }
                            if (!m_SARegime) 
                                break;
                            if ((k == 0 && pp == 0 && region != 0) && !isStreet && na.Level == Pullenti.Address.AddrLevel.Locality) 
                            {
                                li = m_AreaTree.Find(string.Format("{0}${1}S", s, region), k > 0, false, false);
                                if (li == null || li.ObjIds == null) 
                                    break;
                            }
                            else 
                                break;
                        }
                        if (res != null) 
                            return res;
                        if (res2 != null && k == 0) 
                            return res2;
                    }
                }
            }
            return res ?? res2;
        }
        public bool PutRoom(RoomObject ro)
        {
            if (ro.HouseId == 0) 
                return false;
            m_RoomTable.Add(ro.Id, ro);
            if (((ro.HouseId & 0x80000000)) != 0) 
                return true;
            return true;
        }
        public RoomObject GetRoom(int id)
        {
            if (m_RoomTable == null) 
                return null;
            lock (m_RoomTable.m_Lock) 
            {
                return m_RoomTable.Get(id);
            }
        }
        public bool ExistsRoom(int id)
        {
            if (m_RoomTable == null) 
                return false;
            return m_RoomTable.ReadKeyDataLen(id) > 0;
        }
        public Dictionary<Pullenti.Address.GarParam, string> GetRoomParams(int id)
        {
            if (m_RoomParams == null) 
                return null;
            lock (m_RoomParams.m_Lock) 
            {
                return m_RoomParams.GetParams(id);
            }
        }
        public void PutRoomParams(int id, Dictionary<Pullenti.Address.GarParam, string> pars)
        {
            if (m_RoomParams == null) 
                return;
            m_RoomParams.PutParams(id, pars, false);
        }
        public RoomsInHouse GetRoomRooms(int id)
        {
            if (m_RoomsInRooms == null) 
                return null;
            byte[] dat = null;
            lock (m_RoomsInRooms.m_Lock) 
            {
                dat = m_RoomsInRooms.ReadKeyData(id, 0);
            }
            if (dat == null) 
                return null;
            try 
            {
                RoomsInHouse res = new RoomsInHouse();
                res.Load(dat);
                return res;
            }
            catch(Exception ex) 
            {
                return null;
            }
        }
        public void PutRoomsRooms(int id, RoomsInHouse rih)
        {
            if (rih != null && rih.Count > 0) 
                m_RoomsInRooms.WriteKeyData(id, rih.Save());
        }
        public RoomsInHouse GetRoomsInHouse(int houseId)
        {
            if (houseId == 0 || m_RoomsInHouse == null) 
                return null;
            byte[] dat = null;
            lock (m_RoomsInHouse.m_Lock) 
            {
                dat = m_RoomsInHouse.ReadKeyData(houseId, 0);
            }
            if (dat == null) 
                return null;
            RoomsInHouse res = new RoomsInHouse();
            res.Load(dat);
            return res;
        }
        public RoomsInHouse GetRoomsInRooms(int roomId)
        {
            if (roomId == 0 || m_RoomsInRooms == null) 
                return null;
            byte[] dat = null;
            lock (m_RoomsInRooms.m_Lock) 
            {
                dat = m_RoomsInRooms.ReadKeyData(roomId, 0);
            }
            if (dat == null) 
                return null;
            RoomsInHouse res = new RoomsInHouse();
            res.Load(dat);
            return res;
        }
    }
}