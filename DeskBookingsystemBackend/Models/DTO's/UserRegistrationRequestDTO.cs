using System.ComponentModel.DataAnnotations;

namespace drivers.api.Models.DTO_s
{
    public class UserRegistrationRequestDTO
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
