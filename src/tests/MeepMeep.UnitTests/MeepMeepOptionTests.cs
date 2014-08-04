using System;
using System.Linq;
using ApprovalTests;
using ApprovalTests.Reporters;
using FluentAssertions;
using MeepMeep.Testing;
using NUnit.Framework;
using MeepMeep.Extensions;

namespace MeepMeep.UnitTests
{
    [TestFixture]
    [UseReporter(typeof(DiffReporter))]
    public class MeepMeepOptionTests : UnitTestsOf<MeepMeepOptions>
    {
        protected override void OnTestInitialize()
        {
            SUT = new MeepMeepOptions
            {
                Nodes = new []{"http://cb1.local:8091/pools", "http://cb2.local:8091/pools"},
                Bucket = "mybucket",
                BucketPassword = "mybucketpass",
                ClusterUsername = "myclusteruser",
                ClusterPassword = "myclusterpass",
                DocKeyPrefix = "mydoc",
                DocKeySeed = 3,
                DocSamplePath = "c:\\foo\bar\\in_the_house.json",
                WorkloadSize = 42,
                NumOfClients = 13,
                MaximumConcurrencyLevel = 1,
                ThreadPoolMaxNumOfThreads = 1000,
                ThreadPoolMinNumOfThreads = 500,
                WarmupMs = 4242,
                Verbose = true
            };
        }

        [Test]
        public void When_generating_help_text_It_should_return_a_proper_help_screen()
        {
            CustomApprovals.VerifyGitFriendly(SUT.GetHelp());
        }

        [Test]
        public void When_evaluating_options_as_string_It_should_output_key_values_of_the_options()
        {
            Approvals.Verify(SUT);
        }

        [Test]
        public void Can_map_to_couchbase_config()
        {
            var clientConfig = SUT.ToClientConfig();

            clientConfig.Bucket.Should().Be(SUT.Bucket);
            clientConfig.BucketPassword.Should().Be(SUT.BucketPassword);
            clientConfig.Username.Should().Be(SUT.ClusterUsername);
            clientConfig.Password.Should().Be(SUT.ClusterPassword);
            clientConfig.Urls.Should().BeEquivalentTo(SUT.Nodes.Select(n => new Uri(n)));
        }
    }
}