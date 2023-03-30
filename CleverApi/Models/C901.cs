using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CleverApi.Models
{
    public class C901
    {
        public string ID { get; set; }
        public string EnquiryID { get; set; }
        public string SentFrom { get; set; }
        public string SentTo { get; set; }
        public DateTime SentDate { get; set; }
        public string Content { get; set; }
        public bool IsCustomer { get; set; }
        public class EnumIsCustomer
        {
            public const bool CUSTOMER_SEND = false;
            public const bool ADMIN_SEND = true;
        }
    }

    public class ReplyModel
    {
        public string EnquiryID { get; set; }
        public string Content { get; set; }
        public List<C903> ListFile { get; set; }
    }
}