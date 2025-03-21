CREATE PROCEDURE PayOrder
    @OrderID INT,
    @Amount DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Total DECIMAL(18,2);

    SELECT @Total = SUM(Price * Amount)
    FROM OrderPositions
    WHERE OrderID = @OrderID;

    IF ABS(@Total - @Amount) < 0.01
        UPDATE Orders SET IsPaid = 1 WHERE ID = @OrderID;
    ELSE
        THROW 50001, 'Amount does not match order total value', 1;
END