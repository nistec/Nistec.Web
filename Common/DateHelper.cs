using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;


namespace Nistec.Web
{

    /// <summary>
    /// Summary description for PF
    /// </summary>
    public static class DateHelper
    {
        public static DateTime MinDate { get { return new DateTime(1900, 1, 1); } }

        public static DateTime? DateTimeTryParse(string text)
        {
            DateTime date;
            return DateTime.TryParse(text, out date) ? date : (DateTime?)null;
        }

        #region Date time function

        public static string FormtDate(string date)
        {
            DateTime res = DateTime.Now;
            if (DateTime.TryParse(date, DateFormat, System.Globalization.DateTimeStyles.AssumeLocal, out res))
            {
                return res.ToString("dd/MM/yyyy");
            }
            return "";
           
            //return MControl.Types.FormatDate(value, "dd/MM/yyyy", "");
        }

        public static string FormtDate(DateTime value)
        {
            return Types.FormatDate(value.ToString(), "dd/MM/yyyy", "");
        }

        public static bool IsDateTime(string date)
        {
            DateTime res;
            return DateTime.TryParse(date, DateFormat, System.Globalization.DateTimeStyles.None, out res);
        }

        public static bool IsDateTime(string date, string cultre)
        {
            DateTime res = DateTime.Now;
            return DateTime.TryParse(date, new System.Globalization.CultureInfo(cultre, false).DateTimeFormat, System.Globalization.DateTimeStyles.None, out res);
        }

        public static DateTime? ToNullableDateTime(string date, DateTime? defaultValue=null)
        {
            DateTime res;
            return DateTime.TryParse(date, DateFormat, System.Globalization.DateTimeStyles.AssumeLocal, out res) ? res : (DateTime?)defaultValue;
        }

        public static DateTime ToDateTime(string date)
        {
            DateTime res = DateTime.Now;
            if (DateTime.TryParse(date, DateFormat, System.Globalization.DateTimeStyles.AssumeLocal, out res))
                return res;
            return DateTime.Now;
        }

        public static DateTime ToDateTime(string date, string cultre)
        {
            DateTime res = DateTime.Now;
            if (DateTime.TryParse(date, new System.Globalization.CultureInfo(cultre, false).DateTimeFormat, System.Globalization.DateTimeStyles.None, out res))
                return res;
            return DateTime.Now;
        }

         public static DateTime ToDateTime(string date, DateTime defaultValue)
        {
            return Types.ToDateTime(date, DateFormat , defaultValue);
        }

         public static DateTime EndOfDay(this DateTime date)
         {
             return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, 999);
         }

         public static DateTime StartOfDay(this DateTime date)
         {
             return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0);
         }
        
        public static System.Globalization.DateTimeFormatInfo DateFormat
        {
            //get{return new System.Globalization.DateTimeFormatInfo;}
            get { return new System.Globalization.CultureInfo("he-IL", false).DateTimeFormat; }
        }

        public static DateTime GetDefaultDateFrom
        {
            get
            {
                //System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat

                //return ToDateTime(DateTime.Now.AddMonths(-1).ToString(), "en-US");
                return DateTime.Now.AddMonths(-1);
            }
        }

        public static DateTime GetDefaultDateTo
        {
            get
            {

                //return ToDateTime(DateTime.Now.AddDays(1).ToString(),"en-US");
                return DateTime.Now.AddDays(1);
            }
        }
        #endregion

        #region date time converter

        public enum DateFormatStyle
        {
            He = 0,
            US = 1,
            SQL = 2
        }

        /*
        public static DateTime ConvertToDate(string s)
        {
            return ConvertToDate(s, 0, true);
        }
        public static DateTime ConvertToDate(string s, DateFormatStyle dateFormat)
        {
            return ConvertToDate(s, (int)dateFormat, false);
        }
        static DateTime ConvertToDate(string s, int format, bool allFormat)
        {
            if (format > 2)
            {
                throw new Exception("Invalid date format: " + s);
            }
            string strDate = s;
            string re = null;
            DateTime date = DateTime.MaxValue;
            DateFormatStyle dateFormat = (DateFormatStyle)format;

            if (s.Length == 8)
            {
                switch (dateFormat)
                {
                    case DateFormatStyle.US:
                        strDate = s.Insert(2, "-").Insert(5, "-");
                        break;
                    case DateFormatStyle.SQL:
                        strDate = s.Insert(4, "-").Insert(7, "-");
                        break;
                    default:
                        strDate = s.Insert(2, "-").Insert(5, "-");
                        break;
                }
            }
            switch (dateFormat)
            {
                case DateFormatStyle.US:
                    re = "^([0]?[1-9]|[1][0-2])[./-]([0]?[1-9]|[1|2][0-9]|[3][0|1])[./-]([0-9]{4}|[0-9]{2})$"; break;
                case DateFormatStyle.SQL:
                    re = "^([0-9]{4}|[0-9]{2})[./-]([0]?[1-9]|[1][0-2])[./-]([0]?[1-9]|[1|2][0-9]|[3][0|1])$"; break;
                default:
                    re = "^([0]?[1-9]|[1|2][0-9]|[3][0|1])[./-]([0]?[1-9]|[1][0-2])[./-]([0-9]{4}|[0-9]{2})$"; break;
            }

            Match m = Regex.Match(strDate, re);

            if (!m.Success || m.Groups.Count < 4)
            {
                if (allFormat)
                    return ConvertToDate(s, ++format, allFormat);
                else
                    throw new Exception("Invalid date format: " + strDate);
            }
            string day = "0";
            string month = "0";
            string year = "0";

            switch (dateFormat)
            {
                case DateFormatStyle.US:
                    day = m.Groups[2].Value; month = m.Groups[1].Value; year = m.Groups[3].Value; break;
                case DateFormatStyle.SQL:
                    day = m.Groups[3].Value; month = m.Groups[2].Value; year = m.Groups[1].Value; break;
                default:
                    day = m.Groups[1].Value; month = m.Groups[2].Value; year = m.Groups[3].Value; break;
            }

            strDate = year + "-" + month + "-" + day;

            if (!DateTime.TryParse(strDate, out date))
            {
                if (allFormat)
                    return ConvertToDate(s, ++format, allFormat);
                else
                    throw new Exception("Invalid date format: " + strDate);
            }
            return date;
        }
        
        public static DateTime ConvertToDateTime(string s)
        {
            return ConvertToDateTime(s, 0, true, true);
        }
        static DateTime ConvertToDateTime(string s, int format, bool enableTime, bool allFormat)
        {
            if (format > 2)
            {
                throw new Exception("Invalid date format: " + s);
            }
            string strDate = GetValidDateString(s, enableTime, (DateFormatStyle)format);

            string re = null;
            DateTime date = DateTime.MaxValue;
            DateFormatStyle dateFormat = (DateFormatStyle)format;


            if (enableTime)
            {
                switch (dateFormat)
                {
                    case DateFormatStyle.US:
                        re = "^([0]?[1-9]|[1][0-2])[./-]([0]?[1-9]|[1|2][0-9]|[3][0|1])[./-]([0-9]{4}|[0-9]{2})\\s(.*)"; break;
                    case DateFormatStyle.SQL:
                        re = "^([0-9]{4}|[0-9]{2})[./-]([0]?[1-9]|[1][0-2])[./-]([0]?[1-9]|[1|2][0-9]|[3][0|1])\\s(.*)"; break;
                    default:
                        re = "^([0]?[1-9]|[1|2][0-9]|[3][0|1])[./-]([0]?[1-9]|[1][0-2])[./-]([0-9]{4}|[0-9]{2})\\s(.*)"; break;
                }
            }
            else
            {
                switch (dateFormat)
                {
                    case DateFormatStyle.US:
                        re = "^([0]?[1-9]|[1][0-2])[./-]([0]?[1-9]|[1|2][0-9]|[3][0|1])[./-]([0-9]{4}|[0-9]{2})$"; break;
                    case DateFormatStyle.SQL:
                        re = "^([0-9]{4}|[0-9]{2})[./-]([0]?[1-9]|[1][0-2])[./-]([0]?[1-9]|[1|2][0-9]|[3][0|1])$"; break;
                    default:
                        re = "^([0]?[1-9]|[1|2][0-9]|[3][0|1])[./-]([0]?[1-9]|[1][0-2])[./-]([0-9]{4}|[0-9]{2})$"; break;
                }
            }
            Match m = Regex.Match(strDate, re);

            if (!m.Success || m.Groups.Count < 4)
            {
                if (allFormat)
                    return ConvertToDateTime(s, ++format, enableTime, allFormat);
                else
                    throw new Exception("Invalid date format: " + strDate);
            }
            string day = "0";
            string month = "0";
            string year = "0";
            string time = "";

            if (enableTime)
            {
                switch (dateFormat)
                {
                    case DateFormatStyle.US:
                        day = m.Groups[2].Value; month = m.Groups[1].Value; year = m.Groups[3].Value; time = m.Groups[4].Value; break;
                    case DateFormatStyle.SQL:
                        day = m.Groups[3].Value; month = m.Groups[2].Value; year = m.Groups[1].Value; time = m.Groups[4].Value; break;
                    default:
                        day = m.Groups[1].Value; month = m.Groups[2].Value; year = m.Groups[3].Value; time = m.Groups[4].Value; break;
                }
                strDate = year + "-" + month + "-" + day + " " + ConvertToTime(time);
            }
            else
            {
                switch (dateFormat)
                {
                    case DateFormatStyle.US:
                        day = m.Groups[2].Value; month = m.Groups[1].Value; year = m.Groups[3].Value; break;
                    case DateFormatStyle.SQL:
                        day = m.Groups[3].Value; month = m.Groups[2].Value; year = m.Groups[1].Value; break;
                    default:
                        day = m.Groups[1].Value; month = m.Groups[2].Value; year = m.Groups[3].Value; break;
                }
                strDate = year + "-" + month + "-" + day;
            }
            if (!DateTime.TryParse(strDate, out date))
            {
                if (allFormat)
                    return ConvertToDateTime(s, ++format, enableTime, allFormat);
                else
                    throw new Exception("Invalid date format: " + strDate);
            }
            return date;
        }
                static string GetValidDateString(string s, bool enableTime, DateFormatStyle dateFormat)
        {
            string strDate = s;

            if (enableTime)
            {
                if (s.Length < 10)
                    throw new Exception("Invalid date format: " + s);
                if (s.Length < 15)
                    strDate = s.Substring(0, 10) + " 00:00";
                else
                {
                    switch (dateFormat)
                    {
                        case DateFormatStyle.US:
                            if (s.Length > 18)
                                strDate = s.Substring(0, 19);
                            break;
                        case DateFormatStyle.SQL:
                        default:
                            if (s.Length > 16)
                                strDate = s.Substring(0, 16);
                            break;
                    }
                }
            }
            else
            {
                if (s.Length > 10)
                    strDate = s.Substring(0, 10);
                else if (s.Length == 8)
                {
                    switch (dateFormat)
                    {
                        case DateFormatStyle.US:
                            strDate = s.Insert(2, "-").Insert(5, "-");
                            break;
                        case DateFormatStyle.SQL:
                            strDate = s.Insert(4, "-").Insert(7, "-");
                            break;
                        default:
                            strDate = s.Insert(2, "-").Insert(5, "-");
                            break;
                    }
                }

            }
            return strDate;
        }

        */
        /*
        public static bool TryGetValidDate(string s, DateFormatStyle dateFormat, bool enableTime, out string validDate)
        {
            return TryGetValidDate(s, dateFormat, enableTime, out validDate);
        }
        public static bool TryGetValidDate(string s, DateFormatStyle dateFormat, bool enableTime, bool tryParse, out string validDate)
        {
            validDate = s;
            string strDate = s;
            string uDate = null;
            string re = null;
            DateTime date = DateTime.MaxValue;
  

            if (enableTime)
            {
                if (s.Length < 10)
                    return false;
                if (s.Length < 15)
                    strDate = s.Substring(0, 10) + " 00:00";

                switch (dateFormat)
                {
                    case DateFormatStyle.US:
                        if (s.Length > 18)
                            strDate = s.Substring(0, 19);
                        re = "^([0]?[1-9]|[1][0-2])[./-]([0]?[1-9]|[1|2][0-9]|[3][0|1])[./-]([0-9]{4}|[0-9]{2})\\s(.*)"; break;
                    case DateFormatStyle.SQL:
                        if (s.Length > 16)
                            strDate = s.Substring(0, 16);
                        re = "^([0-9]{4}|[0-9]{2})[./-]([0]?[1-9]|[1][0-2])[./-]([0]?[1-9]|[1|2][0-9]|[3][0|1])\\s(.*)"; break;
                    default:
                        if (s.Length > 16)
                            strDate = s.Substring(0, 16);
                        re = "^([0]?[1-9]|[1|2][0-9]|[3][0|1])[./-]([0]?[1-9]|[1][0-2])[./-]([0-9]{4}|[0-9]{2})\\s(.*)"; break;
                }
            }
            else
            {
                if (s.Length > 10)
                    strDate = s.Substring(0, 10);
                switch (dateFormat)
                {
                    case DateFormatStyle.US:
                        re = "^([0]?[1-9]|[1][0-2])[./-]([0]?[1-9]|[1|2][0-9]|[3][0|1])[./-]([0-9]{4}|[0-9]{2})$"; break;
                    case DateFormatStyle.SQL:
                        re = "^([0-9]{4}|[0-9]{2})[./-]([0]?[1-9]|[1][0-2])[./-]([0]?[1-9]|[1|2][0-9]|[3][0|1])$"; break;
                    default:
                        re = "^([0]?[1-9]|[1|2][0-9]|[3][0|1])[./-]([0]?[1-9]|[1][0-2])[./-]([0-9]{4}|[0-9]{2})$"; break;
                }
            }
            Match m = Regex.Match(s, re);

            if (!m.Success || m.Groups.Count < 4)
            {
                return false;
            }
            string day = "0";
            string month = "0";
            string year = "0";
            string time = "";

            if (enableTime)
            {
                switch (dateFormat)
                {
                    case DateFormatStyle.US:
                        day = m.Groups[2].Value; month = m.Groups[1].Value; year = m.Groups[3].Value; time = m.Groups[4].Value;
                        strDate = string.Format("{0}/{1}/{2} {3}", month, day, year, ConvertToTime(time));
                        break;
                    case DateFormatStyle.SQL:
                        day = m.Groups[3].Value; month = m.Groups[2].Value; year = m.Groups[1].Value; time = m.Groups[4].Value;
                        strDate = string.Format("{0}-{1}-{2} {3}", year, month, day, ConvertToTime(time));
                        break;
                    default:
                        day = m.Groups[1].Value; month = m.Groups[2].Value; year = m.Groups[3].Value; time = m.Groups[4].Value;
                        strDate = string.Format("{0}/{1}/{2} {3}", day, month, year, ConvertToTime(time));
                        break;
                }
                uDate = string.Format("{0}-{1}-{2} {3}", year, month, day, ConvertToTime(time));
            }
            else
            {
                switch (dateFormat)
                {
                    case DateFormatStyle.US:
                        day = m.Groups[2].Value; month = m.Groups[1].Value; year = m.Groups[3].Value;
                        strDate = string.Format("{0}/{1}/{2}", month, day, year);
                        break;
                    case DateFormatStyle.SQL:
                        day = m.Groups[3].Value; month = m.Groups[2].Value; year = m.Groups[1].Value;
                        strDate = string.Format("{0}-{1}-{2}", year, month, day);
                        break;
                    default:
                        day = m.Groups[1].Value; month = m.Groups[2].Value; year = m.Groups[3].Value;
                        strDate = string.Format("{0}/{1}/{2}", day, month, year);
                        break;
                }
                uDate = string.Format("{0}-{1}-{2}", year, month, day);
             }
            if (tryParse)
            {
                if (!DateTime.TryParse(uDate, out date))
                {
                    return false;
                }
            }
            validDate = strDate;
            return true;
        }
        */
        public static string ConvertToTime(string s)
        {

            string time = s.TrimStart();
            string re = null;
            DateFormatStyle dateFormat = Regex.IsMatch(s, "[AP]M") ? DateFormatStyle.US : DateFormatStyle.SQL;

            switch (dateFormat)
            {
                case DateFormatStyle.US:
                    re = "^(\\d{1,2}):(\\d{2})\\s([AP]M)$";// "^([0-9]{2}):([0-9]{2}):([0-9]{2})\\s(AM|PM)$"; 
                    break;
                case DateFormatStyle.SQL:
                default:
                    re = "^(\\d{1,2}):(\\d{2})$"; break;
            }

            Match m = Regex.Match(time, re);

            if (!m.Success || m.Groups.Count < 3)
            {
                throw new Exception("Invalid date format: " + time);
            }
            string hour = "00";
            string minute = "00";
            string second = "00";
            string mode = "";

            switch (dateFormat)
            {
                case DateFormatStyle.US:
                    hour = m.Groups[1].Value; minute = m.Groups[2].Value; mode = m.Groups[3].Value;
                    if (mode.ToUpper() == "PM")
                    {
                        hour = (ToInt(hour, 0) + 12).ToString();
                    }

                    break;
                case DateFormatStyle.SQL:
                default:
                    hour = m.Groups[1].Value; minute = m.Groups[2].Value; break;
            }
            time = hour + ":" + minute + ":" + second;

            return time;
        }


        public static int ToInt(string s, int defaultValue)
        {
            int.TryParse(s, out defaultValue);
            return defaultValue;
        }
        #endregion


    }
}
