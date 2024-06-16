using ClosirisTest.DTOs;
using System.Net.Http;
using System.Net.Http.Json;
using Newtonsoft.Json;
using Xunit;
using System.Net;

namespace ClosirisTest
{
    public class AuthTest : IDisposable
    {
        private readonly HttpClient _client;


        public AuthTest(){
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Add("accept", "application/json");
            _client.BaseAddress = new Uri("http://localhost:5089");
        }

        [Fact]
        public async Task Login_Successful(){
            UserModel userModel = new UserModel{
                Email = "alexsandermarin@outlook.com",
                Password = "123Ale_",
            };

            var result = await _client.PostAsJsonAsync("api", userModel);
            result.EnsureSuccessStatusCode();

            var response = await result.Content.ReadAsStringAsync();
            var responseJson = JsonConvert.DeserializeObject<Dictionary<string, string>>(response);

            Assert.True(responseJson.ContainsKey("token"));
        }


        [Fact]
        public async Task Login_Failed(){
            UserModel userModel = new UserModel{
                Email = "alexsandermarin@outlook.com",
                Password = "123mAR_",
            };

            var result = await _client.PostAsJsonAsync("api", userModel);
            Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
        }

        public void Dispose(){
            _client.Dispose();
        }
    }
}