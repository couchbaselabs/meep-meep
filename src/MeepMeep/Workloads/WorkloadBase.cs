using ComposableAsync;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Couchbase.Core;
using EnsureThat;
using MeepMeep.Docs;
using RateLimiter;

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
        protected readonly bool UseSync;
        protected readonly int _rateLimit;
        private readonly bool _enableTiming;

        public abstract string Description { get; }

        protected WorkloadBase(IWorkloadDocKeyGenerator docKeyGenerator, int workloadSize, int warmupMs, bool enableTiming, bool useSync, int rateLimit)
        {
            Ensure.That(docKeyGenerator, "docKeyGenerator").IsNotNull();
            Ensure.That(workloadSize, "workloadSize").IsGt(0);

            DocKeyGenerator = docKeyGenerator;
            WorkloadSize = workloadSize;
            WarmupMs = warmupMs;
            UseSync = useSync;
            _rateLimit = rateLimit;
            _enableTiming = enableTiming;
        }

        public async Task<WorkloadResult> Execute(IBucket bucket, int workloadIndex)
        {
            Ensure.That(bucket, "bucket").IsNotNull();

            await OnPreExecute(bucket, workloadIndex, 0);

            var workloadResult = CreateWorkloadResult();

            var workloadTimer = Stopwatch.StartNew();
            if (_rateLimit == 0)
            {
                for (var i = 0; i < WorkloadSize; i++)
                {
                    await CreateWorkloadTask(bucket, workloadIndex, 0, workloadResult);
                }
            }
            else
            {
                var timeConstraint = TimeLimiter.GetFromMaxCountByInterval(_rateLimit, TimeSpan.FromSeconds(1));
                for (var i = 0; i < WorkloadSize; i++)
                {
                    await timeConstraint;
                    await CreateWorkloadTask(bucket, workloadIndex, 0, workloadResult);
                }
            }

            workloadResult.TimeTaken = workloadTimer.Elapsed;

            await OnPostExecute(bucket);

            return workloadResult;
        }

        private Task CreateWorkloadTask(IBucket bucket, int workloadIndex, int index, WorkloadResult workloadResult)
        {
            Func<TimeSpan> getTiming;
            if (_enableTiming)
            {
                var sw = Stopwatch.StartNew();
                getTiming = () => sw.Elapsed;
            }
            else
            {
                getTiming = () => TimeSpan.Zero;
            }

            try
            {
                return OnExecuteStep(bucket, workloadIndex, index, getTiming)
                    .ContinueWith(task => task.IsFaulted
                        ? workloadResult.Register(new WorkloadOperationResult(task.Exception, getTiming()))
                        : workloadResult.Register(task.Result));
            }
            catch (Exception ex)
            {
                return workloadResult.Register(new WorkloadOperationResult(ex, getTiming()));
            }
        }

        protected virtual WorkloadResult CreateWorkloadResult()
        {
            return new WorkloadResult(
                Description,
                Thread.CurrentThread.Name ?? Thread.CurrentThread.ManagedThreadId.ToString(CultureInfo.InvariantCulture),
                WorkloadSize);
        }

        /// <summary>
        /// Not included in timing. Could be used to perform setup logic.
        /// </summary>
        protected virtual Task OnPreExecute(IBucket bucket, int workloadInex, int opIndex)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Not included in timing. Could be used to perform cleanup logic.
        /// </summary>
        protected virtual Task OnPostExecute(IBucket bucket)
        {
            return Task.CompletedTask;
        }

        protected abstract Task<WorkloadOperationResult> OnExecuteStep(IBucket bucket, int workloadIndex, int opIndex, Func<TimeSpan> getTiming);
    }
}
