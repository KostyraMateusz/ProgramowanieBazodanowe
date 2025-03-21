CREATE PROCEDURE UpdateBasketItem
    @UserId INT,
    @ProductId INT,
    @Amount INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE BasketPositions
    SET Amount = @Amount
    WHERE UserID = @UserId AND ProductID = @ProductId;
END