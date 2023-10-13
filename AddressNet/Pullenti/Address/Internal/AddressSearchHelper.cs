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
    public static class AddressSearchHelper
    {
        public static Pullenti.Address.SearchResult Search(Pullenti.Address.SearchParams sp)
        {
            Pullenti.Address.SearchResult res = new Pullenti.Address.SearchResult() { Params = sp };
            if (sp.ParamTyp != Pullenti.Address.GarParam.Undefined && !string.IsNullOrEmpty(sp.ParamValue)) 
            {
                if (GarHelper.GarIndex == null) 
                    return null;
                List<uint> ids = GarHelper.GarIndex.FindByParam(sp.ParamTyp, sp.ParamValue);
                if (ids == null) 
                    return res;
                res.TotalCount = ids.Count;
                for (int i = 0; i < ids.Count; i++) 
                {
                    if (res.Objects.Count >= sp.MaxCount) 
                        break;
                    uint id = ids[i];
                    if (((id & 0x80000000)) == 0) 
                    {
                        Pullenti.Address.GarObject aa = GarHelper.CreateGarAById((int)id);
                        if (aa != null) 
                            res.Objects.Add(aa);
                        continue;
                    }
                    if (((id & 0x40000000)) == 0) 
                    {
                        Pullenti.Address.Internal.Gar.HouseObject ho = GarHelper.GarIndex.GetHouse((int)((id & 0x3FFFFFFF)));
                        Pullenti.Address.GarObject gh = GarHelper.CreateGarHouse(ho);
                        if (gh != null) 
                            res.Objects.Add(gh);
                    }
                    else 
                    {
                        Pullenti.Address.Internal.Gar.RoomObject ro = GarHelper.GarIndex.GetRoom((int)((id & 0x3FFFFFFF)));
                        Pullenti.Address.GarObject rh = GarHelper.CreateGarRoom(ro);
                        if (rh != null) 
                            res.Objects.Add(rh);
                    }
                }
                return res;
            }
            List<SearchAddressItem> ain = new List<SearchAddressItem>();
            if (sp.Region > 0) 
                ain.Add(new SearchAddressItem() { Level = SearchLevel.Region, Id = sp.Region.ToString() });
            if (!string.IsNullOrEmpty(sp.Area)) 
                ain.Add(new SearchAddressItem() { Level = SearchLevel.District, Text = sp.Area });
            if (!string.IsNullOrEmpty(sp.City)) 
                ain.Add(new SearchAddressItem() { Level = SearchLevel.City, Text = sp.City });
            if (!string.IsNullOrEmpty(sp.Street)) 
                ain.Add(new SearchAddressItem() { Level = SearchLevel.Street, Text = sp.Street });
            if (ain.Count > 0) 
                ain[ain.Count - 1].Search = true;
            int total = 0;
            List<SearchAddressItem> sain = Process(ain, sp.MaxCount, out total);
            res.TotalCount = total;
            if (sain != null) 
            {
                foreach (SearchAddressItem a in sain) 
                {
                    if (a.Tag is Pullenti.Address.Internal.Gar.AreaObject) 
                    {
                        Pullenti.Address.GarObject ga = GarHelper.CreateGarArea(a.Tag as Pullenti.Address.Internal.Gar.AreaObject);
                        if (ga != null) 
                            res.Objects.Add(ga);
                    }
                }
            }
            return res;
        }
        public static Dictionary<string, SearchAddressItem> m_OntoRegs = new Dictionary<string, SearchAddressItem>();
        public static List<SearchAddressItem> Process(List<SearchAddressItem> ain, int maxCount, out int total)
        {
            total = 0;
            if (ain == null || ain.Count == 0) 
                return null;
            List<SearchAddressItem> ain1 = new List<SearchAddressItem>();
            int regId = 0;
            foreach (SearchAddressItem a in ain) 
            {
                if (a.Level == SearchLevel.Region) 
                {
                    if (m_OntoRegs != null) 
                    {
                        foreach (SearchAddressItem it in m_OntoRegs.Values) 
                        {
                            if (it.Text == a.Text) 
                            {
                                int nn;
                                if (int.TryParse(it.Id ?? "", out nn)) 
                                {
                                    regId = nn;
                                    a.Id = it.Id;
                                    a.Text = null;
                                }
                                break;
                            }
                        }
                    }
                    if (regId == 0) 
                    {
                        int nn;
                        if (int.TryParse(a.Id ?? "", out nn)) 
                            regId = nn;
                    }
                }
                else 
                    ain1.Add(a);
            }
            if (regId == 0 && ain1.Count == 0) 
                return null;
            return _process(ain1, regId, maxCount, out total);
        }
        static SearchLevel _calcSearchLevel(Pullenti.Address.Internal.Gar.AreaObject ao)
        {
            Pullenti.Address.GarLevel lev = (Pullenti.Address.GarLevel)ao.Level;
            if (lev == Pullenti.Address.GarLevel.Region) 
                return SearchLevel.Region;
            if (lev == Pullenti.Address.GarLevel.AdminArea || lev == Pullenti.Address.GarLevel.MunicipalArea) 
                return SearchLevel.District;
            if (lev == Pullenti.Address.GarLevel.Settlement || lev == Pullenti.Address.GarLevel.City) 
                return SearchLevel.City;
            if (lev == Pullenti.Address.GarLevel.Locality) 
            {
                if (ao.Typ != null && ao.Typ.Name == "территория") 
                    return SearchLevel.Street;
                return SearchLevel.City;
            }
            if (lev == Pullenti.Address.GarLevel.Area || lev == Pullenti.Address.GarLevel.Street) 
                return SearchLevel.Street;
            return SearchLevel.Undefined;
        }
        static string _getId(int id)
        {
            return string.Format("a{0}", id);
        }
        static List<SearchAddressItem> _process(List<SearchAddressItem> ain, int regId, int maxCount, out int total)
        {
            total = 0;
            AddrSearchFormal mai = null;
            foreach (SearchAddressItem a in ain) 
            {
                if (a.Search) 
                {
                    mai = new AddrSearchFormal(a);
                    mai.RegId = regId;
                    break;
                }
            }
            if (GarHelper.GarIndex == null) 
                return null;
            if (mai == null) 
            {
                if (regId == 0) 
                    return null;
                Pullenti.Address.Internal.Gar.AreaObject ao = GarHelper.GarIndex.GetAOByReg(regId);
                if (ao == null) 
                    return null;
                SearchAddressItem rr = new SearchAddressItem() { Id = _getId(ao.Id), Level = SearchLevel.Region, Tag = ao, Text = string.Format("{0} {1}", ao.Names[0], ao.Typ.Name) };
                if (m_OntoRegs.ContainsKey(rr.Id)) 
                {
                    SearchAddressItem reg = m_OntoRegs[rr.Id];
                    rr.Text = reg.Text;
                    rr.Id = reg.Id ?? reg.Text;
                }
                List<SearchAddressItem> res0 = new List<SearchAddressItem>();
                res0.Add(rr);
                return res0;
            }
            if (string.IsNullOrEmpty(mai.Src.Text) && mai.Src.Level != SearchLevel.Region && ain.Count > 0) 
            {
                List<SearchAddressItem> ain0 = new List<SearchAddressItem>();
                SearchAddressItem aiMax = null;
                foreach (SearchAddressItem a in ain) 
                {
                    if ((a.Level < mai.Src.Level) && !string.IsNullOrEmpty(a.Text)) 
                    {
                        SearchAddressItem aa = new SearchAddressItem() { Level = a.Level, Text = a.Text };
                        ain0.Add(aa);
                        if (aiMax == null) 
                            aiMax = aa;
                        else if (aa.Level > aiMax.Level) 
                            aiMax = aa;
                    }
                }
                if (aiMax != null) 
                    aiMax.Search = true;
                List<SearchAddressItem> res0 = _process(ain0, regId, maxCount, out total);
                if (res0 == null || res0.Count != 1) 
                    return null;
                total = 0;
                Pullenti.Address.Internal.Gar.AreaObject ao = res0[0].Tag as Pullenti.Address.Internal.Gar.AreaObject;
                if (ao == null) 
                    return null;
                List<Pullenti.Address.Internal.Gar.AreaObject> all0 = GarHelper.GarIndex.GetAOChildren(ao);
                List<SearchAddressItem> res00 = new List<SearchAddressItem>();
                Dictionary<int, bool> ggg0 = new Dictionary<int, bool>();
                if (all0 != null) 
                {
                    foreach (Pullenti.Address.Internal.Gar.AreaObject ao0 in all0) 
                    {
                        if (res00.Count >= maxCount) 
                        {
                            total = all0.Count;
                            break;
                        }
                        SearchLevel slev = _calcSearchLevel(ao0);
                        SearchAddressItem ai0 = new SearchAddressItem() { Id = _getId(ao0.Id), Tag = ao0, Level = slev, Parent = res0[0], Text = string.Format("{0} {1}", ao0.Names[0], ao0.Typ.Name) };
                        if (slev == mai.Src.Level) 
                        {
                            if (ggg0.ContainsKey(ao0.Id)) 
                                continue;
                            res00.Add(ai0);
                            ggg0.Add(ao0.Id, true);
                            continue;
                        }
                        if (((int)slev) > ((int)mai.Src.Level)) 
                            continue;
                        List<Pullenti.Address.Internal.Gar.AreaObject> all1 = GarHelper.GarIndex.GetAOChildren(ao0);
                        if (all1 != null) 
                        {
                            foreach (Pullenti.Address.Internal.Gar.AreaObject ao1 in all1) 
                            {
                                if (res00.Count >= maxCount) 
                                {
                                    total = res00.Count + all1.Count;
                                    break;
                                }
                                slev = _calcSearchLevel(ao1);
                                if (slev == mai.Src.Level) 
                                {
                                    if (ggg0.ContainsKey(ao1.Id)) 
                                        continue;
                                    SearchAddressItem sai1 = new SearchAddressItem() { Id = _getId(ao1.Id), Tag = ao1, Level = slev, Parent = ai0, Text = string.Format("{0} {1}", ao1.Names[0], ao1.Typ.Name) };
                                    res00.Add(sai1);
                                    ggg0.Add(ao1.Id, true);
                                    continue;
                                }
                            }
                        }
                    }
                }
                return res00;
            }
            List<SearchAddressItem> res = new List<SearchAddressItem>();
            mai.RegId = regId;
            List<Pullenti.Address.Internal.Gar.AreaTreeObject> all = mai.Search();
            if (all == null || all.Count == 0) 
                return res;
            foreach (SearchAddressItem a in ain) 
            {
                if (!a.Search && (a.Level < mai.Src.Level)) 
                {
                    AddrSearchFormal par = new AddrSearchFormal(a);
                    List<Pullenti.Address.Internal.Gar.AreaTreeObject> pars = par.Search();
                    if (pars.Count == 0) 
                        continue;
                    for (int i = all.Count - 1; i >= 0; i--) 
                    {
                        bool hasPar = false;
                        foreach (Pullenti.Address.Internal.Gar.AreaTreeObject p in pars) 
                        {
                            if (all[i].ParentIds.Contains(p.Id)) 
                            {
                                hasPar = true;
                                break;
                            }
                            else if (all[i].ParentParentIds != null && all[i].ParentParentIds.Contains(p.Id)) 
                            {
                                hasPar = true;
                                break;
                            }
                        }
                        if (!hasPar) 
                            all.RemoveAt(i);
                    }
                }
            }
            Dictionary<int, bool> ggg = new Dictionary<int, bool>();
            for (int k = 0; k < 2; k++) 
            {
                foreach (Pullenti.Address.Internal.Gar.AreaTreeObject a in all) 
                {
                    if (res.Count >= maxCount) 
                    {
                        total = all.Count;
                        break;
                    }
                    Pullenti.Address.Internal.Gar.AreaObject ao = GarHelper.GarIndex.GetAO(a.Id);
                    if (ao == null) 
                        continue;
                    if (ggg.ContainsKey(ao.Id)) 
                        continue;
                    if (!mai.Check(ao, k > 0)) 
                        continue;
                    SearchLevel slev = _calcSearchLevel(ao);
                    if (slev != mai.Src.Level) 
                    {
                        if (slev == SearchLevel.Region && mai.Src.Level == SearchLevel.City && ao.Level == 1) 
                        {
                        }
                        else 
                            continue;
                    }
                    SearchAddressItem ai = new SearchAddressItem() { Id = _getId(ao.Id), Tag = ao, Level = slev, Text = string.Format("{0} {1}", ao.Names[0], ao.Typ.Name) };
                    res.Add(ai);
                    total = res.Count;
                    ggg.Add(ao.Id, true);
                    List<int> parids = a.ParentIds;
                    while (parids != null && parids.Count > 0) 
                    {
                        bool ok = false;
                        foreach (int pid in parids) 
                        {
                            Pullenti.Address.Internal.Gar.AreaObject pao = GarHelper.GarIndex.GetAO(pid);
                            if (pao == null) 
                                continue;
                            SearchLevel slev0 = _calcSearchLevel(pao);
                            if (slev0 == SearchLevel.Undefined) 
                                continue;
                            if (slev0 == slev) 
                                continue;
                            SearchAddressItem pai = new SearchAddressItem() { Id = _getId(pao.Id), Tag = pao, Level = slev0, Text = string.Format("{0} {1}", pao.Names[0], pao.Typ.Name) };
                            ai.Parent = pai;
                            ai = pai;
                            slev = slev0;
                            parids = pao.ParentIds;
                            ok = true;
                            break;
                        }
                        if (slev == SearchLevel.Region || !ok) 
                            break;
                    }
                    if (ai.Level == SearchLevel.Region && m_OntoRegs.ContainsKey(ai.Id)) 
                    {
                        SearchAddressItem reg = m_OntoRegs[ai.Id];
                        ai.Text = reg.Text;
                        ai.Id = reg.Id ?? reg.Text;
                    }
                }
                if (res.Count > 0) 
                    break;
            }
            res.Sort();
            return res;
        }
    }
}