using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Dapper;
using CleverApi.Models;

namespace CleverApi.Services
{
    public class NomineesDirectorNameService : BaseService
    {
        public NomineesDirectorNameService() : base() { }
        public NomineesDirectorNameService(IDbConnection db) : base(db) { }

        public object GetList(int page, int rowPerPage, IDbTransaction transaction = null)
        {
            int skip = (page - 1) * rowPerPage;
            string querySelect = "select * ";
            string queryCount = "select count(*) as Total ";
            string query = " from S102 where Status=@status";

            long totalRow = this._connection.Query<long>(queryCount + query, new { status = S102.EnumStatus.ENABLED }, transaction).FirstOrDefault();
            int totalPage = (int)Math.Ceiling((decimal)totalRow / rowPerPage);
            query += " order by CreatedDate desc offset " + skip + "rows fetch next " + rowPerPage + " rows only";
            List<object> listData = this._connection.Query<object>(querySelect + query, new { status = S102.EnumStatus.ENABLED }, transaction).ToList();
            return new { totalPage, listData };
        }

        public S102 GetByDirectorName(string directorName, IDbTransaction transaction = null) {
            string query = "select top 1 * from S102 where DirectorName=@directorName";
            return this._connection.Query<S102>(query, new { directorName }, transaction).FirstOrDefault();
        }

        public S102 GetById(string id, IDbTransaction transaction = null) {
            string query = "select top 1 * from S102 where ID=@id";
            return this._connection.Query<S102>(query, new { id }, transaction).FirstOrDefault();
        }

        public bool Insert(S102 model, IDbTransaction transaction = null) {
            string query = "INSERT INTO [dbo].[S102] ([ID],[DirectorName],[Status],[CreatedBy],[CreatedDate]) VALUES (@ID,@DirectorName,@Status,@CreatedBy,GETDATE())";
            return this._connection.Execute(query,model,transaction) > 0;
        }

        public bool Update(S102 model, IDbTransaction transaction = null) {
            string query = "UPDATE [dbo].[S102] SET [DirectorName]=@DirectorName,[Status]=@Status,[ModifiedBy]=@ModifiedBy,[ModifiedDate]=getdate() WHERE [ID]=@ID";
            return this._connection.Execute(query,model,transaction) > 0;

        }


        
    }
}