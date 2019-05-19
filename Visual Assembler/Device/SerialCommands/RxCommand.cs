using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visual_Assembler.Device.SerialCommands
{

    public enum RxCommands : byte {
        unknown = 0,
        memory_value = 3,
        cpu_registers = 4,
        ack = 255
    }

    abstract class RxCommand {

        public abstract byte Id { get; }

    }

}
