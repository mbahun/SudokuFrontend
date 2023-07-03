using DataTransferObjects;
using System.Text.Json;
using System.Text;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using SudokuFrontend.AuthProviders;
using System.Net.Http.Headers;
using SudokuFrontend.Utility;

namespace SudokuFrontend.HttpRepository {
    public class AuthenticationService : IAuthenticationService {

        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _options;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly ILocalStorageService _localStorage;


        public AuthenticationService(HttpClient client, AuthenticationStateProvider authStateProvider, ILocalStorageService localStorage) {
            _client = client;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            _authStateProvider = authStateProvider;
            _localStorage = localStorage;
        }


        public async Task RegisterUser(UserForRegistrationDto userForRegistration) {
            userForRegistration.Roles = new string[] { "Player" };
            var body = HttpRequestBodyGenerator<UserForRegistrationDto>.GetBody(userForRegistration);
            var response = await _client.PostAsync("authentication", body);

            if(!response.IsSuccessStatusCode) {
                throw new Exception(response.Content.ReadAsStringAsync().Result);
            }            
        }


        public async Task Login(UserForAuthenticationDto userForAuthentication) {
            var body = HttpRequestBodyGenerator<UserForAuthenticationDto>.GetBody(userForAuthentication);
            var response = await _client.PostAsync("authentication/login", body);
            var responseContent = response.Content.ReadAsStringAsync().Result;
            TokenDto result;

            if (!response.IsSuccessStatusCode) {
                throw new Exception(responseContent);
            }

            try {
                result = JsonSerializer.Deserialize<TokenDto>(responseContent, _options);
            }
            catch (Exception e) {
                throw new Exception($"Failed to parse token response {e.Message}.");
            }

            await _localStorage.SetItemAsync("authToken", result.AccessToken);
            await _localStorage.SetItemAsync("refreshToken", result.RefreshToken);
            //((AuthStateProvider)_authStateProvider).NotifyUserAuthentication(userForAuthentication.UserName);
            ((AuthStateProvider)_authStateProvider).NotifyUserAuthentication(result.AccessToken);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", result.AccessToken);
        }


        public async Task<string> RefreshToken() {
            var accessToken = await _localStorage.GetItemAsync<string>("authToken");
            var refreshToken = await _localStorage.GetItemAsync<string>("refreshToken");

            if(accessToken==null || refreshToken == null) {
                return string.Empty;
            }

            TokenDto tokenDto = new TokenDto {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };

            var body = HttpRequestBodyGenerator<TokenDto>.GetBody(tokenDto);
            var response = await _client.PostAsync("token/refresh", body);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) {
                return string.Empty;
            }
            
            TokenDto tokenDtoResult;

            try {
                tokenDtoResult = JsonSerializer.Deserialize<TokenDto>(responseContent, _options);
            }
            catch {
                return string.Empty;
            }

            await _localStorage.SetItemAsync("authToken", tokenDtoResult.AccessToken);
            await _localStorage.SetItemAsync("refreshToken", tokenDtoResult.RefreshToken);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "bearer", tokenDtoResult.AccessToken);

            return tokenDtoResult.AccessToken;
        }


        public async Task Logout() {
            await _localStorage.RemoveItemAsync("authToken");
            await _localStorage.RemoveItemAsync("refreshToken");

            ((AuthStateProvider)_authStateProvider).NotifyUserLogout();
            _client.DefaultRequestHeaders.Authorization = null;
        }

    }
}
