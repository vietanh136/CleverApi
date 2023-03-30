using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;
using System.Web.Http;
using CleverApi.Models;
using CleverApi.Providers;
using CleverApi.Services;

namespace CleverApi.ApiControllers
{
    public class MailRoomController : ApiBaseController
    {
        [HttpGet]
        public JsonResult GetListMailRoom(int page)
        {
            try
            {
                MailRoomService mailRoomService = new MailRoomService();
                return Success(mailRoomService.GetListMailRoomViewModel(page));
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult SendMail(MailRoomSendModel model)
        {
            try
            {
                using (var connection = BaseService.Connect())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        S200 userAdmin = HelperProvider.GetUserFromRequest(Request, connection, transaction);

                        MailRoomService mailRoomService = new MailRoomService(connection);

                        C800 mailRoom = new C800();
                        mailRoom.ID = Guid.NewGuid().ToString();
                        mailRoom.SentBy = userAdmin.ID;
                        mailRoom.SentTo = model.SentTo;
                        mailRoom.Title = model.Title;
                        mailRoom.Status = C800.EnumStatus.UNREAD;
                        mailRoom.Email = model.Email;
                        if (!mailRoomService.InsertMailRoom(mailRoom, transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
                        List<Attachment> attachments = new List<Attachment>();
                        foreach (var attach in model.ListAttachment)
                        {
                            C801 attachMail = new C801();
                            attachMail.FileName = attach.FileName;
                            attachMail.FileType = attach.FileType;
                            attachMail.ID = Guid.NewGuid().ToString();
                            attachMail.MailID = mailRoom.ID;
                            if (!string.IsNullOrEmpty(attach.FilePath))
                            {
                                string fileName = Guid.NewGuid().ToString() + ".png";
                                string path = HttpContext.Current.Server.MapPath("~" + Constant.MAIL_ROOM_URL);

                                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                                int mod4 = attach.FilePath.Length % 4;
                                if (mod4 > 0)
                                {
                                    attach.FilePath += new string('=', 4 - mod4);
                                }

                                byte[] data = Convert.FromBase64String(attach.FilePath);

                                using (FileStream stream = System.IO.File.Create(path + fileName))
                                {
                                    stream.Write(data, 0, data.Length);
                                    stream.Position = 0;
                                    attachments.Add(new Attachment(stream, attach.FileName + "." + attach.FileExt, attach.FileMime));
                                }
                                attachMail.FilePath = path + fileName;
                            }

                            if (!mailRoomService.InsertMailRoomAttachment(attachMail, transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
                        }

                        LogProvider.InsertLog("Gửi mail cho khách hàng", userAdmin, connection, transaction);


                        //HelperProvider.SendOTPViaEmail("", "", attachments);
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
        public JsonResult DeleteMail(string id)
        {
            try
            {

                using (var connection = BaseService.Connect())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        S200 userAdmin = HelperProvider.GetUserFromRequest(Request, connection, transaction);

                        MailRoomService mailRoomService = new MailRoomService(connection);
                        C800 mailRoom = mailRoomService.GetMailRoom(id, transaction);
                        if (mailRoom == null) throw new Exception("Mail not found");
                        if (mailRoom.Status != C800.EnumStatus.UNREAD) throw new Exception("Cannot delete mail has been read");

                        if (!mailRoomService.DeleteMailRoom(mailRoom.ID, transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);

                        List<C801> listAttachment = mailRoomService.GetListMailAttachment(mailRoom.ID, transaction);
                        foreach (var attach in listAttachment)
                        {
                            HelperProvider.DeleteFile(attach.FilePath);
                        }

                        LogProvider.InsertLog("Xóa mail", userAdmin, connection, transaction);
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
