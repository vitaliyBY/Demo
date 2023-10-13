/*
 * SDK Pullenti Address, version 4.20, august 2023. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Ner.Business
{
    /// <summary>
    /// Типы бизнес-фактов
    /// </summary>
    public enum BusinessFactKind : int
    {
        Undefined = 0,
        /// <summary>
        /// Создан
        /// </summary>
        Create,
        /// <summary>
        /// Упразднён
        /// </summary>
        Delete,
        /// <summary>
        /// Приобретать, покупать
        /// </summary>
        Get,
        /// <summary>
        /// Продавать
        /// </summary>
        Sell,
        /// <summary>
        /// Владеть, иметь
        /// </summary>
        Have,
        /// <summary>
        /// Прибыль, доход
        /// </summary>
        Profit,
        /// <summary>
        /// Убытки
        /// </summary>
        Damages,
        /// <summary>
        /// Соглашение
        /// </summary>
        Agreement,
        /// <summary>
        /// Дочернее предприятие
        /// </summary>
        Subsidiary,
        /// <summary>
        /// Финансировать, спонсировать
        /// </summary>
        Finance,
        /// <summary>
        /// Судебный иск
        /// </summary>
        Lawsuit,
    }
}