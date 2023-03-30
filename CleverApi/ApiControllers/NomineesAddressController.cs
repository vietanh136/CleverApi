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
    public class NomineesAddressController : ApiBaseController
    {
       

        [HttpGet]
        public JsonResult GetAddress()
        {
            try
            {
                NomineesAddressService nomineesAddressService = new NomineesAddressService();
                return Success(nomineesAddressService.GetAddress());
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult Update(S101 model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Address)) throw new Exception("Address cannot be left blank");
                if (string.IsNullOrEmpty(model.ProvinceID)) throw new Exception("Please choose province");
                if (string.IsNullOrEmpty(model.DistrictID)) throw new Exception("Please choose district");
                if (string.IsNullOrEmpty(model.WardID)) throw new Exception("Please choose ward");
                using (var connection = BaseService.Connect())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        S200 userAdmin = HelperProvider.GetUserFromRequest(Request, connection, transaction);
                        NomineesAddressService nomineesAddressService = new NomineesAddressService(connection);

                        if (!nomineesAddressService.Update(model, transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);

                        LogProvider.InsertLog("Cập nhật địa chỉ cho thuê", userAdmin, connection, transaction);

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
