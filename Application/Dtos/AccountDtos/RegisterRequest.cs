using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.AccountDtos
{
    public class RegisterRequest
    {

        [Required]
        public string DisplayName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [RegularExpression("(?=^.{6,10}$)(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&amp;*()_+}{&quot;:;'?/&gt;.&lt;,])(?!.*\\s).*$",
         ErrorMessage = "Password must have 1 Uppercase, 1 Lowercase,1 Number, 1 Non Alphanumeric and at leat 6 characters")]
        public string Password { get; set; }
    }
}