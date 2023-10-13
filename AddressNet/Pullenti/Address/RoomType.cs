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
    /// Типы помещений
    /// </summary>
    public enum RoomType : int
    {
        /// <summary>
        /// Не определено
        /// </summary>
        Undefined = 0,
        /// <summary>
        /// Помещение
        /// </summary>
        Space = 1,
        /// <summary>
        /// Квартира
        /// </summary>
        Flat = 2,
        /// <summary>
        /// Офис
        /// </summary>
        Office = 3,
        /// <summary>
        /// Комната
        /// </summary>
        Room = 4,
        /// <summary>
        /// Кладовка
        /// </summary>
        Panty = 6,
        /// <summary>
        /// Павильон
        /// </summary>
        Pavilion = 9,
        /// <summary>
        /// Гараж (вроде это в House ???)
        /// </summary>
        Garage = 13,
        /// <summary>
        /// Машиноместо
        /// </summary>
        Carplace = 100,
    }
}