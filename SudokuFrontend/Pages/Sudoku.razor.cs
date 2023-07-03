using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using SudokuFrontend.HttpRepository;
using DataTransferObjects;
using Microsoft.JSInterop;


namespace SudokuFrontend.Pages {
    public partial class Sudoku {
        [Inject]
        public HttpInterceptorService Interceptor { get; set; }
        [Inject]
        private ISudokuHttpRepository SudokuRepository { get; set; }
        private string[] _cellValues = new string[120];
        private string Error { get; set; }
        private bool ShowErrors { get; set; }
        protected override void OnInitialized() {
            base.OnInitialized();

            Interceptor.RegisterEvent();
            ShowErrors = false;

            for (int i = 0; i < 81; i++) {
                _cellValues[i] = "";
            }
        }


        private Task OnCellChanged(ChangeEventArgs e, int i) {
            int index = 0, value=0;

            if (int.TryParse(i.ToString(), out index)) {
                
                if(e.Value != null && int.TryParse(e.Value.ToString(), out value)) {
                    if(value > 0 && value <= 9) {
                        _cellValues[index] = value.ToString();
                        return Task.CompletedTask;
                    }
                }
                
                _cellValues[index] = "";
            }    
            return Task.CompletedTask;
        }


        private void Fill(string base64) {
            int value;
            byte[] bytes;

            try {
                bytes = Convert.FromBase64String(base64);
                if (bytes.Length != 81) {
                    throw new Exception("Input game size is wrong!");
                }

                for (int i = 0; i < 81; i++) {
                    _cellValues[i] = "";

                    if (bytes[i] != 0) {
                        _cellValues[i] = bytes[i].ToString();
                    }
                }
            } 
            catch (Exception e){
                ShowErrors = true;
                Error = e.Message;
            }
        }


        private string MakeBase64() {
            byte[] bytes = new byte[81];
            byte value;

            for (int i=0; i<81; i++) {
                if (byte.TryParse(_cellValues[i], out value)) {
                    bytes[i] = value;
                }
                else {
                    bytes[i] = 0;
                }
            }

            return Convert.ToBase64String(bytes); 
        }


        private async Task CheckResult() {
            var solutionBase64 = MakeBase64();
            bool result;

            try {
                result = await SudokuRepository.IsUserGameSolutionOkAsync(
                    new SolutionForCheckingDto { Solution = solutionBase64 });
            }
            catch (Exception e) {
                ShowErrors = true;
                Error = e.Message;
                return;
            }

            if (!result) {
                await JsRuntime.InvokeVoidAsync("alert", "Result is not ok.");
            }
            else {
                await JsRuntime.InvokeVoidAsync("alert", "Great, you solve it!");
            }
        }


        private async Task SaveResult() {
            var solutionBase64 = MakeBase64();

            bool result = await SudokuRepository.SaveChangesForPatchAsync(
                new UserGameForUpdateDto { Solution = solutionBase64 });

            if (!result) {
                await JsRuntime.InvokeVoidAsync("alert", "Result is not ok and game is not saved");
            }
            else {
                await JsRuntime.InvokeVoidAsync("alert", "Great, you earned scores for this one!");
                await PlayNew();
            }
        }


        private async Task PlayNew() {
            GameDto newGame;

            try {
                await SudokuRepository.CreateNewGameAsync();
                newGame = await SudokuRepository.PlayNextGameAsync();
            }
            catch (Exception e) {
                ShowErrors = true;
                Error = e.Message;
                return;
            }

            Fill(newGame.Problem);
        }

        public void Dispose() {
            Interceptor.DisposeEvent();
        }
    }
}
