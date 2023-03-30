using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CleverApi.Services
{
    public class BaseService
    {
        protected IDbConnection _connection;
        public BaseService()
        {
            string constr = ConfigurationManager.ConnectionStrings["CleverlyConnection"].ToString();
            this._connection = new SqlConnection(constr);
        }

        public BaseService(IDbConnection _connection)
        {
            if (_connection == null)
            {
                string constr = ConfigurationManager.ConnectionStrings["CleverlyConnection"].ToString();
                this._connection = new SqlConnection(constr);
            }
            else
            {
                this._connection = _connection;
            }
        }
        public static IDbConnection Connect()
        {
            string constr = ConfigurationManager.ConnectionStrings["CleverlyConnection"].ToString();
            return new SqlConnection(constr);
        }

    }
}