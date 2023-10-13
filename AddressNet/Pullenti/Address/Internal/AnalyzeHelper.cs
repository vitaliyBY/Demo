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
    class AnalyzeHelper
    {
        bool _removeGars(Pullenti.Address.TextAddress addr)
        {
            bool ret = false;
            for (int j = addr.Items.Count - 1; j > 0; j--) 
            {
                Pullenti.Address.AddrObject it1 = addr.Items[j];
                if (it1.Gars.Count < 2) 
                    continue;
                for (int k = j - 1; k >= 0; k--) 
                {
                    Pullenti.Address.AddrObject it0 = addr.Items[k];
                    if (it0.Gars.Count == 0) 
                        continue;
                    int cou = 0;
                    Pullenti.Address.GarObject real = null;
                    bool isActual = false;
                    foreach (Pullenti.Address.GarObject g in it1.Gars) 
                    {
                        if (it0.FindGarByIds(g.ParentIds) != null) 
                        {
                            cou++;
                            real = g;
                        }
                        else if (!g.Expired) 
                            isActual = true;
                    }
                    if (cou == 1) 
                    {
                        if (isActual && real.Expired) 
                            break;
                        else 
                        {
                            it1.Gars.Clear();
                            it1.Gars.Add(real);
                            ret = true;
                        }
                    }
                }
            }
            for (int j = addr.Items.Count - 1; j >= 0; j--) 
            {
                Pullenti.Address.AddrObject it1 = addr.Items[j];
                if (it1.Gars.Count != 1) 
                    continue;
                if (Pullenti.Address.AddressHelper.CompareLevels(it1.Level, Pullenti.Address.AddrLevel.Street) > 0) 
                    continue;
                for (int k = j - 1; k >= 0; k--) 
                {
                    Pullenti.Address.AddrObject it0 = addr.Items[k];
                    if (it0.Gars.Count == 0) 
                        break;
                    Pullenti.Address.GarObject g1 = it1.Gars[0];
                    int cou = 0;
                    Pullenti.Address.GarObject par = null;
                    foreach (Pullenti.Address.GarObject g in it0.Gars) 
                    {
                        if (g1.ParentIds.Contains(g.Id)) 
                        {
                            cou++;
                            par = g;
                        }
                    }
                    if (cou == 1 && it0.Gars.Count > 1) 
                    {
                        it0.Gars.Clear();
                        it0.Gars.Add(par);
                        ret = true;
                    }
                    break;
                }
            }
            for (int i = 0; i < (addr.Items.Count - 1); i++) 
            {
                Pullenti.Address.AddrObject it0 = addr.Items[i];
                if (it0.Gars.Count < 2) 
                    continue;
                Pullenti.Address.AddrObject it1 = addr.Items[i + 1];
                if (it1.Gars.Count != 1) 
                    continue;
                int hasPar = 0;
                foreach (Pullenti.Address.GarObject g in it0.Gars) 
                {
                    if (it1.FindGarByIds(g.ParentIds) != null) 
                        hasPar++;
                }
                if (hasPar > 0 && (hasPar < it0.Gars.Count)) 
                {
                    for (int j = it0.Gars.Count - 1; j >= 0; j--) 
                    {
                        if (it1.FindGarByIds(it0.Gars[j].ParentIds) == null) 
                            it0.Gars.RemoveAt(j);
                    }
                }
            }
            return ret;
        }
        static void _correctObjectByGars(Pullenti.Address.AddrObject it)
        {
            Pullenti.Address.AreaAttributes aa = it.Attrs as Pullenti.Address.AreaAttributes;
            if (aa == null) 
                return;
            List<string> typs = new List<string>();
            List<Pullenti.Address.GarLevel> levs = new List<Pullenti.Address.GarLevel>();
            foreach (Pullenti.Address.GarObject g in it.Gars) 
            {
                bool isRoad = false;
                foreach (string ty in (g.Attrs as Pullenti.Address.AreaAttributes).Types) 
                {
                    if (!typs.Contains(ty)) 
                    {
                        typs.Add(ty);
                        if (ty.Contains("дорога")) 
                            isRoad = true;
                    }
                }
                Pullenti.Address.GarLevel gl = g.Level;
                if (isRoad && gl == Pullenti.Address.GarLevel.Locality) 
                    gl = Pullenti.Address.GarLevel.Area;
                if (!levs.Contains(gl)) 
                    levs.Add(gl);
            }
            if (aa.Types.Count > 0 && ((aa.Types[0] == "населенный пункт" || aa.Types[0] == "почтовое отделение")) && typs.Count == 1) 
            {
                aa.Types[0] = typs[0];
                if (it.Level == Pullenti.Address.AddrLevel.Locality && levs.Count == 1 && levs[0] == Pullenti.Address.GarLevel.City) 
                    it.Level = Pullenti.Address.AddrLevel.City;
            }
            else if (typs.Count == 1 && aa.Types.Count > 1 && aa.Types.IndexOf(typs[0]) > 0) 
            {
                aa.Types.Remove(typs[0]);
                aa.Types.Insert(0, typs[0]);
            }
            if (aa.Types.Count == 0 && typs.Count == 1) 
                aa.Types.Add(typs[0]);
            if (aa.Types.Count > 1 && typs.Count == 1) 
            {
                if (aa.Types.Contains("проезд") && aa.Types.Contains("проспект")) 
                {
                    aa.Types.Clear();
                    aa.Types.Add(typs[0]);
                }
            }
            if (aa.Types.Count == 1 && aa.Types[0] == "район" && typs.Count == 1) 
            {
                aa.Types.Clear();
                aa.Types.Add(typs[0]);
            }
            if (aa.Types.Count == 0) 
                aa.Types.AddRange(typs);
            if ((typs.Count == 1 && it.Level == Pullenti.Address.AddrLevel.Street && aa.Types[0] != typs[0]) && typs[0] != "территория") 
            {
                if ((aa.Types.Count == 1 && aa.Types[0] == "улица" && levs.Count == 1) && levs[0] == Pullenti.Address.GarLevel.Area && it.Gars.Count == 1) 
                {
                    aa.Types.Clear();
                    aa.Types.Add("территория");
                    aa.Miscs.AddRange((it.Gars[0].Attrs as Pullenti.Address.AreaAttributes).Miscs);
                    it.Level = Pullenti.Address.AddrLevel.Territory;
                }
                else 
                {
                    if (aa.Types.Contains(typs[0])) 
                        aa.Types.Remove(typs[0]);
                    aa.Types.Insert(0, typs[0]);
                }
            }
            if (aa.Types.Count > 1 && aa.Types[0] == "улица") 
            {
                aa.Types.RemoveAt(0);
                aa.Types.Add("улица");
            }
            if (aa.Names.Count == 0) 
                return;
            foreach (Pullenti.Address.GarObject g in it.Gars) 
            {
                Pullenti.Address.AreaAttributes ga = g.Attrs as Pullenti.Address.AreaAttributes;
                foreach (string n in aa.Names) 
                {
                    foreach (string gn in ga.Names) 
                    {
                        if (aa.Names.Contains(gn)) 
                        {
                            if (gn != aa.Names[0]) 
                            {
                                aa.Names.Remove(gn);
                                aa.Names.Insert(0, gn);
                            }
                            return;
                        }
                        else if (gn.Contains(n)) 
                        {
                            if (n != aa.Names[0]) 
                            {
                                aa.Names.Remove(n);
                                aa.Names.Insert(0, n);
                            }
                            return;
                        }
                    }
                }
            }
            foreach (Pullenti.Address.GarObject g in it.Gars) 
            {
                Pullenti.Address.AreaAttributes ga = g.Attrs as Pullenti.Address.AreaAttributes;
                NameAnalyzer na = new NameAnalyzer();
                na.Process(ga.Names, (ga.Types.Count == 0 ? null : ga.Types[0]));
                Pullenti.Address.AreaAttributes aa2 = new Pullenti.Address.AreaAttributes();
                _setName(aa2, na.Ref, "NAME");
                if (aa2.Names.Count > 0) 
                {
                    if (!aa.Names.Contains(aa2.Names[0])) 
                        aa.Names.Insert(0, aa2.Names[0]);
                    else if (ga.Names[0].Length == aa.Names[0].Length) 
                        aa.Names.Insert(0, ga.Names[0]);
                    break;
                }
            }
            if (aa.Types.Count == 0 && it.Level == Pullenti.Address.AddrLevel.Street && aa.Names.Count > 0) 
            {
                if (aa.Names[0].EndsWith("ая")) 
                    aa.Types.Add("улица");
            }
        }
        static void _correctLevels(Pullenti.Address.TextAddress addr)
        {
            for (int i = 0; i < addr.Items.Count; i++) 
            {
                Pullenti.Address.AddrObject it = addr.Items[i];
                _correctObjectByGars(it);
                if (it.CrossObject != null) 
                    _correctObjectByGars(it.CrossObject);
                if ((i + 1) >= addr.Items.Count) 
                    continue;
                Pullenti.Address.AreaAttributes aa = it.Attrs as Pullenti.Address.AreaAttributes;
                Pullenti.Address.AddrObject it1 = addr.Items[i + 1];
                if (it.Level == Pullenti.Address.AddrLevel.District) 
                {
                    if (it1.Level == Pullenti.Address.AddrLevel.Territory || it1.Level == Pullenti.Address.AddrLevel.Street) 
                    {
                        if ((it.Attrs as Pullenti.Address.AreaAttributes).Types.Contains("улус")) 
                            it.Level = Pullenti.Address.AddrLevel.Locality;
                    }
                }
                else if (it.Level == Pullenti.Address.AddrLevel.Locality && it1.Level == Pullenti.Address.AddrLevel.Locality) 
                {
                    if (it1.Gars.Count > 0 && it1.Gars[0].Level == Pullenti.Address.GarLevel.Area) 
                        it1.Level = Pullenti.Address.AddrLevel.Territory;
                }
                else if (((it.Level == Pullenti.Address.AddrLevel.Territory && i > 0 && (Pullenti.Address.AddressHelper.CompareLevels(addr.Items[i - 1].Level, Pullenti.Address.AddrLevel.Locality) < 0)) && ((it1.Level == Pullenti.Address.AddrLevel.Territory || it1.Level == Pullenti.Address.AddrLevel.Street)) && it.Gars.Count == 1) && ((it.Gars[0].Level == Pullenti.Address.GarLevel.Locality || it.Gars[0].Level == Pullenti.Address.GarLevel.City))) 
                {
                    if (it.Level == Pullenti.Address.AddrLevel.Territory && aa.Miscs.Contains("дорога")) 
                    {
                    }
                    else 
                    {
                        it.Level = Pullenti.Address.AddrLevel.Locality;
                        if (aa.Types.Contains("территория")) 
                            aa.Types.Remove("территория");
                        string ty = (it.Gars[0].Attrs as Pullenti.Address.AreaAttributes).Types[0];
                        if (!aa.Types.Contains(ty)) 
                            aa.Types.Add(ty);
                    }
                }
                else if ((it.Level == Pullenti.Address.AddrLevel.City && it.Gars.Count > 0 && it.Gars[0].Level == Pullenti.Address.GarLevel.Settlement) && it1.Level == Pullenti.Address.AddrLevel.Locality) 
                {
                    it.Level = Pullenti.Address.AddrLevel.Settlement;
                    aa.Types.Clear();
                    aa.Types.AddRange((it.Gars[0].Attrs as Pullenti.Address.AreaAttributes).Types);
                }
                else if ((it.Level == Pullenti.Address.AddrLevel.Locality && Pullenti.Address.AddressHelper.CompareLevels(it1.Level, Pullenti.Address.AddrLevel.Street) >= 0 && i > 0) && addr.Items[i - 1].Level == Pullenti.Address.AddrLevel.City) 
                {
                    if (it.Gars.Count > 0 && it.Gars[0].Level == Pullenti.Address.GarLevel.Area) 
                    {
                        it.Level = Pullenti.Address.AddrLevel.Territory;
                        aa.Types.Clear();
                        aa.Types.AddRange((it.Gars[0].Attrs as Pullenti.Address.AreaAttributes).Types);
                    }
                }
            }
        }
        static int _getId(string v)
        {
            return int.Parse(v.Substring(1));
        }
        static void _addParIds(List<int> parIds, Pullenti.Address.AddrObject ao)
        {
            foreach (Pullenti.Address.GarObject p in ao.Gars) 
            {
                int id = _getId(p.Id);
                if (!parIds.Contains(id)) 
                    parIds.Add(id);
            }
        }
        static bool _canSearchGars(NameAnalyzer r, Pullenti.Address.TextAddress addr, int i)
        {
            if (r.Level == Pullenti.Address.AddrLevel.Territory || r.Level == Pullenti.Address.AddrLevel.Street) 
            {
                for (int j = 0; j < i; j++) 
                {
                    if (addr.Items[j].Gars.Count > 0) 
                    {
                        Pullenti.Address.AddrObject it = addr.Items[j];
                        if (((it.Level == Pullenti.Address.AddrLevel.RegionCity || it.Level == Pullenti.Address.AddrLevel.City || it.Level == Pullenti.Address.AddrLevel.Settlement) || it.Level == Pullenti.Address.AddrLevel.Locality || it.Level == Pullenti.Address.AddrLevel.Undefined) || it.Level == Pullenti.Address.AddrLevel.Territory) 
                            return true;
                        if (it.Level == Pullenti.Address.AddrLevel.District) 
                        {
                            if ((it.Attrs as Pullenti.Address.AreaAttributes).Types.Contains("улус") || (it.Attrs as Pullenti.Address.AreaAttributes).Types.Contains("городской округ") || (it.Attrs as Pullenti.Address.AreaAttributes).Types.Contains("муниципальный округ")) 
                                return true;
                            if (it.Gars.Count > 0) 
                            {
                                if ((it.Gars[0].Attrs as Pullenti.Address.AreaAttributes).Types.Contains("городской округ")) 
                                    return true;
                            }
                        }
                        if (r.Level == Pullenti.Address.AddrLevel.Territory) 
                        {
                            if (j == (i - 1) && ((it.Level == Pullenti.Address.AddrLevel.District || it.Level == Pullenti.Address.AddrLevel.Settlement))) 
                                return true;
                            if (j == (i - 2) && it.Level == Pullenti.Address.AddrLevel.District && ((addr.Items[j + 1].DetailTyp != Pullenti.Address.DetailType.Undefined || addr.Items[j + 1].Level == Pullenti.Address.AddrLevel.Territory))) 
                                return true;
                        }
                        if (r.Level == Pullenti.Address.AddrLevel.Street && i == 1) 
                        {
                            string nam = r.Ref.GetStringValue("NAME");
                            if (nam != null && nam.IndexOf(' ') > 0) 
                                return true;
                            if (it.Gars[0].RegionNumber == 50) 
                                return true;
                        }
                    }
                }
                return false;
            }
            if (r.Level == Pullenti.Address.AddrLevel.Locality && i == 0) 
                return false;
            return true;
        }
        public Pullenti.Ner.Address.AddressReferent _processAddress(Pullenti.Address.TextAddress addr, out bool hasSecVar)
        {
            hasSecVar = false;
            if (addr.Items.Count == 0) 
                return null;
            Pullenti.Ner.Address.AddressReferent ar = null;
            List<byte> regions = new List<byte>();
            bool otherCountry = false;
            List<int> parIds = new List<int>();
            Pullenti.Address.AddrObject uaCountry = null;
            bool rev = false;
            for (int i = 0; i < addr.Items.Count; i++) 
            {
                Pullenti.Address.AddrObject it = addr.Items[i];
                Pullenti.Address.AreaAttributes aa = it.Attrs as Pullenti.Address.AreaAttributes;
                if (aa == null) 
                    break;
                if (GarHelper.GarIndex == null || otherCountry) 
                    continue;
                if (i == 0 && it.Level == Pullenti.Address.AddrLevel.Country) 
                {
                    if (aa.Names.Contains("Украина")) 
                    {
                        uaCountry = it;
                        addr.Items.RemoveAt(0);
                        i--;
                        continue;
                    }
                    otherCountry = true;
                    continue;
                }
                if (it.Gars.Count > 0) 
                {
                    if (regions.Count == 0) 
                        regions.Add((byte)it.Gars[0].RegionNumber);
                    continue;
                }
                NameAnalyzer r = it.Tag as NameAnalyzer;
                if (r == null) 
                    continue;
                int maxCount = 50;
                if (addr.Items[0].Level == Pullenti.Address.AddrLevel.RegionCity) 
                    maxCount = 100;
                else if (r.Level == Pullenti.Address.AddrLevel.Territory) 
                    maxCount = 200;
                parIds.Clear();
                int pcou = 0;
                for (int j = i - 1; j >= 0; j--) 
                {
                    Pullenti.Address.AddrObject it0 = addr.Items[j];
                    if (it0.Gars.Count == 0) 
                        continue;
                    if (Pullenti.Address.AddressHelper.CompareLevels(it0.Level, it.Level) >= 0 && !Pullenti.Address.AddressHelper.CanBeParent(it0.Level, it.Level)) 
                        break;
                    _addParIds(parIds, it0);
                    pcou++;
                    if (it0.Level == Pullenti.Address.AddrLevel.Locality) 
                        break;
                    if (it.Level == Pullenti.Address.AddrLevel.Territory && pcou > 1) 
                        break;
                    if (it0.Level == Pullenti.Address.AddrLevel.City) 
                    {
                        if (it.Level == Pullenti.Address.AddrLevel.Locality) 
                        {
                            foreach (Pullenti.Address.GarObject g in it0.Gars) 
                            {
                                if (g.ParentIds.Count == 0) 
                                    continue;
                                Pullenti.Address.GarObject gg = this.GetGarObject(g.ParentIds[0]);
                                if (gg != null && gg.Level == Pullenti.Address.GarLevel.MunicipalArea) 
                                    parIds.Add(_getId(gg.Id));
                            }
                        }
                        break;
                    }
                }
                if (parIds.Count == 0) 
                {
                    if (i > 0) 
                    {
                        if (i == 1 && r.Level == Pullenti.Address.AddrLevel.City && addr.Items[0].Gars.Count == 0) 
                        {
                            Pullenti.Address.AddrObject cou = CorrectionHelper.FindCountry(it);
                            if (cou != null) 
                            {
                                addr.Items.Insert(0, cou);
                                break;
                            }
                        }
                        if (it.Level == Pullenti.Address.AddrLevel.City && RegionHelper.IsBigCityA(it) != null) 
                        {
                        }
                        else 
                            continue;
                    }
                    if (Pullenti.Address.AddressHelper.CompareLevels(it.Level, Pullenti.Address.AddrLevel.Locality) >= 0) 
                    {
                        if (m_Params == null) 
                            continue;
                        if (m_Params.DefaultObject == null) 
                        {
                            foreach (int rid in m_Params.DefaultRegions) 
                            {
                                regions.Add((byte)rid);
                            }
                            if (regions.Count == 0) 
                                continue;
                        }
                        else 
                        {
                            parIds.Add(_getId(m_Params.DefaultObject.Id));
                            if (m_Params.DefaultObject.RegionNumber > 0) 
                                regions.Add((byte)m_Params.DefaultObject.RegionNumber);
                            Pullenti.Address.AddrObject to1 = GarHelper.CreateAddrObject(m_Params.DefaultObject);
                            addr.Items.Insert(0, to1);
                            i++;
                        }
                    }
                }
                if (!_canSearchGars(r, addr, i)) 
                {
                    if (m_Params == null || ((m_Params.DefaultRegions.Count == 0 && m_Params.DefaultObject == null))) 
                        continue;
                }
                List<Pullenti.Address.Internal.Gar.AreaTreeObject> probs = GarHelper.GarIndex.GetStringEntries(r, regions, parIds, maxCount);
                if (probs == null && i > 0 && addr.Items[i - 1].DetailTyp != Pullenti.Address.DetailType.Undefined) 
                {
                    parIds.Clear();
                    foreach (Pullenti.Address.GarObject g in addr.Items[i - 1].Gars) 
                    {
                        foreach (string p in g.ParentIds) 
                        {
                            if (!parIds.Contains(_getId(p))) 
                                parIds.Add(_getId(p));
                        }
                    }
                    if (parIds.Count == 0 && i > 1) 
                    {
                        foreach (Pullenti.Address.GarObject g in addr.Items[i - 2].Gars) 
                        {
                            parIds.Add(_getId(g.Id));
                        }
                    }
                    if (parIds.Count > 0) 
                        probs = GarHelper.GarIndex.GetStringEntries(r, regions, parIds, maxCount);
                }
                if ((probs == null && i == 0 && it.Level == Pullenti.Address.AddrLevel.District) && addr.Items.Count > 1 && addr.Items[1].Level == Pullenti.Address.AddrLevel.City) 
                {
                    if (RegionHelper.IsBigCityA(addr.Items[1]) != null && !rev) 
                    {
                        addr.Items.RemoveAt(0);
                        addr.Items.Insert(1, it);
                        i--;
                        rev = true;
                        continue;
                    }
                }
                if ((probs == null && i == 1 && it.Level == Pullenti.Address.AddrLevel.RegionCity) && addr.Items[0].Level == Pullenti.Address.AddrLevel.RegionArea) 
                {
                    addr.Items.RemoveAt(0);
                    i = -1;
                    regions.Clear();
                    continue;
                }
                if (probs == null && i == 0 && r.Level == Pullenti.Address.AddrLevel.City) 
                {
                    Pullenti.Address.AddrObject cou = CorrectionHelper.FindCountry(it);
                    if (cou != null) 
                    {
                        addr.Items.Insert(0, cou);
                        break;
                    }
                }
                if (probs != null && r.Level == Pullenti.Address.AddrLevel.District && ((i + 1) < addr.Items.Count)) 
                {
                    if (addr.Items[i + 1].Level == Pullenti.Address.AddrLevel.Street || ((addr.Items[i + 1].Level == Pullenti.Address.AddrLevel.Locality && (i + 2) == addr.Items.Count))) 
                    {
                        NameAnalyzer alt = r.TryCreateAlternative(false, (i > 0 ? addr.Items[i - 1] : null), ((i + 1) < addr.Items.Count ? addr.Items[i + 1] : null));
                        if (alt != null) 
                        {
                            List<int> parIds0 = new List<int>();
                            foreach (Pullenti.Address.Internal.Gar.AreaTreeObject p in probs) 
                            {
                                parIds0.Add(p.Id);
                            }
                            List<Pullenti.Address.Internal.Gar.AreaTreeObject> probs2 = GarHelper.GarIndex.GetStringEntries(alt, regions, parIds0, maxCount);
                            if (probs2 != null) 
                            {
                                int setls = 0;
                                foreach (Pullenti.Address.Internal.Gar.AreaTreeObject p in probs2) 
                                {
                                    if (p.Level == Pullenti.Address.AddrLevel.Settlement) 
                                        setls++;
                                }
                                if (setls > 0 && (setls < probs2.Count)) 
                                {
                                    for (int jj = probs2.Count - 1; jj >= 0; jj--) 
                                    {
                                        if (probs2[jj].Level == Pullenti.Address.AddrLevel.Settlement) 
                                            probs2.RemoveAt(jj);
                                    }
                                }
                            }
                            if (probs2 != null && probs2.Count == 1) 
                            {
                                Pullenti.Address.AddrObject it1 = addr.Items[i + 1];
                                bool ok2 = true;
                                if (it1.Level == Pullenti.Address.AddrLevel.Locality && probs2[0].Level == it1.Level) 
                                {
                                    ok2 = false;
                                    NameAnalyzer r2 = it1.Tag as NameAnalyzer;
                                    NameAnalyzer alt2 = r2.TryCreateAlternative(true, null, null);
                                    if (alt2 != null) 
                                    {
                                        List<int> parIds2 = new List<int>();
                                        parIds2.Add(probs2[0].Id);
                                        List<Pullenti.Address.Internal.Gar.AreaTreeObject> probs3 = GarHelper.GarIndex.GetStringEntries(alt2, regions, parIds2, maxCount);
                                        if (probs3 != null && probs3.Count == 1) 
                                            ok2 = true;
                                    }
                                }
                                else if (it1.Level == Pullenti.Address.AddrLevel.Street && ((alt.Level == Pullenti.Address.AddrLevel.Locality || alt.Level == Pullenti.Address.AddrLevel.City))) 
                                {
                                    ok2 = false;
                                    List<int> parIds2 = new List<int>();
                                    parIds2.Add(probs2[0].Id);
                                    List<Pullenti.Address.Internal.Gar.AreaTreeObject> probs3 = GarHelper.GarIndex.GetStringEntries(it1.Tag as NameAnalyzer, regions, parIds2, maxCount);
                                    if (probs3 != null && probs3.Count == 1) 
                                        ok2 = true;
                                }
                                if (!ok2) 
                                {
                                }
                                else if (!CreateAltsRegime) 
                                    hasSecVar = true;
                                else 
                                {
                                    probs = probs2;
                                    it.Level = probs2[0].Level;
                                    aa.Types.Clear();
                                    aa.Types.AddRange(alt.Types);
                                    aa.Miscs.Clear();
                                    if (alt.Miscs != null) 
                                        aa.Miscs.AddRange(alt.Miscs);
                                    it.Tag = alt;
                                    r = alt;
                                }
                            }
                        }
                    }
                }
                if (probs == null) 
                {
                    NameAnalyzer alt = r.TryCreateAlternative(false, (i > 0 ? addr.Items[i - 1] : null), ((i + 1) < addr.Items.Count ? addr.Items[i + 1] : null));
                    if (alt != null) 
                    {
                        if (!CreateAltsRegime) 
                            hasSecVar = true;
                        else 
                        {
                            if (_canSearchGars(alt, addr, i)) 
                                probs = GarHelper.GarIndex.GetStringEntries(alt, regions, parIds, maxCount);
                            if (probs != null && ((probs.Count == 1 || it.Level == Pullenti.Address.AddrLevel.District))) 
                            {
                                it.Tag = alt;
                                r = alt;
                                it.Level = probs[0].Level;
                                foreach (Pullenti.Address.Internal.Gar.AreaTreeObject p in probs) 
                                {
                                    if (p.Level != it.Level) 
                                    {
                                        it.Level = Pullenti.Address.AddrLevel.Undefined;
                                        break;
                                    }
                                }
                                aa.Types.Clear();
                                aa.Types.AddRange(alt.Types);
                                aa.Miscs.Clear();
                                if (alt.Miscs != null) 
                                    aa.Miscs.AddRange(alt.Miscs);
                            }
                            else 
                            {
                                NameAnalyzer alt2 = r.TryCreateAlternative(true, (i > 0 ? addr.Items[i - 1] : null), ((i + 1) < addr.Items.Count ? addr.Items[i + 1] : null));
                                if (alt2 != null) 
                                {
                                    List<Pullenti.Address.Internal.Gar.AreaTreeObject> probs2 = null;
                                    if (_canSearchGars(alt2, addr, i)) 
                                        probs2 = GarHelper.GarIndex.GetStringEntries(alt2, regions, parIds, maxCount);
                                    if (probs2 != null && ((probs2.Count == 1 || ((probs2.Count == 2 && probs2[0].Level == probs2[1].Level))))) 
                                    {
                                        probs = probs2;
                                        it.Tag = alt2;
                                        r = alt2;
                                        it.Level = probs[0].Level;
                                        aa.Types.Clear();
                                        aa.Types.AddRange(alt2.Types);
                                        aa.Miscs.Clear();
                                        if (alt2.Miscs != null) 
                                            aa.Miscs.AddRange(alt2.Miscs);
                                    }
                                }
                            }
                        }
                    }
                }
                if (probs != null && probs.Count == 1 && it.Level != probs[0].Level) 
                {
                    if (r.Level == Pullenti.Address.AddrLevel.Territory || ((r.Level == Pullenti.Address.AddrLevel.Locality && ((i == (addr.Items.Count - 1) || addr.Items[i + 1].Level != Pullenti.Address.AddrLevel.Territory))))) 
                    {
                        List<int> parIds2 = new List<int>();
                        parIds2.Add(probs[0].Id);
                        List<Pullenti.Address.Internal.Gar.AreaTreeObject> probs2 = GarHelper.GarIndex.GetStringEntries(r, regions, parIds2, maxCount);
                        if (probs2 != null && ((i + 1) < addr.Items.Count)) 
                        {
                            List<Pullenti.Address.Internal.Gar.AreaTreeObject> prob3 = GarHelper.GarIndex.GetStringEntries(addr.Items[i + 1].Tag as NameAnalyzer, regions, parIds2, maxCount);
                            if (prob3 != null) 
                                probs2 = null;
                        }
                        if (probs2 != null) 
                            probs = probs2;
                    }
                }
                if ((probs != null && probs.Count >= 2 && regions.Count == 0) && ((i + 1) < addr.Items.Count) && RegionHelper.IsBigCityA(addr.Items[i + 1]) != null) 
                {
                    List<Pullenti.Address.Internal.Gar.AreaTreeObject> probs1 = GarHelper.GarIndex.GetStringEntries(addr.Items[i + 1].Tag as NameAnalyzer, regions, parIds, maxCount);
                    if (probs1 != null) 
                    {
                        foreach (Pullenti.Address.Internal.Gar.AreaTreeObject p in probs1) 
                        {
                            if (!regions.Contains(p.Region)) 
                                regions.Add(p.Region);
                        }
                    }
                    if (regions.Count > 0) 
                        probs = GarHelper.GarIndex.GetStringEntries(r, regions, parIds, maxCount);
                }
                if ((probs == null && regions.Count == 1 && parIds.Count > 0) && ((it.Level == Pullenti.Address.AddrLevel.Locality || it.Level == Pullenti.Address.AddrLevel.City))) 
                {
                    if ((i > 1 && RegionHelper.IsBigCityA(addr.Items[i - 1]) != null && addr.Items[i - 2].Level == Pullenti.Address.AddrLevel.District) && addr.Items[i - 2].Gars.Count > 0) 
                    {
                        List<int> pars0 = new List<int>();
                        _addParIds(pars0, addr.Items[i - 2]);
                        probs = GarHelper.GarIndex.GetStringEntries(r, regions, pars0, maxCount);
                    }
                }
                bool allTerrs = true;
                if (probs != null) 
                {
                    foreach (Pullenti.Address.Internal.Gar.AreaTreeObject p in probs) 
                    {
                        if (p.Level != Pullenti.Address.AddrLevel.Territory) 
                            allTerrs = false;
                    }
                }
                if (allTerrs) 
                {
                    if (regions.Count == 1 && parIds.Count > 0 && ((it.Level == Pullenti.Address.AddrLevel.Locality || it.Level == Pullenti.Address.AddrLevel.City))) 
                    {
                        if (probs == null) 
                        {
                            if (RestructHelper.Restruct(this, addr, i)) 
                            {
                                regions.Clear();
                                i = -1;
                                continue;
                            }
                        }
                        List<Pullenti.Address.Internal.Gar.AreaTreeObject> probs2 = GarHelper.GarIndex.GetStringEntries(r, regions, null, maxCount);
                        if (it.Level == Pullenti.Address.AddrLevel.City && probs2 == null) 
                            probs2 = GarHelper.GarIndex.GetStringEntries(r, null, null, maxCount);
                        if (probs2 != null) 
                        {
                            for (int k = probs2.Count - 1; k >= 0; k--) 
                            {
                                Pullenti.Address.Internal.Gar.AreaTreeObject pp = probs2[k];
                                List<string> ids = new List<string>();
                                foreach (int p in pp.ParentIds) 
                                {
                                    ids.Add(string.Format("a{0}", p));
                                }
                                if (addr.FindGarByIds(ids) == null) 
                                    probs2.RemoveAt(k);
                            }
                        }
                        if (probs2 != null && ((probs2.Count == 0 || probs2.Count > 30))) 
                            probs2 = null;
                        if ((probs2 != null && probs2.Count <= 2 && i > 0) && RegionHelper.IsBigCityA(addr.Items[i - 1]) != null) 
                        {
                            if (probs != null && probs.Contains(probs2[0])) 
                            {
                            }
                            else 
                            {
                                addr.Items.RemoveAt(i - 1);
                                i--;
                            }
                        }
                        if (probs == null) 
                            probs = probs2;
                        if (probs != null && probs.Count > 1) 
                        {
                            if (r.Level == Pullenti.Address.AddrLevel.City && r.Strings.Contains("ТРОИЦК")) 
                            {
                                for (int k = probs.Count - 1; k >= 0; k--) 
                                {
                                    if (probs[k].Region != 77) 
                                        probs.RemoveAt(k);
                                }
                                if (probs.Count == 1) 
                                {
                                    if (i > 0) 
                                    {
                                        addr.Items.RemoveRange(0, i);
                                        i = 0;
                                        regions.Clear();
                                        regions.Add(77);
                                    }
                                }
                            }
                        }
                    }
                }
                if ((probs == null && ((it.Level == Pullenti.Address.AddrLevel.City || it.Level == Pullenti.Address.AddrLevel.RegionCity || it.Level == Pullenti.Address.AddrLevel.Locality)) && aa.Number != null) && ((i + 1) < addr.Items.Count) && addr.Items[i + 1].Level == Pullenti.Address.AddrLevel.Street) 
                {
                    string num = aa.Number;
                    Pullenti.Ner.Referent cit = r.Ref.Clone();
                    cit.AddSlot("NUMBER", null, true, 0);
                    NameAnalyzer naa = new NameAnalyzer();
                    naa.InitByReferent(cit, false);
                    List<Pullenti.Address.Internal.Gar.AreaTreeObject> probs2 = GarHelper.GarIndex.GetStringEntries(naa, regions, parIds, maxCount);
                    if (probs2 != null) 
                    {
                        aa.Number = null;
                        it.Tag = naa;
                        r = naa;
                        List<int> pars1 = new List<int>();
                        pars1.Add(probs2[0].Id);
                        List<Pullenti.Address.Internal.Gar.AreaTreeObject> probs3 = GarHelper.GarIndex.GetStringEntries(addr.Items[i + 1].Tag as NameAnalyzer, regions, pars1, maxCount);
                        if (probs3 != null) 
                        {
                        }
                        else 
                        {
                            Pullenti.Ner.Referent stret = (addr.Items[i + 1].Tag as NameAnalyzer).Ref;
                            stret.AddSlot("NUMBER", num, false, 0);
                            (addr.Items[i + 1].Attrs as Pullenti.Address.AreaAttributes).Number = num;
                            naa = new NameAnalyzer();
                            naa.InitByReferent(stret, false);
                            addr.Items[i + 1].Tag = naa;
                        }
                        probs = probs2;
                    }
                }
                if (((regions.Count < 3) && i == (addr.Items.Count - 1) && probs == null) && (((it.Level == Pullenti.Address.AddrLevel.City || it.Level == Pullenti.Address.AddrLevel.Locality || it.Level == Pullenti.Address.AddrLevel.Territory) || it.Level == Pullenti.Address.AddrLevel.Street))) 
                {
                    bool cont = false;
                    foreach (string nn in (it.Attrs as Pullenti.Address.AreaAttributes).Names) 
                    {
                        int ii = nn.IndexOf(' ');
                        if (ii < 0) 
                            continue;
                        if ((it.Attrs as Pullenti.Address.AreaAttributes).Number != null) 
                            break;
                        Pullenti.Ner.Referent rr = null;
                        if (r.Ref is Pullenti.Ner.Geo.GeoReferent) 
                        {
                            rr = new Pullenti.Ner.Geo.GeoReferent();
                            rr.AddSlot(Pullenti.Ner.Geo.GeoReferent.ATTR_NAME, nn.Substring(0, ii).ToUpper(), false, 0);
                            foreach (string ty in (r.Ref as Pullenti.Ner.Geo.GeoReferent).Typs) 
                            {
                                rr.AddSlot(Pullenti.Ner.Geo.GeoReferent.ATTR_TYPE, ty, false, 0);
                            }
                        }
                        else 
                        {
                            rr = new Pullenti.Ner.Address.StreetReferent();
                            (rr as Pullenti.Ner.Address.StreetReferent).Kind = (r.Ref as Pullenti.Ner.Address.StreetReferent).Kind;
                            rr.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NAME, nn.Substring(0, ii).ToUpper(), false, 0);
                            foreach (string ty in (r.Ref as Pullenti.Ner.Address.StreetReferent).Typs) 
                            {
                                rr.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_TYPE, ty, false, 0);
                            }
                        }
                        NameAnalyzer naa = new NameAnalyzer();
                        naa.InitByReferent(rr, false);
                        List<Pullenti.Address.Internal.Gar.AreaTreeObject> probs2 = GarHelper.GarIndex.GetStringEntries(naa, regions, parIds, maxCount);
                        if (probs2 == null && i > 0 && (Pullenti.Address.AddressHelper.CompareLevels(addr.Items[i - 1].Level, Pullenti.Address.AddrLevel.City) < 0)) 
                        {
                            rr = new Pullenti.Ner.Geo.GeoReferent();
                            rr.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NAME, nn.Substring(0, ii).ToUpper(), false, 0);
                            rr.AddSlot("TYPE", "город", false, 0);
                            naa = new NameAnalyzer();
                            naa.InitByReferent(rr, false);
                            probs2 = GarHelper.GarIndex.GetStringEntries(naa, regions, parIds, maxCount);
                        }
                        if (probs2 != null) 
                        {
                            for (int jj = probs2.Count - 1; jj >= 0; jj--) 
                            {
                                if (probs2[jj].Level == Pullenti.Address.AddrLevel.Street) 
                                    probs2.RemoveAt(jj);
                            }
                        }
                        if (probs2 != null && probs2.Count > 0 && (probs2.Count < 20)) 
                        {
                            Pullenti.Ner.Address.StreetReferent ss = new Pullenti.Ner.Address.StreetReferent();
                            ss.AddSlot("NAME", nn.Substring(ii + 1).ToUpper(), false, 0);
                            ss.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_TYPE, "улица", false, 0);
                            if (rr is Pullenti.Ner.Geo.GeoReferent) 
                                ss.AddSlot("GEO", rr, false, 0);
                            else 
                                ss.Higher = rr as Pullenti.Ner.Address.StreetReferent;
                            NameAnalyzer naa2 = new NameAnalyzer();
                            naa2.InitByReferent(ss, false);
                            bool ok = false;
                            List<int> pars0 = new List<int>();
                            foreach (Pullenti.Address.Internal.Gar.AreaTreeObject pp in probs2) 
                            {
                                pars0.Clear();
                                pars0.Add(pp.Id);
                                List<Pullenti.Address.Internal.Gar.AreaTreeObject> probs3 = GarHelper.GarIndex.GetStringEntries(naa2, regions, pars0, maxCount);
                                if (probs3 != null) 
                                {
                                    ok = true;
                                    break;
                                }
                            }
                            if (!ok) 
                                continue;
                            Pullenti.Address.TextAddress tmp = new Pullenti.Address.TextAddress();
                            _createAddressItems(tmp, ss, null, 0);
                            if (tmp.Items.Count == 2) 
                            {
                                addr.Items.RemoveAt(i);
                                addr.Items.InsertRange(i, tmp.Items);
                                i--;
                                cont = true;
                                break;
                            }
                        }
                    }
                    if (cont) 
                        continue;
                }
                if (((probs == null && i == 0 && it.Level == Pullenti.Address.AddrLevel.City) && ((i + 1) < addr.Items.Count) && addr.Items[i + 1].Level == Pullenti.Address.AddrLevel.District) && !rev) 
                {
                    addr.Items.RemoveAt(0);
                    addr.Items.Insert(1, it);
                    i--;
                    rev = true;
                    continue;
                }
                if (((probs == null && ((it.Level == Pullenti.Address.AddrLevel.Street || it.Level == Pullenti.Address.AddrLevel.Territory || it.Level == Pullenti.Address.AddrLevel.Locality)) && i == (addr.Items.Count - 1)) && aa.Number != null && aa.Names.Count > 0) && ar == null) 
                {
                    bool lastNum = false;
                    if (r.Ref.Occurrence.Count > 0) 
                    {
                        string occ = r.Ref.Occurrence[0].GetText();
                        if (occ.EndsWith(aa.Number)) 
                            lastNum = true;
                    }
                    if (lastNum) 
                    {
                        NameAnalyzer na2 = new NameAnalyzer();
                        r.Ref.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NUMBER, null, true, 0);
                        na2.InitByReferent(r.Ref, false);
                        probs = GarHelper.GarIndex.GetStringEntries(na2, regions, parIds, maxCount);
                        r.Ref.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NUMBER, aa.Number, true, 0);
                        if (probs != null) 
                        {
                            ar = new Pullenti.Ner.Address.AddressReferent();
                            int ii = aa.Number.IndexOf('-');
                            if (ii < 0) 
                                ar.HouseOrPlot = aa.Number;
                            else 
                            {
                                ar.House = aa.Number.Substring(0, ii);
                                ar.Flat = aa.Number.Substring(ii + 1);
                            }
                            aa.Number = null;
                        }
                    }
                }
                if ((probs != null && probs.Count > 10 && i == 0) && regions.Count == 0) 
                    probs = null;
                if (it.Level == Pullenti.Address.AddrLevel.Street && probs != null && probs.Count > 5) 
                {
                    if (i == 0) 
                        probs = null;
                    else 
                    {
                        Pullenti.Address.AddrObject it0 = addr.Items[i - 1];
                        if (it0.Level == Pullenti.Address.AddrLevel.District) 
                        {
                            if (it0.Gars.Count > 0) 
                            {
                                foreach (Pullenti.Address.GarObject chi in GarHelper.GetChildrenObjects(it0.Gars[0].Id, true)) 
                                {
                                    if (chi.Level != Pullenti.Address.GarLevel.City) 
                                        continue;
                                    if ((chi.Attrs as Pullenti.Address.AreaAttributes).Names.Count > 0 && (it0.Gars[0].Attrs as Pullenti.Address.AreaAttributes).Names.Contains((chi.Attrs as Pullenti.Address.AreaAttributes).Names[0])) 
                                    {
                                    }
                                    else 
                                        continue;
                                    List<int> pars = new List<int>();
                                    pars.Add(_getId(chi.Id));
                                    List<Pullenti.Address.Internal.Gar.AreaTreeObject> probs1 = GarHelper.GarIndex.GetStringEntries(r, regions, pars, maxCount);
                                    if (probs1 != null && (probs1.Count < 3)) 
                                        probs = probs1;
                                }
                            }
                            if (probs.Count > 3) 
                            {
                                if (i == 1) 
                                    probs = null;
                                else if (addr.Items[i - 2].Level != Pullenti.Address.AddrLevel.City && addr.Items[i - 2].Level != Pullenti.Address.AddrLevel.RegionCity) 
                                    probs = null;
                            }
                        }
                    }
                }
                if (probs == null && i >= 2 && ((it.Level == Pullenti.Address.AddrLevel.Street || it.Level == Pullenti.Address.AddrLevel.Territory))) 
                {
                    Pullenti.Address.AddrObject it0 = addr.Items[i - 1];
                    Pullenti.Address.AddrObject it00 = addr.Items[i - 2];
                    if (it0.Gars.Count > 0 && it0.Gars[0].Expired && it00.Gars.Count == 1) 
                    {
                        List<int> pars = new List<int>();
                        pars.Add(_getId(it00.Gars[0].Id));
                        probs = GarHelper.GarIndex.GetStringEntries(r, regions, pars, maxCount);
                    }
                }
                if ((probs == null && i >= 2 && it.Level == Pullenti.Address.AddrLevel.Street) && addr.Items[0].Level == Pullenti.Address.AddrLevel.RegionCity) 
                {
                    List<Pullenti.Address.Internal.Gar.AreaTreeObject> probs1 = GarHelper.GarIndex.GetStringEntries(r, regions, null, maxCount);
                    if (probs1 != null && probs1.Count == 1) 
                    {
                        probs = probs1;
                        if (addr.Items[i - 1].Gars.Count > 0) 
                            addr.Items[i - 1].Gars.Clear();
                    }
                }
                if (probs == null && regions.Count == 1 && it.Level == Pullenti.Address.AddrLevel.City) 
                {
                    r.Level = Pullenti.Address.AddrLevel.Locality;
                    probs = GarHelper.GarIndex.GetStringEntries(r, regions, parIds, maxCount);
                    r.Level = Pullenti.Address.AddrLevel.City;
                }
                if ((probs == null && it.Level == Pullenti.Address.AddrLevel.Street && i > 0) && addr.Items[i - 1].Level == Pullenti.Address.AddrLevel.Territory && addr.Items[i - 1].Gars.Count == 0) 
                {
                    Pullenti.Address.AddrObject it0 = addr.Items[i - 1];
                    NameAnalyzer na = it0.Tag as NameAnalyzer;
                    Pullenti.Ner.Address.StreetReferent sr = new Pullenti.Ner.Address.StreetReferent();
                    foreach (string n in na.Ref.GetStringValues("NAME")) 
                    {
                        sr.AddSlot("NAME", n, false, 0);
                    }
                    foreach (Pullenti.Ner.Slot s in r.Ref.Slots) 
                    {
                        if (s.TypeName != "NAME") 
                            sr.AddSlot(s.TypeName, s.Value, false, 0);
                    }
                    NameAnalyzer na1 = new NameAnalyzer();
                    na1.InitByReferent(sr, false);
                    probs = GarHelper.GarIndex.GetStringEntries(na1, regions, parIds, maxCount);
                    if (probs != null) 
                    {
                        it.Tag = na1;
                        addr.Items.RemoveAt(i - 1);
                        i--;
                    }
                }
                if (probs != null) 
                {
                    this._addGars(addr, probs, i, regions, false);
                    if ((probs != null && probs.Count > 0 && it.Gars.Count == 0) && i > 0) 
                    {
                        Pullenti.Address.AddrObject it0 = addr.Items[i - 1];
                        if (it0.Gars.Count == 0 && it0.Level == Pullenti.Address.AddrLevel.District && i > 1) 
                            it0 = addr.Items[i - 2];
                        Pullenti.Address.AreaAttributes aa0 = it0.Attrs as Pullenti.Address.AreaAttributes;
                        if ((((it.Level == Pullenti.Address.AddrLevel.Street || it.Level == Pullenti.Address.AddrLevel.Territory)) && it0.Level == Pullenti.Address.AddrLevel.District && it0.Gars.Count == 1) && aa0.Names.Count > 0) 
                        {
                            string nam = aa0.Names[0];
                            if (nam.Length > 5) 
                                nam = nam.Substring(0, nam.Length - 3);
                            List<Pullenti.Address.GarObject> chils = Pullenti.Address.AddressService.GetObjects(it0.Gars[0].Id, true);
                            if (chils != null) 
                            {
                                foreach (Pullenti.Address.GarObject ch in chils) 
                                {
                                    Pullenti.Address.AreaAttributes ga = ch.Attrs as Pullenti.Address.AreaAttributes;
                                    if (ch.Level != Pullenti.Address.GarLevel.City && ch.Level != Pullenti.Address.GarLevel.Locality) 
                                        continue;
                                    if (ga.Names.Count == 0 || !ga.Names[0].StartsWith(nam, StringComparison.OrdinalIgnoreCase)) 
                                        continue;
                                    parIds.Clear();
                                    parIds.Add(_getId(ch.Id));
                                    List<Pullenti.Address.Internal.Gar.AreaTreeObject> probs0 = GarHelper.GarIndex.GetStringEntries(r, regions, parIds, maxCount);
                                    if (probs0 != null) 
                                    {
                                        Pullenti.Address.AddrObject it00 = GarHelper.CreateAddrObject(ch);
                                        if (it00 != null) 
                                        {
                                            addr.Items.Insert(i, it00);
                                            i++;
                                            this._addGars(addr, probs0, i, regions, false);
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
                else if ((it.Level == Pullenti.Address.AddrLevel.Territory && i > 0 && addr.Items[i - 1].Gars.Count == 1) && aa.Names.Count > 0) 
                {
                    Pullenti.Address.GarObject g0 = addr.Items[i - 1].Gars[0];
                    if (g0.ChildrenCount < 500) 
                    {
                        List<Pullenti.Address.GarObject> childs = Pullenti.Address.AddressService.GetObjects(g0.Id, true);
                        if (childs != null) 
                        {
                            foreach (Pullenti.Address.GarObject ch in childs) 
                            {
                                if (ch.Level != Pullenti.Address.GarLevel.Area || ch.Expired) 
                                    continue;
                                Pullenti.Address.AreaAttributes aa0 = ch.Attrs as Pullenti.Address.AreaAttributes;
                                if (aa0.Number != aa.Number) 
                                    continue;
                                if (aa0.Names[0].ToUpper().Contains(aa.Names[0].ToUpper())) 
                                    it.Gars.Add(ch);
                            }
                        }
                        if (it.Gars.Count != 1) 
                            it.Gars.Clear();
                    }
                }
                if (it.Gars.Count == 0) 
                {
                    if (RestructHelper.Restruct(this, addr, i)) 
                    {
                        regions.Clear();
                        i = -1;
                        continue;
                    }
                }
                if (((it.Gars.Count == 0 && it.Level == Pullenti.Address.AddrLevel.District && i == 1) && addr.Items[0].Gars.Count == 1 && aa.Names.Count > 0) && aa.Names[0].Length > 5) 
                {
                    string nam = aa.Names[0].Substring(0, 5);
                    List<Pullenti.Address.GarObject> chi = Pullenti.Address.AddressService.GetObjects(addr.Items[0].Gars[0].Id, true);
                    if (chi != null) 
                    {
                        foreach (Pullenti.Address.GarObject ch in chi) 
                        {
                            if (ch.Level != Pullenti.Address.GarLevel.MunicipalArea && ch.Level != Pullenti.Address.GarLevel.AdminArea) 
                                continue;
                            Pullenti.Address.AreaAttributes aaa = ch.Attrs as Pullenti.Address.AreaAttributes;
                            if (aaa.Names.Count == 0) 
                                continue;
                            if (aaa.Names[0].StartsWith(nam)) 
                            {
                                if (aa.Names[0].Contains(" ") == aaa.Names[0].Contains(" ")) 
                                    it.Gars.Add(ch);
                            }
                        }
                    }
                    it.SortGars();
                }
                if (((it.Gars.Count == 0 && i > 1 && it.Level == Pullenti.Address.AddrLevel.Street) && addr.Items[i - 1].Level == Pullenti.Address.AddrLevel.Territory && addr.Items[i - 1].Gars.Count == 0) && addr.Items[i - 2].Gars.Count == 1 && ((addr.Items[i - 2].Level == Pullenti.Address.AddrLevel.City || addr.Items[i - 2].Level == Pullenti.Address.AddrLevel.RegionCity))) 
                {
                    Pullenti.Address.AreaAttributes aa0 = addr.Items[i - 1].Attrs as Pullenti.Address.AreaAttributes;
                    if (aa0.Names.Count > 0 && aa0.Number == null && aa0.Names[0].Length > 5) 
                    {
                        List<Pullenti.Address.GarObject> chi = Pullenti.Address.AddressService.GetObjects(addr.Items[i - 2].Gars[0].Id, true);
                        string nam = aa0.Names[0].Substring(0, 5);
                        foreach (Pullenti.Address.GarObject ch in chi) 
                        {
                            if (ch.Level != Pullenti.Address.GarLevel.Area) 
                                continue;
                            Pullenti.Address.AreaAttributes aaa = ch.Attrs as Pullenti.Address.AreaAttributes;
                            if (aaa.Names.Count == 0) 
                                continue;
                            if (aaa.Names[0].StartsWith(nam)) 
                            {
                                if (aa0.Names[0].Contains(" ") == aaa.Names[0].Contains(" ")) 
                                    addr.Items[i - 1].Gars.Add(ch);
                            }
                        }
                        if (addr.Items[i - 1].Gars.Count == 1) 
                        {
                            i--;
                            continue;
                        }
                        addr.Items[i - 1].Gars.Clear();
                    }
                }
                if (it.Level == Pullenti.Address.AddrLevel.District && it.Gars.Count > 0) 
                {
                    bool allArea = true;
                    foreach (Pullenti.Address.GarObject g in it.Gars) 
                    {
                        if (g.Level != Pullenti.Address.GarLevel.Area && g.Level != Pullenti.Address.GarLevel.District) 
                            allArea = false;
                    }
                    if (allArea) 
                    {
                        it.Level = Pullenti.Address.AddrLevel.Territory;
                        if (((i + 1) < addr.Items.Count) && addr.Items[i + 1].Level == Pullenti.Address.AddrLevel.City) 
                        {
                            addr.Items.RemoveAt(i);
                            addr.Items.Insert(i + 1, it);
                            it.Gars.Clear();
                            i--;
                            continue;
                        }
                    }
                }
                if (it.Level == Pullenti.Address.AddrLevel.Locality && i > 0 && it.Gars.Count == 1) 
                {
                    Pullenti.Address.AddrObject it0 = addr.Items[i - 1];
                    if (it0.Level == Pullenti.Address.AddrLevel.City && it0.FindGarByIds(it.Gars[0].ParentIds) == null) 
                    {
                        addr.Items.RemoveAt(i - 1);
                        i--;
                    }
                }
                if (it.Gars.Count == 0) 
                {
                    if (it.Level == Pullenti.Address.AddrLevel.Country) 
                        otherCountry = true;
                }
                if (it.CrossObject != null) 
                {
                    r = it.CrossObject.Tag as NameAnalyzer;
                    probs = GarHelper.GarIndex.GetStringEntries(r, regions, parIds, maxCount);
                    if (probs != null) 
                        this._addGars(addr, probs, i, regions, true);
                }
            }
            for (int j = 0; j < (addr.Items.Count - 1); j++) 
            {
                Pullenti.Address.AddrObject it0 = addr.Items[j];
                Pullenti.Address.AddrObject it1 = addr.Items[j + 1];
                if (it0.Gars.Count > 0 || it1.Gars.Count == 0) 
                    continue;
                bool ok = false;
                if (it0.Level == Pullenti.Address.AddrLevel.Locality && it1.Level == Pullenti.Address.AddrLevel.Locality) 
                    ok = true;
                if (!ok) 
                    continue;
                parIds.Clear();
                foreach (Pullenti.Address.GarObject gg in it1.Gars) 
                {
                    parIds.Add(_getId(gg.Id));
                }
                List<Pullenti.Address.Internal.Gar.AreaTreeObject> probs = GarHelper.GarIndex.GetStringEntries(it0.Tag as NameAnalyzer, regions, parIds, 4);
                if (probs != null) 
                {
                    addr.Items[j] = it1;
                    addr.Items[j + 1] = it0;
                    this._addGars(addr, probs, j + 1, regions, false);
                }
            }
            this._removeItems(addr);
            this._removeGars(addr);
            for (int j = 0; j < addr.Items.Count; j++) 
            {
                Pullenti.Address.AddrObject it = addr.Items[j];
                if (it.Gars.Count > 1) 
                {
                    foreach (Pullenti.Address.GarObject g in it.Gars) 
                    {
                        Pullenti.Address.GarObject gg = it.FindGarByIds(g.ParentIds);
                        if (gg != null) 
                        {
                            if (Pullenti.Address.AddressHelper.CanBeEqualLevels(it.Level, gg.Level)) 
                                it.Gars.Remove(g);
                            else 
                                it.Gars.Remove(gg);
                            break;
                        }
                    }
                }
            }
            for (int j = 0; j < addr.Items.Count; j++) 
            {
                Pullenti.Address.AddrObject it = addr.Items[j];
                if (((it.Level == Pullenti.Address.AddrLevel.City || it.Level == Pullenti.Address.AddrLevel.Locality)) && it.Gars.Count > 0 && !it.Gars[0].Expired) 
                {
                }
                else 
                    continue;
                int j0 = j;
                j--;
                bool hasOk = false;
                for (int jj = j; jj > 0; jj--) 
                {
                    it = addr.Items[jj];
                    if (((it.Gars.Count > 0 && it.Gars[0].Expired)) || it.Gars.Count == 0) 
                    {
                    }
                    else 
                        hasOk = true;
                }
                if (hasOk || it.Level == Pullenti.Address.AddrLevel.City) 
                {
                    for (; j > 0; j--) 
                    {
                        it = addr.Items[j];
                        if (it.Gars.Count > 0 && it.Gars[0].Expired) 
                            addr.Items.RemoveAt(j);
                        else if (it.Gars.Count == 0) 
                        {
                            if (it.Level == Pullenti.Address.AddrLevel.District) 
                            {
                                it.Level = Pullenti.Address.AddrLevel.CityDistrict;
                                if ((j0 + 1) <= addr.Items.Count) 
                                    addr.Items.Insert(j0 + 1, it);
                                else 
                                    addr.Items.Add(it);
                            }
                            addr.Items.RemoveAt(j);
                        }
                    }
                }
                break;
            }
            this._addMissItems(addr);
            for (int k = 0; k < (addr.Items.Count - 1); k++) 
            {
                for (int j = 0; j < (addr.Items.Count - 1); j++) 
                {
                    if (Pullenti.Address.AddressHelper.CompareLevels(addr.Items[j].Level, addr.Items[j + 1].Level) > 0) 
                    {
                        Pullenti.Address.AddrObject it = addr.Items[j];
                        addr.Items[j] = addr.Items[j + 1];
                        addr.Items[j + 1] = it;
                    }
                }
            }
            for (int k = 0; k < (addr.Items.Count - 1); k++) 
            {
                if (addr.Items[k].Level == addr.Items[k + 1].Level && addr.Items[k].Level != Pullenti.Address.AddrLevel.Territory) 
                {
                    Pullenti.Address.AddrObject it = addr.Items[k];
                    Pullenti.Address.AddrObject it1 = addr.Items[k + 1];
                    if (it.Gars.Count == it1.Gars.Count && it.Gars.Count > 0 && it.Gars.Contains(it1.Gars[0])) 
                    {
                        addr.Items.RemoveAt(k + 1);
                        k--;
                    }
                    else if (it.Gars.Count == 0 && it1.Gars.Count > 0) 
                    {
                        addr.Items.RemoveAt(k);
                        k--;
                    }
                    else if (it.Gars.Count > 0 && it1.Gars.Count == 0) 
                    {
                        addr.Items.RemoveAt(k + 1);
                        k--;
                    }
                }
            }
            if (uaCountry != null && ((addr.Items.Count == 0 || addr.Items[0].Level != Pullenti.Address.AddrLevel.Country))) 
                addr.Items.Insert(0, uaCountry);
            return ar;
        }
        public void _processRest(Pullenti.Address.TextAddress addr, Pullenti.Ner.Address.AddressReferent ar, bool one, Pullenti.Ner.AnalysisResult aar)
        {
            if (ar != null) 
            {
                HouseRoomHelper.ProcessHouseAndRooms(this, ar, addr);
                bool hasDetails = false;
                foreach (Pullenti.Address.AddrObject it in addr.Items) 
                {
                    if (it.DetailTyp != Pullenti.Address.DetailType.Undefined) 
                        hasDetails = true;
                }
                if (!hasDetails) 
                {
                    string par = null;
                    Pullenti.Address.DetailType det = HouseRoomHelper.CreateDirDetails(ar, out par);
                    if (det != Pullenti.Address.DetailType.Undefined && addr.LastItem != null) 
                    {
                        Pullenti.Address.AddrObject ao = addr.LastItem;
                        if (addr.Items.Count > 1 && ((addr.Items[addr.Items.Count - 2].Level == addr.LastItem.Level || ((addr.LastItem.Level == Pullenti.Address.AddrLevel.Plot && (addr.LastItem.Attrs as Pullenti.Address.HouseAttributes).Number == "б/н"))))) 
                        {
                            if (par == "часть" && addr.Items.Count > 2 && addr.Items[addr.Items.Count - 3].Level == Pullenti.Address.AddrLevel.Territory) 
                                ao = addr.Items[addr.Items.Count - 3];
                            else 
                                ao = addr.Items[addr.Items.Count - 2];
                        }
                        ao.DetailTyp = det;
                        ao.DetailParam = par;
                    }
                }
                else 
                    for (int j = 0; j < (addr.Items.Count - 1); j++) 
                    {
                        Pullenti.Address.AddrObject it = addr.Items[j];
                        if (it.DetailTyp == Pullenti.Address.DetailType.Undefined || it.Gars.Count == 0) 
                            continue;
                        Pullenti.Address.AddrObject it2 = addr.Items[j + 1];
                        if (it2.Gars.Count == 0) 
                            continue;
                        foreach (Pullenti.Address.GarObject g in it2.Gars) 
                        {
                            if (it.FindGarByIds(g.ParentIds) != null) 
                            {
                                it.DetailTyp = Pullenti.Address.DetailType.Undefined;
                                it.DetailParam = null;
                                break;
                            }
                        }
                    }
                HouseRoomHelper.ProcessOtherDetails(addr, ar);
                ar.Tag = addr;
            }
            else if (addr.Text != null) 
            {
                for (int i = addr.EndChar + 1; i < addr.Text.Length; i++) 
                {
                    char ch = addr.Text[i];
                    if (ch == ' ' || ch == ',' || ch == '.') 
                        continue;
                    string txt = addr.Text.Substring(i);
                    Pullenti.Ner.ReferentToken rt = Pullenti.Ner.Address.Internal.AddressItemToken.CreateAddress(txt);
                    if (rt == null && char.IsDigit(txt[0])) 
                        rt = Pullenti.Ner.Address.Internal.AddressItemToken.CreateAddress("дом " + txt);
                    if (rt != null) 
                    {
                        ar = rt.Referent as Pullenti.Ner.Address.AddressReferent;
                        HouseRoomHelper.ProcessHouseAndRooms(this, ar, addr);
                        addr.EndChar = i + rt.EndChar;
                    }
                    break;
                }
            }
            if (addr.LastItem != null) 
            {
                if (Pullenti.Address.AddressHelper.CompareLevels(addr.LastItem.Level, Pullenti.Address.AddrLevel.Street) > 0) 
                {
                    if (this._removeGars(addr)) 
                        this._addMissItems(addr);
                    if (one) 
                        HouseRoomHelper.TryParseListItems(this, addr, aar);
                }
            }
            _correctLevels(addr);
        }
        void _removeItems(Pullenti.Address.TextAddress res)
        {
            for (int j = 0; j < (res.Items.Count - 1); j++) 
            {
                Pullenti.Address.AddrObject it = res.Items[j];
                Pullenti.Address.AddrObject it1 = res.Items[j + 1];
                if (it1.Gars.Count == 0) 
                    continue;
                Pullenti.Address.AreaAttributes aa = it.Attrs as Pullenti.Address.AreaAttributes;
                Pullenti.Address.AreaAttributes aa1 = it1.Attrs as Pullenti.Address.AreaAttributes;
                bool ok = false;
                foreach (Pullenti.Address.GarObject g in it1.Gars) 
                {
                    if (it.FindGarByIds(g.ParentIds) != null) 
                        ok = true;
                }
                if (ok) 
                    continue;
                if (it.Level == Pullenti.Address.AddrLevel.District && it1.Level == Pullenti.Address.AddrLevel.City) 
                {
                    if (aa.Names.Count > 0 && aa1.Names.Count > 0 && aa1.Names[0].Length > 3) 
                    {
                        if (aa.Names[0].StartsWith(aa1.Names[0].Substring(0, 3))) 
                            ok = true;
                    }
                    if (!ok && ((j + 2) < res.Items.Count)) 
                    {
                        Pullenti.Address.AddrObject it2 = res.Items[j + 2];
                        if (it2.Level == Pullenti.Address.AddrLevel.Locality || it2.Level == Pullenti.Address.AddrLevel.City || it2.Level == Pullenti.Address.AddrLevel.Settlement) 
                        {
                            foreach (Pullenti.Address.GarObject g in it2.Gars) 
                            {
                                if (it.FindGarByIds(g.ParentIds) != null) 
                                    ok = true;
                            }
                            if (ok) 
                            {
                                res.Items.RemoveAt(j + 1);
                                it1 = it2;
                            }
                        }
                    }
                    if (j == 0 && it1.Gars.Count == 1) 
                    {
                        res.Items.RemoveAt(0);
                        j--;
                        continue;
                    }
                }
                if ((!ok && it.Level == Pullenti.Address.AddrLevel.City && ((it1.Level == Pullenti.Address.AddrLevel.Locality || it1.Level == Pullenti.Address.AddrLevel.Territory))) && j > 0) 
                {
                    Pullenti.Address.AddrObject it0 = res.Items[j - 1];
                    Pullenti.Address.AreaAttributes aa0 = it0.Attrs as Pullenti.Address.AreaAttributes;
                    if (it0.Level == Pullenti.Address.AddrLevel.District) 
                    {
                        foreach (Pullenti.Address.GarObject g in it1.Gars) 
                        {
                            if (it0.FindGarByIds(g.ParentIds) != null) 
                                ok = true;
                        }
                        if (ok) 
                        {
                            res.Items.RemoveAt(j);
                            j--;
                            continue;
                        }
                    }
                }
            }
        }
        void _addMissItems(Pullenti.Address.TextAddress addr)
        {
            for (int j = 0; j < (addr.Items.Count - 1); j++) 
            {
                Pullenti.Address.AddrObject it0 = addr.Items[j];
                Pullenti.Address.AddrObject it1 = addr.Items[j + 1];
                if (it1.Gars.Count == 0) 
                    continue;
                if (_containsOneOfParent(addr, it1.Gars)) 
                {
                    if (((it0.Level == Pullenti.Address.AddrLevel.RegionCity || it0.Level == Pullenti.Address.AddrLevel.RegionArea)) && it1.Level == Pullenti.Address.AddrLevel.Locality) 
                    {
                    }
                    else 
                        continue;
                }
                Pullenti.Address.GarObject par = this._getCommonParent(addr, it1.Gars);
                if (par == null) 
                    continue;
                if (addr.FindItemByGarLevel(par.Level) != null) 
                    continue;
                Pullenti.Address.GarObject par2 = null;
                Pullenti.Address.GarObject par3 = null;
                if (addr.FindGarByIds(par.ParentIds) != null) 
                {
                }
                else 
                {
                    if (par.ParentIds.Count == 0) 
                        continue;
                    par2 = this.GetGarObject(par.ParentIds[0]);
                    if (par2 == null) 
                        continue;
                    if (addr.FindGarByIds(par2.ParentIds) != null) 
                    {
                    }
                    else 
                    {
                        if (par2.ParentIds.Count == 0) 
                            continue;
                        par3 = this.GetGarObject(par2.ParentIds[0]);
                        if (par3 == null) 
                            continue;
                        if (addr.FindGarByIds(par3.ParentIds) != null) 
                        {
                        }
                        else 
                            continue;
                    }
                }
                Pullenti.Address.AddrObject to1 = GarHelper.CreateAddrObject(par);
                if (to1 != null) 
                {
                    Pullenti.Address.AddrObject exi = addr.FindItemByLevel(to1.Level);
                    if (exi == null) 
                        addr.Items.Insert(j + 1, to1);
                    else if (!exi.Gars.Contains(par)) 
                        exi.Gars.Add(par);
                }
                if (par2 != null) 
                {
                    Pullenti.Address.AddrObject to2 = GarHelper.CreateAddrObject(par2);
                    if (to2 != null) 
                    {
                        Pullenti.Address.AddrObject exi = addr.FindItemByLevel(to2.Level);
                        if (exi == null) 
                            addr.Items.Insert(j + 1, to2);
                        else if (!exi.Gars.Contains(par2)) 
                            exi.Gars.Add(par2);
                    }
                    if (par3 != null) 
                    {
                        Pullenti.Address.AddrObject to3 = GarHelper.CreateAddrObject(par3);
                        if (to3 != null) 
                        {
                            Pullenti.Address.AddrObject exi = addr.FindItemByLevel(to3.Level);
                            if (exi == null) 
                                addr.Items.Insert(j + 1, to3);
                            else if (!exi.Gars.Contains(par2)) 
                                exi.Gars.Add(par3);
                        }
                    }
                }
            }
            if (addr.Items.Count > 0 && addr.Items[0].Gars.Count >= 1 && addr.Items[0].Gars[0].ParentIds.Count > 0) 
            {
                for (Pullenti.Address.GarObject p = this.GetGarObject(addr.Items[0].Gars[0].ParentIds[0]); p != null; p = this.GetGarObject(p.ParentIds[0])) 
                {
                    Pullenti.Address.AddrObject to1 = GarHelper.CreateAddrObject(p);
                    if (to1 != null) 
                        addr.Items.Insert(0, to1);
                    if (p.ParentIds.Count == 0) 
                        break;
                }
            }
        }
        static bool _containsOneOfParent(Pullenti.Address.TextAddress a, List<Pullenti.Address.GarObject> gos)
        {
            foreach (Pullenti.Address.GarObject g in gos) 
            {
                if (a.FindGarByIds(g.ParentIds) != null) 
                    return true;
            }
            return false;
        }
        Pullenti.Address.GarObject _getCommonParent(Pullenti.Address.TextAddress a, List<Pullenti.Address.GarObject> gos)
        {
            string id = null;
            foreach (Pullenti.Address.GarObject g in gos) 
            {
                if (g.ParentIds.Count > 0) 
                {
                    if (id == null || g.ParentIds.Contains(id)) 
                        id = (g.ParentIds.Count > 0 ? g.ParentIds[0] : null);
                    else if (id != null && g.ParentIds.Contains(id)) 
                    {
                    }
                    else 
                        return null;
                }
            }
            if (id == null) 
                return null;
            return this.GetGarObject(id);
        }
        void _addGars(Pullenti.Address.TextAddress addr, List<Pullenti.Address.Internal.Gar.AreaTreeObject> probs, int i, List<byte> regions, bool cross)
        {
            if (probs == null || probs.Count == 0) 
                return;
            Pullenti.Address.AddrObject it = addr.Items[i];
            if (cross) 
                it = it.CrossObject;
            it.Gars.Clear();
            Pullenti.Address.AreaAttributes aa = it.Attrs as Pullenti.Address.AreaAttributes;
            if (it.Level == Pullenti.Address.AddrLevel.Locality) 
            {
                bool hasStreet = false;
                for (int j = i + 1; j < addr.Items.Count; j++) 
                {
                    if (addr.Items[j].Level == Pullenti.Address.AddrLevel.Street) 
                        hasStreet = true;
                }
                if (hasStreet) 
                {
                    for (int j = probs.Count - 1; j >= 0; j--) 
                    {
                        if (probs[j].Level == Pullenti.Address.AddrLevel.Street) 
                            probs.RemoveAt(j);
                    }
                }
            }
            if (probs.Count > 1 && m_Params != null && m_Params.DefaultRegions.Count > 0) 
            {
                int hasReg = 0;
                foreach (Pullenti.Address.Internal.Gar.AreaTreeObject g in probs) 
                {
                    if (m_Params.DefaultRegions.IndexOf(g.Region) >= 0) 
                        hasReg++;
                }
                if (hasReg > 0 && (hasReg < probs.Count)) 
                {
                    for (int k = probs.Count - 1; k >= 0; k--) 
                    {
                        if (m_Params.DefaultRegions.IndexOf(probs[k].Region) < 0) 
                            probs.RemoveAt(k);
                    }
                }
            }
            if (probs.Count > 1 && aa.Miscs.Count > 0 && it.Level != Pullenti.Address.AddrLevel.Territory) 
            {
                int hasEquMisc = 0;
                foreach (Pullenti.Address.Internal.Gar.AreaTreeObject g in probs) 
                {
                    if (aa.FindMisc(g.Miscs) != null) 
                        hasEquMisc++;
                }
                if (hasEquMisc > 0 && (hasEquMisc < probs.Count)) 
                {
                    for (int k = probs.Count - 1; k >= 0; k--) 
                    {
                        if (aa.FindMisc(probs[k].Miscs) == null) 
                            probs.RemoveAt(k);
                    }
                }
            }
            if (((probs.Count > 1 && it.Level != Pullenti.Address.AddrLevel.Territory && it.Level != Pullenti.Address.AddrLevel.District) && !aa.Types.Contains("населенный пункт") && !aa.Types.Contains("станция")) && !aa.Types.Contains("поселение")) 
            {
                int hasEquType = 0;
                foreach (Pullenti.Address.Internal.Gar.AreaTreeObject g in probs) 
                {
                    if (aa.HasEqualType(g.Typs)) 
                        hasEquType++;
                }
                if (hasEquType > 0 && (hasEquType < probs.Count)) 
                {
                    for (int k = probs.Count - 1; k >= 0; k--) 
                    {
                        if (!aa.HasEqualType(probs[k].Typs)) 
                            probs.RemoveAt(k);
                    }
                }
            }
            if (probs.Count > 1 && it.Level != Pullenti.Address.AddrLevel.Undefined) 
            {
                int hasEquLevel = 0;
                int gstat2 = 0;
                foreach (Pullenti.Address.Internal.Gar.AreaTreeObject g in probs) 
                {
                    if (it.Level == g.Level) 
                        hasEquLevel++;
                    if (g.Status == Pullenti.Address.GarStatus.Ok2) 
                        gstat2++;
                }
                if (gstat2 == 0 && hasEquLevel > 0 && (hasEquLevel < probs.Count)) 
                {
                    for (int k = probs.Count - 1; k >= 0; k--) 
                    {
                        if (it.Level != probs[k].Level) 
                            probs.RemoveAt(k);
                    }
                }
            }
            if (probs.Count > 1) 
            {
                int hasErr = 0;
                foreach (Pullenti.Address.Internal.Gar.AreaTreeObject g in probs) 
                {
                    if (g.Status == Pullenti.Address.GarStatus.Error) 
                        hasErr++;
                }
                if (hasErr > 0 && (hasErr < probs.Count)) 
                {
                    for (int k = probs.Count - 1; k >= 0; k--) 
                    {
                        if (probs[k].Status == Pullenti.Address.GarStatus.Error) 
                            probs.RemoveAt(k);
                    }
                }
            }
            if (probs.Count > 1) 
            {
                int hasAct = 0;
                int oktyp = 0;
                List<int> pars = new List<int>();
                foreach (Pullenti.Address.Internal.Gar.AreaTreeObject g in probs) 
                {
                    if (g.Level == Pullenti.Address.AddrLevel.District || g.CheckType(it.Tag as NameAnalyzer) > 0) 
                        oktyp++;
                    if (g.Expired) 
                        hasAct++;
                    if (g.ParentIds != null) 
                    {
                        foreach (int p in g.ParentIds) 
                        {
                            if (!pars.Contains(p)) 
                                pars.Add(p);
                        }
                    }
                }
                if (hasAct > 0 && (hasAct < oktyp) && (pars.Count < 2)) 
                {
                    for (int k = probs.Count - 1; k >= 0; k--) 
                    {
                        if (probs[k].Expired) 
                            probs.RemoveAt(k);
                    }
                }
            }
            if (i > 0 && probs.Count > 1) 
            {
                Pullenti.Address.AddrObject it0 = addr.Items[i - 1];
                if ((it.Level == Pullenti.Address.AddrLevel.Street || it.Level == Pullenti.Address.AddrLevel.Territory || ((it.Level == Pullenti.Address.AddrLevel.Locality && it0.Level == Pullenti.Address.AddrLevel.District)))) 
                {
                    int hasDirParent = 0;
                    foreach (Pullenti.Address.Internal.Gar.AreaTreeObject g in probs) 
                    {
                        if (_findParentProb(it0, g) != null && !g.Expired) 
                            hasDirParent++;
                    }
                    if (hasDirParent > 0 && (hasDirParent < probs.Count)) 
                    {
                        for (int k = probs.Count - 1; k >= 0; k--) 
                        {
                            Pullenti.Address.Internal.Gar.AreaTreeObject g = probs[k];
                            if (_findParentProb(it0, g) != null) 
                                continue;
                            probs.RemoveAt(k);
                        }
                    }
                }
            }
            if (i > 0 && probs.Count > 1) 
            {
                Pullenti.Address.AddrObject it0 = addr.Items[i - 1];
                Pullenti.Address.AreaAttributes aa0 = it0.Attrs as Pullenti.Address.AreaAttributes;
                if (aa0.Names.Count > 0 && ((it.Level == Pullenti.Address.AddrLevel.Locality || it.Level == Pullenti.Address.AddrLevel.Territory)) && it0.Level == Pullenti.Address.AddrLevel.District) 
                {
                    List<Pullenti.Address.Internal.Gar.AreaTreeObject> probs0 = null;
                    foreach (Pullenti.Address.Internal.Gar.AreaTreeObject p in probs) 
                    {
                        if (p.ParentIds == null || p.ParentIds.Count == 0) 
                            continue;
                        Pullenti.Address.GarObject par = this.GetGarObject(string.Format("a{0}", p.ParentIds[0]));
                        if (par == null) 
                            continue;
                        for (int kk = 0; kk < 2; kk++) 
                        {
                            Pullenti.Address.AreaAttributes aa1 = par.Attrs as Pullenti.Address.AreaAttributes;
                            if (aa1.Names.Count > 0 && aa1.Names[0].Length >= 4) 
                            {
                                if (aa0.Names[0].StartsWith(aa1.Names[0].Substring(0, 4))) 
                                {
                                    if (probs0 == null) 
                                        probs0 = new List<Pullenti.Address.Internal.Gar.AreaTreeObject>();
                                    probs0.Add(p);
                                    break;
                                }
                            }
                            if (kk > 0) 
                                break;
                            if (par.ParentIds == null || par.ParentIds.Count == 0) 
                                break;
                            Pullenti.Address.GarObject par2 = this.GetGarObject(par.ParentIds[0]);
                            if (par2 == null) 
                                break;
                            par = par2;
                        }
                    }
                    if (probs0 != null) 
                    {
                        probs.Clear();
                        probs.AddRange(probs0);
                    }
                }
            }
            if ((probs.Count > 1 && it.Level == Pullenti.Address.AddrLevel.Street && aa.Types.Count > 1) && aa.Types.Contains("улица")) 
            {
                string typ0 = (aa.Types[0] == "улица" ? aa.Types[1] : aa.Types[0]);
                int hasTyp = 0;
                foreach (Pullenti.Address.Internal.Gar.AreaTreeObject p in probs) 
                {
                    if (p.Typs.Contains(typ0)) 
                        hasTyp++;
                }
                if (hasTyp > 0 && (hasTyp < probs.Count)) 
                {
                    for (int k = probs.Count - 1; k >= 0; k--) 
                    {
                        if (!probs[k].Typs.Contains(typ0)) 
                            probs.RemoveAt(k);
                    }
                }
            }
            if (probs.Count > 1 && it.Level == Pullenti.Address.AddrLevel.Street && aa.Types.Count > 0) 
            {
                int hasTyp = 0;
                foreach (Pullenti.Address.Internal.Gar.AreaTreeObject p in probs) 
                {
                    if (p.Typs != null && p.Typs.Count == aa.Types.Count) 
                        hasTyp++;
                }
                if (hasTyp > 0 && (hasTyp < probs.Count)) 
                {
                    for (int k = probs.Count - 1; k >= 0; k--) 
                    {
                        if (probs[k].Typs != null && probs[k].Typs.Count != aa.Types.Count) 
                            probs.RemoveAt(k);
                    }
                }
            }
            bool ignoreGars = false;
            foreach (Pullenti.Address.Internal.Gar.AreaTreeObject p in probs) 
            {
                if (it.Level == Pullenti.Address.AddrLevel.Street && i > 0) 
                {
                    bool ok = false;
                    List<string> ids = new List<string>();
                    if (p.ParentIds != null) 
                    {
                        foreach (int id in p.ParentIds) 
                        {
                            ids.Clear();
                            ids.Add(string.Format("a{0}", id));
                            Pullenti.Address.GarObject gg = addr.FindGarByIds(ids);
                            if (gg == null) 
                                continue;
                            if (gg.Level == Pullenti.Address.GarLevel.City || gg.Level == Pullenti.Address.GarLevel.Locality || gg.Level == Pullenti.Address.GarLevel.Area) 
                            {
                                ok = true;
                                break;
                            }
                            if (((gg.Level == Pullenti.Address.GarLevel.AdminArea || gg.Level == Pullenti.Address.GarLevel.Region)) && (gg.Attrs as Pullenti.Address.AreaAttributes).Types.Contains("город")) 
                            {
                                ok = true;
                                break;
                            }
                        }
                    }
                    if (p.ParentParentIds != null && !ok && !aa.Types.Contains("километр")) 
                    {
                        foreach (int id in p.ParentParentIds) 
                        {
                            ids.Clear();
                            ids.Add(string.Format("a{0}", id));
                            Pullenti.Address.GarObject gg = addr.FindGarByIds(ids);
                            if (gg == null) 
                                continue;
                            if ((gg.Level == Pullenti.Address.GarLevel.City || gg.Level == Pullenti.Address.GarLevel.Locality || gg.Level == Pullenti.Address.GarLevel.Area) || gg.Level == Pullenti.Address.GarLevel.Settlement) 
                            {
                                ok = true;
                                break;
                            }
                            if (((gg.Level == Pullenti.Address.GarLevel.AdminArea || gg.Level == Pullenti.Address.GarLevel.Region)) && (gg.Attrs as Pullenti.Address.AreaAttributes).Types.Contains("город")) 
                            {
                                ok = true;
                                break;
                            }
                        }
                    }
                    if (!ok) 
                        continue;
                }
                Pullenti.Address.GarObject g = this.GetGarObject(string.Format("a{0}", p.Id));
                if (g == null) 
                    continue;
                if (p.Miscs != null && p.Miscs.Count > 0) 
                    (g.Attrs as Pullenti.Address.AreaAttributes).Miscs.AddRange(p.Miscs);
                Pullenti.Address.AreaAttributes ga = g.Attrs as Pullenti.Address.AreaAttributes;
                NameAnalyzer na = new NameAnalyzer();
                na.Process(ga.Names, (ga.Types.Count > 0 ? ga.Types[0] : null));
                int co = na.CalcEqualCoef(it.Tag as NameAnalyzer);
                if (co < 0) 
                    continue;
                if (((it.Level == Pullenti.Address.AddrLevel.Locality || it.Level == Pullenti.Address.AddrLevel.Territory)) && i >= 2) 
                {
                    bool ok = false;
                    if (addr.FindGarByIds(g.ParentIds) != null) 
                        ok = true;
                    else 
                        for (int kk = i - 1; kk > 0; kk--) 
                        {
                            Pullenti.Address.AddrObject it0 = addr.Items[kk];
                            if (p.ParentParentIds != null) 
                            {
                                foreach (int ppid in p.ParentParentIds) 
                                {
                                    if (it0.FindGarById(string.Format("a{0}", ppid)) != null) 
                                    {
                                        ok = true;
                                        break;
                                    }
                                }
                            }
                            if (ok) 
                                break;
                            foreach (string pid in g.ParentIds) 
                            {
                                Pullenti.Address.GarObject par = this.GetGarObject(pid);
                                if (par == null) 
                                    continue;
                                Pullenti.Address.AreaAttributes ga0 = par.Attrs as Pullenti.Address.AreaAttributes;
                                if (ga0.Names.Count == 0 || (ga0.Names[0].Length < 4)) 
                                    continue;
                                string sub = ga0.Names[0].Substring(0, 4);
                                Pullenti.Address.AreaAttributes aa0 = it0.Attrs as Pullenti.Address.AreaAttributes;
                                if (aa0.Names.Count > 0 && aa0.Names[0].StartsWith(sub, StringComparison.OrdinalIgnoreCase)) 
                                {
                                    ok = true;
                                    break;
                                }
                            }
                            if (ok) 
                                break;
                            if (p.ParentParentIds != null) 
                            {
                                foreach (int ppid in p.ParentParentIds) 
                                {
                                    Pullenti.Address.GarObject par = this.GetGarObject(string.Format("a{0}", ppid));
                                    if (par == null) 
                                        continue;
                                    Pullenti.Address.AreaAttributes ga0 = par.Attrs as Pullenti.Address.AreaAttributes;
                                    if (ga0.Names.Count == 0 || (ga0.Names[0].Length < 4)) 
                                        continue;
                                    string sub = ga0.Names[0].Substring(0, 4);
                                    Pullenti.Address.AreaAttributes aa0 = it0.Attrs as Pullenti.Address.AreaAttributes;
                                    if (aa0.Names.Count > 0 && aa0.Names[0].StartsWith(sub, StringComparison.OrdinalIgnoreCase)) 
                                    {
                                        ok = true;
                                        break;
                                    }
                                }
                            }
                            if (ok) 
                                break;
                        }
                    if (!ok) 
                        continue;
                }
                if (na.Sec != null || p.Status == Pullenti.Address.GarStatus.Ok2) 
                {
                    if (p.Id == 4001) 
                    {
                    }
                    if ((i + 1) >= addr.Items.Count || na.Sec == null) 
                        continue;
                    Pullenti.Address.AddrObject it1 = addr.Items[i + 1];
                    NameAnalyzer na1 = it1.Tag as NameAnalyzer;
                    if (na1 == null) 
                        continue;
                    if (!na1.CanBeEquals(na.Sec)) 
                        continue;
                    it1.Gars.Add(g);
                    ignoreGars = true;
                    it.Gars.Clear();
                    it.IsReconstructed = true;
                }
                if (g.Level == Pullenti.Address.GarLevel.Region && it.Level == Pullenti.Address.AddrLevel.City && i == 0) 
                    it.Level = Pullenti.Address.AddrLevel.RegionCity;
                else if (g.Level == Pullenti.Address.GarLevel.Region && it.Level != Pullenti.Address.AddrLevel.RegionCity) 
                    it.Level = Pullenti.Address.AddrLevel.RegionArea;
                if (!it.CanBeEqualsGLevel(g)) 
                {
                    if (probs.Count == 1 && it.Level == Pullenti.Address.AddrLevel.Street && g.Level == Pullenti.Address.GarLevel.Area) 
                    {
                    }
                    else 
                        continue;
                }
                if (!ignoreGars) 
                    it.Gars.Add(g);
            }
            if (i == 0 && it.Gars.Count > 1 && ((it.Level == Pullenti.Address.AddrLevel.City || it.Level == Pullenti.Address.AddrLevel.Locality))) 
            {
                bool ok = false;
                foreach (Pullenti.Address.GarObject g in it.Gars) 
                {
                    if (g.Level == Pullenti.Address.GarLevel.City) 
                    {
                        Pullenti.Address.AreaAttributes ga = g.Attrs as Pullenti.Address.AreaAttributes;
                        foreach (string n in ga.Names) 
                        {
                            if (RegionHelper.IsBigCity(n) != null) 
                                ok = true;
                        }
                        if (ok) 
                            break;
                    }
                }
                if (ok) 
                {
                    for (int k = it.Gars.Count - 1; k >= 0; k--) 
                    {
                        Pullenti.Address.AreaAttributes ga = it.Gars[k].Attrs as Pullenti.Address.AreaAttributes;
                        ok = false;
                        if (it.Gars[k].Level == Pullenti.Address.GarLevel.City) 
                        {
                            foreach (string n in ga.Names) 
                            {
                                if (RegionHelper.IsBigCity(n) != null) 
                                    ok = true;
                            }
                        }
                        if (!ok) 
                            it.Gars.RemoveAt(k);
                        if (aa.Types.Count > 0 && aa.Types[0] == "населенный пункт") 
                        {
                            aa.Types.Clear();
                            aa.Types.Add(ga.Types[0]);
                        }
                    }
                }
            }
            if (it.Gars.Count > 1 && it.Level == Pullenti.Address.AddrLevel.City) 
            {
                Pullenti.Address.GarObject g1 = it.FindGarByLevel(Pullenti.Address.GarLevel.MunicipalArea);
                if (g1 != null && it.FindGarByLevel(Pullenti.Address.GarLevel.City) != null) 
                    it.Gars.Remove(g1);
            }
            if (it.Gars.Count > 1 && i > 0 && ((it.Level == Pullenti.Address.AddrLevel.Locality || it.Level == Pullenti.Address.AddrLevel.City || it.Level == Pullenti.Address.AddrLevel.Territory))) 
            {
                for (int j = i - 1; j >= 0; j--) 
                {
                    Pullenti.Address.AddrObject it0 = addr.Items[j];
                    if (it0.Gars.Count == 0) 
                        continue;
                    Pullenti.Address.AreaAttributes ap = it0.Gars[0].Attrs as Pullenti.Address.AreaAttributes;
                    if (ap == null || ap.Names.Count == 0) 
                        break;
                    List<Pullenti.Address.GarObject> gars = null;
                    bool eqParens = false;
                    Pullenti.Address.GarLevel lev = Pullenti.Address.GarLevel.Undefined;
                    foreach (Pullenti.Address.GarObject g in it.Gars) 
                    {
                        if (g.ParentIds.Count == 0) 
                            continue;
                        Pullenti.Address.GarObject par = this.GetGarObject(g.ParentIds[0]);
                        if (par == null) 
                            continue;
                        if (lev == Pullenti.Address.GarLevel.Undefined || par.Level == lev) 
                            lev = par.Level;
                        else 
                        {
                            gars = null;
                            break;
                        }
                        Pullenti.Address.AreaAttributes pp = par.Attrs as Pullenti.Address.AreaAttributes;
                        if (pp == null || pp.Names.Count == 0) 
                            continue;
                        if (it.Gars.Contains(par)) 
                        {
                            gars = null;
                            break;
                        }
                        string str0 = ap.Names[0];
                        string str1 = pp.Names[0];
                        int k;
                        for (k = 0; (k < str0.Length) && (k < str1.Length); k++) 
                        {
                            if (str0[k] != str1[k]) 
                                break;
                        }
                        if (k >= (str0.Length - 1) || k >= (str1.Length - 1)) 
                        {
                            if (gars == null) 
                                gars = new List<Pullenti.Address.GarObject>();
                            gars.Add(g);
                            if (it0.Gars.Contains(par)) 
                                eqParens = true;
                        }
                    }
                    if (gars != null && (gars.Count < it.Gars.Count)) 
                    {
                        it.Gars = gars;
                        if (!eqParens && j > 0) 
                            addr.Items.RemoveAt(j);
                    }
                    break;
                }
                if (it.Gars.Count > 1) 
                {
                    for (int j = i - 1; j >= 0; j--) 
                    {
                        Pullenti.Address.AddrObject it0 = addr.Items[j];
                        if (it0.Gars.Count == 0) 
                            continue;
                        List<Pullenti.Address.GarObject> gars = null;
                        foreach (Pullenti.Address.GarObject g in it.Gars) 
                        {
                            bool ok = false;
                            if (it0.FindGarByIds(g.ParentIds) != null) 
                                ok = true;
                            else 
                                foreach (string pid in g.ParentIds) 
                                {
                                    Pullenti.Address.GarObject p = this.GetGarObject(pid);
                                    if (p == null) 
                                        continue;
                                    if (it0.FindGarByIds(p.ParentIds) != null) 
                                    {
                                        ok = true;
                                        break;
                                    }
                                }
                            if (ok) 
                            {
                                if (gars == null) 
                                    gars = new List<Pullenti.Address.GarObject>();
                                gars.Add(g);
                            }
                        }
                        if (gars == null) 
                            continue;
                        if (gars.Count < it.Gars.Count) 
                            it.Gars = gars;
                        break;
                    }
                }
            }
            if (it.Gars.Count > 1 && it.Level == Pullenti.Address.AddrLevel.Street && aa.Names.Count > 0) 
            {
                int hasNam = 0;
                foreach (Pullenti.Address.GarObject g in it.Gars) 
                {
                    if ((g.Attrs as Pullenti.Address.AreaAttributes).Names.Contains(aa.Names[0]) || (g.Attrs as Pullenti.Address.AreaAttributes).Names[0].Contains(aa.Names[0])) 
                        hasNam++;
                }
                if (hasNam > 0 && (hasNam < it.Gars.Count)) 
                {
                    for (int k = it.Gars.Count - 1; k >= 0; k--) 
                    {
                        if (!(it.Gars[k].Attrs as Pullenti.Address.AreaAttributes).Names.Contains(aa.Names[0]) && !(it.Gars[k].Attrs as Pullenti.Address.AreaAttributes).Names[0].Contains(aa.Names[0])) 
                            it.Gars.RemoveAt(k);
                    }
                }
            }
            if ((i > 0 && it.Gars.Count > 1 && it.Level == Pullenti.Address.AddrLevel.Street) && addr.Items[i - 1].Level == Pullenti.Address.AddrLevel.Territory && addr.Items[i - 1].Gars.Count == 1) 
            {
                Pullenti.Address.GarObject g0 = addr.Items[i - 1].Gars[0];
                int hasNam = 0;
                foreach (Pullenti.Address.GarObject g in it.Gars) 
                {
                    if (g0.ParentIds.Contains(g.Id)) 
                        hasNam++;
                }
                if (hasNam > 0 && (hasNam < it.Gars.Count)) 
                {
                    for (int k = it.Gars.Count - 1; k >= 0; k--) 
                    {
                        if (!g0.ParentIds.Contains(it.Gars[k].Id)) 
                            it.Gars.RemoveAt(k);
                    }
                }
            }
            if ((it.Gars.Count > 1 && i > 0 && it.Level == Pullenti.Address.AddrLevel.Street) && addr.Items[i - 1].Gars.Count == 1) 
            {
                Pullenti.Address.GarObject g0 = addr.Items[i - 1].Gars[0];
                int hasNam = 0;
                foreach (Pullenti.Address.GarObject g in it.Gars) 
                {
                    if (g.ParentIds.Count == 1 && g.ParentIds[0] == g0.Id && !g.Expired) 
                        hasNam++;
                }
                if (hasNam > 0 && (hasNam < it.Gars.Count)) 
                {
                    for (int k = it.Gars.Count - 1; k >= 0; k--) 
                    {
                        if (it.Gars[k].ParentIds.Count != 1 || it.Gars[k].ParentIds[0] != g0.Id) 
                            it.Gars.RemoveAt(k);
                    }
                }
            }
            if (it.Gars.Count > 1 && i > 0 && ((it.Level == Pullenti.Address.AddrLevel.Street || aa.Types.Contains("улица")))) 
            {
                if (aa.Miscs.Count == 0) 
                {
                    int has = 0;
                    foreach (Pullenti.Address.GarObject g in it.Gars) 
                    {
                        if ((g.Attrs as Pullenti.Address.AreaAttributes).Miscs.Count > 0) 
                            has++;
                    }
                    if (has > 0 && (has < it.Gars.Count)) 
                    {
                        for (int k = it.Gars.Count - 1; k >= 0; k--) 
                        {
                            if ((it.Gars[k].Attrs as Pullenti.Address.AreaAttributes).Miscs.Count > 0) 
                                it.Gars.RemoveAt(k);
                        }
                    }
                    else if (has == it.Gars.Count && (it.Tag as NameAnalyzer).Ref != null && (it.Tag as NameAnalyzer).Ref.Occurrence.Count > 0) 
                    {
                        string txt = (it.Tag as NameAnalyzer).Ref.Occurrence[0].GetText();
                        int ii = txt.LastIndexOf(',');
                        if (ii > 0) 
                            txt = txt.Substring(ii + 1).Trim();
                        txt = txt.ToUpper();
                        List<Pullenti.Address.GarObject> gars = new List<Pullenti.Address.GarObject>();
                        foreach (Pullenti.Address.GarObject g in it.Gars) 
                        {
                            Pullenti.Address.AreaAttributes ga = g.Attrs as Pullenti.Address.AreaAttributes;
                            if (ga.Miscs.Count == 0) 
                                continue;
                            string mi = ga.Miscs[0];
                            if (txt.Contains(mi) || txt.Contains(string.Format("{0}.", mi[0]))) 
                                gars.Add(g);
                        }
                        if (gars.Count > 0 && (gars.Count < it.Gars.Count)) 
                            it.Gars = gars;
                    }
                }
                else 
                {
                    int has = 0;
                    foreach (Pullenti.Address.GarObject g in it.Gars) 
                    {
                        if ((g.Attrs as Pullenti.Address.AreaAttributes).Miscs.Contains(aa.Miscs[0])) 
                            has++;
                    }
                    if (has > 0 && (has < it.Gars.Count)) 
                    {
                        for (int k = it.Gars.Count - 1; k >= 0; k--) 
                        {
                            if (!(it.Gars[k].Attrs as Pullenti.Address.AreaAttributes).Miscs.Contains(aa.Miscs[0])) 
                                it.Gars.RemoveAt(k);
                        }
                    }
                }
                if (it.Gars.Count > 1 && aa.Types.Count > 1 && aa.Types.Contains("улица")) 
                {
                    string typ = (aa.Types[0] == "улица" ? aa.Types[1] : aa.Types[0]);
                    int has = 0;
                    foreach (Pullenti.Address.GarObject g in it.Gars) 
                    {
                        if ((g.Attrs as Pullenti.Address.AreaAttributes).Types.Contains(typ)) 
                            has++;
                    }
                    if (has > 0 && (has < it.Gars.Count)) 
                    {
                        for (int k = it.Gars.Count - 1; k >= 0; k--) 
                        {
                            if (!(it.Gars[k].Attrs as Pullenti.Address.AreaAttributes).Types.Contains(typ)) 
                                it.Gars.RemoveAt(k);
                        }
                    }
                }
            }
            if ((it.Gars.Count > 1 && i == 0 && it.Level == Pullenti.Address.AddrLevel.City) && aa.Names.Count > 0) 
            {
                List<Pullenti.Address.GarObject> gars1 = null;
                foreach (Pullenti.Address.GarObject g in it.Gars) 
                {
                    for (Pullenti.Address.GarObject gg = g; gg != null; ) 
                    {
                        if (gg.Level != Pullenti.Address.GarLevel.Region) 
                        {
                            if (gg.ParentIds == null || gg.ParentIds.Count == 0) 
                                break;
                            gg = this.GetGarObject(gg.ParentIds[0]);
                            continue;
                        }
                        Pullenti.Address.AreaAttributes aaa = gg.Attrs as Pullenti.Address.AreaAttributes;
                        if (aaa.Names.Count > 0 && aa.Names[0].Length > 3) 
                        {
                            if (aaa.Names[0].StartsWith(aa.Names[0].Substring(0, aa.Names[0].Length - 3))) 
                            {
                                if (gars1 == null) 
                                    gars1 = new List<Pullenti.Address.GarObject>();
                                gars1.Add(g);
                            }
                        }
                        break;
                    }
                }
                if (gars1 != null) 
                    it.Gars = gars1;
            }
            if (i == 0) 
            {
                foreach (Pullenti.Address.GarObject g in it.Gars) 
                {
                    if (g.RegionNumber != 0 && !regions.Contains((byte)g.RegionNumber)) 
                        regions.Add((byte)g.RegionNumber);
                }
            }
            if (it.Gars.Count > 10) 
                it.Gars.Clear();
            it.SortGars();
        }
        static Pullenti.Address.GarObject _findParentProb(Pullenti.Address.AddrObject it, Pullenti.Address.Internal.Gar.AreaTreeObject ato)
        {
            if (ato.ParentIds.Count == 0) 
                return null;
            foreach (int ii in ato.ParentIds) 
            {
                Pullenti.Address.GarObject go = it.FindGarById(string.Format("a{0}", ii));
                if (go != null) 
                    return go;
            }
            return null;
        }
        static Pullenti.Ner.Processor m_Proc0;
        static Pullenti.Ner.Processor m_Proc1;
        public static void Init()
        {
            m_Proc0 = Pullenti.Ner.ProcessorService.CreateEmptyProcessor();
            m_Proc1 = Pullenti.Ner.ProcessorService.CreateProcessor();
            foreach (Pullenti.Ner.Analyzer a in m_Proc1.Analyzers) 
            {
                if (((a.Name == "GEO" || a.Name == "ADDRESS" || a.Name == "NAMEDENTITY") || a.Name == "DATE" || a.Name == "PHONE") || a.Name == "URI") 
                {
                }
                else 
                    a.IgnoreThisAnalyzer = true;
            }
        }
        Dictionary<string, Pullenti.Address.GarObject> m_GarHash = new Dictionary<string, Pullenti.Address.GarObject>();
        Dictionary<string, Pullenti.Address.Internal.Gar.HousesInStreet> m_Houses = new Dictionary<string, Pullenti.Address.Internal.Gar.HousesInStreet>();
        Dictionary<string, Pullenti.Address.Internal.Gar.RoomsInHouse> m_Rooms = new Dictionary<string, Pullenti.Address.Internal.Gar.RoomsInHouse>();
        public int IndexReadCount = 0;
        public Pullenti.Ner.Address.Internal.AddressItemToken LiteraVariant;
        public Pullenti.Address.ProcessTextParams m_Params;
        public string CorrectedText;
        public bool CreateAltsRegime = false;
        public Pullenti.Address.GarObject GetGarObject(string id)
        {
            if (id == null) 
                return null;
            Pullenti.Address.GarObject res;
            if (m_GarHash.TryGetValue(id, out res)) 
                return res;
            res = GarHelper.GetObject(id);
            if (res == null) 
                return null;
            m_GarHash.Add(id, res);
            if (id[0] != 'a') 
                IndexReadCount++;
            return res;
        }
        public Pullenti.Address.Internal.Gar.HousesInStreet GetHousesInStreet(string id)
        {
            if (id == null) 
                return null;
            Pullenti.Address.Internal.Gar.HousesInStreet res;
            if (m_Houses.TryGetValue(id, out res)) 
                return res;
            res = GarHelper.GarIndex.GetAOHouses(_getId(id));
            if (res != null) 
                IndexReadCount++;
            m_Houses.Add(id, res);
            return res;
        }
        public Pullenti.Address.Internal.Gar.RoomsInHouse GetRoomsInObject(string id)
        {
            if (id == null) 
                return null;
            Pullenti.Address.Internal.Gar.RoomsInHouse res = null;
            if (m_Rooms.TryGetValue(id, out res)) 
                return res;
            if (id[0] == 'h') 
                res = GarHelper.GarIndex.GetRoomsInHouse(_getId(id));
            else if (id[0] == 'r') 
                res = GarHelper.GarIndex.GetRoomsInRooms(_getId(id));
            if (res != null) 
                IndexReadCount++;
            m_Rooms.Add(id, res);
            return res;
        }
        public List<Pullenti.Address.TextAddress> Analyze(string txt, Dictionary<string, Dictionary<string, string>> corr, bool oneAddr, Pullenti.Address.ProcessTextParams pars)
        {
            if (string.IsNullOrEmpty(txt)) 
                return null;
            m_Params = pars;
            Dictionary<string, string> co = null;
            if (corr != null && corr.ContainsKey("")) 
                co = corr[""];
            string secondVar = null;
            string detail = null;
            if (oneAddr) 
            {
                txt = CorrectionHelper.Correct(txt, out secondVar, out detail);
                CorrectedText = txt;
            }
            List<Pullenti.Address.TextAddress> res = this._analyze(txt, co, oneAddr);
            List<Pullenti.Address.TextAddress> res2 = (secondVar == null ? null : this._analyze(secondVar, co, oneAddr));
            if ((res != null && oneAddr && res.Count == 1) && res[0].Items.Count > 0 && Pullenti.Address.AddressHelper.CompareLevels(res[0].Items[0].Level, Pullenti.Address.AddrLevel.Territory) >= 0) 
            {
                int ii = txt.IndexOf(' ');
                if (ii > 0) 
                {
                    string txt1 = string.Format("город {0}, {1}", txt.Substring(0, ii), txt.Substring(ii + 1));
                    List<Pullenti.Address.TextAddress> res1 = this._analyze(txt1, co, oneAddr);
                    if ((res1 != null && res1.Count > 0 && res1[0].Coef > res[0].Coef) && res1[0].Coef >= 80) 
                        res = res1;
                }
            }
            if (res != null && res.Count == 1 && res[0].LastItem != null) 
            {
                if (res2 != null && res2.Count == 1 && res2[0].Coef > res[0].Coef) 
                {
                    HouseRoomHelper.TryProcessDetails(res2[0], detail);
                    return res2;
                }
                if (res[0].LastItem.Gars.Count > 0) 
                {
                    HouseRoomHelper.TryProcessDetails(res[0], detail);
                    return res;
                }
            }
            if (res2 != null && res2.Count == 1) 
            {
                if (res == null || res.Count == 0 || (res[0].Coef < res2[0].Coef)) 
                {
                    HouseRoomHelper.TryProcessDetails(res2[0], detail);
                    return res2;
                }
            }
            if (res != null && detail != null) 
            {
                foreach (Pullenti.Address.TextAddress r in res) 
                {
                    HouseRoomHelper.TryProcessDetails(r, detail);
                }
            }
            return res;
        }
        List<Pullenti.Address.TextAddress> _analyze(string txt, Dictionary<string, string> co, bool oneAddr)
        {
            if (m_Proc1 == null) 
                return new List<Pullenti.Address.TextAddress>();
            Pullenti.Ner.AnalysisResult ar;
            ar = m_Proc1.Process(new Pullenti.Ner.SourceOfAnalysis(txt) { CorrectionDict = co, DoWordCorrectionByMorph = false, UserParams = (oneAddr ? "ADDRESS" : null) }, null, null);
            return this._analyze1(ar, txt, co, oneAddr);
        }
        public List<Pullenti.Address.TextAddress> _analyze1(Pullenti.Ner.AnalysisResult ar, string txt, Dictionary<string, string> co, bool oneAddr)
        {
            List<Pullenti.Address.TextAddress> res = new List<Pullenti.Address.TextAddress>();
            if (ar == null || ar.FirstToken == null) 
                return res;
            string regAcr = null;
            Pullenti.Ner.Token acrEnd = null;
            if (((oneAddr && (ar.FirstToken is Pullenti.Ner.TextToken) && ar.FirstToken.Chars.IsLetter) && ar.FirstToken.LengthChar > 1 && (ar.FirstToken.LengthChar < 4)) && ar.FirstToken.Next != null) 
            {
                regAcr = (ar.FirstToken as Pullenti.Ner.TextToken).Term;
                acrEnd = ar.FirstToken;
            }
            else if ((((oneAddr && (ar.FirstToken is Pullenti.Ner.TextToken) && ar.FirstToken.Chars.IsLetter) && ar.FirstToken.LengthChar == 1 && ar.FirstToken.Next != null) && ar.FirstToken.Next.IsChar('.') && (ar.FirstToken.Next.Next is Pullenti.Ner.TextToken)) && ar.FirstToken.Next.Next.Chars.IsLetter && ar.FirstToken.Next.Next.LengthChar == 1) 
            {
                regAcr = (ar.FirstToken as Pullenti.Ner.TextToken).Term + (ar.FirstToken.Next.Next as Pullenti.Ner.TextToken).Term;
                acrEnd = ar.FirstToken.Next.Next;
                if (acrEnd.Next != null && acrEnd.Next.IsChar('.')) 
                    acrEnd = acrEnd.Next;
            }
            if (regAcr != null && acrEnd.Next != null) 
            {
                List<RegionInfo> regs = RegionHelper.GetRegionsByAbbr(regAcr);
                if (regs != null) 
                {
                    try 
                    {
                        Pullenti.Ner.AnalysisResult ar1 = Pullenti.Ner.ProcessorService.EmptyProcessor.Process(new Pullenti.Ner.SourceOfAnalysis(txt), null, null);
                        foreach (RegionInfo r in regs) 
                        {
                            bool ok = false;
                            for (Pullenti.Ner.Token t = ar1.FirstToken; t != null; t = t.Next) 
                            {
                                if (t.EndChar <= acrEnd.EndChar) 
                                    continue;
                                List<Pullenti.Ner.Core.TerminToken> toks = r.TermCities.TryParseAll(t, Pullenti.Ner.Core.TerminParseAttr.No);
                                if (toks != null && toks.Count == 1) 
                                {
                                    ok = true;
                                    break;
                                }
                            }
                            if (!ok) 
                                continue;
                            txt = string.Format("{0}, {1}", r.Attrs.ToString(), txt.Substring(acrEnd.Next.BeginChar));
                            ar = m_Proc1.Process(new Pullenti.Ner.SourceOfAnalysis(txt) { CorrectionDict = co, DoWordCorrectionByMorph = false, UserParams = (oneAddr ? "ADDRESS" : null) }, null, null);
                            break;
                        }
                    }
                    catch(Exception ex41) 
                    {
                    }
                }
            }
            if (ar.FirstToken.Kit.CorrectedTokens != null) 
            {
                foreach (KeyValuePair<Pullenti.Ner.Token, string> kp in ar.FirstToken.Kit.CorrectedTokens) 
                {
                    if (kp.Key is Pullenti.Ner.TextToken) 
                    {
                    }
                }
            }
            List<string> unknownNames = null;
            for (Pullenti.Ner.Token t = ar.FirstToken; t != null; t = t.Next) 
            {
                if (t is Pullenti.Ner.ReferentToken) 
                {
                    Pullenti.Ner.Referent r = t.GetReferent();
                    if (r == null) 
                        continue;
                    if (r.TypeName == "PHONE" || r.TypeName == "URI") 
                    {
                        if (res.Count > 0) 
                            res[res.Count - 1].EndChar = t.EndChar;
                        continue;
                    }
                    Pullenti.Address.TextAddress addr = new Pullenti.Address.TextAddress();
                    addr.BeginChar = t.BeginChar;
                    addr.EndChar = t.EndChar;
                    _createAddressItems(addr, r, t as Pullenti.Ner.ReferentToken, 0);
                    if (addr.Items.Count == 0) 
                        continue;
                    addr.SortItems();
                    res.Add(addr);
                    r.Tag = addr;
                    if (oneAddr && t.Next != null && t.Next.IsChar('(')) 
                    {
                        Pullenti.Ner.Core.BracketSequenceToken br = Pullenti.Ner.Core.BracketHelper.TryParse(t.Next, Pullenti.Ner.Core.BracketParseAttr.No, 100);
                        if (br != null && (br.LengthChar < 20)) 
                        {
                            t = br.EndToken;
                            addr.EndChar = t.EndChar;
                        }
                    }
                    Pullenti.Ner.Token tt = t.Next;
                    if (tt != null && tt.IsComma) 
                        tt = tt.Next;
                    if (oneAddr && (tt is Pullenti.Ner.TextToken)) 
                    {
                        Pullenti.Ner.Address.Internal.AddressItemToken ait = Pullenti.Ner.Address.Internal.AddressItemToken.TryParsePureItem(tt, null, null);
                        if ((ait != null && ait.Typ == Pullenti.Ner.Address.Internal.AddressItemType.Number && !string.IsNullOrEmpty(ait.Value)) && char.IsLetter(ait.Value[0])) 
                            ait.BuildingType = Pullenti.Ner.Address.AddressBuildingType.Liter;
                        if ((ait == null && tt.LengthChar == 1 && tt.Chars.IsAllUpper) && tt.Chars.IsLetter) 
                            ait = new Pullenti.Ner.Address.Internal.AddressItemToken(Pullenti.Ner.Address.Internal.AddressItemType.Building, tt, tt) { BuildingType = Pullenti.Ner.Address.AddressBuildingType.Liter, Value = (tt as Pullenti.Ner.TextToken).Term };
                        if (ait != null && ait.BuildingType == Pullenti.Ner.Address.AddressBuildingType.Liter) 
                        {
                            LiteraVariant = ait;
                            t = ait.EndToken;
                            addr.EndChar = t.EndChar;
                        }
                    }
                }
                else if ((t is Pullenti.Ner.TextToken) && t.LengthChar > 3 && oneAddr) 
                {
                    Pullenti.Morph.MorphClass mc = t.GetMorphClassInDictionary();
                    if ((((((((mc.IsVerb || t.IsValue("ТОВАРИЩЕСТВО", null) || t.IsValue("МУНИЦИПАЛЬНЫЙ", null)) || t.IsValue("ГОРОДСКОЙ", null) || t.IsValue("СТРАНА", null)) || t.IsValue("ПОЧТОВЫЙ", null) || t.IsValue("ОКАТО", null)) || t.IsValue("СУБЪЕКТ", null) || t.IsValue("СТОЛИЦА", null)) || t.IsValue("КОРДОН", null) || t.IsValue("КОРПУС", null)) || t.IsValue("НОМЕР", null) || t.IsValue("УЧЕТНЫЙ", null)) || t.IsValue("ЗАПИСЬ", null) || t.IsValue("ГОСУДАРСТВЕННЫЙ", null)) || t.IsValue("РЕЕСТР", null) || t.IsValue("ЛЕСНОЙ", null)) 
                    {
                    }
                    else if (t.IsValue("ИНДЕКС", null)) 
                    {
                        if (res.Count > 0) 
                        {
                            if ((t.Next is Pullenti.Ner.NumberToken) && t.Next.LengthChar > 4) 
                                t = t.Next;
                            res[res.Count - 1].EndChar = t.EndChar;
                        }
                    }
                    else 
                    {
                        if (Pullenti.Ner.Core.NumberHelper.TryParseRoman(t) != null) 
                            continue;
                        string uuu = t.GetSourceText();
                        if (uuu.StartsWith("РОС", StringComparison.OrdinalIgnoreCase) || uuu.StartsWith("ФЕДЕР", StringComparison.OrdinalIgnoreCase)) 
                        {
                        }
                        else 
                        {
                            if (unknownNames == null) 
                                unknownNames = new List<string>();
                            unknownNames.Add(uuu);
                        }
                    }
                }
            }
            if (unknownNames == null && res.Count > 0) 
                res[0].BeginChar = 0;
            for (int i = 0; i < (res.Count - 1); i++) 
            {
                if ((res[i].EndChar + 30) > res[i + 1].BeginChar) 
                {
                    if (res[i].Items.Count == 1 && res[i].Items[0].Level == Pullenti.Address.AddrLevel.Country && res[i].Items[0].ToString() == "Россия") 
                    {
                        res[i + 1].BeginChar = res[i].BeginChar;
                        res.RemoveAt(i);
                        i--;
                        continue;
                    }
                    if (res[i].LastItem.ToString() == res[i + 1].Items[0].ToString()) 
                    {
                        res[i].EndChar = res[i + 1].EndChar;
                        res[i].Items.Remove(res[i].LastItem);
                        res[i].Items.AddRange(res[i + 1].Items);
                        res.RemoveAt(i + 1);
                        i--;
                        continue;
                    }
                    string str0 = res[i].ToString();
                    string str1 = res[i + 1].ToString();
                    if (res[i].Items.Count == res[i + 1].Items.Count && str0 == str1 && res[i].LastItem.Tag == res[i + 1].LastItem.Tag) 
                    {
                        if ((res[i + 1].EndChar - res[i + 1].BeginChar) > 10) 
                            res[i].EndChar = res[i + 1].EndChar;
                        res.RemoveAt(i + 1);
                        i--;
                        continue;
                    }
                    if (str1.StartsWith(str0)) 
                    {
                        if ((res[i + 1].EndChar - res[i + 1].BeginChar) < 10) 
                        {
                            res.RemoveAt(i + 1);
                            i--;
                            continue;
                        }
                        res[i + 1].BeginChar = res[i].BeginChar;
                        res.RemoveAt(i);
                        i--;
                        continue;
                    }
                    if (str0.StartsWith(str1)) 
                    {
                        if (res[i + 1].EndChar > res[i].EndChar) 
                            res[i].EndChar = res[i + 1].EndChar;
                        res.RemoveAt(i + 1);
                        i--;
                        continue;
                    }
                    bool ok = res[i].LastItem.CanBeParentFor(res[i + 1].Items[0], null);
                    if (res[i].Items.Count == 1 && res[i].Items[0].Level == Pullenti.Address.AddrLevel.City && res[i + 1].Items[0].Level == Pullenti.Address.AddrLevel.City) 
                    {
                        ok = true;
                        res[i].Items[0].Level = Pullenti.Address.AddrLevel.RegionCity;
                    }
                    if (ok) 
                    {
                        res[i].EndChar = res[i + 1].EndChar;
                        res[i].Items.AddRange(res[i + 1].Items);
                        res.RemoveAt(i + 1);
                        i--;
                    }
                }
            }
            for (int k = 0; k < res.Count; k++) 
            {
                Pullenti.Address.TextAddress r = res[k];
                if (oneAddr) 
                    r.Text = txt;
                Pullenti.Ner.Address.AddressReferent ad = r.LastItem.Tag as Pullenti.Ner.Address.AddressReferent;
                if (ad != null) 
                    r.Items.RemoveAt(r.Items.Count - 1);
                Pullenti.Address.TextAddress r2 = r.Clone();
                Pullenti.Address.TextAddress r3 = r.Clone();
                bool hasSecVar = false;
                CreateAltsRegime = false;
                Pullenti.Ner.Address.AddressReferent ad2 = this._processAddress(r, out hasSecVar);
                this._processRest(r, ad ?? ad2, oneAddr, ar);
                CoefHelper.CalcCoef(this, r, oneAddr, txt, unknownNames);
                if (r.Coef == 100 && !hasSecVar) 
                    continue;
                if (hasSecVar) 
                {
                    CreateAltsRegime = true;
                    ad2 = this._processAddress(r2, out hasSecVar);
                    this._processRest(r2, ad ?? ad2, oneAddr, ar);
                    CoefHelper.CalcCoef(this, r2, oneAddr, txt, unknownNames);
                    if (r2.Coef > r.Coef) 
                    {
                        res[k] = r2;
                        r = r2;
                    }
                    else if ((r2.Coef == r.Coef && r2.ErrorMessage == null && r.ErrorMessage != null) && r2.LastItemWithGar != null && r.LastItemWithGar != null) 
                    {
                        if (Pullenti.Address.AddressHelper.CompareLevels(r2.LastItemWithGar.Level, r.LastItemWithGar.Level) > 0) 
                        {
                            res[k] = r2;
                            r = r2;
                        }
                    }
                }
                if (r.Coef >= 95) 
                    continue;
                if (!oneAddr) 
                    continue;
                if ((r3.Items.Count < 2) || res.Count > 1) 
                    continue;
                RegionInfo reg = RegionHelper.IsBigCityA(r3.Items[0]);
                if (reg != null && reg.Capital != null && (r3.Items[0].Attrs as Pullenti.Address.AreaAttributes).ContainsName(reg.Capital)) 
                {
                }
                else if (r3.Items.Count > 1 && r3.Items[0].Level == Pullenti.Address.AddrLevel.District && r3.Items[1].Level == Pullenti.Address.AddrLevel.City) 
                {
                    reg = RegionHelper.IsBigCityA(r3.Items[1]);
                    if (reg != null && reg.Capital != null && (r3.Items[1].Attrs as Pullenti.Address.AreaAttributes).ContainsName(reg.Capital)) 
                    {
                        Pullenti.Address.AddrObject it = r3.Items[0];
                        r3.Items.RemoveAt(0);
                        r3.Items.Insert(1, it);
                    }
                    else 
                        continue;
                }
                else 
                    continue;
                string txt1 = reg.ReplaceCapitalByRegion(txt);
                if (txt1 != null && txt != txt1) 
                {
                    List<Pullenti.Address.TextAddress> res2 = this.Analyze(txt1, null, true, m_Params);
                    if (res2 != null && res2.Count == 1 && res2[0].Coef > r.Coef) 
                        return res2;
                }
            }
            if (res.Count > 1 && oneAddr) 
            {
                if (res[0].EndChar > res[0].BeginChar) 
                    res.RemoveRange(1, res.Count - 1);
            }
            if (res.Count > 1 && oneAddr) 
            {
                res[0].Coef /= res.Count;
                string msg = string.Format("В строке выделилось {0} адрес{1}, второй: {2}. ", res.Count, (res.Count < 5 ? "а" : "ов"), res[1].ToString());
                if (res[0].ErrorMessage == null) 
                    res[0].ErrorMessage = msg;
                else 
                    res[0].ErrorMessage = string.Format("{0} {1}", res[0].ErrorMessage, msg);
            }
            foreach (Pullenti.Address.TextAddress r in res) 
            {
                CorrectionHelper.CorrectCountry(r);
            }
            return res;
        }
        public Pullenti.Address.TextAddress CreateTextAddressByReferent(Pullenti.Ner.Referent r)
        {
            Pullenti.Address.TextAddress addr = new Pullenti.Address.TextAddress();
            _createAddressItems(addr, r, null, 0);
            if (addr.Items.Count == 0) 
                return null;
            addr.SortItems();
            r.Tag = addr;
            Pullenti.Ner.Address.AddressReferent ad = addr.LastItem.Tag as Pullenti.Ner.Address.AddressReferent;
            if (ad != null) 
                addr.Items.RemoveAt(addr.Items.Count - 1);
            Pullenti.Ner.Referent r2 = r.Clone();
            Pullenti.Ner.Referent r3 = r.Clone();
            bool hasSecVar = false;
            CreateAltsRegime = false;
            Pullenti.Ner.Address.AddressReferent ad2 = this._processAddress(addr, out hasSecVar);
            this._processRest(addr, ad ?? ad2, true, null);
            CoefHelper.CalcCoef(this, addr, true, null, null);
            CorrectionHelper.CorrectCountry(addr);
            return addr;
        }
        public static void _createAddressItems(Pullenti.Address.TextAddress addr, Pullenti.Ner.Referent r, Pullenti.Ner.ReferentToken rt, int lev)
        {
            if (lev > 10 || r == null) 
                return;
            Pullenti.Ner.Geo.GeoReferent own = null;
            Pullenti.Ner.Geo.GeoReferent own2 = null;
            Pullenti.Ner.Address.StreetReferent sown = null;
            Pullenti.Ner.Address.StreetReferent sown2 = null;
            Pullenti.Ner.Address.StreetReferent sown22 = null;
            Pullenti.Address.DetailType detailTyp = Pullenti.Address.DetailType.Undefined;
            string detailParam = null;
            Pullenti.Address.AddrObject detailOrg = null;
            if (r is Pullenti.Ner.Geo.GeoReferent) 
            {
                Pullenti.Ner.Geo.GeoReferent geo = r as Pullenti.Ner.Geo.GeoReferent;
                if (geo.IsState) 
                {
                    if (geo.IsState && geo.Alpha2 != null) 
                    {
                        if (geo.Alpha2 == "RU" && lev > 0) 
                            return;
                        Pullenti.Address.AddrObject cou = CorrectionHelper.CreateCountry(geo.Alpha2, geo);
                        if (cou != null) 
                        {
                            if (addr.Items.Count > 0 && addr.Items[0].Level == Pullenti.Address.AddrLevel.Country) 
                            {
                            }
                            else 
                                addr.Items.Add(cou);
                            return;
                        }
                    }
                }
                Pullenti.Address.AreaAttributes aa = new Pullenti.Address.AreaAttributes();
                Pullenti.Address.AddrObject res = new Pullenti.Address.AddrObject(aa);
                if ((r is Pullenti.Ner.Geo.GeoReferent) && string.Compare(r.ToString(), "ДНР", true) == 0) 
                {
                    r = new Pullenti.Ner.Geo.GeoReferent();
                    r.AddSlot(Pullenti.Ner.Geo.GeoReferent.ATTR_TYPE, "республика", false, 0);
                    r.AddSlot(Pullenti.Ner.Geo.GeoReferent.ATTR_NAME, "ДОНЕЦКАЯ", false, 0);
                    res.Level = Pullenti.Address.AddrLevel.RegionArea;
                }
                else if ((r is Pullenti.Ner.Geo.GeoReferent) && string.Compare(r.ToString(), "ЛНР", true) == 0) 
                {
                    r = new Pullenti.Ner.Geo.GeoReferent();
                    r.AddSlot(Pullenti.Ner.Geo.GeoReferent.ATTR_TYPE, "республика", false, 0);
                    r.AddSlot(Pullenti.Ner.Geo.GeoReferent.ATTR_NAME, "ЛУГАНСКАЯ", false, 0);
                    res.Level = Pullenti.Address.AddrLevel.RegionArea;
                }
                if (geo.ToString() == "область Читинская") 
                {
                    geo = new Pullenti.Ner.Geo.GeoReferent();
                    geo.AddSlot(Pullenti.Ner.Geo.GeoReferent.ATTR_NAME, "ЗАБАЙКАЛЬСКИЙ", false, 0);
                    geo.AddSlot(Pullenti.Ner.Geo.GeoReferent.ATTR_TYPE, "край", false, 0);
                    r = geo;
                }
                List<string> typs = r.GetStringValues(Pullenti.Ner.Geo.GeoReferent.ATTR_TYPE);
                if (geo.Alpha2 == "UA" || geo.Alpha2 == "BY" || geo.Alpha2 == "KZ") 
                    aa.Types.Add("республика");
                else if (typs.Count > 0) 
                    aa.Types.AddRange(typs);
                _setName(aa, r, Pullenti.Ner.Geo.GeoReferent.ATTR_NAME);
                _setMisc(aa, r, Pullenti.Ner.Geo.GeoReferent.ATTR_MISC);
                aa.Number = r.GetStringValue("NUMBER");
                NameAnalyzer na = new NameAnalyzer();
                na.InitByReferent(r, false);
                res.Tag = na;
                addr.Items.Add(res);
                own = geo.Higher;
                if (res.Level == Pullenti.Address.AddrLevel.Undefined) 
                    res.Level = na.Level;
                else 
                    na.Level = res.Level;
                r.Tag = res;
                if (r.OntologyItems != null && r.OntologyItems.Count > 0) 
                {
                    if (r.OntologyItems[0].ExtId is string) 
                        res.ExtId = r.OntologyItems[0].ExtId as string;
                }
            }
            else if (r is Pullenti.Ner.Address.StreetReferent) 
            {
                sown = (r as Pullenti.Ner.Address.StreetReferent).Higher;
                Pullenti.Ner.Referent uni = NameAnalyzer.MergeObjects(sown, r);
                if (uni != null) 
                {
                    _createAddressItems(addr, uni, rt, lev + 1);
                    r.Tag = addr;
                    sown.Tag = addr;
                    return;
                }
                Pullenti.Address.AreaAttributes aa = new Pullenti.Address.AreaAttributes();
                Pullenti.Address.AddrObject res = new Pullenti.Address.AddrObject(aa);
                aa.Types.AddRange((r as Pullenti.Ner.Address.StreetReferent).Typs);
                if (aa.Types.Count > 1 && aa.Types.Contains("улица")) 
                {
                    aa.Types.Remove("улица");
                    aa.Types.Add("улица");
                }
                _setName(aa, r, Pullenti.Ner.Address.StreetReferent.ATTR_NAME);
                _setMisc(aa, r, Pullenti.Ner.Address.StreetReferent.ATTR_MISC);
                Pullenti.Ner.Address.StreetKind ki = (r as Pullenti.Ner.Address.StreetReferent).Kind;
                if (ki == Pullenti.Ner.Address.StreetKind.Road) 
                    aa.Miscs.Add("дорога");
                aa.Number = (r as Pullenti.Ner.Address.StreetReferent).Numbers;
                if ((aa.Number != null && aa.Number.EndsWith("км") && aa.Names.Count == 0) && ki != Pullenti.Ner.Address.StreetKind.Road) 
                {
                    aa.Types.Add("километр");
                    aa.Number = aa.Number.Substring(0, aa.Number.Length - 2);
                }
                NameAnalyzer na = new NameAnalyzer();
                na.InitByReferent(r, false);
                res.Tag = na;
                addr.Items.Add(res);
                own = r.GetSlotValue(Pullenti.Ner.Address.StreetReferent.ATTR_GEO) as Pullenti.Ner.Geo.GeoReferent;
                res.Level = na.Level;
                if (ki == Pullenti.Ner.Address.StreetKind.Road && res.Level == Pullenti.Address.AddrLevel.Street) 
                    res.Level = Pullenti.Address.AddrLevel.Territory;
                r.Tag = res;
            }
            else if (r is Pullenti.Ner.Address.AddressReferent) 
            {
                Pullenti.Ner.Address.AddressReferent ar = r as Pullenti.Ner.Address.AddressReferent;
                sown = ar.GetSlotValue(Pullenti.Ner.Address.AddressReferent.ATTR_STREET) as Pullenti.Ner.Address.StreetReferent;
                List<Pullenti.Ner.Referent> streets = ar.Streets;
                if (streets.Count > 1) 
                {
                    if (ar.Detail == Pullenti.Ner.Address.AddressDetailType.Cross) 
                        sown2 = streets[1] as Pullenti.Ner.Address.StreetReferent;
                    else if (sown.Typs.Contains("очередь") || (ar.Streets[1] as Pullenti.Ner.Address.StreetReferent).Typs.Contains("очередь")) 
                        sown2 = streets[1] as Pullenti.Ner.Address.StreetReferent;
                    else 
                        sown2 = streets[1] as Pullenti.Ner.Address.StreetReferent;
                }
                if (streets.Count > 2) 
                    sown22 = streets[2] as Pullenti.Ner.Address.StreetReferent;
                List<Pullenti.Ner.Geo.GeoReferent> geos = ar.Geos;
                if (geos.Count > 0) 
                {
                    own = geos[0];
                    if (geos.Count > 1) 
                        own2 = geos[1];
                }
                if (ar.Detail != Pullenti.Ner.Address.AddressDetailType.Undefined && ar.Detail != Pullenti.Ner.Address.AddressDetailType.Cross) 
                {
                    detailTyp = HouseRoomHelper.CreateDirDetails(ar, out detailParam);
                    Pullenti.Ner.Geo.GeoReferent own3 = ar.GetSlotValue(Pullenti.Ner.Address.AddressReferent.ATTR_DETAILREF) as Pullenti.Ner.Geo.GeoReferent;
                    if (own3 != null) 
                    {
                        if (own3.Higher == null) 
                            own3.Higher = own;
                        if (own == null) 
                            own = own3;
                        else if (own3.Higher == own) 
                            own = own3;
                        else if (own3.Higher != null && ((own3.Higher.Higher == null || own3.Higher.Higher == own)) && Pullenti.Ner.Geo.Internal.GeoOwnerHelper.CanBeHigher(own, own3.Higher, null, null)) 
                        {
                            own3.Higher.Higher = own;
                            if (sown != null && sown.ParentReferent == own) 
                            {
                                sown.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_GEO, own3, true, 0);
                                own = null;
                            }
                            else 
                                own = own3;
                        }
                    }
                }
                else 
                {
                    Pullenti.Ner.Org.OrganizationReferent org = ar.GetSlotValue(Pullenti.Ner.Address.AddressReferent.ATTR_DETAILREF) as Pullenti.Ner.Org.OrganizationReferent;
                    if (org != null) 
                    {
                        Pullenti.Address.AreaAttributes aa = new Pullenti.Address.AreaAttributes();
                        detailOrg = new Pullenti.Address.AddrObject(aa);
                        detailOrg.Level = Pullenti.Address.AddrLevel.Territory;
                        aa.Types.Add("территория");
                        _setName(aa, org, Pullenti.Ner.Org.OrganizationReferent.ATTR_NAME);
                        _setMisc(aa, org, Pullenti.Ner.Org.OrganizationReferent.ATTR_TYPE);
                        aa.Number = (org as Pullenti.Ner.Org.OrganizationReferent).Number;
                        NameAnalyzer na = new NameAnalyzer();
                        na.InitByReferent(org, false);
                        detailOrg.Tag = na;
                        addr.Items.Add(detailOrg);
                    }
                }
                if (ar.Block != null) 
                {
                    Pullenti.Ner.Address.StreetReferent sr = new Pullenti.Ner.Address.StreetReferent();
                    sr.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_TYPE, "блок", false, 0);
                    sr.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NUMBER, ar.Block, false, 0);
                    Pullenti.Address.AreaAttributes aa = new Pullenti.Address.AreaAttributes();
                    aa.Types.Add("блок");
                    aa.Number = ar.Block;
                    Pullenti.Address.AddrObject ao = new Pullenti.Address.AddrObject(aa) { Level = Pullenti.Address.AddrLevel.Street };
                    NameAnalyzer na = new NameAnalyzer();
                    na.InitByReferent(sr, false);
                    ao.Tag = na;
                    addr.Items.Add(ao);
                }
                Pullenti.Address.HouseAttributes ha = new Pullenti.Address.HouseAttributes();
                Pullenti.Address.AddrObject res = new Pullenti.Address.AddrObject(ha);
                res.Level = Pullenti.Address.AddrLevel.Building;
                res.Tag = ar;
                r.Tag = res;
                addr.Items.Add(res);
            }
            if (sown != null) 
            {
                Pullenti.Address.TextAddress addr1 = new Pullenti.Address.TextAddress();
                _createAddressItems(addr1, sown, null, lev + 1);
                if (addr1.Items.Count > 0) 
                {
                    if (addr1.LastItem.CanBeParentFor(addr.Items[0], null)) 
                    {
                        addr.Items.InsertRange(0, addr1.Items);
                        if (sown2 != null) 
                        {
                            Pullenti.Address.TextAddress addr2 = new Pullenti.Address.TextAddress();
                            _createAddressItems(addr2, sown2, null, lev + 1);
                            if (addr2.LastItem != null && addr2.LastItem.CanBeEqualsLevel(addr1.LastItem)) 
                            {
                                Pullenti.Address.AreaAttributes a1 = addr1.LastItem.Attrs as Pullenti.Address.AreaAttributes;
                                Pullenti.Address.AreaAttributes a2 = addr2.LastItem.Attrs as Pullenti.Address.AreaAttributes;
                                if (a1.Types.Contains("очередь") && a1.Number != null && a1.Names.Count == 0) 
                                {
                                    addr.Params.Add(Pullenti.Address.ParamType.Order, a1.Number);
                                    addr.Items[addr1.Items.Count - 1] = addr2.LastItem;
                                }
                                else if (a2.Types.Contains("очередь") && a2.Number != null && a2.Names.Count == 0) 
                                    addr.Params.Add(Pullenti.Address.ParamType.Order, a1.Number);
                                else if (addr2.LastItem.Level == Pullenti.Address.AddrLevel.Territory) 
                                {
                                    addr.Items.Insert(addr1.Items.Count, addr2.LastItem);
                                    if (sown22 != null) 
                                    {
                                        Pullenti.Address.TextAddress addr3 = new Pullenti.Address.TextAddress();
                                        _createAddressItems(addr3, sown22, null, lev + 1);
                                        if (addr3.LastItem != null && addr3.LastItem.Level == addr1.LastItem.Level) 
                                            addr.Items.Insert(addr1.Items.Count + 1, addr3.LastItem);
                                    }
                                }
                                else 
                                    addr1.LastItem.CrossObject = addr2.LastItem;
                            }
                        }
                    }
                    else if (addr1.LastItem.Level == Pullenti.Address.AddrLevel.Street && ((addr.Items[0].Level == Pullenti.Address.AddrLevel.Territory || addr.Items[0].Level == Pullenti.Address.AddrLevel.Street))) 
                        addr.Items.InsertRange(0, addr1.Items);
                }
            }
            if (own != null) 
            {
                Pullenti.Address.TextAddress addr1 = new Pullenti.Address.TextAddress();
                _createAddressItems(addr1, own, null, lev + 1);
                if (addr1.Items.Count > 0) 
                {
                    if (detailTyp != Pullenti.Address.DetailType.Undefined && sown != null) 
                    {
                        addr1.LastItem.DetailTyp = detailTyp;
                        addr1.LastItem.DetailParam = detailParam;
                    }
                    bool ins = false;
                    if (Pullenti.Address.AddressHelper.CompareLevels(addr1.LastItem.Level, addr.Items[0].Level) < 0) 
                        ins = true;
                    else if (addr1.LastItem.CanBeParentFor(addr.Items[0], null)) 
                        ins = true;
                    else if (addr1.LastItem.Level == Pullenti.Address.AddrLevel.City && ((addr.Items[0].Level == Pullenti.Address.AddrLevel.District || addr.Items[0].Level == Pullenti.Address.AddrLevel.Settlement))) 
                        ins = true;
                    else if (addr1.LastItem.Level == Pullenti.Address.AddrLevel.District && addr.Items[0].Level == Pullenti.Address.AddrLevel.Locality) 
                        ins = true;
                    if (ins) 
                    {
                        if (addr.ToString().StartsWith(addr1.ToString())) 
                        {
                        }
                        else 
                            addr.Items.InsertRange(0, addr1.Items);
                    }
                    else if (addr1.LastItem.Level == Pullenti.Address.AddrLevel.Settlement && addr.Items[0].Level == Pullenti.Address.AddrLevel.District) 
                    {
                        if (addr.ToString().StartsWith(addr1.ToString())) 
                        {
                        }
                        else 
                        {
                            Pullenti.Address.AddrObject it0 = addr.Items[0];
                            addr.Items.Clear();
                            addr.Items.AddRange(addr1.Items);
                            addr.Items.Insert(addr.Items.Count - 1, it0);
                        }
                    }
                    else if (detailTyp != Pullenti.Address.DetailType.Undefined && addr1.LastItem.DetailTyp != Pullenti.Address.DetailType.Undefined && (addr1.Items.Count < addr.Items.Count)) 
                    {
                        int i;
                        for (i = 0; i < (addr1.Items.Count - 1); i++) 
                        {
                            if (addr1.Items[i].ToString() != addr.Items[i].ToString()) 
                                break;
                        }
                        if (i == (addr1.Items.Count - 1) && (Pullenti.Address.AddressHelper.CompareLevels(addr1.Items[i].Level, addr.Items[i].Level) < 0)) 
                            addr.Items.Insert(i, addr1.Items[i]);
                    }
                }
            }
            if (addr.LastItem != null) 
            {
                Pullenti.Address.AreaAttributes aa = addr.LastItem.Attrs as Pullenti.Address.AreaAttributes;
                NameAnalyzer na = addr.LastItem.Tag as NameAnalyzer;
                if ((aa != null && aa.Names.Count > 0 && aa.Number != null) && aa.Number.EndsWith("км") && na.Sec != null) 
                {
                    Pullenti.Address.AreaAttributes aa1 = new Pullenti.Address.AreaAttributes();
                    aa1.Number = aa.Number.Substring(0, aa.Number.Length - 2);
                    aa1.Types.Add("километр");
                    Pullenti.Address.AddrObject km = new Pullenti.Address.AddrObject(aa1);
                    km.Level = Pullenti.Address.AddrLevel.Street;
                    addr.LastItem.Level = Pullenti.Address.AddrLevel.Territory;
                    km.Tag = na.Sec;
                    na.Sec = null;
                    aa.Number = null;
                    addr.Items.Add(km);
                }
            }
        }
        static void _setName(Pullenti.Address.AreaAttributes a, Pullenti.Ner.Referent r, string typ)
        {
            if (r == null) 
                return;
            List<string> names = r.GetStringValues(typ);
            if (names == null || names.Count == 0) 
                return;
            string longName = null;
            for (int i = 0; i < names.Count; i++) 
            {
                string nam = names[i];
                int ii = nam.IndexOf('-');
                if (ii > 0 && ((ii + 1) < nam.Length) && char.IsDigit(nam[ii + 1])) 
                {
                    a.Number = nam.Substring(ii + 1);
                    r.AddSlot("NUMBER", a.Number, false, 0);
                    Pullenti.Ner.Slot ss = r.FindSlot("NAME", nam, true);
                    if (ss != null) 
                        r.Slots.Remove(ss);
                    nam = nam.Substring(0, ii);
                    r.AddSlot("NAME", nam, false, 0);
                }
                if (nam == "МИКРОРАЙОН") 
                {
                    if (!a.Types.Contains(nam.ToLower())) 
                        a.Types.Add(nam.ToLower());
                    names.RemoveAt(i);
                    i--;
                    continue;
                }
                names[i] = Pullenti.Ner.Core.MiscHelper.ConvertFirstCharUpperAndOtherLower(nam);
                if (longName == null) 
                    longName = names[i];
                else if (longName.Length > names[i].Length) 
                    longName = names[i];
            }
            if (names.Count > 1 && names[0] != longName) 
            {
                names.Remove(longName);
                names.Insert(0, longName);
            }
            a.Names = names;
        }
        static void _setMisc(Pullenti.Address.AreaAttributes a, Pullenti.Ner.Referent r, string nam)
        {
            a.Miscs = r.GetStringValues(nam);
            if (a.Miscs.Count > 0) 
            {
                bool hasUp = false;
                foreach (string m in a.Miscs) 
                {
                    if (char.IsUpper(m[0])) 
                        hasUp = true;
                }
                if (hasUp) 
                {
                    for (int i = a.Miscs.Count - 1; i >= 0; i--) 
                    {
                        if (!char.IsUpper(a.Miscs[i][0])) 
                            a.Miscs.RemoveAt(i);
                    }
                }
            }
        }
    }
}