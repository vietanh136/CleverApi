using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using CleverApi.Models;
using Dapper;


namespace CleverApi.Services
{
    public class FrequentlyAskedQuestionsService : BaseService
    {
        public FrequentlyAskedQuestionsService() : base() { }
        public FrequentlyAskedQuestionsService(IDbConnection db) : base(db) { }

        public object GetListQuestion(int page,int rowPerPage, IDbTransaction transaction = null)
        {
            string querySelect = "select * ";
            string queryCount = "select count(*) as Total ";
            string query = " FROM C600 WHERE Status = 1 ";

            int totalRow = this._connection.Query<int>(queryCount + query, null, transaction).FirstOrDefault();
            int totalPage = (int)Math.Ceiling((decimal)totalRow / rowPerPage);

            int skip = (page - 1) * rowPerPage;
            query += " ORDER BY CreatedDate offset " + skip + " rows fetch next " + rowPerPage + " rows only";

            List<object> listData = this._connection.Query<object>(querySelect + query, null, transaction).ToList();
            return new { totalPage, listData };
        }
        public C600 GetQuestionById(string id, IDbTransaction transaction = null)
        {
            string query = "select top 1 * from C600 where ID=@id";
            return this._connection.Query<C600>(query, new { id }, transaction).FirstOrDefault();
        }

        public bool DeleteQuestion(string id, IDbTransaction transaction = null)
        {
            string query = "update C600 set Status=0 where ID=@id";
            return this._connection.Execute(query, new { id }, transaction) > 0;
        }

        public bool InsertQuestion(C600 model, IDbTransaction transaction = null)
        {
            string query = "INSERT INTO [dbo].[C600] ([ID],[Question],[Answer],[FormFill],[ProcessTime],[Payment],[Icon],[Status],[CreatedBy],[CreatedDate]) VALUES (@ID,@Question,@Answer,@FormFill,@ProcessTime,@Payment,@Icon,@Status,@CreatedBy,GETDATE())";
            return this._connection.Execute(query, model, transaction) > 0;
        }

        public bool UpdateQuestion(C600 model, IDbTransaction transaction = null)
        {
            string query = "UPDATE [dbo].[C600] SET [Question]=@Question,[Answer]=@Answer,[FormFill]=@FormFill,[ProcessTime]=@ProcessTime,[Payment]=@Payment,[Icon]=@Icon,[Status]=@Status,[ModifiedBy]=@ModifiedBy,[ModifiedDate]=getdate() WHERE [ID]=@ID";
            return this._connection.Execute(query, model, transaction) > 0;
        }

        public bool CheckQuestionExist(C600 model, IDbTransaction transaction = null) {
            string query = "select count(*) as Total from C600 where Question = @Question";
            if (!string.IsNullOrEmpty(model.ID)) {
                query += " and ID <> @ID";
            }
            int count = this._connection.Query<int>(query, model, transaction).FirstOrDefault();
            return count > 0;
        }

        public List<C601> GetListSteps(string faqId, IDbTransaction transaction = null)
        {
            string query = "select * from C601 where FAQID=@faqId";
            return this._connection.Query<C601>(query, new { faqId }, transaction).ToList();
        }
        public C601 GetStepById(string id, IDbTransaction transaction = null)
        {
            string query = "select top 1 * from C601 where ID=@id";
            return this._connection.Query<C601>(query, new { id }, transaction).FirstOrDefault();
        }
        public bool InsertStep(C601 model, IDbTransaction transaction = null)
        {
            string qeury = "INSERT INTO [dbo].[C601] ([ID],[FAQID],[Description],[Note],[SortOrder]) VALUES (@ID,@FAQID,@Description,@Note,@SortOrder)";
            return this._connection.Execute(qeury, model, transaction) > 0;
        }
        public bool UpdateStep(C601 model, IDbTransaction transaction = null)
        {
            string qeury = "UPDATE [dbo].[C601] SET [Description]=@Description,[Note]=@Note,[SortOrder]=@SortOrder WHERE [ID]=@ID";
            return this._connection.Execute(qeury, model, transaction) > 0;
        }
        public bool DeleteStep(string id, IDbTransaction transaction = null)
        {
            string query = "delete from C601 where ID=@id";
            return this._connection.Execute(query, new { id }, transaction) > 0;
        }

        public List<C602> GetListWhatNeed(string faqId, IDbTransaction transaction = null) {
            string query = "select * from C602 where FAQID=@faqId";
            return this._connection.Query<C602>(query, new { faqId }, transaction).ToList();
        }
        public C602 GetWhatNeedById(string id, IDbTransaction transaction = null)
        {
            string query = "select top 1 * from C602 where ID=@id";
            return this._connection.Query<C602>(query, new { id }, transaction).FirstOrDefault();
        }
        public bool InsertWhatNeed(C602 model, IDbTransaction transaction = null)
        {
            string qeury = "INSERT INTO [dbo].[C602] ([ID],[FAQID],[Description]) VALUES (@ID,@FAQID,@Description)";
            return this._connection.Execute(qeury, model, transaction) > 0;
        }
        public bool UpdateWhatNeed(C602 model, IDbTransaction transaction = null)
        {
            string qeury = "UPDATE [dbo].[C602] SET [Description]=@Description WHERE [ID]=@ID";
            return this._connection.Execute(qeury, model, transaction) > 0;
        }
        public bool DeleteWhatNeed(string id, IDbTransaction transaction = null)
        {
            string query = "delete from C602 where ID=@id";
            return this._connection.Execute(query, new { id }, transaction) > 0;
        }
    }
}