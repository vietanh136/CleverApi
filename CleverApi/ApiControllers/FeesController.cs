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
    public class FeesController : ApiBaseController
    {
        [HttpGet]
        public JsonResult GetFees() {
            try {
                FeesService feesService = new FeesService();
                S100 fees = feesService.GetFees();
                if (fees == null) {
                    fees = new S100();
                    fees.Amount1 = 0;
                    fees.Amount2 = 0;
                    fees.Amount3 = 0;
                    fees.Amount4 = 0;
                    fees.Amount5 = 0;
                    fees.Amount6 = 0;
                    fees.Amount7 = 0;
                    if (!feesService.InitFees(fees)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
                }

                return Success(fees);
            }
            catch (Exception ex) { return Error(ex.Message); }
        }


        [HttpPost]
        public JsonResult UpdateFees(S100 model) {
            try {
                using (var connection = BaseService.Connect()) {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction()) {
                        S200 userAdmin = HelperProvider.GetUserFromRequest(Request,connection,transaction);
                        FeesService feesService = new FeesService(connection);
                        S100 fees = feesService.GetFees(transaction);
                        if (fees == null) {
                            fees = new S100();
                            fees.Amount1 = 0;
                            fees.Amount2 = 0;
                            fees.Amount3 = 0;
                            fees.Amount4 = 0;
                            fees.Amount5 = 0;
                            fees.Amount6 = 0;
                            fees.Amount7 = 0;
                            if (!feesService.InitFees(fees,transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
                        }

                        fees.Amount1 = model.Amount1;
                        fees.Amount2 = model.Amount2;
                        fees.Amount3 = model.Amount3;
                        fees.Amount4 = model.Amount4;
                        fees.Amount5 = model.Amount5;
                        fees.Amount6 = model.Amount6;
                        fees.Amount7 = model.Amount7;
                        if (!feesService.UpdateFees(fees,transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);

                        LogProvider.InsertLog("Cập nhật các loại phí", userAdmin,connection,transaction);
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
