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
    class AddrSearchFormal
    {
        public SearchAddressItem Src;
        public List<string> Words = new List<string>();
        public string StdAdj;
        public string Number;
        public string Typname;
        public int RegId;
        public override string ToString()
        {
            StringBuilder res = new StringBuilder();
            if (Typname != null) 
                res.AppendFormat("{0} ", Typname);
            foreach (string w in Words) 
            {
                res.AppendFormat("{0} ", w);
            }
            if (StdAdj != null) 
                res.AppendFormat("{0} ", StdAdj);
            if (Number != null) 
                res.AppendFormat("{0} ", Number);
            return res.ToString().Trim();
        }
        public AddrSearchFormal(SearchAddressItem src)
        {
            Src = src;
            Pullenti.Ner.AnalysisResult ar = null;
            try 
            {
                ar = Pullenti.Ner.ProcessorService.EmptyProcessor.Process(new Pullenti.Ner.SourceOfAnalysis(src.Text), null, null);
            }
            catch(Exception ex34) 
            {
            }
            if (ar == null) 
                return;
            if (GarHelper.GarIndex == null) 
                return;
            for (Pullenti.Ner.Token t = ar.FirstToken; t != null; t = t.Next) 
            {
                Pullenti.Ner.Address.Internal.StreetItemToken sit = Pullenti.Ner.Address.Internal.StreetItemToken.TryParse(t, null, true, null);
                if ((sit != null && ((sit.Typ == Pullenti.Ner.Address.Internal.StreetItemType.StdAdjective || sit.Typ == Pullenti.Ner.Address.Internal.StreetItemType.StdPartOfName)) && sit.Termin != null) && ((t.Previous != null || t.Next != null))) 
                {
                    StdAdj = NameAnalyzer.CorrectAdj(sit.Termin.CanonicText);
                    if (StdAdj == null) 
                        Words.Add(sit.Termin.CanonicText);
                    t = sit.EndToken;
                    continue;
                }
                if (sit != null && sit.Typ == Pullenti.Ner.Address.Internal.StreetItemType.Number) 
                {
                    Number = sit.Value;
                    t = sit.EndToken;
                    continue;
                }
                if (Typname == null) 
                {
                    if (sit != null && sit.Typ == Pullenti.Ner.Address.Internal.StreetItemType.Noun && Src.Level == SearchLevel.Street) 
                    {
                        Typname = sit.Termin.CanonicText.ToLower();
                        t = sit.EndToken;
                        continue;
                    }
                    if (Src.Level == SearchLevel.City) 
                    {
                        Pullenti.Ner.Geo.Internal.CityItemToken cit = Pullenti.Ner.Geo.Internal.CityItemToken.TryParse(t, null, false, null);
                        if (cit != null && cit.Typ == Pullenti.Ner.Geo.Internal.CityItemToken.ItemType.Noun) 
                        {
                            Typname = cit.Value.ToLower();
                            t = cit.EndToken;
                            continue;
                        }
                    }
                    if (Src.Level == SearchLevel.District) 
                    {
                        Pullenti.Ner.Geo.Internal.TerrItemToken ter = Pullenti.Ner.Geo.Internal.TerrItemToken.TryParse(t, null, null);
                        if (ter != null && ter.TerminItem != null) 
                        {
                            Typname = ter.TerminItem.CanonicText.ToLower();
                            t = ter.EndToken;
                            continue;
                        }
                    }
                }
                if ((t is Pullenti.Ner.TextToken) && t.LengthChar > 1) 
                    Words.Add((t as Pullenti.Ner.TextToken).Term);
            }
            if (Words.Count > 1 && ((char.IsDigit(Words[0][0]) || Words[0].Length == 1))) 
            {
                string n = Words[0];
                Words.RemoveAt(0);
                Words.Add(n);
            }
            for (int i = 0; i < Words.Count; i++) 
            {
                Words[i] = NameAnalyzer.CorrName(Words[i]);
            }
        }
        public bool Check(Pullenti.Address.Internal.Gar.AreaObject ao, bool lite)
        {
            return true;
        }
        public List<Pullenti.Address.Internal.Gar.AreaTreeObject> Search()
        {
            if (Words.Count == 0) 
                return new List<Pullenti.Address.Internal.Gar.AreaTreeObject>();
            List<Pullenti.Address.Internal.Gar.AreaTreeObject> res = new List<Pullenti.Address.Internal.Gar.AreaTreeObject>();
            if (Words.Count > 1) 
            {
                List<string> vars = new List<string>();
                NameAnalyzer.CreateSearchVariants(vars, null, null, string.Format("{0} {1}", Words[0], Words[1]), null);
                foreach (string v in vars) 
                {
                    res = GarHelper.GarIndex.GetAllStringEntriesByStart(v, StdAdj, Number, Src.Level == SearchLevel.Street, RegId);
                    if (res.Count > 0) 
                        break;
                }
            }
            else 
                res = GarHelper.GarIndex.GetAllStringEntriesByStart(Words[0], StdAdj, Number, Src.Level == SearchLevel.Street, RegId);
            if (Words.Count > 1 && res.Count == 0) 
            {
                List<Pullenti.Address.Internal.Gar.AreaTreeObject> res2 = GarHelper.GarIndex.GetAllStringEntriesByStart(Words[1], StdAdj, Number, Src.Level == SearchLevel.Street, RegId);
                if (res.Count == 0) 
                    res = res2;
                else if (res2.Count > 0) 
                {
                    Dictionary<int, bool> hash = new Dictionary<int, bool>();
                    foreach (Pullenti.Address.Internal.Gar.AreaTreeObject r in res2) 
                    {
                        if (!hash.ContainsKey(r.Id)) 
                            hash.Add(r.Id, true);
                    }
                    List<Pullenti.Address.Internal.Gar.AreaTreeObject> res3 = new List<Pullenti.Address.Internal.Gar.AreaTreeObject>();
                    for (int i = res.Count - 1; i >= 0; i--) 
                    {
                        if (hash.ContainsKey(res[i].Id)) 
                            res3.Add(res[i]);
                    }
                    res = res3;
                }
            }
            if (Typname != null) 
            {
                for (int i = res.Count - 1; i >= 0; i--) 
                {
                    Pullenti.Address.Internal.Gar.AreaTreeObject r = res[i];
                    bool ok = false;
                    if (r.Typs != null) 
                    {
                        foreach (string ty in r.Typs) 
                        {
                            if (ty.Contains(Typname) || Typname.Contains(ty)) 
                            {
                                ok = true;
                                break;
                            }
                        }
                    }
                    if (!ok) 
                        res.RemoveAt(i);
                }
            }
            if (Src.IgnoreTerritories) 
            {
                for (int i = res.Count - 1; i >= 0; i--) 
                {
                    Pullenti.Address.Internal.Gar.AreaTreeObject r = res[i];
                    if (r.Level == Pullenti.Address.AddrLevel.Territory) 
                        res.RemoveAt(i);
                }
            }
            return res;
        }
    }
}