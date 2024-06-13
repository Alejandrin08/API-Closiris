using ClosirisTest.DTOs;
using System.Net.Http;
using System.Net.Http.Json;
using Newtonsoft.Json;
using Xunit;
using System.Net;
using System.Net.Http.Headers;
using Xunit.Abstractions;

namespace ClosirisTest
{
    public class FileTest : IDisposable
    {
        private readonly HttpClient _client;
        private readonly ITestOutputHelper _output;

        public FileTest(ITestOutputHelper output)
        {
            _output = output;
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Add("accept", "application/json");
            _client.BaseAddress = new Uri("http://localhost:5089");
        }

      
        public async Task InsertFile_Successful()
        {

            UserModel userModel1 = new UserModel
            {
                Email = "momaosiris@gmail.com",
                Password = "123Pal_",
            };

            var result1 = await _client.PostAsJsonAsync("api", userModel1);
            result1.EnsureSuccessStatusCode();


             var response1 = await result1.Content.ReadAsStringAsync();
            var responseJson1 = JsonConvert.DeserializeObject<Dictionary<string, string>>(response1);

            Singleton.Instance.Token = responseJson1["token"];
           
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Singleton.Instance.Token);

            var fileContent = new ByteArrayContent(System.IO.File.ReadAllBytes("C:\\Users\\Palom\\Desktop\\Proyecto\\API-Closiris\\API-Closiris\\ClosirisTest\\Files\\pulpo.txt"));

            var formData = new MultipartFormDataContent
            {
                { fileContent, "file", "pulpo.txt" }
            };

            _client.DefaultRequestHeaders.Add("folder_name", "TestFolder");

            var response = await _client.PostAsync("api/File", formData);

            var responseContent = await response.Content.ReadAsStringAsync();
            _output.WriteLine("Response: " + responseContent);

            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

       
        public async Task InsertFile_Failed()
        {

            var fileContent = new ByteArrayContent(System.IO.File.ReadAllBytes("C:\\Users\\Palom\\Desktop\\Proyecto\\API-Closiris\\API-Closiris\\ClosirisTest\\Files\\pulpo.txt"));

            var formData = new MultipartFormDataContent
            {
                { fileContent, "file", "pulpo.txt" }
            };

            _client.DefaultRequestHeaders.Add("folder_name", "TestFolder");

            var response = await _client.PostAsync("api/File", formData);

            var responseContent = await response.Content.ReadAsStringAsync();
            

           
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        
        public async Task InsertFileOwner_Successful()
        {

            UserModel userModel1 = new UserModel
            {
                Email = "momaosiris@gmail.com",
                Password = "123Pal_",
            };

            var result1 = await _client.PostAsJsonAsync("api", userModel1);
            result1.EnsureSuccessStatusCode();


             var response1 = await result1.Content.ReadAsStringAsync();
            var responseJson1 = JsonConvert.DeserializeObject<Dictionary<string, string>>(response1);

            Singleton.Instance.Token = responseJson1["token"];
           
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Singleton.Instance.Token);


            _client.DefaultRequestHeaders.Add("file_id", "1");

            var response = await _client.PostAsync("api/FileOwner", null);

            var responseContent = await response.Content.ReadAsStringAsync();
            _output.WriteLine("Response: " + responseContent);

            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        
        public async Task InsertFileOwner_Failed()
        {

            _client.DefaultRequestHeaders.Add("file_id", "2");

            var response = await _client.PostAsync("api/FileOwner", null);

            var responseContent = await response.Content.ReadAsStringAsync();
            _output.WriteLine("Response: " + responseContent);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

      
        public async Task InsertFileShared_Successful()
        {

            UserModel userModel1 = new UserModel
            {
                Email = "momaosiris@gmail.com",
                Password = "123Pal_",
            };

            var result1 = await _client.PostAsJsonAsync("api", userModel1);
            result1.EnsureSuccessStatusCode();


             var response1 = await result1.Content.ReadAsStringAsync();
            var responseJson1 = JsonConvert.DeserializeObject<Dictionary<string, string>>(response1);

            Singleton.Instance.Token = responseJson1["token"];
           
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Singleton.Instance.Token);
            ///exista el otro user
            _client.DefaultRequestHeaders.Add("shared_id", "2");
            _client.DefaultRequestHeaders.Add("file_id", "1");

            var response = await _client.PostAsync("api/FileShared", null);

            var responseContent = await response.Content.ReadAsStringAsync();
            _output.WriteLine("Response: " + responseContent);

            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        
        public async Task InsertFileShared_Failed()
        {
            ///exista el otro user
            _client.DefaultRequestHeaders.Add("shared_id", "1");
            _client.DefaultRequestHeaders.Add("file_id", "1");

            var response = await _client.PostAsync("api/FileShared", null);

            var responseContent = await response.Content.ReadAsStringAsync();
            _output.WriteLine("Response: " + responseContent);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

   
        public async Task DeleteFileShared_Successful()
        {

            UserModel userModel1 = new UserModel
            {
                Email = "momaosiris@gmail.com",
                Password = "123Pal_",
            };

            var result1 = await _client.PostAsJsonAsync("api", userModel1);
            result1.EnsureSuccessStatusCode();


             var response1 = await result1.Content.ReadAsStringAsync();
            var responseJson1 = JsonConvert.DeserializeObject<Dictionary<string, string>>(response1);

            Singleton.Instance.Token = responseJson1["token"];
           
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Singleton.Instance.Token);
            ///exista el otro user
            _client.DefaultRequestHeaders.Add("file_id", "1");

            var response = await _client.DeleteAsync("api/FileShared");

            var responseContent = await response.Content.ReadAsStringAsync();
            _output.WriteLine("Response: " + responseContent);

            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

      
        public async Task DeleteFileShared_fAILED()
        {

            UserModel userModel1 = new UserModel
            {
                Email = "momaosiris@gmail.com",
                Password = "123Pal_",
            };

            var result1 = await _client.PostAsJsonAsync("api", userModel1);
            result1.EnsureSuccessStatusCode();


             var response1 = await result1.Content.ReadAsStringAsync();
            var responseJson1 = JsonConvert.DeserializeObject<Dictionary<string, string>>(response1);

            Singleton.Instance.Token = responseJson1["token"];
           
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Singleton.Instance.Token);
            ///exista el otro user
            _client.DefaultRequestHeaders.Add("file_id", "10");

            var response = await _client.DeleteAsync("api/FileShared");

            var responseContent = await response.Content.ReadAsStringAsync();
            _output.WriteLine("Response: " + responseContent);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }



        public void Dispose()
        {
            _client.Dispose();
        }
    

    }

}