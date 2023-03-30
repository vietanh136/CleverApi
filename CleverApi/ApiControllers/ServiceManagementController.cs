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
    public class ServiceManagementController : ApiBaseController
    {
        [HttpGet]
        public JsonResult GetList(int? page, int? rowPerPage)
        {
            try
            {
                ServiceManagementService serviceManagementService = new ServiceManagementService();
                if (!page.HasValue) page = 1;
                if (!rowPerPage.HasValue) rowPerPage = Constant.ADMIN_PAGE_SIZE;
                return Success(serviceManagementService.GetList(page.Value,rowPerPage.Value));
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetService(int id)
        {
            try
            {
                ServiceManagementService serviceManagementService = new ServiceManagementService();
                return Success(serviceManagementService.GetById(id));
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult Update(C200 model)
        {
            try
            {
                using (var connection = BaseService.Connect())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        S200 userAdmin = HelperProvider.GetUserFromRequest(Request, connection, transaction);
                        ServiceManagementService serviceManagementService = new ServiceManagementService(connection);
                        C200 service = serviceManagementService.GetById(model.ID, transaction);
                        if (service == null) throw new Exception(JsonResult.Message.DATA_NOT_FOUND);

                        C200 checkServiceName = serviceManagementService.GetByServiceName(model.ServiceName, transaction);
                        if (checkServiceName != null && checkServiceName.ID != service.ID) throw new Exception("Service name is not available");

                        service.ModifiedBy = userAdmin.ID;
                        service.ServiceName = model.ServiceName;
                        if (!string.IsNullOrEmpty(model.ServiceImage))
                        {
                            if (!string.IsNullOrEmpty(service.ServiceImage)) {
                                HelperProvider.DeleteFile(service.ServiceImage);
                            }

                            string fileName = Guid.NewGuid().ToString() + ".png";
                            string path = HttpContext.Current.Server.MapPath(Constant.SERVICES_THUMBNAIL_PATH + fileName);
                            HelperProvider.Base64ToImage(model.ServiceImage, path);
                            service.ServiceImage = Constant.SERVICES_THUMBNAIL_URL + fileName;
                        }

                        if (!serviceManagementService.Update(service, transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);

                        LogProvider.InsertLog("Sửa dịch vụ [" + service.ServiceName + "]", userAdmin, connection, transaction);
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
