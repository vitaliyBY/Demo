/*
 * SDK Pullenti Address, version 4.20, august 2023. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Collections;
using System.Collections.Generic;

namespace Pullenti.Address.Internal
{
    class NameChecker : Pullenti.Ner.Geo.Internal.INameChecker
    {
        public bool Check(string name, bool isStreet)
        {
            if (GarHelper.GarIndex == null) 
                return false;
            List<string> vars = new List<string>();
            List<string> vars2 = new List<string>();
            NameAnalyzer.CreateSearchVariants(vars, null, vars2, name, null);
            foreach (string v in vars) 
            {
                if (GarHelper.GarIndex.CheckName(v, isStreet)) 
                    return true;
            }
            foreach (string v in vars2) 
            {
                if (GarHelper.GarIndex.CheckName(v, isStreet)) 
                    return true;
            }
            return false;
        }
    }
}