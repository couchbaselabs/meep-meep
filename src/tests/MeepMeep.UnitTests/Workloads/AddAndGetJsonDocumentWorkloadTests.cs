using System;
using System.Linq;
using Enyim.Caching.Memcached;
using Enyim.Caching.Memcached.Results;
using FakeItEasy;
using FluentAssertions;
using MeepMeep.Docs;
using MeepMeep.Workloads;
using NUnit.Framework;

namespace MeepMeep.UnitTests.Workloads
{
    [TestFixture]
    public class AddAndGetJsonDocumentWorkloadTests : DocumentWorkloadTestsOf<AddAndGetJsonDocumentWorkload>
    {
        [Test]
        public void When_executed_with_10_documents_It_will_try_and_get_10_docs()
        {
            SUT = CreateWorkload(10);

            SUT.Execute(FakeClient, 0);

            A.CallTo(() => FakeClient.ExecuteStore(StoreMode.Add, A<string>.Ignored, A<string>.Ignored))
             .MustHaveHappened(Repeated.Exactly.Times(10));

            var keys = GenerateKeys(10).ToArray();
            foreach (var key in keys)
                A.CallTo(() => FakeClient.ExecuteStore(StoreMode.Add, key, SampleDocuments.Default))
                 .MustHaveHappened(Repeated.Exactly.Once);

            A.CallTo(() => FakeClient.ExecuteGet(A<string>.That.Matches(k => keys.Contains(k))))
             .MustHaveHappened(Repeated.Exactly.Times(10));
        }

        [Test]
        public void When_executed_with_10_documents_It_will_generate_a_result_with_10_operation_results()
        {
            SUT = CreateWorkload(10);

            A.CallTo(() => FakeClient.ExecuteStore(StoreMode.Add, A<string>.Ignored, A<string>.Ignored))
             .Returns(new StoreOperationResult());

            var workloadResult = SUT.Execute(FakeClient, 0);

            workloadResult.CountOperations().Should().Be(10);
        }

        [Test]
        public void When_exception_is_thrown_for_10_docs_by_the_client_It_will_generate_a_result_with_failed_operation_results()
        {
            SUT = CreateWorkload(10);

            A.CallTo(() => FakeClient.ExecuteStore(StoreMode.Add, A<string>.Ignored, A<string>.Ignored))
             .Throws(new Exception("Foo bar"));

            var workloadResult = SUT.Execute(FakeClient, 0);

            workloadResult.CountFailedOperations().Should().Be(10);
        }

        protected virtual AddAndGetJsonDocumentWorkload CreateWorkload(int numOfDocs, int warmupMs = 0)
        {
            return new AddAndGetJsonDocumentWorkload(GetKeyGenerator(), numOfDocs, warmupMs);
        }

        protected override IWorkloadDocKeyGenerator GetKeyGenerator()
        {
            return new DefaultWorkloadDocKeyGenerator("mydoc", AddAndGetJsonDocumentWorkload.DefaultKeyGenerationPart, 0);
        }
    }
}