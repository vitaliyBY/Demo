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

namespace Pullenti.Address.Internal.Gar
{
    public class HouseObject : IComparable<HouseObject>
    {
        public int Id;
        public int ParentId;
        public int AltParentId;
        public string Guid;
        public string HouseNumber;
        public string BuildNumber;
        public string StrucNumber;
        public byte HouseTyp;
        public byte StrucTyp;
        public bool Actual;
        public Pullenti.Address.GarStatus Status;
        public List<int> RoomIds = null;
        public object Tag;
        public override string ToString()
        {
            StringBuilder res = new StringBuilder();
            if (HouseNumber != null) 
                res.AppendFormat("{0}{1}", (HouseTyp == 5 ? "уч." : (HouseTyp == 2 ? "д." : (HouseTyp == 1 ? "влад." : (HouseTyp == 3 ? "дмвлд." : (HouseTyp == 4 ? "гараж" : "?"))))), (string.IsNullOrEmpty(HouseNumber) ? "б/н" : HouseNumber));
            if (BuildNumber != null) 
            {
                if (res.Length > 0) 
                    res.Append(' ');
                res.AppendFormat("корп.{0}", (string.IsNullOrEmpty(BuildNumber) ? "б/н" : BuildNumber));
            }
            if (StrucNumber != null) 
            {
                if (res.Length > 0) 
                    res.Append(' ');
                if (StrucTyp == 2) 
                    res.Append("сооруж.");
                else if (StrucTyp == 3) 
                    res.Append("лит.");
                else 
                    res.Append("стр.");
                res.Append((string.IsNullOrEmpty(StrucNumber) ? "б/н" : StrucNumber));
            }
            return res.ToString();
        }
        static int _getInt(string str)
        {
            if (str == null) 
                return 0;
            int res = 0;
            for (int i = 0; i < str.Length; i++) 
            {
                if (char.IsDigit(str[i])) 
                    res = ((res * 10) + ((int)str[i])) - 0x30;
                else 
                    break;
            }
            return res;
        }
        internal static int _compNums(string str1, string str2)
        {
            int n1 = _getInt(str1);
            int n2 = _getInt(str2);
            if (n1 < n2) 
                return -1;
            if (n1 > n2) 
                return 1;
            if (str1 != null && str2 != null) 
                return string.Compare(str1, str2);
            return 0;
        }
        public int CompareTo(HouseObject other)
        {
            int i = _compNums(HouseNumber, other.HouseNumber);
            if (i != 0) 
                return i;
            if ((((i = _compNums(BuildNumber, other.BuildNumber)))) != 0) 
                return i;
            if ((((i = _compNums(StrucNumber, other.StrucNumber)))) != 0) 
                return i;
            return 0;
        }
    }
}