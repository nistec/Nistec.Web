using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Nistec.Web.Security
{
    public interface IHttpContextAccess
    {
        HttpContextBase Current();
    }

    public class HttpContextAccess : IHttpContextAccess
    {
        public HttpContextBase Current()
        {
            var httpContext = GetStaticProperty();
            if (httpContext == null)
                return null;
            return new HttpContextWrapper(httpContext);
        }

        private HttpContext GetStaticProperty()
        {
            var httpContext = HttpContext.Current;
            if (httpContext == null)
            {
                return null;
            }

            try
            {
                if (httpContext.Request == null)
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
            return httpContext;
        }
    }
}
