using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riverbed.Pricing.Paint.Shared.Data
{

    public class ChangePasswordDto
    {
        public string NewPassword { get; set; } = string.Empty;
    }
    public class UserDto
    {
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "Username is required")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Company is required")]
        public string CompanyId { get; set; } = string.Empty;
        [Required(ErrorMessage = "Required to Make sure User is Active")]
        public bool IsLockedOut { get; set; } = true;
        public Guid CompanyGuid { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
    }

    public class CompanyDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}