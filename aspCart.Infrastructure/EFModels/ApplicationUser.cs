using System;
using Microsoft.AspNetCore.Identity;

namespace aspCart.Infrastructure.EFModels
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public Guid BillingAddressId { get; set; }
    }
}