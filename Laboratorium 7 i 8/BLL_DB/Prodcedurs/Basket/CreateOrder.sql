CREATE PROCEDURE CreateOrder
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @OrderID INT;

    INSERT INTO Orders (UserID, Date, IsPaid)
    VALUES (@UserId, GETDATE(), 0);

    SET @OrderID = SCOPE_IDENTITY();

    INSERT INTO OrderPositions (OrderID, ProductId, Amount, Price)
    SELECT @OrderID, bp.ProductID, bp.Amount, p.Price
    FROM BasketPositions bp
    JOIN Products p ON p.ID = bp.ProductID
    WHERE bp.UserID = @UserId;

    DELETE FROM BasketPositions WHERE UserID = @UserId;

    SELECT 
        o.ID AS OrderID, 
        o.Date AS OrderDate, 
        o.IsPaid,
        SUM(op.Price * op.Amount) AS TotalPrice
    FROM Orders o
    JOIN OrderPositions op ON op.OrderID = o.ID
    WHERE o.ID = @OrderID
    GROUP BY o.ID, o.Date, o.IsPaid;
END