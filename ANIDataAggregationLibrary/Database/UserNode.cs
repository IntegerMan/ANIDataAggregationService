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
    
    public partial class UserNode
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public UserNode()
        {
            this.TrafficIncidents = new HashSet<TrafficIncident>();
            this.Transits = new HashSet<Transit>();
            this.WeatherFrostResults = new HashSet<WeatherFrostResult>();
            this.WeatherPredictions = new HashSet<WeatherPrediction>();
        }
    
        public int UN_ID { get; set; }
        public int UN_UserID { get; set; }
        public int UN_NodeID { get; set; }
        public System.DateTime UN_CreatedTimeUTC { get; set; }
    
        public virtual Node Node { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TrafficIncident> TrafficIncidents { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Transit> Transits { get; set; }
        public virtual User User { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WeatherFrostResult> WeatherFrostResults { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WeatherPrediction> WeatherPredictions { get; set; }
    }
}
