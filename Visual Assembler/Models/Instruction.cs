using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Visual_Assembler.Device;

namespace Visual_Assembler.Models
{
    class Instruction{

        public int Address { get; }

        public Mnemonics OpCode { get; set; }

        public byte Operand { get; set; }

        public IEnumerable<Mnemonics> AvailableMnemonics {
            get { return  Enum.GetValues(typeof(Mnemonics)).Cast<Mnemonics>(); }
        }

        public Instruction(byte address, Mnemonics opcode, byte operand){
            this.Address = address;
            this.OpCode = opcode;
            this.Operand = operand;
        }

    }
}
