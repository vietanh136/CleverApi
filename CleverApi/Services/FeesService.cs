using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Dapper;
using CleverApi.Models;

namespace CleverApi.Services
{
    public class FeesService : BaseService
    {
        public FeesService() : base() { }
        public FeesService(IDbConnection db) : base(db) { }

        public S100 GetFees(IDbTransaction transaction = null) {
            string query = "select top 1 [Amount1],[Amount2],[Amount3],[Amount4],[Amount5],[Amount6],[Amount7] from S100";
            return this._connection.Query<S100>(query, null, transaction).FirstOrDefault();
        }
        public bool InitFees(S100 model, IDbTransaction transaction = null) {
            string query = "INSERT INTO [dbo].[S100] ([Term1],[Term2],[Term3],[Email],[Amount1],[Amount2],[Amount3],[Amount4],[Amount5],[Amount6],[Amount7]) VALUES ('','','','',@Amount1,@Amount2,@Amount3,@Amount4,@Amount5,@Amount6,@Amount7)";
            return this._connection.Execute(query,model,transaction) > 0;
        }
        public bool UpdateFees(S100 fees, IDbTransaction transaction = null) {
            string query = "UPDATE [dbo].[S100] SET [Amount1]=@Amount1,[Amount2]=@Amount2,[Amount3]=@Amount3,[Amount4]=@Amount4,[Amount5]=@Amount5,[Amount6]=@Amount6,[Amount7]=@Amount7";
            return this._connection.Execute(query, fees, transaction) > 0;
        }
    }
}