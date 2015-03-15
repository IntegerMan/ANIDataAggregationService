CREATE VIEW dbo.FrostPredictionDataView
AS

SELECT
  wfr.WFR_HadFrost AS HadFrost,
  ISNULL(wfr.WFR_MinutesToDefrost, 0) AS MinutesToDefrost,
  wp.WP_TempLow AS Low,
  wp.WP_TempHigh AS High,
  wfr.WFR_RainedOvernight AS HadRain,
  wc.WC_HasRain AS HasRain,
  wc.WC_HasClouds AS HasClouds,
  wc.WC_HasStorm AS HasStorm,
  wc.WC_HasWind AS HasWind,
  wc.WC_HasSnow AS HasSnow,
  wc.WC_ID AS WeatherCode
FROM dbo.WeatherFrostResults wfr
INNER JOIN dbo.WeatherPredictions wp
  ON wfr.WFR_PredictionID = wp.WP_ID
INNER JOIN dbo.WeatherCodes wc
  ON wp.WP_WeatherCode = wc.WC_ID

GO