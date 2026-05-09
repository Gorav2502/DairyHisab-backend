namespace MilkCollector.API.DTOs.Auth
{
    public class RegisterRequestDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string District { get; set; }
        public string Password { get; set; }
    }
}
