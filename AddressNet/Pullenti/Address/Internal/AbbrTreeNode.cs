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
    class AbbrTreeNode
    {
        public Dictionary<char, AbbrTreeNode> Children;
        public int Len;
        public Dictionary<string, string> Corrs = null;
        public override string ToString()
        {
            if (Corrs != null) 
            {
                foreach (KeyValuePair<string, string> kp in Corrs) 
                {
                    return string.Format("{0}->{1}", kp.Key, kp.Value);
                }
            }
            return "?";
        }
        public AbbrTreeNode Find(string str, int i)
        {
            AbbrTreeNode tn = this;
            for (int j = i; j < str.Length; j++) 
            {
                if (tn.Children == null) 
                    break;
                AbbrTreeNode tn1;
                if (!tn.Children.TryGetValue(str[j], out tn1)) 
                    break;
                tn = tn1;
            }
            if (tn.Corrs != null) 
                return tn;
            return null;
        }
        public void Add(string str, int i, string corr, string ty)
        {
            if (i < str.Length) 
            {
                AbbrTreeNode tn = null;
                if (Children != null) 
                {
                    if (!Children.TryGetValue(str[i], out tn)) 
                        tn = null;
                }
                if (tn == null) 
                {
                    if (Children == null) 
                        Children = new Dictionary<char, AbbrTreeNode>();
                    Children.Add(str[i], (tn = new AbbrTreeNode() { Len = i + 1 }));
                }
                tn.Add(str, i + 1, corr, ty);
            }
            else 
            {
                if (Corrs == null) 
                    Corrs = new Dictionary<string, string>();
                if (!Corrs.ContainsKey(ty)) 
                    Corrs.Add(ty, corr);
            }
        }
    }
}