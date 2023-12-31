﻿/*
 * SDK Pullenti Address, version 4.20, august 2023. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Semantic.Core
{
    /// <summary>
    /// Семантические роли
    /// </summary>
    public enum SemanticRole : int
    {
        /// <summary>
        /// Обычная
        /// </summary>
        Common = 0,
        /// <summary>
        /// Агент
        /// </summary>
        Agent = 1,
        /// <summary>
        /// Пациент
        /// </summary>
        Pacient = 2,
        /// <summary>
        /// Сильная связь
        /// </summary>
        Strong = 3,
    }
}