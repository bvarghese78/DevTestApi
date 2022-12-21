using dev_test_api_models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dev_test_api_data_access
{
    public interface IConnectDataAccess
    {
        Task<IEnumerable<UserLoginData>> GetUserData(string username);
        void RegisterUser(string username, byte[] hash, byte[] salt);  
    }
}
