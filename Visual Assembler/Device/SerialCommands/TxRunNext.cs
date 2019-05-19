using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visual_Assembler.Device.SerialCommands
{
    class TxRunNext : TxQuery {

        public TxRunNext() {
        }

        public override byte Id {
            get { return (byte)TxCommands.run_next; }
        }

        public override byte[] Serialize() {
            return new byte[] { Id };
        }
    }
}
