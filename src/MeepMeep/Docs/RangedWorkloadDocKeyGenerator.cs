using System;
using System.Collections.Generic;
using System.Linq;
using EnsureThat;

namespace MeepMeep.Docs
{
    public class RangedWorkloadDocKeyGenerator : IWorkloadDocKeyGenerator
    {
        private const string Seperator = ":";
        private static readonly Random Random = new Random();

        protected readonly string DocKeyPrefix;
        protected readonly string WorkloadKey;
        protected readonly int DocKeySeed;
        protected readonly int DocKeyRange;

        public RangedWorkloadDocKeyGenerator(string docKeyPrefix, string workloadKey, int docKeySeed, int docKeyRange)
        {
            Ensure.That(docKeyPrefix, "docKeyPrefix").IsNotNullOrWhiteSpace();
            Ensure.That(workloadKey, "workloadKey").IsNotNullOrWhiteSpace();
            Ensure.That(docKeySeed, "docKeySeed").IsGte(0);
            Ensure.That(docKeyRange, "docKeyRange").IsGte(0);

            DocKeyPrefix = docKeyPrefix;
            WorkloadKey = workloadKey;
            DocKeySeed = docKeySeed;
            DocKeyRange = docKeyRange;
        }

        public virtual string Generate(int workloadIndex, int docIndex)
        {
            return string.Join(Seperator, DocKeyPrefix, WorkloadKey, workloadIndex, Random.Next(DocKeySeed, DocKeyRange));
        }

        public IEnumerable<string> GenerateAllKeys(int workloadIndex, int docIndex)
        {
            return Enumerable
                .Range(DocKeySeed, DocKeyRange)
                .Select(x => string.Join(Seperator, DocKeyPrefix, WorkloadKey, workloadIndex, x));
        }
    }
}
