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
    /// Параметры обработки текста
    /// </summary>
    public class ProcessTextParams
    {
        /// <summary>
        /// Регион(ы) по умолчанию (используются, если в тексте не задан, а 
        /// указан только населённый пункт). 
        /// Задаются номерами регионов (первые 2 цифры КЛАДР). Например, 77 - Москва, 47 - Ленинградская область.
        /// </summary>
        public List<int> DefaultRegions = new List<int>();
        /// <summary>
        /// Объект по умолчанию. Используется, если в тексте недостаточно информации. 
        /// Например, если адрес начинается с улицы, то привязка будет только если DefaultObject 
        /// указывает на город или населенный пункт.
        /// </summary>
        public GarObject DefaultObject = null;
        public override string ToString()
        {
            StringBuilder tmp = new StringBuilder();
            if (DefaultRegions.Count == 1) 
                tmp.AppendFormat("Регион: {0}", DefaultRegions[0]);
            else if (DefaultRegions.Count > 0) 
            {
                tmp.AppendFormat("Регионы: {0}", DefaultRegions[0]);
                for (int i = 1; i < DefaultRegions.Count; i++) 
                {
                    tmp.AppendFormat(",{0}", DefaultRegions[i]);
                }
            }
            if (DefaultObject != null) 
                tmp.AppendFormat(" Объект: {0}", DefaultObject.ToString());
            if (tmp.Length == 0) 
                tmp.Append("Нет");
            return tmp.ToString();
        }
    }
}