using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CleverApi.Models
{
    public class C400
    {
        public string ID { get; set; }
        public int CategoryID { get; set; }
        public string Requirement { get; set; }
        public int MonthPrice { get; set; }
        public int YearPrice { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
    }

    public class C401
    {
        public int ID { get; set; }
        public string Description { get; set; }
    }

    public class C402
    {
        public int ID { get; set; }
        public string Description { get; set; }
        public string ScopeOfWork { get; set; }
    }
}