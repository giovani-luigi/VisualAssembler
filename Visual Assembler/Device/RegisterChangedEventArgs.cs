using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visual_Assembler.Device {
    public class RegisterChangedEventArgs : EventArgs {

        public IReadOnlyCollection<MemoryWord> Memory { get; }
        public byte Accumulator;
        public byte ProgramCounter;

        public RegisterChangedEventArgs(MemoryWord[] memory, byte accumulator, byte programCounter) {
            this.Memory = Array.AsReadOnly(memory);
            this.Accumulator = accumulator;
            this.ProgramCounter = programCounter;
        }

    }
}
