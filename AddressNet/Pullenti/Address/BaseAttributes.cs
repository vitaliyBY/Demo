/*
 * SDK Pullenti Address, version 4.20, august 2023. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System.Text;
using System.Xml;

namespace Pullenti.Address
{
    /// <summary>
    /// Базовый класс для атрибутивных классов: AreaAttributes, HouseAttributes, RoomAttributes, SpecialAttributes
    /// </summary>
    public abstract class BaseAttributes
    {
        /// <summary>
        /// Вывести детальную информацию об атрибутах в текстовом виде
        /// </summary>
        public virtual void OutInfo(StringBuilder res)
        {
        }
        public virtual void Serialize(XmlWriter xml)
        {
        }
        public virtual void Deserialize(XmlNode xml)
        {
        }
        public virtual BaseAttributes Clone()
        {
            return null;
        }
    }
}