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
    public class UserTest : IDisposable {
        private readonly HttpClient _client;
        private readonly ITestOutputHelper _output;

        public UserTest(ITestOutputHelper output) {
            _output = output;
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Add("accept", "application/json");
            _client.BaseAddress = new Uri("http://localhost:5089");
        }

       
       
        [Fact]
        public async Task CreateUserAccount_Successful() {
            UserModel userModel = new UserModel {
                Email = "alesisJesus@gmail.com",
                Password = "123Ale_",
                Name = "Alesis de jesús",
                ImageProfile = null,
            };

            var result = await _client.PostAsJsonAsync("api/userAccount", userModel);
            result.EnsureSuccessStatusCode();

            var response = await result.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.Created, result.StatusCode);
        }

        [Fact]
        public async Task CreateUserAccount_Failed() {
            UserModel userModel = new UserModel {
                Email = "hola",
                Password = "123Ale_",
                Name = "Alejandro Sanchez",
                ImageProfile = null,
            };

            var result = await _client.PostAsJsonAsync("api/userAccount", userModel);
            var responseContent = await result.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            var responseJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseContent);

            Assert.True(responseJson.ContainsKey("errors"));

            var errors = responseJson["errors"] as Newtonsoft.Json.Linq.JArray;
            var firstError = errors[0].ToObject<Dictionary<string, string>>();

            Assert.Equal("Please provide a valid email address", firstError["msg"]);
        }

        [Fact]
        public async Task CreateUser_Successful() {

            UserModel userModel = new UserModel {
                Email = "alesisJesus@gmail.com",
                Plan = "Premium",
                FreeStorage = 104857600
            };

            var result = await _client.PostAsJsonAsync("api/user", userModel);
            result.EnsureSuccessStatusCode();

            var response = await result.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.Created, result.StatusCode);
        }

        [Fact]
        public async Task CreateUser_Failed() {
            UserModel userModel = new UserModel {
                Email = "hola",
                Plan = "Premium",
                FreeStorage = 104857600
            };

            var result = await _client.PostAsJsonAsync("api/user", userModel);
            var responseContent = await result.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            var responseJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseContent);

            Assert.True(responseJson.ContainsKey("errors"));

            var errors = responseJson["errors"] as Newtonsoft.Json.Linq.JArray;
            var firstError = errors[0].ToObject<Dictionary<string, string>>();

            Assert.Equal("Please provide a valid email address", firstError["msg"]);
        }

        [Fact]
        public async Task UpdateUserAccount_Successful() {
            UserModel userModelLogin = new UserModel {
                Email = "gerry@gmail.com",
                Password = "123Gerry_",
            };

            var resultLogin = await _client.PostAsJsonAsync("api", userModelLogin);
            resultLogin.EnsureSuccessStatusCode();

            var responseLogin = await resultLogin.Content.ReadAsStringAsync();
            var responseJsonLogin = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseLogin);

            Singleton.Instance.Token = responseJsonLogin["token"];
           
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Singleton.Instance.Token);

            UserModel userModel = new UserModel{
                Email = "sanchezMarin@gmail.com",
                Name = "Gerry Sanchez Marin",
                ImageProfile = "",
            };

            var result = await _client.PutAsJsonAsync("api/UserAccount", userModel);

            result.EnsureSuccessStatusCode(); 
            var response = await result.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public async Task UpdateUserAccount_Failed(){
            UserModel userModelLogin = new UserModel{
                Email = "gerry@gmail.com",
                Password = "123Gerry_",
            };

            var resultLogin = await _client.PostAsJsonAsync("api", userModelLogin);
            resultLogin.EnsureSuccessStatusCode();

            var responseLogin = await resultLogin.Content.ReadAsStringAsync();
            var responseJsonLogin = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseLogin);

            Singleton.Instance.Token = responseJsonLogin["token"];
           
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Singleton.Instance.Token);

            UserModel userModel = new UserModel{
                Email = "sanchez",
                Name = "Gerry Sanchez Marin",
                ImageProfile = "",
            };

            var result = await _client.PutAsJsonAsync("api/UserAccount", userModel);
            var responseContent = await result.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            var responseJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseContent);

            Assert.True(responseJson.ContainsKey("errors"));

            var errors = responseJson["errors"] as Newtonsoft.Json.Linq.JArray;
            var firstError = errors[0].ToObject<Dictionary<string, string>>();

            Assert.Equal("Please provide a valid email address", firstError["msg"]);
        }

        [Fact]
        public async Task UpdateUserAccount_Unauthorized(){
            UserModel userModel = new UserModel{
                Email = "alex@outlook.com",
                Name = "Alejandro Sanchez Marin",
                ImageProfile = "",
            };

            var result = await _client.PutAsJsonAsync("api/UserAccount", userModel);

            Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [Fact]
        public async Task UpdatePassword_Successful() {
            UserModel userModel = new UserModel {
                Email = "momaosiris@gmail.com",
                Password = "123Pal_",
            };

            var result = await _client.PatchAsJsonAsync("api/Password", userModel);
            result.EnsureSuccessStatusCode();

            var response = await result.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public async Task UpdatePassword_Failed() {
            UserModel userModel = new UserModel {
                Email = "momaosiris@gmail.com",
                Password = "",
            };

            var result = await _client.PatchAsJsonAsync("api/Password", userModel);
            var responseContent = await result.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            var responseJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseContent);

            Assert.True(responseJson.ContainsKey("errors"));

            var errors = responseJson["errors"] as Newtonsoft.Json.Linq.JArray;
            var firstError = errors[0].ToObject<Dictionary<string, string>>();

            Assert.Equal("Password must comply with a password policy ", firstError["msg"]);  
        }

        [Fact]
        public async Task GetUserInfoByEmail_Successful() {
            UserModel userModelLogin = new UserModel {
                Email = "momaosiris@gmail.com",
                Password = "123Pal_",
            };

            var resultLogin = await _client.PostAsJsonAsync("api", userModelLogin);
            resultLogin.EnsureSuccessStatusCode();

            var responseLogin = await resultLogin.Content.ReadAsStringAsync();
            var responseJsonLogin = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseLogin);

            Singleton.Instance.Token = responseJsonLogin["token"];

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Singleton.Instance.Token);
            UserModel userModel = new UserModel {
                Name = "Paloma",
                Id = 3,
            };
            string email = "momaosiris@gmail.com";    

            var getResult = await _client.GetAsync($"api/Info/{email}");
            getResult.EnsureSuccessStatusCode();

            var response = await getResult.Content.ReadAsStringAsync();
            var userInfo = JsonConvert.DeserializeObject<UserModel>(response);

            Assert.Equal(HttpStatusCode.OK, getResult.StatusCode);
            Assert.Equal(userModel.Email, userInfo.Email);
            Assert.Equal(userModel.Name, userInfo.Name);
        }

        [Fact]
        public async Task GetUserInfoByEmail_Failed() {
            UserModel userModelLogin = new UserModel {
                Email = "momaosiris@gmail.com",
                Password = "123Pal_",
            };

            var resultLogin = await _client.PostAsJsonAsync("api", userModelLogin);
            resultLogin.EnsureSuccessStatusCode();

            var responseLogin = await resultLogin.Content.ReadAsStringAsync();
            var responseJsonLogin = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseLogin);

            Singleton.Instance.Token = responseJsonLogin["token"];

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Singleton.Instance.Token);
            
            string email = "momaosiris";    

            var result = await _client.GetAsync($"api/Info/{email}");
            var responseContent = await result.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            var responseJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseContent);

            Assert.True(responseJson.ContainsKey("errors"));

            var errors = responseJson["errors"] as Newtonsoft.Json.Linq.JArray;
            var firstError = errors[0].ToObject<Dictionary<string, string>>();

            Assert.Equal("Please provide a valid email address", firstError["msg"]);
        }

        [Fact]
        public async Task GetUserInfoByEmail_Unauthorized() {
            string email = "momaosiris@gmail.com";

            var getResult = await _client.GetAsync($"api/Info/{email}");

            Assert.Equal(HttpStatusCode.Unauthorized, getResult.StatusCode);
        }

        [Fact]
        public async Task ValidateEmailDuplicity_Successful() {
            string email = "dbaez105@gmail.com";    

            var getResult = await _client.GetAsync($"api/EmailDuplicity/{email}");
            getResult.EnsureSuccessStatusCode();

            var response = await getResult.Content.ReadAsStringAsync();
            var responseJson = JsonConvert.DeserializeObject<Dictionary<string, string>>(response);

            Assert.Equal(HttpStatusCode.OK, getResult.StatusCode);
            Assert.Equal(responseJson["message"], "Email available");
        }

        [Fact]
        public async Task ValidateEmailDuplicity_Failed() {
            string email = "momaosiris@gmail.com";    

            var getResult = await _client.GetAsync($"api/EmailDuplicity/{email}");
            getResult.EnsureSuccessStatusCode();

            var response = await getResult.Content.ReadAsStringAsync();
            var responseJson = JsonConvert.DeserializeObject<Dictionary<string, string>>(response);

            Assert.Equal(HttpStatusCode.OK, getResult.StatusCode);
            Assert.Equal(responseJson["user"], "momaosiris@gmail.com");
        }

        [Fact]
        public async Task UpdateFreeStorage_Successful() {
            UserModel userModelLogin = new UserModel {
                Email = "momaosiris@gmail.com",
                Password = "123Pal_",
            };

            var resultLogin = await _client.PostAsJsonAsync("api", userModelLogin);
            resultLogin.EnsureSuccessStatusCode();

            var responseLogin = await resultLogin.Content.ReadAsStringAsync();
            var responseJsonLogin = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseLogin);

            Singleton.Instance.Token = responseJsonLogin["token"];

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Singleton.Instance.Token);
            
            var data = new {
                freeStorage = 222222
            };

            var result = await _client.PatchAsJsonAsync("api/FreeStorage", data);
            result.EnsureSuccessStatusCode();

            var response = await result.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public async Task UpdateFreeStorage_Failed() {
             UserModel userModelLogin = new UserModel {
                Email = "momaosiris@gmail.com",
                Password = "123Pal_",
            };

            var resultLogin = await _client.PostAsJsonAsync("api", userModelLogin);
            resultLogin.EnsureSuccessStatusCode();

            var responseLogin = await resultLogin.Content.ReadAsStringAsync();
            var responseJsonLogin = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseLogin);

            Singleton.Instance.Token = responseJsonLogin["token"];

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Singleton.Instance.Token);
            
            var data = new {
                freeStorage = "hola"
            };

            var result = await _client.PatchAsJsonAsync("api/FreeStorage", data);
            var responseContent = await result.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            var responseJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseContent);

            Assert.True(responseJson.ContainsKey("errors"));

            var errors = responseJson["errors"] as Newtonsoft.Json.Linq.JArray;
            var firstError = errors[0].ToObject<Dictionary<string, string>>();

            Assert.Equal("Free storage must be decimal number", firstError["msg"]);
        }

        [Fact]
        public async Task UpdateFreeStorage_Unauthorized() {
            var data = new {
                freeStorage = 222222
            };

            var result = await _client.PatchAsJsonAsync("api/FreeStorage", data);

            var response = await result.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}