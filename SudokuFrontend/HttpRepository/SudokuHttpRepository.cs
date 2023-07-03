using System;
using System.Text;
using System.Text.Json;
using DataTransferObjects;
using SudokuFrontend.Utility;

namespace SudokuFrontend.HttpRepository {
    public class SudokuHttpRepository : ISudokuHttpRepository {

        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _options;
        private string _currentGame;

        public SudokuHttpRepository(HttpClient client) {
            _client = client;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }


        public async Task CreateNewGameAsync() {
            var response = await _client.PostAsync("games", null);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) {
                throw new Exception(responseContent);
            }
        }


        public async Task<GameDto> PlayNextGameAsync() {
            var response = await _client.PostAsync("user_game", null);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) {
                throw new Exception(responseContent);
            }

            GameDto newGame;

            try {
                newGame = JsonSerializer.Deserialize<GameDto>(responseContent, _options);
            }
            catch (Exception e) {
                throw new Exception($"Failed to parse user game response {e.Message}.");
            }

            if (newGame == null) {
                throw new Exception("User game deserialized to null.");
            }

            _currentGame = newGame.id.ToString();
            return newGame;
        }


        public async Task<bool> IsUserGameSolutionOkAsync(SolutionForCheckingDto solution) {
            if (_currentGame == null) {
                return false;
            }

            var body = HttpRequestBodyGenerator<SolutionForCheckingDto>.GetBody(solution);
            var response = await _client.PostAsync("user_game/"+ _currentGame + "/solution", body);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) {
                throw new Exception(responseContent);
            }

            var ret = JsonSerializer.Deserialize<Dictionary<string, bool>>(responseContent);
            return ret != null ? ret["isSolutionOk"] : false;
        }


        public async Task<bool> SaveChangesForPatchAsync(UserGameForUpdateDto solution) {
            if (_currentGame == null) {
                return false;

            }
            string content = $$"""
            [
                {
                "op": "replace",
                "path": "/solution",
                "value": "{{solution.Solution}}"
                }
            ]           
            """;
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json-patch+json");
            var response = await _client.PatchAsync("user_game/" + _currentGame, bodyContent);
            var resultContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) {
                return false;
            }

            return true;
        }
    }
}
