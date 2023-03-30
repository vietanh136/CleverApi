using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CleverApi.Models
{
    public class EmailTemplate
    {
        public string EmailAddress { get; set; }
        public string RegistrationConfirmationEmailTemplateTitle { get; set; }
        public string RegistrationConfirmationEmailTemplateContent { get; set; }
        public string ForgotPasswordEmailTemplateTitle{get;set;}
        public string ForgotPasswordEmailTemplateContent { get; set; }
    }
}