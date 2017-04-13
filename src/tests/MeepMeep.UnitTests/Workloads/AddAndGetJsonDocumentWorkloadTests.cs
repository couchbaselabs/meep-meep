using System;
using System.Linq;
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
        public void When_executed_with_10_documents_It_will_try_and_get_10_docs()
        {
            SUT = CreateWorkload(10);

            SUT.Execute(Bucket, 0);

            A.CallTo(() => Bucket.Insert(A<string>.Ignored, A<string>.Ignored))
             .MustHaveHappened(Repeated.Exactly.Times(10));

            var keys = GenerateKeys(10).ToArray();
            foreach (var key in keys)
                A.CallTo(() => Bucket.Insert(key, SampleDocuments.Default))
                 .MustHaveHappened(Repeated.Exactly.Once);

            A.CallTo(() => Bucket.Get<string>(A<string>.That.Matches(k => keys.Contains(k))))
             .MustHaveHappened(Repeated.Exactly.Times(10));
        }

        [Test]
        public void When_executed_with_10_documents_It_will_generate_a_result_with_10_operation_results()
        {
            SUT = CreateWorkload(10);

            A.CallTo(() => Bucket.Insert(A<string>.Ignored, A<string>.Ignored))
             .Returns(new OperationResult<string>());

            var workloadResult = SUT.Execute(Bucket, 0);

            workloadResult.CountOperations().Should().Be(10);
        }

        [Test]
        public void When_exception_is_thrown_for_10_docs_by_the_client_It_will_generate_a_result_with_failed_operation_results()
        {
            SUT = CreateWorkload(10);

            A.CallTo(() => Bucket.Insert(A<string>.Ignored, A<string>.Ignored))
             .Throws(new Exception("Foo bar"));

            var workloadResult = SUT.Execute(Bucket, 0);

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