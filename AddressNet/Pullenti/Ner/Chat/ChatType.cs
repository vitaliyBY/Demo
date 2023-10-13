/*
 * SDK Pullenti Address, version 4.20, august 2023. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Ner.Chat
{
    /// <summary>
    /// Типы диалоговых элементов
    /// </summary>
    public enum ChatType : int
    {
        /// <summary>
        /// Неопределённый
        /// </summary>
        Undefined,
        /// <summary>
        /// Благодарность
        /// </summary>
        Thanks,
        /// <summary>
        /// Вводные слова, мусор и пр., на что не стоит обращать внимание
        /// </summary>
        Misc,
        /// <summary>
        /// Привет
        /// </summary>
        Hello,
        /// <summary>
        /// Пока
        /// </summary>
        Bye,
        /// <summary>
        /// Согласие
        /// </summary>
        Accept,
        /// <summary>
        /// Отказ
        /// </summary>
        Cancel,
        /// <summary>
        /// Занят
        /// </summary>
        Busy,
        /// <summary>
        /// Глагольная группа
        /// </summary>
        Verb,
        /// <summary>
        /// Позже - (например, перезвонить позже)
        /// </summary>
        Later,
        /// <summary>
        /// дата (возможно, относительная)
        /// </summary>
        Date,
        /// <summary>
        /// диапазон дат
        /// </summary>
        DateRange,
        /// <summary>
        /// Просьба повторить
        /// </summary>
        Repeat,
    }
}