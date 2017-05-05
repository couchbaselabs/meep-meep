using System;
using System.Linq;
using MeepMeep.Extensions;

namespace MeepMeep.Output
{
    /// <summary>
    /// Simple console writer used to generate output for testruns etc.
    /// </summary>
    public class ConsoleOutputWriter : IOutputWriter
    {
        protected const string Indent = "  ";
        protected readonly object WriterLock = new object();

        public bool Verbose { get; set; }

        public ConsoleOutputWriter()
        {
            Verbose = false;
        }

        public virtual void Write(string output, params object[] formattingArgs)
        {
            WriteIsolated(ConsoleColor.Cyan, () =>
            {
                if (formattingArgs.Any())
                    Console.WriteLine(output, formattingArgs);
                else
                    Console.WriteLine(output);
            });
        }

        public virtual void Write(string value, Exception ex)
        {
            WriteIsolated(ConsoleColor.Red, () =>
            {
                Console.WriteLine(value);

                if (ex is AggregateException)
                {
                    var aggEx = (AggregateException)ex;
                    foreach (var innerException in aggEx.InnerExceptions)
                        Console.WriteLine(innerException.Message);
                }
                else
                    Console.WriteLine(ex.Message);
            });
        }

        public virtual void Write(MeepMeepOptions options)
        {
            Write(options.ToString());
        }

        public virtual void Write(WorkloadResult workloadResult)
        {
            WriteIsolated(ConsoleColor.Yellow, () =>
            {
                OnWriteWorkloadResultShortInfo(workloadResult);
                OnWriteWorkloadGroupedFailedOperations(workloadResult);

                if (Verbose)
                    OnWriteWorkloadResultVerboseInfo(workloadResult);

                Console.WriteLine();
            });
        }

        protected virtual void OnWriteWorkloadResultShortInfo(WorkloadResult workloadResult)
        {
            WriteIsolated(ConsoleColor.Yellow, () =>
            {
                Console.WriteLine();
                Console.WriteLine("[Completed workload: {0}]", workloadResult.Description);
                Console.WriteLine("{0}[Thread: {1}]", Indent, workloadResult.ThreadName);
                Console.WriteLine("{0}[Workload size: {1}]", Indent, workloadResult.WorkloadSize);
                Console.WriteLine("{0}[Total docsize:{1}]", Indent, workloadResult.GetTotalDocSize());
                Console.WriteLine("{0}[Total time (ms):{1}]", Indent, workloadResult.TimeTaken.TotalMilliseconds);
                Console.WriteLine("{0}[Total operations:{1}]", Indent, workloadResult.CountOperations());
                Console.WriteLine("{0}[Failed operations:{1}]", Indent, workloadResult.CountFailedOperations());
                Console.WriteLine("{0}[Avg operation time (ms):{1}]", Indent, workloadResult.GetAverageOperationMs());
                Console.WriteLine("{0}[Avg operations per/second:{1}]", Indent, workloadResult.CountOperations() / workloadResult.TimeTaken.Seconds);
                Console.WriteLine("{0}[Min successful operation time (ms):{1}]", Indent, workloadResult.GetSuccessfulOperationMinDurationMs());
                Console.WriteLine("{0}[Max successful operation time (ms):{1}]", Indent, workloadResult.GetSuccessfulOperationMaxDurationMs());
                Console.WriteLine("{0}[95th percentile operation time (ms):{1}]", Indent, workloadResult.GetSuccessfullOperationPercentile(0.95));
                Console.WriteLine("{0}[98th percentile operation time (ms):{1}]", Indent, workloadResult.GetSuccessfullOperationPercentile(0.98));
                Console.WriteLine("{0}[99th percentile operation time (ms):{1}]", Indent, workloadResult.GetSuccessfullOperationPercentile(0.99));
            });
        }

        protected virtual void OnWriteWorkloadResultVerboseInfo(WorkloadResult workloadResult)
        {
            Console.WriteLine("{0}[Operation results]", Indent);
            foreach (var operationResult in workloadResult.GetOperationResults())
            {
                Console.WriteLine("{0}{0}[Duration (ms):{1}]", Indent, operationResult.TimeTaken);
                Console.WriteLine("{0}{0}[Succeeded:{1}]", Indent, operationResult.Succeeded);
                Console.WriteLine("{0}{0}[Message:{1}]", Indent, operationResult.Message.RemoveNewLines());
            }
        }

        protected virtual void OnWriteWorkloadGroupedFailedOperations(WorkloadResult workloadResult)
        {
            var failedOperations = workloadResult.GetGroupedFailedOperations();
            if (!failedOperations.Any())
                return;

            OnWrite(ConsoleColor.Magenta, () =>
            {
                Console.WriteLine("{0}[Grouped failed operations: #{1}]", Indent, failedOperations.Count);

                foreach (var failedOperation in failedOperations)
                {
                    Console.WriteLine("{0}{0}[Message:{1}]", Indent, failedOperation.Key.RemoveNewLines());
                    Console.WriteLine("{0}{0}[Count:{1}]", Indent, failedOperation.Value);
                }
            });
        }

        protected virtual void WriteIsolated(ConsoleColor fgColor, Action action)
        {
            lock (WriterLock)
                OnWrite(fgColor, action);
        }

        protected virtual void OnWrite(ConsoleColor fgColor, Action action)
        {
            var orgColor = Console.ForegroundColor;

            Console.ForegroundColor = fgColor;

            try
            {
                action();
            }
            finally
            {
                Console.ForegroundColor = orgColor;
            }
        }
    }
}