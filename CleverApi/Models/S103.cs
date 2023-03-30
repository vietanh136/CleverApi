using System;

namespace CleverApi.Models
{
    public class S103
    {
        public string ID { get; set; }
        public int Type { get; set; }
        public string DocumentName { get; set; }
        public int Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public class EnumStatus
        {
            public const int DISABLED = 0;
            public const int ENABLED = 1;
        }

        public class EnumType {
            public const int INDIVIDUAL_SHAREHOLDER = 2;
            public const int CORPORATE_SHAREHOLDER = 3;
        }
    }
}
