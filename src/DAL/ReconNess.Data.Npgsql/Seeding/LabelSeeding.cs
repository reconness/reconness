using Microsoft.EntityFrameworkCore;
using ReconNess.Entities;
using System;

namespace ReconNess.Data.Npgsql.Seeding
{
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
                    Id = Guid.NewGuid(),
                    Name = "Checking",
                    Color = "#0000FF" // Blue
                },
                new Label
                {
                    Id = Guid.NewGuid(),
                    Name = "Vulnerable",
                    Color = "#FF0000" // Red 
                },
                new Label
                {
                    Id = Guid.NewGuid(),
                    Name = "Interesting",
                    Color = "#FF8C00" // DarkOrange  
                },
                new Label
                {
                    Id = Guid.NewGuid(),
                    Name = "Bounty",
                    Color = "#008000" // Green 
                },
                new Label
                {
                    Id = Guid.NewGuid(),
                    Name = "Ignore",
                    Color = "#A9A9A9" // DarkGray 
                }
            });
        }
    }
}
