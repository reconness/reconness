namespace ReconNess.Domain.Enum;

public enum AgentRunnerStrategy
{
    // An agent can have multiple commands, for example run nmap in each subdomain, if 
    // round robin is seleted we use the same server (Docker) to run all the commands, the next agent pick 
    // the next server if we have one to run their commands
    ROUND_ROBIN,
    // With this option we distribute all the commands for the agent using all the server aviable 
    GREEDY

    // Note: both strategy works the same if we only have one server
}
