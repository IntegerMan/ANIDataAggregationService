//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ANIDataAggregationLibrary.Database
{
    using System;
    using System.Collections.Generic;
    
    public partial class WeatherFrostResult
    {
        public int WFR_ID { get; set; }
        public System.DateTime WFR_CreatedTimeUTC { get; set; }
        public int WFR_PredictionID { get; set; }
        public int WFR_CreatorUserNodeID { get; set; }
        public bool WFR_HadFrost { get; set; }
        public bool WFR_RainedOvernight { get; set; }
        public Nullable<double> WFR_MinutesToDefrost { get; set; }
    
        public virtual UserNode UserNode { get; set; }
        public virtual WeatherPrediction WeatherPrediction { get; set; }
    }
}
