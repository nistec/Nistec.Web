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
        HttpContext _httpContex;

        public HttpContextAccess() { }
        public HttpContextAccess(HttpContext httpContex) {
            _httpContex = httpContex;
        }


        public HttpContextBase Current()
        {

            var httpContext = (_httpContex == null) ? GetStaticProperty() : _httpContex;
            if (httpContext == null)
                return null;
            return new HttpContextWrapper(httpContext);
        }

        private HttpContext GetStaticProperty()
        {
            var httpContext =(_httpContex==null) ? HttpContext.Current: _httpContex;
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
