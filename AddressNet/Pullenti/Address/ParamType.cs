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
    /// Дополнительные параметры адреса
    /// </summary>
    public enum ParamType : int
    {
        Undefined = 0,
        /// <summary>
        /// Очередь (например, в ГСК)
        /// </summary>
        Order,
        /// <summary>
        /// Часть
        /// </summary>
        Part,
        /// <summary>
        /// Этаж
        /// </summary>
        Floor,
        /// <summary>
        /// Генплан
        /// </summary>
        Genplan,
        /// <summary>
        /// Доставочный участок
        /// </summary>
        DeliveryArea,
        /// <summary>
        /// Индекс
        /// </summary>
        Zip,
        /// <summary>
        /// Абон.ящик
        /// </summary>
        SubscriberBox,
    }
}