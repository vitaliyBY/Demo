﻿/*
 * SDK Pullenti Address, version 4.20, august 2023. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Collections;
using System.Collections.Generic;

namespace Pullenti.Ner.Measure.Internal
{
    // Простая единица измерения (для составной единицы используется UnitToken)
    public class Unit
    {
        public string NameCyr;
        public string NameLat;
        public string FullnameCyr;
        public string FullnameLat;
        public Pullenti.Ner.Measure.MeasureKind Kind = Pullenti.Ner.Measure.MeasureKind.Undefined;
        public Unit(string nameCyr, string nameLat, string fnameCyr, string fnameLan)
        {
            NameCyr = nameCyr;
            NameLat = nameLat;
            FullnameCyr = fnameCyr;
            FullnameLat = fnameLan;
        }
        public override string ToString()
        {
            return NameCyr;
        }
        public Unit BaseUnit;
        public Unit MultUnit;
        public double BaseMultiplier;
        public UnitsFactors Factor = UnitsFactors.No;
        public List<string> Keywords = new List<string>();
        public List<Unit> Psevdo = new List<Unit>();
    }
}