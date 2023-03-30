using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CleverApi.Models;
using CleverApi.Services;

namespace CleverApi.ApiControllers
{
    public class NotificationController : ApiBaseController
    {
        [HttpGet]
        public JsonResult GetListNotification(int page) {
            try {
                NotificationService notificationService = new NotificationService();
                return Success(notificationService.GetListNotification(page));
            }
            catch (Exception ex) {
                return Error(ex.Message);
            }
        }
    }
}
