CREATE PROCEDURE LoginUser
    @Login NVARCHAR(255),
    @Password NVARCHAR(255)
AS
BEGIN
    DECLARE @IsValid BIT;

    SELECT @IsValid = CASE 
                           WHEN EXISTS (
                               SELECT 1 
                               FROM Users 
                               WHERE Login = @Login 
                                 AND Password = @Password 
                                 AND IsActive = 1
                           ) 
                           THEN 1
                           ELSE 0
                         END;

    SELECT @IsValid AS IsValid;
END
