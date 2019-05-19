using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Ports;
using System.Threading;
using Visual_Assembler.Device.SerialCommands;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Visual_Assembler.Device
{
    /// <summary>
    /// Provides instance methods, events and properties to commmunicate 
    /// with the FPGA hardware through a serial port RS-232.
    /// </summary>
    public class FpgaBoard {

        #region Events

        /// <summary>
        /// Raised every time a change in the SRAM or CPU registers is detected.
        /// </summary>
        public event EventHandler<RegisterChangedEventArgs> OnRegisterChanged;

        /// <summary>
        /// Raised when a timeout occurs while waiting for a response of 
        /// the hardware through the serial port
        /// </summary>
        public event EventHandler OnTimeout;

        #endregion

        #region Constants

        public const int BOARD_BAUDRATE = 115200; // defined on hardware
        public const int MEMORY_LENGTH = 256; // defined on hardware: 256 addressable registers
        public const int PACKET_LENGTH = 4; // received packet of data is 4 byte fixed length.
        public const int RESPONSE_TIMEOUT = 25; // timeout in 'ms' to wait for a reply after sending a command
        
        #endregion

        #region Private fields

        // serial port objects
        private SerialPort serialPort = null;
        private AutoResetEvent receiveSignal = new AutoResetEvent(false); // used so we can block until we get a response
        private Queue<byte> receivedData = new Queue<byte>();
        object txLock = new object();
        Stopwatch intervalTimer = new Stopwatch();

        // registers:
        private MemoryWord[] memoryModel = new MemoryWord[MEMORY_LENGTH]; // memory word size = 8 + 8 bits 
        private byte Accumulator;
        private byte ProgramCounter;               
        
        // other private fields
        private int timeoutCount = 0;

        #endregion

        #region Ctor

        public FpgaBoard() {
            // initialize device memory with empty values
            for (int i = 0; i < memoryModel.Length; i++) {
                memoryModel[i] = new MemoryWord();
            }
        }

        #endregion

        #region Public properties

        public bool IsConnected {
            get {
                if (serialPort != null) return serialPort.IsOpen;
                return false;
            }
        }

        public int TimeoutCount {
            get { return timeoutCount; }
        }

        #endregion

        #region Serial Port related methods

        /// <summary>
        /// Opens a serial port and connect. Returns TRUE if serial port is connected.
        /// </summary>
        /// <param name="portName">name of the port</param>
        public bool TryConnect(string portName) {

            if (serialPort != null) {
                if (serialPort.IsOpen) return true;
            } else {
                serialPort = new SerialPort();
                serialPort.DataReceived += OnSerialPortDataReceived;
            }

            try {
                serialPort.PortName = portName;
                serialPort.BaudRate = BOARD_BAUDRATE;
                serialPort.Parity = Parity.None;
                serialPort.DataBits = 8;
                serialPort.StopBits = StopBits.One;
                serialPort.Open();
                ResetErrors();
                return serialPort.IsOpen;
            } catch (Exception) { // catch all possible exceptions
                serialPort.Dispose();
                serialPort = null;
                return false;
            }
        }

        /// <summary>
        /// If serial port is connected, disconnect.
        /// </summary>
        public void Disconnect() {
            if (serialPort == null) return;
            if (serialPort.IsOpen) serialPort.Close();
            serialPort.Dispose();
            serialPort = null;
        }
        
        private void OnSerialPortDataReceived(object sender, SerialDataReceivedEventArgs e) {
            byte[] buffer = new byte[serialPort.BytesToRead];
            serialPort.Read(buffer, 0, buffer.Length); // we se buffer.length because of async. behavior of the port
            foreach (byte b in buffer) receivedData.Enqueue(b);
            processData();
        }
        
        /// <summary>
        /// resets error statistics/counters
        /// </summary>
        private void ResetErrors() {
            timeoutCount = 0;
        }
               
        /// <summary>
        /// Sends a TxQuery command, and block until we receive a response or timeout
        /// </summary>
        /// <param name="timeout">Timeout to wait for a response. If not specified, uses default.</param>
        public void WaitQuery(TxQuery command, int timeout = RESPONSE_TIMEOUT) {
            lock (txLock) {
                byte[] cmdBytes = command.Serialize();
                serialPort.Write(cmdBytes, 0, cmdBytes.Length);
                if (!receiveSignal.WaitOne(timeout)) {
                    timeoutCount++; // hardware reply timeout
                    OnTimeout(this, new EventArgs());
                }
            }
        }

        private void processData() {
            while (receivedData.Count >= PACKET_LENGTH) {
                byte[] packet = new byte[PACKET_LENGTH];
                for (int i = 0; i < packet.Length; i++) {
                    packet[i] = receivedData.Dequeue();
                }
                OnNewPacket(packet); // process the current packet
            }
        }

        private void OnNewPacket(byte[] packet) {
            // turn packet of bytes into a command:
            switch (packet[0]) { // the first byte will tell us the kind of command it is

                // command is a reply to a read_memory:
                case (byte)RxCommands.memory_value:
                    RxMemoryValue memVal = new RxMemoryValue(packet);
                    memoryModel[memVal.address] = new MemoryWord(memVal.msb, memVal.lsb);
                    NotifyRegisterChanges();
                    Debug.WriteLine(memVal.ToString());
                    break;

                // command is a reply to a read_cpu:
                case (byte)RxCommands.cpu_registers:
                    RxCpuRegisters cpuVal = new RxCpuRegisters(packet);
                    Accumulator = cpuVal.accumulator;
                    ProgramCounter = cpuVal.programCounter;
                    NotifyRegisterChanges();
                    Debug.WriteLine(cpuVal.ToString());
                    break;

                case (byte)RxCommands.ack:
                    Debug.WriteLine("ACK");
                    break;
            }

            // after processing the packet, we release the waiting threads
            receiveSignal.Set();
        }

        #endregion

        private void NotifyRegisterChanges() {
            OnRegisterChanged?.Invoke(this,
                        new RegisterChangedEventArgs(memoryModel, Accumulator, ProgramCounter));
        }

    }

}
