using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visual_Assembler.Device.SerialCommands
{
    class TxReadMemory : TxQuery {

        private byte address;

        public TxReadMemory(byte address) {
            this.address = address;
        }

        public override byte Id {
            get { return (byte)TxCommands.read_memory; }
        }

        public override byte[] Serialize() {
            return new byte[] { Id, address };
        }
    }
}
