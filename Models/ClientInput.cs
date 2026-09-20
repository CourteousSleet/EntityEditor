using System.ComponentModel.DataAnnotations;

namespace EntityEditor.Models
{
    // Only editable fields are accepted from forms.
    public sealed class ClientInput
    {
        [Required]
        [Display(Name = "Tax number")]
        [RegularExpression(@"[0-9]{10}([0-9]{2})?", ErrorMessage = "Enter a 10 or 12 digit tax number.")]
        public string IndividualTaxNumber { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Organization type")]
        [RegularExpression("EN|IE", ErrorMessage = "Choose a company or an individual entrepreneur.")]
        public string OrganizationType { get; set; }
    }
}