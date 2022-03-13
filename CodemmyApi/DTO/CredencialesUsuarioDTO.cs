using System.ComponentModel.DataAnnotations;

namespace CodemmyApi.DTO
{
    public class CredencialesUsuarioDTO
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
        public string Claim { get; set; }
    }
}
