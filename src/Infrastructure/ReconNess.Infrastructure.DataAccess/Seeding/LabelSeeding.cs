using Microsoft.EntityFrameworkCore;
using ReconNess.Domain.Entities;
using System;

namespace ReconNess.Infrastructure.DataAccess.Seeding;

internal class LabelSeeding
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="modelBuilder"></param>
    internal static void Run(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Label>().HasData(new Label[]
        {
            new Label
            {
                Id = Guid.Parse("cde752b1-3f9e-4ba8-8706-35ad1c1e94ee"),
                Name = "Checking",
                Color = "#0000FF" // Blue
            },
            new Label
            {
                Id = Guid.Parse("1cad5d54-5764-4366-bb2c-cdabc06b29dc"),
                Name = "Vulnerable",
                Color = "#FF0000" // Red 
            },
            new Label
            {
                Id = Guid.Parse("e8942a1a-a535-41cd-941b-67bcc89fe5cd"),
                Name = "Interesting",
                Color = "#FF8C00" // DarkOrange  
            },
            new Label
            {
                Id = Guid.Parse("cd4eb533-4c67-44df-826f-8786c0146721"),
                Name = "Bounty",
                Color = "#008000" // Green 
            },
            new Label
            {
                Id = Guid.Parse("a0d15eac-ec24-4a10-9c3f-007e66f313fd"),
                Name = "Ignore",
                Color = "#A9A9A9" // DarkGray 
            }
        });
    }
}
