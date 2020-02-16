using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Raspberry.IO.SerialPeripheralInterface.NetStandard;
using Raspberry.IO.SerialPeripheralInterface.NetStandard.EnumTypes;
using Raspberry.IO.SerialPeripheralInterface.NetStandard.Interop;
using Tests.Raspberry.IO.NetStandard;

// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Tests.Raspberry.IO.SerialPeripheralInterface.NativeSpiConnectionSpecs
{
    [TestFixture]
    public class If_the_user_creates_a_native_SPI_connection_instance_providing_appropriate_connection_settings : Spec {
        private const int BITS_PER_WORD = 8;
        private const int DELAY = 500;
        private const SpiMode SPI_MODE = SpiMode.Mode2;
        private const int SPEED_IN_HZ = 500000;
        
        private SpiConnectionSettings settings;
        private NativeSpiConnection connection;
        private Mock<ISpiControlDevice> controlDeviceMock;
        private ISpiControlDevice controlDevice;

        private uint controlRequest;
        private uint controlData;

        protected override void EstablishContext() {
            settings = new SpiConnectionSettings {
                BitsPerWord = BITS_PER_WORD,
                Delay = DELAY,
                Mode = SPI_MODE,
                MaxSpeed = SPEED_IN_HZ
            };

            controlDeviceMock = new Mock<ISpiControlDevice>();
            controlDeviceMock.Setup(x => x.Control(It.IsAny<uint>(), ref It.Ref<uint>.IsAny)).Callback((uint a, uint b) => { controlRequest = a; controlData = b;});
            controlDevice = controlDeviceMock.Object;
        }

        protected override void BecauseOf() {
            connection = new NativeSpiConnection(controlDevice, settings);
        }

        [Test]
        public void Should_it_write_the_max_speed_in_Hz_to_the_control_device() {
            UInt32 speed = SPEED_IN_HZ;

            controlDeviceMock.Verify(x => x.Control(It.Is<uint>(y => y == NativeSpiConnection.SPI_IOC_WR_MAX_SPEED_HZ), ref It.Ref<uint>.IsAny), Times.Once);
            controlDeviceMock.Verify(x => x.Control(It.Is<uint>(y => y == NativeSpiConnection.SPI_IOC_RD_MAX_SPEED_HZ), ref It.Ref<uint>.IsAny), Times.Once);
        }

        [Test]
        public void Should_it_write_the_bits_per_word_to_the_control_device()
        {
            byte bitsPerWord = BITS_PER_WORD;

            controlDeviceMock.Verify(x => x.Control(It.Is<uint>(y => y == NativeSpiConnection.SPI_IOC_WR_BITS_PER_WORD), ref It.Ref<uint>.IsAny), Times.Once);
            controlDeviceMock.Verify(x => x.Control(It.Is<uint>(y => y == NativeSpiConnection.SPI_IOC_RD_BITS_PER_WORD), ref It.Ref<uint>.IsAny), Times.Once);
        }

        [Test]
        public void Should_it_write_the_spi_mode_to_the_control_device() {
            var spiMode = (UInt32)SPI_MODE;

            controlDeviceMock.Verify(x => x.Control(It.Is<uint>(y => y == NativeSpiConnection.SPI_IOC_WR_MODE), ref It.Ref<uint>.IsAny), Times.Once);
            controlDeviceMock.Verify(x => x.Control(It.Is<uint>(y => y == NativeSpiConnection.SPI_IOC_RD_MODE), ref It.Ref<uint>.IsAny), Times.Once);
        }

        [Test]
        public void Should_it_set_the_delay() {
            Assert.AreEqual(connection.Delay, DELAY);
        }

        [Test]
        public void Should_it_set_the_speed() {
            Assert.AreEqual(connection.MaxSpeed, SPEED_IN_HZ);
        }

        [Test]
        public void Should_it_set_the_SPI_mode() {
            Assert.AreEqual(connection.Mode, SPI_MODE);
        }

        [Test]
        public void Should_it_set_the_bits_per_word() {
            Assert.AreEqual(connection.BitsPerWord, BITS_PER_WORD);
        }
    }

    [TestFixture]
    public class If_the_user_requests_a_spi_transfer_buffer : Spec {
        private const int BITS_PER_WORD = 16;
        private const int DELAY = 500;
        private const SpiMode SPI_MODE = SpiMode.Mode2;
        private const int SPEED_IN_HZ = 500000;
        private const int REQUESTED_SIZE = 100;

        private SpiConnectionSettings settings;
        private ISpiControlDevice controlDevice;
        private NativeSpiConnection connection;
        private ISpiTransferBuffer buffer;

        protected override void EstablishContext() {
            settings = new SpiConnectionSettings {
                BitsPerWord = BITS_PER_WORD, 
                Delay = DELAY, 
                Mode = SPI_MODE, 
                MaxSpeed = SPEED_IN_HZ
            };

            controlDevice = new Mock<ISpiControlDevice>().Object;
            connection = new NativeSpiConnection(controlDevice, settings);
        }

        protected override void BecauseOf() {
            buffer = connection.CreateTransferBuffer(REQUESTED_SIZE, SpiTransferMode.ReadWrite);
        }

        [Test]
        public void Should_the_buffer_be_initialized_with_the_connections_wordsize() {
            Assert.AreEqual(buffer.ControlStructure.BitsPerWord, BITS_PER_WORD);
        }
        
        [Test]
        public void Should_the_buffer_be_initialized_with_the_connections_delay() {
            Assert.AreEqual(buffer.ControlStructure.Delay, DELAY);
        }

        [Test]
        public void Should_the_buffer_be_initialized_with_the_connections_speed() {
            Assert.AreEqual(buffer.ControlStructure.Speed, SPEED_IN_HZ);
        }

        protected override void Cleanup() {
            if (!ReferenceEquals(buffer, null)) {
                buffer.Dispose();
                buffer = null;
            }
        }
    }

    [TestFixture]
    public class If_the_user_starts_a_single_data_transfer : Spec {
        private const int BITS_PER_WORD = 16;
        private const int DELAY = 500;
        private const int SPEED_IN_HZ = 500000;
        private const int IOCTL_PINVOKE_RESULT_CODE = 1;
        private const int SPI_IOC_MESSAGE_1 = 0x40206b00;

        private Mock<ISpiControlDevice> controlDeviceMock;
        private ISpiControlDevice controlDevice;
        private NativeSpiConnection connection;
        private Mock<ISpiTransferBuffer> bufferMock;
        private ISpiTransferBuffer buffer;
        private int result;
        private SpiTransferControlStructure controlStructure;

        protected override void EstablishContext() {
            // SPI control structure we expect to see during the P/Invoke call
            controlStructure = new SpiTransferControlStructure
            {
                BitsPerWord = BITS_PER_WORD,
                Length = 5,
                Delay = DELAY,
                ChipSelectChange = 1,
                Speed = SPEED_IN_HZ
            };

            controlDeviceMock = new Mock<ISpiControlDevice>();
            controlDevice = controlDeviceMock.Object;
            controlDeviceMock.Setup(x => x.Control(It.IsAny<uint>(), ref It.Ref<SpiTransferControlStructure>.IsAny)).Returns(IOCTL_PINVOKE_RESULT_CODE);

            connection = new NativeSpiConnection( controlDevice );

            bufferMock = new Mock<ISpiTransferBuffer>();
            buffer = bufferMock.Object;
            bufferMock.Setup(x => buffer.ControlStructure).Returns(controlStructure);
        }

        protected override void BecauseOf() {
            result = connection.Transfer(buffer);
        }

        [Test]
        public void Should_the_buffers_control_structure_be_sent_to_the_IOCTL_device() {
            controlDeviceMock.Verify(x => x.Control(It.Is<uint>(y => y == SPI_IOC_MESSAGE_1), ref It.Ref<SpiTransferControlStructure>.IsAny), Times.Once);
        }

        [Test]
        public void Should_it_return_the_pinvoke_result_code() {
            Assert.AreEqual(result, IOCTL_PINVOKE_RESULT_CODE);
        }
    }

    [TestFixture]
    public class If_the_user_starts_a_multi_data_transfer : Spec
    {
        private const int BITS_PER_WORD = 16;
        private const int DELAY = 500;
        private const int SPEED_IN_HZ = 500000;
        private const int IOCTL_PINVOKE_RESULT_CODE = 1;
        private const int SPI_IOC_MESSAGE_1 = 0x40206b00;

        private Mock<ISpiControlDevice> controlDeviceMock;
        private ISpiControlDevice controlDevice;
        private NativeSpiConnection connection;
        private Mock<ISpiTransferBufferCollection> collectionMock;
        private ISpiTransferBufferCollection collection;
        private Mock<ISpiTransferBuffer> bufferMock;
        private ISpiTransferBuffer buffer;
        private int result;
        private SpiTransferControlStructure controlStructure;

        protected override void EstablishContext() {
            controlDeviceMock = new Mock<ISpiControlDevice>();
            controlDevice = controlDeviceMock.Object;
            controlDeviceMock.Setup(x => x.Control(It.IsAny<uint>(), ref It.Ref<SpiTransferControlStructure>.IsAny)).Returns(IOCTL_PINVOKE_RESULT_CODE);

            connection = new NativeSpiConnection( controlDevice );

            // SPI control structure we expect to see during the P/Invoke call
            controlStructure = new SpiTransferControlStructure
            {
                BitsPerWord = BITS_PER_WORD,
                Length = 5,
                Delay = DELAY,
                ChipSelectChange = 1,
                Speed = SPEED_IN_HZ
            };

            bufferMock = new Mock<ISpiTransferBuffer>();
            buffer = bufferMock.Object;
            bufferMock.Setup(x => x.ControlStructure).Returns(controlStructure);

            // setup fake collection to return our "prepared" fake buffer
            collectionMock = new Mock<ISpiTransferBufferCollection>();
            collection = collectionMock.Object;
            collectionMock.Setup(x => x.Length).Returns(1);
            collectionMock.Setup(x => x.GetEnumerator()).Returns(new List<ISpiTransferBuffer> {buffer}.GetEnumerator());
        }

        protected override void BecauseOf() {
            result = connection.Transfer(collection);
        }

        [Test]
        public void Should_the_buffers_control_structure_be_sent_to_the_IOCTL_device() {
            controlDeviceMock.Verify(x => x.Control(It.Is<uint>(y => y == SPI_IOC_MESSAGE_1), ref It.Ref<SpiTransferControlStructure>.IsAny), Times.Once);
        }

        private bool Predicate(IEnumerable<SpiTransferControlStructure> control_structures) {
            return control_structures.Contains(controlStructure);
        }

        [Test]
        public void Should_it_return_the_pinvoke_result_code() {
            Assert.AreEqual(result, IOCTL_PINVOKE_RESULT_CODE);
        }
    }

}