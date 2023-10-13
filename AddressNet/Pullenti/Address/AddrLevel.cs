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
    public enum AddrLevel : int
    {
        /// <summary>
        /// Не определено
        /// </summary>
        Undefined = 0,
        /// <summary>
        /// Страна
        /// </summary>
        Country = 1,
        /// <summary>
        /// Регион
        /// </summary>
        RegionArea = 2,
        /// <summary>
        /// Город-регион
        /// </summary>
        RegionCity = 3,
        /// <summary>
        /// Район (административный, муниципальный, городской)
        /// </summary>
        District = 4,
        /// <summary>
        /// Поселение (сельское, городское)
        /// </summary>
        Settlement = 5,
        /// <summary>
        /// Город
        /// </summary>
        City = 6,
        /// <summary>
        /// Городской район
        /// </summary>
        CityDistrict = 7,
        /// <summary>
        /// Населенный пункт
        /// </summary>
        Locality = 8,
        /// <summary>
        /// Элемент планировочной структуры (территории организаций, СНТ и т.п.)
        /// </summary>
        Territory = 9,
        /// <summary>
        /// Элемент улично-дорожной сети
        /// </summary>
        Street = 10,
        /// <summary>
        /// Участок
        /// </summary>
        Plot = 11,
        /// <summary>
        /// Здание (сооружение)
        /// </summary>
        Building = 12,
        /// <summary>
        /// Помещение, квартира, офис и пр. в здании
        /// </summary>
        Apartment = 13,
        /// <summary>
        /// Комната в помещении
        /// </summary>
        Room = 14,
    }
}