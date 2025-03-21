CREATE PROCEDURE AddProductToBasket
    @UserId INT,
    @ProductId INT,
    @Amount INT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1 FROM BasketPositions WHERE UserID = @UserId AND ProductID = @ProductId
    )
        UPDATE BasketPositions
        SET Amount = Amount + @Amount
        WHERE UserID = @UserId AND ProductID = @ProductId;
    ELSE
        INSERT INTO BasketPositions (UserID, ProductID, Amount)
        VALUES (@UserId, @ProductId, @Amount);
END