using System;
using System.Linq;
using System.Threading.Tasks;
using Couchbase.Core;
using MeepMeep.Docs;

namespace MeepMeep.Workloads
{
    /// <summary>
    /// Workload represetning a mix of get and set operations
    /// using a <see cref="_mutationPercentage"/>.
    /// </summary>
    public class MixedGetSetJsonDocumentWorkload : WorkloadBase
    {
        public const string DefaultKeyGenerationPart = "ajdw";

        private readonly double _mutationPercentage;
        protected readonly string SampleDocument;
        protected readonly Random Randomizer;

        public override string Description { get; }

        public MixedGetSetJsonDocumentWorkload(IWorkloadDocKeyGenerator docKeyGenerator, int workloadSize, int warmupMs, double mutationPercentage, bool enableTiming, bool useSync, string sampleDocument = null)
            : base(docKeyGenerator, workloadSize, warmupMs, enableTiming, useSync)
        {
            Randomizer = new Random();
            SampleDocument = sampleDocument ?? SampleDocuments.Default;
            _mutationPercentage = mutationPercentage;
            Description = string.Format("Mix of Get and Set ({0}%) operations against JSON doc(s) with doc size: {1}.",
                _mutationPercentage,
                SampleDocument.Length);
        }

        protected override Task<WorkloadOperationResult> OnExecuteStep(IBucket bucket, int workloadIndex, int docIndex, Func<TimeSpan> getTiming)
        {
            var key = DocKeyGenerator.Generate(workloadIndex, docIndex);

            if (UseSync)
            {
                var result = Randomizer.NextDouble() <= _mutationPercentage
                    ? bucket.Upsert(key, SampleDocument)
                    : bucket.Get<string>(key);

                return Task.FromResult(
                    new WorkloadOperationResult(result.Success, result.Message, getTiming())
                );
            }

            return (Randomizer.NextDouble() <= _mutationPercentage
                    ? bucket.UpsertAsync(key, SampleDocument)
                    : bucket.GetAsync<string>(key))
                .ContinueWith(
                    task => new WorkloadOperationResult(task.Result.Success, task.Result.Message, getTiming())
                );
        }

        /// <summary>
        /// Not included in timing. Could be used to perform setup logic.
        /// </summary>
        /// <param name="bucket"></param>
        /// <param name="workloadIndex"></param>
        /// <param name="docIndex"></param>
        protected override Task OnPreExecute(IBucket bucket, int workloadIndex, int docIndex)
        {
            var keys = DocKeyGenerator.GenerateAllKeys(workloadIndex, docIndex);
            return Task.WhenAll(
                keys.Select(key => bucket.UpsertAsync(key, SampleDocument))
            );
        }
    }
}
