using dev_test_api_data_access;
using dev_test_api_models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace dev_test_api_business
{
    public class ConnectService : IConnectService
    {
        private IConnectDataAccess _dataAccess;
        private IConfiguration _config;

        public ConnectService(IConnectDataAccess connectDataAccess, IConfiguration configuration)
        {
            _dataAccess = connectDataAccess;
            _config = configuration;
        }

        public string CreateJwtToken(string username)
        {
            if (_config == null)
                return string.Empty;

            // Create token
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config["Authentication:SecretForKey"]));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Add Claims
            var claimsForToken = new List<Claim>();
            claimsForToken.Add(new Claim("sub", username));

            var jwtSecurityToken = new JwtSecurityToken(
                _config["Authentication:Issuer"],
                _config["Authentication:Audience"],
                claimsForToken,
                DateTime.UtcNow,
                DateTime.UtcNow.AddMinutes(10),
                signingCredentials);

            var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return token;
        }

        public async Task<bool> ValidateLogin(string username, string password)
        {
            var res = await _dataAccess.GetUserData(username);

            var user = res.FirstOrDefault(x => x.UserName == username);

            if (user == null)
                return false;

            if (!MatchPasswordHash(password, user.Password, user.Salt))
                return false;
            else
                return true;
        }

        private void Register(string username, string password)
        {
            byte[] hash, salt;

            using (var hmac = new HMACSHA512())
            {
                salt = hmac.Key;
                hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }

            _dataAccess.RegisterUser(username, hash, salt);
        }

        private bool MatchPasswordHash(string userProvidedPassword, byte[] hash, byte[] salt)
        {
            using (var hmac = new HMACSHA512(salt))
            {
                var passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(userProvidedPassword));
                
                for(int i = 0; i < passwordHash.Length; i++)
                {
                    if (passwordHash[i] != hash[i])
                        return false;
                }

                return true;
            }
        }
    }
}
