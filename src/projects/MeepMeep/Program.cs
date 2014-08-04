using System;
using System.Collections.Generic;
using Couchbase;
using MeepMeep.Docs;
using MeepMeep.Extensions;
using MeepMeep.Input;
using MeepMeep.Output;
using MeepMeep.Workloads;
using MeepMeep.Workloads.Runners;
using Couchbase.Management;

namespace MeepMeep
{
    public class Program
    {
        private static readonly IOutputWriter OutputWriter = new ConsoleOutputWriter();

        public static void Main(string[] args)
        {
            try
            {
                var options = ParseCommandLine(args);

                if (options != null)
                    Run(options);
            }
            catch (Exception ex)
            {
                OutputWriter.Write("Unhandled exception.", ex);
            }
        }

        private static MeepMeepOptions ParseCommandLine(string[] args)
        {
            var commandLineParser = new CommandLineParser();
            var options = new MeepMeepOptions();

            if (!commandLineParser.Parse(options, args))
            {
                OutputWriter.Write(options.GetHelp());
                return null;
            }

            return options;
        }

        private static void Run(MeepMeepOptions options)
        {
            /* saakshi change */
            var cluster = new CouchbaseCluster(options.ToClientConfig());
            cluster.FlushBucket(options.Bucket);
            /* change over */

            OutputWriter.Verbose = options.Verbose;

            OutputWriter.Write("Running with options:");
            OutputWriter.Write(options);

            OutputWriter.Write("Running workloads...");

            using (var client = CreateClient(options))
            {
                var runner = CreateRunner(options);

                foreach (var workload in CreateWorkloads(options))
                    runner.Run(workload, client, OnWorkloadCompleted);
            }

            OutputWriter.Write("Completed");
        }

        private static ICouchbaseClient CreateClient(MeepMeepOptions options)
        {
            return new CouchbaseClient(options.ToClientConfig());
        }

        private static IWorkloadRunner CreateRunner(MeepMeepOptions options)
        {
            return new TplBasedWorkloadRunner(options);
        }

        private static IEnumerable<IWorkload> CreateWorkloads(MeepMeepOptions options)
        {
            yield return new AddJsonDocumentWorkload(
                new DefaultWorkloadDocKeyGenerator(options.DocKeyPrefix, AddJsonDocumentWorkload.DefaultKeyGenerationPart, options.DocKeySeed),
                options.WorkloadSize,
                options.WarmupMs,
                SampleDocuments.ReadJsonSampleDocument(options.DocSamplePath));

            //saakshi
            //yield return new AddAndGetJsonDocumentWorkload(
            //    new DefaultWorkloadDocKeyGenerator(options.DocKeyPrefix, AddAndGetJsonDocumentWorkload.DefaultKeyGenerationPart, options.DocKeySeed),
            //    options.WorkloadSize,
            //    options.WarmupMs);
        }

        private static void OnWorkloadCompleted(WorkloadResult workloadResult)
        {
            OutputWriter.Write(workloadResult);
        }
    }
}
