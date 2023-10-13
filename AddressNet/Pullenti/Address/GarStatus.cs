/*
 * SDK Pullenti Address, version 4.20, august 2023. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Address
{
    /// <summary>
    /// Статус анализа наименования ГАР-объекта
    /// </summary>
    public enum GarStatus : int
    {
        /// <summary>
        /// Анализ без замечаний
        /// </summary>
        Ok = 0,
        /// <summary>
        /// Анализ прошёл с замечаниями, возможна непривязка к объекту
        /// </summary>
        Warning = 1,
        /// <summary>
        /// Анализ не прошёл, к объекту привязка производиться не будет
        /// </summary>
        Error = 2,
        /// <summary>
        /// Анализ без замечаний, но в ГАР-объекте слепились 2 разных объекта (нужно дополнительно спец. обработку при проверке)
        /// </summary>
        Ok2 = 3,
    }
}