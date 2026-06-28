using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace webshop_owp.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;
    }
}