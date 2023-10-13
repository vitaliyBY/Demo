/*
 * SDK Pullenti Address, version 4.20, august 2023. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Ner
{
    /// <summary>
    /// Тип написания числительного NumberToken
    /// </summary>
    public enum NumberSpellingType : int
    {
        Undefined = 0,
        /// <summary>
        /// Цифрами
        /// </summary>
        Digit = 1,
        /// <summary>
        /// Римскими цифрами
        /// </summary>
        Roman = 2,
        /// <summary>
        /// Прописью (словами)
        /// </summary>
        Words = 3,
        /// <summary>
        /// Возраст (летие)
        /// </summary>
        Age = 4,
    }
}