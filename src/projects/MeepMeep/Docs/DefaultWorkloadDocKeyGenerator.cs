using EnsureThat;

namespace MeepMeep.Docs
{
    public class DefaultWorkloadDocKeyGenerator : IWorkloadDocKeyGenerator
    {
        protected readonly string DocKeyPrefix;
        protected readonly string WorkloadKey;
        protected readonly int DocKeySeed;

        public DefaultWorkloadDocKeyGenerator(string docKeyPrefix, string workloadKey, int docKeySeed)
        {
            Ensure.That(docKeyPrefix, "docKeyPrefix").IsNotNullOrWhiteSpace();
            Ensure.That(workloadKey, "workloadKey").IsNotNullOrWhiteSpace();
            Ensure.That(docKeySeed, "docKeySeed").IsGte(0);

            DocKeyPrefix = docKeyPrefix;
            WorkloadKey = workloadKey;
            DocKeySeed = docKeySeed;
        }

        public virtual string Generate(int workloadIndex, int docIndex)
        {
            return string.Concat(DocKeyPrefix, ":", WorkloadKey, ":", workloadIndex, ":", DocKeySeed + docIndex + 1);
        }
    }
}