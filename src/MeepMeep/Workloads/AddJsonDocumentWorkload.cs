using System;
using System.Threading.Tasks;
using Couchbase.Core;
using MeepMeep.Docs;

namespace MeepMeep.Workloads
{
    /// <summary>
    /// Workload representing the use-case of adding JSON documents.
    /// </summary>
    public class AddJsonDocumentWorkload : WorkloadBase
    {
        protected readonly string SampleDocument;

        public const string DefaultKeyGenerationPart = "ajdw";

        public override string Description { get; }

        public AddJsonDocumentWorkload(IWorkloadDocKeyGenerator docKeyGenerator, int workloadSize, int warmupMs, bool enableTiming, bool useSync, int rateLimit, string sampleDocument = null)
            : base(docKeyGenerator, workloadSize, warmupMs, enableTiming, useSync, rateLimit)
        {
            SampleDocument = sampleDocument ?? SampleDocuments.Default;
            Description = string.Format("ExecuteStore (Add) of {0} JSON doc(s) with doc size: {1}.",
                WorkloadSize,
                SampleDocument.Length);
        }

        protected override Task<WorkloadOperationResult> OnExecuteStep(IBucket bucket, int workloadIndex, int docIndex, Func<TimeSpan> getTiming)
        {
            var key = DocKeyGenerator.Generate(workloadIndex, docIndex);

            if (UseSync)
            {
                var upsertResult = bucket.Upsert(key, SampleDocument);
                return Task.FromResult(
                    new WorkloadOperationResult(upsertResult.Success, upsertResult.Message, getTiming())
                );
            }

            return bucket.UpsertAsync(key, SampleDocument)
                .ContinueWith(task => new WorkloadOperationResult(task.Result.Success, task.Result.Message, getTiming())
                {
                    DocSize = SampleDocument.Length
                });
        }
    }
}
