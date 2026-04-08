using Microsoft.AspNetCore.Identity;

namespace Riverbed.Pricing.Paint.Shared.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public Guid CompanyGuid { get; set; }

        /// <summary>
        /// User preference for dark mode. Persisted in the database and loaded on login.
        /// </summary>
        public bool IsDarkMode { get; set; }
    }

}
