using System;
using System.Diagnostics;
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
        
        public MixedGetSetJsonDocumentWorkload(IWorkloadDocKeyGenerator docKeyGenerator, int workloadSize, int warmupMs, double mutationPercentage, string sampleDocument = null)
            : base(docKeyGenerator, workloadSize, warmupMs)
        {
            Randomizer = new Random();
            SampleDocument = sampleDocument ?? SampleDocuments.Default;
            _mutationPercentage = mutationPercentage;
            _description = string.Format("Mix of Get and Set ({0}%) operations against JSON doc(s) with doc size: {1}.",
                _mutationPercentage,
                SampleDocument.Length);
        }

        protected override WorkloadOperationResult OnExecuteStep(IBucket bucket, int workloadIndex, int docIndex, Stopwatch sw)
        {
            var key = DocKeyGenerator.Generate(workloadIndex, docIndex);

            sw.Start();
            var storeOpResult = Randomizer.NextDouble() <= _mutationPercentage
                ? bucket.Upsert(key, SampleDocument)
                : bucket.Get<string>(key);
            sw.Stop();

            return new WorkloadOperationResult(storeOpResult.Success, storeOpResult.Message, sw.Elapsed)
            {
                DocSize = SampleDocument.Length
            };
        }
    }
}
