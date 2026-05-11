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
   
}


