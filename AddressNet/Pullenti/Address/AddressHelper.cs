/*
 * SDK Pullenti Address, version 4.20, august 2023. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Collections;
using System.Collections.Generic;

namespace Pullenti.Address
{
    /// <summary>
    /// Разные полезные функции
    /// </summary>
    public static class AddressHelper
    {
        /// <summary>
        /// Получить описание уровня ГАР
        /// </summary>
        public static string GetGarLevelString(GarLevel level)
        {
            if (level == GarLevel.Region) 
                return "регион";
            if (level == GarLevel.AdminArea) 
                return "административный район";
            if (level == GarLevel.MunicipalArea) 
                return "муниципальный район";
            if (level == GarLevel.Settlement) 
                return "сельское/городское поселение";
            if (level == GarLevel.City) 
                return "город";
            if (level == GarLevel.Locality) 
                return "населенный пункт";
            if (level == GarLevel.District) 
                return "район города";
            if (level == GarLevel.Area) 
                return "элемент планировочной структуры";
            if (level == GarLevel.Street) 
                return "элемент улично-дорожной сети";
            if (level == GarLevel.Plot) 
                return "земельный участок";
            if (level == GarLevel.Building) 
                return "здание (сооружение)";
            if (level == GarLevel.Room) 
                return "помещение";
            if (level == GarLevel.Carplace) 
                return "машино-место";
            return level.ToString();
        }
        /// <summary>
        /// Получить описание алресного уровня
        /// </summary>
        public static string GetAddrLevelString(AddrLevel level)
        {
            if (level == AddrLevel.Country) 
                return "страна";
            if (level == AddrLevel.RegionArea) 
                return "регион";
            if (level == AddrLevel.RegionCity) 
                return "город-регион";
            if (level == AddrLevel.District) 
                return "район";
            if (level == AddrLevel.Settlement) 
                return "поселение";
            if (level == AddrLevel.City) 
                return "город";
            if (level == AddrLevel.CityDistrict) 
                return "городской район";
            if (level == AddrLevel.Locality) 
                return "населенный пункт";
            if (level == AddrLevel.Territory) 
                return "элемент планировочной структуры";
            if (level == AddrLevel.Street) 
                return "элемент улично-дорожной сети";
            if (level == AddrLevel.Plot) 
                return "земельный участок";
            if (level == AddrLevel.Building) 
                return "здание (сооружение)";
            if (level == AddrLevel.Apartment) 
                return "помещение";
            if (level == AddrLevel.Room) 
                return "комната";
            return level.ToString();
        }
        /// <summary>
        /// Получить мнемонику картинки для уровня (по мнемонике саму картинку можно получить функцией FindImage)
        /// </summary>
        public static string GetGarLevelImageName(GarLevel Level)
        {
            if (Level == GarLevel.Region) 
                return "region";
            if (Level == GarLevel.AdminArea) 
                return "admin";
            if (Level == GarLevel.MunicipalArea) 
                return "municipal";
            if (Level == GarLevel.Settlement) 
                return "settlement";
            if (Level == GarLevel.City) 
                return "city";
            if (Level == GarLevel.Locality) 
                return "locality";
            if (Level == GarLevel.District) 
                return "district";
            if (Level == GarLevel.Area) 
                return "area";
            if (Level == GarLevel.Street) 
                return "street";
            if (Level == GarLevel.Plot) 
                return "plot";
            if (Level == GarLevel.Building) 
                return "building";
            if (Level == GarLevel.Room) 
                return "room";
            if (Level == GarLevel.Carplace) 
                return "carplace";
            return "undefined";
        }
        /// <summary>
        /// Получить мнемонику картинки для уровня (по мнемонике саму картинку можно получить функцией FindImage)
        /// </summary>
        public static string GetAddrLevelImageName(AddrLevel Level)
        {
            if (Level == AddrLevel.Country) 
                return "country";
            if (Level == AddrLevel.RegionArea) 
                return "region";
            if (Level == AddrLevel.RegionCity) 
                return "city";
            if (Level == AddrLevel.District) 
                return "municipal";
            if (Level == AddrLevel.Settlement) 
                return "settlement";
            if (Level == AddrLevel.City) 
                return "city";
            if (Level == AddrLevel.CityDistrict) 
                return "municipal";
            if (Level == AddrLevel.Locality) 
                return "locality";
            if (Level == AddrLevel.Territory) 
                return "area";
            if (Level == AddrLevel.Street) 
                return "street";
            if (Level == AddrLevel.Plot) 
                return "plot";
            if (Level == AddrLevel.Building) 
                return "building";
            if (Level == AddrLevel.Apartment) 
                return "room";
            if (Level == AddrLevel.Room) 
                return "room";
            return "undefined";
        }
        /// <summary>
        /// Сравнение уровней для сортировки
        /// </summary>
        /// <param name="lev1">первый уровень</param>
        /// <param name="lev2">второй уровень</param>
        /// <return>-1 первый меньше, +1 первый больше, 0 равны</return>
        public static int CompareLevels(AddrLevel lev1, AddrLevel lev2)
        {
            int r1 = (int)lev1;
            int r2 = (int)lev2;
            if (r1 < r2) 
                return -1;
            if (r1 > r2) 
                return 1;
            return 0;
        }
        public static bool CanBeEqualLevels(AddrLevel a, GarLevel g)
        {
            if (a == AddrLevel.Country) 
                return false;
            if (a == AddrLevel.RegionCity || a == AddrLevel.RegionArea) 
                return g == GarLevel.Region;
            if (a == AddrLevel.District) 
                return g == GarLevel.MunicipalArea || g == GarLevel.AdminArea;
            if (a == AddrLevel.Settlement) 
                return g == GarLevel.Settlement;
            if (a == AddrLevel.City) 
                return g == GarLevel.City;
            if (a == AddrLevel.Locality) 
                return g == GarLevel.Locality || g == GarLevel.Area || g == GarLevel.AdminArea;
            if (a == AddrLevel.Territory) 
                return g == GarLevel.Area || g == GarLevel.District;
            if (a == AddrLevel.Street) 
                return g == GarLevel.Street;
            if (a == AddrLevel.Plot) 
                return g == GarLevel.Plot;
            if (a == AddrLevel.Building) 
                return g == GarLevel.Building;
            if (a == AddrLevel.Apartment || a == AddrLevel.Room) 
                return g == GarLevel.Room;
            return false;
        }
        /// <summary>
        /// Проверка уровней на предмет прямого родителя
        /// </summary>
        /// <param name="ch">прямой потомок</param>
        /// <param name="par">родитель</param>
        /// <return>может ли быть</return>
        public static bool CanBeParent(AddrLevel ch, AddrLevel par)
        {
            if (ch == AddrLevel.Country) 
                return false;
            if (ch == AddrLevel.RegionCity || ch == AddrLevel.RegionArea) 
                return par == AddrLevel.Country;
            if (ch == AddrLevel.District) 
            {
                if (par == AddrLevel.Country || par == AddrLevel.RegionCity || par == AddrLevel.RegionArea) 
                    return true;
                if (par == AddrLevel.District) 
                    return true;
            }
            if (ch == AddrLevel.Settlement) 
                return par == AddrLevel.RegionCity || par == AddrLevel.RegionArea || par == AddrLevel.District;
            if (ch == AddrLevel.City) 
                return (par == AddrLevel.Country || par == AddrLevel.RegionCity || par == AddrLevel.RegionArea) || par == AddrLevel.District || par == AddrLevel.Settlement;
            if (ch == AddrLevel.CityDistrict) 
                return par == AddrLevel.City;
            if (ch == AddrLevel.Locality) 
            {
                if ((par == AddrLevel.District || par == AddrLevel.Settlement || par == AddrLevel.City) || par == AddrLevel.RegionCity) 
                    return true;
                if (par == AddrLevel.CityDistrict) 
                    return true;
                if (par == AddrLevel.Locality) 
                    return true;
                return false;
            }
            if (ch == AddrLevel.Territory) 
            {
                if (par == AddrLevel.RegionCity) 
                    return true;
                if ((par == AddrLevel.Locality || par == AddrLevel.City || par == AddrLevel.District) || par == AddrLevel.CityDistrict || par == AddrLevel.Settlement) 
                    return true;
                if (par == AddrLevel.Territory) 
                    return true;
                return false;
            }
            if (ch == AddrLevel.Street) 
            {
                if ((par == AddrLevel.RegionCity || par == AddrLevel.Locality || par == AddrLevel.City) || par == AddrLevel.Territory || par == AddrLevel.CityDistrict) 
                    return true;
                if (par == AddrLevel.District) 
                    return true;
                return false;
            }
            if (ch == AddrLevel.Building || ch == AddrLevel.Plot) 
            {
                if (par == AddrLevel.Locality || par == AddrLevel.Territory || par == AddrLevel.Street) 
                    return true;
                if (par == AddrLevel.City && ch == AddrLevel.Building) 
                    return true;
                if (par == AddrLevel.Plot && ch == AddrLevel.Building) 
                    return true;
                return false;
            }
            if (ch == AddrLevel.Apartment) 
            {
                if (par == AddrLevel.Building) 
                    return true;
                return false;
            }
            if (ch == AddrLevel.Room) 
                return par == AddrLevel.Apartment || par == AddrLevel.Building;
            return false;
        }
        /// <summary>
        /// Получить описание для типа дома
        /// </summary>
        /// <param name="ty">тип</param>
        /// <param name="shortVal">в короткой форме</param>
        public static string GetHouseTypeString(HouseType ty, bool shortVal)
        {
            if (ty == HouseType.Estate) 
                return (shortVal ? "влад." : "владение");
            if (ty == HouseType.HouseEstate) 
                return (shortVal ? "дмвлд." : "домовладение");
            if (ty == HouseType.House) 
                return (shortVal ? "д." : "дом");
            if (ty == HouseType.Plot) 
                return (shortVal ? "уч." : "участок");
            if (ty == HouseType.Garage) 
                return (shortVal ? "гар." : "гараж");
            if (ty == HouseType.Special) 
                return (shortVal ? "" : "специальное строение");
            if (ty == HouseType.Well) 
                return (shortVal ? "скваж." : "скважина");
            return "?";
        }
        /// <summary>
        /// Получить описание для типа строения
        /// </summary>
        /// <param name="ty">тип</param>
        /// <param name="shortVal">в короткой форме</param>
        public static string GetStroenTypeString(StroenType ty, bool shortVal)
        {
            if (ty == StroenType.Construction) 
                return (shortVal ? "сооруж." : "сооружение");
            if (ty == StroenType.Liter) 
                return (shortVal ? "лит." : "литера");
            return (shortVal ? "стр." : "строение");
        }
        /// <summary>
        /// Получить описание для типа помещения
        /// </summary>
        /// <param name="ty">тип</param>
        /// <param name="shortVal">в короткой форме</param>
        public static string GetRoomTypeString(RoomType ty, bool shortVal)
        {
            if (ty == RoomType.Flat) 
                return (shortVal ? "кв." : "квартира");
            if (ty == RoomType.Office) 
                return (shortVal ? "оф." : "офис");
            if (ty == RoomType.Room) 
                return (shortVal ? "комн." : "комната");
            if (ty == RoomType.Space || ty == RoomType.Undefined) 
                return (shortVal ? "помещ." : "помещение");
            if (ty == RoomType.Garage) 
                return (shortVal ? "гар." : "гараж");
            if (ty == RoomType.Carplace) 
                return (shortVal ? "маш.м." : "машиноместо");
            if (ty == RoomType.Pavilion) 
                return (shortVal ? "пав." : "павильон");
            if (ty == RoomType.Panty) 
                return (shortVal ? "клад." : "кладовка");
            return "?";
        }
        /// <summary>
        /// Картинки (иконки) для ГАР-объектов
        /// </summary>
        public static List<ImageWrapper> Images = new List<ImageWrapper>();
        /// <summary>
        /// Найти картинку по идентификатору
        /// </summary>
        /// <param name="imageId">Id картинки</param>
        /// <return>обёртка</return>
        public static ImageWrapper FindImage(string imageId)
        {
            foreach (ImageWrapper img in Images) 
            {
                if (string.Compare(img.Id, imageId, true) == 0) 
                    return img;
            }
            return null;
        }
        /// <summary>
        /// Проверка, что спецтип является направлением
        /// </summary>
        public static bool IsSpecTypeDirection(DetailType typ)
        {
            if ((typ == DetailType.North || typ == DetailType.East || typ == DetailType.West) || typ == DetailType.South) 
                return true;
            if ((typ == DetailType.NorthEast || typ == DetailType.NorthWest || typ == DetailType.SouthEast) || typ == DetailType.SouthWest) 
                return true;
            return false;
        }
        /// <summary>
        /// Получить описание для типа дополнительного параметра
        /// </summary>
        /// <param name="typ">тип</param>
        /// <return>строковое описание</return>
        public static string GetDetailTypeString(DetailType typ)
        {
            if (typ == DetailType.Near) 
                return "вблизи";
            if (typ == DetailType.Central) 
                return "центр";
            if (typ == DetailType.Left) 
                return "левее";
            if (typ == DetailType.Right) 
                return "правее";
            if (typ == DetailType.North) 
                return "на север";
            if (typ == DetailType.West) 
                return "на запад";
            if (typ == DetailType.South) 
                return "на юг";
            if (typ == DetailType.East) 
                return "на восток";
            if (typ == DetailType.NorthEast) 
                return "на северо-восток";
            if (typ == DetailType.NorthWest) 
                return "на северо-запад";
            if (typ == DetailType.SouthEast) 
                return "на юго-восток";
            if (typ == DetailType.SouthWest) 
                return "на юго-запад";
            if (typ == DetailType.KmRange) 
                return "диапазон";
            return typ.ToString();
        }
        public static string GetDetailPartParamString(DetailType typ)
        {
            if (typ == DetailType.Central) 
                return "центральная часть";
            if (typ == DetailType.North) 
                return "северная часть";
            if (typ == DetailType.West) 
                return "западная часть";
            if (typ == DetailType.South) 
                return "южная часть";
            if (typ == DetailType.East) 
                return "восточная часть";
            if (typ == DetailType.NorthEast) 
                return "северо-восточная часть";
            if (typ == DetailType.NorthWest) 
                return "северо-западная часть";
            if (typ == DetailType.SouthEast) 
                return "юго-восточная часть";
            if (typ == DetailType.SouthWest) 
                return "юго-западная часть";
            if (typ == DetailType.Left) 
                return "левая часть";
            if (typ == DetailType.Right) 
                return "правая часть";
            return typ.ToString();
        }
        /// <summary>
        /// Проверка, является ли тип доп.параметра направлением (на сервер, на юг и т.д.)
        /// </summary>
        public static bool IsDetailParamDirection(DetailType typ)
        {
            if ((((((typ == DetailType.Near || typ == DetailType.Central || typ == DetailType.North) || typ == DetailType.West || typ == DetailType.South) || typ == DetailType.East || typ == DetailType.NorthEast) || typ == DetailType.NorthWest || typ == DetailType.SouthEast) || typ == DetailType.SouthWest || typ == DetailType.Left) || typ == DetailType.Right) 
                return true;
            return false;
        }
        /// <summary>
        /// Получить описание для типа дополнительного параметра
        /// </summary>
        /// <param name="typ">тип</param>
        /// <return>строковое описание</return>
        public static string GetParamTypeString(ParamType typ)
        {
            if (typ == ParamType.Order) 
                return "очередь";
            if (typ == ParamType.Part) 
                return "часть";
            if (typ == ParamType.Floor) 
                return "этаж";
            if (typ == ParamType.Genplan) 
                return "ГП";
            if (typ == ParamType.DeliveryArea) 
                return "доставочный участок";
            if (typ == ParamType.Zip) 
                return "индекс";
            if (typ == ParamType.SubscriberBox) 
                return "а/я";
            return typ.ToString();
        }
    }
}