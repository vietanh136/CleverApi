using CleverApi.Models;
using CleverApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CleverApi.Providers;

namespace CleverApi.ApiControllers
{
    public class SystemContentController : ApiBaseController
    {
        [HttpGet]
        public JsonResult GetBusinessAccountTermsConditions() {
            try {
                SystemContentService systemContentService = new SystemContentService();
                return Success(systemContentService.GetBusinessAccountTermsConditions());
            }
            catch (Exception ex) {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetDataProtectionPrivacyPolicy()
        {
            try
            {
                SystemContentService systemContentService = new SystemContentService();
                return Success(systemContentService.GetDataProtectionPrivacyPolicy());
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetTermsConditions()
        {
            try
            {
                SystemContentService systemContentService = new SystemContentService();
                return Success(systemContentService.GetTermsConditions());
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult UpdateBusinessAccountTermsConditions(S100 model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Term1)) throw new Exception("Business Account Terms and Conditions cannot be null");
                using (var connection = BaseService.Connect()) {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction()) {
                        S200 userAdmin = HelperProvider.GetUserFromRequest(Request, connection, transaction);
                        if (userAdmin == null) return Unauthorized();

                        SystemContentService systemContentService = new SystemContentService(connection);
                        if (!systemContentService.UpdateBusinessAccountTermsConditions(model.Term1, transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
                        LogProvider.InsertLog("Cập nhật các điều khoản sử dụng", userAdmin, connection, transaction);
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
        [HttpPost]
        public JsonResult UpdateTermsConditions(S100 model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Term2)) throw new Exception("Terms and Conditions cannot be null");
                using (var connection = BaseService.Connect())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        S200 userAdmin = HelperProvider.GetUserFromRequest(Request, connection, transaction);
                        if (userAdmin == null) return Unauthorized();
                        SystemContentService systemContentService = new SystemContentService(connection);
                        if(!systemContentService.UpdateTermsConditions(model.Term2,transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
                        LogProvider.InsertLog("Cập nhật các điều khoản sử dụng", userAdmin, connection, transaction);
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
        [HttpPost]
        public JsonResult UpdateDataProtectionPrivacyPolicy(S100 model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Term3)) throw new Exception("Data Protection and Privacy Policy cannot be null");
                using (var connection = BaseService.Connect())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        S200 userAdmin = HelperProvider.GetUserFromRequest(Request, connection, transaction);

                        SystemContentService systemContentService = new SystemContentService(connection);
                        if(!systemContentService.UpdateDataProtectionPrivacyPolicy(model.Term3,transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
                        LogProvider.InsertLog("Cập nhật các điều khoản sử dụng", userAdmin, connection, transaction);
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
