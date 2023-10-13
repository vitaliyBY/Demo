﻿/*
 * SDK Pullenti Address, version 4.20, august 2023. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Ner.Chat
{
    /// <summary>
    /// Тип глагольной формы
    /// </summary>
    public enum VerbType : int
    {
        Undefined,
        /// <summary>
        /// Быть, являться
        /// </summary>
        Be,
        /// <summary>
        /// Иметь
        /// </summary>
        Have,
        /// <summary>
        /// Могу
        /// </summary>
        Can,
        /// <summary>
        /// Должен
        /// </summary>
        Must,
        /// <summary>
        /// Говорить, произносить ...
        /// </summary>
        Say,
        /// <summary>
        /// Звонить
        /// </summary>
        Call,
    }
}