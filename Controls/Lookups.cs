using Nistec.Data;
using Nistec.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nistec.Web.Controls
{
    //public class Lookups
    //{
    //    public static V Get<Dbc, V>(string field, string mappingName, V defaultValue, params object[] keyvalueParameters)
    //        where Dbc : IDbContext
    //    {
    //        var sql = SqlFormatter.CreateCommandText(field, mappingName, keyvalueParameters);
    //        using (IDbContext Db = DbContext.Create<Dbc>())
    //        {
    //            return Db.QueryScalar<V>(sql, defaultValue, keyvalueParameters);
    //        }
    //    }
    //    public static string Get<Dbc>(string field, string mappingName, string defaultValue, params object[] keyvalueParameters)
    //        where Dbc : IDbContext
    //    {
    //        var sql = SqlFormatter.CreateCommandText(field, mappingName, keyvalueParameters);
    //        using (IDbContext Db = DbContext.Create<Dbc>())
    //        {
    //            return Db.QueryScalar<string>(sql, defaultValue, keyvalueParameters);
    //        }
    //    }
    //}
}
