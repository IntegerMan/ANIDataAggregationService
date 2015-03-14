using System.Collections.Generic;

namespace ANIDataAggregationService
{
    /// <summary>
    /// Contains utility code to help monitor groups of areas
    /// </summary>
    public static class AreaMonitor
    {

        /// <summary>
        /// Gets the watched zip codes.
        /// </summary>
        /// <returns>Watched zip codes.</returns>
        public static IEnumerable<int> GetWatchedZipCodes()
        {
            return new List<int>
            {
                43002,
                43004,
                43016,
                43017,
                43026,
                43035,
                43054,
                43065,
                43081,
                43082,
                43085,
                43119,
                43123,
                43137,
                43147,
                43201,
                43202,
                43203,
                43204,
                43205,
                43206,
                43207,
                43210,
                43211,
                43212,
                43213,
                43214,
                43215,
                43217,
                43219,
                43220,
                43221,
                43222,
                43223,
                43224,
                43227,
                43228,
                43229,
                43230,
                43231,
                43235,
                43240
            };

        }
    }
}