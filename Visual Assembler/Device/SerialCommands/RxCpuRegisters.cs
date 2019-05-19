using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visual_Assembler.Device.SerialCommands {
    class RxCpuRegisters : RxCommand {

        private int id;

        public byte accumulator;
        public byte programCounter;

        public RxCpuRegisters(byte[] bytes) {
            id = bytes[0];
            accumulator = bytes[1];
            programCounter = bytes[2];
        }

        public override byte Id {
            get { return (byte)id; }
        }

        public override string ToString() {
            return string.Format("Packet 'RxCpuRegisters' ACC={0}; PC={1}", accumulator, programCounter);
        }

    }
}
