using Microsoft.EntityFrameworkCore;
using ReconNess.Domain.Entities;
using ReconNess.Domain.Enum;
using System;

namespace ReconNess.Infrastructure.Data.EF.Npgsql.Seeding;

internal class AgentsSettingSeeding
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="modelBuilder"></param>
    internal static void Run(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AgentsSetting>().HasData(new AgentsSetting
        {
            Id = Guid.Parse("ade752b1-af9e-4ba8-5706-35ad1c1e94ee"),
            Strategy = AgentRunnerStrategy.ROUND_ROBIN
        });

    }
}
