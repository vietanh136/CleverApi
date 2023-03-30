using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using CleverApi.Models;
using Dapper;
namespace CleverApi.Services
{
    public class EmailTemplateService : BaseService
    {
        public EmailTemplateService() : base() { }
        public EmailTemplateService(IDbConnection db) : base(db) { }

        public string GetEmailAddress(IDbTransaction transaction = null)
        {
            string query = "SELECT top 1 Email FROM S100";
            return this._connection.Query<string>(query, null, transaction).FirstOrDefault();
        }

        public object GetRegistrationConfirmationEmailTemplate(IDbTransaction transaction = null) {
            string query = "SELECT * FROM S104 WHERE ID = 1";
            return this._connection.Query<object>(query, null, transaction).FirstOrDefault();
        }

        public object GetForgotPasswordEmailTemplate(IDbTransaction transaction = null)
        {
            string query = "SELECT * FROM S104 WHERE ID = 2";
            return this._connection.Query<object>(query, null, transaction).FirstOrDefault();
        }


        public bool UpdateEmailAddress(string email, IDbTransaction transaction = null) {
            string query = "UPDATE S100 SET Email = @email";
            return this._connection.Execute(query,new { email},transaction) > 0;
        }

        public bool UpdateRegistrationConfirmationEmailTemplate(string title, string content, IDbTransaction transaction = null) {
            string query = "UPDATE S104 SET Title = @title, Description = @content WHERE ID = 1";
            return this._connection.Execute(query,new { title , content },transaction) > 0;
        }

        public bool UpdateForgotPasswordEmailTemplate(string title, string content, IDbTransaction transaction = null)
        {
            string query = "UPDATE S104 SET Title = @title, Description = @content WHERE ID = 2";
            return this._connection.Execute(query, new { title, content }, transaction) > 0;
        }
    }
}
