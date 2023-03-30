using System;
using System.Collections.Generic;

namespace CleverApi.Models
{
    public class C500
    {
        public string ID { get; set; }
        public string Question { get; set; }
        public int Type { get; set; }
        public int Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }

        public class EnumStatus
        {
            public const int ENABLED = 1;
            public const int DISABLED = 0;
        }

        public class EnumType
        {
            public const int ONE_ANSWER = 0;
            public const int MULTIPLE_ANSWER = 1;
        }
    }

    public class BusinessAccountQuestionUpdateModel
    {
        public string ID { get; set; }
        public string Question { get; set; }
        public int Type { get; set; }
        public List<C501> ListAnswer { get; set; }
        public List<string> ListAnswerDelete { get; set; }
    }
}
