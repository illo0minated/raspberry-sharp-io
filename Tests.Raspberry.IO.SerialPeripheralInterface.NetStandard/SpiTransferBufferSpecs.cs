using System;
using NUnit.Framework;
using Raspberry.IO.SerialPeripheralInterface.NetStandard;
using Raspberry.IO.SerialPeripheralInterface.NetStandard.EnumTypes;
using Tests.Raspberry.IO.NetStandard;

// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Tests.Raspberry.IO.SerialPeripheralInterface.SpiTransferBufferSpecs
{
    [TestFixture]
    public class If_the_user_creates_an_spi_transfer_buffer_for_transmission_only : Spec {
        private const int REQUESTED_SIZE = 500;
        private ISpiTransferBuffer buffer;

        protected override void BecauseOf() {
            buffer = new SpiTransferBuffer(REQUESTED_SIZE, SpiTransferMode.Write);
        }

        [Test]
        public void Should_the_structure_contain_a_memory_buffer_for_transmission_data() {
            Assert.IsNotNull(buffer.Tx);
        }

        [Test]
        public void Should_the_structure_contain_no_memory_buffer_to_receive_data() {
            Assert.IsNull(buffer.Rx);
        }

        [Test]
        public void Should_the_buffer_size_be_equal_to_the_requested_size() {
            Assert.AreEqual(buffer.Length, REQUESTED_SIZE);
        }

        [Test]
        public void Should_the_transmission_data_buffer_size_be_equal_to_the_requested_size() {
            Assert.AreEqual(buffer.Tx.Length, REQUESTED_SIZE);
        }

        [Test]
        public void Should_the_transfer_mode_be_write_only() {
            Assert.AreEqual(buffer.TransferMode, SpiTransferMode.Write);
        }

        [Test]
        public void Should_the_transfer_structure_have_the_correct_memory_address_for_TX() {
            Assert.AreEqual(buffer.ControlStructure.Tx, unchecked((UInt64) buffer.Tx.Pointer.ToInt64()));
        }

        [Test]
        public void Should_the_transfer_structure_have_the_correct_memory_address_for_RX() {
            Assert.AreEqual(buffer.ControlStructure.Rx, 0L);
        }

        [Test]
        public void Should_the_transfer_structure_have_the_correct_memory_length() {
            Assert.AreEqual(buffer.ControlStructure.Length, REQUESTED_SIZE);
        }

        protected override void Cleanup() {
            if (!ReferenceEquals(buffer, null)) {
                buffer.Dispose();
                buffer = null;
            }
        }
    }

    [TestFixture]
    public class If_the_user_creates_an_spi_transfer_buffer_for_receive_only : Spec
    {
        private const int REQUESTED_SIZE = 500;
        private ISpiTransferBuffer buffer;

        protected override void BecauseOf() {
            buffer = new SpiTransferBuffer(REQUESTED_SIZE, SpiTransferMode.Read);
        }

        [Test]
        public void Should_the_structure_contain_no_memory_buffer_for_transmission_data() {
            Assert.IsNull(buffer.Tx);
        }

        [Test]
        public void Should_the_structure_contain_a_memory_buffer_to_receive_data() {
            Assert.IsNotNull(buffer.Rx);
        }

        [Test]
        public void Should_the_buffer_size_be_equal_to_the_requested_size() {
            Assert.AreEqual(buffer.Length, REQUESTED_SIZE);
        }

        [Test]
        public void Should_the_receive_data_buffer_size_be_equal_to_the_requested_size() {
            Assert.AreEqual(buffer.Rx.Length, REQUESTED_SIZE);
        }

        [Test]
        public void Should_the_transfer_mode_be_read_only() {
            Assert.AreEqual(buffer.TransferMode, SpiTransferMode.Read);
        }

        [Test]
        public void Should_the_transfer_structure_have_the_correct_memory_address_for_RX() {
            Assert.AreEqual(buffer.ControlStructure.Rx, unchecked((UInt64)buffer.Rx.Pointer.ToInt64()));
        }

        [Test]
        public void Should_the_transfer_structure_have_the_correct_memory_address_for_TX() {
            Assert.AreEqual(buffer.ControlStructure.Tx, 0L);
        }

        [Test]
        public void Should_the_transfer_structure_have_the_correct_memory_length() {
            Assert.AreEqual(buffer.ControlStructure.Length, REQUESTED_SIZE);
        }

        protected override void Cleanup() {
            if (!ReferenceEquals(buffer, null)) {
                buffer.Dispose();
                buffer = null;
            }
        }
    }

    [TestFixture]
    public class If_the_user_creates_an_spi_transfer_buffer_for_transmission_and_receive : Spec
    {
        private const int REQUESTED_SIZE = 500;
        private ISpiTransferBuffer buffer;

        protected override void BecauseOf() {
            buffer = new SpiTransferBuffer(REQUESTED_SIZE, SpiTransferMode.ReadWrite);
        }

        [Test]
        public void Should_the_structure_contain_a_memory_buffer_for_transmission_data() {
            Assert.IsNull(buffer.Tx);
        }

        [Test]
        public void Should_the_structure_contain_a_memory_buffer_to_receive_data() {
            Assert.IsNotNull(buffer.Rx);
        }

        [Test]
        public void Should_the_buffer_size_be_equal_to_the_requested_size() {
            Assert.AreEqual(buffer.Length, REQUESTED_SIZE);
        }

        [Test]
        public void Should_the_transmission_data_buffer_size_be_equal_to_the_requested_size() {
            Assert.AreEqual(buffer.Tx.Length, REQUESTED_SIZE);
        }

        [Test]
        public void Should_the_receive_data_buffer_size_be_equal_to_the_requested_size() {
            Assert.AreEqual(buffer.Rx.Length, REQUESTED_SIZE);
        }

        [Test]
        public void Should_the_data_buffers_not_have_the_same_memory_address() {
            Assert.AreNotEqual(buffer.Rx.Pointer, buffer.Tx.Pointer);
        }

        [Test]
        public void Should_the_transfer_mode_have_set_the_READ_flag() {
            Assert.IsTrue(buffer.TransferMode.HasFlag(SpiTransferMode.Read));
        }

        [Test]
        public void Should_the_transfer_mode_have_set_the_WRITE_flag() {
            Assert.IsTrue(buffer.TransferMode.HasFlag(SpiTransferMode.Write));
        }


        [Test]
        public void Should_the_transfer_structure_have_the_correct_memory_address_for_RX() {
            Assert.AreEqual(buffer.ControlStructure.Rx, unchecked((UInt64)buffer.Rx.Pointer.ToInt64()));
        }

        [Test]
        public void Should_the_transfer_structure_have_the_correct_memory_address_for_TX() {
            Assert.AreEqual(buffer.ControlStructure.Tx, unchecked((UInt64)buffer.Tx.Pointer.ToInt64()));
        }

        [Test]
        public void Should_the_transfer_structure_have_the_correct_memory_length() {
            Assert.AreEqual(buffer.ControlStructure.Length, REQUESTED_SIZE);
        }

        protected override void Cleanup() {
            if (!ReferenceEquals(buffer, null)) {
                buffer.Dispose();
                buffer = null;
            }
        }
    }

    [TestFixture]
    public class If_the_user_changes_various_transfer_settings : Spec {
        private const int REQUESTED_SIZE = 100;
        private const int REQUESTED_BITS_PER_WORD = 16;
        private const bool REQUESTED_CHIP_SELECT_CHANGE = true;
        private const int REQUESTED_DELAY_IN_USEC = 100;
        private const int REQUESTED_SPEED_IN_HZ = 1000000;
        private SpiTransferBuffer buffer;

        protected override void EstablishContext() {
            buffer = new SpiTransferBuffer(REQUESTED_SIZE, SpiTransferMode.Write);
        }

        protected override void BecauseOf() {
            buffer.BitsPerWord = REQUESTED_BITS_PER_WORD;
            buffer.ChipSelectChange = REQUESTED_CHIP_SELECT_CHANGE;
            buffer.Delay = REQUESTED_DELAY_IN_USEC;
            buffer.Speed = REQUESTED_SPEED_IN_HZ;
        }

        [Test]
        public void Should_the_control_structure_have_the_requested_wordsize() {
            Assert.AreEqual(buffer.ControlStructure.BitsPerWord, REQUESTED_BITS_PER_WORD);
        }

        [Test]
        public void Should_the_control_structure_have_the_requested_chip_select_change_value() {
            // ReSharper disable once UnreachableCode
            Assert.AreEqual(buffer.ControlStructure.ChipSelectChange, REQUESTED_CHIP_SELECT_CHANGE ? (byte)1 : (byte)0);
        }

        [Test]
        public void Should_the_control_structure_have_the_requested_delay() {
            Assert.AreEqual(buffer.ControlStructure.Delay, REQUESTED_DELAY_IN_USEC);
        }

        [Test]
        public void Should_the_control_structure_have_the_requested_speed() {
            Assert.AreEqual(buffer.ControlStructure.Speed, REQUESTED_SPEED_IN_HZ);
        }

        protected override void Cleanup() {
            if (!ReferenceEquals(buffer, null)) {
                buffer.Dispose();
                buffer = null;
            }
        }
    }
}