/*
 * SDK Pullenti Address, version 4.20, august 2023. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.IO;
using System.Text;
using System.Threading;

namespace Pullenti.Util
{
    /// <summary>
    /// Различные утилитки работы с файлами
    /// </summary>
    public static class FileHelper
    {
        /// <summary>
        /// Сохранение данных в файле
        /// </summary>
        /// <param name="fileName">имя файла</param>
        /// <param name="data">сохраняемая последовательсноть байт</param>
        public static void SaveDataToFile(string fileName, byte[] data, int len = -1)
        {
            if (data == null) 
                return;
            FileStream f = null;
            try 
            {
                f = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                f.Write(data, 0, (len < 0 ? data.Length : len));
            }
            finally
            {
                if (f != null) 
                    f.Dispose();
            }
        }
        /// <summary>
        /// Получить последовательность байт из файла.
        /// </summary>
        /// <param name="fileName">имя файла</param>
        /// <param name="attampts">число попыток с небольшой задержкой</param>
        /// <return>последовательнсоть байт, null, если файл пусто</return>
        public static byte[] LoadDataFromFile(string fileName, int attampts = 0)
        {
            FileStream f = null;
            byte[] buf = null;
            try 
            {
                Exception ex = null;
                for (int i = 0; i <= attampts; i++) 
                {
                    try 
                    {
                        f = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        break;
                    }
                    catch(Exception e) 
                    {
                        ex = e;
                    }
                    if (i == 0 && !File.Exists(fileName)) 
                        break;
                    try 
                    {
                        Thread.Sleep(500);
                    }
                    catch(Exception ee) 
                    {
                    }
                }
                if (f == null) 
                    throw ex;
                if (f.Length == 0) 
                    return null;
                buf = new byte[(int)f.Length];
                f.Read(buf, 0, (int)f.Length);
            }
            finally
            {
                if (f != null) 
                    f.Dispose();
            }
            return buf;
        }
        /// <summary>
        /// Проверка существования файла по его имени или шаблону (типа *.*). 
        /// Если файл существует и в него кто-то сейчас записывает, то ожидать конца записи.
        /// </summary>
        public static bool IsFileExists(string pattern)
        {
            if (string.IsNullOrEmpty(pattern)) 
                return false;
            try 
            {
                string dir = Path.GetDirectoryName(pattern);
                if (dir == null) 
                    throw new ArgumentException("Невозможно определить папку для пути '" + pattern + "'");
                string name = pattern.Substring(dir.Length);
                if (name[0] == '\\') 
                    name = name.Substring(1);
                string[] files = Directory.GetFiles(dir, name);
                if (files.Length > 0) 
                {
                    if (files.Length == 1) 
                        return CheckFileReady(files[0]);
                    return true;
                }
            }
            catch(Exception ex4373) 
            {
            }
            return false;
        }
        /// <summary>
        /// Метод копирования файла в папку назначения с изменением имени файла на уникальной.
        /// </summary>
        /// <param name="sourceFilePath">Путь к исходному файлу</param>
        /// <param name="destinationFolder">Папка назначения</param>
        /// <return>Полный путь куда скопирован файл</return>
        public static string CopyFileToFolder(string sourceFilePath, string destinationFolder)
        {
            string fileName = Path.GetFileNameWithoutExtension(sourceFilePath);
            string fileExt = Path.GetExtension(sourceFilePath);
            if (string.IsNullOrEmpty(fileName)) 
                throw new ArgumentException("Исходный путь не содержит имени файла. Путь'" + sourceFilePath + "'");
            if (!Directory.Exists(destinationFolder)) 
                throw new ArgumentException("Папка назначения отсутствует.\nПапка назначения'" + destinationFolder + "'");
            string destinationPath = Path.Combine(destinationFolder, fileName + fileExt);
            int i = 1;
            while (File.Exists(destinationPath)) 
            {
                destinationPath = Path.Combine(destinationFolder, (fileName + "_" + i) + fileExt);
                i++;
            }
            File.Copy(sourceFilePath, destinationPath);
            return destinationPath;
        }
        /// <summary>
        /// Проверка, что файл существует и в него никто не пишет. 
        /// А если пишет, то дождаться окончания записи.
        /// </summary>
        public static bool CheckFileReady(string fileName)
        {
            bool ok = false;
            while (true) 
            {
                try 
                {
                    using (FileStream f = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite)) 
                    {
                        ok = true;
                        break;
                    }
                }
                catch(Exception ex4374) 
                {
                }
                try 
                {
                    Thread.Sleep(1000);
                }
                catch(Exception ex) 
                {
                }
                if (!File.Exists(fileName)) 
                    break;
            }
            return ok;
        }
        /// <summary>
        /// Удаление объекта\объектов
        /// </summary>
        /// <param name="path">файл, шаблон или директория</param>
        /// <param name="removeRoot">удалять ли саму директорию (при false только очистка)</param>
        public static bool Remove(string path, bool removeRoot = true)
        {
            bool ret = true;
            string[] fnames = null;
            try 
            {
                fnames = Directory.GetFiles(path);
            }
            catch(Exception ex4375) 
            {
            }
            if (fnames != null) 
            {
                foreach (string f in fnames) 
                {
                    try 
                    {
                        File.Delete(f);
                    }
                    catch(Exception ex) 
                    {
                        ret = false;
                    }
                }
            }
            string[] dirs = null;
            try 
            {
                dirs = Directory.GetDirectories(path);
            }
            catch(Exception ex4376) 
            {
            }
            if (dirs != null) 
            {
                foreach (string d in dirs) 
                {
                    Remove(d, true);
                }
            }
            if (Directory.Exists(path) && removeRoot) 
            {
                try 
                {
                    Directory.Delete(path);
                    ret = true;
                }
                catch(Exception ex) 
                {
                    ret = false;
                }
            }
            return ret;
        }
        /// <summary>
        /// Создание дорожки
        /// </summary>
        public static bool CreateFullPath(string path)
        {
            if (string.IsNullOrEmpty(path)) 
                return false;
            string[] parts = path.Split('\\');
            if (parts == null || (parts.Length < 1)) 
                return false;
            string dir = parts[0];
            try 
            {
                if (dir.Length == 2 && dir[1] == ':') 
                {
                }
                else if (!Directory.Exists(dir)) 
                    Directory.CreateDirectory(dir);
                for (int i = 1; i < parts.Length; i++) 
                {
                    if (dir.Length == 2 && dir[1] == ':') 
                        dir += ("\\" + parts[i]);
                    else 
                        dir = Path.Combine(dir, parts[i]);
                    if (!Directory.Exists(dir)) 
                        Directory.CreateDirectory(dir);
                }
                return true;
            }
            catch(Exception ex) 
            {
                return false;
            }
        }
        public static void CopyDirectory(string src, string dst)
        {
            if (!Directory.Exists(dst)) 
                Directory.CreateDirectory(dst);
            foreach (string f in Directory.GetFiles(src)) 
            {
                File.Copy(f, Path.Combine(dst, Path.GetFileName(f)), true);
            }
            foreach (string d in Directory.GetDirectories(src)) 
            {
                CopyDirectory(d, Path.Combine(dst, Path.GetFileName(d)));
            }
        }
        /// <summary>
        /// Сохранение текста в файл. Формат UTF-8, вставляется префикс EF BB BF.
        /// </summary>
        public static void WriteStringToFile(string str, string fileName)
        {
            if (str == null) 
                str = "";
            byte[] data = Encoding.UTF8.GetBytes(str);
            byte[] pream = Encoding.UTF8.GetPreamble();
            if (data.Length > pream.Length) 
            {
                int i;
                for (i = 0; i < pream.Length; i++) 
                {
                    if (pream[i] != data[i]) 
                        break;
                }
                if (i >= pream.Length) 
                    pream = null;
            }
            if (str.Length == 0) 
                pream = null;
            FileStream fStr = null;
            try 
            {
                fStr = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                if (pream != null) 
                    fStr.Write(pream, 0, pream.Length);
                fStr.Write(data, 0, data.Length);
            }
            finally
            {
                if (fStr != null) 
                    fStr.Dispose();
            }
        }
    }
}