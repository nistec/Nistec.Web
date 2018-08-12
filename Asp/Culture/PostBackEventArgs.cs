using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;

namespace Nistec.Web.Asp
{

   
    public delegate void PostBackHandler(object sender, PostBackEventArgs e);

    public class PostBackEventArgs : EventArgs
    {
        private string _a_0;

        public PostBackEventArgs(string postBackArgs)
        {
            this._a_0 = postBackArgs;
        }

        public string PostBackArgs
        {
            get
            {
                return this._a_0;
            }
        }
    }
}
