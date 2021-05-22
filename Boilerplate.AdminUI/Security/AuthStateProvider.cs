using Blazored.SessionStorage;
using Boilerplate.Shared.Models;
using Boilerplate.Shared.Models.Fundamentals;
using Boilerplate.Shared.Services;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Boilerplate.AdminUI.Security
{
    public class AuthStateProvider : AuthenticationStateProvider
    {
        private readonly IAuthService authService;
        private ISessionStorageService sessionStorage;
        private readonly HttpClient _HttpClient;
        private Response<LoginResultDto> userResult;
        private CurrentUser _currentUser;

        public AuthStateProvider(IAuthService authService, ISessionStorageService sessionStorage, HttpClient httpClient)
        {
            this.authService = authService;
            this.sessionStorage = sessionStorage;
            _HttpClient = httpClient;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var loginmodel = new LoginDto();
            loginmodel.Email = await sessionStorage.GetItemAsync<string>("username");
            loginmodel.Password = await sessionStorage.GetItemAsync<string>("password");

            ClaimsIdentity identity;

            if (loginmodel.Email != null && loginmodel.Password != null)
            {
                userResult = await authService.Authenticate(loginmodel); // AUTH SERVER'a giden metod
                identity = GetClaimsIdentity(userResult.Result);
            }
            else
            {
                identity = new ClaimsIdentity();
            }

            var claimsPrincipal = new ClaimsPrincipal(identity);

            return await Task.FromResult(new AuthenticationState(claimsPrincipal)); // marks user as authenticated even when empty object
        }

        public async Task MarkUserAsAuthenticated(LoginDto model)
        {
            userResult = await authService.Authenticate(model);

            var identity = GetClaimsIdentity(userResult.Result);

            var claimsPrincipal = new ClaimsPrincipal(identity);

            await sessionStorage.SetItemAsync("accessToken", userResult.Result.AccessToken);
            _HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", userResult.Result.AccessToken);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }

        public async Task MarkUserAsLoggedOut()
        {
            await sessionStorage.RemoveItemAsync("username");
            await sessionStorage.RemoveItemAsync("password");

            var identity = new ClaimsIdentity();
            var user = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        private ClaimsIdentity GetClaimsIdentity(LoginResultDto userAuth)
        {
            var claimsIdentity = new ClaimsIdentity();

            if (userAuth != null)
            {
                claimsIdentity = new ClaimsIdentity(new[]
                                {
                                    new Claim(ClaimTypes.Name, userAuth.UserId.ToString()),
                                }, "apiauth_type");
                //foreach (var item in userAuth.Roles)
                //{
                //    claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, item));
                //}
            }

            return claimsIdentity;
        }
    }
}
