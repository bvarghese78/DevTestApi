using dev_test_api_data_access;
using dev_test_api_models;
using dev_test_api_models.DTO;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using System.Net.WebSockets;
using System.Reflection.Emit;
using Microsoft.IdentityModel.Abstractions;
using Microsoft.Extensions.Logging;

namespace dev_test_api_business
{
    public class DevTestService : IDevTestService
    {
        private readonly IDataAccess _devDataAccess;
        private IMapper _mapper;
        private ILogger<DevTestService> _logger;
        const int maxClaimsPageSize = 500;
        public DevTestService(ILogger<DevTestService> logger, IDataAccess dataAccess)
        {
            _devDataAccess = dataAccess;
            _logger = logger;

            var mapperConfig = new MapperConfiguration((cfg) =>
            {
                cfg.CreateMap<Person, NamesDto>()
                    .ForMember(x => x.Id, opt => opt.MapFrom(src => src.Id))
                    .ForMember(x => x.FirstName, opt => opt.MapFrom(src => src.FirstName))
                    .ForMember(x => x.LastName, opt => opt.MapFrom(src => src.LastName));

                cfg.CreateMap<Claims, ClaimsDto>();
                cfg.CreateMap<Claims, ClaimsCodeDto>();
                cfg.CreateMap<Codes, ClaimsCodeDto>();
                cfg.CreateMap<Claims, ClaimsResponseDto>();
                cfg.CreateMap<Person, PersonDetailsDto>();
                cfg.CreateMap<Person, EmailDto>();
                cfg.CreateMap<Person, PhoneDto>();
            });

            _mapper = mapperConfig.CreateMapper();
        }

        /// <summary>
        /// Returns a list of Person DTO because data is too large to return
        /// </summary>
        /// <param name="personExtId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<PersonDto>> GetClaimsByExternalPersonId(int personExtId)
        {
            var persons = await _devDataAccess.GetClaimsByExternalPersonId(personExtId);

            if (persons == null)
            {
                _logger.LogWarning("Claims are empty");
                return Enumerable.Empty<PersonDto>();
            }

            List<PersonDto> listOfPersons = new List<PersonDto>();
            foreach(var person in persons)
            {
                PersonDto dto = new PersonDto();
                dto.Names = new List<NamesDto>();

                dto.OrganizationId = person.OrganizationId;
                dto.Names.Add(_mapper.Map<NamesDto>(person));

                listOfPersons.Add(dto);
            }
            

            return listOfPersons;
        }

        public async Task<ClaimsResponseDto> GetClaimsByPersonId(int personId, int pageNumber, int pageSize)
        {
            if (pageSize > maxClaimsPageSize)
                pageSize = maxClaimsPageSize;

            // Retrieve Claims
            var results = await _devDataAccess.GetClaimsByPersonId(personId, pageNumber, pageSize);

            if (results == null)
            {
                _logger.LogWarning("Get Claims By Person Id is null");
                return new ClaimsResponseDto();
            }

            ClaimsResponseDto claimsResponseDto = new ClaimsResponseDto();
            claimsResponseDto.FirstName = results.First().FirstName;
            claimsResponseDto.LastName = results.First().LastName;
            claimsResponseDto.MiddleName = results.First().MiddleName;
            claimsResponseDto.PersonId = results.First().PID;

            claimsResponseDto.TotalClaimCount = await _devDataAccess.GetClaimsCount(personId);
            claimsResponseDto.Claims = new List<ClaimsDto>();

            // Group claims
            var claimIds = results.GroupBy(x => x.ClaimId).Select(g => g.First().ClaimId).ToList();

            // Retrieve Codes for all claimIds
            var codesForClaims = await _devDataAccess.GetCodesForClaims(claimIds);

            foreach(var result in results)
            {
                var codes = codesForClaims.Where(x => x.Id == result.ClaimId).ToList();

                var claimsDto = _mapper.Map<ClaimsDto>(result);
                claimsDto.Codes = new List<ClaimsCodeDto>();

                foreach (var code in codes)
                {
                    claimsDto.Codes.Add(_mapper.Map<ClaimsCodeDto>(code));
                }

                claimsResponseDto.Claims.Add(claimsDto);
            }

            return claimsResponseDto;
        }

        public async Task<List<PersonDto>> GetPerson()
        {
            var result = await _devDataAccess.GetPersons();

            if(result == null)
            {
                _logger.LogWarning("Empty Person result");
                return new List<PersonDto>();
            }

            var uniqueOrganizations = result.Select(x => x.OrganizationId).Distinct();

            List<PersonDto> personsDto = new List<PersonDto>();

            foreach (var id in uniqueOrganizations)
            {
                var listOfPersonsPerOrganization = result.Where(x => x.OrganizationId == id).ToList();
                PersonDto dto = new PersonDto();
                dto.OrganizationId = id;
                dto.Names = _mapper.Map<List<NamesDto>>(listOfPersonsPerOrganization);

                personsDto.Add(dto);
            }
            return personsDto;
        }

        public async Task<List<PersonDetailsDto>> GetPersonDetails(int organizationId, string? extPersonId, string? email, string? firstName, string? lastName, string? phone)
        {
            var response = await _devDataAccess.GetPersonDetails(organizationId, extPersonId, email, firstName, lastName, phone);

            if(response == null)
            {
                _logger.LogWarning("Person Details is null");
                return new List<PersonDetailsDto>();
            }

            List<PersonDetailsDto> listOfPersonDetailsDto = new List<PersonDetailsDto>();


            var personIds = response.GroupBy(x => x.Id).Select(g => g.First().Id).ToList();
            foreach(var personId in personIds)
            {
                var personList = response.Where(x => x.Id == personId).ToList();

                var personDetailsDto = _mapper.Map<PersonDetailsDto>(personList.FirstOrDefault());
                personDetailsDto.Emails = RetrieveEmails(personList);
                personDetailsDto.Phones = RetrievePhoneNumbers(personList);

                listOfPersonDetailsDto.Add(personDetailsDto);
            }

            return listOfPersonDetailsDto;
        }

        public async Task<int> GetClaimCount(int personId)
        {
            var response = await _devDataAccess.GetClaimsCount(personId);
            return response;
        }

        private List<EmailDto> RetrieveEmails(List<Person> personList)
        {
            var emailIds = personList.GroupBy(x => x.EmailId).Select(g => g.First().EmailId).ToList();
            List<EmailDto> listOfEmails = new List<EmailDto>();
            if (emailIds.Count == 1)
            {
                listOfEmails.Add(new EmailDto() { Email = personList.Select(x => x.Email).FirstOrDefault() });
            }
            else
            {

                foreach (var emailId in emailIds)
                {
                    var uniqueEmailPersonList = personList.Where(x => x.EmailId == emailId);
                    listOfEmails.Add(new EmailDto() { Email = uniqueEmailPersonList.Select(x => x.Email).FirstOrDefault() });
                }
            }

            return listOfEmails;
        }

        private List<PhoneDto> RetrievePhoneNumbers(List<Person> personList)
        {
            var phoneIds = personList.GroupBy(x => x.PhoneId).Select(g => g.First().PhoneId).ToList();
            List<PhoneDto> listOfPhones = new List<PhoneDto>();
            if (phoneIds.Count == 1)
            {
                listOfPhones.Add(new PhoneDto() { PhoneNumber = personList.Select(x => x.Phone).FirstOrDefault() });
            }
            else
            {

                foreach (var phoneId in phoneIds)
                {
                    var uniquePhonePersonList = personList.Where(x => x.PhoneId == phoneId);
                    listOfPhones.Add(new PhoneDto() { PhoneNumber = uniquePhonePersonList.Select(x => x.Phone).FirstOrDefault() });
                }
            }

            return listOfPhones;
        }
    }
}