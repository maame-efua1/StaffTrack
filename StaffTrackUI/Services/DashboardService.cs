using System.Net.Http.Json;
using StaffTrackShared.DTOs;
namespace StaffTrackUI.Services;



public class DashboardService
{
    private readonly HttpClient _httpClient;
    private readonly LoginService _loginService;

    public DashboardService(HttpClient httpClient, LoginService loginService)
    {
        _httpClient = httpClient;
        _loginService = loginService;
    }
    

    public async Task<List<ReportDTO>> GetRecentReportsAsync()
    {
        var response = await _httpClient.GetAsync("report/recent");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<List<ReportDTO>>() ?? new List<ReportDTO>();
        }
        return new List<ReportDTO>();
    }

    public async Task<ProgressDTO> GetProgressAsync()
    {
        var response = await _httpClient.GetAsync("progress");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<ProgressDTO>()
                ?? new ProgressDTO { Percentage = 0, ReportCount = "0/0 reports", Streak = "0 weeks" };
        }
        return new ProgressDTO { Percentage = 0, ReportCount = "0/0 reports", Streak = "0 weeks" };
    }

    public async Task<List<NotificationDTO>> GetNotificationsAsync()
    {
        var response = await _httpClient.GetAsync("notification");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<List<NotificationDTO>>() ?? new List<NotificationDTO>();
        }
        return new List<NotificationDTO>();
    }

    public async Task<bool> MarkAllNotificationsAsReadAsync()
    {
        var response = await _httpClient.PostAsync("notification/mark-all-read", null);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> AddReportsAsync(List<ReportDTO> reports)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("report/addbulk", reports);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    private class ApiResponse<T>
    {
        public string Message { get; set; } = string.Empty;
        public T Data { get; set; } = default!;
    }

    public async Task<List<ReportDTO>> GetAllReportsAsync()
    {
        // Get the current user's ID from LoginService
        var userId = _loginService.CurrentUser?.Id;
        if (string.IsNullOrEmpty(userId))
        {
            // Return empty list if user is not authenticated
            return new List<ReportDTO>();
        }

        try
        {
            // Call the API endpoint with userId
            var response = await _httpClient.GetAsync($"Report/user/{userId}");
            if (response.IsSuccessStatusCode)
            {
                // Deserialize the wrapped response
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<ReportDTO>>>();
                return apiResponse?.Data ?? new List<ReportDTO>();
            }

            // Handle NotFound (404)
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new List<ReportDTO>();
            }

            // Throw exception for other errors
            throw new HttpRequestException($"Failed to fetch reports: {response.ReasonPhrase}");
        }
        catch (Exception ex)
        {
            // Log the error (logging not implemented here, add if needed)
            throw new HttpRequestException($"Error fetching reports: {ex.Message}", ex);
        }
    }
}