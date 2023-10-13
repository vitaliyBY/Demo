/*
 * SDK Pullenti Address, version 4.20, august 2023. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Semantic.Core
{
    /// <summary>
    /// Интерфейс внешней дополнительной онтологии 
    /// (для улучшения качества семантичсекой обработки)
    /// </summary>
    public interface ISemanticOnto
    {
        /// <summary>
        /// Проверка, что в онтологии слова master и slave образуют устойчивую пару
        /// </summary>
        bool CheckLink(string master, string slave);
    }
}