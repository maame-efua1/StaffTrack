using System.Threading.Tasks;
using Microsoft.JSInterop;


namespace StaffTrackUI.Services;
public class AuthService
{
    private readonly IJSRuntime _jsRuntime;

    public AuthService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<bool> IsUserAuthenticatedAsync()
    {
        var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "AuthToken");

        // Check if the token exists and is not empty
        return !string.IsNullOrEmpty(token);
    }

    public async Task<string?> GetTokenAsync()
    {
        return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "AuthToken");
    }

    public async Task LogoutAsync()
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "AuthToken");
    }
}
