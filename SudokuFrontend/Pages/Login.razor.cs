using DataTransferObjects;
using Microsoft.AspNetCore.Components;
using SudokuFrontend.HttpRepository;

namespace SudokuFrontend.Pages {
    public partial class Login {
        private UserForAuthenticationDto _userForAuthentication = new UserForAuthenticationDto();
        [Inject]
        public IAuthenticationService AuthenticationService { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        public bool ShowAuthError { get; set; }
        public string Error { get; set; }


        public async Task ExecuteLogin() { 
            ShowAuthError = false;

            try {
                await AuthenticationService.Login(_userForAuthentication);
            }
            catch(Exception ex) { 
                Error = ex.Message;
                ShowAuthError = true;
                return;
            }
            
            NavigationManager.NavigateTo("/sudoku");
        }

    }
}
