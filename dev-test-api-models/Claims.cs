using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dev_test_api_models
{
    public class Claims
    {
        public int PID { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int ClaimId { get; set; }
        public int ServiceNumber { get; set; }
        public string ServiceName { get; set; }
        public string ServiceAddress { get; set; }
        public string ServiceCity { get; set; }
        public string ServiceZip { get; set; }
        public int ClaimNumber { get; set; }
        public DateTime PaidDate { get; set; }
        public DateTime? ServiceStartDate { get; set; }
        public DateTime? ServiceEndDate { get; set; }
        public string ServiceType { get; set; }
        public double ChargedAmount { get; set; }
        public double PaidAmount { get; set; }
        public string Code { get; set; } = string.Empty;
        public int CodeType { get; set; }
        public int CodeId { get; set; }
        public int? Version { get; set; }



      
    }
}
