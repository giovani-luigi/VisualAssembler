using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visual_Assembler.Device.SerialCommands
{
    class RxMemoryValue : RxCommand{

        private int id;

        public byte address;
        public byte msb; // bits 15-8
        public byte lsb; // bits  7-0

        public RxMemoryValue(byte[] bytes) {
            id = bytes[0];
            address = bytes[1];
            msb = bytes[2];
            lsb = bytes[3];
        }

        public override byte Id {
            get { return (byte)id; }
        }

        public override string ToString() {
            return string.Format("Packet 'RxMemoryValue' ID={0}; Address={1}; Data(MSB)={2}; Data(LSB)={3}",
                id, address, msb, lsb);
        }

    }
}
