using Authorization_Refreshtoken.Models;
using System.ComponentModel.DataAnnotations;

namespace Authorization_Refreshtoken.DTO
{
    public class RegiterDTO
    {
        [Required]
        public string UserName { get; set; }
        public string? SecondName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        public static AppUser ConvertToAppUser(RegiterDTO request)
        {
            return new AppUser
            {
                UserName = request.UserName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                SecondName = request.SecondName,
            };




        }
    }
}
