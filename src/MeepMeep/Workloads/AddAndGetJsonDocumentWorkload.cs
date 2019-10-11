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

        public AddAndGetJsonDocumentWorkload(IWorkloadDocKeyGenerator docKeyGenerator, int workloadSize, int warmupMs, bool enableTiming, bool useSync, int rateLimit, string sampleDocument = null)
            : base(docKeyGenerator, workloadSize, warmupMs, enableTiming, useSync, rateLimit)
        {
            SampleDocument = sampleDocument ?? SampleDocuments.Default;
            Description = string.Format("ExecuteStore (Add) and ExecuteGet by random key, {0} times.", WorkloadSize);
        }

        protected override Task<WorkloadOperationResult> OnExecuteStep(IBucket bucket, int workloadIndex, int docIndex, Func<TimeSpan> getTiming)
        {
            var key = DocKeyGenerator.Generate(workloadIndex, docIndex);
            var randomKey = DocKeyGenerator.Generate(workloadIndex, docIndex);

            if (UseSync)
            {
                var upsertResult = bucket.Upsert(key, SampleDocument);
                var getResult = bucket.Get<string>(randomKey);

                return Task.FromResult(
                    new WorkloadOperationResult(upsertResult.Success && getResult.Success, GetMessage(upsertResult, getResult), getTiming())
                );
            }

            return Task.WhenAll(
                    bucket.UpsertAsync(key, SampleDocument), bucket.GetAsync<string>(randomKey)
                )
                .ContinueWith(tasks => new WorkloadOperationResult(tasks.Result[0].Success && tasks.Result[1].Success,
                    GetMessage(tasks.Result[0], tasks.Result[1]),
                    getTiming())
                {
                    DocSize = GetDocSize(tasks.Result[1]) + SampleDocument.Length
                });
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
