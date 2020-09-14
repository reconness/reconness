using Microsoft.EntityFrameworkCore;
using ReconNess.Core.Models;
using System;

namespace ReconNess.Data.Npgsql.Seeding
{
    internal class AgentTypeSeeding
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelBuilder"></param>
        internal static void Run(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entities.Type>().HasData(new Entities.Type[]
            {
                new Entities.Type
                {
                    Id = Guid.Parse("cde752b1-3f9e-4ba8-8706-35ad1c1e94ee"),
                    Name = AgentTypes.TARGET
                },
                new Entities.Type
                {
                    Id = Guid.Parse("1cad5d54-5764-4366-bb2c-cdabc06b29dc"),
                    Name = AgentTypes.ROOTDOMAIN
                },
                new Entities.Type
                {
                    Id = Guid.Parse("e8942a1a-a535-41cd-941b-67bcc89fe5cd"),
                    Name = AgentTypes.SUBDOMAIN
                },
                new Entities.Type
                {
                    Id = Guid.Parse("cd4eb533-4c67-44df-826f-8786c0146721"),
                    Name = AgentTypes.DIRECTORY
                },
                new Entities.Type
                {
                    Id = Guid.Parse("a0d15eac-ec24-4a10-9c3f-007e66f313fd"),
                    Name = AgentTypes.RESOURCE
                }
            });
        }
    }
}
