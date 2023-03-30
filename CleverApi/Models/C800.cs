using System;
using System.Collections.Generic;

namespace CleverApi.Models
{
    public class C800
    {
        public string ID { get; set; }
        public string SentTo { get; set; }
        public string Email { get; set; }
        public DateTime SentDate { get; set; }
        public string SentBy { get; set; }
        public string Title { get; set; }
        public int Status { get; set; }
        public class EnumStatus {
            public const int UNREAD = 0;
            public const int READ = 1;
        }
    }

    public class MailRoomSendModel : C800
    {
        public List<FileAttachment> ListAttachment { get; set; }
    }
}
