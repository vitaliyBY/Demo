﻿/*
 * SDK Pullenti Address, version 4.20, august 2023. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Text;

namespace Pullenti.Morph
{
    /// <summary>
    /// Информация о символах токена
    /// </summary>
    public class CharsInfo
    {
        public short Value;
        bool GetValue(int i)
        {
            return ((((Value >> i)) & 1)) != 0;
        }
        void SetValue(int i, bool val)
        {
            if (val) 
                Value |= ((short)(1 << i));
            else 
                Value &= ((short)(~(1 << i)));
        }
        /// <summary>
        /// Все символы в верхнем регистре
        /// </summary>
        public bool IsAllUpper
        {
            get
            {
                return this.GetValue(0);
            }
            set
            {
                this.SetValue(0, value);
            }
        }
        /// <summary>
        /// Все символы в нижнем регистре
        /// </summary>
        public bool IsAllLower
        {
            get
            {
                return this.GetValue(1);
            }
            set
            {
                this.SetValue(1, value);
            }
        }
        /// <summary>
        /// Первый символ в верхнем регистре, остальные в нижнем. 
        /// Для однобуквенной комбинации false.
        /// </summary>
        public bool IsCapitalUpper
        {
            get
            {
                return this.GetValue(2);
            }
            set
            {
                this.SetValue(2, value);
            }
        }
        /// <summary>
        /// Все символы в верхнем регистре, кроме последнего (длина >= 3)
        /// </summary>
        public bool IsLastLower
        {
            get
            {
                return this.GetValue(3);
            }
            set
            {
                this.SetValue(3, value);
            }
        }
        /// <summary>
        /// Это буквы
        /// </summary>
        public bool IsLetter
        {
            get
            {
                return this.GetValue(4);
            }
            set
            {
                this.SetValue(4, value);
            }
        }
        /// <summary>
        /// Это латиница
        /// </summary>
        public bool IsLatinLetter
        {
            get
            {
                return this.GetValue(5);
            }
            set
            {
                this.SetValue(5, value);
            }
        }
        /// <summary>
        /// Это кириллица
        /// </summary>
        public bool IsCyrillicLetter
        {
            get
            {
                return this.GetValue(6);
            }
            set
            {
                this.SetValue(6, value);
            }
        }
        public override string ToString()
        {
            if (!IsLetter) 
                return "Nonletter";
            StringBuilder tmpStr = new StringBuilder();
            if (IsAllUpper) 
                tmpStr.Append("AllUpper");
            else if (IsAllLower) 
                tmpStr.Append("AllLower");
            else if (IsCapitalUpper) 
                tmpStr.Append("CapitalUpper");
            else if (IsLastLower) 
                tmpStr.Append("LastLower");
            else 
                tmpStr.Append("Nonstandard");
            if (IsLatinLetter) 
                tmpStr.Append(" Latin");
            else if (IsCyrillicLetter) 
                tmpStr.Append(" Cyrillic");
            else if (IsLetter) 
                tmpStr.Append(" Letter");
            return tmpStr.ToString();
        }
        /// <summary>
        /// Сравнение на совпадение значений всех полей
        /// </summary>
        /// <param name="obj">сравниваемый объект</param>
        public override bool Equals(object obj)
        {
            if (!(obj is CharsInfo)) 
                return false;
            return Value == ((CharsInfo)obj).Value;
        }
    }
}