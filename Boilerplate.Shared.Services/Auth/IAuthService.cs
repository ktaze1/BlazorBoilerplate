using Boilerplate.Shared.Models;
using Boilerplate.Shared.Models.Fundamentals;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Boilerplate.Shared.Services
{
    public interface IAuthService
    {
        Task<Response<LoginResultDto>> Authenticate(LoginDto loginRequest);

        Task Register(RegisterDto registerRequest);

        Task Logout();

        Task<CurrentUser> CurrentUserInfo();
    }
}
