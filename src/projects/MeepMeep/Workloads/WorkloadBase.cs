using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using Couchbase.Core;
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

        public virtual WorkloadResult Execute(IBucket bucket, int workloadIndex)
        {
            Ensure.That(bucket, "bucket").IsNotNull();

            var workloadResult = CreateWorkloadResult();
            var opIndex = 0;
            var numOfWarmups = 0;
            var startedAt = DateTime.Now;
            var sw = new Stopwatch();

            OnPreExecute(bucket);

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
                    opResult = OnExecuteStep(bucket, workloadIndex, 0, sw);
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

            OnPostExecute(bucket);

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
        protected virtual void OnPreExecute(IBucket bucket) { }

        /// <summary>
        /// Not included in timing. Could be used to perform cleanup logic.
        /// </summary>
        protected virtual void OnPostExecute(IBucket bucket) { }

        protected abstract WorkloadOperationResult OnExecuteStep(IBucket bucket, int workloadIndex, int opIndex, Stopwatch sw);
    }
}