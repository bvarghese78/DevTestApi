using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dev_test_api_models
{
    public class Codes
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public int CodeType { get; set; }
        public int CodeId { get; set; }
    }
}
