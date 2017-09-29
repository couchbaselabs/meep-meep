using System;
using System.Linq;
using System.Threading.Tasks;
using Couchbase.Core;
using EnsureThat;

namespace MeepMeep.Workloads.Runners
{
    /// <summary>
    /// Workload runner using the Taks Parallel Library to handle
    /// parallelism of concurrent execution of workloads.
    /// </summary>
    public class TaskBasedWorkloadRunner : IWorkloadRunner
    {
        private readonly int _numOfClients;

        public TaskBasedWorkloadRunner(MeepMeepOptions options)
        {
            Ensure.That(options, "options").IsNotNull();
            _numOfClients = options.NumOfClients;
        }

        public Task Run(IWorkload workload, IBucket bucket, Action<WorkloadResult> onWorkloadCompleted)
        {
            Ensure.That(workload, "workload").IsNotNull();
            Ensure.That(onWorkloadCompleted, "onWorkloadCompleted").IsNotNull();

            try
            {
                return Task.WhenAll(
                    Enumerable.Range(1, _numOfClients)
                        .Select(index =>
                        {
                            return workload.Execute(bucket, index)
                                .ContinueWith(task =>
                                {
                                    onWorkloadCompleted(task.Result);
                                });
                        })
                );
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception while running workload tasks for \"{workload.GetType().Name}\".", ex);
            }
        }
    }
}
