using System.Net.Http.Headers;
using System.Text.Json;

namespace IT_outCRM.Blazor.Services
{
    public class ProfileService : IProfileService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ProfileService> _logger;

        public ProfileService(HttpClient httpClient, ILogger<ProfileService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public void SetToken(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<string?> UploadAvatarAsync(Stream fileStream, string fileName)
        {
            try
            {
                using var content = new MultipartFormDataContent();
                using var streamContent = new StreamContent(fileStream);
                
                // Определяем content type по расширению файла
                var contentType = GetContentType(fileName);
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                
                content.Add(streamContent, "file", fileName);

                var response = await _httpClient.PostAsync("api/profile/avatar", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(jsonResponse);
                    
                    if (doc.RootElement.TryGetProperty("avatarUrl", out var avatarUrlElement))
                    {
                        return avatarUrlElement.GetString();
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Ошибка загрузки аватара: {StatusCode} - {Error}", response.StatusCode, errorContent);
                    throw new InvalidOperationException($"Ошибка загрузки аватара: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при загрузке аватара");
                throw;
            }

            return null;
        }

        public async Task<bool> DeleteAvatarAsync()
        {
            try
            {
                var response = await _httpClient.DeleteAsync("api/profile/avatar");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении аватара");
                return false;
            }
        }

        private string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                _ => "application/octet-stream"
            };
        }
    }
}


