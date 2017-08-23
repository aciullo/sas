using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sas_Futura.Controllers.Services
{
    public class DataAccess
    {
        public string leerConeccion()
        {
            //System.Configuration.Configuration rootWebConfig =
            //    System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("/futura");
           // System.Configuration.ConnectionStringSettings connString;
            //if (0 < rootWebConfig.ConnectionStrings.ConnectionStrings.Count)
            //{
            //    connString = rootWebConfig.ConnectionStrings.ConnectionStrings["FuturaConnection"];
            //    if (null != connString)
            //        return connString.ConnectionString;
            //    else
            //        return ("No Northwind connection string");
            //}
            //return "";

            string connString;
            connString = Startup.FuturaConn;
            return connString;
        }
    }
}