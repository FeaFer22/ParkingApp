-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[FixParkingSlot]
	@name varchar(max),
	@isFixed bit
AS
	UPDATE ParkingSlots SET IsFixed = @isFixed WHERE Name = @name;