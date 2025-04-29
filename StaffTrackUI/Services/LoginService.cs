using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using StaffTrackShared.DTOs;



namespace StaffTrackUI.Services;
public class LoginService
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;
    public UserDTO CurrentUser { get; private set; }

    public event Action OnUserChanged;

    public LoginService(HttpClient httpClient, IJSRuntime jsRuntime)
    {
        _httpClient = httpClient;
        _jsRuntime = jsRuntime;
    }

    public async Task<bool> TryLoginAsync(LoginDTO model)
    {
        var response = await _httpClient.PostAsJsonAsync("Auth/login", model);
        if (response.IsSuccessStatusCode)
        {
            var responseData = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            if (responseData != null && responseData.TryGetValue("token", out var token))
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "AuthToken", token);
                DecodeTokenAndSetUser(token);
                OnUserChanged?.Invoke(); // Notify subscribers
                return true;
            }
        }
        return false;
    }

    public async Task LoadUserFromLocalStorageAsync()
    {
        var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "AuthToken");
        if (!string.IsNullOrWhiteSpace(token))
        {
            DecodeTokenAndSetUser(token);
            OnUserChanged?.Invoke();
        }
    }

    private void DecodeTokenAndSetUser(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

        if (jwtToken != null)
        {
            var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var fullName = jwtToken.Claims.FirstOrDefault(x => x.Type == "FullName")?.Value;

            CurrentUser = new UserDTO
            {
               FullName = fullName
            };
        }
    }

    public async Task LogoutAsync()
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "AuthToken");
        CurrentUser = null;
        OnUserChanged?.Invoke();
    }

    public bool IsAuthenticated() => CurrentUser != null;
}
