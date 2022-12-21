using dev_test_api_models;
using dev_test_api_models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dev_test_api_business
{
    public interface IDevTestService
    {
        Task<List<PersonDto>> GetPerson();
        Task<ClaimsResponseDto> GetClaimsByPersonId(int personId, int offset, int rows);
        Task<IEnumerable<PersonDto>> GetClaimsByExternalPersonId(int personExtId);
        Task<int> GetClaimCount(int personId);
        Task<List<PersonDetailsDto>> GetPersonDetails(int organizationId, string? extPersonId, string? email, string? firstName, string? lastName, string? phone);
    }
}
