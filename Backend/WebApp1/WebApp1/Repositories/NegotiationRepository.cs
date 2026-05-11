using Dapper;
using System.Data.Common;
using WebApp1.Core.Interfaces;
using WebApp1.Core.Models;

namespace WebApp1.Repositories
{
    public class NegotiationRepository : BaseRepository, INegotiationRepository
    {
        public NegotiationRepository(DbConnection db) : base(db){}
        
        public async Task<int> GetPendingNegotiationsCount()
        {
            string query = "SELECT COUNT(*) FROM Negotiations where checkedByAdmin=0";
            return await _db.ExecuteScalarAsync<int>(query);
        }
        public async Task<IEnumerable<Negotiation>> GetPendingNegotiations()
        {
            string queryGet = "select * from Negotiations where checkedByAdmin=0";
            return await _db.QueryAsync<Negotiation>(queryGet);
        }
        public async Task<bool> ProcessNegotiationReview(Rejected_negotiations_phase phase)
        {

            if (phase.ClientID == 0) return false;
            await _db.OpenAsync();
            using (var transaction = _db.BeginTransaction())
            {
                try
                {
                    string sqlInsert = @"INSERT INTO Rejected_negotiations_phases 
                         (ClientID, ProjectCode, UnitID, NegotiationCondition, SuggestedPrice, ReasonOfReject, CheckedDate) 
                         VALUES (@ClientID, @ProjectCode, @UnitID, @NegotiationCondition, @SuggestedPrice, @ReasonOfReject, @CheckedDate)";
                    await _db.ExecuteAsync(sqlInsert,phase, transaction);
                    string status = Convert.ToBoolean(phase.NegotiationCondition) ? "مقبول" : "مرفوض";

                    string updateNegotiations = @"
                          UPDATE Negotiations 
                          SET NegotiationStatus = @status, checkedByAdmin = 1 
                          WHERE ClientID = @ClientID AND ProjectCode = @ProjectCode AND UnitID = @UnitID";

                    await _db.ExecuteAsync(updateNegotiations, new
                    {
                        status,
                        ClientID = phase.ClientID,
                        ProjectCode = phase.ProjectCode,
                        UnitID = phase.UnitID
                    }, transaction);

                    await transaction.CommitAsync();
                    return true;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    return false;
                }
            }
        }
        public async Task<(int count, IEnumerable<NegotiationsDetailsView> negotiations)> GetRejectedNegotiations()
        {

            string sqld = @"select * from Negotiations_2 where NegotiationCondition=0 AND 
                          checkedByAdmin=1 AND Reserved=0";

            var negotiations = await _db.QueryAsync<NegotiationsDetailsView>(sqld);
            return (negotiations.Count(), negotiations);
        }
        public async Task<(int count, IEnumerable<NegotiationsDetailsView> negotiations)> GetApprovedNegotiations()
        {
            string sqld = @"select * from Negotiations_2 where NegotiationCondition=1 AND 
                          checkedByAdmin=1 AND Reserved=0";
            var negotiations = await _db.QueryAsync<NegotiationsDetailsView>(sqld);
            return (negotiations.Count(), negotiations);
        }

        public async Task<(bool Re_Approved, bool Re_Rejected)> UpdateNegotiationReview(Rejected_negotiations_phase phase)
        {

            await _db.OpenAsync();
            using (var transaction = _db.BeginTransaction())
            {

                try
                {
                    string sqlUpdate = @"update Rejected_negotiations_phases set 
                               NegotiationCondition=@NegotiationCondition,
                               SuggestedPrice=@SuggestedPrice,
                               ReasonOfReject=@ReasonOfReject,
                               CheckedDate=@CheckedDate
                               where ClientID=@ClientID and ProjectCode=@ProjectCode and UnitID=@UnitID";
                    var affectedRows = await _db.ExecuteAsync(sqlUpdate, phase, transaction);
                    if (affectedRows == 0)
                    {
                        string sqlInsert = @"INSERT INTO Rejected_negotiations_phases 
                         (ClientID, ProjectCode, UnitID, NegotiationCondition, SuggestedPrice, ReasonOfReject, CheckedDate) 
                         VALUES (@ClientID, @ProjectCode, @UnitID, @NegotiationCondition, @SuggestedPrice, @ReasonOfReject, @CheckedDate)";
                        await _db.ExecuteAsync(sqlInsert, phase, transaction);
                    }
                    string status = Convert.ToBoolean(phase.NegotiationCondition) ? "مقبول" : "مرفوض";
                    string sqlUpdateNegotiation = @"update Negotiations set 
                                                   NegotiationStatus=@status
                                                   where ClientID=@ClientID and 
                                                   ProjectCode=@ProjectCode and 
                                                   UnitID=@UnitID";

                    await _db.ExecuteAsync(sqlUpdateNegotiation,
                        new
                        {
                            status,
                            ClientID = phase.ClientID,
                            ProjectCode = phase.ProjectCode,
                            UnitID = phase.UnitID
                        },
                        transaction);
                    await transaction.CommitAsync();
                    return (Re_Approved: Convert.ToBoolean(phase.NegotiationCondition), Re_Rejected: !Convert.ToBoolean(phase.NegotiationCondition));
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }

            }

        }

       
    }
}
