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
    public class LocationController : ApiBaseController
    {
        [HttpGet]
        public JsonResult GetListProvince() {
            try {
                LocationService locationService = new LocationService();
                return Success(locationService.GetListProvince());
            }
            catch (Exception ex) {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetListDistrict()
        {
            try
            {
                LocationService locationService = new LocationService();
                return Success(locationService.GetListDistrict());
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetListWard()
        {
            try
            {
                LocationService locationService = new LocationService();
                return Success(locationService.GetListWard());
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
    }
}
