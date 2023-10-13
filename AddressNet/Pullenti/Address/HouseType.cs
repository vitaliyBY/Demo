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
    /// Типы домов и участков
    /// </summary>
    public enum HouseType : int
    {
        /// <summary>
        /// Не определено
        /// </summary>
        Undefined = 0,
        /// <summary>
        /// Владение
        /// </summary>
        Estate = 1,
        /// <summary>
        /// Дом
        /// </summary>
        House = 2,
        /// <summary>
        /// Домовладение
        /// </summary>
        HouseEstate = 3,
        /// <summary>
        /// Специальное строение (типа АЗС)
        /// </summary>
        Special = 4,
        /// <summary>
        /// Гараж
        /// </summary>
        Garage = 5,
        /// <summary>
        /// Участок
        /// </summary>
        Plot = 6,
        /// <summary>
        /// Скважина (для месторождений)
        /// </summary>
        Well = 7,
    }
}