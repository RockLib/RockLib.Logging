using System.Threading.Tasks;

namespace Rock.Logging
{
    public partial class LoggerExtensions
    {
        /// <summary>
        /// A Task that has already been completed successfully.
        /// </summary>
        private static readonly Task _completedTask = Task.FromResult(0);
    }
}