CREATE PROCEDURE [dbo].[UnFixParkingSlot]
	@slotName varchar(max)
AS
	DELETE FixedSlots WHERE ParkingSlotName = @slotName;
