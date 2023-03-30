using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CleverApi.Models;
using System.Data;
using Dapper;
namespace CleverApi.Services
{
    public class RoleService : BaseService
    {
        public RoleService() : base() { }
        public RoleService(IDbConnection db) : base(db) { }

        public object GetListRole(int page, int rowPerPage, IDbTransaction transaction = null)
        {
            string querySelect = "SELECT * ";
            string queryCount = "select count(*) as Total";
            string query = " FROM S201 WHERE ID <> '1' AND Status = 1 ";

            int totalRow = this._connection.Query<int>(queryCount + query, null, transaction).FirstOrDefault();
            int totalPage = (int)Math.Ceiling((decimal)totalRow / rowPerPage);

            int skip = (page - 1) * rowPerPage;
            query += " ORDER BY RoleName offset " + skip + " rows fetch next " + rowPerPage + " rows only";
            List<object> listData = this._connection.Query<object>(querySelect + query, null, transaction).ToList();

            return new { totalPage, listData };
        }
        public List<object> GetListAllRole() {
            return this._connection.Query<object>("select * from S201 where Status='1'").ToList();
        }
        public S201 GetRoleById(string id, IDbTransaction transaction = null)
        {
            string query = "select top 1 * from S201 where ID=@id";
            return this._connection.Query<S201>(query, new { id }, transaction).FirstOrDefault();
        }
        public S201 GetRoleByName(string name, IDbTransaction transaction = null)
        {
            string query = "select top 1 * from S201 where RoleName=@name";
            return this._connection.Query<S201>(query, new { name }, transaction).FirstOrDefault();
        }

        public bool CheckRoleSameExist(S201 model, IDbTransaction transaction = null)
        {
            string query = "select count(*) as Total from S201 where RoleName=@RoleName and ID <> @ID";
            return this._connection.Query<int>(query, model, transaction).FirstOrDefault() > 0;
        }

        public bool InsertRole(S201 model, IDbTransaction transaction = null)
        {
            string query = "INSERT INTO [dbo].[S201] ([ID],[RoleName],[Status],[CreatedBy],[CreatedDate]) VALUES (@ID,@RoleName,@Status,@CreatedBy,GETDATE())";
            return this._connection.Execute(query, model, transaction) > 0;
        }

        public bool UpdateRole(S201 model, IDbTransaction transaction = null)
        {
            string query = "UPDATE [dbo].[S201] SET [RoleName]=@RoleName,[Status]=@Status,[ModifiedBy]=@ModifiedBy,[ModifiedDate]=getdate() WHERE [ID]=@ID";
            return this._connection.Execute(query, model, transaction) > 0;
        }

    }
}