using System;
using System.Collections.Generic;
using System.Linq;
using EnsureThat;

namespace MeepMeep
{
    /// <summary>
    /// Represents the result of a single workload session. Created by <see cref="IWorkload.Execute"/>.
    /// </summary>
    [Serializable]
    public class WorkloadResult
    {
        protected readonly IList<WorkloadOperationResult> OperationResults;

        public string Description { get; private set; }
        public string ThreadName { get; private set; }
        public int WorkloadSize { get; private set; }
        public TimeSpan TimeTaken { get; private set; }

        public WorkloadResult(string description, string threadName, int workloadSize)
        {
            Ensure.That(description, "description").IsNotNullOrWhiteSpace();
            Ensure.That(threadName, "threadName").IsNotNullOrWhiteSpace();

            Description = description;
            ThreadName = threadName;
            WorkloadSize = workloadSize;
            OperationResults = new List<WorkloadOperationResult>();
            TimeTaken = new TimeSpan();
        }

        public virtual void Register(WorkloadOperationResult operationResult)
        {
            OperationResults.Add(operationResult);
            TimeTaken += operationResult.TimeTaken;
        }

        public virtual int CountOperations()
        {
            return OperationResults.Count;
        }

        public virtual int CountFailedOperations()
        {
            return OperationResults.Count(o => !o.Succeeded);
        }

        public virtual long GetTotalDocSize()
        {
            return OperationResults
                .Aggregate<WorkloadOperationResult, long>(0, (current, result) => current + result.DocSize);
        }

        public virtual double GetAverageOperationMs()
        {
            return OperationResults.Average(o => o.TimeTaken.TotalMilliseconds);
        }

        public virtual double? GetSuccessfulOperationMaxDurationMs()
        {
            if (!OperationResults.Any(o => o.Succeeded))
                return null;

            return OperationResults
                .Where(o => o.Succeeded)
                .Max(o => o.TimeTaken.TotalMilliseconds);
        }

        public virtual double? GetSuccessfulOperationMinDurationMs()
        {
            if (!OperationResults.Any(o => o.Succeeded))
                return null;

            return OperationResults
                .Where(o => o.Succeeded)
                .Min(o => o.TimeTaken.TotalMilliseconds);
        }

        public virtual IEnumerable<WorkloadOperationResult> GetOperationResults()
        {
            return OperationResults;
        }

        public virtual IDictionary<string, int> GetGroupedFailedOperations()
        {
            return GetOperationResults()
                .Where(o => !o.Succeeded)
                .GroupBy(o => o.Message)
                .Select(g => new {Message = g.Key, Count = g.Count()})
                .ToDictionary(i => i.Message, i => i.Count);
        }
    }
}