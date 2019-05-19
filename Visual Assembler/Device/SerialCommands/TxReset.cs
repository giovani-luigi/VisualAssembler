using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visual_Assembler.Device.SerialCommands
{
    class TxReset : TxQuery {

        public TxReset() {
        }

        public override byte Id {
            get { return (byte)TxCommands.reset_cpu; }
        }

        public override byte[] Serialize() {
            return new byte[] { Id };
        }
    }
}
