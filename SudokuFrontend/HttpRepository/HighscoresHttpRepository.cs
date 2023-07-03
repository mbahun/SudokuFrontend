using DataTransferObjects;
using System.Text.Json;

namespace SudokuFrontend.HttpRepository {
    public class HighscoresHttpRepository : IHighscoresHttpRepository {

        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _options;

        public HighscoresHttpRepository(HttpClient client) {
            _client = client;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<List<HighscoreDto>> GetHighscores() {
            var response = await _client.GetAsync("highscores");
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) {
                throw new Exception(responseContent);
            }
            
            List<HighscoreDto> highscores;

            try {
                highscores = JsonSerializer.Deserialize<List<HighscoreDto>>(responseContent, _options);
            }
            catch (Exception e) {
                throw new Exception($"Failed to parse highscores response {e.Message}.");
            }

            if (highscores == null) {
                throw new Exception("Highscores deserialized to null.");
            }

            return highscores;
        }
    }
}
