using CleverApi.Models;
using CleverApi.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CleverApi.Providers
{
    public class LogProvider
    {
        public static void InsertLog(string message = "",S200 userAdmin = null,IDbConnection connection = null,IDbTransaction transaction = null) {
            if (userAdmin == null) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
            LogsService logsService = new LogsService(connection);
            if (!logsService.InsertLogs(new S500()
            {
                Actions = message,
                UserID = userAdmin.ID,
                UserName = userAdmin.UserName,
            }, transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }
    }
}