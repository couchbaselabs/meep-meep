using System;
using System.Diagnostics;
using System.Text;
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
        private readonly string _description;

        protected readonly string SampleDocument;

        public const string DefaultKeyGenerationPart = "agjdw";

        public override string Description
        {
            get { return _description; }
        }

        public AddAndGetJsonDocumentWorkload(IWorkloadDocKeyGenerator docKeyGenerator, int workloadSize, int warmupMs, string sampleDocument = null)
            : base(docKeyGenerator, workloadSize, warmupMs)
        {
            SampleDocument = sampleDocument ?? SampleDocuments.Default;
            _description = string.Format("ExecuteStore (Add) and ExecuteGet by random key, {0} times.", WorkloadSize);
        }

        protected override WorkloadOperationResult OnExecuteStep(IBucket bucket, int workloadIndex, int docIndex, Stopwatch sw)
        {
            var key = DocKeyGenerator.Generate(workloadIndex, docIndex);
            var randomKey = DocKeyGenerator.Generate(workloadIndex, docIndex);

            sw.Start();
            var storeOpResult = bucket.Upsert(key, SampleDocument);
            var getOpResult = bucket.Get<string>(randomKey);
            sw.Stop();

            return new WorkloadOperationResult(storeOpResult.Success && getOpResult.Success, GetMessage(storeOpResult, getOpResult), sw.Elapsed)
            {
                DocSize = GetDocSize(getOpResult) + SampleDocument.Length
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