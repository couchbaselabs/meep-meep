using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using Couchbase;
using EnsureThat;
using MeepMeep.Docs;

namespace MeepMeep.Workloads
{
    /// <summary>
    /// Base class used for simplifying creation of workloads.
    /// </summary>
    public abstract class WorkloadBase : IWorkload
    {
        protected readonly IWorkloadDocKeyGenerator DocKeyGenerator;
        protected readonly int WorkloadSize;
        protected readonly int WarmupMs;

        public abstract string Description { get; }

        protected WorkloadBase(IWorkloadDocKeyGenerator docKeyGenerator, int workloadSize, int warmupMs)
        {
            Ensure.That(docKeyGenerator, "docKeyGenerator").IsNotNull();
            Ensure.That(workloadSize, "workloadSize").IsGt(0);

            DocKeyGenerator = docKeyGenerator;
            WorkloadSize = workloadSize;
            WarmupMs = warmupMs;
        }

        public virtual WorkloadResult Execute(ICouchbaseClient client, int workloadIndex)
        {
            Ensure.That(client, "client").IsNotNull();

            var workloadResult = CreateWorkloadResult();
            var opIndex = 0;
            var numOfWarmups = 0;
            var startedAt = DateTime.Now;
            var sw = new Stopwatch();

            OnPreExecute(client);

            while (true)
            {
                var isWarmingUp = IsWarmingUp(startedAt);
                if (isWarmingUp)
                    numOfWarmups++;

                if (opIndex >= WorkloadSize + numOfWarmups)
                    break;

                WorkloadOperationResult opResult = null;

                try
                {
                    sw.Reset();
                    //saakshi
                    //opResult = OnExecuteStep(client, workloadIndex, opIndex, sw);
                    opResult = OnExecuteStep(client, workloadIndex, 0, sw);
                }
                catch (Exception ex)
                {
                    sw.Stop();
                    opResult = new WorkloadOperationResult(ex, sw.Elapsed);
                }
                finally
                {
                    if (!isWarmingUp)
                        workloadResult.Register(opResult);
                }
                opIndex++;
            }

            OnPostExecute(client);

            return workloadResult;
        }

        protected virtual WorkloadResult CreateWorkloadResult()
        {
            return new WorkloadResult(
                Description,
                Thread.CurrentThread.Name ?? Thread.CurrentThread.ManagedThreadId.ToString(CultureInfo.InvariantCulture),
                WorkloadSize);
        }

        protected bool IsWarmingUp(DateTime startedAt)
        {
            return WarmupMs > 0 && WarmupMs > (DateTime.Now - startedAt).TotalMilliseconds;
        }

        /// <summary>
        /// Not included in timing. Could be used to perform setup logic.
        /// </summary>
        protected virtual void OnPreExecute(ICouchbaseClient client) { }

        /// <summary>
        /// Not included in timing. Could be used to perform cleanup logic.
        /// </summary>
        protected virtual void OnPostExecute(ICouchbaseClient client) { }

        protected abstract WorkloadOperationResult OnExecuteStep(ICouchbaseClient client, int workloadIndex, int opIndex, Stopwatch sw);
    }
}