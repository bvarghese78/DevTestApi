using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dev_test_api_models.DTO
{
    public class PersonDto
    {
        public int OrganizationId { get; set; }
        public List<NamesDto> Names { get; set; }
    }
}
