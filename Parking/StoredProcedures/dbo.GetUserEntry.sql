CREATE PROCEDURE [dbo].[GetUserEntry]
	@licensePlate varchar(max)
AS
	SELECT * FROM Users WHERE LicensePlate = @licensePlate