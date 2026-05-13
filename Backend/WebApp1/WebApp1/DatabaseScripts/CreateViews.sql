
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

GO

--vw_ClientFullDetails VIEW TO GET ALL CLIENT DETAILS IN ONE VIEW
CREATE VIEW vw_ClientFullDetails AS
SELECT c.*, p.PhoneNumber
FROM ClientExtraDetails c
INNER JOIN Clients p ON 
c.ClientID = p.ClientID

GO 

--vw_ClientUnitsBookings VIEW TO GET ALL BOOKINGS OF A CLIENT
CREATE VIEW vw_ClientUnitsBookings AS
SELECT 
u.BookingID, 
u.ReservationAmount,
u.PaymentMethod, 
u.CheckImagePath, 
u.DownPayment, 
u.FirstInstallmentDate, 
u.InstallmentYears, 
u.BookingDate, 
u.ClientID, 
u.ProjectCode, 
u.UnitID, 
u.Reserved, 
i.InstallmentID, 
i.InstallmentNumber, 
i.DueDate, 
i.MonthlyAmount, 
i.Paid, 
i.PaymentType, 
i.CheckImage, n.ClientName, n.ProjectName, n.unitName
FROM  UnitBooking AS u INNER JOIN
Installments AS i ON u.BookingID = i.BookingID INNER JOIN
Negotiations n ON u.ClientID =n.ClientID AND u.ProjectCode = n.ProjectCode AND u.UnitID = n.UnitID
