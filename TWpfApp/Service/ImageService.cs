using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using TWpfApp.Models;

namespace TWpfApp.Services
{
    public class ImageService
    {
        private readonly HttpClient _httpClient;

        public ImageService()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:5002/api/images/") };
        }

        public async Task<IEnumerable<ImageItem>> GetImagesAsync() =>
            await _httpClient.GetFromJsonAsync<ImageItem[]>("all") ?? [];

        public async Task<bool> AddImageAsync(ImageItem image) =>
            (await _httpClient.PostAsJsonAsync("add", image)).IsSuccessStatusCode;

        public async Task<bool> UpdateImageAsync(int id, string filePath)
        {
            await using var stream = new FileStream(filePath, FileMode.Open);
            var content = new MultipartFormDataContent
            {
                { new StreamContent(stream), "file", Path.GetFileName(filePath) }
            };

            var response = await _httpClient.PutAsync($"update/{id}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteImageAsync(int id) =>
            (await _httpClient.DeleteAsync($"delete/{id}")).IsSuccessStatusCode;
    }
}
