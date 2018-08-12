using System;
using System.Collections.Generic;
using System.Text;
using Nistec.Data.Factory;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace Nistec.Web.Asp
{

    public static class SecurityUtil
    {

        public static bool IsAlphaNumeric(params string[] expression)
        {
            Regex regex = new Regex("^[a-zA-Z0-9]+$");
            foreach (string str in expression)
            {
                if (!regex.Match(str).Success)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsValidString(string s)
        {
            if (s.IndexOfAny(new char[] { 
                '[', ']', '(', ')', '{', '}', '|', '<', '>', '!', '=', ';', ':', '&', '?', '*', 
                '%', '&', '+', ' ', '\''
             }) > 0)
            {
                return false;
            }
            if (s.ToLower().Contains("delete"))
            {
                return false;
            }
            if (s.ToLower().Contains("insert"))
            {
                return false;
            }
            if (s.ToLower().Contains("select"))
            {
                return false;
            }
            if (s.ToLower().Contains("from"))
            {
                return false;
            }
            if (s.ToLower().Contains("script"))
            {
                return false;
            }
            return true;
        }

        public static bool RegexMatch(string pattern, string expression)
        {
            Regex regex = new Regex(pattern);
            return regex.Match(expression).Success;
        }

        public static bool RegexMatch(string pattern, params string[] expression)
        {
            Regex regex = new Regex(pattern);
            foreach (string str in expression)
            {
                if (!regex.Match(str).Success)
                {
                    return false;
                }
            }
            return true;
        }
    }

  
}
