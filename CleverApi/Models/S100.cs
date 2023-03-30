using System.Web.Mvc;

namespace CleverApi.Models
{
    public class S100
    {
        [AllowHtml]
        public string Term1 { get; set; }
        [AllowHtml]
        public string Term2 { get; set; }
        [AllowHtml]
        public string Term3 { get; set; }
        public string Email { get; set; }
        public int Amount1 { get; set; }
        public int Amount2 { get; set; }
        public int Amount3 { get; set; }
        public int Amount4 { get; set; }
        public int Amount5 { get; set; }
        public int Amount6 { get; set; }
        public int Amount7 { get; set; }
    }
}
