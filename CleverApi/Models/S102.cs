using System;

namespace CleverApi.Models
{
    public class S102
    {
        public string ID { get; set; }
        public string DirectorName { get; set; }
        public int Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public class EnumStatus {
            public const int DISABLED = 0;
            public const int ENABLED = 1;
        }
    }
}
