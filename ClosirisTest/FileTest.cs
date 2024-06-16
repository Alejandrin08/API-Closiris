using ClosirisTest.DTOs;
using System.Net.Http;
using System.Net.Http.Json;
using Newtonsoft.Json;
using Xunit;
using System.Net;
using System.Net.Http.Headers;
using Xunit.Abstractions;

namespace ClosirisTest {
    public class FileTest : IDisposable {
        private readonly HttpClient _client;
        private readonly ITestOutputHelper _output;

        public FileTest(ITestOutputHelper output) {
            _output = output;
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Add("accept", "application/json");
            _client.BaseAddress = new Uri("http://localhost:5089");
        }

        [Fact]
        public async Task InsertFile_Successful() {

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

            var fileContent = new ByteArrayContent(System.IO.File.ReadAllBytes("C:\\Users\\Palom\\Desktop\\Proyecto\\API-Closiris\\API-Closiris\\ClosirisTest\\Files\\pulpo.txt"));

            var formData = new MultipartFormDataContent{
                { fileContent, "file", "pulpo.txt" }
            };

            _client.DefaultRequestHeaders.Add("folder_name", "TestFolder");

            var response = await _client.PostAsync("api/File", formData);

            var responseContent = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task InsertFile_Failed(){

            UserModel userModelLogin = new UserModel{
                Email = "momaosiris@gmail.com",
                Password = "123Pal_",
            };

            var resultLogin = await _client.PostAsJsonAsync("api", userModelLogin);
            resultLogin.EnsureSuccessStatusCode();

            var responseLogin = await resultLogin.Content.ReadAsStringAsync();
            var responseJsonLogin = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseLogin);

            Singleton.Instance.Token = responseJsonLogin["token"];

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Singleton.Instance.Token);

            var fileContent = new ByteArrayContent(System.IO.File.ReadAllBytes("C:\\Users\\Palom\\Desktop\\Proyecto\\API-Closiris\\API-Closiris\\ClosirisTest\\Files\\pulpo.txt"));

            var formData = new MultipartFormDataContent
            {
                { fileContent, "file", "pulpo.txt" }
            };

            _client.DefaultRequestHeaders.Add("folder_name", "Compartidos");

            var response = await _client.PostAsync("api/File", formData);
            var responseContent = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var responseJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseContent);

            Assert.True(responseJson.ContainsKey("errors"));

            var errors = responseJson["errors"] as Newtonsoft.Json.Linq.JArray;
            var firstError = errors[0].ToObject<Dictionary<string, string>>();

            Assert.Equal("Please provide a valid folder name", firstError["msg"]);
        }

        [Fact]
        public async Task InsertFile_Unauthorized() {
            var fileContent = new ByteArrayContent(System.IO.File.ReadAllBytes("C:\\Users\\Palom\\Desktop\\Proyecto\\API-Closiris\\API-Closiris\\ClosirisTest\\Files\\pulpo.txt"));

            var formData = new MultipartFormDataContent{
                { fileContent, "file", "pulpo.txt" }
            };

            _client.DefaultRequestHeaders.Add("folder_name", "TestFolder");

            var response = await _client.PostAsync("api/File", formData);

            var responseContent = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task InsertFileOwner_Successful(){
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
            _client.DefaultRequestHeaders.Add("file_id", "1");

            var response = await _client.PostAsync("api/FileOwner", null);

            var responseContent = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task InsertFileOwner_Failed(){
            UserModel userModelLogin = new UserModel{
                Email = "momaosiris@gmail.com",
                Password = "123Pal_",
            };

            var resultLogin = await _client.PostAsJsonAsync("api", userModelLogin);
            resultLogin.EnsureSuccessStatusCode();

            var responseLogin = await resultLogin.Content.ReadAsStringAsync();
            var responseJsonLogin = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseLogin);

            Singleton.Instance.Token = responseJsonLogin["token"];

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Singleton.Instance.Token);
            _client.DefaultRequestHeaders.Add("file_id", "X");

            var response = await _client.PostAsync("api/FileOwner", null);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var responseJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseContent);

            Assert.True(responseJson.ContainsKey("errors"));

            var errors = responseJson["errors"] as Newtonsoft.Json.Linq.JArray;
            var firstError = errors[0].ToObject<Dictionary<string, string>>();

            Assert.Equal("File id is requiered", firstError["msg"]);
        }

        [Fact]
        public async Task InsertFileOwner_Unauthorized(){
            _client.DefaultRequestHeaders.Add("file_id", "2");

            var response = await _client.PostAsync("api/FileOwner", null);

            var responseContent = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task InsertFileShared_Successful() {

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
            _client.DefaultRequestHeaders.Add("shared_id", "2");
            _client.DefaultRequestHeaders.Add("file_id", "1");

            var response = await _client.PostAsync("api/FileShared", null);

            var responseContent = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]        
         public async Task InsertFileShared_Failed() {

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
            _client.DefaultRequestHeaders.Add("shared_id", "2");
            _client.DefaultRequestHeaders.Add("file_id", "Y");

            var response = await _client.PostAsync("api/FileShared", null);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var responseJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseContent);

            Assert.True(responseJson.ContainsKey("errors"));

            var errors = responseJson["errors"] as Newtonsoft.Json.Linq.JArray;
            var firstError = errors[0].ToObject<Dictionary<string, string>>();

            Assert.Equal("File id is requiered", firstError["msg"]);
        }

        [Fact]
        public async Task InsertFileShared_Unauthorized() {
            _client.DefaultRequestHeaders.Add("shared_id", "1");
            _client.DefaultRequestHeaders.Add("file_id", "1");

            var response = await _client.PostAsync("api/FileShared", null);

            var responseContent = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task DeleteFileShared_Successful(){

            UserModel userModelLogin = new UserModel{
                Email = "momaosiris@gmail.com",
                Password = "123Pal_",
            };

            var resultLogin = await _client.PostAsJsonAsync("api", userModelLogin);
            resultLogin.EnsureSuccessStatusCode();

            var responseLogin = await resultLogin.Content.ReadAsStringAsync();
            var responseJsonLogin = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseLogin);

            Singleton.Instance.Token = responseJsonLogin["token"];

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Singleton.Instance.Token);
            _client.DefaultRequestHeaders.Add("file_id", "1");

            var response = await _client.DeleteAsync("api/FileShared");

            var responseContent = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task DeleteFileShared_Failed() {

            UserModel userModelLogin = new UserModel{
                Email = "momaosiris@gmail.com",
                Password = "123Pal_",
            };

            var resultLogin = await _client.PostAsJsonAsync("api", userModelLogin);
            resultLogin.EnsureSuccessStatusCode();

            var responseLogin = await resultLogin.Content.ReadAsStringAsync();
            var responseJsonLogin = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseLogin);

            Singleton.Instance.Token = responseJsonLogin["token"];

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Singleton.Instance.Token);
            _client.DefaultRequestHeaders.Add("file_id", "X");

            var response = await _client.DeleteAsync("api/FileShared");

            var responseContent = await response.Content.ReadAsStringAsync();
            
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var responseJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseContent);

            Assert.True(responseJson.ContainsKey("errors"));

            var errors = responseJson["errors"] as Newtonsoft.Json.Linq.JArray;
            var firstError = errors[0].ToObject<Dictionary<string, string>>();

            Assert.Equal("File id is requiered", firstError["msg"]);
        }

        [Fact]
        public async Task DeleteFileShared_Unauthorized(){

            _client.DefaultRequestHeaders.Add("file_id", "1");

            var response = await _client.DeleteAsync("api/FileShared");

            var responseContent = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetFoldersByUser_Successful(){

            UserModel userModelLogin = new UserModel {
                Email = "cami@gmail.com",
                Password = "123Cami_",
            };

            var resultLogin = await _client.PostAsJsonAsync("api", userModelLogin);
            resultLogin.EnsureSuccessStatusCode();

            var responseLogin = await resultLogin.Content.ReadAsStringAsync();
            var responseJsonLogin = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseLogin);

            Singleton.Instance.Token = responseJsonLogin["token"];

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Singleton.Instance.Token);
            
            var response = await _client.GetAsync("api/Folders");
            var responseContent = await response.Content.ReadAsStringAsync();
            var folders = JsonConvert.DeserializeObject<List<string>>(responseContent);

            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(folders);
            Assert.Single(folders);
            Assert.Equal("Proyecto", folders[0]);
        }

        [Fact]
        public async Task GetFoldersByUser_Unauthorized() {

            var response = await _client.GetAsync("api/Folders");

            var responseContent = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

        }

        [Fact]
        public async Task GetUsersOwnerFile_Successful() {

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
            _client.DefaultRequestHeaders.Add("file_id", "2");

            var response = await _client.GetAsync("api/UsersOwner");

            var responseContent = await response.Content.ReadAsStringAsync();
            var usersOwner = JsonConvert.DeserializeObject<List<UserModel>>(responseContent);

            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            Assert.NotNull(usersOwner);
            Assert.Single(usersOwner);
            Assert.Equal("Camila", usersOwner[0].Name);
        }

        [Fact]
        public async Task GetUsersOwnerFile_Failed() {

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
            _client.DefaultRequestHeaders.Add("file_id", "");

            var response = await _client.GetAsync("api/UsersOwner");

            var responseContent = await response.Content.ReadAsStringAsync();
            
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var responseJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseContent);

            Assert.True(responseJson.ContainsKey("errors"));

            var errors = responseJson["errors"] as Newtonsoft.Json.Linq.JArray;
            var firstError = errors[0].ToObject<Dictionary<string, string>>();

            Assert.Equal("File id is requiered", firstError["msg"]);
        }

        [Fact]
        public async Task GetUsersOwnerFile_Unauthorized() {

            _client.DefaultRequestHeaders.Add("file_id", "2");

            var response = await _client.GetAsync("api/UsersOwner");
 
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            
        }

        [Fact]
        public async Task GetUsersShareFile_Successful() {

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
            _client.DefaultRequestHeaders.Add("file_id", "2");

            var response = await _client.GetAsync("api/UsersShare");

            var responseContent = await response.Content.ReadAsStringAsync();
            var usersOwner = JsonConvert.DeserializeObject<List<UserModel>>(responseContent);

            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            Assert.NotNull(usersOwner);
            Assert.Single(usersOwner);
            Assert.Equal("Paloma", usersOwner[0].Name);
        }

        [Fact]
        public async Task GetUsersShareFile_Failed() {

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
            _client.DefaultRequestHeaders.Add("file_id", "");

            var response = await _client.GetAsync("api/UsersShare");

            var responseContent = await response.Content.ReadAsStringAsync();
            
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var responseJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseContent);

            Assert.True(responseJson.ContainsKey("errors"));

            var errors = responseJson["errors"] as Newtonsoft.Json.Linq.JArray;
            var firstError = errors[0].ToObject<Dictionary<string, string>>();

            Assert.Equal("File id is requiered", firstError["msg"]);
        }

        [Fact]
        public async Task GetUsersShareFile_Unauthorized(){

            _client.DefaultRequestHeaders.Add("file_id", "2");

            var response = await _client.GetAsync("api/UsersShare");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            
        }

        [Fact]
        public async Task DeleteFileFromServer_Successful() {
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

            _client.DefaultRequestHeaders.Add("file_id", "1");

            var response = await _client.DeleteAsync("api/ServerFile");

            var responseContent = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task DeleteFileFromServer_Failed() {
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
            _client.DefaultRequestHeaders.Add("file_id", "X");

            var response = await _client.DeleteAsync("api/ServerFile");
            var responseContent = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var responseJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseContent);

            Assert.True(responseJson.ContainsKey("errors"));

            var errorArray = responseJson["errors"] as JArray;
            var firstError = errorArray[0].ToObject<Dictionary<string, string>>();

            Assert.Equal("File id is requiered", firstError["msg"]);
        }

        [Fact]
        public async Task DeleteFileFromServer_Unauthorized() {
            _client.DefaultRequestHeaders.Add("file_id", "1");

            var response = await _client.DeleteAsync("api/ServerFile");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task DeleteFileRegistration_Successful() {
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

            _client.DefaultRequestHeaders.Add("file_id", "1");

            var response = await _client.DeleteAsync("api/File");

            var responseContent = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task DeleteFileRegistration_Failed() {
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
            _client.DefaultRequestHeaders.Add("file_id", "X");

            var response = await _client.DeleteAsync("api/File");
            var responseContent = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var responseJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseContent);

            Assert.True(responseJson.ContainsKey("errors"));

            var errorArray = responseJson["errors"] as JArray;
            var firstError = errorArray[0].ToObject<Dictionary<string, string>>();

            Assert.Equal("File id is requiered", firstError["msg"]);
        }

        [Fact]
        public async Task DeleteFileRegistration_Unauthorized() {
            _client.DefaultRequestHeaders.Add("file_id", "1");

            var response = await _client.DeleteAsync("api/File");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        public void Dispose() {
            _client.Dispose();
        }


    }

}