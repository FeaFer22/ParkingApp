USE [ParkingAppTest1]
GO
/****** Object:  StoredProcedure [dbo].[GetFixedSlotWhereUserLicensePlate]    Script Date: 30.05.2023 13:02:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[GetFixedSlotWhereUserLicensePlate]
@userLicensePlate varchar(max)
AS
SELECT * FROM FixedSlots WHERE UserLicensePlate = @userLicensePlate