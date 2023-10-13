/*
 * SDK Pullenti Address, version 4.20, august 2023. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Pullenti.Address
{
    /// <summary>
    /// Сервис работы с адресами
    /// </summary>
    public class AddressService
    {
        /// <summary>
        /// Текущая версия
        /// </summary>
        public static string Version = "4.20";
        /// <summary>
        /// Дата создания текущей версии
        /// </summary>
        public static string VersionDate = "2023.08.30";
        /// <summary>
        /// Инициализация движка - необходимо вызывать один раз в начале работы.
        /// </summary>
        public static void Initialize()
        {
            if (m_Inited) 
                return;
            m_Inited = true;
            Pullenti.Ner.ProcessorService.Initialize(null);
            Pullenti.Ner.Money.MoneyAnalyzer.Initialize();
            Pullenti.Ner.Uri.UriAnalyzer.Initialize();
            Pullenti.Ner.Phone.PhoneAnalyzer.Initialize();
            Pullenti.Ner.Date.DateAnalyzer.Initialize();
            Pullenti.Ner.Geo.GeoAnalyzer.Initialize();
            Pullenti.Ner.Geo.Internal.MiscLocationHelper.NameChecker = new Pullenti.Address.Internal.NameChecker();
            Pullenti.Ner.Address.AddressAnalyzer.Initialize();
            Pullenti.Ner.Org.OrganizationAnalyzer.Initialize();
            Pullenti.Ner.Person.PersonAnalyzer.Initialize();
            Pullenti.Ner.Named.NamedEntityAnalyzer.Initialize();
            Pullenti.Address.Internal.AnalyzeHelper.Init();
            Pullenti.Address.Internal.GarHelper.Init(null);
            Pullenti.Address.Internal.CorrectionHelper.Initialize0();
            AddressHelper.Images.Add(new ImageWrapper("country", Pullenti.Address.Internal.ResourceHelper.GetBytes("country.png")));
            AddressHelper.Images.Add(new ImageWrapper("region", Pullenti.Address.Internal.ResourceHelper.GetBytes("region.png")));
            AddressHelper.Images.Add(new ImageWrapper("admin", Pullenti.Address.Internal.ResourceHelper.GetBytes("admin.png")));
            AddressHelper.Images.Add(new ImageWrapper("municipal", Pullenti.Address.Internal.ResourceHelper.GetBytes("municipal.png")));
            AddressHelper.Images.Add(new ImageWrapper("settlement", Pullenti.Address.Internal.ResourceHelper.GetBytes("settlement.png")));
            AddressHelper.Images.Add(new ImageWrapper("city", Pullenti.Address.Internal.ResourceHelper.GetBytes("city.png")));
            AddressHelper.Images.Add(new ImageWrapper("locality", Pullenti.Address.Internal.ResourceHelper.GetBytes("locality.png")));
            AddressHelper.Images.Add(new ImageWrapper("district", Pullenti.Address.Internal.ResourceHelper.GetBytes("district.png")));
            AddressHelper.Images.Add(new ImageWrapper("area", Pullenti.Address.Internal.ResourceHelper.GetBytes("area.png")));
            AddressHelper.Images.Add(new ImageWrapper("street", Pullenti.Address.Internal.ResourceHelper.GetBytes("street.png")));
            AddressHelper.Images.Add(new ImageWrapper("plot", Pullenti.Address.Internal.ResourceHelper.GetBytes("plot.png")));
            AddressHelper.Images.Add(new ImageWrapper("building", Pullenti.Address.Internal.ResourceHelper.GetBytes("building.png")));
            AddressHelper.Images.Add(new ImageWrapper("room", Pullenti.Address.Internal.ResourceHelper.GetBytes("room.png")));
            AddressHelper.Images.Add(new ImageWrapper("carplace", Pullenti.Address.Internal.ResourceHelper.GetBytes("carplace.png")));
        }
        static bool m_Inited = false;
        /// <summary>
        /// Указание директории с индексом ГАР (если не задать, то выделяемые объекты привязываться не будут)
        /// </summary>
        /// <param name="garPath">папка с индексом ГАР</param>
        public static void SetGarIndexPath(string garPath)
        {
            Pullenti.Address.Internal.GarHelper.Init(garPath);
            if (garPath != null) 
            {
                string regFile = Path.Combine(garPath, "regions.xml");
                Pullenti.Address.Internal.RegionHelper.LoadFromFile(regFile);
            }
            Pullenti.Address.Internal.CorrectionHelper.Initialize();
            Pullenti.Address.Internal.ServerHelper.ServerUri = null;
        }
        /// <summary>
        /// Получить папку с используемым ГАР-индексом (если null, то индекс не подгружен)
        /// </summary>
        public static string GetGarIndexPath()
        {
            if (Pullenti.Address.Internal.GarHelper.GarIndex == null) 
                return null;
            return Pullenti.Address.Internal.GarHelper.GarIndex.BaseDir;
        }
        /// <summary>
        /// Получить информацию по индексу и его объектам
        /// </summary>
        public static GarStatistic GetGarStatistic()
        {
            try 
            {
                if (Pullenti.Address.Internal.ServerHelper.ServerUri != null) 
                    return Pullenti.Address.Internal.ServerHelper.GetGarStatistic();
                if (Pullenti.Address.Internal.GarHelper.GarIndex == null) 
                    return null;
                GarStatistic res = new GarStatistic();
                res.IndexPath = Pullenti.Address.Internal.GarHelper.GarIndex.BaseDir;
                res.AreaCount = Pullenti.Address.Internal.GarHelper.GarIndex.AreasCount;
                res.HouseCount = Pullenti.Address.Internal.GarHelper.GarIndex.HousesCount;
                res.RoomCount = Pullenti.Address.Internal.GarHelper.GarIndex.RoomsCount;
                return res;
            }
            catch(Exception ex) 
            {
                return null;
            }
        }
        /// <summary>
        /// Для работы установить связь с сервером и все запросы делать через него 
        /// (используется для ускорения работы для JS и Python)
        /// </summary>
        /// <param name="uri">например, http://localhost:2222, если null, то связь разрывается</param>
        public static bool SetServerConnection(string uri)
        {
            if (uri == null) 
            {
                Pullenti.Address.Internal.ServerHelper.ServerUri = null;
                return true;
            }
            string ver = Pullenti.Address.Internal.ServerHelper.GetServerVersion(uri);
            if (ver == null) 
            {
                Pullenti.Address.Internal.ServerHelper.ServerUri = null;
                return false;
            }
            else 
            {
                SetGarIndexPath(null);
                Pullenti.Address.Internal.ServerHelper.ServerUri = uri;
                return true;
            }
        }
        /// <summary>
        /// Если связь с сервером установлена, то вернёт адрес
        /// </summary>
        public static string GetServerUri()
        {
            return Pullenti.Address.Internal.ServerHelper.ServerUri;
        }
        /// <summary>
        /// Получить версию SDK на сервере
        /// </summary>
        /// <return>версия или null при недоступности сервера</return>
        public static string GetServerVersion(string uri)
        {
            return Pullenti.Address.Internal.ServerHelper.GetServerVersion(uri);
        }
        /// <summary>
        /// Обработать произвольный текст, в котором есть адреса
        /// </summary>
        /// <param name="txt">текст</param>
        /// <param name="pars">дополнительные параметры (null - дефолтовые)</param>
        /// <return>результат - для каждого найденного адреса свой экземпляр</return>
        public static List<TextAddress> ProcessText(string txt, ProcessTextParams pars = null)
        {
            try 
            {
                if (Pullenti.Address.Internal.ServerHelper.ServerUri != null) 
                    return Pullenti.Address.Internal.ServerHelper.ProcessText(txt, pars);
                Pullenti.Address.Internal.AnalyzeHelper ah = new Pullenti.Address.Internal.AnalyzeHelper();
                List<TextAddress> res = ah.Analyze(txt, null, false, pars);
                return res;
            }
            catch(Exception ex) 
            {
                return null;
            }
        }
        /// <summary>
        /// Обработать текст с одним адресом (адресное поле)
        /// </summary>
        /// <param name="txt">исходный текст</param>
        /// <param name="pars">дополнительные параметры (null - дефолтовые)</param>
        /// <return>результат обработки</return>
        public static TextAddress ProcessSingleAddressText(string txt, ProcessTextParams pars = null)
        {
            try 
            {
                if (Pullenti.Address.Internal.ServerHelper.ServerUri != null) 
                    return Pullenti.Address.Internal.ServerHelper.ProcessSingleAddressText(txt, pars);
                Stopwatch sw = new Stopwatch();
                sw.Start();
                Pullenti.Address.Internal.AnalyzeHelper ah = new Pullenti.Address.Internal.AnalyzeHelper();
                List<TextAddress> objs = ah.Analyze(txt, null, true, pars);
                TextAddress res;
                if (objs == null || objs.Count == 0) 
                    res = new TextAddress() { ErrorMessage = "Адрес не выделен", Text = txt };
                else 
                    res = objs[0];
                res.ReadCount = ah.IndexReadCount;
                sw.Stop();
                res.Milliseconds = (int)sw.ElapsedMilliseconds;
                return res;
            }
            catch(Exception ex) 
            {
                return new TextAddress() { ErrorMessage = ex.ToString(), Text = txt };
            }
        }
        /// <summary>
        /// Обработать порцию адресов. Использовать в случае сервера, посылая ему порцию на обработку 
        /// (не более 100-300 за раз), чтобы сократить время на издержки взаимодействия. 
        /// Для обычной работы (не через сервер) это эквивалентно вызову в цикле ProcessSingleAddressText 
        /// и особого смысла не имеет.
        /// </summary>
        /// <param name="txts">список адресов</param>
        /// <param name="pars">дополнительные параметры (null - дефолтовые)</param>
        /// <return>результат (количество совпадает с исходным списком), если null, то какая-то ошибка</return>
        public static List<TextAddress> ProcessSingleAddressTexts(List<string> txts, ProcessTextParams pars = null)
        {
            try 
            {
                if (Pullenti.Address.Internal.ServerHelper.ServerUri != null) 
                    return Pullenti.Address.Internal.ServerHelper.ProcessSingleAddressTexts(txts, pars);
                List<TextAddress> res = new List<TextAddress>();
                foreach (string txt in txts) 
                {
                    res.Add(ProcessSingleAddressText(txt, null));
                }
                return res;
            }
            catch(Exception ex) 
            {
                return null;
            }
        }
        /// <summary>
        /// Искать объекты (для выпадающих списков)
        /// </summary>
        /// <param name="searchPars">параметры запроса</param>
        /// <return>результат</return>
        public static SearchResult SearchObjects(SearchParams searchPars)
        {
            try 
            {
                if (searchPars == null) 
                    return null;
                if (Pullenti.Address.Internal.ServerHelper.ServerUri != null) 
                    return Pullenti.Address.Internal.ServerHelper.SearchObjects(searchPars);
                else 
                    return Pullenti.Address.Internal.AddressSearchHelper.Search(searchPars);
            }
            catch(Exception ex) 
            {
                return null;
            }
        }
        /// <summary>
        /// Получить список дочерних объектов для ГАР-объекта
        /// </summary>
        /// <param name="go">идентификатор объект ГАР (если null, то объекты первого уровня - регионы)</param>
        /// <param name="ignoreHouses">игнорировать дома и помещения</param>
        /// <return>дочерние объекты</return>
        public static List<GarObject> GetObjects(string id, bool ignoreHouses = false)
        {
            try 
            {
                if (Pullenti.Address.Internal.ServerHelper.ServerUri != null) 
                    return Pullenti.Address.Internal.ServerHelper.GetChildrenObjects(id, ignoreHouses);
                else 
                    return Pullenti.Address.Internal.GarHelper.GetChildrenObjects(id, ignoreHouses);
            }
            catch(Exception ex105) 
            {
                return null;
            }
        }
        /// <summary>
        /// Получить объект (вместе с родительской иерархией) по идентификатору
        /// </summary>
        /// <param name="objId">внутренний идентификатор</param>
        /// <return>объект</return>
        public static GarObject GetObject(string objId)
        {
            if (string.IsNullOrEmpty(objId)) 
                return null;
            try 
            {
                if (Pullenti.Address.Internal.ServerHelper.ServerUri != null) 
                    return Pullenti.Address.Internal.ServerHelper.GetObject(objId);
                else 
                    return Pullenti.Address.Internal.GarHelper.GetObject(objId);
            }
            catch(Exception ex106) 
            {
                return null;
            }
        }
        public static List<TextAddress> CreateTextAddressByAnalysisResult(Pullenti.Ner.AnalysisResult ar)
        {
            Pullenti.Address.Internal.AnalyzeHelper ah = new Pullenti.Address.Internal.AnalyzeHelper();
            return ah._analyze1(ar, ar.Sofa.Text, null, false);
        }
        public static TextAddress CreateTextAddressByReferent(Pullenti.Ner.Referent r)
        {
            Pullenti.Address.Internal.AnalyzeHelper ah = new Pullenti.Address.Internal.AnalyzeHelper();
            return ah.CreateTextAddressByReferent(r);
        }
    }
}