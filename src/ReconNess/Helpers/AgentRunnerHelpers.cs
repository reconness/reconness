using ReconNess.Core.Models;
using ReconNess.Entities;
using System;
using System.Text.RegularExpressions;

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
            var agentTrigger = agentRunner.Agent.AgentTrigger;
            if (agentTrigger == null)
            {
                return false;
            }

            var agentTypeTarget = AgentRunnerTypes.CURRENT_TARGET.Equals(agentRunnerType) || AgentRunnerTypes.ALL_DIRECTORIES.Equals(agentRunnerType);
            var agentTypeRootDomain = AgentRunnerTypes.CURRENT_ROOTDOMAIN.Equals(agentRunnerType) || AgentRunnerTypes.ALL_ROOTDOMAINS.Equals(agentRunnerType);
            var agentTypeSubdomain = AgentRunnerTypes.CURRENT_SUBDOMAIN.Equals(agentRunnerType) || AgentRunnerTypes.ALL_SUBDOMAINS.Equals(agentRunnerType);

            if (agentTrigger.SkipIfRunBefore && RanBefore(agentRunner, agentTypeTarget, agentTypeRootDomain, agentTypeSubdomain))
            {
                return true;
            }

            if (agentTypeTarget && SkipTarget(agentRunner, agentTrigger))
            {
                return true;
            }

            if (agentTypeRootDomain && SkipRootDomain(agentRunner, agentTrigger))
            {
                return true;
            }

            if (agentTypeSubdomain && SkipSubdomain(agentRunner, agentTrigger))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="agentRunner"></param>
        /// <param name="agentTypeTarget"></param>
        /// <param name="agentTypeRootDomain"></param>
        /// <param name="agentTypeSubdomain"></param>
        /// <returns></returns>
        private static bool RanBefore(AgentRunner agentRunner, bool agentTypeTarget, bool agentTypeRootDomain, bool agentTypeSubdomain)
        {
            var agentRanBeforeInThisTarget = agentTypeTarget && agentRunner.Target != null &&
                                                 !string.IsNullOrEmpty(agentRunner.Target.AgentsRawBefore) &&
                                                 agentRunner.Target.AgentsRawBefore.Contains(agentRunner.Agent.Name);

            var agentRanBeforeInThisRootDomain = agentTypeRootDomain && agentRunner.RootDomain != null &&
                                                 !string.IsNullOrEmpty(agentRunner.RootDomain.AgentsRawBefore) &&
                                                 agentRunner.RootDomain.AgentsRawBefore.Contains(agentRunner.Agent.Name);

            var agentRanBeforeInThisSubdomain = agentTypeSubdomain && agentRunner.Subdomain != null &&
                                                 !string.IsNullOrEmpty(agentRunner.Subdomain.AgentsRawBefore) &&
                                                 agentRunner.Subdomain.AgentsRawBefore.Contains(agentRunner.Agent.Name);

            return agentRanBeforeInThisTarget || agentRanBeforeInThisRootDomain || agentRanBeforeInThisSubdomain;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="agentRunner"></param>
        /// <param name="agentTrigger"></param>
        /// <returns></returns>
        private static bool SkipTarget(AgentRunner agentRunner, AgentTrigger agentTrigger)
        {
            if (agentTrigger.TargetHasBounty && (agentRunner.Target == null || !agentRunner.Target.HasBounty))
            {
                return true;
            }

            if (!string.IsNullOrEmpty(agentTrigger.TargetIncExcName) && !string.IsNullOrEmpty(agentTrigger.TargetName))
            {
                if ("include".Equals(agentTrigger.TargetIncExcName, StringComparison.OrdinalIgnoreCase))
                {
                    var match = Regex.Match(agentRunner.Target.Name, agentTrigger.TargetName);

                    // if match success dont skip this target  
                    return !match.Success;
                }
                else if ("exclude".Equals(agentTrigger.TargetIncExcName, StringComparison.OrdinalIgnoreCase))
                {
                    var match = Regex.Match(agentRunner.Target.Name, agentTrigger.TargetName);

                    // if match success skip this target 
                    return match.Success;
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="agentRunner"></param>
        /// <param name="agentTrigger"></param>
        /// <returns></returns>
        private static bool SkipRootDomain(AgentRunner agentRunner, AgentTrigger agentTrigger)
        {
            if (agentTrigger.RootdomainHasBounty && (agentRunner.RootDomain == null || !agentRunner.RootDomain.HasBounty))
            {
                return true;
            }

            if (!string.IsNullOrEmpty(agentTrigger.RootdomainIncExcName) && !string.IsNullOrEmpty(agentTrigger.RootdomainName))
            {
                if ("include".Equals(agentTrigger.RootdomainIncExcName, StringComparison.OrdinalIgnoreCase))
                {
                    var match = Regex.Match(agentRunner.RootDomain.Name, agentTrigger.RootdomainName);

                    // if match success dont skip this rootdomain  
                    return !match.Success;
                }
                else if ("exclude".Equals(agentTrigger.RootdomainIncExcName, StringComparison.OrdinalIgnoreCase))
                {
                    var match = Regex.Match(agentRunner.RootDomain.Name, agentTrigger.RootdomainName);

                    // if match success skip this rootdomain 
                    return match.Success;
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="agentRunner"></param>
        /// <param name="agentTrigger"></param>
        /// <returns></returns>
        private static bool SkipSubdomain(AgentRunner agentRunner, AgentTrigger agentTrigger)
        {
            if (agentTrigger.SubdomainHasBounty && (agentRunner.Subdomain == null || agentRunner.Subdomain.HasBounty == null || !agentRunner.Subdomain.HasBounty.Value))
            {
                return true;
            }

            if (agentTrigger.SubdomainHasHttpOrHttpsOpen && (agentRunner.Subdomain == null || agentRunner.Subdomain.HasHttpOpen == null || !agentRunner.Subdomain.HasHttpOpen.Value))
            {
                return true;
            }

            if (agentTrigger.SubdomainIsAlive && (agentRunner.Subdomain == null || agentRunner.Subdomain.IsAlive == null || !agentRunner.Subdomain.IsAlive.Value))
            {
                return true;
            }

            if (agentTrigger.SubdomainIsMainPortal && (agentRunner.Subdomain == null || agentRunner.Subdomain.IsMainPortal == null || !agentRunner.Subdomain.IsMainPortal.Value))
            {
                return true;
            }

            if (!string.IsNullOrEmpty(agentTrigger.SubdomainIncExcName) && !string.IsNullOrEmpty(agentTrigger.SubdomainName))
            {
                if ("include".Equals(agentTrigger.SubdomainIncExcName, StringComparison.OrdinalIgnoreCase))
                {
                    var match = Regex.Match(agentRunner.Subdomain.Name, agentTrigger.SubdomainName);

                    // if match success dont skip this subdomain  
                    return !match.Success;
                }
                else if ("exclude".Equals(agentTrigger.SubdomainIncExcName, StringComparison.OrdinalIgnoreCase))
                {
                    var match = Regex.Match(agentRunner.Subdomain.Name, agentTrigger.SubdomainName);

                    // if match success skip this subdomain 
                    return match.Success;
                }
            }

            if (!string.IsNullOrEmpty(agentTrigger.SubdomainIncExcServicePort) && !string.IsNullOrEmpty(agentTrigger.SubdomainServicePort))
            {
                if ("include".Equals(agentTrigger.SubdomainIncExcServicePort, StringComparison.OrdinalIgnoreCase))
                {
                    foreach (var service in agentRunner.Subdomain.Services)
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
                else if ("exclude".Equals(agentTrigger.SubdomainIncExcServicePort, StringComparison.OrdinalIgnoreCase))
                {
                    foreach (var service in agentRunner.Subdomain.Services)
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
                if ("include".Equals(agentTrigger.SubdomainIncExcIP, StringComparison.OrdinalIgnoreCase))
                {
                    var match = Regex.Match(agentRunner.Subdomain.IpAddress, agentTrigger.SubdomainIP);

                    // if match success dont skip this subdomain  
                    return !match.Success;
                }
                else if ("exclude".Equals(agentTrigger.SubdomainIncExcIP, StringComparison.OrdinalIgnoreCase))
                {
                    var match = Regex.Match(agentRunner.Subdomain.IpAddress, agentTrigger.SubdomainIP);

                    // if match success skip this subdomain 
                    return match.Success;
                }
            }

            if (!string.IsNullOrEmpty(agentTrigger.SubdomainIncExcTechnology) && !string.IsNullOrEmpty(agentTrigger.SubdomainTechnology))
            {
                if ("include".Equals(agentTrigger.SubdomainIncExcTechnology, StringComparison.OrdinalIgnoreCase))
                {
                    var match = Regex.Match(agentRunner.Subdomain.Technology, agentTrigger.SubdomainTechnology);

                    // if match success dont skip this subdomain  
                    return !match.Success;
                }
                else if ("exclude".Equals(agentTrigger.SubdomainIncExcTechnology, StringComparison.OrdinalIgnoreCase))
                {
                    var match = Regex.Match(agentRunner.Subdomain.Technology, agentTrigger.SubdomainTechnology);

                    // if match success skip this subdomain 
                    return match.Success;
                }
            }

            if (!string.IsNullOrEmpty(agentTrigger.SubdomainIncExcLabel) && !string.IsNullOrEmpty(agentTrigger.SubdomainLabel))
            {
                if ("include".Equals(agentTrigger.SubdomainIncExcLabel, StringComparison.OrdinalIgnoreCase))
                {
                    foreach (var label in agentRunner.Subdomain.Labels)
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
                else if ("exclude".Equals(agentTrigger.SubdomainIncExcLabel, StringComparison.OrdinalIgnoreCase))
                {
                    foreach (var label in agentRunner.Subdomain.Labels)
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
