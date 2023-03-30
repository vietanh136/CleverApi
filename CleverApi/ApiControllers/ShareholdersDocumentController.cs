using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CleverApi.Models;
using CleverApi.Services;
using CleverApi.Providers;

namespace CleverApi.ApiControllers
{
    public class ShareholdersDocumentController : ApiBaseController
    {
        [HttpGet]
        public JsonResult GetList(int? page,int? rowPerPage ) {
            try {
                ShareholdersDocumentService shareholdersDocumentService = new ShareholdersDocumentService();
                if (!page.HasValue) page = 1;
                if (!rowPerPage.HasValue) rowPerPage = Constant.ADMIN_PAGE_SIZE;
                return Success(shareholdersDocumentService.GetList(page.Value, rowPerPage.Value));
            }
            catch (Exception ex) {
                return Error(ex.Message);
            }
        }

        public JsonResult GetListShareholderType()
        {
            try
            {
                ShareholdersDocumentService shareholdersDocumentService = new ShareholdersDocumentService();
                return Success(shareholdersDocumentService.GetListShareholderType());
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult Insert(S103 model) {
            try {
                if (string.IsNullOrEmpty(model.DocumentName)) throw new Exception("Document name cannot be left blank");
                if(model.Type <= 0) throw new Exception("Please choose Shareholder type");
                using (var connection = BaseService.Connect()) {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction()) {
                        S200 userAdmin = HelperProvider.GetUserFromRequest(Request,connection,transaction);
                        ShareholdersDocumentService shareholdersDocumentService = new ShareholdersDocumentService(connection);

                        if (shareholdersDocumentService.CheckSameNameExist(model, transaction)) throw new Exception("Document name is not available");

                        model.ID = Guid.NewGuid().ToString();
                        model.CreatedBy = userAdmin.ID;
                        model.Status = S103.EnumStatus.ENABLED;
                        if (!shareholdersDocumentService.Insert(model, transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
                        LogProvider.InsertLog("Thêm hồ sơ cổ đông",userAdmin,connection,transaction);
                        transaction.Commit();
                        return Success();
                    }
                }
            }
            catch (Exception ex) {
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
                        ShareholdersDocumentService shareholdersDocumentService = new ShareholdersDocumentService(connection);
                        S103 shareholderDocument = shareholdersDocumentService.GetById(id,transaction);
                        if (shareholderDocument == null) throw new Exception(JsonResult.Message.DATA_NOT_FOUND);
                        shareholderDocument.Status = S103.EnumStatus.DISABLED;
                        if (!shareholdersDocumentService.Update(shareholderDocument, transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
                        LogProvider.InsertLog("Xóa hồ sơ cổ đông", userAdmin, connection, transaction);
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
