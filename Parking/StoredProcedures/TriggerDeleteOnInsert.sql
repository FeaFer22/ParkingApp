USE ParkingAppTest1;
GO
CREATE TRIGGER FixedSlots_INSERT
ON FixedSlots
AFTER INSERT
AS
DELETE FROM FixedSlots
WHERE FixationEndTime > GETDATE()