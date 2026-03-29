using System.ComponentModel.DataAnnotations;

namespace NexTrack.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Please enter your email address.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [Display(Name = "Email address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter your password.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;
    }
}
