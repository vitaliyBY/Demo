﻿/*
 * SDK Pullenti Address, version 4.20, august 2023. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Ner.Person.Internal
{
    public class PersonAttrTermin : Pullenti.Ner.Core.Termin
    {
        public PersonAttrTermin(string v, Pullenti.Morph.MorphLang lang = null) : base(null, lang, false)
        {
            this.InitByNormalText(v, lang);
        }
        public PersonAttrTerminType Typ = PersonAttrTerminType.Other;
        public PersonAttrTerminType2 Typ2 = PersonAttrTerminType2.Undefined;
        public bool CanBeUniqueIdentifier;
        public int CanHasPersonAfter;
        public bool CanBeSameSurname;
        public bool CanBeIndependant;
        public bool IsBoss;
        public bool IsKin;
        public bool IsMilitaryRank;
        public bool IsNation;
        public bool IsPost;
        public bool IsProfession;
        public bool IsDoubt;
    }
}