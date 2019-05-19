using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visual_Assembler.Device.SerialCommands {
    class TxReadCpuRegisters : TxQuery {

        public TxReadCpuRegisters() {
        }

        public override byte Id {
            get { return (byte)TxCommands.read_cpu; }
        }

        public override byte[] Serialize() {
            return new byte[] { Id };
        }
    }
}
