CREATE PROCEDURE AddProduct
    @Name NVARCHAR(100),
    @Price DECIMAL(18, 2),
    @GroupID INT
AS
BEGIN
    INSERT INTO Products (Name, Price, GroupID, IsActive)
    VALUES (@Name, @Price, @GroupID, 1);
END