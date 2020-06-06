using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using ReconNess.Core.Models;
using ReconNess.Core.Services;
using ReconNess.Entities;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Services
{
    /// <summary>
    /// This class implement <see cref="IScriptEngineService"/>
    /// </summary>
    public class ScriptEngineService : IScriptEngineService
    {
        private Agent agent;

        /// <summary>
        /// <see cref="IScriptEngineService.InintializeAgent(Agent)"/>
        /// </summary>
        /// <param name="agent"></param>
        public void InintializeAgent(Agent agent)
        {
            this.agent = agent;
        }

        /// <summary>
        /// <see cref="IScriptEngineService.ParseInputAsync(string, int, CancellationToken)"/>
        /// </summary>
        public async Task<ScriptOutput> ParseInputAsync(string lineInput, int lineInputCount, string script = null, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(script))
            {
                if (this.agent == null)
                {
                    throw new Exception("You need to call the method InintializeAgent first");
                }

                script = this.agent.Script;
            }

            var globals = new Globals { lineInput = lineInput, lineInputCount = lineInputCount };
            return await CSharpScript.EvaluateAsync<ScriptOutput>(script,
                ScriptOptions.Default.WithImports("ReconNess.Core.Models.ScriptOutput")
                .AddReferences(
                    Assembly.GetAssembly(typeof(ScriptOutput)),
                    Assembly.GetAssembly(typeof(Exception)),
                    Assembly.GetAssembly(typeof(System.Text.RegularExpressions.Regex)))
                , globals: globals);
        }
    }

    /// <summary>
    /// Global Class
    /// </summary>
    public class Globals
    {
        public string lineInput;
        public int lineInputCount;
    }
}
