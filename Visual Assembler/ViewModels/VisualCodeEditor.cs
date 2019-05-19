using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Visual_Assembler.Device;
using Visual_Assembler.Models;

namespace Visual_Assembler.ViewModels
{
    class VisualCodeEditor{

        private int maxLineCount;
        public ObservableCollection<Instruction> Code { get; } = new ObservableCollection<Instruction>();

        public Instruction[] GetCodeInstructions() {
            return Code.ToArray();
        }

        public VisualCodeEditor(int maxLineCount) {
            this.maxLineCount = maxLineCount;
            GenerateEmptyCode();
        }

        public void GenerateEmptyCode() {
            Code.Clear();
            for (int i = 0; i < maxLineCount; i++)
                Code.Add(new Instruction( (byte)i, Mnemonics.nop, 0));
        }

    }
}
