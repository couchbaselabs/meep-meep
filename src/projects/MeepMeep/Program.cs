using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Couchbase;
using Couchbase.Authentication;
using Couchbase.Configuration.Client;
using Couchbase.Core.Transcoders;
using MeepMeep.Docs;
using MeepMeep.Input;
using MeepMeep.Output;
using MeepMeep.Workloads;
using MeepMeep.Workloads.Runners;

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
                if (options == null)
                {
                    OutputWriter.Write("Error parsing command line arguments");
                    return;
                }

                try
                {
                    Run(options);
                }
                catch (AggregateException ex)
                {
                    if (ex.InnerExceptions.OfType<HttpRequestException>().Any())
                    {
                        OutputWriter.Write("Error connecting to cluster, please verify the nodes parameter is correct");
                    }
                    else
                    {
                        throw;
                    }
                }
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
            OutputWriter.Verbose = options.Verbose;

            OutputWriter.Write("Running with options:");
            OutputWriter.Write(options);

            var config = new ClientConfiguration
            {
                Servers = options.Nodes.Select(x => new Uri(x)).ToList(),
                PoolConfiguration = new PoolConfiguration
                {
                    MinSize = 1,
                    MaxSize = 1
                },
                Transcoder = () => options.UseJson ? new DefaultTranscoder() : new BinaryTranscoder()
            };

            using (var cluster = new Cluster(config))
            {
                cluster.Authenticate(new ClusterCredentials
                {
                    ClusterUsername = options.ClusterUsername,
                    ClusterPassword = options.ClusterPassword,
                    BucketCredentials = new Dictionary<string, string>
                    {
                        {options.Bucket, options.BucketPassword}
                    }
                });

                OutputWriter.Write("Preparing bucket:");

                var bucket = cluster.OpenBucket(options.Bucket);
                var bucketManager = bucket.CreateManager();
                bucketManager.Flush();

                OutputWriter.Write("Running workloads...");

                var runner = CreateRunner(options);
                foreach (var workload in CreateWorkloads(options))
                {
                    runner.Run(workload, bucket, OnWorkloadCompleted);
                }
            }

            OutputWriter.Write("Completed");
        }

        private static IWorkloadRunner CreateRunner(MeepMeepOptions options)
        {
            return new TplBasedWorkloadRunner(options);
        }

        private static IEnumerable<IWorkload> CreateWorkloads(MeepMeepOptions options)
        {
            switch (options.WorkloadType)
            {
                case WorkloadType.MutationPercentage:
                    yield return new MixedGetSetJsonDocumentWorkload(
                        new RangedWorkloadDocKeyGenerator(
                            options.DocKeyPrefix, 
                            MixedGetSetJsonDocumentWorkload.DefaultKeyGenerationPart,
                            options.DocKeySeed,
                            options.DocKeyRange
                        ),
                        options.WorkloadSize,
                        options.WarmupMs,
                        options.MutationPercentage,
                        options.EnableOperationTiming,
                        SampleDocuments.ReadJsonSampleDocument(options.DocSamplePath));
                    break;
                case WorkloadType.SetOnly:
                    yield return new AddJsonDocumentWorkload(
                        new RangedWorkloadDocKeyGenerator(
                            options.DocKeyPrefix, 
                            AddJsonDocumentWorkload.DefaultKeyGenerationPart, 
                            options.DocKeySeed,
                            options.DocKeyRange
                        ),
                        options.WorkloadSize,
                        options.WarmupMs,
                        options.EnableOperationTiming,
                        SampleDocuments.ReadJsonSampleDocument(options.DocSamplePath));
                    break;
                case WorkloadType.SetAndGet:
                    yield return new AddAndGetJsonDocumentWorkload(
                        new RangedWorkloadDocKeyGenerator(
                            options.DocKeyPrefix,
                            AddAndGetJsonDocumentWorkload.DefaultKeyGenerationPart,
                            options.DocKeySeed,
                            options.DocKeyRange
                        ),
                        options.WorkloadSize,
                        options.WarmupMs,
                        options.EnableOperationTiming);
                    break;
            }
        }

        private static void OnWorkloadCompleted(WorkloadResult workloadResult)
        {
            OutputWriter.Write(workloadResult);
        }
    }
}
