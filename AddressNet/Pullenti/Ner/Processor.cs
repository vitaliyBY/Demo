﻿/*
 * SDK Pullenti Address, version 4.20, august 2023. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace Pullenti.Ner
{
    /// <summary>
    /// Лингвистический процессор
    /// </summary>
    public class Processor : IDisposable
    {
        internal Processor()
        {
        }
        /// <summary>
        /// Добавить анализатор, если его ещё нет
        /// </summary>
        /// <param name="a">экземпляр анализатора</param>
        public void AddAnalyzer(Analyzer a)
        {
            if (a == null || a.Name == null || m_AnalyzersHash.ContainsKey(a.Name)) 
                return;
            m_AnalyzersHash.Add(a.Name, a);
            m_Analyzers.Add(a);
            a.Progress += new ProgressChangedEventHandler(OnProgressHandler);
            a.Cancel += new CancelEventHandler(OnCancel);
        }
        /// <summary>
        /// Удалить анализатор
        /// </summary>
        public void DelAnalyzer(Analyzer a)
        {
            if (!m_AnalyzersHash.ContainsKey(a.Name)) 
                return;
            m_AnalyzersHash.Remove(a.Name);
            m_Analyzers.Remove(a);
            a.Progress -= new ProgressChangedEventHandler(OnProgressHandler);
            a.Cancel -= new CancelEventHandler(OnCancel);
        }
        public void Dispose()
        {
            foreach (Analyzer w in Analyzers) 
            {
                w.Progress -= new ProgressChangedEventHandler(OnProgressHandler);
                w.Cancel -= new CancelEventHandler(OnCancel);
            }
        }
        /// <summary>
        /// Последовательность обработки данных (анализаторы)
        /// </summary>
        public ICollection<Analyzer> Analyzers
        {
            get
            {
                return m_Analyzers;
            }
        }
        List<Analyzer> m_Analyzers = new List<Analyzer>();
        Dictionary<string, Analyzer> m_AnalyzersHash = new Dictionary<string, Analyzer>();
        /// <summary>
        /// Найти анализатор по его имени
        /// </summary>
        public Analyzer FindAnalyzer(string name)
        {
            Analyzer a;
            if (m_AnalyzersHash.TryGetValue(name ?? "", out a)) 
                return a;
            if (string.IsNullOrEmpty(name)) 
                return null;
            foreach (Analyzer aa in ProcessorService.Analyzers) 
            {
                if (aa.Name == name) 
                {
                    a = aa.Clone();
                    a.IgnoreThisAnalyzer = true;
                    m_Analyzers.Add(a);
                    m_AnalyzersHash.Add(name, a);
                    return a;
                }
            }
            return null;
        }
        /// <summary>
        /// Обработать текст
        /// </summary>
        /// <param name="text">входной контейнер текста</param>
        /// <param name="extOntology">внешняя онтология (null - не используется)</param>
        /// <param name="lang">язык (если не задан, то будет определён автоматически)</param>
        /// <return>аналитический контейнер с результатом</return>
        public AnalysisResult Process(SourceOfAnalysis text, ExtOntology extOntology = null, Pullenti.Morph.MorphLang lang = null)
        {
            return this._process(text, false, false, extOntology, lang);
        }
        /// <summary>
        /// Доделать результат, который был сделан другим процессором
        /// </summary>
        /// <param name="ar">то, что было сделано другим процессором</param>
        public void ProcessNext(AnalysisResult ar)
        {
            if (ar == null) 
                return;
            Pullenti.Ner.Core.AnalysisKit kit = new Pullenti.Ner.Core.AnalysisKit() { Processor = this, Ontology = ar.Ontology };
            kit.InitFrom(ar);
            this._process2(kit, ar, false);
            this._createRes(kit, ar, ar.Ontology, false);
            ar.FirstToken = kit.FirstToken;
        }
        internal AnalysisResult _process(SourceOfAnalysis text, bool ontoRegine, bool noLog, ExtOntology extOntology = null, Pullenti.Morph.MorphLang lang = null)
        {
            m_Breaked = false;
            this.PrepareProgress();
            Stopwatch sw0 = Stopwatch.StartNew();
            if (!noLog) 
                this.OnProgressHandler(this, new ProgressChangedEventArgs(0, "Морфологический анализ"));
            Pullenti.Ner.Core.AnalysisKit kit = new Pullenti.Ner.Core.AnalysisKit(text, false, lang, OnProgressHandler) { Ontology = extOntology, Processor = this, OntoRegime = ontoRegine };
            AnalysisResult ar = new AnalysisResult();
            sw0.Stop();
            string msg;
            this.OnProgressHandler(this, new ProgressChangedEventArgs(100, string.Format("Морфологический анализ завершён")));
            int k = 0;
            for (Token t = kit.FirstToken; t != null; t = t.Next) 
            {
                k++;
            }
            if (!noLog) 
            {
                msg = string.Format("Из {0} символов текста выделено {1} термов за {2} ms", text.Text.Length, k, sw0.ElapsedMilliseconds);
                if (!kit.BaseLanguage.IsUndefined) 
                    msg += string.Format(", базовый язык {0}", kit.BaseLanguage.ToString());
                this.OnMessage(msg);
                ar.Log.Add(msg);
                if (text.CrlfCorrectedCount > 0) 
                    ar.Log.Add(string.Format("{0} переходов на новую строку заменены на пробел", text.CrlfCorrectedCount));
                if (kit.FirstToken == null) 
                    ar.Log.Add("Пустой текст");
            }
            sw0.Start();
            if (kit.FirstToken != null) 
                this._process2(kit, ar, noLog);
            if (!ontoRegine) 
                this._createRes(kit, ar, extOntology, noLog);
            sw0.Stop();
            if (!noLog) 
            {
                if (sw0.ElapsedMilliseconds > 5000) 
                {
                    float f = (float)text.Text.Length;
                    f /= sw0.ElapsedMilliseconds;
                    msg = string.Format("Обработка {0} знаков выполнена за {1} ({2} Kb/sec)", text.Text.Length, OutSecs(sw0.ElapsedMilliseconds), f);
                }
                else 
                    msg = string.Format("Обработка {0} знаков выполнена за {1}", text.Text.Length, OutSecs(sw0.ElapsedMilliseconds));
                this.OnMessage(msg);
                ar.Log.Add(msg);
            }
            if (TimeoutSeconds > 0) 
            {
                if (((DateTime.Now - kit.StartDate)).TotalSeconds > TimeoutSeconds) 
                    ar.IsTimeoutBreaked = true;
            }
            ar.Sofa = text;
            if (!ontoRegine) 
                ar.Entities.AddRange(kit.Entities);
            ar.FirstToken = kit.FirstToken;
            ar.Ontology = extOntology;
            ar.BaseLanguage = kit.BaseLanguage;
            ar.Tag = kit;
            if (kit.Msgs.Count > 0) 
                ar.Log.AddRange(kit.Msgs);
            return ar;
        }
        void _process2(Pullenti.Ner.Core.AnalysisKit kit, AnalysisResult ar, bool noLog)
        {
            string msg;
            Stopwatch sw = Stopwatch.StartNew();
            bool stopByTimeout = false;
            List<Analyzer> anals = new List<Analyzer>(m_Analyzers);
            for (int ii = 0; ii < anals.Count; ii++) 
            {
                Analyzer c = anals[ii];
                if (c.IgnoreThisAnalyzer) 
                    continue;
                if (m_Breaked) 
                {
                    if (!noLog) 
                    {
                        msg = "Процесс прерван пользователем";
                        this.OnMessage(msg);
                        ar.Log.Add(msg);
                    }
                    break;
                }
                if (TimeoutSeconds > 0 && !stopByTimeout) 
                {
                    if (((DateTime.Now - kit.StartDate)).TotalSeconds > TimeoutSeconds) 
                    {
                        m_Breaked = true;
                        if (!noLog) 
                        {
                            msg = "Процесс прерван по таймауту";
                            this.OnMessage(msg);
                            ar.Log.Add(msg);
                        }
                        stopByTimeout = true;
                    }
                }
                if (stopByTimeout) 
                {
                    if (c.Name == "INSTRUMENT") 
                    {
                    }
                    else 
                        continue;
                }
                if (!noLog) 
                    this.OnProgressHandler(c, new ProgressChangedEventArgs(0, string.Format("Работа \"{0}\"", c.Caption)));
                try 
                {
                    kit.m_AnalyzerStack.Add(c.Name);
                    sw.Reset();
                    sw.Start();
                    c.Process(kit);
                    sw.Stop();
                    kit.m_AnalyzerStack.Remove(c.Name);
                    Pullenti.Ner.Core.AnalyzerData dat = kit.GetAnalyzerData(c);
                    if (!noLog) 
                    {
                        msg = string.Format("Анализатор \"{0}\" выделил {1} объект(ов) за {2}", c.Caption, (dat == null ? 0 : dat.Referents.Count), OutSecs(sw.ElapsedMilliseconds));
                        this.OnMessage(msg);
                        ar.Log.Add(msg);
                    }
                }
                catch(Exception ex) 
                {
                    if (kit.m_AnalyzerStack.Contains(c.Name)) 
                        kit.m_AnalyzerStack.Remove(c.Name);
                    if (!noLog) 
                    {
                        ex = new Exception(string.Format("Ошибка в анализаторе \"{0}\" ({1})", c.Caption, ex.Message), ex);
                        this.OnMessage(ex);
                        ar.AddException(ex);
                    }
                }
            }
            if (!noLog) 
                this.OnProgressHandler(null, new ProgressChangedEventArgs(0, "Пересчёт отношений обобщения"));
            try 
            {
                sw.Reset();
                sw.Start();
                Pullenti.Ner.Core.Internal.GeneralRelationHelper.RefreshGenerals(this, kit);
                sw.Stop();
                if (!noLog) 
                {
                    msg = string.Format("Отношение обобщение пересчитано за {0}", OutSecs(sw.ElapsedMilliseconds));
                    this.OnMessage(msg);
                    ar.Log.Add(msg);
                }
            }
            catch(Exception ex) 
            {
                if (!noLog) 
                {
                    ex = new Exception("Ошибка пересчёта отношения обобщения", ex);
                    this.OnMessage(ex);
                    ar.AddException(ex);
                }
            }
        }
        internal void _createRes(Pullenti.Ner.Core.AnalysisKit kit, AnalysisResult ar, ExtOntology extOntology, bool noLog)
        {
            Stopwatch sw = Stopwatch.StartNew();
            int ontoAttached = 0;
            for (int k = 0; k < 2; k++) 
            {
                foreach (Analyzer c in Analyzers) 
                {
                    if (k == 0) 
                    {
                        if (!c.IsSpecific) 
                            continue;
                    }
                    else if (c.IsSpecific) 
                        continue;
                    Pullenti.Ner.Core.AnalyzerData dat = kit.GetAnalyzerData(c);
                    if (dat != null && dat.Referents.Count > 0) 
                    {
                        if (extOntology != null) 
                        {
                            foreach (Referent r in dat.Referents) 
                            {
                                if (r.OntologyItems == null) 
                                {
                                    if ((((r.OntologyItems = extOntology.AttachReferent(r)))) != null) 
                                        ontoAttached++;
                                }
                            }
                        }
                        ar.Entities.AddRange(dat.Referents);
                    }
                }
            }
            for (Token t = kit.FirstToken; t != null; t = t.Next) 
            {
                this._clearTags(t, 0);
            }
            sw.Stop();
            if (extOntology != null && !noLog) 
            {
                string msg = string.Format("Привязано {0} объектов к внешней отнологии ({1} элементов) за {2}", ontoAttached, extOntology.Items.Count, OutSecs(sw.ElapsedMilliseconds));
                this.OnMessage(msg);
                ar.Log.Add(msg);
            }
        }
        void _clearTags(Token t, int lev)
        {
            if (lev > 10) 
                return;
            t.Tag = null;
            if (t is MetaToken) 
            {
                for (Token tt = (t as MetaToken).BeginToken; tt != null && tt.EndChar <= t.EndChar; tt = tt.Next) 
                {
                    this._clearTags(tt, lev + 1);
                }
            }
        }
        static string OutSecs(long ms)
        {
            if (ms < 4000) 
                return string.Format("{0}ms", ms);
            ms /= 1000;
            if (ms < 120) 
                return string.Format("{0}sec", ms);
            return string.Format("{0}min {1}sec", ms / 60, ms % 60);
        }
        /// <summary>
        /// Событие обработки строки состояния процесса. 
        /// Там-же в событии ProgressChangedEventArg в UserState выводятся информационные сообщения. 
        /// Внимание, если ProgressPercentage &lt; 0, то учитывать только информационное сообщение в UserState.
        /// </summary>
        public event ProgressChangedEventHandler Progress;
        Dictionary<object, Pullenti.Ner.Core.Internal.ProgressPeace> m_ProgressPeaces = new Dictionary<object, Pullenti.Ner.Core.Internal.ProgressPeace>();
        object m_ProgressPeacesLock = new object();
        /// <summary>
        /// Прервать процесс анализа
        /// </summary>
        public void BreakProcess()
        {
            m_Breaked = true;
        }
        bool m_Breaked = false;
        /// <summary>
        /// Максимальное время обработки, прервёт при превышении. 
        /// По умолчанию (0) - неограничено.
        /// </summary>
        public int TimeoutSeconds = 0;
        const int MorphCoef = 10;
        void PrepareProgress()
        {
            lock (m_ProgressPeacesLock) 
            {
                lastPercent = -1;
                int co = MorphCoef;
                int total = co;
                foreach (Analyzer wf in Analyzers) 
                {
                    if (!wf.IgnoreThisAnalyzer) 
                        total += (wf.ProgressWeight > 0 ? wf.ProgressWeight : 1);
                }
                m_ProgressPeaces.Clear();
                float max = (float)(co * 100);
                max /= total;
                m_ProgressPeaces.Add(this, new Pullenti.Ner.Core.Internal.ProgressPeace() { Min = 0, Max = max });
                foreach (Analyzer wf in Analyzers) 
                {
                    if (!wf.IgnoreThisAnalyzer) 
                    {
                        float min = max;
                        co += (wf.ProgressWeight > 0 ? wf.ProgressWeight : 1);
                        max = co * 100;
                        max /= total;
                        if (!m_ProgressPeaces.ContainsKey(wf)) 
                            m_ProgressPeaces.Add(wf, new Pullenti.Ner.Core.Internal.ProgressPeace() { Min = min, Max = max });
                    }
                }
            }
        }
        void OnProgressHandler(object sender, ProgressChangedEventArgs e)
        {
            if (Progress == null) 
                return;
            if (e.ProgressPercentage >= 0) 
            {
                Pullenti.Ner.Core.Internal.ProgressPeace pi;
                lock (m_ProgressPeacesLock) 
                {
                    if (m_ProgressPeaces.TryGetValue(sender ?? this, out pi)) 
                    {
                        float p = (e.ProgressPercentage * ((pi.Max - pi.Min))) / 100;
                        p += pi.Min;
                        int pers = (int)p;
                        if (pers == lastPercent && e.UserState == null && !m_Breaked) 
                            return;
                        e = new ProgressChangedEventArgs((int)p, e.UserState);
                        lastPercent = pers;
                    }
                }
            }
            Progress(this, e) /* error */;
        }
        void OnCancel(object sender, CancelEventArgs e)
        {
            if (TimeoutSeconds > 0) 
            {
                if (sender is Pullenti.Ner.Core.AnalysisKit) 
                {
                    if (((DateTime.Now - (sender as Pullenti.Ner.Core.AnalysisKit).StartDate)).TotalSeconds > TimeoutSeconds) 
                        m_Breaked = true;
                }
            }
            e.Cancel = m_Breaked;
        }
        void OnMessage(object message)
        {
            if (Progress != null) 
                Progress(this, new ProgressChangedEventArgs(-1, message)) /* error */;
        }
        int lastPercent;
        /// <summary>
        /// Используется произвольным образом
        /// </summary>
        public object Tag;
    }
}