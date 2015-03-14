CREATE PROCEDURE dbo.InsertUpdateTrafficIncident (@IncidentID AS BIGINT,
@Description NVARCHAR(MAX) = NULL,
@Congsestion NVARCHAR(MAX) = NULL,
@Detour NVARCHAR(MAX) = NULL,
@Lane NVARCHAR(MAX) = NULL,
@RoadClosed BIT,
@Verified BIT,
@CreatedTimeUTC AS DATETIME2,
@ModifiedTimeUTC AS DATETIME2,
@LocationLat AS FLOAT,
@LocationLng AS FLOAT,
@EndLocationLat AS FLOAT = NULL,
@EndLocationLng AS FLOAT = NULL,
@CreatorUserNodeID AS INT,
@SeverityID AS INT,
@TypeID AS INT)
AS

  DECLARE @LocationID AS INT = NULL
  DECLARE @EndLocationID AS INT = NULL

  -- Find the location if it already exists
  SELECT
    @LocationID = loc.GPS_ID
  FROM dbo.GPSLocations loc
  WHERE loc.GPS_Lat = @LocationLat
  AND loc.GPS_Lng = @LocationLng

  -- Okay, no entry for this location yet. Create one
  IF @LocationID IS NULL
  BEGIN
    INSERT INTO dbo.GPSLocations (GPS_Lat, GPS_Lng)
      VALUES (@LocationLat, @LocationLng);

    SET @LocationID = @@Identity
  END

  -- Ignore it if we're not specifying an end location
  IF @EndLocationLat IS NOT NULL
    AND @EndLocationLng IS NOT NULL
  BEGIN

    -- Find the location if it exists
    SELECT
      @EndLocationID = loc.GPS_ID
    FROM dbo.GPSLocations loc
    WHERE loc.GPS_Lat = @EndLocationLat
    AND loc.GPS_Lng = @EndLocationLng

    -- No entry for the end location yet. Create one
    IF @LocationID IS NULL
    BEGIN
      INSERT INTO dbo.GPSLocations (GPS_Lat, GPS_Lng)
        VALUES (@EndLocationLat, @EndLocationLng);

      SET @EndLocationID = @@Identity

    END
  END

  -- Okay, now that we have locations (as needed), check to see if we're doing an insert or an update
  IF EXISTS (SELECT
      1
    FROM dbo.TrafficIncidents ti
    WHERE ti.TI_ID = @IncidentID)
  BEGIN

    -- Update with a more accurate forecast if this was yesterday's 5 day forecast and now it's a 4 day forecast
    UPDATE dbo.TrafficIncidents
    SET TI_Congestion = @Congsestion,
        TI_Description = @Description,
        TI_Detour = @Detour,
        TI_Lane = @Lane,
        TI_Severity = @SeverityID,
        TI_Type = @TypeID,
        TI_RoadClosed = @RoadClosed,
        TI_Verified = @Verified,
        TI_LocationID = @LocationID,
        TI_EndLocationID = @EndLocationID,
        TI_ModifiedTimeUTC = @ModifiedTimeUTC
    WHERE TI_ID = @IncidentID

  END
  ELSE
  BEGIN

    -- We insert using the bing maps ID, not an auto-number id
    SET IDENTITY_INSERT dbo.TrafficIncidents ON

    -- Actually insert
    INSERT INTO dbo.TrafficIncidents (TI_ID, TI_Congestion, TI_Description, TI_Detour, TI_Lane, TI_Severity, TI_Type, TI_RoadClosed, TI_Verified, TI_LocationID, TI_EndLocationID, TI_CreatedTimeUTC, TI_ModifiedTimeUTC, TI_CreatedUserNodeID)
      VALUES (@IncidentID, @Congsestion, @Description, @Detour, @Lane, @SeverityID, @TypeID, @RoadClosed, @Verified, @LocationID, @EndLocationID, @CreatedTimeUTC, @ModifiedTimeUTC, @CreatorUserNodeID);

    -- Not sure we need to do this, but probably should for safety
    SET IDENTITY_INSERT dbo.TrafficIncidents OFF

  END

GO