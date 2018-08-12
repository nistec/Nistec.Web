using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nistec.Web.Security
{

    public delegate LocalizedString Localizer(string text, params object[] args);

    public static class LocalizerExtensions
    {
        public static LocalizedString Plural(this Localizer T, string textSingular, string textPlural, int count, params object[] args)
        {
            return T(count == 1 ? textSingular : textPlural, new object[] { count }.Concat(args).ToArray());
        }

        public static LocalizedString Plural(this Localizer T, string textNone, string textSingular, string textPlural, int count, params object[] args)
        {
            switch (count)
            {
                case 0:
                    return T(textNone, new object[] { count }.Concat(args).ToArray());
                case 1:
                    return T(textSingular, new object[] { count }.Concat(args).ToArray());
                default:
                    return T(textPlural, new object[] { count }.Concat(args).ToArray());
            }
        }
    }

    public static class NullLocalizer
    {

        static NullLocalizer()
        {
            _instance = (format, args) => new LocalizedString((args == null || args.Length == 0) ? format : string.Format(format, args));
        }

        static readonly Localizer _instance;

        public static Localizer Instance { get { return _instance; } }
    }

    public class LocalizedString : MarshalByRefObject, IHtmlString
    {
        private readonly string _localized;
        private readonly string _scope;
        private readonly string _textHint;
        private readonly object[] _args;

        public LocalizedString(string languageNeutral)
        {
            _localized = languageNeutral;
            _textHint = languageNeutral;
        }

        public LocalizedString(string localized, string scope, string textHint, object[] args)
        {
            _localized = localized;
            _scope = scope;
            _textHint = textHint;
            _args = args;
        }

        public static LocalizedString TextOrDefault(string text, LocalizedString defaultValue)
        {
            if (string.IsNullOrEmpty(text))
                return defaultValue;
            return new LocalizedString(text);
        }

        public string Scope
        {
            get { return _scope; }
        }

        public string TextHint
        {
            get { return _textHint; }
        }

        public object[] Args
        {
            get { return _args; }
        }

        public string Text
        {
            get { return _localized; }
        }

        public override string ToString()
        {
            return _localized;
        }

        string IHtmlString.ToHtmlString()
        {
            return _localized;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            if (_localized != null)
                hashCode ^= _localized.GetHashCode();
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType())
                return false;

            var that = (LocalizedString)obj;
            return string.Equals(_localized, that._localized);
        }

    }
}
