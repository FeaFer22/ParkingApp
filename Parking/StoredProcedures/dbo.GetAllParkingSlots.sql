USE [ParkingAppTest1]
GO
/****** Object:  StoredProcedure [dbo].[GetAllParkingSlots]    Script Date: 30.05.2023 12:28:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[GetAllParkingSlots]
AS
	SELECT ParkingSlots.Id, ParkingSlots.Name, ParkingSlots.IsFixed, 
	FixedSlots.FixationTime, FixedSlots.FixationEndTime, FixedSlots.UserLicensePlate
	FROM ParkingSlots
	LEFT JOIN FixedSlots 
	ON ParkingSlots.Name = FixedSlots.ParkingSlotName 
	ORDER BY ParkingSlots.Name