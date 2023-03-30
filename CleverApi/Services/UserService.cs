using CleverApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Dapper;
namespace CleverApi.Services
{
    public class UserService : BaseService
    {
        public UserService() : base() { }
        public UserService(IDbConnection db) : base(db) { }
        public S200 GetUserById(string id, IDbTransaction transaction = null)
        {
            string query = "select top 1 * from S200 where ID=@id";
            return this._connection.Query<S200>(query, new { id }, transaction).FirstOrDefault();
        }

        public bool IsUsernameExist(string username, IDbTransaction transaction = null) {
            string query = "select count(*) as Total from S200 where UserName=@username";
            int count = this._connection.Query<int>(query, new { username }, transaction).FirstOrDefault();
            return count > 0;
        }

        public S200 GetUserAdminByAccount(string account, IDbTransaction transaction = null)
        {
            string query = "select top 1 * from S200 where UserName=@account and UserType='0'";
            return this._connection.Query<S200>(query, new { account }, transaction).FirstOrDefault();
        }

        public S200 GetUserByAccount(string account, IDbTransaction transaction = null)
        {
            string query = "select top 1 * from S200 where UserName=@account";
            return this._connection.Query<S200>(query, new { account }, transaction).FirstOrDefault();
        }

        public object GetListUser(int page ,int rowPerPage , IDbTransaction transaction = null)
        {
            int skip = (page - 1) * rowPerPage;
            string querySelect = "select S200.*,S201.RoleName";
            string queryCount = "select count(*) as Total"; 
            string query = " from S200 left join S201 on S200.RoleID=S201.ID  where S200.Status = @status AND UserType = @userType AND S200.ID <> @adminId ";

            int totalRow = this._connection.Query<int>(queryCount + query, new { status = S200.EnumStatus.ACTIVE, userType = S200.EnumUserType.SYSTEM_USER, adminId = "1" }, transaction).FirstOrDefault();
            int totalPage = (int)Math.Ceiling((decimal)totalRow / rowPerPage);

            query += "  ORDER BY RoleID, UserName offset " + skip + " rows fetch next " + rowPerPage + " rows only";

            List<object> listData = this._connection.Query<object>(querySelect+query, new { status = S200.EnumStatus.ACTIVE, userType = S200.EnumUserType.SYSTEM_USER, adminId = "1" }, transaction).ToList();

            return new { totalPage , listData };
        }

        public bool InsertUser(S200 model, IDbTransaction transaction = null) {
            string query = "INSERT INTO[dbo].[S200] ([ID],[RoleID],[UserName],[Avatar],[PasswordHash],[UserType],[Status],[CreatedBy],[CreatedDate],[FirstName],[LastName]) VALUES (@ID,@RoleID,@UserName,@Avatar,@PasswordHash,@UserType,@Status,@CreatedBy,GETDATE(),@FirstName,@LastName)";
            return this._connection.Execute(query,model,transaction) > 0;
        }

        public bool UpdateUser(S200 model, IDbTransaction transaction = null) {
            string query = "UPDATE [dbo].[S200] SET [RoleID]=@RoleID,[UserName]=@UserName,[FirstName]=@FirstName,[LastName]=@LastName,[Avatar]=@Avatar,[PasswordHash]=@PasswordHash,[UserType]=@UserType,[Status]=@Status,[ModifiedBy]=@ModifiedBy,[ModifiedDate]=getdate() WHERE [ID]=@ID";
            return this._connection.Execute(query, model, transaction) > 0;
        }

        public bool DeleteUser(string id, IDbTransaction transaction = null)
        {
            string qeury = "UPDATE S200 SET Status = @status WHERE ID = @id";
            return this._connection.Execute(qeury, new { status = S200.EnumStatus.INACTIVE, id }, transaction) > 0;
        }

   
    }
}