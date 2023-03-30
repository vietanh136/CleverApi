using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using CleverApi.Models;
using Dapper;
namespace CleverApi.Services
{
    public class ServiceManagementService : BaseService
    {
        public ServiceManagementService() : base() { }
        public ServiceManagementService(IDbConnection db) : base(db) { }

        public object GetList(int page, int rowPerPage, IDbTransaction transaction = null)
        {
            string queryCount = "select count(*) as Total";
            string querySelect = "select *";
            string query = " FROM C200 WHERE Status = 1";

            int totalRow = this._connection.Query<int>(queryCount + query, null, transaction).FirstOrDefault();
            int totalPage = (int)Math.Ceiling((decimal)totalRow / rowPerPage);

            int skip = (page - 1) * rowPerPage;
            query += " ORDER BY ID offset " + skip + " rows fetch next " + rowPerPage + "row only";

            List<object> listData = this._connection.Query<object>(querySelect + query, null, transaction).ToList();
            return new { totalPage, listData };
        }
        public C200 GetById(int id, IDbTransaction transaction = null)
        {
            string query = "select top 1 * from C200 where ID=@id and Status=@status";
            return this._connection.Query<C200>(query, new { id, status = C200.EnumStatus.ENABLED }, transaction).FirstOrDefault();
        }
        public C200 GetByServiceName(string serviceName, IDbTransaction transaction = null)
        {
            string query = "select top 1 * from C200 where ServiceName=@serviceName";
            return this._connection.Query<C200>(query, new { serviceName }, transaction).FirstOrDefault();
        }
        public bool Update(C200 model, IDbTransaction transaction = null)
        {
            string query = "UPDATE [dbo].[C200] SET [ServiceName]=@ServiceName,[ServiceImage]=@ServiceImage,[Status]=@Status,[ModifiedBy]=@ModifiedBy,ModifiedDate=getdate() WHERE ID=@ID";
            return this._connection.Execute(query, model, transaction) > 0;
        }
    }
}
