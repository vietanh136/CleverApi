using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dapper;
using CleverApi.Models;
using System.Data;
namespace CleverApi.Services
{
    public class SystemContentService : BaseService
    {
        public SystemContentService() : base() { }
        public SystemContentService(IDbConnection db) : base(db) { }
        public string GetBusinessAccountTermsConditions(IDbTransaction transaction = null) {
            string query = "SELECT top 1 Term1 FROM S100";
            return this._connection.Query<string>(query, null, transaction).FirstOrDefault();
        }
        public string GetTermsConditions(IDbTransaction transaction = null)
        {
            string query = "SELECT top 1 Term2 FROM S100";
            return this._connection.Query<string>(query, null, transaction).FirstOrDefault();
        }
        public string GetDataProtectionPrivacyPolicy(IDbTransaction transaction = null)
        {
            string query = "SELECT top 1 Term3 FROM S100";
            return this._connection.Query<string>(query, null, transaction).FirstOrDefault();
        }

        public bool UpdateBusinessAccountTermsConditions(string content, IDbTransaction transaction = null)
        {
            string query = "update S100 set Term1 = @content";
            return this._connection.Execute(query, new { content }, transaction) > 0;
        }
        public bool UpdateTermsConditions(string content, IDbTransaction transaction = null)
        {
            string query = "update S100 set Term2 = @content";
            return this._connection.Execute(query, new { content }, transaction) > 0;
        }
        public bool UpdateDataProtectionPrivacyPolicy(string content, IDbTransaction transaction = null)
        {
            string query = "update S100 set Term3 = @content";
            return this._connection.Execute(query, new { content }, transaction) > 0;
        }



    }
}