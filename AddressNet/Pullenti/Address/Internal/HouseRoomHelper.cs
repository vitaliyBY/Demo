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
    static class HouseRoomHelper
    {
        static int _getNumsCount(Pullenti.Address.HouseAttributes ha)
        {
            if (ha == null) 
                return 0;
            int cou = 0;
            if (ha.Number != null) 
                cou++;
            if (ha.StroenNumber != null) 
                cou++;
            if (ha.BuildNumber != null) 
                cou++;
            return cou;
        }
        static Pullenti.Address.HouseType _getHouseType(Pullenti.Ner.Address.AddressHouseType ht)
        {
            if (ht == Pullenti.Ner.Address.AddressHouseType.Estate) 
                return Pullenti.Address.HouseType.Estate;
            if (ht == Pullenti.Ner.Address.AddressHouseType.House) 
                return Pullenti.Address.HouseType.House;
            if (ht == Pullenti.Ner.Address.AddressHouseType.HouseEstate) 
                return Pullenti.Address.HouseType.HouseEstate;
            if (ht == Pullenti.Ner.Address.AddressHouseType.Special) 
                return Pullenti.Address.HouseType.Special;
            return Pullenti.Address.HouseType.Undefined;
        }
        static Pullenti.Address.StroenType _getStroenType(Pullenti.Ner.Address.AddressBuildingType bt)
        {
            if (bt == Pullenti.Ner.Address.AddressBuildingType.Building) 
                return Pullenti.Address.StroenType.Building;
            if (bt == Pullenti.Ner.Address.AddressBuildingType.Construction) 
                return Pullenti.Address.StroenType.Construction;
            if (bt == Pullenti.Ner.Address.AddressBuildingType.Liter) 
                return Pullenti.Address.StroenType.Liter;
            return Pullenti.Address.StroenType.Undefined;
        }
        public static void ProcessHouseAndRooms(AnalyzeHelper ah, Pullenti.Ner.Address.AddressReferent ar, Pullenti.Address.TextAddress addr)
        {
            if (ar.Box != null && ((ar.House != null || ar.Building != null || ar.Corpus != null))) 
            {
                Pullenti.Ner.Address.AddressReferent ar2 = new Pullenti.Ner.Address.AddressReferent();
                ar2.Box = ar.Box;
                ar.Box = null;
                ProcessHouseAndRooms(ah, ar, addr);
                ar.Box = ar2.Box;
                ProcessHouseAndRooms(ah, ar2, addr);
                return;
            }
            if (ar.Plot != null && ((ar.House != null || ar.Building != null || ar.Corpus != null))) 
            {
                Pullenti.Ner.Address.AddressReferent ar2 = new Pullenti.Ner.Address.AddressReferent();
                ar2.Plot = ar.Plot;
                ar.Plot = null;
                ProcessHouseAndRooms(ah, ar2, addr);
                ProcessHouseAndRooms(ah, ar, addr);
                ar.Box = ar2.Plot;
                return;
            }
            if (addr.LastItem == null) 
                return;
            Pullenti.Address.AddrObject plot = null;
            Pullenti.Address.AddrObject house = null;
            Pullenti.Address.AddrObject addHouse = null;
            Pullenti.Address.AddrObject tr = null;
            string flatNum = null;
            if (ar.Field != null && Pullenti.Address.AddressHelper.CompareLevels(addr.LastItem.Level, Pullenti.Address.AddrLevel.Territory) <= 0) 
            {
                Pullenti.Address.AreaAttributes aa = new Pullenti.Address.AreaAttributes();
                aa.Types.Add("поле");
                aa.Number = ar.Field;
                addr.Items.Add(new Pullenti.Address.AddrObject(aa) { Level = Pullenti.Address.AddrLevel.Street });
            }
            Pullenti.Address.AddrObject ao = null;
            for (int i = addr.Items.Count - 1; i >= 0; i--) 
            {
                Pullenti.Address.AddrObject ao0 = addr.Items[i];
                if ((ao0.Level == Pullenti.Address.AddrLevel.Territory || ao0.Level == Pullenti.Address.AddrLevel.Street || ao0.Level == Pullenti.Address.AddrLevel.Locality) || ao0.Level == Pullenti.Address.AddrLevel.City) 
                {
                    if (ao0.Gars.Count > 0) 
                    {
                        ao = ao0;
                        break;
                    }
                    if (ao0.Level == Pullenti.Address.AddrLevel.Locality) 
                        break;
                }
            }
            if (GarHelper.GarIndex != null && ao != null && ao.Gars.Count > 0) 
            {
                List<Pullenti.Address.GarObject> hobjs = null;
                for (int kk = 0; kk < 2; kk++) 
                {
                    foreach (Pullenti.Address.GarObject g in ao.Gars) 
                    {
                        if (g.ChildrenCount == 0) 
                            continue;
                        Pullenti.Address.Internal.Gar.HousesInStreet hinstr = ah.GetHousesInStreet(g.Id);
                        if (hinstr == null) 
                            continue;
                        hobjs = null;
                        if (ah.LiteraVariant != null && ar.Building == null) 
                        {
                            Pullenti.Ner.Address.AddressReferent arr = ar.Clone() as Pullenti.Ner.Address.AddressReferent;
                            arr.Building = ah.LiteraVariant.Value;
                            arr.BuildingType = Pullenti.Ner.Address.AddressBuildingType.Liter;
                            hobjs = _findHousesNew(ah, hinstr, arr, (ao.CrossObject != null ? 1 : 0));
                        }
                        if (hobjs == null) 
                        {
                            if (addr.Items[addr.Items.Count - 1].Level == Pullenti.Address.AddrLevel.Street) 
                            {
                                Pullenti.Address.AreaAttributes aa = addr.Items[addr.Items.Count - 1].Attrs as Pullenti.Address.AreaAttributes;
                                if (aa.Number != null && aa.Types.Contains("блок") && ar.Box != null) 
                                {
                                    Pullenti.Ner.Address.AddressReferent ar2 = ar.Clone() as Pullenti.Ner.Address.AddressReferent;
                                    ar2.Box = string.Format("{0}/{1}", ar2.Box, aa.Number);
                                    hobjs = _findHousesNew(ah, hinstr, ar2, (ao.CrossObject != null ? 1 : 0));
                                }
                            }
                            if (hobjs == null) 
                                hobjs = _findHousesNew(ah, hinstr, ar, (ao.CrossObject != null ? 1 : 0));
                        }
                        Pullenti.Ner.Address.AddressReferent ar0 = ar;
                        if (hobjs != null) 
                        {
                        }
                        else if (ar.Flat == null && ar.CorpusOrFlat == null && ao.CrossObject == null) 
                        {
                            string num = ar.House ?? ar.HouseOrPlot;
                            int ii = -1;
                            bool hiph = false;
                            if (num != null) 
                            {
                                if ((((ii = num.IndexOf('/')))) < 0) 
                                {
                                    ii = num.IndexOf('-');
                                    if (ii > 0) 
                                        hiph = true;
                                }
                            }
                            if (hobjs != null && _getNumsCount(hobjs[0].Attrs as Pullenti.Address.HouseAttributes) > 1) 
                                ii = -1;
                            int nn = 0;
                            if (ii > 0) 
                                int.TryParse(num.Substring(ii + 1), out nn);
                            if (nn > 0 || ((ii > 0 && !hiph))) 
                            {
                                Pullenti.Ner.Address.AddressReferent ar2 = new Pullenti.Ner.Address.AddressReferent();
                                ar2.House = num.Substring(0, ii);
                                ar2.Flat = num.Substring(ii + 1);
                                List<Pullenti.Address.GarObject> hobjs2 = _findHousesNew(ah, hinstr, ar2, 0);
                                if (hobjs2 != null) 
                                {
                                    hobjs = hobjs2;
                                    flatNum = num.Substring(ii + 1);
                                    if ((((ii = flatNum.IndexOf('-')))) > 0) 
                                        flatNum = flatNum.Substring(ii + 1);
                                    ar2.Flat = flatNum;
                                    ar0 = ar2;
                                }
                            }
                            else if (hiph) 
                            {
                                Pullenti.Ner.Address.AddressReferent a1 = new Pullenti.Ner.Address.AddressReferent();
                                a1.House = num.Substring(ii + 1);
                                List<Pullenti.Address.GarObject> hobjs2 = _findHousesNew(ah, hinstr, a1, (ao.CrossObject != null ? 1 : 0));
                                if (hobjs2 != null && hobjs2.Count == 1) 
                                {
                                    Pullenti.Address.AddrObject ao1 = new Pullenti.Address.AddrObject(hobjs2[0].Attrs) { Level = Pullenti.Address.AddrLevel.Building };
                                    ao1.Gars.Add(hobjs2[0]);
                                    if (addr.AdditionalItems == null) 
                                        addr.AdditionalItems = new List<Pullenti.Address.AddrObject>();
                                    addr.AdditionalItems.Add(ao1);
                                }
                            }
                        }
                        if (hobjs != null) 
                        {
                            foreach (Pullenti.Address.GarObject gh in hobjs) 
                            {
                                if ((gh.Attrs as Pullenti.Address.HouseAttributes).Typ == Pullenti.Address.HouseType.Plot) 
                                {
                                    if (plot == null) 
                                        plot = new Pullenti.Address.AddrObject(gh.Attrs.Clone()) { Level = Pullenti.Address.AddrLevel.Plot };
                                    plot.Gars.Add(gh);
                                    continue;
                                }
                                if (house == null) 
                                    house = new Pullenti.Address.AddrObject(gh.Attrs.Clone()) { Level = Pullenti.Address.AddrLevel.Building };
                                house.Gars.Add(gh);
                                if (gh.Tag is List<Pullenti.Address.GarObject>) 
                                {
                                    List<Pullenti.Address.GarObject> gobjs2 = gh.Tag as List<Pullenti.Address.GarObject>;
                                    addHouse = new Pullenti.Address.AddrObject(gobjs2[0].Attrs.Clone()) { Level = Pullenti.Address.AddrLevel.Building };
                                    addHouse.Gars.AddRange(gobjs2);
                                }
                                if (ar.CorpusOrFlat != null && (gh.Attrs as Pullenti.Address.HouseAttributes).BuildNumber == ar.CorpusOrFlat) 
                                {
                                    ar.CorpusOrFlat = null;
                                    ar.Corpus = (gh.Attrs as Pullenti.Address.HouseAttributes).BuildNumber;
                                }
                                else if (gh.ChildrenCount > 0) 
                                {
                                    Pullenti.Address.Internal.Gar.RoomsInHouse rih = ah.GetRoomsInObject(gh.Id);
                                    Pullenti.Address.GarObject gg = _findRoomNew(ah, rih, ar0);
                                    if (gg != null) 
                                    {
                                        if (tr == null) 
                                            tr = new Pullenti.Address.AddrObject(gg.Attrs) { Level = Pullenti.Address.AddrLevel.Apartment };
                                        tr.Gars.Add(gg);
                                    }
                                }
                            }
                        }
                    }
                    if (ao.CrossObject != null) 
                    {
                        string num = ar.House ?? ar.HouseOrPlot;
                        foreach (Pullenti.Address.GarObject g in ao.CrossObject.Gars) 
                        {
                            if (g.ChildrenCount == 0) 
                                continue;
                            Pullenti.Address.Internal.Gar.HousesInStreet hinstr = ah.GetHousesInStreet(g.Id);
                            if (hinstr == null) 
                                continue;
                            hobjs = _findHousesNew(ah, hinstr, ar, 2);
                            ah.IndexReadCount++;
                            if (hobjs != null) 
                            {
                                foreach (Pullenti.Address.GarObject gh in hobjs) 
                                {
                                    if (house == null) 
                                    {
                                        if (num == null || (num.IndexOf('/') < 0)) 
                                            continue;
                                        house = new Pullenti.Address.AddrObject(gh.Attrs.Clone()) { Level = Pullenti.Address.AddrLevel.Building };
                                        (house.Attrs as Pullenti.Address.HouseAttributes).Number = num.Substring(0, num.IndexOf('/'));
                                    }
                                    if (house.CrossObject == null) 
                                        house.CrossObject = new Pullenti.Address.AddrObject(gh.Attrs) { Level = Pullenti.Address.AddrLevel.Building };
                                    house.CrossObject.Gars.Add(gh);
                                    Pullenti.Address.GarObject gg = _findRoomNew(ah, ah.GetRoomsInObject(gh.Id), ar);
                                    if (gg != null) 
                                    {
                                        if (tr == null) 
                                            tr = new Pullenti.Address.AddrObject(gg.Attrs) { Level = Pullenti.Address.AddrLevel.Apartment };
                                        tr.Gars.Add(gg);
                                    }
                                }
                            }
                        }
                        if (house != null && house.CrossObject == null) 
                        {
                            if (num != null && num.IndexOf('/') > 0) 
                            {
                                house.CrossObject = new Pullenti.Address.AddrObject(house.Attrs.Clone()) { Level = Pullenti.Address.AddrLevel.Building };
                                (house.CrossObject.Attrs as Pullenti.Address.HouseAttributes).Number = num.Substring(num.IndexOf('/') + 1);
                            }
                        }
                    }
                    if (hobjs != null) 
                        break;
                    int i = addr.Items.IndexOf(ao);
                    if (i > 0 && addr.Items[i - 1].Level == Pullenti.Address.AddrLevel.Locality && addr.Items[i - 1].Gars.Count > 0) 
                        ao = addr.Items[i - 1];
                    else 
                        break;
                }
            }
            if (plot != null && house != null) 
            {
                if (ar.HouseOrPlot != null) 
                    plot = null;
            }
            if (plot == null && ar.Plot != null) 
                plot = new Pullenti.Address.AddrObject(new Pullenti.Address.HouseAttributes() { Typ = Pullenti.Address.HouseType.Plot, Number = (ar.Plot == "0" ? "б/н" : ar.Plot) }) { Level = Pullenti.Address.AddrLevel.Plot };
            if (plot != null) 
            {
                plot.SortGars();
                addr.Items.Add(plot);
            }
            if (house == null && plot == null) 
            {
                if (ar.House != null) 
                    house = new Pullenti.Address.AddrObject(new Pullenti.Address.HouseAttributes() { Typ = _getHouseType(ar.HouseType), Number = (ar.House == "0" ? "б/н" : ar.House) });
                if (ar.HouseOrPlot != null) 
                    house = new Pullenti.Address.AddrObject(new Pullenti.Address.HouseAttributes() { Typ = Pullenti.Address.HouseType.Undefined, Number = (ar.HouseOrPlot == "0" ? "б/н" : ar.HouseOrPlot) });
                if (ar.Building != null && ((ar.Building != "0" || house == null))) 
                {
                    if (house == null) 
                        house = new Pullenti.Address.AddrObject(new Pullenti.Address.HouseAttributes()) { Level = Pullenti.Address.AddrLevel.Building };
                    (house.Attrs as Pullenti.Address.HouseAttributes).StroenNumber = (ar.Building == "0" ? "б/н" : ar.Building);
                    (house.Attrs as Pullenti.Address.HouseAttributes).StroenTyp = _getStroenType(ar.BuildingType);
                }
                if (ar.Corpus != null && ((ar.Corpus != "0" || house == null))) 
                {
                    if (house == null) 
                        house = new Pullenti.Address.AddrObject(new Pullenti.Address.HouseAttributes()) { Level = Pullenti.Address.AddrLevel.Building };
                    (house.Attrs as Pullenti.Address.HouseAttributes).BuildNumber = (ar.Corpus == "0" ? "б/н" : ar.Corpus);
                }
                if (house == null && ar.Box != null) 
                    house = new Pullenti.Address.AddrObject(new Pullenti.Address.HouseAttributes() { Typ = Pullenti.Address.HouseType.Garage, Number = (ar.Box == "0" ? "б/н" : ar.Box) });
                if (house == null && ar.Well != null) 
                    house = new Pullenti.Address.AddrObject(new Pullenti.Address.HouseAttributes() { Typ = Pullenti.Address.HouseType.Well, Number = (ar.Well == "0" ? "б/н" : ar.Well) });
                if (house != null) 
                {
                    house.Level = Pullenti.Address.AddrLevel.Building;
                    if ((house.Attrs as Pullenti.Address.HouseAttributes).Typ == Pullenti.Address.HouseType.Undefined && (house.Attrs as Pullenti.Address.HouseAttributes).Number != null) 
                    {
                        Pullenti.Address.AddrObject it = addr.FindItemByLevel(Pullenti.Address.AddrLevel.Territory);
                        if (it != null) 
                        {
                            Pullenti.Address.AreaAttributes aa = it.Attrs as Pullenti.Address.AreaAttributes;
                            foreach (string m in aa.Miscs) 
                            {
                                if (m.Contains("гараж")) 
                                {
                                    (house.Attrs as Pullenti.Address.HouseAttributes).Typ = Pullenti.Address.HouseType.Garage;
                                    break;
                                }
                            }
                        }
                    }
                    if (ar.CorpusOrFlat != null) 
                        (house.Attrs as Pullenti.Address.HouseAttributes).BuildNumber = ar.CorpusOrFlat;
                }
            }
            if (house != null) 
            {
                if (ao != null && ao.CrossObject != null && house.CrossObject == null) 
                {
                    string num = (house.Attrs as Pullenti.Address.HouseAttributes).Number;
                    if (num != null && num.IndexOf('/') > 0) 
                    {
                        house.CrossObject = new Pullenti.Address.AddrObject(house.Attrs.Clone());
                        (house.Attrs as Pullenti.Address.HouseAttributes).Number = num.Substring(0, num.IndexOf('/'));
                        (house.CrossObject.Attrs as Pullenti.Address.HouseAttributes).Number = num.Substring(num.IndexOf('/') + 1);
                    }
                }
                house.SortGars();
                addr.Items.Add(house);
                if (addHouse != null) 
                {
                    if (addr.AdditionalItems == null) 
                        addr.AdditionalItems = new List<Pullenti.Address.AddrObject>();
                    addr.AdditionalItems.Add(addHouse);
                }
                if ((house.Attrs as Pullenti.Address.HouseAttributes).Typ == Pullenti.Address.HouseType.Special && ar.Well != null) 
                    addr.Items.Add(new Pullenti.Address.AddrObject(new Pullenti.Address.HouseAttributes() { Typ = Pullenti.Address.HouseType.Well, Number = (ar.Well == "0" ? "б/н" : ar.Well) }) { Level = Pullenti.Address.AddrLevel.Building });
            }
            if (tr == null) 
            {
                Pullenti.Address.RoomAttributes ra = CreateApartmentAttrs(ar, flatNum);
                if (ra != null) 
                {
                    tr = new Pullenti.Address.AddrObject(ra);
                    tr.Level = Pullenti.Address.AddrLevel.Apartment;
                }
            }
            if (tr != null) 
            {
                tr.SortGars();
                addr.Items.Add(tr);
                if (ar.Carplace != null && (tr.Attrs as Pullenti.Address.RoomAttributes).Typ != Pullenti.Address.RoomType.Carplace) 
                    addr.Items.Add(new Pullenti.Address.AddrObject(new Pullenti.Address.RoomAttributes() { Typ = Pullenti.Address.RoomType.Carplace, Number = ar.Carplace }) { Level = Pullenti.Address.AddrLevel.Room });
            }
            if (ar.Room != null) 
            {
                Pullenti.Address.RoomAttributes ra = new Pullenti.Address.RoomAttributes();
                Pullenti.Address.AddrObject room = new Pullenti.Address.AddrObject(ra);
                room.Level = Pullenti.Address.AddrLevel.Room;
                ra.Typ = Pullenti.Address.RoomType.Room;
                ra.Number = ar.Room;
                addr.Items.Add(room);
            }
        }
        public static Pullenti.Address.RoomAttributes CreateApartmentAttrs(Pullenti.Ner.Address.AddressReferent ar, string flatNum)
        {
            Pullenti.Address.RoomAttributes ra = new Pullenti.Address.RoomAttributes();
            if (ar.Flat != null) 
            {
                ra.Number = ar.Flat;
                ra.Typ = Pullenti.Address.RoomType.Flat;
            }
            else if (ar.Office != null) 
            {
                ra.Number = ar.Office;
                ra.Typ = Pullenti.Address.RoomType.Office;
            }
            else if (ar.Space != null) 
            {
                ra.Number = ar.Space;
                ra.Typ = Pullenti.Address.RoomType.Space;
            }
            else if (ar.Pavilion != null) 
            {
                ra.Number = ar.Pavilion;
                ra.Typ = Pullenti.Address.RoomType.Pavilion;
            }
            else if (ar.Pantry != null) 
            {
                ra.Number = ar.Pantry;
                ra.Typ = Pullenti.Address.RoomType.Panty;
            }
            else if (ar.Carplace != null) 
            {
                ra.Number = ar.Carplace;
                ra.Typ = Pullenti.Address.RoomType.Carplace;
            }
            else if (flatNum != null) 
            {
                ra.Number = flatNum;
                ra.Typ = Pullenti.Address.RoomType.Flat;
            }
            else 
                return null;
            if (ra.Number == "НЕТ") 
                return null;
            if (ra.Number == "0") 
                ra.Number = "б/н";
            return ra;
        }
        static int _getId(string v)
        {
            return int.Parse(v.Substring(1));
        }
        static List<Pullenti.Address.GarObject> _findHousesNew(AnalyzeHelper ah, Pullenti.Address.Internal.Gar.HousesInStreet hinst, Pullenti.Ner.Address.AddressReferent a, int crossNum)
        {
            if (a.Plot == null || a.House == null || a.Plot == a.House) 
                return _findHousesNew0(ah, hinst, a, crossNum);
            Pullenti.Ner.Address.AddressReferent pl = new Pullenti.Ner.Address.AddressReferent();
            pl.Plot = a.Plot;
            List<Pullenti.Address.GarObject> res1 = _findHousesNew0(ah, hinst, pl, crossNum);
            a.Plot = null;
            List<Pullenti.Address.GarObject> res2 = _findHousesNew0(ah, hinst, a, crossNum);
            a.Plot = pl.Plot;
            if (res1 == null) 
                return res2;
            if (res2 == null) 
                return res1;
            res1.AddRange(res2);
            return res1;
        }
        static List<Pullenti.Address.GarObject> _findHousesNew0(AnalyzeHelper ah, Pullenti.Address.Internal.Gar.HousesInStreet hinst, Pullenti.Ner.Address.AddressReferent a, int crossNum)
        {
            if (hinst == null) 
                return null;
            if (crossNum > 0 && ((a.House != null || a.HouseOrPlot != null))) 
            {
                string nnn = a.House ?? a.HouseOrPlot;
                int ii = nnn.IndexOf('/');
                if (ii > 0) 
                {
                    Pullenti.Ner.Address.AddressReferent ar1 = new Pullenti.Ner.Address.AddressReferent();
                    ar1.House = (crossNum == 1 ? nnn.Substring(0, ii) : nnn.Substring(ii + 1));
                    List<Pullenti.Address.GarObject> res1 = _findHousesNew0(ah, hinst, ar1, 0);
                    if (res1 != null) 
                    {
                        if (crossNum == 1) 
                        {
                            List<Pullenti.Address.GarObject> res0 = _findHousesNew0(ah, hinst, a, 0);
                            if (res0 != null && res0[0].InternalTag >= res1[0].InternalTag) 
                                return res0;
                        }
                        return res1;
                    }
                }
            }
            if (a.CorpusOrFlat != null) 
            {
                Pullenti.Ner.Address.AddressReferent aa1 = a.Clone() as Pullenti.Ner.Address.AddressReferent;
                aa1.CorpusOrFlat = null;
                aa1.Corpus = a.CorpusOrFlat;
                List<Pullenti.Address.GarObject> res1 = _findHousesNew0(ah, hinst, aa1, crossNum);
                aa1.Corpus = null;
                List<Pullenti.Address.GarObject> res2 = _findHousesNew0(ah, hinst, aa1, crossNum);
                if (res1 != null) 
                {
                    if (res2 == null || (res2[0].InternalTag < res1[0].InternalTag)) 
                    {
                        a.Corpus = a.CorpusOrFlat;
                        a.CorpusOrFlat = null;
                        return res1;
                    }
                }
                if (res2 != null) 
                {
                    a.Flat = a.CorpusOrFlat;
                    a.CorpusOrFlat = null;
                    return res2;
                }
            }
            if (a.House != null && a.House.IndexOf('-') > 0) 
            {
                int ii = a.House.IndexOf('-');
                Pullenti.Ner.Address.AddressReferent a1 = new Pullenti.Ner.Address.AddressReferent();
                a1.House = a.House.Substring(0, ii);
                Pullenti.Ner.Address.AddressReferent a2 = new Pullenti.Ner.Address.AddressReferent();
                a2.House = a.House.Substring(ii + 1);
                if (string.Compare(a1.House, a2.House) < 0) 
                {
                    List<Pullenti.Address.GarObject> res1 = _findHousesNew0(ah, hinst, a1, 0);
                    List<Pullenti.Address.GarObject> res2 = _findHousesNew0(ah, hinst, a2, 0);
                    if (res1 != null && res2 != null) 
                    {
                        res1[0].Tag = res2;
                        return res1;
                    }
                }
                if (a.Flat == null) 
                {
                    List<Pullenti.Address.GarObject> res1 = _findHousesNew0(ah, hinst, a1, 0);
                    if (res1 != null) 
                    {
                        a.Flat = a.House.Substring(ii + 1);
                        return res1;
                    }
                }
            }
            NumberAnalyzer num = NumberAnalyzer.TryParseReferent(a, true);
            if (num == null) 
                return null;
            List<Pullenti.Address.Internal.Gar.HouseObject> hos = hinst.GetHouses(num);
            if (hos == null || hos.Count == 0) 
                return null;
            List<Pullenti.Address.GarObject> res = null;
            double max = (double)0;
            foreach (Pullenti.Address.Internal.Gar.HouseObject ho in hos) 
            {
                NumberAnalyzer num1 = NumberAnalyzer.TryParseHO(ho);
                if (num1 == null) 
                    continue;
                double co = num.CalcCoef(num1);
                if (co <= 0) 
                    continue;
                if (co < max) 
                    continue;
                Pullenti.Address.GarObject go = GarHelper.CreateGarHouse(ho);
                if (go == null) 
                    continue;
                go.InternalTag = 0;
                if (go.Expired) 
                    co /= 3;
                if (co < max) 
                    continue;
                if (co == max) 
                    res.Add(go);
                else 
                {
                    if (res == null) 
                        res = new List<Pullenti.Address.GarObject>();
                    else 
                        res.Clear();
                    res.Add(go);
                    max = co;
                    go.InternalTag = max;
                }
            }
            return res;
        }
        static Pullenti.Address.GarObject _findRoomNew(AnalyzeHelper ah, Pullenti.Address.Internal.Gar.RoomsInHouse rih, Pullenti.Ner.Address.AddressReferent a)
        {
            if (rih == null) 
                return null;
            NumberAnalyzer num = NumberAnalyzer.TryParseReferent(a, false);
            if (num == null) 
                return null;
            List<Pullenti.Address.Internal.Gar.RoomObject> hos = rih.GetRooms(num);
            if (hos == null || hos.Count == 0) 
                return null;
            List<Pullenti.Address.GarObject> res = null;
            double max = (double)0;
            bool hasFlatsAndSpaces = rih.CheckHasFlatsAndSpaces();
            foreach (Pullenti.Address.Internal.Gar.RoomObject ho in hos) 
            {
                NumberAnalyzer num1 = NumberAnalyzer.TryParseRO(ho);
                if (num1 == null) 
                    continue;
                double co = num.CalcCoef(num1);
                if (co <= 0) 
                    continue;
                if (co < max) 
                    continue;
                if (hasFlatsAndSpaces) 
                {
                    if (num.Items[0].Cla == NumberItemClass.Space && num1.Items[0].Cla == NumberItemClass.Flat) 
                        continue;
                    if (num.Items[0].Cla == NumberItemClass.Flat && num1.Items[0].Cla == NumberItemClass.Space) 
                        continue;
                }
                Pullenti.Address.GarObject go = GarHelper.CreateGarRoom(ho);
                if (go == null) 
                    continue;
                if (co == max) 
                    res.Add(go);
                else 
                {
                    if (res == null) 
                        res = new List<Pullenti.Address.GarObject>();
                    else 
                        res.Clear();
                    res.Add(go);
                    max = co;
                }
            }
            if (res == null) 
                return null;
            return res[0];
        }
        public static bool TryParseListItems(AnalyzeHelper ah, Pullenti.Address.TextAddress addr, Pullenti.Ner.AnalysisResult ar)
        {
            Pullenti.Ner.Token t = null;
            if (ar != null) 
            {
                for (t = ar.FirstToken; t != null; t = t.Next) 
                {
                    if (t.EndChar == addr.EndChar) 
                    {
                        t = t.Next;
                        break;
                    }
                }
            }
            if (t == null) 
                return false;
            if (!t.IsCommaAnd && !t.IsHiphen && !t.IsValue("ПО", null)) 
                return false;
            Pullenti.Address.AddrObject it0 = addr.LastItem;
            Pullenti.Address.RoomAttributes room = it0.Attrs as Pullenti.Address.RoomAttributes;
            Pullenti.Address.HouseAttributes house = it0.Attrs as Pullenti.Address.HouseAttributes;
            if (house == null && room == null) 
                return false;
            int n0 = 0;
            string liter = null;
            if (house != null) 
            {
                if (house.Number != null) 
                {
                    if (house.BuildNumber != null || house.StroenNumber != null) 
                        return false;
                    int.TryParse(house.Number, out n0);
                }
                else if (house.BuildNumber != null) 
                {
                    if (house.StroenNumber != null) 
                        return false;
                    int.TryParse(house.BuildNumber, out n0);
                }
                else if (house.StroenNumber != null) 
                {
                    if (!int.TryParse(house.StroenNumber, out n0)) 
                        liter = house.StroenNumber;
                }
                else 
                    return false;
            }
            else if (room.Number == null) 
                return false;
            else 
                int.TryParse(room.Number, out n0);
            int b0 = t.BeginChar;
            Pullenti.Ner.AnalysisResult ar0;
            try 
            {
                ar0 = Pullenti.Ner.ProcessorService.EmptyProcessor.Process(new Pullenti.Ner.SourceOfAnalysis(addr.Text.Substring(b0)) { UserParams = "ADDRESS" }, null, null);
            }
            catch(Exception ex) 
            {
                return false;
            }
            List<string> nums = new List<string>();
            if (liter != null && ah.LiteraVariant != null && liter != ah.LiteraVariant.Value) 
                nums.Add(ah.LiteraVariant.Value);
            for (t = ar0.FirstToken; t != null; t = t.Next) 
            {
                if (!t.IsCommaAnd && !t.IsHiphen && !t.IsValue("ПО", null)) 
                    break;
                bool hiph = t.IsHiphen || t.IsValue("ПО", null);
                t = t.Next;
                if (t == null) 
                    break;
                if (!hiph && t.Next != null && ((t.IsValue("С", null) || t.IsValue("C", null)))) 
                    t = t.Next;
                if ((liter != null && (t is Pullenti.Ner.TextToken) && t.LengthChar == 1) && t.Chars.IsLetter) 
                {
                    nums.Add((t as Pullenti.Ner.TextToken).Term);
                    addr.EndChar = t.EndChar + b0;
                    continue;
                }
                Pullenti.Ner.Address.Internal.AddressItemToken ait = Pullenti.Ner.Address.Internal.AddressItemToken.TryParsePureItem(t, null, null);
                if (ait == null || ait.Value == null) 
                    break;
                bool ok = ait.Typ == Pullenti.Ner.Address.Internal.AddressItemType.Number;
                if (ait.Typ == Pullenti.Ner.Address.Internal.AddressItemType.House && house != null && house.Typ != Pullenti.Address.HouseType.Plot) 
                    ok = true;
                else if (ait.Typ == Pullenti.Ner.Address.Internal.AddressItemType.Flat && room != null) 
                    ok = true;
                else if (ait.Typ == Pullenti.Ner.Address.Internal.AddressItemType.Plot && house != null && house.Typ == Pullenti.Address.HouseType.Plot) 
                    ok = true;
                else if ((liter != null && ait.Typ == Pullenti.Ner.Address.Internal.AddressItemType.Building && !string.IsNullOrEmpty(ait.Value)) && !char.IsDigit(ait.Value[0])) 
                    ok = true;
                if (!ok) 
                    break;
                int n1 = 0;
                int.TryParse(ait.Value, out n1);
                if (hiph && n0 > 0 && n1 > n0) 
                {
                    if ((n1 - n0) > 100) 
                        break;
                    for (int k = n0 + 1; k < n1; k++) 
                    {
                        nums.Add(k.ToString());
                    }
                }
                n0 = n1;
                nums.Add(ait.Value);
                t = ait.EndToken;
                addr.EndChar = t.EndChar + b0;
            }
            if (nums.Count < 1) 
                return false;
            if (nums.Count > 40) 
                return false;
            addr.AdditionalItems = new List<Pullenti.Address.AddrObject>();
            Pullenti.Address.GarObject par = null;
            Pullenti.Address.Internal.Gar.RoomsInHouse rinh = null;
            Pullenti.Address.Internal.Gar.HousesInStreet hinstr = null;
            if (it0.Gars.Count == 1 && it0.Gars[0].ParentIds.Count > 0) 
            {
                par = ah.GetGarObject(it0.Gars[0].ParentIds[0]);
                if (par != null && par.Level == Pullenti.Address.GarLevel.Building) 
                    rinh = ah.GetRoomsInObject(par.Id);
                else if (par != null && ((int)par.Level) >= ((int)Pullenti.Address.GarLevel.Locality)) 
                    hinstr = ah.GetHousesInStreet(par.Id);
            }
            foreach (string n in nums) 
            {
                Pullenti.Address.AddrObject it = it0.Clone();
                it.Gars.Clear();
                if (room != null) 
                    (it.Attrs as Pullenti.Address.RoomAttributes).Number = n;
                else if (house.Number != null) 
                    (it.Attrs as Pullenti.Address.HouseAttributes).Number = n;
                else if (house.BuildNumber != null) 
                    (it.Attrs as Pullenti.Address.HouseAttributes).BuildNumber = n;
                else 
                    (it.Attrs as Pullenti.Address.HouseAttributes).StroenNumber = n;
                addr.AdditionalItems.Add(it);
                if (room != null) 
                {
                    Pullenti.Ner.Address.AddressReferent a = new Pullenti.Ner.Address.AddressReferent();
                    if (room.Typ == Pullenti.Address.RoomType.Flat) 
                        a.Flat = n;
                    else if (room.Typ == Pullenti.Address.RoomType.Space) 
                        a.Space = n;
                    else if (room.Typ == Pullenti.Address.RoomType.Office) 
                        a.Office = n;
                    else if (room.Typ == Pullenti.Address.RoomType.Pavilion) 
                        a.Pavilion = n;
                    else if (room.Typ == Pullenti.Address.RoomType.Panty) 
                        a.Pantry = n;
                    else 
                        continue;
                    Pullenti.Address.GarObject gg = _findRoomNew(ah, rinh, a);
                    if (gg != null) 
                        it.Gars.Add(gg);
                }
                else 
                {
                    Pullenti.Ner.Address.AddressReferent a = new Pullenti.Ner.Address.AddressReferent();
                    if (house.Typ == Pullenti.Address.HouseType.Plot) 
                        a.Plot = n;
                    else if (house.Number != null) 
                        a.House = n;
                    else if (house.BuildNumber != null) 
                        a.Corpus = n;
                    else 
                        a.Building = n;
                    List<Pullenti.Address.GarObject> gg = _findHousesNew(ah, hinstr, a, 0);
                    if (gg != null) 
                        it.Gars.AddRange(gg);
                }
            }
            return true;
        }
        public static Pullenti.Address.DetailType CreateDirDetails(Pullenti.Ner.Address.AddressReferent ar, out string par)
        {
            par = ar.GetStringValue(Pullenti.Ner.Address.AddressReferent.ATTR_DETAILPARAM);
            return _createDirDetails(ar.Detail);
        }
        static Pullenti.Address.DetailType _createDirDetails(Pullenti.Ner.Address.AddressDetailType dt)
        {
            Pullenti.Address.DetailType ty = Pullenti.Address.DetailType.Undefined;
            if (dt == Pullenti.Ner.Address.AddressDetailType.Near) 
                ty = Pullenti.Address.DetailType.Near;
            else if (dt == Pullenti.Ner.Address.AddressDetailType.East) 
                ty = Pullenti.Address.DetailType.East;
            else if (dt == Pullenti.Ner.Address.AddressDetailType.North) 
                ty = Pullenti.Address.DetailType.North;
            else if (dt == Pullenti.Ner.Address.AddressDetailType.NorthEast) 
                ty = Pullenti.Address.DetailType.NorthEast;
            else if (dt == Pullenti.Ner.Address.AddressDetailType.NorthWest) 
                ty = Pullenti.Address.DetailType.NorthWest;
            else if (dt == Pullenti.Ner.Address.AddressDetailType.South) 
                ty = Pullenti.Address.DetailType.South;
            else if (dt == Pullenti.Ner.Address.AddressDetailType.SouthEast) 
                ty = Pullenti.Address.DetailType.SouthEast;
            else if (dt == Pullenti.Ner.Address.AddressDetailType.SouthWest) 
                ty = Pullenti.Address.DetailType.SouthWest;
            else if (dt == Pullenti.Ner.Address.AddressDetailType.West) 
                ty = Pullenti.Address.DetailType.West;
            else if (dt == Pullenti.Ner.Address.AddressDetailType.Range) 
                ty = Pullenti.Address.DetailType.KmRange;
            else if (dt == Pullenti.Ner.Address.AddressDetailType.Central) 
                ty = Pullenti.Address.DetailType.Central;
            else if (dt == Pullenti.Ner.Address.AddressDetailType.Left) 
                ty = Pullenti.Address.DetailType.Left;
            else if (dt == Pullenti.Ner.Address.AddressDetailType.Right) 
                ty = Pullenti.Address.DetailType.Right;
            return ty;
        }
        public static void ProcessOtherDetails(Pullenti.Address.TextAddress addr, Pullenti.Ner.Address.AddressReferent ar)
        {
            if (ar.Floor != null) 
            {
                if (!addr.Params.ContainsKey(Pullenti.Address.ParamType.Floor)) 
                    addr.Params.Add(Pullenti.Address.ParamType.Floor, ar.Floor);
            }
            if (ar.Part != null) 
            {
                if (!addr.Params.ContainsKey(Pullenti.Address.ParamType.Part)) 
                    addr.Params.Add(Pullenti.Address.ParamType.Part, ar.Part);
            }
            if (ar.Genplan != null) 
            {
                if (!addr.Params.ContainsKey(Pullenti.Address.ParamType.Genplan)) 
                    addr.Params.Add(Pullenti.Address.ParamType.Genplan, (ar.Genplan == "0" ? null : ar.Genplan));
            }
            if (ar.DeliveryArea != null) 
            {
                if (!addr.Params.ContainsKey(Pullenti.Address.ParamType.DeliveryArea)) 
                    addr.Params.Add(Pullenti.Address.ParamType.DeliveryArea, (ar.DeliveryArea == "0" ? null : ar.DeliveryArea));
            }
            if (ar.Zip != null) 
            {
                if (!addr.Params.ContainsKey(Pullenti.Address.ParamType.Zip)) 
                    addr.Params.Add(Pullenti.Address.ParamType.Zip, ar.Zip);
            }
            if (ar.PostOfficeBox != null) 
            {
                if (!addr.Params.ContainsKey(Pullenti.Address.ParamType.SubscriberBox)) 
                    addr.Params.Add(Pullenti.Address.ParamType.SubscriberBox, ar.PostOfficeBox);
            }
        }
        public static void TryProcessDetails(Pullenti.Address.TextAddress addr, string details)
        {
            if (string.IsNullOrEmpty(details) || addr.LastItem == null || addr.LastItem.DetailTyp != Pullenti.Address.DetailType.Undefined) 
                return;
            try 
            {
                Pullenti.Ner.AnalysisResult ar0 = Pullenti.Ner.ProcessorService.EmptyProcessor.Process(new Pullenti.Ner.SourceOfAnalysis(details) { UserParams = "ADDRESS" }, null, null);
                for (Pullenti.Ner.Token t = ar0.FirstToken; t != null; t = t.Next) 
                {
                    Pullenti.Ner.Address.Internal.AddressItemToken ait = Pullenti.Ner.Address.Internal.AddressItemToken.TryParsePureItem(t, null, null);
                    if (ait == null || ait.Typ != Pullenti.Ner.Address.Internal.AddressItemType.Detail) 
                        continue;
                    addr.LastItem.DetailTyp = _createDirDetails(ait.DetailType);
                    if (ait.DetailMeters > 0) 
                        addr.LastItem.DetailParam = string.Format("{0}м", ait.DetailMeters);
                    break;
                }
            }
            catch(Exception ex88) 
            {
            }
        }
    }
}