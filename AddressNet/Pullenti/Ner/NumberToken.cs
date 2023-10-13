﻿/*
 * SDK Pullenti Address, version 4.20, august 2023. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.IO;
using System.Text;

namespace Pullenti.Ner
{
    /// <summary>
    /// Метатокен - число (числительное). Причём задаваемое не только цифрами, но и словами, возможно, римская запись и др. 
    /// Для получения см. методы NumberHelper.
    /// </summary>
    public class NumberToken : MetaToken
    {
        public NumberToken(Token begin, Token end, string val, NumberSpellingType typ, Pullenti.Ner.Core.AnalysisKit kit = null) : base(begin, end, kit)
        {
            Value = val;
            Typ = typ;
        }
        /// <summary>
        /// Числовое значение, представленное строкой. Если действительное, то с точкой - разделителем дробных. 
        /// Может быть сколь угодно большим, что не умещаться в системные числовые типы long или double.
        /// </summary>
        public string Value
        {
            get
            {
                return m_Value;
            }
            set
            {
                m_Value = value ?? "";
                if (m_Value.Length > 2 && m_Value.EndsWith(".0")) 
                    m_Value = m_Value.Substring(0, m_Value.Length - 2);
                while (m_Value.Length > 1 && m_Value[0] == '0' && m_Value[1] != '.') 
                {
                    m_Value = m_Value.Substring(1);
                }
                int n;
                if (int.TryParse(m_Value, out n)) 
                    m_IntVal = n;
                else 
                    m_IntVal = null;
                double? d = Pullenti.Ner.Core.NumberHelper.StringToDouble(m_Value);
                if (d == null) 
                    m_RealVal = double.NaN;
                else 
                    m_RealVal = d.Value;
            }
        }
        string m_Value;
        int? m_IntVal;
        double m_RealVal;
        /// <summary>
        /// Целочисленное 32-х битное значение. 
        /// Число Value может быть большое и не умещаться в Int, тогда вернёт null. 
        /// Если есть дробная часть, то тоже вернёт null. 
        /// Long не используется, так как не поддерживается в Javascript. Если что - напрямую работайте с Value.
        /// </summary>
        public int? IntValue
        {
            get
            {
                return m_IntVal;
            }
            set
            {
                Value = value.ToString();
            }
        }
        /// <summary>
        /// Получить действительное значение из Value. Если не удалось, то NaN.
        /// </summary>
        public double RealValue
        {
            get
            {
                return m_RealVal;
            }
            set
            {
                Value = Pullenti.Ner.Core.NumberHelper.DoubleToString(value);
            }
        }
        /// <summary>
        /// Тип числительного
        /// </summary>
        public NumberSpellingType Typ = NumberSpellingType.Digit;
        public override string ToString()
        {
            StringBuilder res = new StringBuilder();
            res.AppendFormat("{0} {1}", Value, Typ.ToString());
            if (Morph != null) 
                res.AppendFormat(" {0}", Morph.ToString());
            return res.ToString();
        }
        public override string GetNormalCaseText(Pullenti.Morph.MorphClass mc = null, Pullenti.Morph.MorphNumber num = Pullenti.Morph.MorphNumber.Undefined, Pullenti.Morph.MorphGender gender = Pullenti.Morph.MorphGender.Undefined, bool keepChars = false)
        {
            return Value.ToString();
        }
        internal override void Serialize(Stream stream)
        {
            base.Serialize(stream);
            Pullenti.Ner.Core.Internal.SerializerHelper.SerializeString(stream, m_Value);
            Pullenti.Ner.Core.Internal.SerializerHelper.SerializeInt(stream, (int)Typ);
        }
        internal override void Deserialize(Stream stream, Pullenti.Ner.Core.AnalysisKit kit, int vers)
        {
            base.Deserialize(stream, kit, vers);
            if (vers == 0) 
            {
                byte[] buf = new byte[(int)8];
                stream.Read(buf, 0, 8);
                long lo = BitConverter.ToInt64(buf, 0);
                Value = lo.ToString();
            }
            else 
                Value = Pullenti.Ner.Core.Internal.SerializerHelper.DeserializeString(stream);
            Typ = (NumberSpellingType)Pullenti.Ner.Core.Internal.SerializerHelper.DeserializeInt(stream);
        }
        internal void _corrDrob(double val)
        {
            Token t = EndToken.Next;
            if (t == null) 
                return;
            if (t.IsValue("ДЕСЯТИ", null) && t.Next != null && t.Next.IsValue("ТЫСЯЧНЫЙ", "ТИСЯЧНИЙ")) 
            {
                EndToken = t.Next;
                RealValue = ((((double)val) / 10000)) + RealValue;
                return;
            }
            if (t.IsValue("ДЕСЯТИ", null) && t.Next != null && t.Next.IsValue("МИЛЛИОННЫЙ", "МІЛЬЙОННИЙ")) 
            {
                EndToken = t.Next;
                RealValue = ((((double)val) / 10000000)) + RealValue;
                return;
            }
            if (t.IsValue("ДЕСЯТЫЙ", "ДЕСЯТИЙ")) 
            {
                EndToken = t;
                RealValue = ((((double)val) / 10)) + RealValue;
                return;
            }
            if (t.IsValue("СТО", null) && t.Next != null && t.Next.IsValue("ТЫСЯЧНЫЙ", "ТИСЯЧНИЙ")) 
            {
                EndToken = t.Next;
                RealValue = ((((double)val) / 100000)) + RealValue;
                return;
            }
            if (t.IsValue("СТО", null) && t.Next != null && t.Next.IsValue("МИЛЛИОННЫЙ", "МІЛЬЙОННИЙ")) 
            {
                EndToken = t.Next;
                RealValue = ((((double)val) / 100000000)) + RealValue;
                return;
            }
            if (t.IsValue("СОТЫЙ", "СОТИЙ")) 
            {
                EndToken = t;
                RealValue = ((((double)val) / 100)) + RealValue;
                return;
            }
            if (t.IsValue("ТЫСЯЧНЫЙ", "ТИСЯЧНИЙ")) 
            {
                EndToken = t;
                RealValue = ((((double)val) / 1000)) + RealValue;
                return;
            }
            if (t.IsValue("ДЕСЯТИТЫСЯЧНЫЙ", "ДЕСЯТИТИСЯЧНИЙ") || (((t is NumberToken) && (t as NumberToken).Value == "10000"))) 
            {
                EndToken = t;
                RealValue = ((((double)val) / 10000)) + RealValue;
                return;
            }
            if (t.IsValue("СТОТЫСЯЧНЫЙ", "СТОТИСЯЧНИЙ") || (((t is NumberToken) && (t as NumberToken).Value == "100000"))) 
            {
                EndToken = t;
                RealValue = ((((double)val) / 100000)) + RealValue;
                return;
            }
            if (t.IsValue("МИЛЛИОННЫЙ", "МІЛЬЙОННИЙ")) 
            {
                EndToken = t;
                RealValue = ((((double)val) / 1000000)) + RealValue;
                return;
            }
            if (t.IsValue("ДЕСЯТИМИЛЛИОННЫЙ", "ДЕСЯТИМІЛЬЙОННИЙ") || (((t is NumberToken) && (t as NumberToken).Value == "10000000"))) 
            {
                EndToken = t;
                RealValue = ((((double)val) / 10000000)) + RealValue;
                return;
            }
            if (t.IsValue("СТОМИЛЛИОННЫЙ", "СТОМІЛЬЙОННИЙ") || (((t is NumberToken) && (t as NumberToken).Value == "100000000"))) 
            {
                EndToken = t;
                RealValue = ((((double)val) / 10000000)) + RealValue;
                return;
            }
        }
    }
}