using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Dapper;
using CleverApi.Models;
namespace CleverApi.Services
{
    public class NomineesAddressService : BaseService
    {
        public NomineesAddressService() : base() { }
        public NomineesAddressService(IDbConnection db) : base(db) { }
       

        public object GetAddress(IDbTransaction transaction = null)
        {
            string query = "select top 1 Address,ProvinceID,DistrictID,WardID from S100";
            return this._connection.Query<object>(query, null, transaction).FirstOrDefault();
        }


        public bool Update(S101 model, IDbTransaction transaction = null)
        {
            string query = "UPDATE [dbo].[S100] SET [Address]=@Address,[ProvinceID]=@ProvinceID,[DistrictID]=@DistrictID,[WardID]=@WardID";
            return this._connection.Execute(query, model, transaction) > 0;
        }

    }
}