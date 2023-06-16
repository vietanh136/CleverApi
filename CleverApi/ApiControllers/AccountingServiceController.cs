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
    public class AccountingServiceController : ApiBaseController
    {
        [HttpGet]
        public JsonResult GetListAccountingServiceCategory()
        {
            try
            {
                AccountingService accountingService = new AccountingService();
                return Success(accountingService.GetListAccountingServiceCategory());
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetAccountingServiceCategory(int id)
        {
            try
            {
                AccountingService accountingService = new AccountingService();
                return Success(accountingService.GetAccountingServiceCategory(id));
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult UpdateAccountingServiceCategory(C401 model)
        {
            try
            {
                using (var connection = BaseService.Connect()) {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction()) {

                        S200 userAdmin = HelperProvider.GetUserFromRequest(Request,connection,transaction);

                        AccountingService accountingService = new AccountingService(connection);

                        C401 category = accountingService.GetAccountingServiceCategoryByName(model, transaction);
                        if (category != null) throw new Exception("This category name is not available");

                        category = accountingService.GetAccountingServiceCategory(model.ID, transaction);
                        if(category == null) throw new Exception(JsonResult.Message.ERROR_SYSTEM);

                        category.Description = model.Description;
                        accountingService.UpdateAccountingServiceCategory(category, transaction);

                        LogProvider.InsertLog("Edit accounting service category name",userAdmin,connection,transaction);
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
        public JsonResult GetListAccountingServiceIncluded() {
            try {
                AccountingService accountingService = new AccountingService();
                return Success(accountingService.GetListAccountingServiceIncluded());
            }
            catch (Exception ex) {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetAccountingServiceIncluded(int id)
        {
            try
            {
                AccountingService accountingService = new AccountingService();
                return Success(accountingService.GetAccountingServiceIncluded(id));
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult UpdateAccountingServiceIncluded(C402 model) {
            try {
                using (var connection = BaseService.Connect()) {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction()) {
                        S200 userAdmin = HelperProvider.GetUserFromRequest(Request,connection,transaction);
                        AccountingService accountingService = new AccountingService(connection);

                        C402 serviceIncluded = accountingService.GetAccountingServiceIncludedByName(model, transaction);
                        if (serviceIncluded != null) throw new Exception("This description is not available");

                        serviceIncluded = accountingService.GetAccountingServiceIncluded(model.ID, transaction);
                        if (serviceIncluded == null) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
                        serviceIncluded.Description = model.Description;
                        serviceIncluded.ScopeOfWork = model.ScopeOfWork;
                        accountingService.UpdateAccountingServiceIncluded(serviceIncluded,transaction);

                        LogProvider.InsertLog("Edit Service Included",userAdmin,connection,transaction);
                        transaction.Commit();
                        return Success();
                    }
                }
            } catch (Exception ex) {
                return Error(ex.Message);
            }
        }


        [HttpGet]
        public JsonResult GetListAccountingService() {
            try {
                AccountingService accountingService = new AccountingService();
                return Success(accountingService.GetListAccountingService());
            }
            catch (Exception ex) {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetAccountingService(string id)
        {
            try
            {
                AccountingService accountingService = new AccountingService();
                return Success(accountingService.GetAccountingService(id));
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetListCategoryNotCreated() {
            try {
                AccountingService accountingService = new AccountingService();
                return Success(accountingService.GetListCategoryNotCreated());
            } catch (Exception ex) { return Error(ex.Message); }
        }

        [HttpPost]
        public JsonResult InsertAccountingService(C400 model) {
            try {
                using (var connection = BaseService.Connect()) {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction()) {
                        S200 userAdmin = HelperProvider.GetUserFromRequest(Request, connection, transaction);

                        AccountingService accountingService = new AccountingService(connection);

                        C401 category = accountingService.GetAccountingServiceCategory(model.CategoryID, transaction);

                        model.ID = Guid.NewGuid().ToString();
                        model.CreatedBy = userAdmin.ID;

                        accountingService.InsertAccountingService(model, transaction);

                        LogProvider.InsertLog("Tạo dịch vụ kế toán cho gói "+ category.Description,userAdmin,connection,transaction);
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
        public JsonResult UpdateAccountingService(C400 model)
        {
            try
            {
                using (var connection = BaseService.Connect())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        S200 userAdmin = HelperProvider.GetUserFromRequest(Request, connection, transaction);

                        AccountingService accountingService = new AccountingService(connection);

                        C400 accounting = accountingService.GetAccountingService(model.ID, transaction);
                        if (accounting == null) throw new Exception(JsonResult.Message.ERROR_SYSTEM);

                        C401 category = accountingService.GetAccountingServiceCategory(model.CategoryID, transaction);

                        accounting.ModifiedBy = userAdmin.ID;
                        accounting.MonthPrice = model.MonthPrice;
                        accounting.Requirement = model.Requirement;
                        accounting.YearPrice = model.YearPrice;
                        
                        accountingService.UpdateAccountingService(accounting, transaction);

                        LogProvider.InsertLog("Sửa dịch vụ kế toán cho gói " + category.Description, userAdmin, connection, transaction);
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
        public JsonResult DeleteAccountingService(string id) {
            try {
                using (var connection = BaseService.Connect())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        S200 userAdmin = HelperProvider.GetUserFromRequest(Request, connection, transaction);

                        AccountingService accountingService = new AccountingService(connection);

                        C400 accounting = accountingService.GetAccountingService(id, transaction);
                        if (accounting == null) throw new Exception(JsonResult.Message.ERROR_SYSTEM);

                        C401 category = accountingService.GetAccountingServiceCategory(accounting.CategoryID, transaction);

                        accountingService.DeleteAccountingService(accounting.ID, transaction);

                        LogProvider.InsertLog("Xóa dịch vụ kế toán cho gói " + category.Description, userAdmin, connection, transaction);
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
