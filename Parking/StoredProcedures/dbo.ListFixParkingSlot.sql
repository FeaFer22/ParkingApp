-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[ListFixParkingSlot]
	@fixationTime datetime2,
	@fixationEndTime datetime2,
	@userLicensePlate varchar(max),
	@parkingSlot varchar(max)
AS
	INSERT INTO FixedSlots VALUES (@fixationTime, @fixationEndTime, @parkingSlot, @userLicensePlate)