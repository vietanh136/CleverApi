using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using CleverApi.Models;
using Dapper;

namespace CleverApi.Services
{
    public class BusinessAccountQuestionsService : BaseService
    {
        public BusinessAccountQuestionsService() : base() { }
        public BusinessAccountQuestionsService(IDbConnection db) : base(db) { }

        public object GetListQuestion(int page,int rowPerPage, IDbTransaction transaction = null)
        {
            string querySelect = "select * ";
            string queryCount = "select count(*) as Total ";
            string query = " from C500 where Status=@status";

            int totalRow = this._connection.Query<int>(queryCount + query, new { status = C500.EnumStatus.ENABLED }, transaction).FirstOrDefault();
            int totalPage = (int)Math.Ceiling((decimal)totalRow / rowPerPage);
            int skip = (page - 1) * rowPerPage;
            query += " order by Question offset " + skip + " rows fetch next " + rowPerPage + " rows only";
            List<object> listData = this._connection.Query<object>(querySelect + query, new { status = C500.EnumStatus.ENABLED }, transaction).ToList();
            return new { totalPage, listData };
        }

        public bool CheckBusinessAccountQuestionsExist(C500 model, IDbTransaction transaction = null) {
            string query = "select count(*) as Total from C500 where Question=@Question";
            if (!string.IsNullOrEmpty(model.ID)) {
                query += " and ID <> @ID";
            }
            int count = this._connection.Query<int>(query, model, transaction).FirstOrDefault();
            return count > 0;
        }

        public C500 GetQuestionById(string id, IDbTransaction transaction = null) {
            string query = "select top 1 * from C500 where ID=@id";
            return this._connection.Query<C500>(query, new { id }, transaction).FirstOrDefault();
        }

        public bool DeleteQuestion(string id, IDbTransaction transaction = null)
        {
            string query = "update C500 set Status=@status where ID=@id ";
            return this._connection.Execute(query, new { id, status = C500.EnumStatus.DISABLED }, transaction) > 0;
        }

        public bool InsertQuestion(C500 model, IDbTransaction transaction = null)
        {
            string query = "INSERT INTO [dbo].[C500] ([ID],[Question],[Type],[Status],[CreatedBy],[CreatedDate]) VALUES (@ID,@Question,@Type,@Status,@CreatedBy,GETDATE())";
            return this._connection.Execute(query, model, transaction) > 0;
        }
        public bool UpdateQuestion(C500 model, IDbTransaction transaction = null)
        {
            string query = "UPDATE [dbo].[C500] SET [Question]=@Question,[Type]=@Type,[Status]=@Status,[ModifiedBy]=@ModifiedBy,[ModifiedDate]=getdate() WHERE [ID]=@ID";
            return this._connection.Execute(query, model, transaction) > 0;
        }

        public List<C501> GetListAnswer(string questionId, IDbTransaction transaction = null)
        {
            string query = "select * from C501 where QuestionID=@questionId";
            return this._connection.Query<C501>(query, new { questionId }, transaction).ToList();
        }
        public C501 GetAnswer(string id, IDbTransaction transaction = null)
        {
            string query = "select top 1 * from C501 where ID=@id";
            return this._connection.Query<C501>(query, new { id }, transaction).FirstOrDefault();
        }
        public bool InsertAnswer(C501 model, IDbTransaction transaction = null) {
            string query = "INSERT INTO [dbo].[C501] ([ID],[QuestionID],[Answer]) VALUES (@ID,@QuestionID,@Answer)";
            return this._connection.Execute(query,model,transaction) > 0;
        }
        public bool UpdateAnswer(C501 model, IDbTransaction transaction = null)
        {
            string query = "UPDATE [dbo].[C501] SET [Answer]=@Answer WHERE [ID]=@ID";
            return this._connection.Execute(query, model, transaction) > 0;
        }
        public bool DeleteAnswer(string id, IDbTransaction transaction = null)
        {
            string query = "delete from C501 where ID=@id";
            return this._connection.Execute(query, new { id}, transaction) > 0;
        }
    }
}