using System.Text;

namespace Rock.Logging.Diagnostics
{
    public interface IStep
    {
        void AddToReport(StringBuilder report);
    }
}