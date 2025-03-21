CREATE PROCEDURE DeleteProduct
    @ProductId INT
AS
BEGIN
    IF EXISTS (
        SELECT 1 FROM OrderPositions WHERE ProductId = @ProductId
    )
    BEGIN
        THROW 50002, 'Cannot delete product that is linked to orders', 1;
    END

    DELETE FROM Products WHERE ID = @ProductId;
END