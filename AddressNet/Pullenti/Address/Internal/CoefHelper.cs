/*
 * SDK Pullenti Address, version 4.20, august 2023. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Pullenti.Address.Internal
{
    static class CoefHelper
    {
        public static Pullenti.Address.TextAddress CalcCoef(AnalyzeHelper ah, Pullenti.Address.TextAddress res, bool one, string txt, List<string> unknownNames)
        {
            if (one) 
                res.Text = txt;
            else if (res.EndChar > 0 && (res.EndChar < txt.Length)) 
                res.Text = txt.Substring(res.BeginChar, (res.EndChar + 1) - res.BeginChar);
            double dcoef = (double)100;
            bool hasManyGars = false;
            bool otherCountry = false;
            StringBuilder msg = null;
            if (res.ErrorMessage != null) 
            {
                if (!res.ErrorMessage.StartsWith("Смена")) 
                    dcoef = 90;
                msg = new StringBuilder();
                msg.Append(res.ErrorMessage);
            }
            for (int j = 0; j < res.Items.Count; j++) 
            {
                Pullenti.Address.AddrObject it = res.Items[j];
                bool noMsg = false;
                if (j == 0 && Pullenti.Address.AddressHelper.CompareLevels(it.Level, Pullenti.Address.AddrLevel.City) > 0) 
                {
                    dcoef /= 2;
                    noMsg = true;
                    if (msg == null) 
                        msg = new StringBuilder();
                    msg.AppendFormat("Первый объект '{0}' слишком низкого уровня ({1}). ", it.ToString(), Pullenti.Address.AddressHelper.GetAddrLevelString(it.Level));
                }
                if ((j + 1) < res.Items.Count) 
                {
                    if (!it.CanBeParentFor(res.Items[j + 1], (j > 0 ? res.Items[j - 1] : null))) 
                    {
                        Pullenti.Address.AddrObject it1 = res.Items[j + 1];
                        if (it.Level == Pullenti.Address.AddrLevel.District && it1.Level == Pullenti.Address.AddrLevel.District) 
                        {
                            if (it1.Gars.Count > 0) 
                            {
                                if (msg == null) 
                                    msg = new StringBuilder();
                                msg.AppendFormat("Объект '{0}' указан в адресе неправильно. ", it.ToString());
                                dcoef *= 0.9;
                                res.Items.RemoveAt(j);
                                j--;
                                continue;
                            }
                        }
                        if (((it.Level == Pullenti.Address.AddrLevel.City || it.Level == Pullenti.Address.AddrLevel.District)) && it1.Level == Pullenti.Address.AddrLevel.District) 
                        {
                            if (it1.Gars.Count == 0) 
                            {
                                res.Items.RemoveAt(j + 1);
                                j--;
                                continue;
                            }
                        }
                        if (it.Level == Pullenti.Address.AddrLevel.RegionArea && it1.Level == Pullenti.Address.AddrLevel.Locality && (it1.Attrs as Pullenti.Address.AreaAttributes).Types.Contains("город")) 
                            it1.Level = Pullenti.Address.AddrLevel.City;
                        else if (it.Level == Pullenti.Address.AddrLevel.Building && it1.Level == Pullenti.Address.AddrLevel.Building) 
                        {
                        }
                        else if (it1.Gars.Count <= 1) 
                        {
                            dcoef /= 2;
                            noMsg = true;
                            if (msg == null) 
                                msg = new StringBuilder();
                            msg.AppendFormat("Объект '{0}' не может быть родителем для '{1}'. ", it.ToString(), it1.ToString());
                        }
                    }
                }
                if (GarHelper.GarIndex == null) 
                    continue;
                if (it.Level == Pullenti.Address.AddrLevel.Room || it.Level == Pullenti.Address.AddrLevel.CityDistrict) 
                    continue;
                Pullenti.Address.AddrObject par = (j > 0 ? res.Items[j - 1] : null);
                if (it.Gars.Count == 0) 
                {
                    if (j == 0 && it.Level == Pullenti.Address.AddrLevel.Country) 
                    {
                        otherCountry = true;
                        continue;
                    }
                    if (it.IsReconstructed) 
                        continue;
                    if (otherCountry) 
                        continue;
                    if (((par != null && par.Gars.Count > 0 && it.Level == par.Level) && it.Level != Pullenti.Address.AddrLevel.Territory && it.Level != Pullenti.Address.AddrLevel.District) && it.Level != Pullenti.Address.AddrLevel.Building) 
                    {
                        res.Items.RemoveAt(j);
                        j--;
                        continue;
                    }
                    if (par != null) 
                    {
                        if (par.Gars.Count == 0 && par.Level != Pullenti.Address.AddrLevel.District) 
                            continue;
                        if (par.Gars.Count > 0 && par.Gars[0].Expired) 
                        {
                            if (it.Level != Pullenti.Address.AddrLevel.Street && it.Level != Pullenti.Address.AddrLevel.Territory) 
                                continue;
                        }
                        Pullenti.Address.HouseAttributes ha = it.Attrs as Pullenti.Address.HouseAttributes;
                        if (ha != null && ha.Number != null) 
                        {
                            if (ha.Number == "б/н") 
                                continue;
                        }
                    }
                    bool not = false;
                    if (Pullenti.Address.AddressHelper.CompareLevels(it.Level, Pullenti.Address.AddrLevel.Plot) >= 0) 
                    {
                        if (par != null && par.Gars.Count == 1 && par.Gars[0].ChildrenCount == 0) 
                        {
                        }
                        else if (it.Level == Pullenti.Address.AddrLevel.Apartment) 
                        {
                        }
                        else if (it.Level == Pullenti.Address.AddrLevel.Building || it.Level == Pullenti.Address.AddrLevel.Plot) 
                        {
                            not = true;
                            if (par != null && par.Gars.Count > 0) 
                            {
                                if (par.Level == Pullenti.Address.AddrLevel.RegionCity) 
                                    dcoef *= 0.9;
                            }
                        }
                        else 
                        {
                            dcoef *= 0.9;
                            not = true;
                        }
                    }
                    else if (Pullenti.Address.AddressHelper.CompareLevels(it.Level, Pullenti.Address.AddrLevel.Territory) >= 0) 
                    {
                        Pullenti.Address.AreaAttributes aa = it.Attrs as Pullenti.Address.AreaAttributes;
                        if (it.Level == Pullenti.Address.AddrLevel.Street && ((((aa.Types.Contains("блок") || aa.Types.Contains("линия") || aa.Types.Contains("ряд")) || aa.Types.Contains("очередь") || aa.Types.Contains("поле")) || aa.Types.Contains("куст")))) 
                        {
                        }
                        else if (it.Level == Pullenti.Address.AddrLevel.Territory && (((aa.Miscs.Contains("дорога") || aa.Miscs.Contains("лесничество") || aa.Miscs.Contains("месторождение")) || aa.Miscs.Contains("участок")))) 
                        {
                        }
                        else if (it.Level == Pullenti.Address.AddrLevel.Street && aa.Names.Count == 0 && aa.Number != null) 
                        {
                        }
                        else if ((par != null && ((it.Level == Pullenti.Address.AddrLevel.Territory || ((it.Level == Pullenti.Address.AddrLevel.Street && ((par.Level == Pullenti.Address.AddrLevel.Locality || par.Level == Pullenti.Address.AddrLevel.City)))))) && par.Gars.Count > 0) && par.Gars[0].Expired) 
                        {
                        }
                        else if (j > 0 && it.Level == Pullenti.Address.AddrLevel.Territory && res.Items[j - 1].DetailTyp != Pullenti.Address.DetailType.Undefined) 
                        {
                        }
                        else if (((j + 1) < res.Items.Count) && res.Items[j + 1].Level == Pullenti.Address.AddrLevel.Territory && res.Items[j + 1].Gars.Count > 0) 
                        {
                        }
                        else if (((j + 1) < res.Items.Count) && res.Items[j + 1].Level == Pullenti.Address.AddrLevel.Street && res.Items[j + 1].Gars.Count == 1) 
                        {
                        }
                        else if (it.Level == Pullenti.Address.AddrLevel.Territory && ((aa.Miscs.Contains("ВЧ") || aa.Miscs.Contains("ПЧ") || aa.Miscs.Contains("военный городок")))) 
                        {
                        }
                        else if (par != null && par.Gars.Count > 0 && ((par.Level == Pullenti.Address.AddrLevel.Territory || par.Level == Pullenti.Address.AddrLevel.Locality))) 
                        {
                            bool ok = par.Gars[0].Expired;
                            if (ok || par.Gars[0].ChildrenCount == 0) 
                                ok = true;
                            else if (((j + 1) < res.Items.Count) && res.Items[j + 1].Gars.Count == 1) 
                                ok = true;
                            else if (par.Gars.Count == 1) 
                            {
                                if (!(par.Attrs as Pullenti.Address.AreaAttributes).HasEqualType((par.Gars[0].Attrs as Pullenti.Address.AreaAttributes).Types)) 
                                {
                                }
                                else 
                                {
                                    ok = true;
                                    List<Pullenti.Address.GarObject> chis = Pullenti.Address.AddressService.GetObjects(par.Gars[0].Id, true);
                                    if (chis != null && chis.Count > 0) 
                                        ok = false;
                                }
                            }
                            if (!ok) 
                                dcoef *= 0.8;
                        }
                        else 
                            dcoef *= 0.8;
                        not = true;
                    }
                    else if (((it.Level == Pullenti.Address.AddrLevel.Locality || it.Level == Pullenti.Address.AddrLevel.Settlement)) && par != null && par.Gars.Count == 1) 
                    {
                        if (((j + 1) < res.Items.Count) && res.Items[j + 1].Gars.Count > 0 && res.Items[j].DetailTyp != Pullenti.Address.DetailType.Undefined) 
                        {
                        }
                        else if (((j + 1) < res.Items.Count) && res.Items[j + 1].Gars.Count == 1 && ((res.Items[j + 1].Level == Pullenti.Address.AddrLevel.Locality || res.Items[0].Level == Pullenti.Address.AddrLevel.RegionCity))) 
                        {
                        }
                        else if (((!ah.CreateAltsRegime && it.Level == Pullenti.Address.AddrLevel.Locality && ((j + 1) < res.Items.Count)) && res.Items[j + 1].Gars.Count == 1 && j > 0) && res.Items[j - 1].Level == Pullenti.Address.AddrLevel.City && res.Items[j - 1].Gars.Count == 1) 
                        {
                        }
                        else 
                            dcoef *= 0.7;
                        not = true;
                    }
                    else if ((j > 0 && it.Level == Pullenti.Address.AddrLevel.District && ((j + 1) < res.Items.Count)) && res.Items[j + 1].Gars.Count > 0) 
                    {
                    }
                    else if ((j > 0 && ((j + 1) < res.Items.Count) && RegionHelper.IsBigCityA(it) != null) && res.Items[j + 1].Gars.Count > 0) 
                    {
                        res.Items.RemoveAt(j);
                        j--;
                        continue;
                    }
                    else 
                    {
                        dcoef *= 0.6;
                        not = true;
                    }
                    if (not && !noMsg) 
                    {
                        if (msg == null) 
                            msg = new StringBuilder();
                        msg.AppendFormat("Объект '{0}' не привязался к ГАР. ", it.ToString());
                    }
                    continue;
                }
                if ((j + 1) < res.Items.Count) 
                {
                    Pullenti.Address.AddrObject it1 = res.Items[j + 1];
                    if (it1.Gars.Count > 0 && (it.Attrs is Pullenti.Address.AreaAttributes) && (it1.Attrs is Pullenti.Address.AreaAttributes)) 
                    {
                        Pullenti.Address.AreaAttributes aa = it.Attrs as Pullenti.Address.AreaAttributes;
                        Pullenti.Address.AreaAttributes aa1 = it1.Attrs as Pullenti.Address.AreaAttributes;
                        bool ok = false;
                        foreach (Pullenti.Address.GarObject g in it1.Gars) 
                        {
                            if (it.FindGarByIds(g.ParentIds) != null) 
                                ok = true;
                            else if (g.ParentIds.Count > 0) 
                            {
                                Pullenti.Address.GarObject pp = ah.GetGarObject(g.ParentIds[0]);
                                if (pp != null) 
                                {
                                    if (it.FindGarByIds(pp.ParentIds) != null) 
                                        ok = true;
                                }
                            }
                        }
                        if (!ok && it.Level == Pullenti.Address.AddrLevel.District && it1.Level == Pullenti.Address.AddrLevel.City) 
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
                        }
                        if (!ok && ((it.Level == Pullenti.Address.AddrLevel.District || (((it.Tag is NameAnalyzer) && (it.Tag as NameAnalyzer).Level == Pullenti.Address.AddrLevel.District)))) && ((it1.Level == Pullenti.Address.AddrLevel.Street || it1.Level == Pullenti.Address.AddrLevel.City))) 
                        {
                            res.Items.RemoveAt(j);
                            j--;
                            continue;
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
                        if (!ok && it.Level == Pullenti.Address.AddrLevel.Locality && ((it1.Level == Pullenti.Address.AddrLevel.Territory || it1.Level == Pullenti.Address.AddrLevel.Street))) 
                        {
                            foreach (Pullenti.Address.GarObject g in it1.Gars) 
                            {
                                Pullenti.Address.GarObject p = ah.GetGarObject((g.ParentIds.Count > 0 ? g.ParentIds[0] : null));
                                if (p != null && it.FindGarByIds(p.ParentIds) != null) 
                                {
                                    ok = true;
                                    break;
                                }
                            }
                        }
                        if (!ok && it.Level == Pullenti.Address.AddrLevel.Territory && it1.Level == Pullenti.Address.AddrLevel.Street) 
                            ok = true;
                        if (!ok && it.Level == Pullenti.Address.AddrLevel.Locality && it1.Level == Pullenti.Address.AddrLevel.Locality) 
                        {
                            if (it.Gars.Count > 0 && it1.FindGarById(it.Gars[0].Id) != null) 
                                ok = true;
                        }
                        if (!ok && it1.Gars.Count > 1) 
                            ok = true;
                        else if ((!ok && it1.Gars.Count == 1 && it.Gars.Count > 0) && it.Gars[0].Expired) 
                            ok = true;
                        else if (it.DetailTyp != Pullenti.Address.DetailType.Undefined) 
                            ok = true;
                        else if (it.Level == it1.Level && it.Level == Pullenti.Address.AddrLevel.Locality && res.AdditionalItems == null) 
                        {
                            int kk;
                            for (kk = j + 1; kk < res.Items.Count; kk++) 
                            {
                                if (res.Items[kk].Level != Pullenti.Address.AddrLevel.Locality) 
                                    break;
                            }
                            if (kk >= res.Items.Count) 
                            {
                                ok = true;
                                res.AdditionalItems = new List<Pullenti.Address.AddrObject>();
                                for (kk = j + 1; kk < res.Items.Count; kk++) 
                                {
                                    res.AdditionalItems.Add(res.Items[kk]);
                                }
                                res.Items.RemoveRange(j + 1, res.Items.Count - j - 1);
                            }
                        }
                        if (!ok) 
                        {
                            if ((it.Level == Pullenti.Address.AddrLevel.District && j == 1 && ((res.Items[0].Level == Pullenti.Address.AddrLevel.RegionArea || res.Items[0].Level == Pullenti.Address.AddrLevel.RegionCity))) && it1.Gars.Count > 0 && (((it1.Level == Pullenti.Address.AddrLevel.City || it1.Level == Pullenti.Address.AddrLevel.District || it1.Level == Pullenti.Address.AddrLevel.Settlement) || ((it1.Level == Pullenti.Address.AddrLevel.Locality && it1.Gars.Count == 1))))) 
                            {
                            }
                            else if (it.Level == Pullenti.Address.AddrLevel.Territory && it1.Level == Pullenti.Address.AddrLevel.Territory) 
                            {
                            }
                            else 
                                dcoef *= 0.9;
                            if (!noMsg) 
                            {
                                if (msg == null) 
                                    msg = new StringBuilder();
                                msg.AppendFormat("Похоже, объект '{0}' указан в адресе неправильно. ", it.ToString());
                            }
                        }
                    }
                }
                if (it.Level == Pullenti.Address.AddrLevel.Territory && it.Gars[0].Level == Pullenti.Address.GarLevel.Street) 
                {
                    if ((j + 1) == res.Items.Count || Pullenti.Address.AddressHelper.CompareLevels(res.Items[j + 1].Level, Pullenti.Address.AddrLevel.Street) > 0) 
                        it.Level = Pullenti.Address.AddrLevel.Street;
                }
                if (it.Gars.Count == 1 || it.Gars[1].Expired) 
                    continue;
                if (Pullenti.Address.AddressHelper.CompareLevels(it.Level, Pullenti.Address.AddrLevel.Building) >= 0) 
                    continue;
                List<string> pars = new List<string>();
                bool hasPar = false;
                foreach (string pid in it.Gars[0].ParentIds) 
                {
                    int kk;
                    for (kk = 1; kk < it.Gars.Count; kk++) 
                    {
                        if (!it.Gars[kk].ParentIds.Contains(pid)) 
                            break;
                    }
                    if (kk >= it.Gars.Count) 
                    {
                        hasPar = true;
                        break;
                    }
                }
                if (!hasPar) 
                {
                    foreach (Pullenti.Address.GarObject g in it.Gars) 
                    {
                        string id = (g.ParentIds.Count > 0 ? g.ParentIds[0] : null);
                        if (g.ParentIds.Count > 1 && j > 0 && res.Items[j - 1].FindGarById(g.ParentIds[1]) != null) 
                            id = g.ParentIds[1];
                        if (id != null && !pars.Contains(id)) 
                            pars.Add(id);
                    }
                }
                double co = (double)(1F / ((pars.Count == 0 ? 1 : pars.Count)));
                if (pars.Count > 1) 
                {
                    if (pars.Count == 2) 
                        co = 0.9;
                    else if (pars.Count == 3) 
                        co = 0.8;
                    else 
                        co = 0.7;
                    List<string> nams = new List<string>();
                    List<string> pars2 = new List<string>();
                    foreach (string p in pars) 
                    {
                        Pullenti.Address.GarObject oo = null;
                        if (par != null) 
                            oo = par.FindGarById(p);
                        if (oo == null) 
                            oo = ah.GetGarObject(p);
                        if (oo == null) 
                            continue;
                        string ss = oo.ToString().ToUpper();
                        if (ss.IndexOf('Ё') >= 0) 
                            ss = ss.Replace('Ё', 'Е');
                        if (!nams.Contains(ss)) 
                            nams.Add(ss);
                        string pp = (oo.ParentIds.Count > 0 ? oo.ParentIds[0] : "");
                        if (!pars2.Contains(pp)) 
                            pars2.Add(pp);
                        if (nams.Count > 1 || pars2.Count > 1) 
                            break;
                    }
                    if (nams.Count == 1 && pars2.Count == 1) 
                        co = 1;
                    else 
                    {
                        if (msg == null) 
                            msg = new StringBuilder();
                        msg.AppendFormat("К объекту '{0}' привязались {1} разные объекта ГАР. ", it.ToString(), it.Gars.Count);
                        if (hasManyGars) 
                            co = 1;
                        hasManyGars = true;
                    }
                }
                dcoef *= co;
            }
            if (res.AdditionalItems != null) 
            {
                List<Pullenti.Address.AddrObject> li = new List<Pullenti.Address.AddrObject>();
                foreach (Pullenti.Address.AddrObject ai in res.AdditionalItems) 
                {
                    if (ai.Gars.Count == 0) 
                        li.Add(ai);
                }
                if (li.Count > 0) 
                {
                    if (msg == null) 
                        msg = new StringBuilder();
                    if (li.Count == 1) 
                        msg.AppendFormat("Объект '{0}' не привязался к ГАР. ", li[0].ToString());
                    else 
                    {
                        msg.AppendFormat("Объекты ");
                        foreach (Pullenti.Address.AddrObject o in li) 
                        {
                            if (o != li[0]) 
                                msg.Append(", ");
                            msg.AppendFormat("'{0}'", o.ToString());
                        }
                        msg.Append(" не привязались к ГАР. ");
                    }
                }
            }
            for (int j = 0; j < (res.Items.Count - 1); j++) 
            {
                Pullenti.Address.AddrObject it = res.Items[j];
                if (it.Level != Pullenti.Address.AddrLevel.District || res.Items[j + 1].Level != Pullenti.Address.AddrLevel.City) 
                    continue;
                Pullenti.Address.AreaAttributes aa = it.Attrs as Pullenti.Address.AreaAttributes;
                bool isCityDistr = false;
                if (it.Gars.Count == 0) 
                {
                    if (aa.Types.Contains("район")) 
                        isCityDistr = true;
                }
                else 
                {
                    Pullenti.Address.AreaAttributes ga = it.Gars[0].Attrs as Pullenti.Address.AreaAttributes;
                    if (ga.Types.Count > 0 && ga.Types[0].Contains("внутригородск")) 
                        isCityDistr = true;
                }
                if (isCityDistr) 
                {
                    it.Level = Pullenti.Address.AddrLevel.CityDistrict;
                    res.Items.RemoveAt(j);
                    res.Items.Insert(j + 1, it);
                }
                break;
            }
            int totalChar = 0;
            int notChar = 0;
            int max = (txt == null ? 0 : txt.Length);
            int i;
            if (one && txt != null) 
            {
                if (res.BeginChar > 0) 
                {
                    string sub = txt.Substring(0, res.BeginChar).Trim().ToUpper();
                    if (sub.EndsWith(",")) 
                        sub = sub.Substring(0, sub.Length - 1).Trim();
                    string rest = txt.Substring(res.BeginChar);
                    if (rest.ToUpper().Contains(sub)) 
                        res.BeginChar = 0;
                    else if (res.ToString().ToUpper().Contains(sub)) 
                        res.BeginChar = 0;
                    else if (dcoef == 100) 
                        res.BeginChar = 0;
                }
                if ((((i = txt.IndexOf("дом,корпус,кв.")))) > 0) 
                    max = i;
                if ((((i = txt.IndexOf("ТП-")))) > 0 && (i < max)) 
                    max = i;
                if ((((i = txt.IndexOf("РП-")))) > 0 && (i < max)) 
                    max = i;
                if ((((i = txt.IndexOf("ВЛ-")))) > 0 && (i < max)) 
                    max = i;
                if ((((i = txt.IndexOf("КЛ-")))) > 0 && (i < max)) 
                    max = i;
                if ((((i = txt.IndexOf("КТПН-")))) > 0 && (i < max)) 
                    max = i;
                for (i = max - 1; i > 0; i--) 
                {
                    if ((char.IsWhiteSpace(txt[i]) || txt[i] == ',' || txt[i] == '-') || txt[i] == '.') 
                        max = i;
                    else 
                        break;
                }
                for (i = 0; i < max; i++) 
                {
                    if (_startsWith(txt, i, "РФ")) 
                    {
                        i += 2;
                        continue;
                    }
                    if (_startsWith(txt, i, "РОССИЯ")) 
                    {
                        i += 6;
                        continue;
                    }
                    if (_startsWith(txt, i, "Г ")) 
                    {
                        i += 2;
                        continue;
                    }
                    if (_startsWith(txt, i, "Г.")) 
                    {
                        i += 2;
                        continue;
                    }
                    if (char.IsLetter(txt[i])) 
                        break;
                }
                int i0 = i;
                if (max > 10 && txt[max - 1] == '0') 
                {
                    for (int ii = max - 2; ii > 0; ii--) 
                    {
                        if (char.IsLetterOrDigit(txt[ii])) 
                            break;
                        else if (txt[ii] == ' ' || txt[ii] == ',') 
                        {
                            max = ii;
                            break;
                        }
                    }
                }
                if (max > 10) 
                {
                    for (int ii = max - 1; ii > 4; ii--) 
                    {
                        if (char.ToUpper(txt[ii]) == 'Т' && char.ToUpper(txt[ii - 1]) == 'Е' && char.ToUpper(txt[ii - 2]) == 'Н') 
                        {
                            max = ii - 2;
                            ii -= 3;
                        }
                        else if ((char.ToUpper(txt[ii]) == 'П' && char.ToUpper(txt[ii - 1]) == 'Р' && char.ToUpper(txt[ii - 2]) == 'О') && char.ToUpper(txt[ii - 2]) == 'К') 
                        {
                            max = ii - 3;
                            ii -= 4;
                        }
                        else if (char.ToUpper(txt[ii]) == 'Р' && char.ToUpper(txt[ii - 1]) == 'О' && char.ToUpper(txt[ii - 2]) == 'К') 
                        {
                            max = ii - 2;
                            ii -= 3;
                        }
                        else if (char.ToUpper(txt[ii]) == 'В' && char.ToUpper(txt[ii - 1]) == 'К') 
                        {
                            max = ii - 1;
                            ii -= 2;
                        }
                        else if (char.ToUpper(txt[ii]) == 'Л' && char.ToUpper(txt[ii - 1]) == 'У' && !char.IsLetter(txt[ii - 2])) 
                        {
                            max = ii - 1;
                            ii -= 2;
                        }
                        else if (((txt[ii] == ' ' || txt[ii] == '.' || txt[ii] == ';') || txt[ii] == ',' || txt[ii] == '-') || txt[ii] == '\\' || txt[ii] == '/') 
                            max = ii;
                        else if (txt[ii] == '0' && !char.IsLetterOrDigit(txt[ii - 1])) 
                            max = ii;
                        else if (char.ToUpper(txt[ii]) == 'Д' && !char.IsLetterOrDigit(txt[ii - 1])) 
                            max = ii;
                        else 
                            break;
                    }
                }
                if ((res.EndChar + 1) < max) 
                {
                    string fff = txt.Substring(res.EndChar + 1, max - res.EndChar - 1).Trim();
                    Pullenti.Address.AreaAttributes aa = null;
                    if (res.LastItem != null) 
                        aa = res.LastItem.Attrs as Pullenti.Address.AreaAttributes;
                    if (aa != null && aa.Types.Count > 0 && aa.Types[0].StartsWith(fff, StringComparison.OrdinalIgnoreCase)) 
                        res.EndChar = max - 1;
                }
                for (; i < max; i++) 
                {
                    if (char.IsLetterOrDigit(txt[i])) 
                    {
                        totalChar++;
                        if ((i < res.BeginChar) || i > res.EndChar) 
                            notChar++;
                    }
                }
                bool notChangeCoef = false;
                if (((res.EndChar + 1) < max) && res.ErrorMessage == null) 
                {
                    if (msg == null) 
                        msg = new StringBuilder();
                    if (i0 < res.BeginChar) 
                        msg.AppendFormat("Непонятные фрагменты: '{0}' и '{1}'. ", txt.Substring(i0, res.BeginChar - i0).Trim(), txt.Substring(res.EndChar + 1, max - res.EndChar - 1).Trim());
                    else 
                    {
                        string ppp = txt.Substring(res.EndChar + 1, max - res.EndChar - 1).Trim();
                        if (ppp.StartsWith(",")) 
                            ppp = ppp.Substring(1).Trim();
                        if (string.IsNullOrEmpty(ppp)) 
                            notChangeCoef = true;
                        else if (ppp.StartsWith("номер учетной", StringComparison.OrdinalIgnoreCase)) 
                            notChangeCoef = true;
                        else 
                        {
                            msg.AppendFormat("Непонятный фрагмент: '{0}'. ", ppp);
                            if (ppp[0] == '/' || ppp[0] == '\\') 
                                ppp = ppp.Substring(1).Trim();
                            if (ppp.StartsWith("ММ", StringComparison.OrdinalIgnoreCase) || ppp.StartsWith("MM", StringComparison.OrdinalIgnoreCase)) 
                                notChangeCoef = true;
                        }
                    }
                }
                else if (i0 < res.BeginChar) 
                {
                    if (msg == null) 
                        msg = new StringBuilder();
                    msg.AppendFormat("Непонятный фрагмент: '{0}'. ", txt.Substring(i0, res.BeginChar - i0).Trim());
                }
                if (totalChar > 0 && notChar > 0 && !notChangeCoef) 
                {
                    if ((notChar < 4) && Pullenti.Address.AddressHelper.CompareLevels(res.LastItem.Level, Pullenti.Address.AddrLevel.Plot) >= 0) 
                    {
                    }
                    else 
                        dcoef *= (((double)((totalChar - notChar))) / totalChar);
                }
            }
            if (unknownNames != null && unknownNames.Count > 0) 
            {
                string all = res.ToString().ToUpper();
                for (int k = unknownNames.Count - 1; k >= 0; k--) 
                {
                    if (all.Contains(unknownNames[k].ToUpper())) 
                        unknownNames.RemoveAt(k);
                }
                if (unknownNames.Count > 0) 
                {
                    if (dcoef == 100 && unknownNames.Count == 1 && res.Text.StartsWith(unknownNames[0], StringComparison.OrdinalIgnoreCase)) 
                    {
                    }
                    else 
                        dcoef *= 0.8;
                    if (msg == null) 
                        msg = new StringBuilder();
                    if (unknownNames.Count == 1) 
                        msg.AppendFormat("Непонятный объект: '{0}'", unknownNames[0]);
                    else 
                        msg.AppendFormat("Непонятные объекты: '{0}'", unknownNames[0]);
                    for (int k = 1; k < unknownNames.Count; k++) 
                    {
                        msg.AppendFormat(", '{0}'", unknownNames[k]);
                    }
                    msg.Append(". ");
                }
            }
            res.Coef = (int)dcoef;
            if (msg != null) 
                res.ErrorMessage = msg.ToString().Trim();
            return res;
        }
        static bool _startsWith(string txt, int i, string sub)
        {
            for (int j = 0; j < sub.Length; j++) 
            {
                if ((i + j) >= txt.Length) 
                    return false;
                if (char.ToUpper(txt[i + j]) != char.ToUpper(sub[j])) 
                    return false;
            }
            return true;
        }
    }
}