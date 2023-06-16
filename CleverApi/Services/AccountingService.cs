using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using CleverApi.Models;
using Dapper;

namespace CleverApi.Services
{
    public class AccountingService : BaseService
    {
        public AccountingService() : base() { }
        public AccountingService(IDbConnection db) : base(db) { }

        #region Category
        public List<object> GetListAccountingServiceCategory(IDbTransaction transaction = null)
        {
            string query = "SELECT * FROM C401 order by ID";
            return this._connection.Query<object>(query, null, transaction).ToList();
        }

        public void UpdateAccountingServiceCategory(C401 model, IDbTransaction transaction = null)
        {
            string query = "UPDATE [dbo].[C401] SET [Description]=@Description WHERE [ID]=@ID";
            int status = this._connection.Execute(query, model, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        public C401 GetAccountingServiceCategory(int id, IDbTransaction transaction = null)
        {
            string query = "select top 1 * from C401 where ID=@id";
            return this._connection.Query<C401>(query, new { id }, transaction).FirstOrDefault();
        }

        public C401 GetAccountingServiceCategoryByName(C401 model, IDbTransaction transaction = null)
        {
            string query = "select top 1 * from C401 where [Description]=@Description";
            if (model.ID > 0)
            {
                query += " and ID <> @ID";
            }
            return this._connection.Query<C401>(query, model, transaction).FirstOrDefault();
        }

        #endregion



        #region Service Included
        public List<object> GetListAccountingServiceIncluded()
        {
            string query = "select * from C402 order by ID";
            return this._connection.Query<object>(query).ToList();
        }

        public C402 GetAccountingServiceIncludedByName(C402 model, IDbTransaction transaction = null)
        {
            string query = "select top 1 * from C402 where Description=@Description";
            if (model.ID > 0)
            {
                query += " and ID <> @ID";
            }
            return this._connection.Query<C402>(query, model, transaction).FirstOrDefault();
        }

        public C402 GetAccountingServiceIncluded(int id, IDbTransaction transaction = null)
        {
            string query = "select top 1 * from C402 where ID=@id";
            return this._connection.Query<C402>(query, new { id }, transaction).FirstOrDefault();
        }

        public void UpdateAccountingServiceIncluded(C402 model, IDbTransaction transaction = null)
        {
            string query = "UPDATE [dbo].[C402] SET [Description]=@Description,[ScopeOfWork]=@ScopeOfWork WHERE [ID]=@ID";
            int status = this._connection.Execute(query, model, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }
        #endregion


        #region Service accounting

        public List<object> GetListAccountingService(IDbTransaction transaction = null)
        {
            string query = "select C400.*,C401.Description from C400 join C401 on C400.CategoryID=C401.ID Order by C400.CategoryID";
            return this._connection.Query<object>(query, null, transaction).ToList();
        }

        public List<object> GetListCategoryNotCreated(IDbTransaction transaction = null) {
            string query = "SELECT * FROM C401 WHERE ID NOT IN (SELECT CategoryID FROM C400)";
            return this._connection.Query<object>(query, null, transaction).ToList();
        }

        public C400 GetAccountingService(string id, IDbTransaction transaction = null)
        {
            string query = "select top 1 * from C400 Where ID=@id";
            return this._connection.Query<C400>(query, new { id }, transaction).FirstOrDefault();
        }

        public void InsertAccountingService(C400 model, IDbTransaction transaction = null) {
            string query = "INSERT INTO [dbo].[C400] ([ID],[CategoryID],[Requirement],[MonthPrice],[YearPrice],[CreatedBy],[CreatedDate]) VALUES (@ID,@CategoryID,@Requirement,@MonthPrice,@YearPrice,@CreatedBy,GETDATE())";
            int status = this._connection.Execute(query, model, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        public void UpdateAccountingService(C400 model, IDbTransaction transaction = null)
        {
            string query = "UPDATE [dbo].[C400] SET [CategoryID]=@CategoryID,[Requirement]=@Requirement,[MonthPrice]=@MonthPrice,[YearPrice]=@YearPrice,[ModifiedBy]=@ModifiedBy,[ModifiedDate]=@ModifiedDate WHERE [ID]=@ID";
            int status = this._connection.Execute(query, model, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        public void DeleteAccountingService(string id, IDbTransaction transaction = null) {
            string query = "delete from C400 where ID=@id";
            int status = this._connection.Execute(query, new { id}, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        #endregion

    }
}