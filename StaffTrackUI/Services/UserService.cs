//using EstateManagement.SharedLibrary.DTOs;
//using System.Net.Http.Json;
//using Interfaces;

//namespace Estatemanagement.Frontend_New.Services
//{
//    public class UserService : IUserService 
//    {
//        private readonly HttpClient _httpClient;

//        public UserService(HttpClient httpClient)
//        {
//            _httpClient = httpClient;
//        }


//        public async Task<List<AspNetRoleDTO>> GetAllRolesAsync()
//        {
//            return await _httpClient.GetFromJsonAsync<List<AspNetRoleDTO>>("api/user/GetAllRoles");
//        }

//        public async Task<string> AddRoleAsync(AspNetRoleDTO role)
//        {
//            var response = await _httpClient.PostAsJsonAsync("api/user/AddRole", role);
//            return await response.Content.ReadAsStringAsync();
//        }

//        public async Task<string> DeleteRoleAsync(int roleId)
//        {
//            var response = await _httpClient.DeleteAsync($"api/user/DeleteRole/{roleId}");
//            return await response.Content.ReadAsStringAsync();
//        }


//        public async Task<string> AssignRoleAsync(AspNetUserRoleDTO userRole)
//        {
//            var response = await _httpClient.PostAsJsonAsync("api/user/AssignRole", userRole);
//            return await response.Content.ReadAsStringAsync();
//        }

//        public async Task<string> RemoveUserRoleAsync(string userId, string roleId)
//        {
//            var response = await _httpClient.DeleteAsync($"api/user/RemoveUserRole/{userId}/{roleId}");
//            return await response.Content.ReadAsStringAsync();
//        }

//        public async Task<List<string>> GetUserRolesAsync(string userId)
//        {
//            return await _httpClient.GetFromJsonAsync<List<string>>($"api/user/GetUserRole/{userId}");
//        }


//        public async Task<List<AspNetUserDTO>> GetAllUsersWithRolesAsync()
//        {
//            return await _httpClient.GetFromJsonAsync<List<AspNetUserDTO>>("api/user/GetAllUsersWithRoles");
//        }

//        public async Task<List<AspNetUserDTO>> GetAllUsersAsync()
//        {
//            return await _httpClient.GetFromJsonAsync<List<AspNetUserDTO>>("api/user/GetAllUsers");
//        }

//        public async Task<AspNetUserDTO> GetUserByIdAsync(string userId)
//        {
//            return await _httpClient.GetFromJsonAsync<AspNetUserDTO>($"api/user/GetUserById/{userId}");
//        }

//        public async Task<string> UpdateUserAsync(string userId, AspNetUserDTO user)
//        {
//            var response = await _httpClient.PutAsJsonAsync($"api/user/UpdateUser/{userId}", user);
//            return await response.Content.ReadAsStringAsync();
//        }

//        public async Task<string> DeleteUserAsync(string userId)
//        {
//            var response = await _httpClient.DeleteAsync($"api/user/DeleteUser/{userId}");
//            return await response.Content.ReadAsStringAsync();
//        }
//    }
//}
