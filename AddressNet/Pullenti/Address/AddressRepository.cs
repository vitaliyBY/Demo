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

namespace Pullenti.Address
{
    /// <summary>
    /// Репозиторий адресов (Адрессарий)
    /// </summary>
    public class AddressRepository : IDisposable
    {
        /// <summary>
        /// Открыть репозиторий
        /// </summary>
        /// <param name="pathName">папка, если папки не существует или пуста, то репозиторий будет в ней создан</param>
        public void Open(string pathName)
        {
            m_BaseDir = Path.GetFullPath(pathName);
            if (!Directory.Exists(m_BaseDir)) 
                Directory.CreateDirectory(m_BaseDir);
            m_Typs = new Pullenti.Address.Internal.RepTypTable(pathName);
            m_Typs.Open(false, 0);
            m_Objs = new Pullenti.Address.Internal.RepObjTable(m_Typs, pathName);
            m_Objs.Open(false, 0);
            m_Chis = new Pullenti.Address.Internal.RepChildrenTable(pathName);
            m_Chis.Open(false, 0);
            m_Root = m_Objs.Get(1);
            if (m_Root == null) 
            {
                m_Root = new RepaddrObject();
                m_Root.Spelling = "Root";
                m_Objs.Add(1, m_Root);
                m_Objs.Flush();
            }
            else 
                m_Root.Children = m_Chis.Get(1);
            m_ATree = new Pullenti.Address.Internal.RepAddrTree();
            string nam = Path.Combine(m_BaseDir, "atree.dat");
            if (File.Exists(nam)) 
                m_ATree.Open(File.ReadAllBytes(nam));
            m_CObjs = new Pullenti.Util.Repository.KeyBaseTable(null, "cobjs", BaseDir);
            m_CObjs.Open(false, 0);
            m_Modified = false;
        }
        /// <summary>
        /// Директория, в которой расположен репозиторий адресов
        /// </summary>
        public string BaseDir
        {
            get
            {
                return m_BaseDir;
            }
        }
        string m_BaseDir;
        Pullenti.Address.Internal.RepTypTable m_Typs;
        Pullenti.Address.Internal.RepObjTable m_Objs;
        Pullenti.Address.Internal.RepChildrenTable m_Chis;
        Pullenti.Address.Internal.RepAddrTree m_ATree;
        Pullenti.Util.Repository.KeyBaseTable m_CObjs;
        Dictionary<int, Pullenti.Address.Internal.RepObjTree> m_OTrees = new Dictionary<int, Pullenti.Address.Internal.RepObjTree>();
        List<int> m_OTreeIds = new List<int>();
        bool m_Modified = false;
        RepaddrObject m_Root;
        /// <summary>
        /// Сохранить изменения (вызывать периодически при добавлении больших объёмов, 
        /// а также в конце загрузки)
        /// </summary>
        public void Commit()
        {
            if (!m_Modified) 
                return;
            if (m_ATree == null) 
                return;
            string nam = Path.Combine(m_BaseDir, "atree.dat");
            using (FileStream f = new FileStream(nam, FileMode.Create, FileAccess.Write)) 
            {
                m_ATree.Save(f);
            }
            m_ATree.Clear();
            byte[] dat = File.ReadAllBytes(nam);
            m_ATree.Open(dat);
            foreach (KeyValuePair<int, Pullenti.Address.Internal.RepObjTree> ot in m_OTrees) 
            {
                if (ot.Value.Modified) 
                {
                    byte[] dat1 = null;
                    using (MemoryStream mem = new MemoryStream()) 
                    {
                        ot.Value.Save(mem);
                        dat1 = mem.ToArray();
                    }
                    m_CObjs.WriteKeyData(ot.Key, dat1);
                }
            }
            m_OTrees.Clear();
            m_OTreeIds.Clear();
            m_Modified = false;
        }
        /// <summary>
        /// Вызывать в конце длительной загрузки - это займёт некоторое время, 
        /// зато уменьшит размер индекса для оптимизации доступа и поиска.
        /// </summary>
        public void Optimize()
        {
            if (m_Modified) 
                this.Commit();
            if (m_Objs != null) 
                m_Objs.Optimize(10);
            if (m_Chis != null) 
                m_Chis.Optimize(10);
            if (m_CObjs != null) 
                m_CObjs.Optimize(10);
        }
        /// <summary>
        /// Завершить работу с репозиторием (крайне желательно вызывать в конце)
        /// </summary>
        public void Dispose()
        {
            if (m_Modified) 
                this.Commit();
            if (m_ATree != null) 
            {
                m_ATree.Clear();
                m_ATree = null;
            }
            foreach (KeyValuePair<int, Pullenti.Address.Internal.RepObjTree> ot in m_OTrees) 
            {
                ot.Value.Clear();
            }
            m_OTrees.Clear();
            m_OTreeIds.Clear();
            if (m_Chis != null) 
            {
                m_Chis.Dispose();
                m_Chis = null;
            }
            if (m_CObjs != null) 
            {
                m_CObjs.Dispose();
                m_CObjs = null;
            }
            if (m_Objs != null) 
            {
                m_Objs.Dispose();
                m_Objs = null;
            }
            if (m_Typs != null) 
            {
                m_Typs.Dispose();
                m_Typs = null;
            }
        }
        Pullenti.Address.Internal.RepObjTree _getTree(int id)
        {
            Pullenti.Address.Internal.RepObjTree res;
            if (m_OTrees.TryGetValue(id, out res)) 
            {
                if (m_OTreeIds[0] != id) 
                {
                    m_OTreeIds.Remove(id);
                    m_OTreeIds.Insert(0, id);
                }
                return res;
            }
            if (m_OTrees.Count >= 100) 
            {
                int id1 = m_OTreeIds[m_OTreeIds.Count - 1];
                if (m_OTrees[id1].Modified) 
                {
                    byte[] dat1 = null;
                    using (MemoryStream mem = new MemoryStream()) 
                    {
                        m_OTrees[id1].Save(mem);
                        dat1 = mem.ToArray();
                    }
                    m_CObjs.WriteKeyData(id1, dat1);
                }
                m_OTrees.Remove(id1);
                m_OTreeIds.RemoveAt(m_OTreeIds.Count - 1);
            }
            res = new Pullenti.Address.Internal.RepObjTree();
            byte[] dat = m_CObjs.ReadKeyData(id, 0);
            if (dat != null) 
                res.Open(dat);
            m_OTrees.Add(id, res);
            m_OTreeIds.Insert(0, id);
            return res;
        }
        /// <summary>
        /// Максимальный идентификатор (равен общему количеству элементов)
        /// </summary>
        public int GetMaxId()
        {
            if (m_Objs == null) 
                return 0;
            return m_Objs.GetMaxKey();
        }
        /// <summary>
        /// Получить объект по его идентификатору
        /// </summary>
        /// <param name="id">идентификатор</param>
        /// <return>объект или null</return>
        public RepaddrObject GetObject(int id)
        {
            if (m_Objs == null) 
                return null;
            RepaddrObject res = m_Objs.Get(id);
            if (res == null) 
                return null;
            res.Children = m_Chis.Get(id);
            return res;
        }
        /// <summary>
        /// Получить экземпляры дочерних объектов объекта
        /// </summary>
        /// <param name="ro">родительский объект (если null, то вернёт объекты первого уровня)</param>
        /// <return>список дочерних объектов RepaddrObject</return>
        public List<RepaddrObject> GetObjects(RepaddrObject ro)
        {
            if (ro == null) 
                return null;
            List<RepaddrObject> res = new List<RepaddrObject>();
            if (ro == null) 
            {
                if (m_Root == null) 
                    return null;
                if (m_Root.Children != null) 
                {
                    foreach (int id in m_Root.Children) 
                    {
                        RepaddrObject o = this.GetObject(id);
                        if (o != null) 
                            res.Add(o);
                    }
                }
            }
            else if (ro.Children != null) 
            {
                foreach (int id in ro.Children) 
                {
                    RepaddrObject o = this.GetObject(id);
                    if (o != null) 
                        res.Add(o);
                }
            }
            _sort(res);
            return res;
        }
        static void _sort(List<RepaddrObject> res)
        {
            res.Sort();
        }
        /// <summary>
        /// Добавить адрес (всю иерархию) в репозиторий. У элементов будут 
        /// устанавливаться поля RepObject с информацией о сохранении.
        /// </summary>
        /// <param name="addr">адресный элемент нижнего уровня</param>
        /// <return>Количество новых добавленных объектов</return>
        public int Add(TextAddress addr)
        {
            if (m_ATree == null) 
                return 0;
            return this._search(addr, true);
        }
        /// <summary>
        /// Без добавления попытаться привязать существующие элементы 
        /// (для кого удалось - устанавливается поле RepObject)
        /// </summary>
        /// <param name="addr">адресный элемент нижнего уровня</param>
        public void Search(TextAddress addr)
        {
            if (m_ATree == null) 
                return;
            this._search(addr, false);
        }
        int _search(TextAddress addr, bool add)
        {
            List<AddrObject> path = new List<AddrObject>();
            AddrObject plot = null;
            AddrObject house = null;
            AddrObject room = null;
            int ret = 0;
            foreach (AddrObject a in addr.Items) 
            {
                if (a.Attrs is RoomAttributes) 
                    room = a;
                else if (a.Attrs is HouseAttributes) 
                {
                    if (a.Level == AddrLevel.Plot) 
                        plot = a;
                    else 
                        house = a;
                }
                else if (a.Attrs is AreaAttributes) 
                    path.Add(a);
            }
            if (path.Count == 0) 
                return ret;
            if ((path[0].Level == AddrLevel.Country || path[0].Level == AddrLevel.RegionCity || path[0].Level == AddrLevel.RegionArea) || path[0].Level == AddrLevel.City) 
            {
            }
            else 
                return -1;
            List<RepaddrObject> opath = new List<RepaddrObject>();
            bool modif = false;
            for (int i = 0; i < path.Count; i++) 
            {
                AddrObject cur = path[i];
                Pullenti.Address.Internal.RepaddrSearchObj so = new Pullenti.Address.Internal.RepaddrSearchObj(cur, m_Typs);
                int coef = 100;
                Pullenti.Address.Internal.RepAddrTreeNodeObj best = null;
                foreach (string str in so.SearchStrs) 
                {
                    List<Pullenti.Address.Internal.RepAddrTreeNodeObj> objs = m_ATree.Find(str);
                    if (objs == null || objs.Count == 0) 
                        continue;
                    foreach (Pullenti.Address.Internal.RepAddrTreeNodeObj o in objs) 
                    {
                        int co = so.CalcCoef(o, (opath.Count > 0 ? opath[0] : null), (opath.Count > 1 ? opath[1] : null));
                        if (co < coef) 
                        {
                            coef = co;
                            best = o;
                        }
                    }
                    if (coef == 0) 
                        break;
                }
                if (best == null) 
                {
                    if (!add) 
                        return ret;
                    RepaddrObject newObj = new RepaddrObject();
                    newObj.Spelling = cur.ToString();
                    newObj.Level = cur.Level;
                    newObj.Types.AddRange((cur.Attrs as AreaAttributes).Types);
                    if (opath.Count > 0) 
                    {
                        if (!AddressHelper.CanBeParent(newObj.Level, opath[0].Level)) 
                            continue;
                        newObj.Parents = new List<int>();
                        newObj.Parents.Add(opath[0].Id);
                    }
                    newObj.Id = m_Objs.GetMaxKey() + 1;
                    if (cur.Gars.Count > 0) 
                    {
                        newObj.GarGuids = new List<string>();
                        foreach (GarObject g in cur.Gars) 
                        {
                            newObj.GarGuids.Add(g.Guid);
                        }
                    }
                    m_Objs.Add(newObj.Id, newObj);
                    cur.RepObject = newObj;
                    best = new Pullenti.Address.Internal.RepAddrTreeNodeObj();
                    best.Id = newObj.Id;
                    best.Lev = so.Lev;
                    best.TypIds.AddRange(so.TypeIds);
                    best.Parents = newObj.Parents;
                    foreach (string str in so.SearchStrs) 
                    {
                        m_ATree.Add(str, best);
                    }
                    modif = true;
                    ret++;
                }
                else 
                {
                    cur.RepObject = this.GetObject(best.Id);
                    if (cur.RepObject == null) 
                        continue;
                    if (add) 
                    {
                        if (best.Correct(cur.RepObject, m_Typs, (opath.Count > 0 ? opath[0] : null))) 
                        {
                            m_Objs.Add(best.Id, cur.RepObject);
                            modif = true;
                        }
                        foreach (string str in so.SearchStrs) 
                        {
                            if (m_ATree.Add(str, best)) 
                                modif = true;
                        }
                    }
                }
                if (cur.RepObject != null) 
                    opath.Insert(0, cur.RepObject);
            }
            if (opath.Count == 0) 
                return ret;
            for (int kk = 0; kk < 3; kk++) 
            {
                int pid = opath[0].Id;
                AddrObject tobj = null;
                if (kk == 0 && plot != null) 
                    tobj = plot;
                else if (kk == 1 && house != null) 
                    tobj = house;
                else if (kk == 2 && room != null) 
                    tobj = room;
                if (tobj == null) 
                    continue;
                List<string> strs = Pullenti.Address.Internal.RepaddrSearchObj.GetSearchStrings(tobj);
                Pullenti.Address.Internal.RepObjTree tree = this._getTree(pid);
                if (tree == null) 
                    break;
                int id = 0;
                foreach (string s in strs) 
                {
                    id = tree.Find(s);
                    if (id > 0) 
                        break;
                }
                if (id == 0) 
                {
                    if (!add) 
                        return ret;
                    RepaddrObject newObj = new RepaddrObject();
                    newObj.Spelling = tobj.ToString();
                    newObj.Level = (kk == 0 ? AddrLevel.Plot : (kk == 1 ? AddrLevel.Building : AddrLevel.Apartment));
                    if (!AddressHelper.CanBeParent(newObj.Level, opath[0].Level)) 
                        continue;
                    newObj.Parents = new List<int>();
                    newObj.Parents.Add(pid);
                    id = m_Objs.GetMaxKey() + 1;
                    newObj.Id = id;
                    if (tobj.Gars.Count > 0) 
                    {
                        newObj.GarGuids = new List<string>();
                        foreach (GarObject g in tobj.Gars) 
                        {
                            newObj.GarGuids.Add(g.Guid);
                        }
                    }
                    m_Objs.Add(newObj.Id, newObj);
                    tobj.RepObject = newObj;
                    ret++;
                    modif = true;
                }
                else 
                {
                    tobj.RepObject = this.GetObject(id);
                    if (tobj.RepObject == null) 
                        break;
                }
                if (add) 
                {
                    foreach (string s in strs) 
                    {
                        if (tree.Add(s, id)) 
                            modif = true;
                    }
                }
                opath.Insert(0, tobj.RepObject);
            }
            if (modif) 
            {
                m_Modified = true;
                m_Objs.Flush();
                bool corrChi = false;
                if (m_Root.Children == null) 
                    m_Root.Children = new List<int>();
                if (!m_Root.Children.Contains(opath[opath.Count - 1].Id)) 
                {
                    m_Root.Children.Add(opath[opath.Count - 1].Id);
                    m_Chis.Add(1, m_Root.Children);
                    corrChi = true;
                }
                for (int i = 0; i < (opath.Count - 1); i++) 
                {
                    if (opath[i + 1].Children == null) 
                        opath[i + 1].Children = new List<int>();
                    if (!AddressHelper.CanBeParent(opath[i].Level, opath[i + 1].Level)) 
                        continue;
                    if (!opath[i + 1].Children.Contains(opath[i].Id)) 
                    {
                        opath[i + 1].Children.Add(opath[i].Id);
                        m_Chis.Add(opath[i + 1].Id, opath[i + 1].Children);
                        corrChi = true;
                    }
                }
                if (corrChi) 
                    m_Chis.Flush();
            }
            return ret;
        }
    }
}