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
    public class DataRoomService : BaseService
    {
        public DataRoomService() : base() { }
        public DataRoomService(IDbConnection db) : base(db) { }
        public object GetListData(int page, IDbTransaction transaction = null)
        {
            string querySelect = "SELECT t.*,d30.ProvinceName,d31.DistrictName,d32.WardName ";
            string queryCount = "select count(*) ";
            string query = " FROM T100 T left join C700 C on t.ID=C.CompanyID left join D300 d30 on t.ProvinceID = d30.ID left join D301 d31 on t.DistrictID = d31.ID  left join D302 d32 on t.WardID = d32.ID WHERE C.Status = 1 and T.IsFolder=1";
            int totalRow = this._connection.Query<int>(queryCount + query, null, transaction).FirstOrDefault();
            int skip = (page - 1) * Constant.ADMIN_PAGE_SIZE;
            query += " order by t.CompanyName offset " + skip + " rows fetch next " + Constant.ADMIN_PAGE_SIZE + " rows only";
            List<object> listData = this._connection.Query<object>(querySelect + query, null, transaction).ToList();
            return new { totalRow, listData };
        }

        public C700 GetCompanyDataRoom(string id, IDbTransaction transaction = null)
        {
            string query = "select * from C700 where ID=@id and Status=1";
            return this._connection.Query<C700>(query, new { id }, transaction).FirstOrDefault();
        }

        public bool DeleteData(string companyId, IDbTransaction transaction = null)
        {
            string query = "UPDATE C700 SET Status = 0 WHERE CompanyID = @companyId";
            return this._connection.Execute(query, new { companyId }, transaction) > 0;
        }

        public bool CreateFolderCompany(C700 model, IDbTransaction transaction = null)
        {
            string query = "INSERT INTO [dbo].[C700] ([ID],[CompanyID],[FolderName],[ParentID],[Status],[CreatedBy],[CreatedDate]) VALUES (@ID,@CompanyID,@FolderName,@ParentID,@Status,@CreatedBy,GETDATE())";
            return this._connection.Execute(query, model, transaction) > 0;
        }


        public bool CreateFile(C701 model, IDbTransaction transaction = null)
        {
            string query = "INSERT INTO [dbo].[C701] ([ID],[FolderID],[FileName],[FilePath],[FileType],[Status],[CreatedBy],[CreatedDate]) VALUES (@ID,@FolderID,@FileName,@FilePath,@FileType,@Status,@CreatedBy,GETDATE())";
            return this._connection.Execute(query, model, transaction) > 0;
        }


        public C700 GetRootFolder(string companyId, IDbTransaction transaction = null)
        {
            string query = "select top 1 * from C700 where CompanyID=@companyId and ParentID is null and Status=1";
            return this._connection.Query<C700>(query, new { companyId }, transaction).FirstOrDefault();
        }

        public List<object> LoadCompanyRootFolder(string companyId, IDbTransaction transaction = null)
        {
            string query = "SELECT * FROM C700 WHERE CompanyID = @companyId AND Status = 1 AND ParentID IS NULL ORDER BY FolderName";
            return this._connection.Query<object>(query, new { companyId }, transaction).ToList();
        }

        public List<C701> LoadCompanyFileInFolder(string folderId, IDbTransaction transaction = null)
        {
            string query = "SELECT * FROM C701 WHERE FolderID = @folderId AND Status = 1 ORDER BY FileName";
            return this._connection.Query<C701>(query, new { folderId }, transaction).ToList();
        }
        public List<C700> LoadCompanyFolderInFolder(string folderId, IDbTransaction transaction = null)
        {
            string query = "SELECT * FROM C700 WHERE ParentID = @folderId AND Status = 1 ORDER BY FolderName";
            return this._connection.Query<C700>(query, new { folderId }, transaction).ToList();
        }

        public C701 GetFile(string id, IDbTransaction transaction = null)
        {
            string query = "select top 1 * from C701 where ID=@id";
            return this._connection.Query<C701>(query, new { id }, transaction).FirstOrDefault();
        }
        public C700 GetFolder(string id, IDbTransaction transaction = null) {
            string query = "select top 1 * from C700 where ID=@id";
            return this._connection.Query<C700>(query, new { id }, transaction).FirstOrDefault();
        }
        public bool DeleteFile(string id, IDbTransaction transaction = null) {
            string query = "update C701 set Status=0 where ID=@id";
            return this._connection.Execute(query,new { id },transaction) > 0;
        }
        public bool DeleteFolder(string id, IDbTransaction transaction = null)
        {
            string query = "update C700 set Status=0 where ID=@id";
            return this._connection.Execute(query, new { id }, transaction) > 0;
        }

        public bool DeleteAllFileInFolder(string folderId, IDbTransaction transaction = null) {
            string query = "update C701 set Status=0 where FolderID=@folderId";
            return this._connection.Execute(query, new { folderId }, transaction) > 0;
        }
        public bool DeleteAllFolderInFolder(string folderId, IDbTransaction transaction = null)
        {
            string query = "update C700 set Status=0 where ParentID=@folderId";
            return this._connection.Execute(query, new { folderId }, transaction) > 0;
        }
    }

}