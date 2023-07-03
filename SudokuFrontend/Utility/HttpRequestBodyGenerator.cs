using System.Text.Json;
using System.Text;

namespace SudokuFrontend.Utility {
    public static class HttpRequestBodyGenerator<T> {
        public static StringContent GetBody(T dtoObject, string mediaType="application/json") {
            var content = JsonSerializer.Serialize(dtoObject);
            var bodyContent = new StringContent(content, Encoding.UTF8, mediaType);
            return bodyContent;
        }
    }
}
