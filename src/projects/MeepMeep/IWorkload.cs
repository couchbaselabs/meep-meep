using System.Threading.Tasks;
using Couchbase.Core;

namespace MeepMeep
{
    /// <summary>
    /// Represents a specific use-case that generates workload against
    /// your cluster.
    /// </summary>
    public interface IWorkload
    {
        /// <summary>
        /// Simple Meta description of the workload.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Will start execute the workload against sent client.
        /// </summary>
        /// <param name="bucket"></param>
        /// <param name="workloadIndex"></param>
        /// <returns></returns>
        Task<WorkloadResult> Execute(IBucket bucket, int workloadIndex);
    }
}
