using Blazored.SessionStorage;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Boilerplate.Shared.Services
{
    public class ServiceBase
    {
        protected readonly HttpClient _httpClient;
        protected readonly ISessionStorageService _sessionStorage;

        public ServiceBase(HttpClient httpClient, ISessionStorageService sessionStorage)
        {
            _httpClient = httpClient;
            _sessionStorage = sessionStorage;
            InitService();
        }

        protected async void InitService()
        {
            if (_httpClient.DefaultRequestHeaders.Authorization == null)
            {
                var accesToken = await _sessionStorage.GetItemAsync<string>("accessToken");
                if (!string.IsNullOrEmpty(accesToken))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accesToken);
                }
            }
        }
    }
}
