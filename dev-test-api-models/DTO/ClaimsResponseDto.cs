using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dev_test_api_models.DTO
{
    public class ClaimsResponseDto
    {
        public int PersonId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int TotalClaimCount { get; set; }
        public List<ClaimsDto> Claims { get; set; }
    }
}
