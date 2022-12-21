using dev_test_api_models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dev_test_api_data_access
{
    public interface IDataAccess
    {
        Task<IEnumerable<Person>> GetPersons();
        Task<IEnumerable<Person>> GetClaimsByExternalPersonId(int? personExtId);
        Task<IEnumerable<Person>> GetPersonDetails(int organizationId, string? extPersonId, string? email, string? firstName, string? lastName, string? phone);
        Task<IEnumerable<Claims>> GetClaimsByPersonId(int? personId, int offset, int rows);
        Task<IEnumerable<Codes>> GetCodesForClaims(List<int> claimIds);
        Task<int> GetClaimsCount(int personId);
    }
}
