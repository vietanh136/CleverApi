using System;

namespace CleverApi.Models
{
    public class C200
    {
        public int ID { get; set; }
        public string ServiceName { get; set; }
        public string ServiceImage { get; set; }
        public int Status { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public class EnumStatus
        {
            public const int ENABLED = 1;
            public const int DISABLED = 0;
        }
    }
}
