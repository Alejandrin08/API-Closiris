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
    public class UserTest : IDisposable
    {
        private readonly HttpClient _client;
        private readonly ITestOutputHelper _output;

        public UserTest(ITestOutputHelper output)
        {
            _output = output;
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Add("accept", "application/json");
            _client.BaseAddress = new Uri("http://localhost:5089");
        }

       
       

        public async Task CreateUserAccount_Successful()
        {
            UserModel userModel = new UserModel
            {
                Email = "alesis@gmail.com",
                Password = "123Ale_",
                Name = "Alejandro Sanchez",
                ImageProfile = null,
            };

            var result = await _client.PostAsJsonAsync("api/userAccount", userModel);
            result.EnsureSuccessStatusCode();

            var response = await result.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.Created, result.StatusCode);
        }

      
        public async Task CreateUserAccount_Failed()
        {
            UserModel userModel = new UserModel
            {
                Email = null,
                Password = "123Ale_",
                Name = "Alejandro Sanchez",
                ImageProfile = null,
            };

            var result = await _client.PostAsJsonAsync("api/userAccount", userModel);

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);



        }

    
        public async Task CreateUser_Successful()
        {
            UserModel userModel1 = new UserModel
            {
                Email = "alvaro@gmail.com",
                Password = "123Ale_",
                Name = "Alejandro Sanchez",
                ImageProfile = null,
            };

            var result1 = await _client.PostAsJsonAsync("api/userAccount", userModel1);

            UserModel userModel = new UserModel
            {
                Email = "alvaro@gmail.com",
                Plan = "Premium",
                FreeStorage = 104857600
            };

            var result = await _client.PostAsJsonAsync("api/user", userModel);
            result.EnsureSuccessStatusCode();

            var response = await result.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.Created, result.StatusCode);
        }

     
        public async Task CreateUser_Failed()
        {
            UserModel userModel = new UserModel
            {
                Email = null,
                Plan = "Premium",
                FreeStorage = 104857600
            };

            var result = await _client.PostAsJsonAsync("api/user", userModel);

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);

        }

       
        public async Task UpdateUserAccount_Successful()
        {
            UserModel userModel1 = new UserModel
            {
                Email = "alejandro@gmail.com",
                Password = "123Ale_",
            };

            var result1 = await _client.PostAsJsonAsync("api", userModel1);
            result1.EnsureSuccessStatusCode();


             var response1 = await result1.Content.ReadAsStringAsync();
            var responseJson1 = JsonConvert.DeserializeObject<Dictionary<string, string>>(response1);

            Singleton.Instance.Token = responseJson1["token"];
           
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Singleton.Instance.Token);

            UserModel userModel = new UserModel
            {
                Email = "palomita@gmail.com",
                Name = "Alejandro Sanchez Marin",
                ImageProfile = "",
            };

            
            var result = await _client.PutAsJsonAsync("api/UserAccount", userModel);

            result.EnsureSuccessStatusCode(); // This will throw an exception if the status code is not a success code

            var response = await result.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }



      
        public async Task UpdateUserAccount_Failed()
        {
            UserModel userModel = new UserModel
            {
                Email = "alex@outlook.com",
                Name = "Alejandro Sanchez Marin",
                ImageProfile = null,
            };

            var result = await _client.PutAsJsonAsync("api/UserAccount", userModel);

            Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
        }

    
        public async Task UpdatePassword_Successful()
        {
            UserModel userModel = new UserModel
            {
                Email = "momaosiris@gmail.com",
                Password = "123Pal_",

            };

            var result = await _client.PostAsJsonAsync("api/Password", userModel);
            result.EnsureSuccessStatusCode();

            var response = await result.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        
        public async Task UpdatePassword_Failed()
        {
            UserModel userModel = new UserModel
            {
                Email = "momaosirispato@gmail.com",
                Password = "123Pal_",

            };

            var result = await _client.PostAsJsonAsync("api/Password", userModel);
            

            var response = await result.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

      
        public async Task GetUserInfoByEmail_Successful()
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
            // Crear un usuario de prueba

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Singleton.Instance.Token);
            UserModel userModel = new UserModel
            {
                Name = "Paoma",
                Id = 1,
            };
            string email = "momaosiris@gmail.com";    

            // Obtener la información del usuario por correo electrónico
            var getResult = await _client.GetAsync($"api/Info/{email}");
            getResult.EnsureSuccessStatusCode();

            var response = await getResult.Content.ReadAsStringAsync();
            var userInfo = JsonConvert.DeserializeObject<UserModel>(response);

            // Verificar la respuesta
            Assert.Equal(HttpStatusCode.Created, getResult.StatusCode);
            Assert.Equal(userModel.Email, userInfo.Email);
            Assert.Equal(userModel.Name, userInfo.Name);
        }

       
        public async Task GetUserInfoByEmail_Failed()
        {
            string email = "ositio@gmail.com";

            var getResult = await _client.GetAsync($"api/Info/{email}");


            Assert.Equal(HttpStatusCode.Unauthorized, getResult.StatusCode);
        }

      
        public async Task ValidateEmailDuplicity_Successful()
        {
            string email = "dbaez105@gmail.com";    

            // Obtener la información del usuario por correo electrónico
            var getResult = await _client.GetAsync($"api/EmailDuplicity/{email}");
            getResult.EnsureSuccessStatusCode();

            var response = await getResult.Content.ReadAsStringAsync();
            var responseJson = JsonConvert.DeserializeObject<Dictionary<string, string>>(response);

            // Verificar la respuesta
            Assert.Equal(HttpStatusCode.OK, getResult.StatusCode);
            Assert.Equal(responseJson["message"], "Email available");
        }

       
        public async Task ValidateEmailDuplicity_Failed()
        {
            string email = "momaosiris@gmail.com";    

            // Obtener la información del usuario por correo electrónico
            var getResult = await _client.GetAsync($"api/EmailDuplicity/{email}");
            getResult.EnsureSuccessStatusCode();

            var response = await getResult.Content.ReadAsStringAsync();
            var responseJson = JsonConvert.DeserializeObject<Dictionary<string, string>>(response);

            // Verificar la respuesta
            Assert.Equal(HttpStatusCode.OK, getResult.StatusCode);
            Assert.Equal(responseJson["user"], "momaosiris@gmail.com");
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}