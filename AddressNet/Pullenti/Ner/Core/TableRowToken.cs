﻿/*
 * SDK Pullenti Address, version 4.20, august 2023. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Collections;
using System.Collections.Generic;

namespace Pullenti.Ner.Core
{
    // Токен - строка таблицы из текста
    public class TableRowToken : Pullenti.Ner.MetaToken
    {
        public TableRowToken(Pullenti.Ner.Token b, Pullenti.Ner.Token e) : base(b, e, null)
        {
        }
        public List<TableCellToken> Cells = new List<TableCellToken>();
        internal bool Eor = false;
        internal bool LastRow = false;
        public override string ToString()
        {
            return string.Format("ROW ({0} cells) : {1}", Cells.Count, this.GetSourceText());
        }
    }
}