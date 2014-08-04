using FluentAssertions;
using MeepMeep.Docs;
using NUnit.Framework;

namespace MeepMeep.UnitTests.Docs
{
    [TestFixture]
    public class WorkloadDocKeyGeneratorTests : UnitTestsOf<DefaultWorkloadDocKeyGenerator>
    {
        [Test]
        public void Can_generate_key_for_seed_0_and_workindex_0_docindex_0()
        {
            SUT = CreateDocKeyGenerator(0);

            SUT.Generate(0, 0).Should().Be("MyPrefix:MyWorkload:0:1");
        }

        [Test]
        public void Can_generate_key_for_seed_0_and_workindex_0_docindex_1()
        {
            SUT = CreateDocKeyGenerator(0);

            SUT.Generate(0, 1).Should().Be("MyPrefix:MyWorkload:0:2");
        }

        [Test]
        public void Can_generate_key_for_seed_0_and_workindex_1_docindex_0()
        {
            SUT = CreateDocKeyGenerator(0);

            SUT.Generate(1, 0).Should().Be("MyPrefix:MyWorkload:1:1");
        }

        [Test]
        public void Can_generate_key_for_seed_0_and_workindex_1_docindex_1()
        {
            SUT = CreateDocKeyGenerator(0);

            SUT.Generate(1, 1).Should().Be("MyPrefix:MyWorkload:1:2");
        }

        [Test]
        public void Can_generate_key_for_seed_1_and_workindex_0_docindex_0()
        {
            SUT = CreateDocKeyGenerator(1);

            SUT.Generate(0, 0).Should().Be("MyPrefix:MyWorkload:0:2");
        }

        [Test]
        public void Can_generate_key_for_seed_1_and_workindex_0_docindex_1()
        {
            SUT = CreateDocKeyGenerator(1);

            SUT.Generate(0, 1).Should().Be("MyPrefix:MyWorkload:0:3");
        }

        [Test]
        public void Can_generate_key_for_seed_1_and_workindex_1_docindex_0()
        {
            SUT = CreateDocKeyGenerator(1);

            SUT.Generate(1, 0).Should().Be("MyPrefix:MyWorkload:1:2");
        }

        [Test]
        public void Can_generate_key_for_seed_1_and_workindex_1_docindex_1()
        {
            SUT = CreateDocKeyGenerator(1);

            SUT.Generate(1, 1).Should().Be("MyPrefix:MyWorkload:1:3");
        }

        protected virtual DefaultWorkloadDocKeyGenerator CreateDocKeyGenerator(int docKeySeed)
        {
            return new DefaultWorkloadDocKeyGenerator("MyPrefix", "MyWorkload", docKeySeed);
        }
    }
}
