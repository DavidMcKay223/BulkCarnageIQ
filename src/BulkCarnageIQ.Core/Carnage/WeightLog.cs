using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkCarnageIQ.Core.Carnage
{
    public class WeightLog
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public DateOnly Date { get; set; }
        public float WeightLbs { get; set; }
    }
}
