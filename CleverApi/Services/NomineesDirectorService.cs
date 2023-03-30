using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using CleverApi.Models;
using Dapper;

namespace CleverApi.Services
{
    public class NomineesDirectorService : BaseService
    {
        public NomineesDirectorService() : base() { }
        public NomineesDirectorService(IDbConnection db) : base(db) { }

        public object GetList(int page,int rowPerPage, IDbTransaction transaction = null)
        {
            string querySelect = "select * ";
            string queryCount = "select count(*) as Total ";
            string query = " from C300";
            int totalRow = this._connection.Query<int>(queryCount + query, null, transaction).FirstOrDefault();
            int totalPage = (int)Math.Ceiling((decimal)totalRow/rowPerPage);

            int skip = (page - 1) * rowPerPage;
            query += " order by MonthNum offset " + skip + " rows fetch next " + rowPerPage + " rows only";

            List<object> listData = this._connection.Query<object>(querySelect + query, null, transaction).ToList();
            return new { totalPage, listData };
        }

        public bool CheckSameValueExist(C300 model, IDbTransaction transaction = null) {
            string query = "select count(*) from C300 where MonthNum=@MonthNum";
            if (!string.IsNullOrEmpty(model.ID)) {
                query += " and ID <> @ID";
            }
            return this._connection.Query<int>(query, model, transaction).FirstOrDefault() > 0;
        }
        public C300 GetById(string id, IDbTransaction transaction = null) {
            string query = "select top 1 * from C300 where ID=@id";
            return this._connection.Query<C300>(query, new { id }, transaction).FirstOrDefault();
        }

        public bool Insert(C300 model, IDbTransaction transaction = null) {
            string query = "INSERT INTO [dbo].[C300] ([ID],[MonthNum],[Amount]) VALUES (@ID,@MonthNum,@Amount)";
            return this._connection.Execute(query,model,transaction) > 0;
        }

        public bool Update(C300 model, IDbTransaction transaction = null)
        {
            string query = "update [dbo].[C300] set [Amount] =@Amount where ID=@ID";
            return this._connection.Execute(query, model, transaction) > 0;
        }

        public bool Delete(string id, IDbTransaction transaction = null) {
            string query = "delete from C300 Where ID=@id";
            return this._connection.Execute(query,new { id },transaction) > 0;
        }
    }
}