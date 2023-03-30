using CleverApi.Models;
using CleverApi.Providers;
using CleverApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace CleverApi.ApiControllers
{
    public class UserController : ApiBaseController
    {
        [HttpPost]
        [AllowAnonymous]
        public JsonResult Login(UserLoginModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Account)) throw new Exception(MESSAGE.LOGIN.ACCOUNT_EMPTY);
                if (string.IsNullOrEmpty(model.Password)) throw new Exception(MESSAGE.LOGIN.PASSWORD_EMPTY);

                UserService userService = new UserService();
                S200 user = userService.GetUserAdminByAccount(model.Account);
                if (user == null) throw new Exception(MESSAGE.LOGIN.WRONG_PASSWORD_OR_ACCOUNT);
                model.Password = HelperProvider.EncodePassword(user.ID, model.Password);
                if (model.Password != user.PasswordHash) throw new Exception(MESSAGE.LOGIN.WRONG_PASSWORD_OR_ACCOUNT);
                if (user.Status == S200.EnumStatus.INACTIVE) throw new Exception(MESSAGE.LOGIN.USER_INACTIVE);
                string token = HelperProvider.CreateToken(user.ID, user.PasswordHash);

                return Success(new
                {
                    user.Avatar,
                    user.CreatedDate,
                    user.ID,
                    user.FirstName,
                    user.LastName,
                    user.UserName,
                    Token = token
                });
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetListUser(int? page = 1, int? rowPerPage = Constant.ADMIN_PAGE_SIZE)
        {
            try
            {
                UserService userService = new UserService();
                if (!page.HasValue) page = 1;
                if (!rowPerPage.HasValue) rowPerPage = Constant.ADMIN_PAGE_SIZE;
                return Success(userService.GetListUser(page.Value, rowPerPage.Value));
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetUser(string id)
        {
            try
            {
                UserService userService = new UserService();
                return Success(userService.GetUserById(id));
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
        [HttpPost]
        public JsonResult AddUser(UserUpdateModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.UserName)) throw new Exception(MESSAGE.USER.USERNAME_NOT_EMPTY);
                if (string.IsNullOrEmpty(model.FirstName)) throw new Exception(MESSAGE.USER.FIRSTNAME_NOT_EMPTY);
                if (string.IsNullOrEmpty(model.LastName)) throw new Exception(MESSAGE.USER.LASTNAME_NOT_EMPTY);
                if (string.IsNullOrEmpty(model.RoleID)) throw new Exception(MESSAGE.USER.ROLE_NOT_EMPTY);
                if (string.IsNullOrEmpty(model.PasswordHash)) throw new Exception(MESSAGE.USER.PASSWORD_NOT_EMPTY);
                if (model.PasswordHash.Length < 8) throw new Exception(MESSAGE.USER.PASSWORD_LENGTH_INVALID);

                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        S200 userAdmin = HelperProvider.GetUserFromRequest(Request, connect, transaction);
                        if (userAdmin == null) return Unauthorized();

                        UserService userService = new UserService(connect);
                        if (userService.IsUsernameExist(model.UserName, transaction)) throw new Exception(MESSAGE.USER.USERNAME_NOT_AVAILABLE);

                        S200 user = new S200();
                        user.UserName = model.UserName;
                        user.RoleID = model.RoleID;
                        user.LastName = model.LastName;
                        user.FirstName = model.FirstName;
                        user.ID = Guid.NewGuid().ToString();
                        user.UserType = S200.EnumUserType.SYSTEM_USER;
                        user.Status = S200.EnumStatus.ACTIVE;
                        user.CreatedBy = userAdmin.ID;
                        user.PasswordHash = HelperProvider.EncodePassword(user.ID, model.PasswordHash);
                        if (!string.IsNullOrEmpty(model.Avatar))
                        {
                            string fileName = Guid.NewGuid().ToString() + ".png";
                            string path = HttpContext.Current.Server.MapPath(Constant.AVATAR_USER_PATH + model.ID + "/" + fileName);

                            HelperProvider.Base64ToImage(model.Avatar, path);
                            user.Avatar = Constant.AVATAR_USER_URL + model.ID + "/" + fileName;
                        }
                        if (!userService.InsertUser(user, transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);

                        LogProvider.InsertLog("Thêm người dùng \"" + model.UserName + "\"", userAdmin, connect, transaction);

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
        public JsonResult UpdateUser(UserUpdateModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.FirstName)) throw new Exception(MESSAGE.USER.FIRSTNAME_NOT_EMPTY);
                if (string.IsNullOrEmpty(model.LastName)) throw new Exception(MESSAGE.USER.LASTNAME_NOT_EMPTY);
                if (string.IsNullOrEmpty(model.RoleID)) throw new Exception(MESSAGE.USER.ROLE_NOT_EMPTY);

                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        S200 userAdmin = HelperProvider.GetUserFromRequest(Request, connect, transaction);
                        if (userAdmin == null) return Unauthorized();

                        UserService userService = new UserService(connect);
                        S200 user = userService.GetUserById(model.ID, transaction);
                        S200 userAccountCheck = userService.GetUserByAccount(model.UserName, transaction);
                        if (userAccountCheck != null && user.ID != userAccountCheck.ID) throw new Exception(MESSAGE.USER.USERNAME_NOT_AVAILABLE);
                        user.UserName = model.UserName;
                        user.FirstName = model.FirstName;
                        user.LastName = model.LastName;
                        user.ModifiedBy = userAdmin.ID;
                        if (!string.IsNullOrEmpty(model.PasswordHash))
                        {
                            user.PasswordHash = HelperProvider.EncodePassword(user.ID, model.PasswordHash);
                        }
                        user.RoleID = model.RoleID;

                        if (!string.IsNullOrEmpty(model.Avatar))
                        {
                            if (!string.IsNullOrEmpty(user.Avatar))
                            {
                                HelperProvider.DeleteFile(user.Avatar);
                            }

                            string fileName = Guid.NewGuid().ToString() + ".png";
                            string path = HttpContext.Current.Server.MapPath(Constant.AVATAR_USER_PATH + user.ID + "/" + fileName);

                            HelperProvider.Base64ToImage(model.Avatar, path);
                            user.Avatar = Constant.AVATAR_USER_URL + model.ID + "/" + fileName;
                        }
                        if (!userService.UpdateUser(user, transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
                        LogProvider.InsertLog("Cập nhật người dùng \"" + user.UserName + "\"", userAdmin, connect, transaction);
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
        public JsonResult DeleteUser(string id)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        S200 userAdmin = HelperProvider.GetUserFromRequest(Request, connect, transaction);
                        if (userAdmin == null) return Unauthorized();

                        UserService userService = new UserService(connect);
                        LogsService logsService = new LogsService(connect);
                        S200 user = userService.GetUserById(id, transaction);
                        if (user == null) throw new Exception(MESSAGE.USER.CANNOT_FOUND_INFORMATION);

                        if (!userService.DeleteUser(user.ID, transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);

                        LogProvider.InsertLog("Delete user \"" + user.UserName + "\"", userAdmin, connect, transaction);

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