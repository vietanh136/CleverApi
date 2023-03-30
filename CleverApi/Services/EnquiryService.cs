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
    public class EnquiryService : BaseService
    {
        public EnquiryService() : base() { }
        public EnquiryService(IDbConnection db) : base(db) { }

        public C900 GetEnquiry(string id, IDbTransaction transaction = null) {
            return this._connection.Query<C900>("select top 1 * from C900 where ID=@id", new { id }, transaction).FirstOrDefault();
        }


        public object GetListEnquiryView(int page, int status,int rowPerPage, IDbTransaction transaction = null)
        {
            string querySelect = "select c.ID,c.SentFrom,c.SentDate,c.Rating,c.Title,c.IsCustomer,c.IsAdmin,c.Status,s.UserName  ";
            string queryCount = "select count(*) as Total";
            string query = " from C900 c left join S200 s on c.SentFrom = s.ID where 1=1";

            if (status != -1)
            {
                query += " and c.Status=@status";
            }

            int totalRow = this._connection.Query<int>(queryCount + query, new { status }, transaction).FirstOrDefault();
            int totalPage = (int)Math.Ceiling((decimal)totalRow/rowPerPage);
            int skip = (page - 1) * rowPerPage;
            query += " Order by c.SentDate desc offset " + skip + " rows fetch next " + rowPerPage + " rows only";
            List<object> listData = this._connection.Query<object>(querySelect + query, new { status }, transaction).ToList();
            return new { totalPage, listData };
        }

        public object GetEnquiryView(string id, IDbTransaction transaction = null)
        {
            string query = "select c.ID,c.SentFrom,c.SentDate,c.Rating,c.[Content],c.Title,c.IsCustomer,c.IsAdmin,c.Status,s.UserName from C900 c left join S200 s on c.SentFrom = s.ID where c.ID=@id ";
            return this._connection.Query<object>(query, new { id }, transaction).FirstOrDefault();
        }

        public List<object> GetListEnquiryReplyView(string enquiryId, IDbTransaction transaction = null)
        {
            string query = "select * from C901 where EnquiryID=@enquiryId ";
            return this._connection.Query<object>(query, new { enquiryId }, transaction).ToList();
        }

        public List<object> GetListEnquiryFileView(string enquiryId, IDbTransaction transaction = null)
        {
            string query = "select * from C902 where EnquiryID=@enquiryId";
            return this._connection.Query<object>(query, new { enquiryId }, transaction).ToList();
        }

        public List<object> GetListEnquiryReplyFileView(string enquiryId, IDbTransaction transaction = null)
        {
            
            string query = "select * from C903 where EnquiryReplyID in (select ID from C901 where EnquiryID=@enquiryId)";
            return this._connection.Query<object>(query, new { enquiryId }, transaction).ToList();
        }

        public bool InsertEnquiryReply(C901 model, IDbTransaction transaction = null) {
            string query = "INSERT INTO [dbo].[C901] ([ID],[EnquiryID],[SentFrom],[SentTo],[SentDate],[Content],[IsCustomer]) VALUES (@ID,@EnquiryID,@SentFrom,@SentTo,GETDATE(),@Content,@IsCustomer)";
            return this._connection.Execute(query, model, transaction) > 0;
        }
        public bool InsertEnquiryFileReply(C903 model, IDbTransaction transaction = null)
        {
            string query = "INSERT INTO [dbo].[C903] ([ID],[EnquiryReplyID],[FileName],[FilePath],[FileType]) VALUES (@ID,@EnquiryReplyID,@FileName,@FilePath,@FileType)";
            return this._connection.Execute(query, model, transaction) > 0;
        }
    }
}