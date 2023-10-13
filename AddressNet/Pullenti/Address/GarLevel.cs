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
    /// Уровень адресного объекта
    /// </summary>
    public enum GarLevel : int
    {
        Undefined = 0,
        /// <summary>
        /// Регион
        /// </summary>
        Region = 1,
        /// <summary>
        /// Административный район
        /// </summary>
        AdminArea = 2,
        /// <summary>
        /// Муниципальный район
        /// </summary>
        MunicipalArea = 3,
        /// <summary>
        /// Сельское/городское поселение
        /// </summary>
        Settlement = 4,
        /// <summary>
        /// Город
        /// </summary>
        City = 5,
        /// <summary>
        /// Населенный пункт
        /// </summary>
        Locality = 6,
        /// <summary>
        /// Район
        /// </summary>
        District = 14,
        /// <summary>
        /// Элемент планировочной структуры
        /// </summary>
        Area = 7,
        /// <summary>
        /// Элемент улично-дорожной сети
        /// </summary>
        Street = 8,
        /// <summary>
        /// Земельный участок
        /// </summary>
        Plot = 9,
        /// <summary>
        /// Здание (сооружение)
        /// </summary>
        Building = 10,
        /// <summary>
        /// Помещение
        /// </summary>
        Room = 11,
        /// <summary>
        /// Машино-место
        /// </summary>
        Carplace = 17,
    }
}