using ReconNess.Core.Models;
using ReconNess.Entities;
using System;
using System.Text.RegularExpressions;

namespace ReconNess.Helpers
{
    public static class AgentRunnerHelpers
    {
        private const string INCLUDE = "include";
        private const string EXCLUDE = "exclude";

        /// <summary>
        /// Check if we need to skip the subdomain and does not the agent in that subdomain
        /// </summary>
        /// <param name="agentRunner">The agent runner</param>
        /// <param name="agentRunnerType">The agent runner type</param>
        public static bool NeedToSkipRun(AgentRunner agentRunner, string agentRunnerType)
        {
            var agentTrigger = agentRunner.Agent.AgentTrigger;
            if (agentTrigger == null)
            {
                return false;
            }

            var agentTypeTarget = AgentRunnerTypes.CURRENT_TARGET.Equals(agentRunnerType) || AgentRunnerTypes.ALL_DIRECTORIES.Equals(agentRunnerType);
            var agentTypeRootDomain = AgentRunnerTypes.CURRENT_ROOTDOMAIN.Equals(agentRunnerType) || AgentRunnerTypes.ALL_ROOTDOMAINS.Equals(agentRunnerType);
            var agentTypeSubdomain = AgentRunnerTypes.CURRENT_SUBDOMAIN.Equals(agentRunnerType) || AgentRunnerTypes.ALL_SUBDOMAINS.Equals(agentRunnerType);


            return (agentTrigger.SkipIfRunBefore && RanBefore(agentRunner, agentTypeTarget, agentTypeRootDomain, agentTypeSubdomain)) ||
                   (agentTypeTarget && SkipTarget(agentRunner.Target, agentTrigger)) ||
                   (agentTypeRootDomain && SkipRootDomain(agentRunner.RootDomain, agentTrigger)) ||
                   (agentTypeSubdomain && SkipSubdomain(agentRunner.Subdomain, agentTrigger));
        }

        /// <summary>
        /// If ran before (target, rootdomain, subdomain)
        /// </summary>
        /// <param name="agentRunner">The agent runner</param>
        /// <param name="agentTypeTarget">if is the Target the agent type</param>
        /// <param name="agentTypeRootDomain">if is the RootDomain the agent type</param>
        /// <param name="agentTypeSubdomain">if is the Subdomain the agent type</param>
        /// <returns>If ran before (target, rootdomain, subdomain)></returns>
        private static bool RanBefore(AgentRunner agentRunner, bool agentTypeTarget, bool agentTypeRootDomain, bool agentTypeSubdomain)
        {
            var agentRanBeforeInThisTarget = agentTypeTarget && agentRunner.Target != null &&
                                                 !string.IsNullOrEmpty(agentRunner.Target.AgentsRanBefore) &&
                                                 agentRunner.Target.AgentsRanBefore.Contains(agentRunner.Agent.Name);

            var agentRanBeforeInThisRootDomain = agentTypeRootDomain && agentRunner.RootDomain != null &&
                                                 !string.IsNullOrEmpty(agentRunner.RootDomain.AgentsRanBefore) &&
                                                 agentRunner.RootDomain.AgentsRanBefore.Contains(agentRunner.Agent.Name);

            var agentRanBeforeInThisSubdomain = agentTypeSubdomain && agentRunner.Subdomain != null &&
                                                 !string.IsNullOrEmpty(agentRunner.Subdomain.AgentsRanBefore) &&
                                                 agentRunner.Subdomain.AgentsRanBefore.Contains(agentRunner.Agent.Name);

            return agentRanBeforeInThisTarget || agentRanBeforeInThisRootDomain || agentRanBeforeInThisSubdomain;
        }

        /// <summary>
        /// If we need to skip this Target
        /// </summary>
        /// <param name="target">The Target</param>
        /// <param name="agentTrigger">Agent trigger configuration</param>
        /// <returns>If we need to skip this Target</returns>
        private static bool SkipTarget(Target target, AgentTrigger agentTrigger)
        {
            if (agentTrigger.TargetHasBounty && (target == null || !target.HasBounty))
            {
                return true;
            }

            if (!string.IsNullOrEmpty(agentTrigger.TargetIncExcName) && !string.IsNullOrEmpty(agentTrigger.TargetName))
            {
                if (INCLUDE.Equals(agentTrigger.TargetIncExcName, StringComparison.OrdinalIgnoreCase))
                {
                    var match = Regex.Match(target.Name, agentTrigger.TargetName);

                    // if match success dont skip this target  
                    return !match.Success;
                }
                else if (EXCLUDE.Equals(agentTrigger.TargetIncExcName, StringComparison.OrdinalIgnoreCase))
                {
                    var match = Regex.Match(target.Name, agentTrigger.TargetName);

                    // if match success skip this target 
                    return match.Success;
                }
            }

            return false;
        }

        /// <summary>
        /// If we need to skip this RootDomain
        /// </summary>
        /// <param name="rootDomain">The rootDomain</param>
        /// <param name="agentTrigger">Agent trigger configuration</param>
        /// <returns>If we need to skip this RootDomain</returns>
        private static bool SkipRootDomain(RootDomain rootDomain, AgentTrigger agentTrigger)
        {
            if (agentTrigger.RootdomainHasBounty && (rootDomain == null || !rootDomain.HasBounty))
            {
                return true;
            }

            if (!string.IsNullOrEmpty(agentTrigger.RootdomainIncExcName) && !string.IsNullOrEmpty(agentTrigger.RootdomainName))
            {
                if (INCLUDE.Equals(agentTrigger.RootdomainIncExcName, StringComparison.OrdinalIgnoreCase))
                {
                    var match = Regex.Match(rootDomain.Name, agentTrigger.RootdomainName);

                    // if match success dont skip this rootdomain  
                    return !match.Success;
                }
                else if (EXCLUDE.Equals(agentTrigger.RootdomainIncExcName, StringComparison.OrdinalIgnoreCase))
                {
                    var match = Regex.Match(rootDomain.Name, agentTrigger.RootdomainName);

                    // if match success skip this rootdomain 
                    return match.Success;
                }
            }

            return false;
        }

        /// <summary>
        /// If we need to skip this Subdomain
        /// </summary>
        /// <param name="subdomain">The subdomain</param>
        /// <param name="agentTrigger">Agent trigger configuration</param>
        /// <returns>If we need to skip this Subdomain</returns>
        private static bool SkipSubdomain(Subdomain subdomain, AgentTrigger agentTrigger)
        {
            if (agentTrigger.SubdomainHasBounty && (subdomain == null || subdomain.HasBounty == null || !subdomain.HasBounty.Value))
            {
                return true;
            }

            if (agentTrigger.SubdomainHasHttpOrHttpsOpen && (subdomain == null || subdomain.HasHttpOpen == null || !subdomain.HasHttpOpen.Value))
            {
                return true;
            }

            if (agentTrigger.SubdomainIsAlive && (subdomain == null || subdomain.IsAlive == null || !subdomain.IsAlive.Value))
            {
                return true;
            }

            if (agentTrigger.SubdomainIsMainPortal && (subdomain == null || subdomain.IsMainPortal == null || !subdomain.IsMainPortal.Value))
            {
                return true;
            }

            if (!string.IsNullOrEmpty(agentTrigger.SubdomainIncExcName) && !string.IsNullOrEmpty(agentTrigger.SubdomainName))
            {
                if (INCLUDE.Equals(agentTrigger.SubdomainIncExcName, StringComparison.OrdinalIgnoreCase))
                {
                    var match = Regex.Match(subdomain.Name, agentTrigger.SubdomainName);

                    // if match success dont skip this subdomain  
                    return !match.Success;
                }
                else if (EXCLUDE.Equals(agentTrigger.SubdomainIncExcName, StringComparison.OrdinalIgnoreCase))
                {
                    var match = Regex.Match(subdomain.Name, agentTrigger.SubdomainName);

                    // if match success skip this subdomain 
                    return match.Success;
                }
            }

            if (!string.IsNullOrEmpty(agentTrigger.SubdomainIncExcServicePort) && !string.IsNullOrEmpty(agentTrigger.SubdomainServicePort))
            {
                if (INCLUDE.Equals(agentTrigger.SubdomainIncExcServicePort, StringComparison.OrdinalIgnoreCase))
                {
                    foreach (var service in subdomain.Services)
                    {
                        var matchService = Regex.Match(service.Name, agentTrigger.SubdomainServicePort);
                        var matchPort = Regex.Match(service.Port.ToString(), agentTrigger.SubdomainServicePort);

                        // if match success service or port dont skip this subdomain  
                        if (matchService.Success || matchPort.Success)
                        {
                            return false;
                        }
                    }

                    return true;
                }
                else if (EXCLUDE.Equals(agentTrigger.SubdomainIncExcServicePort, StringComparison.OrdinalIgnoreCase))
                {
                    foreach (var service in subdomain.Services)
                    {
                        var matchService = Regex.Match(service.Name, agentTrigger.SubdomainServicePort);
                        var matchPort = Regex.Match(service.Port.ToString(), agentTrigger.SubdomainServicePort);

                        // if match success services or port skip this subdomain  
                        if (matchService.Success || matchPort.Success)
                        {
                            return true;
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(agentTrigger.SubdomainIncExcIP) && !string.IsNullOrEmpty(agentTrigger.SubdomainIP))
            {
                if (INCLUDE.Equals(agentTrigger.SubdomainIncExcIP, StringComparison.OrdinalIgnoreCase))
                {
                    var match = Regex.Match(subdomain.IpAddress, agentTrigger.SubdomainIP);

                    // if match success dont skip this subdomain  
                    return !match.Success;
                }
                else if (EXCLUDE.Equals(agentTrigger.SubdomainIncExcIP, StringComparison.OrdinalIgnoreCase))
                {
                    var match = Regex.Match(subdomain.IpAddress, agentTrigger.SubdomainIP);

                    // if match success skip this subdomain 
                    return match.Success;
                }
            }

            if (!string.IsNullOrEmpty(agentTrigger.SubdomainIncExcTechnology) && !string.IsNullOrEmpty(agentTrigger.SubdomainTechnology))
            {
                if (INCLUDE.Equals(agentTrigger.SubdomainIncExcTechnology, StringComparison.OrdinalIgnoreCase))
                {
                    var match = Regex.Match(subdomain.Technology, agentTrigger.SubdomainTechnology);

                    // if match success dont skip this subdomain  
                    return !match.Success;
                }
                else if (EXCLUDE.Equals(agentTrigger.SubdomainIncExcTechnology, StringComparison.OrdinalIgnoreCase))
                {
                    var match = Regex.Match(subdomain.Technology, agentTrigger.SubdomainTechnology);

                    // if match success skip this subdomain 
                    return match.Success;
                }
            }

            if (!string.IsNullOrEmpty(agentTrigger.SubdomainIncExcLabel) && !string.IsNullOrEmpty(agentTrigger.SubdomainLabel))
            {
                if (INCLUDE.Equals(agentTrigger.SubdomainIncExcLabel, StringComparison.OrdinalIgnoreCase))
                {
                    foreach (var label in subdomain.Labels)
                    {
                        var match = Regex.Match(label.Label.Name, agentTrigger.SubdomainLabel);

                        // if match success label dont skip this subdomain  
                        if (match.Success)
                        {
                            return false;
                        }
                    }

                    return true;
                }
                else if (EXCLUDE.Equals(agentTrigger.SubdomainIncExcLabel, StringComparison.OrdinalIgnoreCase))
                {
                    foreach (var label in subdomain.Labels)
                    {
                        var match = Regex.Match(label.Label.Name, agentTrigger.SubdomainLabel);

                        // if match success label skip this subdomain  
                        if (match.Success)
                        {
                            return true;
                        }
                    };
                }
            }

            return false;
        }

        /// <summary>
        /// Obtain the channel, we use the channel to send notification to the frontend (tarminal and logs)
        /// and to register the Runners process
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
