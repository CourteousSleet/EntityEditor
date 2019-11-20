using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EntityEditor.Models
{
    public class Client
    {
        public int ID { get; set; }
        public string IndividualTaxNumber { get; set; }
        public string Name { get; set; }
        public string OrganizationType { get; set; } //Can be "Individual Entrepreneur" or "Entity". Outer navigation goes by checking OrgType value
        public DateTime UpdateDate { get; set; }
        public DateTime CreationDate { get; set; }

        public ICollection<Founder> Founders { get; set; }
    }
}
