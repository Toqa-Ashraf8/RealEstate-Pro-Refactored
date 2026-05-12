
-- Negotiations_2 VIEW TO GET ALL NEGOTIATION REQUESTS (APPROVED _ REJECTED)
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

GO

--vw_Booked_Clients VIEW TO REVIEW BOOKED CLIENTS 


CREATE VIEW vw_Booked_Clients AS 
SELECT n.*,
u.BookingID,
u.BookingDate
FROM Negotiations n 
INNER JOIN UnitBooking u 
ON n.UnitID=u.UnitID 
AND n.ProjectCode=u.ProjectCode


GO
--vw_ClientExtraDetails

CREATE VIEW vw_ClientExtraDetails AS
SELECT c.*,
u.BookingID,
u.UnitID,
u.ProjectCode
FROM ClientExtraDetails c 
INNER JOIN UnitBooking u 
ON c.ClientID=u.ClientID






