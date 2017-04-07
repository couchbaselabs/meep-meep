using System;
using Couchbase.Core;

namespace MeepMeep
{
    /// <summary>
    /// Responsible for executing workloads <see cref="IWorkload"/>.
    /// Orchestras parallelism, configuration of threadpool settings etc.
    /// </summary>
    public interface IWorkloadRunner
    {
        /// <summary>
        /// Runs the workload and as soon as a task is completed, the result
        /// is passed to the callback <paramref name="onWorkloadCompleted"/>.
        /// Depending on the runner and the configuration of it, there can be
        /// many workloads running in parallel.
        /// </summary>
        /// <param name="workload"></param>
        /// <param name="bucket"></param>
        /// <param name="onWorkloadCompleted"></param>
        void Run(IWorkload workload, IBucket bucket, Action<WorkloadResult> onWorkloadCompleted);
    }
}