using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nistec.Web.Asp
{
    // Summary:
    //     Encapsulates the result of an action method and is used to perform a framework-level
    //     operation on behalf of the action method.
    public abstract class ActionResult
    {
        // Summary:
        //     Initializes a new instance of the System.Web.Mvc.ActionResult class.
        protected ActionResult()
        {

        }

        //// Summary:
        ////     Enables processing of the result of an action method by a custom type that
        ////     inherits from the System.Web.Mvc.ActionResult class.
        ////
        //// Parameters:
        ////   context:
        ////     The context in which the result is executed. The context information includes
        ////     the controller, HTTP content, request context, and route data.
        public abstract void ExecuteResult(bool endResponse = false);
    }
}
