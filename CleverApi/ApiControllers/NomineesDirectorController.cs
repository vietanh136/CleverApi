using CleverApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CleverApi.Services;
using CleverApi.Providers;
using System.Web;

namespace CleverApi.ApiControllers
{
    public class NomineesDirectorController : ApiBaseController
    {
        [HttpGet]
        public JsonResult GetList(int? page,int? rowPerPage ) {
            try {
                NomineesDirectorService nomineesDirectorService = new NomineesDirectorService();
                if (!page.HasValue) page = 1;
                if (!rowPerPage.HasValue) rowPerPage = Constant.ADMIN_PAGE_SIZE;
                return Success(nomineesDirectorService.GetList(page.Value,rowPerPage.Value));
            }
            catch (Exception ex) {
                return Error(ex.Message);

            }
        }

        [HttpGet]
        public JsonResult GetData(string id)
        {
            try
            {
                NomineesDirectorService nomineesDirectorService = new NomineesDirectorService();
                return Success(nomineesDirectorService.GetById(id));
            }
            catch (Exception ex)
            {
                return Error(ex.Message);

            }
        }

        [HttpGet]
        public JsonResult Delete(string id) {
            try {
                using (var connection = BaseService.Connect()) {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction()) {
                        S200 userAdmin = HelperProvider.GetUserFromRequest(Request,connection,transaction);

                        NomineesDirectorService nomineesDirectorService = new NomineesDirectorService(connection);
                        C300 nomineesDirector = nomineesDirectorService.GetById(id,transaction);
                        if (nomineesDirector == null) throw new Exception(JsonResult.Message.DATA_NOT_FOUND);

                        if (!nomineesDirectorService.Delete(id,transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
                        LogProvider.InsertLog("Xóa dịch vụ giám đốc " +userAdmin.UserName,userAdmin,connection,transaction);

                        transaction.Commit();
                        return Success();
                    }
                }
            }
            catch (Exception ex) {
                return Error(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult Insert(C300 model) {
            try {
                if (model.Amount <= 0) throw new Exception("Amount cannot be less than 0");

                using (var connection = BaseService.Connect()) {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction()) {
                        S200 userAdmin = HelperProvider.GetUserFromRequest(Request,connection,transaction);

                        NomineesDirectorService nomineesDirectorService = new NomineesDirectorService(connection);
                        if (nomineesDirectorService.CheckSameValueExist(model, transaction)) throw new Exception("Number of Month is not available");

                        model.ID = Guid.NewGuid().ToString();
                        if (!nomineesDirectorService.Update(model, transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);

                        LogProvider.InsertLog("Thêm dịch vụ giám đốc",userAdmin,connection,transaction);
                        transaction.Commit();
                        return Success();
                    }
                }
            }
            catch (Exception ex) {
                return Error(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult Update(C300 model)
        {
            try
            {
                if (model.Amount <= 0) throw new Exception("Amount cannot be less than 0");
                using (var connection = BaseService.Connect())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        S200 userAdmin = HelperProvider.GetUserFromRequest(Request, connection, transaction);
                        NomineesDirectorService nomineesDirectorService = new NomineesDirectorService(connection);
                        if (!nomineesDirectorService.Update(model, transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
                        LogProvider.InsertLog("Cập nhật dịch vụ giám đốc", userAdmin, connection, transaction);
                        transaction.Commit();
                        return Success();
                    }
                }
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

    }
}
