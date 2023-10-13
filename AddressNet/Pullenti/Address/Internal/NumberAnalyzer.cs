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
    public class NumberAnalyzer
    {
        public List<NumberItem> Items = new List<NumberItem>();
        public override string ToString()
        {
            if (Items.Count == 0) 
                return "?";
            if (Items.Count == 1) 
                return Items[0].ToString();
            StringBuilder res = new StringBuilder();
            res.Append(Items[0].ToString());
            for (int i = 1; i < Items.Count; i++) 
            {
                res.AppendFormat(", {0}", Items[i].ToString());
            }
            return res.ToString();
        }
        public double CalcCoef(NumberAnalyzer num)
        {
            double res = (double)0;
            foreach (NumberItem it in num.Items) 
            {
                it.Twix = null;
            }
            foreach (NumberItem it in Items) 
            {
                it.Twix = null;
                NumberItem best = null;
                double max = (double)0;
                foreach (NumberItem it1 in num.Items) 
                {
                    if (it1.Twix == null) 
                    {
                        double co = it.EqualCoef(it1);
                        if (co > max) 
                        {
                            best = it1;
                            max = co;
                        }
                    }
                }
                if (best == null) 
                    continue;
                if (best.Twix != null) 
                    continue;
                it.Twix = best;
                best.Twix = it;
                res += max;
            }
            if (res == 0) 
                return 0;
            int cou = 0;
            foreach (NumberItem it in Items) 
            {
                if (it.Value == "0") 
                    continue;
                cou++;
                if (it.Twix == null) 
                {
                    if (it == Items[0]) 
                        return 0;
                    res /= 2;
                }
                else if (Items.IndexOf(it) != num.Items.IndexOf(it.Twix)) 
                    res /= 2;
            }
            foreach (NumberItem it in num.Items) 
            {
                if (it.Value == "0") 
                    continue;
                cou++;
                if (it.Twix == null) 
                {
                    if (it == num.Items[0]) 
                        return 0;
                    res /= 2;
                }
            }
            if (cou == 0) 
                cou = 1;
            return res;
        }
        public static NumberAnalyzer TryParseReferent(Pullenti.Ner.Address.AddressReferent ar, bool house)
        {
            NumberAnalyzer res = new NumberAnalyzer();
            if (house) 
            {
                if (ar.HouseOrPlot != null) 
                {
                    List<NumberItem> nums = NumberItem.Parse(ar.HouseOrPlot, null, NumberItemClass.Undefined);
                    if (nums != null) 
                        res.Items.AddRange(nums);
                }
                else if (ar.Plot != null) 
                {
                    List<NumberItem> nums = NumberItem.Parse(ar.Plot, "уч.", NumberItemClass.Plot);
                    if (nums != null) 
                        res.Items.AddRange(nums);
                }
                else if (ar.Box != null) 
                {
                    List<NumberItem> nums = NumberItem.Parse(ar.Box, "гар.", NumberItemClass.Garage);
                    if (nums != null) 
                        res.Items.AddRange(nums);
                }
                else 
                {
                    if (ar.House != null) 
                    {
                        Pullenti.Ner.Address.AddressHouseType ty = ar.HouseType;
                        List<NumberItem> nums = NumberItem.Parse(ar.House, (ty == Pullenti.Ner.Address.AddressHouseType.Estate ? "влад." : (ty == Pullenti.Ner.Address.AddressHouseType.HouseEstate ? "дмвлд." : "д.")), NumberItemClass.House);
                        if (nums != null) 
                            res.Items.AddRange(nums);
                    }
                    if (ar.Corpus != null) 
                    {
                        List<NumberItem> nums = NumberItem.Parse(ar.Corpus, "корп.", NumberItemClass.House);
                        if (nums != null) 
                            res.Items.AddRange(nums);
                    }
                    if (ar.CorpusOrFlat != null) 
                    {
                        List<NumberItem> nums = NumberItem.Parse(ar.CorpusOrFlat, "корп.", NumberItemClass.House);
                        if (nums != null) 
                        {
                            nums[0].CanBeFlat = true;
                            res.Items.AddRange(nums);
                        }
                    }
                    if (ar.Building != null) 
                    {
                        Pullenti.Ner.Address.AddressBuildingType ty = ar.BuildingType;
                        List<NumberItem> nums = NumberItem.Parse(ar.Building, (ty == Pullenti.Ner.Address.AddressBuildingType.Construction ? "сооруж." : (ty == Pullenti.Ner.Address.AddressBuildingType.Liter ? "лит." : "стр.")), NumberItemClass.House);
                        if (nums != null) 
                            res.Items.AddRange(nums);
                    }
                }
            }
            else 
            {
                Pullenti.Address.RoomAttributes attr = HouseRoomHelper.CreateApartmentAttrs(ar, null);
                if (attr != null) 
                {
                    List<NumberItem> nums = NumberItem.Parse(attr.Number, Pullenti.Address.AddressHelper.GetRoomTypeString(attr.Typ, true), (attr.Typ == Pullenti.Address.RoomType.Carplace ? NumberItemClass.Carplace : (attr.Typ == Pullenti.Address.RoomType.Flat ? NumberItemClass.Flat : NumberItemClass.Space)));
                    if (nums == null) 
                        return null;
                    res.Items.AddRange(nums);
                }
            }
            if (res.Items.Count == 0) 
                return null;
            return res;
        }
        public static NumberAnalyzer TryParseHO(Pullenti.Address.Internal.Gar.HouseObject ho)
        {
            NumberAnalyzer res = new NumberAnalyzer();
            if (ho.HouseNumber != null) 
            {
                List<NumberItem> nums = NumberItem.Parse(ho.HouseNumber, (ho.HouseTyp == 1 ? "влад." : (ho.HouseTyp == 2 ? "д." : (ho.HouseTyp == 3 ? "дмвлд." : (ho.HouseTyp == 4 ? "гар." : (ho.HouseTyp == 5 ? "уч." : "д."))))), (ho.HouseTyp == 4 ? NumberItemClass.Garage : (ho.HouseTyp == 5 ? NumberItemClass.Plot : NumberItemClass.House)));
                if (nums != null) 
                    res.Items.AddRange(nums);
            }
            if (ho.BuildNumber != null) 
            {
                List<NumberItem> nums = NumberItem.Parse(ho.BuildNumber, "корп.", NumberItemClass.House);
                if (nums != null) 
                    res.Items.AddRange(nums);
            }
            if (ho.StrucNumber != null) 
            {
                List<NumberItem> nums = NumberItem.Parse(ho.StrucNumber, (ho.StrucTyp == 2 ? "сооруж." : (ho.HouseTyp == 3 ? "лит." : "стр.")), NumberItemClass.House);
                if (nums != null) 
                    res.Items.AddRange(nums);
            }
            return res;
        }
        public static NumberAnalyzer TryParseRO(Pullenti.Address.Internal.Gar.RoomObject ro)
        {
            List<NumberItem> nums = NumberItem.Parse(ro.Number, Pullenti.Address.AddressHelper.GetRoomTypeString(ro.Typ, true), (ro.Typ == Pullenti.Address.RoomType.Carplace ? NumberItemClass.Carplace : (ro.Typ == Pullenti.Address.RoomType.Flat ? NumberItemClass.Flat : NumberItemClass.Space)));
            if (nums == null) 
                return null;
            NumberAnalyzer res = new NumberAnalyzer();
            res.Items.AddRange(nums);
            return res;
        }
    }
}