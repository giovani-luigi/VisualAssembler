using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Visual_Assembler.Device;
using Visual_Assembler.ViewModels.Commands;

namespace Visual_Assembler.ViewModels {
    class ProgrammerViewModel {

        private FpgaBoard device;
        private MemoryWord[] program;
        
        private bool isBusy = false;
               
        public ObservableCollection<string> Log { get; }
        private object logLock = new object();

        public ProgrammerViewModel(FpgaBoard device, MemoryWord[] program) {
            this.device = device;
            this.program = program;
            Log = new ObservableCollection<string>();
           
            // as we will access the log from other threads, we need to synchronize with the main thread
            BindingOperations.EnableCollectionSynchronization(Log, logLock);

            CmdCloseWindow = new RelayCommand<Window>((x) => OnCommand_CloseWindow(x), (x) => !isBusy);
            CmdProgram = new RelayCommand(() => OnCommand_Program(), ()=>!isBusy);
        }

        public RelayCommand<Window> CmdCloseWindow { get; }
        private void OnCommand_CloseWindow(Window wnd) {
            wnd?.Close();
        }
        
        public RelayCommand CmdProgram { get; }
        private void OnCommand_Program() {
            if (device.IsConnected) {
                isBusy = true;
                ClearLog();
                WriteLog("Iniciando programação da memória SRAM");
                WriteLog(string.Format("Tamanho total: {0} palavras", FpgaBoard.MEMORY_LENGTH));
                Task.Factory.StartNew(() => {
                    // write each memory address to the board
                    WriteLog("Espaço de programa:");
                    for (int address = 0; address < program.Length; address++) {
                        MemoryWord word = program[address];
                        device.WaitQuery(new Device.SerialCommands.TxWriteMemory((byte)address, word.MSB, word.LSB));
                        WriteLog(string.Format("Gravando {0} no endereço {1:X2}", word, address)); // report progress
                    }
                }).ContinueWith((t) => {
                    WriteLog("Finalizado.");
                    isBusy = false;
                });
            } 
        }

        private void WriteLog(string message) {
            Log.Add(string.Format("[{0}]: {1}", DateTime.Now.ToLongTimeString(), message));
        }

        private void ClearLog() {
            Log.Clear();
        }

    }
}
