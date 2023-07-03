using DataTransferObjects;

namespace SudokuFrontend.HttpRepository {
    public interface IAuthenticationService {
        Task RegisterUser(UserForRegistrationDto userForRegistration);
        Task Login(UserForAuthenticationDto userForAuthentication);
        Task<string> RefreshToken();
        Task Logout();
    }
}
