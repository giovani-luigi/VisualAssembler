using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visual_Assembler.Device.SerialCommands
{

    public enum TxCommands : byte {
        unknown = 0,
        run_next = 1,
        write_memory = 2,
        read_memory = 3,
        read_cpu = 4,
        reset_cpu = 5
    }

    public abstract class TxQuery
    {

        public abstract byte Id { get; }

        public abstract byte[] Serialize();

    }
}
