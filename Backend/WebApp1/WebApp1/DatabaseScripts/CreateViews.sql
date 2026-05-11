
-- Negotiations_2 VIEW 
CREATE VIEW Negotiations_2  AS
SELECT n.*,  
    r.NegotiationCondition, 
    r.SuggestedPrice, 
    r.ReasonOfReject, 
    r.CheckedDate
FROM Negotiations n 
INNER JOIN Rejected_negotiations_phases r 
    ON n.ClientID = r.ClientID 
    AND n.ProjectCode = r.ProjectCode 
    AND n.UnitID = r.UnitID


--SELECT dbo.Negotiations.*, dbo.Rejected_negotiations_phases.NegotiationCondition, dbo.Rejected_negotiations_phases.SuggestedPrice, dbo.Rejected_negotiations_phases.ReasonOfReject, 
--                  dbo.Rejected_negotiations_phases.CheckedDate
--FROM     dbo.Negotiations INNER JOIN
--                  dbo.Rejected_negotiations_phases ON dbo.Negotiations.ClientID = dbo.Rejected_negotiations_phases.ClientID AND dbo.Negotiations.ProjectCode = dbo.Rejected_negotiations_phases.ProjectCode AND 
--                  dbo.Negotiations.UnitID = dbo.Rejected_negotiations_phases.UnitID