using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CleverApi.Models
{
    public class JsonResult
    {
        public string status { get; set; }
        public object data { get; set; }
        public string message { get; set; }
        public override string ToString()
        {
            string content = "{\"status\":\"" + this.status + "\",\"data\":null ,\"message\":\"" + this.message + "\"  }";
            return content;
        }
        public static class Status
        {
            public const string SUCCESS = "success";
            public const string ERROR = "error";
            public const string UNAUTHORIZED = "unauthorized";
            public const string UNAUTHENTICATED = "unauthenticated";
        }
        public static class Message
        {
            public const string ERROR_SYSTEM = "System get error while processing.";
            public const string TOKEN_EXPIRED = "Your token has expired.";
            public const string NO_PERMISSION = "You has no permission.";
            public const string UPDATED_SUCCESS = "Updated successfully.";
            public const string DELETED_SUCCESS = "Deleted successfully.";
            public const string DATA_NOT_FOUND = "Data not found";
        }
    }

    public class MESSAGE {
        public class LOGIN {
            public const string ACCOUNT_EMPTY = "User name cannot be left blank.";
            public const string PASSWORD_EMPTY = "Password cannot be left blank.";
            public const string USER_INACTIVE = "Your account is inactive.";
            public const string WRONG_PASSWORD_OR_ACCOUNT = "Account or password wrong.";
        }

        public class USER {
            public const string CANNOT_FOUND_INFORMATION = "Cannot found user infomation.";
            public const string USERNAME_NOT_EMPTY = "Username cannot be left blank";
            public const string FIRSTNAME_NOT_EMPTY = "First name cannot be left blank";
            public const string LASTNAME_NOT_EMPTY = "Last name cannot be left blank";
            public const string FULLNAME_NOT_EMPTY = "Fullname cannot be left blank";
            public const string ROLE_NOT_EMPTY = "Role cannot be left blank";
            public const string PASSWORD_NOT_EMPTY = "Password cannot be left blank";
            public const string USERNAME_NOT_AVAILABLE = "Username is not available";
            public const string PASSWORD_LENGTH_INVALID = "Password must be at least 8 characters";
        }
        
    }

    public class Constant
    {
        public const int PAGE_NEWS_SIZE = 6;
        public const int PAGE_SIZE = 10;
        public const int ADMIN_PAGE_SIZE = 15;
        public const int USER_PAGE_SIZE = 20;

        public const string AVATAR_USER_URL = "/files/avatar/";
        public const string AVATAR_USER_PATH = "~/files/avatar/";

        public const string SERVICES_THUMBNAIL_URL = "/files/services/";
        public const string SERVICES_THUMBNAIL_PATH = "~/files/services/";

        public const string FAQ_THUMBNAIL_URL = "/files/faq/";
        public const string FAQ_THUMBNAIL_PATH = "~/files/faq/";

        public const string DATA_ROOM_URL = "/files/dataroom/";
        public const string DATA_ROOM_PATH = "~/files/dataroom/";

        public const string ENQUIRY_FILE_URL = "/files/enquiry/";
        public const string MAIL_ROOM_URL = "/files/mailroom/";
    }
}