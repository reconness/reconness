using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using NLog;
using ReconNess.Core.Models;
using ReconNess.Core.Services;
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
        protected static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        /// <inheritdoc/>
        public async Task<ScriptOutput> TerminalOutputParseAsync(string script, string lineInput, int lineInputCount, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var globals = new Globals { lineInput = lineInput, lineInputCount = lineInputCount };
            return await CSharpScript.EvaluateAsync<ScriptOutput>(script,
                ScriptOptions.Default.WithImports("ReconNess.Core.Models.ScriptOutput")
                .AddReferences(
                    Assembly.GetAssembly(typeof(ScriptOutput)),
                    Assembly.GetAssembly(typeof(Exception)),
                    Assembly.GetAssembly(typeof(System.Text.RegularExpressions.Regex)))
                , globals: globals, cancellationToken: cancellationToken);
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
