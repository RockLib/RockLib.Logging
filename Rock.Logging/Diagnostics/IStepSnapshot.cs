using System.Text;

namespace Rock.Logging.Diagnostics
{
    public interface IStepSnapshot
    {
        void AddToReport(StringBuilder report);
    }
}