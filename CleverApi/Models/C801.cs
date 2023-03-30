namespace CleverApi.Models
{
    public class C801
    {
        public string ID { get; set; }
        public string MailID { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }
    }

    public class FileAttachment : C801{ 
        public string FileMime { get; set; }
        public string FileExt { get; set; }
    }
}
