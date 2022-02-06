using System.Threading;
using System.Threading.Tasks;

namespace AppCommand.Abstracts
{
    /// <summary>
    /// All commands MUST inherited from this abstract class. 
    /// </summary>
    public abstract class AbstractCommand
    {
        /// <summary>
        /// Body of the command. When a command runs, this method will be invoked
        /// </summary>
        /// <param name="args">Arguments that application runs with them</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns></returns>
        public abstract Task Run(string[] args, CancellationToken cancellationToken = default);
    }
}