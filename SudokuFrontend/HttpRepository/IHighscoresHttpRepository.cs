using DataTransferObjects;

namespace SudokuFrontend.HttpRepository {
    public interface IHighscoresHttpRepository {
        Task<List<HighscoreDto>> GetHighscores();
    }
}
