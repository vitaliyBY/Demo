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

namespace Pullenti.Address.Internal
{
    static class CorrectionHelper
    {
        public static string Correct(string txt, out string secondVar, out string details)
        {
            secondVar = null;
            details = null;
            txt = txt.Trim();
            if (string.IsNullOrEmpty(txt)) 
                return txt;
            int ii = txt.IndexOf("областьг");
            if (ii > 0) 
            {
                StringBuilder tmp = new StringBuilder(txt);
                tmp.Insert(ii + 7, ' ');
                txt = tmp.ToString();
            }
            if (txt.Contains("снт Тверь")) 
                txt = txt.Replace("снт Тверь", "г.Тверь");
            if (txt.Contains("Санкт-Петербур ")) 
                txt = txt.Replace("Санкт-Петербур ", "Санкт-Петербург ");
            txt = txt.Replace("кл-ще", "кладбище");
            txt = txt.Replace("областьасть", "область");
            txt = txt.Replace("ж/д_ст", "железнодорожная станция");
            txt = txt.Replace(" - ", "-");
            txt = txt.Replace("\\\\", "\\");
            txt = txt.Replace("\\\"", "\"");
            txt = txt.Replace('\t', ' ');
            if (txt.EndsWith("д., , ,")) 
                txt = txt.Substring(0, txt.Length - 7).Trim();
            if (txt.IndexOf('*') > 0) 
                txt = txt.Replace('*', '-');
            if (txt[txt.Length - 1] == '/') 
                txt = txt.Substring(0, txt.Length - 1);
            if (txt.StartsWith("НЕТ,", StringComparison.OrdinalIgnoreCase)) 
                txt = txt.Substring(4).Trim();
            if (txt.StartsWith("СУБЪЕКТ", StringComparison.OrdinalIgnoreCase)) 
                txt = txt.Substring(7).Trim();
            if (txt.StartsWith("ФЕДЕРАЦИЯ.", StringComparison.OrdinalIgnoreCase)) 
                txt = string.Format("{0} {1}", txt.Substring(0, 9), txt.Substring(10));
            int i0 = 0;
            if (txt.StartsWith("РОССИЯ", StringComparison.OrdinalIgnoreCase)) 
                i0 = 6;
            else if (txt.StartsWith("РФ", StringComparison.OrdinalIgnoreCase)) 
                i0 = 2;
            else if (txt.StartsWith("RU", StringComparison.OrdinalIgnoreCase)) 
                i0 = 2;
            else if (txt.StartsWith("Р.Ф.", StringComparison.OrdinalIgnoreCase)) 
                i0 = 4;
            else if (txt.StartsWith("г. Москва и Московская область", StringComparison.OrdinalIgnoreCase)) 
            {
                i0 = 30;
                string txt1 = txt.Substring(i0);
                if (txt1.Contains("Москва") || txt1.Contains("Москов")) 
                {
                }
                else 
                    i0 = 12;
            }
            else if (txt.StartsWith("г. Санкт-Петербург и Ленинградская область", StringComparison.OrdinalIgnoreCase)) 
                i0 = 42;
            if (i0 > 0 && ((i0 + 1) < txt.Length) && ((!char.IsLetter(txt[i0]) || (((int)txt[i0 - 1]) < 0x80)))) 
            {
                for (; i0 < txt.Length; i0++) 
                {
                    if (char.IsLetter(txt[i0])) 
                    {
                        txt = txt.Substring(i0);
                        break;
                    }
                }
            }
            if (txt.StartsWith("МО", StringComparison.OrdinalIgnoreCase) && txt.Length > 3) 
            {
                if (txt[2] == ' ' || txt[2] == ',') 
                    txt = "Московская область" + txt.Substring(2);
            }
            if (((txt.StartsWith("М\\О", StringComparison.OrdinalIgnoreCase) || (txt.StartsWith("М/О", StringComparison.OrdinalIgnoreCase)))) && txt.Length > 3) 
                txt = "Московская область " + txt.Substring(3);
            for (int i = 0; i < txt.Length; i++) 
            {
                if (txt[i] == ' ' || txt[i] == ',') 
                {
                    if (i < 4) 
                        break;
                    Pullenti.Address.AddrObject countr;
                    if (m_Cities.TryGetValue(txt.Substring(0, i).ToUpper(), out countr)) 
                        txt = string.Format("{0}, город {1}", countr.ToString(), txt);
                    break;
                }
            }
            if (m_Root == null) 
                return txt;
            for (int i = 0; i < (txt.Length - 5); i++) 
            {
                if (txt[i] == 'у' && txt[i + 1] == 'л' && char.IsUpper(txt[i + 2])) 
                {
                    txt = string.Format("{0}.{1}", txt.Substring(0, i + 2), txt.Substring(i + 2));
                    break;
                }
            }
            for (int i = 10; i < (txt.Length - 3); i++) 
            {
                if (txt[i - 1] == ' ' || txt[i - 1] == ',') 
                {
                    if (((_isStartOf(txt, i, "паспорт", false) || _isStartOf(txt, i, "выдан", false) || _isStartOf(txt, i, "Выдан", false)) || _isStartOf(txt, i, "серия", false) || _isStartOf(txt, i, "док:", false)) || _isStartOf(txt, i, "док.:", false)) 
                    {
                        txt = txt.Substring(0, i).TrimEnd();
                        break;
                    }
                    else if (_isStartOf(txt, i, "ОКАТО", false) && i >= (txt.Length - 20)) 
                    {
                        txt = txt.Substring(0, i).TrimEnd();
                        break;
                    }
                    else if (_isStartOf(txt, i, "адрес ориентира:", false)) 
                    {
                        details = txt.Substring(0, i);
                        txt = txt.Substring(i + 16).Trim();
                        i = 0;
                    }
                    else if ((_isStartOf(txt, i, "ОВД", false) || _isStartOf(txt, i, "УВД", false) || _isStartOf(txt, i, "РОВД", false)) || _isStartOf(txt, i, "ГОВД", false)) 
                    {
                        int kk;
                        bool br = false;
                        for (kk = 10; kk < (i - 2); kk++) 
                        {
                            if (_isStartOf(txt, kk, "кв", false) || _isStartOf(txt, kk, "Кв", false)) 
                            {
                                if (txt[kk + 2] == '.' || txt[kk + 2] == ' ') 
                                {
                                    kk += 2;
                                    for (; kk < (i - 2); kk++) 
                                    {
                                        if (txt[kk] != ' ' && txt[kk] != '.') 
                                            break;
                                    }
                                    if (char.IsDigit(txt[kk])) 
                                    {
                                        for (; kk < i; kk++) 
                                        {
                                            if (!char.IsDigit(txt[kk])) 
                                                break;
                                        }
                                        txt = txt.Substring(0, kk);
                                        br = true;
                                    }
                                    break;
                                }
                            }
                        }
                        if (br) 
                            break;
                        int j = i - 2;
                        int sp = 0;
                        for (; j > 0; j--) 
                        {
                            if (txt[j] == ' ' && txt[j - 1] != ' ') 
                            {
                                sp++;
                                if (sp >= 4) 
                                    break;
                            }
                        }
                        if (j > 10 && sp == 4) 
                        {
                            txt = txt.Substring(0, j).TrimEnd();
                            break;
                        }
                    }
                }
            }
            string txt0 = txt.ToUpper();
            for (int i = 0; i < txt0.Length; i++) 
            {
                if (!char.IsLetter(txt0[i])) 
                    continue;
                if (((i > 10 && char.IsDigit(txt[i - 1]) && char.IsUpper(txt[i])) && ((i + 2) < txt.Length) && txt[i + 1] == 'к') && char.IsDigit(txt[i + 2])) 
                {
                    txt = string.Format("{0} {1}", txt.Substring(0, i + 1), txt.Substring(i + 1));
                    txt0 = txt.ToUpper();
                    continue;
                }
                if (_isStartOf(txt0, i, "РНД", true)) 
                {
                    txt = string.Format("{0}Ростов-на-Дону {1}", txt.Substring(0, i), txt.Substring(i + 3));
                    txt0 = txt.ToUpper();
                    continue;
                }
                if (_isStartOf(txt0, i, "РСО", true)) 
                {
                    txt = string.Format("{0}республика Северная Осетия {1}", txt.Substring(0, i), txt.Substring(i + 3));
                    txt0 = txt.ToUpper();
                    continue;
                }
                if (_isStartOf(txt0, i, "РС(Я)", false)) 
                {
                    txt = string.Format("{0}республика Саха (Якутия){1}", txt.Substring(0, i), txt.Substring(i + 5));
                    txt0 = txt.ToUpper();
                    continue;
                }
                if (_isStartOf(txt0, i, "РС (Я)", false)) 
                {
                    txt = string.Format("{0}республика Саха (Якутия){1}", txt.Substring(0, i), txt.Substring(i + 6));
                    txt0 = txt.ToUpper();
                    continue;
                }
                if (_isStartOf(txt0, i, "СПБ", true)) 
                {
                    txt = string.Format("{0}Санкт-Петербург {1}", txt.Substring(0, i), txt.Substring(i + 3));
                    txt0 = txt.ToUpper();
                    continue;
                }
                if (_isStartOf(txt0, i, "ДО ВОСТРЕБ", false)) 
                {
                    txt = txt.Substring(0, i).Trim();
                    txt0 = txt.ToUpper();
                    break;
                }
                if (i == 0 || txt[i - 1] == ',' || txt[i - 1] == ' ') 
                {
                }
                else 
                    continue;
                if (_isStartOf(txt0, i, "ХХХ", false) || _isStartOf(txt0, i, "XXX", false)) 
                {
                    txt = txt.Substring(0, i) + txt.Substring(i + 3);
                    txt0 = txt.ToUpper();
                    continue;
                }
                AbbrTreeNode tn = m_Root.Find(txt0, i);
                if (tn == null) 
                    continue;
                int j = i + tn.Len;
                bool ok = false;
                if (tn.Len == 2 && txt0[i] == 'У' && txt0[i + 1] == 'Л') 
                    continue;
                if (tn.Len == 2 && txt0[i] == 'С' && txt0[i + 1] == 'Т') 
                    continue;
                if ((tn.Len == 3 && txt0[i] == 'П' && txt0[i + 1] == 'Е') && txt0[i + 2] == 'Р') 
                    continue;
                for (; j < txt0.Length; j++) 
                {
                    if (txt0[j] == '.' || txt0[j] == ' ') 
                        ok = true;
                    else 
                        break;
                }
                if (j >= txt0.Length || !ok || tn.Corrs == null) 
                    continue;
                foreach (KeyValuePair<string, string> kp in tn.Corrs) 
                {
                    if (!_isStartOf(txt0, j, kp.Key, false)) 
                        continue;
                    if (tn.Len == 8 && _isStartOf(txt0, i, "НОВГОРОД", false)) 
                        continue;
                    if (tn.Len == 2 && _isStartOf(txt0, i, "ПР", false)) 
                        continue;
                    StringBuilder tmp = new StringBuilder(txt);
                    tmp.Remove(i, tn.Len);
                    if (tmp[i] == '.') 
                        tmp.Remove(i, 1);
                    tmp.Insert(i, kp.Value + " ");
                    txt = tmp.ToString();
                    txt0 = txt.ToUpper();
                    break;
                }
            }
            for (int i = 0; i < txt.Length; i++) 
            {
                if (!char.IsLetter(txt[i]) && txt[i] != '-') 
                {
                    string city = txt.Substring(0, i);
                    if (RegionHelper.IsBigCity(city) == null) 
                        continue;
                    bool ok = false;
                    for (int j = i; j < txt.Length; j++) 
                    {
                        if (char.IsWhiteSpace(txt[j])) 
                            continue;
                        if (txt[j] == 'г' || txt[j] == 'Г') 
                            ok = true;
                        break;
                    }
                    if (!ok) 
                        txt = string.Format("г.{0},{1}", txt.Substring(0, i), txt.Substring(i));
                    break;
                }
            }
            for (int i = 0; i < txt.Length; i++) 
            {
                if (txt[i] == ' ') 
                {
                    if (_isStartOf(txt, i + 1, "филиал", false) || _isStartOf(txt, i + 1, "ФИЛИАЛ", false)) 
                    {
                        string reg = txt.Substring(0, i);
                        string city = null;
                        AbbrTreeNode tn = m_Root.Find(reg.ToUpper(), 0);
                        if (tn != null && tn.Corrs != null) 
                        {
                            foreach (KeyValuePair<string, string> kp in tn.Corrs) 
                            {
                                if (kp.Key == "ОБ") 
                                {
                                    string nam = kp.Value;
                                    reg = nam + " область";
                                    RegionInfo r = RegionHelper.FindRegionByAdj(nam);
                                    if (r != null && r.Capital != null) 
                                        city = r.Capital;
                                    break;
                                }
                            }
                        }
                        if (city != null) 
                            secondVar = string.Format("г.{0}, {1}", city, txt.Substring(i + 7));
                        string txt1 = string.Format("{0}, {1}", reg, txt.Substring(i + 7));
                        txt = txt1;
                    }
                    break;
                }
            }
            txt0 = txt.ToUpper();
            if (_isStartOf(txt0, 0, "ФИЛИАЛ ", false)) 
                txt = txt.Substring(7);
            if (txt.Length > 10 && txt[0] == 'г' && txt[1] == ',') 
                txt = "г." + txt.Substring(2);
            if (txt.Length < 20) 
                return txt;
            if (char.IsLetter(txt[txt.Length - 1])) 
            {
                for (int i = txt.Length - 7; i > 10; i--) 
                {
                    if (char.IsLetter(txt[i])) 
                    {
                        if (txt[i - 1] == '.' && char.IsUpper(txt[i]) && ((int)txt[i]) > 0x80) 
                        {
                            if (txt[i + 1] == '.') 
                                continue;
                            if (char.IsUpper(txt[i - 2])) 
                                continue;
                            bool hasCap = false;
                            for (int j = i - 3; j > 10; j--) 
                            {
                                if (txt[j] == ',') 
                                {
                                    string p0 = txt.Substring(0, j + 1);
                                    string p1 = txt.Substring(i);
                                    string p2 = txt.Substring(j + 1, i - j - 2);
                                    txt = string.Format("{0}{1},{2}", p0, p1, p2);
                                    break;
                                }
                                else if (!hasCap && !char.IsLetter(txt[j]) && char.IsLetter(txt[j + 1])) 
                                {
                                    if (!char.IsUpper(txt[j + 1])) 
                                        break;
                                    hasCap = true;
                                }
                            }
                            break;
                        }
                    }
                    else 
                        break;
                }
            }
            return txt;
        }
        static bool _isStartOf(string txt, int i, string sub, bool checkNonLetSurr = false)
        {
            bool noCasesens = false;
            if (i > 0 && txt[i - 1] == ' ') 
                noCasesens = true;
            for (int j = 0; j < sub.Length; j++) 
            {
                if ((i + j) >= txt.Length) 
                    return false;
                else if (sub[j] == txt[i + j]) 
                {
                }
                else if (noCasesens && char.ToUpper(sub[j]) == char.ToUpper(txt[i + j])) 
                {
                }
                else 
                    return false;
            }
            if (checkNonLetSurr) 
            {
                if (i > 0 && char.IsLetter(txt[i - 1])) 
                    return false;
                if (((i + sub.Length) < txt.Length) && char.IsLetter(txt[i + sub.Length])) 
                    return false;
            }
            return true;
        }
        public static void Initialize()
        {
            m_Root = new AbbrTreeNode();
            foreach (RegionInfo r in RegionHelper.Regions) 
            {
                Pullenti.Address.AreaAttributes a = r.Attrs as Pullenti.Address.AreaAttributes;
                if (a == null) 
                    continue;
                if (a.Types.Count == 0 || a.Types.Contains("город")) 
                    continue;
                if (a.Names.Count == 0) 
                    continue;
                if (a.Types[0] == "республика") 
                    _add(a.Names[0], "респ");
                else if (a.Types[0] == "край") 
                {
                    _add(a.Names[0], "кр");
                    if (a.Names[0].EndsWith("ий")) 
                        _add(a.Names[0].Substring(0, a.Names[0].Length - 2) + "ая", "об");
                }
                else if (a.Types[0] == "область") 
                {
                    _add(a.Names[0], "об");
                    if (a.Names[0].EndsWith("ая")) 
                        _add(a.Names[0].Substring(0, a.Names[0].Length - 2) + "ий", "р");
                }
                else if (a.Types[0] == "автономная область") 
                {
                    _add(a.Names[0], "об");
                    _add(a.Names[0], "ао");
                }
                else if (a.Types[0] == "автономный округ") 
                {
                    _add(a.Names[0], "ок");
                    _add(a.Names[0], "ао");
                }
                else if (a.Types[0] == "город") 
                    _add(a.Names[0], "г");
                else 
                {
                }
            }
        }
        public static void Initialize0()
        {
            string dat = ResourceHelper.GetString("CitiesNonRus.txt");
            Pullenti.Address.AddrObject country = null;
            foreach (string line in dat.Split('\n')) 
            {
                if (line.StartsWith("//")) 
                    continue;
                if (line.StartsWith("*")) 
                {
                    Pullenti.Address.AreaAttributes aa = new Pullenti.Address.AreaAttributes();
                    country = new Pullenti.Address.AddrObject(aa) { Level = Pullenti.Address.AddrLevel.Country };
                    aa.Names.Add(line.Substring(1).Trim());
                    continue;
                }
                if (country == null) 
                    continue;
                foreach (string v in line.Split(';')) 
                {
                    string city = v.ToUpper().Trim();
                    if (string.IsNullOrEmpty(city)) 
                        continue;
                    if (!m_Cities.ContainsKey(city)) 
                        m_Cities.Add(city, country);
                }
            }
        }
        static AbbrTreeNode m_Root;
        static Dictionary<string, Pullenti.Address.AddrObject> m_Cities = new Dictionary<string, Pullenti.Address.AddrObject>();
        public static Pullenti.Address.AddrObject FindCountry(Pullenti.Address.AddrObject obj)
        {
            Pullenti.Address.AreaAttributes aa = obj.Attrs as Pullenti.Address.AreaAttributes;
            if (aa == null) 
                return null;
            Pullenti.Address.AddrObject res;
            foreach (string nam in aa.Names) 
            {
                if (m_Cities.TryGetValue(nam.ToUpper(), out res)) 
                    return res;
            }
            return null;
        }
        static void _add(string corr, string typ)
        {
            typ = typ.ToUpper();
            corr = corr.ToUpper();
            for (int i = 1; i < (corr.Length - 2); i++) 
            {
                if (!Pullenti.Morph.LanguageHelper.IsCyrillicVowel(corr[i])) 
                {
                    string str = corr.Substring(0, i + 1);
                    if (RegionHelper.IsBigCity(str) != null) 
                    {
                    }
                    else 
                        m_Root.Add(str, 0, corr, typ);
                }
            }
        }
        public static void CorrectCountry(Pullenti.Address.TextAddress addr)
        {
            if (addr.Items.Count == 0) 
                return;
            if (addr.Items[0].Level == Pullenti.Address.AddrLevel.Country) 
                return;
            if (addr.Items[0].Gars.Count == 0) 
                return;
            int reg = addr.Items[0].Gars[0].RegionNumber;
            if ((reg == 90 || reg == 93 || reg == 94) || reg == 95) 
                addr.Items.Insert(0, CreateCountry("UA", null));
            else 
                addr.Items.Insert(0, CreateCountry("RU", null));
        }
        public static Pullenti.Address.AddrObject CreateCountry(string alpha, Pullenti.Ner.Geo.GeoReferent geo)
        {
            Pullenti.Address.AreaAttributes aa = new Pullenti.Address.AreaAttributes();
            if (alpha == "RU") 
                aa.Names.Add("Россия");
            else if (alpha == "UA") 
                aa.Names.Add("Украина");
            else if (alpha == "BY") 
                aa.Names.Add("Белоруссия");
            else if (alpha == "KZ") 
                aa.Names.Add("Казахстан");
            else if (alpha == "MD") 
                aa.Names.Add("Молдавия");
            else if (alpha == "GE") 
                aa.Names.Add("Грузия");
            else if (alpha == "AZ") 
                aa.Names.Add("Азербайджан");
            else if (alpha == "AM") 
                aa.Names.Add("Армения");
            else if (geo != null) 
                aa.Names.Add(geo.ToString());
            else 
                return null;
            Pullenti.Address.AddrObject res = new Pullenti.Address.AddrObject(aa);
            res.Level = Pullenti.Address.AddrLevel.Country;
            return res;
        }
    }
}