using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using CleverApi.Models;
using Dapper;
namespace CleverApi.Services
{
    public class LocationService : BaseService
    {
        public LocationService() : base() { }
        public LocationService(IDbConnection db) : base(db) { }

        public List<object> GetListProvince(IDbTransaction transaction = null)
        {
            string query = "select ID as value, ProvinceName as text from D300 order by ProvinceName";
            return this._connection.Query<object>(query, null, transaction).ToList();
        }

        public List<object> GetListDistrict(IDbTransaction transaction = null)
        {
            string query = "select ID as value,DistrictName as text, ProvinceID from D301 order by DistrictName";
            return this._connection.Query<object>(query, null, transaction).ToList();
        }

        public List<object> GetListWard(IDbTransaction transaction = null)
        {
            string query = "select ID as value, WardName as text, DistrictID from D302 order by WardName";
            return this._connection.Query<object>(query, null, transaction).ToList();
        }
    }
}