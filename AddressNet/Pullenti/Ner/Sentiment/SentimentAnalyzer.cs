/*
 * SDK Pullenti Address, version 4.20, august 2023. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Collections;
using System.Collections.Generic;

namespace Pullenti.Ner.Sentiment
{
    /// <summary>
    /// Анализатор для сентиментов (эмоциональная оценка)
    /// </summary>
    public class SentimentAnalyzer : Pullenti.Ner.Analyzer
    {
        public const string ANALYZER_NAME = "SENTIMENT";
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
                return "Сентиментный анализ";
            }
        }
        public override string Description
        {
            get
            {
                return "Выделение тональных объектов";
            }
        }
        public override Pullenti.Ner.Analyzer Clone()
        {
            return new SentimentAnalyzer();
        }
        public override ICollection<Pullenti.Ner.Metadata.ReferentClass> TypeSystem
        {
            get
            {
                return new Pullenti.Ner.Metadata.ReferentClass[] {Pullenti.Ner.Sentiment.Internal.MetaSentiment.GlobalMeta};
            }
        }
        public override Dictionary<string, byte[]> Images
        {
            get
            {
                Dictionary<string, byte[]> res = new Dictionary<string, byte[]>();
                res.Add(Pullenti.Ner.Sentiment.Internal.MetaSentiment.ImageId, Pullenti.Ner.Business.Internal.ResourceHelper.GetBytes("neutral.png"));
                res.Add(Pullenti.Ner.Sentiment.Internal.MetaSentiment.ImageIdGood, Pullenti.Ner.Business.Internal.ResourceHelper.GetBytes("good.png"));
                res.Add(Pullenti.Ner.Sentiment.Internal.MetaSentiment.ImageIdBad, Pullenti.Ner.Business.Internal.ResourceHelper.GetBytes("bad.png"));
                return res;
            }
        }
        public override IEnumerable<string> UsedExternObjectTypes
        {
            get
            {
                return new string[] {"ALL"};
            }
        }
        public override Pullenti.Ner.Referent CreateReferent(string type)
        {
            if (type == SentimentReferent.OBJ_TYPENAME) 
                return new SentimentReferent();
            return null;
        }
        public override bool IsSpecific
        {
            get
            {
                return true;
            }
        }
        public override int ProgressWeight
        {
            get
            {
                return 1;
            }
        }
        public override Pullenti.Ner.Core.AnalyzerData CreateAnalyzerData()
        {
            return new Pullenti.Ner.Core.AnalyzerDataWithOntology();
        }
        public override void Process(Pullenti.Ner.Core.AnalysisKit kit)
        {
            Pullenti.Ner.Core.AnalyzerData ad = kit.GetAnalyzerData(this);
            for (Pullenti.Ner.Token t = kit.FirstToken; t != null; t = t.Next) 
            {
                if (!(t is Pullenti.Ner.TextToken)) 
                    continue;
                if (!t.Chars.IsLetter) 
                    continue;
                Pullenti.Ner.Core.TerminToken tok = m_Termins.TryParse(t, Pullenti.Ner.Core.TerminParseAttr.No);
                if (tok == null) 
                    continue;
                int coef = (int)tok.Termin.Tag;
                if (coef == 0) 
                    continue;
                Pullenti.Ner.Token t0 = t;
                Pullenti.Ner.Token t1 = tok.EndToken;
                for (Pullenti.Ner.Token tt = t.Previous; tt != null; tt = tt.Previous) 
                {
                    Pullenti.Ner.Core.TerminToken tok0 = m_Termins.TryParse(tt, Pullenti.Ner.Core.TerminParseAttr.No);
                    if (tok0 != null) 
                    {
                        if (((int)tok0.Termin.Tag) == 0) 
                        {
                            coef *= 2;
                            t0 = tt;
                            continue;
                        }
                        break;
                    }
                    if ((tt is Pullenti.Ner.TextToken) && (tt as Pullenti.Ner.TextToken).Term == "НЕ") 
                    {
                        coef = -coef;
                        t0 = tt;
                        continue;
                    }
                    break;
                }
                for (Pullenti.Ner.Token tt = t1.Next; tt != null; tt = tt.Next) 
                {
                    if (!(tt is Pullenti.Ner.TextToken)) 
                        break;
                    if (!tt.Chars.IsLetter) 
                        continue;
                    Pullenti.Ner.Core.TerminToken tok0 = m_Termins.TryParse(tt, Pullenti.Ner.Core.TerminParseAttr.No);
                    if (tok0 == null) 
                        break;
                    coef += ((int)tok0.Termin.Tag);
                    tt = (t1 = tok0.EndToken);
                }
                if (coef == 0) 
                    continue;
                SentimentReferent sr = new SentimentReferent();
                sr.Kind = (coef > 0 ? SentimentKind.Positive : SentimentKind.Negative);
                sr.Coef = (coef > 0 ? coef : -coef);
                sr.Spelling = Pullenti.Ner.Core.MiscHelper.GetTextValue(t0, t1, Pullenti.Ner.Core.GetTextAttr.FirstNounGroupToNominative);
                sr = ad.RegisterReferent(sr) as SentimentReferent;
                Pullenti.Ner.ReferentToken rt = new Pullenti.Ner.ReferentToken(sr, t0, t1);
                kit.EmbedToken(rt);
                t = rt;
            }
        }
        static Pullenti.Ner.Core.TerminCollection m_Termins = new Pullenti.Ner.Core.TerminCollection();
        static bool m_Inited;
        public static void Initialize()
        {
            if (m_Inited) 
                return;
            m_Inited = true;
            Pullenti.Ner.Sentiment.Internal.MetaSentiment.Initialize();
            Pullenti.Ner.Core.Termin.AssignAllTextsAsNormal = true;
            try 
            {
                for (int i = 0; i < 2; i++) 
                {
                    string str = Pullenti.Ner.Business.Internal.ResourceHelper.GetString((i == 0 ? "Positives.txt" : "Negatives.txt"));
                    if (str == null) 
                        continue;
                    foreach (string line0 in str.Split('\n')) 
                    {
                        string line = line0.Trim();
                        if (string.IsNullOrEmpty(line)) 
                            continue;
                        int coef = (i == 0 ? 1 : -1);
                        m_Termins.Add(new Pullenti.Ner.Core.Termin(line) { Tag = coef });
                    }
                }
            }
            catch(Exception ex) 
            {
            }
            foreach (string s in new string[] {"ОЧЕНЬ", "СИЛЬНО"}) 
            {
                m_Termins.Add(new Pullenti.Ner.Core.Termin(s) { Tag = (int)0 });
            }
            Pullenti.Ner.Core.Termin.AssignAllTextsAsNormal = false;
            Pullenti.Ner.ProcessorService.RegisterAnalyzer(new SentimentAnalyzer());
        }
    }
}