using DataTransferObjects;
using Microsoft.AspNetCore.Components;
using SudokuFrontend.HttpRepository;

namespace SudokuFrontend.Pages {
    public partial class Highscores {
        public List<HighscoreDto> HighscoresList { get; set; } = new List<HighscoreDto>();
        [Inject]
        public IHighscoresHttpRepository HighscoresRepo { get; set; }
        public bool ShowScoreError { get; set; }
        public string Error { get; set; }
        protected async override Task OnInitializedAsync() {
            ShowScoreError = false;

            try {
                HighscoresList = await HighscoresRepo.GetHighscores();
            }
            catch (Exception ex) {
                Error = ex.Message;
                ShowScoreError = true;
            }
        }

    }
}
