using Dapper;
using dev_test_api_models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dev_test_api_data_access
{
    public class ConnectDataAccess : IConnectDataAccess
    {
        private readonly IConfiguration configuration;
        private readonly string CONNECTION_STRING;

        public ConnectDataAccess(IConfiguration config)
        {
            configuration = config;
            CONNECTION_STRING = configuration["SqlServerConnectionString"] ?? string.Empty;
        }
        public async Task<IEnumerable<UserLoginData>> GetUserData(string username)
        {

            using (var conn = new SqlConnection(CONNECTION_STRING))
            {
                string sql = @"SELECT [Id]
    ,[UserName]
    ,[Password]
    ,[Salt]
FROM [DevTest].[dbo].[Login]
WHERE UserName = @UserName";

                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("UserName", username);
                var res = await conn.QueryAsync<UserLoginData>(sql, dynamicParameters);

                return res;
            }
        }

        public void RegisterUser(string username, byte[] hash, byte[] salt)
        {
            using(var conn = new SqlConnection(CONNECTION_STRING))
            {
                string sql = @"INSERT INTO [DevTest].[dbo].[Login] ([Id]
    ,[UserName]
    ,[Password]
    ,[Salt])
VALUES(@id, @u, @p, @s)";

                conn.Execute(sql, new {id = 1, u = username, p = hash, s = salt});
            }
            
        }
    }
}
