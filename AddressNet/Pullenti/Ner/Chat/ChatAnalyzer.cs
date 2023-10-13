/*
 * SDK Pullenti Address, version 4.20, august 2023. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Collections;
using System.Collections.Generic;

namespace Pullenti.Ner.Chat
{
    public class ChatAnalyzer : Pullenti.Ner.Analyzer
    {
        public const string ANALYZER_NAME = "CHAT";
        public override string Name
        {
            get
            {
                return ANALYZER_NAME;
            }
        }
        public override string Caption
        {
            get
            {
                return "Элемент диалога";
            }
        }
        public override string Description
        {
            get
            {
                return "";
            }
        }
        public override Pullenti.Ner.Analyzer Clone()
        {
            return new ChatAnalyzer();
        }
        public override ICollection<Pullenti.Ner.Metadata.ReferentClass> TypeSystem
        {
            get
            {
                return new Pullenti.Ner.Metadata.ReferentClass[] {Pullenti.Ner.Chat.Internal.MetaChat.GlobalMeta};
            }
        }
        public override Dictionary<string, byte[]> Images
        {
            get
            {
                Dictionary<string, byte[]> res = new Dictionary<string, byte[]>();
                res.Add(Pullenti.Ner.Chat.Internal.MetaChat.ImageId, Pullenti.Ner.Business.Internal.ResourceHelper.GetBytes("chat.png"));
                return res;
            }
        }
        public override Pullenti.Ner.Referent CreateReferent(string type)
        {
            if (type == ChatReferent.OBJ_TYPENAME) 
                return new ChatReferent();
            return null;
        }
        public override int ProgressWeight
        {
            get
            {
                return 1;
            }
        }
        public override bool IsSpecific
        {
            get
            {
                return true;
            }
        }
        public override void Process(Pullenti.Ner.Core.AnalysisKit kit)
        {
            Pullenti.Ner.Core.AnalyzerData ad = kit.GetAnalyzerData(this);
            List<Pullenti.Ner.Chat.Internal.ChatItemToken> toks = new List<Pullenti.Ner.Chat.Internal.ChatItemToken>();
            for (Pullenti.Ner.Token t = kit.FirstToken; t != null; t = t.Next) 
            {
                Pullenti.Ner.Chat.Internal.ChatItemToken cit = Pullenti.Ner.Chat.Internal.ChatItemToken.TryParse(t);
                if (cit == null) 
                    continue;
                toks.Add(cit);
                t = cit.EndToken;
            }
            for (int i = 0; i < (toks.Count - 1); i++) 
            {
                if (((toks[i].Typ == ChatType.Accept || toks[i].Typ == ChatType.Cancel)) && _canMerge(toks[i], toks[i + 1])) 
                {
                    if (toks[i + 1].Typ == toks[i].Typ) 
                    {
                        toks[i].EndToken = toks[i + 1].EndToken;
                        toks.RemoveAt(i + 1);
                        i--;
                        continue;
                    }
                    if (toks[i + 1].Typ == ChatType.Cancel || ((toks[i + 1].Typ == ChatType.Verb && toks[i + 1].Not))) 
                    {
                        toks[i + 1].BeginToken = toks[i].BeginToken;
                        toks.RemoveAt(i);
                        i--;
                        continue;
                    }
                }
            }
            foreach (Pullenti.Ner.Chat.Internal.ChatItemToken cit in toks) 
            {
                ChatReferent cr = new ChatReferent() { Typ = cit.Typ };
                if (cit.Value != null) 
                    cr.Value = cit.Value;
                if (cit.VTyp != VerbType.Undefined) 
                    cr.AddVerbType(cit.VTyp);
                if (cit.Not) 
                    cr.Not = true;
                cr = ad.RegisterReferent(cr) as ChatReferent;
                Pullenti.Ner.ReferentToken rt = new Pullenti.Ner.ReferentToken(cr, cit.BeginToken, cit.EndToken);
                kit.EmbedToken(rt);
            }
        }
        static bool _canMerge(Pullenti.Ner.Chat.Internal.ChatItemToken t1, Pullenti.Ner.Chat.Internal.ChatItemToken t2)
        {
            for (Pullenti.Ner.Token t = t1.EndToken.Next; t != null && (t.EndChar < t2.BeginChar); t = t.Next) 
            {
                if (!(t is Pullenti.Ner.TextToken)) 
                    return false;
                if (t.LengthChar < 2) 
                    continue;
                Pullenti.Morph.MorphClass mc = t.GetMorphClassInDictionary();
                if (((mc.IsAdverb || mc.IsPreposition || mc.IsPronoun) || mc.IsPersonalPronoun || mc.IsMisc) || mc.IsConjunction) 
                    continue;
                return false;
            }
            return true;
        }
        static bool m_Inited;
        public static void Initialize()
        {
            if (m_Inited) 
                return;
            m_Inited = true;
            try 
            {
                Pullenti.Ner.Chat.Internal.MetaChat.Initialize();
                Pullenti.Ner.Core.Termin.AssignAllTextsAsNormal = true;
                Pullenti.Ner.Chat.Internal.ChatItemToken.Initialize();
                Pullenti.Ner.Core.Termin.AssignAllTextsAsNormal = false;
            }
            catch(Exception ex) 
            {
                throw new Exception(ex.Message, ex);
            }
            Pullenti.Ner.ProcessorService.RegisterAnalyzer(new ChatAnalyzer());
        }
    }
}