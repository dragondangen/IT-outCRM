using System.ComponentModel.DataAnnotations;

namespace IT_outCRM.Application.DTOs.Auth
{
    public class RegisterDto
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = "User";

        // Extended fields for full registration
        public string CompanyName { get; set; } = string.Empty;
        public string Inn { get; set; } = string.Empty;
        public string LegalForm { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        
        /// <summary>
        /// "Customer" or "Executor"
        /// </summary>
        public string UserType { get; set; } = string.Empty;
    }
}
