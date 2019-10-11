using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine;
using Couchbase;
using Couchbase.Authentication;
using Couchbase.Configuration.Client;
using Couchbase.Core.Transcoders;
using MeepMeep.Docs;
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
                var parser = new Parser(settings =>
                {
                    settings.IgnoreUnknownArguments = true;
                    settings.CaseSensitive = false;
                    settings.HelpWriter = Console.Out;
                });

                parser.ParseArguments<MeepMeepOptions>(args)
                    .WithParsed(Run)
                    .WithNotParsed(errors =>
                    {
                        errors = errors.ToList();
                        if (errors.Count() == 1 && errors.Any(error =>
                                error is HelpRequestedError || error is VersionRequestedError))
                        {
                            return;
                        }

                        OutputWriter.Write("Error parsing command line arguments");
                        foreach (var error in errors)
                        {
                            OutputWriter.Write(error.ToString());
                        }
                    });
            }
            catch (Exception ex)
            {
                OutputWriter.Write("Unhandled exception.", ex);
            }
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
                    MinSize = options.PoolMin,
                    MaxSize = options.PoolMax
                },
                Transcoder = () => new DefaultTranscoder(),
                OperationTracingEnabled = false,
                OrphanedResponseLoggingEnabled = false,
                UseSsl = options.UseSsl
            };

            if (!options.VerifySslCerts)
            {
                config.KvServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            }

            using (var cluster = new Cluster(config))
            {
                var authenticator = new ClassicAuthenticator(options.ClusterUsername, options.ClusterPassword)
                {
                    BucketCredentials = {{options.Bucket, options.BucketPassword}}
                };
                cluster.Authenticate(authenticator);

                OutputWriter.Write("Connecting to cluster...");
                var bucket = cluster.OpenBucket(options.Bucket);

                if (options.FlushBucket)
                {
                    OutputWriter.Write("Flushing bucket: {0}", options.Bucket);
                    var bucketManager = bucket.CreateManager();
                    bucketManager.Flush();
                }

                OutputWriter.Write("Running workloads...");

                var workload = CreateWorkload(options);
                var runner = CreateRunner(options);
                runner.Run(workload, bucket, OnWorkloadCompleted).Wait();
            }

            OutputWriter.Write("Completed");
        }

        private static IWorkloadRunner CreateRunner(MeepMeepOptions options)
        {
            return new TaskBasedWorkloadRunner(options);
        }

        private static IWorkload CreateWorkload(MeepMeepOptions options)
        {
            switch (options.WorkloadType)
            {
                case WorkloadType.MutationPercentage:
                    return new MixedGetSetJsonDocumentWorkload(
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
                        options.UseSync,
                        options.RateLimit,
                        SampleDocuments.ReadJsonSampleDocument(options.DocSamplePath));
                case WorkloadType.SetOnly:
                    return new AddJsonDocumentWorkload(
                        new RangedWorkloadDocKeyGenerator(
                            options.DocKeyPrefix,
                            AddJsonDocumentWorkload.DefaultKeyGenerationPart,
                            options.DocKeySeed,
                            options.DocKeyRange
                        ),
                        options.WorkloadSize,
                        options.WarmupMs,
                        options.EnableOperationTiming,
                        options.UseSync,
                        options.RateLimit,
                        SampleDocuments.ReadJsonSampleDocument(options.DocSamplePath));
                case WorkloadType.SetAndGet:
                    return new AddAndGetJsonDocumentWorkload(
                        new RangedWorkloadDocKeyGenerator(
                            options.DocKeyPrefix,
                            AddAndGetJsonDocumentWorkload.DefaultKeyGenerationPart,
                            options.DocKeySeed,
                            options.DocKeyRange
                        ),
                        options.WorkloadSize,
                        options.WarmupMs,
                        options.EnableOperationTiming,
                        options.UseSync,
                        options.RateLimit);
                default:
                    throw new ArgumentException($"Unknown workload type: {options.WorkloadType}");
            }
        }

        private static void OnWorkloadCompleted(WorkloadResult workloadResult)
        {
            OutputWriter.Write(workloadResult);
        }
    }
}
