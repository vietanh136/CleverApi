using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CleverApi.Services;
using CleverApi.Models;
using CleverApi.Providers;
using System.Web;
using System.IO;

namespace CleverApi.ApiControllers
{
    public class DataRoomController : ApiBaseController
    {
        [HttpGet]
        public JsonResult GetListCompanyDataRoom(int page)
        {
            try
            {
                DataRoomService dataRoomService = new DataRoomService();
                return Success(dataRoomService.GetListData(page));
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetListCompany()
        {
            try
            {
                CompanyService companyService = new CompanyService();
                return Success(companyService.GetListCompanyToCreateFolder());
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult DeleteCompanyDataRoom(string id)
        {
            try
            {
                using (var connection = BaseService.Connect())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        S200 userAdmin = HelperProvider.GetUserFromRequest(Request, connection, transaction);
                        DataRoomService dataRoomService = new DataRoomService(connection);
                        CompanyService companyService = new CompanyService(connection);
                        C700 dataRoom = dataRoomService.GetCompanyDataRoom(id, transaction);
                        if (dataRoom == null) throw new Exception(JsonResult.Message.DATA_NOT_FOUND);

                        T100 company = companyService.GetCompany(dataRoom.CompanyID, transaction);
                        if (company == null) throw new Exception(JsonResult.Message.DATA_NOT_FOUND);

                        if (!dataRoomService.DeleteData(company.ID, transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
                        LogProvider.InsertLog("Xóa data room của công ty " + company.CompanyName, userAdmin, connection, transaction);
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
        public JsonResult CreateCompanyFolder(C700 model)
        {
            try
            {

                if (string.IsNullOrEmpty(model.FolderName)) throw new Exception("Folder name cannot be left blank");
                if (string.IsNullOrEmpty(model.CompanyID)) throw new Exception("Please choose company");
                using (var connection = BaseService.Connect())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        S200 userAdmin = HelperProvider.GetUserFromRequest(Request, connection, transaction);

                        CompanyService companyService = new CompanyService(connection);
                        T100 company = companyService.GetCompany(model.CompanyID, transaction);
                        if (company == null) throw new Exception("Company not found");

                        string companyName = HelperProvider.RemoveUnicode(company.CompanyName.Replace(" ", ""));
                        string rootPath = Constant.DATA_ROOM_URL + companyName + "/" + model.FolderName;
                        if (!Directory.Exists(HttpContext.Current.Server.MapPath("~" + rootPath)))
                        {
                            Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~" + rootPath));
                        }
                        string originalFolderName = model.FolderName;
                        DataRoomService dataRoomService = new DataRoomService(connection);
                        model.ID = Guid.NewGuid().ToString();
                        model.Status = C700.EnumStatus.ENABLED;
                        model.CreatedBy = userAdmin.ID;
                        model.FolderName = rootPath;
                        if (!dataRoomService.CreateFolderCompany(model, transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
                        if (!companyService.UpdateCompanyoFolderCreatedStatus(company.ID, transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
                        LogProvider.InsertLog("Thêm folder cho công ty " + company.CompanyName, userAdmin, connection, transaction);
                        transaction.Commit();
                        return Success(new
                        {
                            DataRoomId = model.ID,
                            RootFolder = originalFolderName,
                            CompanyName = company.CompanyName
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }


        [HttpPost]
        public JsonResult CreateFolder(C700 model)
        {
            try
            {

                if (string.IsNullOrEmpty(model.FolderName)) throw new Exception("Folder name cannot be left blank");
                if (string.IsNullOrEmpty(model.CompanyID)) throw new Exception("Please choose company");
                using (var connection = BaseService.Connect())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        S200 userAdmin = HelperProvider.GetUserFromRequest(Request, connection, transaction);
                        DataRoomService dataRoomService = new DataRoomService(connection);
                        CompanyService companyService = new CompanyService(connection);
                        T100 company = companyService.GetCompany(model.CompanyID, transaction);
                        if (company == null) throw new Exception("Company not found");

                        string companyName = HelperProvider.RemoveUnicode(company.CompanyName.Replace(" ", ""));
                        string folderPath = model.FolderName;
                        C700 parentFolder = dataRoomService.GetCompanyDataRoom(model.ParentID, transaction);
                        while (parentFolder != null)
                        {
                            folderPath = parentFolder.FolderName + "/" + folderPath;
                            parentFolder = dataRoomService.GetCompanyDataRoom(parentFolder.ParentID, transaction);
                        }


                        if (!Directory.Exists(HttpContext.Current.Server.MapPath("~" + folderPath)))
                        {
                            Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~" + folderPath));
                        }

                        model.ID = Guid.NewGuid().ToString();
                        model.Status = C700.EnumStatus.ENABLED;
                        model.CreatedBy = userAdmin.ID;
                        if (!dataRoomService.CreateFolderCompany(model, transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
                        LogProvider.InsertLog("Thêm folder cho công ty " + company.CompanyName, userAdmin, connection, transaction);
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
        public JsonResult UploadFile(FileUploadModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.FileName)) throw new Exception("File name cannot be left blank");
                if (string.IsNullOrEmpty(model.FileData)) throw new Exception("Please choose file to upload");
                using (var connection = BaseService.Connect())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        S200 userAdmin = HelperProvider.GetUserFromRequest(Request, connection, transaction);

                        DataRoomService dataRoomService = new DataRoomService(connection);
                        CompanyService companyService = new CompanyService(connection);

                        C700 folder = dataRoomService.GetCompanyDataRoom(model.FolderID, transaction);
                        if (folder == null) throw new Exception("Folder not found");

                        T100 company = companyService.GetCompany(folder.CompanyID, transaction);
                        if (company == null) throw new Exception("Company not found");

                        string fullPathFolder = folder.FolderName;
                        if (!string.IsNullOrEmpty(folder.ParentID))
                        {
                            C700 parentFolder = dataRoomService.GetCompanyDataRoom(folder.ParentID, transaction);
                            while (parentFolder != null)
                            {
                                fullPathFolder = parentFolder.FolderName + "/" + fullPathFolder;
                                parentFolder = dataRoomService.GetCompanyDataRoom(parentFolder.ParentID, transaction);
                            }
                        }

                        C701 file = new C701();
                        file.ID = Guid.NewGuid().ToString();
                        file.CreatedBy = userAdmin.ID;
                        file.FileName = model.FileName;
                        file.FilePath = fullPathFolder + "/" + file.FileName + "." + model.FileExt;
                        file.FileType = model.FileType;
                        file.FolderID = model.FolderID;
                        file.Status = C701.EnumStatus.ENABLED;
                        if (!dataRoomService.CreateFile(file, transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);

                        LogProvider.InsertLog("Thêm file cho công ty " + company.CompanyName, userAdmin, connection, transaction);

                        int mod4 = model.FileData.Length % 4;

                        // as of my research this mod4 will be greater than 0 if the base 64 string is corrupted
                        if (mod4 > 0)
                        {
                            model.FileData += new string('=', 4 - mod4);
                        }

                        byte[] data = Convert.FromBase64String(model.FileData);

                        using (FileStream stream = System.IO.File.Create(HttpContext.Current.Server.MapPath("~" + file.FilePath)))
                        {
                            stream.Write(data, 0, data.Length);
                        }
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
        public JsonResult LoadListFolderAndFileInRoot(string companyId)
        {
            try
            {
                DataRoomService dataRoomService = new DataRoomService();

                C700 rootFolder = dataRoomService.GetRootFolder(companyId);


                return Success(new
                {
                    CurrentFolder = rootFolder,
                    ListFile = dataRoomService.LoadCompanyFileInFolder(rootFolder.ID),
                    ListFolder = dataRoomService.LoadCompanyFolderInFolder(rootFolder.ID)
                });
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult LoadListDataInFolder(string folderId)
        {
            try
            {
                DataRoomService dataRoomService = new DataRoomService();
                C700 folder = dataRoomService.GetCompanyDataRoom(folderId);
                return Success(new
                {
                    CurrentFolder = folder,
                    ListFile = dataRoomService.LoadCompanyFileInFolder(folder.ID),
                    ListFolder = dataRoomService.LoadCompanyFolderInFolder(folder.ID)
                });
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult CheckFolderHasChild(string id)
        {
            try
            {
                return Success(HelperProvider.CheckFolderHasFile(id));
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult DeleteFolder(string id) {
            try {
                using (var connection = BaseService.Connect()) {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction()) {
                        S200 userAdmin = HelperProvider.GetUserFromRequest(Request,connection,transaction);
                        
                        HelperProvider.DeleteAllFileAndFolderInFolder(id,connection,transaction);
                        DataRoomService dataRoomService = new DataRoomService(connection);
                        C700 folder = dataRoomService.GetFolder(id,transaction);
                        if (folder == null) throw new Exception(JsonResult.Message.ERROR_SYSTEM);

                        CompanyService companyService = new CompanyService(connection);
                        T100 company = companyService.GetCompany(folder.CompanyID,transaction);
                        if (company == null) throw new Exception("Company not found");

                        dataRoomService.DeleteFolder(folder.ID,transaction);

                        string folderPath = folder.FolderName;
                        C700 parentFolder = dataRoomService.GetCompanyDataRoom(folder.ParentID, transaction);
                        while (parentFolder != null)
                        {
                            folderPath = parentFolder.FolderName + "/" + folderPath;
                            parentFolder = dataRoomService.GetCompanyDataRoom(parentFolder.ParentID, transaction);
                        }

                        if (Directory.Exists(HttpContext.Current.Server.MapPath("~" + folderPath))) {
                            Directory.Delete(HttpContext.Current.Server.MapPath("~" + folderPath), true);
                        }

                        LogProvider.InsertLog($"Xóa folder {folder.FolderName} của công ty {company.CompanyName}", userAdmin,connection,transaction);
                        transaction.Commit();
                        return Success();
                    }
                }
            }
            catch (Exception ex) {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult DeleteFile(string id)
        {
            try
            {
                using (var connection = BaseService.Connect())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        S200 userAdmin = HelperProvider.GetUserFromRequest(Request, connection, transaction);

                        HelperProvider.DeleteAllFileAndFolderInFolder(id, connection, transaction);
                        DataRoomService dataRoomService = new DataRoomService(connection);
                        C701 file = dataRoomService.GetFile(id, transaction);
                        if (file == null) throw new Exception("File not found");

                        C700 folder = dataRoomService.GetFolder(file.FolderID,transaction);
                        if (folder == null) throw new Exception(JsonResult.Message.ERROR_SYSTEM);

                        CompanyService companyService = new CompanyService(connection);
                        T100 company = companyService.GetCompany(folder.CompanyID, transaction);
                        if (company == null) throw new Exception("Company not found");

                        dataRoomService.DeleteFile(file.ID, transaction);

                        if (File.Exists(HttpContext.Current.Server.MapPath("~" + file.FilePath)))
                        {
                            File.Delete(HttpContext.Current.Server.MapPath("~" + file.FilePath));
                        }
                        LogProvider.InsertLog($"Xóa file {file.FileName} của công ty {company.CompanyName}", userAdmin, connection, transaction);

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
