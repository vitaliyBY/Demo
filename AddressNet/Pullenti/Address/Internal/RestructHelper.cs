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
    static class RestructHelper
    {
        public static void Initialize()
        {
        }
        static Pullenti.Address.GarObject m_Moscow = null;
        static Pullenti.Address.GarObject Moscow
        {
            get
            {
                if (m_Moscow != null) 
                    return m_Moscow;
                Pullenti.Address.SearchParams sp = new Pullenti.Address.SearchParams();
                sp.City = "Москва";
                sp.Region = 77;
                Pullenti.Address.SearchResult sr = Pullenti.Address.AddressService.SearchObjects(sp);
                if (sr.Objects.Count != 1) 
                    return null;
                m_Moscow = sr.Objects[0];
                return m_Moscow;
            }
        }
        public static bool Restruct(AnalyzeHelper ah, Pullenti.Address.TextAddress addr, int i)
        {
            if (i == 0) 
                return false;
            Pullenti.Address.AddrObject it0 = addr.Items[0];
            if (it0.Gars.Count == 0) 
                return false;
            int reg = it0.Gars[0].RegionNumber;
            Pullenti.Address.AddrObject it = addr.Items[i];
            Pullenti.Address.AddrObject it1 = addr.Items[i - 1];
            if (reg == 50 && it.Level == Pullenti.Address.AddrLevel.City && (it.Attrs as Pullenti.Address.AreaAttributes).Names.Contains("Юбилейный")) 
            {
                string txt = "область Московская, городской округ Королёв, город Королёв, микрорайон Юбилейный";
                Pullenti.Address.TextAddress addr1 = Pullenti.Address.AddressService.ProcessSingleAddressText(txt, null);
                if (addr1 == null || addr1.Coef != 100) 
                    return false;
                addr.ErrorMessage = string.Format("Смена объекта: '{0}' на '{1}'. ", it.ToString(), addr1.LastItem.ToString());
                addr.Items.RemoveRange(0, i + 1);
                if (addr1.Items[0].Level == Pullenti.Address.AddrLevel.Country) 
                    addr1.Items.RemoveAt(0);
                addr.Items.InsertRange(0, addr1.Items);
                return true;
            }
            if (reg == 50 && (((it.Level == Pullenti.Address.AddrLevel.City || it.Level == Pullenti.Address.AddrLevel.Locality || it.Level == Pullenti.Address.AddrLevel.Settlement) || ((it.Level == Pullenti.Address.AddrLevel.Street && i == 1))))) 
            {
                Pullenti.Address.GarObject mos = Moscow;
                if (mos == null) 
                    return false;
                List<int> pars = new List<int>();
                pars.Add(int.Parse(mos.Id.Substring(1)));
                List<byte> regs = new List<byte>();
                regs.Add(77);
                NameAnalyzer r = it.Tag as NameAnalyzer;
                List<Pullenti.Address.Internal.Gar.AreaTreeObject> probs = GarHelper.GarIndex.GetStringEntries(r, regs, pars, 10);
                if (probs == null) 
                    return false;
                Pullenti.Address.AddrObject distr = null;
                if (it.Level != Pullenti.Address.AddrLevel.Locality) 
                {
                    if (probs.Count != 1) 
                        return false;
                }
                else 
                {
                    if (probs.Count > 10) 
                        return false;
                    bool ok = false;
                    foreach (Pullenti.Address.Internal.Gar.AreaTreeObject pr in probs) 
                    {
                        foreach (int pid in pr.ParentIds) 
                        {
                            Pullenti.Address.GarObject par = Pullenti.Address.AddressService.GetObject(string.Format("a{0}", pid));
                            if (par == null) 
                                continue;
                            Pullenti.Address.AreaAttributes paa = par.Attrs as Pullenti.Address.AreaAttributes;
                            if (paa.Names.Count == 0) 
                                continue;
                            string nam = paa.Names[0];
                            if (nam.Length < 4) 
                                continue;
                            for (int ii = 1; ii < i; ii++) 
                            {
                                Pullenti.Address.AreaAttributes xaa = addr.Items[ii].Attrs as Pullenti.Address.AreaAttributes;
                                if (xaa.Names.Count == 0) 
                                    continue;
                                if (xaa.Names[0].StartsWith(nam.Substring(0, 4))) 
                                {
                                    ok = true;
                                    break;
                                }
                                if (xaa.Names.Contains("Наро-Фоминский")) 
                                {
                                    if (nam.Contains("Маруш")) 
                                        ok = true;
                                }
                            }
                            if (ok) 
                            {
                                if (probs.Count > 1) 
                                    distr = GarHelper.CreateAddrObject(par);
                                break;
                            }
                        }
                        if (ok) 
                            break;
                    }
                    if (!ok) 
                        return false;
                }
                addr.ErrorMessage = string.Format("Смена объекта: '{0}' на '{1}'. ", it0.ToString(), mos.ToString());
                addr.Items.RemoveRange(0, i);
                addr.Items.Insert(0, GarHelper.CreateAddrObject(mos));
                if (distr != null) 
                    addr.Items.Insert(1, distr);
                return true;
            }
            if ((reg == 72 && i == 1 && it.Level == Pullenti.Address.AddrLevel.District) && (((it.Attrs as Pullenti.Address.AreaAttributes).Names.Contains("Янао") || (it.Attrs as Pullenti.Address.AreaAttributes).Names.Contains("Югра")))) 
            {
                addr.Items.RemoveAt(0);
                it.Level = Pullenti.Address.AddrLevel.RegionArea;
                return true;
            }
            if (reg == 72 && it.Level == Pullenti.Address.AddrLevel.City) 
            {
                List<byte> regs = new List<byte>();
                regs.Add(86);
                regs.Add(89);
                NameAnalyzer r = it.Tag as NameAnalyzer;
                List<Pullenti.Address.Internal.Gar.AreaTreeObject> probs = GarHelper.GarIndex.GetStringEntries(r, regs, null, 10);
                if (probs != null && probs.Count == 1) 
                {
                    Pullenti.Address.GarObject gar = Pullenti.Address.AddressService.GetObject(string.Format("a{0}", probs[0].Id));
                    if (gar == null) 
                        return false;
                    addr.ErrorMessage = string.Format("Смена объекта: '{0}' на регион {1}. ", it0.ToString(), probs[0].Region);
                    it.Gars.Add(gar);
                    addr.Items.RemoveRange(0, i);
                    return true;
                }
            }
            return false;
        }
    }
}