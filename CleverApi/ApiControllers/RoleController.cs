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
    public class RoleController : ApiBaseController
    {
        [HttpGet]
        public JsonResult LoadListRole(int? page,int? rowPerPage)
        {
            try
            {
                RoleService roleService = new RoleService();
                if (!page.HasValue) page = 1;
                if (!rowPerPage.HasValue) rowPerPage = Constant.ADMIN_PAGE_SIZE;
                return Success(roleService.GetListRole(page.Value,rowPerPage.Value));
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult LoadListAllRole()
        {
            try
            {
                RoleService roleService = new RoleService();
                return Success(roleService.GetListAllRole());
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult LoadRole(string id)
        {
            try
            {
                RoleService roleService = new RoleService();
                return Success(roleService.GetRoleById(id));
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult InsertRole(S201 model) {
            try {
                if (string.IsNullOrEmpty(model.RoleName)) throw new Exception("Role name cannot be left blank");
                using (var connection = BaseService.Connect()) {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction()) {

                        S200 userAdmin = HelperProvider.GetUserFromRequest(Request,connection,transaction);
                        RoleService roleService = new RoleService(connection);
                        S201 rolenameExist = roleService.GetRoleByName(model.RoleName,transaction);
                        if (rolenameExist != null) throw new Exception("Role name is not available");

                        model.ID = Guid.NewGuid().ToString();
                        model.CreatedBy = userAdmin.ID;
                        model.Status = S201.EnumStatus.ENABLED;
                        if (!roleService.InsertRole(model, transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);

                        LogProvider.InsertLog("Thêm nhóm quyền "+ model.RoleName, userAdmin,connection,transaction);

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
        public JsonResult UpdateRole(S201 model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.RoleName)) throw new Exception("Role name cannot be left blank");
                using (var connection = BaseService.Connect())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        S200 userAdmin = HelperProvider.GetUserFromRequest(Request, connection, transaction);
                        RoleService roleService = new RoleService(connection);
                        S201 role = roleService.GetRoleById(model.ID, transaction);
                        if (role == null) throw new Exception(JsonResult.Message.DATA_NOT_FOUND);
                        if (roleService.CheckRoleSameExist(model, transaction)) throw new Exception("Role name is not available");

                        role.ModifiedBy = userAdmin.ID;
                        role.RoleName = model.RoleName;
                        if (!roleService.UpdateRole(role, transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);

                        LogProvider.InsertLog("Cập nhật nhóm quyền " + role.RoleName, userAdmin, connection, transaction);

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
        public JsonResult DeleteRole(string id) {
            try
            {
                using (var connection = BaseService.Connect()) {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction()) {
                        S200 userAdmin = HelperProvider.GetUserFromRequest(Request,connection,transaction);

                        RoleService roleService = new RoleService(connection);
                        S201 role = roleService.GetRoleById(id, transaction);
                        if (role == null) throw new Exception(JsonResult.Message.DATA_NOT_FOUND);

                        role.Status = S201.EnumStatus.DISABLED;
                        role.ModifiedBy = userAdmin.ID;
                        if (!roleService.UpdateRole(role, transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);

                        LogProvider.InsertLog("Xóa nhóm quyền " + role.RoleName , userAdmin,connection,transaction);

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
