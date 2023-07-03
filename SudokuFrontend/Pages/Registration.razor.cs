using DataTransferObjects;
using Microsoft.AspNetCore.Components;
using SudokuFrontend.HttpRepository;

namespace SudokuFrontend.Pages {
    public partial class Registration {

        private UserForRegistrationDto _userForRegistration = new UserForRegistrationDto();
        [Inject]
        public IAuthenticationService AuthenticationService { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        public bool ShowRegistrationError { get; set; }
        public string Error { get; set; }

        public async Task Register() {
            ShowRegistrationError = false;

            try {
                await AuthenticationService.RegisterUser(_userForRegistration);
            }
            catch (Exception ex) { 
                Error = ex.Message;
                ShowRegistrationError = true;
                return;
            }
         
            NavigationManager.NavigateTo("/");
        }


    }
}
