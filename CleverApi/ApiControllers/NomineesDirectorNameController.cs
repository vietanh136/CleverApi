using CleverApi.Models;
using CleverApi.Providers;
using CleverApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CleverApi.ApiControllers
{
    public class NomineesDirectorNameController : ApiBaseController
    {
        [HttpGet]
        public JsonResult GetList(int? page,int? rowPerPage)
        {
            try
            {
                NomineesDirectorNameService nomineesDirectorNameService = new NomineesDirectorNameService();
                if (!page.HasValue) page = 1;
                if (!rowPerPage.HasValue) rowPerPage = Constant.ADMIN_PAGE_SIZE;
                return Success(nomineesDirectorNameService.GetList(page.Value, rowPerPage.Value));
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult Insert(S102 model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.DirectorName)) throw new Exception("Director name cannot be left blank");
                using (var connection = BaseService.Connect())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        S200 userAdmin = HelperProvider.GetUserFromRequest(Request, connection, transaction);

                        NomineesDirectorNameService nomineesDirectorNameService = new NomineesDirectorNameService(connection);
                        S102 userDirectorNameExisted = nomineesDirectorNameService.GetByDirectorName(model.DirectorName, transaction);
                        if (userDirectorNameExisted != null) throw new Exception("Director name is not available");

                        model.ID = Guid.NewGuid().ToString();
                        model.CreatedBy = userAdmin.ID;
                        model.Status = S102.EnumStatus.ENABLED;
                        if (!nomineesDirectorNameService.Insert(model, transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);

                        LogProvider.InsertLog("Thêm tên giám đốc cho thuê", userAdmin, connection, transaction);
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

        [HttpGet]
        public JsonResult GetDetail(string id)
        {
            try
            {
                NomineesDirectorNameService nomineesDirectorNameService = new NomineesDirectorNameService();
                return Success(nomineesDirectorNameService.GetById(id));
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult Delete(string id)
        {
            try
            {
                using (var connection = BaseService.Connect())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        S200 userAdmin = HelperProvider.GetUserFromRequest(Request, connection, transaction);

                        NomineesDirectorNameService nomineesDirectorNameService = new NomineesDirectorNameService(connection);
                        S102 nomineesDirectorName = nomineesDirectorNameService.GetById(id, transaction);
                        if (nomineesDirectorName == null) throw new Exception(JsonResult.Message.DATA_NOT_FOUND);

                        nomineesDirectorName.Status = S102.EnumStatus.DISABLED;

                        if (!nomineesDirectorNameService.Update(nomineesDirectorName, transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);

                        LogProvider.InsertLog("Xóa tên giám đốc cho thuê", userAdmin, connection, transaction);
                        transaction.Commit();
                        return Success(null, "Deleted \"" + nomineesDirectorName.DirectorName + "\" successfully");

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
