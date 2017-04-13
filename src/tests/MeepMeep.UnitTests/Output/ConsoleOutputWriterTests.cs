using System;
using ApprovalTests;
using ApprovalTests.Reporters;
using MeepMeep.Output;
using MeepMeep.Testing;
using MeepMeep.Testing.TestData;
using NUnit.Framework;

namespace MeepMeep.UnitTests.Output
{
    [Ignore("AppVeyor does not support ApprovalsTests")]
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class ConsoleOutputWriterTests : UnitTestsOf<ConsoleOutputWriter>
    {
        protected override void OnFixtureInitialize()
        {
            SUT = new ConsoleOutputWriter();
        }

        [Test]
        public void Can_write_workload_result()
        {
            var workloadResult = WorkloadResultTestData.GetWithFailedAndSucceededOperations();

            var output = ConsoleInterceptor.Intercept(() => SUT.Write(workloadResult));

            CustomApprovals.VerifyGitFriendly(output);
        }

        [Test]
        public void Can_write_exception()
        {
            var output = ConsoleInterceptor.Intercept(() => SUT.Write("Test of exception", new Exception("My simle exception")));

            Approvals.Verify(output);
        }

        [Test]
        public void Can_write_aggregate_exception_with_no_inner_exceptions()
        {
            var output = ConsoleInterceptor.Intercept(() => SUT.Write("Test of aggregate exception", new AggregateException("My simle exception")));

            Approvals.Verify(output);
        }

        [Test]
        public void Can_write_aggregate_exception_with_inner_exceptions()
        {
            var output = ConsoleInterceptor.Intercept(() =>
                SUT.Write("Test of aggregate exception with inner ex.", new AggregateException("My simle exception", new Exception("Ex one"), new Exception("Ex two"))));

            Approvals.Verify(output);
        }
    }
}