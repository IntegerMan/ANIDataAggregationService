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
    
    public partial class WeatherSource
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public WeatherSource()
        {
            this.WeatherPredictions = new HashSet<WeatherPrediction>();
        }
    
        public int WS_ID { get; set; }
        public string WS_Name { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WeatherPrediction> WeatherPredictions { get; set; }
    }
}
