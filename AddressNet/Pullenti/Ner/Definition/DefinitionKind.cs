﻿/*
 * SDK Pullenti Address, version 4.20, august 2023. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Ner.Definition
{
    /// <summary>
    /// Тип тезиса
    /// </summary>
    public enum DefinitionKind : int
    {
        /// <summary>
        /// Непонятно
        /// </summary>
        Undefined,
        /// <summary>
        /// Просто утрерждение
        /// </summary>
        Assertation,
        /// <summary>
        /// Строгое определение
        /// </summary>
        Definition,
        /// <summary>
        /// Отрицание
        /// </summary>
        Negation,
    }
}