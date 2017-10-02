using System;
using EnsureThat;

namespace MeepMeep
{
    /// <summary>
    /// Represents a single operation performed within a workload.
    /// Contained in <see cref="WorkloadResult"/>.
    /// </summary>
    public class WorkloadOperationResult
    {
        public TimeSpan TimeTaken { get; set; }
        public bool Succeeded { get; set; }
        public string Message { get; set; }
        public int DocSize { get; set; }

        public WorkloadOperationResult(Exception ex, TimeSpan timeTaken)
        {
            Ensure.That(ex, "ex").IsNotNull();

            Succeeded = false;
            Message = ex.Message;
            TimeTaken = timeTaken;
        }

        public WorkloadOperationResult(bool succeeded, string message, TimeSpan timeTaken)
        {
            Succeeded = succeeded;
            Message = message;
            TimeTaken = timeTaken;
        }
    }
}
