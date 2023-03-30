using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dapper;
using CleverApi.Models;
using System.Data;
namespace CleverApi.Services
{
    public class MailRoomService : BaseService
    {
        public MailRoomService() : base() { }
        public MailRoomService(IDbConnection db) : base(db) { }

        public object GetListMailRoomViewModel(int page, IDbTransaction transaction = null)
        {
            string querySelect = "select * ";
            string queryCount = "select count(*) as Total";
            string query = " from C800 ";

            int totalRow = this._connection.Query<int>(queryCount + query, null, transaction).FirstOrDefault();

            int skip = (page - 1) * Constant.ADMIN_PAGE_SIZE;
            query += " order by SentDate desc offset " + skip + " rows fetch next " + Constant.ADMIN_PAGE_SIZE + " rows only";
            var listData = this._connection.Query<object>(querySelect + query, null, transaction).ToList();
            return new { totalRow, listData };
        }

        public bool DeleteMailRoom(string id, IDbTransaction transaction = null)
        {
            string query = "update C800 set Status=2 where ID=@id";
            return this._connection.Execute(query, new { id }, transaction) > 0;
        }

        public bool InsertMailRoom(C800 model, IDbTransaction transaction = null) {
            string query = "INSERT INTO [dbo].[C800] ([ID],[SentTo],[Email],[SentDate],[SentBy],[Title],[Status]) VALUES (@ID,@SentTo,@Email,GETDATE(),@SentBy,@Title,@Status)";
            return this._connection.Execute(query, model, transaction) > 0;
        }

        public bool InsertMailRoomAttachment(C801 model, IDbTransaction transaction = null) {
            string query = "INSERT INTO [dbo].[C801] ([ID],[MailID],[FileName],[FilePath],[FileType]) VALUES (@ID,@MailID,@FileName,@FilePath,@FileType)";
            return this._connection.Execute(query, model, transaction) > 0;
        }

        public C800 GetMailRoom(string id, IDbTransaction transaction = null) {
            string query = "select top 1 * from C800 where ID=@id";
            return this._connection.Query<C800>(query, new { id }, transaction).FirstOrDefault();
        }

        public List<C801> GetListMailAttachment(string emailId, IDbTransaction transaction = null) {
            string query = "select * from C801 where MailID=@emailId";
            return this._connection.Query<C801>(query, new { emailId }, transaction).ToList();
        }
    }
}