using BattleshipsApp.Model;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;

namespace BattleshipsApp.Classes
{
    public static class ApiHandler
    {
        private const string _baseUrlProduction = "https://v2.checkersapi.site/";
        private const string _baseUrlDebug = "http://localhost/";
        private static bool IsDebug = true;
        private static string _baseUrl = (IsDebug ? _baseUrlDebug : _baseUrlProduction) + "api/";
        public static string BaseSignalUrl = (IsDebug ? _baseUrlDebug : _baseUrlProduction) + "notifications";
        private static HttpClient _httpClient = new HttpClient();
        public static string? Login { get; private set; } = null;
        private static void HandleErrors(Tuple<int, string>? response = null)
        {
            if (response is null) throw new ConnectionException();
            int statusCode = response.Item1;
            string message = response.Item2;
            if (statusCode / 100 == 2) return;
            if (statusCode == 401 && message == "Плохой токен") throw new AuthorizationException();
            throw new ApiException(message);
        }
        private static async Task<Tuple<int, string>> PostAsync(string url, object? data = null)
        {
            url = _baseUrl + url;
            string serialised = JsonSerializer.Serialize(data);
            StringContent content = new StringContent(serialised, Encoding.UTF8, "application/json");
            HttpResponseMessage response = new HttpResponseMessage();
            try { response = await _httpClient.PostAsync(url, data is null ? new StringContent("") : content); }
            catch { HandleErrors(); }
            var tuple = Tuple.Create((int)(response.StatusCode), await response.Content.ReadAsStringAsync());
            HandleErrors(tuple);
            return tuple;
        }
        private static async Task<Tuple<int, string>> GetAsync(string url)
        {
            url = _baseUrl + url;
            HttpResponseMessage response = new HttpResponseMessage();
            try { response = await _httpClient.GetAsync(url); }
            catch { HandleErrors(); }
            var tuple = Tuple.Create((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            HandleErrors(tuple);
            return tuple;
        }
        private static async Task<Tuple<int, string>> DeleteAsync(string url)
        {
            url = _baseUrl + url;
            HttpResponseMessage response = new HttpResponseMessage();
            try { response = await _httpClient.DeleteAsync(url); }
            catch { HandleErrors(); }
            var tuple = Tuple.Create((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            HandleErrors(tuple);
            return tuple;
        }
        public static void Exit()
        {
            _httpClient.Dispose();
            _httpClient = new HttpClient();
            Login = null;
        }
        public static async Task Authorize(string login, string password)
        {
            var response = await PostAsync(
                "Players/authorize",
                new { Login = login, Password = password }
            );
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", response.Item2);
            Login = login;
        }
        public static async Task Register(string login, string password)
        {
            await PostAsync(
            "Players/register",
            new { Login = login, Password = password }
            );

        }
        public static async Task<Player> Me()
        {
            var response = await GetAsync(
                "Profile/me"
            );
            return JsonSerializer.Deserialize<Player>(response.Item2)!;
        }
        public static async Task ChangeName(string name)
        {
            await PostAsync(
            "Profile/changeName",
            name
            );
        }
        public static async Task ChangePassword(string oldPassword, string newPassword)
        {
            await PostAsync(
            "Profile/changePassword",
            new { OldPassword = oldPassword, NewPassword = newPassword }
            );
        }
        public static async Task<List<GameDTO>> GetGames()
        {
            var response = await GetAsync("Game");
            return JsonSerializer.Deserialize<List<GameDTO>>(response.Item2)!;
        }
        public static async Task CreateGame()
        {
            await PostAsync("Game");
        }
        public static async Task Ping()
        {
            await GetAsync("Ping");
        }
        public static async Task ConnectToGame(string nickName)
        {
            await PostAsync("Game/connect", nickName);
        }
        public static async Task PostBoats(List<Boat> boats)
        {
            await PostAsync("Game/boats", boats);
        }

    }
}
