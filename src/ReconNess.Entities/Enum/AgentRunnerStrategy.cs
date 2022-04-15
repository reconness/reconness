namespace ReconNess.Entities.Enum
{
    public enum AgentRunnerStrategy
    {
        // An agent can have multiple commands, for example run nmap in each subdomain, if 
        // round robin is seleted we use the same VM (Docker) to run all the commands, the next agent pick 
        // the next VM if we have one to run their commands
        ROUND_ROBIN,
        // With this option we distribute all the commands for the agent using all the VM aviable 
        GREEDY

        // Note: both strategy works the same if we only have one VM
    }
}
