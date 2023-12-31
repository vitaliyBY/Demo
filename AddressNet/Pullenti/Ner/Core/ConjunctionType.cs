﻿/*
 * SDK Pullenti Address, version 4.20, august 2023. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Ner.Core
{
    /// <summary>
    /// Типы союзов и служебных слов
    /// </summary>
    public enum ConjunctionType : int
    {
        /// <summary>
        /// Неопределено
        /// </summary>
        Undefined,
        /// <summary>
        /// Запятая
        /// </summary>
        Comma,
        /// <summary>
        /// И
        /// </summary>
        And,
        /// <summary>
        /// Или
        /// </summary>
        Or,
        /// <summary>
        /// ни ... ни ...
        /// </summary>
        Not,
        /// <summary>
        /// Но
        /// </summary>
        But,
        /// <summary>
        /// Если
        /// </summary>
        If,
        /// <summary>
        /// То
        /// </summary>
        Then,
        /// <summary>
        /// Иначе
        /// </summary>
        Else,
        /// <summary>
        /// Когда
        /// </summary>
        When,
        /// <summary>
        /// Потому что
        /// </summary>
        Because,
        /// <summary>
        /// Включая
        /// </summary>
        Include,
        /// <summary>
        /// Исключая
        /// </summary>
        Except,
    }
}