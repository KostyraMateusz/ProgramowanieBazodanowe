CREATE PROCEDURE ChangeProductStatus
    @ProductId INT
AS
BEGIN
    UPDATE Products
    SET IsActive = CASE WHEN IsActive = 1 THEN 0 ELSE 1 END
    WHERE ID = @ProductId;
END