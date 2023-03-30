using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Dapper;
using CleverApi.Models;
using CleverApi.Providers;
namespace CleverApi.Services
{
    public class CompanyService : BaseService
    {
        public CompanyService() : base() { }
        public CompanyService(IDbConnection db) : base(db) { }

        public bool UpdateCompanyoFolderCreatedStatus(string id, IDbTransaction transaction = null)
        {
            string query = "UPDATE T100 SET IsFolder = 1 WHERE ID = @id";
            return this._connection.Execute(query, new { id }, transaction) > 0;
        }

        public List<object> GetListCompanyToCreateFolder(IDbTransaction transaction = null) {
            string query = "SELECT CompanyName as text, ID as value FROM T100 WHERE Status = 1 AND IsFolder = 0 ORDER BY CompanyName";
            return this._connection.Query<object>(query, null, transaction).ToList();
        }

        public T100 GetCompany(string id,IDbTransaction transaction = null) {
            string query = "select * from T100 where ID=@id";
            return this._connection.Query<T100>(query, new { id }, transaction).FirstOrDefault();
        }
    }
}