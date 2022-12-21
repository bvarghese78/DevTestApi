using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dev_test_api_models.DTO;

namespace dev_test_api_models
{
    public class Person
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public int ExternalPersonId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Email { get; set; }
        public int EmailId { get; set; }
        public int PhoneId { get; set; }
        public string Phone { get; set; }
        //public List<EmaiDto> Emails { get; set; } = new List<EmaiDto>();
        //public List<PhoneDto> Phones { get; set; }
    }
}
