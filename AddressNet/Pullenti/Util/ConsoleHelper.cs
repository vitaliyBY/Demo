/*
 * SDK Pullenti Address, version 4.20, august 2023. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace Pullenti.Util
{
    /// <summary>
    /// Запись в лог-файл и на экран
    /// </summary>
    public static class ConsoleHelper
    {
        static bool m_HideConsoleOutput;
        public static bool HideConsoleOutput
        {
            get
            {
                return m_HideConsoleOutput;
            }
            set
            {
                m_HideConsoleOutput = value;
            }
        }
        public static bool HideLogOutput;
        public static bool OutDate = true;
        public static bool CloseStreamAfterEachWrite = false;
        /// <summary>
        /// Удалять лог-файлы, которые были созданы древнее, чем указанное число дней от текущей даты
        /// </summary>
        public static int RemoveLogsOlderThisDays = 7;
        public static string Clear(bool saveLog)
        {
            if (HideLogOutput) 
                return null;
            try 
            {
                if (m_Stream != null) 
                {
                    m_Stream.Dispose();
                    m_Stream = null;
                }
            }
            catch(Exception ex4366) 
            {
            }
            string ret = null;
            try 
            {
                if (File.Exists(LogFileName)) 
                {
                    if (saveLog) 
                    {
                        FileInfo fi = new FileInfo(LogFileName);
                        DateTime dt = fi.LastWriteTime;
                        string fname = Path.Combine(Path.GetDirectoryName(LogFileName), (ret = GetDtFileName(dt)));
                        try 
                        {
                            File.Copy(LogFileName, fname, true);
                        }
                        catch(Exception ex4367) 
                        {
                        }
                    }
                    try 
                    {
                        File.Delete(LogFileName);
                    }
                    catch(Exception ex4368) 
                    {
                    }
                    LogFileLength = 0;
                }
                if (RemoveLogsOlderThisDays > 0) 
                {
                    try 
                    {
                        foreach (string f in Directory.GetFiles(Path.GetDirectoryName(LogFileName), Prefix + "*.txt")) 
                        {
                            FileInfo fi = new FileInfo(f);
                            if (((DateTime.Now - fi.LastWriteTime)).TotalDays >= RemoveLogsOlderThisDays) 
                                fi.Delete();
                        }
                    }
                    catch(Exception ex) 
                    {
                    }
                }
            }
            catch(Exception ex4369) 
            {
            }
            return ret;
        }
        static string GetDtFileName(DateTime dt)
        {
            return string.Format("{0}{1}{2}{3}{4}{5}.txt", m_Prefix, dt.Year.ToString("D04"), dt.Month.ToString("D02"), dt.Day.ToString("D02"), dt.Hour.ToString("D02"), dt.Minute.ToString("D02"));
        }
        static string m_Prefix = "log";
        public static string Prefix
        {
            get
            {
                return m_Prefix;
            }
            set
            {
                m_Prefix = value;
                m_LogFileName = null;
            }
        }
        /// <summary>
        /// Имя файла для лога
        /// </summary>
        public static string LogFileName
        {
            get
            {
                try 
                {
                    if (m_LogFileName == null) 
                        m_LogFileName = Path.Combine(LogDirectory, m_Prefix + ".txt");
                }
                catch(Exception ex4370) 
                {
                }
                return m_LogFileName;
            }
            set
            {
                m_LogFileName = value;
            }
        }
        static string m_LogFileName;
        public static string m_LogDirectory;
        public static string LogDirectory
        {
            get
            {
                if (m_LogDirectory != null) 
                    return m_LogDirectory;
                return AppDomain.CurrentDomain.BaseDirectory;
            }
            set
            {
                m_LogDirectory = value;
                m_LogFileName = null;
            }
        }
        static long LogFileLength = -1;
        const long MaxLogFileLength = 100000000;
        public static event EventHandler MessageOccured;
        public static void Write(string str)
        {
            lock (m_Lock) 
            {
                _write(str);
            }
        }
        static object m_Lock = new object();
        static void _write(string str)
        {
            if (MessageOccured != null) 
                MessageOccured(str, EventArgs.Empty) /* error */;
            try 
            {
                if (!HideConsoleOutput) 
                    Console.Write(str);
            }
            catch(Exception ex4371) 
            {
            }
            if (HideLogOutput) 
                return;
            try 
            {
                if (LogFileLength < 0) 
                {
                    FileInfo fi = new FileInfo(LogFileName);
                    if (!fi.Exists) 
                        LogFileLength = 0;
                    else 
                        LogFileLength = fi.Length;
                }
                if (LogFileLength > MaxLogFileLength) 
                {
                    if (!HideConsoleOutput) 
                        Console.Write("\r\nLog file too long, it's copied and cleared");
                    string fname = Clear(true);
                    if (fname != null) 
                        _write(string.Format("This log-file is continued, previous part in file {0}\r\n", fname));
                }
            }
            catch(Exception ex4372) 
            {
            }
            try 
            {
                if (m_Stream == null) 
                    m_Stream = new FileStream(LogFileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
                if (str.IndexOf('\n') >= 0 || m_Stream.Length == 0) 
                {
                    DateTime dt = DateTime.Now;
                    string date = "";
                    if (OutDate) 
                        date = string.Format("{0}.{1}.{2} ", dt.Year.ToString("D04"), dt.Month.ToString("D02"), dt.Day.ToString("D02"));
                    string time = string.Format("\n{0}{1}:{2}:{3} ", date, dt.Hour.ToString("D02"), dt.Minute.ToString("D02"), dt.Second.ToString("D02"));
                    if (m_Stream.Length == 0) 
                        str = string.Format("{0} {1}", time.Trim(), str);
                    str = str.Replace("\n", time);
                }
                byte[] dat = Encoding.UTF8.GetBytes(str);
                m_Stream.Position = m_Stream.Length;
                m_Stream.Write(dat, 0, dat.Length);
                m_Stream.Flush();
                LogFileLength = m_Stream.Length;
                if (CloseStreamAfterEachWrite) 
                {
                    m_Stream.Dispose();
                    m_Stream = null;
                }
                bool first = true;
                foreach (string li in str.Split('\n')) 
                {
                    string line = li.Trim();
                    if (string.IsNullOrEmpty(line)) 
                    {
                        first = false;
                        continue;
                    }
                    if (first && m_Lines.Count > 0) 
                        m_Lines[m_Lines.Count - 1] += line;
                    else 
                        m_Lines.Add(line);
                    if (m_Lines.Count > m_MaxLines) 
                        m_Lines.RemoveAt(0);
                    first = false;
                }
            }
            catch(Exception ex) 
            {
            }
        }
        static FileStream m_Stream;
        public static void WriteLine(string str)
        {
            Write(str + "\r\n");
        }
        public static void WriteMemory(bool collect = false)
        {
            if (collect) 
                GC.Collect();
            Write(string.Format("\r\nUsed Memory: {0}Mb ", GC.GetTotalMemory(false) / 1000000));
        }
        static List<string> m_Lines = new List<string>();
        static int m_MaxLines = 300;
        /// <summary>
        /// Получить последние строки из лога
        /// </summary>
        public static List<string> GetLastLines()
        {
            lock (m_Lock) 
            {
                return new List<string>(m_Lines);
            }
        }
    }
}