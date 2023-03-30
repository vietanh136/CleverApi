using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Dapper;
using CleverApi.Models;
namespace CleverApi.Services
{
    public class ShareholdersDocumentService : BaseService
    {
        public ShareholdersDocumentService() : base() { }
        public ShareholdersDocumentService(IDbConnection db) : base(db) { }
        public object GetList(int page,int rowPerPage, IDbTransaction transaction = null) {
            string queryCount = "select count(*) as Total ";
            string querySelect = "select S103.*,D100.Description ";
            string query = " from S103 left join D100 on S103.Type=D100.ID where Status=@status ";

            long totalRow = this._connection.Query<long>(queryCount + query, new { status = S103.EnumStatus.ENABLED }, transaction).FirstOrDefault();
            int totalPage = (int)Math.Ceiling((decimal)totalRow / rowPerPage);
            int skip = (page - 1) * rowPerPage;
            query += " order by CreatedDate desc offset " + skip + " rows fetch next " + rowPerPage + " rows only";

            List<object> listData = this._connection.Query<object>(querySelect + query, new { status = S103.EnumStatus.ENABLED }, transaction).ToList();
            return new {
                totalPage,
                listData
            };
        }
        public S103 GetById(string id, IDbTransaction transaction = null) {
            string query = "select top 1 * from [dbo].[S103] where [ID]=@id";
            return this._connection.Query<S103>(query, new { id }, transaction).FirstOrDefault();
        }

        public bool CheckSameNameExist(S103 model, IDbTransaction transaction = null) {
            string query = "select count(*) as Total from S103 where DocumentName=@DocumentName";
            if (!string.IsNullOrEmpty(model.ID)) {
                query += " and ID <> @ID";
            }

            return this._connection.Query<int>(query, model, transaction).FirstOrDefault() > 0;
        }
        public bool Insert(S103 model, IDbTransaction transaction = null) {
            string query = "INSERT INTO [dbo].[S103] ([ID],[Type],[DocumentName],[Status],[CreatedBy],[CreatedDate]) VALUES (@ID,@Type,@DocumentName,@Status,@CreatedBy,getdate())";
            return this._connection.Execute(query, model, transaction) > 0;
        }

        public bool Update(S103 model, IDbTransaction transaction = null) {
            string query = "UPDATE [dbo].[S103] SET [Type]=@Type,[DocumentName]=@DocumentName,[Status]=@Status,[ModifiedBy]=@ModifiedBy,[ModifiedDate]=getdate() WHERE [ID]=@ID";
            return this._connection.Execute(query, model, transaction) > 0;
        }

        public List<object> GetListShareholderType(IDbTransaction transaction = null) {
            string query = "select ID as value, Description as text from D100 where ID in (2,3)";
            return this._connection.Query<object>(query, null, transaction).ToList();
        }
    }
}