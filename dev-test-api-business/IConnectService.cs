using dev_test_api_models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dev_test_api_business
{
    public interface IConnectService
    {
        Task<bool> ValidateLogin(string username, string password);
        string CreateJwtToken(string username);
    }
}
