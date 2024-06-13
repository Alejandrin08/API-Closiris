namespace ClosirisTest.DTOs
{
    public class UserModel
    {
        public int Id { get; set; } = 0;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ImageProfile { get; set; } 
        public string Token { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Plan { get; set; } = string.Empty;
        public decimal FreeStorage { get; set; } = 0;

    }
}