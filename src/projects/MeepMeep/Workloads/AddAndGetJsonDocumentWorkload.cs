using System;
using System.Text;
using System.Threading.Tasks;
using Couchbase;
using Couchbase.Core;
using MeepMeep.Docs;

namespace MeepMeep.Workloads
{
    /// <summary>
    /// Workload representing use-case of getting documents by key.
    /// The key is randomly generated within the available range for
    /// each get operation.
    /// </summary>
    public class AddAndGetJsonDocumentWorkload : WorkloadBase
    {
        protected readonly string SampleDocument;

        public const string DefaultKeyGenerationPart = "agjdw";

        public override string Description { get; }

        public AddAndGetJsonDocumentWorkload(IWorkloadDocKeyGenerator docKeyGenerator, int workloadSize, int warmupMs, bool enableTiming, string sampleDocument = null)
            : base(docKeyGenerator, workloadSize, warmupMs, enableTiming)
        {
            SampleDocument = sampleDocument ?? SampleDocuments.Default;
            Description = string.Format("ExecuteStore (Add) and ExecuteGet by random key, {0} times.", WorkloadSize);
        }

        protected override async Task<WorkloadOperationResult> OnExecuteStep(IBucket bucket, int workloadIndex, int docIndex, Func<TimeSpan> getTiming)
        {
            var key = DocKeyGenerator.Generate(workloadIndex, docIndex);
            var randomKey = DocKeyGenerator.Generate(workloadIndex, docIndex);

            var results = await Task.WhenAll(
                bucket.UpsertAsync(key, SampleDocument), bucket.GetAsync<string>(randomKey)
            );

            return new WorkloadOperationResult(results[0].Success && results[1].Success, GetMessage(results[0], results[1]), getTiming())
            {
                DocSize = GetDocSize(results[1]) + SampleDocument.Length
            };
        }

        protected virtual string GetMessage(IOperationResult storeOpResult, IOperationResult getOpResult)
        {
            var sb = new StringBuilder();

            if (storeOpResult != null && !string.IsNullOrEmpty(storeOpResult.Message))
                sb.Append("StoreOp: ").Append(storeOpResult.Message).Append(". ");

            if (getOpResult != null && !string.IsNullOrEmpty(getOpResult.Message))
                sb.Append("GetOp: ").Append(getOpResult.Message).Append(". ");

            return sb.ToString().TrimEnd();
        }

        protected virtual int GetDocSize(IOperationResult<string> getOpResult)
        {
            return getOpResult != null && getOpResult.Value != null
                ? getOpResult.Value.ToString().Length
                : 0;
        }
    }
}
