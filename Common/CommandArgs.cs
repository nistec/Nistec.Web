using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using Nistec;

namespace Nistec.Web
{
    public interface IArgs
    {
        string[] ToArray();
 
        int Length{get;}
    }

    public class CommandArgs : Dictionary<string, string>
    {
        public const string ArgName = "carg";

        public CommandArgs()
            : base(StringComparer.OrdinalIgnoreCase)
        {

        }

        #region properties

        public string Get(string key)
        {
            if (this.ContainsKey(key))
                return this[key];
            return "";

        }

        public string GetAlter(params string[] keys)
        {
            foreach (string key in keys)
            {
                if (this.ContainsKey(key))
                    return this[key];
            }

            return "";
        }

        public T Get<T>(string key, T defaultValue)
        {
            if (this.ContainsKey(key))
                return GenericTypes.Convert<T>(this[key], defaultValue);
            return defaultValue;

        }

        public T Get<T>(string key)
        {
            if (this.ContainsKey(key))
                return GenericTypes.Convert<T>(this[key]);
            return GenericTypes.Default<T>();

        }

        public string Get(string key, bool decypt)
        {
            string value = Get(key);
            if (string.IsNullOrEmpty(value))
                return "";
            if (decypt)
            {
                return Nistec.Runtime.RequestQuery.DecryptEx32(value);
            }
            return value;
        }
        

        //public new string this[string key]
        //{
        //    get
        //    {
        //        if (this.ContainsKey(key))
        //            return this[key];
        //        return "";
        //    }
        //    set
        //    {
        //        this[key] = value;
        //    }
        //}

        #endregion


        public static CommandArgs Empty
        {
            get { return new CommandArgs(); }
        }

        public static string EncodeArgs(string qs)
        {
            return Nistec.Runtime.BaseConverter.ToBase32(qs);
        }
        public static CommandArgs DecodeArgs(string cargs)
        {
            string qs= Nistec.Runtime.BaseConverter.FromBase32(cargs);
            return CommandArgs.ParseCommand(qs);
        }
        public static CommandArgs DecodeArgs(string cargs, bool removePrefix)
        {
            string qs = Nistec.Runtime.BaseConverter.FromBase32(cargs);
            if (removePrefix)
            {
                qs = qs.Replace("?", "").Replace(ArgName + "=", "");
            }
            return CommandArgs.ParseCommand(qs);
        }
        

        //public static string EncodeRequestArgs(string qs)
        //{
        //    return EncodeArgs("?" + ArgName + "=" + qs);
        //}
        //public static string EncodeRequestArgs(CommandArgs arg)
        //{
        //    return EncodeArgs("?" + ArgsToQueryString(arg));
        //}
        public static CommandArgs DecodeRequestArgs(System.Web.HttpRequest request, bool removePrefix)
        {
            string arg = request.QueryString[ArgName];
            if (string.IsNullOrEmpty(arg))
            {
                return CommandArgs.ParseCommand(arg);
            }
             return DecodeArgs(arg, removePrefix);
        }
        public static CommandArgs DecodeRequestArgs(System.Web.HttpRequest request)
        {
            string arg = request.QueryString[ArgName];
            if (string.IsNullOrEmpty(arg))
            {
                return CommandArgs.ParseCommand(arg);
            }
            return DecodeArgs(arg);
        }

        public static string[] ParseArgs(string args, char splitter)
        {

            if (string.IsNullOrEmpty(args))
            {
                return null;
            }
            string[] strArray = args.Split(new char[] { splitter });


            return strArray;
        }

        public static string[] ParseArgs(string args)
        {

            if (string.IsNullOrEmpty(args))
            {
                return null;
            }
            string[] strArray = args.Split(new char[] { '|', ';', ',' });


            return strArray;
        }

        public static string ToArgs(IArgs args)
        {
            if (args == null)
            {
                return null;
            }
            StringBuilder sb = new StringBuilder();

            foreach (string s in args.ToArray())
            {
                sb.Append(s + "|");
            }

            sb = sb.Remove(sb.Length - 1, 1);

            return sb.ToString();
        }
       

        public static CommandArgs ParseCommand(string cmd)
        {
            CommandArgs dictionary = new CommandArgs();

            if (cmd == null)
                cmd = string.Empty;
            //cmd = cmd.ToLowerInvariant();

            string str = cmd;
 
            if (string.IsNullOrEmpty(cmd))
            {
                return dictionary;
            }

            //var dictionary = new Dictionary<string, string>();
            foreach (string str3 in str.Split(new char[] { ';','|','&' }))
            {
                if (!string.IsNullOrEmpty(str3))
                {
                    string[] strArray = str3.Split(new char[] { '=' });
                    if (strArray.Length == 2)
                    {
                        dictionary[strArray[0]] = strArray[1];
                    }
                    else
                    {
                        dictionary[str3] = null;
                    }
                }
            }

            return dictionary;
        }


        public static CommandArgs ParseRequest(System.Web.HttpRequest request)
        {

            if (request.QueryString == null)
            {
                throw new ArgumentException("invalid request");
            }
            return ParseRequest( request.RawUrl);
        }

        public static CommandArgs ParseRequest(string url)
        {
   
            if (url == null)
                url = string.Empty;
            //url = url.ToLowerInvariant();

            string qs = string.Empty;
             
            if (url.Contains("?"))
            {
                qs = url.Substring(url.IndexOf("?") + 1);
                url = url.Substring(0, url.IndexOf("?"));
            }

            return ParseQueryString(qs);
        }

        
        public static string QueryStringFormat(string pattern, params object[] args)
        {
            return string.Format(pattern, args);
        }

        public static string ArgsToQueryString(CommandArgs args)
        {
            StringBuilder sb = new StringBuilder();

            foreach (KeyValuePair<string, string> entry in args)
            {
                sb.AppendFormat("&{0}={1}", entry.Key, entry.Value);
            }

            sb = sb.Remove(0, 1);

            return sb.ToString();
        }

        public static CommandArgs ParseQueryString(params string[] qs)
        {
            StringBuilder sb = new StringBuilder();
            int len=qs.Length;
            for (int i = 0; i < qs.Length; i++)
            {
                sb.Append(qs[i]);
                if (i < len - 1 && !qs[i].EndsWith("&") && !qs[i + 1].StartsWith("&"))
                {
                    sb.Append("&");
                }
            }

           return ParseQueryString(sb.ToString());
        }

        public static string CLeanQueryString(string qs)
        {
            return qs.Replace("&amp;","&");
        }

        public static CommandArgs ParseQueryString(string qs)
        {
            CommandArgs dictionary = new CommandArgs();

            if (qs == null)
                qs = string.Empty;
            //url = url.ToLowerInvariant();

            string str = CLeanQueryString(qs);

            if (string.IsNullOrEmpty(str))
            {
                return dictionary;
            }
            if (!str.Contains('='))
            {
                return dictionary;
            }
            //var dictionary = new Dictionary<string, string>();
            foreach (string arg in str.Split(new char[] { '&' }))
            {
                if (!string.IsNullOrEmpty(arg))
                {
                    string[] strArray = arg.Split(new char[] { '=' });
                    if (strArray.Length == 2)
                    {
                        string key = Regx.RegexReplace("amp;", strArray[0],"");
                        dictionary[key] = strArray[1];
                    }
                    else
                    {
                        dictionary[arg] = null;
                    }
                }
            }

            return dictionary;
        }

        public static CommandArgs ParseQueryString(NameValueCollection qs)
        {
            CommandArgs dictionary = new CommandArgs();
            if (qs != null)
            {
                for (int i = 0; i < qs.Count; i++)
                {
                    dictionary[qs.Keys[i]] = qs[i];
                }
            }

            return dictionary;
        }

        public static void ParseArgs<T1, T2>(string args,char splitter, out T1 arg1, out T2 arg2)
        {
            arg1 = default(T1);
            arg2 = default(T2);
            string[] list = args.Split(splitter);

            if (list == null || list.Length == 0)
            {
                return;
            }

            arg1 = GenericTypes.Convert<T1>(list[0]);
            if (list.Length > 1)
                arg2 = GenericTypes.Convert<T2>(list[1]);
        }
        public static void ParseArgs<T1, T2, T3>(string args, char splitter, out T1 arg1, out T2 arg2, out T3 arg3)
        {
            arg1 = default(T1);
            arg2 = default(T2);
            arg3 = default(T3);

            string[] list = args.Split(splitter);

            if (list == null || list.Length == 0)
            {
                return;
            }

            arg1 = GenericTypes.Convert<T1>(list[0]);
            if (list.Length > 1)
                arg2 = GenericTypes.Convert<T2>(list[1]);
            if (list.Length > 2)
                arg3 = GenericTypes.Convert<T3>(list[2]);
        }

        public static void ParseArgs<T1, T2, T3, T4>(string args, char splitter, out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4)
        {
            arg1 = default(T1);
            arg2 = default(T2);
            arg3 = default(T3);
            arg4 = default(T4);

            string[] list = args.Split(splitter);

            if (list == null || list.Length == 0)
            {
                return;
            }

            arg1 = GenericTypes.Convert<T1>(list[0]);
            if (list.Length > 1)
                arg2 = GenericTypes.Convert<T2>(list[1]);
            if (list.Length > 2)
                arg3 = GenericTypes.Convert<T3>(list[2]);
            if (list.Length > 3)
                arg4 = GenericTypes.Convert<T4>(list[3]);
        }
    }
}
