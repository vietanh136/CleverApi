using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CleverApi.Models
{
    public class C900
    {
        public string ID { get; set; }
        public string SentFrom { get; set; }
        public DateTime SentDate { get; set; }
        public int Rating { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public bool IsCustomer { get; set; }
        public bool IsAdmin { get; set; }
        public int Status { get; set; }
        public class EnumIsCustomer {
            public const int CUSTOMER_UNREAD = 0;
            public const int CUSTOMER_READ = 1;
        }
        public class EnumIsAdmin
        {
            public const int ADMIN_UNREAD = 0;
            public const int ADMIN_READ = 1;
        }
        public class EnumStatus
        {
            public const int PROCESSING = 0;
            public const int SOLVED = 1;
            public const int CANCEL = 2;
        }
    }
}