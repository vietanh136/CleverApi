using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CleverApi.Models
{
    public class C902
    {
        public string ID { get; set; }
        public string EnquiryID { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public int FileType { get; set; }
        public class EnumFileType {
            public const int WORD = 1;
            public const int EXCEL = 2;
            public const int POWERPOINT = 3;
            public const int PDF = 4;
            public const int IMAGE = 5;
        }
    }
}