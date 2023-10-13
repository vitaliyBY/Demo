/*
 * SDK Pullenti Address, version 4.20, august 2023. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Demo
{
    class Program
    {
        public static void Main(string[] args)
        {
            // Можно работать через сервер, тогда инициалиация не нужна
            if (Pullenti.Address.AddressService.SetServerConnection("http://localhost:2222")) 
            {
                string serverVersion = Pullenti.Address.AddressService.GetServerVersion("http://localhost:2222");
                Console.WriteLine("Server version: {0}", serverVersion ?? "?");
            }
            else 
            {
                // Обязательная инициализация один раз перед использованием SDK
                Console.Write("Initialize SDK Pullenti Address v.{0} ... ", Pullenti.Address.AddressService.Version);
                Pullenti.Address.AddressService.Initialize();
                Console.WriteLine("OK");
                // Указание SDK папки с индексом ГАР
                string garPath = "Gar77";
                if (!Directory.Exists(garPath)) 
                {
                    Console.WriteLine("Gar path {0} not exists", garPath);
                    return;
                }
                Pullenti.Address.AddressService.SetGarIndexPath(garPath);
            }
            Pullenti.Address.GarStatistic info = Pullenti.Address.AddressService.GetGarStatistic();
            if (info != null) 
                Console.WriteLine(info.ToString());
            Console.WriteLine("Root Gar Objects:");
            // посмотрим объекты 1-го уровня (в демо-варианте только Москва)
            List<Pullenti.Address.GarObject> rootGars = Pullenti.Address.AddressService.GetObjects(null, false);
            Pullenti.Address.GarObject garMoscow = null;
            if (rootGars != null) 
            {
                foreach (Pullenti.Address.GarObject go1 in rootGars) 
                {
                    // а вот так можно получить дочерние объекты (по Id родителя)
                    List<Pullenti.Address.GarObject> children = Pullenti.Address.AddressService.GetObjects(go1.Id, false);
                    Console.WriteLine("{0} ({1} children)", go1.ToString(), (children == null ? 0 : children.Count));
                    // установим по умолчанию город - Москву
                    if (go1.ToString() == "город Москва") 
                        garMoscow = go1;
                }
            }
            // поищем по именам
            Pullenti.Address.SearchParams sp = new Pullenti.Address.SearchParams();
            sp.Street = "советск";
            sp.MaxCount = 100;
            Console.Write("\nSearch {0} ...", sp);
            Pullenti.Address.SearchResult sr = Pullenti.Address.AddressService.SearchObjects(sp);
            if (sr != null) 
            {
                Console.Write(" found {0} object(s)", sr.Objects.Count);
                foreach (Pullenti.Address.GarObject o in sr.Objects) 
                {
                    Console.Write("\n{0} ({1})", o.ToString(), o.Guid);
                    Pullenti.Address.GarObject parent = Pullenti.Address.AddressService.GetObject((o.ParentIds.Count > 0 ? o.ParentIds[0] : null));
                    if (parent != null) 
                        Console.Write(" - {0}", parent.ToString());
                }
            }
            else 
                Console.WriteLine("Fatal search error");
            // дополнительные параметры анализа
            Pullenti.Address.ProcessTextParams pars = new Pullenti.Address.ProcessTextParams();
            // дефолтовый регион (или можно явно город как pars.DefaultObject = garMoscow)
            pars.DefaultRegions.Add(77);
            // анализ текстовых фрагментов с адресами
            string text = "адрес 16 парковая улица, дом номер два квартира 3 в которой проживает кое-кто";
            Console.WriteLine("\n\nAnalyze text: {0}", text);
            List<Pullenti.Address.TextAddress> addrs = Pullenti.Address.AddressService.ProcessText(text, pars);
            if (addrs != null) 
            {
                foreach (Pullenti.Address.TextAddress addr in addrs) 
                {
                    Console.WriteLine("Address: {0}", addr.GetFullPath(", "));
                    // детализируем элементы адреса
                    foreach (Pullenti.Address.AddrObject item in addr.Items) 
                    {
                        Console.WriteLine("  {0}: {1}", item.Level, item.ToString());
                        if (item.Gars.Count > 0) 
                        {
                            // привязанных объектов ГАР в принципе может быть несколько
                            foreach (Pullenti.Address.GarObject gar in item.Gars) 
                            {
                                Console.WriteLine("   Gar: {0} ({1})", gar.ToString(), gar.Guid);
                            }
                        }
                    }
                }
            }
            // а вот анализ текста, который содержит ТОЛЬКО АДРЕС, и ничего более (поле ввода адреса, например)
            text = "Москва, ул. 16-я Парковая д.2 кв.3 и какой-то мусор";
            Pullenti.Address.TextAddress saddr = Pullenti.Address.AddressService.ProcessSingleAddressText(text, pars);
            Console.WriteLine("\nAnalyze single address: {0}", text);
            if (saddr == null) 
                Console.WriteLine("Fatal process error");
            else 
            {
                Console.WriteLine("Coefficient: {0}", saddr.Coef);
                if (saddr.ErrorMessage != null) 
                    Console.WriteLine("Message: {0}", saddr.ErrorMessage);
                foreach (Pullenti.Address.AddrObject item in saddr.Items) 
                {
                    Console.Write("Item: {0}", item.ToString());
                    if (item.Gars.Count > 0) 
                    {
                        foreach (Pullenti.Address.GarObject gar in item.Gars) 
                        {
                            Console.Write(" (Gar: {0}, GUID={1})", gar.ToString(), gar.Guid);
                        }
                    }
                    Console.WriteLine("");
                }
            }
            Console.WriteLine("Bye!");
        }
    }
}