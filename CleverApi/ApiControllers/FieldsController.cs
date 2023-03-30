using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CleverApi.Services;
using CleverApi.Models;
using CleverApi.Providers;

namespace CleverApi.ApiControllers
{
    public class FieldsController : ApiBaseController
    {
        [HttpGet]
        public JsonResult GetList(int? page, int? rowPerPage) {
            try {
                FieldsService fieldsService = new FieldsService();
                if (!page.HasValue) page = 1;
                if (!rowPerPage.HasValue) rowPerPage = Constant.ADMIN_PAGE_SIZE;
                return Success(fieldsService.GetList(page.Value,rowPerPage.Value));
            }
            catch (Exception ex) {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetField(string id)
        {
            try {
                FieldsService fieldsService = new FieldsService();
                return Success(fieldsService.GetById(id));
            }
            catch (Exception ex) {
                return Error(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult Update(C100 model)
        {
            try {
                using (var connection = BaseService.Connect()) {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction()) {
                        S200 userAdmin = HelperProvider.GetUserFromRequest(Request,connection,transaction);
                        FieldsService fieldsService = new FieldsService(connection);
                        C100 field = fieldsService.GetById(model.ID, transaction);
                        if (field == null) throw new Exception(JsonResult.Message.DATA_NOT_FOUND);

                        field.FieldNameEN = model.FieldNameEN;
                        field.IsPause = model.IsPause;
                        if (!fieldsService.Update(field, transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);

                        LogProvider.InsertLog("Sửa ngành nghề ["+field.ID+"]",userAdmin,connection,transaction);
                        transaction.Commit();
                        return Success();
                    }
                }
            }
            catch (Exception ex) {
                return Error(ex.Message);
            }
        }
    }
}
