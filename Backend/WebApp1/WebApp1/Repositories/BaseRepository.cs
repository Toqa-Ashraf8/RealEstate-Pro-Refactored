using Dapper;
using System.Data.Common;


    
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


