/*
 * SDK Pullenti Address, version 4.20, august 2023. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Collections;
using System.Collections.Generic;

namespace Pullenti.Util.Repository
{
    public class DateIndexTable : FixRecordBaseTable
    {
        public DateIndexTable(IRepository index, string name) : base(index, name)
        {
        }
        static DateTime? s_MinDate = null;
        protected int CalcKey(DateTime dt, bool readOnly)
        {
            if (s_MinDate == null) 
            {
                byte[] dat = this._readIndex0();
                if (dat != null) 
                {
                    int i = 0;
                    s_MinDate = BaseTable.ToDate(dat, ref i);
                    if (s_MinDate == null) 
                        s_MinDate = (new DateTime(2010, 1, 1)).AddDays(-1);
                }
            }
            if (s_MinDate == null) 
            {
                if (readOnly) 
                    return -1;
                s_MinDate = dt.Date;
                List<byte> tmp = new List<byte>();
                BaseTable.GetBytesForDate(tmp, s_MinDate.Value);
                this._writeIndex0(tmp.ToArray());
            }
            else if (s_MinDate.Value > dt.Date) 
            {
                if (readOnly) 
                    return -1;
                this.Flush();
                TimeSpan ts0 = s_MinDate.Value - dt.Date;
                this._shiftIndex((int)ts0.TotalDays);
                s_MinDate = dt.Date;
                List<byte> tmp = new List<byte>();
                BaseTable.GetBytesForDate(tmp, s_MinDate.Value);
                this._writeIndex0(tmp.ToArray());
            }
            TimeSpan ts = dt.Date - s_MinDate.Value;
            return ((int)ts.TotalDays) + 1;
        }
        public DateTime? GetDateById(int id)
        {
            if (s_MinDate == null) 
                this.CalcKey(DateTime.Now, true);
            if (s_MinDate == null) 
                return null;
            return s_MinDate.Value.AddDays(id - 1);
        }
    }
}