using Azure;
using Dapper;
using dev_test_api_models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Runtime.InteropServices;
using static Dapper.SqlMapper;

namespace dev_test_api_data_access
{
    public class DataAccess : IDataAccess
    {
        private readonly IConfiguration configuration;
        private readonly string CONNECTION_STRING;
        public DataAccess(IConfiguration config)
        {
            configuration = config;
            CONNECTION_STRING = configuration["SqlServerConnectionString"] ?? string.Empty;
        }

        public async Task<IEnumerable<Person>> GetPersons()
        {
            try
            {
                using (var conn = new SqlConnection(CONNECTION_STRING))
                {
                    string sql = @"SELECT [Id]
      ,[OrganizationId]
      ,[FirstName]
      ,[LastName]
  FROM [DevTest].[dbo].[Person]";

                    //DynamicParameters dynamicParameters = new DynamicParameters();
                    var res = await conn.QueryAsync<Person>(sql);

                    return res;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        public async Task<IEnumerable<Claims>> GetClaimsByPersonId(int? personId, int offset, int rows)
        {
            try
            {
                string sql = @"SELECT p.[Id] as PID
      ,p.[FirstName]
      ,p.[Middle]
      ,p.[LastName]
	  ,c.[Id] as ClaimId
      ,c.[ServiceNumber]
      ,c.[ServiceName]
      ,c.[ServiceAddress]
      ,c.[ServiceCity]
      ,c.[ServiceState]
      ,c.[ServiceZip]
      ,c.[PaidDate]
      ,c.[DateofServiceStart]
      ,c.[DateofServiceEnd]
      ,c.[ServiceType]
      ,c.[ChargedAmount]
      ,c.[PaidAmount]
    from [DevTest].[dbo].[Person] p
    inner join [DevTest].[dbo].[Claims] c on  p.id = c.ExternalPersonId
  where p.Id = @personId
  order by c.Id
  offset @offset
  rows fetch next @rows rows only";
                using (var conn = new SqlConnection(CONNECTION_STRING))
                {
                    IEnumerable<Claims> response;
                    DynamicParameters dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("personId", personId);
                    dynamicParameters.Add("offset", offset);
                    dynamicParameters.Add("rows", rows);

                    response = await conn.QueryAsync<Claims>(sql, dynamicParameters);


                    return response;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<IEnumerable<Codes>> GetCodesForClaims(List<int> claimIds)
        {
            try
            {
                string sql = @"SELECT c.[Id]
      ,fkc.[Code]
	  ,fkc.[CodeType]
	  ,fkc.[Id] as CodeId
	  ,fkc.[Version]
  FROM [DevTest].[dbo].[Claims] c
  inner join [DevTest].[dbo].[ClaimFKCodes] cfc on cfc.ExternalClaimId = c.Id
  inner join [DevTest].[dbo].[FKCodes] fkc on fkc.id = cfc.FKCodeId
  where c.id in @Ids";
                using (var conn = new SqlConnection(CONNECTION_STRING))
                {
                    IEnumerable<Codes> response;

                    response = await conn.QueryAsync<Codes>(sql, new {Ids = claimIds});

                    return response;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<IEnumerable<Person>> GetClaimsByExternalPersonId(int? personExtId)
        {
            string sql = @"SELECT TOP (1000) [Id]
      ,[OrganizationId]
      ,[ExternalPersonId]
      ,[SubscriberNumber]
      ,[SubscriberId]
      ,[SocialSecurityNumber]
      ,[Suffix]
      ,[FirstName]
      ,[Middle]
      ,[LastName]
      ,[DateofBirth]
      ,[Gender]
      ,[Address]
      ,[Address2]
      ,[City]
      ,[State]
      ,[PostalCode]
      ,[PostalCode2]
      ,[BillingNumber]
      ,[BillingName]
      ,[BillingAddress]
      ,[BillingCity]
      ,[BillingState]
      ,[BillingPostalCode]
      ,[BillingPostalCode2]
  FROM [DevTest].[dbo].[Person]
  where ExternalPersonId = @personExt;";

            using (var conn = new SqlConnection(CONNECTION_STRING))
            {
                IEnumerable<Person> response;
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("personExt", personExtId);

                response = await conn.QueryAsync<Person>(sql, dynamicParameters);

                return response;
            }
        }

        public async Task<int> GetClaimsCount(int personId)
        {
            string sql = @"SELECT COUNT(p.Id)
    from [DevTest].[dbo].[Person] p
    inner join [DevTest].[dbo].[Claims] c on  p.id = c.ExternalPersonId
  where p.Id = @personId";

            using(var conn = new SqlConnection(CONNECTION_STRING))
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("personId", personId);

                int response = await conn.QuerySingleAsync<int>(sql, dynamicParameters);

                return response;
            }
        }

        public async Task<IEnumerable<Person>> GetPersonDetails(int organizationId, string? extPersonId, string? email, string? firstName, string? lastName, string? phone)
        {
            string sql = @"SELECT p.[Id]
      ,p.[OrganizationId]
      ,p.[ExternalPersonId]
      ,[SubscriberNumber]
      ,[SubscriberId]
      ,[SocialSecurityNumber]
      ,[Suffix]
      ,[FirstName]
      ,[Middle]
      ,[LastName]
      ,[DateofBirth]
      ,[Gender]
      ,[Address]
      ,[Address2]
      ,[City]
      ,[State]
      ,[PostalCode]
      ,[PostalCode2]
      ,[BillingNumber]
      ,[BillingName]
      ,[BillingAddress]
      ,[BillingCity]
      ,[BillingState]
      ,[BillingPostalCode]
      ,[BillingPostalCode2]
	  ,e.[Email]
	  ,e.[Id] EmailId
	  ,ph.[Id] PhoneId
	  ,ph.[Phone]
  FROM [DevTest].[dbo].[Person] p
  inner join [DevTest].[dbo].[PersonEmail] e on p.Id = E.ExternalPersonId
  inner join [DevTest].[dbo].[PersonPhone] ph on p.Id = ph.ExternalPersonId
  where p.OrganizationId = @orgId";

            using (var conn = new SqlConnection(CONNECTION_STRING))
            {
                IEnumerable<Person> response;
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("orgId", organizationId);


                if (extPersonId != null)
                {
                    sql += " AND ExternalPersonId = @extPerson;";
                    dynamicParameters.Add("extPerson", extPersonId);
                }

                if (firstName != null)
                {
                    sql += " AND FirstName = @first;";
                    dynamicParameters.Add("first", firstName);
                }

                if (lastName != null)
                {
                    sql += " AND LastName = @last;";
                    dynamicParameters.Add("last", lastName);
                }

                response = await conn.QueryAsync<Person>(sql, dynamicParameters);

                return response;
            }
        }

      
    }
}