using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using CleverApi.Models;
using Dapper;

namespace CleverApi.Services
{
    public class FieldsService : BaseService
    {
        public FieldsService() : base() { }
        public FieldsService(IDbConnection db) : base(db) { }

        public object GetList(int page,int rowPerPage, IDbTransaction transaction = null)
        {
            string queryCount = "select count(*) as Total";
            string querySelect = "select *";
            string query = " FROM C100 WHERE Status = 1 ";

            int totalRow = this._connection.Query<int>(queryCount + query, null, transaction).FirstOrDefault();
            int totalPage = (int)Math.Ceiling((decimal)totalRow / rowPerPage);

            int skip = (page - 1) * rowPerPage;
            query += " ORDER BY ID offset " + skip + " rows fetch next "+rowPerPage + " row only";

            List<object> listData = this._connection.Query<object>(querySelect + query, null, transaction).ToList();
            return new { totalPage, listData };
        }

        public C100 GetById(string id, IDbTransaction transaction = null)
        {
            string query = "SELECT top 1 * FROM C100 WHERE  ID=@id";
            return this._connection.Query<C100>(query, new { id }, transaction).FirstOrDefault();
        }

        public bool Update(C100 model, IDbTransaction transaction = null)
        {
            string query = "UPDATE C100 SET FieldNameEN = @FieldNameEN, IsPause = @IsPause WHERE ID = @ID";
            return this._connection.Execute(query, model, transaction) > 0;
        }
    }
}