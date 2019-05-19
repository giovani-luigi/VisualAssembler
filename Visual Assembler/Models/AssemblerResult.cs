using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Visual_Assembler.Device;

namespace Visual_Assembler.Models {
    class AssemblerResult {

        public AssemblerResult(MemoryWord[] data, IEnumerable<string> errors, IEnumerable<string> warnings) {
            this.data = data;
            this.errors = errors;
            this.warnings = warnings;
        }

        public IEnumerable<string> errors { get; }
        public IEnumerable<string> warnings { get; }
        public MemoryWord[] data { get; }

    }
}
