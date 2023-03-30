using CleverApi.Models;
using CleverApi.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CleverApi.Providers
{
    public class HelperProvider
    {
        public static string EncodePassword(string userId, string password)
        {
            try
            {
                userId = CreateMD5(userId);
                password = CreateMD5(userId[userId.Length - 1] + password + userId[0]);
                password = CreateMD5(userId[userId.Length - 2] + password + userId[1]);
                return password;
            }
            catch (Exception ex) { return null; }
        }
        public static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString().ToLower();
            }
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string CreateToken(string userId, string password)
        {
            try
            {
                string token = "";
                token = Guid.NewGuid().ToString();
                token = Base64Encode(token);

                //Mã Id
                token = token.Substring(0, 10) + "vkK9@3FH3*Y67!" + userId + "vkK9@3FH3*Y67!" + token.Substring(10, token.Length - 10);
                token = Base64Encode(token);

                //Mã hoá password
                token = token.Substring(0, 10) + "9aB67$$W07XoKd" + password + "9aB67$$W07XoKd" + token.Substring(10, token.Length - 10);
                token = Base64Encode(token);

                //Mã hóa thời gian
                token = token.Substring(0, 10) + "uEiq*8f@93GXj" + MakeCode() + "uEiq*8f@93GXj" + token.Substring(10, token.Length - 10);
                token = Base64Encode(token);


                //Ma hoa lan cuoi
                for (int i = 0; i < 3; i++)
                {
                    token = token + Guid.NewGuid().ToString();
                    token = Base64Encode(token);
                }

                token = "Cleverly " + token;
                return token;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static S200 DecodeToken(string token)
        {
            try
            {
                S200 customer = new S200();
                String[] tokenSplit;

                for (int i = 0; i < 3; i++)
                {
                    token = HelperProvider.Base64Decode(token);
                    token = token.Substring(0, token.Length - 36);
                }

                token = HelperProvider.Base64Decode(token);
                tokenSplit = token.Split(new[] { "uEiq*8f@93GXj" }, StringSplitOptions.None);

                string code = tokenSplit[1];
                token = tokenSplit[0] + tokenSplit[2];

                token = HelperProvider.Base64Decode(token);
                tokenSplit = token.Split(new[] { "9aB67$$W07XoKd" }, StringSplitOptions.None);
                customer.PasswordHash = tokenSplit[1];
                token = tokenSplit[0] + tokenSplit[2];

                token = HelperProvider.Base64Decode(token);
                tokenSplit = token.Split(new[] { "vkK9@3FH3*Y67!" }, StringSplitOptions.None);
                customer.ID = tokenSplit[1];

                return customer;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static void ValidEmailFormat(string email)
        {
            if (string.IsNullOrEmpty(email)) throw new Exception("Email cannot be null");
            string[] emailPart = email.Split('@');
            if (emailPart.Length < 2) throw new Exception("Invalid email");
            string[] emailPartAfter = emailPart[1].Split('.');
            if (emailPartAfter.Length < 2) throw new Exception("Invalid email");
        }

        public static S200 GetUserFromRequest(HttpRequestMessage request, IDbConnection connect = null, IDbTransaction transaction = null)
        {
            if (request.Headers.Authorization == null) return null;
            string token = request.Headers.Authorization.ToString();
            token = token.Replace("Cleverly ", "").Trim();
            UserService userService = new UserService(connect);
            S200 user = DecodeToken(token);

            S200 userFromDatabase = userService.GetUserById(user.ID, transaction);
            if (userFromDatabase == null) return null;
            if (userFromDatabase.PasswordHash != user.PasswordHash) return null;

            return userFromDatabase;
        }

        public static string MakeCode()
        {
            try
            {
                DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                ulong time = (ulong)(DateTime.UtcNow - epoch).TotalMilliseconds;

                char[] _base = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V' };
                char[] Code = new char[10];

                Code[0] = _base[(time >> 45) & 31];
                Code[1] = _base[(time >> 40) & 31];
                Code[2] = _base[(time >> 35) & 31];
                Code[3] = _base[(time >> 30) & 31];
                Code[4] = _base[(time >> 25) & 31];
                Code[5] = _base[(time >> 20) & 31];
                Code[6] = _base[(time >> 15) & 31];
                Code[7] = _base[(time >> 10) & 31];
                Code[8] = _base[(time >> 5) & 31];
                Code[9] = _base[time & 31];

                string ShareCode = new string(Code);

                return ShareCode;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public static void Base64ToImage(string base64String, string pathToSave)
        {
            try
            {
                List<string> listPartFolder = pathToSave.Split('\\').ToList();
                listPartFolder.RemoveAt(listPartFolder.Count - 1);
                string director = string.Join("\\", listPartFolder);
                if (!Directory.Exists(director)) Directory.CreateDirectory(director);
                // Convert base 64 string to byte[]
                byte[] imageBytes = Convert.FromBase64String(base64String);
                // Convert byte[] to Image
                using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
                {
                    Image image = Image.FromStream(ms, true);
                    Bitmap bmp = new Bitmap(image);

                    ImageCodecInfo imageCodecInfo = GetEncoder(ImageFormat.Png);
                    System.Drawing.Imaging.Encoder encoder = System.Drawing.Imaging.Encoder.Quality;
                    EncoderParameters encoderParameters = new EncoderParameters(1);
                    EncoderParameter encoderParameter = new EncoderParameter(encoder, 65);
                    encoderParameters.Param[0] = encoderParameter;

                    bmp.Save(pathToSave, imageCodecInfo, encoderParameters);
                }
            }
            catch (Exception ex) { throw ex; }
        }

        public static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (var codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }


        public static bool DeleteFile(string path)
        {
            try
            {
                //xoa anh cu
                if (System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath(path)))
                {
                    System.IO.File.Delete(System.Web.HttpContext.Current.Server.MapPath(path));
                }
                return true;
            }
            catch (Exception ex) { return false; }
        }
        public static string PrettyNumber(decimal number)
        {
            string strNumber = number.ToString("G29");
            if (strNumber.IndexOf('.') > -1) { strNumber = long.Parse(strNumber.Split('.')[0]).ToString("N0") + "." + strNumber.Split('.')[1]; }
            else { strNumber = number.ToString("N0"); }
            return strNumber;
        }
        public static string RemoveUnicode(string text)
        {
            string[] arr1 = new string[] { "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ",
    "đ",
    "é","è","ẻ","ẽ","ẹ","ê","ế","ề","ể","ễ","ệ",
    "í","ì","ỉ","ĩ","ị",
    "ó","ò","ỏ","õ","ọ","ô","ố","ồ","ổ","ỗ","ộ","ơ","ớ","ờ","ở","ỡ","ợ",
    "ú","ù","ủ","ũ","ụ","ư","ứ","ừ","ử","ữ","ự",
    "ý","ỳ","ỷ","ỹ","ỵ",};
            string[] arr2 = new string[] { "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a",
    "d",
    "e","e","e","e","e","e","e","e","e","e","e",
    "i","i","i","i","i",
    "o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o",
    "u","u","u","u","u","u","u","u","u","u","u",
    "y","y","y","y","y",};
            for (int i = 0; i < arr1.Length; i++)
            {
                text = text.Replace(arr1[i], arr2[i]);
                text = text.Replace(arr1[i].ToUpper(), arr2[i].ToUpper());
            }
            return text;
        }


        public static bool CheckFolderHasFile(string folderId, IDbConnection connection = null, IDbTransaction transaction = null)
        {
            DataRoomService dataRoomService = new DataRoomService(connection);
            List<C700> listFolder = dataRoomService.LoadCompanyFolderInFolder(folderId, transaction);
            List<C701> listFile = dataRoomService.LoadCompanyFileInFolder(folderId, transaction);
            if (listFile.Count > 0) return true;

            if (listFolder.Count > 0)
            {
                foreach (var folder in listFolder)
                {
                    return CheckFolderHasFile(folder.ID, connection, transaction);
                }
            }

            return false;
        }

        public static void DeleteFileDataRoom(string fileId, IDbConnection connection = null, IDbTransaction transaction = null)
        {
            DataRoomService dataRoomService = new DataRoomService(connection);
            C701 file = dataRoomService.GetFile(fileId, transaction);
            if (file == null) throw new Exception("File not found");

            if (File.Exists(HttpContext.Current.Server.MapPath("~" + file.FilePath)))
            {
                File.Delete(HttpContext.Current.Server.MapPath("~" + file.FilePath));
            }
            if (!dataRoomService.DeleteFile(file.ID, transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        public static void DeleteAllFileAndFolderInFolder(string folderId,  IDbConnection connection = null, IDbTransaction transaction = null)
        {
            DataRoomService dataRoomService = new DataRoomService(connection);
            List<C700> listSubFolder = dataRoomService.LoadCompanyFolderInFolder(folderId, transaction);
            dataRoomService.DeleteAllFileInFolder(folderId,transaction);
            if (listSubFolder.Count > 0)
            {
                dataRoomService.DeleteAllFolderInFolder(folderId, transaction);
                foreach (var folder in listSubFolder)
                {
                    DeleteAllFileAndFolderInFolder(folder.ID, connection,transaction);
                }
            }

        }

        public static async Task<bool> SendOTPViaEmail(string email, string code, List<Attachment> attachments = null)
        {
            try
            {
                SmtpClient clientDetails = new SmtpClient();
                clientDetails.Port = 587;
                clientDetails.Host = "mailer-0104.inet.vn";
                clientDetails.EnableSsl = true;
                clientDetails.DeliveryMethod = SmtpDeliveryMethod.Network;
                clientDetails.UseDefaultCredentials = false;
                clientDetails.Credentials = new NetworkCredential("devproteamvn@gmail.com", "123456!@#$%^Loc");
                clientDetails.Timeout = 300000;
                //Message Details
                MailMessage mailDetails = new MailMessage();
                mailDetails.From = new MailAddress("devproteamvn@gmail.com");
                mailDetails.To.Add(email);
                mailDetails.Subject = "[SwayVN] Mã xác thực";
                mailDetails.IsBodyHtml = false;
                mailDetails.Body = "Mã xác thực của bạn là " + code;
                if (attachments != null) {
                    if (attachments.Count > 0) {
                        foreach (var attachment in attachments) {
                            mailDetails.Attachments.Add(attachment);
                        }
                    }
                }
              

                clientDetails.Send(mailDetails);
                return true;
            }
            catch (Exception ex) { }
            return false;
        }
    }
}