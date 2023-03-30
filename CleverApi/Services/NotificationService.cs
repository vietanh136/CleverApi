using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using CleverApi.Models;
using Dapper;
namespace CleverApi.Services
{
    public class NotificationService : BaseService
    {
        public object GetListNotification(int page, IDbTransaction transaction = null)
        {
            string queryCount = "select count(*) as Total";
            string querySelect = "select *";
            string query = " from S500";

            int totalRow = this._connection.Query<int>(queryCount + query, null, transaction).FirstOrDefault();
            int totalPage = (int)Math.Ceiling((decimal)totalRow / Constant.ADMIN_PAGE_SIZE);

            int skip = (page - 1) * Constant.ADMIN_PAGE_SIZE;
            query += " order by LogTime desc offset "+ skip + " rows fetch next "+ Constant.ADMIN_PAGE_SIZE+" rows only";
            List<object> listData = this._connection.Query<object>(querySelect + query, null, transaction).ToList();
            return new { totalPage, listData };

        }
    }
}