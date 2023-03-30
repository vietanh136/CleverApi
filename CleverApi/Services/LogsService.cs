using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Dapper;
using CleverApi.Models;
namespace CleverApi.Services
{
    public class LogsService : BaseService
    {
        public LogsService() : base() { }
        public LogsService(IDbConnection db) : base(db) { }

        public bool InsertLogs(S500 model, IDbTransaction transaction = null) {
            string query = "INSERT INTO [dbo].[S500] ([UserID],[UserName],[LogTime],[Actions]) VALUES (@UserID,@UserName,GETDATE(),@Actions)";
            return this._connection.Execute(query,model,transaction) > 0;
        }
    }
}