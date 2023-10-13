﻿/*
 * SDK Pullenti Address, version 4.20, august 2023. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Ner.Keyword
{
    /// <summary>
    /// Тип ключевой комбинации
    /// </summary>
    public enum KeywordType : int
    {
        /// <summary>
        /// Неопределён
        /// </summary>
        Undefined = 0,
        /// <summary>
        /// Объект (именная группа)
        /// </summary>
        Object = 1,
        /// <summary>
        /// Именованная сущность
        /// </summary>
        Referent = 2,
        /// <summary>
        /// Предикат (глагол)
        /// </summary>
        Predicate = 3,
        /// <summary>
        /// Автоаннотация всего текста
        /// </summary>
        Annotation = 4,
    }
}