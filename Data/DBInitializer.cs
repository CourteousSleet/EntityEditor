using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EntityEditor.Data;
using EntityEditor.Models;

namespace EntityEditor.Data
{
    public class DBInitializer
    {
        public static void Initialize(EntityEditorContext context)
        {
            context.Database.EnsureCreated();

            // Look for any Clients.
            if (context.Clients.Any())
            {
                return;   // DB has been seeded
            }


            var founders = new Founder[]
            {
                new Founder{ID=1,IndividualTaxNumber="123456789012",Initials="!", UpdateDate= DateTime.Parse("2019-09-01"), CreationDate = DateTime.Parse("2019-11-01")},
                new Founder{ID=2,IndividualTaxNumber="123456789012",Initials="!2", UpdateDate= DateTime.Parse("2319-09-01"), CreationDate = DateTime.Parse("2019-09-21")},
                new Founder{ID=3,IndividualTaxNumber="123456789012",Initials="!3", UpdateDate= DateTime.Parse("1019-19-01"), CreationDate = DateTime.Parse("2029-01-01")},
                new Founder{ID=4,IndividualTaxNumber="123456789012",Initials="!4", UpdateDate= DateTime.Parse("2119-29-01"), CreationDate = DateTime.Parse("2019-09-01")},
                new Founder{ID=5,IndividualTaxNumber="123456789012",Initials="!5", UpdateDate= DateTime.Parse("2029-22-01"), CreationDate = DateTime.Parse("2019-09-01")}
            };
            foreach (Founder f in founders)
            {
                context.Founders.Add(f);
            }
            context.SaveChanges();

            var clients = new Client[]
            {
                new Client{ID= 1, IndividualTaxNumber="123456789012",Name="!",OrganizationType= "EN", UpdateDate= DateTime.Parse("2019-09-01"), CreationDate = DateTime.Parse("2019-09-01")},
                new Client{ID= 2, IndividualTaxNumber="123456789012",Name="!1",OrganizationType= "IE", UpdateDate= DateTime.Parse("2019-09-01"), CreationDate=DateTime.Parse("2017-09-01")},
                new Client{ID= 3, IndividualTaxNumber="123456789012",Name="!2",OrganizationType= "IE", UpdateDate= DateTime.Parse("2019-09-01"), CreationDate=DateTime.Parse("2018-09-01")},
                new Client{ID= 4, IndividualTaxNumber="123456789012",Name="!3",OrganizationType= "IE", UpdateDate= DateTime.Parse("2019-09-01"), CreationDate=DateTime.Parse("2017-09-01")},
                new Client{ID= 5, IndividualTaxNumber="123456789012",Name="!4",OrganizationType= "IE", UpdateDate= DateTime.Parse("2019-09-01"), CreationDate=DateTime.Parse("2017-09-01")},
                new Client{ID= 6, IndividualTaxNumber="123456789012",Name="!5",OrganizationType= "IE", UpdateDate= DateTime.Parse("2019-09-01"), CreationDate=DateTime.Parse("2016-09-01")},
                new Client{ID= 7, IndividualTaxNumber="123456789012",Name="!6",OrganizationType= "IE", UpdateDate= DateTime.Parse("2019-09-01"), CreationDate=DateTime.Parse("2018-09-01")},
                new Client{ID= 8, IndividualTaxNumber= "123456789012",Name="!7",OrganizationType= "IE", UpdateDate= DateTime.Parse("2019-09-01"), CreationDate=DateTime.Parse("2019-09-01") }
            };
            foreach (Client c in clients)
            {
                context.Clients.Add(c);
            }
            context.SaveChanges();
        }
    }
}
