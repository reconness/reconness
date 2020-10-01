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
        public static bool NeedToSkipRun(AgentRunner agentRunner, string agentRunnerType)
        {
            if (agentRunner.Agent.AgentTrigger.SkipIfRunBefore)
            {
                var agentTypeTarget = AgentRunnerTypes.CURRENT.Equals(agentRunnerType) && AgentTypes.TARGET.Equals(agentRunner.Agent.AgentType.Name);
                var agentRanBeforeInThisTarget = (agentTypeTarget || AgentRunnerTypes.ALLDIRECTORY.Equals(agentRunnerType)) &&
                                                 agentRunner.Target != null &&
                                                 !string.IsNullOrEmpty(agentRunner.Target.AgentsRawBefore) &&
                                                 agentRunner.Target.AgentsRawBefore.Contains(agentRunner.Agent.Name);

                var agentTypeRootDomain = AgentRunnerTypes.CURRENT.Equals(agentRunnerType) && AgentTypes.ROOTDOMAIN.Equals(agentRunner.Agent.AgentType.Name);
                var agentRanBeforeInThisRootDomain = (agentTypeRootDomain || AgentRunnerTypes.ALLROOTDOMAIN.Equals(agentRunnerType)) &&
                                                     agentRunner.RootDomain != null &&
                                                     !string.IsNullOrEmpty(agentRunner.RootDomain.AgentsRawBefore) &&
                                                     agentRunner.RootDomain.AgentsRawBefore.Contains(agentRunner.Agent.Name);

                var agentTypeSubdomain = AgentRunnerTypes.CURRENT.Equals(agentRunnerType) && AgentTypes.SUBDOMAIN.Equals(agentRunner.Agent.AgentType.Name);
                var agentRanBeforeInThisSubdomain = (agentTypeSubdomain || AgentRunnerTypes.ALLSUBDOMAIN.Equals(agentRunnerType)) &&
                                                     agentRunner.Subdomain != null &&
                                                     !string.IsNullOrEmpty(agentRunner.Subdomain.AgentsRawBefore) &&
                                                     agentRunner.Subdomain.AgentsRawBefore.Contains(agentRunner.Agent.Name);

                if (agentRanBeforeInThisTarget || agentRanBeforeInThisRootDomain || agentRanBeforeInThisSubdomain)
                {
                    return true;
                }
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
            if (agentRunner.Target == null)
            {
                return $"{agentRunner.Agent.Name}";
            }

            if (agentRunner.RootDomain == null)
            {
                return $"{agentRunner.Agent.Name}_{agentRunner.Target.Name}";
            }

            if (agentRunner.Subdomain == null)
            {
                return $"{agentRunner.Agent.Name}_{agentRunner.Target.Name}_{agentRunner.RootDomain.Name}";
            }

            return $"{agentRunner.Agent.Name}_{agentRunner.Target.Name}_{agentRunner.RootDomain.Name}_{agentRunner.Subdomain.Name}";
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
            if (agentRunner.Target == null)
            {
                return $"{agentRunner.Agent.Name}";
            }

            if (agentRunner.RootDomain == null)
            {
                return $"{agentRunner.Agent.Name}_{agentRunner.Target.Name}";
            }

            if (agentRunner.Subdomain == null)
            {
                return $"{agentRunner.Agent.Name}_{agentRunner.Target.Name}_{agentRunner.RootDomain.Name}";
            }

            return $"{agentRunner.Agent.Name}_{agentRunner.Target.Name}_{agentRunner.RootDomain.Name}_{agentRunner.Subdomain.Name}";
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
