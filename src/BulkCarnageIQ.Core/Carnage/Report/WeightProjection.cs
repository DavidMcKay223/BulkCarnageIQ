using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkCarnageIQ.Core.Carnage.Report
{
    public class WeightProjection
    {
        public float CurrentWeight { get; set; }
        public float ProjectedWeight7Days { get; set; }
        public float ProjectedWeight14Days { get; set; }
    }
}
