using System;
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
    public class AddJsonDocumentWorkloadTests : DocumentWorkloadTestsOf<AddJsonDocumentWorkload>
    {
        [Test]
        public void When_no_warmup_and_executed_with_10_documents_It_will_try_and_insert_10_docs()
        {
            SUT = CreateWorkload(numOfDocs: 10, warmupMs: 0);

            SUT.Execute(FakeClient, 0);

            A.CallTo(() => FakeClient.ExecuteStore(StoreMode.Add, A<string>.Ignored, A<string>.Ignored))
             .MustHaveHappened(Repeated.Exactly.Times(10));

            foreach (var key in GenerateKeys(10))
                A.CallTo(() => FakeClient.ExecuteStore(StoreMode.Add, key, SampleDocuments.Default))
                 .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void When_no_warmup_and_executed_with_10_documents_It_will_generate_a_result_with_10_operation_results()
        {
            SUT = CreateWorkload(numOfDocs: 10, warmupMs: 0);

            A.CallTo(() => FakeClient.ExecuteStore(StoreMode.Add, A<string>.Ignored, A<string>.Ignored))
             .Returns(new StoreOperationResult());

            var workloadResult = SUT.Execute(FakeClient, 0);

            workloadResult.CountOperations().Should().Be(10);
        }

        [Test]
        public void When_no_warmup_and_exception_is_thrown_for_10_docs_by_the_client_It_will_generate_a_result_with_failed_operation_results()
        {
            SUT = CreateWorkload(numOfDocs: 10, warmupMs: 0);

            A.CallTo(() => FakeClient.ExecuteStore(StoreMode.Add, A<string>.Ignored, A<string>.Ignored))
             .Throws(new Exception("Foo bar"));

            var workloadResult = SUT.Execute(FakeClient, 0);

            workloadResult.CountFailedOperations().Should().Be(10);
        }

        protected virtual AddJsonDocumentWorkload CreateWorkload(int numOfDocs, int warmupMs)
        {
            return new AddJsonDocumentWorkload(GetKeyGenerator(), numOfDocs, warmupMs);
        }

        protected override IWorkloadDocKeyGenerator GetKeyGenerator()
        {
            return new DefaultWorkloadDocKeyGenerator("mydoc", AddJsonDocumentWorkload.DefaultKeyGenerationPart, 0);
        }
    }
}