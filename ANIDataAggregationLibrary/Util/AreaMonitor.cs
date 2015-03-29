using System.Collections.Generic;
using System.Linq;
using ANIDataAggregationLibrary.Database;

namespace ANIDataAggregationLibrary.Util
{
    /// <summary>
    /// Contains utility code to help monitor groups of areas
    /// </summary>
    public static class AreaMonitor
    {

        /// <summary>
        /// Gets the watched zip codes.
        /// </summary>
        /// <param name="dataModel">The data model.</param>
        /// <returns>Watched zip codes.</returns>
        public static IEnumerable<int> GetWatchedZipCodes(AniEntities dataModel)
        {
            // Pull back active zip codes from the database
            return dataModel.ZipCodes.Where(z => z.ServiceStatus.IsActive).Select(z => z.ID).ToList();
        }
    }
}