using System;
using System.Collections.Generic;

namespace pissboy23 {

    public class CPU : Component {

        public static Clock clock;
        public static Registers registers;

        public Func<int>[] opcodes8 = new Func<int>[] { // returns machine cycles taken
            () => { // 0x00 NOP
                registers.pc++;
                return 1;
            },
            () => { // 0x01 LD BC, d16
                registers.b = GameBoy.memory.ReadByte(registers.pc);
                registers.pc++;
                registers.c = GameBoy.memory.ReadByte(registers.pc);
                registers.pc++;
                return 3;
            },
            () => { // 0x02 LD (BC), A
                ushort address = (ushort) ((registers.c << 8) + registers.b);
                GameBoy.memory.WriteByte(address, registers.a);
                return 2;
            },
            () => { // 0x03 INC BC
                registers.b++;
                registers.c++;
                return 2;
            },
            () => { // 0x04 INC B
                registers.b++;
                return 1;
            },
            () => { // 0x05 DEC B
                registers.b--;
                return 1;
            },
            () => { // 0x06 LD B, d8
                registers.b = GameBoy.memory.ReadByte(registers.pc);
                registers.pc++;
                return 2;
            },
            () => { // 0x07 RLCA
                registers.a = (byte) ((registers.a >> 7) | ((registers.a & 0x7f) << 1));
                return 1;
            }
        };
        
        public void reset(){}
    }

    public struct Clock {
        public int m, t;
    }

    public struct Registers {
        public byte a, b, c, d, e, h, l, f;
        public ushort pc, sp;
        public int m, t;
    }
}
