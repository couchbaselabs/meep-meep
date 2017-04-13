using FluentAssertions;
using MeepMeep.Input;
using NUnit.Framework;

namespace MeepMeep.UnitTests.Input
{
    [TestFixture]
    public class CommandLineParserTests : UnitTestsOf<CommandLineParser>
    {
        protected override void OnFixtureInitialize()
        {
            SUT = new CommandLineParser();
        }

        [Test]
        public void When_parsing_without_args_It_should_have_sensible_defaults()
        {
            var options = new MeepMeepOptions();

            SUT.Parse(options).Should().Be(true);

            options.Should().NotBeNull();
            options.Nodes.Should().BeEquivalentTo(new object[] { "http://127.0.0.1:8091/pools" });
            options.Bucket.Should().Be("default");
            options.BucketPassword.Should().Be(string.Empty);
            options.ClusterUsername.Should().Be(string.Empty);
            options.ClusterPassword.Should().Be(string.Empty);
            options.NumOfClients.Should().Be(1);
            options.WorkloadSize.Should().Be(20000);
            options.DocSamplePath.Should().BeEmpty();
            options.DocKeyPrefix.Should().Be("mm");
            options.DocKeySeed.Should().Be(0);
            options.MaximumConcurrencyLevel.Should().Be(null);
            options.ThreadPoolMinNumOfThreads.Should().Be(null);
            options.ThreadPoolMaxNumOfThreads.Should().Be(null);
            options.WarmupMs.Should().Be(100);
            options.Verbose.Should().Be(false);
            options.MutationPercentage.Should().Be(0.33);
            options.WorkloadType.Should().Be(WorkloadType.MutationPercentage);
            options.EnableOperationTiming.Should().Be(false);
        }
    }
}