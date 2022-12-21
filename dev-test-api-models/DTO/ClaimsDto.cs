using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dev_test_api_models.DTO
{
    public class ClaimsDto
    {
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
        public List<ClaimsCodeDto> Codes { get; set; }
    }
}
