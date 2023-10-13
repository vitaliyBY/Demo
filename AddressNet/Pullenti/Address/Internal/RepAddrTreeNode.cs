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
    class RepAddrTreeNode
    {
        public List<RepAddrTreeNodeObj> Objs = null;
        public int LazyPos;
        public bool Loaded;
        public Dictionary<char, RepAddrTreeNode> Children = null;
        public void Unload()
        {
            if (LazyPos == 0) 
                return;
            if (Children != null) 
                Children.Clear();
            Children = null;
            if (Objs != null) 
                Objs.Clear();
            Objs = null;
            Loaded = false;
        }
    }
}