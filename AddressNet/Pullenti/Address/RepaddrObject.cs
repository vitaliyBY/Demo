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

namespace Pullenti.Address
{
    /// <summary>
    /// Адресный элемент из Адрессария
    /// </summary>
    public class RepaddrObject : IComparable<RepaddrObject>
    {
        /// <summary>
        /// Уникальный идентификатор (в рамках репозитория)
        /// </summary>
        public int Id;
        /// <summary>
        /// Родительский идентификатор (м.б. несколько)
        /// </summary>
        public List<int> Parents = null;
        /// <summary>
        /// Идентификаторы дочерних элементов
        /// </summary>
        public List<int> Children = null;
        /// <summary>
        /// Вариант текстового нормализованного представления
        /// </summary>
        public string Spelling;
        /// <summary>
        /// Уровень объекта
        /// </summary>
        public AddrLevel Level;
        /// <summary>
        /// Тип объекта (м.б. несколько вариантов)
        /// </summary>
        public List<string> Types = new List<string>();
        /// <summary>
        /// Идентификаторы (Guid) объектов ГАР, если есть
        /// </summary>
        public List<string> GarGuids;
        public override string ToString()
        {
            return Spelling;
        }
        public void OutInfo(StringBuilder res)
        {
            res.AppendFormat("Уникальный ID: {0}\r\n", Id);
            res.AppendFormat("Нормализация: {0}\r\n", Spelling);
            if (Level != AddrLevel.Undefined) 
                res.AppendFormat("Уровень: {0} - {1}\r\n", (int)Level, AddressHelper.GetAddrLevelString(Level));
            if (GarGuids != null) 
            {
                foreach (string g in GarGuids) 
                {
                    res.AppendFormat("ГАР-объект: {0}\r\n", g);
                }
            }
        }
        public int CompareTo(RepaddrObject other)
        {
            int l1 = (int)Level;
            if (Level == AddrLevel.Country) 
                l1 = 0;
            int l2 = (int)other.Level;
            if (other.Level == AddrLevel.Country) 
                l2 = 0;
            if (l1 < l2) 
                return -1;
            if (l1 > l2) 
                return 1;
            return string.Compare(Spelling, other.Spelling);
        }
    }
}