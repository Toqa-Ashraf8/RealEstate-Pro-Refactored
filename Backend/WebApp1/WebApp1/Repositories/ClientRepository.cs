using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using WebApp1.Core.Interfaces;
using WebApp1.Core.Models;


namespace WebApp1.Repositories
{
    public class ClientRepository :BaseRepository, IClientRepository
    {
        
        public ClientRepository(DbConnection db) : base(db){}
        
        public async Task<IEnumerable<Project>> GetAllProjects()
        {
            return await GetAll<Project>("select ProjectCode,ProjectName from Projects");
        }
        public async Task<IEnumerable<Unit>> GetUnitsByProject(int projectid)
        {
            return await GetAll<Unit>(@"select * from vw_Project_Available_Units 
                            where ProjectCode = @ProjectCode AND ReservedStatus = 0", 
                            new { 
                                ProjectCode= projectid 
                            });
        }
        public async Task<IEnumerable<Unit>> GetUnitPrice(int unitid)
        {
            return await GetAll<Unit>("select * from Units where UnitID =@UnitID", new { UnitID=unitid });
        }

        public async Task<(int id, bool saved, bool updated)> UpsertClient(Client cl)
        {
            bool updated = false;
            bool saved = false;
            int id = Convert.ToInt32(cl.ClientID);

            await _db.OpenAsync();
            using var transaction = _db.BeginTransaction();

            try
            {
                if (id == 0)
                {

                    string sqlin = @"insert into Clients (ClientName,PhoneNumber, ClientStatus,Notes) 
                                           values(@ClientName,@PhoneNumber,@ClientStatus,@Notes);
                                           SELECT CAST (SCOPE_IDENTITY() as int);";
                    id = await _db.ExecuteScalarAsync<int>(sqlin, cl, transaction);
                    saved = true;
                }
                else
                {
                    string sqlupdate = @"update Clients set ClientName=@ClientName,PhoneNumber=@PhoneNumber,
                                               ClientStatus=@ClientStatus,Notes=@Notes where ClientID=@ClientID";
                    await _db.ExecuteAsync(sqlupdate, cl, transaction);
                    updated = true;
                }
               
                await _db.ExecuteAsync("delete Negotiations where ClientID=@ClientID", new { ClientID= id }, transaction);
                if (cl.negotiations.Count > 0)
                {
                    string insertDetails = @"insert into Negotiations 
                                                    (serialCode,ClientID,ClientName,ProjectCode,ProjectName,
                                                    UnitID,unitName,OriginalPrice,NegotiationPrice, DiscountAmount
                                                   ,NegotiationStatus,NegotiationDate,checkedByAdmin,Requester,Reserved)
                                                    values(@serialCode,@ClientID,@ClientName,@ProjectCode,@ProjectName,@UnitID
                                                   ,@unitName,@OriginalPrice,@NegotiationPrice,@DiscountAmount
                                                   ,@NegotiationStatus,@NegotiationDate,@checkedByAdmin,@Requester,@Reserved)
                                                    SELECT CAST (SCOPE_IDENTITY() as int)";
                    cl.negotiations.ForEach(n => n.ClientID = id);
                    cl.negotiations.ForEach(n => n.ClientName = cl.ClientName);
                    await _db.ExecuteAsync(insertDetails, cl.negotiations, transaction);
                }                
                await transaction.CommitAsync();
                return (id, saved, updated);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("Database Error: " + ex.Message);
            }
        }

        public async Task<bool> DeleteClient(int id)
        {
            await _db.OpenAsync();
            using var transaction = _db.BeginTransaction();
            try
            {
                if (id > 0)
                {
                    var parm = new { ClientID = id };
                    await _db.ExecuteAsync("delete Negotiations where ClientID=@ClientID", parm, transaction);
                    await _db.ExecuteAsync("delete Clients where ClientID=@ClientID", parm, transaction);
                }
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }

        }

        public async Task<IEnumerable<Client>> GetAllClients()
        {
            return await GetAll<Client>("select * from Clients");
        }

        public async Task<IEnumerable<Negotiation>> GetClientNegotiations(int clientid)
        {
            return await GetAll<Negotiation>("select * from Negotiations where ClientID=@ClientID", new { ClientID= clientid });
        }

        public async Task<(Client? client, IEnumerable<Negotiation> negotiations, bool isnull)> GetFirstClient()
        {
            string sql = @"SELECT TOP(1) * FROM Clients ORDER BY ClientID ASC;
                   SELECT * FROM Negotiations WHERE ClientID = (SELECT TOP(1) ClientID FROM Clients ORDER BY ClientID ASC)";
            using var multi = await _db.QueryMultipleAsync(sql);

            var client = await multi.ReadFirstOrDefaultAsync<Client>();

            var negotiations = await multi.ReadAsync<Negotiation>();

            bool isnull = (client == null);

            return (client, negotiations, isnull);
        }

        public async Task<(Client? client, IEnumerable<Negotiation> negotiations_l, bool isnull)> GetLastClient()
        {
            string sql = @"
                SELECT TOP(1) * FROM Clients ORDER BY ClientID DESC;
                SELECT * FROM Negotiations WHERE ClientID = (SELECT TOP(1) ClientID FROM Clients ORDER BY ClientID DESC)";

            using var multi = await _db.QueryMultipleAsync(sql);

            var client = await multi.ReadFirstOrDefaultAsync<Client>();
            var negotiations_l = await multi.ReadAsync<Negotiation>();

            bool isnull = (client == null);

            return (client, negotiations_l, isnull);

        }
        public async Task<(Client? client, IEnumerable<Negotiation> negotiations, bool isLast, bool isEmpty)> GetNextClient(int currentId)
        {
            string sqlNext = "SELECT TOP(1) * FROM Clients WHERE ClientID > @id ORDER BY ClientID ASC";
            var client = await _db.QueryFirstOrDefaultAsync<Client>(sqlNext, new { id = currentId });

            bool isLast = false;
            if (client == null) 
            {
                isLast = true;
                client = await _db.QueryFirstOrDefaultAsync<Client>("SELECT * FROM Clients WHERE ClientID = @id", new { id = currentId });
            }

            if (client == null) return (null, null, false, true); 

            var negotiations = await _db.QueryAsync<Negotiation>("SELECT * FROM Negotiations WHERE ClientID = @id", new { id = client.ClientID });
            return (client, negotiations, isLast, false);
        }

        public async Task<(Client? client,IEnumerable<Negotiation> negotiations, bool isFirst, bool isEmpty)> GetPreviousClient(int currentId)
        {
           
            bool isFirst = false;
            string sqlPrev = "SELECT TOP(1) * FROM Clients WHERE ClientID < @id ORDER BY ClientID DESC";
            var client=await _db.QueryFirstOrDefaultAsync<Client>(sqlPrev, new { id = currentId });

            if(client== null)
            {
                isFirst = false;
                client = await _db.QueryFirstOrDefaultAsync<Client>("SELECT * FROM Clients WHERE ClientID = @id", new { id = currentId });
            }
           if(client == null) return (null, null, false, true); 

            var negotiations = await _db.QueryAsync<Negotiation>("SELECT * FROM Negotiations WHERE ClientID = @id", new { id = client.ClientID });
            
            return (client, negotiations, isFirst, false);


        }


    }
}
