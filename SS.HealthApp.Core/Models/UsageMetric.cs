//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SS.HealthApp.Core.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class UsageMetric
    {
        public long UsageMetricID { get; set; }
        public string Class { get; set; }
        public string Method { get; set; }
        public string Info { get; set; }
        public System.DateTime Moment { get; set; }
        public string CompanyID { get; set; }
        public string UserID { get; set; }
    }
}
