using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Visual_Assembler.Device;
using Visual_Assembler.Models;
using Visual_Assembler.ViewModels.Commands;

namespace Visual_Assembler.ViewModels {

    class MainWindowViewModel : INotifyPropertyChanged {

        public event PropertyChangedEventHandler PropertyChanged;

        private FpgaBoard device = new FpgaBoard();
        private VisualCodeEditor visualCodeEditor = new VisualCodeEditor(256 - 3); // 8-bit addressing minus last 3 reserved addresses

        public MainWindowViewModel() {

            Instructions = visualCodeEditor.Code;

            // wire commands using the relay class:
            CmdConnect = new RelayCommand(() => OnCommand_Connect(), () => !String.IsNullOrEmpty(SelectedPort) && !device.IsConnected);
            CmdDisconnect = new RelayCommand(() => OnCommand_Disconnect(), () => device.IsConnected);
            CmdRunNext = new RelayCommand(() => OnCommand_RunNext(), () => device.IsConnected);
            CmdCompileAndProgram = new RelayCommand(() => OnCommand_CmdCompileAndProgram(), () => device.IsConnected);
            CmdViewDiagram = new RelayCommand(() => OnCommand_ViewDiagram());
            CmdResetCpu = new RelayCommand(() => OnCommand_ResetCpu(), () => device.IsConnected);
            CmdRefreshWatch = new RelayCommand(() => OnCommand_RefreshWatch(), () => device.IsConnected);
            CmdEraseCode = new RelayCommand(() => OnCommand_EraseCode(), () => true);

            // subscribe to device events
            device.OnRegisterChanged += Device_OnRegisterChanged;
            device.OnTimeout += Device_OnTimeout;

        }

        private int GetBaseValue() {
            switch (DisplayRadix) {
                case Radix.radix16:
                    return 16;
                case Radix.radix2:
                    return 2;
                default:
                    return 10;
            }
        }

        private string FormatToDisplay(byte value) {
            switch (DisplayRadix) {
                case Radix.radix16:
                    return string.Format("0x{0:x2}", value).ToUpper();
                case Radix.radix2:
                    return Convert.ToString(value, 2).PadLeft(8, '0').ToUpper();
                default:
                    return value.ToString().ToUpper();
            }
        }

        private void Device_OnRegisterChanged(object sender, RegisterChangedEventArgs e) {
            // update properties of this view model so the view will be updated

            // ACC and PC registers
            CpuAccumulator = FormatToDisplay(e.Accumulator);
            CpuProgramCounter = FormatToDisplay(e.ProgramCounter);
            SelectedIndex = e.ProgramCounter; // update the selected instruction on the listview

            // last 3 SRAM words
            MemoryValueAt0xFF = FormatToDisplay(e.Memory.ElementAt(0xFF).LSB);
            MemoryValueAt0xFE = FormatToDisplay(e.Memory.ElementAt(0xFE).LSB);
            MemoryValueAt0xFD = FormatToDisplay(e.Memory.ElementAt(0xFD).LSB);

        }

        private void Device_OnTimeout(object sender, EventArgs e) {
            // wpf binding will dispatch to the UI thread automatically
            Timeouts = ((FpgaBoard)sender).TimeoutCount.ToString();
        }

        #region Bindings

        private string status = "Status: -";
        public string Status {
            get { return status; }
            private set {
                status = string.Format("Status: {0}", value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Status"));
            }
        }
        private string timeouts = "Timeouts: 0";
        public string Timeouts {
            get { return timeouts; }
            set {
                timeouts = string.Format("Timeouts: {0}",value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Timeouts"));
            }
        }

        public List<string> AvailablePorts {
            get { return System.IO.Ports.SerialPort.GetPortNames().ToList(); }
        }

        public ObservableCollection<Instruction> Instructions { get; }

        private Radix displayRadix = Radix.radix10;
        public Radix DisplayRadix {
            get { return displayRadix; }
            set {
                displayRadix = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DisplayRadix"));
            }
        }

        private string selectedPort = "";
        public string SelectedPort {
            get { return selectedPort; }
            set {
                selectedPort = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedPort"));
            }
        }

        private string cpuAccumulator;
        public string CpuAccumulator {
            get { return cpuAccumulator; }
            private set {
                cpuAccumulator = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CpuAccumulator"));
            }
        }

        private string cpuProgramCounter;
        public string CpuProgramCounter {
            get { return cpuProgramCounter; }
            private set {
                cpuProgramCounter = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CpuProgramCounter"));
            }
        }

        private string memoryValueAt0xFF;
        public string MemoryValueAt0xFF {
            get { return memoryValueAt0xFF; }
            private set {
                memoryValueAt0xFF = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MemoryValueAt0xFF"));
            }
        }

        private string memoryValueAt0xFE;
        public string MemoryValueAt0xFE {
            get { return memoryValueAt0xFE; }
            private set {
                memoryValueAt0xFE = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MemoryValueAt0xFE"));
            }
        }

        private string memoryValueAt0xFD;
        public string MemoryValueAt0xFD {
            get { return memoryValueAt0xFD; }
            private set {
                memoryValueAt0xFD = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MemoryValueAt0xFD"));
            }
        }

        private int selectedIndex = -1; // starts with no selection
        public int SelectedIndex {
            get { return selectedIndex; }
            set {
                selectedIndex = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedIndex"));
            }
        }

        #endregion

        #region Commands

        public RelayCommand CmdConnect { get; }
        private void OnCommand_Connect() {
            device.TryConnect(SelectedPort);
            if (device.IsConnected) {
                Status = "Conectado.";
                UpdateWatchWindow();
            } else {
                Status = "Falha ao conectar.";
            }
        }

        public RelayCommand CmdDisconnect { get; }
        private void OnCommand_Disconnect() {
            device.Disconnect();
            Status = "Desconectado";
        }

        public RelayCommand CmdRunNext { get; }
        private void OnCommand_RunNext() {
            if (device.IsConnected) {
                device.WaitQuery(new Device.SerialCommands.TxRunNext());
                UpdateWatchWindow();
                Status = "Conectado; Instrução executada.";
            }
        }

        public RelayCommand CmdResetCpu { get; }
        private void OnCommand_ResetCpu() {
            if (device.IsConnected) {
                device.WaitQuery(new Device.SerialCommands.TxReset());
                UpdateWatchWindow();
                Status = "Conectado; CPU Resetada.";
            }
        }

        public RelayCommand CmdViewDiagram { get; }
        private void OnCommand_ViewDiagram() {
            Views.DiagramView window = new Views.DiagramView();
            window.ShowDialog();
        }

        public RelayCommand CmdCompileAndProgram { get; }
        private void OnCommand_CmdCompileAndProgram() {
            if (device.IsConnected) {

                Status = "Conectado; Programa carregado para a memória SRAM.";

                // assemble the program, i.e. turn mnemonic and operands into byte code to store as memory words
                MemoryWord[] assembledProgram = new Assembler().Assemble(visualCodeEditor.GetCodeInstructions()).data;

                // show GUI for programming if code result is not null
                if (assembledProgram != null) {                    
                    Views.ProgrammerView progView = new Views.ProgrammerView(device, assembledProgram);
                    progView.ShowDialog();
                }                
            }
        }

        public RelayCommand CmdRefreshWatch { get; }
        private void OnCommand_RefreshWatch() {
            UpdateWatchWindow();
        }

        public RelayCommand CmdEraseCode { get; }
        private void OnCommand_EraseCode() {
            visualCodeEditor.GenerateEmptyCode();
        }

        #endregion

        private void UpdateWatchWindow() {
            if (device.IsConnected) {
                // we will read from the FPGA the registers being displayed in the watch window
                device.WaitQuery(new Device.SerialCommands.TxReadCpuRegisters());
                device.WaitQuery(new Device.SerialCommands.TxReadMemory(0xFF));
                device.WaitQuery(new Device.SerialCommands.TxReadMemory(0xFE));
                device.WaitQuery(new Device.SerialCommands.TxReadMemory(0xFD));
            }
        }

    }
}
