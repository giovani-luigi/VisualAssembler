using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Visual_Assembler.Device;

namespace Visual_Assembler.Models {
    class Assembler {

        public Assembler() {
        }

        public AssemblerResult Assemble(Instruction[] program) {
            MemoryWord[] data = new MemoryWord[program.Length];
            List<string> errors = new List<string>();
            List<string> warnings = new List<string>();
            for (int address = 0; address < program.Length ; address++) {
                Instruction currentInstruction = program[address];
                data[address] = new MemoryWord((byte)currentInstruction.OpCode, currentInstruction.Operand);
            }
            return new AssemblerResult(data, errors, warnings);
        }

    }
}
