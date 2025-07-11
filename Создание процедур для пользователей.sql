CREATE PROCEDURE [dbo].[UpdateUserWithoutPassword]
    @Login nvarchar(50),
    @StatusID int,
    @EmployeeID int
AS
BEGIN
    UPDATE Users 
    SET Login_user = @Login, 
        StatusID = @StatusID
    WHERE EmployeeID = @EmployeeID
END
GO

CREATE PROCEDURE [dbo].[UpdateUserWithPassword]
    @Login nvarchar(50),
    @PlainPassword nvarchar(64),
    @StatusID int,
    @EmployeeID int
AS
BEGIN
    DECLARE @HashedPassword nvarchar(64)
    SET @HashedPassword = CONVERT(nvarchar(64), HASHBYTES('SHA2_256', @PlainPassword), 2)
    
    UPDATE Users 
    SET Login_user = @Login, 
        Password_user = @HashedPassword,
        StatusID = @StatusID
    WHERE EmployeeID = @EmployeeID
END
GO