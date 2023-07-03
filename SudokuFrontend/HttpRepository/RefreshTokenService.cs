using Microsoft.AspNetCore.Components.Authorization;

namespace SudokuFrontend.HttpRepository {
    public class RefreshTokenService {
        private readonly AuthenticationStateProvider _authProvider;
        private readonly IAuthenticationService _authService;

        public RefreshTokenService(AuthenticationStateProvider authProvider, IAuthenticationService authService) {
            _authProvider = authProvider;
            _authService = authService;
        }

        public async Task<string> TryRefreshToken() {
            var authState = await _authProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user == null) {
                return string.Empty;
            }

            var expClaim = user.FindFirst(c => c.Type.Equals("exp"));

            if (expClaim == null) {
                return string.Empty;
            }

            var expTime = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(expClaim.Value));
            var timeUTC = DateTime.UtcNow;

            var diff = expTime - timeUTC;
            if (diff.TotalMinutes <= 10) {
                return await _authService.RefreshToken();
            }

            return string.Empty;
        }
    }
}
