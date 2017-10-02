﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CommandLine;

namespace MeepMeep
{
    /// <summary>
    /// Options constructed via command line arguments.
    /// Used to set the scene; to configure workload runners, workloads etc.
    /// </summary>
    public class MeepMeepOptions
    {
        protected const string NullOutputValue = "<null>";

        [Option('n', "nodes", Separator = ' ', HelpText = "Space separated list of nodes to connect to.", Default = new[] { "couchbase://localhost" })]
        public IEnumerable<string> Nodes { get; set; }

        [Option('b', "bucket", HelpText = "Name of the Bucket", Default = "default")]
        public string Bucket { get; set; }

        [Option('p', "bucketpassword", HelpText = "Password for the bucket.", Default = "")]
        public string BucketPassword { get; set; }

        [Option('c', "num-clients", HelpText = "Number of client sessions.", Default = 1)]
        public int NumOfClients { get; set; }

        [Option('s', "wl-size", HelpText = "The workload size. Depending on Workload, it could e.g. represent the num of documents to work with (per client session).", Default = 20000)]
        public int WorkloadSize { get; set; }

        [Option("doc-sample-path", HelpText = "Path to a file with an UTF8 JSON document, e.g used for inserts.", Default = "")]
        public string DocSamplePath { get; set; }

        [Option("doc-key-prefix", HelpText = "Used as a prefix to generated document keys.", Default = "mm")]
        public string DocKeyPrefix { get; set; }

        [Option("doc-key-seed", HelpText = "Used as a starting seed when generating document keys.", Default = 0)]
        public int DocKeySeed { get; set; }

        [Option("doc-key-range", HelpText = "The maximum range of document IDs use above doc-key-seed.", Default = 1000)]
        public int DocKeyRange { get; set; }

        [Option("clusteruser", HelpText = "Cluster Administrator username.", Default = "Administrator")]
        public string ClusterUsername { get; set; }

        [Option("clusterpassword", HelpText = "Cluster Administraor password.", Default = "password")]
        public string ClusterPassword { get; set; }

        [Option('w', "warmup-ms", HelpText = "Determines how long time (ms) it takes before a work load's operation results is being seen as part of the workload.", Default = 100)]
        public int WarmupMs { get; set; }

        [Option('v', "verbose", HelpText = "Determines detail level of output generated by output writers etc.", Default = false)]
        public bool Verbose { get; set; }

        [Option('r', "mutation-percentage", HelpText = "The percentage of operations that should be mutations.", Default = 0.33)]
        public double MutationPercentage { get; set; }

        [Option("workload-type", HelpText = "The type of workload to be used.", Default = WorkloadType.MutationPercentage)]
        public WorkloadType WorkloadType { get; set; }

        [Option('t', "enable-timings", HelpText = "Time operations and output at end of workload.", Default = false)]
        public bool EnableOperationTiming { get; set; }

        [Option("use-json", HelpText = "Enable writing JSON values instead of raw bytes.", Default = false)]
        public bool UseJson { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach (var property in GetOptionProperties())
            {
                sb.Append(property.Name);
                sb.Append("=");

                var value = property.GetValue(this, new object[0]);

                switch (value)
                {
                    case null:
                        sb.AppendLine(NullOutputValue);
                        break;
                    case string _:
                        sb.Append("\"");
                        sb.Append(value);
                        sb.AppendLine("\"");
                        break;
                    case string[] _:
                        sb.Append("\"");
                        sb.Append(string.Join(" ", (string[])value));
                        sb.AppendLine("\"");
                        break;
                    default:
                        sb.AppendLine(value.ToString());
                        break;
                }
            }

            return sb.ToString();
        }

        protected virtual PropertyInfo[] GetOptionProperties()
        {
            return GetType()
                .GetProperties()
                .Where(p => p.CanRead && p.GetCustomAttributes(false).Any())
                .ToArray();
        }
    }
}