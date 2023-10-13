/*
 * SDK Pullenti Address, version 4.20, august 2023. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Pullenti.Ner.Chat
{
    public class ChatReferent : Pullenti.Ner.Referent
    {
        public ChatReferent() : base(OBJ_TYPENAME)
        {
            InstanceOf = Pullenti.Ner.Chat.Internal.MetaChat.GlobalMeta;
        }
        public const string OBJ_TYPENAME = "CHAT";
        public const string ATTR_TYPE = "TYPE";
        public const string ATTR_VALUE = "VALUE";
        public const string ATTR_NOT = "NOT";
        public const string ATTR_VERBTYPE = "VERBTYPE";
        public override string ToStringEx(bool shortVariant, Pullenti.Morph.MorphLang lang, int lev = 0)
        {
            StringBuilder res = new StringBuilder();
            res.Append(Typ.ToString());
            if (Not) 
                res.Append(" not");
            string val = Value;
            if (val != null) 
                res.AppendFormat(" {0}", val);
            List<VerbType> vty = VerbTypes;
            if (vty.Count > 0) 
            {
                res.Append("[");
                for (int i = 0; i < vty.Count; i++) 
                {
                    if (i > 0) 
                        res.Append(", ");
                    res.Append(vty[i].ToString());
                }
                res.Append("]");
            }
            return res.ToString();
        }
        public ChatType Typ
        {
            get
            {
                string str = this.GetStringValue(ATTR_TYPE);
                if (string.IsNullOrEmpty(str)) 
                    return ChatType.Undefined;
                try 
                {
                    return (ChatType)Enum.Parse(typeof(ChatType), str, true);
                }
                catch(Exception ex793) 
                {
                }
                return ChatType.Undefined;
            }
            set
            {
                this.AddSlot(ATTR_TYPE, value.ToString().ToUpper(), true, 0);
            }
        }
        public bool Not
        {
            get
            {
                return this.GetStringValue(ATTR_NOT) == "true";
            }
            set
            {
                if (!value) 
                    this.AddSlot(ATTR_NOT, null, true, 0);
                else 
                    this.AddSlot(ATTR_NOT, "true", true, 0);
            }
        }
        public string Value
        {
            get
            {
                return this.GetStringValue(ATTR_VALUE);
            }
            set
            {
                this.AddSlot(ATTR_VALUE, value, true, 0);
            }
        }
        public List<VerbType> VerbTypes
        {
            get
            {
                List<VerbType> res = new List<VerbType>();
                foreach (Pullenti.Ner.Slot s in Slots) 
                {
                    if (s.TypeName == ATTR_VERBTYPE) 
                    {
                        try 
                        {
                            res.Add((VerbType)Enum.Parse(typeof(VerbType), s.Value as string, true));
                        }
                        catch(Exception ex794) 
                        {
                        }
                    }
                }
                return res;
            }
        }
        public void AddVerbType(VerbType vt)
        {
            this.AddSlot(ATTR_VERBTYPE, vt.ToString().ToUpper(), false, 0);
        }
        public override bool CanBeEquals(Pullenti.Ner.Referent obj, Pullenti.Ner.Core.ReferentsEqualType typ = Pullenti.Ner.Core.ReferentsEqualType.WithinOneText)
        {
            ChatReferent tr = obj as ChatReferent;
            if (tr == null) 
                return false;
            if (tr.Typ != Typ) 
                return false;
            if (tr.Value != Value) 
                return false;
            if (tr.Not != Not) 
                return false;
            return true;
        }
    }
}