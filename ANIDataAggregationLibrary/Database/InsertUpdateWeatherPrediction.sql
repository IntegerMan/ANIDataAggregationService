CREATE PROCEDURE dbo.InsertUpdateWeatherPrediction (@PredictionDateUTC AS DATE,
@CreatorNodeID AS INT,
@ZipCode AS INT,
@Low AS FLOAT,
@High AS FLOAT,
@Code AS INT,
@MinutesToDefrost AS FLOAT = NULL)
AS

  IF EXISTS (SELECT
      1
    FROM dbo.WeatherPredictions wp
    WHERE wp.WP_ZipCode = @ZipCode
    AND wp.WP_PredictionDateUTC = @PredictionDateUTC)
  BEGIN

    -- Do not allow updating dates that are before today
    IF @PredictionDateUTC > GETUTCDATE() begin

      -- Update with a more accurate forecast if this was yesterday's 5 day forecast and now it's a 4 day forecast
      UPDATE dbo.WeatherPredictions
      SET WP_TempLow = @Low,
          WP_TempHigh = @High,
          WP_WeatherCode = @Code,
          WP_MinutesToDefrost = @MinutesToDefrost
      WHERE WP_ZipCode = @ZipCode
      AND WP_PredictionDateUTC = @PredictionDateUTC

    End

  END
  ELSE
  BEGIN

    -- Add the item into the database. Note that this can add today's entries if they don't exist yet
    INSERT INTO dbo.WeatherPredictions (WP_CreatedTimeUTC, WP_PredictionDateUTC, WP_CreatorUserNodeID, WP_ZipCode, WP_TempLow, WP_TempHigh, WP_WeatherSourceID, WP_WeatherCode, WP_MinutesToDefrost)
      VALUES (SYSUTCDATETIME(), @PredictionDateUTC, @CreatorNodeID, @ZipCode, @Low, @High, 1, @Code, @MinutesToDefrost)

  END

GO