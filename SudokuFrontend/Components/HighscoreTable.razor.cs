using DataTransferObjects;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace SudokuFrontend.Components {
    public partial class HighscoreTable {
        [Parameter]
        public List<HighscoreDto> Highscores { get; set; }
    }
}
