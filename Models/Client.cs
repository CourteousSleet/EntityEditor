using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EntityEditor.Models
{
    public class Client
    {
        public int ID { get; set; }
        [Display(Name = "Tax number")]
        public string IndividualTaxNumber { get; set; }
        public string Name { get; set; }
        [Display(Name = "Organization type")]
        public string OrganizationType { get; set; } // EN: company; IE: individual entrepreneur.
        [Display(Name = "Updated (UTC)")]
        public DateTime UpdateDate { get; set; }
        [Display(Name = "Created (UTC)")]
        public DateTime CreationDate { get; set; }

        public ICollection<Founder> Founders { get; set; } = new List<Founder>();
    }
}
