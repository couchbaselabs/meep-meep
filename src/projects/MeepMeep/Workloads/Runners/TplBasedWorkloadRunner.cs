using System;
using System.Threading;
using System.Threading.Tasks;
using Couchbase.Core;
using EnsureThat;

namespace MeepMeep.Workloads.Runners
{
    /// <summary>
    /// Workload runner using the Taks Parallel Library to handle
    /// parallelism of concurrent execution of workloads.
    /// </summary>
    public class TplBasedWorkloadRunner : IWorkloadRunner
    {
        protected readonly int NumOfClients;
        protected readonly TaskScheduler TaskScheduler;

        public TplBasedWorkloadRunner(MeepMeepOptions options)
        {
            Ensure.That(options, "options").IsNotNull();

            NumOfClients = options.NumOfClients;
            ConfigureThreadPool(options.ThreadPoolMinNumOfThreads, options.ThreadPoolMaxNumOfThreads);
            TaskScheduler = CreateTaskScheduler(options.MaximumConcurrencyLevel);
        }

        public virtual void Run(IWorkload workload, IBucket bucket, Action<WorkloadResult> onWorkloadCompleted)
        {
            Ensure.That(workload, "workload").IsNotNull();
            Ensure.That(onWorkloadCompleted, "onWorkloadCompleted").IsNotNull();

            try
            {
                Task.WaitAll(StartWorkloadTasks(workload, bucket, onWorkloadCompleted));
            }
            catch (Exception ex)
            {
                throw new Exception(
                    string.Format("Exception while running workload tasks for \"{0}\".", workload.GetType().Name),
                    ex);
            }
        }

        protected virtual Task[] StartWorkloadTasks(IWorkload workload, IBucket bucket, Action<WorkloadResult> onWorkloadCompleted)
        {
            var tasks = new Task[NumOfClients];

            for (var i = 0; i < tasks.Length; i++)
            {
                tasks[i] = new Task(s =>
                {
                    //saakshi
                    //var workloadIndex = (int)s;
                    var workloadIndex = 1;

                    Thread.CurrentThread.Name = string.Concat(workload.GetType().Name, workloadIndex);

                    var result = workload.Execute(bucket, workloadIndex);

                    onWorkloadCompleted(result);
                }, i);

                tasks[i].Start(TaskScheduler);
            }

            return tasks;
        }

        protected virtual void ConfigureThreadPool(int? minNumOfThreads, int? maxNumOfThreads)
        {
            if (minNumOfThreads.HasValue)
                SetMinWorkerThreads(minNumOfThreads.Value);

            if (maxNumOfThreads.HasValue)
                SetMaxWorkerThreads(maxNumOfThreads.Value);
        }

        protected virtual TaskScheduler CreateTaskScheduler(int? maximumConcurrencyLevel)
        {
            return maximumConcurrencyLevel.HasValue
                ? new LimitedConcurrencyLevelTaskScheduler(maximumConcurrencyLevel.Value)
                : TaskScheduler.Current;
        }

        protected virtual void SetMaxWorkerThreads(int value)
        {
            int wt, ct;
            ThreadPool.GetMaxThreads(out wt, out ct);
            ThreadPool.SetMaxThreads(value, ct);
        }

        protected virtual void SetMinWorkerThreads(int value)
        {
            int wt, ct;
            ThreadPool.GetMinThreads(out wt, out ct);
            ThreadPool.SetMinThreads(value, ct);
        }
    }
}