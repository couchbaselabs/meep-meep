using System;
using Couchbase.Core;
using MeepMeep.Docs;

namespace MeepMeep.Workloads
{
    public class MixedGetSetJsonDocumentWorkload : WorkloadBase
    {
        public const string DefaultKeyGenerationPart = "ajdw";

        private readonly string _description;
        private readonly double _mutationPercentage;
        protected readonly string SampleDocument;
        protected readonly Random Randomizer;

        public override string Description
        {
            get { return _description; }
        }
        
        public MixedGetSetJsonDocumentWorkload(IWorkloadDocKeyGenerator docKeyGenerator, int workloadSize, int warmupMs, double mutationPercentage, bool enableTiming, string sampleDocument = null)
            : base(docKeyGenerator, workloadSize, warmupMs, enableTiming)
        {
            Randomizer = new Random();
            SampleDocument = sampleDocument ?? SampleDocuments.Default;
            _mutationPercentage = mutationPercentage;
            _description = string.Format("Mix of Get and Set ({0}%) operations against JSON doc(s) with doc size: {1}.",
                _mutationPercentage,
                SampleDocument.Length);
        }

        protected override WorkloadOperationResult OnExecuteStep(IBucket bucket, int workloadIndex, int docIndex, Func<TimeSpan> getTiming)
        {
            var key = DocKeyGenerator.Generate(workloadIndex, docIndex);

            var storeOpResult = Randomizer.NextDouble() <= _mutationPercentage
                ? bucket.Upsert(key, SampleDocument)
                : bucket.Get<string>(key);

            return new WorkloadOperationResult(storeOpResult.Success, storeOpResult.Message, getTiming())
            {
                DocSize = SampleDocument.Length
            };
        }

        /// <summary>
        /// Not included in timing. Could be used to perform setup logic.
        /// </summary>
        /// <param name="bucket"></param>
        /// <param name="workloadIndex"></param>
        /// <param name="docIndex"></param>
        protected override void OnPreExecute(IBucket bucket, int workloadIndex, int docIndex)
        {
            foreach (var key in DocKeyGenerator.GenerateAllKeys(workloadIndex, docIndex))
            {
                bucket.Upsert(key, SampleDocument);
            }
        }
    }
}
