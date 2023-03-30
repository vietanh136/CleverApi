using System;
using System.Collections.Generic;

namespace CleverApi.Models
{
    public class C600
    {
        public string ID { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public string FormFill { get; set; }
        public string ProcessTime { get; set; }
        public int Payment { get; set; }
        public string Icon { get; set; }
        public int Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public class EnumPayment {
            public const int FREE = 0;
            public const int CONDITION = 1;
        }
        public class EnumStatus
        {
            public const int ENABLED = 1;
            public const int DISABLED = 0;
        }
    }

    public class FAQUpdateModel {
        public string ID { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public string FormFill { get; set; }
        public string ProcessTime { get; set; }
        public int Payment { get; set; }
        public string Icon { get; set; }
        public int Status { get; set; }
        public List<C601> ListSteps { get; set; }
        public List<C602> ListWhatNeed { get; set; }
        public List<string> ListStepsDelete { get; set; }
        public List<string> ListWhatNeedDelete { get; set; }
    }
}
