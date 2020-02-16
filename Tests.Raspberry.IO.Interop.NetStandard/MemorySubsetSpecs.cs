using System;
using System.Collections;
using System.Linq;
using NUnit.Framework;
using Raspberry.IO.Interop.NetStandard;
using Tests.Raspberry.IO.NetStandard;

// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming

namespace Tests.Raspberry.IO.Interop.MemorySubsetSpecs
{
    [TestFixture]
    public class If_the_user_tries_to_create_a_subset_that_exceeds_the_memory_boundaries : Spec {
        private ManagedMemory managedMemory;

        protected override void EstablishContext() {
            managedMemory = new ManagedMemory(10);
        }

        private static IEnumerable TestCases {
            get { 
                yield return new TestCaseData(11, 1);
                yield return new TestCaseData(0, 11);
                yield return new TestCaseData(-1, 10);
                yield return new TestCaseData(10, 1); 
            }
        }

        [Test,TestCaseSource(nameof(TestCases))]
        public void Should_it_throw_an_exception(int startOffset, int length)
        {
            Assert.Throws<ArgumentOutOfRangeException>(delegate { new MemorySubset(managedMemory, startOffset, length, false); });
        }

        protected override void Cleanup() {
            if (managedMemory != null) {
                managedMemory.Dispose();
                managedMemory = null;
            }
        }
    }

    [TestFixture]
    public class If_the_user_tries_to_copy_data_to_a_memory_subset_exceeding_the_memory_boundaries : Spec {
        private ManagedMemory managedMemory;
        private MemorySubset subset;

        protected override void EstablishContext() {
            managedMemory = new ManagedMemory(10);
            subset = new MemorySubset(managedMemory, 2, 4, true);
        }

        private static IEnumerable TestCases {
            get {
                yield return new TestCaseData(4, 1, true);
                yield return new TestCaseData(0, 5, true);
                yield return new TestCaseData(-1, 2, true);
            }
        }

        [Test, TestCaseSource(nameof(TestCases))]
        public void Should_it_throw_an_exception(int startOffset, int length, bool shouldThrowArgumentOutOfRangeException) {
            var data = new byte[length];
            Assert.Throws<ArgumentOutOfRangeException>(delegate { subset.Copy(data, 0, startOffset, length); });
        }

        protected override void Cleanup() {
            if (subset != null) {
                subset.Dispose();
                subset = null;
            }
        }
    }

    [TestFixture]
    public class If_the_user_copies_data_to_a_memory_subset : Spec {
        private ManagedMemory managedMemory;
        private MemorySubset subset;

        protected override void EstablishContext() {
            managedMemory = new ManagedMemory(10);
            subset = new MemorySubset(managedMemory, 2, 4, true);
        }

        protected override void BecauseOf() {
            var data = new byte[] {0x1, 0x2, 0x3, 0x4};
            subset.Copy(data, 0, 0, data.Length);
        }

        [Test]
        public void Should_the_origin_memory_contain_the_correct_data()
        {
            var expected = new byte[] {0x0, 0x0, 0x1, 0x2, 0x3, 0x4, 0x0, 0x0, 0x0, 0x0};
            var memo = managedMemory.ToArray();

            for (var i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], memo[i]);
            }
        }

        protected override void Cleanup() {
            if (subset != null) {
                subset.Dispose();
                subset = null;
            }
        }
    }

    [TestFixture]
    public class If_the_user_writes_data_to_a_memory_subset : Spec
    {
        private static IEnumerable TestCases {
            get {
                yield return new TestCaseData(-1, 0x1, null);

                yield return new TestCaseData(0, 0x1, new byte[] {0x0, 0x0, 0x1, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0});

                yield return new TestCaseData(1, 0x1, new byte[] {0x0, 0x0, 0x0, 0x1, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0});

                yield return new TestCaseData(2, 0x1, new byte[] {0x0, 0x0, 0x0, 0x0, 0x1, 0x0, 0x0, 0x0, 0x0, 0x0});

                yield return new TestCaseData(3, 0x1, new byte[] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x1, 0x0, 0x0, 0x0, 0x0 });

                yield return new TestCaseData(4, 0x1, null);
            }
        }

        [Test,TestCaseSource(nameof(TestCases))]
        public void Should_the_origin_memory_contain_the_correct_data_when_using_the_WRITE_method(int startOffset, int value, byte[] expectedResult) {
            using (var managedMemory = new ManagedMemory(10)) {
                using (var subset = new MemorySubset(managedMemory, 2, 4, false)) {
                    if (expectedResult == null)
                    {
                        Assert.Throws<ArgumentOutOfRangeException>(delegate { subset.Write(startOffset, (byte)value); });
                    }
                    else
                    {
                        subset.Write(startOffset, (byte) value);
                        var memo = managedMemory.ToArray();
                        for (var i = 0; i < expectedResult.Length; i++)
                        {
                            Assert.AreEqual(expectedResult[i], memo[i]);
                        }
                    }
                }
            }
        }

        [Test, TestCaseSource(nameof(TestCases))]
        public void Should_the_origin_memory_contain_the_correct_data_when_using_the_INDEXER_method(int startOffset, int value, byte[] expectedResult) {
            using (var managedMemory = new ManagedMemory(10)) {
                using (var subset = new MemorySubset(managedMemory, 2, 4, false)) {
                    if (expectedResult == null)
                    {
                        Assert.Throws<ArgumentOutOfRangeException>(delegate { var result = subset[startOffset] = (byte)value; });
                    }
                    else
                    {
                        subset[startOffset] = (byte)value;
                        var memo = managedMemory.ToArray();
                        for (var i = 0; i < expectedResult.Length; i++)
                        {
                            Assert.AreEqual(expectedResult[i], memo[i]);
                        }
                    }
                }
            }
        }
    }

    [TestFixture]
    public class If_the_user_reads_data_from_a_memory_subset : Spec {
        private ManagedMemory managedMemory;
        private readonly byte[] content = {0x0, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9};
        private MemorySubset subset;

        protected override void EstablishContext() {
            managedMemory = new ManagedMemory(10);
            managedMemory.Copy(content, 0, 0, 10);
            subset = new MemorySubset(managedMemory, 2, 4, true);
        }

        private static IEnumerable TestCases {
            get {
                yield return new TestCaseData(-1, (byte)0xF);
                yield return new TestCaseData(0, (byte)0x2);
                yield return new TestCaseData(1, (byte)0x3);
                yield return new TestCaseData(2, (byte)0x4);
                yield return new TestCaseData(3, (byte)0x5);
                yield return new TestCaseData(4, (byte)0xF);
            }
        }

        [Test,TestCaseSource(nameof(TestCases))]
        public void Should_the_result_be_correct_when_using_the_READ_method(int startOffset, byte expected) {
            if (expected == 0xF)
            {
                Assert.Throws<ArgumentOutOfRangeException>(delegate  { expected = subset[startOffset]; });
            }
            else
            {
                Assert.AreEqual(expected, subset[startOffset]);
            }
        }

        [Test, TestCaseSource(nameof(TestCases))]
        public void Should_the_result_be_correct_when_using_the_INDEXER_method(int startOffset, byte expected) {
            if (expected == 0xF)
            {
                Assert.Throws<ArgumentOutOfRangeException>(delegate { expected = subset[startOffset]; });
            }
            else
            {
                Assert.AreEqual(expected, subset[startOffset]);
            }
        }

        protected override void Cleanup() {
            if (subset != null) {
                subset.Dispose();
                subset = null;
            }
        }
    }
}