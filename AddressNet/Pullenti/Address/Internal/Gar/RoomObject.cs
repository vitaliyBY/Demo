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
    public class RoomObject : IComparable<RoomObject>
    {
        public int Id;
        public uint HouseId;
        public string Number;
        public Pullenti.Address.RoomType Typ;
        public bool Actual;
        public string Guid;
        public List<uint> ChildrenIds = null;
        public override string ToString()
        {
            StringBuilder res = new StringBuilder();
            res.AppendFormat("{0}{1}", Pullenti.Address.AddressHelper.GetRoomTypeString(Typ, true), Number);
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
        public int CompareTo(RoomObject other)
        {
            int i = _compNums(Number, other.Number);
            if (i != 0) 
                return i;
            return 0;
        }
    }
}