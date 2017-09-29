using System;
using System.Linq;
using System.Threading.Tasks;
using Couchbase;
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
        public async Task When_executed_with_10_documents_It_will_try_and_get_10_docs()
        {
            SUT = CreateWorkload(10);

            await SUT.Execute(Bucket, 0);

            A.CallTo(() => Bucket.UpsertAsync(A<string>.Ignored, A<string>.Ignored))
             .MustHaveHappened(Repeated.Exactly.Times(10));

            var keys = GenerateKeys(1).ToArray();
            foreach (var key in keys)
                A.CallTo(() => Bucket.UpsertAsync(key, SampleDocuments.Default))
                 .MustHaveHappened(Repeated.Exactly.Times(10));

            A.CallTo(() => Bucket.GetAsync<string>(A<string>.That.Matches(k => keys.Contains(k))))
             .MustHaveHappened(Repeated.Exactly.Times(10));
        }

        [Test]
        public async Task When_executed_with_10_documents_It_will_generate_a_result_with_10_operation_results()
        {
            SUT = CreateWorkload(10);

            A.CallTo(() => Bucket.UpsertAsync(A<string>.Ignored, A<string>.Ignored))
             .Returns(new OperationResult<string>());

            var workloadResult = await SUT.Execute(Bucket, 0);

            workloadResult.CountOperations().Should().Be(10);
        }

        [Test]
        public async Task When_exception_is_thrown_for_10_docs_by_the_client_It_will_generate_a_result_with_failed_operation_results()
        {
            SUT = CreateWorkload(10);

            A.CallTo(() => Bucket.UpsertAsync(A<string>.Ignored, A<string>.Ignored))
             .Throws(new Exception("Foo bar"));

            var workloadResult = await SUT.Execute(Bucket, 0);

            workloadResult.CountFailedOperations().Should().Be(10);
        }

        protected virtual AddAndGetJsonDocumentWorkload CreateWorkload(int numOfDocs, int warmupMs = 0)
        {
            return new AddAndGetJsonDocumentWorkload(GetKeyGenerator(), numOfDocs, warmupMs, false);
        }

        protected override IWorkloadDocKeyGenerator GetKeyGenerator()
        {
            return new DefaultWorkloadDocKeyGenerator("mydoc", AddAndGetJsonDocumentWorkload.DefaultKeyGenerationPart, 0);
        }
    }
}
