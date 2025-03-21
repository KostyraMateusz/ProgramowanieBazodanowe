CREATE PROCEDURE AddGroup
    @Name NVARCHAR(100),
    @ParentId INT = NULL
AS
BEGIN
    INSERT INTO ProductGroups (Name, ParentID)
    VALUES (@Name, @ParentId);
END