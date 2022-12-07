using Microsoft.EntityFrameworkCore;
using ReconNess.Infrastructure.Identity.Entities;
using System;

namespace ReconNess.Infrastructure.DataAccess.Seeding;

internal class IdentitySeeding
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="modelBuilder"></param>
    internal static void Run(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>().HasData(new Role
        {
            Id = Guid.Parse("ade752b1-af9e-4ba8-5706-35ad1c1e94ee"),
            Name = "Admin",
            NormalizedName = "ADMIN",
            ConcurrencyStamp = "ade752b1-af9e-4ba8-5706-35ad1c1e94ee"
        },
        new Role
        {
            Id = Guid.Parse("0de752b1-1f3e-4aa8-571a-15ae1c1e94e5"),
            Name = "Member",
            NormalizedName = "MEMBER",
            ConcurrencyStamp = "0de752b1-1f3e-4aa8-571a-15ae1c1e94e5"
        });

    }
}
