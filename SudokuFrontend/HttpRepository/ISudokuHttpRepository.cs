using DataTransferObjects;

namespace SudokuFrontend.HttpRepository {
    public interface ISudokuHttpRepository {
        Task CreateNewGameAsync();
        Task<GameDto> PlayNextGameAsync();
        Task<bool> IsUserGameSolutionOkAsync(SolutionForCheckingDto solution); 
        Task<bool> SaveChangesForPatchAsync(UserGameForUpdateDto solution);
    }
}
