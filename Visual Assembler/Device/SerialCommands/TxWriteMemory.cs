using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Visual_Assembler.Models;

namespace Visual_Assembler.Device.SerialCommands
{
    class TxWriteMemory : TxQuery {

        private byte address;
        private byte msb;
        private byte lsb;

        public TxWriteMemory(byte address, byte msb, byte lsb) {
            this.address = address;
            this.msb = msb;
            this.lsb = lsb;
        }

        public override byte Id {
            get { return (byte)TxCommands.write_memory; }
        }

        public override byte[] Serialize() {
            return new byte[] { Id, address, msb, lsb };
        }
    }
}
