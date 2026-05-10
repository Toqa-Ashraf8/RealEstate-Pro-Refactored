using Dapper;
using System.Data;
using System.Data.Common;
using WebApp1.Core.Models;



public abstract class BaseRepository
    {
        protected readonly DbConnection _db;

        protected BaseRepository(DbConnection db)
        {
            _db = db;
        }

        protected async Task<IEnumerable<T>> GetAll<T>(string query, object? parameters = null)
        {
            return await _db.QueryAsync<T>(query, parameters);
        }
       protected async Task AddNewNegotiationPhase(Rejected_negotiations_phase entity,IDbTransaction transaction=null)
       {
         string sqlInsert = @"INSERT INTO Rejected_negotiations_phases 
                         (ClientID, ProjectCode, UnitID, NegotiationCondition, SuggestedPrice, ReasonOfReject, CheckedDate) 
                         VALUES (@ClientID, @ProjectCode, @UnitID, @NegotiationCondition, @SuggestedPrice, @ReasonOfReject, @CheckedDate)";

           await _db.ExecuteAsync(sqlInsert, entity);
    }
}


