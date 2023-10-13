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
    /// Тип детализирующего указателя
    /// </summary>
    public enum DetailType : int
    {
        /// <summary>
        /// Не определено
        /// </summary>
        Undefined,
        /// <summary>
        /// Около, в районе
        /// </summary>
        Near,
        /// <summary>
        /// Направление на север
        /// </summary>
        North,
        /// <summary>
        /// Направление на восток
        /// </summary>
        East,
        /// <summary>
        /// Направление на юг
        /// </summary>
        South,
        /// <summary>
        /// Направление на запад
        /// </summary>
        West,
        /// <summary>
        /// Направление на северо-запад
        /// </summary>
        NorthWest,
        /// <summary>
        /// Направление на северо-восток
        /// </summary>
        NorthEast,
        /// <summary>
        /// Направление на юго-запад
        /// </summary>
        SouthWest,
        /// <summary>
        /// Направление на юго-восток
        /// </summary>
        SouthEast,
        /// <summary>
        /// Центральная часть
        /// </summary>
        Central,
        /// <summary>
        /// Левая часть
        /// </summary>
        Left,
        /// <summary>
        /// Правая часть
        /// </summary>
        Right,
        /// <summary>
        /// Километровый диапазон (км.455+990-км.456+830)
        /// </summary>
        KmRange,
    }
}