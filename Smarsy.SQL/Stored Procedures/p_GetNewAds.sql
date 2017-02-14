﻿CREATE PROCEDURE [dbo].[p_GetNewAds]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT AdText,
        AdDate
    FROM Ad
    WHERE CAST(CreateDtime AS DATE) >= CAST(DATEADD(DAY, -1, SYSDATETIME()) AS DATE)
          OR CAST(AdDate AS DATE)
          BETWEEN CAST(DATEADD(DAY, -1, SYSDATETIME()) AS DATE) AND CAST(SYSDATETIME() AS DATE)
    ORDER BY AdDate DESC;
END;