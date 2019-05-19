using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visual_Assembler.Device
{
    public class MemoryWord
    {
        public byte MSB { get; }
        public byte LSB { get; }

        public MemoryWord() { } 

        public MemoryWord(byte msb, byte lsb) {
            this.MSB = msb;
            this.LSB = lsb;
        }

        public override string ToString() {
            return string.Format("0x{0:X2}{1:X2}", MSB, LSB);
        }
    }
}
