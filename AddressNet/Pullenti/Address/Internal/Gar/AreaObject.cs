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
    /// <summary>
    /// Адресный объект ГАР ФИАС
    /// </summary>
    public class AreaObject : IComparable<AreaObject>
    {
        public int Id;
        public List<int> ParentIds = new List<int>();
        public List<int> ParentParentIds;
        public AreaType Typ;
        public List<string> Names = new List<string>();
        public AreaType OldTyp;
        public short Level;
        public bool Actual;
        public byte Region;
        public Pullenti.Address.GarStatus Status;
        public string Guid;
        /// <summary>
        /// Здесь старший бит: 0 - из табл. Address, 1 - из House
        /// </summary>
        public List<uint> ChildrenIds = new List<uint>();
        public const uint HOUSEMASK = 0x80000000;
        public const uint ROOMMASK = 0xC0000000;
        public object Tag;
        public override string ToString()
        {
            StringBuilder res = new StringBuilder();
            if (Id > 0) 
                res.AppendFormat("{0}: ", Id);
            if (!Actual) 
                res.Append("(*) ");
            res.AppendFormat("[{0}] ", Level);
            if (Typ != null) 
                res.AppendFormat("{0} ", Typ.Name);
            if (OldTyp != null) 
                res.AppendFormat("(уст. {0}) ", OldTyp.Name);
            if (Status != Pullenti.Address.GarStatus.Ok) 
                res.AppendFormat("{0} ", Status);
            for (int i = 0; i < Names.Count; i++) 
            {
                res.AppendFormat("{0}{1}", (i > 0 ? "/" : ""), Names[i]);
            }
            return res.ToString();
        }
        public void MergeWith(AreaObject ao)
        {
            if (ao.Actual == Actual || ((Actual && !ao.Actual))) 
            {
                foreach (string n in ao.Names) 
                {
                    if (!Names.Contains(n)) 
                        Names.Add(n);
                }
                if (ao.OldTyp != null && OldTyp == null) 
                    OldTyp = ao.OldTyp;
                else if (Typ != null && ao.Typ != Typ && OldTyp == null) 
                    OldTyp = ao.Typ;
                if (ao.Level > 0 && Level == 0) 
                    Level = ao.Level;
            }
            else if (!Actual && ao.Actual) 
            {
                Actual = true;
                List<string> nams = new List<string>(ao.Names);
                foreach (string n in Names) 
                {
                    if (!nams.Contains(n)) 
                        nams.Add(n);
                }
                Names = nams;
                if (Typ != ao.Typ) 
                {
                    OldTyp = Typ;
                    Typ = ao.Typ;
                }
                Level = ao.Level;
            }
            else 
            {
            }
        }
        public int CompareTo(AreaObject other)
        {
            if (Level < other.Level) 
                return -1;
            if (Level > other.Level) 
                return 1;
            if (Names.Count > 0 && other.Names.Count > 0) 
            {
                int i = HouseObject._compNums(Names[0], other.Names[0]);
                if (i != 0) 
                    return i;
            }
            return 0;
        }
    }
}