using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visual_Assembler.Device
{
    public enum Mnemonics{
        nop     = 0b00000000,
        halt    = 0b11111111,
        jmp     = 0b00000001,
        jiz     = 0b00000010,
        jinz    = 0b00000011,
        jic     = 0b00000100,
        jinc    = 0b00000101,
        movla   = 0b00100100,
        movra   = 0b00100101,
        movar   = 0b00100110,
        not     = 0b10001000,
        set     = 0b10001001,
        clr     = 0b10001010,
        addl    = 0b10001011,
        subl    = 0b10001100,
        andl    = 0b10001111,
        iorl    = 0b10010000,
        xorl    = 0b10010001,
        addr    = 0b11101011,
        subr    = 0b11101100,
        andr    = 0b11101111,
        iorr    = 0b11110000,
        xorr    = 0b11110001
    }
}
