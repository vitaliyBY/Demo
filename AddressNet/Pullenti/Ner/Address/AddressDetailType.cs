/*
 * SDK Pullenti Address, version 4.20, august 2023. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Ner.Address
{
    /// <summary>
    /// Детализация местоположения
    /// </summary>
    public enum AddressDetailType : int
    {
        Undefined,
        /// <summary>
        /// Пересечение
        /// </summary>
        Cross,
        /// <summary>
        /// Возле
        /// </summary>
        Near,
        /// <summary>
        /// Общежитие
        /// </summary>
        Hostel,
        /// <summary>
        /// На север
        /// </summary>
        North,
        /// <summary>
        /// На юг
        /// </summary>
        South,
        /// <summary>
        /// На запад
        /// </summary>
        West,
        /// <summary>
        /// На восток
        /// </summary>
        East,
        /// <summary>
        /// На сереро-запад
        /// </summary>
        NorthWest,
        /// <summary>
        /// На северо-восток
        /// </summary>
        NorthEast,
        /// <summary>
        /// На юго-запад
        /// </summary>
        SouthWest,
        /// <summary>
        /// На юго-восток
        /// </summary>
        SouthEast,
        /// <summary>
        /// Центральный
        /// </summary>
        Central,
        /// <summary>
        /// Слева
        /// </summary>
        Left,
        /// <summary>
        /// Справа
        /// </summary>
        Right,
        /// <summary>
        /// Это один очень специфический случай
        /// </summary>
        Range,
    }
}