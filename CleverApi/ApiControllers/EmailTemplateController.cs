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
    public class EmailTemplateController : ApiBaseController
    {
        [HttpGet]
        public JsonResult GetEmailTemplate()
        {
            try
            {
                EmailTemplateService emailTemplateService = new EmailTemplateService();

                return Success(new
                {
                    emailAddress = emailTemplateService.GetEmailAddress(),
                    registrationConfirmationEmailTemplate = emailTemplateService.GetRegistrationConfirmationEmailTemplate(),
                    forgotPasswordEmailTemplate = emailTemplateService.GetForgotPasswordEmailTemplate()
                });
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult UpdateEmailTemplate(EmailTemplate model) {
            try {
                if (string.IsNullOrEmpty(model.EmailAddress)) throw new Exception("Email address cannot be left blank");
                if (string.IsNullOrEmpty(model.RegistrationConfirmationEmailTemplateTitle)) throw new Exception("Registration Confirmation Email Template title cannot be left blank");
                if (string.IsNullOrEmpty(model.RegistrationConfirmationEmailTemplateContent)) throw new Exception("Registration Confirmation Email Template content cannot be left blank");
                if (string.IsNullOrEmpty(model.ForgotPasswordEmailTemplateTitle)) throw new Exception("Forgot Password Email Template title cannot be left blank");
                if (string.IsNullOrEmpty(model.ForgotPasswordEmailTemplateContent)) throw new Exception("Forgot Password Email Template content cannot be left blank");

                HelperProvider.ValidEmailFormat(model.EmailAddress);

                using (var connection = BaseService.Connect()) {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction()) {
                        S200 userAdmin = HelperProvider.GetUserFromRequest(Request,connection,transaction);
                        EmailTemplateService emailTemplateService = new EmailTemplateService(connection);

                        if (!emailTemplateService.UpdateEmailAddress(model.EmailAddress, transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);

                        if (!emailTemplateService.UpdateRegistrationConfirmationEmailTemplate(
                            model.RegistrationConfirmationEmailTemplateTitle,
                            model.RegistrationConfirmationEmailTemplateContent,
                            transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);

                        if (!emailTemplateService.UpdateForgotPasswordEmailTemplate(
                            model.ForgotPasswordEmailTemplateTitle,
                            model.ForgotPasswordEmailTemplateContent,
                            transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);

                        LogProvider.InsertLog("Chỉnh sửa email template",userAdmin,connection,transaction);
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
