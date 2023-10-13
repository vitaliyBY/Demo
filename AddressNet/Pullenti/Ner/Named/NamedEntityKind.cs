﻿/*
 * SDK Pullenti Address, version 4.20, august 2023. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Ner.Named
{
    /// <summary>
    /// Категории мелких именованных сущностей
    /// </summary>
    public enum NamedEntityKind : int
    {
        /// <summary>
        /// Неопределённая
        /// </summary>
        Undefined,
        /// <summary>
        /// Планеты
        /// </summary>
        Planet,
        /// <summary>
        /// Разные географические объекты (не города) - реки, моря, континенты ...
        /// </summary>
        Location,
        /// <summary>
        /// Памятники и монументы
        /// </summary>
        Monument,
        /// <summary>
        /// Выдающиеся здания
        /// </summary>
        Building,
        /// <summary>
        /// Произведения искусства (книги, фильмы, спектакли ...)
        /// </summary>
        Art,
        /// <summary>
        /// Награды, ордена, титулы (пока не реализовано)
        /// </summary>
        Award,
    }
}