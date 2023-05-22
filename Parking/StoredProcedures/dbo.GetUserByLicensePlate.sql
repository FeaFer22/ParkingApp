CREATE PROCEDURE [dbo].[GetUserByLicensePlate]
	@licensePlate varchar(max)
AS
	SELECT Id, LastName, FirstName, MiddleName, ContactNumber FROM Users WHERE LicensePlate = @licensePlate;