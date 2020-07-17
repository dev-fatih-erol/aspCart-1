using System.ComponentModel.DataAnnotations;

namespace aspCart.Web.Models.ContactUsViewModels
{
    public class ContactUsModel
    {
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Message { get; set; }
    }
}