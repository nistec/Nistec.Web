using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace Nistec.Web.Asp
{
    public static class PageExtension
    {
        public static ActionResult RedirectLocal(this Page controller, string redirectUrl, Func<ActionResult> invalidUrlBehavior)
        {
            if (!string.IsNullOrWhiteSpace(redirectUrl) && controller.Request.IsLocalUrl(redirectUrl))
            {
                return new RedirectResult(redirectUrl);
            }
            return invalidUrlBehavior != null ? invalidUrlBehavior() : null;
        }

        public static ActionResult RedirectLocal(this Page controller, string redirectUrl)
        {
            return RedirectLocal(controller, redirectUrl, (string)null);
        }

        public static ActionResult RedirectLocal(this Page controller, string redirectUrl, string defaultUrl)
        {
            if (controller.Request.IsLocalUrl(redirectUrl))
            {
                return new RedirectResult(redirectUrl);
            }

            return new RedirectResult(defaultUrl ?? "~/");
        }
    }
}
