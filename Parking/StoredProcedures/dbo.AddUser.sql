CREATE PROCEDURE [dbo].[AddUser]
	@lastName varchar(max),
	@firstName varchar(max),
	@middleName varchar(max),
	@licensePlate varchar(max),
	@contactNumber varchar(max),
	@passwordHash varbinary(max),
	@passwordSalt varbinary(max)
AS
	INSERT INTO [dbo].[Users]([LastName], [FirstName], [MiddleName], [LicensePlate], [ContactNumber], [PasswordHash], [PasswordSalt]) 
	VALUES (@lastName, @firstName, @middleName, @licensePlate, @contactNumber, @passwordHash, @passwordSalt);