using ReconNess.Core.Models;
using System;

namespace ReconNess.Helpers
{
    public static class AgentRunnerHelpers
    {
        /// <summary>
        /// Check if we need to skip the subdomain and does not the agent in that subdomain
        /// </summary>
        /// <param name="agentRunner"></param>
        public static bool NeedToSkipRun(AgentRunner agentRunner)
        {
            // TODO: Add trigger feature
            if (agentRunner.Subdomain == null)
            {
                return false;
            }

            return false;
        }

        /// <summary>
        /// Obtain the channel to send the menssage
        /// </summary>
        /// <param name="agent">The agent</param>
        /// <param name="rootDomain">The domain</param>
        /// <param name="subdomain">The subdomain</param>
        /// <returns>The channel to send the menssage</returns>
        public static string GetChannel(AgentRunner agentRunner)
        {
            return agentRunner.Subdomain == null ?
                $"{agentRunner.Agent.Name}_{agentRunner.Target.Name}_{agentRunner.RootDomain.Name}" :
                $"{agentRunner.Agent.Name}_{agentRunner.Target.Name}_{agentRunner.RootDomain.Name}_{agentRunner.Subdomain.Name}";
        }

        /// <summary>
        /// Obtain the key to store the process
        /// </summary>
        /// <param name="agent">The agent</param>
        /// <param name="rootDomain">The domain</param>
        /// <param name="subdomain">The subdomain</param>
        /// <returns>The channel to send the menssage</returns>
        public static string GetKey(AgentRunner agentRunner)
        {
            return agentRunner.Subdomain == null ?
                $"{agentRunner.Agent.Name}_{agentRunner.Target.Name}_{agentRunner.RootDomain.Name}" :
                $"{agentRunner.Agent.Name}_{agentRunner.Target.Name}_{agentRunner.RootDomain.Name}_{agentRunner.Subdomain.Name}";
        }

        /// <summary>
        /// Obtain the command to run on bash
        /// </summary>
        /// <param name="agentRunner">The agent</param>
        /// <returns>The command to run on bash</returns>
        public static string GetCommand(AgentRunner agentRunner)
        {
            var command = agentRunner.Command;
            if (string.IsNullOrWhiteSpace(command))
            {
                command = agentRunner.Agent.Command;
            }

            var envUserName = Environment.GetEnvironmentVariable("ReconnessUserName") ??
                              Environment.GetEnvironmentVariable("ReconnessUserName", EnvironmentVariableTarget.User);

            var envPassword = Environment.GetEnvironmentVariable("ReconnessPassword") ??
                              Environment.GetEnvironmentVariable("ReconnessPassword", EnvironmentVariableTarget.User);

            return $"{command.Replace("{{domain}}", agentRunner.Subdomain == null ? agentRunner.RootDomain.Name : agentRunner.Subdomain.Name)}"
                .Replace("{{target}}", agentRunner.Target.Name)
                .Replace("{{rootDomain}}", agentRunner.RootDomain.Name)
                .Replace("{{userName}}", envUserName)
                .Replace("{{password}}", envPassword)
                .Replace("\"", "\\\"");
        }
    }
}
