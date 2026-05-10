using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.Common;
using WebApp1.Core.Interfaces;
using WebApp1.Core.Models;

namespace WebApp1.Repositories
{
    public class NegotiationRepository :INegotiationRepository
    {
        private readonly DbConnection _db;
        public NegotiationRepository(DbConnection db)
        {
            _db = db;
        }
        public async Task <IEnumerable<int>> GetPendingNegotiationsCount()
        {
            string query = "SELECT COUNT(*) FROM Negotiations where checkedByAdmin=0";
            return await _db.QueryAsync<int>(query);
        }
        public async Task<IEnumerable<Negotiation>> GetPendingNegotiations()
        {
            string queryGet = "select * from Negotiations where checkedByAdmin=0";
            return await _db.QueryAsync<Negotiation>(queryGet);
        }

    }
}
