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

        public AddJsonDocumentWorkload(IWorkloadDocKeyGenerator docKeyGenerator, int workloadSize, int warmupMs, bool enableTiming, string sampleDocument = null)
            : base(docKeyGenerator, workloadSize, warmupMs, enableTiming)
        {
            SampleDocument = sampleDocument ?? SampleDocuments.Default;
            Description = string.Format("ExecuteStore (Add) of {0} JSON doc(s) with doc size: {1}.",
                WorkloadSize,
                SampleDocument.Length);
        }

        protected override async Task<WorkloadOperationResult> OnExecuteStep(IBucket bucket, int workloadIndex, int docIndex, Func<TimeSpan> getTiming)
        {
            var key = DocKeyGenerator.Generate(workloadIndex, docIndex);

            var result = await bucket.UpsertAsync(key, SampleDocument);

            return new WorkloadOperationResult(result.Success, result.Message, getTiming())
            {
                DocSize = SampleDocument.Length
            };
        }
    }
}
