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
    
    public partial class ActiveTrafficIncidentInfoSelect_Result
    {
        public long id { get; set; }
        public string Description { get; set; }
        public string Congestion { get; set; }
        public string Detour { get; set; }
        public string Lane { get; set; }
        public bool RoadClosed { get; set; }
        public bool Verified { get; set; }
        public Nullable<System.DateTime> StartTimeUTC { get; set; }
        public Nullable<System.DateTime> EndTimeUTC { get; set; }
        public int SeverityID { get; set; }
        public string SeverityName { get; set; }
        public int TypeID { get; set; }
        public string TypeName { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
    }
}
