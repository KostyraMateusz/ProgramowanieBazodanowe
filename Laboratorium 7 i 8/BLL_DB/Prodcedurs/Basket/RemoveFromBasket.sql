CREATE PROCEDURE RemoveFromBasket
    @UserId INT,
    @ProductId INT
AS
BEGIN
    DELETE FROM BasketPositions
    WHERE UserID = @UserId AND ProductID = @ProductId;
END