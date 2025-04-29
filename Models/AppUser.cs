using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Authorization_Refreshtoken.Models
{
    public class AppUser : IdentityUser
    {
        [MaxLength(30)]
        public string? SecondName { get; set; }
        public DateTime BirthDate { get; set; }

        public int Age 
        {
            get
            {
                var today = DateTime.Today;
                var age=BirthDate.Year-today.Year;
                if (BirthDate.Date > today.AddDays(-age)) --age;
                return age;
            }
                
        }


    }
}