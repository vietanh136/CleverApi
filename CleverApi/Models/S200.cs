using System;

namespace CleverApi.Models
{
    public class S200
    {
        public string ID { get; set; }
        public string RoleID { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Avatar { get; set; }
        public string PasswordHash { get; set; }
        public int UserType { get; set; }
        public int Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }

        public class EnumStatus {
            public const int ACTIVE = 1;
            public const int INACTIVE = 0;
        }

        public class EnumUserType
        {
            public const int SYSTEM_USER = 0;
            public const int CUSTOMER = 1;
        }
    }
    public class UserUpdateModel {
        public string ID { get; set; }
        public string RoleID { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Avatar { get; set; }
        public string PasswordHash { get; set; }
    }
    public class UserLoginModel
    {
        public string Account { get; set; }
        public string Password { get; set; }
    }
}
