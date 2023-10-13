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
    /// Типы параметров ГАР
    /// </summary>
    public enum GarParam : int
    {
        /// <summary>
        /// Не определён
        /// </summary>
        Undefined = 0,
        /// <summary>
        /// Уникальный GUID
        /// </summary>
        Guid = 1,
        /// <summary>
        /// Код КЛАДР (тип 10 в ГАР)
        /// </summary>
        KladrCode = 2,
        /// <summary>
        /// Почтовый индекс (тип 5 в ГАР)
        /// </summary>
        PostIndex = 3,
        /// <summary>
        /// Код ОКАТО (тип 6 в ГАР)
        /// </summary>
        Okato = 4,
        /// <summary>
        /// Код ОКТМО (тип 7 в ГАР)
        /// </summary>
        Oktmo = 5,
        /// <summary>
        /// Кадастровый номер (тип 8)
        /// </summary>
        KadasterNumber = 6,
        /// <summary>
        /// Реестровый номер (тип 13)
        /// </summary>
        ReesterNumber = 7,
    }
}