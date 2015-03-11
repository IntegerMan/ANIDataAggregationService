using System;
using System.IO;
using System.Net;
using System.Text;

namespace ANIDataAggregationService
{
    public class WeatherForecastRecordingProcessor
    {
        public string ZipCode { get; set; }

        public WeatherForecastRecordingProcessor(string zipCode = "43035")
        {
            this.ZipCode = zipCode;
        }

        /// <summary>
        /// Gets tomorrow's weather prediction for this zip code and records it in the database.
        /// </summary>
        public void RecordTomorrowsWeatherPrediction()
        {
            var responseData = GetWeatherForecast();

            Console.WriteLine(responseData);
        }

        private string GetWeatherForecast()
        {
            string responseData = null;

            // Grab the data from a GET request based on our zip code.
            var uriString = string.Format("http://xml.weather.yahoo.com/forecastrss/{0}_f.xml", this.ZipCode);
            var request = WebRequest.Create(uriString);
            var response = request.GetResponse();

            // Read the response and store it into a variable.
            using (var stream = response.GetResponseStream())
            {
                if (stream != null)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        responseData = reader.ReadToEnd();
                    }
                }
            }

            /* Response Data Resmbles:

                <?xml version="1.0" encoding="UTF-8" standalone="yes" ?>
		        <rss version="2.0" xmlns:yweather="http://xml.weather.yahoo.com/ns/rss/1.0" xmlns:geo="http://www.w3.org/2003/01/geo/wgs84_pos#">
			        <channel>
		
                        <title>Yahoo! Weather - Lewis Center, OH</title>
                        <link>http://us.rd.yahoo.com/dailynews/rss/weather/Lewis_Center__OH/*http://weather.yahoo.com/forecast/USOH0505_f.html</link>
                        <description>Yahoo! Weather for Lewis Center, OH</description>
                        <language>en-us</language>
                        <lastBuildDate>Wed, 11 Mar 2015 12:47 am EDT</lastBuildDate>
                        <ttl>60</ttl>
                        <yweather:location city="Lewis Center" region="OH"   country="US"/>
                        <yweather:units temperature="F" distance="mi" pressure="in" speed="mph"/>
                        <yweather:wind chill="37"   direction="0"   speed="0" />
                        <yweather:atmosphere humidity="100"  visibility="0.25"  pressure="30.04"  rising="0" />
                        <yweather:astronomy sunrise="7:50 am"   sunset="7:32 pm"/>
                        <image>
                        <title>Yahoo! Weather</title>
                        <width>142</width>
                        <height>18</height>
                        <link>http://weather.yahoo.com</link>
                        <url>http://l.yimg.com/a/i/brand/purplelogo//uh/us/news-wea.gif</url>
                        </image>
                        <item>
                        <title>Conditions for Lewis Center, OH at 12:47 am EDT</title>
                        <geo:lat>40.2</geo:lat>
                        <geo:long>-83.01</geo:long>
                        <link>http://us.rd.yahoo.com/dailynews/rss/weather/Lewis_Center__OH/*http://weather.yahoo.com/forecast/USOH0505_f.html</link>
                        <pubDate>Wed, 11 Mar 2015 12:47 am EDT</pubDate>
                        <yweather:condition  text="Fog"  code="20"  temp="37"  date="Wed, 11 Mar 2015 12:47 am EDT" />
                        <description><![CDATA[
                        <img src="http://l.yimg.com/a/i/us/we/52/20.gif"/><br />
                        <b>Current Conditions:</b><br />
                        Fog, 37 F<BR />
                        <BR /><b>Forecast:</b><BR />
                        Tue - Foggy. High: 46 Low: 36<br />
                        Wed - AM Fog/PM Sun. High: 54 Low: 29<br />
                        Thu - Partly Cloudy. High: 59 Low: 38<br />
                        Fri - Showers. High: 52 Low: 48<br />
                        Sat - Rain. High: 59 Low: 37<br />
                        <br />
                        <a href="http://us.rd.yahoo.com/dailynews/rss/weather/Lewis_Center__OH/*http://weather.yahoo.com/forecast/USOH0505_f.html">Full Forecast at Yahoo! Weather</a><BR/><BR/>
                        (provided by <a href="http://www.weather.com" >The Weather Channel</a>)<br/>
                        ]]></description>
                        <yweather:forecast day="Tue" date="10 Mar 2015" low="36" high="46" text="Foggy" code="20" />
                        <yweather:forecast day="Wed" date="11 Mar 2015" low="29" high="54" text="AM Fog/PM Sun" code="20" />
                        <yweather:forecast day="Thu" date="12 Mar 2015" low="38" high="59" text="Partly Cloudy" code="30" />
                        <yweather:forecast day="Fri" date="13 Mar 2015" low="48" high="52" text="Showers" code="11" />
                        <yweather:forecast day="Sat" date="14 Mar 2015" low="37" high="59" text="Rain" code="12" />
                        <guid isPermaLink="false">USOH0505_2015_03_14_7_00_EDT</guid>
                        </item>
                    </channel>
                </rss>

                */

            return responseData;
        }
    }
}