using System.Net.Http.Json;
using StaffTrackShared.DTOs;
namespace StaffTrackUI.Services;



public class DashboardService
{
    private readonly HttpClient _httpClient;

    public DashboardService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<ReportDTO>> GetRecentReportsAsync()
    {
        var response = await _httpClient.GetAsync("api/reports/recent");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<List<ReportDTO>>() ?? new List<ReportDTO>();
        }
        return new List<ReportDTO>();
    }

    public async Task<ProgressDTO> GetProgressAsync()
    {
        var response = await _httpClient.GetAsync("api/progress");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<ProgressDTO>()
                ?? new ProgressDTO { Percentage = 0, ReportCount = "0/0 reports", Streak = "0 weeks" };
        }
        return new ProgressDTO { Percentage = 0, ReportCount = "0/0 reports", Streak = "0 weeks" };
    }

    public async Task<List<NotificationDTO>> GetNotificationsAsync()
    {
        var response = await _httpClient.GetAsync("api/notifications");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<List<NotificationDTO>>() ?? new List<NotificationDTO>();
        }
        return new List<NotificationDTO>();
    }

    public async Task<bool> MarkAllNotificationsAsReadAsync()
    {
        var response = await _httpClient.PostAsync("api/notifications/mark-all-read", null);
        return response.IsSuccessStatusCode;
    }
}