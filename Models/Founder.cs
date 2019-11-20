using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EntityEditor.Models
{
    public class Founder
    {
        public int ID { get; set; }
        public string IndividualTaxNumber { get; set; }
        public string Initials { get; set; } //OR Name -- I am not sure still
        public DateTime UpdateDate { get; set; }
        public DateTime CreationDate { get; set; }

        public Client Client;
    }
}
