using System;

namespace CleverApi.Models
{
    public class C701
    {
        public string ID { get; set; }
        public string FolderID { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public int FileType { get; set; }
        public int Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public class EnumStatus
        {
            public const int ENABLED = 1;
            public const int DISABLED = 0;
        }
        public class EnumFileType
        {
            public const int WORD = 1;
            public const int EXCEL = 2;
            public const int POWERPOINT = 3;
            public const int PDF = 4;
            public const int IMAGE = 5;
        }
    }

    public class FileUploadModel
    {
        public string FolderID { get; set; }
        public string FileName { get; set; }
        public int FileType { get; set; }
        public string FileExt { get; set; }
        public string FileData { get; set; }
    }
}
