using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using CleverApi.Models;
using CleverApi.Providers;
using CleverApi.Services;
namespace CleverApi.ApiControllers
{
    public class EnquiryController : ApiBaseController
    {
        [HttpGet]
        public JsonResult GetListEnquiry(int? page, int status,int? rowPerPage)
        {
            try
            {
                EnquiryService enquiryService = new EnquiryService();
                if (!page.HasValue) page = 1;
                if (!rowPerPage.HasValue) rowPerPage = Constant.ADMIN_PAGE_SIZE;
                return Success(enquiryService.GetListEnquiryView(page.Value, status, rowPerPage.Value));
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetEnquiry(string id)
        {
            try
            {
                EnquiryService enquiryService = new EnquiryService();

                var enquiry = enquiryService.GetEnquiryView(id);
                var listFile = enquiryService.GetListEnquiryFileView(id);

                return Success(new { enquiry, listFile });
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult GetListEnquiryReply(string enquiryId)
        {
            try
            {
                EnquiryService enquiryService = new EnquiryService();
                var listReply = enquiryService.GetListEnquiryReplyView(enquiryId);
                var listReplyFile = enquiryService.GetListEnquiryReplyFileView(enquiryId);
                return Success(new { listReply, listReplyFile });
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult SendReply(ReplyModel model)
        {
            try
            {
                using (var connection = BaseService.Connect())
                {
                    connection.Open();
                    using (var transcation = connection.BeginTransaction())
                    {
                        S200 userAdmin = HelperProvider.GetUserFromRequest(Request, connection, transcation);

                        EnquiryService enquiryService = new EnquiryService(connection);

                        C900 enquiry = enquiryService.GetEnquiry(model.EnquiryID, transcation);
                        if (enquiry == null) throw new Exception("Data not found");


                        C901 reply = new C901();
                        reply.Content = model.Content;
                        reply.EnquiryID = model.EnquiryID;
                        reply.ID = Guid.NewGuid().ToString();
                        reply.IsCustomer = C901.EnumIsCustomer.ADMIN_SEND;
                        reply.SentTo = enquiry.SentFrom;
                        if (!enquiryService.InsertEnquiryReply(reply, transcation)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);

                        foreach (var file in model.ListFile)
                        {
                            C903 replyFile = new C903();
                            replyFile.EnquiryReplyID = reply.ID;
                            replyFile.FileName = file.FileName;
                            if (string.IsNullOrEmpty(file.FilePath))
                            {
                                string fileName = Guid.NewGuid().ToString() + ".png";
                                string path = HttpContext.Current.Server.MapPath("~" + Constant.ENQUIRY_FILE_URL + enquiry + "/");
                                int mod4 = file.FilePath.Length % 4;

                                // as of my research this mod4 will be greater than 0 if the base 64 string is corrupted
                                if (mod4 > 0)
                                {
                                    file.FilePath += new string('=', 4 - mod4);
                                }

                                byte[] data = Convert.FromBase64String(file.FilePath);

                                using (FileStream stream = System.IO.File.Create(path + fileName))
                                {
                                    stream.Write(data, 0, data.Length);
                                }

                                replyFile.FilePath = Constant.ENQUIRY_FILE_URL + enquiry + "/" + fileName;

                            }

                            replyFile.FileType = file.FileType;
                            replyFile.ID = Guid.NewGuid().ToString();
                            if (!enquiryService.InsertEnquiryFileReply(replyFile, transcation)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
                        }
                        LogProvider.InsertLog("Gửi mail cho khách hàng " + reply.SentTo,userAdmin,connection,transcation);
                        transcation.Commit();
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
