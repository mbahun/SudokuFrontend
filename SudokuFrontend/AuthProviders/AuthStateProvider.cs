using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;
using System.Security.Claims;
using System.Net.Http.Headers;
using SudokuFrontend.Utility;

namespace SudokuFrontend.AuthProviders
{
    public class AuthStateProvider : AuthenticationStateProvider {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationState _anonymous;


        public AuthStateProvider(HttpClient httpClient, ILocalStorageService localStorage) {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }


        public override async Task<AuthenticationState> GetAuthenticationStateAsync() {
            string? token;
            try {
                //https://github.com/dotnet/aspnetcore/issues/28684
                token = await _localStorage.GetItemAsync<string>("authToken");
            }catch {
                return _anonymous;
            }

            if (string.IsNullOrWhiteSpace(token)) {
                return _anonymous;
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(token), "jwtAuthType")));
        }


        /*
        public void NotifyUserAuthentication(string userName) {
            var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, userName) }, "jwtAuthType"));
            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            NotifyAuthenticationStateChanged(authState);
        }
        */

        public void NotifyUserAuthentication(string token) {
            var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(token), "jwtAuthType"));
            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            NotifyAuthenticationStateChanged(authState);
        }


        public void NotifyUserLogout() {
            var authState = Task.FromResult(_anonymous);
            NotifyAuthenticationStateChanged(authState);
        }
    }
}
