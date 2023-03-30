using CleverApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http.Cors;
using System.Net.Http;
using System.Web.Http;
using CleverApi.Filters;
namespace CleverApi.ApiControllers
{
    [ApiTokenRequire]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ApiBaseController : ApiController
    {
        [HttpGet]
        [AllowAnonymous]
        public JsonResult Success(object data = null, string message = "")
        {
            return new JsonResult { status = JsonResult.Status.SUCCESS, data = data, message = message };
        }


        [HttpGet]
        [AllowAnonymous]
        public JsonResult Error(string message = "")
        {
            return new JsonResult { status = JsonResult.Status.ERROR, data = null, message = message };
        }


        [HttpGet]
        [AllowAnonymous]
        public JsonResult Unauthorized()
        {
            return new JsonResult { status = JsonResult.Status.UNAUTHORIZED, data = null, message = JsonResult.Message.NO_PERMISSION };
        }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult Unauthenticated()
        {
            return new JsonResult { status = JsonResult.Status.UNAUTHENTICATED, data = null, message = JsonResult.Message.TOKEN_EXPIRED };
        }

    }
}
