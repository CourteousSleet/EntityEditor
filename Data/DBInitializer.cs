using System;
using System.Collections.Generic;
using System.Linq;
using EntityEditor.Models;

namespace EntityEditor.Data
{
    public static class DBInitializer
    {
        public static void Initialize(EntityEditorContext context)
        {
            context.Database.EnsureCreated();
            // Preserve existing data; seed only a completely empty database.
            if (context.Clients.Any() || context.Founders.Any())
                return;

            var date = new DateTime(2019, 9, 1, 0, 0, 0, DateTimeKind.Utc);
            context.Clients.AddRange(
                new Client
                {
                    IndividualTaxNumber = "1234567890",
                    Name = "Example company",
                    OrganizationType = "EN",
                    CreationDate = date,
                    UpdateDate = date,
                    Founders = new List<Founder>
                    {
                        new Founder
                        {
                            IndividualTaxNumber = "123456789012",
                            Initials = "Example founder",
                            CreationDate = date,
                            UpdateDate = date
                        }
                    }
                },
                new Client
                {
                    IndividualTaxNumber = "123456789012",
                    Name = "Example entrepreneur",
                    OrganizationType = "IE",
                    CreationDate = date,
                    UpdateDate = date
                });
            // Insert the graph atomically using SQL Server-generated keys.
            context.SaveChanges();
        }
    }
}