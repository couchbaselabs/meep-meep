using System.Collections.Generic;
using Couchbase.Core;
using FakeItEasy;
using MeepMeep.Docs;
using NUnit.Framework;

namespace MeepMeep.UnitTests.Workloads
{
    [TestFixture]
    public abstract class DocumentWorkloadTestsOf<T> : UnitTestsOf<T> where T : class
    {
        protected IBucket Bucket;

        protected abstract IWorkloadDocKeyGenerator GetKeyGenerator();

        protected override void OnTestInitialize()
        {
            Bucket = A.Fake<IBucket>();
        }

        protected virtual IEnumerable<string> GenerateKeys(int numOfKeys)
        {
            for (var i = 0; i < numOfKeys; i++)
                yield return GetKeyGenerator().Generate(0, i);
        }
    }
}