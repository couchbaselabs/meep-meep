using System;
using System.Diagnostics;
using System.Text;
using Couchbase;
using Enyim.Caching.Memcached;
using Enyim.Caching.Memcached.Results;
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

        protected readonly Random Randomizer;

        public const string DefaultKeyGenerationPart = "agjdw";

        public override string Description
        {
            get { return _description; }
        }

        public AddAndGetJsonDocumentWorkload(IWorkloadDocKeyGenerator docKeyGenerator, int workloadSize, int warmupMs, string sampleDocument = null)
            : base(docKeyGenerator, workloadSize, warmupMs)
        {
            Randomizer = new Random();
            SampleDocument = sampleDocument ?? SampleDocuments.Default;
            _description = string.Format("ExecuteStore (Add) and ExecuteGet by random key, {0} times.", WorkloadSize);
        }

        protected override WorkloadOperationResult OnExecuteStep(ICouchbaseClient client, int workloadIndex, int docIndex, Stopwatch sw)
        {
            var key = DocKeyGenerator.Generate(workloadIndex, docIndex);
            var randomKey = DocKeyGenerator.Generate(workloadIndex, GetRandomDocIndex(0, docIndex));

            sw.Start();
            //saakshi
            //var storeOpResult = client.ExecuteStore(StoreMode.Add, key, SampleDocument);
            var storeOpResult = client.ExecuteStore(StoreMode.Set, key, SampleDocument);
            var getOpResult = client.ExecuteGet(randomKey);
            sw.Stop();

            return new WorkloadOperationResult(storeOpResult.Success && getOpResult.Success, GetMessage(storeOpResult, getOpResult), sw.Elapsed)
            {
                DocSize = GetDocSize(getOpResult) + SampleDocument.Length
            };
        }

        protected virtual string GetMessage(IStoreOperationResult storeOpResult, IGetOperationResult getOpResult)
        {
            var sb = new StringBuilder();

            if (storeOpResult != null && !string.IsNullOrEmpty(storeOpResult.Message))
                sb.Append("StoreOp: ").Append(storeOpResult.Message).Append(". ");

            if (getOpResult != null && !string.IsNullOrEmpty(getOpResult.Message))
                sb.Append("GetOp: ").Append(getOpResult.Message).Append(". ");

            return sb.ToString().TrimEnd();
        }

        protected virtual int GetDocSize(IGetOperationResult getOpResult)
        {
            return getOpResult != null && getOpResult.Value != null
                ? getOpResult.Value.ToString().Length
                : 0;
        }

        protected virtual int GetRandomDocIndex(int min, int max)
        {
            return Randomizer.Next(min, max);
        }
    }
}