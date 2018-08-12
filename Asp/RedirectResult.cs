using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Nistec.Web.Asp
{
    // Summary:
    //     Controls the processing of application actions by redirecting to a specified
    //     URI.
    public class RedirectResult : ActionResult
    {

        bool _Permanent;

        string _Url;

        // Summary:
        //     Initializes a new instance of the System.Web.Mvc.RedirectResult class.
        //
        // Parameters:
        //   url:
        //     The target URL.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     The url parameter is null.
        public RedirectResult(string url)
        {
            _Url = url;
            _Permanent = true;
        }
        //
        // Summary:
        //     Initializes a new instance of the System.Web.Mvc.RedirectResult class using
        //     the specified URL and permanent-redirection flag.
        //
        // Parameters:
        //   url:
        //     The URL.
        //
        //   permanent:
        //     A value that indicates whether the redirection should be permanent.
        public RedirectResult(string url, bool permanent)
        {
            _Url = url;
            _Permanent = permanent;
        }

        // Summary:
        //     Gets a value that indicates whether the redirection should be permanent.
        //
        // Returns:
        //     true if the redirection should be permanent; otherwise, false.
        public bool Permanent { get { return _Permanent; } }
        //
        // Summary:
        //     Gets or sets the target URL.
        //
        // Returns:
        //     The target URL.
        public string Url { get{ return _Url; } }

        // Summary:
        //     Enables processing of the result of an action method by a custom type that
        //     inherits from the System.Web.Mvc.ActionResult class.
        //
        // Parameters:
        //   context:
        //     The context within which the result is executed.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     The context parameter is null.
        public override void ExecuteResult(bool endResponse=false)
        {
            if(Permanent)
                HttpContext.Current.Response.RedirectPermanent(Url, endResponse);
            else
                HttpContext.Current.Response.Redirect(Url, endResponse);
        }
    }
}
